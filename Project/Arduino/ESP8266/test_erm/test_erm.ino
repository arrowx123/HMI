
const int erm_size = 4;
// up down left right
const int erm_pin[] = {5, 4, 12, 14};

const int led = 16;      //drive internal led    D0

const int intensity_size = 5;
const int intensity[] = {0, 255, 511, 767, 1023};

void setup() {
    // put your setup code here, to run once:
//    delay(1000);
    //  Rotary Switch
    for (int i = 0; i < erm_size; i++) {
        pinMode(erm_pin[i], OUTPUT);

        digitalWrite(erm_pin[i], LOW);
    }
    Serial.println("setPin: set the ERM output pins.");

//  Output
    pinMode(led, INPUT_PULLUP);
    
}

void loop() {
    // put your main code here, to run repeatedly:
    for (int j = 0; j < erm_size; j++) {
        for (int i = intensity_size - 1; i >= 0; i--) {

            delay(1000);
            analogWrite(erm_pin[j], intensity[i]);
            if(j == 0){
              analogWrite(erm_pin[3], intensity[i]);
            }
            else if(j == 1){
              analogWrite(erm_pin[2], intensity[i]);
            }
            else if(j == 2){
              analogWrite(erm_pin[0], intensity[i]);
            }
            else if(j == 3){
              analogWrite(erm_pin[1], intensity[i]);
            }
            
            Serial.printf("i: %d; j: %d\n", i, j);
            
        }

        delay(1000);
        digitalWrite(led, HIGH);
        delay(1000);
        digitalWrite(led, LOW);

    }
}
