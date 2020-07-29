# Custom Weapons
#### A plugin that allows you to load custom weapons in Little Witch Nobeta and switch between them at runtime.

* [Installing](#Installing)
  * [BepInEx](#BepInEx)
  * [Plugin](#Plugin)
* [Usage](#Usage)
* [Creating weapons](#Creating-weapons)
  * [Unity](#Unity)
  * [Project](#Project)
* [Installing weapons](#Installing-weapons)


## Installing

### BepInEx
This plugin is made for BepInEx, so you will need to install it by [downloading it here](https://github.com/BepInEx/BepInEx/releases/latest/). You want `BepInEx_x86_*.zip` if you're on Windows. 
Simply unzip it into the root folder of your game, the one with `LittleWitchNobeta.exe`. (`winhttp.dll` should end up in the same folder.)

### Plugin
Until I make a release, you will need to build this project, which will create a `CustomWeapons.dll`.
After installing BepInEx drop `CustomWeapons.dll` into the `BepInEx/plugins/` folder in your game folder.

## Usage
In game after loading into a save you can press `F7` and `F8` to switch between your weapons.

If you have a debug version of the plugin you can use `F9` to dump the curret scene.

## Creating weapons
Creating weapons is very simple if you're familiar with Unity and 3D modeling. 

### Unity
You will need to use [Unity 5.6.0f3](). If you already have another version, you can install multiple versions at the same time by changing the install folder. 

Make sure you target `.Net 2.0.0`, by going to `Edit` -> `Project Settings` -> `Player` -> `Other Settings` -> `Configration`

### Project
Once you have the correct version of unity installed, create/open a project and install this [`.unitypackage`](https://nobeta.moe/files/CustomWeapons.unitypackage) into it.

You should see a `CustomWeapons.unity` Scene file, in there is the default weapon as an example to how yours should look.

You've got your root object, which you can name what you want, I named it `Default`. Here you will need to attach a `Weapon Descriptor` component. It has the following settings you want to change:


| Name              | Description             |
| ----------------- |:----------------------- |
|**Author Name**|Your name |
|**Weapon Name**|Your weapon's name|
|**Disable Effect**|Disables weapon effect (Charging spells)|
| **Cover Image**  |A sprite to use as a cover image (Not used by the plugin yet)| 


Inside your root object you should have another object called `Weapon`. Inside it you should put your own object containing whatever you want.
It should look like this:

* \<Weapon Name\>
  * Weapon
    * \<Your Model Here\>

:::warning
Note: scripts won't work unless they're compiled into this plugin, or into the game.
:::

After you're done and happy with your weapon, export it using the `Weapon Export` window. You can find it at `Window` -> `Weapon Exporter`. 
The exporter will prompt you to save a `.weapon` file.


## Installing weapons
You can drop that weapon file inside `Custom/Weapons/` in your game's root folder.
You will need to restart to reload new weapons, this is planned to change.
