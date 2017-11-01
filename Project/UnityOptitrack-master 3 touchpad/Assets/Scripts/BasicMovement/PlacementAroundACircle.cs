using UnityEngine;
using System.Collections;

public class PlacementAroundACircle : MonoBehaviour {

	//	parameters to be set
	private float radius;


	//internal parameters
	private Rigidbody[] rbs;
	private int numOfComponents;
	private bool inCircle = false;

	private Vector3[] startPositions;
	private Vector3[] endPositions;

	private bool calculateEndPosition = false;



	public void setRadius(float placementRadius)
	{
		radius = placementRadius;
	}


	// Use this for initialization
	void Start () {
	
		rbs = GetComponentsInChildren<Rigidbody> ();
		numOfComponents = rbs.Length;

		Debug.Log ("PlacementAroundACircle: ");
		Debug.Log(numOfComponents);

		startPositions = new Vector3[numOfComponents];
		endPositions = new Vector3[numOfComponents];
	
		Debug.Log ("start radius: " + radius);

	}



	public void placementAroundACircle(bool trigger)
	{
		if (!calculateEndPosition) {
			
			float angle = 2 * Mathf.PI / numOfComponents;

			for (int i = 0; i < numOfComponents; i++) {
				
				startPositions[i] = rbs[i].transform.localPosition;
				endPositions [i] = new Vector3 (Mathf.Sin (i * angle) * radius, 0.0f, Mathf.Cos (i * angle) * radius);


				Debug.Log ("currentName: " + rbs [i].name);
				Debug.Log ("radius: " + radius);
				Debug.Log ("angle: " + angle);

			}
			calculateEndPosition = true;
		}
			

		if (trigger) {
			if (!inCircle) {
				for (int i = 0; i < numOfComponents; i++) {
					rbs [i].transform.localPosition = endPositions [i];
				}
				inCircle = true;
			} else {
				for (int i = 0; i < numOfComponents; i++) {
					rbs [i].transform.localPosition = startPositions [i];
				}
				inCircle = false;
			}
		}
	}


	// Update is called once per frame
	void Update () {
	
	}
}
