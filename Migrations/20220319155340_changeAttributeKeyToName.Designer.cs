﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using POPNetwork.Controllers;

#nullable disable

namespace POPNetwork.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220319155340_changeAttributeKeyToName")]
    partial class changeAttributeKeyToName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("FriendActivityFriendUser", b =>
                {
                    b.Property<string>("adminsApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("createdActivitiesid")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("adminsApplicationUserId", "createdActivitiesid");

                    b.HasIndex("createdActivitiesid");

                    b.ToTable("FriendActivityFriendUser");
                });

            modelBuilder.Entity("FriendActivityFriendUser1", b =>
                {
                    b.Property<string>("participantsApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("participatingActivitiesid")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("participantsApplicationUserId", "participatingActivitiesid");

                    b.HasIndex("participatingActivitiesid");

                    b.ToTable("FriendActivityFriendUser1");
                });

            modelBuilder.Entity("FriendUserFriendUserAttribute", b =>
                {
                    b.Property<string>("attributesname")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("usersApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("attributesname", "usersApplicationUserId");

                    b.HasIndex("usersApplicationUserId");

                    b.ToTable("FriendUserFriendUserAttribute");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("POPNetwork.Models.CasualUser", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ApplicationUserId");

                    b.ToTable("CasualUsers");
                });

            modelBuilder.Entity("POPNetwork.Models.DatingUser", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ApplicationUserId");

                    b.ToTable("DatingUsers");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivity", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("dateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("invitationMethod")
                        .HasColumnType("int");

                    b.Property<int>("inviteCap")
                        .HasColumnType("int");

                    b.Property<bool>("isPhysical")
                        .HasColumnType("bit");

                    b.Property<int>("maximumAge")
                        .HasColumnType("int");

                    b.Property<int>("minimumAge")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Point>("searchLocation")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.Property<double>("searchRadius")
                        .HasColumnType("float");

                    b.Property<Point>("targetLocation")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.HasKey("id");

                    b.ToTable("FriendActivities");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityDynamicValues", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("friendActivityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("pendingInvites")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("friendActivityId")
                        .IsUnique();

                    b.ToTable("FriendActivitiesDynamicValues");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUser", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<string>("description")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("");

                    b.Property<Point>("location")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.HasKey("ApplicationUserId");

                    b.ToTable("FriendUsers");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUserAttribute", b =>
                {
                    b.Property<string>("name")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("name");

                    b.ToTable("FriendUserAttributes");
                });

            modelBuilder.Entity("POPNetwork.Models.Invitation", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("inviteeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("inviterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("transferType")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("inviteeId");

                    b.HasIndex("inviterId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("POPNetwork.Models.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<int>("age")
                        .HasColumnType("int");

                    b.Property<DateTime>("birthdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("FriendActivityFriendUser", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendUser", null)
                        .WithMany()
                        .HasForeignKey("adminsApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendActivity", null)
                        .WithMany()
                        .HasForeignKey("createdActivitiesid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FriendActivityFriendUser1", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendUser", null)
                        .WithMany()
                        .HasForeignKey("participantsApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendActivity", null)
                        .WithMany()
                        .HasForeignKey("participatingActivitiesid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FriendUserFriendUserAttribute", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendUserAttribute", null)
                        .WithMany()
                        .HasForeignKey("attributesname")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendUser", null)
                        .WithMany()
                        .HasForeignKey("usersApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("POPNetwork.Models.CasualUser", b =>
                {
                    b.HasOne("POPNetwork.Models.ApplicationUser", "user")
                        .WithOne("casualUser")
                        .HasForeignKey("POPNetwork.Models.CasualUser", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("POPNetwork.Models.DatingUser", b =>
                {
                    b.HasOne("POPNetwork.Models.ApplicationUser", "user")
                        .WithOne("datingUser")
                        .HasForeignKey("POPNetwork.Models.DatingUser", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityDynamicValues", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendActivity", null)
                        .WithOne("dynamicValues")
                        .HasForeignKey("POPNetwork.Models.FriendActivityDynamicValues", "friendActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUser", b =>
                {
                    b.HasOne("POPNetwork.Models.ApplicationUser", "user")
                        .WithOne("friendUser")
                        .HasForeignKey("POPNetwork.Models.FriendUser", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("POPNetwork.Models.Invitation", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendUser", null)
                        .WithMany("receivedInvitations")
                        .HasForeignKey("inviteeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendActivity", null)
                        .WithMany("receivedInvitations")
                        .HasForeignKey("inviteeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendUser", null)
                        .WithMany("givenInvitations")
                        .HasForeignKey("inviterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendActivity", null)
                        .WithMany("givenInvitations")
                        .HasForeignKey("inviterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivity", b =>
                {
                    b.Navigation("dynamicValues")
                        .IsRequired();

                    b.Navigation("givenInvitations");

                    b.Navigation("receivedInvitations");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUser", b =>
                {
                    b.Navigation("givenInvitations");

                    b.Navigation("receivedInvitations");
                });

            modelBuilder.Entity("POPNetwork.Models.ApplicationUser", b =>
                {
                    b.Navigation("casualUser")
                        .IsRequired();

                    b.Navigation("datingUser")
                        .IsRequired();

                    b.Navigation("friendUser")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
