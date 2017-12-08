//using UnityEngine;
//using System.Collections;
//
//public class SendVibrationSignal : MonoBehaviour
//{
//
//	private OscDataSender OSCDataSenderController;
//
//	// Use this for initialization
//	void Start ()
//	{
//	
//		GameObject OSCDataSenderObject = GameObject.FindWithTag ("OSCDataSender");
//		if (OSCDataSenderObject != null) {
//			OSCDataSenderController = OSCDataSenderObject.GetComponent<OscDataSender> ();
//		} else {
//			Debug.Log ("SendVibrationSignal: can not find OscDataSender!");
//		}
//	}
//
////	public void sendCollisionVibration ()
////	{
////		OSCDataSenderController.sendCollisionVibration ();
////	}
////
////	public void sendCoupleVibration ()
////	{
////
////		OSCDataSenderController.sendCoupleVibration ();
////	}
//	
//	// Update is called once per frame
//	void Update ()
//	{
//	
//	}
//}
