# PhantomEdge - Setup and Integration Guide

## ? Completed Scripts

### UI Systems
- **UIManager.cs** - HUD management (health bar, score display, game over/victory panels)
- **PauseMenu.cs** - Pause menu with resume/restart/main menu/quit functionality
- **MainMenu.cs** - Main menu with start/continue/settings/credits/quit buttons

### Gameplay Systems
- **SpawnManager.cs** - Wave-based enemy spawning with multiple spawn points
- **CheckpointSystem.cs** - Checkpoint and respawn system

### Enemy System Enhancement
- **EnemyHealth.cs** - Added OnDeath and OnHealthChanged events for better integration

---

## ?? Integration Steps

### 1. Scene Setup - Build Settings

Add scenes to Build Settings in this order:
1. `Assets/Scenes/Mainmenu.unity` (Index 0)
2. `Assets/Scenes/Scene1.unity` (Index 1)
3. `Assets/Scenes/Scene2.unity` (Index 2)
4. `Assets/Scenes/Hub.unity` (Index 3)

**How to add scenes:**
- Go to File > Build Settings
- Drag scenes from Project window into "Scenes In Build"
- Reorder them by dragging

---

### 2. Main Menu Scene Setup (Mainmenu.unity)

#### Create UI Canvas:
1. Right-click in Hierarchy > UI > Canvas
2. Set Canvas Scaler to "Scale With Screen Size" (1920x1080 reference)

#### Add MainMenu GameObject:
1. Create Empty GameObject named "MainMenu"
2. Add `MainMenu.cs` component
3. Create UI Buttons:
   - Start Game
   - Continue (disabled by default)
   - Settings
   - Credits
   - Quit

#### Settings Panel:
1. Create Panel (child of Canvas)
2. Add Volume Slider
3. Add Close Button
4. Initially set inactive

#### Credits Panel:
1. Create Panel (child of Canvas)
2. Add Text (credits content)
3. Add Close Button
4. Initially set inactive

#### AudioManager Setup:
1. Create Empty GameObject named "AudioManager"
2. Add `AudioManager.cs` component
3. Add AudioSource component
4. Assign audio clips in Inspector:
   - Damage Clip
   - Score Clip
   - Click Clip

#### GameManager Setup:
1. Create Empty GameObject named "GameManager"
2. Add `GameManager.cs` component

---

### 3. Gameplay Scene Setup (Scene1, Scene2, Hub)

#### Add UIManager:
1. Create UI > Canvas (if not exists)
2. Create Empty GameObject "UIManager" under Canvas
3. Add `UIManager.cs` component

#### HUD Elements:
Create these under Canvas:
1. **Health Bar:**
   - UI > Slider (name: "HealthBar")
   - Set Fill color to red/green
   - Assign to UIManager's healthBar field

2. **Health Text:**
   - UI > Text - TextMeshPro (name: "HealthText")
   - Position near health bar
   - Assign to UIManager's healthText field

3. **Score Text:**
   - UI > Text - TextMeshPro (name: "ScoreText")
   - Position top-right corner
   - Assign to UIManager's scoreText field

#### Game Over Panel:
1. Create Panel (name: "GameOverPanel")
2. Add TextMeshPro "Game Over" title
3. Add TextMeshPro for final score
4. Add Buttons:
   - Restart (calls UIManager.OnRestartButton)
   - Main Menu (calls UIManager.OnMenuButton)
5. Initially set inactive
6. Assign to UIManager's gameOverPanel field

#### Victory Panel:
1. Create Panel (name: "VictoryPanel")
2. Add TextMeshPro "Victory!" title
3. Add TextMeshPro for final score
4. Add Buttons:
   - Next Level (calls UIManager.OnNextLevelButton)
   - Main Menu (calls UIManager.OnMenuButton)
5. Initially set inactive
6. Assign to UIManager's victoryPanel field

#### Add PauseMenu:
1. Create Empty GameObject "PauseMenu" under Canvas
2. Add `PauseMenu.cs` component
3. Create Pause Panel:
   - Panel (name: "PauseMenuPanel")
   - Add TextMeshPro "PAUSED" title
   - Add Buttons:
     * Resume (assign to resumeButton field)
     * Restart (assign to restartButton field)
     * Main Menu (assign to mainMenuButton field)
     * Quit (assign to quitButton field)
4. Initially set PauseMenuPanel inactive
5. Assign panel and buttons in PauseMenu Inspector

---

### 4. Enemy Spawning Setup

#### For Wave-Based Levels (Scene1, Scene2):

1. **Create Spawn Points:**
   - Create Empty GameObject named "SpawnPoints"
   - Create multiple child Empty GameObjects (name: "SpawnPoint1", "SpawnPoint2", etc.)
   - Position them around the level

2. **Add SpawnManager:**
   - Create Empty GameObject named "SpawnManager"
   - Add `SpawnManager.cs` component
   - Configure in Inspector:
     * Assign spawn point transforms
     * Create waves:
       - Click "+" to add a wave
       - Set wave name
       - Add enemy spawns (assign prefabs, set counts)
       - Set timing parameters
     * Enable Auto Start Waves
     * Set Loop Waves if desired

**Example Wave Setup:**
```
Wave 1:
- 3x Goblin enemies
- 2 second delay between spawns
- 5 second delay before next wave

Wave 2:
- 2x Skeleton enemies
- 2x Mushroom enemies
- 3 second delay between spawns
- 5 second delay before next wave
```

---

### 5. Checkpoint Setup

1. **Add CheckpointSystem:**
   - Create Empty GameObject named "CheckpointSystem"
   - Add `CheckpointSystem.cs` component
   - Configure respawn settings in Inspector

2. **Create Checkpoint Objects:**
   - Create Empty GameObject (name: "Checkpoint1")
   - Add `Checkpoint` component (part of CheckpointSystem.cs)
   - Add Collider2D component (set as Trigger)
   - Add visual indicators:
     * Create child sprite for inactive state
     * Create child sprite for active state
   - Assign visual GameObjects to Checkpoint component
   - Place checkpoint at strategic locations

---

### 6. Player Setup Verification

Ensure player has these components:
- `PlayerController.cs`
- `HealthSystem.cs`
- `PlayerCombat.cs`
- Rigidbody2D (NOT kinematic)
- Collider2D
- Animator
- Tag: "Player"

---

### 7. Enemy Prefab Verification

Each enemy prefab needs:
- `EnemyController.cs`
- `EnemyHealth.cs`
- Rigidbody2D (NOT kinematic, Gravity Scale = 1)
- Collider2D
- Animator
- Tag: "Enemy"

---

### 8. Testing Checklist

#### Main Menu:
- [ ] Start button loads Scene1
- [ ] Settings panel opens/closes
- [ ] Volume slider works
- [ ] Credits panel opens/closes
- [ ] Quit button works

#### Gameplay:
- [ ] Health bar updates when taking damage
- [ ] Score increases when collecting items/defeating enemies
- [ ] Pause menu opens with Escape key
- [ ] Resume button works
- [ ] Restart button reloads scene
- [ ] Player respawns at checkpoint after death

#### Enemy Spawning:
- [ ] Enemies spawn in waves
- [ ] Spawn points randomize correctly
- [ ] Wave delay works between waves
- [ ] Score increases on enemy death
- [ ] All waves complete message appears

#### Checkpoints:
- [ ] Checkpoint activates when player touches it
- [ ] Visual indicator changes on activation
- [ ] Player respawns at checkpoint after death

---

## ?? Recommended Next Steps

### Polish:
1. Add background music to scenes
2. Add ambient sound effects
3. Create particle effects for:
   - Enemy death
   - Checkpoint activation
   - Damage hits
   - Collectibles
4. Add screen shake on damage
5. Add UI animations (fade in/out)

### Additional Features:
1. Save/Load system (PlayerPrefs or JSON)
2. Power-up system
3. Boss battles
4. Multiple player characters
5. Difficulty settings
6. Achievements system

---

## ?? Common Issues & Solutions

### UI not appearing:
- Check Canvas is set to Screen Space - Overlay
- Verify Canvas Scaler is configured
- Check Z position of UI elements (should be 0)

### Enemies not spawning:
- Verify enemy prefabs are assigned in SpawnManager
- Check spawn points are assigned
- Ensure Auto Start Waves is enabled
- Check console for spawn errors

### Player not respawning:
- Verify CheckpointSystem exists in scene
- Check player has HealthSystem component
- Ensure checkpoint has Trigger collider
- Check player tag is "Player"

### Buttons not working:
- Verify button has Button component
- Check OnClick events are assigned
- Ensure AudioManager exists for click sounds
- Check GameManager exists in scene

---

## ?? Integration Support

All scripts are fully documented with:
- Serialized fields marked with [Header] for organization
- Public properties for external access
- Events for loose coupling
- Null checks for safety
- Debug logs for troubleshooting

Refer to individual script comments for detailed usage.

---

## ? Script Features Summary

### UIManager:
- Auto-finds player in scene
- Subscribes to GameManager events
- Updates HUD in real-time
- Handles game over/victory screens
- Provides public methods for buttons

### PauseMenu:
- Listens for Escape key
- Pauses/resumes game via GameManager
- Handles time scale
- Audio feedback on button clicks

### MainMenu:
- Scene loading functionality
- Settings panel with volume control
- Credits panel
- Continue button (ready for save system)

### SpawnManager:
- Wave-based spawning
- Multiple spawn points
- Random or sequential spawning
- Tracks active enemies
- Completion detection
- Visual spawn radius gizmos

### CheckpointSystem:
- Auto-respawn on death
- Visual checkpoint indicators
- Configurable respawn health
- Singleton pattern for global access

---

## ?? Priority Tasks Remaining

1. **CRITICAL - Unity Editor Setup:**
   - Add scenes to Build Settings
   - Create UI elements in scenes
   - Assign references in Inspector
   - Test all connections

2. **HIGH - Level Population:**
   - Place enemies in Scene1 and Scene2
   - Add collectibles
   - Set up spawn points
   - Add checkpoints

3. **MEDIUM - Polish:**
   - Add background music
   - Add sound effects
   - Test full gameplay loop
   - Balance difficulty

Remember: Most of the code is complete! The remaining work is primarily Unity Editor setup and configuration.
