# PlayerCamera

## Overview
`PlayerCamera.cs` manages the camera's following behavior for the player. It allows the camera to follow the player smoothly, with an additional effect based on mouse movement. 

## **How to Use**
### **1. Attach the Script**
- Attach `PlayerCamera.cs` to the **camera** GameObject.
- Assign the **target** (usually the player’s **Transform**) in the **Inspector**.

### **2. Configure Values**
| Parameter | Type | Description |
|-----------|------|-------------|
| `target` | `Transform` | The target for the camera to follow (usually the player). |
| `smoothing` | `float` | The smoothing factor for camera movement. Higher values result in slower, smoother transitions. |
| `mouseOffsetEffect` | `float` | The strength of the camera's reaction to mouse movement. |

## **How It Works**
1. **Camera Following**:
   - The camera follows the player’s position 
   - Use `MathHelper.DampVec3` with the `smoothing` factor to ensure the smooth movement.

2. **Mouse-Based Camera Offset**:
	- Hotkey to toggle(TODO)
   - Camera position is adjusted based on the mouse's position.

4. **Screen Shake (TODO)**:
   - Add a shaking effect to the camera based on events.



