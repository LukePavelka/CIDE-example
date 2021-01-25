import keyboard
import time
import json
import pickle
import json
import io
import sys
import os
os.chdir(os.path.dirname(os.path.abspath(__file__)))

args=sys.argv[1:]
Destination=args[0]
with open('jumpFromManagment.data', 'rb') as filehandle:
    placesList = pickle.load(filehandle)
    keyboard.play(placesList, speed_factor=1)
    keyboard.write(Destination)
    time.sleep(3)
    with open('keystroke2.data', 'rb') as filehandles:
        placesList1 = pickle.load(filehandles)
        keyboard.play(placesList1, speed_factor=0.3) 
