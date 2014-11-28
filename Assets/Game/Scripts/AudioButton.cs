using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the behavior of the audio button.
/// </summary>
public class AudioButton : MonoBehaviour
{
	public Sprite buttonOn;
	public Sprite buttonOnMouseOver;
	public Sprite buttonOff;
	public Sprite buttonOffMouseOver;

	// Use this for initialization
	void Start ()
	{
		// Load from the player prefs the initial status of the button
		RefreshButtonImage();
	}

	void OnMouseEnter()
	{
		if(AudioController.instance.On)
		{
			transform.GetComponent<SpriteRenderer>().sprite = buttonOnMouseOver;
		}
		else
		{
			transform.GetComponent<SpriteRenderer>().sprite = buttonOffMouseOver;
		}
	}
	
	void OnMouseExit()
	{
		RefreshButtonImage();
	}

	void OnMouseOver()
	{
		if(Input.GetMouseButtonDown(0))
		{
			ToggleButton();
			RefreshButtonImage();
		}
	}

	/// <summary>
	/// Refreshs the button image to match the state of the AudioController.
	/// </summary>
	private void RefreshButtonImage()
	{
		if(AudioController.instance.On)
		{
			transform.GetComponent<SpriteRenderer>().sprite = buttonOn;
		}
		else
		{
			transform.GetComponent<SpriteRenderer>().sprite = buttonOff;
		}
	}

	/// <summary>
	/// <para>Toggles between the on/off states of the audio button button.</para>
	/// <para>The states of the button are actually the states of the AudioController, so this function updates the 
	/// current state of the AudioController</para>
	/// </summary>
	private void ToggleButton()
	{
		AudioController.instance.On = !AudioController.instance.On;
	}
}

