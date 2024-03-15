using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.OctoVersion;
using Nuke.Common.Tools.SonarScanner;
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(_ => _.Verify);

    [Secret, Parameter] readonly string SonarToken;
    [Required, Solution(GenerateProjects = true)] readonly Solution Solution;
    [Required, OctoVersion(AutoDetectBranch = true)] readonly OctoVersionInfo OctoVersionInfo;

    string SonarOrganization => "arwynfr";
    string SonarKey => "ArwynFr_dotnet-integration-testing";

    string TestIncludes => $"[{Solution.ArwynFr_IntegrationTesting.Name}]*";
    AbsolutePath TestResultsDirectory => RootDirectory / Constants.XUnit.DefaultTestResultsDirectoryName;
    AbsolutePath TestResultsGlob => TestResultsDirectory / "*" / Constants.XUnit.OpenCoverFilename;
    Project TestProject => Solution.ArwynFr_IntegrationTesting_Tests;

    Target VerifyFormat => _ => _
        .Unlisted()
        .Executes(() => DotNetTasks.DotNet("format --verify-no-changes"));

    Target VerifyRoslyn => _ => _
        .Unlisted()
        .Executes(() => DotNetTasks.DotNet("tool run roslynator analyze"));

    Target VerifyOutdated => _ => _
        .Unlisted()
        .Executes(() => DotNetTasks.DotNet("tool run dotnet-outdated --fail-on-updates"));

    Target VerifySonarqube => _ => _
        .Unlisted()
        .Requires(() => SonarToken)
        .Executes(() =>
        {
            SonarScannerTasks.SonarScannerBegin(_ => _
                .SetOrganization(SonarOrganization)
                .SetProjectKey(SonarKey)
                .SetOpenCoverPaths(TestResultsGlob)
                .SetServer(Constants.Sonarqube.SonarCloudUrl)
                .SetToken(SonarToken)
                .SetQualityGateWait(true));

            DotNetTasks.DotNetTest(_ => _
                .SetProjectFile(TestProject.Path)
                .EnableCollectCoverage()
                .SetDataCollector(Constants.XUnit.CoverletCollectorName)
                .SetResultsDirectory(TestResultsDirectory)
                .AddRunSetting(Constants.XUnit.FormatSetting, Constants.XUnit.OpenCoverFormat)
                .AddRunSetting(Constants.XUnit.IncludeSetting, TestIncludes));

            SonarScannerTasks.SonarScannerEnd(_ => _
                .SetToken(SonarToken));
        });

    Target Verify => _ => _
        .DependsOn(VerifyFormat, VerifyOutdated, VerifyRoslyn, VerifySonarqube);
}
