using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Castle.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using POPNetwork.Controllers;
using POPNetwork.Global;
using POPNetwork.Models;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace POPNetwork.Modules;
public class AzureBlobModule
{
    static BlobServiceClient blobServiceClient;

    public static bool init()
    {
        //create connection string
        var connectionString = POPNetwork.Startup.externalConfiguration.GetConnectionString("StorageAccount");

        //create service client object
        blobServiceClient = new BlobServiceClient(connectionString);

        return true;
    }

    /// <summary>
    /// when a new user is created, create their corresponding containers
    /// </summary>
    /// <param name="id">id of the user</param>
    /// <returns></returns>
    public static async void createFriendUserContainer(string id)
    {
        //create object
        BlobContainerClient blobContainerClient = await blobServiceClient.CreateBlobContainerAsync("friends-" + id, PublicAccessType.Blob);
    }

    /// <summary>
    /// when a user is deleted, delete their corresponding containers, and contents thereof
    /// </summary>
    /// <param name="id">id of the user</param>
    /// <returns></returns>
    public static async void deleteFriendUserContainer(string id)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("friends-" + id);

        try
        {
            await blobContainerClient.DeleteAsync();
        }
        catch (Azure.RequestFailedException ex)
        {
            //TODO Add to error log
        }
    }


    /// <summary>
    /// upload profile image
    /// </summary>
    /// <param name="num">the number the image is</param>
    /// <returns></returns>

    public static async Task<Pair<bool, string>> uploadFriendUserProfileImage(ApplicationUser user, FriendUser friendUser, ApplicationDbContext context, IFormFile image, short profile_num)
    {
        //compile url
        string fileName = "profile-image-" + profile_num.ToString() + ".png";
        string blobName = fileName;

        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("friends-" + user.Id.ToString());

        //check if container exists
        if (!blobContainerClient.Exists())
        {
            //if not, create it
            BlobContainerInfo blobContainerInfo = await blobContainerClient.CreateAsync();

            //set user profile image active values
            friendUser.profile_image_0_active = false;
            friendUser.profile_image_1_active = false;
            friendUser.profile_image_2_active = false;

            IdentityModule.SafelySaveChanges(context);
        }

        //get client for the container
        BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

        //set content type to "image/jpg"
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = "image/png" };

        //check if blob already exsits
        if (blobClient.Exists())
        {
            //delete it
            await blobClient.DeleteAsync();
        }

        //create the blob
        try
        {
            //open stream and upload image to azure blob
            using (Stream file = image.OpenReadStream())
            {
                await blobClient.UploadAsync(file, blobHttpHeaders);
            }

            //update profile image status
            switch (profile_num)
            {
                case 0:
                    {
                        if (!friendUser.profile_image_0_active)
                        {
                            friendUser.profile_image_0_active = true;
                            IdentityModule.SafelySaveChanges(context);
                        }
                        break;
                    }
                case 1:
                    {
                        if (!friendUser.profile_image_1_active)
                        {
                            friendUser.profile_image_1_active = true;
                            IdentityModule.SafelySaveChanges(context);
                        }
                        break;
                    }
                case 2:
                    {
                        if (!friendUser.profile_image_2_active)
                        {
                            friendUser.profile_image_2_active = true;
                            IdentityModule.SafelySaveChanges(context);
                        }
                        break;
                    }
            }
        }
        catch (Azure.RequestFailedException e)
        {
            return new Pair<bool, string>(false, e.Message);
        }

        //1. delete current azure stoarge acocunt
        //2. create new one from template in westus unser pop_social_west resource group
        //3. https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload
        //4. https://docs.microsoft.com/en-us/azure/storage/blobs/storage-custom-domain-name?tabs=azure-portal
        //5. add connection string to config file
        return new Pair<bool, string>(true, "");
    }

    public static string getFriendUserProfileImageUrl(string user_id, short profile_num)
    {
        string baseDomain = Startup.externalConfiguration.GetSection("Urls").GetValue<string>("BaseDomain");

        //https://popsocialstorage.blob.core.windows.net/
        //return ("https://blob." + baseDomain + "/friends-" + user_id + "/" +  "profile-image-" + profile_num.ToString() + ".jpg");
        return ("https://popsocialstorage.blob.core.windows.net/friends-" + user_id + "/" + "profile-image-" + profile_num.ToString() + ".png");
    }
}