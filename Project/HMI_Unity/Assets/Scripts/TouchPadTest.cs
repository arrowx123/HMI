using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TouchPadTest : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	private SimplePositionMovement simplePositionMovement;
	private OscDataSender oscDataSender;


	public float addInterval = 1.0f;

	void Start()
	{
		GameObject drillCombinationObject = GameObject.FindWithTag ("DrillCombination");
		if (drillCombinationObject != null) {
			simplePositionMovement = drillCombinationObject.GetComponent<SimplePositionMovement> ();
		}

		GameObject OSCDataSenderObject = GameObject.FindWithTag ("OSCDataSender");
		if (OSCDataSenderObject != null) {
			oscDataSender = OSCDataSenderObject.GetComponent<OscDataSender> ();
		}

		if (oscDataSender == null) {
			Debug.Log("TouchPadTest -> OSCDataSenderObject does not exist!");
		} else {
			Debug.Log("TouchPadTest -> OSCDataSenderObject builds successfully!");
		}
	}

	public void OnPointerDown (PointerEventData data)
	{
//		simplePositionMovement.moveUp ();
		oscDataSender.send_osc_msg ("q");
	}

	public void OnPointerUp (PointerEventData data)
	{

		oscDataSender.send_osc_msg ("qq");
	}

	public void OnDrag (PointerEventData data)
	{

	}

}
