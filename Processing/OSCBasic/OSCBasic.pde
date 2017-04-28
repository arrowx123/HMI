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
String addressPattern = "/led";

String targetIPAddress1 = "132.206.74.137";
//String targetIPAddress1 = "127.0.0.1";
String targetIPAddress2 = "142.157.115.29";
//String targetIPAddress2 = "192.168.43.137";


//Default ports
int listenPort = 8888;
int sendPort = 9999;

ArrayList recs;

void create_rec() {

    recs = new ArrayList();
    recs.add(new Rec(20, 20, 50, 50));
    recs.add(new Rec(80, 20, 50, 50));
    recs.add(new Rec(140, 20, 50, 50));
    recs.add( new Rec(200, 50, 20, 20) );
    //recs.add( new Rec(80, 20, 50, 20) );
    //recs.add( new Rec(20, 80, 20, 50) );
    //recs.add( new Rec(80, 50, 50, 20) );
    //recs.add( new Rec(50, 80, 20, 50) );
    //recs.add( new Rec(80, 80, 50, 50) );

}

void setup() {
    size(400, 400);
    frameRate(40);

    create_rec();

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
            if (i == 0) {
                sendOSCMessage("couple");
            } else if (i == 1) {
                sendOSCMessage("rotate");
            } else if (i == 2) {
                sendOSCMessage("couple_rotate");
            } else if (i == 3) {
                sendOSCMessage("stop");
            }
        }
    }

}


void sendOSCMessage(String message) {

    OscMessage myMessage = new OscMessage("/led");
    myMessage.add(message); /* add an int to the osc message */
    oscP5.send(myMessage, myRemoteLocation1);
    oscP5.send(myMessage, myRemoteLocation2);

    println("Message sent: " + message);

}


void keyReleased() {

    //sendOSCMessage(new String(new char[]{key, key}));

}


void keyPressed() {

    //sendOSCMessage(new String(new char[]{key}));
    sendOSCMessage(new String("couple"));

}


/* incoming osc message are forwarded to the oscEvent method. */
void oscEvent(OscMessage theOscMessage) {

    /* print the address pattern and the typetag of the received OscMessage */
    println("### received an osc message.");
    println("addrpattern: " + theOscMessage.addrPattern());

    String typeTag = theOscMessage.typetag();
    println("typetag: " + typeTag);

    println("length: " + theOscMessage.arguments().length);
    println("value: ");

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