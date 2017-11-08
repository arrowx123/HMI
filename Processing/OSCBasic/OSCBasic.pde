import hypermedia.net.*;

//MOVERIO IP: 132.206.74.162
//Mac     IP: 132.206.74.142
//Arduino IP: 132.206.74.137

import netP5.*;
import oscP5.*;
import oscP5.*;
import netP5.*;


OscP5 oscP5;
NetAddress myRemoteLocation1;
NetAddress myRemoteLocation2;

char currentKey = ' ';
String addressPattern = "/to_unity";

//mac
//String targetIPAddress1 = "132.206.74.142";

//moverio
String targetIPAddress1 = "132.206.74.142";
//impact wrench
String targetIPAddress2 = "132.206.74.137";


//Default ports
int localPort = 9999;
int remotePort = 9998;

ArrayList recs;
String osc_msg[] = {"stop", "couple", "rotate", "maximum_torque",
                    "collide_up", "collide_down", "collide_left", "collide_right"};

String ip_to_send_UDP = "127.0.0.1";
int port_to_send_UDP = 9997;
UDP udp;
UDP udp_sender;

void create_rec() {

    recs = new ArrayList();
    recs.add(new Rec(20, 20, 50, 50));
    recs.add(new Rec(80, 20, 50, 50));
    recs.add(new Rec(140, 20, 50, 50));
    recs.add( new Rec(200, 20, 50, 50) );
    recs.add( new Rec(260, 20, 50, 50) );
    recs.add( new Rec(20, 80, 50, 50) );
    recs.add( new Rec(80, 80, 50, 50) );
    recs.add( new Rec(140, 80, 50, 50) );

}

void UDP_setup(){
  
  udp = new UDP( this, 9997 );
  //udp.log( true );     // <-- printout the connection activity
  udp.listen( true );
  
  udp_sender = new UDP( this, 9996 );

}

void setup() {
    size(400, 400);
    frameRate(40);

    create_rec();

    /* start oscP5, listening for incoming messages at port 12000 */
    oscP5 = new OscP5(this, localPort);

    /* myRemoteLocation is a NetAddress. a NetAddress takes 2 parameters,
     * an ip address and a port number. myRemoteLocation is used as parameter in
     * oscP5.send() when sending osc packets to another computer, device,
     * application. usage see below. for testing purposes the listening port
     * and the port of the remote location address are the same, hence you will
     * send messages back to this sketch.
     */
    myRemoteLocation1 = new NetAddress(targetIPAddress1, remotePort);
    myRemoteLocation2 = new NetAddress(targetIPAddress2, remotePort);
    
    UDP_setup();
}

void send_UDP(String msg){
  
    String ip       = "localhost";  // the remote IP address
    int port        = 6100;    // the destination port
    
    // formats the message for Pd
    msg = msg+";\n";
    // send the message
    udp.send( msg, ip, port );
  
}

void receive( byte[] data, String ip, int port ) {  // <-- extended handler
  
  
  // get the "real" message =
  // forget the ";\n" at the end <-- !!! only for a communication with Pd !!!
  //data = subset(data, 0, data.length-2);
  String message = new String( data );
  
  // print the result
  println( "UDP receive: \""+message+"\" from "+ip+" on port "+port );
}



void draw() {
    background(0);

    for (int i = recs.size() - 1; i >= 0; i--) {
        Rec aRec = (Rec) recs.get(i);
        aRec.draw();
    }
}


void mousePressed() {
    /* in the following different ways of creating osc messages are shown by example */

}

void mouseClicked() {

    for (int i = recs.size() - 1; i >= 0; i--) {
        Rec aRec = (Rec) recs.get(i);
        if (aRec.clickCheck(mouseX, mouseY)) {
          sendOSCMessage(osc_msg[i]);
        }
    }

}


void sendOSCMessage(String message) {

    OscMessage myMessage = new OscMessage(addressPattern);
    myMessage.add(message); /* add an int to the osc message */
    oscP5.send(myMessage, myRemoteLocation1);
    oscP5.send(myMessage, myRemoteLocation2);

    println("Message sent: " + message);

}


void keyReleased() {

    sendOSCMessage(new String(new char[]{key, key}));

}


void keyPressed() {

    //sendOSCMessage(new String(new char[]{key}));
  String msg = "";  
  
  String h = String.valueOf(hour());
  String m = String.valueOf(minute());
  String s = String.valueOf(second());
  String millis = String.valueOf(millis());
  msg += h + ":" + m + ":" + s + "." + millis + ";";
  
  double p_x = 9.35;
  double p_y = 1.23;
  double p_z = 5.34;
  
  double angle_a = 123.42;
  double angle_b = 114.32;
  double angle_c = 238.84;
  
  msg += String.valueOf(p_x) + ";" + String.valueOf(p_y) + ";" + String.valueOf(p_z) + ";" +
         String.valueOf(angle_a) + ";" + String.valueOf(angle_b) + ";" + String.valueOf(angle_c) + ";";
  
  boolean OutsideWorkspace = true;
  boolean RobotReady = false;
  msg += String.valueOf(OutsideWorkspace) + ";" + String.valueOf(RobotReady);
  
  println("msg: " + msg);
  
  udp_sender.send(msg, ip_to_send_UDP, port_to_send_UDP);

}


/* incoming osc message are forwarded to the oscEvent method. */
void oscEvent(OscMessage theOscMessage) {

    /* print the address pattern and the typetag of the received OscMessage */
    println("### received an osc message.");
    println("addrpattern: " + theOscMessage.addrPattern());

    String typeTag = theOscMessage.typetag();
    println("typetag: " + typeTag);

    println("length: " + theOscMessage.arguments().length);
    print("value: ");

    for (int i = 0; i < theOscMessage.arguments().length; i++) {

        if (typeTag.charAt(i) == 's')
            println(theOscMessage.get(i).stringValue());
        else if (typeTag.charAt(i) == 'i')
            println(theOscMessage.get(i).intValue());
        else if (typeTag.charAt(i) == 'f')
            println(theOscMessage.get(i).floatValue());
    }
    println();
}


class Rec {
    int x, y, w, h;
    color c;

    Rec(int _x, int _y, int _w, int _h) {
        x = _x;
        y = _y;
        w = _w;
        h = _h;
        c = rcolor();
    }

    void draw() {
        fill(c);
        rect(x, y, w, h);
    }

    color rcolor() {
        return color(random(255), random(255), random(255));
    }

    boolean clickCheck(int _x, int _y) {
        if (_x > x && _y > y && _x < x + w && _y < y + h) {
            return true;
        } else {
            return false;
        }
    }
}