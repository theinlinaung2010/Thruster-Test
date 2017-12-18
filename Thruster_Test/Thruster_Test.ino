#include <CmdMessenger.h>
#include <HX711.h>
#include <Servo.h>
//#include <Wire.h>
#include <ACS712.h>
#include <Adafruit_ADS1015.h>

CmdMessenger cmdMessenger = CmdMessenger(Serial);

//Load cell related constants
const long calibration_factor = 396050;
const int DOUT = 8;
const int CLK = 9;
HX711 loadCell(DOUT, CLK);

//ESC parameters
const int escPin = 10;
const int escMaxFwd = 160;
const int escMaxRev = 20;
const int escNeutral = 90;
Servo esc;

Adafruit_ADS1115 ads; 
ACS712 currentSensor(ACS712_05B, A1);

//Voltage and current sensor constants
const int voltageSensorPin = A0;
const float voltageFactor = 0.0166;
const int currentSensorPin = A1;        

const int dataSendInterval = 100;
unsigned long lastTime;
unsigned long timeOffset = 0;
bool isAlive = false;
int throttle = 0;

enum Command
{
  kStart, kStop, kMotorValue, kTime, kThrust, kVoltage, kCurrent, kRpm, kThrottle, kTare,
};

void setup() {
  Serial.begin(115200);
  AttachCommandCallbacks();
  loadCell.set_scale(calibration_factor);
  loadCell.tare();
  esc.attach(escPin);
  esc.write(escNeutral);
  ads.setGain(GAIN_EIGHT);      // 8x gain   +/- 0.512V  1 bit = 0.015625mV
  ads.begin();
  currentSensor.calibrate();
  lastTime = millis();
}

void loop() {
  cmdMessenger.feedinSerialData();
  if (isAlive && millis() - lastTime > dataSendInterval) {
    lastTime = millis();
    SendTime();
    SendThrust();
    SendVoltage();
    SendCurrent();
    //SendRPM();
    SendThrottle();
  }
}

void SendTime() {
  cmdMessenger.sendCmdStart(kTime);
  cmdMessenger.sendCmdBinArg<uint32_t>((uint32_t)(millis() - timeOffset));
  cmdMessenger.sendCmdEnd();
  
}

void SendThrust() {
  cmdMessenger.sendCmdStart(kThrust);
  cmdMessenger.sendCmdBinArg<float>(loadCell.get_units()); 
  cmdMessenger.sendCmdEnd();
}

void SendVoltage() {
  float voltage = analogRead(voltageSensorPin) * voltageFactor;
  cmdMessenger.sendCmdStart(kVoltage);
  cmdMessenger.sendCmdBinArg<float>(voltage);
  cmdMessenger.sendCmdEnd();
}

void SendCurrent() {
  float current = currentSensor.getCurrentDC();
  cmdMessenger.sendCmdStart(kCurrent);
  cmdMessenger.sendCmdBinArg<float>(current);
  cmdMessenger.sendCmdEnd();
}

void SendRPM() {
  float rpm = 1.0;
  cmdMessenger.sendCmdStart(kRpm);
  cmdMessenger.sendCmdBinArg<float>(rpm);
  cmdMessenger.sendCmdEnd();
}

void SendThrottle() {
  cmdMessenger.sendCmdStart(kThrottle);
  cmdMessenger.sendCmdBinArg<int16_t>(throttle);
  cmdMessenger.sendCmdEnd();
}

void AttachCommandCallbacks()
{
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kStart, OnStart);
  cmdMessenger.attach(kStop, OnStop);
  cmdMessenger.attach(kMotorValue, OnMotorValue);
  cmdMessenger.attach(kTare, OnTare);
}

void OnMotorValue() {
  throttle = cmdMessenger.readBinArg<int16_t>();
  if(throttle > 0){
    //Forward drive
    int escVal = map(throttle, 0, 100, escNeutral, escMaxFwd);
    esc.write(escVal);
  }else if(throttle < 0){
    //Reverse drive
    int escVal = map(-throttle, 0, 100, escNeutral, escMaxRev);
    esc.write(escVal);
  }else{
    //Neutral
    esc.write(escNeutral);
  } 
}

void OnTare(){
  loadCell.tare();
}

void OnUnknownCommand() {
  cmdMessenger.sendCmd(0, "Command without attached callback");
}

void OnStart()
{
  isAlive = true;
  timeOffset = millis();
}

void OnStop()
{
  isAlive = false;
}


