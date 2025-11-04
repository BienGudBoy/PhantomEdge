# Player Prefab Setup Guide

## Required Components

Add the following components to the player prefabs (Ninja.prefab, char_blue_0.prefab):

### 1. Rigidbody2D Component
- Mass: 1
- Linear Drag: 0
- Angular Drag: 0.05
- Gravity Scale: 3 (for feel of gravity)
- Collision Detection: Continuous
- Sleeping Mode: Never Sleep
- Freeze Rotation: Z-axis (check box)

### 2. Capsule Collider 2D (or Box Collider 2D)
- Size: Match sprite bounds
- Offset: Center the collider
- Is Trigger: Unchecked
- Edge Radius: 0.1 (for capsule)
- Direction: Vertical (for capsule)

### 3. PlayerController Script
- Move Speed: 5
- Sprint Multiplier: 1.5
- Jump Force: 10
- Ground Check Radius: 0.2
- Ground Check Point: Create empty child GameObject
- Ground Layer: Assign "Ground" layer

### 4. HealthSystem Script
- Max Health: 100

### 5. Input Actions Setup
- Attach Input System component
- Reference: Assets/InputSystem_Actions.inputactions
- Bind PlayerController methods to input actions in Inspector

## Ground Check Point Setup
Create an empty GameObject child of the player:
- Position: (0, -sprite_height/2, 0)
- Name: "GroundCheck"

## Layer Setup
Create a "Ground" layer and assign it to:
- Ground platforms
- Floor tiles
- Any surface the player should stand on

## Tag Setup
Set player prefabs to "Player" tag for enemy detection.




