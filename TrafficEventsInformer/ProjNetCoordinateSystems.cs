namespace TrafficEventsInformer
{
    public static class ProjNetCoordinateSystems
    {
        // S-JTSK (EPSG:5514) coordinate system in WKT format
        public const string SJTSK = @"PROJCS[""S-JTSK / Krovak East North"",
                                  GEOGCS[""S-JTSK"",
                                    DATUM[""System_Jednotne_Trigonometricke_Site_Katastralni"",
                                      SPHEROID[""Bessel 1841"", 6377397.155, 299.1528128,
                                        AUTHORITY[""EPSG"", ""7004""]],
                                      TOWGS84[589,76,480,0,0,0,0],
                                      AUTHORITY[""EPSG"", ""6152""]],
                                    PRIMEM[""Greenwich"", 0,
                                      AUTHORITY[""EPSG"", ""8901""]],
                                    UNIT[""degree"", 0.0174532925199433,
                                      AUTHORITY[""EPSG"", ""9122""]],
                                    AUTHORITY[""EPSG"", ""4156""]],
                                  PROJECTION[""Krovak""],
                                  PARAMETER[""latitude_of_center"", 49.5],
                                  PARAMETER[""longitude_of_center"", 24.83333333333333],
                                  PARAMETER[""azimuth"", 30.28813975277778],
                                  PARAMETER[""pseudo_standard_parallel_1"", 78.5],
                                  PARAMETER[""scale_factor"", 0.9999],
                                  PARAMETER[""false_easting"", 0],
                                  PARAMETER[""false_northing"", 0],
                                  UNIT[""metre"", 1,
                                    AUTHORITY[""EPSG"", ""9001""]],
                                  AXIS[""X"", EAST],
                                  AXIS[""Y"", NORTH],
                                  AUTHORITY[""EPSG"", ""5514""]]";

        // WGS84 (EPSG:4326) coordinate system in WKT format
        public const string WGS84 = @"GEOGCS[""WGS 84"",
                                  DATUM[""WGS_1984"",
                                    SPHEROID[""WGS 84"", 6378137, 298.257223563,
                                      AUTHORITY[""EPSG"", ""7030""]],
                                    AUTHORITY[""EPSG"", ""6326""]],
                                  PRIMEM[""Greenwich"", 0,
                                    AUTHORITY[""EPSG"", ""8901""]],
                                  UNIT[""degree"", 0.0174532925199433,
                                    AUTHORITY[""EPSG"", ""9122""]],
                                  AUTHORITY[""EPSG"", ""4326""]]";
    }
}
