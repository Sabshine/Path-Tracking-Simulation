# Path Tracking Simulation Branch
This branch makes use of System.Windows.Forms to animated the movement of the different path tracking algorithms. This has been code in Visual Studio Code, therefore the this [Microsoft Guide](https://learn.microsoft.com/en-gb/dotnet/desktop/winforms/how-to-create-a-windows-forms-application-from-the-command-line?view=netframeworkdesktop-4.8) has been followed. Painting and drawing controls can be found [here](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/custom-painting-drawing?view=netdesktop-8.0).
The three tracking algorithms:
- Baseline Controller: Only calculates how to get to the next waypoint and steers accordingly.
- Stanley Controller: Ontop of the geometric calculation it works with the crossTrackError.
- Pure Pursuit Controller: Ontop of the geometric calculation it works with the crossTrackError and has a look ahead distance.

## How to run?
Dotnet 8.0 is required.
To run this project use `dotnet run` in the terminal.

## [Improved Stanley Branch](https://github.com/Sabshine/Path-Tracking-Simulation/tree/improved-stanley-simulation)
This branch contains an improved version of the Stanley controller.
