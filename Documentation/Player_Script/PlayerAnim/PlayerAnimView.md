# PlayerAnimView

## Overview
`PlayerAnimView.cs` handles the animation for the player character. It updates the player's animation speed based on the player's movement speed. 

## **How to Use**
### **1. Attach the Script**
- Attach `PlayerAnimView.cs` to the **player** GameObject.
- Assign the **Animator** component in the **Inspector** 

### **2. Configure Values**
| Parameter | Type | Description |
|-----------|------|-------------|
| `playerAnimator` | `Animator` | The Animator component for the player character. |
| `blendName` | `string` | The name of the animation blend parameter (e.g., "IdleToWalk"). |
| `speedOfWalkName` | `string` | The name of the parameter controlling the walk speed (e.g., "AnimWalkSpeed"). |

## **How It Works**
1. **Events**:
   - `EventManager.setPlayerAnimSpeed` to update the animation speed.

2. **Setting Speed**:
     - The `blendName` parameter (e.g., `"IdleToWalk"`) is set to the normalized speed value, clamped between `0` and `1`.
     - The `speedOfWalkName` parameter (e.g., `"AnimWalkSpeed"`) is set to the player's speed, ensuring a minimum value of `1`.


