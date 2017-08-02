using UnityEngine;
using System.Collections;
using Live2D.Cubism.Core;
using UnityEngine.UI;

/// <summary>
/// This class contains all information about values and UI elements related
/// to one CubismPart.
/// </summary>
public class CubismPartInfo
{
	/// <summary>
	/// The cubism part.
	/// </summary>
	public CubismPart Part { get; set; }

	/// <summary>
	/// Whether or not the part opacity is currently being overridden by the user.
	/// </summary>
	public bool Active { get; set; }

	/// <summary>
	/// The part opacity slider UI element.
	/// </summary>
	public Slider Slider { get; set; }

	/// <summary>
	/// The override toggle UI element.
	/// </summary>
	public Toggle Toggle { get; set; }

	/// <summary>
	/// The text field UI element that shows the current part opacity.
	/// </summary>
	public Text ValueText { get; set; }

	/// <summary>
	/// The background image of the part list entry. Indicates override state.
	/// </summary>
	public Image BackgroundTint { get; set; }

	/// <summary>
	/// Whether or not the last opacity value was set by the animation.
	/// Needed because the manual call to change the slider value also calls the slider value changed callback.
	/// </summary>
	public bool ValueSetByAnimation { get; set; }

	/// <summary>
	/// The override opacity value set by the user.
	/// </summary>
	public float OverrideVal { get; set; }

	/// <summary>
	/// The default opacity of the part (opacity when model is loaded).
	/// </summary>
	public float DefaultOpacity { get; set; }

	/// <summary>
	/// CubismPartInfo Constructor.
	/// </summary>
	public CubismPartInfo(CubismPart part, Toggle toggle, Text valueText, Image backgroundTint,
		Slider slider, bool active, bool valueSetByAnimation, float overrideVal, float defaultOpacity) {
		this.Part = part;
		this.Toggle = toggle;
		this.ValueText = valueText;
		this.BackgroundTint = backgroundTint;
		this.Slider = slider;
		this.Active = active;
		this.ValueSetByAnimation = valueSetByAnimation;
		this.OverrideVal = overrideVal;
		this.DefaultOpacity = defaultOpacity;
	}

}
