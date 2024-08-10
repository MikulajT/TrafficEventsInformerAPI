namespace TrafficEventsInformer.Models
{
    public enum EventType
    {
        AbnormalTraffic = 1,
        Accident,
        Activity,
        AnimalPresenceObstruction,
        AuthorityOperation,
        CarParks,
        Conditions,
        ConstructionWorks,
        DisturbanceActivity,
        EnvironmentalObstruction,
        EquipmentOrSystemFault,
        GeneralInstructionOrMessageToRoadUsers,
        GeneralNetworkManagement,
        GeneralObstruction,
        InfrastructureDamageObstruction,
        MaintenanceWorks,
        NetworkManagement,
        NonRoadEventInformation,
        NonWeatherRelatedRoadConditions,
        Obstruction,
        OperatorAction,
        PoorEnvironmentConditions,
        PublicEvent,
        ReroutingManagement,
        RoadConditions,
        RoadOperatorServiceDisruption,
        RoadOrCarriagewayOrLaneManagement,
        RoadsideAssistance,
        RoadsideServiceDisruption,
        Roadworks,
        SpeedManagement,
        TrafficElement,
        TransitInformation,
        VehicleObstruction,
        WeatherRelatedRoadConditions,
        WinterDrivingManagement
    }

    public enum ServiceResult
    {
        Success,
        ResourceExists
        // Error result is not needed since it is handled by global exception error handler
    }
}
