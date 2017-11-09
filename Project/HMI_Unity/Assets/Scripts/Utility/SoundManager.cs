using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

	public AudioSource collisionSource;
	public AudioSource vibrationSource;
	public AudioSource torqueSource;
	public AudioSource coupleSource;

	public static SoundManager instance = null;

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	void Awake ()
	{

		if (instance == null)
			instance = this;
		else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

	}

	public void play_collision ()
	{
//		if (!collisionSource.isPlaying)
		collisionSource.Play ();
	}

	public void stop_collision ()
	{
		collisionSource.Stop ();
	}


	public void play_vibration ()
	{
		if (!vibrationSource.isPlaying)
			vibrationSource.Play ();
	}

	public void stop_vibration ()
	{
		vibrationSource.Stop ();
	}


	public void play_torque ()
	{
		if (!torqueSource.isPlaying)
			torqueSource.Play ();
	}

	public void stop_torque ()
	{
		torqueSource.Stop ();
	}


	public void play_couple ()
	{
		if (!coupleSource.isPlaying)
		coupleSource.Play ();
	}

	public void stop_couple ()
	{
		coupleSource.Stop ();
	}



	public void PlayNewClip (AudioClip clip)
	{

		collisionSource.clip = clip;
		collisionSource.Play ();

	}

	// Use this for initialization
	void Start ()
	{
//		efxSource.Play ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
