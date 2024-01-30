# ANIMA
## Voice Integration and TTS of MotionInput

ANIMA is a voicebanking tool aimed at patients with MND or ALS, allowing them to preserve their voice. It uses Coqui TTS to process an synthesise voice. ANIMA will work as a standalone program, as  well as provide a voice integration layer to UCL MotionInput. ANIMA currently only supports Enlish but has the potential and will eventually have multi-language support.

## Features

- Voice banking and sysnthesis in English
- Built-in image OCR with direct speech from selection
- Generation of a custom ANIMA voiceprint file for distribution and preservation


## Installation

ANIMA currently requires the following to run:
- Python 3.9.11
- Visual studio C++ build
- ffmpeg (free)
- Tesseract-OCR exe (C:\\Program Files) (optional - required for some files kept from v1 but redundant in v2)
- Docker (optional - for building and running the back-end with the front-end MFC)

Then run:
```sh
 $ pip install -r requirements.txt 
```

# Using ANIMA

## CLI options 
### Creating an ANIMA profile/voiceprint file
```sh
py cli.py --create_profile --profile_path "[full path including .animaprofile to write to]" --input_voice_path "[full path to .wav file]" --lang "en"
```

### Live CLI speech synthesis using ANIMA profile/voiceprint file
```sh
py cli.py --use_profile --profile_path "[full path to .animaprofile]" --lang "en"
```

### Speech synthesis using JSON file
```sh
py cli.py --use_json "[full path to .json file]"
```

Requires JSON file in format:
```json
{
    "profile": "[full path to .animaprofile]",
    "text": "[Text to be converted to speech]",
    "lang": "en"
}
```

### Voice cloning (redundant - from v1)

```sh
 $ py cli.py --voice_clone --text "example text" --voice_name "voice_name" --lang "lang_code" --out_file "out_filename.wav"
```

### TTS (redundant - from v1)
```sh
$ py cli.py --tts --text "example text" --lang "lang_code" --out_file "out_filename.wav"
```

### "audios" file structure (redundant - from v1)
    | "audios"
        | "default_speaker"
            | lang
                | out_audio_file
        | voice_name
            | lang
                | "input_voice.wav"
                | out_audio_file
            
 ### "models.json" data structure (redundant - from v1)
    {
        "TTS_models": {
            "lang": {
                "tts_model": "tts_models/lang/dataset/model_name"
            }
        },
        "voice_cloning_models": {
            "lang": {
                "tts_model": "tts_models/lang/dataset/model_name"
            }
        }
    }

## UI option

ANIMA also has a front-end MFC application that can be used to interact with the app in a user-friendly way. To run the UI option, it has to be built first.

To build the python-MFC app, you must first understand the system architecture. 

The steps for building are: 


1. Navigate using terminal to the project directory
2. Create a python virtual environment with the correct version of python
3. Ensure that a "data" file is present with the following structure:
##### 
    | data
        | data.json
        | status.json
        | profiles
            | "profileName".animaProfile
(This file structure is already present in this repo)

4. Launch docker

5. Create the docker image using the python
```sh
docker image build -t anima .
```

6. Create a .tar of the image to place into the MFC app (optional for personal use, required to create distributable file)
```sh
docker save anima > Anima.tar
```
This will produce a file named "Anima.tar". Place this into the MFC compiled ANIMA folder.

7. Download the MFC front-end compiled executable for ANIMA and run it using the "Anima.exe"