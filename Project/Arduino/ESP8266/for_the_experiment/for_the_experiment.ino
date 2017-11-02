/*---------------------------------------------------------------------------------------------

  Command:
  1.  /setLedValue/       ???     /setLedValue
  2.  /setOSCIP/          ???     /setOSCIP
  3.  /setOSCRemotePort/  ???     /setOSCRemotePort
  4.  /setOSCLocalPort/   ???     /setOSCLocalPort
  5.  /setOSCDelay/       ???     /setOSCDelay
  6.  /showSetting

MOVERIO IP: 132.206.74.162
Mac     IP: 132.206.74.142
Arduino IP: 132.206.74.137

--------------------------------------------------------------------------------------------- */

#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include <OSCMessage.h>
#include <OSCBundle.h>
#include <OSCData.h>
#include <TimedAction.h>


//function definition
void set_erms();


// The parameter of the wifi network
char *ssid = "srl-mini";
char *pass = "sre_lab_mcgill";

// The list of commands for the http server
//String requests[] = {"setLedValue", "setOSCIP", "setOSCRemotePort", "setOSCLocalPort", "setOSCDelay", "showSetting"};

//old
//String osc_msg[] = {"couple", "rotate", "couple_rotate", "maximum_torque", "stop", "collide"};
//new
String osc_msg[] = {"stop", "couple", "rotate", "maximum_torque",
                    "collide_up", "collide_down", "collide_left", "collide_right"};
int osc_msg_num = 8;
int direction_collide_parameter[] = {5, 8};


char OSCIP_char[20];
char msgChar[100];
String OSCIP = "132.206.74.142"; //mac
//String OSCIP = "132.206.74.162"; //Moverio

// OSC address pattern
char addressPattern[] = "/to_unity";


//TimedAction motor_thread[] = {TimedAction(50, set_erm_0), TimedAction(50, set_erm_1), TimedAction(50, set_erm_2),
//                              TimedAction(50, set_erm_3), TimedAction(50, set_erm_4), TimedAction(50, set_erm_5)};

TimedAction motor_thread[] = {TimedAction(50, set_erms)};
int num_of_motor_thread = 1;


int pwm_value[] = {0, 255, 511, 767, 1023};


// The arrangement of GPIOs in the righttop corner of NodeMCU
//const int trigger_drillBitControl = 4;      //yellow:   " "         D2
const int trigger_drillBitControl = 16;
//const int rotationDirectionControl = 5;     //white:    "q"         D1

//const int led = 16;      //drive internal led    D0
//const int motor = 10;    //drive motor           SD2

// up down left right
const int erm_pin[] = {5, 4, 12, 14};
const int erm_size = 4;

const unsigned int OSCLocalPort = 9999; // local port to listen for OSC packets (actually not used for sending)
const unsigned int OSCRemotePort = 8888; // remote port to receive OSC
const unsigned int serverListenPort = 80;

WiFiUDP Udp;                                  // A UDP instance to let us send and receive packets over UDP
//WiFiServer server(serverListenPort);          // Create an instance of the server specify the port to listen on

int last_motor_mode = 0;
int motor_mode = 0;
int osc_delay = 25;

void motor_control(int pwm_idx, int erm_idx) {
    analogWrite(erm_pin[erm_idx], pwm_value[pwm_idx]);
}

void activate_all_erms(int pwm_idx) {
    analogWrite(erm_pin[0], pwm_value[pwm_idx]);
    analogWrite(erm_pin[1], pwm_value[pwm_idx]);
    analogWrite(erm_pin[2], pwm_value[pwm_idx]);
    analogWrite(erm_pin[3], pwm_value[pwm_idx]);
}

void activate_up_erms(int pwm_idx) {
    analogWrite(erm_pin[0], pwm_value[pwm_idx]);
    analogWrite(erm_pin[3], pwm_value[pwm_idx]);
}

void activate_down_erms(int pwm_idx) {
    analogWrite(erm_pin[1], pwm_value[pwm_idx]);
    analogWrite(erm_pin[2], pwm_value[pwm_idx]);
}

void activate_left_erms(int pwm_idx) {
    analogWrite(erm_pin[0], pwm_value[pwm_idx]);
    analogWrite(erm_pin[2], pwm_value[pwm_idx]);
}

void activate_right_erms(int pwm_idx) {
    analogWrite(erm_pin[1], pwm_value[pwm_idx]);
    analogWrite(erm_pin[3], pwm_value[pwm_idx]);
}



void set_erms() {

//    Serial.print("motor_mode: ");
//    Serial.println(motor_mode);

    static int interval = 0;

//  stop
    if (motor_mode == 0) {
        activate_all_erms(0);
    }
//  couple
    else if (motor_mode == 1) {
        if (interval == 0 || interval == 1 || interval == 2 || interval == 3 || interval == 4) {
            activate_all_erms(0);
            interval++;
        } else if (interval == 5 || interval == 6 || interval == 7) {
            activate_all_erms(3);
            interval++;
        } else if (interval == 8) {
            motor_mode = last_motor_mode;
            interval = 0;
        }
    }
//  rotate
    else if (motor_mode == 2) {
        activate_all_erms(3);
    }
//  maximum_torque
    else if (motor_mode == 3) {
        if (interval == 1) {
            activate_all_erms(0);
            interval++;
        } else if (interval == 4) {
            activate_all_erms(3);
        } else if (interval == 6) {
            interval = 0;
        }
        interval++;
    }
//  collide_up
    else if (motor_mode == 4) {
        if (interval < direction_collide_parameter[0]) {
            activate_up_erms(0);
            interval++;
        } else if (interval < direction_collide_parameter[1]) {
            activate_up_erms(4);
            interval++;
        } else {
            motor_mode = last_motor_mode;
            interval = 0;
        }
    }
//  collide_down
    else if (motor_mode == 5) {
        if (interval < direction_collide_parameter[0]) {
            activate_down_erms(0);
            interval++;
        } else if (interval < direction_collide_parameter[1]) {
            activate_down_erms(4);
            interval++;
        } else {
            motor_mode = last_motor_mode;
            interval = 0;
        }
    }
//  collide_left
    else if (motor_mode == 6) {
        if (interval < direction_collide_parameter[0]) {
            activate_left_erms(0);
            interval++;
        } else if (interval < direction_collide_parameter[1]) {
            activate_left_erms(4);
            interval++;
        } else {
            motor_mode = last_motor_mode;
            interval = 0;
        }
    }
//  collide_right
    else if (motor_mode == 7) {
        if (interval < direction_collide_parameter[0]) {
            activate_right_erms(0);
            interval++;
        } else if (interval < direction_collide_parameter[1]) {
            activate_right_erms(4);
            interval++;
        } else {
            motor_mode = last_motor_mode;
            interval = 0;
        }
    }
}

//{"couple", "rotate", "couple_rotate", "maximum_torque", "stop", "collide"};
//{"stop", "couple", "rotate", "maximum_torque","collide_up", "collide_down", "collide_left", "collide_right"};
//void set_erm_0() {
//
//    static int interval = 0;
//
//    if (motor_mode == 0) {
//        if (interval == 0 || interval == 1 || interval == 2 || interval == 3 || interval == 4) {
//            motor_control(pwmValue[0]);
//            interval++;
//        } else if (interval == 5 || interval == 6 || interval == 7) {
//            motor_control(pwmValue[1]);
//            interval++;
//        } else if (interval == 8) {
//            motor_mode = last_motor_mode;
//            interval = 0;
//        }
//    }
//}
//
//void set_erm_1() {
//    if (motor_mode == 1) {
//        motor_control(pwmValue[3]);
//    }
//}
//
//void set_erm_2() {
//    if (motor_mode == 2) {
//        motor_control(pwmValue[3]);
//    }
//}
//
//void set_erm_3() {
//    static int interval = 0;
//    if (motor_mode == 3) {
//        if (interval == 1) {
//            motor_control(pwmValue[0]);
//            interval++;
//        } else if (interval == 4) {
//            motor_control(pwmValue[4]);
//        } else if (interval == 6) {
//            interval = 0;
//        }
//        interval++;
//    }
//}
//
//void set_erm_4() {
//    if (motor_mode == 4) {
//        motor_control(pwmValue[0]);
//    }
//}
//
//void set_erm_5() {
//
//    static int interval = 0;
//
//    if (motor_mode == 5) {
//        if (interval == 0 || interval == 1 || interval == 2 || interval == 3 || interval == 4) {
//            motor_control(pwmValue[0]);
//            interval++;
//        } else if (interval == 5 || interval == 6) {
//            motor_control(pwmValue[2]);
//            interval++;
//        } else if (interval == 7) {
//            motor_mode = last_motor_mode;
//            interval = 0;
//        }
//    }
//}

void setPin() {
    pinMode(trigger_drillBitControl, INPUT_PULLUP);
    Serial.println("setPin: set trigger.");

    for (int i = 0; i < erm_size; i++) {
        pinMode(erm_pin[i], OUTPUT);
    }
    activate_all_erms(0);
    Serial.println("setPin: set the ERM output pins.");
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


//void setServer() {
//
//    // Start the server
//    server.begin();
//}

void setUDP() {
    // Start UDP
    Udp.begin(OSCLocalPort);
    Serial.println("Server & UDP started");
    Serial.print("Local port: ");
    Serial.println(Udp.localPort());
    Serial.print("Remote port: ");
    Serial.println(OSCRemotePort);
    Serial.println();

    // Set OSC IP
    OSCIP.toCharArray(OSCIP_char, OSCIP.length() + 1);

}


void sendOSCMsg(String msgString) {
    Serial.println("msg sent: " + msgString + ".");

    delay(10);
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

}


void setup() {

    setSerial();
    Serial.println("setSerial finished.\n");

    setPin();
    Serial.println("setPin finished.\n");

    setWIFI();
    Serial.println("setWIFI finished.\n");

    setUDP();
    Serial.println("setUDP finished.\n");

//    setServer();
//    Serial.println("setServer finished.\n");

}


bool buttonPressed(int buttonIndex) {

    if (!digitalRead(buttonIndex))
        return true;
    else
        return false;
}

//void motorControl(bool control) {
//    if (control) {
//        analogWrite(motor, pwmValue[airIntensity]);
//    } else {
//        analogWrite(motor, 0);
//    }
//}



/*
 * if trigger is pressed,
 * send an OSC message to Moverio / Mac
 */
bool trigger_DrillBitControlBool = false;

void sendTrigger_DrillBitControl() {
    if (buttonPressed(trigger_drillBitControl)) {
        sendOSCMsg("start_vibration");
        trigger_DrillBitControlBool = true;

//        motorControl(true);

        return;
    }
    if (trigger_DrillBitControlBool) {
        sendOSCMsg("stop_vibration");
        trigger_DrillBitControlBool = false;

//        motorControl(false);
    }
    return;
}


//String processRequest(WiFiClient &client) {
//
//    String req = client.readStringUntil('\r');
//    String output = "";
//    Serial.println("\n");
//    Serial.println("req: " + req);
//    client.flush();
//
//    // Match the request
//    if (req.indexOf(requests[0]) != -1) {
//        String currentCommand = "setLedValue";
//
//        int firstPos = req.indexOf(currentCommand);
//        int secondPos = req.indexOf(currentCommand, firstPos + 1);
//
//        String ledValue_string = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
//        ledValue = ledValue_string.toInt();
//
//
//        // Set ledValue according to the request
//        digitalWrite(led, ledValue);
//
//        Serial.println("ledValue is: " + ledValue_string);
//        output = "Set ledValue = " + ledValue_string;
//    } else if (req.indexOf(requests[1]) != -1) {
//        String currentCommand = "setOSCIP";
//
//        int firstPos = req.indexOf(currentCommand);
//        int secondPos = req.indexOf(currentCommand, firstPos + 1);
//
//        OSCIP = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
//
//        Serial.println("New OSC IP is: " + OSCIP);
//        output = "Set OSC IP to " + OSCIP;
//    } else if (req.indexOf(requests[2]) != -1) {
//        String currentCommand = "setOSCRemotePort";
//
//        int firstPos = req.indexOf(currentCommand);
//        int secondPos = req.indexOf(currentCommand, firstPos + 1);
//
//        String OSCRemotePort_string = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
//        OSCRemotePort = OSCRemotePort_string.toInt();
//
//        Serial.println("New OSC remote port is: " + OSCRemotePort);
//        output = "Set OSC remote port to " + OSCRemotePort_string;
//    } else if (req.indexOf(requests[3]) != -1) {
//        String currentCommand = "setOSCLocalPort";
//
//        int firstPos = req.indexOf(currentCommand);
//        int secondPos = req.indexOf(currentCommand, firstPos + 1);
//
//        String OSCLocalPort_string = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
//        OSCLocalPort = OSCLocalPort_string.toInt();
//
//        Serial.println("New OSC local port is: " + OSCLocalPort);
//        output = "Set OSC local port to" + OSCLocalPort_string;
//    } else if (req.indexOf(requests[4]) != -1) {
//        String currentCommand = "setOSCDelay";
//
//        int firstPos = req.indexOf(currentCommand);
//        int secondPos = req.indexOf(currentCommand, firstPos + 1);
//
//        String OSCDelay_string = req.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
//        osc_delay = OSCDelay_string.toInt();
//
//        Serial.println("New OSC delay millisecond is: " + osc_delay);
//        output = "Set OSC delay millisecond to " + OSCDelay_string;
//    } else if (req.indexOf(requests[5]) != -1) {
//
//        output = "The overall setting is:" + String("\n")
//                 + "LedValue is: " + String(ledValue) + "\n"
//                 + "OSCIP is: " + String(OSCIP) + "\n"
//                 + "OSCRemotePort is: " + String(OSCRemotePort) + "\n"
//                 + "OSCLocalPort is: " + String(OSCLocalPort) + "\n";
//
//    } else {
////    Serial.println("invalid request");
////    return;
//    }
//    return output;
//}


void OSC_send() {

    sendTrigger_DrillBitControl();

}

// Build a webserver allowing parameter setting
//void Wifi_control() {
//    // Check if a client has connected
//    WiFiClient client = server.available();
//    if (!client) {
//        return;
//    }
//
//    // Wait until the client sends some data
//    Serial.println("new client");
////  while(!client.available()){
////    delay(1);
////  }
//
//    // Process the request of the client
//    String output = processRequest(client);
//
//
//    client.flush();
//
//    // Prepare the response
////  String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nGPIO is now ";
////  s += (val)?"high":"low";
////  s += "</html>\n";
//
//    // Send the response to the client
////  client.print(s);
//    client.println(output);
//    delay(1);
//    Serial.println("Client disonnected");
//
//    // The client will actually be disconnected
//    // when the function returns and 'client' object is detroyed
//    client.stop();
//}


//{"couple", "rotate", "couple_rotate", "maximum_torque", "stop", "collide"};
//{"stop", "couple", "rotate", "maximum_torque", "collide_up", "collide_down", "collide_left", "collide_right"};

void process_received_osc(String msg) {

//    Serial.print("Receive OSC Message: ");
//    Serial.println(msg);

    if (motor_mode != 1 && motor_mode != 4 && motor_mode != 5 && motor_mode != 6 && motor_mode != 7)
        last_motor_mode = motor_mode;

    for (int i = 0; i < osc_msg_num; i++) {
        if (msg == osc_msg[i]) {
            motor_mode = i;
            break;
        }
    }

//    Serial.print("Set motor mode to ");
//    Serial.println(motor_mode);
}

// receive OSC messages and decode them
void OSC_receive() {

    int size = Udp.parsePacket();
    String _string = ".................................";
    int string_length = 0;
    int comma_pos = 0;

    if (size <= 0)
        return;

    for (int i = 0; i < size; i++) {
        unsigned int tmp = Udp.read();
        _string = String(_string + String(char(tmp)));
    }

    for (int i = 0; i < _string.length(); i++) {
        if (_string.charAt(i) == ',') {
            comma_pos = i;
            break;
        }
    }

    String msg = _string.substring(comma_pos + 2);
    process_received_osc(msg);

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

//  Control Moverio through sending OSC messages
    OSC_send();

//  Receive OSC messages
    OSC_receive();

    /*
     * The webserver allowing parameter setting
     * Use curl to change internal parameters
     */
//    Wifi_control();

    for (int i = 0; i < num_of_motor_thread; i++)
        motor_thread[i].check();

}

