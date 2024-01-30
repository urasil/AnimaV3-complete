import os
from pathlib import Path
from playsound import playsound
from pathlib import PureWindowsPath
from pydub import AudioSegment
from typing import List


class AudioManager():
    """
    Manager to organise audio folders and files in "audios".

    The "audios" structure is:
        | "default_speaker"
            | lang
                | out_audio_file
        | voice_name
            | lang
                | "input_voice.wav"
                | out_audio_file
    """
    def __init__(self):
        self.parent_dir = os.getcwd()
        self.audios_dir = os.path.join(self.parent_dir, "audios")

    def play_audio(self, lang: str, file_path: str, voice_name="default_speaker"):
        """
        Play saved audio. 

        Args: 
            lang (str): language code
            file_path (str): path to audio in path/out_file.wav format
            voice_name (str, optional): name given to voice. Default to default_speaker
        """
        curr_dir = os.path.join(self.audios_dir, voice_name, lang, file_path)
        print(f"The file {file_path} is playing...")
        playsound(curr_dir)


    def combine_wav_files(self, wav_files: List[str]):
        """
        Combine .wav files.

        Args:
            wav_files (list[str]): A list of wav files.
        """
        init_file = wav_files[0]
        len_wav_files = len(wav_files)
        init_wav = AudioSegment.from_wav(init_file)

        for wav in range(1, len_wav_files):
            curr_file = wav_files[wav]
            curr_wav = AudioSegment.from_wav(curr_file)
            init_wav += curr_wav
            os.remove(curr_file)

        os.remove(init_file)
        parent_path = os.getcwd()
        curr_path = os.path.join(parent_path, "input_voice.wav")
        init_wav.export(curr_path, format="wav")

        print(f"The files {wav_files} are all combined!")

        return curr_path


    def list_audio(self):
        """
        List all saved audios. 
        """
        print("The audios are listed below:")
        for voice in os.listdir(self.audios_dir):
            print(f"{voice}:")
            lang_dir = os.path.join(self.audios_dir, voice)
            for lang in os.listdir(lang_dir):
                print(f"  {lang}:")
                audio_dir = os.path.join(self.audios_dir, voice, lang)
                for audio in os.listdir(audio_dir):
                    print(f"    {audio}")
            print("")


    def edit_voice_name(self, voice_name: str, new_voice_name: str):
        """
        Edit voice_name folder name. 

        Args:
            voice_name (str): original voice name
            new_voice_name (str): new voice name
        """
        valid_voice = self.__check_path(voice_name)
        invalid_new_voice = self.__check_path(new_voice_name)

        if not valid_voice:
            raise self.InvalidPath()
        if invalid_new_voice:
            raise self.InvalidDuplicatePath()

        curr_voice_name = os.path.join(self.audios_dir, voice_name)
        updated_voice_name = os.path.join(self.audios_dir, new_voice_name)
        Path(curr_voice_name).rename(updated_voice_name)

        print(f"The voice {voice_name} is changed to {new_voice_name}")


    def edit_audio_name(self, lang: str, audio_name: str, new_audio_name: str, voice_name="default_speaker"):
        """
        Edit audio file name. 

        Args:
            lang (str): language code
            audio_name (str): original audio name
            new_audio_name (str): new audio name
            voice_name (str): name given to voice. Default to default_speaker
        """
        valid_audio = self.__check_path(voice_name, lang, audio_name)
        invalid_new_audio = self.__check_path(voice_name, lang, new_audio_name)

        if not valid_audio:
            raise self.InvalidPath()
        if invalid_new_audio:
            raise self.InvalidDuplicatePath()

        curr_audio_name = os.path.join(self.audios_dir, voice_name, lang, audio_name)
        updated_audio_name = os.path.join(self.audios_dir, voice_name, lang, new_audio_name)
        Path(curr_audio_name).rename(updated_audio_name)

        print(f"The voice {audio_name} is changed to {new_audio_name}")


    def create_voice(self, voice_name: str, lang: str, file_path:str):
        """
        Create a voice_name/lang folder with a input_voice.wav. 

        Args:
            voice_name (str): name given to voice
            lang (str): language code
            file_path (str): input voice path
        """

        valid_file_path = os.path.exists(file_path)
        if not valid_file_path:
            raise self.InvalidPath()

        valid_path = self.__check_path(voice_name, lang)
        curr_dir = os.path.join(self.audios_dir, voice_name, lang)

        if valid_path:
            raise self.InvalidDuplicatePath()


        os.makedirs(curr_dir)

        new_path = os.path.join(curr_dir, "input_voice.wav")
        Path(file_path).rename(new_path)

        print(f"The voice {voice_name} ({lang}) is created")


    def dir_audio_to_folder(self, lang, out_file, voice_name=None):
        """
        Controller to direct arguments for TTS or voice cloning.

        Args:
            lang (str): language code 
            out_file (str): output audio file path after TTS or voice cloning
            voice_name (str): name given to voice folder. Default to None
        """
        if voice_name is None:
            out_voice_dir = os.path.join(self.audios_dir, "default_speaker", lang, out_file)
            out_dir = PureWindowsPath(out_voice_dir).as_posix()
            return [None, out_dir]
        else:
            audio_dir = os.path.join(self.audios_dir, voice_name, lang)
            in_voice_dir = os.path.join(audio_dir, "input_voice.wav")
            out_voice_dir = os.path.join(audio_dir, out_file)

            in_dir = PureWindowsPath(in_voice_dir).as_posix()
            out_dir = PureWindowsPath(out_voice_dir).as_posix()
            return  [in_dir, out_dir]


    def delete_voice(self, voice_name: str, lang=None, file=None):
        """
        Delete voice, language or audio file. 

        Args: 
            voice_name (str): name given to voice
            lang (str): langauge code. Default to None
            file (str): audio file to be deleted. Default to None
        """

        valid_path = self.__check_path(voice_name, lang, file)

        if not valid_path:
            raise self.InvalidPath()

        if lang is None and file is None:
            if voice_name ==  "default_speaker":
                raise self.InvalidVoiceName()
            self.__delete_voice(voice_name)
        elif file is None:
            self.__delete_lang(voice_name, lang)
        else:
            self.__delete_audio(voice_name, lang, file)


    def __check_path (self, voice_name: str, lang=None, file=None):
        """
        Check if file path is valid.

        Args: 
            voice_name (str): name given to voice
            lang (str): langauge code. Default to None
            file (str): audio file. Default to None
        """
        if file is not None and lang is None:
            raise self.InvalidPath()

        curr_dir = self.__join_dir(voice_name, lang, file)
        valid_path = os.path.exists(curr_dir)

        return valid_path

    def __join_dir(self, voice_name: str, lang=None, file=None):
        """
        Combines all directory to one directory.

        Args: 
            voice_name (str): name given to voice
            lang (str): langauge code. Default to None
            file (str): audio file. Default to None
        """
        if file is None and lang is None:
            curr_dir = os.path.join(self.audios_dir, voice_name)
        elif file is None:
            curr_dir = os.path.join(self.audios_dir, voice_name, lang)
        else:
            curr_dir = os.path.join(self.audios_dir, voice_name, lang, file)

        return curr_dir


    def __delete_audio(self, voice_name: str, lang: str, file:str):
        """
        Delete audio file.

        Args: 
            voice_name (str): name given to voice
            lang (str): langauge code
            file (str): audio file
        """
        curr_dir = os.path.join(self.audios_dir, voice_name, lang, file)
        os.remove(curr_dir)

        print(f"The audio {file} in {voice_name} ({lang}) is deleted")


    def __delete_lang(self, voice_name: str, lang: str):
        """
        Delete langauge folder.

        Args: 
            voice_name (str): name given to voice
            lang (str): langauge code
        """
        curr_dir = os.path.join(self.audios_dir, voice_name, lang)
        for file in os.listdir(curr_dir):
            self.__delete_audio(voice_name, lang, file)
        os.rmdir(curr_dir)

        print(f"The language {lang} in {voice_name} is deleted")


    def __delete_voice(self, voice_name: str):
        """
        Delete voice folder.

        Args: 
            voice_name (str): name given to voice
        """
        curr_dir = os.path.join(self.audios_dir, voice_name)
        for file in os.listdir(curr_dir):
            self.__delete_lang(voice_name, file)
        os.rmdir(curr_dir)

        print(f"The voice {voice_name} is deleted")


    class InvalidPath(Exception):
        def __init__(self) -> None:
            super().__init__(f"Invalid file path: the file does not exist")


    class InvalidDuplicatePath(Exception):
        def __init__(self) -> None:
            super().__init__(f"Invalid file path: the file already exists")


    class InvalidVoiceName(Exception):
        def __init__(self) -> None:
            super().__init__(f"Invalid voice name: default speaker cannot be deleted")
