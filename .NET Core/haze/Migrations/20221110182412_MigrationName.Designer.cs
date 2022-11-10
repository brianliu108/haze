﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using haze.DataAccess;

#nullable disable

namespace haze.Migrations
{
    [DbContext(typeof(HazeContext))]
    [Migration("20221110182412_MigrationName")]
    partial class MigrationName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("haze.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("haze.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("EndDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("haze.Models.FavouriteCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("FavouriteCategories");
                });

            modelBuilder.Entity("haze.Models.FavouritePlatform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("PlatformId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlatformId");

                    b.HasIndex("UserId");

                    b.ToTable("FavouritePlatforms");
                });

            modelBuilder.Entity("haze.Models.PaymentInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BillingAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreditCardNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExpiryDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShippingAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PaymentInfo");
                });

            modelBuilder.Entity("haze.Models.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Platforms");
                });

            modelBuilder.Entity("haze.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EventId")
                        .HasColumnType("int");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("haze.Models.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("сategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("сategoryId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("haze.Models.ProductPlatform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("platformId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("platformId");

                    b.ToTable("ProductPlatforms");
                });

            modelBuilder.Entity("haze.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Newsletter")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Verified")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("haze.Models.Category", b =>
                {
                    b.HasOne("haze.Models.Product", null)
                        .WithMany("Categories")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("haze.Models.FavouriteCategory", b =>
                {
                    b.HasOne("haze.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("haze.Models.User", null)
                        .WithMany("FavouriteCategories")
                        .HasForeignKey("UserId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("haze.Models.FavouritePlatform", b =>
                {
                    b.HasOne("haze.Models.Platform", "Platform")
                        .WithMany()
                        .HasForeignKey("PlatformId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("haze.Models.User", null)
                        .WithMany("FavouritePlatforms")
                        .HasForeignKey("UserId");

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("haze.Models.PaymentInfo", b =>
                {
                    b.HasOne("haze.Models.User", null)
                        .WithMany("PaymentInfos")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("haze.Models.Platform", b =>
                {
                    b.HasOne("haze.Models.Product", null)
                        .WithMany("Platforms")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("haze.Models.Product", b =>
                {
                    b.HasOne("haze.Models.Event", null)
                        .WithMany("Products")
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("haze.Models.ProductCategory", b =>
                {
                    b.HasOne("haze.Models.Category", "сategory")
                        .WithMany()
                        .HasForeignKey("сategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("сategory");
                });

            modelBuilder.Entity("haze.Models.ProductPlatform", b =>
                {
                    b.HasOne("haze.Models.Platform", "platform")
                        .WithMany()
                        .HasForeignKey("platformId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("platform");
                });

            modelBuilder.Entity("haze.Models.Event", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("haze.Models.Product", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Platforms");
                });

            modelBuilder.Entity("haze.Models.User", b =>
                {
                    b.Navigation("FavouriteCategories");

                    b.Navigation("FavouritePlatforms");

                    b.Navigation("PaymentInfos");
                });
#pragma warning restore 612, 618
        }
    }
}