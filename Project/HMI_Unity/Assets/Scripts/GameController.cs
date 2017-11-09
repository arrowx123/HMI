using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{

	//	external parameters
	public float triggerRotationSpeed = 2.0f;
	public float triggerRotationDiff = -15.0f;
	public Vector3 triggerRotationAxis = new Vector3 (0.0f, 1.0f, 0.0f);
	public Vector3 drillBitRotationAxis = new Vector3 (0.0f, 0.0f, 1.0f);

	public float airRegulatorRotationSpeed = 10.0f;
	public float airRegulatorRotationDiff = 90.0f;
	public int airIntensity = 3;
	public Vector3 airRegulatorRotationAxis = new Vector3 (0.0f, 1.0f, 0.0f);

	public float rotationTriggerMovementSpeed = 10.0f;
	public Vector3 rotationTriggerPosDiff = new Vector3 (0.0f, 5.0f, 0.0f);

	public float radius = 500.0f;

	public float changeSmoothPos = 0.01f;

	public float movementSpeed = 10.0f;
	public float rotationSpeed = 200.0f;

	public float flangeMovementSpeed = 0.5f;

	//	True for 3D, False for 2D;
	public bool moverio3DDisplay = false;
	public int moverioUnityBrightness = 20;

	//	OptiTrack;
	public bool receivePositionData = false;

	//True for local; False for world;
	public bool localReference = true;


	private float drillBitRotationSpeed;
	public float drillBitSpeedMultiplication = 1000.0f;

	public int wait_milliseconds = 250;


	//	internal parameters
	private static GameController _instance;

	public bool openOutput = false;
	public bool allowControl = true;
	public bool OSCControl = false;


	//	experiment parameters
	public bool randomize_order = true;
	public bool haptic_or_audio = true;
	private bool hapticFeedback = true;

	private SoundManager soundManagerController;
	private GameObject _2DCamera;

	private TwoAngleRotation triggerController;

	private TwoPointsMovement rotationTriggerController;
	private float drillBitRotation_direction = -1;

	private Rotation drillBitController;

	private PlacementAroundACircle drillCombinationController_placementAroundACircle;
	private SimplePositionMovement drillCombinationController_simplePositionMovement;
	private SimplePositionMovement handleController_simplePositionMovement;

	private OscDataReceiver OSCDataReceiverController;
	private OscDataSender OSCDataSenderController;

	private FixedAngleRotation airRegulatorController;

	private DetectCollision wrenchFittingController;

	private const int subtask_cnt_upperbound = 8;
	private FlangeNutState[] FlangeNutStateController = new FlangeNutState[subtask_cnt_upperbound];
	private string[] flangenut_tags = new string[subtask_cnt_upperbound] {"flangenut_0", "flangenut_1", "flangenut_2", "flangenut_3",
		"flangenut_4", "flangenut_5", "flangenut_6", "flangenut_7"
	};
	private flangeNutRotation[] flangeNut_Rotation = new flangeNutRotation[subtask_cnt_upperbound];
	private flangeNutSimplePositionMovement[] FlangeNut_simplePositionMovement = new flangeNutSimplePositionMovement[subtask_cnt_upperbound];
	private GameObject[] current_arrow_indicator = new GameObject[subtask_cnt_upperbound];
	private bool initialized = false;

	private OutputSystem OutputSystemController;

	private int[][] subtask_order_array = {
		new int[] { 0, 1, 2, 3, 4, 5, 6, 7 },
		new int[] { 2, 4, 7, 3, 5, 0, 6, 1 },
		new int[] { 2, 4, 7, 3, 5, 0, 6, 1 },
		new int[] { 3, 5, 0, 6, 1, 7, 4, 2 },
		new int[] { 4, 2, 3, 0, 6, 1, 7, 5 },
		new int[] { 7, 2, 5, 3, 0, 4, 1, 6 },
		new int[] { 1, 4, 7, 3, 0, 2, 6, 5 }
	};
	private int[] subtask_order;



	private int subtask_index = 0;
	static private int round_index = 1;

	private const int subtask_cnt = 8;
	private const int round_cnt = 2;

	private SceneParameterManager SceneParameterManagerController;
	static private System.DateTime[] time_record = new System.DateTime[2 * round_cnt];

	public int get_subtask_index ()
	{
		if (subtask_index >= subtask_order.Length) {
			return -1;
		} else {
			return subtask_order [subtask_index];
		}
	}

	void setMoverioController ()
	{
		MoverioController.Instance.SetDisplayBrightness (moverioUnityBrightness);

		if (moverio3DDisplay)
			MoverioController.Instance.SetDisplay3D (true);
		else
			MoverioController.Instance.SetDisplay3D (false);
		Debug.Log ("GameController: Set Moverio Controller.");

		//		MoverioController.Instance.MuteAudio (false);
		MoverioController.Instance.MuteAudio (false);
		MoverioController.Instance.MuteDisplay (false);

	}


	public static GameController Instance {
		get {
			if (_instance == null) {
				Debug.Log ("GameController: Please Add MoverioController Prefab To Scene!");
			}
			return _instance;
		}
	}

	void Awake ()
	{
		_instance = this;
	}
		
	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Newly start!");
		//		setMoverioController ();

		if (haptic_or_audio) {
			hapticFeedback = true;
		} else {
			hapticFeedback = false;
		}


		time_record [2 * (round_index - 1)] = System.DateTime.Now;
//		Debug.Log (System.DateTime.Now);


		Debug.Log ("round_index: " + round_index);

		OscDataReceiver.Instance.setChangeSmoothPos (changeSmoothPos);

//		GameObject drillCombinationObject = GameObject.FindWithTag ("DrillCombination");
//		if (drillCombinationObject != null) {
//			drillCombinationController_placementAroundACircle = drillCombinationObject.GetComponent<PlacementAroundACircle> ();
//			drillCombinationController_placementAroundACircle.setRadius (radius);
//
//			drillCombinationController_simplePositionMovement = drillCombinationObject.GetComponent<SimplePositionMovement> ();
//			drillCombinationController_simplePositionMovement.setReference (localReference);
//		} else {
//			if (openOutput) {
//				Debug.Log ("GameController: can not find drillCombinationController_circle");
//			}
//		}

		GameObject handleObject = GameObject.FindWithTag ("Handle");
		if (handleObject != null) {
			handleController_simplePositionMovement = handleObject.GetComponent<SimplePositionMovement> ();
			handleController_simplePositionMovement.setReference (localReference);
		} else {
			if (openOutput) {
				Debug.Log ("GameController: can not find handleController_simplePositionMovement");
			}
		}


		GameObject triggerObject = GameObject.FindWithTag ("Trigger");
		if (triggerObject != null) {
			triggerController = triggerObject.GetComponent<TwoAngleRotation> ();

			triggerController.setRotationSpeed (triggerRotationSpeed);
			triggerController.setRotationDiff (triggerRotationDiff);
			triggerController.setRotationAxis (triggerRotationAxis);
		} else {
			if (openOutput) {
				Debug.Log ("GameController: can not find triggerController");
			}
		}


		GameObject rotationTriggerObject = GameObject.FindWithTag ("RotationTrigger");
		if (rotationTriggerObject != null) {
			rotationTriggerController = rotationTriggerObject.GetComponent<TwoPointsMovement> ();

			rotationTriggerController.setMovementSpeed (rotationTriggerMovementSpeed);
			rotationTriggerController.setPosDiff (rotationTriggerPosDiff);
		} else {
			if (openOutput) {
				Debug.Log ("GameController: can not find rotationTriggerController");
			}
		}


		GameObject airRegulatorObject = GameObject.FindWithTag ("AirRegulator");
		if (airRegulatorObject != null) {
			airRegulatorController = airRegulatorObject.GetComponent<FixedAngleRotation> ();

			airRegulatorController.setRotationSpeed (airRegulatorRotationSpeed);
			airRegulatorController.setRotationDiff (airRegulatorRotationDiff);
			airRegulatorController.setRotationAxis (airRegulatorRotationAxis);
		} else {
			if (openOutput) {
				Debug.Log ("GameController: can not find airRegulatorController");
			}
		}


		GameObject drillBitObject = GameObject.FindWithTag ("DrillBit");
		if (drillBitObject != null) {

			drillBitController = drillBitObject.GetComponent<Rotation> ();
			drillBitController.setRotationAxis (drillBitRotationAxis);

		} else {
			if (openOutput) {
				Debug.Log ("GameController: can not find drillBitController");

			}
		}


		for (int i = 0; i < subtask_cnt; i++) {

			GameObject flangeNutObject = GameObject.FindWithTag (flangenut_tags [i]);
			if (flangeNutObject != null) {

				flangeNut_Rotation [i] = flangeNutObject.GetComponent<flangeNutRotation> ();
				flangeNut_Rotation [i].setRotationAxis (drillBitRotationAxis);

			} else {
				if (openOutput) {
					Debug.Log ("GameController: can not find flangeNut_Rotation " + i);
				}
			}
			if (flangeNutObject != null) {

				FlangeNut_simplePositionMovement [i] = flangeNutObject.GetComponent<flangeNutSimplePositionMovement> ();
				FlangeNut_simplePositionMovement [i].setReference (localReference);

			} else {
				if (openOutput) {
					Debug.Log ("GameController: can not find FlangeNut_simplePositionMovement " + i);
				}
			}
		}
			
			

		GameObject wrenchFittingObject = GameObject.FindWithTag ("WrenchFitting");
		if (wrenchFittingObject != null) {

			wrenchFittingController = wrenchFittingObject.GetComponent<DetectCollision> ();

		} else {
			if (openOutput) {
				Debug.Log ("GameController: can not find wrenchFittingController");
			}
		}
			

		if (OSCControl) {
		
			GameObject OSCDataReceiverObject = GameObject.FindWithTag ("OSCDataReceiver");
			if (OSCDataReceiverObject != null) {
				OSCDataReceiverController = OSCDataReceiverObject.GetComponent<OscDataReceiver> ();
			}
		}

		GameObject OSCDataSenderObject = GameObject.FindWithTag ("OSCDataSender");
		if (OSCDataSenderObject != null) {
			OSCDataSenderController = OSCDataSenderObject.GetComponent<OscDataSender> ();
		}


		for (int i = 0; i < subtask_cnt; i++) {
			
			GameObject FlangeNutStateObject = GameObject.FindWithTag ("flangenut_" + (char)(i + '0'));
			if (FlangeNutStateObject != null) {

				FlangeNutStateController [i] = FlangeNutStateObject.GetComponent<FlangeNutState> ();

			} else {
				Debug.Log ("flangeNutRotation: can not find FlangeNutStateController " + (char)(i + '0'));

			}
		}


		GameObject GameControllerObject = GameObject.FindWithTag ("GameController");
		if (GameControllerObject != null) {
			OutputSystemController = GameControllerObject.GetComponent<OutputSystem> ();
			Debug.Log ("GameController: Set OutputSystem!");
		} else {
			Debug.Log ("GameController: Can not find OutputSystem!");
		}


		for (int i = 0; i < subtask_cnt; i++) {
			string tmp = "arrow_indicator_" + (char)(i + '0');
			Debug.Log (tmp);
			current_arrow_indicator [i] = GameObject.FindWithTag (tmp);
		}


		GameObject SceneParameterManagerControllerObject = GameObject.FindWithTag ("SceneParameterManager");
		if (SceneParameterManagerControllerObject != null) {
			SceneParameterManagerController = SceneParameterManagerControllerObject.GetComponent<SceneParameterManager> ();
		} else {
			Debug.Log ("GameController: Can not find SceneParameterManager!");
		}


		GameObject soundManagerControllerObject = GameObject.FindWithTag ("SoundManager");
		if (soundManagerControllerObject != null) {
			soundManagerController = soundManagerControllerObject.GetComponent<SoundManager> ();
		} else {
			Debug.Log ("GameController: Can not find SoundManager!");
		}

		_2DCamera = GameObject.FindWithTag ("2DCamera");
//		if (cameraObject != null) {
//			_2DCamera = cameraObject.GetComponent<Camera> ();	
//		} else {
//			Debug.Log ("_2DCamer: Can not find 2DCamera!");
//		}


		if (!randomize_order) {
			subtask_order = subtask_order_array [0];
		} else {
			int rand = Random.Range (1, subtask_order_array.GetLength (0));

//			Debug.Log ("subtask_order_array.GetLength (0): " + subtask_order_array.GetLength (0));
//			Debug.Log ("rand: " + rand);

			subtask_order = subtask_order_array [rand];
		}

//		Debug.Log ("subtask_order: " + subtask_order);


	}


	// Update is called once per frame
	void Update ()
	{

		if (!initialized) {
			for (int i = 0; i < subtask_cnt; i++) {
				if (current_arrow_indicator [i] != null) {
					current_arrow_indicator [i].SetActive (false);
				}
			}

			OutputSystemController.display_round_index (round_index);
			initialized = true;
		}

		//		Do not allow control
		if (!allowControl)
			return;

		setHandleSimplePositionMovement ();

		setTrigger ();

		setDrillBitRotation (drillBitRotation_direction);

//		setRotationTrigger ();
//		setDrillCombinationControllerPlacementAroundACircle ();
//		setAirRegulartor ();
	}



	bool currentTriggerControl = false;
	bool lastTriggerControl = false;

	private void setTrigger ()
	{
		if (triggerController.inMovement () == false) {
			if (!OSCControl) {
				currentTriggerControl = Input.GetKey (KeyCode.Space);
			} else {
				currentTriggerControl = OSCDataReceiverController.getTriggerControl ();
			}
		}

		if (currentTriggerControl && !lastTriggerControl) {
//			Debug.Log ("GameController: Trigger is triggered.");

			triggerController.rotateToNextAngle ();
			lastTriggerControl = true;
		} else if (!currentTriggerControl && lastTriggerControl) {

//			Debug.Log ("GameController: Trigger is released.");

			triggerController.rotateToNextAngle ();
			lastTriggerControl = false;
		}
	}

	//	private void setTrigger()
	//	{
	//
	//		bool triggerControlDown;
	//		bool triggerControlUp;
	//
	//		triggerControlDown = Input.GetButtonDown ("TriggerControl");
	//		triggerControlUp = Input.GetButtonUp ("TriggerControl");
	//
	//		if (triggerControlDown && triggerController.returnRotationTriggerControl () == false) {
	//			triggerController.rotationTriggerTrigger ();
	//		} else if (triggerControlUp && triggerController.returnRotationTriggerControl () == false) {
	//			triggerController.rotationTriggerTrigger ();
	//		}
	//
	//	}


	int currentRotationTriggerControl = 1;
	int lastRotationTriggerControl = 1;

	private void setRotationTrigger ()
	{
		if (!OSCControl) {
			if (Input.GetKey (KeyCode.R))
				currentRotationTriggerControl = 0;
			else
				currentRotationTriggerControl = 1; 
		} else {
			if (OSCDataReceiverController.getRotationTriggerControl ())
				currentRotationTriggerControl = 0;
			else
				currentRotationTriggerControl = 1;
		}


		if (currentRotationTriggerControl != lastRotationTriggerControl) {
			if (rotationTriggerController.inMovement () == false) {
				rotationTriggerController.move ();
				if (drillBitRotation_direction == -1) {
					drillBitRotation_direction = 1;
				} else
					drillBitRotation_direction = -1;
			}
			lastRotationTriggerControl = currentRotationTriggerControl;
		}
	}

	//	private void setRotationTrigger()
	//	{
	//
	//		bool rotationTriggerControl;
	//
	//		rotationTriggerControl = Input.GetButtonDown ("RotationTriggerControl");
	//		if (rotationTriggerControl && rotationTriggerController.returnRotationTriggerControl () == false) {
	//			rotationTriggerController.rotationTriggerTrigger ();
	//			if (drillBitRotation_direction == -1) {
	//				drillBitRotation_direction = 1;
	//			} else
	//				drillBitRotation_direction = -1;
	//		} else {
	//
	//		}
	//	}

	private void updateAirIntensity ()
	{
		airIntensity = airIntensity % 4 + 1;
	}

	int currentAirRegulartorControl = -1;
	int lastAirRegulartorControl = -1;

	private void setAirRegulartor ()
	{
		if (!OSCControl) {
			if (Input.GetButton ("airRegulatorControl_0"))
				currentAirRegulartorControl = 0;
			else if (Input.GetButton ("airRegulatorControl_1"))
				currentAirRegulartorControl = 1;
			else if (Input.GetButton ("airRegulatorControl_2"))
				currentAirRegulartorControl = 2;
			else if (Input.GetButton ("airRegulatorControl_3"))
				currentAirRegulartorControl = 3;
				
		} else {
			currentAirRegulartorControl = OSCDataReceiverController.getAirRegulatorControl ();
			airIntensity = currentAirRegulartorControl;
		}


		if (currentAirRegulartorControl > lastAirRegulartorControl) {
			if (airRegulatorController.inMovement () == false) {

				airRegulatorController.rotateToNextAngle (true);
				Debug.Log ("GameController -> currentAirIntensity: " + airIntensity);
				lastAirRegulartorControl = currentAirRegulartorControl;
			}
		} else if (currentAirRegulartorControl < lastAirRegulartorControl) {
			if (airRegulatorController.inMovement () == false) {

				airRegulatorController.rotateToNextAngle (false);
				Debug.Log ("GameController -> currentAirIntensity: " + airIntensity);
				lastAirRegulartorControl = currentAirRegulartorControl;
			}
		}
	}


	public Vector3 getCurrentNutPosition ()
	{
		Vector3 position = new Vector3 (0, 0, 0);

		if (subtask_index >= subtask_order.Length)
			return position;

		GameObject currentNutObject = GameObject.FindWithTag ("flangenut_" + subtask_order [subtask_index]);

		if (currentNutObject == null) {
			Debug.Log ("GameController: can not find triggerController");
		} else {
			position = currentNutObject.transform.position;
		}

//		Debug.Log ("position: " + position);


		return position;
	
	}


	bool drillBitControl = false;
	bool coupleFeedbackSend = false;
	bool max_torque_display = false;

	IEnumerator enableCompletionSign ()
	{
		//		print (Time.time);
		//		OutputSystemController.set_completion_current_sign (true);
		yield return new WaitForSeconds (0.3f);
		//		OutputSystemController.set_completion_current_sign (false);

//		OSCDataSenderController.send_maximum_torque_message ();
		OutputSystemController.set_maximum_torque_warning (true);
		//		print (Time.time);
	}

	IEnumerator startVibration ()
	{
		//		print (Time.time);
		//		OutputSystemController.set_completion_current_sign (true);

		//		yield return new WaitForSeconds (0.35f);
		yield return new WaitForSeconds (0.0f);

		if (hapticFeedback) {
			OSCDataSenderController.send_rotate_message ();
		}
//		Debug.Log ("startVibration!");
		//		print (Time.time);
	}

	IEnumerator stopVibration ()
	{
		//		print (Time.time);
		//		OutputSystemController.set_completion_current_sign (true);
		//		yield return new WaitForSeconds (0.35f);

//		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.0f);

		if (hapticFeedback) {
			OSCDataSenderController.send_stop_message ();
		}
		//		Debug.Log ("startVibration!");
		//		print (Time.time);
	}

	//	public void stop_stop_vibration_coroutine(){
	//			StopCoroutine (stop_vibration_coroutine);
	//	}

	IEnumerator disableCompletionSign ()
	{
		//		print (Time.time);
//		OutputSystemController.set_completion_current_sign (true);
		yield return new WaitForSeconds (2);
//		OutputSystemController.set_completion_current_sign (false);

		OutputSystemController.set_maximum_torque_warning (false);
		//		print (Time.time);
	}

	private void setDrillBitRotation (float direction)
	{
		if (!OSCControl) {
			drillBitControl = Input.GetKey (KeyCode.Space);
		} else {
			drillBitControl = OSCDataReceiverController.getDrillBitControl ();
		}

		if (drillBitControl)
			drillBitRotationSpeed = 1 * direction;
		else
			drillBitRotationSpeed = 0;

//		Vector3 drillBitSpeed = new Vector3 (0.0f, 0.0f, drillBitRotationSpeed) * drillBitSpeedMultiplication * (airIntensity + 1);

		float drillBitSpeed = drillBitRotationSpeed * drillBitSpeedMultiplication * (airIntensity + 1);

//		Debug.Log ("wrenchFittingController.isCollided (): " + wrenchFittingController.isCollided ());
//		Debug.Log ("wrenchFittingController.isCoupled (): " + wrenchFittingController.isCoupled(0));

		if (drillBitSpeed == 0) {
			drillBitController.setRotationSpeed (drillBitSpeed);
		} else {
			if (wrenchFittingController.isCollided () == false) {
				drillBitController.setRotationSpeed (drillBitSpeed);
			} else if (wrenchFittingController.isCollided () == true) {
				drillBitController.setRotationSpeed (0);
			}
		}



//		for (int i = 0; i < 3; i++) {
//
//			Debug.Log ("coupled " + (char)(i + '0') + " " + wrenchFittingController.isCoupled (i));
//
//		}

		IEnumerator start_vibration_coroutine = startVibration ();
		IEnumerator stop_vibration_coroutine = stopVibration ();

		bool output_warning = false;
		bool finish_current_task = true;
		bool finish_current_task_vibration_control = true;

//		if (current_arrow_indicator [subtask_index] != null)
//			current_arrow_indicator [subtask_index].SetActive (true);

		if (current_arrow_indicator [subtask_order [subtask_index]] != null)
			current_arrow_indicator [subtask_order [subtask_index]].SetActive (true);


		if (FlangeNutStateController [subtask_order [subtask_index]].toTheMaximumPos ()
		    && wrenchFittingController.isCoupled (subtask_order [subtask_index])
/*		    && drillBitSpeed != 0)) {) {*/) {
			
			output_warning = true;
		}

		if (!FlangeNutStateController [subtask_order [subtask_index]].toTheMaximumPos () || output_warning == true) {
			finish_current_task = false;
		}

		if (output_warning) {
			if (hapticFeedback)
				OSCDataSenderController.send_maximum_torque_message ();
			StartCoroutine (enableCompletionSign ());
//			OutputSystemController.set_maximum_torque_warning (output_warning);
			max_torque_display = true;
		}

//		Debug.Log ("output_warning: " + output_warning);
//		Debug.Log ("max_torque_display: " + max_torque_display);
		if (!output_warning && max_torque_display) {
			StartCoroutine (disableCompletionSign ());
			max_torque_display = false;
		}
//		OutputSystemController.set_maximum_torque_warning (output_warning);

		if (finish_current_task) {

			current_arrow_indicator [subtask_order [subtask_index]].SetActive (false);

			subtask_index++;
//			max_torque_display = true;

//			OutputSystemController.set_completion_current_sign(true);
//			finish_current_task_vibration_control = true;
			OSCDataSenderController.send_stop_message ();
			OSCDataSenderController.send_stop_message ();
			OSCDataSenderController.send_stop_message ();

			if (subtask_index < subtask_cnt) {
				current_arrow_indicator [subtask_order [subtask_index]].SetActive (true);
			}
		}

		if (subtask_index == subtask_cnt) {
		
//			current task ends

//			OutputSystemController.display_finished_signal ();
//			System.Threading.Thread.Sleep (1000);
//			System.Threading.Thread.Sleep (wait_milliseconds);
			if (hapticFeedback) {
				OSCDataSenderController.send_stop_message ();
				OSCDataSenderController.send_stop_message ();
				OSCDataSenderController.send_stop_message ();

				soundManagerController.stop_torque ();
				soundManagerController.stop_torque ();
				soundManagerController.stop_torque ();
				soundManagerController.stop_vibration ();
				soundManagerController.stop_vibration ();
				soundManagerController.stop_vibration ();
			} else {
				soundManagerController.stop_torque ();
				soundManagerController.stop_torque ();
				soundManagerController.stop_torque ();
				soundManagerController.stop_vibration ();
				soundManagerController.stop_vibration ();
				soundManagerController.stop_vibration ();
			}


			time_record [2 * (round_index - 1) + 1] = System.DateTime.Now;

//			System.Threading.Thread.Sleep (wait_milliseconds);
			SceneManager.LoadScene (0);

			initialized = false;
			round_index++;

//			Debug.Log (subtask_cnt);
//			Debug.Log (round_index);
//			Debug.Log (subtask_index);



			if (round_index > round_cnt) {
//				Debug.Log ("time_record: " + time_record[0]);
				if (hapticFeedback) {
					OSCDataSenderController.send_stop_message ();
					OSCDataSenderController.send_stop_message ();
					OSCDataSenderController.send_stop_message ();

					soundManagerController.stop_torque ();
					soundManagerController.stop_torque ();
					soundManagerController.stop_torque ();
					soundManagerController.stop_vibration ();
					soundManagerController.stop_vibration ();
					soundManagerController.stop_vibration ();
				} else {
					soundManagerController.stop_torque ();
					soundManagerController.stop_torque ();
					soundManagerController.stop_torque ();
					soundManagerController.stop_vibration ();
					soundManagerController.stop_vibration ();
					soundManagerController.stop_vibration ();
				}

				SceneParameterManagerController.SetSceneArguments (time_record);

//				Application.LoadLevel (2);
				SceneManager.LoadScene (2);
			}
			return;
		
		}

		if (drillBitSpeed != 0) {
			
			if (wrenchFittingController.isCoupled (subtask_order [subtask_index])) {

				OSCDataSenderController.send_test_point ("test_point_4");

				flangeNut_Rotation [subtask_order [subtask_index]].setRotationSpeed (drillBitSpeed / 100);
				FlangeNut_simplePositionMovement [subtask_order [subtask_index]].moveAgainstZ (flangeMovementSpeed);

//				Debug.Log ("flangeNut_Rotation in " + subtask_index);
				if (output_warning == false) {
					if (hapticFeedback) {
						StartCoroutine (start_vibration_coroutine);
//						Debug.Log ("first startcoroutine startVibration!");

						soundManagerController.play_vibration ();

					} else {
						soundManagerController.play_vibration ();
					}
				} else {
					if (hapticFeedback) {
//						OSCDataSenderController.send_maximum_torque_message ();
						StopCoroutine (start_vibration_coroutine);
//						Debug.Log ("first stopcoroutine startVibration!");

						soundManagerController.stop_vibration ();
						soundManagerController.play_torque ();
					} else {
						soundManagerController.stop_vibration ();
						soundManagerController.play_torque ();
					}
				}

			} else {

				OSCDataSenderController.send_test_point ("test_point_5");
				flangeNut_Rotation [subtask_order [subtask_index]].setRotationSpeed (0);
				FlangeNut_simplePositionMovement [subtask_order [subtask_index]].moveAgainstZ (0);

				if (wrenchFittingController.isCollided () == false) {
					if (hapticFeedback) {
						StartCoroutine (start_vibration_coroutine);
//						Debug.Log ("second startcoroutine startVibration!");

						soundManagerController.play_vibration ();
					} else {
						soundManagerController.play_vibration ();
					}
				} else if (wrenchFittingController.isCollided () == true) {
					if (hapticFeedback) {
						StopCoroutine (start_vibration_coroutine);
//						Debug.Log ("second stopcoroutine startVibration!");

						OSCDataSenderController.send_stop_message ();
						soundManagerController.stop_torque ();
						soundManagerController.stop_vibration ();
					} else {
						soundManagerController.stop_torque ();
						soundManagerController.stop_vibration ();
					}
				}
			}

		} else {
			
			flangeNut_Rotation [subtask_order [subtask_index]].setRotationSpeed (0);
			FlangeNut_simplePositionMovement [subtask_order [subtask_index]].moveAgainstZ (0);
			if (wrenchFittingController.isCoupled (subtask_order [subtask_index])) {

				OSCDataSenderController.send_test_point ("test_point_1");
				if (!coupleFeedbackSend) {
					if (hapticFeedback) {
						OSCDataSenderController.send_couple_message ();
					}

					soundManagerController.play_couple ();
					coupleFeedbackSend = true;
				} else {
//					OSCDataSenderController.send_stop_message ();
					if (max_torque_display)
						StartCoroutine (stop_vibration_coroutine);
					soundManagerController.stop_torque ();
				}
					
			} else {
				coupleFeedbackSend = false;
			
				soundManagerController.stop_couple ();

				if (wrenchFittingController.isCollided () == true) {

					StopCoroutine (stop_vibration_coroutine);
					OSCDataSenderController.send_test_point ("test_point_2");

//					OSCDataSenderController.send_collision_warning_message ();
//					if (finish_current_task_vibration_control) {
//						if (hapticFeedback) {
//							OSCDataSenderController.send_stop_message ();
//						}
//						finish_current_task_vibration_control = false;
//					}
//					Debug.Log ("I am here!!");
//					if (hapticFeedback) {
//						OSCDataSenderController.send_stop_message ();
//					} else {
//						soundManagerController.stop_vibration ();
//						soundManagerController.stop_torque ();
//					}

				} else if (wrenchFittingController.isCollided () == false) {

					OSCDataSenderController.send_test_point ("test_point_3");
					if (hapticFeedback) {
						StopCoroutine (start_vibration_coroutine);
						OSCDataSenderController.send_stop_message ();

						soundManagerController.stop_vibration ();
						soundManagerController.stop_torque ();
					} else {
						soundManagerController.stop_vibration ();
						soundManagerController.stop_torque ();
					}
				}
			}
		}

//		if (wrenchFittingController.isCollided () == false) {
//			_2DCamera.SetActive (true);
//		} else {
//			System.Threading.Thread.Sleep (100);
//			_2DCamera.SetActive (false);
//		}
	}

	public void set2DCamera (bool _switch)
	{
		if (_switch)
			_2DCamera.SetActive (true);
		else
			_2DCamera.SetActive (false);
	}

	public void send_collide_vibration ()
	{
		if (hapticFeedback) {
			OSCDataSenderController.send_collision_message ();
			Debug.Log ("send_collide_vibration");
		}
	}


	public void send_collide_up_vibration ()
	{
		if (hapticFeedback) {
			OSCDataSenderController.send_collision_up_message ();
			Debug.Log ("send_collide_up_vibration");
		}
	}


	public void send_collide_down_vibration ()
	{
		if (hapticFeedback) {
			OSCDataSenderController.send_collision_down_message ();
			Debug.Log ("send_collide_down_vibration");
		}
	}


	public void send_collide_left_vibration ()
	{
		if (hapticFeedback) {
			OSCDataSenderController.send_collision_left_message ();
			Debug.Log ("send_collide_left_vibration");
		}
	}


	public void send_collide_right_vibration ()
	{
		if (hapticFeedback) {
			OSCDataSenderController.send_collision_right_message ();
			Debug.Log ("send_collide_right_vibration");
		}
	}


	bool currentPlacementAroundACircleControl = false;
	bool lastPlacementAroundACircleControl = false;

	private void setDrillCombinationControllerPlacementAroundACircle ()
	{
		if (!OSCControl) {
			currentPlacementAroundACircleControl = Input.GetKey (KeyCode.P);
		} else {
			currentPlacementAroundACircleControl = OSCDataReceiverController.getPlacementAroundACircleControl ();
		}


//		Debug.Log ("currentPlacementAroundACircleControl: " + currentPlacementAroundACircleControl);
//		Debug.Log ("lastPlacementAroundACircleControl: " + lastPlacementAroundACircleControl);

		if (!lastPlacementAroundACircleControl && currentPlacementAroundACircleControl) {
			drillCombinationController_placementAroundACircle.placementAroundACircle (true);
			lastPlacementAroundACircleControl = true;
		} else if (lastPlacementAroundACircleControl && !currentPlacementAroundACircleControl) {
			lastPlacementAroundACircleControl = false;
		}
	}
		
	//	private void setDrillCombinationControllerPlacementAroundACircle ()
	//	{
	//
	//		bool trigger = Input.GetKeyDown (KeyCode.Space);
	//
	//		drillCombinationController_placementAroundACircle.placementAroundACircle (trigger);
	//
	//	}

	Vector3 movement = new Vector3 (0, 0, 0);
	Vector3 rotation = new Vector3 (0, 0, 0);

	private void setHandleSimplePositionMovement ()
	{
		if (!OSCControl) {
			if (Input.GetKey (KeyCode.Q))
				movement.x = 1;
			else if (Input.GetKey (KeyCode.E))
				movement.x = -1;

			if (Input.GetKey (KeyCode.W))
				movement.y = 1;
			else if (Input.GetKey (KeyCode.S))
				movement.y = -1;

			if (Input.GetKey (KeyCode.A))
				movement.z = 1;
			else if (Input.GetKey (KeyCode.D))
				movement.z = -1;


			if (Input.GetKey (KeyCode.F1))
				rotation.x = 1;
			else if (Input.GetKey (KeyCode.F2))
				rotation.x = -1;

			if (Input.GetKey (KeyCode.F3))
				rotation.y = 1;
			else if (Input.GetKey (KeyCode.F4))
				rotation.y = -1;

			if (Input.GetKey (KeyCode.F5))
				rotation.z = 1;
			else if (Input.GetKey (KeyCode.F6))
				rotation.z = -1;
		} else {
			movement.x = OSCDataReceiverController.getMoveX ();
			movement.y = OSCDataReceiverController.getMoveY ();
			movement.z = OSCDataReceiverController.getMoveZ ();

			rotation.x = OSCDataReceiverController.getRotateX ();
			rotation.y = OSCDataReceiverController.getRotateY ();
			rotation.z = OSCDataReceiverController.getRotateZ ();
		}


		handleController_simplePositionMovement.setMovePosition (movement * movementSpeed);
		handleController_simplePositionMovement.setMoveRotation (rotation * rotationSpeed);

		movement = new Vector3 (0, 0, 0);
		rotation = new Vector3 (0, 0, 0);

	}
}