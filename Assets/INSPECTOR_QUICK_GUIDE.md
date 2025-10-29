# ?? Unity Inspector Quick Assignment Guide

**YOUR ONLY REMAINING TASK - 15-30 Minutes!**

All GameObjects, Components, and Scripts are created. You just need to connect the dots!

---

## ? IMPORTANT: Sorting Layer Fix Applied!

**? If your characters were rendering behind backgrounds:**

The sorting layers have been automatically fixed in Scene1! 

For Scene2 and Hub, run: **Menu ? Tools ? Fix Sorting Layers**

See `Assets/SORTING_LAYER_FIX.md` for details.

---

## ?? Checklist Format

Use this checklist for each scene. Check off items as you complete them!

---

## Scene1 Integration Checklist

### ? UIManager Assignments
- [ ] Open **Scene1** in Unity
- [ ] Select **UIManager** GameObject in Hierarchy
- [ ] In Inspector, find these **empty fields** (they'll show "None"):
  - [ ] `Health Bar` field ? Drag **HealthBar** from Hierarchy
  - [ ] `Health Text` field ? Drag **HealthText** from Hierarchy
  - [ ] `Score Text` field ? Drag **ScoreText** from Hierarchy
  - [ ] `Game Over Panel` field ? Drag **GameOverPanel** from Hierarchy
  - [ ] `Victory Panel` field ? Drag **VictoryPanel** from Hierarchy
- [ ] Click **Apply** at top of Inspector (if it's a prefab)

### ? PauseMenu Assignments
- [ ] Select **PauseMenu** GameObject in Hierarchy
- [ ] In Inspector, assign:
  - [ ] `Pause Menu Panel` field ? Drag **PauseMenuPanel** from Hierarchy

*Note: Button assignments come after you create buttons (see UI Creation below)*

### ? SpawnManager Configuration
- [ ] Select **SpawnManager** GameObject in Hierarchy
- [ ] In Inspector, find **Spawn Points** array:
  - [ ] Set Size to **4**
  - [ ] Element 0 ? Drag **SpawnPoint1** (from SpawnPoints parent)
  - [ ] Element 1 ? Drag **SpawnPoint2**
  - [ ] Element 2 ? Drag **SpawnPoint3**
  - [ ] Element 3 ? Drag **SpawnPoint4**
- [ ] Scroll down to **Waves** array:
  - [ ] Click **+** to add Wave 1
  - [ ] Set Wave Name: "Wave 1"
  - [ ] In Enemy Spawns array, click **+**
  - [ ] Assign **Enemy Prefab** (find Globlin/Skeleton in Project)
  - [ ] Set **Enemy Count**: 3
  - [ ] Set **Spawn Delay**: 2
  - [ ] Set **Wave Delay**: 5
- [ ] Check ?? **Auto Start Waves**
- [ ] Check ?? **Loop Waves** (optional)

### ? AudioManager Configuration
- [ ] Select **AudioManager** GameObject in Hierarchy
- [ ] In Inspector, assign (find in Project window):
  - [ ] `Damage Clip` ? Drag audio file
  - [ ] `Score Clip` ? Drag audio file
  - [ ] `Click Clip` ? Drag audio file

### ? UI Panel Creation (Game Over)
- [ ] Select **GameOverPanel** in Hierarchy
- [ ] Right-click GameOverPanel ? UI ? Text - TextMeshPro
  - [ ] Rename to "GameOverTitle"
  - [ ] Set Text: "GAME OVER"
  - [ ] Set Font Size: 72
  - [ ] Center alignment
  - [ ] Position at top-center of panel
- [ ] Right-click GameOverPanel ? UI ? Button
  - [ ] Rename to "RestartButton"
  - [ ] Position in center
  - [ ] Change button text to "Restart"
  - [ ] In Inspector, scroll to **Button** component
  - [ ] Click **+** in OnClick() event
  - [ ] Drag **UIManager** GameObject into object field
  - [ ] Select function: UIManager ? OnRestartButton()
- [ ] Right-click GameOverPanel ? UI ? Button
  - [ ] Rename to "MainMenuButton"
  - [ ] Position below Restart button
  - [ ] Change text to "Main Menu"
  - [ ] OnClick() ? UIManager ? OnMenuButton()

### ? UI Panel Creation (Victory)
- [ ] Select **VictoryPanel** in Hierarchy
- [ ] Right-click VictoryPanel ? UI ? Text - TextMeshPro
  - [ ] Rename to "VictoryTitle"
  - [ ] Set Text: "VICTORY!"
  - [ ] Set Font Size: 72
  - [ ] Center alignment
- [ ] Right-click VictoryPanel ? UI ? Button
  - [ ] Rename to "NextLevelButton"
  - [ ] Text: "Next Level"
  - [ ] OnClick() ? UIManager ? OnNextLevelButton()
- [ ] Right-click VictoryPanel ? UI ? Button
  - [ ] Rename to "MainMenuButton"
  - [ ] Text: "Main Menu"
  - [ ] OnClick() ? UIManager ? OnMenuButton()

### ? Pause Menu Panel Creation
- [ ] Select **PauseMenuPanel** in Hierarchy
- [ ] Right-click PauseMenuPanel ? UI ? Text - TextMeshPro
  - [ ] Text: "PAUSED"
  - [ ] Font Size: 72
  - [ ] Center at top
- [ ] Create 4 buttons under PauseMenuPanel:
  1. **ResumeButton**: OnClick() ? PauseMenu ? OnResumeButton()
  2. **RestartButton**: OnClick() ? PauseMenu ? OnRestartButton()
  3. **MainMenuButton**: OnClick() ? PauseMenu ? OnMainMenuButton()
  4. **QuitButton**: OnClick() ? PauseMenu ? OnQuitButton()
- [ ] Select **PauseMenu** GameObject
- [ ] Assign in Inspector:
  - [ ] `Resume Button` ? Drag ResumeButton
  - [ ] `Restart Button` ? Drag RestartButton
  - [ ] `Main Menu Button` ? Drag MainMenuButton
  - [ ] `Quit Button` ? Drag QuitButton

### ? Testing
- [ ] Save scene (**Ctrl+S**)
- [ ] Run **Tools ? Validate Scene Setup**
- [ ] Check for warnings in Console
- [ ] Press **Play** to test
- [ ] Press **Escape** to test Pause Menu

---

## Scene2 Integration Checklist

Repeat all steps from Scene1:
- [ ] UIManager assignments (5 fields)
- [ ] PauseMenu assignments (5 fields)
- [ ] SpawnManager configuration
- [ ] AudioManager configuration
- [ ] Create Game Over Panel UI
- [ ] Create Victory Panel UI
- [ ] Create Pause Menu Panel UI
- [ ] Test and validate

---

## Hub Integration Checklist

Repeat all steps from Scene1:
- [ ] UIManager assignments (5 fields)
- [ ] PauseMenu assignments (5 fields)
- [ ] SpawnManager configuration (optional for Hub)
- [ ] AudioManager configuration
- [ ] Create Game Over Panel UI
- [ ] Create Victory Panel UI
- [ ] Create Pause Menu Panel UI
- [ ] Test and validate

---

## Mainmenu Scene Integration Checklist

### ? Main Menu Button Creation
- [ ] Open **Mainmenu** scene
- [ ] Select **Canvas** in Hierarchy
- [ ] Create buttons (GameObject ? UI ? Button - TextMeshPro):
  1. [ ] **StartButton** - Text: "Start Game"
  2. [ ] **ContinueButton** - Text: "Continue" (set inactive initially)
  3. [ ] **SettingsButton** - Text: "Settings"
  4. [ ] **CreditsButton** - Text: "Credits"
  5. [ ] **QuitButton** - Text: "Quit Game"
- [ ] Position buttons vertically in center of screen

### ? Settings Panel Creation
- [ ] Create Panel (GameObject ? UI ? Panel)
- [ ] Rename to "SettingsPanel"
- [ ] Set initially **inactive** (uncheck in Inspector)
- [ ] Under SettingsPanel, create:
  - [ ] Text - "Settings" title
  - [ ] Slider - Rename to "VolumeSlider"
  - [ ] Button - "CloseButton" text: "Close"
    - OnClick() ? MainMenu ? OnCloseSettings()

### ? Credits Panel Creation
- [ ] Create Panel (GameObject ? UI ? Panel)
- [ ] Rename to "CreditsPanel"
- [ ] Set initially **inactive**
- [ ] Under CreditsPanel, create:
  - [ ] Text - "Credits" title
  - [ ] Text - Your credits text
  - [ ] Button - "CloseButton" text: "Close"
    - OnClick() ? MainMenu ? OnCloseCredits()

### ? MainMenu Assignments
- [ ] Select **MainMenu** GameObject
- [ ] In Inspector, assign:
  - [ ] `Start Button` ? Drag StartButton
  - [ ] `Continue Button` ? Drag ContinueButton
  - [ ] `Settings Button` ? Drag SettingsButton
  - [ ] `Credits Button` ? Drag CreditsButton
  - [ ] `Quit Button` ? Drag QuitButton
  - [ ] `Settings Panel` ? Drag SettingsPanel
  - [ ] `Credits Panel` ? Drag CreditsPanel
  - [ ] `Volume Slider` ? Drag VolumeSlider

### ? Button OnClick Events
Wire up all buttons:
- [ ] StartButton OnClick() ? MainMenu ? OnStartGame()
- [ ] ContinueButton OnClick() ? MainMenu ? OnContinue()
- [ ] SettingsButton OnClick() ? MainMenu ? OnSettings()
- [ ] CreditsButton OnClick() ? MainMenu ? OnCredits()
- [ ] QuitButton OnClick() ? MainMenu ? OnQuit()

### ? Testing
- [ ] Save scene
- [ ] Run **Tools ? Validate Scene Setup**
- [ ] Press Play
- [ ] Test all buttons
- [ ] Test Settings panel opens/closes
- [ ] Test Credits panel opens/closes

---

## ?? Quick Reference: Where to Find Things

### Finding GameObjects:
- Look in **Hierarchy** window (left side of Unity)
- Use **Search** bar at top of Hierarchy
- Objects under Canvas are UI elements

### Finding Prefabs:
- Open **Project** window (bottom of Unity)
- Navigate to `Assets/Prefabs/`
- Enemy prefabs likely in `Assets/Prefabs/Enemies/`

### Finding Audio Clips:
- Project window
- Navigate to `Assets/Audio/` or similar
- Look for `.wav`, `.mp3`, or `.ogg` files

### Assigning References:
1. Click the **empty field** in Inspector
2. **Drag** the GameObject/asset from Hierarchy or Project
3. **Drop** it in the field
4. It should show the name (not "None")

---

## ??? Troubleshooting

### "I can't find a GameObject"
- Check if it's a child of Canvas or another parent
- Use Hierarchy search bar
- Verify you're in the correct scene

### "Field still shows 'None' after dragging"
- Make sure you're dragging the correct type
  - Slider field needs a Slider component
  - GameObject field needs any GameObject
  - Button field needs a Button component
- Try right-clicking the field and selecting the object from the list

### "Button OnClick() has no functions"
- Make sure you dragged the correct GameObject (UIManager, MainMenu, PauseMenu)
- The function dropdown only appears AFTER you assign the object

### "Validation still shows warnings"
- Double-check all fields are assigned (not "None")
- Make sure panels are inactive (unchecked) except Canvas
- Save the scene and run validation again

---

## ? Final Checklist

After completing all scenes:
- [ ] Scene1 fully configured and tested
- [ ] Scene2 fully configured and tested
- [ ] Hub fully configured and tested
- [ ] Mainmenu fully configured and tested
- [ ] All validations pass (Menu ? Tools ? Validate Scene Setup)
- [ ] Test game flow: Mainmenu ? Scene1 ? Scene2
- [ ] Test pause menu in all gameplay scenes
- [ ] Test game over and victory screens

---

## ?? YOU'RE DONE!

After completing this checklist, your game is fully integrated and ready to play!

**Next steps (optional polish):**
- Adjust UI positioning and sizes
- Add colors and styling to UI elements
- Test gameplay balance
- Add more enemies to SpawnManager waves
- Place more checkpoints in levels
- Add collectibles and powerups

**Congratulations! You've successfully integrated PhantomEdge!** ???
