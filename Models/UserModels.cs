using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static POPNetwork.Global.GlobalProperties;
using static POPNetwork.Models.MessageModels;

namespace POPNetwork.Models;

//user models
public class FriendUser
{
    public FriendUser()
    {
        shown = true;
        lastActive = DateTime.Now;
        shown = true;
        isActive = true;
        profile_image_0_active = false;
        profile_image_1_active = false;
        profile_image_2_active = false;

        attributes = new HashSet<FriendUserAttribute>();
        points = new HashSet<FriendUserPoint>();
        createdActivities = new HashSet<FriendActivity>();
        participatingActivities = new HashSet<FriendActivity>();

        givenFriendUserToFriendUserInvitations = new HashSet<FriendUserToFriendUserInvitation>();
        receivedFriendUserToFriendUserInvitations = new HashSet<FriendUserToFriendUserInvitation>();
        givenFriendUserToFriendActivityInvitations = new HashSet<FriendUserToFriendActivityInvitation>();
        receivedFriendActivityToFriendUserInvitations = new HashSet<FriendActivityToFriendUserInvitation>();
    }

    //ref foreign key for the application user one to one relationship
    [Key]
    public string ApplicationUserId { get; set; }

    //user
    public virtual ApplicationUser user { get; set; }

    //description
    public string description { get; set; }

    //profile picture 1 (main) SET BlobName MAX LENGTH 
    //[Column(TypeName = "varchar(200)")]
    //[MaxLength()] - for ints, use the number in varchar for it's max length

    //location
    public Point location { get; set; }

    //if active
    public bool isActive { get; set; }

    //last active
    public DateTime lastActive { get; set; }

    //want to be shown
    public bool shown { get; set; }

    //profile images active status
    public bool profile_image_0_active { get; set; }
    public bool profile_image_1_active { get; set; }
    public bool profile_image_2_active { get; set; }


    //dynamics values
    public virtual FriendUserDynamicValues dynamicValues { get; set; }

    //attributes
    public virtual ICollection<FriendUserAttribute> attributes { get; set; }

    //points
    public virtual ICollection<FriendUserPoint> points { get; set; }

    //activities they are admins of
    public virtual ICollection<FriendActivity> createdActivities { get; set; }

    //activities they are participants of
    public virtual ICollection<FriendActivity> participatingActivities { get; set; }

    //given friend user to friend user invitations
    public virtual ICollection<FriendUserToFriendUserInvitation> givenFriendUserToFriendUserInvitations { get; set; }

    //received friend user to friend user invitations
    public virtual ICollection<FriendUserToFriendUserInvitation> receivedFriendUserToFriendUserInvitations { get; set; }

    //given friend user to friend activity invitations
    public virtual ICollection<FriendUserToFriendActivityInvitation> givenFriendUserToFriendActivityInvitations { get; set; }

    //recieved friend activity to friend user invitations
    public virtual ICollection<FriendActivityToFriendUserInvitation> receivedFriendActivityToFriendUserInvitations { get; set; }
}

public class FriendUserDynamicValues
{
    public FriendUserDynamicValues()
    {
        numberOfReports = 0;
    }

    //friend user id
    [Key]
    public string friendUserId { get; set; }

    //friend user
    public virtual FriendUser friendUser { get; set; }

    //number of reports
    public byte numberOfReports { get; set; }
}

//feature models
public class FriendUserAttribute
{
    public FriendUserAttribute()
    {
        users = new HashSet<FriendUser>();
    }

    //attribute
    [Key]
    public string name { get; set; }

    //users who have this attribute
    public virtual ICollection<FriendUser> users { get; set; }
}

public class FriendUserPoint
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    public virtual FriendUser friendUser { get; set; }

    public string friendUserId { get; set; }

    public string caption { get; set; }

    public byte orderAddedIndex { get; set; }

    public FriendUserPoint(byte index)
    {
        caption = "";
        orderAddedIndex = index;
    }

    public FriendUserPoint()
    {
        caption = "";
    }
}

public class FriendActivity
{
    public FriendActivity()
    {
        shown = true;
        isActive = true;
        targetLocation = new Point(0, 0) { SRID = 4326 };
        searchLocation = new Point(0, 0) { SRID = 4326 };
        gender = "all";
        address = "";
        inviteCap = int.MaxValue;
        participantsCap = int.MaxValue;
        searchRadius = double.MaxValue;
        attributes = new HashSet<FriendActivityAttribute>();
        points = new HashSet<FriendActivityPoint>();
        participants = new HashSet<FriendUser>();
        admins = new List<FriendUser>();

        givenFriendActivityToFriendActivityInvitations = new HashSet<FriendActivityToFriendActivityInvitation>();
        receivedFriendActivityToFriendActivityInvitations = new HashSet<FriendActivityToFriendActivityInvitation>();
        givenFriendActivityToFriendUserInvitations = new HashSet<FriendActivityToFriendUserInvitation>();
        receivedFriendUserToFriendActivityInvitations = new HashSet<FriendUserToFriendActivityInvitation>();
        announcements = new HashSet<FriendActivityAnnouncement>();
        conversations = new HashSet<FriendActivityConversation>();
    }

    //id
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    //name
    [Required]
    public string name { get; set; }

    //want to be shown
    public bool shown { get; set; }

    //is it being shown
    public bool isActive { get; set; }

    //description
    public string description { get; set; }

    //address
    public string address { get; set; }

    //date
    public DateTime dateTime { get; set; }

    //is physical
    public bool isPhysical { get; set; }

    //target location 
    public Point targetLocation { get; set; }

    //search location 
    public Point searchLocation { get; set; }

    //maximum age
    public int maximumAge { get; set; }

    //minimum age
    public int minimumage { get; set; }

    //gender
    public string gender { get; set; }

    //attributes
    public virtual ICollection<FriendActivityAttribute> attributes { get; set; }

    //points
    public virtual ICollection<FriendActivityPoint> points { get; set; }

    //invitation method
    public JOIN_INVITATION_METHODS invitationMethod { get; set; }

    //invite cap
    public int inviteCap { get; set; }

    //number of participants
    public int? participantsCap { get; set; }

    //search radius
    public double searchRadius { get; set; }

    //parties in the activity
    public virtual ICollection<FriendUser> participants { get; set; }

    //admins in the activity
    public virtual ICollection<FriendUser> admins { get; set; }

    //dynamic values
    public virtual FriendActivityDynamicValues dynamicValues { get; set; }

    //given friend user to friend user invitations
    public virtual ICollection<FriendActivityToFriendActivityInvitation> givenFriendActivityToFriendActivityInvitations { get; set; }

    //received friend user to friend user invitations
    public virtual ICollection<FriendActivityToFriendActivityInvitation> receivedFriendActivityToFriendActivityInvitations { get; set; }

    //recieved friend activity to friend user invitations
    public virtual ICollection<FriendActivityToFriendUserInvitation> givenFriendActivityToFriendUserInvitations { get; set; }

    //given friend user to friend activity invitations
    public virtual ICollection<FriendUserToFriendActivityInvitation> receivedFriendUserToFriendActivityInvitations { get; set; }

    //announcments
    public virtual ICollection<FriendActivityAnnouncement> announcements { get; set; }

    //conversations
    public virtual ICollection<FriendActivityConversation> conversations { get; set; }

    //size
    //invite only (only the admin can request people to join)
    //invites requred (invite must be accepted to join activity, else anyone can join
}

public class AnnouncementBase
{
    public AnnouncementBase()
    {
        timeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    //descriminator
    public string descriminator { get; set; }

    //message of announcement
    public string message { get; set; }

    public long timeStamp { get; set; }
}

public class FriendActivityAnnouncement : AnnouncementBase
{
    public string friendActivityId { get; set; }

    public virtual FriendActivity friendActivity { get; set; }
}

public class FriendActivityAttribute
{
    public FriendActivityAttribute()
    {
        activities = new HashSet<FriendActivity>();
    }

    //attribute
    [Key]
    public string name { get; set; }

    //users who have this attribute
    public virtual ICollection<FriendActivity> activities { get; set; }
}
public class FriendActivityPoint
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    public virtual FriendActivity friendActivity { get; set; }

    public string friendActivityId { get; set; }

    public string caption { get; set; }

    public byte orderAddedIndex { get; set; }

    public FriendActivityPoint(byte index)
    {
        orderAddedIndex = index;
        caption = "";
    }

    public FriendActivityPoint()
    {
        caption = "";
    }
}

public class FriendActivityDynamicValues
{
    public FriendActivityDynamicValues()
    {
        pendingInvites = 0;
        numParticipants = 0;
        numAdmins = 1;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    //friend activity id
    public string friendActivityId { get; set; }

    //corresponding friend activity
    //public FriendActivity friendActivity { get; set; }

    //number of pending invites
    public int pendingInvites { get; set; }

    //TODO both nums, add proper updates
    //number of participants
    public int numParticipants { get; set; }

    //number of admins
    public int numAdmins { get; set; }
}
//dynamically changing (prone to being updated alot) friendactivity table
//stores number of invites sent
public class UpdateFriendUserInformationModel
{
    [Display(Name = "name")]
    [StringLength(1024)]
    public string? name { get; set; }

    [Display(Name = "gender")]
    [StringLength(1024)]
    public string? gender { get; set; }

    [Display(Name = "birthdate")]
    [StringLength(1024)]
    public string? birthdate { get; set; }

    [Display(Name = "description")]
    [StringLength(1024)]
    public string? description { get; set; }

    [Display(Name = "attributes")]
    public List<string>? attributes { get; set; }

    [Display(Name = "location")]
    public CoordinateModel? location { get; set; }

    [Display(Name = "points")]
    public List<string>? points { get; set; }

    [Display(Name = "new_points")]
    public List<string>? new_points { get; set; }

    [Display(Name = "shown")]
    public bool? shown { get; set; }
}

public class SetDescriptionModel
{
    [Required(ErrorMessage = "Description Required")]
    [Display(Name = "description")]
    [StringLength(1024)]
    public string description { get; set; }
}

public class SetBirthDateModel
{
    [Required(ErrorMessage = "Birthdate Required")]
    [Display(Name = "birth_date")]
    [StringLength(1024)]
    public string birthdate { get; set; }
}

public class AttributeModel
{
    [Required(ErrorMessage = "Attribute Required")]
    [Display(Name = "attribute")]
    [StringLength(1024)]
    public string attribute { get; set; }
}

public class GetUserInformationModel
{
    [Required(ErrorMessage = "Username Required")]
    [Display(Name = "Username")]
    [StringLength(1024)]
    public string username { get; set; }
}

//for like attributes, put in their attributes
//for their location, put in their location

public class GetFriendUsersModel
{
    //if they want to search from a different location, they can by entering this
    [Display(Name = "location")]
    public CoordinateModel? location { get; set; }

    [Required(ErrorMessage = "Radius Required")]
    [Display(Name = "radius")]
    public double radius { get; set; }

    [Required(ErrorMessage = "Page Size Required")]
    [Display(Name = "page size")]
    public int page_size { get; set; }

    [Required(ErrorMessage = "Page Number Required")]
    [Display(Name = "page number")]
    public int page_number { get; set; }

    [Required(ErrorMessage = "Minimum Age Required")]
    [Display(Name = "minimum age")]
    public int minimum_age { get; set; }

    [Required(ErrorMessage = "Maximum Age Required")]
    [Display(Name = "maximum age")]
    public int maximum_age { get; set; }

    //[Required(ErrorMessage = "Gender Required")]
    [Display(Name = "gender")]
    [StringLength(1024)]
    public string? gender { get; set; }

    [Display(Name = "attributes")]
    public List<string>? attributes { get; set; }
}

public class GetFriendUserMapModel
{
    //if they want to search from a different location, they can by entering this
    [Display(Name = "location")]
    //[Required(ErrorMessage = "Location Required")]
    public CoordinateModel location { get; set; }

    [Required(ErrorMessage = "Radius Required")]
    [Display(Name = "radius")]
    public double radius { get; set; }

    [Required(ErrorMessage = "Minimum Age Required")]
    [Display(Name = "minimum age")]
    public int minimum_age { get; set; }

    [Required(ErrorMessage = "Maximum Age Required")]
    [Display(Name = "maximum age")]
    public int maximum_age { get; set; }

    //[Required(ErrorMessage = "Gender Required")]
    [Display(Name = "gender")]
    [StringLength(1024)]
    public string? gender { get; set; }

    [Display(Name = "attributes")]
    public List<string>? attributes { get; set; }
}

public class CreateFriendActivityModel
{
    [Required(ErrorMessage = "Title Required")]
    [Display(Name = "title")]
    [StringLength(1024)]
    public string title { get; set; }

    [Display(Name = "description")]
    [StringLength(1024)]
    public string? description { get; set; }

    [Required(ErrorMessage = "Attributes Required")]
    [Display(Name = "attributes")]
    public List<string> attributes { get; set; }

    [Required(ErrorMessage = "Date Required")]
    [Display(Name = "datetime")]
    [StringLength(1024)]
    public string date_time { get; set; }

    [Required(ErrorMessage = "Is Physical Event")]
    [Display(Name = "is_physical")]
    public bool is_physical { get; set; }

    [Display(Name = "target_location")]
    public CoordinateModel? target_location { get; set; }

    [Display(Name = "search_location")]
    public CoordinateModel? search_location { get; set; }

    [Display(Name = "search_radius")]
    public double? search_radius { get; set; }

    [Display(Name = "address")]
    [StringLength(1024)]
    public string address { get; set; }

    [Display(Name = "invitation_method")]
    [StringLength(1024)]
    public string invitation_method { get; set; }

    [Display(Name = "invite_cap")]
    public int? invite_cap { get; set; }

    [Display(Name = "participants_cap")]
    public int? participants_cap { get; set; }

    [Display(Name = "gender")]
    [StringLength(1024)]
    public string? gender { get; set; }

    [Required(ErrorMessage = "Minimum Age Required")]
    [Display(Name = "minimum_age")]
    public int minimum_age { get; set; }

    [Required(ErrorMessage = "Maximum Age Required")]
    [Display(Name = "maximum_age")]
    public int maximum_age { get; set; }

    [Display(Name = "new_points")]
    public List<string>? new_points { get; set; }
}

public class UpdateFriendActivityModel
{
    [Required(ErrorMessage = "Activity Id Required")]
    [Display(Name = "id")]
    [StringLength(1024)]
    public string id { get; set; }

    [Display(Name = "title")]
    [StringLength(1024)]
    public string? title { get; set; }

    [Display(Name = "description")]
    [StringLength(1024)]
    public string? description { get; set; }

    [Display(Name = "attributes")]
    public List<string>? attributes { get; set; }

    [Display(Name = "datetime")]
    [StringLength(1024)]
    public string? date_time { get; set; }

    [Display(Name = "is_physical_event")]
    public bool? is_physical { get; set; }

    [Display(Name = "target_location")]
    public CoordinateModel? target_location { get; set; }

    [Display(Name = "search_location")]
    public CoordinateModel? search_location { get; set; }

    [Display(Name = "search_radius")]
    public double? search_radius { get; set; }

    [Display(Name = "address")]
    [StringLength(1024)]
    public string? address { get; set; }

    [Display(Name = "invitation_method")]
    [StringLength(1024)]
    public string? invitation_method { get; set; }

    [Display(Name = "invite_cap")]
    public int? invite_cap { get; set; }

    [Display(Name = "participants_cap")]
    public int? participants_cap { get; set; }

    [Display(Name = "gender")]
    [StringLength(1024)]
    public string? gender { get; set; }

    [Display(Name = "minimum_age")]
    public int? minimum_age { get; set; }

    [Display(Name = "maximum_age")]
    public int? maximum_age { get; set; }

    [Display(Name = "points")]
    public List<string>? points { get; set; }

    [Display(Name = "new_points")]
    public List<string>? new_points { get; set; }

    [Display(Name = "shown")]
    public bool? shown { get; set; }
}

public class GetFriendActivitiesModel
{
    //if they want to search from a different location, they can by entering this
    [Display(Name = "location")]
    public CoordinateModel? location { get; set; }

    [Required(ErrorMessage = "Radius Required")]
    [Display(Name = "radius")]
    public double radius { get; set; }

    [Required(ErrorMessage = "Page Size Required")]
    [Display(Name = "page size")]
    public int page_size { get; set; }

    [Required(ErrorMessage = "Page Number Required")]
    [Display(Name = "page number")]
    public int page_number { get; set; }

    [Display(Name = "attributes")]
    public List<string>? attributes { get; set; }

    [Display(Name = "medium")]
    [StringLength(1024)]
    public string? medium { get; set; }
}

public class GetFriendActivitiesMapModel
{
    //if they want to search from a different location, they can by entering this
    [Display(Name = "location")]
    public CoordinateModel? location { get; set; }

    [Required(ErrorMessage = "Radius Required")]
    [Display(Name = "radius")]
    public double radius { get; set; }

    [Display(Name = "attributes")]
    public List<string>? attributes { get; set; }

    [Display(Name = "medium")]
    [StringLength(1024)]
    public string? medium { get; set; }
}

public class AddFriendGroupModel
{
    [Required(ErrorMessage = "Attribues Required")]
    [Display(Name = "attributes")]
    public List<string> attribues { get; set; }
}

public class FriendUserSearchModel
{
    public Point location { get; set; }

    public string description { get; set; }

    public bool[] profile_images_active { get; set; }

    public ApplicationUser user { get; set; }

    //public string mainProfileImageURL {get; set;}

    public FriendUserSearchModel(ApplicationUser user)
    {
        this.user = user;
    }

    public override int GetHashCode()
    {
        return this.user.Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj == null)
        {
            return false;
        }

        FriendUserSearchModel other = obj as FriendUserSearchModel;

        if (this.user.Id == other.user.Id)
        {
            return true;
        }

        return false;
    }
}

public class FriendActivitySearchModel
{
    public string id { get; set; }

    public string name { get; set; }

    public DateTime dateTime { get; set; }

    public Point location { get; set; }

    //public string mainProfileImageURL {get; set;}

    public FriendActivitySearchModel(string id)
    {
        this.id = id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj == null)
        {
            return false;
        }

        FriendActivitySearchModel other = obj as FriendActivitySearchModel;

        if (this.id == other.id)
        {
            return true;
        }

        return false;
    }
}

public class FriendUserUploadProfileImageModel
{
    [Required(ErrorMessage = "Image Required")]
    [Display(Name = "image")]
    public short num { get; set; }

    [Required(ErrorMessage = "Image Required")]
    [Display(Name = "image")]
    public IFormFile image { get; set; }
}

public class FriendActivitySearchMapModel
{
    public string id { get; set; }

    public string name { get; set; }

    public Point location { get; set; }

    //public string mainProfileImageURL {get; set;}

    public FriendActivitySearchMapModel(string id)
    {
        this.id = id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj == null)
        {
            return false;
        }

        FriendActivitySearchModel other = obj as FriendActivitySearchModel;

        if (this.id == other.id)
        {
            return true;
        }

        return false;
    }
}

public class CoordinateModel
{
    [Required(ErrorMessage = "Longitue Required")]
    [Display(Name = "longitude")]
    public double longitude { get; set; }

    [Required(ErrorMessage = "Latitude Required")]
    [Display(Name = "latitude")]
    public double latitude { get; set; }
}

public class DatingUser
{
    //ref foreign key for the application user one to one relationship
    [Key]
    public string ApplicationUserId { get; set; }

    //user
    public virtual ApplicationUser user { get; set; }

    //description
    public string description { get; set; }

    //if active / showing
    public Boolean active { get; set; }
}

public class CasualUser
{
    //ref foreign key for the application user one to one relationship
    [Key]
    public string ApplicationUserId { get; set; }

    //user
    public virtual ApplicationUser user { get; set; }

    //description
    public string description { get; set; }

    //if active / showing
    public Boolean active { get; set; }
}

[Index(nameof(friendUserFirstId), nameof(friendUserLastId))]
public class FriendUserFriendUserBlock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    public virtual FriendUser friendUserFirst { get; set; }

    public string friendUserFirstId { get; set; }

    public virtual FriendUser friendUserLast { get; set; }

    public string friendUserLastId { get; set; }
}

[Index(nameof(friendActivityId), nameof(friendUserId))]
public class FriendActivityFriendUserBlock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    public virtual FriendActivity friendActivity { get; set; }

    public string friendActivityId { get; set; }

    public virtual FriendUser friendUser { get; set; }

    public string friendUserId { get; set; }   
}

//TODO add in delete methods
[Index(nameof(friendActivityId), nameof(friendUserId))]
public class FriendUserFriendActivityBlock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }

    public virtual FriendActivity friendActivity { get; set; }

    public string friendActivityId { get; set; }

    public virtual FriendUser friendUser { get; set; }

    public string friendUserId { get; set; }
}

public class SendFeedbackModel
{
    [Required(ErrorMessage = "Feedback Required")]
    [Display(Name = "feedback")]
    [StringLength(4096)]
    public string feedback { get; set; }
}