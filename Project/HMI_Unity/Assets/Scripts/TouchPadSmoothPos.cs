using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TouchPadSmoothPos : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	private OptiTrack optiTrack;
	public float addInterval = 1.0f;

	void Start()
	{
		GameObject optiTrackObject = GameObject.FindWithTag ("OptiTrack");
		if (optiTrackObject != null) {
			optiTrack = optiTrackObject.GetComponent<OptiTrack> ();
		}
	}

	public void OnPointerDown (PointerEventData data)
	{
		
	}

	public void OnPointerUp (PointerEventData data)
	{

	}

	public void OnDrag (PointerEventData data)
	{

	}

}
