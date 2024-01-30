using dotnetAnima.Core;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace dotnetAnima
{
    /// <summary>
    /// Interaction logic for BankVoiceWindow.xaml
    /// </summary>
    public partial class BankVoiceWindow : Page
    {
        int progressCount, textCount, buttonClickedCount;
        
        List<string> stringList;
        AudioRecorder recorder;

        private string frontendJsonFilePath;
        private Dictionary<string, string> frontendJsonObject;

        private string backendJsonFilePath;
        private Dictionary<string, string> backendJsonObject;
        public BankVoiceWindow()
        {
            InitializeComponent();
        
            TextSeperator textSeperator = new TextSeperator();
            stringList = textSeperator.ReadAndSeparateText();

            recorder = new AudioRecorder();
            
            frontendJsonFilePath = @"../../../frontend.json";
            string frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
            frontendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(frontendJsonContent);

            backendJsonFilePath = @"../../../backend.json";
            string backendJsonFileContent = File.ReadAllText(backendJsonFilePath);
            backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonFileContent);


            this.progressCount = 0;
            this.buttonClickedCount = 0;
            this.textCount = 0;
            pageText.Text = stringList[0];
        }

        private void readingBackendJson()
        {
            string backendJsonFileContent = File.ReadAllText(backendJsonFilePath);
            backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonFileContent);
        }


        // The method that runs when the START RECORDING button is pressed
        private void StartRecording(object sender, RoutedEventArgs e)
        {
            this.buttonClickedCount++;

            // Starting the voice recording
            if (this.buttonClickedCount == 1)
            {
                recorder.StartRecording("../../../output.wav");
                restartButton.Visibility = Visibility.Visible;
            }

            // Ending the voice recording
            if (this.buttonClickedCount == 11)
            {
                recorder.StopRecording();
                pageText.Visibility = Visibility.Hidden;
                speakerName.Visibility = Visibility.Visible;
                info.Visibility = Visibility.Visible;
                info2.Visibility = Visibility.Visible;
                listenButton.Visibility = Visibility.Visible;
            }
            // Going to the next page
            if (this.buttonClickedCount == 12)
            {
                // needs an aysnc function that awaits confirmation from backend
                FinishingUpWithRegistration();
            }
            this.textCount++;
            ChangeInstructions();
            ChangeButtonName();
            ChangeText();
            ChangeBorderColors();
        }

        private async void FinishingUpWithRegistration()
        {
            string updatedJsonContent = JsonConvert.SerializeObject(frontendJsonObject, Formatting.Indented);
            File.WriteAllText(frontendJsonFilePath, updatedJsonContent);

            await WaitBackendConfirmationForProfileCreation();
            
            this.NavigationService.Navigate(new TextToSpeechWindow());
            recorder.StopSound();
        }

        private async Task WaitBackendConfirmationForProfileCreation()
        {
            while (backendJsonObject["profileCreationSuccess"] != "true")
            {
                readingBackendJson();
                await Task.Delay(1000);
            }
        }



        // Handling the actions after restart button has been pressed
        public void RestartClick(object sender, RoutedEventArgs e)
        {
            recorder.StopRecording();
            recorder.StopSound();
            this.buttonClickedCount = 0;
            this.textCount = 0;
            this.progressCount = 0;
            speakerName.Visibility = Visibility.Hidden;
            pageText.Visibility = Visibility.Visible;
            info.Visibility = Visibility.Hidden;
            info2.Visibility = Visibility.Hidden;
            pageText.Text = stringList[0];
            instructions.Inlines.Clear();
            instructions.Inlines.Add(new Run("Select START READING") { FontWeight = FontWeights.Bold });
            instructions.Inlines.Add(new LineBreak());
            instructions.Inlines.Add(new Run("then begin reading"));
            instructions.Inlines.Add(new LineBreak());
            instructions.Inlines.Add(new Run("the text below"));


            foreach (var child in progress.Children)
            {
                
                if (child is Border anyBorder)
                {
                    anyBorder.Background = Brushes.White;
                }
                
                if (progress.Children[progressCount] is Border border)
                {
                    border.Background = Brushes.DeepSkyBlue;
                }
            }
            lovelyButton.Content = "START READING";
            restartButton.Visibility = Visibility.Hidden;
            listenButton.Visibility = Visibility.Hidden;

        }

        // Playing sound after listen recording button has been pressed
        public void ListenRecording(object sender, RoutedEventArgs e)
        {
            recorder.PlaySound();
        }

        // Changing the instructions at the top of the window
        public void ChangeInstructions()
        {

            if (this.buttonClickedCount == 1)
            {
                instructions.Inlines.Clear();
                instructions.Inlines.Add(new Run(" RECORDING STARTED") { FontWeight = FontWeights.Bold });
                instructions.Inlines.Add(new LineBreak());
                instructions.Inlines.Add(new Run("read through all pages"));
                instructions.Inlines.Add(new LineBreak());
                instructions.Inlines.Add(new Run("ps: small mistakes are fine!") { FontSize = 14, FontWeight = FontWeights.DemiBold, FontStyle = FontStyles.Italic});
            }
            else if (this.buttonClickedCount == 10)
            {
                instructions.Inlines.Clear();
                instructions.Inlines.Add(new Run("Select FINISH READING") { FontWeight = FontWeights.Bold });
                instructions.Inlines.Add(new LineBreak());
                instructions.Inlines.Add(new Run("to stop recording"));
            }
            else if (this.buttonClickedCount == 11)
            {
                instructions.Inlines.Clear();
                instructions.Inlines.Add(new Run("     Select ALL DONE!") { FontWeight = FontWeights.Bold });
                instructions.Inlines.Add(new LineBreak());
                instructions.Inlines.Add(new Run("     to continue"));
            }
        }

        // Changing the text that the user is reading
        public void ChangeText()
        {
            if(this.buttonClickedCount != 1) 
            {
                if(stringList.Count > this.textCount)
                {
                    pageText.Text = stringList[this.textCount];
                }
            }  
            else 
            {
                this.textCount--;
            }
        }

        private void SpeakerNameTextChanged(object sender, TextChangedEventArgs e)
        {
            if(speakerName != null && speakerName.Visibility != Visibility.Hidden)
            {
                frontendJsonObject["speakerName"] = speakerName.Text;
                frontendJsonObject["nameOfCurrentUser"] = frontendJsonObject["speakerName"];
            }
        }

        // Changing the main button name appropriately 
        public void ChangeButtonName()
        {
            if(this.buttonClickedCount == 1) 
            {
                lovelyButton.Content = "READ NEXT PAGE";
            }
            if(this.buttonClickedCount == 10) 
            {
                lovelyButton.Content = "FINISH READING";
            }
            if(this.buttonClickedCount == 11)
            {
                lovelyButton.Content = "ALL DONE!";
            }
        }

        // Changing the progress bar colors
        private void ChangeBorderColors()
        {
            if (progressCount >= 0 && progressCount < progress.Children.Count)
            {
                foreach (var child in progress.Children)
                {
                    if (child is Border anyBorder)
                    {
                        anyBorder.Background = Brushes.White;
                    }
                }

                // Set the selected border color to blue
                if (progress.Children[progressCount] is Border border)
                {
                    border.Background = Brushes.DeepSkyBlue;
                }
                this.progressCount++;
            }
        }
    }
}
