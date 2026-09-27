# Unseen without Untoggling (UwU) System

Adds a unity component that integrates with VRCFury to easily create parameters that toggle on and off based on the built-in IsLocal and IsOnFriendsList parameters as well as other customizable conditions.

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

![component](Media/Component.png)

* `Output Prefix (Required):` The prefix for the `Default Output Variable` as well as a name for the files and other variables needed for the UwU System to work. If this is left blank, the component will do nothing.
  * `Default Output Variable:` Displays the name of the parameter that will be used as the starting point of the logic for the rest of the System, but can also be used by itself more simple setups. The name is always `Output Prefix`/Load.
* `Threshold State:` This is the resulting value of the `Default Output Variable` when within the `Visibility Threshold`. Default is "true".
* `Add In-Game Menu:` Adds a menu at the path specified in `Menu Path`. Default is "true".
* `Allow False/True for Self:` Allows the UwU System to be "turned off". Adds a "None" option to the `Visibility Threshold` list. Name depends on the setting of `Threshold State` above. Default is "false".
* `Visibility Threshold:` Changes the situations when the `Default Output Variable` will be the value set in `Threshold State`. Possible entries are "Self Only", "Friends and Self Only", and "Everyone".
* `Menu Path:` Only visible if `Add In-Game Menu` is checked. Allows setting the in-game menu to a custom path in your avatar's expression menu. Subfolders are indicated with a `/`. Default is "UwU System".
* `Self/Friends/Global Toggle Saved:` Only visible if `Add In-Game Menu` is checked. Allows the Self, Friends, and Global toggle of the in-game menu to be saved between worlds.
* `Additional Output Parameters & Conditions`: Optional settings that allow for more custom parameters to be made with optional custom conditions for more advanced setups.
  * `Custom Output # Name:` Allows setting the name of a custom parameter as an output
  * `Condition State:` Allows setting the state of the Output parameter when the `Default Output Variable` and the Conditions set below it (if any) are true. Default is "true".
  * `Conditions`: List of conditions for the `Custom Output # Name`
    * Enter the Name, Type, and desired value of Parameter(s)

That's it!
Some other notes:
* Currently condition parameters can only be listed once per custom output parameter.

## 🎉 Publishing a Release

You can make a release by running the [Build Release](.github/workflows/release.yml) action. The version specified in your `package.json` file will be used to define the version of the release.

## 📃 Rebuilding the Listing

Whenever you make a change to a release - manually publishing it, or manually creating, editing or deleting a release, the [Build Repo Listing](.github/workflows/build-listing.yml) action will make a new index of all the releases available, and publish them as a website hosted fore free on [GitHub Pages](https://pages.github.com/). This listing can be used by the VPM to keep your package up to date, and the generated index page can serve as a simple landing page with info for your package. The URL for your package will be in the format `https://username.github.io/repo-name`.

## 🏠 Customizing the Landing Page (Optional)

The action which rebuilds the listing also publishes a landing page. The source for this page is in `Website/index.html`. The automation system uses [Scriban](https://github.com/scriban/scriban) to fill in the objects like `{{ this }}` with information from the latest release's manifest, so it will stay up-to-date with the name, id and description that you provide there. You are welcome to modify this page however you want - just use the existing `{{ template.objects }}` to fill in that info wherever you like. The entire contents of your "Website" folder are published to your GitHub Page each time.

## 💻 Technical Stuff

You are welcome to make your own changes to the automation process to make it fit your needs, and you can create Pull Requests if you have some changes you think we should adopt. Here's some more info on the included automation:

### Build Release Action
[release.yml](/.github/workflows/release.yml)

This is a composite action combining a variety of existing GitHub Actions and some shell commands to create both a .zip of your Package and a .unitypackage. It creates a release which is named for the `version` in the `package.json` file found in your target Package, and publishes the zip, the unitypackage and the package.json file to this release.

### Build Repo Listing
[build-listing.yml](.github/workflows/build-listing.yml)

This is a composite action which builds a vpm-compatible [Repo Listing](https://vcc.docs.vrchat.com/vpm/repos) based on the releases you've created. In order to find all your releases and combine them into a listing, it checks out [another repository](https://github.com/vrchat-community/package-list-action) which has a [Nuke](https://nuke.build/) project which includes the VPM core lib to have access to its types and methods. This project will be expanded to include more functionality in the future - for now, the action just calls its `BuildRepoListing` target.
