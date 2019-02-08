using Microsoft.EntityFrameworkCore;
using Npgsql;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Core.Dto;
using static Core.Constants;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = IPAddress.Parse("127.0.0.1");
            var port = 3000;
            var listener = new TcpListener(address, port);
            listener.Start();

            Console.WriteLine("Awaiting connection...");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Received connection.");
                Task.Run(() => ClientHandler(client));
            }
        }

        static void ClientHandler(TcpClient client)
        {
            var requestInfoDeserializer = new XmlSerializer(typeof(FileRequestInfo));

            using (var stream = client.GetStream())
            {
                byte[] readBuffer = new byte[1024];
                var bytes = new List<byte>();
                do
                {
                    stream.Read(readBuffer, 0, readBuffer.Length);
                    bytes.AddRange(readBuffer);
                }
                while (stream.DataAvailable);
                var requestInfo = (FileRequestInfo)requestInfoDeserializer.Deserialize(new MemoryStream(bytes.ToArray()));

                switch (requestInfo.RequestType)
                {
                    case RequestType.Download:
                        HandleDownload(stream, requestInfo);
                        break;
                    case RequestType.Upload:
                        HandleUpload(stream, requestInfo);
                        break;
                    case RequestType.View:
                        HandleView(stream, requestInfo);
                        break;
                }
            }
        }

        private static void HandleView(NetworkStream stream, FileRequestInfo requestInfo)
        {
            var context = new SmartShareContext();
            var file = context.UploadedFiles.First(x => x.Name == requestInfo.FileName);
            if (!(file.Password == requestInfo.Password))
            {
                return;
            }

            if (file.DownloadsExceeded() || file.IsExpired())
            {
                stream.Close();
                file.Delete(context);
                return;
            }

            var view = file.CreateView();
            var responseSerializer = new XmlSerializer(typeof(FileView));
            responseSerializer.Serialize(stream, view);
        }

        static void HandleUpload(NetworkStream stream, FileRequestInfo requestInfo)
        {
            var context = new SmartShareContext();
            var connection = context.Database.GetDbConnection();

            // check if file already exists, and if that file is expired we can delete it and continue the upload.
            try
            {
                var possibleFile = context.UploadedFiles.First(x => x.Name == requestInfo.FileName);
                if (possibleFile != null)
                {
                    if (possibleFile.DownloadsExceeded() || possibleFile.IsExpired())
                    {
                        possibleFile.Delete(context);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("File table is empty");
            }

            stream.Write(new byte[1] { ACK_BYTE }); // let client know we're ready to receive

            if (connection is NpgsqlConnection)
            {
                var blobManager = new NpgsqlLargeObjectManager((NpgsqlConnection)connection);
                uint oid;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    oid = blobManager.Create();
                    using (var blobStream = blobManager.OpenReadWrite(oid))
                    {
                        stream.CopyTo(blobStream);
                    }

                    transaction.Commit();
                }

                stream.Close();
                connection.Close();

                Console.WriteLine("Upload successful.");

                var uploadedFile = new UploadedFile(requestInfo, oid);

                context.Add(uploadedFile);
                context.SaveChanges();
            }
            else
            {
                throw new NotSupportedException("Unsupported database adapter");
            }
        }

        static void HandleDownload(NetworkStream stream, FileRequestInfo requestInfo)
        {
            var context = new SmartShareContext();
            var file = context.UploadedFiles.First(x => x.Name == requestInfo.FileName);
            if (file == null)
            {
                stream.Write(new byte[] { FAIL_BYTE });
                return;
            }

            if (!(file.Password == requestInfo.Password))
            {
                stream.Write(new byte[] { FAIL_BYTE });
                return;
            }

            if (file.DownloadsExceeded() || file.IsExpired())
            {
                stream.Write(new byte[] { FAIL_BYTE });
                stream.Close();
                file.Delete(context);
                return;
            }

            stream.Write(new byte[] { ACK_BYTE });
            file.DownloadCount++;
            context.UploadedFiles.Update(file);
            context.SaveChanges();

            var connection = context.Database.GetDbConnection();
            if (connection is NpgsqlConnection)
            {
                var conn = (NpgsqlConnection)connection;
                conn.Open();
                var blobManager = new NpgsqlLargeObjectManager(conn);
                using (var transaction = connection.BeginTransaction())
                {
                    var blobStream = blobManager.OpenRead(file.BlobOid);
                    blobStream.CopyTo(stream);
                    transaction.Commit();
                }
                conn.Close();
                Console.WriteLine("Download successful.");
            }
            else
            {
                throw new NotSupportedException("Unsupported database adapter.");
            }
        }
    }
}