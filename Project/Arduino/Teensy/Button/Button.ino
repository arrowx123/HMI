//#include <OSCBoards.h>
//#include <OSCBundle.h>
//#include <OSCData.h>
//#include <OSCMatch.h>
//#include <OSCMessage.h>
//#include <OSCTiming.h>
//#include <SLIPEncodedSerial.h>
//#include <SLIPEncodedUSBSerial.h>



int ledPin = 13;
int buttonPin = 12;

int longDelay = 400;
int shortDelay = 80;

bool startButton = true;

int triggerControl                  = 0;
int rotationTriggerControl          = 1;
int drillBitControl                 = 2;
int placementAroundACircleControl   = 3;

int moveXPos                           = 4;
int moveXNeg                           = 5;
int moveYPos                           = 6;
int moveYNeg                           = 7;
int moveZPos                           = 8;
int moveZNeg                           = 9;

int rotateXPos                         = 14;
int rotateXNeg                         = 15;
int rotateYPos                         = 16;
int rotateYNeg                         = 17;
int rotateZPos                         = 18;
int rotateZNeg                         = 19;


void setup()
{
  Serial.begin(9600);
  
  pinMode(ledPin, OUTPUT);       // LED

  
  pinMode(buttonPin, INPUT_PULLUP); // Pushbutton

//  Low voltage means pressed button.
  pinMode(triggerControl, INPUT_PULLUP); // Pushbutton
  pinMode(rotationTriggerControl, INPUT_PULLUP); // Pushbutton
  pinMode(drillBitControl, INPUT_PULLUP); // Pushbutton
  pinMode(placementAroundACircleControl, INPUT_PULLUP); // Pushbutton
  
  pinMode(moveXPos, INPUT_PULLUP); // Pushbutton
  pinMode(moveXNeg, INPUT_PULLUP); // Pushbutton
  pinMode(moveYPos, INPUT_PULLUP); // Pushbutton
  pinMode(moveYNeg, INPUT_PULLUP); // Pushbutton
  pinMode(moveZPos, INPUT_PULLUP); // Pushbutton
  pinMode(moveZNeg, INPUT_PULLUP); // Pushbutton
  
  pinMode(rotateXPos, INPUT_PULLUP); // Pushbutton
  pinMode(rotateXNeg, INPUT_PULLUP); // Pushbutton
  pinMode(rotateYPos, INPUT_PULLUP); // Pushbutton
  pinMode(rotateYNeg, INPUT_PULLUP); // Pushbutton
  pinMode(rotateZPos, INPUT_PULLUP); // Pushbutton
  pinMode(rotateZNeg, INPUT_PULLUP); // Pushbutton
}

bool buttonPressed(int buttonIndex)
{
  if(!digitalRead(buttonIndex))
    return true;
  else
    return false;  
}

void sendMsg(String msg)
{
  Serial.println("msg: " + msg);
}



void sendTriggerControl()
{
  if (buttonPressed(triggerControl))
    sendMsg("p");
  else
    sendMsg("pp");
}
void sendRotationTriggerControl()
{
  if (buttonPressed(rotationTriggerControl))
    sendMsg("q");
  else
    sendMsg("qq");
}
void sendDrillBitControl()
{
  if (buttonPressed(drillBitControl))
    sendMsg("p");
  else
    sendMsg("pp");
}
void sendPlacementAroundACircleControl()
{
  if (buttonPressed(placementAroundACircleControl))
    sendMsg(" ");
  else
    sendMsg("  ");
}
void sendMoveXPos()
{
  if (buttonPressed(moveXPos))
    sendMsg("1");
  else
    sendMsg("11");
}
void sendMoveXNeg()
{
  if (buttonPressed(moveXNeg))
    sendMsg("2");
  else
    sendMsg("22");
}
void sendMoveYPos()
{
  if (buttonPressed(moveYPos))
    sendMsg("3");
  else
    sendMsg("33");
}
void sendMoveYNeg()
{
  if (buttonPressed(moveYNeg))
    sendMsg("4");
  else
    sendMsg("44");
}
void sendMoveZPos()
{
  if (buttonPressed(moveZPos))
    sendMsg("5");
  else
    sendMsg("55");
}
void sendMoveZNeg()
{
  if (buttonPressed(moveZNeg))
    sendMsg("6");
  else
    sendMsg("66");
}
void sendRotateXPos()
{
  if (buttonPressed(rotateXPos))
    sendMsg("7");
  else
    sendMsg("77");
}
void sendRotateXNeg()
{
  if (buttonPressed(rotateXNeg))
    sendMsg("8");
  else
    sendMsg("88");
}
void sendRotateYPos()
{
  if (buttonPressed(rotateYPos))
    sendMsg("9");
  else
    sendMsg("99");
}
void sendRotateYNeg()
{
  if (buttonPressed(rotateYNeg))
    sendMsg("0");
  else
    sendMsg("00");
}
void sendRotateZPos()
{
  if (buttonPressed(rotateZPos))
    sendMsg("-");
  else
    sendMsg("--");
}
void sendRotateZNeg()
{
  if (buttonPressed(rotateZNeg))
    sendMsg("+");
  else
    sendMsg("++");
}



void loop()
{
//  String currentMsg = Serial.read();
//  Serial.println(currentMsg);
  
//  if(currentMsg)
  
  if (digitalRead(buttonPin)) {
    // D7 pin is high due to pullup resistor
    digitalWrite(ledPin, LOW);   // LED on
    delay(longDelay);                  // Slow blink
    digitalWrite(ledPin, HIGH);  // LED off
    delay(longDelay);
  } else {
    // D7 pin is low due to pushbutton pressed
    digitalWrite(ledPin, LOW);   // LED on
    delay(shortDelay);                   // Fast blink
    digitalWrite(ledPin, HIGH);  // LED off
    delay(shortDelay);
  }

  

  if(startButton)
  { 
    sendTriggerControl();
    sendRotationTriggerControl();
    sendDrillBitControl();
    sendPlacementAroundACircleControl();
    
    sendMoveXPos();
    sendMoveXNeg();
    sendMoveYPos();
    sendMoveYNeg();
    sendMoveZPos();
    sendMoveZNeg();
  
    sendRotateXPos();
    sendRotateXNeg();
    sendRotateYPos();
    sendRotateYNeg();
    sendRotateZPos();
    sendRotateZNeg();
  }
}
