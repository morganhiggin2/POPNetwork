using Castle.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using POPNetwork.Controllers;
using POPNetwork.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static POPNetwork.Global.GlobalProperties;

namespace POPNetwork.Modules
{
    public class InvitationModule
    {
        /// <summary>
        /// resolves invitation as a user
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentUser"></param>
        /// <param name="invitationBase">invitation base of invitation, must already be of sub type for casting</param>
        /// <returns></returns>
        public static Pair<bool, string> ResolveInvitation(ApplicationDbContext context, ApplicationUser currentUser, InvitationBase invitationBase)
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
                    return new Pair<bool, string>(false, "Friend User for current user does not exist");
                }

                //ger friend user
                FriendUser friendUser = invitation.inviter;

                //get activity
                FriendActivity friendActivity = invitation.invitee;

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {
                        //remove invitation
                        context.Remove(invitation);

                        IdentityModule.SafelySaveChanges(context);

                        return new Pair<bool, string>(false, "Friend User is invalid and Friend Activity does not exist");
                    }

                    //remove from friend user list
                    friendUser.givenFriendUserToFriendActivityInvitations.Remove(invitation);

                    //remove inviation 
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend Activity does not exist");
                }

                //check if current user is admin for this activity
                if (!currentFriendUser.createdActivities.Contains(friendActivity))
                {
                    return new Pair<bool, string>(false, "Friend User is not admin in Friend Activity");
                }

                if (friendUser == null)
                {
                    friendActivity.receivedFriendUserToFriendActivityInvitations.Remove(invitation);

                    //decrease dynamic count
                    friendActivity.dynamicValues.pendingInvites--;

                    //check if it can become active if it is currently not active
                    if (!friendActivity.isActive)
                    {
                        //get dynamic values, check active conditions
                        UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                    }

                    //remove invitation
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend User is invalid");
                }

                //add frienduser as participant in activity
                friendActivity.participants.Add(friendUser);

                friendActivity.dynamicValues.numParticipants++;

                //add activity to participating activities of friend user
                friendUser.participatingActivities.Add(friendActivity);

                //remove inviation from activities
                friendActivity.receivedFriendUserToFriendActivityInvitations.Remove(invitation);

                //remove from friend user list
                friendUser.givenFriendUserToFriendActivityInvitations.Remove(invitation);

                //decrease dynamic count
                friendActivity.dynamicValues.pendingInvites--;

                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                
                //remove invitation
                context.Remove(invitation);
            }
            //if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation) invitationBase;

                if (invitation == null)
                {
                    return new Pair<bool, string>(false, "Invitation of type was not found");
                }

                //get friend user
                FriendUser friendUser = currentUser.friendUser;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                //check if friend user is the one we are using
                if (invitation.inviteeId != currentUser.Id)
                {
                    return new Pair<bool, string>(false, "Friend User is not a recipient in this invitation");
                }

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {
                        //remove inviation 
                        context.Remove(invitation);

                        IdentityModule.SafelySaveChanges(context);

                        return new Pair<bool, string>(false, "Friend User is invalid and Friend Activity does not exist");
                    }

                    //remove from friend user list
                    friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);

                    //remove inviation 
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend Activity does not exist");
                }

                if (friendUser == null)
                {
                    friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                    //decrease dynamic count
                    friendActivity.dynamicValues.pendingInvites--;

                    //check if it can become active if it is currently not active
                    if (!friendActivity.isActive)
                    {
                        //get dynamic values, check active conditions
                        UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                    }

                    //remove invitation
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend User is invalid");
                }

                //add frienduser as participant in activity
                friendActivity.participants.Add(friendUser);

                friendActivity.dynamicValues.numParticipants++;

                //add activity to participating activities of friend user
                friendUser.participatingActivities.Add(friendActivity);

                //remove invitation from friend activity invitations
                friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                //remove invitation from friend user invitations
                friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);

                //decrease dynamic count
                friendActivity.dynamicValues.pendingInvites--;

                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                
                //remove invitation
                context.Remove(invitation);
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation) invitationBase;

                //get friend user
                FriendUser friendUser = currentUser.friendUser;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                //check if friend user is the one we are using
                if (invitation.inviteeId != currentUser.Id)
                {
                    return new Pair<bool, string>(false, "Friend User is not a recipient in this invitation");
                }

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {

                        //remove inviation 
                        context.Remove(invitation);

                        IdentityModule.SafelySaveChanges(context);

                        return new Pair<bool, string>(false, "Friend User is invalid and Friend Activity does not exist");
                    }

                    //remove from friend user list
                    friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);

                    //remove inviation 
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend Activity does not exist");
                }

                if (friendUser == null)
                {

                    friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                    //decrease dynamic count
                    friendActivity.dynamicValues.pendingInvites--;

                    //check if it can become active if it is currently not active
                    if (!friendActivity.isActive)
                    {
                        //get dynamic values, check active conditions
                        UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                    }

                    //remove invitation
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend User is invalid");
                }

                //add frienduser as participant in activity
                friendActivity.admins.Add(friendUser);

                friendActivity.dynamicValues.numAdmins++;

                //remove from participants
                friendActivity.participants.Remove(friendUser);

                friendActivity.dynamicValues.numParticipants--;

                //add activity to participating activities of friend user
                friendUser.createdActivities.Add(friendActivity);

                //remove from participating activities
                friendUser.participatingActivities.Remove(friendActivity);

                //remove inviation from activities
                friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                //remove from friend user list
                friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);
                
                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                
                //decrease dynamic count
                friendActivity.dynamicValues.pendingInvites--;

                //remove invitation
                context.Remove(invitation);
            }
            else
            {
                return new Pair<bool, string>(false, "Invalid invitation type");
            }

            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");
        }

        /// <summary>
        /// removes invitation, regardless of missing values or underlying problems
        /// a safe way to delete a invitation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="invitationBase">invitation base of invitation, must already be of sub type for casting</param>
        /// <returns></returns>
        public static Pair<bool, string> RemoveInvitation(ApplicationDbContext context, InvitationBase invitationBase, bool permissionRequired, ApplicationUser currentUser = null)
        {
            //if invitation was participant requesting to join activity
            if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get given inviation
                FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation) invitationBase;

                if (invitation == null)
                {
                    return new Pair<bool, string>(false, "Invitation of type was not found");
                }

                //get current friendUser
                FriendUser currentFriendUser = currentUser.friendUser;

                if (currentFriendUser == null)
                {
                    return new Pair<bool, string>(false, "Friend User for current user does not exist");
                }

                //ger friend user
                FriendUser friendUser = invitation.inviter;

                //get activity
                FriendActivity friendActivity = invitation.invitee;

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {
                        //remove invitation
                        context.Remove(invitation);

                        IdentityModule.SafelySaveChanges(context);

                        return new Pair<bool, string>(false, "Friend User is invalid and Friend Activity does not exist");
                    }

                    //remove from friend user list
                    friendUser.givenFriendUserToFriendActivityInvitations.Remove(invitation);

                    //remove inviation 
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend Activity does not exist");
                }

                if (friendUser == null)
                {
                    friendActivity.receivedFriendUserToFriendActivityInvitations.Remove(invitation);

                    //decrease dynamic count
                    friendActivity.dynamicValues.pendingInvites--;

                    //check if it can become active if it is currently not active
                    if (!friendActivity.isActive)
                    {
                        //get dynamic values, check active conditions
                        UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                    }

                    //remove invitation
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend User is invalid");
                }

                //if permission is required, either current user is admin in activity or friend user is the current user
                if (permissionRequired && !(friendUser.ApplicationUserId == currentFriendUser.ApplicationUserId || currentFriendUser.createdActivities.Contains(friendActivity)))
                {
                    return new Pair<bool, string>(false, "Friend User is not a recipient in this invitation");
                }

                //remove inviation from activities
                friendActivity.receivedFriendUserToFriendActivityInvitations.Remove(invitation);

                //remove from friend user list
                friendUser.givenFriendUserToFriendActivityInvitations.Remove(invitation);

                //decrease dynamic count
                friendActivity.dynamicValues.pendingInvites--;

                //check if it can become active if it is currently not active
                if (!friendActivity.isActive)
                {
                    //get dynamic values, check active conditions
                    UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                }

                //remove invitation
                context.Remove(invitation);

            }//if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation) invitationBase;

                if (invitation == null)
                {
                    return new Pair<bool, string>(false, "Invitation of type was not found");
                }

                //get friend user
                FriendUser friendUser = currentUser.friendUser;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {
                        //remove inviation 
                        context.Remove(invitation);

                        IdentityModule.SafelySaveChanges(context);

                        return new Pair<bool, string>(false, "Friend User is invalid and Friend Activity does not exist");
                    }

                    //remove from friend user list
                    friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);

                    //remove inviation 
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend Activity does not exist");
                }

                if (friendUser == null)
                {
                    friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                    //decrease dynamic count
                    friendActivity.dynamicValues.pendingInvites--;

                    //check if it can become active if it is currently not active
                    if (!friendActivity.isActive)
                    {
                        //get dynamic values, check active conditions
                        UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                    }

                    //remove invitation
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend User is invalid");
                }

                //check if friend user is the one we are using or the current user is an admin of the activity
                if (permissionRequired && !(invitation.inviteeId == currentUser.Id || friendUser.createdActivities.Contains(friendActivity)))
                {
                    return new Pair<bool, string>(false, "Friend User is not a recipient in this invitation");
                }

                //remove invitation from friend activity invitations
                friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                //remove invitation from friend user invitations
                friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);

                //decrease dynamic count
                friendActivity.dynamicValues.pendingInvites--;

                //check if it can become active if it is currently not active
                if (!friendActivity.isActive)
                {
                    //get dynamic values, check active conditions
                    UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                }

                //remove invitation
                context.Remove(invitation);
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation) invitationBase;

                if (invitation == null)
                {
                    return new Pair<bool, string>(false, "Invitation of type was not found");
                }

                //get friend user
                FriendUser friendUser = currentUser.friendUser;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {

                        //remove inviation 
                        context.Remove(invitation);

                        IdentityModule.SafelySaveChanges(context);

                        return new Pair<bool, string>(false, "Friend User is invalid and Friend Activity does not exist");
                    }

                    //remove from friend user list
                    friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);

                    //remove inviation 
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend Activity does not exist");
                }

                if (friendUser == null)
                {

                    friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                    //decrease dynamic count
                    friendActivity.dynamicValues.pendingInvites--;

                    //check if it can become active if it is currently not active
                    if (!friendActivity.isActive)
                    {
                        //get dynamic values, check active conditions
                        UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                    }

                    //remove invitation
                    context.Remove(invitation);

                    IdentityModule.SafelySaveChanges(context);

                    return new Pair<bool, string>(false, "Friend User is invalid");
                }

                //check if friend user is the one we are using
                if (permissionRequired && !(invitation.inviteeId == currentUser.Id || friendUser.createdActivities.Contains(friendActivity)))
                {
                    return new Pair<bool, string>(false, "Friend User is not a recipient in this invitation");
                }

                //remove inviation from activities
                friendActivity.givenFriendActivityToFriendUserInvitations.Remove(invitation);

                //remove from friend user list
                friendUser.receivedFriendActivityToFriendUserInvitations.Remove(invitation);

                //decrease dynamic count
                friendActivity.dynamicValues.pendingInvites--;

                //check if it can become active if it is currently not active
                if (!friendActivity.isActive)
                {
                    //get dynamic values, check active conditions
                    UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                }

                //remove invitation
                context.Remove(invitation);
            }
            else
            {
                return new Pair<bool, string>(false, "Invalid invitation type");
            }

            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");
        }

        public static Pair<bool, string> RequestToJoinActivityAsParticipant(ApplicationDbContext context, FriendUser friendUser, FriendActivity friendActivity)
        {
            //check if friend user is already a participant
            if (friendActivity.participants.Contains(friendUser))
            {
                return new Pair<bool, string>(false, "Friend user is already a participant of this activity");
            }

            //check if user is already an admin
            if (friendUser.createdActivities.Contains(friendActivity))
            {
                return new Pair<bool, string>(false, "Friend user is already an admin of this activity");
            }

            //check if user is blocked from activity
            if (context.FriendActivityFriendUserBlocks.Where(b => b.friendUserId == friendUser.ApplicationUserId && b.friendActivityId == friendActivity.id).Any())
            {
                return new Pair<bool, string>(false, "Friend user is blocked from this activity");
            }

            //if invite only
            if (friendActivity.invitationMethod == JOIN_INVITATION_METHODS.INVITE_ONLY)
            {
                return new Pair<bool, string>(false, "Activity is by invitation only");
            }

            //if size orfinvite cap is to be exceded
            if (friendActivity.dynamicValues.pendingInvites >= friendActivity.inviteCap)
            {
                return new Pair<bool, string>(false, "Activity inviation limit has been reached, try later when the admin clears pending invitations");
            }

            //if size of participants cap is to be exceded
            if (friendActivity.dynamicValues.numParticipants >= friendActivity.participantsCap)
            {
                return new Pair<bool, string>(false, "Activity participants limit has been reached");
            }

            //if by invite required
            if (friendActivity.invitationMethod == JOIN_INVITATION_METHODS.INVITE_REQUIRED)
            {
                //check if invitation already exists
                FriendUserToFriendActivityInvitation invitation = context.FriendUserToFriendActivityInvitations.FirstOrDefault(i => i.inviterId == friendUser.ApplicationUserId && i.inviteeId == friendActivity.id && i.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY);

                if (invitation != null)
                {
                    return new Pair<bool, string>(false, "Invitation already exists");
                }

                //create new invitation
                invitation = new FriendUserToFriendActivityInvitation();

                //set timestamp
                invitation.timeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                //add invitation to context
                context.Add(invitation);

                //add to invitation sets
                friendUser.givenFriendUserToFriendActivityInvitations.Add(invitation);
                friendActivity.receivedFriendUserToFriendActivityInvitations.Add(invitation);

                //link navigation properties
                invitation.inviter = friendUser;
                invitation.invitee = friendActivity;

                //set id's 
                invitation.inviterId = friendUser.ApplicationUserId;
                invitation.inviteeId = friendActivity.id;

                //set types
                invitation.transferType = INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY;

                //increase dynamic count
                friendActivity.dynamicValues.pendingInvites++;

                //check if it can become active if it is currently not active
                if (friendActivity.isActive)
                {
                    //get dynamic values, check active conditions
                    UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                }

                //create notification / message to users
                Pair<bool, string> result = MessageModule.createInvitationMessage(context, friendUser.user, invitation);

                if (!result.First)
                {
                    return result;
                }
            }
            //anyone can join
            else if (friendActivity.invitationMethod == JOIN_INVITATION_METHODS.ANYONE)
            {
                //add to participants of activity
                friendActivity.participants.Add(friendUser);

                friendActivity.dynamicValues.numParticipants++;

                //check if it can become active if it is currently not active
                if (friendActivity.isActive)
                {
                    //get dynamic values, check active conditions
                    UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
                }

                //add to friend user's participating activities
                friendUser.participatingActivities.Add(friendActivity);
            }
            //is by invite only, so cannot join
            else
            {
                return new Pair<bool, string>(false, "Activity is by invitation only, cannot join");
            }

            //by invite only?

            //if invitation required
            //create invitation
            //create
            //update in current user's created invitatinos
            //add to other activities pending invitations (admin or group)
            //update invitation count

            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");
        }

        public static Pair<bool, string> RequestToJoinParticipantAsActivity(ApplicationDbContext context, FriendActivity friendActivity, FriendUser friendUser, FriendUser currentFriendUser)
        {
            //check if invitation already exists
            FriendActivityToFriendUserInvitation invitation = context.FriendActivityToFriendUserInvitations.FirstOrDefault(i => i.inviterId == friendActivity.id && i.inviteeId == friendUser.ApplicationUserId && i.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY);

            if (invitation != null)
            {
                return new Pair<bool, string>(false, "Invitation already exists");
            }

            //check if user it participant in activity
            if (friendActivity.participants.Contains(friendUser))
            {
                return new Pair<bool, string>(false, "Friend User is already a participant of this activity");
            }

            //check if already an admin
            if (friendUser.createdActivities.Contains(friendActivity))
            {
                return new Pair<bool, string>(false, "Friend user is already an admin of this activity");
            }

            //check if user is blocked from activity
            if (context.FriendActivityFriendUserBlocks.Where(b => b.friendUserId == friendUser.ApplicationUserId && b.friendActivityId == friendActivity.id).Any())
            {
                return new Pair<bool, string>(false, "Friend user is blocked from this activity");
            }

            //check if current user is admin, in authority to make this call
            if (!currentFriendUser.createdActivities.Contains(friendActivity))
            {
                return new Pair<bool, string>(false, "Current Friend User is not an admin of this activity");
            }

            //create new invitation
            invitation = new FriendActivityToFriendUserInvitation();

            //set timestamp
            invitation.timeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //add invitation to context
            context.Add(invitation);

            //add to invitation sets
            friendActivity.givenFriendActivityToFriendUserInvitations.Add(invitation);
            friendUser.receivedFriendActivityToFriendUserInvitations.Add(invitation);

            //link navigation properties
            invitation.inviter = friendActivity;
            invitation.invitee = friendUser;

            //set types
            invitation.transferType = INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY;

            //increase dynamic count
            friendActivity.dynamicValues.pendingInvites++;

            //check if it can become active if it is currently not active
            if (friendActivity.isActive)
            {
                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
            }

            IdentityModule.SafelySaveChanges(context);

            //create notification / message to users
            Pair<bool, string> result = MessageModule.createInvitationMessage(context, friendUser.user, invitation);

            return result;
        }

        public static Pair<bool, string> RequestToPromoteParticipantToAdmin(ApplicationDbContext context, FriendActivity friendActivity, FriendUser friendUser, FriendUser currentFriendUser)
        {
            //check if invitation already exists
            FriendActivityToFriendUserInvitation invitation = context.FriendActivityToFriendUserInvitations.FirstOrDefault(i => i.inviterId == friendActivity.id && i.inviteeId == friendUser.ApplicationUserId && i.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY);

            if (invitation != null)
            {
                return new Pair<bool, string>(false, "Invitation already exists");
            }

            //check if user it participant in activity
            if (!friendActivity.participants.Contains(friendUser))
            {
                return new Pair<bool, string>(false, "Friend User is not a participant of this activity");
            }

            //check if already an admin
            if (friendUser.createdActivities.Contains(friendActivity))
            {
                return new Pair<bool, string>(false, "Friend user is already an admin of this activity");
            }

            //check if current user is admin, in authority to make this call
            if (!currentFriendUser.createdActivities.Contains(friendActivity))
            {
                return new Pair<bool, string>(false, "Current Friend User is not an admin of this activity");
            }

            //create new invitation
            invitation = new FriendActivityToFriendUserInvitation();

            //set timestamp
            invitation.timeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //add invitation to context
            context.Add(invitation);

            //add to invitation sets
            friendActivity.givenFriendActivityToFriendUserInvitations.Add(invitation);
            friendUser.receivedFriendActivityToFriendUserInvitations.Add(invitation);

            //link navigation properties
            invitation.inviter = friendActivity;
            invitation.invitee = friendUser;

            //set types
            invitation.transferType = INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY;

            //increase dynamic count
            friendActivity.dynamicValues.pendingInvites++;

            //check if it can become active if it is currently not active
            if (friendActivity.isActive)
            {
                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
            }

            IdentityModule.SafelySaveChanges(context);

            //create notification / message to users
            Pair<bool, string> result = MessageModule.createInvitationMessage(context, friendUser.user, invitation);

            return result;
        }

        public static Pair<bool, Pair<string, JObject>> GetViewInvitationInformation(ApplicationUser currentUser, InvitationBase invitationBase)
        {
            //master container
            JObject invitationInformationContainer = new JObject();

            //if invitation was participant requesting to join activity
            if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get given inviation
                FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation)invitationBase;

                if (invitation == null)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Invitation of type was not found", null));
                }

                //get current friendUser
                FriendUser currentFriendUser = currentUser.friendUser;

                if (currentFriendUser == null)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User for current user does not exist", null));
                }

                //ger friend user
                FriendUser friendUser = invitation.inviter;

                //get activity
                FriendActivity friendActivity = invitation.invitee;

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {
                        return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is invalid and Friend Activity does not exist", null));
                    }

                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend Activity does not exist", null));
                }

                if (friendUser == null)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is invalid", null));
                }

                bool isInviter = friendUser.ApplicationUserId == currentFriendUser.ApplicationUserId;
                bool isInvitee = currentFriendUser.createdActivities.Contains(friendActivity);

                //if permission is required, either current user is admin in activity or friend user is the current user
                if (!(isInviter || isInvitee))
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is not a recipient in this invitation", null));
                }

                if (isInviter && isInvitee)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend user is both inviter and invitee", null));
                }

                //add components
                invitationInformationContainer.Add(new JProperty("is_invitee", isInvitee));

                if (isInviter)
                {
                    invitationInformationContainer.Add(new JProperty("type", "person"));
                    invitationInformationContainer.Add(new JProperty("id", friendUser.ApplicationUserId));
                    invitationInformationContainer.Add(new JProperty("name", currentUser.name));
                    invitationInformationContainer.Add(new JProperty("invitation_type_message", "You requested to join " + friendActivity.name));
                    //add local profile image links
                }
                else
                {
                    invitationInformationContainer.Add(new JProperty("type", "activity"));
                    invitationInformationContainer.Add(new JProperty("id", friendActivity.id));
                    invitationInformationContainer.Add(new JProperty("name", friendActivity.name));
                    invitationInformationContainer.Add(new JProperty("invitation_type_message", currentUser.name + " is requesting you to join"));
                    //add local profile image links
                }

                invitationInformationContainer.Add(new JProperty("invitation_id", invitationBase.id));


            }
            //if activity requested friend user to join as participant
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                if (invitation == null)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Invitation of type was not found", null));
                }

                //get friend user
                FriendUser friendUser = invitation.invitee;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {
                        return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is invalid and Friend Activity does not exist", null));
                    }

                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend Activity does not exist", null));
                }

                if (friendUser == null)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is invalid", null));
                }

                bool isInviter = friendUser.createdActivities.Contains(friendActivity);
                bool isInvitee = invitation.inviteeId == currentUser.Id;

                //if permission is required, either current user is admin in activity or friend user is the current user
                if (!(isInviter || isInvitee))
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is not a recipient in this invitation", null));
                }

                if (isInviter && isInvitee)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend user is both inviter and invitee", null));
                }

                //add components
                invitationInformationContainer.Add(new JProperty("is_invitee", isInvitee));

                if (isInviter)
                {
                    invitationInformationContainer.Add(new JProperty("type", "activity"));
                    invitationInformationContainer.Add(new JProperty("id", friendActivity.id));
                    invitationInformationContainer.Add(new JProperty("name", friendActivity.name));
                    invitationInformationContainer.Add(new JProperty("invitation_type_message", "Your activity requested " + currentUser.name + " to join"));
                    //add local profile image links
                }
                else
                {
                    invitationInformationContainer.Add(new JProperty("type", "person"));
                    invitationInformationContainer.Add(new JProperty("id", friendUser.ApplicationUserId));
                    invitationInformationContainer.Add(new JProperty("name", currentUser.name));
                    invitationInformationContainer.Add(new JProperty("invitation_type_message", friendActivity.name + " requests you to join"));
                    //add local profile image links
                }

                invitationInformationContainer.Add(new JProperty("invitation_id", invitationBase.id));
            }
            //if activity requested friend user to join as admin
            else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
            {
                //get current invitation
                FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;

                if (invitation == null)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Invitation of type was not found", null));
                }

                //get friend user
                FriendUser friendUser = currentUser.friendUser;

                //get activity
                FriendActivity friendActivity = invitation.inviter;

                if (friendActivity == null)
                {
                    if (friendUser == null)
                    {
                        return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is invalid and Friend Activity does not exist", null));
                    }

                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend Activity does not exist", null));
                }

                if (friendUser == null)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is invalid", null));
                }

                bool isInviter = friendUser.createdActivities.Contains(friendActivity);
                bool isInvitee = invitation.inviteeId == currentUser.Id;

                //if permission is required, either current user is admin in activity or friend user is the current user
                if (!(isInviter || isInvitee))
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend User is not a recipient in this invitation", null));
                }

                if (isInviter && isInvitee)
                {
                    return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Friend user is both inviter and invitee", null));
                }

                //add components
                invitationInformationContainer.Add(new JProperty("is_invitee", isInvitee));

                if (isInviter)
                {
                    invitationInformationContainer.Add(new JProperty("type", "activity"));
                    invitationInformationContainer.Add(new JProperty("id", friendActivity.id));
                    invitationInformationContainer.Add(new JProperty("name", friendActivity.name));
                    invitationInformationContainer.Add(new JProperty("invitation_type_message", "Your activity requested " + currentUser.name + " be promoted to admin"));
                    //add local profile image links
                }
                else
                {
                    invitationInformationContainer.Add(new JProperty("type", "person"));
                    invitationInformationContainer.Add(new JProperty("id", friendUser.ApplicationUserId));
                    invitationInformationContainer.Add(new JProperty("name", currentUser.name));
                    invitationInformationContainer.Add(new JProperty("invitation_type_message", friendActivity.name + " requests you to be an admin"));
                    //add local profile image links
                }

                invitationInformationContainer.Add(new JProperty("invitation_id", invitationBase.id));
            }
            else
            {
                return new Pair<bool, Pair<string, JObject>>(false, new Pair<string, JObject>("Invalid invitation type", null));
            }

            return new Pair<bool, Pair<string, JObject>>(true, new Pair<string, JObject>("", invitationInformationContainer));
        }

        public static void clearOldInvitations(ApplicationDbContext context)
        {
            //get current time in milliseconds
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //get parameters
            double invitationLimitDays = Startup.externalConfiguration.GetSection("InternalParameters").GetValue<double>("OLD_INVITATION_LIMIT");
            long invitationLimit = TimeSpan.FromDays(invitationLimitDays).Ticks / TimeSpan.TicksPerMillisecond;

            long minInvitationTimestamp = currentTime - invitationLimit;

            //get all invitations that are old
            List<InvitationBase> oldInvitations = context.InvitationBases.Where(i => (i.timeStamp <= minInvitationTimestamp)).ToList();

            //remove each invitation
            foreach(InvitationBase invitation in oldInvitations)
            {
                InvitationModule.RemoveInvitation(context, invitation, false);
            }

            return;
        }

        /// <summary>
        /// remove invitation of friend user requesting to join activity as participant
        /// </summary>
        /*public static void RemoveRequestToJoinActivityAsParticipantInvitation(ApplicationUser user, )
        {
            if (friendActivity == null)
            {
                friendUser = user.friendUser;

                if (friendUser == null)
                {
                    //remove inviation 
                    _context.Invitations.Remove(invitation);

                    IdentityModule.SafelySaveChanges(_context);

                    return BadRequest("Friend User is invalid");
                }

                //remove from friend user list
                friendUser.givenInvitations.Remove(invitation);

                //remove inviation 
                _context.Invitations.Remove(invitation);

                IdentityModule.SafelySaveChanges(_context);

                return BadRequest("Friend Activity does not exist");
            }

            //get friend user
            friendUser = user.friendUser;

            if (friendUser == null)
            {
                return BadRequest("Friend User is invalid");
            }

            //remove inviation from activities
            friendActivity.receivedInvitations.Remove(invitation);

            //remove from friend user list
            friendUser.givenInvitations.Remove(invitation);
        }*/

        //RequestToPromoteParticipantToAdmin

        //RequestToJoinParticipantAsActivity
    }
}
