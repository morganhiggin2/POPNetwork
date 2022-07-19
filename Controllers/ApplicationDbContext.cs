using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using POPNetwork.Models;
using POPNetwork.Modules;
using System.Linq;
using System.Data;
using System;
using static POPNetwork.Models.MessageModels;

namespace POPNetwork.Controllers;
public class ApplicationDbContext : IdentityDbContext//<ApplicationUser>
{
    //for the Identity system, user information
    public DbSet<ApplicationUser> UserInformation { get; set; }

    //friend users
    public DbSet<FriendUser> FriendUsers { get; set; }

    //friend user dynamic values
    public DbSet<FriendUserDynamicValues> FriendUsersDynamicValues { get; set; }

    //attributes associated with each user
    public DbSet<FriendUserAttribute> FriendUserAttributes { get; set; }

    //friend user points
    public DbSet<FriendUserPoint> FriendUserPoints { get; set; }

    //friend activities
    public DbSet<FriendActivity> FriendActivities { get; set; }

    //friend activity announcements
    public DbSet<FriendActivityAnnouncement> FriendActivityAnnouncements { get; set; }

    //friend activity attributes
    public DbSet<FriendActivityAttribute> FriendActivityAttributes { get; set; }

    //friend activity points
    public DbSet<FriendActivityPoint> friendActivityPoints { get; set; }

    //friend dynamic activity values
    public DbSet<FriendActivityDynamicValues> FriendActivitiesDynamicValues { get; set; }

    //dating users
    public DbSet<DatingUser> DatingUsers { get; set; }

    //casual users
    public DbSet<CasualUser> CasualUsers { get; set; }

    public DbSet<InvitationBase> InvitationBases { get; set; }

    public DbSet<FriendUserToFriendUserInvitation> FriendUserToFriendUserInvitations { get; set; }
    public DbSet<FriendUserToFriendActivityInvitation> FriendUserToFriendActivityInvitations { get; set; }
    public DbSet<FriendActivityToFriendUserInvitation> FriendActivityToFriendUserInvitations { get; set; }
    public DbSet<FriendActivityToFriendActivityInvitation> FriendActivityToFriendActivityInvitations { get; set; }
    public DbSet<FriendActivityFriendUserBlock> FriendActivityFriendUserBlocks { get; set; }
    public DbSet<FriendUserFriendUserBlock> FriendUserFriendUserBlocks { get; set; }
    public DbSet<ResetPasswordKeyCode> ResetPasswordKeyCodes { get; set; }
    public DbSet<VerifyEmailKeyCode> VerifyEmailKeyCodes { get; set; }

    public DbSet<UserExpoToken> UserExpoTokens { get; set; }
    public DbSet<FriendUserMessage> FriendUserMessages { get; set; }
    public DbSet<DirectMessage> DirectMessages { get; set; }
    public DbSet<AnnouncementBase> AnnouncementBases { get; set; }

    public DbSet<ConversationBase> ConversationBases { get; set; }
    public DbSet<FriendActivityConversation> FriendActivityConversations { get; set; }
    public DbSet<ConversationMessage> ConversationMessages { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //include relationship for attributes of application user many-to-many

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.expoTokens)
            .WithOne(e => e.user)
            .HasForeignKey(e => e.userId);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.friendUser)
            .WithOne(u => u.user)
            .HasForeignKey<FriendUser>(u => u.ApplicationUserId);
            //.OnDelete(DeleteBehavior.ClientCascade);
            //.HasPrincipalKey<FriendUser>(u => u.id);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.datingUser)
            .WithOne(u => u.user)
            .HasForeignKey<DatingUser>(u => u.ApplicationUserId);
            //.OnDelete(DeleteBehavior.ClientCascade);
            //.HasPrincipalKey<DatingUser>(u => u.id);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.casualUser)
            .WithOne(u => u.user)
            .HasForeignKey<CasualUser>(u => u.ApplicationUserId);
        //.OnDelete(DeleteBehavior.ClientCascade);
        //.HasPrincipalKey<CasualUser>(u => u.id);

        builder.Entity<FriendUser>()
            .HasOne(u => u.dynamicValues)
            .WithOne(d => d.friendUser)
            .HasForeignKey<FriendUserDynamicValues>(d => d.friendUserId);

        builder.Entity<FriendUser>()
            .HasMany(u => u.attributes)
            .WithMany(a => a.users);

        builder.Entity<FriendUser>()
            .HasMany(u => u.points)
            .WithOne(p => p.friendUser)
            .HasForeignKey(p => p.friendUserId);

        builder.Entity<FriendUser>()
            .HasMany(u => u.createdActivities)
            .WithMany(a => a.admins);

        builder.Entity<FriendUser>()
            .HasMany(u => u.participatingActivities)
            .WithMany(a => a.participants);

        builder.Entity<FriendUser>()
            .Property(u => u.description)
            .HasDefaultValue("");

        builder.Entity<FriendActivity>()
            .HasOne(a => a.dynamicValues)
            .WithOne()
            .HasForeignKey<FriendActivityDynamicValues>(d => d.friendActivityId);

        builder.Entity<FriendActivity>()
            .HasMany(a => a.announcements)
            .WithOne(a => a.friendActivity)
            .HasForeignKey(a => a.friendActivityId);

        builder.Entity<FriendActivity>()
            .HasMany(a => a.attributes)
            .WithMany(a => a.activities);

        builder.Entity<FriendActivity>()
            .HasMany(a => a.points)
            .WithOne(p => p.friendActivity)
            .HasForeignKey(p => p.friendActivityId);

        builder.Entity<FriendActivity>()
            .HasMany(a => a.conversations)
            .WithOne(c => c.friendActivity)
            .HasForeignKey(c => c.friendActivityId);

        builder.Entity<FriendUser>()
            .HasMany(u => u.givenFriendUserToFriendUserInvitations)
            .WithOne(i => i.inviter)
            .HasForeignKey(i => i.inviterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendUser>()
            .HasMany(u => u.receivedFriendUserToFriendUserInvitations)
            .WithOne(i => i.invitee)
            .HasForeignKey(i => i.inviteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendUser>()
            .HasMany(u => u.givenFriendUserToFriendActivityInvitations)
            .WithOne(i => i.inviter)
            .HasForeignKey(i => i.inviterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendUser>()
            .HasMany(u => u.receivedFriendActivityToFriendUserInvitations)
            .WithOne(i => i.invitee)
            .HasForeignKey(i => i.inviteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendActivity>()
            .HasMany(u => u.givenFriendActivityToFriendActivityInvitations)
            .WithOne(i => i.inviter)
            .HasForeignKey(i => i.inviterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendActivity>()
            .HasMany(u => u.receivedFriendActivityToFriendActivityInvitations)
            .WithOne(i => i.invitee)
            .HasForeignKey(i => i.inviteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendActivity>()
            .HasMany(u => u.givenFriendActivityToFriendUserInvitations)
            .WithOne(i => i.inviter)
            .HasForeignKey(i => i.inviterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendActivity>()
            .HasMany(u => u.receivedFriendUserToFriendActivityInvitations)
            .WithOne(i => i.invitee)
            .HasForeignKey(i => i.inviteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendUserFriendUserBlock>()
            .HasOne(b => b.friendUserFirst)
            .WithMany()
            .HasForeignKey(b => b.friendUserFirstId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendUserFriendUserBlock>()
            .HasOne(b => b.friendUserLast)
            .WithMany()
            .HasForeignKey(b => b.friendUserLastId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendActivityFriendUserBlock>()
            .HasOne(b => b.friendActivity)
            .WithMany()
            .HasForeignKey(b => b.friendActivityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendActivityFriendUserBlock>()
            .HasOne(b => b.friendUser)
            .WithMany()
            .HasForeignKey(b => b.friendUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InvitationBase>()
            .HasDiscriminator(i => i.descriminator);

        builder.Entity<ResetPasswordKeyCode>()
            .HasOne(c => c.applicationUser)
            .WithOne()
            .HasForeignKey<ResetPasswordKeyCode>(c => c.applicationUserId);

        builder.Entity<VerifyEmailKeyCode>()
            .HasOne(c => c.applicationUser)
            .WithOne()
            .HasForeignKey<VerifyEmailKeyCode>(c => c.applicationUserId);

        builder.Entity<ConversationBase>()
            .HasDiscriminator(i => i.descriminator);

        builder.Entity<AnnouncementBase>()
            .HasDiscriminator(a => a.descriminator);

        //map custom user functions
        //builder.HasDbFunction(typeof(ApplicationDbContext).GetMethod(nameof(HaversineDistance), new[] {typeof(double), typeof(double), typeof(double), typeof(double)})).HasName("HaversineDistance");
    }

    //user define methods
    /// <summary>
    /// Calculate haversine distance between two latitude and longitude points.
    /// X is latitiude and Y is longitude.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns>double</returns>
    public double HaversineDistance(double lat1, double lat2, double lon1, double lon2)
        => throw new NotSupportedException();
}
