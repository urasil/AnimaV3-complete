using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace dotnetAnima.Core
{
    public class AudioRecorder
    {
        private WaveInEvent waveIn;
        private WaveFileWriter writer;
        private SoundPlayer soundPlayer;

        // Write recorded audio data to file
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }
        public void StartRecording(string outputPath)
        {
            waveIn = new WaveInEvent();
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(44100, 1); // 44.1 kHz, 16-bit mono
            writer = new WaveFileWriter(outputPath, waveIn.WaveFormat);

            waveIn.StartRecording();
        }

        public void StopRecording() 
        {
            waveIn.StopRecording();
            waveIn.Dispose();
            writer.Dispose();
        }

        public void PlaySound()
        {
            soundPlayer = new SoundPlayer("../../../output.wav");
            soundPlayer.Play();
        }

        public void StopSound() 
        {
            if(soundPlayer != null)
            {
                soundPlayer.Stop();
            }
        }
    }
}
