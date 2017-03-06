# csproj-referencing-xproj Test Plan

For many of the tests below, there are multiple tests in a compatibility matrix to use. For each, I've written up a set of "variables" that can be substituted into each set of instructions. 
The [.NET Platform Standard](https://github.com/dotnet/corefx/blob/master/Documentation/architecture/net-platform-standard.md) is highly recommended reading.

## UWP

1. Create a new xproj project that targets and exposes a simple class library, and add a class Class1 if it's not already there.
1. Update the frameworks section in the xproj class library to target just `netstandard1.3`.
1. Enable production of build outputs on the xproj project in both Debug and Release configurations.
1. Create a Universal C# Windows Application.
1. Switch your project to an x86 build platform.
1. Add a reference from the application to class library to the xproj project. *Expected:* the reference is correctly added.
1. Consume the class library inside the application by creating an instance of Class1 in the App.xaml.cs constructor.
1. At the use of the constructor, F12 on Class1. *Expected:* we correctly navigate to the source. We should **not** navigate to metadata.
1. Set a breakpoint in the constructor inside the class library.
1. F5 the application. *Expected:* the application runs, and the breakpoint is hit.
1. Package the application. *Expected:* a package can be built.
1. Switch to a Release configuration, and build. *Expected:* the build succeeds, and the package is built.
1. F5 the application in release. *Expected:* the application runs and the breakpoint is hit.

## Desktop Console Application

| `$CONSOLEAPPTARGET` | `$CONSOLEAPPPROJECTJSONMONIKER` | `XPROJTARGET` | `$EXPECTEDTARGET` |
| ------------------- | ------------------------------- | ------------- | ----------------- |
| .NET Framework 4.5.2 | `net452` | `netstandard1.2` and `net452` | `net452` |
| .NET Framework 4.6 | `net46` | `netstandard1.4` | `netstandard1.4` |

1. Create a new xproj project called ClassLibrary1 that targets and exposes a simple class, called Class1.
1. Update the frameworks section in the xproj class library to target just `$XPROJTARGET`.
1. Enable production of build outputs on the xproj project in both Debug and Release configurations.
1. Create a console application targeting `$CONSOLEAPPTARGET`.
1. Add a project.json containing 

    ```json
    {
        "frameworks": { "$CONSOLEAPPPROJECTJSONMONIKER" : { } },
        "runtimes": { "win" : { } }
    }
    ```
    with the appropriate replacements.
1. **Workaround:** Unload and reload the console application.
1. Add a reference from the application to class library. *Expected:* the reference is correctly added.
1. Go to the project reference in Solution Explorer, and click the project reference. Open the properties window, and verify the path mentions the artifact is coming from `$EXPECTEDTARGET`.
1. Consume the class library inside the application by creating an instance of Class1 in Program.Main.
1. At the use of the constructor, F12 on Class1. *Expected:* we correctly navigate to the source. We should **not** navigate to metadata.
1. Set a breakpoint in the constructor inside the class library.
1. F5 the application. *Expected:* the application runs, and the breakpoint is hit.

### Testing Project Reference Closure

1. Continuing with the desktop console app scenario, add another xproj project (ClassLibrary2) the same as the first one, and add Class2 to that project.
1. Enable production of build outputs on the xproj project in both Debug and Release configurations.
1. Add a reference from ClassLibrary1 to ClassLibrary2.
1. Consume ClassLibrary2 in ClassLibrary1's constructor.
1. Build and F5, and confirm that breakpoints can be hit in Class2. *Expected:* everything works.

## Portable Projects targeting .NET Standard

| `$XPROJTARGET` | `$PORTABLETARGET` |
| -------------- | ----------------- |
| `netstandard1.4` | `netstandard1.4` |
| `netstandard1.3` | `netstandard1.4` |

1. Create a new xproj class library.
1. Update the frameworks section in the xproj class library to target just `$XPROJTARGET`.
1. Enable production of build outputs on the xproj project in both Debug and Release configurations.
1. Create a new project.json-targeting portable class library by targeting ASP.NET 5.0 and .NET 4.6.
1. Change the contents of project.json to:

    ```json
    {
      "dependencies": {
        "NETStandard.Library": "1.0.0-rc2-23811"
      },
      "frameworks": {
        "$PORTABLETARGET": { }
      }
    }
    ```
1. **Workaround:** If `$XPROJTARGET` starts with `netstandard`, insert `"imports": [ "portable-net45+win8", "dotnet5.6", "dnxcore50" ]`, where the number after dotnet should be the number you inserted for netstandard in the previous step, plus 4.1.
1. Rebuild. *Expected:* the portable library builds.
1. Add a reference from the portable class library to the xproj project. *Expected:* the reference is correctly added.
1. Consume the xproj project in the portable class library and F12 on a usage of a type from the xproj project. *Expected:* we correctly navigate to the source.
1. Build the solution. *Expected:* everything builds.

## Portable Projects targeting Profiles

| `$XPROJTARGET` | `$PORTABLEPROJECTTARGETS` | `$PORTABLEPROFILENUMBER` |
| -------------- | ------------------------- | ------------------------ |
| `netstandard1.1` | .NET Framework 4.5 and Windows 8 | 7 |
| `netstandard1.1` | .NET Framework 4.5, Windows 8, and Windows Phone 8.1 | 111 |

1. Create a new xproj class library.
1. Update the frameworks section in the xproj class library to target just `$XPROJTARGET`.
1. Enable production of build outputs on the xproj project in both Debug and Release configurations.
1. Create portable class library targeting `$PORTABLEPROJECTTARGETS`.
1. Add a project.json with the contents of:

    ```json
    {
      "dependencies": {
      },
      "frameworks": {
        ".NETPortable,Version=v4.5,Profile=Profile$PORTABLEPROFILENUMBER": { }
      }
    }
    ```
    making substitutions as necessary.
1. Rebuild. *Expected:* the portable library builds.
1. Add a reference from the portable class library to the xproj project. *Expected:* the reference is correctly added.
1. Consume the xproj project in the portable class library and F12 on a usage of a type from the xproj project. *Expected:* we correctly navigate to the source.
1. Build the solution. *Expected:* everything builds.
