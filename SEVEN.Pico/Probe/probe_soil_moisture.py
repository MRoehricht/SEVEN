from machine import Pin, I2C, ADC
import time
import apiclient
import bme280

myId = '7a73f8ae-0000-0000-bbbb-7ab5a00a9c1d'
adc = ADC(Pin(27))# Create an ADC object linked to pin 27
led = Pin("LED", Pin.OUT)
i2c=I2C(0,sda=Pin(16), scl=Pin(17), freq=400000)    

def my_map(x, in_min, in_max, out_min, out_max):
  return int((x-in_min) * (out_max-out_min) / (in_max-in_min) + out_min)

def loop():
    while True:
      led.value(0)
      # Read ADC and convert to voltage
      uvVoltageADC = adc.read_u16() * (3.3 / 65535)        
      moistureVoltage = "{}V".format(round(uvVoltageADC, 2))  
      
      #0.95V Nass =  100%
      #2.41V Trocken = 0%
      value_percentage=my_map(uvVoltageADC, 2.50, 0.95, 0, 100)      
      #Type = SoilMoisture = 64
           
      #values = {'ProbeId': myId, 'MeasurementItems':[{"Type":64,"Value":value_percentage}]}  
      #print(values)
      bme = bme280.BME280(i2c=i2c)          #Init BME280
      values = {'probeId': myId,'temperature': bme.temperature, 'airpressure' : bme.airpressure, 'humidity': bme.humidity} 
      
      
      #json = 'http://192.168.178.4:5009/measurement/create/%7B%22ProbeId%22%3A%20%227a73f8ae-0000-0000-bbbb-7ab5a00a9c1d%22%2C%20%22MeasurementItems%22%3A%5B%20%7B%22Type%22%3A64%2C%22Value%22%3A%22' + str(value_percentage) +'%22%7D%5D%7D'
      json = 'http://192.168.178.4:5009/measurement/create/%7B%22ProbeId%22%3A%20%227a73f8ae-0000-0000-bbbb-7ab5a00a9c1d%22%2C%20%22MeasurementItems%22%3A%5B%20%7B%22Type%22%3A64%2C%22Value%22%3A%22' + str(value_percentage) +'%22%7D%2C%20%7B%22Type%22%3A1%2C%22Value%22%3A%22' + str(bme.temperature) +'%22%7D%2C%20%7B%22Type%22%3A8%2C%22Value%22%3A%22' + str(bme.humidity) +'%22%7D%2C%7B%22Type%22%3A256%2C%22Value%22%3A%22' + str(bme.airpressure) +'%22%7D%5D%7D'
                
                
      status_code = apiclient.sendJSON(json)
      print(status_code)
      
      if status_code == 200:
          led.value(1)
          time.sleep(10)
          led.value(0)
      else:
          values = range(20)
          for i in values:
              led.value(1)
              time.sleep(0.1)
              led.value(0)
              time.sleep(0.1)
      
      time.sleep(500)           


try:
    ip = apiclient.connect()
    loop()
except KeyboardInterrupt:
    machine.reset()




