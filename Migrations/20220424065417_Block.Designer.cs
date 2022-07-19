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
    [Migration("20220424065417_Block")]
    partial class Block
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("FriendActivityFriendActivityAttribute", b =>
                {
                    b.Property<string>("activitiesid")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("attributesname")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("activitiesid", "attributesname");

                    b.HasIndex("attributesname");

                    b.ToTable("FriendActivityFriendActivityAttribute");
                });

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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("dateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.Property<bool>("shown")
                        .HasColumnType("bit");

                    b.Property<Point>("targetLocation")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.HasKey("id");

                    b.ToTable("FriendActivities");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityAttribute", b =>
                {
                    b.Property<string>("name")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("name");

                    b.ToTable("FriendActivityAttributes");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityDynamicValues", b =>
                {
                    b.Property<string>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("friendActivityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("numAdmins")
                        .HasColumnType("int");

                    b.Property<int>("numParticipants")
                        .HasColumnType("int");

                    b.Property<int>("pendingInvites")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("friendActivityId")
                        .IsUnique();

                    b.ToTable("FriendActivitiesDynamicValues");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityFriendUserBlock", b =>
                {
                    b.Property<string>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("friendActivityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("friendUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("id");

                    b.HasIndex("friendUserId");

                    b.HasIndex("friendActivityId", "friendUserId");

                    b.ToTable("FriendActivityFriendUserBlocks");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityPoint", b =>
                {
                    b.Property<string>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("caption")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("friendActivityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte>("orderAddedIndex")
                        .HasColumnType("tinyint");

                    b.HasKey("id");

                    b.HasIndex("friendActivityId");

                    b.ToTable("friendActivityPoints");
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

                    b.Property<DateTime>("lastActive")
                        .HasColumnType("datetime2");

                    b.Property<Point>("location")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.Property<bool>("shown")
                        .HasColumnType("bit");

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

            modelBuilder.Entity("POPNetwork.Models.FriendUserFriendUserBlock", b =>
                {
                    b.Property<string>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("friendUserFirstId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("friendUserLastId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("id");

                    b.HasIndex("friendUserLastId");

                    b.HasIndex("friendUserFirstId", "friendUserLastId");

                    b.ToTable("FriendUserFriendUserBlocks");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUserPoint", b =>
                {
                    b.Property<string>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("caption")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("friendUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<byte>("orderAddedIndex")
                        .HasColumnType("tinyint");

                    b.HasKey("id");

                    b.HasIndex("friendUserId");

                    b.ToTable("friendUserPoints");
                });

            modelBuilder.Entity("POPNetwork.Models.InvitationBase", b =>
                {
                    b.Property<string>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("InvitationType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("transferType")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("InvitationBases");

                    b.HasDiscriminator<string>("InvitationType").HasValue("InvitationBase");
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

            modelBuilder.Entity("POPNetwork.Models.FriendActivityToFriendActivityInvitation", b =>
                {
                    b.HasBaseType("POPNetwork.Models.InvitationBase");

                    b.Property<string>("inviteeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("FriendActivityToFriendActivityInvitation_inviteeId");

                    b.Property<string>("inviterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("FriendActivityToFriendActivityInvitation_inviterId");

                    b.HasIndex("inviteeId");

                    b.HasIndex("inviterId");

                    b.HasDiscriminator().HasValue("FriendActivityToFriendActivityInvitation");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityToFriendUserInvitation", b =>
                {
                    b.HasBaseType("POPNetwork.Models.InvitationBase");

                    b.Property<string>("inviteeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("FriendActivityToFriendUserInvitation_inviteeId");

                    b.Property<string>("inviterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("FriendActivityToFriendUserInvitation_inviterId");

                    b.HasIndex("inviteeId");

                    b.HasIndex("inviterId");

                    b.HasDiscriminator().HasValue("FriendActivityToFriendUserInvitation");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUserToFriendActivityInvitation", b =>
                {
                    b.HasBaseType("POPNetwork.Models.InvitationBase");

                    b.Property<string>("inviteeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("FriendUserToFriendActivityInvitation_inviteeId");

                    b.Property<string>("inviterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("FriendUserToFriendActivityInvitation_inviterId");

                    b.HasIndex("inviteeId");

                    b.HasIndex("inviterId");

                    b.HasDiscriminator().HasValue("FriendUserToFriendActivityInvitation");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUserToFriendUserInvitation", b =>
                {
                    b.HasBaseType("POPNetwork.Models.InvitationBase");

                    b.Property<string>("inviteeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("inviterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasIndex("inviteeId");

                    b.HasIndex("inviterId");

                    b.HasDiscriminator().HasValue("FriendUserToFriendUserInvitation");
                });

            modelBuilder.Entity("FriendActivityFriendActivityAttribute", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendActivity", null)
                        .WithMany()
                        .HasForeignKey("activitiesid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendActivityAttribute", null)
                        .WithMany()
                        .HasForeignKey("attributesname")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("POPNetwork.Models.FriendActivityFriendUserBlock", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendActivity", "friendActivity")
                        .WithMany()
                        .HasForeignKey("friendActivityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendUser", "friendUser")
                        .WithMany()
                        .HasForeignKey("friendUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("friendActivity");

                    b.Navigation("friendUser");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityPoint", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendActivity", "friendActivity")
                        .WithMany("points")
                        .HasForeignKey("friendActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("friendActivity");
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

            modelBuilder.Entity("POPNetwork.Models.FriendUserFriendUserBlock", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendActivity", "friendUserFirst")
                        .WithMany()
                        .HasForeignKey("friendUserFirstId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendUser", "friendUserLast")
                        .WithMany()
                        .HasForeignKey("friendUserLastId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("friendUserFirst");

                    b.Navigation("friendUserLast");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUserPoint", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendUser", "friendUser")
                        .WithMany("points")
                        .HasForeignKey("friendUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("friendUser");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityToFriendActivityInvitation", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendActivity", "invitee")
                        .WithMany("receivedFriendActivityToFriendActivityInvitations")
                        .HasForeignKey("inviteeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendActivity", "inviter")
                        .WithMany("givenFriendActivityToFriendActivityInvitations")
                        .HasForeignKey("inviterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("invitee");

                    b.Navigation("inviter");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivityToFriendUserInvitation", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendUser", "invitee")
                        .WithMany("receivedFriendActivityToFriendUserInvitations")
                        .HasForeignKey("inviteeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendActivity", "inviter")
                        .WithMany("givenFriendActivityToFriendUserInvitations")
                        .HasForeignKey("inviterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("invitee");

                    b.Navigation("inviter");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUserToFriendActivityInvitation", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendActivity", "invitee")
                        .WithMany("receivedFriendUserToFriendActivityInvitations")
                        .HasForeignKey("inviteeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendUser", "inviter")
                        .WithMany("givenFriendUserToFriendActivityInvitations")
                        .HasForeignKey("inviterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("invitee");

                    b.Navigation("inviter");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUserToFriendUserInvitation", b =>
                {
                    b.HasOne("POPNetwork.Models.FriendUser", "invitee")
                        .WithMany("receivedFriendUserToFriendUserInvitations")
                        .HasForeignKey("inviteeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("POPNetwork.Models.FriendUser", "inviter")
                        .WithMany("givenFriendUserToFriendUserInvitations")
                        .HasForeignKey("inviterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("invitee");

                    b.Navigation("inviter");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendActivity", b =>
                {
                    b.Navigation("dynamicValues")
                        .IsRequired();

                    b.Navigation("givenFriendActivityToFriendActivityInvitations");

                    b.Navigation("givenFriendActivityToFriendUserInvitations");

                    b.Navigation("points");

                    b.Navigation("receivedFriendActivityToFriendActivityInvitations");

                    b.Navigation("receivedFriendUserToFriendActivityInvitations");
                });

            modelBuilder.Entity("POPNetwork.Models.FriendUser", b =>
                {
                    b.Navigation("givenFriendUserToFriendActivityInvitations");

                    b.Navigation("givenFriendUserToFriendUserInvitations");

                    b.Navigation("points");

                    b.Navigation("receivedFriendActivityToFriendUserInvitations");

                    b.Navigation("receivedFriendUserToFriendUserInvitations");
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
