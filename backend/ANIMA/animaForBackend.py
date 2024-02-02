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
animaProfilesPath = "../../animaProfiles/"  # directory of anime profiles

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
        profilePath = animaProfilesPath + currentUser + ".animaprofile"
        self.anima.use_profile_to_talk(profile_path=profilePath, text=text, lang=language)

    def registerProfile(self):
        newUser = frontendJson["speakerName"]
        self.anima.create_profile(profile_path=f"{animaProfilesPath}{newUser}.animaprofile", speaker_wav="../../output.wav", lang=language)
    
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
    observer = Observer(frontEndJsonPath=frontendJsonFilePath)
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
                    if change[1] != "":
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
                    if change[1] != "":
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
                    if change[1] != "":
                        try:
                            text = functions.registerProfileFromImport(change[1])
                            backendJson["importSuccess"] = "true"
                            writeToBackendJson()
                        except:
                            print("Failed to import file")
                            backendJson["importSuccess"] = "false"
                            writeToBackendJson()
        time.sleep(1)
    print(functions.convertImageToText("C:\\Users\\urasa\\Pictures\\try.jpg"))