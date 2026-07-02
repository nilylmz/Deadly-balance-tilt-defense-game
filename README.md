# Tilt Defense: Dark Balance

Unity 2D dark fantasy defense prototype for a physical-computing control scheme.

## Open and Play

1. Open this folder as a Unity project.
2. Open `Assets/Scenes/TiltDefense.unity`.
3. Press Play.

The scene is intentionally empty. `GameBootstrapper` creates the camera, battlefield, menus, player, managers, UI, enemies, and effects at runtime.

## Temporary Keyboard Controls

- Left/Right arrows: simulate balance-board lane movement.
- Q: left flick, Left Shockwave, hits far-left and left lanes.
- E: right flick, Right Shockwave, hits right and far-right lanes.
- W: upward lift, Reload / Charge.
- S: downward slam, Energy Splash, damages and slows all lanes.
- R: circular gesture, Temporary Shield.
- Space: handheld emergency button, quick shot in current lane.

## Hardware Extension Point

`Assets/Scripts/InputManager.cs` defines `IGameInputProvider`. Replace `KeyboardInputProvider` with a future serial/Arduino provider that converts:

- balance-board MPU6050 roll into lane steps,
- wrist MPU6050 gestures into `AbilityCommand` values,
- handheld button presses into `AbilityCommand.EmergencyShot`.

The player, ability, wave, enemy, projectile, and UI systems already consume the abstract input provider instead of reading hardware directly.
