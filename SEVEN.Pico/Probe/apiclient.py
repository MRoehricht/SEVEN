import urequests as requests
import network
import secrets
from time import sleep

def connect():
    #Connect to WLAN
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    wlan.connect(secrets.SSID, secrets.PASSWORD)
    while wlan.isconnected() == False:
        print('Waiting for connection...')
        sleep(1)
    ip = wlan.ifconfig()[0]
    print(f'Connected on {ip}')
    return ip

def sendJSON(json):
    print(json)
    request = requests.get(json)
    
    print(request.content) # HTTP-Response-Status-Code ausgeben
    print('Status-Code:', request.status_code)
    return request.status_code


