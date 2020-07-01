﻿// <auto-generated />
using System;
using MAVN.Service.Referral.MsSqlRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    [DbContext(typeof(ReferralContext))]
    partial class ReferralContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("referral")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("MAVN.Service.Referral.MsSqlRepositories.Entities.FriendReferralEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CampaignId")
                        .HasColumnName("campaign_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnName("creation_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("ReferredId")
                        .HasColumnName("referred_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ReferrerId")
                        .HasColumnName("referrer_id")
                        .HasColumnType("uuid");

                    b.Property<int>("State")
                        .HasColumnName("state")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("friend_referral");
                });

            modelBuilder.Entity("MAVN.Service.Referral.MsSqlRepositories.Entities.PurchaseReferralHistoryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ReferredId")
                        .HasColumnName("referred_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ReferrerId")
                        .HasColumnName("referrer_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ReferredId");

                    b.HasIndex("ReferrerId");

                    b.ToTable("purchase_referral");
                });

            modelBuilder.Entity("MAVN.Service.Referral.MsSqlRepositories.Entities.ReferralEntity", b =>
                {
                    b.Property<Guid>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("customer_id")
                        .HasColumnType("uuid");

                    b.Property<string>("ReferralCode")
                        .HasColumnName("referral_code")
                        .HasColumnType("varchar(64)");

                    b.HasKey("CustomerId");

                    b.HasIndex("ReferralCode")
                        .IsUnique();

                    b.ToTable("customer_referral");
                });

            modelBuilder.Entity("MAVN.Service.Referral.MsSqlRepositories.Entities.ReferralHotelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CampaignId")
                        .HasColumnName("campaign_id")
                        .HasColumnType("uuid");

                    b.Property<string>("ConfirmationToken")
                        .HasColumnName("confirmation_token")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnName("creation_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("EmailHash")
                        .HasColumnName("email_hash")
                        .HasColumnType("char(64)");

                    b.Property<DateTime>("ExpirationDateTime")
                        .HasColumnName("expiration_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FullNameHash")
                        .HasColumnName("name_hash")
                        .HasColumnType("char(64)");

                    b.Property<string>("Location")
                        .HasColumnName("location")
                        .HasColumnType("text");

                    b.Property<string>("PartnerId")
                        .HasColumnName("partner_id")
                        .HasColumnType("text");

                    b.Property<int>("PhoneCountryCodeId")
                        .HasColumnName("phone_country_code_id")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumberHash")
                        .HasColumnName("phone_number_hash")
                        .HasColumnType("char(64)");

                    b.Property<string>("ReferrerId")
                        .HasColumnName("referrer_id")
                        .HasColumnType("text");

                    b.Property<bool>("StakeEnabled")
                        .HasColumnName("stake_enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnName("state")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ConfirmationToken");

                    b.HasIndex("EmailHash");

                    b.HasIndex("ReferrerId");

                    b.ToTable("referral_hotel");
                });

            modelBuilder.Entity("MAVN.Service.Referral.MsSqlRepositories.Entities.PurchaseReferralHistoryEntity", b =>
                {
                    b.HasOne("MAVN.Service.Referral.MsSqlRepositories.Entities.ReferralEntity", "Referred")
                        .WithMany("PurchasesReferred")
                        .HasForeignKey("ReferredId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MAVN.Service.Referral.MsSqlRepositories.Entities.ReferralEntity", "Referrer")
                        .WithMany("PurchaseReferrers")
                        .HasForeignKey("ReferrerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
