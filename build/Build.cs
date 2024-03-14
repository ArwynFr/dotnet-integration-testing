using System.Linq;

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.OctoVersion;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities.Collections;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(_ => _.Verify);

    [Secret, Parameter] readonly string NugetApikey;
    [Secret, Parameter] readonly string SonarToken;
    [Secret, Parameter] readonly string GhToken;

    [Required, Solution(GenerateProjects = true)] readonly Solution Solution;
    [Required, OctoVersion(AutoDetectBranch = true)] readonly OctoVersionInfo OctoVersionInfo;
    [PathVariable] readonly Tool Gh;
    [PathVariable] readonly Tool Git;

    string SonarOrganization => "arwynfr";
    string SonarKey => "ArwynFr_dotnet-integration-testing";
    AbsolutePath DotSonar => RootDirectory / ".sonarqube";
    string TestIncludes => $"[{Solution.ArwynFr_IntegrationTesting.Name}]*";
    AbsolutePath TestResultsDirectory => RootDirectory / Constants.XUnit.DefaultTestResultsDirectoryName;
    AbsolutePath TestResultsGlob => TestResultsDirectory / "*" / Constants.XUnit.OpenCoverFilename;
    Project TestProject => Solution.ArwynFr_IntegrationTesting_Tests;
    AbsolutePath NugetGlob => RootDirectory / Constants.Nuget.PackageGlob;
    bool IsPreRelease => !string.IsNullOrEmpty(OctoVersionInfo.PreReleaseTag);

    Target RestoreTools => _ => _
        .Unlisted()
        .DependentFor(VerifyOutdated, VerifyRoslyn)
        .Executes(() => DotNetTasks.DotNetToolRestore());

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

    Target Clean => _ => _
        .Before(RestoreTools)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean();
            TestResultsDirectory.DeleteDirectory();
            NugetGlob.GlobFiles().DeleteFiles();
            DotSonar.DeleteDirectory();
        });

    Target Fix => _ => _
        .Before(Verifying)
        .DependsOn(RestoreTools)
        .Executes(() =>
        {
            DotNetTasks.DotNet("tool run dotnet-outdated -- --upgrade");
            DotNetTasks.DotNetFormat();
        });

    Target Verifying => _ => _
        .DependentFor(VerifyFormat, VerifyOutdated, VerifyRoslyn, VerifySonarqube)
        .Unlisted();

    Target Verify => _ => _
        .DependsOn(VerifyFormat, VerifyOutdated, VerifyRoslyn, VerifySonarqube);

    Target Pack => _ => _
        .DependsOn(Verify)
        .Executes(() => DotNetTasks.DotNetPack(_ => _
            .SetProject(Solution.ArwynFr_IntegrationTesting)
            .SetProperty("Version", OctoVersionInfo.FullSemVer)
            .SetOutputDirectory(RootDirectory)));

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => NugetApikey)
        .Executes(() => DotNetTasks.DotNetNuGetPush(_ => _
            .SetSource(Constants.Nuget.DefaultNugetSource)
            .SetApiKey(NugetApikey)
            .SetTargetPath(NugetGlob)));

    Target Release => _ => _
        .Unlisted()
        .TriggeredBy(Publish)
        .Requires(() => GhToken)
        .Executes(() => Gh.Invoke(
            arguments: $"release create '{OctoVersionInfo.FullSemVer}' --generate-notes",
            environmentVariables: EnvironmentInfo.Variables
                .ToDictionary(x => x.Key, x => x.Value)
                .SetKeyValue("GH_TOKEN", GhToken).AsReadOnly()));

    Target Tags => _ => _
        .Unlisted()
        .TriggeredBy(Publish)
        .OnlyWhenStatic(() => !IsPreRelease)
        .Executes(() =>
        {
            Git.Invoke($"tag --force 'v{OctoVersionInfo.Major}.{OctoVersionInfo.Minor}'");
            Git.Invoke($"tag --force 'v{OctoVersionInfo.Major}'");
            Git.Invoke("push origin --tags --force");
        });
}