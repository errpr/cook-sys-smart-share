﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Server;

namespace Server.Migrations
{
    [DbContext(typeof(SmartShareContext))]
    partial class SmartShareContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Server.Models.UploadedFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("BlobOid")
                        .HasColumnName("blob_manager_oid");

                    b.Property<DateTime>("Created")
                        .HasColumnName("time_created");

                    b.Property<int>("DownloadCount")
                        .HasColumnName("total_downloads");

                    b.Property<DateTime>("Expires")
                        .HasColumnName("time_expires");

                    b.Property<int?>("MaxDownloads")
                        .HasColumnName("max_downloads");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnName("password");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("files");
                });
#pragma warning restore 612, 618
        }
    }
}
