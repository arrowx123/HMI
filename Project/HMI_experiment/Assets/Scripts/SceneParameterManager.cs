using UnityEngine;
using System.Collections;

public class SceneParameterManager : MonoBehaviour
{

	static private System.DateTime[] time_record;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame

	public void SetSceneArguments(System.DateTime[] _time_record){
	
		time_record = _time_record;
	
	}

	public System.DateTime[] GetSceneArguments ()
	{
		return time_record;
	}
}
