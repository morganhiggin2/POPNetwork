
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POPNetwork.Controllers;
using POPNetwork.Models;
using POPNetwork.Modules;
using POPNetwork.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;

namespace POPNetwork;
public class Startup : StartupBase
{
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;

    //future: read from json config file
    //Note: this is a NO-REPLY email, consider no-reply@...
    public static IConfiguration externalConfiguration;

    public Startup(IWebHostEnvironment env, IConfiguration config)
    {
        this.configuration = config;
        this.environment = env;
    }

    public override void Configure(IApplicationBuilder app)
    {
        //when using signalr locally, disable this
        app.UseHttpsRedirection();

        app.UseHttpLogging();

        app.UseRouting();

        app.UseCors(builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });

        //start side tasks
        Task.Run(() => SideTasks.taskLoop(app.ApplicationServices.GetRequiredService<ApplicationDbContext>(), app.ApplicationServices.GetRequiredService<UserManager<ApplicationUser>>()));

        //start azure blob connection
        AzureBlobModule.init();

        /*

        app.UseRouting(routes =>
        {
            routes.MapControllers();
        });*/
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        //services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

        //load runtime variables for connection string
        string server = Startup.externalConfiguration.GetSection("SQLStrings").GetValue<string>("server");
        string database = Startup.externalConfiguration.GetSection("SQLStrings").GetValue<string>("database");
        string adminLogin = Startup.externalConfiguration.GetSection("SQLStrings").GetValue<string>("admin_login");
        string adminPassword = Startup.externalConfiguration.GetSection("SQLStrings").GetValue<string>("admin_password");

        //"Server=DESKTOP-LB7T7SQ\\SQLDEVELOPER;Database=POP;User Id=god;Password=god;Trusted_Connection=true;MultipleActiveResultSets=true;"
        services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies()
                //When deploying on azure, replace with default string. 
                .UseSqlServer("Server=" + server + ";Initial Catalog=" + database + ";User Id=" + adminLogin + ";Password=" + adminPassword + ";MultipleActiveResultSets=true;Trusted_Connection=False;Encrypt=True;",
                    o => {
                        o.UseNetTopologySuite();
                    });
            });

        services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(options =>
            {
                options.AllowedCertificateTypes = CertificateTypes.All;
            }
        );

        //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddIdentity<ApplicationUser, IdentityRole>(options => { }) //options.SignIn.RequireConfirmedAccount = true
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = System.TimeSpan.FromDays(30);
        });

        //for email service
        services.AddTransient<IEmailSender, EmailSender>();
        services.Configure<AuthMessageSenderOptions>(options => 
        {
            
        });

        //set password reset token expiration to five minutes
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = System.TimeSpan.FromMinutes(5);
        });

        services.AddControllers();
    }
}

//for windows: admin terminal -> "net stop http"

//https://docs.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad