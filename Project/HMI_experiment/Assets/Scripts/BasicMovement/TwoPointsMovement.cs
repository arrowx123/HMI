using UnityEngine;
using System.Collections;

//	Move the object from one position to another position
public class TwoPointsMovement : MonoBehaviour {

//	parameters to be set
	private float movementSpeed;
	private Vector3 posDiff;
	private Vector3 localPosDiff;



	private Vector3 scaleVector;
	private float scale;

	//	private Vector3 drillControl;
	private bool rotationTriggerControl = false;

	private Rigidbody rb;
	private float startTime;

	private Vector3 startPos;
	private Vector3 endPos;
	private float journeyLength;

	private bool stage = false;

	//	Vector3 triggerSpeed = Vector3.zero;

	private bool rotationTriggerTriggered;



//	parameters initialization
	public void setMovementSpeed(float speed)
	{
		movementSpeed = speed;

		Debug.Log ("set movementSpeed: " + movementSpeed);
	}

	public void setPosDiff(Vector3 diff)
	{
		localPosDiff = diff;
		posDiff = transform.TransformVector (localPosDiff) * scale;

		Debug.Log("diff: " + diff);
		Debug.Log("globalPosDiff: " + posDiff);
	}





	// Use this for initialization
	void Start () {

//		this.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);

		rb = GetComponent<Rigidbody> ();

		scaleVector = transform.localScale;
		scale = 1 / scaleVector.x;

		Debug.Log ("start movementSpeed: " + movementSpeed);

		if (rb == null) {
			Debug.Log ("TwoPointsMovement: Can not find the rigid body!");
		}
	}



	public void move()
	{
		initialize ();
		rotationTriggerControl = true;
	}

	public bool inMovement()
	{
		return rotationTriggerControl;
	}

	public bool returnStage()
	{
		return stage;
	}

	void initialize()
	{
		posDiff = transform.TransformVector (localPosDiff) * scale;

		startTime = Time.time;

		if (!stage) {
//			startPos = rb.transform.localPosition;
			startPos = rb.position;
			endPos = startPos + posDiff;

			Debug.Log ("startPos: " + startPos + "\n");
			Debug.Log ("startPos: " + endPos + "\n");
		} else {
//			startPos = rb.transform.localPosition - posDiff;
			startPos = rb.position - posDiff;
//			endPos = rb.transform.localPosition;
			endPos = rb.position;

			Debug.Log ("startPos: " + startPos + "\n");
			Debug.Log ("startPos: " + endPos + "\n");
		}

		journeyLength = Vector3.Distance (startPos, endPos);
	}


	// Update is called once per frame
	void Update () {

		float distCovered;
		float fracJourney;

		if (rotationTriggerControl == false)
			return;

		if (!stage) {
			distCovered = (Time.time - startTime) * movementSpeed;
			fracJourney = distCovered / journeyLength;

//			rb.transform.localPosition = Vector3.Lerp (startPos, endPos, fracJourney);
			rb.position = Vector3.Lerp (startPos, endPos, fracJourney);

//			if (rb.transform.position == endPos) {
			if (rb.position == endPos) {
				startTime = Time.time;
				stage = true;

				rotationTriggerControl = false;
			}
		} else {
			distCovered = (Time.time - startTime) * movementSpeed;
			fracJourney = distCovered / journeyLength;

//			rb.transform.localPosition = Vector3.Lerp (endPos, startPos, fracJourney);
			rb.position = Vector3.Lerp (endPos, startPos, fracJourney);

//			if (rb.transform.position == startPos) {
			if (rb.position == startPos) {
				startTime = Time.time;
				stage = false;
				rotationTriggerControl = false;
			}
		}
	}

	void FixedUpdate ()
	{

	}
}
