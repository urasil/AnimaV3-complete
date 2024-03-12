import os
import easyocr


class ImgToStrings():
    """
    Converts .jpg file text to strings.
    """
    def __init__(self) -> None:
        self.lang = ["en", "fr", "pt"]

    def img_to_str(self, img_path: str, lang: str):
        """
        Convert text in .jpg image to strings. English, Portuguese, French, and German are available.

        Args:
            img_path (str): image filepath in path/image format. Only allows .jpg file
            lang (str): language code
        """
        self.__check_lang(lang)

        reader = easyocr.Reader([lang])
        try:
            textToReturn = ""
            result = reader.readtext(img_path)
            for(bbox, text, prob) in result:
                if text != None:
                    print(text)
                    textToReturn += (text + " ")
            textToReturn = textToReturn.strip()
            print(textToReturn)
        except:
            print("Error: Unable to read text from image.")
            textToReturn = ""
        return textToReturn

    
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