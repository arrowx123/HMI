using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinishScene : MonoBehaviour
{

	private SceneParameterManager SceneParameterManagerController;
	private System.DateTime[] time_record;

	public Text[] time_display = new Text[12];

	// Use this for initialization
	void Start ()
	{

		GameObject SceneParameterManagerControllerObject = GameObject.FindWithTag ("SceneParameterManager");
		if (SceneParameterManagerControllerObject != null) {
			SceneParameterManagerController = SceneParameterManagerControllerObject.GetComponent<SceneParameterManager> ();
		} else {
			Debug.Log ("GameController: Can not find SceneParameterManager!");
		}

		System.DateTime[] time_record = SceneParameterManagerController.GetSceneArguments ();

		Debug.Log ("In FinishScene: ");

		for (int i = 0; i < time_record.Length; i++) {
			time_display [i].text = time_record [i].ToString();
			Debug.Log (time_record [i]);
		}



//		System.Threading.Thread.Sleep (5000);

//		#if UNITY_EDITOR
//		// Application.Quit() does not work in the editor so
//		// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
//		UnityEditor.EditorApplication.isPlaying = false;
//		#else
//		Application.Quit();
//		#endif

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
