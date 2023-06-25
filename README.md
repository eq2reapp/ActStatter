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

## Configuration and Usage
Please refer to the [wiki help article](https://github.com/eq2reapp/ActStatter/wiki/Help).

## Building
This project was created using [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/).

The project user file is configured to launch ACT during debug -- this path may be different on your machine.

The entry point for the plugin is in __StatterPlugin.cs__, and the main UI is __StatterUI.cs__.

The plugin saves its settings in the following location: __%userprofile%\appdata\Roaming\Advanced Combat Tracker\Config\Statter.config.xml__.

If you build and add the Debug version of the plugin, you will notice some extra info on the plugin settings tab.

## Contact
Send an in-game tell to Skyfire.Reapp if you have questions.

Bug reports [can be filed here](https://github.com/eq2reapp/ActStatter/issues), and I'll try my best to get to them!
