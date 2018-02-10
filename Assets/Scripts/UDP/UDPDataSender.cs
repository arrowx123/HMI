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
    public double time; // 8 bytes
    public double x;
    public double y;
    public double z;
    public double a;
    public double b;
    public double c;
    //public double outsideWorkspace;
    //public double robotReady;
}

public class UDPDataSender : MonoBehaviour
{

    Socket sock;
    IPAddress serverAddr;
    IPEndPoint endPoint;
    EndPoint source_endPoint;
    
    public int target_port = 65413;
    public int port_send = 6912;
    //public string ip_address = "127.0.0.1";
    //   public string ip_address = "142.157.27.42";
    //laval robot
    //public string ip_address = "132.203.102.14"
    //public string ip_address = "132.206.74.144";
    public string target_ip = "132.203.102.138";

    public double displacement_threshold_pos = 0.001f;
    public double displacement_threshold_ang = 1.0f;

    public double[] x_boundary = { -5, 5 };
    public double[] y_boundary = { -5, 5 };
    public double[] z_boundary = { -5, 5 };

    public double[] x_real_range = { -300, 300 };
    public double[] y_real_range = { -300, 300 };
    public double[] z_real_range = { -300, 300 };

    //	private GameController gameController;
    //private OptiTrack optitrackController;
    private GenericFunctionsClass genericFunctionsClassController;

    private UDPDataReceiver UDPDataReceiverController;

    //public HapticClassScript myHapticClassScript;

    private Boolean within_standard_displacement;

    void initialize_UDP()
    {

        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
            ProtocolType.Udp);

        serverAddr = IPAddress.Parse(target_ip);
        endPoint = new IPEndPoint(serverAddr, target_port);
        source_endPoint = new IPEndPoint(IPAddress.Any, port_send);
        sock.Bind(source_endPoint);
    }

    // Use this for initialization
    void Start()
    {
        initialize_UDP();

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

        GameObject UDPDataReceiverControllerObject = GameObject.FindWithTag("UDPReceiver");
        if (UDPDataReceiverControllerObject != null)
        {
            UDPDataReceiverController = UDPDataReceiverControllerObject.GetComponent<UDPDataReceiver>();
        }
    }

    void send_UDP_to_robot(udp_data_format data_to_send)
    {

        byte[] send_buffer = getBytes(data_to_send);
        sock.SendTo(send_buffer, endPoint);
        //Debug.Log("Send UDP data.");
    }

    public Boolean get_within_standard_displacement()
    {
        return within_standard_displacement;
    }

    //public Boolean get_standard_displacement(double time, double position_x, double position_y, double position_z)
    //{

    //    double rec_time = UDPDataReceiverController.get_local_time();
    //    double rec_p_x = UDPDataReceiverController.get_position_x();
    //    double rec_p_y = UDPDataReceiverController.get_position_y();
    //    double rec_p_z = UDPDataReceiverController.get_position_z();

    //    double displacement_pos = Math.Abs(rec_p_x - position_x) + Math.Abs(rec_p_y - position_y) + Math.Abs(rec_p_z - position_z);
    //    //double displacement_angle = Math.Abs (rec_a_a - angle_a) + Math.Abs (rec_a_b - angle_b) + Math.Abs (rec_a_c - angle_c);

    //    double duration = (rec_time - time);
    //    if (displacement_pos / duration < displacement_threshold_pos)
    //    {
    //        return true;
    //    }
    //    else
    //        return false;
    //}

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
    void Update()
    {
        double time = Time.realtimeSinceStartup;

        double position_x = genericFunctionsClassController.get_haptic_cursor_position().x;
        double position_y = genericFunctionsClassController.get_haptic_cursor_position().y;
        double position_z = genericFunctionsClassController.get_haptic_cursor_position().z;

        //Debug.Log(position_x + "\t" + position_y + "\t" + position_z + "\t");

        position_x = x_real_range[0] + (x_real_range[1] - x_real_range[0]) / (x_boundary[1] - x_boundary[0]) * (position_x - x_boundary[0]);
        position_y = y_real_range[0] + (y_real_range[1] - y_real_range[0]) / (y_boundary[1] - y_boundary[0]) * (position_y - y_boundary[0]);
        position_z = z_real_range[0] + (z_real_range[1] - z_real_range[0]) / (z_boundary[1] - z_boundary[0]) * (position_z - z_boundary[0]);

        //Debug.Log(position_x + "\t" + position_y + "\t" + position_z + "\t");

        double random_double = (new System.Random()).NextDouble();
        udp_data_format data_to_send;
        data_to_send.time = UDPDataReceiverController.get_remote_time();
        data_to_send.x = position_x;
        data_to_send.y = position_y;
        data_to_send.z = position_z;
        data_to_send.a = 0.0f;
        data_to_send.b = 0.0f;
        data_to_send.c = 0.0f;

        //Debug.Log("UDPDataReceiverController.is_initialized (): " + UDPDataReceiverController.is_initialized());
        //Debug.Log("UDPDataReceiverController.get_robot_ready (): " + UDPDataReceiverController.get_robot_ready());
        if (UDPDataReceiverController.get_robot_ready())
        {
            //double relative_time = time - UDPDataReceiverController.get_local_time() + UDPDataReceiverController.get_remote_time();
            send_UDP_to_robot(data_to_send);
        }
        
        //within_standard_displacement = get_standard_displacement(time, position_x, position_y, position_z);
    }
}