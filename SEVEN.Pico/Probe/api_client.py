import urequests as requests
import network
import secrets

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

    request = requests.get('https://feiertage-api.de/api/?jahr=2016&nur_land=TH')
    
#print(request.content) # HTTP-Response-Status-Code ausgeben
    print('Status-Code:', request.status_code)
    return request.status_code