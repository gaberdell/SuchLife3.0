# Health

## Overview
`Health.cs` manages the health system for a GameObject, allowing it to take damage, heal, and die. It can also display health as text in the scene and trigger events when damage is taken or the object dies.

## **How to Use**
### **1. Attach the Script**
- Attach `Health.cs` to any GameObject that requires a health system (e.g., player, enemy).
- Set the `maxHealth` property in the Inspector to define the maximum health value.
- Optionally, enable `showHealthText` to display health in the scene.

### **2. Use the Public Methods**
- **`TakeDamage(int amount)`**: Reduces the object's health by the specified amount.
- **`Heal(int amount)`**: Restores the object's health by the specified amount.
- The `onDeath` and `onDamageTaken` UnityEvents can be hooked up in the Inspector to trigger actions when these events occur.

## **How It Works**
1. **Health System**:
   - When the `TakeDamage` method is called, the `currentHealth` is reduced by the specified amount. If the health drops to 0 or below, the `Die` method is invoked, and the GameObject is destroyed.
   - The `Heal` method restores health but ensures it doesn't exceed `maxHealth`.
   
2. **Text Display**:
   - If `showHealthText` is enabled, a `TextMesh` is created as a child of the GameObject to display the current health value in the world.
   - The health text updates whenever the player takes damage or heals.


## **Inspector Properties**
| Property         | Type         | Description |
|------------------|--------------|-------------|
| `maxHealth`      | `int`        | Maximum health of the GameObject. |
| `currentHealth`  | `int`        | Current health of the GameObject (automatically updated). |
| `showHealthText` | `bool`       | If true, a health text is displayed in the scene. |
| `onDeath`        | `UnityEvent` | Event triggered when the object dies. |
| `onDamageTaken`  | `UnityEvent<int>` | Event triggered when damage is taken, passing the damage amount. |


