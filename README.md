# Statter

This [Advanced Combat Tracker (ACT)](http://advancedcombattracker.com/) plugin adds a capability to view a graph of combat stats parsed after encounters.

## Setup (recommended)
1. Open ACT and select the `Plugins` tab
2. Click the `Get Plugins...` button
3. In the window that appears, select `(90) [EQ2] Statter` and then click `Download and Enable`
4. Follow the configuration instructions below

![image](https://user-images.githubusercontent.com/93482228/183271014-f705682f-06df-4868-8189-0267bf689f18.png)

## Setup (manual)
1. Download the latest version of the plugin [ActStatter.dll](https://github.com/eq2reapp/ActStatter/blob/master/bin/Release/ActStatter.dll?raw=true) to a location on your computer
2. Open ACT and select the `Plugins` tab
3. Click `Browse...` and select the location where you saved the plugin
4. Click `Add/Enable Plugin`
5. Follow the configuration instructions below

## Configuration
1. Click the new `Statter` tab inside the `Plugins` tab
2. Add a couple of stats to start (eg. Fervor, Potency) by clicking the `+` button and selecting the desired stat
3. Take a moment to read the `Instructions` on this tab, they'll explain a few of the less obvious features of the plugin (including how to generate the stats in-game)

![image](https://user-images.githubusercontent.com/93482228/183270941-47d67654-2217-4827-91b7-b477adcdc28d.png)

## Usage
1. Create a macro in-game that includes the command `/do_file_commands statter.txt`
    * Every time you run this macro, your selected stats will be dumped to the log and parsed -- so use this macro as often as you would like to parse your stats
    * You may want to add the macro step to a temp buff or other frequently used spell to track your stats without having to click the macro manually
2. You should now be able to right-click on an encounter in ACT, and select `View Encounter Stats` at the bottom of the menu
3. This will open a window where you can see the stats that you selected in the initial setup
4. Click on a stat to see it graphed over the duration of the encounter
    * If you select more than one stat, they will be overlayed on the same graph (albeit using the same scale...)
4. If you mouse-over the graph, it will show you the time and stat value at the mouse's position
5. The grey area on the graph (only applicable when a single stat is shown) represents the average over the duration of the encounter

![image](https://user-images.githubusercontent.com/93482228/183271036-0e4c1663-5dce-48b1-a6f6-c3ad0ba05e2a.png)

![image](https://user-images.githubusercontent.com/93482228/183271061-ff488d69-403d-4de2-8c36-1664065d021c.png)

## Building
This project was created using [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/).

The project user file is configured to launch ACT during debug -- this path may be different on your machine.

The entry point for the plugin is in __StatterPlugin.cs__, and the main UI is __StatterUI.cs__.

The plugin saves its settings in the following location: __%userprofile%\appdata\Roaming\Advanced Combat Tracker\Config\Statter.config.xml__.

If you build and add the Debug version of the plugin, you will notice some extra info on the plugin settings tab.

## Contact
Send an in-game tell to Skyfire.Reapp if you have questions.

Bug reports [can be filed here](https://github.com/eq2reapp/ActStatter/issues), and I'll try my best to get to them!
