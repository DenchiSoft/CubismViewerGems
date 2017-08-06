 
using Live2D.Cubism.Framework.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Live2D.Cubism.Core;


namespace Live2D.Cubism.Viewer.Gems.Animating
{

	/// <summary>
	/// Shows available clips in dropdown and starts selected animation.
	/// </summary>
	public sealed class AutoAnimator : MonoBehaviour
	{
		// Dropdown UI element.
		private Dropdown animDropdown;

		// Cubism viewer.
		private CubismViewer viewer;

		// Full filenames of json animation files.
		private string[] files;

		/// <summary>
		/// Hotkey for next animation.
		/// </summary>
		[SerializeField]
		CubismViewerKeyboardHotkey NextAnimHotKey = new CubismViewerKeyboardHotkey
		{
			Key = KeyCode.RightArrow
		};
				
		/// <summary>
		/// Hotkey for previous animation.
		/// </summary>
		[SerializeField]
		CubismViewerKeyboardHotkey PrevAnimHotKey = new CubismViewerKeyboardHotkey
		{
			Key = KeyCode.LeftArrow
		};
				
		/// <summary>
		/// Called by Unity. Registers handler.
		/// </summary>
		private void Start()
		{
			var viewer = GetComponent<CubismViewer>();

			// Get dropdown UI element, clear content and disable.
			animDropdown = GameObject.Find("animDropdown").GetComponent<Dropdown>();
			animDropdown.ClearOptions();
			animDropdown.captionText.text = "Load one motion first";
			animDropdown.enabled = false;

			// Register dropdown selection listener.
			animDropdown.onValueChanged.AddListener(delegate{DropdownSelected();});


			// Fail silently in release.
			if (viewer == null)
			{
				Debug.LogWarning("Not attached to viewer!");


				return;
			}
				

			// Register event handlers.
			viewer.OnFileDrop += HandleFileDrop;
			viewer.OnNewModel += OnNewModel;
		}

		/// <summary>
		/// Called by Unity. Updates controls.
		/// </summary>
		private void Update()
		{
			// Return if no animations have been loaded.
			if (!animDropdown.enabled)
				return;

			// Play next animation loop on hotkey.
			if (NextAnimHotKey.EvaluateJust())
			{
				animDropdown.value = animDropdown.value == files.Length ? 0 : animDropdown.value + 1;
			}

			// Play previous animation loop on hotkey.
			if (PrevAnimHotKey.EvaluateJust())
			{
				animDropdown.value = animDropdown.value == 0 ? files.Length : animDropdown.value - 1;
			}

		}

		/// <summary>
		/// Called when animation is selected in dropdown.
		/// </summary>
		private void DropdownSelected() {
			var model = viewer.Model;

			// Make sure animation component is attached to model.
			var animator = model.GetComponent<Animation>();

			if (animator == null)
			{
				animator = model.gameObject.AddComponent<Animation>();
			}

			// Check if "no animation" entry is selected.
			if (animDropdown.value == 0) {
				animator.Stop();
				animator.clip = null;
				return;
			}

			string absolutePath = files[animDropdown.value - 1];

			// Deserialize animation.
			var model3Json = CubismMotion3Json.LoadFrom(CubismViewerIo.LoadAsset<string>(absolutePath));
			var clipName = CubismViewerIo.GetFileName(absolutePath);
			var clip = model3Json.ToAnimationClip();
			clip.wrapMode = WrapMode.Loop;
			clip.legacy = true;

			// Set clip info in animator (needed for recording with CubismRecorder).
			clip.name = clipName;
			animator.clip = clip;

			// Play animation.
			animator.AddClip(clip, clipName);
			animator.Play(clipName);
		}


		/// <summary>
		/// Handles file drops.
		/// </summary>
		/// <param name="sender">Event source.</param>
		/// <param name="absolutePath">Absolute path of dropped file.</param>
		private void HandleFileDrop(CubismViewer sender, string absolutePath)
		{
			// Skip non-motion files.
			if (!absolutePath.EndsWith("motion3.json"))
			{
				return;
			}

			// Save reference to viewer.
			viewer = sender;

			// Get all full file paths of motion files.
			files = System.IO.Directory.GetFiles(Path.GetDirectoryName(absolutePath), "*.motion3.json");

			// Get filenames without path for display.
			List<string> filenames = files.Select(a => Path.GetFileName(a).Replace(".motion3.json", String.Empty)).ToList();

			// Add option for no animation.
			filenames.Insert(0, "--- None ---");

			// Get index of currently selected file.
			int selected = Array.IndexOf(files, absolutePath) + 1;

			// Enable dropdown and show list of filenames.
			animDropdown.ClearOptions();
			animDropdown.AddOptions(filenames);
			animDropdown.enabled = true;
			animDropdown.value = selected;
		}

		/// <summary>
		/// Called when a new Model is loaded.
		/// </summary>
		/// <param name="sender">The Sender/CubismViewer.</param>
		/// <param name="model">The new Model.</param>
		private void OnNewModel(CubismViewer sender, CubismModel model) {
			// Clear animation list when new model is loaded.
			animDropdown.ClearOptions();
			animDropdown.captionText.text = "Load one motion first";
			animDropdown.enabled = false;
		}
	}
}
