using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Globalization;
using System.Reflection;
using TrafficEventsInformer.Ef;
using TrafficEventsInformer.Services;

namespace TrafficEventsInformer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            // Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Services");
            var supportedCultures = new[]
            {
                //new CultureInfo("en-US"),
                new CultureInfo("cs-CZ")
            };
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("cs-CZ");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.File($"Logs/{Assembly.GetExecutingAssembly().GetName().Name}.log")
                .WriteTo.Console()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trafficeventsinformer-firebase-adminsdk-610ik-10035f39e0.json")),
            });

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

            var tst = builder.Configuration.GetConnectionString("ConnectionString");

            builder.Services.AddTransient<IGeoService, GeoService>();
            builder.Services.AddTransient<ITrafficRoutesRepository, TrafficRoutesRepository>();
            builder.Services.AddTransient<ITrafficRoutesService, TrafficRoutesService>();
            builder.Services.AddTransient<ITrafficEventsRepository, TrafficEventsRepository>();
            builder.Services.AddTransient<ITrafficEventsService, TrafficEventsService>();
            builder.Services.AddTransient<IPushNotificationService, PushNotificationService>();

            var app = builder.Build();

            // Localization
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            // Create database in docker container
            if (app.Environment.EnvironmentName == "Docker")
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            //app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}