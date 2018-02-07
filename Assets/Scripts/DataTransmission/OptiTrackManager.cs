/**
 * Adapted from johny3212
 * Written by Matt Oskamp
 */
using UnityEngine;
using System;
using System.Collections;
using OptitrackManagement;

public class OptiTrackManager : MonoBehaviour
{
    public string myName = "OptitrackManager";
    public float scale = 1.0f;
    private static OptiTrackManager instance;
    private bool receive_zero = false;

    public Vector3 origin = Vector3.zero;
    // set this to wherever you want the center to be in your scene

    //	whether to receive the position data from OptiTrack
    private bool receivePositionData = false;

    public static OptiTrackManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        instance = this;

        receivePositionData = GameController.Instance.get_receivePositionData();
    }

    ~OptiTrackManager()
    {
        Debug.Log("OptitrackManager: Destruct");
        OptitrackManagement.DirectMulticastSocketClient.Close();
    }

    void Start()
    {
        Debug.Log(myName + ": Initializing");

        OptitrackManagement.DirectMulticastSocketClient.Start();
        OptitrackManagement.DirectMulticastSocketClient.Start();
        OptitrackManagement.DirectMulticastSocketClient.Start();
        Application.runInBackground = true;
    }

    public OptiTrackRigidBody getOptiTrackRigidBody(int index)
    {
        // only do this if you want the raw data
        if (OptitrackManagement.DirectMulticastSocketClient.IsInit())
        {
            DataStream networkData = OptitrackManagement.DirectMulticastSocketClient.GetDataStream();
            return networkData.getRigidbody(index);
        }
        else
        {
            OptitrackManagement.DirectMulticastSocketClient.Start();
            return getOptiTrackRigidBody(index);
        }
    }

    public Vector3 getPosition(int rigidbodyIndex)
    {
        if (receivePositionData == false)
            return Vector3.zero;

        while (true)
        {
            //			bool exit = false;
            if (OptitrackManagement.DirectMulticastSocketClient.IsInit())
            {
                DataStream networkData = OptitrackManagement.DirectMulticastSocketClient.GetDataStream();
                Vector3 pos = origin + networkData.getRigidbody(rigidbodyIndex).position * scale;
                pos.x = -pos.x; // not really sure if this is the best way to do it
                                //pos.y = pos.y; // these may change depending on your configuration and calibration
                                //pos.z = -pos.z;

                // has to get a position which does not equal zero!
                // if this program can not receive the position data, it will get stuck. 
                if (receive_zero)
                    return pos;

                if (pos != Vector3.zero)
                    return pos;
            }
            //			else {
            //				return Vector3.zero;
            //			}
        }
    }

    public Quaternion getOrientation(int rigidbodyIndex)
    {
        if (receivePositionData == false)
            return Quaternion.identity;

        // should add a way to filter it
        while (true)
        {
            if (OptitrackManagement.DirectMulticastSocketClient.IsInit())
            {
                DataStream networkData = OptitrackManagement.DirectMulticastSocketClient.GetDataStream();
                Quaternion rot = networkData.getRigidbody(rigidbodyIndex).orientation;

                // change the handedness from motive
                //rot = new Quaternion(rot.z, rot.y, rot.x, rot.w); // depending on calibration

                // Invert pitch and yaw
                Vector3 euler = rot.eulerAngles;

                // these may change depending on your calibration
                rot.eulerAngles = new Vector3(euler.x, -euler.y, -euler.z);
                //				rot.eulerAngles = new Vector3 (-euler.z, -euler.y, -euler.x);
                //				rot.eulerAngles = new Vector3 (euler.x, -euler.y, -euler.z);

                // has to get a orientation which does not equal zero!
                // if this program can not receive the orientation data, it will get stuck. 

                if (receive_zero)
                    return rot;
                if (rot != Quaternion.identity)
                    return rot;
            }
            //			else {
            //				return Quaternion.identity;
            //			}
        }
    }

    public void set_receive_zero(bool b)
    {
        receive_zero = b;
    }


    public void DeInitialize()
    {
        OptitrackManagement.DirectMulticastSocketClient.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}