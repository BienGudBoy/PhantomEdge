# "YOU DIED" Screen Implementation

## ✅ Implementation Complete

A Dark Souls-like "YOU DIED" screen has been implemented for when the player runs out of HP.

## 📋 What Was Created

### 1. **YouDiedScreen.cs** Script
- Location: `Assets/Scripts/UI/YouDiedScreen.cs`
- Features:
  - Dramatic fade-in animation with dark overlay
  - Large "YOU DIED" text in dark red color
  - Blinking "Press any button to continue" text
  - Restart and Main Menu buttons
  - Fully customizable colors and timing

### 2. **GameManager Integration**
- Updated `GameManager.cs` to call `YouDiedScreen.ShowDeathScreen()` when player dies
- Removed old generic game over handling

### 3. **UIManager Updates**
- Updated `UIManager.cs` to avoid conflicts with YouDiedScreen
- Death handling is now centralized through GameManager → YouDiedScreen

### 4. **Setup Script**
- Location: `Assets/Editor/YouDiedScreenSetup.cs`
- Menu item: `Tools > Setup YouDiedScreen`
- Automatically creates all UI elements and assigns references

## 🎮 How It Works

1. When player HP reaches 0, `HealthSystem` triggers `OnDeath` event
2. `GameManager.HandlePlayerDeath()` is called
3. `GameManager` finds `YouDiedScreen` component in the scene
4. `YouDiedScreen.ShowDeathScreen()` is called, which:
   - Fades in a dark overlay (2 seconds)
   - Shows "YOU DIED" text with fade-in animation (1.5 seconds)
   - After delay, shows blinking continue text and buttons

## 🛠️ Setup Instructions

### For Each Gameplay Scene (Scene1, Scene2, Hub):

1. **Open the scene** in Unity Editor
2. **Run the setup script:**
   - Go to `Tools > Setup YouDiedScreen`
   - This will automatically create all UI elements
3. **Verify the setup:**
   - Check that `YouDiedScreen` GameObject exists under Canvas
   - Verify all references are assigned in the Inspector
   - The panel should be inactive by default

### Manual Setup (if needed):

If the automated setup doesn't work, you can manually create:

1. **Create YouDiedScreen GameObject** under Canvas
   - Add `YouDiedScreen.cs` component

2. **Create DeathScreenPanel** (child of YouDiedScreen)
   - Add `RectTransform` and `Image` components
   - Set anchors to stretch full screen
   - Set inactive by default

3. **Create DarkOverlay** (child of DeathScreenPanel)
   - Add `RectTransform` and `Image` components
   - Set anchors to stretch full screen
   - Color: Black with alpha 0

4. **Create YouDiedText** (child of DeathScreenPanel)
   - Add `RectTransform` and `TextMeshProUGUI` components
   - Text: "YOU DIED"
   - Font size: 120
   - Color: Dark red (0.8, 0.1, 0.1)
   - Center aligned, bold
   - Position: Center of screen, slightly above center

5. **Create ContinueText** (child of DeathScreenPanel)
   - Add `RectTransform` and `TextMeshProUGUI` components
   - Text: "Press any button to continue"
   - Font size: 36
   - Color: White
   - Center aligned
   - Position: Below YouDiedText
   - Set inactive by default

6. **Create RestartButton** (child of DeathScreenPanel)
   - Add `RectTransform`, `Image`, and `Button` components
   - Add child Text GameObject with "Restart" text
   - Position: Below continue text, left side
   - Set inactive by default

7. **Create MenuButton** (child of DeathScreenPanel)
   - Add `RectTransform`, `Image`, and `Button` components
   - Add child Text GameObject with "Main Menu" text
   - Position: Below continue text, right side
   - Set inactive by default

8. **Assign References** in YouDiedScreen component:
   - Death Screen Panel → DeathScreenPanel GameObject
   - Dark Overlay → DarkOverlay Image
   - You Died Text → YouDiedText TextMeshProUGUI
   - Continue Text → ContinueText TextMeshProUGUI
   - Restart Button → RestartButton Button
   - Menu Button → MenuButton Button

## 🎨 Customization

You can customize the death screen in the Inspector:

### Animation Settings:
- **Fade In Duration**: How long the overlay takes to fade in (default: 2s)
- **Text Delay**: Delay before showing "YOU DIED" text (default: 0.5s)
- **Text Fade In Duration**: How long text takes to fade in (default: 1.5s)
- **Continue Text Delay**: Delay before showing continue text (default: 3s)
- **Continue Text Blink Speed**: Speed of blinking animation (default: 1s)

### Colors:
- **Overlay Color**: Color of the dark overlay (default: Black with 90% opacity)
- **Text Color**: Color of "YOU DIED" text (default: Dark red)

## 🧪 Testing

To test the death screen:

1. Play the game in a gameplay scene
2. Take damage until HP reaches 0
3. The "YOU DIED" screen should appear with animations
4. Click "Restart" to restart the level
5. Click "Main Menu" to return to main menu

## 📝 Notes

- The death screen uses `Time.unscaledDeltaTime` so it works even when the game is paused
- The screen automatically hides when restarting or returning to menu
- All UI elements are created under the Canvas, so they appear on top of everything
- The setup script can be run multiple times safely (it checks for existing objects)

## 🔄 Next Steps

1. Run `Tools > Setup YouDiedScreen` in Scene2 and Hub scenes
2. Test the death screen in each scene
3. Adjust colors and timing to match your game's aesthetic
4. Optionally add sound effects when the death screen appears

