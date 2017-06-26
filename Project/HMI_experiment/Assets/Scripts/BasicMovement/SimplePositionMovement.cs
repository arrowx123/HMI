using UnityEngine;
using System.Collections;

//	Move each componenet of the "DrillCombination" simultaneously
public class SimplePositionMovement : MonoBehaviour
{

	//	parameters to be set
	private bool reference;

	// internal parameters
	private Vector3 movement;
	private Vector3 rotation;

	private Rigidbody rb;


	public void setReference (bool newReference)
	{
		reference = newReference;
	}
		

	// Use this for initialization
	void Start ()
	{
	
//		rb = GetComponent<Rigidbody> ();
//
//		if (rb == null) {
//			Debug.Log ("Can not find rigid body (DrillCombinationController)!");
//		}

	}



	// Update is called once per frame
	void Update ()
	{

		if (reference) {
			transform.Rotate (rotation * Time.deltaTime, Space.Self);	
			transform.Translate (movement * Time.deltaTime, Space.Self);	
		} else {
			transform.Rotate (rotation * Time.deltaTime, Space.World);	
			transform.Translate (movement * Time.deltaTime, Space.World);	
		}
	}
		

	public void setMovePosition (Vector3 newMovement)
	{
		movement = newMovement;
	}
		

	public void setMoveRotation (Vector3 newRotation)
	{
		rotation = newRotation;
	}

	public void moveTowardX(float movementSpeed){
		Vector3 newMovement;
		newMovement.x = movementSpeed;
		newMovement.y = 0;
		newMovement.z = 0;

		movement = newMovement;
	}

	public void moveTowardY(float movementSpeed){
		Vector3 newMovement;
		newMovement.x = 0;
		newMovement.y = movementSpeed;
		newMovement.z = 0;

		movement = newMovement;
	}

	public void moveTowardZ(float movementSpeed){
		Vector3 newMovement;
		newMovement.x = 0;
		newMovement.y = 0;
		newMovement.z = movementSpeed;

		movement = newMovement;
	}

	public void moveAgainstZ(float movementSpeed){
		Vector3 newMovement;
		newMovement.x = 0;
		newMovement.y = 0;
		newMovement.z = -movementSpeed;

		movement = newMovement;
	}


	public void moveUp()
	{
		movement = new Vector3 (0.0f, 1.0f, 0.0f);
	}

	void FixedUpdate ()
	{
		
	}
}
