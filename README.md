# Deadly Balance: Tilt Defense

An interactive motion-controlled zombie defense game developed in Unity 6 using a custom-built balance board controller.

## Overview

Deadly Balance is a Unity-based action game that replaces traditional keyboard controls with body movement. Players control the character by leaning left or right on a custom-built balance board equipped with an MPU6050 motion sensor. A physical push button mounted beneath the board is used to fire projectiles.

The project combines hardware and software to create an embodied gaming experience where physical movement becomes the primary gameplay mechanic.

## Features

- Motion-controlled gameplay using a custom balance board
- Five-lane movement system
- Wave-based zombie defense
- Progressive difficulty scaling
- Real-time Arduino and Unity serial communication
- Health, score, and wave management
- Animated enemies
- Background music and sound effects
- Pause and Game Over menus

## Technologies

### Software

- Unity 6
- C#
- Arduino IDE

### Hardware

- Arduino Leonardo
- MPU6050 Accelerometer/Gyroscope
- Physical Push Button
- Custom Wooden Balance Board

## Gameplay

Players move between five predefined lanes by shifting their body weight.

Movement Flow:

```
Player Movement
      ↓
Balance Board Tilts
      ↓
MPU6050 Measures Angle
      ↓
Arduino Processes Data
      ↓
Serial Communication
      ↓
Unity Receives Input
      ↓
Character Moves
```

Shooting is performed using a physical push button mounted beneath the balance board.

## Project Structure

```
Assets/
├── Animations/
├── Audio/
├── Materials/
├── Prefabs/
├── Scenes/
├── Scripts/
├── Sprites/
└── UI/
```

## Controls

| Action | Controller |
|---------|------------|
| Move Left | Lean Left |
| Move Right | Lean Right |
| Shoot | Physical Push Button |

## Screenshots

Add gameplay screenshots here.

Example:

```
/Screenshots
    menu.png
    gameplay.png
    gameover.png
```

## Hardware Setup

- Arduino Leonardo
- MPU6050 Motion Sensor
- USB Serial Communication
- Physical Push Button
- Custom Balance Board

## Future Improvements

- Additional enemy types
- New maps
- Boss battles
- Multiplayer mode
- Achievement system
- Online leaderboard

Sabancı University

## License

This project was developed for educational purposes.
