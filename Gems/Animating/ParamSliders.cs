using UnityEngine;
using Live2D.Cubism.Core;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Live2D.Cubism.Viewer.Gems.Animating
{
	/// <summary>
	/// Shows available parameters and lets you control them.
	/// </summary>
	public sealed class ParamSliders : MonoBehaviour
	{
		// Cubism viewer.
		private CubismViewer viewer;

		// List containing all value/UI related information for each CubismParameter.
		private List<CubismParameterInfo> CubismParamsInfo;

		// If set, all parameters are reset on the next frame.
		private bool ResetAllParams;

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
			Button resetOverrideButton = GameObject.Find("ResetOverrideButton").GetComponent<Button>();
			resetOverrideButton.onClick.AddListener(delegate {ResetOverrideClicked(); });

			Button resetPositionButtonText = GameObject.Find("ResetPositionButton").GetComponent<Button>();
			resetPositionButtonText.onClick.AddListener(delegate {ResetPositionClicked(); });

			viewer.OnNewModel += OnNewModel;
		}

		/// <summary>
		/// Called when a new Model is loaded.
		/// </summary>
		/// <param name="sender">The Sender/CubismViewer.</param>
		/// <param name="model">The new Model.</param>
		private void OnNewModel(CubismViewer sender, CubismModel model) {
			// Check if old model is currently loaded.
			if (CubismParamsInfo != null) {
				// Destroy all old UI elements if they exist.
				foreach (CubismParameterInfo param in CubismParamsInfo) {
					GameObject.Destroy(param.Slider.gameObject.transform.parent.gameObject);
				}
			}

			// Get template for parameter entries (find over parent because it's not enabled)
			GameObject paramEntryTemplate = GameObject.Find("paramScroll").transform.Find("paramEntryTemplate").gameObject;

			// Get scroll view content box. Parameter sliders are instantiated inside of this.
			GameObject paramScrollContent = GameObject.Find("paramScrollContent");

			CubismParamsInfo = new List<CubismParameterInfo>();

			// Populate parameter UI scroll view.
			foreach (CubismParameter p in model.Parameters) {
				// Instantiate from template.
				GameObject newParam = (GameObject)Instantiate(paramEntryTemplate);
				newParam.transform.SetParent(paramScrollContent.transform);
				newParam.SetActive(true);
				newParam.name = p.Id;

				// Set slider values.
				Slider s = newParam.GetComponentInChildren<Slider>();
				s.maxValue = p.MaximumValue;
				s.minValue = p.MinimumValue;
				s.value = p.Value;

				// Set text fields.
				Text t = newParam.GetComponentsInChildren<Text>()[3];
				newParam.GetComponentsInChildren<Text>()[0].text = p.Id;
				newParam.GetComponentsInChildren<Text>()[1].text = p.MinimumValue.ToString();
				newParam.GetComponentsInChildren<Text>()[2].text = p.MaximumValue.ToString();
				t.text = p.Value.ToString();

				Toggle to = newParam.GetComponentInChildren<Toggle>();
				Image img = newParam.GetComponent<Image>();

				// Create list of all CubismParameters and their respective UI elements/override state.
				CubismParameterInfo param = new CubismParameterInfo(p, to, t, img, s, false, false, p.Value);
				CubismParamsInfo.Add(param);

				// Listeners for slider and toggle button.
				to.onValueChanged.AddListener(delegate(bool newValue) {ParamActiveStatusChanged(newValue, param); });
				s.onValueChanged.AddListener(delegate(float newValue) {ParamValueChanged(newValue, param); });
			}

			// HACK Manually set scroll content height to height of children. Correct way to do this?
			int paramEntryHeight = (int) ((RectTransform) paramEntryTemplate.transform).rect.height * model.Parameters.Length;
			((RectTransform) paramScrollContent.transform).sizeDelta = new Vector2(0, paramEntryHeight);

		}


		/// <summary>
		/// Called when the reset override button is clicked.
		/// Resets override state of all parameters.
		/// </summary>
		private void ResetOverrideClicked() {
			if (CubismParamsInfo == null || CubismParamsInfo.Count == 0)
				return;

			// Call override toggle to false callback for all parameters.
			foreach (CubismParameterInfo param in CubismParamsInfo) {
				param.Toggle.isOn = false;
			}
		}

		/// <summary>
		/// Called when the reset position button is clicked.
		/// Resets character (all parameters) back to default state.
		/// </summary>
		private void ResetPositionClicked() {
			if (CubismParamsInfo == null || CubismParamsInfo.Count == 0)
				return;
			
			// Reset all parameters to default pose.
			ResetAllParams = true;
		}

		/// <summary>
		/// Called when override button is toggled.
		/// Also called when user presses the reset button or manually moves a slider.
		/// </summary>
		/// <param name="newValue">Checked or unchecked.</param>
		/// <param name="param">The associated CubismParameterInfo.</param>
		private void ParamActiveStatusChanged(bool newValue, CubismParameterInfo param) {
			param.Active = newValue;
			param.BackgroundTint.enabled = newValue;
			param.OverrideVal = param.Parameter.Value;
		}

		/// <summary>
		/// Called when the user or the animation changes the slider value.
		/// </summary>
		/// <param name="newValue">New value of the slider.</param>
		/// <param name="param">The associated CubismParameterInfo.</param>
		private void ParamValueChanged(float newValue, CubismParameterInfo param) {

			// Check if the call came from LateUpdate().
			if (!param.ValueSetByAnimation) {
				// If not, the parameter is now considered in override mode.
				param.Active = true;
				param.OverrideVal = newValue;
				param.Toggle.isOn = true;
				param.BackgroundTint.enabled = true;
			} else {
				param.ValueSetByAnimation = false;
			}
		}

		/// <summary>
		/// Called by Unity. 
		/// </summary>
		void LateUpdate()
		{
			if (CubismParamsInfo == null || CubismParamsInfo.Count == 0)
				return;

			// Iterate over all parameters.
			// If a paremeter is in override mode, set override value.
			// If not, set value and indicate it has been set by the animation.
			foreach (CubismParameterInfo param in CubismParamsInfo) {
				if (ResetAllParams) {
					param.ValueSetByAnimation = true;
					param.Parameter.Value = param.Parameter.DefaultValue;
					param.OverrideVal = param.Parameter.DefaultValue;
					param.Slider.value = param.Parameter.DefaultValue;
					param.ValueText.text = System.Math.Round(param.Parameter.Value, 2).ToString();
				} else if (param.Active) {
					param.Parameter.Value = param.OverrideVal;
					param.ValueText.text = System.Math.Round(param.OverrideVal, 2).ToString();
				} else {
					param.ValueSetByAnimation = true;
					param.Slider.value = param.Parameter.Value;
					param.ValueText.text = System.Math.Round(param.Parameter.Value, 2).ToString();
				}
			}

			// Only reset once.
			ResetAllParams = false;
		}
	}
}
