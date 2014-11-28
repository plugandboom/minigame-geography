using UnityEngine;
using System.Collections;

/// <summary>
/// Main menu screen.
/// </summary>
public class MenuController : MonoBehaviour
{
	public GameObject curtain;
	public string firstLevel;

	void Start()
	{
		StartCoroutine(FadeIn());
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(!Physics.Raycast(ray, 100f))
			{
				StartCoroutine(FadeOut());
			}
		}
	}

	/// <summary>
	/// Plays the fade out effect when openening a level.
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

		if(PlayerPrefs.HasKey("current_level") && PlayerPrefs.GetString("current_level") != "main_menu")
		{
			Application.LoadLevel(PlayerPrefs.GetString("current_level"));
		}
		else
		{
			Application.LoadLevel(firstLevel);
		}
	}

	/// <summary>
	/// Plays the fade in effect animation when opening the menu screen.
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
	}
}

