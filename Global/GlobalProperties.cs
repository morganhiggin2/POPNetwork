namespace POPNetwork.Global
{
    public class GlobalProperties
    {
        public static string[] PORTAL_TYPES = { "friends", "dating", "casual" };

        public static string[] GENDER_SELECT_OPTIONS = { "all", "male", "female", "other" };

        public static string[] GENDER_OPTIONS = { "male", "female", "other" };

        public static string[] USER_ROLE_TYPES = { "admin", "participant" };

        public static string[] FRIEND_EVENT_TYPES = { "group", "activity" };

        public enum EVENT_TYPES 
        {
            FRIEND_USER,
            FRIEND_ACTIVITY,
            FRIEND_GROUP
        };

        public enum JOIN_INVITATION_METHODS
        {
            ANYONE, //anyone can join
            INVITE_REQUIRED, //use invitations for joining
            INVITE_ONLY //can only be recieved an invite
        };

        /// <summary>
        /// transfer for invitee
        /// </summary>
        public enum INVITATION_TRANSFER_TYPES
        {
            FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY,
            FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY,
            FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY
        };

        /// <summary>
        /// types for messages
        /// </summary>
        public enum MESSAGE_TYPES
        {
            DIRECT,
            CONVERSATION,
            INVITATION,
            ANNOUNCEMENT,
        }

        public enum CONVERSATION_TYPES
        {
            FRIEND_ACTIVITY_ALL,
            FRIEND_ACTIVITY_ADMINS,
        }
    }
}
