﻿using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System;

public class UDPDataReceiver : MonoBehaviour
{

	Thread udpListeningThread;

	public int portNumberReceive = 5000;
	UdpClient udpClient;

    private int receive_msg_size = 72;
    private static int double_bytes_number = 8;

    private int target_port;
    private IPAddress target_ip;

    private IPEndPoint RemoteIpEndPoint;

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
    

    private void assign_values_from_UDP_packet (double[] return_data)
	{
        can_assign_value = false;
        //Debug.Log("UDPDataReceiver: assign_values_from_UDP_packet");

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
        robot_ready = true;

        //Debug.LogFormat(
        //    "RECV: time: {0} pose: {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00} {6:0.00} other: {7} {8}",
        //    remote_time, position_x, position_y, position_z,
        //    angle_a, angle_b, angle_c,
        //    outside_workspace, robot_ready
        //    );

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

    public int get_target_port()
    {
        return target_port;
    }

    public IPAddress get_target_ip()
    {
        return target_ip;
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

    public void udp_send_data(byte[] bytes) {
        udpClient.Connect(RemoteIpEndPoint);
        udpClient.Send(bytes, bytes.Length);
    }

    public void UdpListener ()
	{
        while (true)
        {
            //			//Listening 
            try
            {
                // Blocks until a message returns on this socket from a remote host.
                byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);

                //Debug.Log("receiveBytes.Length: " + receiveBytes.Length);
                //Debug.Log("Received packets from: " + RemoteIpEndPoint.Address.ToString() + ":" + RemoteIpEndPoint.Port.ToString());
                target_port = RemoteIpEndPoint.Port;
                target_ip = RemoteIpEndPoint.Address;

                RemoteIpEndPoint = new IPEndPoint(target_ip, target_port);

                if (receiveBytes != null && receiveBytes.Length == receive_msg_size)
                {
                    double[] return_data = BAToDouble(receiveBytes, receive_msg_size);

                    if (can_assign_value)
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
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        udpClient = new UdpClient(portNumberReceive);

        initListenerThread();
    }

    void OnApplicationQuit()
    {
        udpListeningThread.Abort();
        if (udpClient != null)
            udpClient.Close();
    }

}
