using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the main character and its movemento over the terrain.
/// </summary>
public class Traveler : MonoBehaviour {

	private GameController _gameController;
	private Hexagon _targetHexagon;

	void OnEnable()
	{
		Hexagon.OnMapChange += SearchPath;
	}

	void OnDisable()
	{
		Hexagon.OnMapChange -= SearchPath;
	}

	void Start()
	{
		_gameController = 
			((GameObject)GameObject.FindGameObjectWithTag(Tag.gameController)).GetComponent<GameController>();

		FindTargetHexagon();

		GetComponent<Pathfinding>().FindPath(transform.position, _targetHexagon.transform.position);
	}

	void FixedUpdate()
	{
		if(GetComponent<Pathfinding>().Path.Count > 1 && rigidbody.velocity == Vector3.zero){
			StartCoroutine(Move(GetComponent<Pathfinding>().Path[1]));
			GetComponent<Pathfinding>().Path.RemoveAt(0);
		}
	}

	/// <summary>
	/// Moves the main character to the targeted coordinates.
	/// </summary>
	/// <param name="target">targeted coordinates.</param>
	private IEnumerator Move(Vector3 target)
	{
		float distance = (target - transform.position).magnitude;
		transform.LookAt(target);
		while(distance > 0.1f)
		{
			rigidbody.velocity = (target - transform.position).normalized * 2f;
			distance = (target - transform.position).magnitude;
			yield return null;
		}
		rigidbody.velocity = Vector3.zero;
		if(CurrentHexagon == _targetHexagon)
		{
			_gameController.EndGame(true);
		}
		yield return null;
	}

	/// <summary>
	/// Gets the current hexagon under the main character.
	/// </summary>
	/// <value>The hexagon under the main character.</value>
	public Hexagon CurrentHexagon{
		get{
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, Layer.floor))
			{
				return hit.collider.GetComponentInParent<Hexagon>();
			}
			return null;
		}
	}

	/// <summary>
	/// Finds out which hexagon is the destination and store it on a private variable.
	/// </summary>
	private void FindTargetHexagon()
	{
		
		GameObject finalDestination = GameObject.FindGameObjectWithTag(Tag.destination);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(finalDestination.transform.position + Vector3.up, Vector3.down, out hit, 2f, Layer.floor))
		{
			_targetHexagon = hit.collider.GetComponentInParent<Hexagon>();
		}
	}

	/// <summary>
	/// Gets the hexagon that corresponds to the final destination.
	/// </summary>
	/// <value>The targeted hexagon.</value>
	public Hexagon TargetHexagon{
		get{
			return _targetHexagon;
		}
	}

	/// <summary>
	/// Searchs for a path from the character current position to the final destination.
	/// </summary>
	private void SearchPath()
	{
		GetComponent<Pathfinding>().FindPath(transform.position, _targetHexagon.transform.position);
	}
}
