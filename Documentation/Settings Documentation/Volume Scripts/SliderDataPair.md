# SliderDataPair
> Found in VolumeSettingsController.cs
> Note it does not inherit from monobehavior or anything else for that matter

## Description
### Use
Refer to VolumeSettingsController as this is a helper function for it.
### How it works
VolumeSettingsController binds its set volume function when it is started. This will then trigger it, it uses the custom MathHelper class in order to perform the calculations from a slider value to a logarithmic scale that is used by the mixer group system.
