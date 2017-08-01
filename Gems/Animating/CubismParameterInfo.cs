using UnityEngine;
using System.Collections;
using Live2D.Cubism.Core;
using UnityEngine.UI;

/// <summary>
/// This class contains all information about values and UI elements related
/// to one CubismParameter.
/// </summary>
public class CubismParameterInfo
{
	/// <summary>
	/// The cubism parameter.
	/// </summary>
	public CubismParameter Parameter { get; set; }

	/// <summary>
	/// Whether or not the parameter is currently being overridden by the user.
	/// </summary>
	public bool Active { get; set; }

	/// <summary>
	/// The parameter value slider UI element.
	/// </summary>
	public Slider Slider { get; set; }

	/// <summary>
	/// The override toggle UI element.
	/// </summary>
	public Toggle Toggle { get; set; }

	/// <summary>
	/// The text field UI element that shows the current parameter value.
	/// </summary>
	public Text ValueText { get; set; }

	/// <summary>
	/// The background image of the parameter list entry. Indicates override state.
	/// </summary>
	public Image BackgroundTint { get; set; }

	/// <summary>
	/// Whether or not the last value was set by the animation.
	/// Needed because the manual call to change the slider value also calls the slider value changed callback.
	/// </summary>
	public bool ValueSetByAnimation { get; set; }

	/// <summary>
	/// The override value set by the user
	/// </summary>
	public float OverrideVal { get; set; }

	/// <summary>
	/// CubismParameterInfo Constructor.
	/// </summary>
	public CubismParameterInfo(CubismParameter parameter, Toggle toggle, Text valueText, Image backgroundTint,
			Slider slider, bool active, bool valueSetByAnimation, float overrideVal) {
		this.Parameter = parameter;
		this.Toggle = toggle;
		this.ValueText = valueText;
		this.BackgroundTint = backgroundTint;
		this.Slider = slider;
		this.Active = active;
		this.ValueSetByAnimation = valueSetByAnimation;
		this.OverrideVal = overrideVal;
	}

}
