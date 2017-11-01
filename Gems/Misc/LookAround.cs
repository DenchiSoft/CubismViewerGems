using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;

namespace Live2D.Cubism.Viewer.Gems.Misc
{
	/// <summary>
	/// Makes the character look towards the mouse pointer when you press ALT + the left mouse button.
	/// </summary>
	public sealed class LookAround : MonoBehaviour {
		// Percentage of the screen that gets cut off when tracking the mouse.
		// Cut off outer 10%, this means tracking only uses the inner 90%.
		private const float screenCutOff = 0.1f;

		// List of all parameter names that can be tracked and their usual min/max.
		private readonly string[] paramNames = new string[]
		{
			"ParamAngleX",		// -30 to 30
			"ParamAngleY",		// -30 to 30
			"ParamAngleZ",		// -30 to 30
			"ParamBodyAngleX",	// -10 to 10
			"ParamBodyAngleY",	// -10 to 10
			"ParamBodyAngleZ",	// -10 to 10
			"ParamEyeBallX",	// -1  to 1
			"ParamEyeBallY"		// -1  to 1
		};

		// List of all trackable parameters that were found in the model.
		private List<CubismParameter> CubismParams;
			
		/// <summary>
		/// Hotkey for making character look towards mouse pointer.
		/// </summary>
		[SerializeField]
		CubismViewerMouseButtonHotkey LookHotKey = new CubismViewerMouseButtonHotkey
		{
			Modifier = KeyCode.LeftAlt,
			Button = 0
		};

		/// <summary>
		/// Called by Unity. Initializes instance.
		/// </summary>
		void Start () {
			var viewer = GetComponent<CubismViewer>();

			// Fail silently in release.
			if (viewer == null)
			{
				Debug.LogWarning("Not attached to viewer!");
				return;
			}

			// Set new model listener.
			viewer.OnNewModel += OnNewModel;
		}

		/// <summary>
		/// Called when a new Model is loaded.
		/// </summary>
		/// <param name="sender">The Sender/CubismViewer.</param>
		/// <param name="model">The new Model.</param>
		private void OnNewModel(CubismViewer sender, CubismModel model) {
			// Find all trackable parameters in model.
			CubismParams = new List<CubismParameter>();

			foreach (CubismParameter p in model.Parameters) {
				foreach (string name in paramNames) {
					if (p.Id == name) {
						CubismParams.Add(p);
					}
				}
			}
		}

		/// <summary>
		/// Calculates the mouse position within the screen.
		/// </summary>
		/// <returns>The mouse position in percent of the screen (between 0 and 1).</returns>
		private Vector2 CalculateMousePos() {
			// Remove top, bottom, left and right 10% of the screen and calculate position in the resulting rectangle in percent.
			float minScreen = screenCutOff;
			float maxScreen = 1 - 2 * minScreen;
			float mousePosX = Mathf.Clamp(Input.mousePosition.x - Screen.width  * minScreen, 0, maxScreen * Screen.width ) / (Screen.width  * maxScreen);
			float mousePosY = Mathf.Clamp(Input.mousePosition.y - Screen.height * minScreen, 0, maxScreen * Screen.height) / (Screen.height * maxScreen);

			return new Vector2(mousePosX, mousePosY);
		}

		/// <summary>
		/// Called by Unity. Updates character.
		/// </summary>
		void LateUpdate () {
			
			if (CubismParams == null || CubismParams.Count == 0)
				return;
			
			// Handle Alt + Left Mouse Key
			if (LookHotKey.Evaluate())
			{
				Vector2 pos = CalculateMousePos();

				// Set parameters accordingly.
				// Parameters like "ParamAngleX" that end in "X" are considered to be controlled by the x position of the cursor, etc.
				foreach (CubismParameter p in CubismParams) {
					if (p.Id.EndsWith("X") || p.Id.EndsWith("ParamAngleZ") || p.Id.EndsWith("ParamBodyAngleZ")) {
						p.Value = Mathf.Lerp(p.MinimumValue, p.MaximumValue, pos.x);
					} else if (p.Id.EndsWith("Y")) { 
						p.Value = Mathf.Lerp(p.MinimumValue, p.MaximumValue, pos.y);
					}
				}
			}
		}
	}
}