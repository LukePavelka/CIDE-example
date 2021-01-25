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
time.sleep(3)
with open('keystroke.data', 'rb') as filehandle:
    placesList = pickle.load(filehandle)
    reversed_list = placesList[::-1]
    preStart = []
    preStart += [reversed_list.pop()]
    preStart += [reversed_list.pop()]
    preStart += [reversed_list.pop()]
    preStart += [reversed_list.pop()]
    keyboard.play(preStart, speed_factor=1)
    time.sleep(10)
    keyboard.play(reversed_list[::-1], speed_factor=1)
    keyboard.write(Destination)
    time.sleep(3)
    with open('keystroke2.data', 'rb') as filehandles:
      placesList1 = pickle.load(filehandles)
      keyboard.play(placesList1, speed_factor=0.3)
    
    
    
    
    
    
    
    
  # time.sleep(5)
# keyboard.play(Phase1, speed_factor=1)

# time.sleep(5)
# print("starting")
# recorded = keyboard.record(until='esc')
# recorded.pop()
# with open('keystroke2.data', 'wb') as filehandle:
#     pickle.dump(recorded, filehandle)
