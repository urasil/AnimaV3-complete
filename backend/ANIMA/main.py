from os import listdir
import numpy
import numba
import json
import sys
from os.path import join, isfile
from src.ANIMA import ANIMA
from src.manage_audio import AudioManager 

def main():
    anima = ANIMA()
    audio_manager = AudioManager()

    f = open("./data/data.json", "r")
    data = json.load(f)
    print(data)
    f.close()

    profile_path = data["profile"]
    text = data["text"]
    lang = data["lang"]

    wavs = [f for f in listdir("./data/temp") if (isfile(join("./data/temp", f)) and f.endswith(".wav"))]
    if len(wavs) > 0:
        wav = wavs[0]
        profileName = wav.replace(".wav", ".animaprofile")
        print(join("./data/profiles", profileName), join("./data/temp", wav))
        anima.create_profile(profile_path=join("./data/profiles", profileName), speaker_wav=join("./data/temp", wav), lang=lang)

    # anima.use_profile_with_string(profile_path=profile_path, lang="en", text=text)
    anima.use_profile_with_string_outfile(profile_path=profile_path, lang="en", text=text)
    
main()
sys.exit()