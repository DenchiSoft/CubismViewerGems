using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Live2D.Cubism.Framework.Physics;
using Live2D.Cubism.Core;

namespace Live2D.Cubism.Viewer.Gems.Animating
{
	/// <summary>
	/// Lets you toggle physics on/off for a model.
	/// </summary>
	public sealed class PhysicsController : MonoBehaviour {
		// The Cubism Physics Controller for the loaded model. NULL if model has no physics file.
		private CubismPhysicsController physController;

		// Physics on/off toggle button.
		private Toggle physicsToggle;

		// Text field for physics toggle button.
		private Text PhysicsToggleLabel;

		// Use this for initialization
		void Start () {
			var viewer = GetComponent<CubismViewer>();

			// Fail silently in release.
			if (viewer == null)
			{
				Debug.LogWarning("Not attached to viewer!");
				return;
			}
				
			// Set listener for physics toggle button.
			physicsToggle = GameObject.Find("PhysicsToggle").GetComponent<Toggle>();
			PhysicsToggleLabel = GameObject.Find("PhysicsToggleLabel").GetComponent<Text>();
			physicsToggle.onValueChanged.AddListener(delegate {setPhysics(physicsToggle.isOn); });
			physicsToggle.isOn = true;

			// Set new model listener.
			viewer.OnNewModel += OnNewModel;
		}

		/// <summary>
		/// Physics button toggle listener.
		/// </summary>
		/// <param name="setOn"><c>true</c> if checked, <c>false</c> otherwise.</param>
		private void setPhysics(bool setOn) {
			// Disable/Enable physics controller for model if it has one. Otherwise just return.
			if (physController != null) 
				physController.enabled = setOn;
		}

		/// <summary>
		/// Called when a new Model is loaded.
		/// </summary>
		/// <param name="sender">The Sender/CubismViewer.</param>
		/// <param name="model">The new Model.</param>
		private void OnNewModel(CubismViewer sender, CubismModel model) {
			// Get physics controller for model. NULL if it doesn't have one.
			// This would be the case if no physics file has been found.
			physController = model.GetComponent<CubismPhysicsController>();

			// Disable/Enable physics toggle button depending on whether or not a physics controller is present.
			if (physController == null) {
				physicsToggle.isOn = false;
				physicsToggle.interactable = false;
				PhysicsToggleLabel.text = "No physics file found";
			} else {
				physicsToggle.isOn = true;
				physicsToggle.interactable = true;
				PhysicsToggleLabel.text = "Physics On/Off";
			}
		}
	}
}