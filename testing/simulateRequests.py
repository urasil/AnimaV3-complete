import json
import time
import os


# Load the frontend.json and backend.json file
def loadFrontendJson():
    with open("../frontend.json", "r") as json_file:
        frontendJson = json.load(json_file)
    return frontendJson

def writeFrontendJson(frontendJson):
    with open("../frontend.json", "w") as json_file:
        json.dump(frontendJson, json_file)

def loadBackendJson():
    with open("../backend.json", "r") as json_file:
        backendJson = json.load(json_file)
    return backendJson

def writeBackendJson(backendJson):
    with open("../backend.json", "w") as json_file:
        json.dump(backendJson, json_file)

# Test speaking functionality
def testSpeakFunctionality():
    frontendJson = loadFrontendJson()
    frontendJson["speakID"] = "test_speak"
    frontendJson["content"] = "This is a test message."
    writeFrontendJson(frontendJson)
    time.sleep(5)  # Wait for backend processing

    backendJson = loadBackendJson()
    assert backendJson["speechSuccess"] == "true"
    print("Speaking functionality test passed.")

# Test registering a new profile
def testRegisterNewProfile():
    frontendJson = loadFrontendJson()
    frontendJson["speakerName"] = "TestUser"
    writeFrontendJson(frontendJson)
    time.sleep(5) 

    backendJson = loadBackendJson()
    assert backendJson["profileCreationSuccess"] == "true"
    print("Registering new profile test passed.")

# Test reading a file and speaking its content
def testReadFileAndSpeak():
    frontendJson = loadFrontendJson()
    frontendJson["readFileID"] = "test_read"
    frontendJson["readFilePath"] = "easyText.png"
    writeFrontendJson(frontendJson)
    time.sleep(5)  

    backendJson = loadBackendJson()
    assert backendJson["readFileSuccess"] == "true"
    print("Reading file and speaking test passed.")

# Test importing an animaprofile
def testImportAnimaprofile():
    frontendJson = loadFrontendJson()
    frontendJson["importFilePath"] = "C:\\Users\\urasa\\Downloads\\record.wav"
    writeFrontendJson(frontendJson)
    time.sleep(5) 
    
    backendJson = loadBackendJson()
    assert backendJson["importSuccess"] == "true"
    print("Importing animaprofile test passed.")

# Test stopping speech
def testStopSpeech():
    frontendJson = loadFrontendJson()
    frontendJson["stopSpeakTrigger"] = "true"
    writeFrontendJson(frontendJson)
    time.sleep(5) 

    backendJson = loadBackendJson()
    assert backendJson["stopSpeakSuccess"] == "true"
    print("Stopping speech test passed.")

# Test changing speaking language
def testChangeLanguage():
    frontendJson = loadFrontendJson()
    frontendJson["language"] = "fr-fr"  
    writeFrontendJson(frontendJson)

    time.sleep(5) 
    print("Changing language test passed.")

# Main test function to run all tests
def runAllTests():
    try:
        createNewUser()
        testSpeakFunctionality()
        testRegisterNewProfile()
        testReadFileAndSpeak()
        testImportAnimaprofile()
        testStopSpeech()
        testChangeLanguage()
    except Exception as e:
        print("Error occurred during testing:", e)

# Entry point for running tests
if __name__ == "__main__":
    runAllTests()