# eq2statter

This [Advanced Combat Tracker (ACT)](http://advancedcombattracker.com/) plugin
will display a graph of combat stats during parsed encounters.

## Setup
1. Download ACT
2. Download the plugin
 [ActStatter.dll](https://github.com/eq2reapp/ActStatter/blob/master/bin/Release/ActStatter.dll?raw=true)
 to a location on your hard drive
3. Setup ACT as you would normally
4. Open ACT and select the __Plugins__ tab
5. Click __Browse...__, and select the location where you saved the plugin
6. Click __Add/Enable Plugin__
7. Click the new __Statter__ tab to configure the plugin
8. Add a couple of stats to start (eg. Fervor, Potency) by clicking the __+__ button and selecting the desired stat
    * Optionally select the colour that will be used to display the stat in the encounter graph
9. Take a moment to read the __Instructions__ on this tab, they'll explain a few of the less obvious features of the plugin

## Usage
1. Create a macro in the EQ2 client that includes the command __/do_file_commands statter.txt__
    * Every time you run this macro, your selected stats will be dumped to the log and parsed -- so use this macro as often as you would like to parse your stats
    * Add the macro step to a temp buff or other frequently used spell to track your stats without having to click the macro manually.
2. You should now be able to right-click on an encounter in ACT, and select __View Encounter Stats__ at the bottom of the menu
3. This will open a window where you will see the stats that you selected in the initial setup
4. Click on a stat to see it graphed over the duration of the encounter
    * If you select more than one stat, they will be overlayed on the same graph (albeit using the same scale...)
4. If you mouse-over the graph, it will show you the time and stat value at the mouse's position
5. The grey area on the graph (only applicable when a single stat is shown) represents the average over the duration of the encounter

## Building
This project was created using [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/).

The project user file is configured to launch ACT during debug -- this path may be different on your machine.

The entry point for the plugin is in __StatterPlugin.cs__, and the main UI is __StatterUI.cs__.

The plugin saves its settings in the following location: __%userprofile%\appdata\Roaming\Advanced Combat Tracker\Config\Statter.config.xml__.

If you build and add the Debug version of the plugin, you will notice some extra info on the plugin settings tab, as well as logging to your desktop folder.

## Contact
Send an in-game tell to Skyfire.Reapp if you have questions.

Bug reports [can be filed here](https://github.com/eq2reapp/ActStatter/issues), and I'll try my best to get to them!
