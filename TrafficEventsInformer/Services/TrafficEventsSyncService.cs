using Serilog;

namespace TrafficEventsInformer.Services
{
    public class TrafficEventsSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TrafficEventsSyncService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Logger.Information("Automatic traffic events sync");

                using (var scope = _scopeFactory.CreateScope())
                {
                    ITrafficEventsService _trafficEventsService = scope.ServiceProvider.GetRequiredService<ITrafficEventsService>();
                    _trafficEventsService.SyncRouteEventsAsync();
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}