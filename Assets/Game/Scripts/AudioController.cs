using UnityEngine;
using System.Collections;

/// <summary>
/// <para>Manages the configuration for the audio played in the game.</para>
/// <para>To make sure all the audio is managed by a single AudioController, this class implements the singleton 
/// pattern. In order to retrieve the class object, call AudioController.instance.</para>
/// <para>Other classes that need to play sounds should always check this class object for the audio on/off 
/// configuration.</para>
/// <para>Internally the class uses two AudioSources, one the play ambient and wind sounds, and the other to play 
/// chimes and droplets sounds.</para>
/// </summary>
public class AudioController : MonoBehaviour
{
	public float minTrack2Interval;
	public float maxTrack2Interval;

	private static AudioController _instance;

	private AudioSource _audioSource1;
	private AudioClip[] _track1;
	private float _startTrack1At;
	private AudioSource _audioSource2;
	private AudioClip[] _track2;
	private float _startTrack2At;
	private bool _soundOn;
	private bool _soundInfoLoaded;
	
	void Awake()
	{
		if(_instance == null)
		{
			// If this is the first instance, make it the Singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			// If a Singleton already exists and you find another reference in scene, destroy it!
			if(this != _instance)
				Destroy(this.gameObject);
		}
	}
	
	public void Start()
	{
		// Instantiate 2 AudioSourceComponents
		_audioSource1 = gameObject.AddComponent<AudioSource>();
		_audioSource2 = gameObject.AddComponent<AudioSource>();

		_audioSource1.mute = !On;
		_audioSource2.mute = !On;

		// Initialize the audio lists dynamically, instead of populating them from the editor
		_track1 = LoadAudioClipsFromFolder("Audio/Background1");
		_track2 = LoadAudioClipsFromFolder("Audio/Background2");

		_audioSource1.clip = GetRandomClip(_track1);
		_audioSource1.Play();
		UpdateTrack1StarterTime();

		// Manually start the track2 starter time
		_startTrack2At = Time.time + Random.Range(minTrack2Interval, maxTrack2Interval);
	}

	public void Update()
	{
		if(Time.time >= _startTrack1At)
		{
			_audioSource1.clip = GetRandomClip(_track1);
			_audioSource1.Play();
			UpdateTrack1StarterTime();
		}

		if(Time.time >= _startTrack2At)
		{
			_audioSource2.clip = GetRandomClip(_track2);
			_audioSource2.Play();
			UpdateTrack2StarterTime();
		}
	}

	/// <summary>
	/// Updates the time to start playing the next AudioClip on the Track1
	/// </summary>
	private void UpdateTrack1StarterTime()
	{
		_startTrack1At = Time.time + _audioSource1.clip.length;
	}

	/// <summary>
	/// Updates the time to start playing the next AudioClip on the Track2
	/// </summary>
	private void UpdateTrack2StarterTime()
	{
		_startTrack2At = Time.time + _audioSource2.clip.length + Random.Range(minTrack2Interval, maxTrack2Interval);
	}

	/// <summary>
	/// Gets a random clip from a list of AudioClips.
	/// </summary>
	/// <returns>A random AudioClip.</returns>
	/// <param name="audioList">An AudioClip list.</param>
	private AudioClip GetRandomClip(AudioClip[] audioList)
	{
		return audioList[Random.Range(0, audioList.Length)];
	}

	/// <summary>
	/// <para>This class implements the singleton pattern.</para>
	/// <para>This pattern is used to retrieve the single instance of the AudioController.</para>
	/// </summary>
	/// <value>The instance.</value>
	public static AudioController instance
	{
		get
		{
			if(_instance == null)
			{
				// Since _instance is defined when the awake method is called, this if probably won't be called, but in
				// case it is, we better not let it return a null value, unless there is really no AudioController on
				// the scene.
				_instance = GameObject.FindObjectOfType<AudioController>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	/// <summary>
	/// Loads all the audio clips from a folder.
	/// </summary>
	/// <returns>The audio clips from the folder.</returns>
	/// <param name="path">Path to a folder with the AudioClips.</param>
	private AudioClip[] LoadAudioClipsFromFolder(string path)
	{
		return Resources.LoadAll<AudioClip>(path);
	}

	/// <summary>
	/// Loads the audio configuration.
	/// </summary>
	private void LoadConfig()
	{
		if(PlayerPrefs.HasKey("SoundOn"))
		{
			if(PlayerPrefs.GetInt("SoundOn") == 1)
			{
				_soundOn = true;
			}
			else
			{
				_soundOn = false;
			}
		}
		else
		{
			_soundOn = true;
		}
	}

	/// <summary>
	/// Saves the current audio configuration (mute or unmute).
	/// </summary>
	private void SaveConfig()
	{
		if(_soundOn)
		{
			PlayerPrefs.SetInt("SoundOn", 1);
		}
		else
		{
			PlayerPrefs.SetInt("SoundOn", 0);
		}
	}

	/// <summary>
	/// Mute all audio.
	/// </summary>
	private void Mute()
	{
		_audioSource1.mute = true;
		_audioSource2.mute = true;
	}

	/// <summary>
	/// Unmute all audio.
	/// </summary>
	private void Unmute()
	{
		_audioSource1.mute = false;
		_audioSource2.mute = false;
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="AudioController"/> is on.
	/// </summary>
	/// <value><c>true</c> if on; otherwise, <c>false</c>.</value>
	public bool On{
		get{
			if(!_soundInfoLoaded)
			{
				LoadConfig();
				_soundInfoLoaded = true;
			}
			return _soundOn;
		}
		set{
			_soundOn = value;
			SaveConfig();
			if(_soundOn)
			{
				Unmute ();
			}
			else
			{
				Mute ();
			}
		}
	}
}

