using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rug.Osc;

public class OscDataReceiver : ReceiveOscBehaviourBase
{

	//	public parameters
	//	public Vector3 Offset = new Vector3(4f, 0, 0);



	// control parameters
	private int airRegulatorControl = 3;
	private bool rotationTriggerControl;
	private bool drillBitControl;
	private bool placementAroundACircleControl;
	private bool triggerControl;

	private int moveX;
	private int moveY;
	private int moveZ;

	private int rotateX;
	private int rotateY;
	private int rotateZ;

	private float changeSmoothPos;


	public override void setChangeSmoothPos (float newChangeSmoothPos)
	{
		changeSmoothPos = newChangeSmoothPos;
	}


	// for external scripts to retrieve parameters
	public bool getTriggerControl ()
	{
		return triggerControl;
	}

	public bool getRotationTriggerControl ()
	{
		return rotationTriggerControl;
	}

	public bool getDrillBitControl ()
	{
		return drillBitControl;
	}

	public int getAirRegulatorControl ()
	{
		return airRegulatorControl;
	}

	public bool getPlacementAroundACircleControl ()
	{
		return placementAroundACircleControl;
	}

	public int getMoveX ()
	{
		return moveX;
	}

	public int getMoveY ()
	{
		return moveY;
	}

	public int getMoveZ ()
	{
		return moveZ;
	}

	public int getRotateX ()
	{
		return rotateX;
	}

	public int getRotateY ()
	{
		return rotateY;
	}

	public int getRotateZ ()
	{
		return rotateZ;
	}





	protected override void ReceiveMessage (OscMessage message)
	{
		
//		Debug.Log("Receive position"); 
//		Debug.Log("message.Count: " + message.Count); 

		for (int i = 0; i < message.Count; i++) {
			
//			Debug.Log (message [i].GetType ());
//			Debug.Log (message [i]);


//			if (message [i].Equals ("p")) {
//				triggerControlDown
//			}

			string currentString = message [i].ToString ();
//			Debug.Log ("Receive string: " + currentString);

			if (currentString.Equals (" ")) {
				triggerControl = true;
			} else if (currentString.Equals ("  ")) {
				triggerControl = false;
			}

			if (currentString.Equals ("r")) {
				rotationTriggerControl = true;
			} else if (currentString.Equals ("rr")) {
				rotationTriggerControl = false;
			}

			if (currentString.Equals (" ")) {
				drillBitControl = true;
			} else if (currentString.Equals ("  ")) {
				drillBitControl = false;
			}

			if (currentString.Equals ("p")) {
				placementAroundACircleControl = true;
			} else if (currentString.Equals ("pp")) {
				placementAroundACircleControl = false;
			}
				

			if (currentString.Equals ("a0")) {
				airRegulatorControl = 0;
			} else if (currentString.Equals ("a1")) {
				airRegulatorControl = 1;
			} else if (currentString.Equals ("a2")) {
				airRegulatorControl = 2;
			} else if (currentString.Equals ("a3")) {
				airRegulatorControl = 3;
			}




			if (currentString.Equals ("q")) {
				moveX = 1;
			} else if (currentString.Equals ("qq")) {
				moveX = 0;
			}

			if (currentString.Equals ("e")) {
				moveX = -1;
			} else if (currentString.Equals ("ee")) {
				moveX = 0;
			}

			if (currentString.Equals ("w")) {
				moveY = 1;
			} else if (currentString.Equals ("ww")) {
				moveY = 0;
			}

			if (currentString.Equals ("s")) {
				moveY = -1;
			} else if (currentString.Equals ("ss")) {
				moveY = 0;
			}

			if (currentString.Equals ("a")) {
				moveZ = 1;
			} else if (currentString.Equals ("aa")) {
				moveZ = 0;
			}

			if (currentString.Equals ("d")) {
				moveZ = -1;
			} else if (currentString.Equals ("dd")) {
				moveZ = 0;
			}



			if (currentString.Equals ("7")) {
				rotateX = 1;
			} else if (currentString.Equals ("77")) {
				rotateX = 0;
			}

			if (currentString.Equals ("8")) {
				rotateX = -1;
			} else if (currentString.Equals ("88")) {
				rotateX = 0;
			}

			if (currentString.Equals ("9")) {
				rotateY = 1;
			} else if (currentString.Equals ("99")) {
				rotateY = 0;
			}

			if (currentString.Equals ("0")) {
				rotateY = -1;
			} else if (currentString.Equals ("00")) {
				rotateY = 0;
			}

			if (currentString.Equals ("-")) {
				rotateZ = 1;
			} else if (currentString.Equals ("--")) {
				rotateZ = 0;
			}

			if (currentString.Equals ("=")) {
				rotateZ = -1;
			} else if (currentString.Equals ("==")) {
				rotateZ = 0;
			}


			if (currentString.Equals ("[")) {
				
			} else if (currentString.Equals ("[[")) {
				OptiTrack.Instance.changeSmoothPos (changeSmoothPos);
			}
			
			if (currentString.Equals ("]")) {

			} else if (currentString.Equals ("]]")) {
				OptiTrack.Instance.changeSmoothPos (-changeSmoothPos);
			}
		}



//		if (message.Count != 3) 
//		{
//			Debug.LogError(string.Format("Unexpected argument count {0}", message.Count));  
//		
//			return; 
//		}
//
//		if (!(message[0] is float) || 
//		    !(message[1] is float) ||
//		    !(message[2] is float))
//		{
//			Debug.LogError(string.Format("Unexpected argument type"));  
//
//			return; 
//		}
//
//		// get the position from the message arguments 
//		float x = (float)message [0];
//		float y = (float)message [1];
//		float z = (float)message [2];
//
//		// assign the transform position from the x, y, z and add the offset
//		this.gameObject.transform.position = new Vector3 (x, y, z) + Offset; 
	}
}
