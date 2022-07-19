using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POPNetwork.Models;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        name = "Friendly";
        gender = "other";
        expoTokens = new HashSet<UserExpoToken>();
    }

    //first name
    public string name { get; set; }

    //gender
    public string gender { get; set; }

    //age
    public int age { get; set; }

    //expo tokens for push notifications
    public virtual ICollection<UserExpoToken> expoTokens { get; set; }

    public System.DateTime birthdate { get; set; }

    //friend user link
    public virtual FriendUser friendUser { get; set; }

    //dating user link
    public virtual DatingUser datingUser {  get; set; }

    //casual user link
    public virtual CasualUser casualUser { get; set; }
}

public class UserExpoToken
{
    /// <summary>
    /// expo token id
    /// </summary>
    [Key]
    public string expoToken { get; set;}

    /// <summary>
    /// id of the application user
    /// </summary>
    public string userId { get; set; }

    /// <summary>
    /// user 
    /// </summary>
    public virtual ApplicationUser user { get; set; }
}

public class RegisterModelView
{
    [Required(ErrorMessage = "Username Required")]
    [Display(Name = "Username")]
    public string username { get; set; }

    [Required(ErrorMessage = "Email Required")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")]
    public string email { get; set; }

    [Required(ErrorMessage = "Password Required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string password { get; set; }

    [Required(ErrorMessage = "Expo Token Required")]
    [DataType(DataType.Password)]
    [Display(Name = "Expo Token")]
    public string expo_token { get; set; }
}

public class LoginModelView
{
    [Required(ErrorMessage = "Username Required")]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password Required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Expo Token Required")]
    [DataType(DataType.Password)]
    [Display(Name = "Expo Token")]
    public string expo_token { get; set; }
}

public class ResetPasswordKeyCode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }
    public virtual ApplicationUser applicationUser { get; set; }

    public string applicationUserId { get; set; }

    public uint keycode { get; set; }
}

public class VerifyEmailKeyCode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string id { get; set; }
    public virtual ApplicationUser applicationUser { get; set; }

    public string applicationUserId { get; set; }

    public uint keycode { get; set; }

    public string email { get; set; }
}

public class UpdateUserInformationModel
{
    [Display(Name = "name")]
    public string? name { get; set; }

    [Display(Name = "gender")]
    public string? gender { get; set; }

    [Display(Name = "birthdate")]
    public string? birthdate { get; set; }
}

public class ResetEmailModel
{
    [Display(Name = "email")]
    [DataType(DataType.EmailAddress)]
    public string? email { get; set; }
}

public class ResetPasswordUnauthorizedModel
{
    [Required(ErrorMessage = "Old Password Required")]
    [DataType(DataType.Password)]
    public string old_password { get; set; }

    [Required(ErrorMessage = "New Password Required")]
    [DataType(DataType.Password)]
    public string new_password { get; set; }

    public string password_reset_token { get; set; }

    public string username { get; set; }
}

public class ResetPasswordAuthorizedModel
{
    [Required(ErrorMessage = "Old Password Required")]
    [DataType(DataType.Password)]
    public string old_password { get; set; }

    [Required(ErrorMessage = "New Password Required")]
    [DataType(DataType.Password)]
    public string new_password { get; set; }
}

//api token: [DataType("nvarchar(450)")] 