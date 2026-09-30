# Unseen without Untoggling (UwU) System

Adds a unity component that integrates with VRCFury to easily create parameters that toggle on and off based on the built-in IsLocal and IsOnFriendsList parameters as well as other customizable conditions.

## ⚖️ Disclaimer

Users are solely responsible for ensuring that their use of this package, and any content created, modified, or uploaded with it, complies with the [Creator Guidelines](https://hello.vrchat.com/creator-guidelines) and [VRChat Terms of Service](https://hello.vrchat.com/terms)

## ▶ Getting Started

* Go to [This Link](https://Myrddin714.github.io/UwU_System)
to add this repository to your VCC or ALCOM.

  * Add this repository to your Unity project.
  * Choose or create an object in your avatar hierarchy.
  * Click the 'Add Component' button in the inspector window and select 'UwU System'.
  * Enter a Parameter `Output Prefix` in the first textbox.
  * Set other settings as desired (See below for details).
That's it. You can use the Parameters made by this component to trigger custom logic on your avatar (Examples below)

## 🔒 Requirements

* VRCFury is required for this package to work. Refer to the [VRCFury Website](https://vrcfury.com/download) for instructions on adding it to your project.
* The `Output Prefix` field is required and must be unique per avatar. If duplicates are found only one will be built.

## 🤖 Customizing the Component

Customizing the UwU System has many different options to allow you to tailor it to your use case.

!\[component](Media/Component.png)

* `Output Prefix (Required):` The prefix for the `Default Output Variable` as well as a name for the files and other variables needed for the UwU System to work. If this is left blank, the component will do nothing.

  * `Default Output Variable:` Displays the name of the parameter that will be used as the starting point of the logic for the rest of the System, but can also be used by itself more simple setups. The name is always `Output Prefix`/Load.
* `Threshold State:` This is the resulting value of the `Default Output Variable` when within the `Visibility Threshold`. Default is "true".
* `Add In-Game Menu:` Adds a menu at the path specified in `Menu Path`. Default is "true".
* `Allow False/True for Self:` Allows the UwU System to be "turned off". Adds a "None" option to the `Visibility Threshold` list. Name depends on the setting of `Threshold State` above. Default is "false".
* `Visibility Threshold:` Changes the situations when the `Default Output Variable` will be the value set in `Threshold State`. Possible entries are "Self Only", "Friends and Self Only", and "Everyone".
* `Menu Path:` Only visible if `Add In-Game Menu` is checked. Allows setting the in-game menu to a custom path in your avatar's expression menu. Subfolders are indicated with a `/`. Default is "UwU System".
* `Self/Friends/Global Toggle Saved:` Only visible if `Add In-Game Menu` is checked. Allows the Self, Friends, and Global toggle of the in-game menu to be saved between worlds.
* `Additional Output Parameters \& Conditions`: Optional settings that allow for more custom parameters to be made with optional custom conditions for more advanced setups.

  * `Custom Output # Name:` Allows setting the name of a custom parameter as an output
  * `Condition State:` Allows setting the state of the Output parameter when the `Default Output Variable` and the Conditions set below it (if any) are true. Default is "true".
  * `Conditions`: List of conditions for the `Custom Output # Name`

    * Enter the Name, Type, and desired value of Parameter(s)

## 📝 Notes

* The Assets folder `UwUTemp` is created and used by this tool to store the controllers that build when entering play mode or starting an avatar build with an UwU System component. It is not recommended to store other assets in this folder.
* Each UwU System component uses 3 synced bits on the avatar, but only if an in-game menu is used.
* Currently condition parameters have to be unique per custom output parameter. This may change in the future if needed.

