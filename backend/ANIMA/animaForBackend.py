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
        self.anima.use_profile_to_talk(profile_path=self.profileToUse(), text=text, lang='en')

    def registerProfile(self):
        newUser = frontendJson["speakerName"]
        self.anima.create_profile(profile_path=f"../../animaProfiles/{newUser}.animaprofile", speaker_wav="../../output.wav", lang="en")
    
    def profileToUse(self):
        currentUser = frontendJson["nameOfCurrentUser"]
        return "../../animaProfiles/" + currentUser + ".animaprofile"
    
    def convertImageToText(self, path):
        return self.imageConverter.img_to_str(path, "eng")

if __name__ == "__main__":
    observer = Observer()
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