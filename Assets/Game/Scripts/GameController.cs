using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the start and end of a level.
/// </summary>
public class GameController : MonoBehaviour
{
	public int startingUpPower;
	public int startingDownPower;
	public string nextLevel;
	public GameObject curtain;
	public PowerDisplay powerDisplay;

	private Traveler traveler;
	private int _upPower;
	private int _downPower;

	void Start()
	{
		traveler = ((GameObject)GameObject.FindGameObjectWithTag(Tag.player)).GetComponent<Traveler>();
		_upPower = startingUpPower;
		_downPower = startingDownPower;
		RefreshPowerDisplay();
		StartCoroutine(FadeIn());
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			StartCoroutine(FadeOut(false, true));
		}
	}

	/// <summary>
	/// Ends the game.
	/// </summary>
	/// <param name="accomplished">If set to <c>true</c> the level was accomplished.</param>
	public void EndGame(bool accomplished)
	{
		StartCoroutine(FadeOut(accomplished, false));
	}

	/// <summary>
	/// <para>Coroutine played at the end of each level, with the fade out effect.</para>
	/// <para>If the accomplished param is true, loads the next stage, otherwise, reloads this same level.</para>
	/// <para>If the toMenu param is true, the user is leaving the level, and returning to the main menu. In this case,
	/// the main menu is loaded instead of the current or the next level.</para>
	/// </summary>
	/// <param name="accomplished">If set to <c>true</c> the stage was accomplished.</param>
	/// <param name="toMenu">If set to <c>true</c> returns the user back to the main screen.</param>
	private IEnumerator FadeOut(bool accomplished, bool toMenu)
	{
		curtain.SetActive(true);
		Color c = curtain.renderer.material.color;
		c.a = 0;
		curtain.renderer.material.color = c;
		for(int i = 0; i <= 255; i=i+10)
		{
			c.a = ((float)i)/255f;
			curtain.renderer.material.color = c;
			yield return new WaitForSeconds(10f/256f);
		}
		if(accomplished)
		{
			// Save the next level as the actual level
			PlayerPrefs.SetString("current_level", nextLevel);
			PlayerPrefs.Save();

			Application.LoadLevel(nextLevel);
		}else{
			if(toMenu)
			{
				Application.LoadLevel("main_menu");
			}
			Application.LoadLevel(Application.loadedLevelName);
		}
	}

	/// <summary>
	/// Coroutine played at the beginning of each level, with the fade in effect.
	/// </summary>
	private IEnumerator FadeIn()
	{
		curtain.SetActive(true);
		Color c = curtain.renderer.material.color;
		c.a = 1;
		curtain.renderer.material.color = c;
		for(int i = 255; i >= 0; i=i-10)
		{
			c.a = ((float)i)/255f;
			curtain.renderer.material.color = c;
			yield return new WaitForSeconds(10f/256f);
		}
		curtain.SetActive(false);
	}

	/// <summary>
	/// Gets or sets the amount of power to move the tiles up.
	/// </summary>
	/// <value>Up power.</value>
	public int upPower{
		get{
			return _upPower;
		}
		set{
			_upPower = value;
			RefreshPowerDisplay();
			StartCoroutine(CheckEndGame());
		}
	}

	/// <summary>
	/// Gets or sets the amount of power to move the tiles down.
	/// </summary>
	/// <value>Down power.</value>
	public int downPower{
		get{
			return _downPower;
		}
		set{
			_downPower = value;
			RefreshPowerDisplay();
			StartCoroutine(CheckEndGame());
		}
	}

	/// <summary>
	/// <para>Coroutine that checks if the player ran out of power or if a route was found for the main character.
	/// </para>
	/// <para>If any of those two things happened, start the coroutine to end the current stage.</para>
	/// </summary>
	private IEnumerator CheckEndGame()
	{
		if(_upPower == 0 && _downPower == 0)
		{
			yield return new WaitForSeconds(2f);
			if(traveler.GetComponent<Pathfinding>().Path.Count == 0)
			{
				StartCoroutine(FadeOut(false, false));
			}
		}
	}

	/// <summary>
	/// Updates the visual display with the correct amount of powers left.
	/// </summary>
	private void RefreshPowerDisplay()
	{
		if(powerDisplay != null)
		{
			powerDisplay.Refresh(_upPower, _downPower);
		}
	}
}

