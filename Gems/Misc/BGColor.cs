using UnityEngine;
using uCPf;

namespace Live2D.Cubism.Viewer.Gems.Misc
{
	/// <summary>
	/// Show color picker to set BG color.
	/// </summary>
	public sealed class BGColor : MonoBehaviour {
		// Color picker game object.
		private GameObject colorPickerGO;

		// Color picker script.
		private ColorPicker colorPicker;

		// BG Color is set for this camera.
		private Camera mainCamera;

		// Ready parameter. Set to true when dialog is first opened.
		private bool ready = false;

		/// <summary>
		/// Called by Unity.
		/// </summary>
		void Start () {
			// Disable color picker.
			colorPickerGO = GameObject.Find("PresetColorPicker");
			colorPickerGO.SetActive(false);
			colorPicker = colorPickerGO.GetComponent<ColorPicker>();

			mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		}

		/// <summary>
		/// Gets called when button in UI for color picker is clicked.
		/// Activates/Deactivates color picker.
		/// </summary>
		public void BGColorButtonClicked() {
			if (colorPickerGO.activeSelf) {
				colorPickerGO.SetActive(false);
			} else {
				colorPickerGO.SetActive(true);
			}

			ready = true;
		}

		/// <summary>
		/// Called by color picker when color is changed.
		/// </summary>
		public void ColorChanged() {
			// Only do this once color picker has been shown once.
			// Necessary because the color picker calls this function once at initialization, resulting in
			// a nullreference exception.
			if (ready) {
				mainCamera.backgroundColor = colorPicker.color;
			}
		}
	}
}