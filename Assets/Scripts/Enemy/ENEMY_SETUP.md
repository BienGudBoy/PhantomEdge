# Enemy Prefab Setup Guide

## Required Components for Enemies

Add the following components to all enemy prefabs (Globlin, Skeleton, Mushroom, Flying eye, Sword):

### 1. Rigidbody2D Component
- Mass: 1
- Linear Drag: 1
- Angular Drag: 0.05
- Gravity Scale: 2
- Collision Detection: Continuous
- Sleeping Mode: Never Sleep
- Freeze Rotation: Z-axis (check box)

### 2. Capsule Collider 2D (or Box Collider 2-point) 
- Size: Match sprite bounds
- Offset: Center the collider
- Is Trigger: Unchecked
- Edge Radius: 0.1

### 3. EnemyController Script
- Patrol Speed: 2
- Chase Speed: 4
- Patrol Distance: 5
- Detection Range: 8
- Attack Range: 1.5

### 4. EnemyHealth Script
- Max Health: 50 (or adjust per enemy type)
  - Flying Eye: 30 (weaker)
  - Goblin: 50 (standard)
  - Skeleton: 60 (stronger)
  - Mushroom: 40
  - Sword: 70 (strongest)

### 5. Tag Setup
Set enemy prefabs to "Enemy" tag for player detection

## Layer Setup
Assign enemies to "Enemy" layer (create if doesn't exist)

## AI Behavior Types
Enemies will automatically:
1. **Patrol**: Walk back and forth in a range
2. **Idle**: Pause between patrol points
3. **Chase**: When player detected within range
4. **Attack**: When player is in attack range
5. **Hurt**: When taking damage
6. **Dead**: When health reaches 0




