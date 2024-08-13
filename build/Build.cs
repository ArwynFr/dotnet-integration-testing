using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.OctoVersion;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities.Collections;
using System.Linq;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(_ => _.Test);

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

    Target Clean => _ => _
        .Before(Restore, Test)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean();
            TestResultsDirectory.DeleteDirectory();
            NugetGlob.GlobFiles().DeleteFiles();
            DotSonar.DeleteDirectory();
        });

    Target Restore => _ => _
        .Unlisted()
        .Executes(() => DotNetTasks.DotNetToolRestore());

    Target Lint => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNet("format --verify-no-changes");
            DotNetTasks.DotNet("tool run roslynator analyze");
            DotNetTasks.DotNet("tool run dotnet-outdated --fail-on-updates");
        });
        
    Target Test => _ => _
        .DependsOn(Lint)
        .Executes(() => DotNetTasks.DotNetTest(_ => _
            .SetProjectFile(TestProject.Path)
            .EnableCollectCoverage()
            .SetDataCollector(Constants.XUnit.CoverletCollectorName)
            .SetResultsDirectory(TestResultsDirectory)
            .AddRunSetting(Constants.XUnit.FormatSetting, Constants.XUnit.OpenCoverFormat)
            .AddRunSetting(Constants.XUnit.IncludeSetting, TestIncludes)));

    Target Sonarqube => _ => _
        .Requires(() => SonarToken)
        .Executes(() => SonarScannerTasks.SonarScannerBegin(_ => _
            .SetOrganization(SonarOrganization)
            .SetProjectKey(SonarKey)
            .SetOpenCoverPaths(TestResultsGlob)
            .SetToken(SonarToken)
            .SetQualityGateWait(true)))
        .Inherit(Test)
        .Executes(() => SonarScannerTasks.SonarScannerEnd(_ => _.SetToken(SonarToken)));

    Target Pack => _ => _
        .Executes(() => DotNetTasks.DotNetPack(_ => _
            .SetProject(Solution.ArwynFr_IntegrationTesting)
            .SetProperty("Version", OctoVersionInfo.FullSemVer)
            .SetProperty("IncludeSymbols", true)
            .SetProperty("SymbolPackageFormat", "snupkg")
            .SetOutputDirectory(RootDirectory)));

    Target Publish => _ => _
        .DependsOn(Pack, Sonarqube)
        .Requires(() => NugetApikey, () => GhToken)
        .Executes(() =>
        {
            DotNetTasks.DotNetNuGetPush(_ => _
                .SetSource(Constants.Nuget.DefaultNugetSource)
                .SetApiKey(NugetApikey)
                .SetTargetPath(NugetGlob));

            Gh.Invoke(
                arguments: $"release create {OctoVersionInfo.FullSemVer} --generate-notes",
                environmentVariables: EnvironmentInfo.Variables
                    .ToDictionary(x => x.Key, x => x.Value)
                    .SetKeyValue("GH_TOKEN", GhToken).AsReadOnly());
        });

    Target Tags => _ => _
        .Unlisted()
        .TriggeredBy(Publish)
        .OnlyWhenStatic(() => !IsPreRelease)
        .Executes(() =>
        {
            Git.Invoke($"tag --force v{OctoVersionInfo.Major}.{OctoVersionInfo.Minor}");
            Git.Invoke($"tag --force v{OctoVersionInfo.Major}");
            Git.Invoke("push origin --tags --force");
        });
}
