import json
import sys
import argparse
from argparse import RawTextHelpFormatter
from src.ANIMA import ANIMA
from src.manage_audio import AudioManager
from src.pdf_to_txt import PdfToStrings
from src.img_to_txt import ImgToStrings

def main():
    # Create the parser
    description = """
    This is a CLI to run ANIMA and to demonstrate its current features.

    Example commands are shown below:\n

    #list downloaded models for TTS and voice cloning
        $py cli.py --list_models
        
    #list all saved audios 
        $py cli.py --list_audios

    #voice cloning
        $py cli.py --voice_clone --text "example text" --voice_name "voice_name" --lang "lang_code" --out_file "out_filename.wav"

    #tts
        $py cli.py --tts --text "example text" --lang "lang_code" --out_file "out_filename.wav"

    # voice_cloning with pdf
        $py cli.py --voice_clone_pdf --text_file "filename" --voice_name "voice_name" --out_file "out_filename.wav"

    # voice_cloning with image
        $py cli.py --voice_clone_img --text_file "filename" --voice_name "voice_name" --out_file "out_filename.wav"

    # tts with pdf
        $py cli.py --tts_pdf --text_file "filename" --out_file "out_filename.wav"

    # tts with image
        $py cli.py --tts_img --text_file "filename" --out_file "out_filename.wav"

    # add language model in "models.json"
        $py cli.py --add_model --model_type "model_type" --lang "lang_code" --model_path "model_path"

    #create voice
        $py cli.py --create --voice_name "voice_name" --lang "lang_code" --in_file "in_filename"

    # create voice with multiple files
        $py cli.py --create --voice_name "voice_name" --lang "lang_code" --in_file_list "in_file1" "in_file2"

    #edit existing voice name
        $py cli.py --edit_voice_name --in_file "in_filename.wav" --out_file "out_filename.wav"
    
    #edit existing audio name
        $py cli.py --edit_audio_name --voice_name "voice_name" --lang "lang_code" --in_file "in_filename.wav" --out_file "out_filename.wav"

    #play audio 
        $py cli.py --play --voice_name "voice_name" --lang "lang_code" --out_file "out_filename"

    #delete voice
        $py cli.py --delete_voice --voice_name "voice_name"

    #delete language
        $py cli.py --delete_lang --voice_name "voice_name" --lang "lang_code"

    #delete audio
        $py cli.py --delete_audio --voice_name "voice_name" --lang "lang_code" --in_file "in_filename"
    """

    parser = argparse.ArgumentParser(prog="anima", description=description, allow_abbrev=False, epilog="Enjoy ANIMA ;)", formatter_class=RawTextHelpFormatter)

    # Add arguments
    parser.add_argument("--list_models",
                        action="store_true",
                        help="List all downloaded TTS models and voice cloning models.")

    parser.add_argument("--list_audios",
                        action="store_true",
                        help="List all saved audios (.wav).")

    parser.add_argument("--voice_clone",
                        nargs='?',
                        const=True,
                        help="Call voice clone.")

    parser.add_argument("--tts",
                        nargs='?',
                        const=True,
                        help="Call TTS.")

    parser.add_argument("--voice_clone_pdf",
                        nargs='?',
                        const=True,
                        help="Convert pdf to voice-clone speech.")

    parser.add_argument("--voice_clone_img",
                        nargs='?',
                        const=True,
                        help="Convert image to voice-clone speech.")

    parser.add_argument("--tts_pdf",
                        nargs='?',
                        const=True,
                        help="Convert pdf to tts.")

    parser.add_argument("--tts_img",
                        nargs='?',
                        const=True,
                        help="Convert image to tts.")

    parser.add_argument("--play",
                        nargs='?',
                        const=True,
                        help="Play audio.")


    parser.add_argument("--add_model",
                        nargs='?',
                        const=True,
                        help="Add tts model or voice cloning model.")

    parser.add_argument("--create",
                        nargs='?',
                        const=True,
                        help="Create voice for voice cloning")

    parser.add_argument("--edit_voice_name",
                        nargs='?',
                        const=True,
                        help="Edit voice name")

    parser.add_argument("--edit_audio_name",
                        nargs='?',
                        const=True,
                        help="Edit audio name")

    parser.add_argument("--delete_voice",
                        nargs='?',
                        const=True,
                        help="Delete voice")

    parser.add_argument("--delete_lang",
                        nargs='?',
                        const=True,
                        help="Delete language of a voice")
    
    parser.add_argument("--delete_audio",
                        nargs='?',
                        const=True,
                        help="Delete audio")
    
    parser.add_argument("--text",
                        type=str,
                        default=None,
                        help="Text for voice conversion or voice cloning.")

    parser.add_argument("--voice_name",
                        type=str,
                        default="default_speaker",
                        help="Voice input for speech generation.")

    parser.add_argument("--out_file",
                        type=str,
                        default=None,
                        help="Output .wav filename.")

    parser.add_argument("--lang",
                        type=str,
                        default=None,
                        help="Language for voice conversion or voice cloning.")

    parser.add_argument("--text_file",
                        type=str,
                        default=None,
                        help="Text filename.")

    parser.add_argument("--model_type",
                        type=str,
                        default=None,
                        help="TTS model or voice cloning model.")

    parser.add_argument("--model_path",
                        type=str,
                        default=None,
                        help="Deep learning models path for speech generation.")

    parser.add_argument("--in_file",
                        type=str,
                        default=None,
                        help="Input .wav filename.")

    parser.add_argument("--in_file_list",
                        nargs="+",
                        type=str,
                        default=[],
                        help="Input .wav files.")


    parser.add_argument("--create_profile",
                        nargs='?',
                        const=True,
                        help="Create voice profile (experimental).")

    parser.add_argument("--use_profile",
                        nargs='?',
                        const=True,
                        help="Generate speech using voice profile (experimental).")

    parser.add_argument("--profile_path",
                        nargs='?',
                        const=True,
                        help="Path to voice profile.")

    parser.add_argument("--input_voice_path",
                        nargs='?',
                        const=True,
                        help="Path to recorded input voice.")
    
    parser.add_argument("--use_json",
                        nargs='?',
                        const=True,
                        help="Use JSON file to run TTS. Requires path of JSON file.")

    # Execute the parse_args() method
    args = parser.parse_args()

    anima = ANIMA()
    audio_manager = AudioManager()
    pdf_to_str = PdfToStrings()
    img_to_str = ImgToStrings()

    #list models for TTS and voice cloning
    if args.list_models:
        anima.list_language_models()
        sys.exit()

    #list all saved audios 
    if args.list_audios:
        audio_manager.list_audio()
        sys.exit()

    lang = args.lang
    text = args.text 
    text_file = args.text_file
    voice_name = args.voice_name
    in_file_list = args.in_file_list
    in_file = args.in_file
    out_file = args.out_file
    model_type = args.model_type
    model_path = args.model_path

    profile_path = args.profile_path
    input_voice_path = args.input_voice_path
    json_path = args.use_json
    
    # py cli.py --create_profile --profile_path "full path including .animaprofile to write to" --input_voice_path "full path to .wav file" --lang "en"

    if args.create_profile is not None:
        anima.create_profile(profile_path=profile_path, speaker_wav=input_voice_path, lang=lang)
        sys.exit()

    # py cli.py --use_profile --profile_path "full path to .animaprofile" --lang "en"

    if args.use_profile is not None:
        anima.use_profile_continuous(profile_path=profile_path, lang=lang)
        sys.exit()

    # py cli.py --use_json "full path to json file"

    if args.use_json is not None: 
        #TODO MOVE TO ANIMA.PY 
        f = open(str(json_path), "r")
        data = json.load(f)
        print(data)
        f.close()

        profile_path = data["profile"]
        text = data["text"]
        lang = data["lang"]

        anima.use_profile_with_string(profile_path=profile_path, lang=lang, text=text)
        sys.exit()

    if args.voice_clone is not None:
        in_dir, out_dir = audio_manager.dir_audio_to_folder(lang, out_file, voice_name)
        anima.voice_clone(text, in_dir, out_dir, lang)
        sys.exit()
    
    if args.tts is not None:
        in_dir, out_dir = audio_manager.dir_audio_to_folder(lang, out_file)
        anima.tts_default_voice(text, out_dir, lang)
        sys.exit()

    if args.voice_clone_pdf is not None:
        text = pdf_to_str.pdf_to_str(text_file)
        in_dir, out_dir = audio_manager.dir_audio_to_folder("en", out_file, voice_name)
        anima.voice_clone(text, in_dir, out_dir, "en")
        sys.exit()

    if args.voice_clone_img is not None:
        text = img_to_str.img_to_str(text_file, "eng")
        in_dir, out_dir = audio_manager.dir_audio_to_folder("en", out_file, voice_name)
        anima.voice_clone(text, in_dir, out_dir, "en")
        sys.exit()

    if args.tts_pdf is not None:
        text = pdf_to_str.pdf_to_str(text_file)
        in_dir, out_dir = audio_manager.dir_audio_to_folder("en", out_file)
        anima.tts_default_voice(text, out_dir, "en")
        sys.exit()

    if args.tts_img is not None:
        text = img_to_str.img_to_str(text_file, "eng")
        in_dir, out_dir = audio_manager.dir_audio_to_folder("en", out_file)
        anima.tts_default_voice(text, out_dir, "en")
        sys.exit()

    if args.add_model is not None:
        anima.add_language_model(model_type, lang, model_path)
        sys.exit()

    if args.create is not None:
        if in_file is not None:
            audio_manager.create_voice(voice_name, lang, in_file)
            sys.exit()
        
        if in_file_list is not None:
            file_path = audio_manager.combine_wav_files(in_file_list)
            audio_manager.create_voice(voice_name, lang, file_path)
            sys.exit()

    if args.edit_voice_name is not None:
        audio_manager.edit_voice_name(in_file, out_file)
        sys.exit()

    if args.edit_audio_name is not None:
        audio_manager.edit_audio_name(lang, in_file, out_file, voice_name)
        sys.exit()

    if args.play is not None:
        audio_manager.play_audio(lang, out_file, voice_name)
        sys.exit()

    if args.delete_voice is not None:
        audio_manager.delete_voice(voice_name)
        sys.exit()

    if args.delete_lang is not None:
        audio_manager.delete_voice(voice_name, lang)
        sys.exit()

    if args.delete_audio is not None:
        audio_manager.delete_voice(voice_name, lang, in_file)
        sys.exit()

def test_main():
    anima = ANIMA()
    anima.use_profile_continuous(profile_path='data/yunus.animaprofile', lang='en')
    sys.exit()

def testProfileCreation():
    anima = ANIMA()
    anima.create_profile(profile_path="data/yunus.animaprofile", speaker_wav="C:\\Users\\urasa\\Downloads\\yunus.wav", lang="en")

if __name__ == "__main__":
    #testProfileCreation()
    #test_main()
    main()