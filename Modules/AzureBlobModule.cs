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
        string fileName = "profile-image-" + profile_num.ToString() + ".jpg";
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
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = "image/jpg" };

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
        return ("https://popsocialstorage.blob.core.windows.net/friends-" + user_id + "/" + "profile-image-" + profile_num.ToString() + ".jpg");
    }

    //https://medium.com/a-techies-tidbits/image-upload-with-react-native-and-net-core-6ff6a41caec5

    //upload image
    //converting image formats
    //check image formats in react native https://stackoverflow.com/questions/48854594/c-sharp-how-to-open-heic-image


    //set mime type
    //https://docs.microsoft.com/zh-cn/javascript/api/@azure/storage-blob/blobhttpheaders?view=azure-node-latest
    //response.Content.Headers.ContentType = new MediaTypeHeaderValue("jpg"); 





    /*
     const blobServiceClient = BlobServiceClient.fromConnectionString(connectionstr)

    const containerClient=blobServiceClient.getContainerClient('test')
    const blobclient=containerClient.getBlockBlobClient('test.jpg')
    let fileStream = fs.createReadStream('E:\\dog.jpg');
    const blobOptions = { blobHTTPHeaders: { blobContentType: 'image/jpg' } };
    blobclient.uploadStream(fileStream,undefined ,undefined ,blobOptions)
     */

    //blobClient.UploadStream(your_stream, overwrite: true);

    //get status

    //react native image crop picker reduce image size: maxWidth and maxHeight from the options.

    /*
     public static async Task<HttpResponseMessage> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "files/{id}")]
    HttpRequest req, string id, TraceWriter log)
{
    var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
    var client = account.CreateCloudBlobClient();
    var container = client.GetContainerReference("sitecontent");
    var blob = container.GetBlockBlobReference(id);

    var stream = new MemoryStream();
    await blob.DownloadToStreamAsync(stream);
    stream.Seek(0, SeekOrigin.Begin);

    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
    result.Content = new StreamContent(stream);
    result.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/octet-stream");

    return result;
}*/
}

//https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=environment-variable-windows

//maybe store main profile image also in lower resolution for easier and better downloading, but do that later in the program

//https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs?view=azure-dotnet
//https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet

//adding user: 

//How to deal with difference of extensions for pictures (.jpg and .png)?

//Format: https://myaccount.blob.core.windows.net/mycontainer/myblob

//container name contains username, and in it contains all information for all realms

//Endpoints
//mycontainer = "Friends"
//  profilepicture1.ext
//  profilepicture2.ext
//  ...

/*
To stream file to user, use 

[Route("api/[controller]")]
public class DownloadController : Controller {
    //GET api/download/12345abc
    [HttpGet("{id}")]
    public async Task<IActionResult> Download(string id) {
        Stream stream = await {{__get_stream_based_on_id_here__}}

        if(stream == null)
            return NotFound(); // returns a NotFoundResult with Status404NotFound response.

        return File(stream, "application/octet-stream"); // returns a FileStreamResult
    }    
}


    //upload activity image (s)
    //called after AddFriendActivity
    //adds images links (our custom ones) to it.

    public async Task<IActionResult> UploadFriendActivityPhoto(IFormFile file) 
    {
        using (FileStream filestream = System.IO.File.Create(_environment.WebRootPath + "\\uploads\\" + files.files.FileName))
        {
            file.CopyTo();
            filestream.Flush();
            return "\\uploads\\" + files.files.FileName;
        }

        StorageCredentialscreden = newStorageCredentials(accountname, accesskey);

        CloudStorageAccountacc = newCloudStorageAccount(creden, useHttps: true);

        CloudBlobClient client = acc.CreateCloudBlobClient();

        CloudBlobContainercont = client.GetContainerReference("mysample");

        cont.CreateIfNotExists();

        cont.SetPermissions(newBlobContainerPermissions


        {

            PublicAccess = BlobContainerPublicAccessType.Blob


        });

        CloudBlockBlobcblob = cont.GetBlockBlobReference("Sampleblob.jpg");

        using (Stream file = System.IO.File.OpenRead(@ "D:\amit\Nitin sir\Nitinpandit.jpg"))

        {

            cblob.UploadFromStream(file);

        }
    }

or with 

return Redirect("http://www.google.com");

for url files

for uploading files, 

https://stackoverflow.com/questions/63384025/uploading-a-pdf-file-to-azure-blob-storage-using-rest-api-c-sharp-without-usi

https://docs.microsoft.com/en-us/dotnet/api/overview/azure/storage.common-readme
 


so for download files, use redirect

for upload files, use the code above


 */
