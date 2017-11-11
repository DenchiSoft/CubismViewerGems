


# CubismViewerGems
Some _gems_ for the __Live2D Cubism Viewer__ at https://github.com/Live2D/CubismViewer to speed up and simplify Cubism model/animation testing in Unity.

__Releases:__ Standalone Windows builds can be found in the [releases section](https://github.com/DenchiSoft/CubismViewerGems/releases).


## Contents

- [Controls](#controls)
- [Gems](#gems)
  - [AutoAnimator](#autoanimator)
  - [AnimationController](#animationcontroller)
  - [ParamSliders](#paramsliders)
  - [PartSliders](#partsliders)
  - [PhysicsController](#physicscontroller)
  - [LookAround](#lookaround)
  - [BGColor](#bgcolor)
  - [CubismRecorder](#cubismrecorder)
- [Screenshots](#screenshots)

## Controls
All controls/shortcuts are listed in the table below.

| Command | Description |
| --- | --- |
| <kbd>CTRL</kbd> + `Drag` | Drag model position |
| <kbd>ALT</kbd> + `Mouse Wheel` | Resize model |
| <kbd>SHIFT</kbd> + `Mouse Wheel` | Change animation speed |
| <kbd>SPACE</kbd> | Reset animation speed |
| <kbd>ALT</kbd> + `Drag` | Make model look at mouse pointer |

## Gems

### `AutoAnimator`
This _gem_ allows you to cycle through animations using hotkeys (default is left/right arrow keys) or select animations from a dropdown menu. This _gem_ works with and without the official `SimpleAnimator` _gem_.

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then, create a `UI Dropdown` and name it `animDropdown`. When you start the scene, select your model and animation like normal. The dropdown menu will automatically be populated with all animations that are in the same folder as the selected animation. You can now select any animation from the dropdown menu to play it. You can also use the hotkeys (default is left/right arrow keys) to cycle through all animations in the list.

### `AnimationController`
This _gem_ allows you to slow down and speed up animations (or even stop them completely). This makes it easier to spot subtle movement errors.

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then, create a `UI Text` and name it `speedText` (optional, shows the current animation speed). While pressing the hotkey (default is left shift), scroll the mouse wheel to speed up or slow down the animation. Pressing the speed reset hotkey (default is space) will reset the speed to 100%.

### `ParamSliders`
This _gem_ allows you to view and modify parameter values of your model. When playing animations, sliders will reflect the parameter values, but can be overridden for individual parameters at the same time.

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then, add `paramScroll.prefab` to your UI. When you start the scene, select your model and (optionally) animation like normal. The sliders will show the current parameter value while the animation is playing. Drag any slider to fix the associated parameter value. The red tint will indicate that this parameter is now being overridden, meaning it is no longer controlled by the animation. Other parameters will still be controlled by the animation. Uncheck the toggle button to stop overriding a parameter and give control back to the animation. The `Reset Override` button resets the override mode for all parameters. The `Reset Position` button resets the model back to the default pose.


### `PartSliders`
This _gem_ allows you to view and modify part opacities of your model. When playing animations, sliders will reflect the part opacity values, but can be overridden for individual parts at the same time.

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then, add `partScroll.prefab` to your UI. When you start the scene, select your model and (optionally) animation like normal. The sliders will show the current part opacity value while the animation is playing. Drag any slider to fix the associated part opacity value. The red tint will indicate that this part opacity is now being overridden, meaning it is no longer controlled by the animation. Other part opacities will still be controlled by the animation. Uncheck the toggle button to stop overriding a part opacity value and give control back to the animation. The `Reset Override` button resets the override mode for all parts. The `Reset Visibility` button resets all part visibility values back to default.


### `PhysicsController`
This _gem_ allows you to disable and enable physics for a model, given a physics file is in the same directory as the model file.

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then, add `PhysicsToggle.prefab` to your UI. When you start the scene, select your model. If a physics file was found in the same folder as the model, you can now use the toggle button to turn on/off physics for your model. Physics work with and without animations, much like in the Live2D Cubism Physics Editor.

### `LookAround`
This _gem_ allows you to make your character "look at" your cursor. 

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then click and drag while pressing the ALT-key on your keyboard. This will make your model/character look towards the mouse pointer. This works together nicely with physics and animations: If you make the character look towards your mouse pointer, this will override any value set by the animation.

_Note:_ The affected parameter IDs are `ParamAngleX`, `ParamAngleY`, `ParamAngleZ`, `ParamEyeBallX`, `ParamEyeBallY`, `ParamBodyAngleX`, `ParamBodyAngleY` and `ParamBodyAngleZ`.

### `BGColor`
This _gem_ allows you to change the canvas background color. 

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Next, download _"uGUI Color Picker - Free Versuin uCPf"_ by Magcat from the Unity Asset store and add its `PresetColorPicker.prefab` to your UI. Make sure the GameObject name is PresetColorPicker. This GameObject has a ColorPicker-script attached. In this script, set the `onChange` to the `ColorChanged` function of the `BGColor` script. Lastly, add a button to your UI and set its `onChange` to the `BGColorButtonClicked` function of the `BGColor` script.

Clicking the button will now open and close the color picker dialog. Changing the color in the dialog will change the camera's background color. 


### `CubismRecorder`
This _gem_ allows you to capture Cubism model animations and record them to `.mov` files in high quality at any resolution/FPS. It is based on [FFmpegOut](https://github.com/keijiro/FFmpegOut) by Keijiro Takahashi.

__Setup/Usage:__ Download and import `FFmpegOut.unitypackage` from https://github.com/keijiro/FFmpegOut/releases (tested with Version 0.1.1). Then, add this _gem_ (`CubismRecorder.cs`) to the `CubismViewer` object and add `RecordPanel.prefab` to your UI. This _gem_ requires the `AutoAnimator` _gem_, so make sure you also have that one added to your scene. When you start the scene, select your model and animation like normal. The `Record Animation` button will start recording your currently playing animation from start to finish at the set framerate (up to 120 FPS) and the current window resolution. The UI is not recorded. Checking the `Double Resolution` box will record the current animation at double the window resolution. The output videos are saved as high quality `.mov` files, so be careful as they tend to become quite big when capturing at high resolution/framerate. When playing the scene in Unity, the videos are saved in the project root folder. When playing a standalone build, videos are saved next to the executable. 

_Note:_ This _gem_ can be used together with the `AnimationController` _gem_, although it is not recommended.


## Screenshots
![Screenshot](https://raw.githubusercontent.com/DenchiSoft/CubismViewerGems/master/images/viewer_screenshot_v1_0.png "Screenshot")
