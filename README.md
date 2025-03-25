# PoC Config Reload Branch
This branch makes use of [Newtonsoft.Json](https://www.nuget.org/packages/newtonsoft.json/).
The configuration JSON loads at start and reloads after a button click. The AGVConfigManager makes use of the generic ConfigurationManager within the company software. This is however excluded from github. Also the new models that have been created are in this exclusion map.

## How to run?
Dotnet 8.0 is required.
To run this project use `dotnet run` in the terminal.