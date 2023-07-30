from machine import Pin, I2C, ADC
from time import sleep
import api_client
import bme280

myId = '00000000-0000-0000-0000-000000000001'
adc = ADC(Pin(26))# Create an ADC object linked to pin 26

def loop():
    while True:
      bme = bme280.BME280(i2c=i2c)          #Init BME280        
      
      # Read ADC and convert to voltage
      uvVoltageADC = adc.read_u16() * (3.3 / 65535)	        
	  uvVoltage = "{}V".format(round(uvVoltageADC, 2))
	  
	  values = {'probeId': myId,'temperature': bme.temperature, 'airpressure' : bme.airpressure, 'humidity': bme.humidity, 'uvVoltage' : uvVoltage}  
      print(values)   
      
      status_code = api_client.sendJSON('https://feiertage-api.de/api/?jahr=2016&nur_land=TH')
      print(status_code)    
   
      sleep(10)           #delay of 10s


try:
    ip = api_client.connect()
    loop()
except KeyboardInterrupt:
    machine.reset()

