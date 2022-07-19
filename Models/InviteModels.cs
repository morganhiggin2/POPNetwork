using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static POPNetwork.Global.GlobalProperties;

namespace POPNetwork.Models
{
    public class InvitationBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string id { get; set; }

        public INVITATION_TRANSFER_TYPES transferType { get; set; }

        [Required]
        public string descriminator { get; set; }

        public long timeStamp { get; set; }
    }

    public class FriendUserToFriendUserInvitation : InvitationBase
    {
        public virtual FriendUser inviter { get; set; }

        public string inviterId { get; set; }

        public virtual FriendUser invitee { get; set; }

        public string inviteeId { get; set; }
    }

    public class FriendActivityToFriendUserInvitation : InvitationBase
    {
        public virtual FriendActivity inviter { get; set; }

        public string inviterId { get; set; }

        public virtual FriendUser invitee { get; set; }

        public string inviteeId { get; set; }
    }

    public class FriendUserToFriendActivityInvitation : InvitationBase
    {
        public virtual FriendUser inviter { get; set; }

        public string inviterId { get; set; }

        public virtual FriendActivity invitee { get; set; }

        public string inviteeId { get; set; }
    }
    public class FriendActivityToFriendActivityInvitation : InvitationBase
    {
        public virtual FriendActivity inviter { get; set; }

        public string inviterId { get; set; }

        public virtual FriendActivity invitee { get; set; }

        public string inviteeId { get; set; }
    }
}

//invitations need experation period
//add cleaning for expired invitations to loop in main thread

//for the controller

//send the invition

//get all current invitations where invitee is current user
    //loop though each created activity
    //get invitations

//get all current invitations where current user is inviter

//get all current invitations where the invter is the current user's created activity (so only gets for that one activity)

//request invitation
    //can be done by current user or ANY of their created activities
//accept invitation (invitation id)
//decline invitation (invitation id)
//retract invitation (invitation id), invitation's inviter must be current user