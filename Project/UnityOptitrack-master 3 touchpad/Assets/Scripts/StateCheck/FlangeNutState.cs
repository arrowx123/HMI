using UnityEngine;
using System.Collections;

public class FlangeNutState : MonoBehaviour {

	private float yMaximum = 0.075f;

	// Use this for initialization
	void Start () {
		Vector3 ori_pos;
		ori_pos.x = 0.0f;
		ori_pos.y = 0.35f;
		ori_pos.z = 0.0f;

		transform.localPosition = ori_pos;
	}
	
	// Update is called once per frame
	void Update () {

//		float x = transform.localPosition.x;
//		float y = transform.localPosition.y;
//		float z = transform.localPosition.z;
//
//		Debug.Log ("x: " + x + " y: " + y + " z: " + z);
	}


	public bool toTheMaximumPos(){
		if (transform.localPosition.y < yMaximum) {
			return true;
		} else {
			return false;
		}
	}
}