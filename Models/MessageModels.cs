using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static POPNetwork.Global.GlobalProperties;

namespace POPNetwork.Models
{
    public class MessageModels
    {
        [Index(nameof(recieptId))]
        [Index(nameof(userExpoToken))]
        public class FriendUserMessage
        {
            public FriendUserMessage()
            {
                //not expected to be set on creation
                recieptId = "";
            }

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public string id { get; set; }

            // recept of the message
            public string recieptId { get; set; }

            //timestamp
            public long timeStamp { get; set; }

            //user expo id of who it was sent to
            public string userExpoToken { get; set; }

            //type of message
            public MESSAGE_TYPES messageType { get; set; }

            //id of the type message
            public string messageId { get; set; }
        }

        public class DirectMessage
        {
            public DirectMessage()
            {
                instanceCount = 0;
            }

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            //the id of the message
            public string id { get; set; }

            //id of the user who sent it
            public string senderUserId { get; set; }

            //id of the user who is receiving it
            public string receiverUserId { get; set; }

            //the body of the message
            public string body { get; set; }

            //number of message instances
            public uint instanceCount { get; set; }
        }

        public class ConversationBase
        {
            public ConversationBase()
            {
                lastActive = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }

            //the id of the conversation
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public string id { get; set; }

            [Required]
            public string descriminator { get; set; }

            //type of conversation
            public CONVERSATION_TYPES conversationType { get; set; }

            //when last active
            public long lastActive { get; set; }
        }

        public class FriendActivityConversation : ConversationBase
        {
            public virtual FriendActivity friendActivity { get; set; }

            public string friendActivityId { get; set; }
        }

        public class ConversationMessage
        {
            public ConversationMessage()
            {
                instanceCount = 0;
            }

            //the id of the message
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public string id { get; set; }

            //id of the user who sent it
            public string senderUserId { get; set; }

            //id of the conversation
            public string conversationId { get; set; }

            //the body of the message
            public string body { get; set; }

            //number of message instances
            public uint instanceCount { get; set; }
        }

        public class SendDirectMessageModel
        {
            //other user id
            [Required(ErrorMessage = "Other User Id Required")]
            [Display(Name = "other_id")]
            [StringLength(1024)]
            public string other_id { get; set; }

            //body of direct message
            [Required(ErrorMessage = "Body of Message Required")]
            [Display(Name = "body")]
            [StringLength(1024)]
            public string body { get; set; }

            //body of direct message
            [Required(ErrorMessage = "Expo Token")]
            [Display(Name = "expo_token")]
            [StringLength(1024)]
            public string expo_token { get; set; }
        }

        public class SendConversationMessageModel
        {
            //other user id
            [Required(ErrorMessage = "Other User Id Required")]
            [Display(Name = "other_id")]
            [StringLength(1024)]
            public string conversation_id { get; set; }

            //body of direct message
            [Required(ErrorMessage = "Body of Message Required")]
            [Display(Name = "body")]
            [StringLength(1024)]
            public string body { get; set; }

            //body of direct message
            [Required(ErrorMessage = "Expo Token")]
            [Display(Name = "expo_token")]
            [StringLength(1024)]
            public string expo_token { get; set; }
        }

        public class MakeAnnouncementModel
        {
            //body of accouncement
            [Required(ErrorMessage = "Activity Id Required")]
            [Display(Name = "activity_id")]
            [StringLength(1024)]
            public string activity_id { get; set; }

            //body of accouncement
            [Required(ErrorMessage = "Announcement Message Required")]
            [Display(Name = "message")]
            [StringLength(1024)]
            public string message { get; set; }

            [Required(ErrorMessage = "Expo Token Required")]
            [Display(Name = "expo_token")]
            [StringLength(1024)]
            public string expo_token { get; set; }
        }
    }
}
