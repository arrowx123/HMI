using UnityEngine;
using System.Collections;

//	Move the object from one position to another position
public class FixedAngleRotation : MonoBehaviour
{

	//	parameters to be set
	private float rotationSpeed;
	private float rotationDiff;
	private Vector3 rotationAxis;


	//internal parameters
	private bool rotationTriggerControl = false;
	private Rigidbody rb;
	private float cumulativeAngle;
	private bool rotationTriggerTriggered;
	// False: anticlockwise; True: clockwise
	private bool rotationDirection = false; 


	//	parameters initialization
	public void setRotationSpeed (float speed)
	{
		rotationSpeed = speed;
	}

	public void setRotationDiff (float diff)
	{
		rotationDiff = diff;
	}

	public void setRotationAxis(Vector3 axis)
	{
		rotationAxis = axis;
	}



	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();

		Debug.Log ("FixedAngleRotation: Initialization succeeds.");

		if (rb == null) {
			Debug.Log ("FixedAngleRotation: Can not find the rigid body!");
		}
	}



	public void rotateToNextAngle (bool direction)
	{
		rotationTriggerControl = true;
		rotationDirection = direction;
	}

	public bool inMovement ()
	{
		return rotationTriggerControl;
	}



	// Update is called once per frame
	void FixedUpdate ()
	{

		if (rotationTriggerControl == false)
			return;

		float currentAngleGap = rotationSpeed;

		if (cumulativeAngle + currentAngleGap > rotationDiff) {
			currentAngleGap = rotationDiff - cumulativeAngle;	

			rotationTriggerControl = false;
			cumulativeAngle = 0;

		} else {
			cumulativeAngle += rotationSpeed;
		}

		if(rotationDirection)
			transform.Rotate (rotationAxis, currentAngleGap, Space.Self);
		else
			transform.Rotate (rotationAxis, -currentAngleGap, Space.Self);
	}
}
