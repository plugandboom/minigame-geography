using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the hexagonal tiles used to buil the game terrain.
/// </summary>
public class Hexagon : MonoBehaviour {

	public Hexagon[] neighbors;
	public Material highlightedMaterial;
	public AudioClip moveSound;
	public AudioClip stopSound;

	private static Dictionary<string, Hexagon> all;
	private WaypointNode waypoint;
	private Material _originalMaterial;
	private Traveler _traveler;
	private GameController _gameController;
	private bool _moving;
	private int _movingHexagonCount;

	private enum Movement{Up, Down};

	public delegate void MapChanged();
	public static event MapChanged OnMapChange;

	void Awake()
	{
		ConfigWayPoint();

		ConfigHexagonName();

		if(all == null)
		{
			all = new Dictionary<string, Hexagon>();
		}
		all.Add(gameObject.name, this);

		// Start the movement count as zero
		_movingHexagonCount = 0;

		_originalMaterial = transform.FindChild("hexagon_body").renderer.material;
	}

	void Start()
	{
		_traveler = ((GameObject)GameObject.FindGameObjectWithTag(Tag.player)).GetComponent<Traveler>();
		_gameController = 
			((GameObject)GameObject.FindGameObjectWithTag(Tag.gameController)).GetComponent<GameController>();

		ConfigHexagonNeighborsInfo();
		RefreshNeighborsOnGraph();
	}

	/// <summary>
	/// Sets the object Waypoint, used by the PathFinder.
	/// </summary>
	private void ConfigWayPoint()
	{
		waypoint = GetComponentInChildren<WaypointNode>();
		waypoint.position = transform.position;
	}

	/// <summary>
	/// Configs the unique name of the hexagon based on its position.
	/// </summary>
	private void ConfigHexagonName()
	{
		gameObject.name = transform.position.x + ":" + transform.position.z;
	}

	/// <summary>
	/// Updates the list of hexagons close to the object.
	/// </summary>
	private void ConfigHexagonNeighborsInfo()
	{
		neighbors = new Hexagon[6];
		for(int i = 0; i < 6; i++)
		{
			Quaternion rotation = Quaternion.AngleAxis(30 + (60 * i), Vector3.up);
			Vector3 vector = rotation * (Vector3.forward);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(transform.position - new Vector3(0, 50f, 0), vector, out hit, 2f, Layer.floor))
			{
				Hexagon neighbor = hit.collider.GetComponentInParent<Hexagon>();
				if(neighbor != null)
				{
					neighbors[i] = neighbor;
				}
			}
		}
	}

	/// <summary>
	/// Updates the Pathfinder neighborhood information of the object.
	/// </summary>
	private void RefreshNeighborsOnGraph()
	{
		// Clear the list with the node previous connections. It is important to do this, because sometimes the old
		// neighbours, that were no longer neighbours wouldn't be removed from the list. If at some point the game 
		// starts having performance issues, this can be rewritten.
		waypoint.neighbors = new List<WaypointNode>();

		for(int i = 0; i < 6; i++)
		{
			if(neighbors[i] != null)
			{
				WaypointNode neighborWp = neighbors[i].GetComponentInChildren<WaypointNode>();
				if(Mathf.Abs(transform.position.y - neighbors[i].transform.position.y) < 0.1)
				{
					waypoint.neighbors.Add(neighborWp);
				}
			}
		}

	}

	/// <summary>
	/// Gets an hexagon among all by its id.
	/// </summary>
	/// <returns>The hexagon.</returns>
	/// <param name="id">Identifier.</param>
	public static Hexagon FindHexagon(string id)
	{
		return all[id];
	}

	void OnMouseEnter()
	{
		if(Movable())
		{
			transform.FindChild("hexagon_body").renderer.material = highlightedMaterial;
		}
	}

	void OnMouseExit()
	{
		transform.FindChild("hexagon_body").renderer.material = _originalMaterial;
	}

	void OnMouseOver()
	{
		if(Movable())
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(_gameController.upPower > 0)
				{
					_gameController.upPower--;
					StartCoroutine(Move(Movement.Up));
				}
			}
			if(Input.GetMouseButtonDown(1))
			{
				if(_gameController.downPower > 0)
				{
					_gameController.downPower--;
					StartCoroutine(Move(Movement.Down));
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Hexagon"/> is moving.
	/// </summary>
	/// <value><c>true</c> if moving; otherwise, <c>false</c>.</value>
	private bool moving{
		get{
			return _moving;
		}
		set{
			_moving = value;
			if(_moving)
			{
				_movingHexagonCount += 1;
			}
			else
			{
				_movingHexagonCount -= 1;
			}
		}
	}

	/// <summary>
	/// Check if this is a movable hexagon.
	/// </summary>
	/// <returns><c>true</c> if is a movable hexagon; otherwise, <c>false</c>.</returns>
	private bool Movable()
	{
		if(this != _traveler.CurrentHexagon && this != _traveler.TargetHexagon)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Moves the object up or down.
	/// </summary>
	/// <param name="movement">The up or down movement.</param>
	private IEnumerator Move(Movement movement)
	{
		// Start the movement Audio
		PlayAudio(moveSound);

		Vector3 direction = Vector3.down;
		if(movement == Movement.Up)
		{
			direction = Vector3.up;
		}

		float targetY = transform.position.y + direction.y;
		moving = true;
		MoveNeighbors(movement);

		while(transform.position.y != targetY)
		{
			transform.position += direction * 2f * Time.deltaTime;
			if((movement == Movement.Up && transform.position.y > targetY) || 
			   (movement == Movement.Down && transform.position.y < targetY))
			{
				transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
			}
			yield return null;
		}
		moving = false;
		ConfigWayPoint();

		RefreshPath ();

		// Stop the movement audio and play the stop audio
		PlayAudio(stopSound);
		
		yield return null;
	}

	/// <summary>
	/// Updates the connections between the hexagons, so a path can be searched.
	/// </summary>
	private void RefreshPath()
	{
		// If all the hexagons stopped moving
		if(_movingHexagonCount == 0)
		{
			foreach(string hexName in all.Keys)
			{
				all[hexName].RefreshNeighborsOnGraph();
			}
			
			if(OnMapChange != null)
			{
				OnMapChange();
			}
		}
	}

	/// <summary>
	/// Checks which neighbors of the object should also move and moves them.
	/// </summary>
	/// <param name="movement">The up or down movement.</param>
	private void MoveNeighbors(Movement movement)
	{
		for(int i = 0; i < 6; i++)
		{
			if(neighbors[i] != null)
			{
				float distance = 0;
				if(movement == Movement.Up)
				{
					distance = transform.position.y - neighbors[i].transform.position.y;
				}
				else // movement == Movement.Down
				{
					distance = neighbors[i].transform.position.y - transform.position.y;
				}

				if(distance > 0.5f){
					if(neighbors[i].Movable() && !neighbors[i].moving)
					{
						StartCoroutine(neighbors[i].Move(movement));
					}
				}
			}
		}
	}

	void OnDestroy()
	{
		// Clean the list of hexagons
		all.Remove(gameObject.name);
	}

	/// <summary>
	/// Playes and AudioClip. Used when a hexagon moves.
	/// </summary>
	/// <param name="clip">The AudioClip to play.</param>
	private void PlayAudio(AudioClip clip)
	{
		if(AudioController.instance.On)
		{
			audio.clip = clip;
			audio.Play ();
		}
	}
}
