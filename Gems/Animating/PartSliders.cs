using UnityEngine;
using Live2D.Cubism.Core;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Live2D.Cubism.Viewer.Gems.Animating
{
	/// <summary>
	/// Shows available parts and lets you control their opacity.
	/// </summary>
	public sealed class PartSliders : MonoBehaviour
	{
		// Cubism viewer.
		private CubismViewer viewer;

		// List containing all opacity/UI related information for each CubismPart.
		private List<CubismPartInfo> CubismPartsInfo;

		// If set, all parts are reset on the next frame.
		private bool ResetAllParts;

		/// <summary>
		/// Called by Unity. 
		/// </summary>
		private void Start()
		{
			var viewer = GetComponent<CubismViewer>();

			// Fail silently in release.
			if (viewer == null)
			{
				Debug.LogWarning("Not attached to viewer!");
				return;
			}

			// Listeners for button clicks and new model event.
			Button resetOverrideButton = GameObject.Find("ResetPartOverrideButton").GetComponent<Button>();
			resetOverrideButton.onClick.AddListener(delegate {ResetOverrideClicked(); });


			Button resetPositionButtonText = GameObject.Find("ResetPartButton").GetComponent<Button>();
			resetPositionButtonText.onClick.AddListener(delegate {ResetPartClicked(); });

			viewer.OnNewModel += OnNewModel;
		}

		/// <summary>
		/// Called when a new Model is loaded.
		/// </summary>
		/// <param name="sender">The Sender/CubismViewer.</param>
		/// <param name="model">The new Model.</param>
		private void OnNewModel(CubismViewer sender, CubismModel model) {
			// Check if old model is currently loaded.
			if (CubismPartsInfo != null) {
				// Destroy all old UI elements if they exist.
				foreach (CubismPartInfo part in CubismPartsInfo) {
					GameObject.Destroy(part.Slider.gameObject.transform.parent.gameObject);
				}
			}

			// Get template for part entries (find over parent because it's not enabled)
			GameObject partEntryTemplate = GameObject.Find("partScroll").transform.Find("PartEntryTemplate").gameObject;

			// Get scroll view content box. Part sliders are instantiated inside of this.
			GameObject partScrollContent = GameObject.Find("partScrollContent");

			CubismPartsInfo = new List<CubismPartInfo>();

			// Populate part UI scroll view.
			foreach (CubismPart p in model.Parts) {
				// Instantiate from template.
				GameObject newPart = (GameObject)Instantiate(partEntryTemplate);
				newPart.transform.SetParent(partScrollContent.transform);
				newPart.SetActive(true);
				newPart.name = p.Id;

				// Set slider values.
				Slider s = newPart.GetComponentInChildren<Slider>();
				s.maxValue = 1;
				s.minValue = 0;
				s.value = p.Opacity;

				// Set text fields.
				Text t = newPart.GetComponentsInChildren<Text>()[3];
				newPart.GetComponentsInChildren<Text>()[0].text =p.Id;
				t.text = p.Opacity.ToString();

				Toggle to = newPart.GetComponentInChildren<Toggle>();
				Image img = newPart.GetComponent<Image>();

				// Create list of all CubismParts and their respective UI elements/override state.
				CubismPartInfo part = new CubismPartInfo(p, to, t, img, s, false, false, p.Opacity, p.Opacity);
				CubismPartsInfo.Add(part);

				// Listeners for slider and toggle button.
				to.onValueChanged.AddListener(delegate(bool newValue) {PartActiveStatusChanged(newValue, part); });
				s.onValueChanged.AddListener(delegate(float newValue) {PartOpacityChanged(newValue, part); });
			}

			// HACK Manually set scroll content height to height of children. Correct way to do this?
			int partEntryHeight = (int) ((RectTransform) partEntryTemplate.transform).rect.height * model.Parts.Length;
			((RectTransform) partScrollContent.transform).sizeDelta = new Vector2(0, partEntryHeight);

		}


		/// <summary>
		/// Called when the reset override button is clicked.
		/// Resets override state of all parts.
		/// </summary>
		private void ResetOverrideClicked() {
			if (CubismPartsInfo == null || CubismPartsInfo.Count == 0)
				return;

			// Call override toggle to false callback for all parts.
			foreach (CubismPartInfo part in CubismPartsInfo) {
				part.Toggle.isOn = false;
			}
		}

		/// <summary>
		/// Called when the reset opacity button is clicked.
		/// Resets all part opacities back to default state.
		/// </summary>
		private void ResetPartClicked() {
			if (CubismPartsInfo == null || CubismPartsInfo.Count == 0)
				return;
			
			// Reset all parts to default (value when model was loaded).
			ResetAllParts = true;
		}

		/// <summary>
		/// Called when override button is toggled.
		/// Also called when user presses the reset button or manually moves a slider.
		/// </summary>
		/// <param name="newValue">Checked or unchecked.</param>
		/// <param name="part">The associated CubismPartInfo.</param>
		private void PartActiveStatusChanged(bool newValue, CubismPartInfo part) {
			part.Active = newValue;
			part.BackgroundTint.enabled = newValue;
			part.OverrideVal = part.Part.Opacity;
		}

		/// <summary>
		/// Called when the user or the animation changes the slider value.
		/// </summary>
		/// <param name="newValue">New value of the slider.</param>
		/// <param name="part">The associated CubismPartInfo.</param>
		private void PartOpacityChanged(float newValue, CubismPartInfo part) {

			// Check if the call came from LateUpdate().
			if (!part.ValueSetByAnimation) {
				// If not, the part is now considered in override mode.
				part.Active = true;
				part.OverrideVal = newValue;
				part.Toggle.isOn = true;
				part.BackgroundTint.enabled = true;
			} else {
				part.ValueSetByAnimation = false;
			}
		}

		/// <summary>
		/// Called by Unity. 
		/// </summary>
		void LateUpdate()
		{
			if (CubismPartsInfo == null || CubismPartsInfo.Count == 0)
				return;

			// Iterate over all parts.
			// If a parts is in override mode, set override value.
			// If not, set value and indicate it has been set by the animation.
			foreach (CubismPartInfo parts in CubismPartsInfo) {
				if (ResetAllParts) {
					parts.ValueSetByAnimation = true;
					parts.Part.Opacity = parts.DefaultOpacity;
					parts.OverrideVal = parts.DefaultOpacity;
					parts.Slider.value = parts.DefaultOpacity;
					parts.ValueText.text = System.Math.Round(parts.Part.Opacity, 2).ToString();
				} else if (parts.Active) {
					parts.Part.Opacity = parts.OverrideVal;
					parts.ValueText.text = System.Math.Round(parts.OverrideVal, 2).ToString();
				} else {
					parts.ValueSetByAnimation = true;
					parts.Slider.value = parts.Part.Opacity;
					parts.ValueText.text = System.Math.Round(parts.Part.Opacity, 2).ToString();
				}
			}

			// Only reset once.
			ResetAllParts = false;
		}
	}
}
