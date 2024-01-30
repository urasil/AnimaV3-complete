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

    def computerSpeak(self, text, currentUser):
        print(currentUser)
        profilePath = "../../animaProfiles/" + currentUser + ".animaprofile"
        self.anima.use_profile_to_talk(profile_path=profilePath, text=text, lang='en')

    def registerProfile(self):
        newUser = frontendJson["speakerName"]
        self.anima.create_profile(profile_path=f"../../animaProfiles/{newUser}.animaprofile", speaker_wav="../../output.wav", lang="en")
    
    def converToText(self, path):
        extension = path.split(".")[-1]
        if(extension == "pdf"):
            return self.pdfConverter.pdf_to_str(path)
        elif(extension == "jpg"):
            return self.imageConverter.img_to_str(path, "eng")
        else:
            return False
    
    def registerProfileFromImport(self, path):
        name = path.split("\\")[-1].split(".")[0]
        self.anima.create_profile(profile_path=f"../../animaProfiles/{name}.animaprofile", speaker_wav=path, lang="en")

if __name__ == "__main__":
    currentUser = frontendJson["nameOfCurrentUser"]
    observer = Observer()
    functions = BackendFunctionalites()
    while(True):
        changes = observer.detectChanges()
        print(changes)
        if(changes != []):
            for change in changes:

                # Setting which voice profile to use
                if(change[0] == "nameOfCurrentUser"):
                    currentUser = change[1]

                # Talking
                elif(change[0] == "content"):
                    print("speak")
                    try:
                        functions.computerSpeak(change[1], currentUser)
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
                        text = functions.converToText(change[1])
                        functions.computerSpeak(text)
                        backendJson["readFileSuccess"] = "true"
                        writeToBackendJson()
                    except:
                        print("Failed to read file")
                        backendJson["readFileSuccess"] = "false"
                        writeToBackendJson()

                elif(change[0] == "importFilePath"):
                    print("import file")
                    try:
                        text = functions.registerProfileFromImport(change[1])
                        backendJson["importFileSuccess"] = "true"
                        writeToBackendJson()
                    except:
                        print("Failed to import file")
                        backendJson["importFileSuccess"] = "false"
                        writeToBackendJson()
        time.sleep(1)
    print(functions.convertImageToText("C:\\Users\\urasa\\Pictures\\try.jpg"))