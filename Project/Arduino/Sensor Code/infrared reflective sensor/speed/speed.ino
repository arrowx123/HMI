/* This is the code used with the infrared reflective sensor
*  Used to determine the RPM of objects
*  Ensure that the infrared reflective sensor is properly calibrated before use
*/

 #include <TimerOne.h>
unsigned int counter=0;
void blink()
{
    counter++;
}
void timerIsr()
{
    Timer1.detachInterrupt();  //disable the timer1
    Serial.print("The speed of the motor: ");
    Serial.print(counter,DEC);
    Serial.println("round/s");
    counter=0;
    Timer1.attachInterrupt( timerIsr );  //enable the timer1
}
void setup()
{
    Serial.begin(115200);
    Timer1.initialize(500000); // set a timer of length 0.5sec
    attachInterrupt(0, blink, RISING);  //INT0
    Timer1.attachInterrupt( timerIsr ); // attach the service routine here
}
void loop()
{

}
