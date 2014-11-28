using UnityEngine;
using System.Collections;

/// <summary>
/// Reesponsible for the behavior of the restart button.
/// </summary>
public class RestartButton : MonoBehaviour
{
	public Sprite defaultSprite;
	public Sprite mouseOverSprite;

	private GameController _gameController;

	void Start ()
	{
		_gameController = GameObject.FindGameObjectWithTag(Tag.gameController).GetComponent<GameController>();
	}

	void OnMouseEnter()
	{
		transform.GetComponent<SpriteRenderer>().sprite = mouseOverSprite;
	}
	
	void OnMouseExit()
	{
		transform.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}
	
	void OnMouseOver()
	{
		if(Input.GetMouseButtonDown(0))
		{
			_gameController.EndGame(false);
		}
	}
}

