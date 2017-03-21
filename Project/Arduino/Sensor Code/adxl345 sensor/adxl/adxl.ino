/* This is the code used to receive data from ADXL345
*/

/*---------------------------------------------------------------------------------------------

  Open Sound Control (OSC) library for the ESP8266
--------------------------------------------------------------------------------------------- */
#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include <OSCMessage.h>
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_ADXL345_U.h>

/* Assign a unique ID to this sensor at the same time */
Adafruit_ADXL345_Unified accel = Adafruit_ADXL345_Unified(12345);

char ssid[] = "Connectify-richie";          // your network SSID (name)
char pass[] = "helloworld";                    // your network password

WiFiUDP Udp;                                // A UDP instance to let us send and receive packets over UDP
const IPAddress outIp(192,168,27,1);        // remote IP of your computer
const unsigned int outPort = 7110;          // remote port to receive OSC
const unsigned int localPort = 7110;        // local port to listen for OSC packets (actually not used for sending)

void setup() {
    Serial.begin(115200);

    // Connect to WiFi network
    Serial.println();
    Serial.println();
    Serial.print("Connecting to ");
    Serial.println(ssid);
    WiFi.begin(ssid, pass);

    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }
    Serial.println("");

    Serial.println("WiFi connected");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());

    Serial.println("Starting UDP");
    Udp.begin(localPort);
    Serial.print("Local port: ");
    Serial.println(Udp.localPort());


    //accelerometer
    #ifndef ESP8266
    while (!Serial); // for Leonardo/Micro/Zero
    #endif
    Serial.println("Accelerometer Test"); Serial.println("");

    Wire.begin(14,12);
  /* Initialise the sensor */
    if(!accel.begin())
    {
    /* There was a problem detecting the ADXL345 ... check your connections */
        Serial.println("Ooops, no ADXL345 detected ... Check your wiring!");
        while(1);
    }

  /* Set the range to whatever is appropriate for your project */
    accel.setRange(ADXL345_RANGE_16_G  );

}

void loop() {
      /* Get a new sensor event */ 
    sensors_event_t event; 
    accel.getEvent(&event);
    int16_t acc_x = event.acceleration.x;
    int16_t acc_y = event.acceleration.y;
    int16_t acc_z = event.acceleration.z;
    Serial.println();
    Serial.println(acc_x);
    Serial.println(acc_y);
    Serial.println(acc_z);
    OSCMessage msg("/osc");
    msg.add(acc_x);
    msg.add(acc_y);
    msg.add(acc_z);
    Udp.beginPacket(outIp, outPort);
    msg.send(Udp);
    Serial.print("Message Sent");
    Udp.endPacket();
    msg.empty();
    delay(500);
}
