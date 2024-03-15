public static class Constants
{
    public static class XUnit
    {
        public const string CoverletCollectorName = "XPlat Code Coverage";
        public const string FormatSetting = "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format";
        public const string IncludeSetting = "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Include";
        public const string OpenCoverFormat = "opencover";
        public const string OpenCoverFilename = "coverage.opencover.xml";
        public const string DefaultTestResultsDirectoryName = "TestResults";
    }
    public static class Nuget
    {
        public const string DefaultNugetSource = "https://api.nuget.org/v3/index.json";
        public const string PackageGlob = "*.nupkg";
    }
    public static class Sonarqube
    {
        public const string SonarCloudUrl = "https://sonarcloud.io";
        public const string HostParameter = "sonar.host.url";
        public const string ReportsParameter = "sonar.cs.opencover.reportsPaths";
    }
}
