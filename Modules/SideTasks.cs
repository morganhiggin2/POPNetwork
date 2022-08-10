using Microsoft.AspNetCore.Identity;
using POPNetwork.Controllers;
using POPNetwork.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace POPNetwork.Modules
{
    public class SideTasks
    {
        /// <summary>
        /// timed loop to take care of tasks
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="_userManager"></param>
        /// <returns></returns>
        public static Task taskLoop(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            //five minutes
            int waitTime = 300000;

            //counters
            int dayCounter = DateTime.Now.DayOfYear;
            long fifteenMinuteCounter = 0;

            while (true)
            {
                //wait waitTime
                Thread.Sleep(waitTime);

                //if the day of the year has changed over the course of the waittime
                if (DateTime.Now.DayOfYear != dayCounter)
                {
                    //perform day tasks
                    dailyTasks(_context, _userManager);

                    //reset dayCounter
                    dayCounter = DateTime.Now.DayOfYear;
                } 

                //if more than or equal to fifteen minutes has passed
                if (fifteenMinuteCounter >= 900000)
                {
                    //perform fifteen minute tasks
                    fifteenMinuteTasks(_context, _userManager);
                }

                //increment counters
                fifteenMinuteCounter += waitTime;
            }
        }

        /// <summary>
        /// start thread that runs tasks once a day at midnight
        /// </summary>
        public static void dailyTasks(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            //update ages
            UserModule.updateAges(_context, _userManager);

            //clear old invitations
            InvitationModule.clearOldInvitations(_context);

            //clear old activity announcements
            MessageModule.removeOldAnnouncements(_context);

            //clear old conversations
            MessageModule.removeOldConversations(_context);

            //clear old friend user messages, conversation messages, and direct messages
            MessageModule.removeOldFriendUserMessages(_context);

            //deactivate old friend activities
            UserModule.setNotActiveForOldActivities(_context);

            //remove old locations
            LocationModule.DeleteOldLocations(_context);
        }

        public static void fifteenMinuteTasks(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            //check expo push notifications receipts
            MessageModule.checkPushReceipts();

            //send out notifications for activities participants are in that they start soon
            MessageModule.sendOutSoonFriendActivitiyReminders(_context);
        }
    }
}
