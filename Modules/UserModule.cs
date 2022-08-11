
using POPNetwork.Controllers;
using POPNetwork.Models;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using static POPNetwork.Global.GlobalProperties;
using Castle.Core;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace POPNetwork.Modules;
public class UserModule
{
    /// <summary>
    /// create user in the friends, dating, and casual realms
    /// </summary>
    /// <param name="context"></param>
    /// <param name="user"></param>
    public static void createUser(ApplicationDbContext context, ApplicationUser user)
    {
        //using(ApplicationDBContext context = serviceProvider.GetService<ApplicationDBContext>())
        //create new friend user

        //latitude, longitude
        FriendUser friendUser = new FriendUser() { user = user, description = "", location = new Point(90, 0) { SRID = 4326 } };

        //add friend user to sql database
        context.FriendUsers.Add(friendUser);

        //link friend user to user
        user.friendUser = friendUser;

        //create friend user dynamic values
        FriendUserDynamicValues friendUserDynamicValues = new FriendUserDynamicValues();

        //set linking
        friendUserDynamicValues.friendUser = friendUser;

        //add it to context
        context.FriendUsersDynamicValues.Add(friendUserDynamicValues);

        //create dating user
        DatingUser datingUser = new DatingUser() { active = false, user = user, description = "" };

        //add dating user to sql database
        context.DatingUsers.Add(datingUser);

        //link dating user to user
        user.datingUser = datingUser;

        //create casual user
        CasualUser casualUser = new CasualUser() { active = false, user = user, description = "" };

        //add casual user to sql databse
        context.CasualUsers.Add(casualUser);

        //link casual user to user
        user.casualUser = casualUser;

        //save changes
        IdentityModule.SafelySaveChanges(context);

        //create azure blob conatiner for friend user
        AzureBlobModule.createFriendUserContainer(user.Id);
    }

    /// <summary>
    /// remove user from the friend, dating, and casual realms
    /// </summary>
    /// <param name="context"></param>
    /// <param name="user"></param>
    public static void RemoveUser(ApplicationDbContext context, ApplicationUser user)
    {
        //get list of expo tokens
        List<UserExpoToken> userExpoTokens = user.expoTokens.ToList();

        //get string list of expo tokens
        List<string> expoTokens = userExpoTokens.Select(e => e.expoToken).ToList();

        //remove the expo tokens
        context.UserExpoTokens.RemoveRange(userExpoTokens);

        //delete the friend user

        //get the friend user
        FriendUser friendUser = user.friendUser;

        //remove the dynamic values
        context.Remove(friendUser.dynamicValues);

        //remove the messages
        MessageModule.removeUserMessages(context, expoTokens);

        //remove friend user from activities they are participating in
        foreach (FriendActivity activity in friendUser.participatingActivities.ToList())
        {
            activity.participants.Remove(friendUser);

            activity.dynamicValues.numParticipants--;

            //check if it can become active if it is currently not active
            if (!activity.isActive)
            {
                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(activity);
            }
        }

        //remove blocks
        foreach (FriendUserFriendUserBlock block in context.FriendUserFriendUserBlocks.Where(b => (b.friendUserFirstId == friendUser.ApplicationUserId || b.friendUserLastId == friendUser.ApplicationUserId)).ToList())
        {
            context.FriendUserFriendUserBlocks.Remove(block);
        }

        foreach (FriendActivityFriendUserBlock block in context.FriendActivityFriendUserBlocks.Where(b => (b.friendUserId == friendUser.ApplicationUserId)).ToList())
        {
            context.FriendActivityFriendUserBlocks.Remove(block);
        }

        /*//remove pending messages
        foreach (FriendUserMessage pendingMessage in friendUser.pendingMessages)
        {
            //if direct message
            if (pendingMessage.messageType == MESSAGE_TYPES.DIRECT_MESSAGE)
            {
                //find direct message
                DirectMessage directMessage = context.DirectMessages.Find(pendingMessage.messageId);

                if (directMessage != null)
                {
                    //remove direct message
                    context.DirectMessages.Remove(directMessage);
                }
            }

            //remove message
            context.FriendUserMessages.Remove(pendingMessage);
        }*/

        //remove points from user
        foreach (FriendUserPoint point in friendUser.points.ToList())
        {
            //remove point
            removeFriendUserPoint(context, friendUser, point);
        }

        //get all invitations
        List<FriendUserToFriendActivityInvitation> friendUserToFriendActivityInvitations = friendUser.givenFriendUserToFriendActivityInvitations.ToList();
        List<FriendActivityToFriendUserInvitation> friendActivityToFriendUserInvitations = friendUser.receivedFriendActivityToFriendUserInvitations.ToList();
        List<FriendUserToFriendUserInvitation> friendUserToFriendUserInvitations = friendUser.receivedFriendUserToFriendUserInvitations.ToList();
        friendUserToFriendUserInvitations.AddRange(friendUser.givenFriendUserToFriendUserInvitations.ToList());

        //get list of all invitation bases
        List<InvitationBase> invitationBases = new List<InvitationBase>();
        invitationBases.AddRange(friendUserToFriendActivityInvitations);
        invitationBases.AddRange(friendActivityToFriendUserInvitations);
        invitationBases.AddRange(friendUserToFriendUserInvitations);

        //remove invitations for user
        //type of the invitation

        //result
        Pair<bool, string> result;

        //clear invitations
        foreach (InvitationBase invitation in invitationBases)
        {
            //TODO fix is for deleting invitations
            if (invitation.descriminator == "FriendUserToFriendActivityInvitation")
            {
                result = InvitationModule.RemoveInvitation(context, invitation, false, user);
            }
            else if (invitation.descriminator == "FriendActivityToFriendUserInvitation")
            {
                result = InvitationModule.RemoveInvitation(context, invitation, false, user);
            }
            else if (invitation.descriminator == "FriendActivityToFriendActivityInvitation")
            {
                result = InvitationModule.RemoveInvitation(context, invitation, false, user);
            }
            else
            {
                throw new Exception("Unknown invitation type");
            }

            if (!result.First)
            {
                throw new Exception(result.Second);
            }
        }

        //remove from attributes
        foreach (FriendUserAttribute attribute in friendUser.attributes.ToList())
        {
            attribute.users.Remove(friendUser);
        }


        //delete created activities
        foreach (FriendActivity friendActivity in friendUser.createdActivities.ToList())
        {
            /*activity.admins.Remove(friendUser);

            activity.dynamicValues.numAdmins--;

            //if user is only member in activity
            if (activity.dynamicValues.numAdmins == 0)
            {
                //get rid of the activity

                //remove participants and this activity from their participating activities

                //get participants
                List<FriendUser> participants = activity.participants.ToList();

                //remove activity from participants list
                foreach (FriendUser p in participants)
                {
                    //remove activity from their list of activities they are in
                    p.participatingActivities.Remove(activity);
                }

                //clear participants list
                activity.participants.Clear();

                //get all invitations
                friendUserToFriendActivityInvitations = activity.receivedFriendUserToFriendActivityInvitations.ToList();
                friendActivityToFriendUserInvitations = activity.givenFriendActivityToFriendUserInvitations.ToList();
                List<FriendActivityToFriendActivityInvitation> friendActivityToFriendActivityInvitations = activity.receivedFriendActivityToFriendActivityInvitations.ToList();
                friendActivityToFriendActivityInvitations.AddRange(activity.givenFriendActivityToFriendActivityInvitations.ToList());

                //get list of all invitation bases
                invitationBases.Clear();
                invitationBases.AddRange(friendUserToFriendActivityInvitations);
                invitationBases.AddRange(friendActivityToFriendUserInvitations);
                invitationBases.AddRange(friendActivityToFriendActivityInvitations);


                //clear invitations
                foreach (InvitationBase invitation in invitationBases)
                {
                    invitationType = invitation.GetType();

                    if (invitationType is FriendUserToFriendActivityInvitation)
                    {
                        result = InvitationModule.RemoveInvitation(context, invitation, false);
                    }
                    else if (invitationType is FriendActivityToFriendUserInvitation)
                    {
                        result = InvitationModule.RemoveInvitation(context, invitation, false);
                    }
                    else if (invitationType is FriendActivityToFriendActivityInvitation)
                    {
                        result = InvitationModule.RemoveInvitation(context, invitation, false);
                    }
                    else
                    {
                        throw new Exception("Unknown invitation type");
                    }

                    if (!result.First)
                    {
                        throw new Exception(result.Second);
                    }
                }

                //remove corresponding activity dynamics value entry
                context.FriendActivitiesDynamicValues.Remove(activity.dynamicValues);

                //remove activity from context
                context.FriendActivities.Remove(activity);
            }

            //remove activity from current user's created activities list
            friendUser.createdActivities.Remove(activity);*/

            friendActivity.admins.Remove(friendUser);

            friendActivity.dynamicValues.numAdmins--;

            //if user is only member in activity
            if (friendActivity.dynamicValues.numAdmins == 0)
            {
                //get rid of the activity

                //remove participants and this activity from their participating activities

                //get participants
                List<FriendUser> participants = friendActivity.participants.ToList();

                //remove activity from participants list
                foreach (FriendUser p in participants)
                {
                    //remove activity from their list of activities they are in
                    p.participatingActivities.Remove(friendActivity);
                }

                //clear participants list
                friendActivity.participants.Clear();

                //remove points
                foreach (FriendActivityPoint point in friendActivity.points.ToList())
                {
                    removeFriendActivityPoint(context, friendActivity, point);
                }

                //clear invitations

                //get all invitations
                friendUserToFriendActivityInvitations = friendActivity.receivedFriendUserToFriendActivityInvitations.ToList();
                friendActivityToFriendUserInvitations = friendActivity.givenFriendActivityToFriendUserInvitations.ToList();
                List<FriendActivityToFriendActivityInvitation> friendActivityToFriendActivityInvitations = friendActivity.receivedFriendActivityToFriendActivityInvitations.ToList();
                friendActivityToFriendActivityInvitations.AddRange(friendActivity.givenFriendActivityToFriendActivityInvitations.ToList());

                //get list of all invitation bases
                invitationBases = new List<InvitationBase>();
                invitationBases.AddRange(friendUserToFriendActivityInvitations);
                invitationBases.AddRange(friendActivityToFriendUserInvitations);
                invitationBases.AddRange(friendActivityToFriendActivityInvitations);

                foreach (InvitationBase invitation in invitationBases)
                {
                    if (invitation.descriminator == "FriendUserToFriendActivityInvitation")
                    {
                        result = InvitationModule.RemoveInvitation(context, invitation, false, user);
                    }
                    else if (invitation.descriminator == "FriendActivityToFriendUserInvitation")
                    {
                        result = InvitationModule.RemoveInvitation(context, invitation, false, user);
                    }
                    else if (invitation.descriminator == "FriendActivityToFriendActivityInvitation")
                    {
                        result = InvitationModule.RemoveInvitation(context, invitation, false, user);
                    }
                    else
                    {
                        throw new Exception("Unknown invitation type");
                    }
                }

                //remove blocks
                foreach (FriendActivityFriendUserBlock block in context.FriendActivityFriendUserBlocks.Where(b => (b.friendActivityId == friendActivity.id)).ToList())
                {
                    context.Remove(block);
                }

                //remove from attributes
                foreach (FriendActivityAttribute attribute in friendActivity.attributes.ToList())
                {
                    attribute.activities.Remove(friendActivity);
                }

                //clear all announcements
                List<FriendActivityAnnouncement> friendActivityAnnouncements = friendActivity.announcements.ToList();
                context.FriendActivityAnnouncements.RemoveRange(friendActivityAnnouncements);

                //remove corresponding activity dynamics value entry
                context.FriendActivitiesDynamicValues.Remove(friendActivity.dynamicValues);

                //remove activity from context
                context.FriendActivities.Remove(friendActivity);
            }

            //remove activity from current user's created activities list
            friendUser.createdActivities.Remove(friendActivity);
        }

        //remove friend user azure blob storage container and contents
        AzureBlobModule.deleteFriendUserContainer(user.Id);

        //delete from friend users list
        context.FriendUsers.Remove(friendUser);

        //delete the dating user

        //get the dating user
        DatingUser datingUser = user.datingUser;

        //delete from context
        context.DatingUsers.Remove(datingUser);

        //delete the casual user

        //get the causal user
        CasualUser casualUser = user.casualUser;

        //delete from the context
        context.CasualUsers.Remove(casualUser);

        //save changes
        IdentityModule.SafelySaveChanges(context);
    }

    /// <summary>
    /// execute query and algorithm to search for users with defined properties
    /// </summary>
    /// <param name="_friendUser">current friend user</param>
    /// <param name="_context">db context</param>
    /// <param name="_searchLocation">search location</param>
    /// <param name="_radius">radius of search</param>
    /// <param name="_pageSize">size of page</param>
    /// <param name="_pageNumber">page number, starts at 0</param>
    /// <param name="_minimumAge">minimum age of users</param>
    /// <param name="_maximumAge">maximum age of users</param>
    /// <param name="_gender">specified gender of users</param>
    /// <param name="_attributes">list of specific attributes</param>
    /// <returns></returns>
    public static List<FriendUserSearchModel> searchForFriendUsers(FriendUser _friendUser, ApplicationDbContext _context, Point _searchLocation, double _radius, int _pageSize, int _pageNumber, int _minimumAge, int _maximumAge, string _gender, List<string> _attributes)
    {
        //if they set it to the max age (100), set age to insanly high number to include 100+ old people
        if (_maximumAge == 100)
        {
            _maximumAge = 1000;
        }

        //hashset of users from the queries, using hashset to easily remove duplicates later
        List<FriendUserSearchModel> users = new List<FriendUserSearchModel>();

        //list of attributes
        List<FriendUserAttribute> attributes;

        //if they did not provide their own attributes
        if (_attributes == null || _attributes.Count == 0)
        {
            //get their attributes
            //attributes = _friendUser.attributes.ToList();

            //do a blank global search
            attributes = new List<FriendUserAttribute>();
        }
        else
        {
            attributes = new List<FriendUserAttribute>();

            FriendUserAttribute tempAttribute;

            foreach (string attributeName in _attributes)
            {
                //attempt to find the attribute
                tempAttribute = _context.FriendUserAttributes.Find(attributeName);

                //if it was found
                if (tempAttribute != null)
                {
                    //add it to the list
                    attributes.Add(tempAttribute);
                }
            }

            //we still have no attribues, return no results
            if (attributes.Count == 0)
            {
                return new List<FriendUserSearchModel>();
            }
        }

        //performance increase ==> ask for more than numattribites ratio because we know some will be duplicates, allow for some overflow, allow for it to add more than requested

        //make sure page number is greater than 0
        if (_pageNumber < 1)
        {
            return new List<FriendUserSearchModel>();
        }

        //get id's of users you are blocked to
        HashSet<string> blockedIDs = _context.FriendUserFriendUserBlocks.Where(b => b.friendUserFirstId == _friendUser.ApplicationUserId).Select(b => b.friendUserLastId).ToHashSet();

        //if no attributes are used
        if (attributes.Count == 0)
        {
            //do a mass search
            //7926.3812 * Math.Asin(Math.Sqrt(0.5 - Math.Cos((_searchLocation.Y - c.location.Y) * Math.PI / 180) / 2 + Math.Cos(c.location.Y * Math.PI / 180) * Math.Cos(_searchLocation.Y * Math.PI / 180) * (1 - Math.Cos((_searchLocation.X - c.location.X) * Math.PI / 180)) / 2))

            var tempUsers = _context.FriendUsers
                .Select(c => new
                {
                    description = c.description,
                    shown = c.shown,
                    user = c.user,
                    profile_images_active = new bool[3] {c.profile_image_0_active, c.profile_image_1_active, c.profile_image_2_active},
                    distance = c.location.Distance(_searchLocation) * 0.000621371, //0.000621371 is the approx miles per degree,
                    location = c.location,
                })
                .Where(c => c.shown && c.distance < _radius && c.user.age >= _minimumAge && c.user.age <= _maximumAge && (_gender == null || _gender == "" || c.user.gender == _gender))
                .Skip(_pageSize * (_pageNumber - 1))
                .Take(_pageSize)
                .ToList();

            //load each user from query into users
            foreach (var userInfo in tempUsers)
            {
                if (userInfo.user.Id != _friendUser.ApplicationUserId && !blockedIDs.Contains(userInfo.user.Id))
                {
                    users.Add(new FriendUserSearchModel(userInfo.user) { location = userInfo.location, description = userInfo.description, profile_images_active = userInfo.profile_images_active });
                }
            }
        }
        else
        {
            //we are doing a search involving attributes
            //to keep track of how many users had been grabed for each attribute
            //in order to know how many to skip in the case we call it again
            Dictionary<string, int> attributeSkipCount = new Dictionary<string, int>();

            //for seeing if there is a duplicate
            HashSet<FriendUserSearchModel> usersHash = new HashSet<FriendUserSearchModel>();

            //add attributes to attributeSkipCount dictionary
            foreach (FriendUserAttribute friendUserAttribute in attributes)
            {
                attributeSkipCount.Add(friendUserAttribute.name, 0);
            }

            //number of users to fetch total
            int numToFetch = _pageSize * _pageNumber;

            //how many users left to fetch
            int leftToFetch = numToFetch;

            //how many users to query for per attribute
            int numPerAttribute = leftToFetch / attributes.Count;

            //number of results to fetch
            int numToFetchPer = (int)((double)numPerAttribute * 1.3);

            //which attribute we are querying
            int index = 0;

            //temp holder for users from query
            List<FriendUserSearchModel> tempUsers = new List<FriendUserSearchModel>();

            while (leftToFetch > 0)
            {
                //clear temp list
                tempUsers.Clear();

                //if we reach the last attribute
                if (index == attributes.Count)
                {
                    //set index to 0
                    index = 0;
                }

                //if we have no attributes left
                if (attributes.Count == 0)
                {
                    //break out of the while loop
                    break;
                }

                //recalculate numAttributes due to errors in not dividing perfectly
                numPerAttribute = leftToFetch / (attributes.Count - index);

                //number to fetch, performance increase measure
                numToFetchPer = (int)((double)numPerAttribute * 1.3);

                //query attribute (only get infomration needed, so change variable type in list
                //tempUsers = QueryFriendUserAttribute(attributes[index], location, model.radius, attributeSkipCount[attributes[index].attribute], numPerAttribute);

                var queryUsers = attributes[index].users
                .Select(c => new
                {
                    description = c.description,
                    shown = c.shown,
                    user = c.user,
                    distance = c.location.Distance(_searchLocation) * 0.000621371, //0.000621371 is the approx miles per degree,
                    location = c.location,
                })
                .Where(c => c.shown && c.distance < _radius && c.user.age >= _minimumAge && c.user.age <= _maximumAge && (_gender == null || _gender == "" || c.user.gender == _gender))
                .Skip(attributeSkipCount[attributes[index].name])
                .Take(numToFetchPer)
                .ToList();

                //load each user from query into users
                foreach (var userInfo in queryUsers)
                {
                    if (userInfo.user.Id != _friendUser.ApplicationUserId && !blockedIDs.Contains(userInfo.user.Id))
                    {
                        tempUsers.Add(new FriendUserSearchModel(userInfo.user) { location = userInfo.location, description = userInfo.description });
                    }
                }
                

                //record amount queries for in dictionary
                attributeSkipCount[attributes[index].name] += numToFetchPer;

                //decrease the amount we need to fetch
                leftToFetch -= tempUsers.Count;

                //if we got less than we asked for
                //means that attribute is out of searchable users
                if (tempUsers.Count < numToFetchPer)
                {
                    //remove attribute from list
                    attributes.Remove(attributes[index]);

                    //move index back one because we remove an item in a list that is before it (where it was at)
                    index--;
                }

                //NOTE: we check count before checking for duplicates because
                //we only want to remove attributes that had no users left, 
                //not one's that returned duplicates

                //add to list of users (also removes duplicates due to nature of hashset)
                foreach (FriendUserSearchModel tempFriendUser in tempUsers)
                {
                    //if we are out of the number of 

                    //add user to hashset, if it was not successfuly added (because it was a duplicate
                    if (!usersHash.Add(tempFriendUser))
                    {
                        //increase left to fetch to account for duplicate user
                        leftToFetch++;
                    }
                    else
                    {
                        //if we are now on the page
                        if (users.Count >= ((_pageNumber - 1) * _pageSize))
                        {
                            users.Add(tempFriendUser);
                        }
                    }
                }

                //increase the index to query the next attribute
                index++;
            }
        }

        return users.ToList();
    }


    /// <summary>
    /// execute query and algorithm to search for users with defined properties
    /// </summary>
    /// <param name="_friendUser">current friend user</param>
    /// <param name="_context">db context</param>
    /// <param name="_searchLocation">search location</param>
    /// <param name="_radius">radius of search</param>
    /// <param name="_minimumAge">minimum age of users</param>
    /// <param name="_maximumAge">maximum age of users</param>
    /// <param name="_gender">specified gender of users</param>
    /// <param name="_attributes">list of specific attributes</param>
    /// <returns></returns>
    public static List<Point> searchForFriendUsersMap(FriendUser _friendUser, ApplicationDbContext _context, Point _searchLocation, double _radius, int _minimumAge, int _maximumAge, string _gender, List<string> _attributes)
    {
        //if they set it to the max age (100), set age to insanly high number to include 100+ old people
        if (_maximumAge == 100)
        {
            _maximumAge = 1000;
        }

        //get search limit
        int searchLimit = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<int>("MAP_LOAD_LIMIT");

        //hashset of users from the queries, using hashset to easily remove duplicates later
        List<Point> users = new List<Point>();

        //list of attributes
        List<FriendUserAttribute> attributes;

        //if they did not provide their own attributes
        if (_attributes == null || _attributes.Count == 0)
        {
            //get their attributes
            //attributes = _friendUser.attributes.ToList();

            //do a blank global search
            attributes = new List<FriendUserAttribute>();
        }
        else
        {
            attributes = new List<FriendUserAttribute>();

            FriendUserAttribute tempAttribute;

            foreach (string attributeName in _attributes)
            {
                //attempt to find the attribute
                tempAttribute = _context.FriendUserAttributes.Find(attributeName);

                //if it was found
                if (tempAttribute != null)
                {
                    //add it to the list
                    attributes.Add(tempAttribute);
                }
            }

            //we still have no attribues, return no results
            if (attributes.Count == 0)
            {
                return new List<Point>();
            }
        }

        //performance increase ==> ask for more than numattribites ratio because we know some will be duplicates, allow for some overflow, allow for it to add more than requested


        //get id's of users you are blocked to
        HashSet<string> blockedIDs = _context.FriendUserFriendUserBlocks.Where(b => b.friendUserFirstId == _friendUser.ApplicationUserId).Select(b => b.friendUserLastId).ToHashSet();

        //if no attributes are used
        if (attributes.Count == 0)
        {
            //do a mass search
            var tempUsers = _context.FriendUsers
                .Select(c => new
                {
                    location = c.location,
                    shown = c.shown,
                    age = c.user.age,
                    gender = c.user.gender,
                    Id = c.user.Id,
                    distance = c.location.Distance(_searchLocation) * 0.000621371, //0.000621371 is the approx miles per degree,
                })
                .Where(c => c.shown && c.distance < _radius && c.age >= _minimumAge && c.age <= _maximumAge && (_gender == null || _gender == "" || c.gender == _gender))
                .Take(searchLimit)
                .ToList();

            //load each user from query into users
            foreach (var userInfo in tempUsers)
            {
                if (userInfo.Id != _friendUser.ApplicationUserId && !blockedIDs.Contains(userInfo.Id))
                {
                    users.Add(userInfo.location);
                }
            }
        }
        else
        {
            HashSet<string> userIdsHash = new HashSet<string>();

            for (int index = 0; index < attributes.Count; index++)
            {
                var tempUsers = attributes[index].users
                    .Select(c => new
                    {
                        location = c.location,
                        shown = c.shown,
                        gender = c.user.gender,
                        age = c.user.age,
                        Id = c.user.Id,
                        distance = c.location.Distance(_searchLocation) * 0.000621371, //0.000621371 is the approx miles per degree,
                    })
                    .Where(c => c.shown && c.distance < _radius && c.age >= _minimumAge && c.age <= _maximumAge && (_gender == null || _gender == "" || c.gender == _gender))
                    .Take(searchLimit / attributes.Count)
                    .ToList();

                //load each user from query into users
                foreach (var userInfo in tempUsers)
                {
                    if (userInfo.Id != _friendUser.ApplicationUserId && !blockedIDs.Contains(userInfo.Id) && !userIdsHash.Add(userInfo.Id))
                    {
                        users.Add(userInfo.location);
                    }
                }
            }
        }

        return users.ToList();
    }

    //search globally for users, groups, and activities all at once
    //public static List<> searchFor
    public static List<FriendUserSearchModel> searchForFriendGlobalsByString(ICollection<FriendUser> users, string searchString, int skip, int take)
    {
        //take 1/3 of take from people, acitivty, and skip. skip 1/3 as well from previous 

        //for titles and not names
        //apply usual distance search
        //then split each title by space and search for it based on split searchstring
        //then apply search for it's preferences

        //either searching through icollection or dbset
        return null;
    }


    /// <summary>
    /// search icollection of friend users based on search string
    /// </summary>
    /// <param name="users"></param>
    /// <param name="searchString"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    /*public static List<FriendUserSearchModel> searchForFriendUsersByString(ICollection<FriendUser> users, string searchString, int skip, int take)
    {
        //split the search string by space
        string[] splitSearchString = searchString.Split(' ');

        //list of users from search result
        List<FriendUserSearchModel> searchedUsers;

        if (splitSearchString.Length == 0)
        {
            searchedUsers = new List<FriendUserSearchModel>();
        }
        //if only one
        else if (splitSearchString.Length == 1) {
            //get first and last name
            string name = splitSearchString[0];

            //search for it being either first or last name
            searchedUsers = users
                .Select(u => new FriendUserSearchModel(u.user) { location = u.location })
                .Where(u => (u.user.name == name))
                .Skip(skip)
                .Take(take)
                .ToList();
        }
        else if (splitSearchString.Length >= 2)
        {
            //get firstname
            string firstName = splitSearchString[0];

            //combine rest of seperated string with space for last name
            string lastName = "";

            foreach (string s in splitSearchString) 
            {
                lastName += s + " ";
            }

            //remove the last extra space
            lastName = lastName.Trim();

            //search for first and the rest of the strings being the last name space seperated
            searchedUsers = users
                .Select(u => new FriendUserSearchModel(u.user) { location = u.location })
                .Where(u => (u.user.firstName == firstName && u.user.lastName == lastName))
                .Skip(skip)
                .Take(take)
                .ToList();
        }
        else
        {
            searchedUsers = new List<FriendUserSearchModel>();
        }

        //remove blocked and users that are yourself

        //either searching through icollection or dbset
        return searchedUsers;
    }*/

    /// <summary>
    /// get age in years based on their birthdate
    /// </summary>
    /// <param name="birthdate"></param>
    /// <returns>age in years</returns>
    public static int getUserAge(DateTime birthdate)
    {
        int yearsDiff = DateTime.Now.Year - birthdate.Year;

        //if month and day are less
        if (DateTime.Now.Month < birthdate.Month || (DateTime.Now.Month == birthdate.Month && DateTime.Now.Day < birthdate.Day))
        {
            yearsDiff -= 1;
        }

        return yearsDiff;
    }

    /// <summary>
    /// update all user ages
    /// </summary>
    /// <param name="_context"></param>
    /// <param name="_userManager"></param>
    public static void updateAges(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager)
    {
        //temp variable
        int userAge;

        //for each user in the system
        foreach (var userInfo in _userManager.Users.Select(u => new { u.Id, u.birthdate, u.age }))
        {
            //calculate the age of the user
            userAge = getUserAge(userInfo.birthdate);

            //if the age is now greater
            if (userAge > userInfo.age)
            {
                //update user age
                _userManager.FindByIdAsync(userInfo.Id).Result.age = userAge;
            }
        }

        IdentityModule.SafelySaveChanges(_context);
    }

    /// <summary>
    /// gets computer version of attribute
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string getValidAttributeName(string name)
    {
        //remove capital letters
        name = name.ToLower();

        //remove special characters
        name = Regex.Replace(name, "[^a-z0-9 ]+", "");

        return name;
    }

    /// <summary>
    /// adds attribute to activity.
    /// assumes attribute is not already in activity.
    /// assumes attribute name is valid (already called getValidAttributeName()).
    /// does not call save changes
    /// </summary>
    /// <param name="attribute"></param>
    public static void addFriendActivityAttribute(ApplicationDbContext context, FriendActivity activity, string attributeName)
    {
        //attempt to get attribute
        FriendActivityAttribute attribute = context.FriendActivityAttributes.FirstOrDefault(a => a.name == attributeName);

        //if it does not exist
        if (attribute == null)
        {
            //create it
            attribute = new FriendActivityAttribute() { name = attributeName };
        }

        //add friend activity to it
        attribute.activities.Add(activity);

        //add attribute to friend activity
        activity.attributes.Add(attribute);
    }

    //add friend user point
    public static string addFriendActivityPoint(ApplicationDbContext context, FriendActivity friendActivity, string caption, byte index)
    {
        //create friend user point
        FriendActivityPoint point = new FriendActivityPoint(index);

        //add description 
        point.caption = caption;

        //add to friend user
        friendActivity.points.Add(point);

        //add friend activity link
        point.friendActivity = friendActivity;

        //add to context
        context.Add(point);

        //save changes
        IdentityModule.SafelySaveChanges(context);

        //return id
        return (point.id);
    }

    public static void removeFriendActivityPoint(ApplicationDbContext context, FriendActivity friendActivity, FriendActivityPoint point)
    {
        friendActivity.points.Remove(point);

        context.Remove(point);
    }

    /// <summary>
    /// removes attribute from friend user
    /// assumes attribute is already in friend user
    /// </summary>
    /// <param name="context"></param>
    /// <param name="activity"></param>
    /// <param name="attributeName"></param>
    public static void removeFriendActivityAttribute(ApplicationDbContext context, FriendActivity activity, FriendActivityAttribute attribute)
    {
        //add friend activity to it
        attribute.activities.Remove(activity);

        //add attribute to friend activity
        activity.attributes.Remove(attribute);
    }

    /// <summary>
    /// adds attribute to friend user
    /// assumes attribute is not already in friend user.
    /// assumes attribute name is valid (already called getValidAttributeName()).
    /// does not call save changes
    /// </summary>
    /// <param name="attribute"></param>
    public static void addFriendUserAttribute(ApplicationDbContext context, FriendUser friendUser, string attributeName)
    {
        //attempt to get attribute
        FriendUserAttribute attribute = context.FriendUserAttributes.FirstOrDefault(a => a.name == attributeName);

        //if it does not exist
        if (attribute == null)
        {
            //create it
            attribute = new FriendUserAttribute() { name = attributeName };
        }

        //add friend user to it
        attribute.users.Add(friendUser);

        //add attribute to friend friend user
        friendUser.attributes.Add(attribute);
    }
    public static void removeFriendUserAttribute(ApplicationDbContext context, FriendUser friendUser, FriendUserAttribute attribute)
    {
        //add friend activity to it
        attribute.users.Remove(friendUser);

        //add attribute to friend activity
        friendUser.attributes.Remove(attribute);
    }

    //add friend user point
    public static string addFriendUserPoint(ApplicationDbContext context, FriendUser friendUser, string caption, byte index)
    {
        //create friend user point
        FriendUserPoint point = new FriendUserPoint(index);

        //add description 
        point.caption = caption;

        //add to friend user
        friendUser.points.Add(point);

        //add friend user link
        point.friendUser = friendUser;

        //add to context
        context.Add(point);

        //save changes
        IdentityModule.SafelySaveChanges(context);

        //return id
        return (point.id);
    }

    public static void removeFriendUserPoint(ApplicationDbContext context, FriendUser friendUser, FriendUserPoint point)
    {
        friendUser.points.Remove(point);

        context.Remove(point);
    }

    /// <summary>
    /// execute query and algorithm to search for activities with defined properties
    /// </summary>
    /// <param name="_user">current user</param>
    /// <param name="_friendUser">current friend user</param>
    /// <param name="_context">db context</param>
    /// <param name="_searchLocation">search location</param>
    /// <param name="_radius">search radius</param>
    /// <param name="_pageSize">size of page</param>
    /// <param name="_pageNumber">page number, starts at 0</param>
    /// <param name="_attributes">list of specific attributes</param>
    /// <param name="medium">medium</param>
    /// <returns></returns>
    public static List<FriendActivitySearchModel> searchForFriendActivities(ApplicationUser _user, FriendUser _friendUser, ApplicationDbContext _context, Point _searchLocation, double _radius, int _pageSize, int _pageNumber, List<string> _attributes, string medium)
    {
        //if the radius is zero, return no activities (this avoids divide by 0 error
        if (_radius == 0.0)
        {
            return new List<FriendActivitySearchModel>();
        }

        //get list of activity ids the current friend user is in
        HashSet<string> currentFriendActivityIds = new HashSet<string>();

        foreach (FriendActivity friendActivity in _friendUser.createdActivities)
        {
            currentFriendActivityIds.Add(friendActivity.id);
        }

        foreach (FriendActivity friendActivity in _friendUser.participatingActivities)
        {
            currentFriendActivityIds.Add(friendActivity.id);
        }

        //hashset of users from the queries, using hashset to easily remove duplicates later
        List<FriendActivitySearchModel> activities = new List<FriendActivitySearchModel>();

        //list of attributes
        List<FriendActivityAttribute> attributes = new List<FriendActivityAttribute>();

        //if they did not provide their own attributes
        if (_attributes == null || _attributes.Count == 0)
        {
            /*//get friend user attributes from friend user
            List<FriendUserAttribute> friendUserAttributes = _friendUser.attributes.ToList();

            //found friend activity attribute
            FriendActivityAttribute foundFriendActivityAttribute;

            //for each friend user attribute
            foreach(FriendUserAttribute friendUserAttribute in friendUserAttributes)
            {
                //get its corresponding friend activity attribute
                foundFriendActivityAttribute = _context.FriendActivityAttributes.Find(friendUserAttribute.name);

                if (foundFriendActivityAttribute != null)
                {
                    attributes.Add(foundFriendActivityAttribute);
                }
            }*/
        }
        else
        {
            attributes = new List<FriendActivityAttribute>();

            FriendActivityAttribute tempAttribute;

            foreach (string attributeName in _attributes)
            {
                //attempt to find the attribute
                tempAttribute = _context.FriendActivityAttributes.Find(attributeName);

                //if it was found
                if (tempAttribute != null)
                {
                    //add it to the list
                    attributes.Add(tempAttribute);
                }
            }

            //we still have no attribues, return no results
            if (attributes.Count == 0)
            {
                return new List<FriendActivitySearchModel>();
            }
        }

        //performance increase ==> ask for more than numattribites ratio because we know some will be duplicates, allow for some overflow, allow for it to add more than requested

        //get id's of activities you are blocked to
        HashSet<string> blockedIDs = _context.FriendActivityFriendUserBlocks.Where(b => b.friendUserId == _friendUser.ApplicationUserId).Select(b => b.friendActivityId).ToHashSet();

        //make sure page number is greater than 0
        if (_pageNumber < 1)
        {
            return new List<FriendActivitySearchModel>();
        }

        //if no attributes are used
        if (attributes.Count == 0)
        {
            //do a mass search

            //eagarly execute the query and get the list, then converts the got data into a lightweight object
            var tempActivities = _context.FriendActivities.Select(c => new
            {
                Id = c.id,
                isActive = c.isActive,
                invitationMethod = c.invitationMethod,
                searchRadius = c.searchRadius,
                minimumAge = c.minimumage,
                maximumAge = c.maximumAge,
                gender = c.gender,
                is_physical = c.isPhysical,
                distance = c.searchLocation.Distance(_searchLocation) * 0.000621371,
                targetLocation = c.targetLocation,
                dateTime = c.dateTime,
                name = c.name,
            })
                .Where(c => c.distance < _radius && c.distance < c.searchRadius && c.isActive && c.invitationMethod != JOIN_INVITATION_METHODS.INVITE_ONLY && _user.age >= c.minimumAge && _user.age <= c.maximumAge && (c.gender == "all" || _user.gender == c.gender) && (medium == "" || (medium == "physical" && c.is_physical) || (medium == "virtual" && !c.is_physical)))
                .Skip(_pageSize * (_pageNumber - 1))
                .Take(_pageSize)
                .ToList();

            /*
             var tempActivities = _context.FriendActivities.Select(c => new
            {
                Id = c.id,
                description = c.description,
                isActive = c.isActive,
                invitationMethod = c.invitationMethod,
                searchRadius = c.searchRadius,
                minimumAge = c.minimumage,
                maximumAge = c.maximumAge,
                gender = c.gender,
                distance = c.searchLocation.Distance(_searchLocation) * 0.000621371,
                targetLocation = c.targetLocation,
                name = c.name,
            })
                .Where(c => c.distance < _radius && c.distance < c.searchRadius && c.isActive && c.invitationMethod != JOIN_INVITATION_METHODS.INVITE_ONLY && _user.age >= c.minimumAge && _user.age <= c.maximumAge && (c.gender == "all" || _user.gender == c.gender))
                .Skip(_pageSize * (_pageNumber - 1))
                .Take(_pageSize)
                .ToList();*/

            //load each activity from query into users
            foreach (var activityInfo in tempActivities)
            {
                if (!currentFriendActivityIds.Contains(activityInfo.Id) && !blockedIDs.Contains(activityInfo.Id))
                {
                    activities.Add(new FriendActivitySearchModel(activityInfo.Id) { location = activityInfo.targetLocation, name = activityInfo.name, dateTime = activityInfo.dateTime });
                }
            }
        }
        else
        {
            //we are doing a search involving attributes
            //to keep track of how many users had been grabed for each attribute
            //in order to know how many to skip in the case we call it again
            Dictionary<string, int> attributeSkipCount = new Dictionary<string, int>();

            //for seeing if there is a duplicate
            HashSet<FriendActivitySearchModel> activitiesHash = new HashSet<FriendActivitySearchModel>();

            //add attributes to attributeSkipCount dictionary
            foreach (FriendActivityAttribute friendUserAttribute in attributes)
            {
                attributeSkipCount.Add(friendUserAttribute.name, 0);
            }

            //number of activities to fetch total
            int numToFetch = _pageSize * _pageNumber;

            //how many activities left to fetch
            int leftToFetch = numToFetch;

            //how many activities to query for per attribute
            int numPerAttribute = leftToFetch / attributes.Count;

            //number of results to fetch
            int numToFetchPer = (int)((double)numPerAttribute * 1.3);

            //which attribute we are querying
            int index = 0;

            //temp holder for activities from query
            List<FriendActivitySearchModel> tempActivities = new List<FriendActivitySearchModel>();

            while (leftToFetch > 0)
            {
                //clear temp list
                tempActivities.Clear();

                //if we reach the last attribute
                if (index == attributes.Count)
                {
                    //set index to 0
                    index = 0;
                }

                //if we have no attributes left
                if (attributes.Count == 0)
                {
                    //break out of the while loop
                    break;
                }

                //recalculate numAttributes due to errors in not dividing perfectly
                numPerAttribute = leftToFetch / (attributes.Count - index);

                //number to fetch, performance increase measure
                numToFetchPer = (int)((double)numPerAttribute * 1.3);

                //query attribute (only get infomration needed, so change variable type in list
                //tempUsers = QueryFriendUserAttribute(attributes[index], location, model.radius, attributeSkipCount[attributes[index].attribute], numPerAttribute);

                //query for results

                //eagarly execute the query and get the list, then converts the got data into a lightweight object
                var queryActivities = attributes[index].activities
                    .Select(c => new
                    {
                        Id = c.id,
                        isActive = c.isActive,
                        invitationMethod = c.invitationMethod,
                        searchRadius = c.searchRadius,
                        minimumAge = c.minimumage,
                        maximumAge = c.maximumAge,
                        gender = c.gender,
                        is_physical = c.isPhysical,
                        distance = c.searchLocation.Distance(_searchLocation) * 0.000621371,
                        targetLocation = c.targetLocation,
                        dateTime = c.dateTime,
                        name = c.name,
                    })
                    .Where(c => c.distance < _radius && c.distance < c.searchRadius && c.isActive && c.invitationMethod != JOIN_INVITATION_METHODS.INVITE_ONLY && _user.age >= c.minimumAge && _user.age <= c.maximumAge && (c.gender == "all" || _user.gender == c.gender) && (medium == "" || (medium == "physical" && c.is_physical) || (medium == "virtual" && !c.is_physical)))
                    .Skip(attributeSkipCount[attributes[index].name])
                    .Take(numToFetchPer)
                    .ToList();

                //load each activity from query into users
                foreach (var activityInfo in queryActivities)
                {
                    if (!currentFriendActivityIds.Contains(activityInfo.Id) && !blockedIDs.Contains(activityInfo.Id))
                    {
                        tempActivities.Add(new FriendActivitySearchModel(activityInfo.Id) { location = activityInfo.targetLocation, name = activityInfo.name, dateTime = activityInfo.dateTime });
                    }
                }

                //record amount queries for in dictionary
                attributeSkipCount[attributes[index].name] += numToFetchPer;

                //decrease the amount we need to fetch
                leftToFetch -= tempActivities.Count;

                //if we got less than we asked for
                //means that attribute is out of searchable users
                if (tempActivities.Count < numToFetchPer)
                {
                    //remove attribute from list
                    attributes.Remove(attributes[index]);

                    //move index back one because we remove an item in a list that is before it (where it was at)
                    index--;
                }

                //NOTE: we check count before checking for duplicates because
                //we only want to remove attributes that had no users left, 
                //not one's that returned duplicates

                //add to list of users (also removes duplicates due to nature of hashset)
                foreach (FriendActivitySearchModel tempFriendActivity in tempActivities)
                {
                    //if we are out of the number of 

                    //add user to hashset, if it was not successfuly added (because it was a duplicate
                    if (!activitiesHash.Add(tempFriendActivity))
                    {
                        //increase left to fetch to account for duplicate user
                        leftToFetch++;
                    }
                    else
                    {
                        //if we are now on the page
                        if (activities.Count >= ((_pageNumber - 1) * _pageSize))
                        {
                            activities.Add(tempFriendActivity);
                        }
                    }
                }

                //increase the index to query the next attribute
                index++;
            }
        }

        return activities.ToList();
    }



    /// <summary>
    /// execute query and algorithm to search for activities with defined properties
    /// </summary>
    /// <param name="_user">current user</param>
    /// <param name="_friendUser">current friend user</param>
    /// <param name="_context">db context</param>
    /// <param name="_searchLocation">search location</param>
    /// <param name="_radius">search radius</param>
    /// <param name="_attributes">list of specific attributes</param>
    /// <param name="medium">meidum</param>
    /// <returns></returns>
    public static List<FriendActivitySearchMapModel> searchForFriendActivitiesMap(ApplicationUser _user, FriendUser _friendUser, ApplicationDbContext _context, Point _searchLocation, double _radius, List<string> _attributes, string medium)
    {
        //if the radius is zero, return no activities (this avoids divide by 0 error
        if (_radius == 0.0)
        {
            return new List<FriendActivitySearchMapModel>();
        }

        int searchLimit = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<int>("MAP_LOAD_LIMIT");

        //get list of activity ids the current friend user is in
        HashSet<string> currentFriendActivityIds = new HashSet<string>();

        foreach (FriendActivity friendActivity in _friendUser.createdActivities)
        {
            currentFriendActivityIds.Add(friendActivity.id);
        }

        //hashset of users from the queries, using hashset to easily remove duplicates later
        List<FriendActivitySearchMapModel> activities = new List<FriendActivitySearchMapModel>();

        //list of attributes
        List<FriendActivityAttribute> attributes = new List<FriendActivityAttribute>();

        //if they did not provide their own attributes
        if (_attributes == null || _attributes.Count == 0)
        {
            /*//get friend user attributes from friend user
            List<FriendUserAttribute> friendUserAttributes = _friendUser.attributes.ToList();

            //found friend activity attribute
            FriendActivityAttribute foundFriendActivityAttribute;

            //for each friend user attribute
            foreach(FriendUserAttribute friendUserAttribute in friendUserAttributes)
            {
                //get its corresponding friend activity attribute
                foundFriendActivityAttribute = _context.FriendActivityAttributes.Find(friendUserAttribute.name);

                if (foundFriendActivityAttribute != null)
                {
                    attributes.Add(foundFriendActivityAttribute);
                }
            }*/
        }
        else
        {
            attributes = new List<FriendActivityAttribute>();

            FriendActivityAttribute tempAttribute;

            foreach (string attributeName in _attributes)
            {
                //attempt to find the attribute
                tempAttribute = _context.FriendActivityAttributes.Find(attributeName);

                //if it was found
                if (tempAttribute != null)
                {
                    //add it to the list
                    attributes.Add(tempAttribute);
                }
            }

            //we still have no attribues, return no results
            if (attributes.Count == 0)
            {
                return new List<FriendActivitySearchMapModel>();
            }
        }

        //performance increase ==> ask for more than numattribites ratio because we know some will be duplicates, allow for some overflow, allow for it to add more than requested

        //get id's of activities you are blocked to
        HashSet<string> blockedIDs = _context.FriendActivityFriendUserBlocks.Where(b => b.friendUserId == _friendUser.ApplicationUserId).Select(b => b.friendActivityId).ToHashSet();

        //if no attributes are used
        if (attributes.Count == 0)
        {
            //do a mass search

            //eagarly execute the query and get the list, then converts the got data into a lightweight object
            var tempActivities = _context.FriendActivities.Select(c => new
            {
                Id = c.id,
                description = c.description,
                isActive = c.isActive,
                invitationMethod = c.invitationMethod,
                searchRadius = c.searchRadius,
                minimumAge = c.minimumage,
                maximumAge = c.maximumAge,
                gender = c.gender,
                is_physical = c.isPhysical,
                distance = c.searchLocation.Distance(_searchLocation) * 0.000621371,
                targetLocation = c.targetLocation,
                name = c.name,
            })
                .Where(c => c.distance < _radius && c.distance < c.searchRadius && c.isActive && c.invitationMethod != JOIN_INVITATION_METHODS.INVITE_ONLY && _user.age >= c.minimumAge && _user.age <= c.maximumAge && (c.gender == "all" || _user.gender == c.gender) && ((medium == "physical" || medium == "") && c.is_physical))
                .Take(searchLimit)
                .ToList();

            //load each activity from query into users
            foreach (var activityInfo in tempActivities)
            {
                if (!currentFriendActivityIds.Contains(activityInfo.Id) && !blockedIDs.Contains(activityInfo.Id))
                {
                    activities.Add(new FriendActivitySearchMapModel(activityInfo.Id) { location = activityInfo.targetLocation, name = activityInfo.name });
                }
            }
        }
        else
        {
            HashSet<string> activityIdsHash = new HashSet<string>();

            for (int index = 0; index < attributes.Count; index++)
            {
                //eagarly execute the query and get the list, then converts the got data into a lightweight object
                var queryActivities = attributes[index].activities
                    .Select(c => new
                    {
                        Id = c.id,
                        description = c.description,
                        isActive = c.isActive,
                        invitationMethod = c.invitationMethod,
                        searchRadius = c.searchRadius,
                        minimumAge = c.minimumage,
                        maximumAge = c.maximumAge,
                        gender = c.gender,
                        is_physical = c.isPhysical,
                        distance = c.searchLocation.Distance(_searchLocation) * 0.000621371,
                        targetLocation = c.targetLocation,
                        name = c.name,
                    })
                    .Where(c => c.distance < _radius && c.distance < c.searchRadius && c.isActive && c.invitationMethod != JOIN_INVITATION_METHODS.INVITE_ONLY && _user.age >= c.minimumAge && _user.age <= c.maximumAge && (c.gender == "all" || _user.gender == c.gender) && ((medium == "physical" || medium == "") && c.is_physical))
                    .Take(searchLimit / attributes.Count)
                    .ToList();

                //load each activity from query into users
                foreach (var activityInfo in queryActivities)
                {
                    if (!currentFriendActivityIds.Contains(activityInfo.Id) && !blockedIDs.Contains(activityInfo.Id) && !activityIdsHash.Add(activityInfo.Id))
                    {
                        activities.Add(new FriendActivitySearchMapModel(activityInfo.Id) { location = activityInfo.targetLocation, name = activityInfo.name });
                    }
                }
            }
        }

        return activities.ToList();
    }

    /// <summary>
    /// remove friend user from participating activity
    /// </summary>
    /// <param name="context"></param>
    /// <param name="friendUser"></param>
    /// <param name="friendActivity"></param>
    /// <returns></returns>
    public static Pair<bool, string> removeFriendUserFromParticipatingFriendActivity(ApplicationDbContext context, FriendUser friendUser, FriendActivity friendActivity)
    {
        //remove from activity
        friendActivity.participants.Remove(friendUser);

        friendActivity.dynamicValues.numParticipants--;

        //check if it can become active if it is currently not active
        if (!friendActivity.isActive)
        {
            //get dynamic values, check active conditions
            UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
        }

        //remove from participating activies list
        friendUser.participatingActivities.Remove(friendActivity);

        IdentityModule.SafelySaveChanges(context);

        return new Pair<bool, string>(true, "");
    }

    //remove friend user from created activity
    public static Pair<bool, string> removeFriendUserFromCurrentFriendActivity(ApplicationDbContext context, FriendUser friendUser, FriendActivity friendActivity)
    {
        friendActivity.admins.Remove(friendUser);

        friendActivity.dynamicValues.numAdmins--;

        //if user is only member in activity
        if (friendActivity.dynamicValues.numAdmins == 0)
        {
            //get rid of the activity

            //remove participants and this activity from their participating activities

            //get participants
            List<FriendUser> participants = friendActivity.participants.ToList();

            //remove activity from participants list
            foreach (FriendUser p in participants)
            {
                //remove activity from their list of activities they are in
                p.participatingActivities.Remove(friendActivity);
            }

            //clear participants list
            friendActivity.participants.Clear();

            //remove points
            foreach (FriendActivityPoint point in friendActivity.points.ToList())
            {
                removeFriendActivityPoint(context, friendActivity, point);
            }

            //remove from attributes
            foreach (FriendActivityAttribute attribute in friendActivity.attributes.ToList())
            {
                attribute.activities.Remove(friendActivity);
            }

            //clear all announcements
            List<FriendActivityAnnouncement> friendActivityAnnouncements = friendActivity.announcements.ToList();
            context.FriendActivityAnnouncements.RemoveRange(friendActivityAnnouncements);

            //clear invitations

            //get all invitations
            List<FriendUserToFriendActivityInvitation> friendUserToFriendActivityInvitations = friendActivity.receivedFriendUserToFriendActivityInvitations.ToList();
            List<FriendActivityToFriendUserInvitation> friendActivityToFriendUserInvitations = friendActivity.givenFriendActivityToFriendUserInvitations.ToList(); 
            List<FriendActivityToFriendActivityInvitation> friendActivityToFriendActivityInvitations = friendActivity.receivedFriendActivityToFriendActivityInvitations.ToList();
            friendActivityToFriendActivityInvitations.AddRange(friendActivity.givenFriendActivityToFriendActivityInvitations.ToList());

            //get list of all invitation bases
            List<InvitationBase> invitationBases = new List<InvitationBase>();
            invitationBases.AddRange(friendUserToFriendActivityInvitations);
            invitationBases.AddRange(friendActivityToFriendUserInvitations);
            invitationBases.AddRange(friendActivityToFriendActivityInvitations);

            //type of the invitation
            Type invitationType;

            //result
            Pair<bool, string> result;

            foreach(InvitationBase invitation in invitationBases)
            {
                if (invitation.descriminator == "FriendUserToFriendActivityInvitation")
                {
                    result = InvitationModule.ResolveInvitation(context, friendUser.user, invitation);
                }
                else if (invitation.descriminator == "FriendActivityToFriendUserInvitation")
                {
                    result = InvitationModule.ResolveInvitation(context, friendUser.user, invitation);
                }
                else if (invitation.descriminator == "FriendActivityToFriendActivityInvitation")
                {
                    result = InvitationModule.ResolveInvitation(context, friendUser.user, invitation);
                }
                else
                {
                    return new Pair<bool, string>(false, "Unknown invitation type");
                }

                if (!result.First)
                {
                    return result;
                }
            }

            //remove blocks
            foreach (FriendActivityFriendUserBlock block in context.FriendActivityFriendUserBlocks.Where(b => (b.friendActivityId == friendActivity.id)).ToList())
            {
                context.Remove(block);
            }

            //remove corresponding activity dynamics value entry
            context.FriendActivitiesDynamicValues.Remove(friendActivity.dynamicValues);

            //remove activity from context
            context.FriendActivities.Remove(friendActivity);
        } 

        //remove activity from current user's created activities list
        friendUser.createdActivities.Remove(friendActivity);

        IdentityModule.SafelySaveChanges(context);

        return new Pair<bool, string>(true, "");
    }

    public static string ParseToReadDateTime(DateTime dateTime)
    {
        string date = "";

        //get date string

        DayOfWeek currentDayOfWeek = DateTime.Now.DayOfWeek;

        DayOfWeek dateTimeDayOfWeek = dateTime.DayOfWeek;

        //if the datetime day is farther in the week than the current day and they are both in the same week
        if (((int)dateTimeDayOfWeek) - ((int)currentDayOfWeek) > 0 && (dateTime - DateTime.Now).Days / 7 == 0)
        {
            date = dateTimeDayOfWeek.ToString();
        }   
        else
        {
            date = dateTime.ToString("M/d/yyyy");
        }

        //get time string

        string time = dateTime.ToString("h:mm t") + "M";

        return date + " at " + time;
    }

    public static Pair<bool, string> leaveActivityAsParticipant(ApplicationDbContext context, FriendUser friendUser, FriendActivity friendActivity)
    {
        //add friend user as participant to activity
        friendActivity.participants.Remove(friendUser);

        friendActivity.dynamicValues.numParticipants--;

        //check if it can become active if it is currently not active
        if (!friendActivity.isActive)
        {
            //get dynamic values, check active conditions
            UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
        }

        //remove activity from current user's created activities list
        friendUser.participatingActivities.Remove(friendActivity);

        IdentityModule.SafelySaveChanges(context);

        return new Pair<bool, string>(true, "");
    }

    /// <summary>
    /// adds friend activity friend user block
    /// </summary>
    /// <param name="context"></param>
    /// <param name="friendActivity"></param>
    /// <param name="friendUser"></param>
    /// <returns></returns>
    public static Pair<bool, string> FriendActivityFriendUserBlock(ApplicationDbContext context, FriendActivity friendActivity, FriendUser friendUser)
    {
        FriendActivityFriendUserBlock block = new FriendActivityFriendUserBlock();

        //check if block already exists
        List<FriendActivityFriendUserBlock> blocks = context.FriendActivityFriendUserBlocks.Where(b => (b.friendActivityId == friendActivity.id && b.friendUserId == friendUser.ApplicationUserId)).ToList();

        if (blocks.Count != 0)
        {
            return new Pair<bool, string>(false, "Block already exists");
        }

        //add links
        block.friendActivity = friendActivity;
        block.friendUser = friendUser;

        //add to context
        context.Add(block);

        return new Pair<bool, string>(true, "");
    }

    /// <summary>
    /// adds friend user friend user block
    /// </summary>
    /// <param name="context"></param>
    /// <param name="friendUserFirst"></param>
    /// <param name="friendUserLast"></param>
    /// <returns></returns>
    public static Pair<bool, string> FriendUserFriendUserBlock(ApplicationDbContext context, FriendUser friendUserFirst, FriendUser friendUserLast)
    {

        //check if block already exists
        List<FriendUserFriendUserBlock> blocks = context.FriendUserFriendUserBlocks.Where(b => (b.friendUserFirstId == friendUserFirst.ApplicationUserId && b.friendUserLastId == friendUserLast.ApplicationUserId)).ToList();

        if (blocks.Count != 0)
        {
            return new Pair<bool, string>(false, "Block already exists");
        }

        if (friendUserFirst.ApplicationUserId == friendUserLast.ApplicationUserId)
        {
            return new Pair<bool, string>(false, "Cannot block yourself");
        }

        //make first block
        FriendUserFriendUserBlock blockFirst = new FriendUserFriendUserBlock();

        //add links
        blockFirst.friendUserFirst = friendUserFirst;
        blockFirst.friendUserLast = friendUserLast;

        //make second block
        FriendUserFriendUserBlock blockLast = new FriendUserFriendUserBlock();

        //add links
        blockLast.friendUserFirst = friendUserLast;
        blockLast.friendUserLast = friendUserFirst;

        //add to context
        context.Add(blockFirst);
        context.Add(blockLast);

        //check if user is participant in any of the activities you are admin in
        HashSet<string> firstCreatedActivityIds = friendUserFirst.createdActivities.Select(a => a.id).ToHashSet();
        HashSet<string> firstParticipatingActivityIds = friendUserFirst.participatingActivities.Select(a => a.id).ToHashSet();
        HashSet<string> lastCreatedActivityIds = friendUserLast.createdActivities.Select(a => a.id).ToHashSet();
        HashSet<string> lastParticipatingActivityIds = friendUserLast.participatingActivities.Select(a => a.id).ToHashSet();

        //friend activities to remove
        List<FriendActivity> friendActivitiesToRemove = new List<FriendActivity>();
        foreach (string firstActivityID in firstCreatedActivityIds)
        {
            if (lastParticipatingActivityIds.Contains(firstActivityID))
            {
                friendActivitiesToRemove.Add(context.FriendActivities.Find(firstActivityID));
            }
            
        }

        //for each activity to remove
        foreach(FriendActivity removeActivity in friendActivitiesToRemove)
        {
            //have the other user leave as a participant
            leaveActivityAsParticipant(context, friendUserLast, removeActivity);

            //create block
            FriendActivityFriendUserBlock block = new FriendActivityFriendUserBlock();

            //add links
            block.friendActivity = removeActivity;
            block.friendUser = friendUserLast;

            //add to context
            context.Add(block);
        }

        //clear friend activities to remove
        friendActivitiesToRemove.Clear();

        foreach (string firstActivityID in firstParticipatingActivityIds)
        {
            if (lastCreatedActivityIds.Contains(firstActivityID))
            {
                friendActivitiesToRemove.Add(context.FriendActivities.Find(firstActivityID));
            }
        }

        //for each activity to remove
        foreach (FriendActivity removeActivity in friendActivitiesToRemove)
        {
            //have the other user leave as a participant
            leaveActivityAsParticipant(context, friendUserFirst, removeActivity);

            //create block
            FriendActivityFriendUserBlock block = new FriendActivityFriendUserBlock();

            //add links
            block.friendActivity = removeActivity;
            block.friendUser = friendUserFirst;

            //add to context
            context.Add(block);
        }

        return new Pair<bool, string>(true, "");
    }

    public static Pair<bool, string> FriendUserFriendUserReport(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, FriendUser currentFriendUser, FriendUser otherFriendUser)
    {
        //get dynamic values of other friend user
        FriendUserDynamicValues dynamicValues = otherFriendUser.dynamicValues;

        //increase number of reports by 1
        dynamicValues.numberOfReports++;

        //get parameters
        double reportLimit = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<double>("REPORT_LIMIT");

        //if this excedds the allowed number of reports
        if (dynamicValues.numberOfReports >= reportLimit)
        {
            //remove user
            //remove friend, dating, and casual user
            UserModule.RemoveUser(context, otherFriendUser.user);

            //remove user from roles they are in
            userManager.RemoveFromRolesAsync(otherFriendUser.user, userManager.GetRolesAsync(otherFriendUser.user).Result);

            //remove from user manager
            userManager.DeleteAsync(otherFriendUser.user);

            //sign out user
            signInManager.SignOutAsync();

            //send email from no-reply
            //send email
            //get parameters
            string fromEmail = Startup.externalConfiguration.GetSection("EmailStrings").GetValue<string>("SupportEmail");
            string fromPassword = Startup.externalConfiguration.GetSection("EmailStrings").GetValue<string>("SupportPassword");
            string toEmail = otherFriendUser.user.Email;

            //create message
            MailMessage message = new MailMessage(fromEmail, toEmail);

            //set values
            message.Subject = "POP Your account has been removed";
            message.Body = "Your account has recived too many reports, and as such, has been deleted";

            //create email object
            SmtpClient client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            //set cridentials
            client.UseDefaultCredentials = true;
            client.Timeout = 5000;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                return new Pair<bool, string>(false, "Could not send email");
            }
        }

        return new Pair<bool, string>(true, "");
    }

    /// <summary>
    /// check and update conditions for friend activity -> isActivity
    /// does not save context
    /// </summary>
    /// <param name="friendActivity"></param>
    /// <param name="friendActivityDynamicValues"></param>
    public static void checkIsActiveConditionsFriendActivity(FriendActivity friendActivity, FriendActivityDynamicValues friendActivityDynamicValues = null)
    {
        //check if it can become active if it is currently not active
        
        if (!friendActivity.shown)
        {
            friendActivity.isActive = false;

            return;
        }

        //get dynamic values if it has not been provided
        if (friendActivityDynamicValues == null)
        {
            friendActivityDynamicValues = friendActivity.dynamicValues;
        }

        //check condition for pending invites
        if (friendActivityDynamicValues.pendingInvites > friendActivity.inviteCap)
        {
            friendActivity.isActive = false;

            return;
        }
        
        //if too many participants
        if (friendActivityDynamicValues.numParticipants > friendActivity.participantsCap)
        {
            friendActivity.isActive = false;

            return;
        }

        //if too old
        if (friendActivity.dateTime < DateTime.Now.AddDays(-1))
        {
            friendActivity.isActive = false;

            return;

        }

        //if all conditions have been met, is is active
        friendActivity.isActive = true;

        return;
    }

    /// <summary>
    /// check and update conditions for friend activity -> isActivity
    /// does not save context
    /// </summary>
    /// <param name="friendUser"></param>
    /// <param name="friendUserDynamicValues"></param>
    public static void checkIsActiveConditionsFriendUser(FriendUser friendUser, FriendUserDynamicValues friendUserDynamicValues = null)
    {
        //check if it can become active if it is currently not active

        if (!friendUser.shown)
        {
            friendUser.isActive = false;

            return;
        }

        //get dynamic values if it has not been provided
        if (friendUserDynamicValues == null)
        {
            friendUserDynamicValues = friendUser.dynamicValues;
        }

        //get parameters
        byte reportLimit = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<byte>("REPORT_LIMIT");

        //check condition for pending invites
        if (friendUserDynamicValues.numberOfReports <= reportLimit)
        {
            friendUser.isActive = false;

            return;
        }

        //if all conditions have been met, is is active
        friendUser.isActive = true;

        return;
    }

    /// <summary>
    /// for activities that are more than a day old, set them to not active
    /// </summary>
    /// <param name="context"></param>
    public static void setNotActiveForOldActivities(ApplicationDbContext context)
    {
        foreach(var friendActivityInfo in context.FriendActivities.Select(c => new {c.dateTime, c.id})) 
        {
            if (friendActivityInfo.dateTime < DateTime.Now.AddDays(-1))
            {
                //get friend activity
                FriendActivity friendActivity = context.FriendActivities.Find(friendActivityInfo.id);

                //change is active
                friendActivity.isActive = false;
            }
        }

        IdentityModule.SafelySaveChanges(context);
    }

    /// <summary>
    /// get the neat version for the date time
    /// ex. next monday as 9:15 am
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string getNeatDateTime(DateTime dateTime)
    {
        DateTime now = DateTime.Now;

        TimeSpan timeSpan = dateTime - now;

        string timeString = "";

        //was in the past
        if (timeSpan.Minutes < 0)
        {
            timeString += "Was ";

            //today
            if (now.Day == dateTime.Day)
            {
                timeString += "today at ";
            }
            else if (now.Day - 1 == dateTime.Day)
            {
                timeString += "yesterday at ";
            }
            else
            {
                timeString += dateTime.ToString("MMM d") + " at ";
            }
        }
        //in the future or now
        else
        {
            //today
            if (now.Day == dateTime.Day)
            {
                timeString += "Today at ";
            }
            //tomorrow
            else if (now.Day + 1 == dateTime.Day)
            {
                timeString += "Tomorrow at ";
            }
            else
            {
                //get day of week gap
                int dayOfWeekGap = 6 - ((int)now.DayOfWeek);

                if (timeSpan.Days <= 7 && dateTime.DayOfWeek > now.DayOfWeek)
                {
                    timeString += "This " + dateTime.DayOfWeek.ToString() + " at ";
                }
                else if (timeSpan.Days <= 7 + dayOfWeekGap)
                {
                    timeString += "Next " + dateTime.DayOfWeek.ToString() + " at ";
                }
                else
                {
                    timeString += dateTime.ToString("MMM d") + " at ";
                }
            }
        }

        //add time
        timeString += dateTime.ToString("h:mm tt");

        return timeString;
    }

    public static Pair<bool, string> sendFeedbackEmail(ApplicationUser user, string feedback)
    {
        //set email paramaters
        string fromEmail = Startup.externalConfiguration.GetSection("EmailStrings").GetValue<string>("SupportEmail");
        string fromPassword = Startup.externalConfiguration.GetSection("EmailStrings").GetValue<string>("SupportPassword");

        MailMessage message = new MailMessage(fromEmail, fromEmail);
        message.Subject = "Feedback from user " + user.UserName;
        message.Body = feedback;

        //create email object
        SmtpClient client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            UseDefaultCredentials = false,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(fromEmail, fromPassword),
            Timeout = 5000,
        };

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            return new Pair<bool, string>(false, "Could not send email");
        }

        return new Pair<bool, string>(true, "");
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    /// <param name="points">list of points </param>
    /// <param name="mapBottomLeftPoint">bottom left points of the map (longitude, latitidue)</param>
    /// <param name="mapWidth">width of the map (degrees)</param>
    /// <param name="mapHeight">height of the map (degrees)</param>
    /// <returns></returns>
    public static List<GroupPoint> groupPointsForMap(List<GeoCoordinate> points, Point mapBottomLeftPoint, double mapWidth, double mapHeight)
    {

    }*/

    //for lightweight handeling of user after query searches
    public class BasicQueryUser
    {
        /// <summary>
        /// application id of user
        /// </summary>
        public string applicationID;

        /// <summary>
        /// location of user
        /// </summary>
        public Point location;

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="_applicationID"></param>
        /// <param name="_location"></param>
        public BasicQueryUser(string _applicationID, Point _location)
        {
            this.applicationID = _applicationID;
            this.location = _location;
        }

        public override int GetHashCode()
        {
            return applicationID.GetHashCode();
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

            BasicQueryUser? other = obj as BasicQueryUser;

            if (other != null && other.applicationID == this.applicationID)
            {
                return true;
            }

            return false;
        }
    }

    public class GroupPoint
    {
        /// <summary>
        /// number of elements in group
        /// </summary>
        int count;

        /// <summary>
        /// location of group
        /// </summary>
        Point location;
    }

    //temp classes
    private class FriendUserQueryTemp
    {
        //include full last name (let phone cut off to initial)
        //include main profile image link

        public string username { get; set; }

        public string name { get; set; }

        public Point location { get; set; }

        public int age { get; set; }

        public string gender { get; set; }

        public FriendUserQueryTemp(FriendUser friendUser)
        {
            //get the application user
            ApplicationUser user = friendUser.user;

            //get the username
            this.username = user.UserName;

            //get the first name
            this.name = user.name;

            //get the location
            this.location = friendUser.location;

            //get age
            this.age = user.age;

            //get gender
            this.gender = user.gender;
        }

        public override int GetHashCode()
        {
            return username.GetHashCode();
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

            FriendUserQueryTemp other = obj as FriendUserQueryTemp;

            if (other.username == this.username)
            {
                return true;
            }

            return false;
        }
    }
}

/*CREATE FUNCTION dbo.CommentedPostCountForBlog(@lat_1 double, @lon_1 double, @lat_2 double, @lon_2 double)
RETURNS double
AS
BEGIN
    RETURN (
        7926.3812 * ASIN(SQRT(0.5 - COS((@lon_2 - @lon_1) * PI / 180) / 2 + COS(@lon_1 * PI / 180) * COS(@lon_2 * PI / 180) * (1 - COS((@lat_2 - @lat_1) * PI  / 180)) / 2))
    )
END*/