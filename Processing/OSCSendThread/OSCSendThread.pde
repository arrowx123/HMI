import netP5.*;
import oscP5.*;

/**
 * oscP5sendreceive by andreas schlegel
 * example shows how to send and receive osc messages.
 * oscP5 website at http://www.sojamo.de/oscP5
 */
 
import oscP5.*;
import netP5.*;
  
OscP5 oscP5;
NetAddress myRemoteLocation;

KeyDetection keyDetection = new KeyDetection();

char currentKey = ' ';
String addressPattern = "/test";

int listenPort = 5006;
int sendPort = 5008;

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
  myRemoteLocation = new NetAddress("127.0.0.1", sendPort);
  
  keyDetection.start();
}



void draw() {
  background(0);  
}



void mousePressed() {
  /* in the following different ways of creating osc messages are shown by example */
  
  
  //int tmpInt = 123;
  //float tmpFloat = 12.34;
  //double tmpDouble = 53.72;
  
  //OscMessage myMessage = new OscMessage("control1");
  //myMessage.add(tmpInt); /* add an int to the osc message */
  //oscP5.send(myMessage, myRemoteLocation); 
  
  //myMessage = new OscMessage("control2");
  //myMessage.add(tmpFloat);
  //oscP5.send(myMessage, myRemoteLocation); 
  
  //myMessage = new OscMessage("control3");
  //myMessage.add(tmpDouble);
  //oscP5.send(myMessage, myRemoteLocation); 
  
  /* send the message */
  //oscP5.send(myMessage, myRemoteLocation); 
  //print("sending an osc message.\n");
}



void sendOSCMessage(String message)
{
  
  OscMessage myMessage = new OscMessage(addressPattern);
  myMessage.add(message); /* add an int to the osc message */
  oscP5.send(myMessage, myRemoteLocation); 
   
  println("Message sent");
  
}



void keyReleased()
{
  
  currentKey = '0';
  
  println("released key: " + key);

}



void keyPressed()
{
  
  currentKey = key;
  
  println("pressed key: " + key);
  
  
  //int tmpInt = 123;
  //float tmpFloat = 12.34;
  //double tmpDouble = 53.72;
  
  //if (key == '1')
  //{
  //  OscMessage myMessage = new OscMessage("control1");
  //  myMessage.add(tmpInt); /* add an int to the osc message */
  //  oscP5.send(myMessage, myRemoteLocation); 
  //}
  //else if (key == '2')
  //{
  //  OscMessage myMessage = new OscMessage("control2");
  //  myMessage.add(tmpFloat);
  //  oscP5.send(myMessage, myRemoteLocation);
  //}
  //else if (key == '3')
  //{
  //  OscMessage myMessage = new OscMessage("control3");
  //  myMessage.add(tmpDouble);
  //  oscP5.send(myMessage, myRemoteLocation); 
  //}
  
  //else if (key == '4')
  //{
  //  float pos = 5.0;
    
  //  OscMessage myMessage = new OscMessage("/abc");
  //  myMessage.add(pos);
  //  myMessage.add(pos);
  //  myMessage.add(pos);
  //  oscP5.send(myMessage, myRemoteLocation); 
  //}

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



class KeyDetection extends Thread
{
  
  int prevTime = millis();
  float fr = 100;    //Framerate of the separate thread, the higher, the more artifacts
  boolean limitedFramerate = true; //Disables the frame limiting, go as fast as it can!
 
  KeyDetection()
  {
    
  }
 
  void start()
  {
    super.start();
  }
 
  void run()
  {
    while (true)
    {
      boolean runIt = false;
      
      if(limitedFramerate)
      {
        if(millis() - prevTime > 1000.0/fr)
          runIt = true;
      }
      else
        runIt = true;
      
      if(runIt)
      {
        prevTime = millis();
        
        if(currentKey != '0' && currentKey != ' ')
        {
          println("currentKey is: " + currentKey);
          
          
          sendOSCMessage(new String(new char[]{currentKey}));
          
        }
      
      }
    }
  }
 
  void quit()
  {
    interrupt();
  }
  
}