using UnityEngine;
using System.Collections;

public class OscDataSender : SendOscBehaviourBase
{

	//	public parameters
	public string OscAddress = "/to_arduino";
	//	static public string[] requests = { "couple", "rotate", "collide", "maximum_torque", "stop" };
	static public string[] requests = {
		"stop",
		"couple",
		"rotate",
		"maximum_torque",
		"collide_up",
		"collide_down",
		"collide_left",
		"collide_right",
		"collide"
	};
		


	private Vector3 m_LastPosition;



	// Use this for initialization
	public override void Start ()
	{
		m_LastPosition = this.gameObject.transform.position; 
	}
	
	// Update is called once per frame
	public override void Update ()
	{	

		Vector3 pos = this.gameObject.transform.position; 

		// only send if the position has changed
		if (m_LastPosition != pos) {
			Debug.Log ("Send position"); 

//			Debug.Log ("OscAddress: " + OscAddress);

			Send (new Rug.Osc.OscMessage (OscAddress, pos.x, pos.y, pos.z));
//			Send(new Rug.Osc.OscMessage(pos.x, pos.y, pos.z));
		}

		m_LastPosition = pos; 
	}


	public void send_couple_message ()
	{
		send_osc_msg (requests [1]);
	}

	public void  send_rotate_message ()
	{
		send_osc_msg (requests [2]);
	}

	public void send_collision_message ()
	{
		send_osc_msg (requests [8]);
	}

	public void send_collision_up_message ()
	{
		send_osc_msg (requests [4]);
	}

	public void send_collision_down_message ()
	{
		send_osc_msg (requests [5]);
	}

	public void send_collision_left_message ()
	{
		send_osc_msg (requests [6]);
	}

	public void send_collision_right_message ()
	{
		send_osc_msg (requests [7]);
	}

	public void  send_maximum_torque_message ()
	{
		send_osc_msg (requests [3]);
	}

	public void  send_stop_message ()
	{
		send_osc_msg (requests [0]);
	}

	public void send_osc_msg (string msg)
	{
		Send (new Rug.Osc.OscMessage (OscAddress, msg));
//		Debug.Log ("Send OSC msg:" + msg);
	}

	public void send_test_point (string msg)
	{
		Send (new Rug.Osc.OscMessage (OscAddress, msg));
	}
}
