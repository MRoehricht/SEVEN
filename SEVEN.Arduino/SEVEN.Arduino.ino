/*
  SEVEN Arduino Project to receive commands.
 */

#include <SPI.h>
#include <WiFiNINA.h>
#include "arduino_secrets.h"
#include "arduino_commands.h"
#include "arduino_statusnames.h"

///////please enter your sensitive data in the Secret tab/arduino_secrets.h
char ssid[] = SECRET_SSID;  // your network SSID (name)
char pass[] = SECRET_PASS;  // your network password (use for WPA, or use as key for WEP)


int headlight_led = LED_BUILTIN;
int status = WL_IDLE_STATUS;
bool sendImage = false;
WiFiServer server(80);

void setup() {
  //Initialize serial and wait for port to open:
  Serial.begin(9600);
  while (!Serial) {
    ;  // wait for serial port to connect. Needed for native USB port only
  }

  pinMode(headlight_led, OUTPUT);  // set the LED pin mode
  // check for the WiFi module:
  if (WiFi.status() == WL_NO_MODULE) {
    Serial.println("Communication with WiFi module failed!");
    // don't continue
    while (true)
      ;
  }

  // attempt to connect to WiFi network:
  while (status != WL_CONNECTED) {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid, pass);

    // wait 10 seconds for connection:
    delay(10000);
  }
  server.begin();
  // you're connected now, so print out the status:
  printWiFiStatus();
}

void loop() {
  // compare the previous status to the current status
  if (status != WiFi.status()) {
    // it has changed update the variable
    status = WiFi.status();

    if (status == WL_AP_CONNECTED) {
      // a device has connected to the AP
      Serial.println("Device connected to AP");
    } else {
      // a device has disconnected from the AP, and we are back in listening mode
      Serial.println("Device disconnected from AP");
    }
  }

  WiFiClient client = server.available();  // listen for incoming clients

  if (client) {                    // if you get a client,
    Serial.println("new client");  // print a message out the serial port
    String currentLine = "";       // make a String to hold incoming data from the client
    while (client.connected()) {   // loop while the client's connected
      delayMicroseconds(10);       // This is required for the Arduino Nano RP2040 Connect - otherwise it will loop so fast that SPI will never be served.
      if (client.available()) {    // if there's bytes to read from the client,
        char c = client.read();    // read a byte, then
        Serial.write(c);           // print it out to the serial monitor
        if (c == '\n') {           // if the byte is a newline character

          // if the current line is blank, you got two newline characters in a row.
          // that's the end of the client HTTP request, so send a response:
          if (currentLine.length() == 0) {
            // HTTP headers always start with a response code (e.g. HTTP/1.1 200 OK)
            // and a content-type so the client knows what's coming, then a blank line:
            client.println("HTTP/1.1 200 OK");
            client.println("Content-Type: application/json");
            client.println("Access-Control-Allow-Origin: *");
            client.println();
            String json = getStatus();
            client.println(json);
            // The HTTP response ends with another blank line:
            client.println();
            // break out of the while loop:
            break;
          } else {  // if you got a newline, then clear currentLine:
            currentLine = "";
          }
        } else if (c != '\r') {  // if you got anything else but a carriage return character,
          currentLine += c;      // add it to the end of the currentLine
        }
        // Check to see if the client request was "GET /HL0" or "GET /HL1":
        if (currentLine.endsWith(COMMAND_HEADLIGHTS_ON)) {
          digitalWrite(headlight_led, HIGH);
        }
        if (currentLine.endsWith(COMMAND_HEADLIGHTS_OFF)) {
          digitalWrite(headlight_led, LOW);
        }
        if (currentLine.endsWith(COMMAND_CAMERA_TAKEFOTO)) {
          takeFoto();
        }
      }
    }
    // close the connection:
    client.stop();
    Serial.println("client disconnected");
  }
}

void takeFoto() {
  sendImage = true;
}

String getStatus() {
  ///STATUS_HEADLIGHTS
  String headlightStatus = getSwitchStatuses(STATUS_HEADLIGHTS, getBoolString(digitalRead(headlight_led)));

  String imageJson = "";
  if (sendImage) {
    String image = "    _(  )_( )_	\n   (_   _    _)\n  / /(_) (__)	\n / / / / / /	\n/ / / / / /	\n";
    imageJson = ", ImageData:\"" + image + "\"";
    sendImage = false;
  }

  return "{\"Id\":\"Matze1\",SwitchStatuses:[" + headlightStatus + "]" + imageJson + "}";
}

String getSwitchStatuses(String name, String status) {
  return "{\"Name\":\"" + name + "\", \"Status\":\"" + status + "\"}";
}

String getBoolString(bool value) {
  if (value) {
    return "true";
  }
  return "false";
}

void printWiFiStatus() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print where to go in a browser:
  Serial.print("To see this page in action, open a browser to http://");
  Serial.println(ip);
}
