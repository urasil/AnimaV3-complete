import pytesseract
import os
from PIL import Image


class ImgToStrings():
    """
    Converts .jpg file text to strings.
    """
    def __init__(self) -> None:
        self.lang = ["eng", "por", "fra", "deu_frak"]
        pytesseract.pytesseract.tesseract_cmd = '../../tesseract/tesseract.exe'

    def img_to_str(self, img_path: str, lang: str):
        """
        Convert text in .jpg image to strings. English, Portuguese, French, and German are available.

        Args:
            img_path (str): image filepath in path/image format. Only allows .jpg file
            lang (str): language code
        """
        self.__check_file_path(img_path)
        self.__check_lang(lang)

        try:
            image = Image.open(img_path)
            text = pytesseract.image_to_string(image, lang=lang)
            return repr(text)
        except Exception as e:
            print(f"Error occurred during OCR: {e}")
            return ""


    def __check_file_path(self, img_path):
        """
        Check is file path is valid. 

        Args: 
            img_path (str): image filepath in path/image format. Only allows .jpg file
        """
        img_path_len = len(img_path) 

        if img_path_len < 4 or img_path[(img_path_len - 4): img_path_len] != ".jpg":
            raise self.InvalidFilename()

        parent_path = os.getcwd()
        valid_img_path = os.path.exists(img_path)

        if not valid_img_path:
            raise self.InvalidImgPath(img_path)

    
    def __check_lang(self, lang):
        """
        Check if language code is valid. 

        Args: 
            lang (str): language code
        """
        if lang not in self.lang:
            raise self.InvalidLang(lang)

    
    class InvalidLang(Exception):
        def __init__(self, lang) -> None:
            super().__init__(f"Invalid lang: The lang \"{lang}\" does not exist")


    class InvalidImgPath(Exception):
        def __init__(self, img_path) -> None:
            super().__init__(f"Invalid image file path: The image {img_path} path does not exist")


    class InvalidFilename(Exception):
        def __init__(self) -> None:
            super().__init__(f"Invalid image file format: the filename must end with \".jpg\"")
