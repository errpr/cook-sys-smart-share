using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Core.Dto;
using Npgsql;
using static Core.Constants;

namespace Server.Models
{
    [Table("files")]
    public class UploadedFile
    {
        public UploadedFile() { }

        public UploadedFile(FileRequestInfo requestInfo, uint oid)
        {
            BlobOid = oid;
            Name = requestInfo.FileName;
            CreatedTime = DateTime.Now;
            Password = requestInfo.Password;
            MaxDownloads = requestInfo.MaxDownloadCount;

            if (requestInfo.DurationMinutes < DURATION_MINIMUM || requestInfo.DurationMinutes > DURATION_MAXIMUM)
            {
                ExpireTime = DateTime.Now.AddMinutes(DURATION_DEFAULT);
            }
            else
            {
                ExpireTime = DateTime.Now.AddMinutes(requestInfo.DurationMinutes);
            }
        }

        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("blob_manager_oid")]
        public uint BlobOid { get; set; }
        
        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("time_created")]
        public DateTime CreatedTime { get; set; }
        
        [Column("time_expires")]
        public DateTime ExpireTime { get; set; }
        
        [Column("max_downloads")]
        public int? MaxDownloads { get; set; }

        [Column("total_downloads")]
        public int DownloadCount { get; set; } = 0;

        [Required]
        [Column("password")]
        public string Password { get; set; }

        public FileView CreateView()
        {
            int? remainingDownloads = null;
            if (MaxDownloads != null)
            {
                remainingDownloads = MaxDownloads - DownloadCount;
            }

            return new FileView()
            {
                FileName = Name,
                RemainingDownloads = remainingDownloads,
                RemainingMinutes = (int)(ExpireTime - DateTime.Now).TotalMinutes,
            };
        }

        public bool IsExpired()
        {
            return ExpireTime < DateTime.Now;
        }

        /// <summary>
        /// Deletes blob from DB and removes record from files table
        /// </summary>
        /// <param name="context"></param>
        public void Delete(DbContext context)
        {
            var connection = context.Database.GetDbConnection();
            if (connection is NpgsqlConnection)
            {
                var conn = (NpgsqlConnection)connection;
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var blobManager = new NpgsqlLargeObjectManager(conn);
                    blobManager.Unlink(BlobOid);
                    transaction.Commit();
                }
                conn.Close();
                context.Remove(this);
                context.SaveChanges();
            }
            else
            {
                throw new NotSupportedException("Unsupported database adapter.");
            }
        }

        public bool DownloadsExceeded()
        {
            return (MaxDownloads != null && DownloadCount >= MaxDownloads);
        }
    }

}
