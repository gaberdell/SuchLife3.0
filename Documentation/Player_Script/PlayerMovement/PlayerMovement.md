
# PlayerMovement

## Overview
`PlayerMovement.cs` handles player movement and rotation using Unity’s Input System. It supports both keyboard and mouse input, applying acceleration, deceleration, and rotation logic for smooth movement.

## **How to Use**
### **1. Attach the Script**
- Attach `PlayerMovement.cs` to the **player** GameObject.
- Assign the necessary **colliders** and **rigidbody** components in the **Inspector**.

### **2. Configure Values**
| Parameter | Type | Description |
|-----------|------|-------------|
| `input` | `PlayerInput` | Reference to the player’s input system. |
| `deadZone` | `float` | Minimum input magnitude before movement applies. |
| `maxSpeed` | `float` | Maximum movement speed. |
| `accel` | `float` | Acceleration applied when moving. |
| `stopingAccel` | `float` | Deceleration applied when stopping. |
| `plrCollider` | `CircleCollider2D` | Collider for the player. |
| `rigidBody` | `Rigidbody2D` | Rigidbody for physics-based movement. |
| `rotationSpeed` | `float` | Speed at which the player rotates. |

## **How It Works**
1. **Handles Movement**:
   - Reads input from `InputHandler`.
   - Applies acceleration if input exceeds `deadZone`.
   - Applies deceleration when input stops.

2. **Handles Rotation**:
   - If using a mouse, rotates toward the cursor.
   - Otherwise, rotates in the direction of movement.
   - Make sure the the rotation stays within the range of `[-180, 180]` degrees and applies a smooth rotation transition. 
   - Checks for any overshooting and adjusts the rotation

3. **Handle Collision**:
   - Currently going unused might use it later though for custom collision effects

