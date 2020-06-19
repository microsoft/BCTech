// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

#define ARDUINO 180

// WIFI
#include <ESP8266WiFi.h>
#include "wifi_setup.h"

// AZURE IOT
#include "src/iotc/common/string_buffer.h"
#include "src/iotc/iotc.h"
#include "azure_iot_device_setup.h"
void on_event(IOTContext ctx, IOTCallbackInfo* callbackInfo);
#include "src/connection.h"

// JSON
#include <Arduino_JSON.h>

// SENSORS
#include <Adafruit_Sensor.h>
#include <DHT.h>
#include <DHT_U.h>
#include "HX711.h"
#include "sensor_setup.h"

// GLOBALS
DHT_Unified dht(DHTPIN, DHTTYPE);
uint32_t min_sensor_delay; // miliseconds
uint32_t emit_frequency = EMIT_FREQUENCY;
uint32_t currSettingVersion = 0;
HX711 scale;
int blueLED = 2;

void setup()
{
  pinMode(blueLED, OUTPUT);     // blue led
  digitalWrite(blueLED, HIGH);  // Turn the LED off

  Serial.begin(512000);
  // connect
  ConnectWifi();
  ConnectAzureIoTCentral();

  // init sensor(s)
  sensor_t temperaturesensor, humiditysensor;
  SetupTemperatureAndHumiditySensor(&temperaturesensor, &humiditysensor);
  PrintSensorInfoToSerial(&temperaturesensor, &humiditysensor);
  scale.begin(LOADCELL_DOUT_PIN, LOADCELL_SCK_PIN);
  scale.set_scale(SCALE_CALIBRATION);
  scale.tare();

  pinMode(BUTTON_PIN, INPUT_PULLUP);
}

void loop()
{
  if (isConnected) // Azure IoT callback event
  {
    unsigned long ms = millis();
    if (ms - lastTick > emit_frequency) // send telemetry every x seconds
    {
      sensors_event_t event;
      float currTemp, currHumidity, currWeight;

      currWeight = GetWeightAndPrintToSerial();
      currTemp = GetTemperatureAndPrintToSerial(&event);
      currHumidity = GetHumidityAndPrintToSerial(&event);

      // send sensor data to azure iot
      SendSensorDataToAzureIoT(TEMPERATURE, &currTemp);
      SendSensorDataToAzureIoT(HUMIDITY, &currHumidity);
      SendSensorDataToAzureIoT(WEIGHT, &currWeight);

      lastTick = ms;

      // blink once to show connection is still active
      digitalWrite(blueLED, LOW);   // Turn the LED on
      delay(20);                      // Wait
      digitalWrite(blueLED, HIGH);  // Turn the LED off
    }

    iotc_do_work(context);  // do background work for iotc

    // scale reset button (tare)
    if (digitalRead(BUTTON_PIN) == LOW) // button is pressed
    {

      Serial.println("Reset button pressed. Resetting scale...");
      scale.tare();
    }

  }
  else
  {
    iotc_free_context(context);
    context = NULL;
    ConnectAzureIoTCentral();
  }
}

/* -----------------------------------------------------
    FUNCTIONS
*/
void ConnectWifi()
{
  // Fill in your wifi network's ssid and password in wifi_setup.h

  const char* ssid = WIFI_SSID;
  const char* password =  WIFI_PASSWORD;

  connect_wifi(ssid, password);
}

void ConnectAzureIoTCentral()
{
  // Fill in the scope id, device id and device key in azure_iot_device_setup.h
  // You can find these values in your azure iot central app: Azure IoT Central -> Devices -> <your device> -> Connect
  const char* scope = SCOPE_ID;
  const char* device_id = DEVICE_ID;
  const char* device_key = DEVICE_KEY;

  connect_client(scope, device_id, device_key);

  if (context != NULL)
  {
    //lastTick = 0;  // set timer in the past to enable first telemetry a.s.a.p
    lastTick = millis();  // set timer to now to avoid sending data too soon
  }
}

void SetupTemperatureAndHumiditySensor(sensor_t* temperaturesensor, sensor_t* humiditysensor)
{
  // set up temp sensor
  dht.begin();
  // DHT22
  // Initialize device.
  dht.temperature().getSensor(temperaturesensor);
  min_sensor_delay = temperaturesensor->min_delay / 1000;

  dht.humidity().getSensor(humiditysensor);
  min_sensor_delay = max(int(min_sensor_delay), humiditysensor->min_delay / 1000);
}

void PrintTemperatureSensorInfoToSerial(sensor_t* sensor)
{
  // Print temperature sensor details.
  Serial.println(F("------------------------------------"));
  Serial.println(F("Temperature Sensor"));
  Serial.print  (F("Sensor Type: ")); Serial.println(sensor->name);
  Serial.print  (F("Driver Ver:  ")); Serial.println(sensor->version);
  Serial.print  (F("Unique ID:   ")); Serial.println(sensor->sensor_id);
  Serial.print  (F("Max Value:   ")); Serial.print(sensor->max_value); Serial.println(F("°C"));
  Serial.print  (F("Min Value:   ")); Serial.print(sensor->min_value); Serial.println(F("°C"));
  Serial.print  (F("Resolution:  ")); Serial.print(sensor->resolution); Serial.println(F("°C"));
  Serial.print  (F("Minimum Delay:  ")); Serial.print((sensor->min_delay) / 1000); Serial.println(F(" ms"));
  Serial.println(F("------------------------------------"));
}

void PrintHumiditySensorInfoToSerial(sensor_t* sensor)
{
  // Print humidity sensor details.
  Serial.println(F("Humidity Sensor"));
  Serial.print  (F("Sensor Type: ")); Serial.println(sensor->name);
  Serial.print  (F("Driver Ver:  ")); Serial.println(sensor->version);
  Serial.print  (F("Unique ID:   ")); Serial.println(sensor->sensor_id);
  Serial.print  (F("Max Value:   ")); Serial.print(sensor->max_value); Serial.println(F("%"));
  Serial.print  (F("Min Value:   ")); Serial.print(sensor->min_value); Serial.println(F("%"));
  Serial.print  (F("Resolution:  ")); Serial.print(sensor->resolution); Serial.println(F("%"));
  Serial.print  (F("Minimum Delay:  ")); Serial.print((sensor->min_delay) / 1000); Serial.println(F(" ms"));
  Serial.println(F("------------------------------------"));
}

void PrintSensorInfoToSerial(sensor_t* temperaturesensor, sensor_t* humiditysensor)
{
  Serial.println(F("DHT22 Unified Sensor Example"));
  PrintTemperatureSensorInfoToSerial(temperaturesensor);
  PrintHumiditySensorInfoToSerial(humiditysensor);
}

float GetTemperatureAndPrintToSerial(sensors_event_t* event)
{
  dht.temperature().getEvent(event);
  if (isnan(event->temperature)) // isnan = IsNotANumber
  {
    Serial.println(F("Error reading temperature!"));
    return 0.00;
  }
  else
  {
    float currTemp = event->temperature;
    /*
      Serial.print(F("Temperature: "));
      Serial.print(currTemp);
      Serial.println(F("°C"));
    */
    return currTemp;
  }
}

float GetHumidityAndPrintToSerial(sensors_event_t* event)
{
  dht.humidity().getEvent(event);
  if (isnan(event->relative_humidity))
  {
    Serial.println(F("Error reading humidity!"));
    return 0.00;
  }
  else
  {
    float currHumidity = event->relative_humidity;
    /*
      Serial.print(F("Humidity: "));
      Serial.print(currHumidity);
      Serial.println(F("%"));
    */
    return currHumidity;
  }
}

float GetWeightAndPrintToSerial()
{
  float currWeight;
  if (scale.is_ready())
  {
    currWeight = scale.get_units(1);
  }
  /*
    Serial.print(F("Weight: "));
    Serial.print(currWeight);
    Serial.println(F("g"));
  */
  return (float) currWeight;
}

uint32_t UpdateTelemetryEmitFrequency(int newValue)
{
  return max(int(min_sensor_delay), newValue);
}

// AZURE IOT
void on_event(IOTContext ctx, IOTCallbackInfo* callbackInfo)
{
  // ConnectionStatus
  if (strcmp(callbackInfo->eventName, "ConnectionStatus") == 0)
  {
    LOG_VERBOSE("Is connected ? %s (%d)", callbackInfo->statusCode == IOTC_CONNECTION_OK ? "YES" : "NO", callbackInfo->statusCode);
    isConnected = callbackInfo->statusCode == IOTC_CONNECTION_OK;
    return;
  }

  // payload buffer doesn't have a null ending.
  // add null ending in another buffer before print
  AzureIOT::StringBuffer buffer;
  if (callbackInfo->payloadLength > 0)
  {
    buffer.initialize(callbackInfo->payload, callbackInfo->payloadLength);
  }

  LOG_VERBOSE("[%s] event was received. Payload =>\n%s\n", callbackInfo->eventName, buffer.getLength() ? *buffer : "EMPTY");

  if (strcmp(callbackInfo->eventName, "SettingsUpdated") == 0)
  {
    if (buffer.getLength())
    {
      JSONVar Settings = JSON.parse(*buffer);
      uint32_t req_freq;
      uint32_t newSettingVersion;
      uint32_t prev_freq = emit_frequency;

      if (Settings.hasOwnProperty("desired"))
      {
        req_freq = (int)Settings["desired"]["emit_freq"]["value"];
        newSettingVersion = (int)Settings["desired"]["$version"];
      }
      else
      {
        if (not Settings.hasOwnProperty("emit_freq"))
        {
          return;
        }

        req_freq = (int)Settings["emit_freq"]["value"];
        newSettingVersion = (int)Settings["$version"];
      }


      if (currSettingVersion == newSettingVersion)
      {
        LOG_VERBOSE("Emit Frequency NOT updated:\n Version:%u \n Old: %u \n Req: %u \n New: %u\n", currSettingVersion, prev_freq, req_freq, emit_frequency);
      }
      else
      {
        emit_frequency = UpdateTelemetryEmitFrequency(req_freq);
        LOG_VERBOSE("Updated to version: %u", newSettingVersion);
        currSettingVersion = newSettingVersion;
        LOG_VERBOSE("Emit Frequency updated: \n Version:%u \n Old: %u \n Req: %u \n New: %u\n", currSettingVersion, prev_freq, req_freq, emit_frequency);
      }
    }
  }

  if (strcmp(callbackInfo->eventName, "Command") == 0)
  {
    String command = callbackInfo->tag;
    if (command == "tare")
    {
      Serial.println("Resetting scale...");
      scale.tare();
    }
    else
    {
      LOG_VERBOSE("- Command name was => %s\r\n", callbackInfo->tag);
    }
  }
}

void SendSensorDataToAzureIoT(const char* fieldname, float * value)
{
  char msg[64] = {0};

  // compose message
  int pos = snprintf(msg, sizeof(msg) - 1, "{\"%s\": %.1f}", fieldname, *value);

  // send message
  int errorCode = iotc_send_telemetry(context, msg, pos);
  msg[pos] = 0;

  // log error
  if (errorCode != 0)
  {
    LOG_ERROR("Sending %s has failed with error code %d", fieldname, errorCode);
  }
}
// -----------------------------------------------------
