﻿// <auto-generated />
using System;
using AdvertisementAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdvertisementAPI.Migrations
{
    [DbContext(typeof(AdContext))]
    [Migration("20240304143058_Authentication")]
    partial class Authentication
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AdvertisementAPI.ADInfo", b =>
                {
                    b.Property<int?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("ID"));

                    b.Property<string>("LINK")
                        .HasColumnType("text");

                    b.Property<int?>("SITEID")
                        .HasColumnType("integer");

                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Advertisements");
                });

            modelBuilder.Entity("AdvertisementAPI.AdStatistic", b =>
                {
                    b.Property<int?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("ID"));

                    b.Property<int>("CLICKS")
                        .HasColumnType("integer");

                    b.Property<int?>("STATID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("STATID"));

                    b.Property<DateTime>("VALIDDATE")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("VIEWS")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("ADStatistics");
                });
#pragma warning restore 612, 618
        }
    }
}
