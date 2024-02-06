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


def readyBackend():
    backendJson["backendReady"] = "true"
    writeToBackendJson()

def quitBackend():
    backendJson["backendReady"] = "false"
    writeToBackendJson()

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
    
    def convertToText(self, path):
        extension = path.split(".")[-1]
        print(extension)
        if(extension == "pdf"):
            return self.pdfConverter.pdf_to_str(path)
        elif(extension == "jpg" or extension == "jpeg" or extension == "png"):
            return self.imageConverter.img_to_str(path, "en")
        else:
            return False
    
    def registerProfileFromImport(self, path):
        name = path.split("\\")[-1].split(".")[0]
        self.anima.create_profile(profile_path=f"../../animaProfiles/{name}.animaprofile", speaker_wav=path, lang="en")

def main():
    currentUser = frontendJson["nameOfCurrentUser"]
    observer = Observer(frontEndJsonPath=frontendJsonFilePath)
    functions = BackendFunctionalites()
    readyBackend()
    try:
        while(True):
            changes = observer.detectChanges()
            print(changes)
            if(changes):
                # Setting which voice profile to use
                if("nameOfCurrentUser" in changes):
                    currentUser = changes["nameOfCurrentUser"]
                    frontendJson["nameOfCurrentUser"] = changes["nameOfCurrentUser"]


                # Talking
                if("speakID" in changes):  #speakID changes means that user sends a speaking request, though the same content
                    if("content" in changes): # different content from previous request
                        print("speak")
                        if changes["content"] != "":
                            try:
                                frontendJson["content"] = changes["content"]
                                functions.computerSpeak(frontendJson["content"], currentUser)
                                backendJson["speechSuccess"] = "true"
                                writeToBackendJson()
                            except Exception as e:
                                print("Failed to speak", e)
                        else:
                            backendJson["speechSuccess"] = "true"   #accept empty text but do nothing
                            writeToBackendJson()
                        
                    else:  # same content from previous request but new request
                        print("speak")
                        if frontendJson["content"] != "":
                            try:
                                functions.computerSpeak(frontendJson["content"], currentUser)
                                backendJson["speechSuccess"] = "true"
                                writeToBackendJson()
                            except Exception as e:
                                print("Failed to speak", e)
                        else:
                            backendJson["speechSuccess"] = "true"  #accept empty text but do nothing
                            writeToBackendJson()



                # New profile to be registered
                if("speakerName" in changes):
                    print("register profile")
                    try:
                        frontendJson["speakerName"] = changes["speakerName"]
                        functions.registerProfile()
                        backendJson["profileCreationSuccess"] = "true"
                        writeToBackendJson()
                    except Exception as e:
                        print("Failed to register profile ",e)
                if ("readFileID" in changes):
                    if("readFilePath" in changes):
                        print("read file and speak")
                        if changes["readFilePath"] != "":
                            try:
                                frontendJson["readFilePath"] = changes["readFilePath"]
                                text = functions.convertToText(changes["readFilePath"])
                                functions.computerSpeak(text, currentUser)
                                backendJson["readFileSuccess"] = "true"
                                writeToBackendJson()
                            except Exception as e:
                                print("Failed to read file ",e)
                                backendJson["readFileSuccess"] = "false"
                                writeToBackendJson()
                        else:
                            raise ValueError("Invalid path")
                    else:  # same file path but another function call
                        print("read file and speak")
                        if frontendJson["readFilePath"] != "":
                            try:
                                text = functions.convertToText(frontendJson["readFilePath"])
                                functions.computerSpeak(text, currentUser)
                                backendJson["readFileSuccess"] = "true"
                                writeToBackendJson()
                            except Exception as e:
                                print("Failed to read file ",e)
                                backendJson["readFileSuccess"] = "false"
                                writeToBackendJson()
                        else:
                            raise ValueError("Invalid path")

                if("importFilePath" in changes):
                    print("import file")
                    if changes["importFilePath"] != "":
                        try:
                            frontendJson["importFilePath"] = changes["importFilePath"]
                            text = functions.registerProfileFromImport(changes["importFilePath"])
                            backendJson["importSuccess"] = "true"
                            writeToBackendJson()
                        except Exception as e:
                            print("Failed to import file ", e)
                            backendJson["importSuccess"] = "false"
                            writeToBackendJson()
            time.sleep(1)
    except:
        quitBackend()

if __name__ == "__main__":
    main()