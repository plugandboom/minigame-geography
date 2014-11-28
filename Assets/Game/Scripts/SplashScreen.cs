using UnityEngine;
using System.Collections;

/// <summary>
/// Splash screen with the Plug & Boom Labs logo shown at the start of the game.
/// </summary>
public class SplashScreen : MonoBehaviour {

	public GameObject curtain;

	// Use this for initialization
	void Start () {
		StartCoroutine(FadeIn());
	}

	/// <summary>
	/// Applies the fade out effect before loading the main menu.
	/// </summary>
	/// <returns>The out.</returns>
	private IEnumerator FadeOut()
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
		
		Application.LoadLevel("main_menu");
	}

	/// <summary>
	/// Applies the fade in effect when the splash screen is shown.
	/// </summary>
	/// <returns>The in.</returns>
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
		yield return new WaitForSeconds(2f);
		StartCoroutine(FadeOut());
		yield return null;
	}
}
