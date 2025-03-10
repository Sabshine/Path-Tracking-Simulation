# Improved Stanley Branch
This branch makes use of System.Windows.Forms to animated the movement of the different path tracking algorithms. This has been code in Visual Studio Code, therefore the this [Microsoft Guide](https://learn.microsoft.com/en-gb/dotnet/desktop/winforms/how-to-create-a-windows-forms-application-from-the-command-line?view=netframeworkdesktop-4.8) has been followed. Painting and drawing controls can be found [here](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/custom-painting-drawing?view=netdesktop-8.0).
The three tracking algorithms:
- Baseline Controller: Only calculates how to get to the next waypoint and steers accordingly.
- Old Stanley Controller: Ontop of the geometric calculation it works with the crossTrackError.
- Improved Stanley Controller: Fixed the calculation error of the old Stanley Controller.

## How to run?
Dotnet 8.0 is required.
To run this project use `dotnet run` in the terminal.

## [Path Tracking Simulation Branch](https://github.com/Sabshine/Path-Tracking-Simulation/tree/path-tracking-simulation)
In this branch you can simulate the Baseline Controller, Stanley Controller and Pure Pursuit Controller.