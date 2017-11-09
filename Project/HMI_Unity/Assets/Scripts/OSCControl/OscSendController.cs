using System.Net;
using System.Collections;
using UnityEngine;
using Rug.Osc;

public class OscSendController : MonoBehaviour {

//	public parameters
	public int RemotePort = 9999;
	public string RemoteAddress = "132.206.74.137"; 


	#region Private Members

	// Sender Instance
	private OscSender m_Sender;

	#endregion

	public OscSender Sender { get { return m_Sender; } }



	public OscSendController() { }

	void Awake () { 
		
		// Log the start
		Debug.Log ("OscSendController -> Starting Osc"); 
		
		// Ensure that the sender is disconnected
		Disconnect (); 
		
		// The address to send to 
		IPAddress address = IPAddress.Parse (RemoteAddress); 
		
		// The port to send to 
		int port = RemotePort;
		
		// Create an instance of the sender
		m_Sender = new OscSender(address, 0, port);
		
		// Connect the sender
		m_Sender.Connect ();
		
		// We are now connected
		Debug.Log ("OscSendController -> Sender Connected"); 
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}	
	
	// OnDestroy is called when the object is destroyed
	public void OnDestroy () {
		Disconnect (); 
	}
	
	private void Disconnect () {
		
		// If the sender exists
		if (m_Sender != null) {
			
			// Disconnect the sender
			Debug.Log ("OscSendController -> Disconnecting Sender"); 
			
			m_Sender.Dispose (); 
			
			// Nullifiy the sender 
			m_Sender = null;
		}
	}
}
