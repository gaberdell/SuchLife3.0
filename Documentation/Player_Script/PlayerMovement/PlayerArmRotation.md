
# PlayerArmRotation

## Overview
`PlayerArmRotation.cs` rotates a player's arm based on the parent's rotation. It creates a natural transition while ensuring the arm catches up if the difference exceeds a certain threshold.




## **How to Use**
### **1. Attach the Script**
- Attach `PlayerArmRotation.cs` to the **player's arm** GameObject.
- Assign the **parent** (e.g., the player’s body) in the **Inspector**.

### **2. Configure Values**
| Parameter | Type | Description |
|-----------|------|-------------|
| `parent` | `Transform` | Reference to the parent object (defaults to the GameObject’s parent if not set). |
| `angleSmoothing` | `float` | Controls how smoothly the arm follows the parent’s rotation (higher = slower response). |
| `catchUpAngle` | `float` | The threshold angle difference before the arm instantly adjusts to match the parent. |



## **How It Works**
1. **Calculate Rotation Difference**:  
   - `armRotDif` measures the difference between the arm’s current rotation and the parent’s rotation.  
   - `MathHelper.RotationTo180Scale` ensures the angle stays within `(-180, 180)`.

2. **Catch-Up Mechanism**:  
   - If the difference exceeds `catchUpAngle`, the arm instantly rotates closer.  
   - Otherwise, it smoothly adjusts using `MathHelper.DampAngle`.

3. **Apply Rotation**:  
   - Updates the arm’s rotation each frame to maintain a smooth following effect.

