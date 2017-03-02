# Topper

[![install from nuget](https://img.shields.io/nuget/v/Topper.svg?style=flat-square)](https://www.nuget.org/packages/Topper)

Generic Windows service host.

Based on Topshelf. Exposes a drastically simplified API based on `IDisposable`.

Also, it is assumed that you want to use Serilog for logging.

## Getting started

Create `YourNewAwesomeWindowsService` as a Console Application project targeting at least .NET 4.5.2.

Include the NuGet package :package: 

    Install-Package Topper -ProjectName YourNewAwesomeWindowsService

and clean up your `Program.cs` so it becomes nice like this: :sunflower: 

    namespace YourNewAwesomeWindowsService
    {
        class Program
        {
            static void Main()
            {
                
            }
        }
    }

and then you configure Topper by going

	var configuration = new ServiceConfiguration()
		.Add(.. function that returns an IDisposable ..)
		.Add(.. another function that returns an IDisposable ..);

	ServiceHost.Run(configuration);

in `Main`, which could look like this:

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

:monkey_face: Easy!

Topper uses Serilog :zap: to log things, so you probably want to

    Install-Package Serilog.Sinks.ColoredConsole -ProjectName YourNewAwesomeWindowsService

and configure the global :earth_africa: logger before starting your service:

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


And that is how you use Topper.


---


