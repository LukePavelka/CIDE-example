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
with open('CargoTransfer.data', 'rb') as filehandle:
    placesList = pickle.load(filehandle)
    keyboard.play(placesList, speed_factor=1)

    
# time.sleep(5)
# keyboard.play(Phase1, speed_factor=1)

# time.sleep(5)
# print("starting")
# recorded = keyboard.record(until='esc')
# recorded.pop()
# with open('keystroke2.data', 'wb') as filehandle:
#     pickle.dump(recorded, filehandle)
