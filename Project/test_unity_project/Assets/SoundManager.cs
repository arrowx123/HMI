using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource efxSource;
	// Use this for initialization
	void Start () {
	
	}

	public void PlaySingle(){
		efxSource.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
