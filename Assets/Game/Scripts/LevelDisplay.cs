using UnityEngine;
using System.Collections;

/// <summary>
/// Displayer with the number of the current level.
/// </summary>
public class LevelDisplay : MonoBehaviour {

	void Start()
	{
		TextMesh t = (TextMesh)gameObject.GetComponent(typeof(TextMesh));
		int level = Application.loadedLevel -1;
		t.text = level.ToString();
	}
}
