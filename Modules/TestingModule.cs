using Castle.Core;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using POPNetwork.Controllers;
using POPNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static POPNetwork.Global.GlobalProperties;
using static POPNetwork.Models.MessageModels;

namespace POPNetwork.Modules
{
    public class TestingModule
    {
        public const string TESTING_USERNAME = "admin";
        public static List<string> expoTokens;

        public static void init()
        {
            expoTokens = new List<string>();
        }

        /// <summary>
        /// when the tester logs in
        /// </summary>
        public static void testerLogin(ApplicationDbContext context, ApplicationUser user, string expoToken)
        {
            //if tester
            if (user.UserName != TESTING_USERNAME)
            {
                return;
            }

            //check if new expo token
            /*List<UserExpoToken> foundExpoTokens = context.UserExpoTokens.Where(d => d.userId == user.Id && d.expoToken == expoToken).ToList();

            if (foundExpoTokens.Count > 0)
            {
                expoTokens.Add(expoToken);
            }
            */

            expoTokens.Add(expoToken);
        }

        /// <summary>
        /// get messages for the testing user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="expoToken"></param>
        /// <returns></returns>
        public static JArray getMessages(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ApplicationUser user, string expoToken)
        {
            //needed:
            //testing user with username
            //purpleorangepink has an activity, and testing user is a participant in it - free hong kong protest
            //activity has a conversation - free hong kong - everyone
            //request from purpleorangepink to join admin's activity - physics 141a study group

            //if not user
            if (user.UserName != TESTING_USERNAME)
            {
                return new JArray();
            }

            //if the expo token is a new expo token (they are getting messages for the first time on the device
            if (!expoTokens.Contains(expoToken))
            {
                return new JArray();
            }

            //remove expo token
            expoTokens.Remove(expoToken);

            JArray messagesArray = new JArray();

            //send direct message
            //attempt to get user
            IdentityUser popUser = context.Users.Where(u => u.UserName == "purpleorangepink").First();

            //get first other user on platform is purpleorangepink cannot be found
            if (popUser == null)
            {
                popUser = context.Users.First();
            }

            if (popUser != null)
            {
                //create message object
                JObject messageContainer = new JObject();

                //add values
                messageContainer.Add(new JProperty("type", Enum.GetName(MESSAGE_TYPES.DIRECT).ToLower()));
                messageContainer.Add(new JProperty("timestamp", DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
                messageContainer.Add(new JProperty("body", "hello, how are you?"));

                //add type specific values
                messageContainer.Add(new JProperty("user_id", popUser.Id));
                messageContainer.Add(new JProperty("other_user_id", popUser.Id));
                messageContainer.Add(new JProperty("user_name", MessageModule.createUserName(userManager, popUser.Id)));
                messageContainer.Add(new JProperty("is_you", false));

                messagesArray.Add(messageContainer);
            }

            //send conversation message
            FriendActivity activity = user.friendUser.participatingActivities.Where(a => a.name == "Free hong kong activism protest").FirstOrDefault();

            if (activity == null)
            {
                activity = user.friendUser.createdActivities.FirstOrDefault();
            }

            if (activity != null)
            {
                ConversationBase conversationBase = activity.conversations.FirstOrDefault();
                FriendUser creator = activity.admins.First();

                if (conversationBase != null && creator != null)
                {
                    //create message object
                    JObject messageContainer = new JObject();

                    //add values
                    messageContainer.Add(new JProperty("type", Enum.GetName(MESSAGE_TYPES.CONVERSATION).ToLower()));
                    messageContainer.Add(new JProperty("timestamp", DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - 1));
                    messageContainer.Add(new JProperty("body", "this is a message from the conversation"));

                    //add type specific values
                    messageContainer.Add(new JProperty("user_id", creator.ApplicationUserId));
                    messageContainer.Add(new JProperty("user_name", creator.user.UserName));
                    messageContainer.Add(new JProperty("conversation_id", conversationBase.id));
                    messageContainer.Add(new JProperty("conversation_name", MessageModule.createConversationTitle(conversationBase)));
                    messageContainer.Add(new JProperty("is_you", false));

                    //add them to array
                    messagesArray.Add(messageContainer);
                }
            }

            FriendUserToFriendActivityInvitation invitation = null;

            foreach(FriendActivity act in user.friendUser.createdActivities)
            {
                List<FriendUserToFriendActivityInvitation> invitations = act.receivedFriendUserToFriendActivityInvitations.ToList();

                if (invitations.Count > 0)
                {
                    invitation = invitations[0];
                    break;
                }
            }

            if (invitation != null)
            {
                Pair<string, string> otherValuePair = MessageModule.getInvitationTypeAndId(user, context, invitation);
                
                if (invitation != null && otherValuePair != null && otherValuePair.First != null && otherValuePair.Second != null)
                {
                    //create message object
                    JObject messageContainer = new JObject();

                    messageContainer.Add(new JProperty("type", Enum.GetName(MESSAGE_TYPES.INVITATION).ToLower()));
                    messageContainer.Add(new JProperty("timestamp", DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - 2));
                    messageContainer.Add(new JProperty("body", MessageModule.createInvitationBody(invitation)));

                    //add type specific values
                    messageContainer.Add(new JProperty("name", MessageModule.getInvitationName(invitation)));
                    messageContainer.Add(new JProperty("invitation_id", invitation.id));
                    messageContainer.Add(new JProperty("other_type", otherValuePair.First));
                    messageContainer.Add(new JProperty("other_id", otherValuePair.Second));

                    //add them to array
                    messagesArray.Add(messageContainer);
                }
            }

            //send accounement message
            if (activity != null)
            {
                //create message object
                JObject messageContainer = new JObject();

                //add values
                messageContainer.Add(new JProperty("type", Enum.GetName(MESSAGE_TYPES.ANNOUNCEMENT).ToLower()));
                messageContainer.Add(new JProperty("timestamp", DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - 3));
                messageContainer.Add(new JProperty("body", "we are going to be running 10 minutes behind schedule"));

                //add type specific values
                messageContainer.Add(new JProperty("name", activity.name));
                messageContainer.Add(new JProperty("announcement_id", activity.id));
                messageContainer.Add(new JProperty("announcement_type", "activity"));

                //add them to array
                messagesArray.Add(messageContainer);
            }

            return messagesArray;
        }
    }
}
