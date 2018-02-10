using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Runtime.InteropServices;


struct udp_data_format
{
 public   double time; // 8 bytes
    public double x;
    public double y;
    public double z;
    public double a;
    public double b;
    public double c;
    public double outsideWorkspace;
    public double robotReady;
}

public class UDPDataSender : MonoBehaviour
{

	Socket sock;
	IPAddress serverAddr;
	IPEndPoint endPoint;
    EndPoint source_endPoint;

    //public int target_port = 9997;
    public int target_port = 6910;
    public int source_port = 6912;
    //public string ip_address = "127.0.0.1";
    //   public string ip_address = "142.157.27.42";
    //laval robot
    //public string ip_address = "132.203.102.14"
    public string ip_address = "132.206.74.144";


    public double displacement_threshold_pos = 0.001f;
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
        endPoint = new IPEndPoint(serverAddr, target_port);
        source_endPoint = new IPEndPoint(IPAddress.Any, source_port);
        sock.Bind(source_endPoint);
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

	public void send_UDP_to_robot (double time, double position_x, double position_y, double position_z)
	{
        //string text = time.ToString ("hh:mm:ss.fff") + ";" +
        //              position_x + ";" + position_y + ";" + position_z;
        string text = time + ";" + position_x + ";" + position_y + ";" + position_z;

        byte[] send_buffer = Encoding.ASCII.GetBytes (text);
		sock.SendTo (send_buffer, endPoint);
        Debug.Log("Send UDP data: " + text);
    }

	public Boolean get_within_standard_displacement ()
	{
		return within_standard_displacement;
	}

	public Boolean get_standard_displacement (double time, double position_x, double position_y, double position_z)
	{

        double rec_time = UDPDataReceiverController.get_local_time ();
		double rec_p_x = UDPDataReceiverController.get_position_x ();
		double rec_p_y = UDPDataReceiverController.get_position_y ();
		double rec_p_z = UDPDataReceiverController.get_position_z ();

		double displacement_pos = Math.Abs (rec_p_x - position_x) + Math.Abs (rec_p_y - position_y) + Math.Abs (rec_p_z - position_z);
		//double displacement_angle = Math.Abs (rec_a_a - angle_a) + Math.Abs (rec_a_b - angle_b) + Math.Abs (rec_a_c - angle_c);

		double duration = (rec_time - time);
		if (displacement_pos / duration < displacement_threshold_pos) {
			return true;
		} else
			return false;
	}

    byte[] getBytes(udp_data_format str)
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(str, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return arr;
    }

    // Update is called once per frame
    void Update ()
	{
        //Debug.Log("UDPDataSender: Upate.");
        //DateTime time = DateTime.Now;


        //Debug.Log(DateTime.Now.Ticks);

        double time = Time.realtimeSinceStartup;

  //      double position_x = optitrackController.get_handle_position ().x;
		//double position_y = optitrackController.get_handle_position ().y;
		//double position_z = optitrackController.get_handle_position ().z;
        double position_x = genericFunctionsClassController.get_haptic_cursor_position().x;
        double position_y = genericFunctionsClassController.get_haptic_cursor_position().y;
        double position_z = genericFunctionsClassController.get_haptic_cursor_position().z;

        position_x = x_real_range [0] + (x_real_range [1] - x_real_range [0]) / (x_boundary [1] - x_boundary [0]) * (position_x - x_boundary [0]);
		position_y = y_real_range [0] + (y_real_range [1] - y_real_range [0]) / (y_boundary [1] - y_boundary [0]) * (position_y - y_boundary [0]);
		position_z = z_real_range [0] + (z_real_range [1] - z_real_range [0]) / (z_boundary [1] - z_boundary [0]) * (position_z - z_boundary [0]);

        double random_double = (new System.Random()).NextDouble();
        udp_data_format tmp;
        tmp.time = random_double+0.1f;
        tmp.x = random_double + 0.2f;
        tmp.y = random_double + 0.3f;
        tmp.z = random_double + 0.4f;
        tmp.a = random_double + 0.5f;
        tmp.b = random_double + 0.6f;
        tmp.c = random_double + 0.7f;
        tmp.outsideWorkspace = random_double + 0.8f;
        tmp.robotReady = random_double + 0.9f;

        byte[] send_buffer = getBytes(tmp);
        //byte[] send_buffer = Encoding.ASCII.GetBytes(tmp.ToString());
        sock.SendTo(send_buffer, endPoint);
        Debug.Log("Send UDP data: " + tmp.ToString());
        System.Threading.Thread.Sleep(1000);

        //double angle_a = optitrackController.get_handle_angle ().x;
        //double angle_b = optitrackController.get_handle_angle ().y;
        //double angle_c = optitrackController.get_handle_angle ().z;

        //Debug.Log("UDPDataReceiverController.is_initialized (): " + UDPDataReceiverController.is_initialized());
        //Debug.Log("UDPDataReceiverController.get_robot_ready (): " + UDPDataReceiverController.get_robot_ready());
        if (UDPDataReceiverController.is_initialized () && UDPDataReceiverController.get_robot_ready())
        {
            //send_UDP_to_robot(time, position_x, position_y, position_z, angle_a, angle_b, angle_c);
            double relative_time = time - UDPDataReceiverController.get_local_time() + UDPDataReceiverController.get_remote_time();
            //send_UDP_to_robot(time, position_x, position_y, position_z);
            //Debug.Log("Sent data through UDP.");
        }

		//within_standard_displacement = get_standard_displacement (time, position_x, position_y, position_z,
			//angle_a, angle_b, angle_c);
        within_standard_displacement = get_standard_displacement(time, position_x, position_y, position_z);

        //		Debug.Log ("Time.deltaTime: " + Time.deltaTime);
    }
}