#tool "nuget:?package=ReportGenerator"

var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var version = Argument("build", "0.1.0");
var solution = "../universal-hive-bot.sln";
var cliProject = "../src/Functional.UniversalBot.CLI/Functional.UniversalBot.CLI.fsproj";
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
    var compressOutput = true;
    var msBuildSettings = new DotNetMSBuildSettings 
    {
        Version = version,
    };

    var projects = new [] {
        "../src/Functional.UniversalBot.Plugin.HiveEngine/Functional.UniversalBot.Plugin.HiveEngine.fsproj",
        "../src/Functional.UniversalBot.Hive/Functional.UniversalBot.Plugin.Hive.fsproj",
        "../src/Functional.UniversalBot.Plugin.Terracore/Functional.UniversalBot.Plugin.Terracore.fsproj",
    };

    foreach(var project in projects){
        DotNetPublish(project, new DotNetPublishSettings 
        {
            Configuration = configuration,
            OutputDirectory = $"{publishDir}/plugins",
            NoBuild = noBuild,
            MSBuildSettings = msBuildSettings,
        });
    }

    DotNetPublish(cliProject, new DotNetPublishSettings 
        {
            Configuration = configuration,
            OutputDirectory = $"{publishDir}/linux",
            PublishReadyToRun = compressOutput,
            PublishReadyToRunShowWarnings = compressOutput,
            PublishSingleFile = compressOutput,
            PublishTrimmed = false,
            SelfContained = compressOutput,
            Runtime = "linux-x64",
            NoBuild = noBuild,
            MSBuildSettings = msBuildSettings,
        });
    DotNetPublish(cliProject, new DotNetPublishSettings 
        {
            Configuration = configuration,
            OutputDirectory = $"{publishDir}/win",
            PublishReadyToRun = compressOutput,
            PublishReadyToRunShowWarnings = compressOutput,
            PublishSingleFile = compressOutput,
            PublishTrimmed = false,
            SelfContained = compressOutput,
            Runtime = "win-x64",
            NoBuild = noBuild,
            MSBuildSettings = msBuildSettings,
        });
    DotNetPublish(cliProject, new DotNetPublishSettings 
        {
            Configuration = configuration,
            OutputDirectory = $"{publishDir}/mac",
            PublishReadyToRun = compressOutput,
            PublishReadyToRunShowWarnings = compressOutput,
            PublishSingleFile = compressOutput,
            PublishTrimmed = false,
            SelfContained = compressOutput,
            Runtime = "osx-x64",
            NoBuild = noBuild,
            MSBuildSettings = msBuildSettings,
        });
});

Task("CopyFiles")
    .IsDependentOn("Publish")
    .Does(() =>
    {
        DeleteFiles($"{publishDir}*/*.pdb");
        DeleteFiles($"{publishDir}*/*.xml");

        CopyFiles($"{publishDir}plugins/*.Plugin.*.dll", $"{publishDir}/linux");
        CopyFiles($"{publishDir}plugins/*.Plugin.*.dll", $"{publishDir}/win");
        CopyFiles($"{publishDir}plugins/*.Plugin.*.dll", $"{publishDir}/mac");
    });

Task("Pack")
    .IsDependentOn("CopyFiles")
    .Does(() =>
{
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
