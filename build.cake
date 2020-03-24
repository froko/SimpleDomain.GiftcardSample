#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");

Task("Reset")
    .Does(() => {
        CleanDirectories("./src/**/obj");
        CleanDirectories("./src/**/bin");
    });

GitVersion versionInfo = null;
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    });

Task("Build")
    .IsDependentOn("Version")
    .Does(() => {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = "Release"
        };

        DotNetCoreBuild("./src/GiftcardSample.sln", settings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        var settings = new DotNetCoreTestSettings
        {
            Configuration = "Release"
        };


        var testProjectFiles = GetFiles("./src/*.Facts/*.csproj");
        foreach(var file in testProjectFiles)
        {
            DotNetCoreTest(file.FullPath, settings);
        }

        var scenarioProjectFiles = GetFiles("./src/*.Scenarios/*.csproj");
        foreach(var file in scenarioProjectFiles)
        {
            DotNetCoreTest(file.FullPath, settings);
        }
    });

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);