using POPNetwork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.AspNetCore;
using System.Net;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args) {

        /*POPNetwork.Startup.externalConfiguration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("runtimevariables.json")
            .Build();

        IWebHostBuilder hostBuilder = WebHost.CreateDefaultBuilder(args)
        .UseKestrel(options =>
        {
            
        })
        //.UseIISIntegration()
        .UseStartup<Startup>();*/

        POPNetwork.Startup.externalConfiguration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("runtimevariables.json")
            .Build();

        IWebHostBuilder hostBuilder = WebHost.CreateDefaultBuilder(args)
        .UseKestrel(options =>
        {
            options.ListenAnyIP(80, listenOptions =>
            {
                listenOptions.UseHttps("purpleorangepinkcert.pfx", "hallTo4$");
            });
            options.ListenAnyIP(443, listenOptions =>
            {
                listenOptions.UseHttps("purpleorangepinkcert.pfx", "hallTo4$");
            });
        })
        //.UseIISIntegration()
        .UseUrls("https://localhost:5001", "https://locahost:80", "https://localhost:5000", "https://localhost:443")
        .UseStartup<Startup>();
         

        //192.168.1.79

        return hostBuilder;
    }
}

//.UseUrls("https://localhost:5001", "https://locahost:80", "https://localhost:5000", "https://localhost:443")

/*.UseKestrel(options =>
                {
                    options.ListenAnyIP(80, listenOptions =>
                    {
                        listenOptions.UseHttps("purpleorangepinkcert.pfx", "hallTo4$");
                    });
                })
            .UseIISIntegration()

 
 options.Listen(IPAddress.Loopback, 80, listenOptions =>
                {
                    listenOptions.UseHttps("purpleorangepinkcert.pfx", "hallTo4$");
                });
                options.Listen(IPAddress.Loopback, 443, listenOptions =>
                {
                    listenOptions.UseHttps("purpleorangepinkcert.pfx", "hallTo4$");
                });*/

//https://api.purpleorangepink.com/api/User/GetMe
//.UseUrls("https://localhost:5001", "https://192.168.1.79:5000", "https://2600:1700:5f20:8250::12:5003") //, "https://45.30.91.176:80", "https://45.30.91.176:443")

//default port is 80

//2600:1700:5f20:8250::12
//https://192.168.1.79:5000

//192.168.1.79

//var connectionString = builder.Configuration.GetConnectionString("Server=DESKTOP-LB7T7SQ\\SQLDEVELOPER;Database=POP;User Id=god;Password=god;Trusted_Connection=true;MultipleActiveResultSets=true;");


/*
var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder();
//var connectionString = builder.Configuration.GetConnectionString("Server=DESKTOP-LB7T7SQ\\SQLDEVELOPER;Database=POP;User Id=god;Password=god;Trusted_Connection=true;MultipleActiveResultSets=true;");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=DESKTOP-LB7T7SQ\\SQLDEVELOPER;Database=POP;User Id=god;Password=god;Trusted_Connection=true;MultipleActiveResultSets=true;"));

//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { }) //options.SignIn.RequireConfirmedAccount = true
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opts => {
    opts.Password.RequiredLength = 5;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});


builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "POPNetwork", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "POPNetwork v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
*/








/*
Filestorage: Azure Blob
Rest Api: https://docs.microsoft.com/en-us/rest/api/storageservices/blob-service-rest-api
Azure Storage (Parent of Azure Blob) Rest Api: https://docs.microsoft.com/en-us/rest/api/storagerp/
Create User: https://docs.microsoft.com/en-us/rest/api/storagerp/storage-sample-create-account




 Messaging: Signalr
server side: https://mindofai.github.io/Building-a-Real-Time-Chat-App-With-SignalR-and-Xamarin/
with react native using npm package: https://www.npmjs.com/package/@microsoft/signalr
client user offline: https://stackoverflow.com/questions/21836317/using-signalr-with-one-to-one-including-offline-modus
and https://stackoverflow.com/questions/62526391/can-signalr-queue-up-messages-when-client-is-disconnected-and-resend-them-to-cli
connecting .net to hub: https://stackoverflow.com/questions/59743613/connect-react-native-app-to-net-core-signalr-hub
When app is in background: https://rnfirebase.io/messaging/notifications
 */