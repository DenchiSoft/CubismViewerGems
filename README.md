# CubismViewerGems
Some _gems_ for the __Live2D Cubism Viewer__ at https://github.com/Live2D/CubismViewer to speed up and simplify Cubism model/animation testing in Unity.

### `AutoAnimator`
This _gem_ allows you to cycle through animations using hotkeys (default is left/right arrow keys) or select animations from a dropdown menu. This _gem_ works with and without the official `SimpleAnimator` _gem_.

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then, create a `UI Dropdown` and name it `animDropdown`. When you start the scene, select your model and animation like normal. The dropdown menu will automatically be populated with all animations that are in the same folder as the selected animation. You can now select any animation from the dropdown menu to play it. You can also use the hotkeys (default is left/right arrow keys) to cycle through all animations in the list.

### `AnimationController`
This _gem_ allows you to slow down and speed up animations (or even stop them completely). This makes it easier to spot subtle movement errors.

__Setup/Usage:__ Add the _gem_ to the `CubismViewer` object. Then, create a `UI Text` and name it `speedText` (optional, shows the current animation speed). While pressing the hotkey (default is left shift), scroll the mouse wheel to speed up or slow down the animation. Pressing the speed reset hotkey (default is space) will reset the speed to 100%.
