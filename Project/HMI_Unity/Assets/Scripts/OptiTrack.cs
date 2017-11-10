/**
 * Adapted from johny3212
 * Written by Matt Oskamp
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class OptiTrack : MonoBehaviour
{

	public int handleRigidbodyIndex = 0;
	public int moverioRigidbodyIndex = 1;

	public Vector3 rotationOffset;

	private DetectCollision wrenchFittingController;
	private GameController GameController;

	//	private float[] position_y = { -0.9f, -1.9f, -0.9f, -1.9f, -0.9f, -1.9f, -0.9f, -1.9f };
	//	private float[] position_z = { 0f, 0f, -1f, -1f, -2f, -2f, -3f, -3f };
	//	private float[] position_z = { 0f, 0f, -2f, -2f, -4f, -4f, -6f, -6f };

	//	private float[] position_y = { -1.3f, -3.3f, -1.3f, -3.3f, -1.3f, -3.3f, -1.3f, -3.3f };
	//	private float[] position_z = { 1f, 1f, -1f, -1f, -3f, -3f, -5f, -5f };


	private float[] position_y = { -0.8f, -2.8f, -0.8f, -2.8f, -0.8f, -2.8f, -0.8f, -2.8f };
	private float[] position_z = { 0f, 0f, -2f, -2f, -4f, -4f, -6f, -6f };

	//	public Text log1;
	//	public Text log2;
	//	public Text log3;
	//	public Text log4;
	//	public Text log5;
	//	public Text log6;

	private float smoothPos = 1.0f;
	private float error_thres = 2.0f;
	private float handle_nut_diff = 3.8f;


	private static OptiTrack _instance;

	/** Handle position & rotation information in the 3D environment **/
	private Vector3 handleOriginalPos;
	private Vector3 handleCurrentPos;
	private Quaternion handleOriginalRot;
	private Quaternion handleCurrentRot;

	/** Moverio position & rotation information in the 3D environment **/
	//	private Vector3 moverioOriginalPos;
	private Vector3 moverioCurrentPos;
	//	private Quaternion moverioOriginalRot;
	private Quaternion moverioCurrentRot;

	private Vector3 handleLastPos;


	private Rigidbody[] optiTrackObjects;
	private Rigidbody moverio;
	private Rigidbody handle;
	private Camera currentCamera;

	private bool receivePositionData = false;



	public static OptiTrack Instance {
		get {
			if (_instance == null) {
				Debug.Log ("OptiTrack: Instance");
			}
			return _instance;
		}
	}


	public void changeSmoothPos (float changeInterval)
	{
		smoothPos = smoothPos + changeInterval;
	}

	//	public void addFieldOfView(float addInterval)
	//	{
	//		currentCamera.fieldOfView = currentCamera.fieldOfView + addInterval;
	//	}


	/** Get the objects of Moverio & Handle **/
	void allocateOptiTrackObjects ()
	{
		optiTrackObjects = this.GetComponentsInChildren<Rigidbody> ();
		foreach (Rigidbody optiTrackObject in optiTrackObjects) {
			if (optiTrackObject.tag == "Moverio") {
				moverio = optiTrackObject;

				currentCamera = moverio.GetComponentInChildren<Camera> ();
			} else if (optiTrackObject.tag == "Handle") {
				handle = optiTrackObject;
			}
		}
	}

	Vector3 roundToTenthDigtPos (Vector3 num)
	{
		Vector3 roundNum = new Vector3 (0.0f, 0.0f, 0.0f);

		roundNum.x = ((int)(num.x * 10) / 1) / 10.0f;
		roundNum.y = ((int)(num.y * 10) / 1) / 10.0f;
		roundNum.z = ((int)(num.z * 10) / 1) / 10.0f;

//		Debug.Log ("OptiTrack: num is: " + num + " " + "roundNum is: " + roundNum);

		return roundNum;
	}

	Vector3 roundToTenthDigtRot (Vector3 num)
	{
		Vector3 roundNum = new Vector3 (0.0f, 0.0f, 0.0f);

		roundNum.x = ((int)(num.x) / 2) * 2 / 1.0f;
		roundNum.y = ((int)(num.y) / 2) * 2 / 1.0f;
		roundNum.z = ((int)(num.z) / 2) * 2 / 1.0f;

//		Debug.Log ("OptiTrack: num is: " + num + " " + "roundNum is: " + roundNum);

		return roundNum;
	}

	public Vector3 get_handle_position ()
	{

		return handle.transform.position;

	}

	public Vector3 get_handle_angle ()
	{

		return handle.transform.rotation.eulerAngles;

	}


	void Awake ()
	{
		_instance = this;
		receivePositionData = GameController.Instance.receivePositionData;
	}


	// Use this for initialization
	void Start ()
	{


		Debug.Log ("OptiTrack: Start test.");

		allocateOptiTrackObjects ();

		handleOriginalPos = OptiTrackManager.Instance.getPosition (handleRigidbodyIndex);
		handleOriginalRot = OptiTrackManager.Instance.getOrientation (handleRigidbodyIndex);
		handleOriginalRot = handleOriginalRot * Quaternion.Euler (rotationOffset);

		handleLastPos = handleOriginalPos;

		GameObject wrenchFittingObject = GameObject.FindWithTag ("WrenchFitting");
		if (wrenchFittingObject != null) {

			wrenchFittingController = wrenchFittingObject.GetComponent<DetectCollision> ();

		} else {
			Debug.Log ("OptiTrack: can not find wrenchFittingController");
		}


		GameObject GameControllerObject = GameObject.FindWithTag ("GameController");
		if (GameControllerObject != null) {

			GameController = GameControllerObject.GetComponent<GameController> ();

		} else {
			Debug.Log ("OptiTrack: can not find GameController");
		}
			
		GameObject working_object_object = GameObject.FindWithTag ("working_object");
		initialize_coupled_position (working_object_object);
//		for (int i = 0; i < position_y.Length; i++) {
//			position_y [i] += working_object_object.transform.position.y;
//		}
//		for (int i = 0; i < position_z.Length; i++) {
//			position_z [i] += working_object_object.transform.position.z;
//		}

	}

	void initialize_coupled_position (GameObject working_object_object)
	{
		for (int i = 0; i < position_y.Length; i++) {
			position_y [i] += working_object_object.transform.position.y;
		}
		for (int i = 0; i < position_z.Length; i++) {
			position_z [i] += working_object_object.transform.position.z;
		}
	}


	// Update is called once per frame
	void Update ()
	{

//		Debug.Log ("OptiTrack Update function.");


/** Update the handle position and rotation **/
		handleCurrentPos = OptiTrackManager.Instance.getPosition (handleRigidbodyIndex);
		handleCurrentRot = OptiTrackManager.Instance.getOrientation (handleRigidbodyIndex);

		Vector3 diff_handle = (handleCurrentPos - handleOriginalPos) * smoothPos;
		Vector3 euler_handle = handleCurrentRot.eulerAngles - handleOriginalRot.eulerAngles;

//		handle.transform.position = roundToTenthDigtPos(diff);
//		handle.transform.rotation = Quaternion.Euler (roundToTenthDigtRot(euler));


/** Update the moverio position and rotation **/
		moverioCurrentPos = OptiTrackManager.Instance.getPosition (moverioRigidbodyIndex);
		moverioCurrentRot = OptiTrackManager.Instance.getOrientation (moverioRigidbodyIndex);

		Vector3 diff_moverio = (moverioCurrentPos - handleOriginalPos) * smoothPos;
		Vector3 euler_moverio = moverioCurrentRot.eulerAngles - handleOriginalRot.eulerAngles;

//		moverio.transform.position = roundToTenthDigtPos (diff_moverio);
//		moverio.transform.rotation = Quaternion.Euler (roundToTenthDigtRot (euler_moverio));

		moverio.transform.position = diff_moverio;
		moverio.transform.rotation = Quaternion.Euler (euler_moverio);


//		GameController.getCurrentNutPosition ();


//		Debug.Log ("handle: " + handle.transform.rotation);
//		Debug.Log ("handle: " + handle.transform.position);

		if (!receivePositionData) {
//			handle.transform.position = diff_handle;
//			handle.transform.rotation = Quaternion.Euler (euler_handle);
			return;
		}


		Vector3 diff = handleCurrentPos - handleLastPos;
		float error = Math.Abs (diff.x) + Math.Abs (diff.y) + Math.Abs (diff.z);

//		Debug.Log ("diff_handle: " + diff_handle);
//		Debug.Log ("diff: " + diff);
//		Debug.Log ("error: " + error);


		int subtask_index = GameController.get_subtask_index ();
//		Debug.Log ("diff_handle: " + diff_handle);


		if (wrenchFittingController.isCoupledSpecific () == subtask_index) {

			if (error < error_thres) {

				Vector3 currentNutPosition = GameController.getCurrentNutPosition ();

//				Debug.Log ("diff_handle: " + diff_handle);
//				Debug.Log ("currentNutPosition.x: " + currentNutPosition.x);

				float newX = 0.0f;
				if (diff_handle.x + handle_nut_diff < currentNutPosition.x)
					newX = diff_handle.x;
				else
					newX = currentNutPosition.x - handle_nut_diff;

//				Vector3 newPos = new Vector3 (newX, handleLastPos.y - handleOriginalPos.y,
//					                 handleLastPos.z - handleOriginalPos.z);
				Vector3 newPos = new Vector3 (newX, position_y [subtask_index], position_z [subtask_index]);

				Vector3 lastPos = newPos + handleOriginalPos;
//				new Vector3 (handleOriginalPos.x + newPos.x, handleLastPos.y, handleLastPos.z);

				
				handle.transform.position = newPos;
				handleLastPos = lastPos;

				handle.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));

			} else {
				
				handle.transform.position = diff_handle;
				handle.transform.rotation = Quaternion.Euler (euler_handle);

				handleLastPos = handleCurrentPos;

			}
		} else {
			handle.transform.position = diff_handle;
			handle.transform.rotation = Quaternion.Euler (euler_handle);

			handleLastPos = handleCurrentPos;
		}

//		Debug.Log ("moverio: " + moverioCurrentPos + " " + moverioCurrentRot);
//		Debug.Log ("handle: " + handle.transform.rotation + " " + handleCurrentRot);
//		Debug.Log (" ");
//		Debug.Log ("handle: " + handle.transform.rotation);
//		printLog();
	}

	public float returnSmoothPos ()
	{
		return smoothPos;
	}

	public float returnFieldOfView ()
	{
		return currentCamera.fieldOfView;
	}

	/** Return Handle position & rotation **/
	public Vector3 returnHandleOriginalPos ()
	{
		return handleOriginalPos;
	}

	public Vector3 returnHandleCurrentPos ()
	{
		return handleCurrentPos;
	}

	public Quaternion returnHandleOriginalRot ()
	{
		return handleOriginalRot;
	}

	public Quaternion returnHandleCurrentRot ()
	{
		return handleCurrentRot;
	}

	/** Return Moverio position & rotation **/
	//	public Vector3 returnMoverioOriginalPos()
	//	{
	//		return moverioOriginalPos;
	//	}

	public Vector3 returnMoverioCurrentPos ()
	{
		return moverioCurrentPos;
	}

	//	public Quaternion returnMoverioOriginalRot()
	//	{
	//		return moverioOriginalRot;
	//	}

	public Quaternion returnMoverioCurrentRot ()
	{
		return moverioCurrentRot;
	}



	//	String addDigit(String s)
	//	{
	//		int dotPos = s.IndexOf (".");
	//		int stringLength = s.Length;
	//
	//		int gap = precisionLength + 1 - (stringLength - dotPos);
	//
	//		for (int i = 0; i < gap; i++) {
	//			s += "0";
	//		}
	//
	//		for (int i = 0; i < precisionLengthBeforeDot + - dotPos; i++) {
	//			if (s [0] == '-') {
	//				s = s.Insert (1, "0");
	//			} else
	//				s = s.Insert (0, "0");
	//		}
	//
	//		return s;
	//	}
	//
	//	void printLog()
	//	{
	//		String log1Text;
	//		String log2Text;
	//		String log3Text;
	//		String log4Text;
	//		String log5Text;
	//		String log6Text;
	//
	//		Vector3 originalEuler 	= originalRot.eulerAngles;
	//		Vector3 currentEuler 	= currentRot.eulerAngles;
	//		Vector3 diffEuler = this.transform.rotation.eulerAngles;
	//
	//		float Pos_oldX = Mathf.Round(originalPos.x * precision) / precision;
	//		float Pos_oldY = Mathf.Round(originalPos.y * precision) / precision;
	//		float Pos_oldZ = Mathf.Round(originalPos.z * precision) / precision;
	//
	//		float Pos_newX = Mathf.Round(currentPos.x * precision) / precision;
	//		float Pos_newY = Mathf.Round(currentPos.y * precision) / precision;
	//		float Pos_newZ = Mathf.Round(currentPos.z * precision) / precision;
	//
	//		float Pos_diffX = Mathf.Round(this.transform.position.x * precision) / precision;
	//		float Pos_diffY = Mathf.Round(this.transform.position.y * precision) / precision;
	//		float Pos_diffZ = Mathf.Round(this.transform.position.z * precision) / precision;
	//
	//		float Rot_oldX = Mathf.Round(originalEuler.x * precision) / precision;
	//		float Rot_oldY = Mathf.Round(originalEuler.y * precision) / precision;
	//		float Rot_oldZ = Mathf.Round(originalEuler.z * precision) / precision;
	//
	//		float Rot_newX = Mathf.Round(currentEuler.x * precision) / precision;
	//		float Rot_newY = Mathf.Round(currentEuler.y * precision) / precision;
	//		float Rot_newZ = Mathf.Round(currentEuler.z * precision) / precision;
	//
	//		float Rot_diffX = Mathf.Round(diffEuler.x * precision) / precision;
	//		float Rot_diffY = Mathf.Round(diffEuler.y * precision) / precision;
	//		float Rot_diffZ = Mathf.Round(diffEuler.z * precision) / precision;
	//
	//
	//
	//
	//		log1Text =	"Pos_X1: " + String.Format ("{0, 10}", addDigit(Pos_oldX.ToString ())) + "\t\t ";
	//		log1Text +=	"Pos_Y1: " + String.Format ("{0, 10}", addDigit(Pos_oldY.ToString ())) + "\t\t ";
	//		log1Text +=	"Pos_Z1: " + String.Format ("{0, 10}", addDigit(Pos_oldZ.ToString ())) + "\t\t ";
	//
	//		log2Text =	"Pos_X2: " + String.Format ("{0, 10}", addDigit(Pos_newX.ToString ())) + "\t\t ";
	//		log2Text +=	"Pos_Y2: " + String.Format ("{0, 10}", addDigit(Pos_newY.ToString ())) + "\t\t ";
	//		log2Text +=	"Pos_Z2: " + String.Format ("{0, 10}", addDigit(Pos_newZ.ToString ())) + "\t\t ";
	//
	//		log3Text =	"Pos_X3: " + String.Format ("{0, 10}", addDigit(Pos_diffX.ToString ())) + "\t\t ";
	//		log3Text +=	"Pos_Y3: " + String.Format ("{0, 10}", addDigit(Pos_diffY.ToString ())) + "\t\t ";
	//		log3Text +=	"Pos_Z3: " + String.Format ("{0, 10}", addDigit(Pos_diffZ.ToString ())) + "\t\t ";
	//
	//
	//		log4Text =	"Rot_X1: " + String.Format ("{0, 10}", addDigit(Rot_oldX.ToString ())) + "\t\t ";
	//		log4Text +=	"Rot_Y1: " + String.Format ("{0, 10}", addDigit(Rot_oldY.ToString ())) + "\t\t ";
	//		log4Text +=	"Rot_Z1: " + String.Format ("{0, 10}", addDigit(Rot_oldZ.ToString ())) + "\t\t ";
	//
	//		log5Text =	"Rot_X2: " + String.Format ("{0, 10}", addDigit(Rot_newX.ToString ())) + "\t\t ";
	//		log5Text +=	"Rot_Y2: " + String.Format ("{0, 10}", addDigit(Rot_newY.ToString ())) + "\t\t ";
	//		log5Text +=	"Rot_Z2: " + String.Format ("{0, 10}", addDigit(Rot_newZ.ToString ())) + "\t\t ";
	//
	//		log6Text =	"Rot_X3: " + String.Format ("{0, 10}", addDigit(Rot_diffX.ToString ())) + "\t\t ";
	//		log6Text +=	"Rot_Y3: " + String.Format ("{0, 10}", addDigit(Rot_diffY.ToString ())) + "\t\t ";
	//		log6Text +=	"Rot_Z3: " + String.Format ("{0, 10}", addDigit(Rot_diffZ.ToString ())) + "\t\t ";
	//
	//		log1.text = log1Text;
	//		log2.text = log2Text;
	//		log3.text = log3Text;
	//		log4.text = log4Text;
	//		log5.text = log5Text;
	//		log6.text = log6Text;
	//	}
}
