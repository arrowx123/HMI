using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour
{
	private Vector3 rotationAxis;
	private float rotationSpeed = 0;


	public void setRotationAxis(Vector3 axis)
	{
		rotationAxis = axis;
	}

	// Use this for initialization
	void Start ()
	{
		
	}


	public void setRotationSpeed (float realRotationSpeed)
	{
		rotationSpeed = realRotationSpeed;
//		Debug.Log ("Rotation -> rotationSpeed:" + rotationSpeed);
	}

	void FixedUpdate ()
	{
		transform.Rotate (rotationAxis, rotationSpeed, Space.Self);
	}
}
