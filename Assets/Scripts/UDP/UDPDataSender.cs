using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System;

public class UDPDataSender : MonoBehaviour
{

	Socket sock;
	IPAddress serverAddr;
	IPEndPoint endPoint;

	public int port = 9997;
	public string ip_address = "127.0.0.1";

	public double displacement_threshold_pos = 1.0f;
	public double displacement_threshold_ang = 1.0f;

	public double[] x_boundary = { -5, 5 };
	public double[] y_boundary = { -5, 5 };
	public double[] z_boundary = { -5, 5 };

	public double[] x_real_range = { -9999.9, 9999.9 };
	public double[] y_real_range = { -9999.9, 9999.9 };
	public double[] z_real_range = { -9999.9, 9999.9 };

	//	private GameController gameController;
	//private OptiTrack optitrackController;
    private GenericFunctionsClass genericFunctionsClassController;

    private UDPDataReceiver UDPDataReceiverController;

    //public HapticClassScript myHapticClassScript;

    private Boolean within_standard_displacement;

	void initialize_UDP ()
	{

		sock = new Socket (AddressFamily.InterNetwork, SocketType.Dgram,
			ProtocolType.Udp);

		serverAddr = IPAddress.Parse (ip_address);
		endPoint = new IPEndPoint (serverAddr, port);
	}

	// Use this for initialization
	void Start ()
	{
		initialize_UDP ();

//		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
//		if (gameControllerObject != null) {
//			gameController = gameControllerObject.GetComponent<GameController> ();
//		}

		//GameObject optitrackControllerObject = GameObject.FindWithTag ("OptiTrack");
		//if (optitrackControllerObject != null) {
		//	optitrackController = optitrackControllerObject.GetComponent<OptiTrack> ();
		//}

        GameObject genericFunctionsClassControllerObject = GameObject.FindWithTag("open_haptics");
        if (genericFunctionsClassControllerObject != null)
        {
            genericFunctionsClassController = genericFunctionsClassControllerObject.GetComponent<GenericFunctionsClass>();
        }

        GameObject UDPDataReceiverControllerObject = GameObject.FindWithTag ("UDPReceiver");
		if (UDPDataReceiverControllerObject != null) {
            UDPDataReceiverController = UDPDataReceiverControllerObject.GetComponent<UDPDataReceiver> ();
		}
	}

	public void send_UDP_to_robot (DateTime time, double position_x, double position_y, double position_z)
	{
		string text = time.ToString ("hh:mm:ss.fff") + ";" +
		              position_x + ";" + position_y + ";" + position_z;

		byte[] send_buffer = Encoding.ASCII.GetBytes (text);
		sock.SendTo (send_buffer, endPoint);
//		Debug.Log ("Send UDP data");
	}

	public Boolean get_within_standard_displacement ()
	{
		return within_standard_displacement;
	}

	public Boolean get_standard_displacement (DateTime time, double position_x, double position_y, double position_z)
	{

		DateTime rec_time = UDPDataReceiverController.get_time ();
		double rec_p_x = UDPDataReceiverController.get_position_x ();
		double rec_p_y = UDPDataReceiverController.get_position_y ();
		double rec_p_z = UDPDataReceiverController.get_position_z ();

		double displacement_pos = Math.Abs (rec_p_x - position_x) + Math.Abs (rec_p_y - position_y) + Math.Abs (rec_p_z - position_z);
		//double displacement_angle = Math.Abs (rec_a_a - angle_a) + Math.Abs (rec_a_b - angle_b) + Math.Abs (rec_a_c - angle_c);

		double duration = (rec_time - time).Milliseconds;
		if (displacement_pos / duration < displacement_threshold_pos) {
			return true;
		} else
			return false;
	}


	// Update is called once per frame
	void Update ()
	{
        //Debug.Log("UDPDataSender: Upate.");
		DateTime time = DateTime.Now;

  //      double position_x = optitrackController.get_handle_position ().x;
		//double position_y = optitrackController.get_handle_position ().y;
		//double position_z = optitrackController.get_handle_position ().z;
        double position_x = genericFunctionsClassController.get_haptic_cursor_position().x;
        double position_y = genericFunctionsClassController.get_haptic_cursor_position().y;
        double position_z = genericFunctionsClassController.get_haptic_cursor_position().z;

        position_x = x_real_range [0] + (x_real_range [1] - x_real_range [0]) / (x_boundary [1] - x_boundary [0]) * (position_x - x_boundary [0]);
		position_y = y_real_range [0] + (y_real_range [1] - y_real_range [0]) / (y_boundary [1] - y_boundary [0]) * (position_y - y_boundary [0]);
		position_z = z_real_range [0] + (z_real_range [1] - z_real_range [0]) / (z_boundary [1] - z_boundary [0]) * (position_z - z_boundary [0]);

        //double angle_a = optitrackController.get_handle_angle ().x;
        //double angle_b = optitrackController.get_handle_angle ().y;
        //double angle_c = optitrackController.get_handle_angle ().z;

        //Debug.Log("UDPDataReceiverController.is_initialized (): " + UDPDataReceiverController.is_initialized());
        //Debug.Log("UDPDataReceiverController.get_robot_ready (): " + UDPDataReceiverController.get_robot_ready());
        if (UDPDataReceiverController.is_initialized () && UDPDataReceiverController.get_robot_ready())
        {
            //send_UDP_to_robot(time, position_x, position_y, position_z, angle_a, angle_b, angle_c);
            send_UDP_to_robot(time, position_x, position_y, position_z);
            //Debug.Log("Sent data through UDP.");
        }

		//within_standard_displacement = get_standard_displacement (time, position_x, position_y, position_z,
			//angle_a, angle_b, angle_c);
        within_standard_displacement = get_standard_displacement(time, position_x, position_y, position_z);

        //		Debug.Log ("Time.deltaTime: " + Time.deltaTime);
    }
}