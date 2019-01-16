# Topper

[![install from nuget](https://img.shields.io/nuget/v/Topper.svg?style=flat-square)](https://www.nuget.org/packages/Topper)

Generic Windows service host - makes an ordinary Console Application hostable in the following scenarios:

* To be F5-debugged locally - on your developer machine
* To be installed as a Windows Service - on the servers in your basement
* To be executed as an Azure Web Job - in the cloud!!

Based on Topshelf. Exposes a drastically simplified API, where "services" are simply factories that return something `IDisposable`.

Targets .NET Standard 2.0, so you must target either netcoreapp2.0 (or later), or net462 (or later) in your Console Application.

## Getting started

Create `YourNewAwesomeWindowsService` as a Console Application project targeting AT LEAST .NET 4.6.2 or .NET Core App 2.0.

Include the NuGet package :package: 

    Install-Package Topper -ProjectName YourNewAwesomeWindowsService

and clean up your `Program.cs` so it becomes nice like this: :sunflower: 

```csharp
namespace YourNewAwesomeWindowsService
{
    class Program
    {
        static void Main()
        {
                
        }
    }
}
```
and then you configure Topper by going

```csharp
var configuration = new ServiceConfiguration()
	.Add(.. function that returns an IDisposable ..)
	.Add(.. another function that returns an IDisposable ..);

ServiceHost.Run(configuration);
```

in `Main`, which could look like this:

```csharp
namespace YourNewAwesomeWindowsService
{
    class Program
    {
        static void Main()
        {
            var configuration = new ServiceConfiguration()
                .Add(() => new MyNewAwesomeService());

            ServiceHost.Run(configuration);                
        }
    }
}
```

:monkey_face: Easy!

Topper uses LibLog :zap: to log things.  If you want to use Serilog, you probably want to

```psh
Install-Package Serilog.Sinks.ColoredConsole -ProjectName YourNewAwesomeWindowsService
```

and configure the global :earth_africa: logger before starting your service:

```csharp
namespace YourNewAwesomeWindowsService
{
    class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            var configuration = new ServiceConfiguration()
                .Add(() => new MyNewAwesomeService());

            ServiceHost.Run(configuration);                
        }
    }
}
```


And that is how you use Topper.

## How to run locally?

Press F5 or CTRL+F5 in Visual Studio.

Run the .exe

## How to run as Windows Service?

Open an elevated command prompt, and run the .exe with the `install` argument, like so:

```dos
C:\apps\YourApp> YourApp.exe install
```

and then some Windows Service Control :traffic_light: stuff will appear and tell you some details on how it was installed.

You can remove it again like this:

```dos
C:\apps\YourApp> YourApp.exe uninstall
```

Not exactly surprising. :clap:

## How to run as Azure Web Job?

Just run it as you would any other Console Application as a Continuous Web Job.

Topper automatically monitors for the presence of the `WEBJOBS_SHUTDOWN_FILE`, to be able to shut down gracefully and dispose your `IDisposable`s. :recycle:

---


