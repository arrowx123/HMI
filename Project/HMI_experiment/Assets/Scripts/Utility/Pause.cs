using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour
{
	void Start()
	{
		
	}

	public void start_pause(int seconds){

		StartCoroutine(pause_function(seconds));
	
	}

	IEnumerator pause_function(int seconds)
	{
		print(Time.time);
		yield return new WaitForSeconds(seconds);
		print(Time.time);
	}
}