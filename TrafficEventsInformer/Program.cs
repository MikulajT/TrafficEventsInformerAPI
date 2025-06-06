using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
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
            string env = builder.Environment.EnvironmentName;

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

            builder.Services.AddTransient<IGeoService, GeoService>();
            builder.Services.AddTransient<ITrafficRoutesRepository, TrafficRoutesRepository>();
            builder.Services.AddTransient<ITrafficRoutesService, TrafficRoutesService>();
            builder.Services.AddTransient<ITrafficEventsRepository, TrafficEventsRepository>();
            builder.Services.AddTransient<ITrafficEventsService, TrafficEventsService>();
            builder.Services.AddTransient<IUsersRepository, UsersRepository>();
            builder.Services.AddTransient<IUsersService, UsersService>();
            builder.Services.AddTransient<IPushNotificationService, PushNotificationService>();
            builder.Services.AddSingleton<IDopplerService, DopplerService>();

            builder.Services.AddHostedService<TrafficEventsSyncService>();

            // Add services to the container.
            // Add controllers and support for XML input/output
            builder.Services.AddControllers(options =>
            {
                options.InputFormatters.Add(new XmlSerializerInputFormatter(options)); // Enable XML deserialization
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter()); // Enable XML response support
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                var dopplerService = serviceProvider.GetRequiredService<IDopplerService>();

                options.UseNpgsql(dopplerService.GetDopplerSecretsAsync().Result.DbConnectionString);
                //options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
            });

            // Needed for Google Cloud Run
#if !DEBUG
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
#endif

            var app = builder.Build();

            // Needed for Postgre
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // Localization
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            // Always init database
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    dbContext.Database.EnsureDeleted();
            //    dbContext.Database.EnsureCreated();
            //}

            app.UseSwagger();
            app.UseSwaggerUI();
            //app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}