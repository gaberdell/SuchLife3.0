# HelperFunctions

## Overview
`HelperFunctions.cs` provides utility functions.Includes a function to toggle the game's pause state by modifying `Time.timeScale`.

## **How to Use**



## **How It Works**
1. **Pausing and Resuming the Game**:
   - checks the current `Time.timeScale` value:
     - If it's `0` (paused), it sets `Time.timeScale` to `1` (resumes the game).
     - Otherwise, it sets `Time.timeScale` to `0` (pauses the game).
   - Returns a boolean indicating the new pause state (`true` for paused, `false` for running).
   
