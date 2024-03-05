import json
import sys
import time
from src.ANIMA import ANIMA
from src.manage_audio import AudioManager
from src.pdf_to_txt import PdfToStrings
from src.img_to_txt import ImgToStrings
from observer import Observer
import os
import sounddevice as sd
import numpy as np

frontendJsonFilePath = "../../frontend.json"
backendJsonFilePath = "../../backend.json"
language = "en"  # default language
animaProfilesPath = "../../animaProfiles/"
detectPeriod = 0.5  # unit of second

with open(frontendJsonFilePath, "r") as json_file:
    frontendJson = json.load(json_file)

with open(backendJsonFilePath, "r") as json_file:
    backendJson = json.load(json_file)

language = frontendJson["language"] # update the language to match with json
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
        self.audioLength = 0  # the length of generated audio, unit of second
        self.language = language

    def computerSpeak(self, text, currentUser):
        """
        Use animaprofile to speak
        """
        print(currentUser+" speaking in "+self.language)
        profilePath = animaProfilesPath + currentUser + ".animaprofile"
        wav = self.anima.wav_from_profile(profile_path=profilePath, lang=self.language, text=text)
        self.audioLength = len(wav)/16000
        sd.play(np.array(wav), 16000)

    def registerProfile(self):
        """
        Use a recorded wav to generate an animaprofile
        """
        newUser = frontendJson["speakerName"]
        self.anima.create_profile(profile_path=f"{animaProfilesPath}{newUser}.animaprofile", speaker_wav="../../output.wav", lang=self.language)
    
    def convertToText(self, path):
        """
        Use OCR to convert a pdf/image into texts
        """
        extension = path.split(".")[-1]
        print(extension)
        if(extension == "pdf"):
            return self.pdfConverter.pdf_to_str(path)
        elif(extension == "jpg" or extension == "jpeg" or extension == "png"):
            return self.imageConverter.img_to_str(img_path=path, lang=self.language)
        else:
            return False
    
    def registerProfileFromImport(self, path):
        name = path.split("\\")[-1].split(".")[0]
        self.anima.create_profile(profile_path=f"{animaProfilesPath}{name}.animaprofile", speaker_wav=path, lang=self.language)

    def stopSpeak(self):
        sd.stop()

    def changeLanguage(self,lang):
        self.language = lang
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
                                backendJson["audioLength"] = str(functions.audioLength)
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
                
                # A file to be read
                if ("readFileID" in changes):
                    if("readFilePath" in changes):
                        print("read file and speak")
                        if changes["readFilePath"] != "":
                            try:
                                frontendJson["readFilePath"] = changes["readFilePath"]
                                text = functions.convertToText(changes["readFilePath"])
                                functions.computerSpeak(text, currentUser)
                                backendJson["readFileSuccess"] = "true"
                                backendJson["audioLength"] = str(functions.audioLength)
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
                # An animaprofile to be imported
                if("importFilePath" in changes):
                    
                    if changes["importFilePath"] != "":
                        try:
                            print("import file")
                            frontendJson["importFilePath"] = changes["importFilePath"]
                            text = functions.registerProfileFromImport(changes["importFilePath"])
                            backendJson["importSuccess"] = "true"
                            writeToBackendJson()
                        except Exception as e:
                            print("Failed to import file ", e)
                            backendJson["importSuccess"] = "false"
                            writeToBackendJson()
                # Stop the current speak
                if("stopSpeakTrigger" in changes):
                    print("stop speak")
                    if changes["stopSpeakTrigger"] != "":
                        try:
                            if changes["stopSpeakTrigger"] == "true":
                                functions.stopSpeak()
                                frontendJson["stopSpeakTrigger"] = changes["stopSpeakTrigger"]
                                backendJson["stopSpeakSuccess"] = "true"
                                writeToBackendJson()
                        except Exception as e:
                            print("Failed to stop speak: ", e)
                            backendJson["stopSpeakSuccess"] = "false"
                            writeToBackendJson()
                
                # change speaking langauge
                if("language" in changes):
                    print("Language changed")
                    if changes["language"] != "":
                        try:
                            functions.changeLanguage(changes["language"])
                        except Exception as e:
                            print("Failed to change speaking language", e)
                            
            time.sleep(detectPeriod)
    except:
        quitBackend()

if __name__ == "__main__":
    main()