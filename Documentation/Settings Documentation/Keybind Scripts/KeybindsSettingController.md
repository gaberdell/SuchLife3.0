# KeybindsSettingController
> Found in KeybindsSettingController.cs

## Description
### Use
Use this class to manage your input action rebind buttons. Manage a set of rebind buttons easiest way is to use the SetActionButton and ResetActionButton prefabs. Add a new SavableInputAction to the serialized array in the editor. Specify its Model (the button), Reset Model (the reset button). The view (the script that handles how the action button will look) alongside the reset view (handles the reset buttons look).
Specify an input action reference. Then specify if it is multi-action (Move for example counts as it contains the multiple WASD keys). Specify the multi-action keys index if it is a multi action. Finally specify the save action name. Prefered save action name is the action name followed by a piece to specify its specific action if it is multi action then by the "Save".

### How it works
> Inherits monobehavior manages a set of buttons that the player can bind buttons to. It does this by representing them using the serializable class SavableInputAction