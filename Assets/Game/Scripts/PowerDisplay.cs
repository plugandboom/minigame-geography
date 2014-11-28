using UnityEngine;
using System.Collections;

/// <summary>
/// Displayer with the amount of movements left.
/// </summary>
public class PowerDisplay : MonoBehaviour
{
	public TextMesh raisePower;
	public TextMesh lowerPower;

	/// <summary>
	/// Updates the HUD with the amount left of each power.
	/// </summary>
	/// <param name="raise">Raise.</param>
	/// <param name="lower">Lower.</param>
	public void Refresh(int raise, int lower)
	{
		raisePower.text = raise.ToString();
		lowerPower.text = lower.ToString();
	}
}

