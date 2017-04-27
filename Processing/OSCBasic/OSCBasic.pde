
import netP5.*;
import oscP5.*;

/**
 * oscP5sendreceive by andreas schlegel
 * example shows how to send and receive osc messages.
 * oscP5 website at http://www.sojamo.de/oscP5
 */
 
//MOVERIO IP: 132.206.74.162
//Mac     IP: 132.206.74.142
//Arduino IP: 132.206.74.137
 
import oscP5.*;
import netP5.*;
  
OscP5 oscP5;
NetAddress myRemoteLocation1;
NetAddress myRemoteLocation2;

char currentKey = ' ';
String addressPattern = "/led";

String targetIPAddress1 = "132.206.74.137";
//String targetIPAddress1 = "127.0.0.1";
String targetIPAddress2 = "142.157.115.29";
//String targetIPAddress2 = "192.168.43.137";


//Default ports
int listenPort = 8888;
int sendPort = 9999;


void setup() {
  size(400,400);
  frameRate(40);
  
  /* start oscP5, listening for incoming messages at port 12000 */
  oscP5 = new OscP5(this, listenPort);
  
  /* myRemoteLocation is a NetAddress. a NetAddress takes 2 parameters,
   * an ip address and a port number. myRemoteLocation is used as parameter in
   * oscP5.send() when sending osc packets to another computer, device, 
   * application. usage see below. for testing purposes the listening port
   * and the port of the remote location address are the same, hence you will
   * send messages back to this sketch.
   */
  myRemoteLocation1 = new NetAddress(targetIPAddress1, sendPort);
  myRemoteLocation2 = new NetAddress(targetIPAddress2, sendPort);
}



void draw() {
  background(0);  
}



void mousePressed() {
  /* in the following different ways of creating osc messages are shown by example */
  
}



void sendOSCMessage(String message)
{
  
  OscMessage myMessage = new OscMessage("/led");
  myMessage.add(message); /* add an int to the osc message */
  oscP5.send(myMessage, myRemoteLocation1);
  oscP5.send(myMessage, myRemoteLocation2); 
   
  println("Message sent: " + message);
  
}



void keyReleased()
{
  
  sendOSCMessage(new String(new char[]{key, key}));

}



void keyPressed()
{
  
  sendOSCMessage(new String(new char[]{key}));

}



/* incoming osc message are forwarded to the oscEvent method. */
void oscEvent(OscMessage theOscMessage) {
  
  /* print the address pattern and the typetag of the received OscMessage */
  println("### received an osc message.");
  println("addrpattern: "+theOscMessage.addrPattern());
  
  String typeTag = theOscMessage.typetag();
  println("typetag: " + typeTag);
  
  println("length: " + theOscMessage.arguments().length);
  println("value: ");
  
  for(int i = 0; i < theOscMessage.arguments().length ; i ++)
  {
    
    if(typeTag.charAt(i) == 's')
      println(theOscMessage.get(i).stringValue());
    else if(typeTag.charAt(i) == 'i')
      println(theOscMessage.get(i).intValue());
    else if(typeTag.charAt(i) == 'f')
      println(theOscMessage.get(i).floatValue());
      
  }
  
  println();
  
}