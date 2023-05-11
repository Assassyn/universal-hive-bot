var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var version = Argument("build", "0.1.0");
var solution = "../universal-hive-bot.sln";
var cliProject = "../src/Functional.UltimateHiveBot.CLI/Functional.UltimateHiveBot.CLI.fsproj";
var publishDir = "./publish/";
var artifacts = "./artifacts/";
var tests = "./tests/";

Task("Clean")
    .Does(() =>
{
    CleanDirectory(publishDir);
    CleanDirectory(artifacts);
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild(solution, new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
{
    var noBuild = false;
    var msBuildSettings = new DotNetMSBuildSettings 
    {
        Version = version,
    };

    DotNetPublish(cliProject, new DotNetPublishSettings 
        {
            Configuration = configuration,
            OutputDirectory = $"{publishDir}/linux",
            PublishReadyToRun = true,
            PublishReadyToRunShowWarnings = true,
            PublishSingleFile = true,
            PublishTrimmed = true,
            SelfContained = true,
            Runtime = "linux-x64",
            NoBuild = noBuild,
            MSBuildSettings = msBuildSettings,
        });
    DotNetPublish(cliProject, new DotNetPublishSettings 
        {
            Configuration = configuration,
            OutputDirectory = $"{publishDir}/win",
            PublishReadyToRun = true,
            PublishReadyToRunShowWarnings = true,
            PublishSingleFile = true,
            PublishTrimmed = true,
            SelfContained = true,
            Runtime = "win-x64",
            NoBuild = noBuild,
            MSBuildSettings = msBuildSettings,
        });
    DotNetPublish(cliProject, new DotNetPublishSettings 
        {
            Configuration = configuration,
            OutputDirectory = $"{publishDir}/mac",
            PublishReadyToRun = true,
            PublishReadyToRunShowWarnings = true,
            PublishSingleFile = true,
            PublishTrimmed = true,
            SelfContained = true,
            Runtime = "osx-x64",
            NoBuild = noBuild,
            MSBuildSettings = msBuildSettings,
        });
});

Task("Pack")
    .IsDependentOn("Publish")
    .Does(() =>
{

    DeleteFiles($"{publishDir}linux/*.pdb");
    DeleteFiles($"{publishDir}win/*.pdb");
    DeleteFiles($"{publishDir}mac/*.pdb");

    Zip($"{publishDir}linux", $"{artifacts}linux-{version}.zip");
    Zip($"{publishDir}win", $"{artifacts}win-{version}.zip");
    Zip($"{publishDir}mac", $"{artifacts}mac-{version}.zip");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest(solution, new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        ArgumentCustomization = args=>args.Append($"--logger trx;LogFileName=\"TestResults.xml\" --collect:\"XPlat Code Coverage\"")
    });
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
