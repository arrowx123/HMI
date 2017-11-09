using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class OutputSystem : MonoBehaviour
{

	// Use this for initialization


	private OptiTrack optiTrack;


	/** Optitrack Precision Control **/
	private int precisionLength = 4;
	private int precisionLengthBeforeDot = 3;

	private float expBase = 10.0f;
	private float precision;

	/** From "OptiTrack.cs" **/
	/** Handle position & rotation information in the 3D environment **/
	private Vector3 handleOriginalPos;
	private Vector3 handleCurrentPos;
	private Quaternion handleOriginalRot;
	private Quaternion handleCurrentRot;

	private float smoothPos;
	private float fieldOfView;



	/** Moverio position & rotation information in the 3D environment **/
	//	private Vector3 moverioOriginalPos;
	private Vector3 moverioCurrentPos;
	//	private Quaternion moverioOriginalRot;
	private Quaternion moverioCurrentRot;


	// From "ObjectController.cs"
	//	public float smoothPos;
	private Vector3 controlMovement;

	public Text round_index_msg;
	public Text maximum_torque_warning;
	public Text collision_warning;
	public Text finished_signal;

	public Text indication_1;
	public Text log2;
	public Text log3;
	public Text log4;
	public Text log5;
	public Text log6;
	public Text controlParameters;
	public Text logView;


	public void display_round_index (int round_index)
	{
		round_index_msg.text = "Trial " + (char)(round_index + '0');	
	}

	public void set_maximum_torque_warning (bool _switch)
	{
		if (_switch) {
			maximum_torque_warning.text = "Maximum torque!";
		} else {
			maximum_torque_warning.text = "";
		}
	}


	public void set_completion_current_sign (bool _switch)
	{
		if (_switch) {
			maximum_torque_warning.text = "Current Nut Fixed!";
		} else {
			maximum_torque_warning.text = "";
		}
	}

	public void set_collision_warning (bool _switch)
	{
	
		if (_switch) {
			collision_warning.text = "Collision!!";
		} else {
			collision_warning.text = "";
		}
	}

	public void display_finished_signal ()
	{
	
		finished_signal.text = "You have finished this trial.";

	}
		
	public void setIndication_1 (string msg)
	{
		indication_1.text = msg;
	}


	void Start ()
	{

		precision = Mathf.Pow (expBase, precisionLength);

/** First, using "tag" to find "GameObject" **/
/**	Then, using "component" to find the script **/

		GameObject optiTrackObject = GameObject.FindWithTag ("OptiTrack");
		if (optiTrackObject != null) {
			optiTrack = optiTrackObject.GetComponent<OptiTrack> ();
		}

		if (optiTrack == null) {
			Debug.Log ("OutputSystem: Cannot find 'OptiTrack' script");
		}

		setIndication_1 ("");
	}

	void updateValues ()
	{
		// From "OptiTrack.cs"
//		handleOriginalPos = optiTrack.returnHandleOriginalPos ();
//		handleCurrentPos = optiTrack.returnHandleCurrentPos ();
//
//		handleOriginalRot = optiTrack.returnHandleOriginalRot ();
//		handleCurrentRot = optiTrack.returnHandleCurrentRot ();
//
////		moverioOriginalPos = optiTrack.returnMoverioOriginalPos ();
//		moverioCurrentPos = optiTrack.returnMoverioCurrentPos ();
//
////		moverioOriginalRot = optiTrack.returnMoverioOriginalRot ();
//		moverioCurrentRot = optiTrack.returnMoverioCurrentRot ();
//
//		smoothPos = OptiTrack.Instance.returnSmoothPos ();


		// From "ObjectController.cs"
//		controlMovement = objectController.returnObjectControl ();



	}
	
	// Update is called once per frame
	void Update ()
	{
		
		updateValues ();
		
		printLog ();
	
	}

	String addDigit (String s)
	{
		int dotPos = s.IndexOf (".");
		int stringLength = s.Length;

		int gap = precisionLength + 1 - (stringLength - dotPos);

		for (int i = 0; i < gap; i++) {
			s += "0";
		}

		for (int i = 0; i < precisionLengthBeforeDot + -dotPos; i++) {
			if (s [0] == '-') {
				s = s.Insert (1, "0");
			} else
				s = s.Insert (0, "0");
		}

		return s;
	}

	Vector3 getApproximateNumber (Vector3 variable)
	{
		float x = Mathf.Round (variable.x * precision) / precision;
		float y = Mathf.Round (variable.y * precision) / precision;
		float z = Mathf.Round (variable.z * precision) / precision;

		return new Vector3 (x, y, z);	
	}

	void printLog ()
	{
		/** display control parameters **/
//		String controlParametersText;
//		
//		
//		float rotateX = Mathf.Round(controlMovement.x * precision) / precision;
//		float rotateY = Mathf.Round(controlMovement.y * precision) / precision;
//		float rotateZ = Mathf.Round(controlMovement.z * precision) / precision;
//		
////		Debug.Log ("2: " + rotateX.ToString() + " " + rotateY.ToString() + " " + rotateZ.ToString());
//		
//		controlParametersText =		"rotateX: " + String.Format ("{0, 10}", addDigit(rotateX.ToString ())) + "\t\t ";
//		controlParametersText +=	"rotateY: " + String.Format ("{0, 10}", addDigit(rotateY.ToString ())) + "\t\t ";
//		controlParametersText +=	"rotateZ: " + String.Format ("{0, 10}", addDigit(rotateZ.ToString ())) + "\t\t ";
//		
//		controlParameters.text = controlParametersText;



		/** display the position & rotation information of Handle & Moverio **/
		handleOriginalPos = optiTrack.returnHandleOriginalPos ();
		handleCurrentPos = optiTrack.returnHandleCurrentPos ();

		handleOriginalRot = optiTrack.returnHandleOriginalRot ();
		handleCurrentRot = optiTrack.returnHandleCurrentRot ();

//		moverioOriginalPos = optiTrack.returnMoverioOriginalPos ();
		moverioCurrentPos = optiTrack.returnMoverioCurrentPos ();

//		moverioOriginalRot = optiTrack.returnMoverioOriginalRot ();
		moverioCurrentRot = optiTrack.returnMoverioCurrentRot ();

		smoothPos = optiTrack.returnSmoothPos ();



		Vector3 handleOriginalEuler = handleOriginalRot.eulerAngles;
		Vector3 handleCurrentEuler = handleCurrentRot.eulerAngles;
		Vector3 moverioCurrentEuler = moverioCurrentRot.eulerAngles;


		Vector3 handleOldPos = getApproximateNumber (handleOriginalPos);
		Vector3 handleNewPos = getApproximateNumber (handleCurrentPos);

		Vector3 handleOldRot = getApproximateNumber (handleOriginalEuler);
		Vector3 handleNewRot = getApproximateNumber (handleCurrentEuler);

		Vector3 handlePosDiff = getApproximateNumber (handleNewPos - handleOldPos) * smoothPos;
		Vector3 handleRotDiff = getApproximateNumber (handleNewRot - handleOldRot) * smoothPos;

//		Vector3 originalEuler 	= originalRot.eulerAngles;
//		Vector3 currentEuler 	= currentRot.eulerAngles;
//		Vector3 diffEuler = this.transform.rotation.eulerAngles;

//		Vector3 PosDiff = (currentPos - originalPos) * smoothPos;

//		float Pos_oldX = Mathf.Round(originalPos.x * precision) / precision;
//		float Pos_oldY = Mathf.Round(originalPos.y * precision) / precision;
//		float Pos_oldZ = Mathf.Round(originalPos.z * precision) / precision;
//
//		float Pos_newX = Mathf.Round(currentPos.x * precision) / precision;
//		float Pos_newY = Mathf.Round(currentPos.y * precision) / precision;
//		float Pos_newZ = Mathf.Round(currentPos.z * precision) / precision;

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


//		String log1Text;
//		String log2Text;
//		String log3Text;
//		String log4Text;
//		String log5Text;
//		String log6Text;
//
//
//
//		log1Text =	"Pos_X1: " + String.Format ("{0, 10}", addDigit(handleOldPosX.ToString ())) + "\t\t ";
//		log1Text +=	"Pos_Y1: " + String.Format ("{0, 10}", addDigit(handleOldPosY.ToString ())) + "\t\t ";
//		log1Text +=	"Pos_Z1: " + String.Format ("{0, 10}", addDigit(handleOldPosZ.ToString ())) + "\t\t ";
//
//		log2Text =	"Pos_X2: " + String.Format ("{0, 10}", addDigit(handleNewPosX.ToString ())) + "\t\t ";
//		log2Text +=	"Pos_Y2: " + String.Format ("{0, 10}", addDigit(handleNewPosY.ToString ())) + "\t\t ";
//		log2Text +=	"Pos_Z2: " + String.Format ("{0, 10}", addDigit(handleNewPosZ.ToString ())) + "\t\t ";
//
//		log3Text =	"Pos_X3: " + String.Format ("{0, 10}", addDigit(handlePosDiffX.ToString ())) + "\t\t ";
//		log3Text +=	"Pos_Y3: " + String.Format ("{0, 10}", addDigit(handlePosDiffY.ToString ())) + "\t\t ";
//		log3Text +=	"Pos_Z3: " + String.Format ("{0, 10}", addDigit(handlePosDiffZ.ToString ())) + "\t\t ";
//
//
//		log4Text =	"Rot_X1: " + String.Format ("{0, 10}", addDigit(handleOldRotX.ToString ())) + "\t\t ";
//		log4Text +=	"Rot_Y1: " + String.Format ("{0, 10}", addDigit(handleOldRotY.ToString ())) + "\t\t ";
//		log4Text +=	"Rot_Z1: " + String.Format ("{0, 10}", addDigit(handleOldRotZ.ToString ())) + "\t\t ";
//
//		log5Text =	"Rot_X2: " + String.Format ("{0, 10}", addDigit(handleNewRotX.ToString ())) + "\t\t ";
//		log5Text +=	"Rot_Y2: " + String.Format ("{0, 10}", addDigit(handleNewRotY.ToString ())) + "\t\t ";
//		log5Text +=	"Rot_Z2: " + String.Format ("{0, 10}", addDigit(handleNewRotZ.ToString ())) + "\t\t ";
//
//		log6Text =	"Rot_X3: " + String.Format ("{0, 10}", addDigit(handleRotDiffX.ToString ())) + "\t\t ";
//		log6Text +=	"Rot_Y3: " + String.Format ("{0, 10}", addDigit(handleRotDiffY.ToString ())) + "\t\t ";
//		log6Text +=	"Rot_Z3: " + String.Format ("{0, 10}", addDigit(handleRotDiffZ.ToString ())) + "\t\t ";
//
//


//		log1.text = log1Text;
//		log2.text = log2Text;
//		log3.text = log3Text;
//		log4.text = log4Text;
//		log5.text = log5Text;
//		log6.text = log6Text;


		String logViewTest;

		logViewTest =	"smoothPos: " + smoothPos.ToString () + "\n";
		logViewTest = logViewTest + "fieldOfView: " + fieldOfView.ToString ();

		logView.text = logViewTest;

		log2.text = "smoothPos: " + smoothPos;
	}
}
