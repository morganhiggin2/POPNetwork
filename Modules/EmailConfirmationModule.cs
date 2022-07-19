using Castle.Core;
using Microsoft.Extensions.Configuration;
using POPNetwork.Controllers;
using POPNetwork.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace POPNetwork.Modules
{
    public class EmailConfirmationModule
    {
        public static Pair<bool, string> generateKeyCode(ApplicationDbContext context, ApplicationUser user, string email)
        {
            VerifyEmailKeyCode keycode = context.VerifyEmailKeyCodes.FirstOrDefault(c => c.applicationUserId == user.Id);

            //enforce one keycode only
            if (keycode == null)
            {
                //make new keycode
                keycode = new VerifyEmailKeyCode();

                //set user
                keycode.applicationUser = user;

                //add to context
                context.VerifyEmailKeyCodes.Add(keycode);
            }

            Random random = new Random();

            //set keycode
            keycode.keycode = (uint)(random.Next(1, 10) * 100000 + random.Next(1, 10) * 10000 + random.Next(1, 10) * 1000 + random.Next(1, 10) * 100 + random.Next(1, 10) * 10 + random.Next(1, 10));
            keycode.email = email;

            //send email
            //get parameters
            string fromEmail = Startup.externalConfiguration.GetSection("EmailStrings").GetValue<string>("SupportEmail");
            string fromPassword = Startup.externalConfiguration.GetSection("EmailStrings").GetValue<string>("SupportPassword");
            string toEmail = email;

            MailMessage message = new MailMessage(fromEmail, toEmail);
            message.Subject = "Verify your email with POP";
            message.Body = "The keycode to verify your email is: \n\n" + keycode.keycode + "\n\nDo not share with anyone";

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

            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");
        }

        //enter keycode, get password reset token (actual method in identity for this)
        //set with expiration date (like 5 minutes)
        /// <summary>
        /// enter the keycode for the reset password
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user"></param>
        /// <returns>if successful, password regeneration token</returns>
        public static Pair<bool, string> enterKeyCode(ApplicationDbContext context, ApplicationUser user, uint key)
        {
            VerifyEmailKeyCode keycode = context.VerifyEmailKeyCodes.FirstOrDefault(c => c.applicationUserId == user.Id);

            //enforce one keycode only
            if (keycode == null)
            {
                return new Pair<bool, string>(false, "Keycode does not exist");
            }

            if (keycode.keycode != key)
            {
                return new Pair<bool, string>(false, "Keycode is invalid");
            }

            //remove keycode from system
            context.VerifyEmailKeyCodes.Remove(keycode);

            user.Email = keycode.email;
            user.EmailConfirmed = true;

            IdentityModule.SafelySaveChanges(context);

            return new Pair<bool, string>(true, "");
        }
    }
}
