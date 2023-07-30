from machine import Pin, I2C, ADC
from time import sleep
import urequests as requests
import network
import bme280
 
i2c=I2C(0,sda=Pin(16), scl=Pin(17), freq=400000)    
myId = 'ef8b26c7-87d6-4668-abdb-70a9ba28206e'
adc = ADC(Pin(26))# Create an ADC object linked to pin 26
ssid = 'FRITZ!Box 7530 DB'
password = '17762799179013343854'

def connect():
    #Connect to WLAN
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    wlan.connect(ssid, password)
    while wlan.isconnected() == False:
        print('Waiting for connection...')
        sleep(1)
    ip = wlan.ifconfig()[0]
    print(f'Connected on {ip}')
    return ip



def loop():
    while True:
      bme = bme280.BME280(i2c=i2c)          #Init BME280
      values = {'probeId': myId,'temperature': bme.temperature, 'airpressure' : bme.airpressure, 'humidity': bme.humidity}  
      print(values)
      
      
      # Read ADC and convert to voltage
      val = adc.read_u16()
      val = val * (3.3 / 65535)
      print(round(val, 2), "V") # Keep only 2 digits
      
    #<50 => UV-Index 0
    #<227 => UV-Index 1
    #<318 => UV-Index 2
    #<408 => UV-Index 3
    #<503 => UV-Index 4
    #<606 => UV-Index 5
    #<696 => UV-Index 6
    #<795 => UV-Index 7
    #<881 => UV-Index 8
    #<976 => UV-Index 9
    #<1079 => UV-Index 10
    #>1170 => UV-Index 11
      
      request = requests.get('https://feiertage-api.de/api/?jahr=2016&nur_land=TH')
      print(request.content)    
      
      sleep(10)           #delay of 10s


try:
    ip = connect()
    loop()
except KeyboardInterrupt:
    machine.reset()

