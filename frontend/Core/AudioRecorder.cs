using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using System.Windows.Forms;

namespace dotnetAnima.Core
{
    public class AudioRecorder
    {
        private WaveInEvent waveIn;
        private WaveFileWriter writer;
        private SoundPlayer soundPlayer;
        private string audioPath;
        

        

        // Write recorded audio data to file
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }
        public void StartRecording(string outputPath)
        {
            audioPath = outputPath;  
            CleanUp(); // Make sure the previous resources are disposed.
            waveIn = new WaveInEvent();
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(44100, 1); // 44.1 kHz, 16-bit mono
            writer = new WaveFileWriter(audioPath, waveIn.WaveFormat);

            try
            {
                waveIn.StartRecording();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error starting recording: {e.Message}");
                MessageBox.Show($"Error starting recording: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanUp();
            }
            
        }

        public void StopRecording() 
        {
            try
            {
                waveIn?.StopRecording();   // if waveIn != null, then waveIn.StopRecording()
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error stopping recording: {e.Message}");
                MessageBox.Show($"Error stopping recording: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CleanUp();
            }
            
        }

        public void PlaySound()
        {
            if(File.Exists(audioPath))
            {
                try
                {
                    soundPlayer = new SoundPlayer(audioPath);
                    soundPlayer.Play();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error playing sound: {e.Message}");
                    MessageBox.Show($"Error playing sound: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Console.WriteLine("Audio file does not exist");
                MessageBox.Show($"Audio file does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        public void StopSound() 
        {
            try
            {
                soundPlayer?.Stop();
            }
            catch(Exception e) 
            {
                Console.WriteLine($"Error stopping sound: {e.Message}");
                MessageBox.Show($"Error stopping sound: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void CleanUp()  // clean up the resources used in recording
        {
            /* ?. is Null-conditional operator    */

            waveIn?.Dispose();          // if waveIn != null, then waveIn.Dispose()
            writer?.Dispose();
            waveIn = null;
            writer = null;
        }
    }
}
