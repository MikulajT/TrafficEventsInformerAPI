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

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
            builder.Services.AddTransient<IGeoService, GeoService>();
            builder.Services.AddTransient<ITrafficRoutesRepository, TrafficRoutesRepository>();
            builder.Services.AddTransient<ITrafficRoutesService, TrafficRoutesService>();
            builder.Services.AddTransient<ITrafficEventsRepository, TrafficEventsRepository>();
            builder.Services.AddTransient<ITrafficEventsService, TrafficEventsService>();

            var app = builder.Build();

            // Localization
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}