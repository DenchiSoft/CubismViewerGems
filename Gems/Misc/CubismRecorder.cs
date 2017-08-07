using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFmpegOut;
using UnityEngine.UI;
using Live2D.Cubism.Viewer;
using Live2D.Cubism.Viewer.Gems.Animating;


/// <summary>
/// Lets you record Cubism Animations as .mov files.
/// </summary>
public class CubismRecorder : MonoBehaviour {

	// Whether or not the camera is currently recording.
	private bool Recording;

	// The button to start recording.
	private Button RecordAnimButton;

	// The FPS text input field.
	InputField FPSInputField;

	/// <summary>
	/// Called by Unity.
	/// </summary>
	void Start () {
		// Set initial recording state to false.
		Recording = false;

		// Listeners for button click.
		RecordAnimButton = GameObject.Find("RecordAnimButton").GetComponent<Button>();
		RecordAnimButton.onClick.AddListener(delegate {StartRecord(); });

		// 30 FPS as default.
		FPSInputField = GameObject.Find("FPSInputField").GetComponent<InputField>();
		FPSInputField.text = "30";
	}

	/// <summary>
	/// Called when Record Animation button is pressed.
	/// </summary>
	private void StartRecord() {
		// Return if already recording.
		if (Recording) {
			return;
		}
			
		var viewer = GetComponent<CubismViewer>();
		if (viewer == null || viewer.Model == null) {
			return;
		}

		Animation anim = viewer.Model.GetComponent<Animation>();
		if (anim == null || anim.clip == null) {
			return;
		}
			
		Recording = true;

		// Get FPS and clamp between 1 and 120.
		InputField FPSInputField = GameObject.Find("FPSInputField").GetComponent<InputField>();
		int fps = int.Parse(FPSInputField.text);
		fps = Mathf.Clamp(fps, 1, 120);
		FPSInputField.text = fps.ToString();

		// Get double res toggle state.
		Toggle doubleResToggle = GameObject.Find("DoubleResToggle").GetComponent<Toggle>();
		int multiplier = doubleResToggle.isOn ? 2 : 1;

		// Destroy old CubismCapture if it still exists and add new.
		GameObject cam = Camera.main.gameObject;
		CubismCameraCapture cubismCapture = cam.GetComponent<CubismCameraCapture>();

		if (cubismCapture != null)
			GameObject.Destroy(cubismCapture);

		// Set parameters for capture.
		cubismCapture = cam.AddComponent<CubismCameraCapture>();
		cubismCapture.enabled = false;

		cubismCapture._anim = anim;
		cubismCapture._recordLength = anim.clip.length;
		cubismCapture._frameRate = fps;
		cubismCapture._width = Screen.width * multiplier;
		cubismCapture._height = Screen.height * multiplier;
		cubismCapture._material = new Material(Shader.Find("FFmpegOutCubism/CubismCameraCapture"));

		// Start capture.
		anim.Stop();
		cubismCapture.enabled = true;
		anim.Play(anim.clip.name);

		// Start progress indicator.
		StartCoroutine(EnableButton(anim.clip.length));
	}

	/// <summary>
	/// Indicates capture progress and enables capture button when done.
	/// </summary>
	/// <param name="length">Animation clip length.</param>
	private IEnumerator EnableButton(float length) {
		Text progressText = RecordAnimButton.gameObject.GetComponentInChildren<Text>();

		// Show capture progress.
		for (int i = 0; i <= 100; i++) {
			yield return new WaitForSeconds(length / 100f);
			progressText.text = "Progress: " + i + "%";
		}

		// Indicate finished state and enable button.
		progressText.text = "Saving...";
		yield return new WaitForSeconds(1.0f);
		progressText.text = "Record Animation";
		Recording = false;
	}
}
