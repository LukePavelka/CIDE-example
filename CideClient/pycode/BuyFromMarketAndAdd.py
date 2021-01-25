import keyboard
import time
import json
import pickle
import json
import io
import sys
import os
os.chdir(os.path.dirname(os.path.abspath(__file__)))

time.sleep(2)
with open('BuyFromMarketAndAdd.data', 'rb') as filehandles:
    placesList1 = pickle.load(filehandles)
    keyboard.play(placesList1, speed_factor=1)
