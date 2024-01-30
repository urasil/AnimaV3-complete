import pdfplumber

class PdfToStrings():
    """
    Converts .pdf file text to strings with symbols removed. 
    """
    def pdf_to_str(self, file_path: str, file_page_line=None, password=None):
        """
        Convert text in pdf to strings with optional file page and line specified. 

        Args:
            file_path (str): pdf filepath in path/filename.pdf format
            file_page_line (dict["page": int, "lines": list[int]]): page and lines of the file.
            password (str): pdf file password
        """
        self.__check_file_path(file_path)

        with pdfplumber.open(file_path, password=password) as pdf:
            text_list = []
            text = ""
            
            text_list = self.__select_page_line(pdf, file_page_line)
            text = "".join(text_list)

            return text


    def __select_page_line(self, pdf: pdfplumber.pdf.PDF, file_page_line=None):
        """
        Selects text from page and lines specified. 

        Args: 
            pdf (pdfplumber.pdf.PDF): pdf file
            file_page_line (dict["page": int, "lines": list[int]]): page and lines of the file. 
        """
        pdf_pages = pdf.pages
        num_page = len(pdf_pages)
        text_list = []
        selected_line_list = []

        if file_page_line is not None:
            new_file_page_line = self.__pdf_page_index(file_page_line)

            page = new_file_page_line["page"]
            lines = new_file_page_line["lines"]

            curr_page = pdf.pages[page]
            curr_text = curr_page.extract_text()

            if lines != []:
                n = 0
                for text in curr_text.split("\n"):
                    n += 1
                    for line in lines:
                        if line <= 0:
                            raise self.InvalidPageLineNumber()
                        if line == n:
                            selected_line_list.append(text)

                if selected_line_list == []:
                    raise self.InvalidLineNumber()

                return selected_line_list

            return [curr_text]    

        else:
            for count in range(num_page):
                curr_page = pdf_pages[count]
                curr_text = curr_page.extract_text()
                text_list.append(curr_text)

        return text_list


    def __pdf_page_index(self, page: dict["page": int, "lines": list[int]]):
        """
        Converts page to index.

        Args:
            file_page_line (dict["page": int, "lines": list[int]]): page and lines of the file. 
        """

        if page["page"] <= 0:
            raise self.InvalidPageLineNumber()

        page["page"] -= 1
            
        return page


    def __check_file_path(self, file_path):
        """
        Check if filename ends with .pdf.

        Args: 
            file_path (str): pdf filepath in path/filename.pdf format
        """
        file_path_len = len(file_path) 

        if file_path_len < 4 or file_path[(file_path_len - 4): file_path_len] != ".pdf":
            raise self.InvalidFilename()


    class InvalidPageLineNumber(Exception):
        def __init__(self) -> None:
            super().__init__(f"Invalid page number: the page and line number must be > 0")
    
    class InvalidLineNumber(Exception):
        def __init__(self) -> None:
            super().__init__(f"Invalid line number: the line number is out of range")


    class InvalidFilename(Exception):
        def __init__(self) -> None:
            super().__init__(f"Invalid filename: the filename must end with \".pdf\"")
