using Core.Dto;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Xml.Serialization;
using static Core.Constants;

namespace Client
{
    public static class Api
    {
        private const string HOST = "localhost";
        private const int PORT = 3000;

        public static FileView View(string fileName, string password)
        {
            var client = new TcpClient(HOST, PORT);
            using (var stream = client.GetStream())
            {
                var requestSerializer = new XmlSerializer(typeof(FileRequestInfo));
                var fileViewInfo = new FileRequestInfo()
                {
                    RequestType = RequestType.View,
                    FileName = fileName,
                    Password = password,
                };
                requestSerializer.Serialize(stream, fileViewInfo);
                var responseSerializer = new XmlSerializer(typeof(FileView));
                return (FileView)responseSerializer.Deserialize(stream);
            }
        }
        
        public static bool Download(string fileName, string password)
        {
            try
            {
                var client = new TcpClient(HOST, PORT);

                using (var stream = client.GetStream())
                {
                    var requestSerializer = new XmlSerializer(typeof(FileRequestInfo));
                    var fileUploadInfo = new FileRequestInfo()
                    {
                        RequestType = RequestType.Download,
                        FileName = fileName,
                        Password = password,
                    };

                    // Serialize request to server
                    requestSerializer.Serialize(stream, fileUploadInfo);

                    var responseByte = new byte[1];
                    stream.Read(responseByte);
                    if (responseByte[0] == FAIL_BYTE)
                    {
                        return false;
                    }

                    using (FileStream decompressedFileStream = File.Create(fileName))
                    {
                        using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public static bool Upload(FileInfo fileInfo, int? maxDownloads, int duration, string password)
        {
            try
            {
                var client = new TcpClient(HOST, PORT);

                using (var stream = client.GetStream())
                {
                    var requestSerializer = new XmlSerializer(typeof(FileRequestInfo));
                    var fileUploadInfo = new FileRequestInfo()
                    {
                        RequestType = RequestType.Upload,
                        FileName = fileInfo.Name,
                        DurationMinutes = duration,
                        MaxDownloadCount = maxDownloads,
                        Password = password,
                    };
                    
                    requestSerializer.Serialize(stream, fileUploadInfo);

                    // Wait for server to acknowledge with a byte, then start streaming file. 
                    var response = new byte[1];
                    stream.Read(response);
                    if (response[0] != ACK_BYTE)
                    {
                        return false;
                    }
                    try
                    {
                        using (var fileStream = File.Open(fileInfo.FullName, FileMode.Open))
                        {
                            using (GZipStream compressionStream = new GZipStream(stream,
                           CompressionMode.Compress))
                            {
                                fileStream.CopyTo(compressionStream);

                            }
                        }
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}