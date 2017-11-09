using UnityEngine;
using System.Collections;

public class flangeNutRotation : MonoBehaviour
{
	private Vector3 rotationAxis;
	private float rotationSpeed = 0;

	private FlangeNutState FlangeNutStateController;


	public void setRotationAxis (Vector3 axis)
	{
		rotationAxis = axis;
	}

	// Use this for initialization
	void Start ()
	{
		
		FlangeNutStateController = this.gameObject.GetComponent<FlangeNutState> ();

//		GameObject FlangeNutStateObject = GameObject.FindWithTag ("flangenut");
//
//		if (FlangeNutStateObject != null) {
//
//			FlangeNutStateController = FlangeNutStateObject.GetComponent<FlangeNutState> ();
//
//		} else {
//			Debug.Log ("flangeNutRotation: can not find FlangeNutStateController");
//			
//		}

	}


	public void setRotationSpeed (float realRotationSpeed)
	{
		rotationSpeed = realRotationSpeed;
		//		Debug.Log ("Rotation -> rotationSpeed:" + rotationSpeed);
	}

	void FixedUpdate ()
	{
		if (!FlangeNutStateController.toTheMaximumPos ())
			transform.Rotate (rotationAxis, rotationSpeed, Space.Self);
	}
}