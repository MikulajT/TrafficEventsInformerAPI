﻿using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRoutesRepository
    {
        IEnumerable<TrafficRoute> GetTrafficRouteNames();
        void AddRoute(string routeName, string routeCoordinates);
        IEnumerable<TrafficRoute> GetUsersRoutes();
        TrafficRoute GetUsersRoute(int routeId);
        void DeleteRoute(int routeId);
        void UpdateRoute(UpdateRouteRequest requestData);
    }
}
