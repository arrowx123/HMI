using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnClick : MonoBehaviour
{

	//	// Use this for initialization
	//	void Start () {
	//
	//	}
	//
	//	// Update is called once per frame
	//	void Update () {
	//
	//	}

	public void LoadScene (int level)
	{
		GameObject buttonTextObject = GameObject.FindWithTag ("button_text");
		Text buttonText = buttonTextObject.GetComponent<Text> ();

		buttonText.text = "Pressed";
//		buttonTextObject.text
//		buttonTextObject.



//		Application.LoadLevel (level);
		SceneManager.LoadScene (level);

	}

}
