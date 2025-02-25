# ResolutionSettingsController
> Found in ResolutionSettingsController.cs

## Description
### Use
Tell it what textmesh pro drop down you want it to use to give resolutions to.
### How it works
Uses the UnityEngine to tell the gather all of the current available resolutions. Will display the resolutions inside of the dropdown box alongside setting the refresh rate to the one displayed on that resolution. Will save the current amount of resolutions so if it were to potentially change to another one that supports a different type it would return to default as opposed to keeping its previously set vlaue of the changed resolution.
### TODO:
A notable issue is that the save system is designed to throw away previously set screen resolution values if the player switches screens. Currently a low priorty issue sense I do not expect many players to switch run the game and then switch to another screen with different resolutions. However, it is something that should be fixed sometime.

