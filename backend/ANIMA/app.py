import json
from os import listdir
from os.path import isfile, join
import sys
from flask import Flask
import subprocess

from src.manage_audio import AudioManager
from src.ANIMA import ANIMA

app = Flask(__name__)

anima = ANIMA()
audio_manager = AudioManager()

with open("./data/data.json", "r") as f:
    data = json.load(f)

profile_path = data["profile"]
text = data["text"]
lang = data["lang"]

synth = anima.init_synthesizer(lang="en")

f = open("./data/status.json", "w")
f.write(json.dumps({"ready" : True}))
f.close()

@app.route('/')
def test():
    print("whooop")
    return "works"

@app.route('/play')
def play():

    with open("./data/status.json", "w") as f:
        f.write(json.dumps({"ready" : False}))

    f = open("./data/data.json", "r")
    data = json.load(f)
    f.close()

    profile_path = data["profile"]
    text = data["text"]

    anima.use_synthesizer_outfile(synthesizer=synth, profile_path=profile_path, text=text)

    # out = subprocess.check_output([sys.executable, "main.py"], stdin=subprocess.PIPE)
    # p = subprocess.Popen(['main.py'], stdout=subprocess.PIPE, stdin=subprocess.PIPE, stderr=subprocess.PIPE)#

    with open("./data/status.json", "w") as f:
        f.write(json.dumps({"ready" : True}))
    return str(text)

@app.route('/record')
def record():
    with open("./data/status.json", "w") as f:
        f.write(json.dumps({"ready" : False}))

    wavs = [f for f in listdir("./data/temp") if (isfile(join("./data/temp", f)) and f.endswith(".wav"))]
    if len(wavs) > 0:
        wav = wavs[0]
        profileName = wav.replace(".wav", ".animaprofile")
        print(join("./data/profiles", profileName), join("./data/temp", wav))
        anima.create_profile(profile_path=join("./data/profiles", profileName), speaker_wav=join("./data/temp", wav), lang=lang)
        
        with open("./data/status.json", "w") as f:
            f.write(json.dumps({"ready" : True}))

        return join("./data/profiles", profileName)
    else:
        return "Please place the input voice .wav in the ./data/temp folder for use"

if __name__ == "__main__":
    app.run(debug=True, port=80, host="127.0.0.1")