using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DetectCollision : MonoBehaviour
{

	//	private OscDataSender OSCDataSenderController;

	//	private SendVibrationSignal SendVibrationSignalController;
	private OutputSystem OutputSystemController;
	private SoundManager soundManagerController;
	private GameController gameController;
	private OscDataSender OSCDataSenderController;

	private bool coupleGeneral = false;
	private bool enable_sound = true;

	private bool[] bolt_trigger = { false, false, false, false, false, false, false, false };
	private bool[,] flange_trigger = new bool[,] {
		{ false, false, false, false, false },
		{ false, false, false, false, false },
		{ false, false, false, false, false },
		{ false, false, false, false, false },
		{ false, false, false, false, false },
		{ false, false, false, false, false },
		{ false, false, false, false, false },
		{ false, false, false, false, false }
	};
	private bool other_trigger = false;

	private bool[] coupled = { false, false, false, false, false, false, false, false };
	private bool collided = false;

	private string other_collider = "other_collider";
	private string[] bolt_collider = {
		"bolt_collider_1_0",
		"bolt_collider_1_1",
		"bolt_collider_1_2",
		"bolt_collider_1_3",
		"bolt_collider_1_4",
		"bolt_collider_1_5",
		"bolt_collider_1_6",
		"bolt_collider_1_7"
	};
	private string[,] flange_collider = new string[,] { { "flange_collider_1_0", "flange_collider_2_0", "flange_collider_3_0",
			"flange_collider_4_0", "flange_collider_5_0"
		}, { "flange_collider_1_1", "flange_collider_2_1", "flange_collider_3_1",
			"flange_collider_4_1", "flange_collider_5_1"
		}, { "flange_collider_1_2", "flange_collider_2_2", "flange_collider_3_2",
			"flange_collider_4_2", "flange_collider_5_2"
		}, { "flange_collider_1_3", "flange_collider_2_3", "flange_collider_3_3",
			"flange_collider_4_3", "flange_collider_5_3"
		}, { "flange_collider_1_4", "flange_collider_2_4", "flange_collider_3_4",
			"flange_collider_4_4", "flange_collider_5_4"
		}, { "flange_collider_1_5", "flange_collider_2_5", "flange_collider_3_5",
			"flange_collider_4_5", "flange_collider_5_5"
		}, { "flange_collider_1_6", "flange_collider_2_6", "flange_collider_3_6",
			"flange_collider_4_6", "flange_collider_5_6"
		}, { "flange_collider_1_7", "flange_collider_2_7", "flange_collider_3_7",
			"flange_collider_4_7", "flange_collider_5_7"
		}
	};

	// Use this for initialization
	void Start ()
	{

		GameObject GameControllerObject = GameObject.FindWithTag ("GameController");
//		if (GameControllerObject != null) {
//			SendVibrationSignalController = GameControllerObject.GetComponent<SendVibrationSignal> ();
//		} else {
//			Debug.Log ("DetectCollision: Can not find SendVibrationSignal!");
//		}

		if (GameControllerObject != null) {
			OutputSystemController = GameControllerObject.GetComponent<OutputSystem> ();
		} else {
			Debug.Log ("DetectCollision: Can not find OutputSystem!");
		}

//		GameObject OSCDataSenderObject = GameObject.FindWithTag ("OSCDataSender");
//		if (OSCDataSenderObject != null) {
//			OSCDataSenderController = OSCDataSenderObject.GetComponent<OscDataSender> ();
//		}

		GameObject soundManagerControllerObject = GameObject.FindWithTag ("SoundManager");
		if (soundManagerControllerObject != null) {
			soundManagerController = soundManagerControllerObject.GetComponent<SoundManager> ();
		} else {
			Debug.Log ("GameController: Can not find SoundManager!");
		}

		GameObject OSCDataSenderObject = GameObject.FindWithTag ("OSCDataSender");
		if (OSCDataSenderObject != null) {
			OSCDataSenderController = OSCDataSenderObject.GetComponent<OscDataSender> ();
		}

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if(gameControllerObject != null){
			gameController = GameControllerObject.GetComponent<GameController> ();
		}
			
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if (collided) {
//			playCollisionSound ();
//		} else {
//			stopCollisionSound ();
//		}
	}

	//	void OnCollisionEnter(Collision col)
	//	{
	//		Debug.Log ("DetectCollision: Enter." + col.gameObject.name);
	////		Destroy (col.gameObject);
	//
	////		GameControllerObject.
	//
	//		SendVibrationSignalController.sendCollisionVibration ();
	//		OutputSystemController.setIndication_1 ("In Collision!");
	//	}
	//
	//	void OnCollisionExit(Collision col){
	//		OutputSystemController.setIndication_1 ("");
	//	}

	public bool isCoupled (int index)
	{
		return coupled [index];
	}

	public bool isCollided ()
	{
		return collided;
	}

	public bool isCoupledGeneral ()
	{

		coupleGeneral = false;
		for (int i = 0; i < coupled.Length; i++) {
			if (coupled [i]) {
				coupleGeneral = true;
				break;
			}
		}

		return coupleGeneral;
	}

	public int isCoupledSpecific ()
	{
		
		int coupleIndex = -1;
		for (int i = 0; i < coupled.Length; i++) {
			if (coupled [i]) {
				coupleIndex = i;
				break;
			}
		}

		return coupleIndex;
	}

	private void playCollisionSound ()
	{

		soundManagerController.play_collision ();

	}

	private void stopCollisionSound ()
	{

		soundManagerController.stop_collision ();

	}
		
	void OnTriggerEnter (Collider other)
	{
//		Debug.Log ("enter here: " + other.gameObject.tag);

		bool oriCollided = collided;

		if (other.gameObject.tag.Contains (other_collider)) {
			other_trigger = true;
		}

		for (int i = 0; i < bolt_collider.Length; i++) {
			if (other.gameObject.tag.Contains (bolt_collider [i]))
				bolt_trigger [i] = true;
//			else
//				bolt_trigger [i] = false;
		}



		for (int i = 0; i < flange_trigger.GetLength (0); i++) {
			for (int j = 0; j < flange_trigger.GetLength (1); j++) {
				if (other.gameObject.tag.Contains (flange_collider [i, j]))
					flange_trigger [i, j] = true;
//				else
//					flange_trigger [i, j] = false;
			}
		}


		for (int i = 0; i < flange_collider.GetLength (0); i++) {

//			Debug.Log ("bolt_trigger: " + bolt_trigger [i]);
				
//			if (!bolt_trigger [i] && !flange_trigger [i, 0] && flange_trigger [i, 1] && flange_trigger [i, 2]) {

			bool judge = true;
			judge &= !bolt_trigger [i];
			for (int j = 0; j < flange_collider.GetLength (1); j++) {
				if (j == 0) {
					judge &= !flange_trigger [i, j];
				} else {
					judge &= flange_trigger [i, j];
				}
			}

			judge &= !bolt_trigger [i];

			if (judge) {
//				OutputSystemController.setIndication_1 ("Coupled " + (char)(i + '0'));

				if (i == gameController.get_subtask_index ()) {
					OutputSystemController.setIndication_1 ("Coupled ");
				} else {
					OutputSystemController.setIndication_1 ("Wrong Coupling");
				}

				OutputSystemController.set_collision_warning (false);
				coupled [i] = true;
			} else {
				coupled [i] = false;
			}
		}

		bool tmp = true;
		for (int i = 0; i < flange_collider.GetLength (0); i++) {

//			tmp &= !coupled [i];
			tmp &= !bolt_trigger [i];
			tmp &= !flange_trigger [i, 0];
			tmp &= !other_trigger;

		}
		if (!tmp) {

			OutputSystemController.setIndication_1 ("");
			OutputSystemController.set_collision_warning (true);

			collided = true;
		
		} else {
//			OutputSystemController.setIndication_1 ("");
			OutputSystemController.set_collision_warning (false);
			collided = false;
		}


		if (oriCollided != collided && collided && enable_sound) {
			playCollisionSound ();

			gameController.send_collide_vibraion ();
		}


//		if (collided) {
//			OSCDataSenderController.send_collision_warning_message ();
//		} else {
//			OSCDataSenderController.send_stop_message ();
//		}
	}


	void OnTriggerStay (Collider other)
	{
//		if (!flangenut_trigger && bolt_trigger_1 && bolt_trigger_2 && !bolt_trigger_4)
//			SendVibrationSignalController.sendCoupleVibration ();
//		else
//			SendVibrationSignalController.sendCollisionVibration ();
	
	}


	void OnTriggerExit (Collider other)
	{
//		Debug.Log ("exit here: " + other.gameObject.tag);

		bool oriCollided = collided;

		if (other.gameObject.tag.Contains (other_collider)) {
			other_trigger = false;
		}

		for (int i = 0; i < bolt_collider.Length; i++) {
			if (other.gameObject.tag.Contains (bolt_collider [i]))
				bolt_trigger [i] = false;
		}

//		Debug.Log (flange_trigger.GetLength (0));
//		Debug.Log (flange_trigger.GetLength (1));

		for (int i = 0; i < flange_trigger.GetLength (0); i++) {
			for (int j = 0; j < flange_trigger.GetLength (1); j++) {
				if (other.gameObject.tag.Contains (flange_collider [i, j]))
					flange_trigger [i, j] = false;
			}
		}


		bool hasCouple = false;
		for (int i = 0; i < flange_collider.GetLength (0); i++) {

			//			Debug.Log ("bolt_trigger: " + bolt_trigger [i]);

			//			if (!bolt_trigger [i] && !flange_trigger [i, 0] && flange_trigger [i, 1] && flange_trigger [i, 2]) {

			bool judge = true;
			for (int j = 0; j < flange_collider.GetLength (1); j++) {
				if (j == 0) {
					judge &= !flange_trigger [i, j];
				} else {
					judge &= flange_trigger [i, j];
				}
			}

			judge &= !bolt_trigger [i];

			if (judge) {
				//				OutputSystemController.setIndication_1 ("Coupled " + (char)(i + '0'));
				if (i == gameController.get_subtask_index ()) {
					OutputSystemController.setIndication_1 ("Coupled ");
				} else {
					OutputSystemController.setIndication_1 ("Wrong Coupling");
				}

				OutputSystemController.set_collision_warning (false);
				coupled [i] = true;
				hasCouple = true;
			} else {
				coupled [i] = false;
			}
		}

		bool tmp = true;
		for (int i = 0; i < flange_collider.GetLength (0); i++) {

//			tmp &= !coupled [i];
			tmp &= !bolt_trigger [i];
			tmp &= !flange_trigger [i, 0];
			tmp &= !other_trigger;

		}
		if (!tmp && !hasCouple) {

			OutputSystemController.setIndication_1 ("");
			OutputSystemController.set_collision_warning (true);

//			Debug.Log ("here1!");
//			Debug.Log ("couple: " + coupled [0]);

			collided = true;

		} else {
			if (!hasCouple)
				OutputSystemController.setIndication_1 ("");
			OutputSystemController.set_collision_warning (false);

//			Debug.Log ("here2!");

			collided = false;
		}
			
		if (oriCollided != collided && collided && enable_sound) {
			playCollisionSound ();

			gameController.send_collide_vibraion ();
		}

//		if (collided) {
//			OSCDataSenderController.send_collision_warning_message ();
//		} else {
//			OSCDataSenderController.send_stop_message ();
//		}
	}
}