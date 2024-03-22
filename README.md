# Anima V3
## Index
- [Introdcution](#introdcution)
- [System Architecture](#system-architecture)
  - [Frontend](#frontend)
  - [Backend](#backend)
  - [JSON Files](#json-files)
  - [Local Database](#local-database)
- [Key Features](#key-features)
- [UI Example](#ui-example)
- [Usage](#usage-in-visual-studio)
- [Anima CLI](#anima-cli)
- [Deployment Manual](#deployment-manual)
- [User Manual](#user-manual)
- [Supported Languages](#supported-languages)
- [Packages](#packages)
- [License](#license)


## Introdcution
Anima V3 is an offline and secure voice banking solution. Anima V3 preserves the person's voice so that even when their natural voice is lost, they can still communicate and maintain their sense of self.  
With Anima V3, you can record your voice, and we can create a blueprint of it, giving those in need a way to speak with Anima V3 is our vision.
## System Architecture
The design of Anima v3 consists of a frontend, backend, JSON files and a local data folder.
![System architecture](/doc/img/overview.png "System architecture")
### Frontend
The frontend is built on the .NET framework. It serves as the user interface tool where users interact with the system. It provides a user-friendly environment for engaging with the system's features and functionalities, facilitating smooth communication between users and the backend processes.
### Backend
The backend component operates as a subprocess run by the frontend, as an executable compiled from Python. The backend houses the essential logic for text-to-speech, OCR, and animaprofile generation. It operates independently, efficiently processing text and images to synthesize audio while seamlessly integrating with the frontend.
### JSON Files
The JSON files serve as a communication bridge between the frontend and backend.
- Frontend JSON File (frontend.json):  
Contains essential information about the current user, such as their name, preferred language, and unique identifiers for audio files. The frontend writes data to this file, while the backend reads from it to determine the current status and perform necessary actions accordingly.
- Backend JSON File (backend.json): 
Records the success status of various backend operations, including profile creation, file reading, speech synthesis, and more. Additionally, it maintains information about the readiness of the backend system and the length of audio files processed. The frontend writes to this file, while the backend reads from it to report operation outcomes and system readiness.

### Local Database
For optimal user privacy, we maintain a local database within an 'animaprofiles' folder. This database stores user data in output.wav files, ensuring that the most recent audio generated is consistently updated and overwritten, aligning with our commitment to safeguarding user information.  
## Key Features
The key features of Anima can be split into 2 sections:  
### Voice Setup
- Record Voice
- Recorded Voice Playback
- Restart Voice Recording
- Choose Language
### Text to Speech
- Read from textbox
- Read from PDF/image
- Select Voice
- Import Voice
- Redo Voice
## UI Example
![home page](/doc/img/home.png "home page")
![tts page](/doc/img/tts.png "tts page")
## Usage in Visual Studio
To use it in Visual Studio, the backend([animaForBackend.py](/backend/ANIMA/animaForBackend.py)) has to be started manually, after starting the frontend.
## Anima CLI
[Anima CLI](/backend/ANIMA "/backend/ANIMA") can be used as a standalone Python program, which also provides APIs to be incorporated in other projects.
## Deployment Manual
[Deployment manual.pdf](/doc/Deployment_Manual.pdf "Deployment manual")
## User Manual
[User Manual.pdf](/doc/User_manual.pdf "User Manual")
## Supported Languages
| Language | Status |
| --- | :---: |
| English (en) | ✅ |
| French (fr-fr) | ✅ |
| Portuguese (pt-br) | ✅ |
## Packages
A lot of Python libraries used in the backend of Anima V3, they are key in the functionalities of Anima v3. We selected these packages due to their open-source nature, aligning with our commitment to provide Anima v3 free of charge to all users.
Here are 3 very important ones used in Anima v3.
### Coqui TTS
[Public GitHub Repository](https://github.com/coqui-ai/TTS)  
[Documentation](https://docs.coqui.ai/en/latest/)  
Coqui TTS is a Python library for text-to-speech (TTS) synthesis, utilized for high-quality speech generation. It offers TTS models for English, French and Portuguese, matching our requirement of supporting multiple languages.
### EasyOCR
[Public GitHub Repository](https://github.com/JaidedAI/EasyOCR)  
[Documentation](https://www.jaided.ai/easyocr/documentation/)  
EasyOCR is a Python library designed for optical character recognition (OCR) tasks, enabling the extraction of text from images and PDFs. We used EasyOCR for its multi-language support, including English, French, Portuguese which suits the needs of Anima v3.
### PyOxidizer
[Public GitHub Repository](https://github.com/indygreg/PyOxidizer)  
[Documentation](https://pyoxidizer.readthedocs.io/en/stable/)  
PyOxidizer is a Python library and packaging tool that creates standalone executables, simplifying deployment for Anima v3. It was the key component of the complilation of the backend of Anima v3 which was written in Python.
## License
Tesseract OCR uses [Apache License Version 2.0](/doc/Tesseract%20OCR%20LICENSE/LICENSE).  
EasyOCR uses [Apache License Version 2.0](/doc/Tesseract%20OCR%20LICENSE/LICENSE).  
Pyoxidizer uses [MPL-2.0 License](/doc/Coqui-TTS%20LICENSE/LICENSE).  
Coqui-TTS uses [MPL-2.0 License](/doc/Coqui-TTS%20LICENSE/LICENSE).  


