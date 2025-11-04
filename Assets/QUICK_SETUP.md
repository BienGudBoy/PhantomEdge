# PhantomEdge - Quick Setup Reference Card

## ?? 5-Minute Setup Checklist

### Step 1: Build Settings (2 min)
```
File > Build Settings
Drag these scenes in order:
1. Mainmenu.unity
2. Scene1.unity
3. Scene2.unity
4. Hub.unity
```

### Step 2: Main Menu Scene (1 min)
```
Open: Mainmenu.unity

Add to Scene:
- Canvas (UI)
  ?? MainMenu (Empty GameObject)
      - Add MainMenu.cs component
      - Create 5 buttons (Start, Continue, Settings, Credits, Quit)
- AudioManager (Empty GameObject)
  - Add AudioManager.cs + AudioSource
- GameManager (Empty GameObject)
  - Add GameManager.cs
```

### Step 3: Gameplay Scenes Setup (2 min per scene)
```
Open: Scene1.unity (repeat for Scene2, Hub)

Add to Scene:
- Canvas (UI)
  ?? UIManager (Empty GameObject)
      - Add UIManager.cs
      - Create: Health Bar (Slider), Health Text, Score Text
      - Create: Game Over Panel (with buttons)
      - Create: Victory Panel (with buttons)
  ?? PauseMenu (Empty GameObject)
      - Add PauseMenu.cs
      - Create: Pause Panel (with Resume/Restart/Menu/Quit buttons)

- SpawnPoints (Empty GameObject)
  ?? SpawnPoint1, SpawnPoint2, etc. (position around level)

- SpawnManager (Empty GameObject)
  - Add SpawnManager.cs
  - Assign spawn points
  - Create waves with enemy prefabs

- CheckpointSystem (Empty GameObject)
  - Add CheckpointSystem.cs

- Checkpoint1, Checkpoint2 (Empty GameObjects)
  - Add Checkpoint component
  - Add Collider2D (Is Trigger: ON)
  - Add visual sprites
```

---

## ?? Quick Component Reference

### Player Must Have:
```
? PlayerController.cs
? HealthSystem.cs
? PlayerCombat.cs
? Rigidbody2D (NOT kinematic)
? Collider2D
? Animator
? Tag: "Player"
```

### Enemy Must Have:
```
? EnemyController.cs
? EnemyHealth.cs
? Rigidbody2D (NOT kinematic, Gravity: 1)
? Collider2D
? Animator
? Tag: "Enemy"
```

---

## ?? Inspector Assignment Quick List

### UIManager Assignments:
- healthBar ? Health Slider
- healthText ? Health TextMeshPro
- scoreText ? Score TextMeshPro
- gameOverPanel ? Game Over Panel GameObject
- finalScoreText ? Final Score TextMeshPro (in panel)
- victoryPanel ? Victory Panel GameObject
- victoryScoreText ? Victory Score TextMeshPro (in panel)

### PauseMenu Assignments:
- pauseMenuPanel ? Pause Panel GameObject
- resumeButton ? Resume Button
- restartButton ? Restart Button
- mainMenuButton ? Main Menu Button
- quitButton ? Quit Button

### MainMenu Assignments:
- startButton ? Start Game Button
- continueButton ? Continue Button
- settingsButton ? Settings Button
- creditsButton ? Credits Button
- quitButton ? Quit Button
- settingsPanel ? Settings Panel GameObject
- volumeSlider ? Volume Slider
- settingsCloseButton ? Settings Close Button
- creditsPanel ? Credits Panel GameObject
- creditsCloseButton ? Credits Close Button

### SpawnManager Assignments:
- spawnPoints ? Array of spawn point transforms
- waves ? Configure wave settings:
  - Wave Name
  - Enemy Spawns (prefab + count)
  - Time Between Spawns
  - Delay Before Next Wave

### Checkpoint Assignments:
- activeIndicator ? Active sprite GameObject
- inactiveIndicator ? Inactive sprite GameObject

### AudioManager Assignments:
- damageClip ? Damage sound AudioClip
- scoreClip ? Score/collect sound AudioClip
- clickClip ? UI click sound AudioClip

---

## ?? Button OnClick Events Quick Setup

### Main Menu Buttons:
- Start ? MainMenu.OnStartButton()
- Continue ? MainMenu.OnContinueButton()
- Settings ? MainMenu.OnSettingsButton()
- Credits ? MainMenu.OnCreditsButton()
- Quit ? MainMenu.OnQuitButton()
- Settings Close ? MainMenu.OnSettingsClose()
- Credits Close ? MainMenu.OnCreditsClose()

### Pause Menu Buttons:
- Resume ? PauseMenu.OnResumeButton()
- Restart ? PauseMenu.OnRestartButton()
- Main Menu ? PauseMenu.OnMainMenuButton()
- Quit ? PauseMenu.OnQuitButton()

### Game Over Panel Buttons:
- Restart ? UIManager.OnRestartButton()
- Main Menu ? UIManager.OnMenuButton()

### Victory Panel Buttons:
- Next Level ? UIManager.OnNextLevelButton()
- Main Menu ? UIManager.OnMenuButton()

---

## ? Common Settings

### Canvas:
```
Render Mode: Screen Space - Overlay
Canvas Scaler: Scale With Screen Size
Reference Resolution: 1920 x 1080
Match: 0.5
```

### Health Bar Slider:
```
Min Value: 0
Max Value: 100 (will be set by code)
Whole Numbers: ON
Fill Rect: Set to green/red gradient
```

### Panels (Settings, Credits, Pause, Game Over, Victory):
```
Initially: INACTIVE (uncheck in Inspector)
Anchors: Stretch full screen
Background: Semi-transparent dark color
```

### Checkpoint Collider:
```
Is Trigger: ON
Size: Adjust to cover checkpoint area (e.g., 2x2)
```

### Spawn Points:
```
Just empty GameObjects with transforms
Position them where you want enemies to spawn
SpawnManager will handle the spawning
```

---

## ?? Testing Order

1. **Main Menu:**
   - Play Mainmenu scene
   - Click Start ? Should load Scene1
   - Test Settings and Credits panels

2. **Gameplay:**
   - Play Scene1
   - Check health bar updates
   - Check score updates
   - Press ESC ? Pause menu should appear
   - Test all pause menu buttons

3. **Combat:**
   - Attack enemies
   - Take damage
   - Verify sounds play
   - Check enemy spawns in waves

4. **Checkpoints:**
   - Touch checkpoint
   - Take damage until death
   - Verify respawn at checkpoint

5. **Victory/Game Over:**
   - Complete all waves ? Victory screen
   - Die without checkpoint ? Game Over screen
   - Test buttons on both screens

---

## ?? Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| UI not visible | Check Canvas settings and Camera |
| Buttons don't work | Verify OnClick events assigned |
| No spawn | Check SpawnManager has waves and prefabs |
| No pause | Ensure PauseMenu panel assigned |
| No health bar | Verify UIManager finds player |
| No respawn | Check CheckpointSystem in scene |
| No sound | Verify AudioManager has clips assigned |

---

## ?? Remember

- **Scripts are DONE** ?
- **Unity setup is NEXT** ??
- **Follow INTEGRATION_GUIDE.md** for detailed steps ??
- **Test after each section** ??

**You got this! ??**
