using Castle.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using POPNetwork.Models;
using POPNetwork.Modules;
using System.Threading.Tasks;

namespace POPNetwork.Controllers;

[ApiController]
[Route("api/AccountManager")]
public class IdentityController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public IdentityController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)//, ILoggerFactory loggerFactory)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        //_logger = loggerFactory.CreateLogger<AccountController>();
    }

    //TODO ADD EXPO TOKEN TO REGISTER

    /// <summary>
    /// Endpoint, register user
    /// </summary>
    /// <param name="model"></param>
    /// <returns>200 blank message if accepted, 404 with error message otherwise</returns>
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterModelView model)
    {
        //check if user already exists
        ApplicationUser user = await _userManager.FindByNameAsync(model.username);

        if (user != null)
        {
            //compile error message with all the errors
            string errorMessage = "Invalid username or password";

            return BadRequest(errorMessage);
        }

        //create new application user model instance
        user = new ApplicationUser() { UserName = model.username, Email = model.email, EmailConfirmed = false, age = 100, birthdate = System.DateTime.Today.AddYears(-100) };

        //put in own method
        //create role
        await IdentityModule.checkRoles(_roleManager);

        //attempt to create user
        var result = await _userManager.CreateAsync(user, model.password);

        if (result.Succeeded)
        {
            //add role
            await _userManager.AddToRoleAsync(user, "User");

            //create friend, dating, and casual user
            UserModule.createUser(_context, user);

            //attempt to sign in and get result
            var signInResult = await _signInManager.PasswordSignInAsync(model.username, model.password, isPersistent: true, lockoutOnFailure: true);

            //if it succeded, return 200 success
            if (result.Succeeded)
            {
                //attempt to find user
                user = await _userManager.FindByNameAsync(model.username);

                //if user does not exist, return 400 error with message
                if (user == null)
                {
                    return BadRequest("Invalid username or password");
                }

                //add expo token
                MessageModule.addExpoToken(_context, user, model.expo_token);

                return Ok();
            }
            //if locked out, return 400 error with message
            else if (signInResult.IsLockedOut)
            {
                return BadRequest("Sign in failed due to user being locked out");
                //return error code
            }
            //if is not allowed, return 400 error with message
            else if (signInResult.IsNotAllowed)
            {
                return BadRequest("Sign in failed because user is not allowed to sign in");
            }
            //if requires two factor authentication, return 400 error with message
            else if (signInResult.RequiresTwoFactor)
            {
                return BadRequest("Sign in failed because user requires two factor authentication");
            }
            //else, for another reason
            else
            {
                //attempt to find user
                user = await _userManager.FindByNameAsync(model.username);

                //if user does not exist, return 400 error with message
                if (user == null)
                {
                    return BadRequest("Invalid username or password");
                }
                //else, password was wrong, so return 400 error with message
                else
                {
                    return BadRequest("Invalid username or password");
                }
            }
        }
        else
        {
            //delete user
            await _userManager.DeleteAsync(user);

            //compile error message with all the errors
            /*string errorMessage = "User cannot be created because of the following errors:\n";

            //get each error
            foreach (IdentityError error in result.Errors)
            {
                //add it to error message
                errorMessage += error.ToString() + "\n";
            }*/

            string errorMessage = "Invalid username or password";

            return BadRequest(errorMessage);
        }
    }

    //TODO ADD EXPO TOKEN TO LOGIN 
    //in case they switch devices

    /// <summary>
    /// Endpoint, Log in user
    /// </summary>
    /// <param name="model"></param>
    /// <returns>200 blank message if accepted, 404 with error message otherwise</returns>
    [HttpPost("LogIn")]
    [AllowAnonymous]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> LogIn(LoginModelView model)
    {
        //attempt to sign in and get result
        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true, lockoutOnFailure: true);

        //if it succeded, return 200 success
        if (result.Succeeded)
        {
            //attempt to find user
            var user = await _userManager.FindByNameAsync(model.Username);

            //if user does not exist, return 400 error with message
            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }

            //add expo token
            MessageModule.addExpoToken(_context, user, model.expo_token);

            //return the user_id
            JObject masterContainer = new JObject();

            masterContainer.Add(new JProperty("user_id", user.Id));

            return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
        //if locked out, return 400 error with message
        else if (result.IsLockedOut)
        {
            return BadRequest("Sign in failed due to user being locked out");
            //return error code
        }
        //if is not allowed, return 400 error with message
        else if (result.IsNotAllowed)
        {
            return BadRequest("Sign in failed because user is not allowed to sign in");
        }
        //if requires two factor authentication, return 400 error with message
        else if (result.RequiresTwoFactor)
        {
            return BadRequest("Sign in failed because user requires two factor authentication");
        }
        //else, for another reason
        else
        {
            //attempt to find user
            var user = await _userManager.FindByNameAsync(model.Username);

            //if user does not exist, return 400 error with message
            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }
            //else, password was wrong, so return 400 error with message
            else
            {
                return BadRequest("Invalid username or password");
            }
        }
    }

    /// <summary>
    /// Endpoint, Sign in user
    /// </summary>
    /// <param name="model"></param>
    /// <returns>200 blank message if accepted, 404 blank message otherise</returns>
    [HttpGet("LogOut")]
    [Authorize]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOff()
    {
        //sign out user
        await _signInManager.SignOutAsync();

        return Ok();
    }

    //TODO Test invitations
    /// <summary>
    /// Remove user from the system
    /// </summary>
    /// <returns>200 ok</returns>
    [HttpDelete("Remove")]
    [Authorize]
    public async Task<IActionResult> Remove()
    {
        //get the current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //remove friend, dating, and casual user
        UserModule.RemoveUser(_context, user);

        //remove user from roles they are in
        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

        //remove from user manager
        await _userManager.DeleteAsync(user);

        //sign out user
        await _signInManager.SignOutAsync();

        return Ok();
    }

    /// <summary>
    /// check if the server is up and running
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("ValidateServerConnection")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckServerConnection()
    {
        return Ok();
    }

    /// <summary>
    /// generate random expo token
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetTemporaryExpoToken")]
    public async Task<IActionResult> GenerateRandomTemporaryExpoToken()
    {
        //get random token
        string token = MessageModule.getRandomExpoToken(_context);

        //put token in json
        JObject masterContainer = new JObject();

        masterContainer.Add(new JProperty("expo_token", token));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// validate api token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("ValidateUserApiToken")]
    [Authorize]
    public async Task<IActionResult> CheckApiToken()
    {
        //get user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //return the user_id
        JObject masterContainer = new JObject();

        masterContainer.Add(new JProperty("user_id", user.Id));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// get the email of the current user
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetEmail")]
    [Authorize]
    public async Task<IActionResult> GetEmail()
    {
        //get user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        JObject masterContainer = new JObject();

        masterContainer.Add(new JProperty("email", user.Email));
        masterContainer.Add(new JProperty("is_confirmed", user.EmailConfirmed));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// get the email of the current user
    /// </summary>
    /// <returns></returns>
    [HttpPost("ResetEmail")]
    [Authorize]
    public async Task<IActionResult> ResetEmail(ResetEmailModel model)
    {
        //get user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //set email
        user.Email = model.email;

        //set email confirmed to false
        user.EmailConfirmed = false;

        return Ok();
    }

    //
    /// <summary>
    /// get the email of the current user
    /// </summary>
    /// <returns></returns>
    [HttpPost("RequestToVerifyEmail")]
    [Authorize]
    public async Task<IActionResult> RequestToVerifyEmail(ResetEmailModel model)
    {
        //get user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //email to everidy
        string verifyEmail;

        //if they did not supply an email to verify
        if (model.email == null)
        {
            //set the email to be their email
            verifyEmail = user.Email;
        }
        else
        {
            verifyEmail = model.email;
        }

        Pair<bool, string> result = EmailConfirmationModule.generateKeyCode(_context, user, verifyEmail);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }

        return Ok();
    }

    //
    [HttpGet("VerifyEmail")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail(uint keycode)
    {
        //get user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //enter key code and change verify email status
        Pair<bool, string> result = EmailConfirmationModule.enterKeyCode(_context, user, keycode);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }

        return Ok();
    }

    //
    /// <summary>
    /// send keycode to email to reset password
    /// </summary>
    /// <param name="keycode"></param>
    /// <returns></returns>
    [HttpGet("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(string username)
    {
        //get user
        ApplicationUser user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return BadRequest("Invalid username");
        }

        Pair<bool, string> result = PasswordRecoveryModule.generateKeyCode(_context, user);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }

        return Ok();
    }

    //
    /// <summary>
    /// get password reset token for the current user
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetResetPasswordToken")]
    public async Task<IActionResult> GetResetPasswordToken(string username, uint keycode)
    {
        //get user
        ApplicationUser user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return BadRequest("Invalid username");
        }

        Pair<bool, string> result = PasswordRecoveryModule.enterKeyCode(_context, user, keycode);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            //generate password reset token
            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            //generate jobject
            JObject masterContainer = new JObject();

            //add token to jobject
            masterContainer.Add(new JProperty("password_reset_token", passwordResetToken));


            return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
    }

    //
    /// <summary>
    /// reset the password of the current user
    /// </summary>
    /// <returns></returns>
    [HttpPost("ResetPasswordWithToken")]
    public async Task<IActionResult> ResetPasswordUnauthorized(ResetPasswordUnauthorizedModel model)
    {
        //get user
        ApplicationUser user = await _userManager.FindByNameAsync(model.username);

        if (user == null)
        {
            return BadRequest();
        }

        //if password is incorrect
        if (!(await _userManager.CheckPasswordAsync(user, model.old_password)))
        {
            return BadRequest("Old password is incorrect");
        }

        //check if new password is valid
        PasswordValidator<ApplicationUser> passwordValidator = new PasswordValidator<ApplicationUser>();

        if (!(await passwordValidator.ValidateAsync(_userManager, user, model.new_password)).Succeeded)
        {
            return BadRequest("New password is not valid, try a different one");
        }

        //reset password
        IdentityResult result = await _userManager.ResetPasswordAsync(user, model.password_reset_token, model.new_password);

        if (!result.Succeeded)
        {
            return BadRequest("Password reset token expired or is invalid, or password is invalid");
        }

        return Ok();
    }

    [HttpPost("ResetPassword")]
    [Authorize]
    public async Task<IActionResult> ResetPasswordAuthorized(ResetPasswordAuthorizedModel model)
    {
        //get user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //check if old password is correct
        PasswordValidator<ApplicationUser> passwordValidator = new PasswordValidator<ApplicationUser>();

        if (!((await passwordValidator.ValidateAsync(_userManager, user, model.old_password)).Succeeded))
        {
            return BadRequest("Old password is incorrect");
        }

        //get password reset token
        string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        //reset password
        IdentityResult result = await _userManager.ResetPasswordAsync(user, passwordResetToken, model.new_password);

        if (!result.Succeeded)
        {
            return BadRequest("Password reset token expired or is invalid, or password is invalid");
        }

        return Ok();
    }
}
