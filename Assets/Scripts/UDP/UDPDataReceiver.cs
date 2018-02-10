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

    private int receive_msg_size = 72;
    private static int double_bytes_number = 8;

	//private DateTime time;
    private double remote_time;
    private double local_time;
	private double position_x;
	private double position_y;
	private double position_z;
    private double angle_a;
    private double angle_b;
    private double angle_c;
    private bool outside_workspace;
	private bool robot_ready;
    private bool can_assign_value = true;

	//private bool initialized = false;

	//public bool is_initialized ()
	//{
	//	return initialized;
	//}

	private void assign_values_from_UDP_packet (double[] return_data)
	{
        can_assign_value = false;
        Debug.Log("UDPDataReceiver: assign_values_from_UDP_packet");

        //time = Convert.ToDateTime (split [0]);
        remote_time = return_data[0];
        //local_time = Time.realtimeSinceStartup;
        //Debug.Log("time: " + time);

        position_x = return_data[1];
        //Debug.Log("position_x: " + position_x);

        position_y = return_data[2];
        //Debug.Log("position_y: " + position_y);

        position_z = return_data[3];
        //Debug.Log("position_z: " + position_z);

        angle_a = return_data[4];
        angle_b = return_data[5];
        angle_c = return_data[6];

        outside_workspace = return_data[7] == 0 ? false : true;
        robot_ready = return_data[8] == 0 ? false : true;

        Debug.Log(  
            remote_time + "\t" +
            position_x + "\t" + position_y + "\t" + position_z + "\t" +
            angle_a + "\t" + angle_b + "\t" + angle_c + "\t" +
            outside_workspace + "\t" + robot_ready
            );

        //initialized = true;
        can_assign_value = true;
    }

    //public DateTime get_time ()
    public double get_local_time()
    {
		return local_time;
	}
    public double get_remote_time()
    {
        return remote_time;
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

    public double get_angle_a()
    {
        return angle_a;
    }

    public double get_angle_b()
    {
        return angle_b;
    }

    public double get_angle_c()
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

    private void stopListenerThread()
    {
        udpListeningThread.Abort();
    }

    public static double[] BAToDouble(byte[] bytes, int size)
    {
        int len = size / double_bytes_number;
        double[] values = new double[len];

        for(int i = 0; i < len; i ++)
        {
            int index = i * double_bytes_number;
            values[i] = BitConverter.ToDouble(bytes, index);
        }

        //for(int i = 0; i < len; i++)
        //{
        //    Debug.Log(values[i]);
        //}
        
        return values;
    }

    public void UdpListener ()
	{
		receivingUdpClient = new UdpClient (portNumberReceive);

        while (true)
        {
            //			//Listening 
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, 5000);

                // Blocks until a message returns on this socket from a remote host.
                byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                
                //Debug.Log("receiveBytes.Length: " + receiveBytes.Length);

                if (receiveBytes != null && receiveBytes.Length == receive_msg_size)
                {
                    double[] return_data = BAToDouble(receiveBytes, receive_msg_size);
                    Debug.Log("Received packets from: " + RemoteIpEndPoint.Address.ToString() + ":" + RemoteIpEndPoint.Port.ToString());

                    if(can_assign_value)
                        assign_values_from_UDP_packet(return_data);
                }
            }
            catch (Exception e)
            {
                //Debug.Log(e.ToString());
            }
        }
    }

    void Start()
    {
        initListenerThread();
    }

    void OnApplicationQuit()
    {
        udpListeningThread.Abort();
        if (receivingUdpClient != null)
            receivingUdpClient.Close();
    }

}
