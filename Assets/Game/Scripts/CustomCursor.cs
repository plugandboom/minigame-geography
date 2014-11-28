using UnityEngine;
using System.Collections;

/// <summary>
/// Configures the customized cursor used inside the game.
/// </summary>
public class CustomCursor : MonoBehaviour {
	
	public Texture2D cursorTex;
	private CursorMode cursorMode;
	private Vector2 hotSpot;

	void Start () {
	
		cursorMode = CursorMode.Auto;
		hotSpot = Vector2.zero;
		Cursor.SetCursor(cursorTex, hotSpot, cursorMode);
	}
}
