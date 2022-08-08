using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Castle.Core;
using Newtonsoft.Json.Linq;
using POPNetwork.Models;
using System.Text.RegularExpressions;
using POPNetwork.Modules;
using NetTopologySuite.Geometries;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using POPNetwork.Global;
using static POPNetwork.Modules.LocationModule;
using static POPNetwork.Global.GlobalProperties;
using static POPNetwork.Models.MessageModels;

namespace POPNetwork.Controllers;

[ApiController]
[Route("api/User")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)//, ILoggerFactory loggerFactory)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        //_emailSender = emailSender;
        //_smsSender = smsSender;
        //_logger = loggerFactory.CreateLogger<AccountController>();
    }
    //one endpoint for casual to tell wether or not someone looked at a profile and messages or not

    //GENERIC USER ENDPOINTS

    [HttpPost("Generic/UpdateUserInformation")]
    [Authorize]
    public async Task<IActionResult> UpdateUserInformation(UpdateUserInformationModel model)
    {
        //get the current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //set the information that was given

        if (model.name != null)
        {
            user.name = model.name.ToLower();
        }

        if (model.gender != null)
        {
            if (!GlobalProperties.GENDER_OPTIONS.Contains(model.gender))
            {
                return BadRequest("Gender is not valid");
            }

            user.gender = model.gender;
        }

        if (model.birthdate != null)
        {
            DateTime birthdate;

            try
            {
                birthdate = DateTime.ParseExact(model.birthdate, "d/M/yyyy", null);
            }
            catch (FormatException)
            {
                try
                {
                    birthdate = DateTime.ParseExact(model.birthdate, "dd/MM/yyyy HH:mm", null);
                }
                catch (FormatException)
                {
                    return BadRequest("Birthdate is in wrong format");
                }
            }

            //if messed up birthday results is less than 18, simply don't update
            if (DateTime.Now.Year - birthdate.Year < 18)
            {
                return Ok();
            }

            //get years difference
            int yearsDiff = UserModule.getUserAge(birthdate);

            //if less than 18
            if (yearsDiff < 18)
            {
                return BadRequest("Must be at least 18 years old");
            }

            //update age 
            user.age = yearsDiff;

            //update birthdate
            user.birthdate = birthdate;
        }

        //save changes
        IdentityModule.SafelySaveChanges(_context);

        //return ok
        return Ok();
    }

    //FRIEND ENDPOINTS

    /// <summary>
    /// get current friend user information
    /// </summary>
    /// <returns></returns>
    [HttpGet("Friends/GetCurrentFriendUserInformation")]
    [Authorize]
    public async Task<IActionResult> GetCurrentFriendUserInformation()
    {
        //get the current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //attempt to get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User cannot be found");
        }

        //master container for json information
        JObject masterContainer = new JObject();

        //create user information container
        JObject userInformationContainer = new JObject();

        //add user information
        userInformationContainer.Add(new JProperty("name", user.name));
        userInformationContainer.Add(new JProperty("gender", user.gender));
        userInformationContainer.Add(new JProperty("birthdate", user.birthdate.ToString("MM/dd/yyyy")));
        userInformationContainer.Add(new JProperty("description", friendUser.description));
        userInformationContainer.Add(new JProperty("shown", friendUser.shown));

        //get profile image uris
        JArray profileImageUrisArray = new JArray();

        if (friendUser.profile_image_0_active)
        {
            profileImageUrisArray.Add(AzureBlobModule.getFriendUserProfileImageUrl(user.Id, 0));
        }
        if (friendUser.profile_image_1_active)
        {
            profileImageUrisArray.Add(AzureBlobModule.getFriendUserProfileImageUrl(user.Id, 1));
        }
        if (friendUser.profile_image_2_active)
        {
            profileImageUrisArray.Add(AzureBlobModule.getFriendUserProfileImageUrl(user.Id, 2));
        }

        userInformationContainer.Add(new JProperty("profile_image_uris", profileImageUrisArray));

        //add location
        JObject locationContainer = new JObject();

        locationContainer.Add(new JProperty("latitude", friendUser.location.Y));
        locationContainer.Add(new JProperty("longitude", friendUser?.location.X));

        userInformationContainer.Add(new JProperty("location", locationContainer));

        //add attributes
        List<string> attributes = friendUser.attributes.Select(a => a.name).ToList();

        JArray attributesListContainer = new JArray();

        foreach (string a in attributes)
        {
            attributesListContainer.Add(a);
        }

        userInformationContainer.Add(new JProperty("attributes", attributesListContainer));

        //get points
        List<FriendUserPoint> points = friendUser.points.ToList();

        //sort points by order added index
        points.Sort(delegate (FriendUserPoint point1, FriendUserPoint point2) { return (point1.orderAddedIndex.CompareTo(point2.orderAddedIndex)); });

        JArray pointsListContainer = new JArray();

        JObject pointContainer;

        foreach(FriendUserPoint point in points)
        {
            pointContainer = new JObject();

            //add values
            pointContainer.Add(new JProperty("id", point.id)); 
            pointContainer.Add(new JProperty("caption", point.caption));
            pointContainer.Add(new JProperty("image_uri", ""));

            //add to array
            pointsListContainer.Add(pointContainer);
        }

        userInformationContainer.Add(new JProperty("points", pointsListContainer));

        masterContainer.Add(new JProperty("user_information", userInformationContainer));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// update friend user information
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Friends/UpdateUserInformation")]
    [Authorize]
    public async Task<IActionResult> UpdateFriendUserInformation(UpdateFriendUserInformationModel model)
    {
        //get the current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }
        
        //attempt to get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User cannot be found");
        }

        //create master container
        JObject masterContainer = new JObject();

        JArray newPointIdsContainer = new JArray();

        //for later use in points
        List<string> currentPoints = null;

        //remove old points
        if (model.new_points != null)
        {

            byte pointCount = 0;

            //only get the captions of the points if we are going to need them later
            if (model.points != null)
            {
                //get list of current points
                currentPoints = friendUser.points.Select(p => p.id).ToList();

                //get count
                pointCount = (byte)currentPoints.Count;
            }
            else
            {
                pointCount = (byte)friendUser.points.Count();
            }

            //temp
            string pointId;

            foreach (string pointCaption in model.new_points)
            {
                //create new point
                pointId = UserModule.addFriendUserPoint(_context, friendUser, pointCaption, pointCount);

                //add point id
                newPointIdsContainer.Add(pointId);
            }
        }

        masterContainer.Add(new JProperty("new_point_ids", newPointIdsContainer));

        //remove old points
        if (model.points != null)
        {
            //add new attributes, remove old ones
            //get current points as string list
            if (currentPoints == null)
            {
                currentPoints = friendUser.points.Select(p => p.id).ToList();
            }

            //get list or points that have been removed
            List<string> removedPoints = currentPoints.Where(p => !model.points.Contains(p)).ToList();

            //remove removed points
            foreach (string pointId in removedPoints)
            {
                FriendUserPoint point = friendUser.points.FirstOrDefault(p => p.id == pointId);

                UserModule.removeFriendUserPoint(_context, friendUser, point);
            }
        }

        //update the application user information

        if (model.name != null)
        {
            user.name = model.name.ToLower();
        }

        if (model.gender != null)
        {
            if (!GlobalProperties.GENDER_OPTIONS.Contains(model.gender))
            {
                return BadRequest("Gender is not valid");
            }

            user.gender = model.gender;
        }

        if (model.birthdate != null)
        {
            DateTime birthdate;

            try
            {
                birthdate = DateTime.ParseExact(model.birthdate, "d/M/yyyy", null);
            }
            catch (FormatException)
            {
                try
                {
                    birthdate = DateTime.ParseExact(model.birthdate, "dd/MM/yyyy HH:mm", null);
                }
                catch (FormatException)
                {
                    return BadRequest("Birthdate is in wrong format");
                }
            }

            //if messed up birthday results is less than 18, simply don't update
            if (DateTime.Now.Year - birthdate.Year < 18)
            {
                return Ok();
            }

            //get years difference
            int yearsDiff = UserModule.getUserAge(birthdate);

            //if less than 18
            if (yearsDiff < 18)
            {
                return BadRequest("Must be at least 18 years old");
            }

            //update age 
            user.age = yearsDiff;

            //update birthdate
            user.birthdate = birthdate;
        }

        //update friend user information

        //update friend user values

        if (model.description != null)
        {
            friendUser.description = model.description;
        }

        if (model.shown.HasValue)
        {
            friendUser.shown = model.shown.Value;

            //check and set isActive to correct value
            UserModule.checkIsActiveConditionsFriendUser(friendUser);
        }

        if (model.attributes != null)
        {
            //add new attributes, remove old ones
            //get current attributes as string list
            List<string> currentAttributes = friendUser.attributes.Select(a => a.name).ToList();

            //get list of new attributes
            List<string> newAttributes = model.attributes.Where(a => !currentAttributes.Contains(a)).ToList();

            //get list or attributes that have been removed
            List<string> removedAttributes = currentAttributes.Where(a => !model.attributes.Contains(a)).ToList();

            //remove removed attributes
            foreach (string attributeName in removedAttributes)
            {
                FriendUserAttribute attribute = friendUser.attributes.FirstOrDefault(a => a.name == attributeName);

                UserModule.removeFriendUserAttribute(_context, friendUser, attribute);
            }

            //temp
            string validAttribute;

            //add attributes
            foreach (string attribute in newAttributes)
            {
                validAttribute = UserModule.getValidAttributeName(attribute);

                UserModule.addFriendUserAttribute(_context, friendUser, validAttribute);
            }
        }

        if (model.location != null)
        {
            friendUser.location = new Point(model.location.longitude, model.location.latitude) { SRID = 4326 };
        }

        //remove old points

        //return new point ids

        //save changes
        IdentityModule.SafelySaveChanges(_context);

        //get the id's of the new points

        //return ok
        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    //for a given point id, uploads or adds image in azure blobs
    //intended to also be used after creating or updating fried user when it returns the new point ids
      //thr returned point ids are in the order that they were added, in the react native app, use the index
      //of it's new points added as the key for the index of the returned new point ids
    /*public async Task<IActionResult> AddOrUpdateFriendUserPointImage()
    {

    }*/

    /// <summary>
    /// add atrribute to friend user's attributes
    /// </summary>
    /// <param name="model"></param>
    /// <returns>200 ok with attribute in body</returns>
    [HttpPost("Friends/AddAttribute")]
    [Authorize]
    public async Task<IActionResult> AddFriendUserAttribute(AttributeModel model)
    {
        //if attribute is nullf
        if (model.attribute == null)
        {
            return BadRequest("attribute is null");
        }

        //get the current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        //get valid name
        string attributeName = UserModule.getValidAttributeName(model.attribute);

        // check if attribute already exists
        FriendUserAttribute attribute = _context.FriendUserAttributes.Find(attributeName);

        //if the attribute was not found
        if (attribute == null)
        {
            //create the attribute
            attribute = new FriendUserAttribute { name = attributeName };

            //add the attribute to the 
            _context.FriendUserAttributes.Add(attribute);
        }

        //if user already contains attribute
        if (friendUser.attributes.FirstOrDefault(c => c.name == attributeName, null) != null)
        {
            //return ok
            return Ok("Attribute has already been added");
        }

        //add attribute to list of attributes for user
        friendUser.attributes.Add(attribute);

        //add user to list of users for that attribute
        attribute.users.Add(user.friendUser);

        //save changes in context
        IdentityModule.SafelySaveChanges(_context);

        //return the attribute name (in case it was corrected)

        //create jobject
        JObject jsonObject = new JObject();

        //add attribute as property to jobject
        jsonObject.Add(new JProperty("attribute_name", attributeName));

        return Content(jsonObject.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    [HttpPost("Friends/UploadProfileImage")]
    [Authorize]
    public async Task<IActionResult> UploadFriendUserProfileImage([FromForm] FriendUserUploadProfileImageModel model)
    {
        //get the current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //attempt to get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User cannot be found");
        }

        Pair<bool, string> result = await AzureBlobModule.uploadFriendUserProfileImage(user, friendUser, _context, model.image, model.num);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// remove atrribute from friend user's attributes
    /// </summary>
    /// <param name="model"></param>
    /// <returns>200 ok</returns>
    [HttpPost("Friends/RemoveAttribute")]
    [Authorize]
    public async Task<IActionResult> RemoveAttribute(AttributeModel model)
    {
        //get the current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //find the attribute
        FriendUserAttribute attribute = user.friendUser.attributes.FirstOrDefault(a => a.name == model.attribute);

        //if attribute was not found
        if (attribute == null)
        {
            return BadRequest("attribute not found");
        }

        //remove user from list of users for the attribute
        attribute.users.Remove(user.friendUser);

        //remove the attribute
        user.friendUser.attributes.Remove(attribute);

        //if there is nobody left in the list of users for the attribute 
        if (attribute.users.Count == 0)
        {
            //delete the attribute itself
            _context.Remove(attribute);
        }

        return Ok();
    }

    /// <summary>
    /// find like friend users, infinite scrolling, with filters
    /// </summary>
    /// <param name="model"></param>
    /// <returns>json content containing the users as well as other statistics with 200 ok</returns>
    [HttpPost("Friends/SearchUsers")]
    [Authorize]
    public async Task<IActionResult> SearchFriendUsers(GetFriendUsersModel model)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //now implement for infinite scrolling

        //shuffle list of users (to randomize)
        // users.Shuffle();

        //location
        Point searchLocation;

        //get friend user
        FriendUser friendUser = _context.FriendUsers.Find(user.Id);

        //if they did not provided a location, use the location of the friend user
        if (model.location == null || (model.location.latitude == 360 && model.location.longitude == 360))
        {
            if (friendUser == null)
            {
                //add to ERROR LOG

                return BadRequest("Your user does not exist");
            }

            searchLocation = friendUser.location;
        }
        //they did provide a location, so use that
        else
        {
            searchLocation = new Point(model.location.longitude, model.location.latitude) { SRID = 4326 };
        }

        //get list of users from query
        //List<BasicQueryUser> users = UserModule.searchForFriendUsers(friendUser, _context, searchLocation, model.radius, model.pageSize, model.pageNumber, model.minimum_age, model.maximum_age, model.gender, model.attributes);
        List<FriendUserSearchModel> users = UserModule.searchForFriendUsers(friendUser, _context, searchLocation, model.radius, model.page_size, model.page_number, model.minimum_age, model.maximum_age, model.gender, model.attributes);

        //create master jobject
        JObject masterContainer = new JObject();

        //create sub array for users
        JArray jsonUsers = new JArray();

        //temp variables
        ApplicationUser tempApplicationUser;

        foreach (FriendUserSearchModel tempUser in users)
        {
            //jobject for user
            JObject jsonFriendUser = new JObject();

            //get applicationuser by applicationid (primary key)
            //friendUser = _context.FriendUsers.Find(tempUser.applicationID);

            //add application user id as id
            jsonFriendUser.Add(new JProperty("id", tempUser.user.Id));

            //add  name
            jsonFriendUser.Add(new JProperty("name", tempUser.user.name));

            //add type
            jsonFriendUser.Add(new JProperty("type", "person"));

            //get distance to nearest mile
            jsonFriendUser.Add(new JProperty("distance", Math.Floor(LocationModule.haversineDistance(searchLocation, tempUser.location))));

            //add age
            jsonFriendUser.Add(new JProperty("age", tempUser.user.age));

            //add description
            jsonFriendUser.Add(new JProperty("description", tempUser.description));

            //get profile image uri
            string profileImageUri = "";

            for (short i = 0; i < 3; i++)
            {
                if (tempUser.profile_images_active[i])
                {
                    profileImageUri = AzureBlobModule.getFriendUserProfileImageUrl(tempUser.user.Id, i);
                    break;
                }
            }

            jsonFriendUser.Add(new JProperty("profile_image_uri", profileImageUri));

            //add user json object to array
            jsonUsers.Add(jsonFriendUser);
        }

        //create json object for statistics
        JObject jsonStats = new JObject();

        //user count (number of users got)
        jsonStats.Add(new JProperty("user_count", (users.Count)));

        //add sub jobjects to master container

        //add list of users
        masterContainer.Add(new JProperty("users", jsonUsers));

        //add statistics
        masterContainer.Add(new JProperty("statistics", jsonStats));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// sarch like friend users for the map
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Friends/SearchUsersMap")]
    [Authorize]
    public async Task<IActionResult> SearchFriendUsersMap(GetFriendUserMapModel model)
    {
        //get user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //perform algorithm


        //get information from model

        //location
        Point searchLocation;

        //get the search radius
        //double searchRadius = LocationModule.haversineDistance(new Point(model.location.latitude, model.location.latitude + model.width), new Point(model.location.longitude, model.location.longitude + model.height));
        double searchRadius = model.radius;

        //get friend user
        FriendUser friendUser = _context.FriendUsers.Find(user.Id);

        //if they did not provided a location, use the location of the frienduser
        if (model.location == null || (model.location.latitude == 360 && model.location.longitude == 360))
        {
            if (friendUser == null)
            {
                //add to ERROR LOG

                return BadRequest("Your user does not exist");
            }

            searchLocation = friendUser.location;
        }
        //they did provide a location, so use that
        else
        {
            searchLocation = new Point(model.location.longitude, model.location.latitude) { SRID = 4326 };
        }

        //compile into json

        //get list of users from query
        List<Point> users = UserModule.searchForFriendUsersMap(friendUser, _context, searchLocation, searchRadius, model.minimum_age, model.maximum_age, model.gender, model.attributes);

        //list of groups
        List<GroupPoint> groups = new List<GroupPoint>();

        //determine the r factor
        double rFactor = Math.Min(5.0, (3.0 / 8.0) * Math.Log2(Math.Max(users.Count, 5.0) / 5.0) + 2.0);

        //get group radius
        double groupRadius = searchRadius / rFactor;

        //temp variables
        GroupPoint closest = null;
        double distance = 0.0;

        foreach(Point userPoint in users)
        {
            closest = null;

            //it will never excede this value, as it is larger than the radius of search
            distance = searchRadius + 1.0;

            //get the cloest group point
            foreach (GroupPoint groupPoint in groups)
            {

                if (groupPoint.distance(userPoint.Y, userPoint.X) <= distance)
                {
                    closest = groupPoint;
                    distance = groupPoint.distance(userPoint.Y, userPoint.X);
                }
            }

            //if point is within our group radius
            if (closest != null && distance < groupRadius)
            {
                //join cloest group
                closest.addPoint(userPoint.Y, userPoint.X);
            }
            else
            {
                //make a new group
                groups.Add(new GroupPoint(userPoint.Y, userPoint.X));
            }
        }

        //all groups have been made, now time to merge close groups
        //reverse for loop so we can remove elements if need be in sequential order
        for (int i = groups.Count - 1; i >= 0; i--)
        {
            closest = null;
            distance = searchRadius + 1.0;

            //find the cloest other group
            for(int j = groups.Count - 1; j >= 0; j--)
            {
                //if it is cloesr than the other point and not itself
                if (groups[j].distance(groups[i]) < distance && i != j)
                {
                    closest = groups[j];
                    distance = groups[j].distance(groups[i]);
                }
            }

            //if there is a cloest group and is in the groupRadius
            if (closest != null && distance < groupRadius)
            {
                //merge the groups
                closest.mergeGroup(groups[i]);

                //get rid of the old group point
                groups.RemoveAt(i);
            }
        }

        //for protecting peopls's privacy, remove groups with only one people
        for (int i = groups.Count - 1; i >= 0; i--)
        {
            if (groups[i].count == 1)
            {
                groups.RemoveAt(i);
            }
        }

        //create master jobject
        JObject masterContainer = new JObject();

        //create sub array for users
        JArray jsonGroups= new JArray();

        foreach (GroupPoint groupPoint in groups)
        {
            //jobject for group
            JObject jsonGroupPoint = new JObject();

            //add latitude
            jsonGroupPoint.Add(new JProperty("latitude", groupPoint.coordinate.latitude));

            //add longitude
            jsonGroupPoint.Add(new JProperty("longitude", groupPoint.coordinate.longitude));

            //add count
            jsonGroupPoint.Add(new JProperty("count", groupPoint.count));

            //add user json object to array
            jsonGroups.Add(jsonGroupPoint);
        }

        //create json object for statistics
        JObject jsonStats = new JObject();

        //group count (number of users got)
        jsonStats.Add(new JProperty("group_count", (groups.Count)));

        //add type
        jsonStats.Add(new JProperty("type", "people"));

        //add list of users
        masterContainer.Add(new JProperty("groups", jsonGroups));

        //add statistics
        masterContainer.Add(new JProperty("statistics", jsonStats));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// get basic user information, intended for viewing other users
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpGet("Friends/GetBasicUserInformation")]
    [Authorize]
    public async Task<IActionResult> GetBasicFriendUserInformation(string id)
    {
        //attempt to get other user
        ApplicationUser otherUser = await _userManager.FindByIdAsync(id);

        //if otheruser does not exists
        if (otherUser == null)
        {
            return BadRequest("User does not exist");
        }

        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get friend user
        FriendUser friendUser = user.friendUser;

        //get friend user
        FriendUser friendOtherUser = otherUser.friendUser;

        //if otheruser does not want to be shown, say does not exist
        if (!friendOtherUser.shown)
        {
            return BadRequest("User does not exist");
        }

        //check if user is blocked
        if (_context.FriendUserFriendUserBlocks.Where(b => b.friendUserFirstId == friendUser.ApplicationUserId && b.friendUserLastId == friendOtherUser.ApplicationUserId).Any())
        {
            return BadRequest("User is blocked");
        }

        //text michael that i will contact you if i need help

        //create master container
        JObject masterContainer = new JObject();

        //create user information container
        JObject userInformation = new JObject();

        //add user information
        //add username
        userInformation.Add(new JProperty("id", id));

        //add age
        userInformation.Add(new JProperty("age", otherUser.age));

        //add profile pictures

        //add name
        userInformation.Add(new JProperty("name", otherUser.name));

        //add gender
        userInformation.Add(new JProperty("gender", otherUser.gender));

        //add description
        userInformation.Add(new JProperty("description", friendOtherUser.description));

        //add profile images

        //get profile image uris
        JArray profileImageUrisArray = new JArray();

        if (friendOtherUser.profile_image_0_active)
        {
            profileImageUrisArray.Add(AzureBlobModule.getFriendUserProfileImageUrl(otherUser.Id, 0));
        }
        if (friendOtherUser.profile_image_1_active)
        {
            profileImageUrisArray.Add(AzureBlobModule.getFriendUserProfileImageUrl(otherUser.Id, 1));
        }
        if (friendOtherUser.profile_image_2_active)
        {
            profileImageUrisArray.Add(AzureBlobModule.getFriendUserProfileImageUrl(otherUser.Id, 2));
        }

        userInformation.Add(new JProperty("profile_image_uris", profileImageUrisArray));

        //add attributes
        //create attributes array
        JArray attributesArrayContainer = new JArray();

        //get attributes from friend
        var attributes = friendOtherUser.attributes.Select(c => new { c.name }).ToList();

        foreach (var attr in attributes)
        {
            attributesArrayContainer.Add(attr.name.ToString());
        }

        //add attributes
        userInformation.Add(new JProperty("attributes", attributesArrayContainer));

        //add points
        List<FriendUserPoint> points = friendUser.points.ToList();

        //sort points by add index
        points.Sort(delegate(FriendUserPoint point1, FriendUserPoint point2) { return (point1.orderAddedIndex.CompareTo(point2.orderAddedIndex)); });

        JArray pointsListContainer = new JArray();

        JObject pointContainer;

        foreach (FriendUserPoint point in points)
        {
            pointContainer = new JObject();

            //add values
            pointContainer.Add(new JProperty("id", point.id));
            pointContainer.Add(new JProperty("caption", point.caption));
            pointContainer.Add(new JProperty("image_uri", ""));

            //add to array
            pointsListContainer.Add(pointContainer);
        }

        userInformation.Add(new JProperty("points", pointsListContainer));

        //add distance
        //get distance
        int distance = (int)Math.Floor(LocationModule.haversineDistance(friendUser.location, friendOtherUser.location));

        //add distance
        userInformation.Add(new JProperty("distance", distance));

        //add user information to master container
        masterContainer.Add(new JProperty("user_information", userInformation));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    //ACTIVITY ENDPOINTS

    /// <summary>
    /// add an activity to the current friend user
    /// </summary>
    /// <returns></returns>
    [HttpPost("Friends/CreateActivity")]
    [Authorize]
    public async Task<IActionResult> CreateFriendActivity(CreateFriendActivityModel model)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        DateTime date;

        try
        {
            date = DateTime.ParseExact(model.date_time, "d/M/yyyy H:m", null);
        }
        catch (FormatException)
        {
            try
            {
                date = DateTime.ParseExact(model.date_time, "dd/MM/yyyy HH:mm", null);
            }
            catch (FormatException)
            {
                return BadRequest("datetime is in wrong format");
            }
        }

        JOIN_INVITATION_METHODS invitationMethod;

        if (!Enum.TryParse<JOIN_INVITATION_METHODS>(model.invitation_method.ToUpper(), out invitationMethod))
        {
            return BadRequest("Invalid invitation method type");
        }

        //get friend user
        FriendUser friendUser = user.friendUser;

        //create new friend activity 
        //Note: if there already is a activity with same name, add new one anyways, they are identified by id, not name.
        FriendActivity friendActivity = new FriendActivity();

        //create master container
        JObject masterContainer = new JObject();

        //set the values
        friendActivity.name = model.title;
        friendActivity.dateTime = date;
        friendActivity.isPhysical = model.is_physical;
        friendActivity.minimumage = model.minimum_age;
        friendActivity.maximumAge = model.maximum_age;
        friendActivity.invitationMethod = invitationMethod;
        friendActivity.admins.Add(friendUser);

        //temp
        string validAttribute;

        //add attributes
        foreach (string attribute in model.attributes)
        {
            validAttribute = UserModule.getValidAttributeName(attribute);

            UserModule.addFriendActivityAttribute(_context, friendActivity, validAttribute);
        }
        
        //set description
        if (model.description != null)
        {
            friendActivity.description = model.description;
        }

        //if given gender
        if (model.gender != null)
        {
            if (!GlobalProperties.GENDER_SELECT_OPTIONS.Contains(model.gender))
            {
                return BadRequest("Not a valid gender option");
            }

            friendActivity.gender = model.gender;
        }
        else
        {
            friendActivity.gender = "all";
        }

        //if given address
        if (model.address != null)
        {
            friendActivity.address = model.address;
        }

        //if physical activity
        if (model.is_physical)
        {
            //if target location is not given, bad request, it must be given
            if (model.target_location == null)
            {
                return BadRequest("Target location must be set");
            }
            else
            {
                friendActivity.targetLocation = new Point(model.target_location.longitude, model.target_location.latitude) { SRID = 4326 };
            }

            //if search location is not given
            if (model.search_location == null)
            {
                friendActivity.searchLocation = new Point(model.target_location.longitude, model.target_location.latitude) { SRID = 4326 };
            }
            //set to search location
            else
            {
                friendActivity.searchLocation = new Point(model.search_location.longitude, model.search_location.latitude) { SRID = 4326 };
            }
        }
        else
        {
            //if search location is given
            if (model.search_location != null)
            {
                friendActivity.searchLocation = new Point(model.search_location.longitude, model.search_location.latitude) { SRID = 4326 };
                friendActivity.targetLocation = new Point(model.search_location.longitude, model.search_location.latitude) { SRID = 4326 };
            }
            else
            {
                return BadRequest("Seaarch location must be set");
            }
        }

        //if search radius
        if (model.search_radius != null)
        {
            friendActivity.searchRadius = model.search_radius.GetValueOrDefault();
        }
        else
        {
            friendActivity.searchRadius = long.MaxValue;
        }

        //if invite cap
        if (model.invite_cap != null)
        {
            friendActivity.inviteCap = model.invite_cap.GetValueOrDefault();
        }

        //if participant cap
        if (model.participants_cap != null)
        {
            friendActivity.participantsCap = model.participants_cap.GetValueOrDefault();
        }

        //create friend activity dynamic value entry
        FriendActivityDynamicValues friendActivityDynamicValues = new FriendActivityDynamicValues { numAdmins = 1 };

        //add dynamic value to friend activity
        friendActivity.dynamicValues = friendActivityDynamicValues;

        //add friend activity to friend user
        friendUser.createdActivities.Add(friendActivity);

        //add friend activity dynamic value entry
        _context.FriendActivitiesDynamicValues.Add(friendActivityDynamicValues);

        //add it to context
        _context.FriendActivities.Add(friendActivity);

        IdentityModule.SafelySaveChanges(_context);

        JArray newPointIdsContainer = new JArray();

        //add new points
        if (model.new_points != null)
        {
            //get number current points
            byte currentPointCount = (byte)friendActivity.points.Select(p => p.id).Count();

            //temp
            string pointId;

            foreach (string pointCaption in model.new_points)
            {
                //create new point
                pointId = UserModule.addFriendActivityPoint(_context, friendActivity, pointCaption, currentPointCount);

                //add point id
                newPointIdsContainer.Add(pointId);
            }
        }

        masterContainer.Add(new JProperty("new_point_ids", newPointIdsContainer));

        //create jobject for return data

        JObject statsContainer = new JObject();

        statsContainer.Add(new JProperty("activity_id", friendActivity.id ));

        masterContainer.Add(new JProperty("stats", statsContainer));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// update friend activity
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Friends/UpdateActivity")]
    [Authorize]
    public async Task<IActionResult> UpdateFriendActivity(UpdateFriendActivityModel model)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is invalid");
        }

        //get activity
        FriendActivity friendActivity = friendUser.createdActivities.FirstOrDefault(a => a.id == model.id);

        if (friendActivity == null)
        {
            return BadRequest("Invalid and Unknown Activity Id");
        }

        //create master container
        JObject masterContainer = new JObject();

        JArray newPointIdsContainer = new JArray();

        //for later use
        List<string> currentPoints = null;

        //add new points
        if (model.new_points != null)
        {
            byte pointCount = 0;

            //only get the captions of the points if we are going to need them later
            if (model.points != null)
            {
                //get list of current points
                currentPoints = friendActivity.points.Select(p => p.id).ToList();

                //get count
                pointCount = (byte)currentPoints.Count;
            }
            else
            {
                pointCount = (byte)friendActivity.points.Count();
            }

            //temp
            string pointId;

            foreach (string pointCaption in model.new_points)
            {
                //create new point
                pointId = UserModule.addFriendActivityPoint(_context, friendActivity, pointCaption, pointCount);

                //add point id
                newPointIdsContainer.Add(pointId);
            }
        }

        masterContainer.Add(new JProperty("new_point_ids", newPointIdsContainer));

        //remove old points
        if (model.points != null)
        {
            //add new attributes, remove old ones
            //get current attributes as string list
            if (currentPoints == null)
            {
                currentPoints = friendActivity.points.Select(p => p.id).ToList();
            }

            //get list or attributes that have been removed
            List<string> removedPoints = currentPoints.Where(p => !model.points.Contains(p)).ToList();

            //remove removed points
            foreach (string pointId in removedPoints)
            {
                FriendActivityPoint point = friendActivity.points.FirstOrDefault(p => p.id == pointId);

                UserModule.removeFriendActivityPoint(_context, friendActivity, point);
            }

            //TODO update current points that have been changed
        }

        if (model.title != null)
        {
            friendActivity.name = model.title;
        }

        if (model.description != null)
        {
            friendActivity.description = model.description;
        }

        if (model.attributes != null)
        {
            //add new attributes, remove old ones
            //get current attributes as string list
            List<string> currentAttributes = friendActivity.attributes.Select(a => a.name).ToList();

            //get list of new attributes
            List<string> newAttributes = model.attributes.Where(a => !currentAttributes.Contains(a)).ToList();

            //get list or attributes that have been removed
            List<string> removedAttributes = currentAttributes.Where(a => !model.attributes.Contains(a)).ToList();

            //remove removed attributes
            foreach (string attributeName in removedAttributes)
            {
                FriendActivityAttribute attribute = friendActivity.attributes.FirstOrDefault(a => a.name == attributeName);

                UserModule.removeFriendActivityAttribute(_context, friendActivity, attribute);
            }

            //temp
            string validAttribute;

            //add attributes
            foreach (string attribute in newAttributes)
            {
                validAttribute = UserModule.getValidAttributeName(attribute);

                UserModule.addFriendActivityAttribute(_context, friendActivity, validAttribute);
            }
        }

        if (model.date_time != null)
        {
            DateTime date;

            try
            {
                date = DateTime.ParseExact(model.date_time, "d/M/yyyy H:m", null);
            }
            catch (FormatException)
            {
                try
                {
                    date = DateTime.ParseExact(model.date_time, "dd/MM/yyyy HH:mm", null);
                }
                catch (FormatException)
                {
                    return BadRequest("Birthdate is in wrong format");
                }
            }

            friendActivity.dateTime = date;

            //if too old
            if (date < DateTime.Now.AddDays(-1))
            {
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
            }
        }

        if (model.is_physical != null)
        {
            friendActivity.isPhysical = model.is_physical.GetValueOrDefault();
        }

        if (model.target_location != null)
        {
            friendActivity.targetLocation = new Point(model.target_location.longitude, model.target_location.latitude) { SRID = 4326 };
        }

        if (model.search_location != null)
        {
            friendActivity.searchLocation = new Point(model.search_location.longitude, model.search_location.latitude) { SRID = 4326 };

            if (!friendActivity.isPhysical)
            {
                friendActivity.targetLocation = new Point(model.search_location.longitude, model.search_location.latitude) { SRID = 4326 };
            }
        }

        if (model.search_radius != null)
        {
            friendActivity.searchRadius = model.search_radius.GetValueOrDefault();
        }

        if (model.address != null)
        {
            friendActivity.address = model.address;
        }

        if (model.invitation_method != null)
        {
            JOIN_INVITATION_METHODS invitationMethod;

            if (!Enum.TryParse<JOIN_INVITATION_METHODS>(model.invitation_method.ToUpper(), out invitationMethod))
            {
                return BadRequest("Invalid invitation method type");
            }

            friendActivity.invitationMethod = invitationMethod;
        }

        if (model.invite_cap != null)
        {
            friendActivity.inviteCap = model.invite_cap.GetValueOrDefault();
        }

        if (model.participants_cap != null)
        {
            friendActivity.participantsCap = model.participants_cap.GetValueOrDefault();
        }

        if (model.gender != null)
        {
            if (!GlobalProperties.GENDER_SELECT_OPTIONS.Contains(model.gender))
            {
                return BadRequest("Not a valid gender option");
            }

            friendActivity.gender = model.gender;
        }

        if (model.minimum_age != null)
        {
            friendActivity.minimumage = model.minimum_age.GetValueOrDefault();
        }

        if (model.maximum_age != null)
        {
            friendActivity.maximumAge = model.maximum_age.GetValueOrDefault();
        }

        if (model.shown.HasValue)
        {
            friendActivity.shown = model.shown.Value;

            //check if it can become active if it is currently not active
            if (!friendActivity.isActive)
            {
                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
            }
        }

        IdentityModule.SafelySaveChanges(_context);

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    //gets all accouncments made for activity (that has not been deleted by the sidetask)
    //gets gets historical, not pending, accouncements
    [HttpGet("Friends/GetActivityAnnouncements")]
    [Authorize]
    public async Task<IActionResult> FriendActivityGetAnnouncements(string activity_id)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest();
        }

        FriendActivity friendActivity = _context.FriendActivities.Find(activity_id);

        //if friend activity does not exist
        if (friendActivity == null)
        {
            return BadRequest("Friend activity does not exist");
        }

        /*//if friend user is not admin or participant in activity
        if (!friendActivity.admins.Contains(friendUser))
        {
            return BadRequest("Friend user is not admin in activity");
        }*/

        //get announcements
        List<FriendActivityAnnouncement> friendActivityAnnouncements = friendActivity.announcements.ToList();

        //create master jobject
        JObject masterContainer = new JObject();

        JArray announcementsArrayContainer = new JArray();

        foreach (FriendActivityAnnouncement friendActivityAnnouncement in friendActivityAnnouncements)
        {
            //create message object
            JObject announcementContainer = new JObject();

            //add values
            announcementContainer.Add(new JProperty("type", Enum.GetName(MESSAGE_TYPES.ANNOUNCEMENT).ToLower()));
            announcementContainer.Add(new JProperty("announcement_id", friendActivityAnnouncement.id));
            announcementContainer.Add(new JProperty("body", friendActivityAnnouncement.message));

            //add them to array
            announcementsArrayContainer.Add(announcementContainer);
        }

        masterContainer.Add(new JProperty("announcements", announcementsArrayContainer));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// search activities
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Friends/SearchActivities")]
    [Authorize]
    public async Task<IActionResult> SearchFriendActivities(GetFriendActivitiesModel model)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //location
        Point searchLocation;

        //get friend user
        FriendUser friendUser = _context.FriendUsers.Find(user.Id);

        //if they did not provided a location, use the location of the frienduser
        if (model.location == null || (model.location.latitude == 360 && model.location.longitude == 360))
        {
            if (friendUser == null)
            {
                //add to ERROR LOG

                return BadRequest("Your user does not exist");
            }

            searchLocation = friendUser.location;
        }
        //they did provide a location, so use that
        else
        {
            searchLocation = new Point(model.location.longitude, model.location.latitude) { SRID = 4326 };
        }

        //get list of users from query
        //List<BasicQueryUser> users = UserModule.searchForFriendUsers(friendUser, _context, searchLocation, model.radius, model.pageSize, model.pageNumber, model.minimum_age, model.maximum_age, model.gender, model.attributes);
        List<FriendActivitySearchModel> activities = UserModule.searchForFriendActivities(user, friendUser, _context, searchLocation, model.radius, model.page_size, model.page_number, model.attributes, model.medium);

        //create master jobject
        JObject masterContainer = new JObject();

        //create sub array for users
        JArray jsonActivities = new JArray();

        //temp variables
        ApplicationUser tempApplicationUser;

        foreach (FriendActivitySearchModel tempActivity in activities)
        {
            //jobject for user
            JObject jsonActivity = new JObject();

            //get applicationuser by applicationid (primary key)
            //friendUser = _context.FriendUsers.Find(tempUser.applicationID);

            //add application user id as id
            jsonActivity.Add(new JProperty("id", tempActivity.id));

            //get name
            jsonActivity.Add(new JProperty("name", tempActivity.name));

            //add type
            jsonActivity.Add(new JProperty("type", "activity"));

            //get distance to nearest mile
            jsonActivity.Add(new JProperty("distance", Math.Floor(LocationModule.haversineDistance(searchLocation, tempActivity.location))));

            //return when it will occur
            jsonActivity.Add(new JProperty("date_time", UserModule.getNeatDateTime(tempActivity.dateTime)));

            //add user json object to array
            jsonActivities.Add(jsonActivity);
        }

        //create json object for statistics
        JObject jsonStats = new JObject();

        //user count (number of users got)
        jsonStats.Add(new JProperty("activity_count", (activities.Count)));

        //add sub jobjects to master container

        //add list of users
        masterContainer.Add(new JProperty("activities", jsonActivities));

        //add statistics
        masterContainer.Add(new JProperty("statistics", jsonStats));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// search activities for map
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Friends/SearchActivitiesMap")]
    [Authorize]
    public async Task<IActionResult> SearchFriendActivitiesMap(GetFriendActivitiesMapModel model)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        if (user == null)
        {
            return BadRequest();
        }

        //location
        Point searchLocation;

        //get friend user
        FriendUser friendUser = _context.FriendUsers.Find(user.Id);

        //if they did not provided a location, use the location of the frienduser
        if (model.location == null || (model.location.latitude == 360 && model.location.longitude == 360))
        {
            if (friendUser == null)
            {
                //add to ERROR LOG

                return BadRequest("Your user does not exist");
            }

            searchLocation = friendUser.location;
        }
        //they did provide a location, so use that
        else
        {
            searchLocation = new Point(model.location.longitude, model.location.latitude) { SRID = 4326 };
        }

        //get list of users from query
        //List<BasicQueryUser> users = UserModule.searchForFriendUsers(friendUser, _context, searchLocation, model.radius, model.pageSize, model.pageNumber, model.minimum_age, model.maximum_age, model.gender, model.attributes);
        List<FriendActivitySearchMapModel> activities = UserModule.searchForFriendActivitiesMap(user, friendUser, _context, searchLocation, model.radius, model.attributes, model.medium);

        //create master jobject
        JObject masterContainer = new JObject();

        //create sub array for users
        JArray jsonActivities = new JArray();

        //temp variables
        ApplicationUser tempApplicationUser;

        foreach (FriendActivitySearchMapModel tempActivity in activities)
        {
            //jobject for user
            JObject jsonActivityPoint = new JObject();

            //get applicationuser by applicationid (primary key)
            //friendUser = _context.FriendUsers.Find(tempUser.applicationID);

            //add application user id as id
            jsonActivityPoint.Add(new JProperty("id", tempActivity.id));

            //get name
            jsonActivityPoint.Add(new JProperty("name", tempActivity.name));

            //add latitude
            jsonActivityPoint.Add(new JProperty("latitude", tempActivity.location.Y));

            //add longitude
            jsonActivityPoint.Add(new JProperty("longitude", tempActivity.location.X));

            //add user json object to array
            jsonActivities.Add(jsonActivityPoint);
        }

        //create json object for statistics
        JObject jsonStats = new JObject();

        //user count (number of users got)
        jsonStats.Add(new JProperty("activities_count", (activities.Count)));

        //add type
        jsonStats.Add(new JProperty("type", "activity"));


        //add sub jobjects to master container

        //add list of users
        masterContainer.Add(new JProperty("activities", jsonActivities));

        //add statistics
        masterContainer.Add(new JProperty("statistics", jsonStats));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// leave the activity as a participant
    /// </summary>
    /// <param name="id">activity id</param>
    /// <returns></returns>
    [HttpGet("Friends/LeaveActivityAsParticipant")]
    [Authorize]
    public async Task<IActionResult> LeaveActivityAsParticipant(string id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is invalid");
        }

        //get activity
        FriendActivity friendActivity = friendUser.participatingActivities.FirstOrDefault(a => a.id  == id);

        if (friendActivity == null)
        {
            return BadRequest("Invalid and Unknown Activity Id");
        }

        Pair<bool, string> result = UserModule.leaveActivityAsParticipant(_context, friendUser, friendActivity);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// remove current user from said participating or created friend activity
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("Friends/LeaveActivityAsUser")]
    [Authorize]
    public async Task<IActionResult> LeaveActivityAsUser(string id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is invalid");
        }

        //get activity
        FriendActivity friendActivity = _context.FriendActivities.Find(id);

        if (friendActivity == null)
        {
            return BadRequest("Invalid and Unknown Activity Id");
        }

        Pair<bool, string> result;

        //if admin
        if (friendUser.participatingActivities.Contains(friendActivity))
        {
            result = UserModule.removeFriendUserFromParticipatingFriendActivity(_context, friendUser, friendActivity);
        }
        else if (friendUser.createdActivities.Contains(friendActivity))
        {
            result = UserModule.removeFriendUserFromCurrentFriendActivity(_context, friendUser, friendActivity);
        }
        else
        {
            result = new Pair<bool, string>(false, "Friend user it not apart of this activity");
        }

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// remove current user from said participating or created friend activity
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("Friends/RemoveParticipantAsActivity")]
    [Authorize]
    public async Task<IActionResult> RemoveParticipantAsActivity(string activity_id, string user_id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is invalid");
        }

        FriendUser otherFriendUser = _context.FriendUsers.Find(user_id);

        if (otherFriendUser == null)
        {
            return BadRequest("Other Friend user was not found");
        }

        //get activity
        FriendActivity friendActivity = _context.FriendActivities.Find(activity_id);

        if (friendActivity == null)
        {
            return BadRequest("Invalid and Unknown Activity Id");
        }

        Pair<bool, string> result;

        //check if current user is admin in activity
        if (friendUser.createdActivities.Contains(friendActivity))
        {
            if (otherFriendUser.participatingActivities.Contains(friendActivity))
            {
                result = UserModule.removeFriendUserFromParticipatingFriendActivity(_context, otherFriendUser, friendActivity);

                if (!result.First)
                {
                    return BadRequest(result.Second);
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return BadRequest("Other Friend user is not a participant in the activity");
            }
        }
        else
        {
            return BadRequest("Friend user is not admin in activity");
        }
    }

    //get admins primary information with image links
    //make page searchable (so load x at a time)

    //get participants primary information with image links
    //make page searchable (so load x at a time)

    //join event (participant) (send invite)

    //accept invite (add it to now participant participationactivities events)
    //send notification


    //remove current user from it's created activities (or delete if only one)
    //remove activity from participants p.parcitpantactivities

    /// <summary>
    /// get list of groups and activities for the current user
    /// </summary>
    /// <returns></returns>
    [HttpGet("Friends/GetManagedItems")]
    [Authorize]
    public async Task<IActionResult> GetManagedItems()
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        //get created activities
        List<FriendActivity> friendActivities = friendUser.createdActivities.ToList();
        friendActivities.AddRange(friendUser.participatingActivities.ToList());

        //create json lists
        JObject masterContainer = new JObject();

        JArray listContainer = new JArray();

        JObject tempActivity;
        JObject tempInvitation;

        //for each activity
        foreach (FriendActivity friendActivity in friendActivities)
        {
            tempActivity = new JObject();

            //add values from activity to json object
            tempActivity.Add(new JProperty("title", friendActivity.name));
            tempActivity.Add(new JProperty("id", friendActivity.id));
            tempActivity.Add(new JProperty("type", "activity"));
            tempActivity.Add(new JProperty("date_time", UserModule.getNeatDateTime(friendActivity.dateTime)));
            tempActivity.Add(new JProperty("description", friendActivity.description));

            //add jobject for friend activity to list array
            listContainer.Add(tempActivity);
        }

        /*
        List<FriendUserToFriendActivityInvitation> friendUserToFriendActivityInvitations = friendUser.givenFriendUserToFriendActivityInvitations.OrderBy(i => i.timeStamp).ToList();

        foreach (FriendUserToFriendActivityInvitation invitation in friendUserToFriendActivityInvitations)
        {

            tempInvitation = new JObject();

            //add values from activity to json object
            tempInvitation.Add(new JProperty("title", "Invitation to " + invitation.invitee.name));
            tempInvitation.Add(new JProperty("id", invitation.id));
            tempInvitation.Add(new JProperty("type", "invitation"));

            //add jobject for friend activity to list array
            listContainer.Add(tempInvitation);
        }*/

        //add to master container
        masterContainer.Add(new JProperty("list", listContainer));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    [HttpGet("Friends/GetCreatedItems")]
    [Authorize]
    public async Task<IActionResult> GetCreatedItems()
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        //get created activities
        List<FriendActivity> friendActivities = friendUser.createdActivities.ToList();

        //create json lists
        JObject masterContainer = new JObject();

        JArray listContainer = new JArray();

        JObject tempActivity;
        JObject tempInvitation;

        //for each activity
        foreach (FriendActivity friendActivity in friendActivities)
        {
            tempActivity = new JObject();

            //add values from activity to json object
            tempActivity.Add(new JProperty("title", friendActivity.name));
            tempActivity.Add(new JProperty("id", friendActivity.id));
            tempActivity.Add(new JProperty("type", "activity"));
            tempActivity.Add(new JProperty("date_time", UserModule.getNeatDateTime(friendActivity.dateTime)));
            tempActivity.Add(new JProperty("description", friendActivity.description));

            //add jobject for friend activity to list array
            listContainer.Add(tempActivity);
        }

        //add to master container
        masterContainer.Add(new JProperty("list", listContainer));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// get full information about activity current user is admin in
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Friends/GetActivityInformation")]
    [Authorize]
    public async Task<IActionResult> GetFriendActivityInformation(string id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        //get friend activity
        FriendActivity friendActivity = friendUser.createdActivities.FirstOrDefault(a => a.id == id);

        if (friendActivity == null)
        {
            return BadRequest("Friend User is not an admin in this activity");
        }

        //create master container
        JObject masterContainer = new JObject();

        JObject activityInformationContainer = new JObject();

        //set the values
        activityInformationContainer.Add(new JProperty("title", friendActivity.name));
        activityInformationContainer.Add(new JProperty("description", friendActivity.description));
        activityInformationContainer.Add(new JProperty("date", friendActivity.dateTime.ToString("MM/dd/yyyy H:mm")));
        activityInformationContainer.Add(new JProperty("is_physical", friendActivity.isPhysical));
        activityInformationContainer.Add(new JProperty("minimum_age", friendActivity.minimumage));
        activityInformationContainer.Add(new JProperty("maximum_age", friendActivity.maximumAge));
        activityInformationContainer.Add(new JProperty("gender", friendActivity.gender));
        activityInformationContainer.Add(new JProperty("invitation_type", friendActivity.invitationMethod.ToString().ToLower()));
        activityInformationContainer.Add(new JProperty("invite_cap", friendActivity.inviteCap));
        activityInformationContainer.Add(new JProperty("participants_cap", friendActivity.participantsCap));
        activityInformationContainer.Add(new JProperty("shown", friendActivity.shown));

        JObject locationContainer = new JObject();
        locationContainer.Add(new JProperty("latitude", friendActivity.targetLocation.Y));
        locationContainer.Add(new JProperty("longitude", friendActivity.targetLocation.X));

        activityInformationContainer.Add(new JProperty("target_location", locationContainer));

        locationContainer = new JObject();
        locationContainer.Add(new JProperty("latitude", friendActivity.searchLocation.Y));
        locationContainer.Add(new JProperty("longitude", friendActivity.searchLocation.X));

        activityInformationContainer.Add(new JProperty("search_location", locationContainer));

        //attributes only to be added if they are a participant in the activity
        if (friendUser.participatingActivities.Contains(friendActivity))
        {
            activityInformationContainer.Add(new JProperty("address", friendActivity.address));
        }

        //get attributes
        List<string> attributes = friendActivity.attributes.Select(a => a.name).ToList();

        JArray attributesArray = new JArray();

        //add them to container
        foreach (string attributeName in attributes)
        {
            attributesArray.Add(attributeName);
        }

        activityInformationContainer.Add(new JProperty("attributes", attributesArray));

        //add points
        List<FriendActivityPoint> points = friendActivity.points.ToList();

        //sort points by add index
        points.Sort(delegate (FriendActivityPoint point1, FriendActivityPoint point2) { return (point1.orderAddedIndex.CompareTo(point2.orderAddedIndex)); });

        JArray pointsListContainer = new JArray();

        JObject pointContainer;

        foreach (FriendActivityPoint point in points)
        {
            pointContainer = new JObject();

            //add values
            pointContainer.Add(new JProperty("id", point.id));
            pointContainer.Add(new JProperty("caption", point.caption));
            pointContainer.Add(new JProperty("image_uri", ""));

            //add to array
            pointsListContainer.Add(pointContainer);
        }

        activityInformationContainer.Add(new JProperty("points", pointsListContainer));

        //if given address
        if (friendActivity.address != null)
        {
            activityInformationContainer.Add(new JProperty("address", friendActivity.address));
        }
        else
        {
            activityInformationContainer.Add(new JProperty("address", ""));
        }

        //if physical activity
        if (friendActivity.isPhysical)
        {
            int distance = (int)Math.Floor(LocationModule.haversineDistance(friendUser.location, friendActivity.targetLocation));

            activityInformationContainer.Add(new JProperty("distance", distance));
        }

        //add activity information container to master container
        masterContainer.Add(new JProperty("activity_information", activityInformationContainer));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// get basic information about activity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Friends/GetBasicActivityInformation")]
    [Authorize]
    public async Task<IActionResult> GetBasicFriendActivityInformation(string id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        //get friend activity
        FriendActivity friendActivity = _context.FriendActivities.Find(id);

        if (friendActivity == null)
        {
            return BadRequest("Friend Activity could not be found");
        }

        //check if user is blocked
        if (_context.FriendActivityFriendUserBlocks.Where(b => b.friendUserId == friendUser.ApplicationUserId && b.friendActivityId == friendActivity.id).Any())
        {
            return BadRequest("User is blocked from activity");
        }

        //create master container
        JObject masterContainer = new JObject();

        JObject activityInformationContainer = new JObject();

        //set the values
        activityInformationContainer.Add(new JProperty("title", friendActivity.name));
        activityInformationContainer.Add(new JProperty("description", friendActivity.description));
        activityInformationContainer.Add(new JProperty("date", UserModule.getNeatDateTime(friendActivity.dateTime)));
        activityInformationContainer.Add(new JProperty("is_physical", friendActivity.isPhysical));

        //if physical activity
        if (friendActivity.isPhysical)
        {
            int distance = (int)Math.Floor(LocationModule.haversineDistance(friendUser.location, friendActivity.targetLocation));

            activityInformationContainer.Add(new JProperty("distance", distance));
        }

        activityInformationContainer.Add(new JProperty("invitation_type", friendActivity.invitationMethod.ToString().ToLower()));

        //get isAdmin or isParticipant
        bool isAdmin = friendUser.createdActivities.Contains(friendActivity);
        bool isParticipant = friendUser.participatingActivities.Contains(friendActivity);
        bool isInActivity = isAdmin || isParticipant;

        //add if admin or participant
        activityInformationContainer.Add(new JProperty("is_admin", isAdmin));
        activityInformationContainer.Add(new JProperty("is_participant", isParticipant));
        activityInformationContainer.Add(new JProperty("is_in_activity", isInActivity));

        //add target location
        JObject locationObject = new JObject();

        locationObject.Add(new JProperty("latitude", friendActivity.targetLocation.Y));
        locationObject.Add(new JProperty("longitude", friendActivity.targetLocation.X));

        activityInformationContainer.Add(new JProperty("location", locationObject));

        //add if apart of activity
        if (isInActivity)
        {
            //if given address
            if (friendActivity.address != null)
            {
                activityInformationContainer.Add(new JProperty("address", friendActivity.address));
            }
            else
            {
                activityInformationContainer.Add(new JProperty("address", ""));
            }
        }

        //get attributes
        List<string> attributes = friendActivity.attributes.Select(a => a.name).ToList();

        JArray attributesArray = new JArray();

        //add them to container
        foreach(string attributeName in attributes)
        {
            attributesArray.Add(attributeName);
        }

        activityInformationContainer.Add(new JProperty("attributes", attributesArray));

        //add points
        List<FriendActivityPoint> points = friendActivity.points.ToList();

        //sort points by add index
        points.Sort(delegate (FriendActivityPoint point1, FriendActivityPoint point2) { return (point1.orderAddedIndex.CompareTo(point2.orderAddedIndex)); });

        JArray pointsListContainer = new JArray();

        JObject pointContainer;

        foreach (FriendActivityPoint point in points)
        {
            pointContainer = new JObject();

            //add values
            pointContainer.Add(new JProperty("id", point.id));
            pointContainer.Add(new JProperty("caption", point.caption));
            pointContainer.Add(new JProperty("image_uri", ""));

            //add to array
            pointsListContainer.Add(pointContainer);
        }

        activityInformationContainer.Add(new JProperty("points", pointsListContainer));

        //add dynamic information
        activityInformationContainer.Add(new JProperty("num_members", friendActivity.dynamicValues.numParticipants + friendActivity.dynamicValues.numAdmins));

        //add activity information container to master container
        masterContainer.Add(new JProperty("activity_information", activityInformationContainer));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }


    /// <summary>
    /// get primary information for groups and activities for the current user
    /// </summary>
    /// <returns></returns>
    [HttpPost("Friends/GetPrimaryActivitiesAndGroups")]
    [Authorize]
    public async Task<IActionResult> GetPrimaryFriendActivitiesAndGroups(int id)
    {
        return NotFound();
    }

    /// <summary>
    /// get participants for activity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Friends/ActivityGetParticipants")]
    [Authorize]
    public async Task<IActionResult> FriendActivityGetParticipants(string id)
    {
        //get id, title, and other basic information (like date and time for activity)
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        //get friend activity
        FriendActivity friendActivity = _context.FriendActivities.Find(id);

        //get participants
        List<FriendUser> participants = friendActivity.participants.ToList();

        //get list of blocked TODO

        //create master container
        JObject masterContainer = new JObject();

        //create participants container
        JArray participantsContainer = new JArray();

        foreach (FriendUser participantFriend in participants)
        {
            //get user
            ApplicationUser participantUser = participantFriend.user;

            JObject participantContainer = new JObject();

            //add id
            participantContainer.Add(new JProperty("id", participantUser.Id));

            //add first name
            participantContainer.Add(new JProperty("name", participantUser.name));

            //add age
            participantContainer.Add(new JProperty("age", participantUser.age));

            //add distance from acitvity
            participantContainer.Add(new JProperty("distance", LocationModule.haversineDistance(participantFriend.location, friendActivity.targetLocation)));

            //add type
            participantContainer.Add(new JProperty("type", "person"));

            //get profile image uri
            string imageUri = "";

            if (participantFriend.profile_image_0_active)
            {
                imageUri = AzureBlobModule.getFriendUserProfileImageUrl(participantUser.Id, 0);
            }
            else if (participantFriend.profile_image_1_active)
            {
                imageUri = AzureBlobModule.getFriendUserProfileImageUrl(participantUser.Id, 1);
            }
            else if (participantFriend.profile_image_2_active)
            {
                imageUri = AzureBlobModule.getFriendUserProfileImageUrl(participantUser.Id, 2);
            }

            //add image
            participantContainer.Add(new JProperty("image_uri", imageUri));

            //add profile image link (however it is got in the profile image manager, either we store the link or we get it because its the same)

            //add participant information to container
            participantsContainer.Add(participantContainer);
        }

        masterContainer.Add(new JProperty("participants", participantsContainer));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    /// <summary>
    /// get the admins of the friend activity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Friends/ActivityGetAdmins")]
    [Authorize]
    public async Task<IActionResult> FriendActivityGetAdmins(string id)
    {
        //get id, title, and other basic information (like date and time for activity)
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        //get friend activity
        FriendActivity friendActivity = _context.FriendActivities.Find(id);

        //get participants
        List<FriendUser> participants = friendActivity.admins.ToList();

        //create master container
        JObject masterContainer = new JObject();

        //create participants container
        JArray participantsContainer = new JArray();

        foreach (FriendUser participantFriend in participants)
        {
            //get user
            ApplicationUser participantUser = participantFriend.user;

            JObject participantContainer = new JObject();

            //add id
            participantContainer.Add(new JProperty("id", participantUser.Id));

            //add first name
            participantContainer.Add(new JProperty("name", participantUser.name));

            //add age
            participantContainer.Add(new JProperty("age", participantUser.age));

            //add distance from acitvity
            participantContainer.Add(new JProperty("distance", LocationModule.haversineDistance(participantFriend.location, friendActivity.targetLocation)));

            //add type
            participantContainer.Add(new JProperty("type", "person"));

            //get profile image uri
            string imageUri = "";

            if (participantFriend.profile_image_0_active)
            {
                imageUri = AzureBlobModule.getFriendUserProfileImageUrl(participantUser.Id, 0);
            }
            else if (participantFriend.profile_image_1_active)
            {
                imageUri = AzureBlobModule.getFriendUserProfileImageUrl(participantUser.Id, 1);
            }
            else if (participantFriend.profile_image_2_active)
            {
                imageUri = AzureBlobModule.getFriendUserProfileImageUrl(participantUser.Id, 2);
            }

            //add image
            participantContainer.Add(new JProperty("image_uri", imageUri));

            //add profile image link (however it is got in the profile image manager, either we store the link or we get it because its the same)

            //add participant information to container
            participantsContainer.Add(participantContainer);
        }

        masterContainer.Add(new JProperty("admins", participantsContainer));


        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    //INVITATION ENDPOINTS

    /// <summary>
    /// send request to freiend user to join activity
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Friends/RequestToJoinParticipantAsActivity")]
    [Authorize]
    public async Task<IActionResult> RequestToJoinParticipantAsActivity(string activity_id, string participant_id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest("User is not found");
        }

        //get current friend user
        FriendUser currentFriendUser = user.friendUser;

        if (currentFriendUser == null)
        {
            return BadRequest("Current Friend User is invalid");
        }

        //get friend user
        FriendUser friendUser = _context.FriendUsers.Find(participant_id);

        if (friendUser == null)
        {
            return BadRequest("Friend User is invalid");
        }

        //get activity
        FriendActivity friendActivity = _context.FriendActivities.Find(activity_id);

        if (friendActivity == null)
        {
            return BadRequest("Invalid and Unknown Activity Id");
        }

        Pair<bool, string> result = InvitationModule.RequestToJoinParticipantAsActivity(_context, friendActivity, friendUser, currentFriendUser);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// send request to friend user who is a participant in the activity to be promoted to admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Friends/RequestToPromoteParticipantToAdmin")]
    [Authorize]
    public async Task<IActionResult> RequestToPromoteParticipantToAdmin(string activity_id, string participant_id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get current friend user
        FriendUser currentFriendUser = user.friendUser;

        if (currentFriendUser == null)
        {
            return BadRequest("Current Friend User is invalid");
        }

        //get friend user
        FriendUser friendUser = _context.FriendUsers.Find(participant_id);

        if (friendUser == null)
        {
            return BadRequest("Friend User is invalid");
        }

        //get activity
        FriendActivity friendActivity = _context.FriendActivities.Find(activity_id);

        if (friendActivity == null)
        {
            return BadRequest("Invalid and Unknown Activity Id");
        }

        Pair<bool, string> result = InvitationModule.RequestToPromoteParticipantToAdmin(_context, friendActivity, friendUser, currentFriendUser);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// request to join activity as a participant
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("Friends/RequestToJoinActivityAsParticipant")]
    [Authorize]
    public async Task<IActionResult> RequestToJoinActivityAsParticipant(string id)
    {
        //for testing, anyone can join created activity

        //invite only
        //send back if invitation, and if so, invitation id

        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get friend user
        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is invalid");
        }

        //get activity
        FriendActivity friendActivity = _context.FriendActivities.Find(id);

        if (friendActivity == null)
        {
            return BadRequest("Invalid and Unknown Activity Id");
        }

        Pair<bool, string> result = InvitationModule.RequestToJoinActivityAsParticipant(_context, friendUser, friendActivity);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// accept an invitation
    /// </summary>
    /// <param name="id">invitation id</param>
    /// <returns></returns>
    [HttpGet("Generic/AcceptInvitation")]
    [Authorize]
    public async Task<IActionResult> AcceptInvitation(string id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get base invitation
        InvitationBase invitationBase = _context.InvitationBases.Find(id);

        if (invitationBase == null)
        {
            return BadRequest("Invitation does not exist");
        }

        Pair<bool, string> result;

        //get specific invitation
        if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
        {
            FriendUserToFriendActivityInvitation invitation = (FriendUserToFriendActivityInvitation)invitationBase;//_context.FriendUserToFriendActivityInvitations.Find(id);

            result = InvitationModule.ResolveInvitation(_context, user, invitation);
        }
        else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
        {
            FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;//_context.FriendActivityToFriendUserInvitations.Find(id);

            result = InvitationModule.ResolveInvitation(_context, user, invitation);
        }
        else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
        {
            FriendActivityToFriendUserInvitation invitation = (FriendActivityToFriendUserInvitation)invitationBase;//_context.FriendActivityToFriendUserInvitations.Find(id);

            result = InvitationModule.ResolveInvitation(_context, user, invitation);
        }
        else
        {
            return BadRequest("Unknown transfer type");
        }

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// reject invitation
    /// </summary>
    /// <param name="id">invitation id</param>
    /// <returns></returns>
    [HttpGet("Generic/RejectInvitation")]
    [Authorize]
    public async Task<IActionResult> RejectInvitation(string id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get base invitation
        InvitationBase invitationBase = _context.InvitationBases.Find(id);

        if (invitationBase == null)
        {
            return BadRequest("Invitation does not exist");
        }

        Pair<bool, string> result = InvitationModule.RemoveInvitation(_context, invitationBase, true, user);

        /*
        //get specific invitation
        if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ANONYMOUS_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
        {
            FriendUserToFriendActivityInvitation invitation = _context.FriendUserToFriendActivityInvitations.Find(id);

            if (invitation == null)
            {
                return BadRequest("Specific invitation was not found");
            }

            result = InvitationModule.RemoveInvitation(_context, invitation, true, user);
        }
        else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_ANONYMOUS_PROMOTION_TO_PARTICIPANT_IN_ACTIVITY)
        {
            FriendActivityToFriendUserInvitation invitation = _context.FriendActivityToFriendUserInvitations.Find(id);

            if (invitation == null)
            {
                return BadRequest("Specific invitation was not found");
            }

            result = InvitationModule.RemoveInvitation(_context, invitation, true, user);
        }
        else if (invitationBase.transferType == INVITATION_TRANSFER_TYPES.FRIEND_ACTIVITY_REQUESTS_PARTICIPANT_PROMOTION_TO_ADMIN_IN_ACTIVITY)
        {
            FriendActivityToFriendUserInvitation invitation = _context.FriendActivityToFriendUserInvitations.Find(id);

            if (invitation == null)
            {
                return BadRequest("Specific invitation was not found");
            }

            result = InvitationModule.RemoveInvitation(_context, invitation, true, user);
        }
        else
        {
            return BadRequest("Unknown transfer type");
        }*/

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// view invitation
    /// </summary>
    /// <param name="id">invitation id</param>
    /// <returns></returns>
    [HttpGet("Generic/ViewInvitation")]
    [Authorize]
    public async Task<IActionResult> GetInviationInformation(string id)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //get invitation
        InvitationBase invitation = _context.InvitationBases.Find(id);

        if (invitation == null)
        {
            return BadRequest("Invitation does not exist");
        }

        Pair<bool, Pair<string, JObject>> result = InvitationModule.GetViewInvitationInformation(user, invitation);

        if (!result.First)
        {
            return BadRequest(result.Second.First);
        }
        else
        {
            JObject invitationInformationContainer = result.Second.Second;

            JObject masterContainer = new JObject();

            masterContainer.Add(new JProperty("invitation_information", invitationInformationContainer));

            return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
    }

    //BLOCK ENDPOINTS

    /// <summary>
    /// friend user blocks current friend activity
    /// </summary>
    /// <param name="activity_id"></param>
    /// <returns></returns>
    [HttpGet("Friends/Block/UserActivity")]
    [Authorize]
    public async Task<IActionResult> FriendActivityFriendUserBlock(string activity_id)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        FriendActivity friendActivity = _context.FriendActivities.Find(activity_id);

        if (friendUser.createdActivities.Contains(friendActivity))
        {
            return BadRequest("Friend user is admin of activity");
        }

        Pair<bool, string> result = UserModule.FriendActivityFriendUserBlock(_context, friendActivity, friendUser);

        //remove parson from activity as participant if they are in it
        //continue removing if they were found
        if (friendUser.participatingActivities.Remove(friendActivity))
        {
            //remove friend from activities participants
            friendActivity.participants.Remove(friendUser);

            friendActivity.dynamicValues.numParticipants--;
            
            //check if it can become active if it is currently not active
            if (!friendActivity.isActive)
            {
                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
            }
        }

        IdentityModule.SafelySaveChanges(_context);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// friend activity blocks friend user
    /// </summary>
    /// <param name="activity_id"></param>
    /// <param name="user_id"></param>
    /// <returns></returns>
    [HttpGet("Friends/Block/ActivityUser")]
    [Authorize]
    public async Task<IActionResult> FriendUserFriendActivityBlock(string activity_id, string user_id)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser currentFriendUser = user.friendUser;

        if (currentFriendUser == null)
        {
            return BadRequest("Current friend user is null");
        }

        FriendActivity friendActivity = _context.FriendActivities.Find(activity_id);

        FriendUser friendUser = _context.FriendUsers.Find(user_id);

        if (friendUser.createdActivities.Contains(friendActivity))
        {
            return BadRequest("Friend user is admin of activity");
        }

        if (friendUser == null)
        {
            return BadRequest("Friend user not found");
        }

        if (!currentFriendUser.createdActivities.Contains(friendActivity))
        {
            return BadRequest("Current friend user is not admin in activity");
        }

        Pair<bool, string> result = UserModule.FriendActivityFriendUserBlock(_context, friendActivity, friendUser);

        //remove parson from activity as participant if they are in it
        //continue removing if they were found
        if (friendUser.participatingActivities.Remove(friendActivity))
        {
            //remove friend from activities participants
            friendActivity.participants.Remove(friendUser);

            friendActivity.dynamicValues.numParticipants--;

            //check if it can become active if it is currently not active
            if (!friendActivity.isActive)
            {
                //get dynamic values, check active conditions
                UserModule.checkIsActiveConditionsFriendActivity(friendActivity);
            }
        }

        IdentityModule.SafelySaveChanges(_context);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// block friend user from user
    /// </summary>
    /// <param name="user_id"></param>
    /// <returns></returns>
    [HttpGet("Friends/Block/User")]
    [Authorize]
    public async Task<IActionResult> FriendUserFriendUserBlock(string user_id)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser currentFriendUser = user.friendUser;

        if (currentFriendUser == null)
        {
            return BadRequest("Current friend user is null");
        }

        FriendUser friendUser = _context.FriendUsers.Find(user_id);

        if (friendUser == null)
        {
            return BadRequest("Friend user not found");
        }

        Pair<bool, string> result = UserModule.FriendUserFriendUserBlock(_context, currentFriendUser, friendUser);

        IdentityModule.SafelySaveChanges(_context);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    //REPORT ENDPOINTS

    /// <summary>
    /// current friend user reports other friend user
    /// </summary>
    /// <param name="user_id"></param>
    /// <returns></returns>
    [HttpGet("Friends/Report/User")]
    [Authorize]
    public async Task<IActionResult> FriendUserFriendUserReport(string user_id)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser currentFriendUser = user.friendUser;

        if (currentFriendUser == null)
        {
            return BadRequest("Current friend user is null");
        }

        FriendUser otherFriendUser = _context.FriendUsers.Find(user_id);

        if (otherFriendUser == null)
        {
            return BadRequest("Friend user to report was not found");
        }

        //report user and get result
        Pair<bool, string> result = UserModule.FriendUserFriendUserReport(_context, _userManager, _signInManager, currentFriendUser, otherFriendUser);
        
        IdentityModule.SafelySaveChanges(_context);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    //gets invitations of user
    //https://stackoverflow.com/questions/21487173/c-sharp-list-to-hold-multiple-object-types-inherited-from-the-same-base-class
    //this works as the list is just object references, so casting from and to doesn't affect the object itself

    //upload activity image (s)
    //called after AddFriendActivity
    //adds images links (our custom ones) to it.

    /*public async Task<IActionResult> UploadFriendActivityPhoto(IFormFile file) 
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
    }*/

    //MESSAGE ENDPOINTS

    /// <summary>
    /// send a direct message
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Friends/Messages/SendDirectMessage")]
    [Authorize]
    public async Task<IActionResult> SendDirectMessage(SendDirectMessageModel model)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        ApplicationUser otherUser = await _userManager.FindByIdAsync(model.other_id);

        if (otherUser == null)
        {
            return BadRequest("Other user does not exist");
        }

        //create direct message
        Pair<bool, string> result = MessageModule.createDirectMessage(_context, user, model.expo_token, otherUser, model.body);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            //return the timestamp
            JObject masterContainer = new JObject();

            masterContainer.Add(new JProperty("timestamp", DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));

            return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
    }

    //get pending messages
    [HttpPost("Friends/Messages/FriendActivityMakeAccouncement")]
    [Authorize]
    public async Task<IActionResult> FriendActivityMakeAnnouncement(MakeAnnouncementModel model)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest();
        }

        FriendActivity friendActivity = _context.FriendActivities.Find(model.activity_id);

        //if friend activity does not exist
        if (friendActivity == null)
        {
            return BadRequest("Friend activity does not exist");
        }

        //if friend user is not admin in activity
        if (!friendUser.createdActivities.Contains(friendActivity))
        {
            return BadRequest("Friend user is not admin in activity");
        }

        //create friend activity announcement
        Pair<bool, string> result = MessageModule.createFriendActivityAnnouncement(_context, user, model.expo_token, friendActivity, model.message);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// create conversation for activity
    /// </summary>
    /// <param name="activity_id"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    [HttpGet("Friends/Messages/FriendActivityCreateConversation")]
    [Authorize]
    public async Task<IActionResult> FriendActivityCreateConversation(string activity_id, string includes)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        FriendUser friendUser = user.friendUser;

        if (friendUser == null)
        {
            return BadRequest("Friend User is null");
        }

        //get friend activity
        FriendActivity friendActivity = _context.FriendActivities.Find(activity_id);

        if (friendActivity == null)
        {
            return BadRequest("Friend activity id is invalid or unknown");
        }

        //conversation type
        CONVERSATION_TYPES conversationType;

        if (includes == "all")
        {
            //if friend user is not admin
            if (!friendUser.createdActivities.Contains(friendActivity) && !friendUser.participatingActivities.Contains(friendActivity))
            {
                return BadRequest("Must be admin or participant of activity to create conversation");
            }

            conversationType = CONVERSATION_TYPES.FRIEND_ACTIVITY_ALL;
        }
        else if (includes == "admins")
        {
            //if friend user is not admin
            if (!friendUser.createdActivities.Contains(friendActivity))
            {
                return BadRequest("Must be admin of activity to create conversation");
            }

            conversationType = CONVERSATION_TYPES.FRIEND_ACTIVITY_ADMINS;
        }
        else
        {
            return BadRequest("Invalid includes type");
        }

        //create conversation
        Pair<bool, string> result = MessageModule.createFriendActivityConversation(_context, friendActivity, conversationType);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            //get conversation base
            ConversationBase conversationBase = _context.ConversationBases.Find(result.Second);

            //return converstaion name and id
            JObject masterContainer = new JObject();

            JObject conversationInformation = new JObject();

            //add values
            conversationInformation.Add(new JProperty("title", MessageModule.createConversationTitle(conversationBase)));
            conversationInformation.Add(new JProperty("id", conversationBase.id));

            masterContainer.Add(new JProperty("conversation_information", conversationInformation));

            return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
    }

    /// <summary>
    /// send conversation message
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Friends/Messages/SendConversationMessage")]
    [Authorize]
    public async Task<IActionResult> SendConversationMessage(SendConversationMessageModel model)
    {
        //attempt to get current user
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //create conversation
        Pair<bool, string> result = MessageModule.createConversationMessage(_context, user, model.expo_token, model.conversation_id, model.body);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            //return the timestamp
            JObject masterContainer = new JObject();

            masterContainer.Add(new JProperty("timestamp", DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));

            return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
        }
    }


    //get pending messages
    [HttpGet("Friends/Messages/GetPendingMessages")]
    [Authorize]
    public async Task<IActionResult> GetPendingMessages(string expo_token)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //if expo token is not in user's tokens
        if (user.expoTokens.FirstOrDefault(m => m.expoToken == expo_token) == null)
        {
            return BadRequest("Expo token is not valid for this user");
        }

        //create jobject
        JObject masterContainer = new JObject();

        JArray messageArrayContainer = MessageModule.getPendingFriendUserMessagesJson(_context, _userManager, user, expo_token);

        masterContainer.Add(new JProperty("messages", messageArrayContainer));

        return Content(masterContainer.ToString(Newtonsoft.Json.Formatting.None), "application/json");
    }

    [HttpGet("Generic/SendFeedback")]
    [Authorize]
    public async Task<IActionResult> SendFeedback(SendFeedbackModel model)
    {
        ApplicationUser user = await _userManager.GetUserAsync(this.User);

        //if user does not exists
        if (user == null)
        {
            return BadRequest();
        }

        //send email with message
        Pair<bool, string> result = UserModule.sendFeedbackEmail(user, model.feedback);

        if (!result.First)
        {
            return BadRequest(result.Second);
        }
        else
        {
            return Ok();
        }
    }

    public class GroupPoint
    {
        public GeoCoordinate coordinate;

        /// <summary>
        /// the number of points in this group
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        /// <param name=""></param>
        public GroupPoint(double lat, double lon)
        {
            this.coordinate = new GeoCoordinate(lat, lon);
            this.count = 1;
        }

        /// <summary>
        /// add point to group
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        /// <param name=""></param>
        public void addPoint(double lat, double lon)
        {
            this.coordinate.latitude = (this.coordinate.latitude * (double)(this.count) + lat) / ((double)(this.count) + 1.0);
            this.coordinate.longitude = (this.coordinate.longitude * (double)(this.count) + lon) / ((double)(this.count) + 1.0);
            this.count++;
        }

        /// <summary>
        /// merge groups together
        /// </summary>
        /// <param name="group"></param>
        public void mergeGroup(GroupPoint group)
        {
            this.coordinate.latitude = (this.coordinate.latitude * (double)(this.count) + group.coordinate.latitude * (double)(group.count)) / ((double)(this.count) + (double)(group.count));
            this.coordinate.longitude = (this.coordinate.longitude* (double)(this.count) + group.coordinate.longitude * (double)(group.count)) / ((double)(this.count) + (double)(group.count));
            this.count += group.count;
        }

        /// <summary>
        /// distance from other group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public double distance(GroupPoint group)
        {
            return this.coordinate.distance(group.coordinate);
        }

        /// <summary>
        /// distance from other geocoordinate
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double distance(GeoCoordinate point)
        {
            return this.coordinate.distance(point);
        }

        /// <summary>
        /// distance from other geocoordinate
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        /// <returns></returns>
        public double distance(double lat, double lon)
        {
            return this.coordinate.distance(lat, lon);
        }
    }

    
}

/*



//TODO endpoionts that enter more user information in

//doing this allows us to search from like attributes or custom attributes from the search bar, with the other filers (like age range)
private async Task<IActionResult> SearchFriendUsers(HashSet<FriendUserAttribute> attributes) //model
{
return Ok();
}

/// <summary>
/// get basic friend user information (information that can be made public)
/// </summary>
/// <param name="model"></param>
/// <returns>200 with json body if successful, not found if the user was not found</returns>
public async Task<IActionResult> GetBasicFriendUserInformation(GetUserInformationModel model)
{
//get the requested user
ApplicationUser user = await _userManager.FindByNameAsync(model.username);

//get the friend user
FriendUser friendUser = user.friendUser;

//if user was not found
if (user == null)
{
    //return not found request
    return NotFound();
}

//create the master json object
JObject masterContainer = new JObject();

//add username
masterContainer.Add(new JProperty("username", user.UserName));

//add description
masterContainer.Add(new JProperty("description", friendUser.description));

//add attributes

//create jarray for attributes
JArray attributes = new JArray();

//get the attributes for the user and add them to the jarray
foreach (FriendUserAttribute attribute in friendUser.attributes.ToList())
{
    attributes.Add(attribute.attribute);
}

//add the jarray to the master container
masterContainer.Add(new JProperty("attributes", attributes));

//... add more things

return Ok();
}

*/

/*

/// <summary>
/// get atrributes of friend user
/// </summary>
/// <param name="model"></param>
/// <returns>200 ok, returns json of attributes</returns>
public async Task<IActionResult> GetAttributes()
{
    //get the current user
    ApplicationUser user = await _userManager.GetUserAsync(this.User);

    return BadRequest("Not Implemented Yet!");
}

*/

/*

    public void Shuffle<T>(this IList<T> list)
    {
        Random random = new Random();
        int n = list.Count;

        for (int i = list.Count - 1; i > 1; i--)
        {
            int rnd = random.Next(i + 1);

            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }*/