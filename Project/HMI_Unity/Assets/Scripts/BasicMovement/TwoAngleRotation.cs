using UnityEngine;
using System.Collections;

//	Move the object from one position to another position
public class TwoAngleRotation : MonoBehaviour
{

	//	parameters to be set
	private float rotationSpeed;
	private float rotationDiff;
	private Vector3 rotationAxis;


	private bool rotationTriggerControl = false;
	private bool stage = false;

	private Rigidbody rb;
	private float cumulativeAngle;
	private bool rotationTriggerTriggered;

	private GameController gameController;


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

		Debug.Log ("TwoAngleRotation: Initialization succeeds.");

		if (rb == null) {
			Debug.Log ("TwoAngleRotation: Can not find the rigid body!");
		}
	}



	public void rotateToNextAngle ()
	{
//		Debug.Log ("Trigger rotateToNextAngle.");
		if (rotationDiff < 0) {
			stage = !stage;
			rotationDiff = Mathf.Abs (rotationDiff);
		}
		rotationTriggerControl = true;
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

			stage = !stage;

		} else {
			cumulativeAngle += rotationSpeed;
		}

		if (stage) {
			transform.Rotate (rotationAxis, currentAngleGap, Space.Self);
		} else {
			transform.Rotate (rotationAxis, -currentAngleGap, Space.Self);
		}
	}
}
