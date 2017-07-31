 
using Live2D.Cubism.Framework.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;


namespace Live2D.Cubism.Viewer.Gems.Animating
{

	/// <summary>
	/// Shows available clips in dropdown and starts selected animation.
	/// </summary>
	public sealed class AnimationControls : MonoBehaviour
	{
		// Text UI element to view current speed.
		private Text speedText;

		/// <summary>
		/// Hotkey for changing animation speed.
		/// </summary>
		[SerializeField]
		CubismViewerMouseScrollHotkey AnimSpeedHotKey = new CubismViewerMouseScrollHotkey
		{
			Modifier = KeyCode.LeftShift
		};

		/// <summary>
		/// Hotkey for speed reset.
		/// </summary>
		[SerializeField]
		CubismViewerKeyboardHotkey AnimSpeedResetHotKey = new CubismViewerKeyboardHotkey
		{
			Key = KeyCode.Space
		};

		// Upper and lower speed limit.
		private const float upperSpeedLimit = 3;
		private const float lowerSpeedLimit = 0;

		/// <summary>
		/// Animation speed.
		/// </summary>
		[SerializeField, Range(lowerSpeedLimit, upperSpeedLimit)]
		float AnimSpeed;

		/// <summary>
		/// Scale to apply to anim speed on scroll.
		/// </summary>
		[SerializeField, Range(0.01f, 0.1f)]
		float AnimSpeedScale = 0.03f;

		/// <summary>
		/// Called by Unity. Set speed to 1 on start.
		/// </summary>
		private void Start()
		{
			// Get speed text view.
			speedText = GameObject.Find("speedText").GetComponent<Text>();

			// Reset animation speed.
			AnimSpeed = 1.0f;
		}

		/// <summary>
		/// Sets text of animation speed text field if it exists.
		/// </summary>
		private void SetAnimSpeedText() {
			// Return if text field doesn't exist.
			if (speedText == null)
				return;

			// Show speed in %.
			speedText.text = "Speed: " + (int) (AnimSpeed * 100) + "%";
		}

		/// <summary>
		/// Called by Unity. Updates controls.
		/// </summary>
		private void Update()
		{

			// Handle zoom.
			if (AnimSpeedHotKey.Evaluate())
			{
				AnimSpeed += (Input.mouseScrollDelta.y * AnimSpeedScale);
				AnimSpeed = Mathf.Clamp(AnimSpeed, lowerSpeedLimit, upperSpeedLimit);
			}

			if (AnimSpeedResetHotKey.EvaluateJust())
			{
				AnimSpeed = 1.0f;
			}

			Time.timeScale = AnimSpeed;
			SetAnimSpeedText();
		}

	}
}
