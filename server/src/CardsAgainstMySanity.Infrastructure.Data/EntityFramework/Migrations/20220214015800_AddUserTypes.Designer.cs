﻿// <auto-generated />
using System;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220214015800_AddUserTypes")]
    partial class AddUserTypes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CardsAgainstMySanity.Domain.Auth.Tokens.RefreshToken", b =>
                {
                    b.Property<Guid>("Token")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("token");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Token");

                    b.HasIndex("UserId");

                    b.ToTable("refresh_token", (string)null);
                });

            modelBuilder.Entity("CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models.UserDbModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AccessToken")
                        .HasColumnType("character varying")
                        .HasColumnName("access_token");

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("avatar_url");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("ip_address");

                    b.Property<DateTime>("LastPong")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_pong");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password_hash");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<int>("UserTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.HasIndex("UserTypeId");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models.UserTypeDbModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("UserTypeDbModel");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "account"
                        },
                        new
                        {
                            Id = 2,
                            Name = "guest"
                        },
                        new
                        {
                            Id = 3,
                            Name = "admin"
                        });
                });

            modelBuilder.Entity("CardsAgainstMySanity.Domain.Auth.Tokens.RefreshToken", b =>
                {
                    b.HasOne("CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models.UserDbModel", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models.UserDbModel", b =>
                {
                    b.HasOne("CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models.UserTypeDbModel", "UserType")
                        .WithMany("Users")
                        .HasForeignKey("UserTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserType");
                });

            modelBuilder.Entity("CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models.UserDbModel", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models.UserTypeDbModel", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
