/*---------------------------------------------------------------------------------------------

  Command:
  1.  /setLedValue/       ???     /setLedValue
  2.  /setOSCIP/          ???     /setOSCIP
  3.  /setOSCRemotePort/  ???     /setOSCRemotePort
  4.  /setOSCLocalPort/   ???     /setOSCLocalPort
  5.  /showSetting

MOVERIO IP: 132.206.74.162
Mac     IP: 132.206.74.142
Arduino IP: 132.206.74.137

--------------------------------------------------------------------------------------------- */

#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include <OSCMessage.h>

// The parameter of the wifi connection
//const int size = 1010;
char *ssid = "srl-mini";
char *pass = "sre_lab_mcgill";

String requests[] = {"setLedValue", "setOSCIP",
                     "setOSCRemotePort", "setOSCLocalPort",
                     "showSetting"};


char OSCIP_char[20];
char msgChar[100];
String OSCIP = "10.0.0.101";
char addressPattern[] = "/test";

// Air Intensity Level 0 ~ 3
int airIntensityLevelNum = 4;
int airIntensity = -1;
int pwmValue[] = {255, 511, 767, 1023};

// Rotation direction
int rotationDirectionNum = 2;
int rotationDirection = -1;

// The arrangement of GPIOs in the righttop corner of NodeMCU
const int trigger_drillBitControl = 4;      //yellow:   "p"         D2
const int rotationDirectionControl = 5;     //white:    "q"         D1

const int led = 16;      //drive internal led    D0
const int motor = 10;    //drive motor           SD2


// Air regulator
const int airRegulator_0 = 0;    //D3
const int airRegulator_1 = 2;    //D4
const int airRegulator_2 = 14;   //D5
const int airRegulator_3 = 12;   //D6

int ledValue = 1;

int OSCLocalPort = 9999;
int OSCRemotePort = 8888;
int serverListenPort = 80;


WiFiUDP Udp;                                  // A UDP instance to let us send and receive packets over UDP
WiFiServer server(serverListenPort);          // Create an instance of the server specify the port to listen on

const IPAddress outIp(132, 206, 74, 142);       // remote IP of your computer
//const IPAddress outIp(10,40,10,105);        // remote IP of your computer

//const unsigned int outPort = 9999;          // remote port to receive OSC
//const unsigned int localPort = 8888;        // local port to listen for OSC packets (actually not used for sending)

bool useEAPInit = false;


void setPin() {
//  Pushbutton
    pinMode(trigger_drillBitControl, INPUT_PULLUP);
    pinMode(rotationDirectionControl, INPUT_PULLUP);
    Serial.println("setPin: set pushbuttons.");

//  Rotary Switch
    pinMode(airRegulator_0, INPUT_PULLUP);
    pinMode(airRegulator_1, INPUT_PULLUP);
    pinMode(airRegulator_2, INPUT_PULLUP);
    pinMode(airRegulator_3, INPUT_PULLUP);
    Serial.println("setPin: set the rotary switch.");

//  Output
    pinMode(led, OUTPUT);
    pinMode(motor, OUTPUT);
    Serial.println("setPin: set outputs.");

    // Turn off the led & motor
    digitalWrite(led, ledValue);
    Serial.println("setPin: set the led.");

    digitalWrite(motor, LOW);
    Serial.println("setPin: set the motor.");
}


void setWIFI() {
    delay(100);
    Serial.println();
    Serial.println();
    Serial.print("Connecting to ");
    Serial.println(ssid);

    WiFi.begin(ssid, pass);

    while (WiFi.status() != WL_CONNECTED) {
        delay(1000);
        Serial.print(".");
    }

    Serial.println("\n");
    Serial.println("WiFi connected");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
}


void setSerial() {
    Serial.begin(115200);
    Serial.println("Serial Starts.");
}


void setServer() {

    // Start the server
    server.begin();

    // Start UDP
    Udp.begin(OSCLocalPort);
    Serial.println("Server & UDP started");
    Serial.print("Local port: ");
    Serial.println(Udp.localPort());
    Serial.println();

    // Set OSC IP
    OSCIP.toCharArray(OSCIP_char, OSCIP.length() + 1);

}

void sendOSCMsg(String msgString) {
    Serial.println("msg sent: " + msgString + ".");

    // Send OSC message
    OSCMessage msg(addressPattern);


    msgString.toCharArray(msgChar, msgString.length() + 1);
    msg.add(msgChar);

    OSCIP.toCharArray(OSCIP_char, OSCIP.length() + 1);
//  Serial.print("\nOSCIP_char is: ");
//  Serial.println(OSCIP_char);

    Udp.beginPacket(OSCIP_char, OSCRemotePort);
    msg.send(Udp);
    Udp.endPacket();
    msg.empty();
    delay(100);
}

void turnOnLed() {
    ledValue = 0;
    digitalWrite(led, ledValue);
}


void turnOffLed() {
    ledValue = 1;
    digitalWrite(led, ledValue);
}


void setup() {
    turnOffLed();

    setSerial();
    Serial.println("setSerial finished.\n");

    setPin();
    Serial.println("setPin finished.\n");

    setWIFI();
    Serial.println("setWIFI finished.\n");

    setServer();
    Serial.println("setServer finished.\n");

    turnOnLed();
}


bool buttonPressed(int buttonIndex) {
    if (!digitalRead(buttonIndex))
        return true;
    else
        return false;
}

void changeAirIntensity() {
    airIntensity = (airIntensity + 1) % airIntensityLevelNum;
}

void changeAirIntensity(int level) {
    airIntensity = level;
}

void changeRotationDirection() {
    rotationDirection = (rotationDirection + 1) % rotationDirectionNum;
}

void changeRotationDirection(int level) {
    rotationDirection = level;
}

void motorControl(bool control) {
    if (control) {
        analogWrite(motor, pwmValue[airIntensity]);
    } else {
        analogWrite(motor, 0);
    }
}


bool trigger_DrillBitControlBool = false;

void sendTrigger_DrillBitControl() {
    if (buttonPressed(trigger_drillBitControl)) {
        sendOSCMsg("p");
        trigger_DrillBitControlBool = true;

        motorControl(true);

        return;
    }
    if (trigger_DrillBitControlBool) {
        sendOSCMsg("pp");
        trigger_DrillBitControlBool = false;

        motorControl(false);
    }
}


// Control rotation trigger
int lastRotationDirection = -1;

void sendrotationDirectionControl() {
    if (buttonPressed(rotationDirectionControl)) {
        changeRotationDirection(0);
    } else {
        changeRotationDirection(1);
    }
    if (lastRotationDirection != rotationDirection) {
        lastRotationDirection = rotationDirection;
        sendOSCMsg("r" + String(rotationDirection));
        Serial.printf("The current rotationDirection is: %d.\n", rotationDirection);
    }
}

// Control air intensity
int lastAirIntensity = -1;

void sendAirRegulatorControl() {
//  Serial.printf("Checking airIntensity.\n");
    if (buttonPressed(airRegulator_0)) {
        changeAirIntensity(0);
    } else if (buttonPressed(airRegulator_1)) {
        changeAirIntensity(1);
    } else if (buttonPressed(airRegulator_2)) {
        changeAirIntensity(2);
    } else if (buttonPressed(airRegulator_3)) {
        changeAirIntensity(3);
    }

    if (lastAirIntensity != airIntensity) {
        lastAirIntensity = airIntensity;
        Serial.printf("The current airIntensity is: %d.\n", airIntensity);
        sendOSCMsg("a" + String(airIntensity));
    }
}


void OSCControl() {

    sendAirRegulatorControl();
    sendrotationDirectionControl();
    sendTrigger_DrillBitControl();

}


String processRequest(WiFiClient &client) {

    String req = client.readStringUntil('\r');
    String output = "";
    Serial.println("\n");
    Serial.println("req: " + req);
    client.flush();

    // Match the request
    if (req.indexOf(requests[0]) != -1) {
        String currentCommand = "setLedValue";

        int firstPos = req.indexOf(currentCommand);
        int secondPos = req.indexOf(currentCommand, firstPos + 1);

        String ledValue_string = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
        ledValue = ledValue_string.toInt();


        // Set ledValue according to the request
        digitalWrite(led, ledValue);

        Serial.println("ledValue is: " + ledValue_string);
        output = "Set ledValue = " + ledValue_string;
    } else if (req.indexOf(requests[1]) != -1) {
        String currentCommand = "setOSCIP";

        int firstPos = req.indexOf(currentCommand);
        int secondPos = req.indexOf(currentCommand, firstPos + 1);

        OSCIP = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);

        Serial.println("New OSC IP is: " + OSCIP);
        output = "Set OSC IP to " + OSCIP;
    } else if (req.indexOf(requests[2]) != -1) {
        String currentCommand = "setOSCRemotePort";

        int firstPos = req.indexOf(currentCommand);
        int secondPos = req.indexOf(currentCommand, firstPos + 1);

        String OSCRemotePort_string = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
        OSCRemotePort = OSCRemotePort_string.toInt();

        Serial.println("New OSC remote port is: " + OSCRemotePort);
        output = "Set OSC remote port to " + OSCRemotePort_string;
    } else if (req.indexOf(requests[3]) != -1) {
        String currentCommand = "setOSCLocalPort";

        int firstPos = req.indexOf(currentCommand);
        int secondPos = req.indexOf(currentCommand, firstPos + 1);

        String OSCLocalPort_string = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
        OSCLocalPort = OSCLocalPort_string.toInt();

        Serial.println("New OSC local port is: " + OSCLocalPort);
        output = "Set OSC local port to" + OSCLocalPort_string;
    } else if (req.indexOf(requests[4]) != -1) {

        output = "The overall setting is:" + String("\n")
                 + "LedValue is: " + String(ledValue) + "\n"
                 + "OSCIP is: " + String(OSCIP) + "\n"
                 + "OSCRemotePort is: " + String(OSCRemotePort) + "\n"
                 + "OSCLocalPort is: " + String(OSCLocalPort) + "\n";

    } else {
//    Serial.println("invalid request");

//    return;
    }


    return output;

}


void loop() {

    // Confirm WIFI connection
    if (WiFi.status() != WL_CONNECTED) {
        Serial.println("Reconnecting WIFI.");
    }
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }

    // Control Moverio through OSC
    OSCControl();


    // Build a webserver allowing parameter setting
    // Check if a client has connected
    WiFiClient client = server.available();
    if (!client) {
        return;
    }

    // Wait until the client sends some data
    Serial.println("new client");
//  while(!client.available()){
//    delay(1);
//  }

    // Process the request of the client
    String output = processRequest(client);


    client.flush();

    // Prepare the response
//  String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nGPIO is now ";
//  s += (val)?"high":"low";
//  s += "</html>\n";

    // Send the response to the client
//  client.print(s);
    client.println(output);
    delay(1);
    Serial.println("Client disonnected");

    // The client will actually be disconnected
    // when the function returns and 'client' object is detroyed
    client.stop();
}

