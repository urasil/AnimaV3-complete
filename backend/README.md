# ANIMA CLI
## Voice Integration and TTS of MotionInput

ANIMA is a voicebanking tool aimed at patients with MND or ALS, allowing them to preserve their voice. It uses Coqui TTS to process an synthesise voice. ANIMA will work as a standalone program, as  well as provide a voice integration layer to UCL MotionInput. 

## Features

- Voice banking and sysnthesis in English, French and Portuguese
- Built-in image OCR with direct speech from selection
- Generation of a custom ANIMA voiceprint file for distribution and preservation


## Installation

ANIMA CLI currently requires the following to run:
- Python 3.9.X
- ffmpeg (free)
- Tesseract-OCR exe (C:\\Program Files) (optional - required for some files kept from v1 but redundant in v2)


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

