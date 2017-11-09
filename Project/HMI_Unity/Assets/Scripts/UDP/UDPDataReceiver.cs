using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System;

public class UDPDataReceiver : MonoBehaviour
{

	Thread udpListeningThread;
	Thread udpSendingThread;

	public int portNumberReceive = 5000;
	UdpClient receivingUdpClient;

	private DateTime time;
	private double position_x;
	private double position_y;
	private double position_z;
	private double angle_a;
	private double angle_b;
	private double angle_c;
	private bool outside_workspace;
	private bool robot_ready;

	private bool initialized = false;

	public bool is_initialized ()
	{
		return initialized;
	}

	private void assign_values_from_UDP_packet (string return_data)
	{
		string[] stringSeparators = new string[] { ";" };
		string[] split = return_data.Split (stringSeparators, StringSplitOptions.None);

		time = Convert.ToDateTime (split [0]);
		position_x = Convert.ToDouble (split [1]);
		position_y = Convert.ToDouble (split [2]);
		position_z = Convert.ToDouble (split [3]);

		angle_a = Convert.ToDouble (split [4]);
		angle_b = Convert.ToDouble (split [5]);
		angle_c = Convert.ToDouble (split [6]);

		outside_workspace = Convert.ToBoolean (split [7]);
		robot_ready = Convert.ToBoolean (split [8]);

		//					Debug.Log("time: " + time);
		//					Debug.Log("position_x: " + position_x);
		//					Debug.Log("angle_a: " + angle_a);
		//					Debug.Log("outside_workspace: " + outside_workspace);
		//					Debug.Log("robot_ready: " + robot_ready);

		initialized = true;

	}

	public DateTime get_time ()
	{
		return time;
	}

	public double get_position_x ()
	{
		return position_x;
	}

	public double get_position_y ()
	{
		return position_y;
	}

	public double get_position_z ()
	{
		return position_z;
	}

	public double get_angle_a ()
	{
		return angle_a;
	}

	public double get_angle_b ()
	{
		return angle_b;
	}

	public double get_angle_c ()
	{
		return angle_c;
	}

	public Boolean get_outside_workspace ()
	{
		return outside_workspace;
	}

	public Boolean get_robot_ready ()
	{
		return robot_ready;
	}

	private void initListenerThread ()
	{

		Debug.Log ("Started on : " + portNumberReceive.ToString ());
		udpListeningThread = new Thread (new ThreadStart (UdpListener));

		// Run in background
		udpListeningThread.IsBackground = true;
		udpListeningThread.Start ();
	}

	public void UdpListener ()
	{
		receivingUdpClient = new UdpClient (portNumberReceive);

		while (true) {
			//Listening 
			try {
				IPEndPoint RemoteIpEndPoint = new IPEndPoint (IPAddress.Any, 0);
				//IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, 5000);

				// Blocks until a message returns on this socket from a remote host.
				byte[] receiveBytes = receivingUdpClient.Receive (ref RemoteIpEndPoint);

				if (receiveBytes != null) {
					string return_data = Encoding.ASCII.GetString (receiveBytes);
					Debug.Log ("Message Received: " + return_data.ToString ());
					Debug.Log ("Address IP Sender: " + RemoteIpEndPoint.Address.ToString ());
					Debug.Log ("Port Number Sender: " + RemoteIpEndPoint.Port.ToString ());

					assign_values_from_UDP_packet (return_data);

//					foreach (string s in split) {
//						Debug.Log ("s: " + s);
//					}

//					if (returnData.ToString () == "TextTest") {
//						//Do something if TextTest is received
//					}
				}
			} catch (Exception e) {
				Debug.Log (e.ToString ());
			}
		}
	}

	void Start ()
	{
		initListenerThread ();
	}
}
