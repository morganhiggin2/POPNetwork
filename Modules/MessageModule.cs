using Expo.Server.Client;
using Expo.Server.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Castle.Core;
using System.Threading.Tasks;
using static POPNetwork.Models.MessageModels;
using POPNetwork.Models;
using static POPNetwork.Global.GlobalProperties;
using POPNetwork.Controllers;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace POPNetwork.Modules
{   public class MessageModule
    {
        /// <summary>
        /// adds expo token
        /// save changes to user before
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <param name="expoToken"></param>
        /// <returns></returns>
        public static void addExpoToken(ApplicationDbContext context, ApplicationUser user, string expoToken)
        {
            //attempt to find existing expo token
            UserExpoToken userExpoToken = context.UserExpoTokens.Find(expoToken);

            //if found
            if (userExpoToken != null) {
                //if it belongs to a different user
                if (userExpoToken.userId != user.Id)
                {
                    //change the user of the expo token
                    userExpoToken.user = user;

                    IdentityModule.SafelySaveChanges(context);
                }
            }
            else
            {
                //create user expo token
                userExpoToken = new UserExpoToken();

                //set expo token
                userExpoToken.expoToken = expoToken;

                //set user
                userExpoToken.user = user;

                //add it to user list
                user.expoTokens.Add(userExpoToken);

                IdentityModule.SafelySaveChanges(context);
            }
        }

        /// <summary>
        /// get random expo token of 8 characters long, random tokens start with -
        /// </summary>
        /// <returns></returns>
        public static string getRandomExpoToken(ApplicationDbContext context)
        {
            //generated token
            string generatedToken = "";

            //random generator
            Random random = new Random((int)DateTime.Now.Ticks);

            while (generatedToken == "")
            {
                //generate random token
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                generatedToken = new string(Enumerable.Repeat(chars, 7)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                //if it is not unique
                if (context.UserExpoTokens.Find(generatedToken) != null)
                {
                    generatedToken = "";
                }
            }

            return "-" + generatedToken;
        }

        /// <summary>
        /// get list of expo tokens to send the message to
        /// </summary>
        /// <param name="user">current user</param>
        /// <param name="expoToken">current expo token, can be blank if createCopeis is false</param>
        /// <param name="recipients">user to send</param>
        /// <param name="createCopies">create copies for the current user's other devices</param>
        /// <returns></returns>
        public static List<string> getMessageExpoTokens (ApplicationUser user, string expoToken, List<ApplicationUser> recipients, bool createCopies)
        {
            //get list of receipients expo tokens
            List<string> expoTokens = new List<string>();

            if (createCopies)
            {
                //add list of user's other expo ids
                expoTokens.AddRange(user.expoTokens.Select(e => e.expoToken).ToList());

                //remove current expo token
                expoTokens.Remove(expoToken);
            }

            foreach (ApplicationUser otherUser in recipients)
            {
                //add other user's expo tokens
                expoTokens.AddRange(otherUser.expoTokens.Select(e => e.expoToken).ToList());
            }

            return expoTokens;
        }

        /// <summary>
        /// create friend user message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expoTokens">expo tokens to create messages for</param>
        /// <param name="messageTypeId"></param>
        /// <param name="messageType"></param>
        /// <returns>number of instances of the message created</returns>
        public static List<FriendUserMessage> createFriendUserMessages(ApplicationDbContext context, List<string> expoTokens, string messageTypeId, MESSAGE_TYPES messageType)
        {
            List<FriendUserMessage> friendUserMessages = new List<FriendUserMessage> ();

            //create messages for receipients
            foreach (string messageExpoToken in expoTokens)
            {
                //create message
                FriendUserMessage friendUserMessage = new FriendUserMessage();

                //get current time
                friendUserMessage.timeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                //set message type id
                friendUserMessage.messageId = messageTypeId;

                //set message type
                friendUserMessage.messageType = messageType;

                //set message sending expo token
                friendUserMessage.userExpoToken = messageExpoToken;

                //add to context
                context.FriendUserMessages.Add(friendUserMessage);

                //add friend user message to list
                friendUserMessages.Add(friendUserMessage);
            }

            IdentityModule.SafelySaveChanges(context);

            return friendUserMessages;
        }

        public static string createDirectPushMessageBody(DirectMessage directMessage, ApplicationUser fromUser)
        {
            string body = "";

            if (fromUser.name != "")
            {
                
                body = fromUser.name + " sent you a message";
            }
            else
            {
                body = "You received a message";
            }

            return body;
        }

        public static string createInvitationPushBody(InvitationBase invitationBase)
        {
            //if invitation was participant requesting to join activity
            if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get given inviation
                FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation)invitationBase;

                //ger friend user
                FriendUser friendUser = invitation.inviter;

                //get activity
                FriendActivity friendActivity = invitation.invitee;

                return (friendUser.user.name + " sent an invitation for your activity");
            }
            //if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                return (friendActivity.name + " sent an invitation");
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                return (friendActivity.name + " sent an invitation");
            }
            else
            {
                return "Unsupported Invitation";
            }
        }

        public static string createInvitationBody(InvitationBase invitationBase)
        {
            //if invitation was participant requesting to join activity
            if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get given inviation
                FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation)invitationBase;

                //ger friend user
                FriendUser friendUser = invitation.inviter;

                //get activity
                FriendActivity friendActivity = invitation.invitee;

                return (friendUser.user.name + " is requesting you to join " + friendActivity.name);
            }
            //if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                return (friendActivity.name + " requests you to join");
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                return (friendActivity.name + " requests you to be an admin");
            }
            else
            {
                return "Unsupported Invitation";
            }
        }

        /// <summary>
        /// get the from name of the invitation sender
        /// </summary>
        /// <param name="invitationBase"></param>
        /// <returns></returns>
        public static string getInvitationName(InvitationBase invitationBase)
        {
            //if invitation was participant requesting to join activity
            if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get given inviation
                FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation)invitationBase;

                //ger friend user
                FriendUser friendUser = invitation.inviter;

                return (friendUser.user.name);
            }
            //if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                return (friendActivity.name);
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;
                //get activity
                FriendActivity friendActivity = invitation.inviter;

                return (friendActivity.name);
            }
            else
            {
                return "Unsupported Invitation";
            }
        }

        public static string createAnnouncementPushBody(AnnouncementBase announcementBase)
        {
            if (announcementBase.descriminator == "FriendActivityAnnouncement")
            {
                //get current announcement
                FriendActivityAnnouncement announcement = (FriendActivityAnnouncement)announcementBase;

                //get friend activity from announcement
                FriendActivity friendActivity = announcement.friendActivity;

                return friendActivity.name + "\n" + announcement.message;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// get the name of the sender of the announcement
        /// </summary>
        /// <param name="announcementBase"></param>
        /// <returns></returns>
        public static string getAnnouncementFromName(AnnouncementBase announcementBase)
        {
            if (announcementBase.descriminator == "FriendActivityAnnouncement")
            {
                //get current announcement
                FriendActivityAnnouncement announcement = (FriendActivityAnnouncement)announcementBase;

                //get friend activity from announcement
                FriendActivity friendActivity = announcement.friendActivity;

                return friendActivity.name;
            }
            else
            {
                return "";
            }
        }

        public static string getAnnouncementType(AnnouncementBase announcementBase)
        {
            if (announcementBase.descriminator == "FriendActivityAnnouncement")
            {
                return "activity";
            }
            else
            {
                return "";
            }
        }

        public static string getAnnouncementOtherId(AnnouncementBase announcementBase)
        {
            if (announcementBase.descriminator == "FriendActivityAnnouncement")
            {
                FriendActivityAnnouncement friendActivityAnnouncement = (FriendActivityAnnouncement) announcementBase;

                return friendActivityAnnouncement.friendActivityId;
            }
            else
            {
                return "";
            }
        }

        public static string createConversationPushMessageBody(ConversationBase conversationBase, ApplicationUser fromUser)
        {
            string body = "";

            if (conversationBase.descriminator == "FriendActivityConversation")
            {
                FriendActivityConversation friendActivityConversation = (FriendActivityConversation)conversationBase;

                FriendActivity friendActivity = friendActivityConversation.friendActivity;

                if (friendActivityConversation.conversationType == CONVERSATION_TYPES.FRIEND_ACTIVITY_ALL)
                {
                    body += friendActivity.name + " conversation\n";

                    if (fromUser.name != "")
                    {
                        body += fromUser.name + " sent a message";
                    }
                    else
                    {
                        body += "a message was sent";
                    }
                }
                else if (friendActivityConversation.conversationType == CONVERSATION_TYPES.FRIEND_ACTIVITY_ADMINS)
                {
                    body += friendActivity.name + " creators conversation\n";

                    if (fromUser.name != "")
                    {
                        body += fromUser.name + " sent a message";
                    }
                    else
                    {
                        body += "a message was sent";
                    }
                }
            }

            return body;
        }
        public static string createConversationTitle(ConversationBase conversationBase)
        {
            string title = "";

            if (conversationBase.descriminator == "FriendActivityConversation")
            {
                FriendActivityConversation friendActivityConversation = (FriendActivityConversation)conversationBase;

                FriendActivity friendActivity = friendActivityConversation.friendActivity;

                if (friendActivityConversation.conversationType == CONVERSATION_TYPES.FRIEND_ACTIVITY_ALL)
                {
                    title += "everyone - " + friendActivity.name;
                }
                else if (friendActivityConversation.conversationType == CONVERSATION_TYPES.FRIEND_ACTIVITY_ADMINS)
                {
                    title += "creators only - " + friendActivity.name;
                }
            }

            return title;
        }

        public static Pair<string, string> getInvitationTypeAndId(ApplicationUser currentUser, ApplicationDbContext context, InvitationBase invitationBase)
        {
            //if invitation was participant requesting to join activity
            if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get given inviation
                FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation)invitationBase;

                //get current friendUser
                FriendUser currentFriendUser = currentUser.friendUser;

                if (currentFriendUser == null)
                {
                    InvitationModule.RemoveInvitation(context, invitationBase, false);

                    return new Pair<string, string>(null, null);
                }

                //ger friend user
                FriendUser friendUser = invitation.inviter;

                //get activity
                FriendActivity friendActivity = invitation.invitee;

                //add checks to prevent invalid invitation being sent
                if (friendActivity == null)
                {
                    InvitationModule.RemoveInvitation(context, invitationBase, false);

                    return new Pair<string, string>(null, null);
                }

                if (friendUser == null)
                {
                    InvitationModule.RemoveInvitation(context, invitationBase, false);

                    return new Pair<string, string>(null, null);
                }

                return new Pair<string, string>("person", invitation.inviterId);

            }
            //if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                if (friendActivity == null)
                {
                    InvitationModule.RemoveInvitation(context, invitationBase, false);

                    return new Pair<string, string>(null, null);
                }

                if (friendUser == null)
                {
                    InvitationModule.RemoveInvitation(context, invitationBase, false);

                    return new Pair<string, string>(null, null);
                }

                return new Pair<string, string>("activity", invitation.inviterId);
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = currentUser.friendUser;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                if (friendActivity == null)
                {
                    InvitationModule.RemoveInvitation(context, invitationBase, false);

                    return new Pair<string, string>(null, null);
                }

                if (friendUser == null)
                {
                    InvitationModule.RemoveInvitation(context, invitationBase, false);

                    return new Pair<string, string>(null, null);
                }

                return new Pair<string, string>("activity", invitation.inviterId);
            }
            else
            {
                InvitationModule.RemoveInvitation(context, invitationBase, false);

                return new Pair<string, string>(null, null);
            }
        }

        /// <summary>
        /// create the initialed name for the application user
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string createUserName(UserManager<ApplicationUser> userManager, string userId)
        {
            ApplicationUser user = userManager.FindByIdAsync(userId).Result;

            if (user == null)
            {
                return "";
            }

            return user.name;
        }

        /// <summary>
        /// send push notification to expo tokens
        /// </summary>
        /// <param name="messageBody"></param>
        /// <param name="recepientExpoTokens"></param>
        /// <returns></returns>
        public static Pair<bool, List<string>> sendPushNotifications(string messageBody, List<string> recepientExpoTokens)
        {
            //get rid of temporary expo tokens
            List<string> validRecepientExpoTokens = new List<string>();

            foreach(string token in recepientExpoTokens)
            {
                if (token.Length > 0 && token.Substring(0, 1) != "-")
                {
                    validRecepientExpoTokens.Add(token);
                }
            }

            var expoSDKClient = new Expo.Server.Client.PushApiClient();
            var pushTicketReq = new Expo.Server.Models.PushTicketRequest()
            {
                PushTo = validRecepientExpoTokens,
                PushBadgeCount = 1,
                PushBody = messageBody,
                PushPriority = "default"
            };
            var result = expoSDKClient.PushSendAsync(pushTicketReq).GetAwaiter().GetResult();

            //check for errors
            if (result?.PushTicketErrors?.Count() > 0)
            {
                List<string> errors = new List<string>();

                //if there is an error
                foreach (var error in result.PushTicketErrors)
                {
                    //return error code
                    errors.Add(error.ErrorMessage);
                }

                if (errors.Count > 0)
                {
                    return new Pair<bool, List<string>>(false, errors);
                }
            }

            //return list of receipt ids
            List<string> receiptIds = new List<string>();

            if (result?.PushTicketStatuses?.Count() > 0)
            {
                foreach (var ticketStatus in result?.PushTicketStatuses)
                {
                    receiptIds.Add(ticketStatus.TicketId);
                }
            }
            return new Pair<bool, List<string>>(true, receiptIds);
        }

        public static void checkPushReceipts()
        {
            //account for the fact that the receipt may not exist anymore or has not been created (very very rare this last case)
            //wait for them to be delivered

            /*var pushReceiptResult = expoSDKClient.PushGetReceiptsAsync(pushReceiptReq).Result;

            if (pushReceiptResult?.ErrorInformations?.Count() > 0)
            {
                foreach (var error in result.ErrorInformations)
                {
                    Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
                }
            }
            foreach (var pushReceipt in pushReceiptResult.PushTicketReceipts)
            {
                Console.WriteLine($"TicketId & Delivery Status: {pushReceipt.Key} {pushReceipt.Value.DeliveryStatus} {pushReceipt.Value.DeliveryMessage}");
            }
            */
        }

        public static Pair<bool, string> createDirectMessage(ApplicationDbContext context, ApplicationUser fromUser, string expoToken, ApplicationUser otherUser, string message)
        {
            //check if expo token belongs to user
            if (fromUser.expoTokens.FirstOrDefault(e => e.expoToken == expoToken) == null)
            {
                return new Pair<bool, string>(false, "Expo token does not belong to this user");
            }

            //get list of receipients
            List<ApplicationUser> recipients = new List<ApplicationUser>();

            //add user to list
            recipients.Add(otherUser);

            //get list of expo tokens to send messages to
            List<string> expoTokens = getMessageExpoTokens(fromUser, expoToken, recipients, true);

            //create message
            DirectMessage directMessage = new DirectMessage();

            //set values
            directMessage.body = message;
            directMessage.senderUserId = fromUser.Id;
            directMessage.receiverUserId = otherUser.Id;
            directMessage.instanceCount = (uint)expoTokens.Count;

            //add to context
            context.DirectMessages.Add(directMessage);

            //save changes
            IdentityModule.SafelySaveChanges(context);

            //create friend user messages
            List<FriendUserMessage> friendUserMessages = createFriendUserMessages(context, expoTokens, directMessage.id, MESSAGE_TYPES.DIRECT);

            //get header body for direct message
            string messageBody = createDirectPushMessageBody(directMessage, fromUser);

            //remove users expo tokens for notifications

            //remove list of user's other expo ids
            /*List<string> fromUserExpoTokens = fromUser.expoTokens.Select(e => e.expoToken).ToList();

            foreach (string fromUserExpoToken in fromUserExpoTokens)
            {
                expoTokens.Remove(fromUserExpoToken);
            }*/

            //send push notifications
            Pair<bool, List<string>> results = sendPushNotifications(messageBody, expoTokens);

            if (!results.First)
            {
                string totalError = "";

                foreach (string error in results.Second)
                {
                    totalError += error + ";";
                }

                totalError = totalError.Remove(totalError.Length - 2);

                IdentityModule.SafelySaveChanges(context);

                return new Pair<bool, string>(false, totalError);
            }

            /*
            List<string> receiptIds = results.Second;
            
            //add receipt ids to messages
            for (int i = 0; i < friendUserMessages.Count; i++)
            {
                if (receiptIds[i] != null)
                {
                    friendUserMessages[i].recieptId = receiptIds[i];
                }
            }*/

            //if the message is received immediatly, the friendUserMessage will have been deleted
            //so account for the error
            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");

            //notify other user of direct message
            //notifyOfDirectMessage(directMessage, fromUser, otherUser);
        }

        public static Pair<bool, string> createFriendActivityConversation(ApplicationDbContext context, FriendActivity friendActivity, CONVERSATION_TYPES conversationType)
        {
            //friend activity conversation, attempt to find
            FriendActivityConversation conversation = friendActivity.conversations.FirstOrDefault(c => c.conversationType == conversationType);

            //if conversation already exists
            if (conversation != null)
            {
                return new Pair<bool, string>(true, conversation.id);
            }

            //create conversation
            conversation = new FriendActivityConversation();

            //add friend activity
            conversation.friendActivity = friendActivity;

            //add values
            conversation.conversationType = conversationType;

            context.FriendActivityConversations.Add(conversation);

            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, conversation.id);
        }

        public static Pair<bool, string> createConversationMessage(ApplicationDbContext context, ApplicationUser fromUser, string expoToken, string conversationId, string message)
        {
            //check if expo token belongs to user
            if (fromUser.expoTokens.FirstOrDefault(e => e.expoToken == expoToken) == null)
            {
                return new Pair<bool, string>(false, "Expo token does not belong to this user");
            }

            //get conversation
            ConversationBase conversationBase = context.ConversationBases.Find(conversationId);

            if (conversationBase == null)
            {
                return new Pair<bool, string>(false, "Conversation does not exist");
            }

            List<ApplicationUser> recipients = new List<ApplicationUser>();

            //create conversation message
            if (conversationBase.descriminator == "FriendActivityConversation")
            {
                FriendActivityConversation friendActivityConversation = (FriendActivityConversation)conversationBase;

                FriendActivity friendActivity = friendActivityConversation.friendActivity;

                if (friendActivityConversation.conversationType == CONVERSATION_TYPES.FRIEND_ACTIVITY_ALL)
                {
                    //get acitivty users
                    recipients.AddRange(friendActivity.admins.Select(u => u.user).ToList());
                    recipients.AddRange(friendActivity.participants.Select(u => u.user).ToList());

                    //if user is not in activity
                    if (!recipients.Remove(fromUser))
                    {
                        return new Pair<bool, string>(false, "Not a member of the activity, cannot join conversation");
                    }
                }
                else if (friendActivityConversation.conversationType == CONVERSATION_TYPES.FRIEND_ACTIVITY_ADMINS)
                {
                    recipients.AddRange(friendActivity.admins.Select(u => u.user).ToList());

                    //if user is not in activity
                    if (!recipients.Remove(fromUser))
                    {
                        return new Pair<bool, string>(false, "Not an admin of the activity, cannot join conversation");
                    }
                }
            }

            //get expo tokens
            List<string> expoTokens = getMessageExpoTokens(fromUser, expoToken, recipients, true);

            //create conversation message
            ConversationMessage conversationMessage = new ConversationMessage();

            //set values
            conversationMessage.conversationId = conversationBase.id;
            conversationMessage.senderUserId = fromUser.Id;
            conversationMessage.instanceCount = (uint)expoTokens.Count;
            conversationMessage.body = message;

            //update conversation last active
            conversationBase.lastActive = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //add to context
            context.ConversationMessages.Add(conversationMessage);

            IdentityModule.SafelySaveChanges(context);

            //create friend user messages
            List<FriendUserMessage> friendUserMessages = createFriendUserMessages(context, expoTokens, conversationMessage.id, MESSAGE_TYPES.CONVERSATION);

            //get header body for direct message
            string messageBody = createConversationPushMessageBody(conversationBase, fromUser);

            //remove users expo tokens for notifications

            //remove list of user's other expo ids
            /*List<string> fromUserExpoTokens = fromUser.expoTokens.Select(e => e.expoToken).ToList();

            foreach (string fromUserExpoToken in fromUserExpoTokens)
            {
                expoTokens.Remove(fromUserExpoToken);
            }*/

            //send push notifications
            Pair<bool, List<string>> results = sendPushNotifications(messageBody, expoTokens);

            if (!results.First)
            {
                string totalError = "";

                foreach (string error in results.Second)
                {
                    totalError += error + ";";
                }

                totalError = totalError.Remove(totalError.Length - 2);

                return new Pair<bool, string>(false, totalError);
            }
            
            //add receipt ids to messages
            /*List<string> receiptIds = results.Second;
            
            //add receipt ids to messages
            for (int i = 0; i < friendUserMessages.Count; i++)
            {
                if (receiptIds[i] != null)
                {
                    friendUserMessages[i].recieptId = receiptIds[i];
                }
            }*/

            //if the message is received immediatly, the friendUserMessage will have been deleted
            //so account for the error
            
            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, conversationBase.id);

            //notify other user of direct message
            //notifyOfDirectMessage(directMessage, fromUser, otherUser);
        }

        /// <summary>
        /// create invitation message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fromUser"></param>
        /// <param name="invitationBase"></param>
        /// <returns></returns>
        public static Pair<bool, string> createInvitationMessage(ApplicationDbContext context, ApplicationUser fromUser, InvitationBase invitationBase)
        {
            //expo tokens
            List<string> expoTokens = new List<string>();

            List<ApplicationUser> recipients;

            //if invitation was participant requesting to join activity
            if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get given inviation
                FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation)invitationBase;

                //get activity
                FriendActivity friendActivity = invitation.invitee;

                //get list of recipients
                recipients = new List<ApplicationUser>();

                //add admins users
                recipients.AddRange(friendActivity.admins.Select(u => u.user).ToList());

                //get expo tokens
                expoTokens = getMessageExpoTokens(fromUser, "", recipients, false);
            }
            //if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get list of recipients
                recipients = new List<ApplicationUser>();

                //add user
                recipients.Add(friendUser.user);

                //get expo tokens
                expoTokens = getMessageExpoTokens(fromUser, "", recipients, false);
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get list of recipients
                recipients = new List<ApplicationUser>();

                //add user
                recipients.Add(friendUser.user);

                //get expo tokens
                expoTokens = getMessageExpoTokens(fromUser, "", recipients, false);
            }
            else
            {
                return new Pair<bool, string> (false, "Unsupported Invitation");
            }

            //create friend user messages
            List<FriendUserMessage> friendUserMessages = createFriendUserMessages(context, expoTokens, invitationBase.id, MESSAGE_TYPES.INVITATION);

            //get header body for direct message
            string messageBody = createInvitationPushBody(invitationBase);

            //send push notifications
            Pair<bool, List<string>> results = sendPushNotifications(messageBody, expoTokens);

            if (!results.First)
            {
                string totalError = "";

                foreach (string error in results.Second)
                {
                    totalError += error + ";";
                }

                totalError = totalError.Remove(totalError.Length - 2);

                return new Pair<bool, string>(false, totalError);
            }

            /*List<string> receiptIds = results.Second;
            
            //add receipt ids to messages
            for (int i = 0; i < friendUserMessages.Count; i++)
            {
                if (receiptIds[i] != null)
                {
                    friendUserMessages[i].recieptId = receiptIds[i];
                }
            }*/

            //if the message is received immediatly, the friendUserMessage will have been deleted
            //so account for the error
            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");
        }

        public static Pair<bool, string> createFriendActivityAnnouncement(ApplicationDbContext context, ApplicationUser fromUser, string expoToken, FriendActivity friendActivity, string message)
        {
            //check if expo token belongs to user
            if (fromUser.expoTokens.FirstOrDefault(e => e.expoToken == expoToken) == null)
            {
                return new Pair<bool, string>(false, "Expo token does not belong to this user");
            }

            //create announcement
            FriendActivityAnnouncement friendActivityAnnouncement = new FriendActivityAnnouncement();

            //set values
            friendActivityAnnouncement.friendActivity = friendActivity;
            friendActivityAnnouncement.message = message;

            //add to context
            context.FriendActivityAnnouncements.Add(friendActivityAnnouncement);

            IdentityModule.SafelySaveChanges(context);

            //get recipients
            List<ApplicationUser> recipients = new List<ApplicationUser>();

            //add admins and participants
            recipients.AddRange(friendActivity.admins.Select(u => u.user).ToList());
            recipients.AddRange(friendActivity.participants.Select(u => u.user).ToList());

            //remove current user
            recipients.Remove(fromUser);
            
            //get expo tokens
            List<string> expoTokens = getMessageExpoTokens(fromUser, "", recipients, false);

            //create friend user messages
            List<FriendUserMessage> friendUserMessages = createFriendUserMessages(context, expoTokens, friendActivityAnnouncement.id, MESSAGE_TYPES.ANNOUNCEMENT);

            //get header body for direct message
            string messageBody = createAnnouncementPushBody(friendActivityAnnouncement);

            //send push notifications
            Pair<bool, List<string>> results = sendPushNotifications(messageBody, expoTokens);

            if (!results.First)
            {
                string totalError = "";

                foreach (string error in results.Second)
                {
                    totalError += error + ";";
                }

                totalError = totalError.Remove(totalError.Length - 2);

                return new Pair<bool, string>(false, totalError);
            }
            
            /*List<string> receiptIds = results.Second;
            
            //add receipt ids to messages
            for (int i = 0; i < friendUserMessages.Count; i++)
            {
                if (receiptIds[i] != null)
                {
                    friendUserMessages[i].recieptId = receiptIds[i];
                }
            }*/

            //if the message is received immediatly, the friendUserMessage will have been deleted
            //so account for the error
            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");
        }

        /// <summary>
        /// remove existing messages for user
        /// does not save changes
        /// </summary>
        /// <param name="expoTokens">expo tokens of user's devices</param>
        /// <returns></returns>
        public static Pair<bool, string> removeUserMessages(ApplicationDbContext context, List<string> expoTokens)
        {
            //for each user's expo token
            foreach(string expoToken in expoTokens)
            {
                //get expo token's messages
                List<FriendUserMessage> friendUserMessages = context.FriendUserMessages.Where(m => m.userExpoToken == expoToken).ToList();

                //remove them
                context.FriendUserMessages.RemoveRange(friendUserMessages);
            }

            return new Pair<bool, string>(true, "");
        }

        /// <summary>
        /// remove all old friend user messages
        /// removes old direct and conversation messages as well
        /// </summary>
        /// <param name="context"></param>
        public static void removeOldFriendUserMessages(ApplicationDbContext context)
        {
            //get parameters
            long messagesLimitDays = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<long>("OLD_MESSAGES_LIMIT");
            long messagesLimit = TimeSpan.FromDays(messagesLimitDays).Ticks / TimeSpan.TicksPerMillisecond;

            long minTimeStamp = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - messagesLimit;

            List<FriendUserMessage> friendUserMessages = context.FriendUserMessages.Where(m => m.timeStamp <= minTimeStamp).ToList();

            //old temporary expo tokens to remove (assure no duplicates)
            HashSet<string> toRemoveExpoTokens = new HashSet<string>();

            foreach(FriendUserMessage friendUserMessage in friendUserMessages)
            {
                if (friendUserMessage.messageType == MESSAGE_TYPES.DIRECT)
                {
                    DirectMessage directMessage = context.DirectMessages.Find(friendUserMessage.messageId);

                    if (directMessage != null)
                    {
                        directMessage.instanceCount--;
                    }

                    //delete direct message if count is 0
                    if (directMessage.instanceCount == 0)
                    {
                        context.DirectMessages.Remove(directMessage);
                    }
                }
                else if (friendUserMessage.messageType == MESSAGE_TYPES.CONVERSATION){
                    ConversationMessage conversationMessage = context.ConversationMessages.Find(friendUserMessage.messageId);

                    if (conversationMessage != null)
                    {
                        conversationMessage.instanceCount--;
                    }

                    //delete direct message if count is 0
                    if (conversationMessage.instanceCount == 0)
                    {
                        context.ConversationMessages.Remove(conversationMessage);
                    }
                }

                //remove message with expo token as they have been collected
                toRemoveExpoTokens.Add(friendUserMessage.userExpoToken);

                //remove friend user message
                context.FriendUserMessages.Remove(friendUserMessage);

                //save context in loop
                IdentityModule.SafelySaveChanges(context);
            }

            //remove messages from old temporary expo tokens
            removeUserMessages(context, toRemoveExpoTokens.ToList());

            IdentityModule.SafelySaveChanges(context);

            //remove all old user expo tokens
            foreach (string oldTempExpoToken in toRemoveExpoTokens)
            {
                //get user expo token object
                UserExpoToken userExpoToken = context.UserExpoTokens.Find(oldTempExpoToken);

                if (userExpoToken != null)
                {
                    //remove expo token
                    context.UserExpoTokens.Remove(userExpoToken);
                }
            }

            IdentityModule.SafelySaveChanges(context);
        }

        public static void removeOldAnnouncements(ApplicationDbContext context)
        {
            //get parameters
            double announcementLimitDays = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<double>("OLD_ANNOUNCEMENT_LIMIT");
            long announcementLimit = TimeSpan.FromDays(announcementLimitDays).Ticks / TimeSpan.TicksPerMillisecond;

            long minTimeStamp = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - announcementLimit;

            //get old conversations
            List<AnnouncementBase> announcementBases = context.AnnouncementBases.Where(m => m.timeStamp <= minTimeStamp).ToList();

            context.AnnouncementBases.RemoveRange(announcementBases);

            //save context
            IdentityModule.SafelySaveChanges(context);
        }

        public static void removeOldConversations(ApplicationDbContext context)
        {
            //get parameters
            double conversationLimitDays = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<double>("OLD_CONVERSATION_LIMIT");
            long conversationLimit = TimeSpan.FromDays(conversationLimitDays).Ticks / TimeSpan.TicksPerMillisecond;

            long minTimeStamp = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - conversationLimit;

            //get old conversations
            List<ConversationBase> conversationBases = context.ConversationBases.Where(m => m.lastActive <= minTimeStamp).ToList();

            foreach (ConversationBase conversationBase in conversationBases)
            {
                //get conversation messages of this conversation
                List<ConversationMessage> conversationMessages = context.ConversationMessages.Where(m => m.conversationId == conversationBase.id).ToList();

                foreach(ConversationMessage conversationMessage in conversationMessages)
                {
                    //remove regardless of instance count
                    context.ConversationMessages.Remove(conversationMessage);
                }

                //remove conversation
                context.ConversationBases.Remove(conversationBase);
            }

            //save context
            IdentityModule.SafelySaveChanges(context);
        }

        /// <summary>
        /// gets the pending messages for the given expoToken and puts them into the json format
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static JArray getPendingFriendUserMessagesJson(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ApplicationUser user, string expoToken)
        {
            //for every type of message, if it doesn't exist, just simply not return it and delete the message anyways
            //no errors should be thrown for this

            //create the jobject
            JArray messagesArrayContainer = new JArray();

            //get the messages
            List<FriendUserMessage> friendUserMessages = context.FriendUserMessages.Where(m => m.userExpoToken == expoToken).OrderBy(m => m.timeStamp).ToList();

            //add message to master array container
            foreach(FriendUserMessage friendUserMessage in friendUserMessages)
            {
                if (friendUserMessage.messageType == MESSAGE_TYPES.DIRECT)
                {
                    //get direct message
                    DirectMessage directMessage = context.DirectMessages.Find(friendUserMessage.messageId);

                    //if the direct message was found
                    if (directMessage != null)
                    {
                        //create message object
                        JObject messageContainer = new JObject();

                        //add values
                        messageContainer.Add(new JProperty("type", Enum.GetName(friendUserMessage.messageType).ToLower()));
                        messageContainer.Add(new JProperty("timestamp", friendUserMessage.timeStamp));
                        messageContainer.Add(new JProperty("body", directMessage.body));

                        //add type specific values
                        messageContainer.Add(new JProperty("user_id", directMessage.senderUserId));
                        messageContainer.Add(new JProperty("other_user_id", directMessage.senderUserId == user.Id ? directMessage.receiverUserId : directMessage.senderUserId));
                        messageContainer.Add(new JProperty("user_name", createUserName(userManager, directMessage.senderUserId)));
                        messageContainer.Add(new JProperty("is_you", directMessage.senderUserId == user.Id));

                        //add them to array
                        messagesArrayContainer.Add(messageContainer);

                        //decrement direct messages counter
                        directMessage.instanceCount--;

                        if (directMessage.instanceCount == 0)
                        {
                            //delete direct message
                            context.DirectMessages.Remove(directMessage);
                        }
                    }
                }
                else if (friendUserMessage.messageType == MESSAGE_TYPES.CONVERSATION)
                {
                    //get direct message
                    ConversationMessage conversationMessage = context.ConversationMessages.Find(friendUserMessage.messageId);

                    //if the direct message was found
                    if (conversationMessage != null)
                    {
                        //create message object
                        JObject messageContainer = new JObject();

                        //get conversation
                        ConversationBase conversationBase = context.ConversationBases.Find(conversationMessage.conversationId);

                        if (conversationBase != null)
                        {
                            //add values
                            messageContainer.Add(new JProperty("type", Enum.GetName(friendUserMessage.messageType).ToLower()));
                            messageContainer.Add(new JProperty("timestamp", friendUserMessage.timeStamp));
                            messageContainer.Add(new JProperty("body", conversationMessage.body));

                            //add type specific values
                            messageContainer.Add(new JProperty("user_id", conversationMessage.senderUserId));
                            messageContainer.Add(new JProperty("user_name", createUserName(userManager, conversationMessage.senderUserId)));
                            messageContainer.Add(new JProperty("conversation_id", conversationMessage.conversationId));
                            messageContainer.Add(new JProperty("conversation_name", createConversationTitle(conversationBase)));
                            messageContainer.Add(new JProperty("is_you", conversationMessage.senderUserId == user.Id));

                            //add them to array
                            messagesArrayContainer.Add(messageContainer);
                        }

                        //decrement direct messages counter
                        conversationMessage.instanceCount--;

                        if (conversationMessage.instanceCount == 0)
                        {
                            //delete direct message
                            context.ConversationMessages.Remove(conversationMessage);
                        }
                    }
                }
                else if (friendUserMessage.messageType == MESSAGE_TYPES.INVITATION)
                {
                    //get direct message
                    InvitationBase invitationBase = context.InvitationBases.Find(friendUserMessage.messageId);

                    //get other id and other type
                    Pair<string, string> otherValuePair = getInvitationTypeAndId(user, context, invitationBase);

                    //if the direct message was found
                    if (invitationBase != null && otherValuePair.First != null)
                    {
                        //create message object
                        JObject messageContainer = new JObject();

                        //add values
                        messageContainer.Add(new JProperty("type", Enum.GetName(friendUserMessage.messageType).ToLower()));
                        messageContainer.Add(new JProperty("timestamp", friendUserMessage.timeStamp));
                        messageContainer.Add(new JProperty("body", createInvitationBody(invitationBase)));

                        //add type specific values
                        messageContainer.Add(new JProperty("name", getInvitationName(invitationBase)));
                        messageContainer.Add(new JProperty("invitation_id", invitationBase.id));
                        messageContainer.Add(new JProperty("other_type", otherValuePair.First));
                        messageContainer.Add(new JProperty("other_id", otherValuePair.Second));

                        //add them to array
                        messagesArrayContainer.Add(messageContainer);
                    }
                }
                else if (friendUserMessage.messageType == MESSAGE_TYPES.ANNOUNCEMENT)
                {
                    //get direct message
                    AnnouncementBase announcementBase = context.AnnouncementBases.Find(friendUserMessage.messageId);

                    //if the direct message was found
                    if (announcementBase != null)
                    {
                        //create message object
                        JObject messageContainer = new JObject();

                        //add values
                        messageContainer.Add(new JProperty("type", Enum.GetName(friendUserMessage.messageType).ToLower()));
                        messageContainer.Add(new JProperty("timestamp", friendUserMessage.timeStamp));
                        messageContainer.Add(new JProperty("body", announcementBase.message));

                        //add type specific values
                        messageContainer.Add(new JProperty("name", getAnnouncementFromName(announcementBase)));
                        messageContainer.Add(new JProperty("announcement_id", getAnnouncementOtherId(announcementBase)));
                        messageContainer.Add(new JProperty("announcement_type", getAnnouncementType(announcementBase)));

                        //add them to array
                        messagesArrayContainer.Add(messageContainer);
                    }
                }
            }

            //in case the messge got deleted in the middle of this transaction
            foreach(FriendUserMessage friendUserMessage in friendUserMessages)
            {
                if (context.FriendUserMessages.Contains(friendUserMessage))
                {
                    context.Remove(friendUserMessage);
                }
            }

            IdentityModule.SafelySaveChanges(context);

            //for testing user @deletewhentestingisdone
            JArray testingMessages = TestingModule.getMessages(context, userManager, user, expoToken);
            messagesArrayContainer.Merge(testingMessages);

            return messagesArrayContainer;
        }

        /// <summary>
        /// send out notifications to participants of activities that start soon
        /// </summary>
        /// <param name="context"></param>
        public static void sendOutSoonFriendActivitiyReminders(ApplicationDbContext context)
        {
            ///foreach(FriendActivity friend)
        }
    }
}

//to enable mutiple devices, have the message be sent to expo tokens and not users. If that user
//sends a message, it send a message to the other expo token of the other device 
//of itself sending it to another person. When it pulls that it adds it to it's 
//chat database

/*

//have way to change globalvariable at runtime to turn off this feature without republish?

if user is admin 
and new expo token
  set global variable to true

if getting pending messagse
populate with fake messages
set global variable to false

-direct, from bob
-conversation, from shared activity with bob
-invitation, requesting to join bob's other activity (still pending)
  -two invitations, one for accept, and one for reject
-announcement, from shared actiivty with bob

 */