# VolumeSettingsController
> Found in VolumeSettingsController.cs

## Description
### Use
Create a type value pair in the editor. Connect the model (slider), the VolumeTextView for each instance of a mixer group, and the name of its associated audio mixer.
### How it works
Script on enable makes sure to subscribe the SliderDataPair setSliderVal to the changed value. Also makes sure to unsubscribe them when it needs to. When the script loads it makes sure to call the load function to load in all the function.

