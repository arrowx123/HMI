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

    Thread udpSenderThread;
    private UdpClient udpClient;
    
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
    
    // Use this for initialization
    void Start()
    {

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
        else {
            Debug.Log("UDPDataSender: genericFunctionsClassController NULL");
        }

        GameObject UDPDataReceiverControllerObject = GameObject.FindWithTag("UDPReceiver");
        if (UDPDataReceiverControllerObject != null)
        {
            UDPDataReceiverController = UDPDataReceiverControllerObject.GetComponent<UDPDataReceiver>();
        }
        else
        {
            Debug.Log("UDPDataSender: UDPDataReceiverController NULL");
        }
        
    }

    void send_UDP_to_robot(udp_data_format data_to_send)
    {

        byte[] send_buffer = getBytes(data_to_send);
        
        Debug.Log("UDPDataReceiverController.get_target_ip(): " + UDPDataReceiverController.get_target_ip());
        Debug.Log("UDPDataReceiverController.get_target_port(): " + UDPDataReceiverController.get_target_port());
        
        UDPDataReceiverController.udp_send_data(send_buffer);
        Debug.Log("send_buffer.Length: " + send_buffer.Length);
        
        Debug.LogFormat(
            "SENT: time: {0} pose: {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00} {6:0.00}",
            data_to_send.time, data_to_send.x, data_to_send.y, data_to_send.z,
            data_to_send.a, data_to_send.b, data_to_send.c
        );
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

    byte[] getBytes(double db)
    {
        int size = Marshal.SizeOf(db);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(db, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return arr;
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

    public void UdpSender()
    {
        double time = Time.realtimeSinceStartup;

        double position_x = genericFunctionsClassController.get_haptic_cursor_position().x;
        double position_y = genericFunctionsClassController.get_haptic_cursor_position().y;
        double position_z = genericFunctionsClassController.get_haptic_cursor_position().z;

        //Debug.Log(position_x + "\t" + position_y + "\t" + position_z + "\t");

        position_x = (position_x * 1000) - 1350;
        position_y = (position_y * 1000) - 850;
        //position_z *= 1000;

        //position_x = x_real_range[0] + (x_real_range[1] - x_real_range[0]) / (x_boundary[1] - x_boundary[0]) * (position_x - x_boundary[0]);
        //position_y = y_real_range[0] + (y_real_range[1] - y_real_range[0]) / (y_boundary[1] - y_boundary[0]) * (position_y - y_boundary[0]);
        //position_z = z_real_range[0] + (z_real_range[1] - z_real_range[0]) / (z_boundary[1] - z_boundary[0]) * (position_z - z_boundary[0]);

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
        
        //Debug.Log("UDPDataReceiverController.get_robot_ready (): " + UDPDataReceiverController.get_robot_ready());
        if (UDPDataReceiverController.get_robot_ready())
        {
            //double relative_time = time - UDPDataReceiverController.get_local_time() + UDPDataReceiverController.get_remote_time();
            send_UDP_to_robot(data_to_send);
        }

        //within_standard_displacement = get_standard_displacement(time, position_x, position_y, position_z);
    }

    // Update is called once per frame
    void Update()
    {
        UdpSender();
    }
}