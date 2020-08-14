# Changelog

## 1.0.0
* Initial version

## 1.0.1
* Exception handling

## 2.0.0
* Lose the Serilog dependency in favor of LibLog - thanks [scardetto]

## 2.1.0
* Make it Azure Web Job-hostable

## 2.1.1
* Fix it so that it works as Azure Web Job

## 2.1.2
* Fix service name when calling Topshelf

## 2.1.3
* Don't spam the logs with shutdown notifications

## 2.1.4
* Make Azure Web Job shutdown logic log it if something goes wrong

## 3.0.0
* Target .NET Standard 2.0

## 3.1.0
* Add ability to parallelize initialization/disposal of services
* Service add function now exists in overload that passes a `CancellationToken` to the asynchronous initialization function
* Include XML docs

## 3.2.0
* Provide ability to customize Topshelf's `HostConfigurator`, making it possible e.g. to configure a service's description, crash recovery, etc. - thanks [igitur]

## 3.2.1
* Work around bug in Topshelf, which would not trigger Windows Service recovery if startup failed

[igitur]: https://github.com/igitur
[scardetto]: https://github.com/scardetto