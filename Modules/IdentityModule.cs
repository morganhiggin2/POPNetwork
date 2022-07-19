
using Microsoft.AspNetCore.Identity;
using POPNetwork.Controllers;
using System.Threading.Tasks;

namespace POPNetwork.Modules;
public class IdentityModule
{
    /// <summary>
    /// check if roles exist, and if they don't, create them. Must be done before assignment.
    /// </summary>
    /// <param name="_roleManager"></param>
    public static async Task checkRoles(RoleManager<IdentityRole> _roleManager)
    {
        IdentityResult roleResult;

        var roleCheck = await _roleManager.RoleExistsAsync("User");

        if (!roleCheck)
        {
            roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        //for more steps: https://social.technet.microsoft.com/wiki/contents/articles/51333.asp-net-core-2-0-getting-started-with-identity-and-role-management.aspx
    }

    /// <summary>
    /// safely save changes when dealing with concurrency exception
    /// </summary>
    /// <param name="context"></param>
    public static void SafelySaveChanges(ApplicationDbContext context)
    {
        try
        {
            context.SaveChanges();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {

        }
    }
}
