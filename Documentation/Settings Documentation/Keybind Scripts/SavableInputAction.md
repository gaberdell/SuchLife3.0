# SavableInputAction
> Found in KeybindsSettingController.cs
> Note it does not inherit from monobehavior or anything else for that matter

## Description
### Use
Please refer to the KeybindsSettingController documentation for an overview of what this is used for. As this is a helper function for it.
### How it works
A helper class that handles the interactive rebinding process by abstracting it out so it is easy to make more. It needs the prerequisites of the model (button), reset model (reset button), view (KeybindSettingsView), reset view (KeybindSettingsView), input action reference, a bool to tell it if it is multi action or not, alongside its multi action index (will be ignored if it is not multi-action), save string
The public function startRebinding is used to begin the classes repinding process. This is used in the FullscreenSettingsController where it is subscribed to its models onClick event. End rebind is called when the rebound has ended and is called from the interactive rebind in the startRebinding event. Rest function is used on the inside to reset the value back to its original state before the player altered with it. It additionally communicates to the reset view in order to properly update it. The load function is called by the KeybindsSettingController in order to load its value into the place on KeybindsSettingController Starting through Unity's PlayerPrefs. The save function is called whenever the value is changed and saves it through PlayerPrefs.