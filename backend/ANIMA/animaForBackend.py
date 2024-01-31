import json
import sys
import time
from src.ANIMA import ANIMA
from src.manage_audio import AudioManager
from src.pdf_to_txt import PdfToStrings
from src.img_to_txt import ImgToStrings
from observer import Observer
import os

frontendJsonFilePath = "../../frontend.json"
backendJsonFilePath = "../../backend.json"
language = "en"
animaProfilesPath = "../../animaProfiles/"

with open(frontendJsonFilePath, "r") as json_file:
    frontendJson = json.load(json_file)

with open(backendJsonFilePath, "r") as json_file:
    backendJson = json.load(json_file)

def writeToBackendJson():
    with open(backendJsonFilePath, "w") as jsonFile:
        json.dump(backendJson, jsonFile)

class BackendFunctionalites:

    def __init__(self):
        self.anima = ANIMA()
        self.imageConverter = ImgToStrings()
        self.pdfConverter = PdfToStrings()

    def computerSpeak(self, text):
        self.anima.use_profile_to_talk(profile_path=self.profileToUse(), text=text, lang='en')  # ANIMA already has a similar method for this

    def registerProfile(self):
        newUser = frontendJson["speakerName"]
        self.anima.create_profile(profile_path=f"{animaProfilesPath}{newUser}.animaprofile", speaker_wav="../../output.wav", lang=language)
    
    def profileToUse(self):
        currentUser = frontendJson["nameOfCurrentUser"]
        return animaProfilesPath + currentUser + ".animaprofile"
    
    def convertImageToText(self, path):
        return self.imageConverter.img_to_str(path, "eng")

if __name__ == "__main__":
    observer = Observer(frontEndJsonPath=frontendJsonFilePath)
    functions = BackendFunctionalites()
    while(True):
        changes = observer.detectChanges()
        if(changes != []):
            for change in changes:

                # Setting which voice profile to use
                if(change[0] == "nameOfCurrentUser"):
                    functions.profileToUse()

                # Talking
                elif(change[0] == "content"):
                    print("speak")
                    try:
                        functions.computerSpeak(change[1])
                        backendJson["speechSuccess"] = "true"
                        writeToBackendJson()
                    except Exception as e:
                        print("Failed to speak", e)
                # New profile to be registered
                elif(change[0] == "speakerName"):
                    print("register profile")
                    try:
                        functions.registerProfile()
                        backendJson["profileCreationSuccess"] = "true"
                        writeToBackendJson()
                    except:
                        print("Failed to register profile")

                elif(change[0] == "readFilePath"):
                    print("read file and speak")
                    try:
                        text = functions.convertImageToText(change[1])
                        functions.computerSpeak(text)
                        backendJson["readFileSuccess"] = ""
                        writeToBackendJson()
                    except:
                        print("Failed to read file")
        time.sleep(1)