# Jetpack Button Implementation

## Overview
This implementation adds jetpack activation functionality to the MyButton on the UIGameplayView prefab. When the button is clicked, it toggles the jetpack on/off for the local player.

## Files Modified

### 1. Scripts/MyButton.cs
- Enhanced the existing SimpleButtonTest script
- Added support for a jetpack activation handler callback
- Added visual feedback with color changes (green when active, white when inactive)
- Added UpdateJetpackStatus method to update button appearance

### 2. Scripts/UI/GameplayViews/UIGameplayView.cs
- Added `_myButton` serialized field to reference the MyButton component
- Added `OnMyButtonClick()` method to handle jetpack activation/deactivation
- Connected the MyButton to the UIGameplayView in OnInitialize/OnDeinitialize
- Added visual status updates in OnTick method

### 3. UI/Prefabs/GameplayViews/UIGameplayView.prefab
- Added `_myButton: {fileID: 7883197297227283414}` field to reference the MyButton component

## How It Works

1. **Initialization**: When UIGameplayView initializes, it sets up the MyButton click handler
2. **Button Click**: When MyButton is clicked, it calls the OnMyButtonClick method
3. **Jetpack Toggle**: The method checks if the local agent has a jetpack and toggles its active state
4. **Visual Feedback**: The button color changes to green when jetpack is active, white when inactive
5. **Cleanup**: When UIGameplayView deinitializes, it removes the click handler

## Features

- **Toggle Functionality**: Click once to activate, click again to deactivate
- **Visual Feedback**: Button color changes based on jetpack status
- **Safety Checks**: Only works when local agent and jetpack are available
- **Proper Cleanup**: Event handlers are properly removed on deinitialization

## Usage

1. The MyButton is positioned in the top-left corner of the gameplay UI
2. Click the button to activate/deactivate the jetpack
3. The button will turn green when jetpack is active
4. The button will turn white when jetpack is inactive

## Technical Details

- Uses Unity's Button component for input handling
- Integrates with the existing Agent and Jetpack systems
- Follows the same pattern as other UI elements in the project
- Maintains proper separation of concerns between UI and gameplay logic
