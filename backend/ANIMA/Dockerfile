# FROM python:3.9.11-buster

# COPY ./requirements.txt ./requirements.txt

# # RUN pip install -r requirements.txt
# RUN apt-get update
# RUN apt-get install ffmpeg libsm6 libxext6 libasound-dev libportaudio2 libportaudiocpp0 portaudio19-dev -y
# RUN pip install pyaudio

# WORKDIR /code

# COPY . /code/

# CMD ["pip", "install", "-r", "requirements.txt", ";", "flask", "run", "--host", "0.0.0.0", "--port", "80"]

FROM python:3.9.11

COPY ./requirements.txt ./requirements.txt

RUN pip install -r requirements.txt
RUN apt-get update
RUN apt-get install ffmpeg libsm6 libxext6 libasound-dev libportaudio2 libportaudiocpp0 portaudio19-dev -y
RUN pip install pyaudio

WORKDIR /code

COPY . /code/

CMD ["flask", "run", "--host", "0.0.0.0", "--port", "80"]
