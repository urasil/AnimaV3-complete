using dotnetAnima.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace dotnetAnima {
/// <summary>
/// Interaction logic for BankVoiceWindow.xaml
/// </summary>
public partial class BankVoiceWindow : Page {
    int progressCount, textCount, buttonClickedCount;

    List<string> stringList;
    AudioRecorder recorder;

    private string frontendJsonFilePath;
    string frontendJsonContent;
    private Dictionary<string, string> frontendJsonObject;

    private string backendJsonFilePath;
    string backendJsonFileContent;
    private Dictionary<string, string> backendJsonObject;
    public BankVoiceWindow() {
        InitializeComponent();
        textToSpeech.Visibility = Visibility.Hidden;
        manageVoicesButton.Visibility = Visibility.Hidden;
        int len = this.ExtractNames().Length;
        if (len > 0) {
            textToSpeech.Visibility = Visibility.Visible;
            manageVoicesButton.Visibility = Visibility.Visible;
        }
        recorder = new AudioRecorder();

        frontendJsonFilePath = @"../../../frontend.json";
        frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
        frontendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                frontendJsonContent);

        backendJsonFilePath = @"../../../backend.json";
        backendJsonFileContent = File.ReadAllText(backendJsonFilePath);
        backendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                backendJsonFileContent);

        List<LanguageItem> languageItems =
            new List<LanguageItem>  // language dropdown list sources
            {
                new LanguageItem(
                    "Images\\icons\\country flag\\united_kingdom_640.png",
                    "English"),
                new LanguageItem("Images\\icons\\country flag\\france_640.png",
                                 "French"),
                new LanguageItem(
                    "Images\\icons\\country flag\\portugal_640.png",
                    "Portuguese"),
            };
        speakingLang.ItemsSource = languageItems;
        switch (DefaultLanguageSelected()) {
            case "en":
                speakingLang.SelectedItem = languageItems[0];
                break;
            case "fr-fr":
                speakingLang.SelectedItem = languageItems[1];
                break;
            case "pt-br":
                speakingLang.SelectedItem = languageItems[2];
                break;
        }

        // RETURN button rename
        string[] animaFiles =
            Directory.GetFiles("../../../animaProfiles", "*.animaprofile");

        // load text to read
        string current_language = frontendJsonObject["language"];
        TextSeperator textSeperator = new TextSeperator(current_language);
        stringList = textSeperator.ReadAndSeparateText();

        this.progressCount = 0;
        this.buttonClickedCount = 0;
        this.textCount = 0;
        pageText.Text = stringList[0];
    }

    private void readingBackendJson() {
        backendJsonFileContent = File.ReadAllText(backendJsonFilePath);
        backendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                backendJsonFileContent);
    }

    // Return to Menu Button
    public void StopVoicebanking(object sender, RoutedEventArgs e) {
        string[] animaFiles =
            Directory.GetFiles("../../../animaProfiles", "*.animaprofile");
        if (animaFiles.Length >= 1) {
            recorder.StopRecording();
            this.NavigationService.Navigate(new ManageVoicesWindow());
        } else {
            recorder.StopRecording();
            this.NavigationService.Navigate(new AnimaHomePage());
        }
    }

    private string[] ExtractNames() {
        string[] filePaths =
            Directory.GetFiles("../../../animaProfiles",
                               "*.animaprofile");  // only get files with
                                                   // extension of 'animeprofile'
        string[] namesList = new string[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++) {
            string fileName =
                System.IO.Path.GetFileNameWithoutExtension(filePaths[i]);
            namesList[i] = fileName;
        }

        return namesList;
    }

    // The method that runs when the START RECORDING button is pressed
    private void StartRecording(object sender, RoutedEventArgs e) {
        this.buttonClickedCount++;
        speakingLang.IsEnabled = false;
        // Starting the voice recording
        if (this.buttonClickedCount == 1) {
            recorder.StartRecording("../../../output.wav");
            restartButton.Visibility = Visibility.Visible;
        }

        // Ending the voice recording
        if (this.buttonClickedCount == 11) {
            recorder.StopRecording();
            pageText.Visibility = Visibility.Hidden;
            speakerName.Visibility = Visibility.Visible;
            info.Visibility = Visibility.Visible;
            info2.Visibility = Visibility.Visible;
            listenButton.Visibility = Visibility.Visible;

            lovelyButton.Opacity =
                0.3;  // disable this button when textbox is empty
            lovelyButton.IsEnabled = false;  // user enables the button back by
                                             // inputting a name in the textbox
        }
        // Going to the next page
        if (this.buttonClickedCount >= 12) {
            string[] names = ExtractNames();
            if (names.Contains(frontendJsonObject["speakerName"])) {
                MessageBox.Show(
                    $"The name {frontendJsonObject[" speakerName
                    "]} has already been taken. Please select another name.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } else {
                // needs an aysnc function that awaits confirmation from backend
                FinishingUpWithRegistration();
            }
        }
        this.textCount++;
        ChangeInstructions();
        ChangeButtonName();
        ChangeText();
        ChangeBorderColors();
    }

    private async void FinishingUpWithRegistration() {
        backendJsonObject["profileCreationSuccess"] =
            "false";  // reset profileCreationSuccess
        backendJsonFileContent = JsonConvert.SerializeObject(backendJsonObject);
        File.WriteAllText(backendJsonFilePath, backendJsonFileContent);

        string updatedJsonContent = JsonConvert.SerializeObject(
            frontendJsonObject, Formatting.Indented);
        File.WriteAllText(frontendJsonFilePath, updatedJsonContent);

        ButtonHelper.DisableButton(
            lovelyButton, false);  // disable button during the waiting backend

        await WaitBackendConfirmationForProfileCreation();

        ButtonHelper.DisableButton(lovelyButton, true);

        recorder.StopSound();
        this.NavigationService.Navigate(new TextToSpeechWindow());
    }

    private async Task WaitBackendConfirmationForProfileCreation() {
        while (backendJsonObject["profileCreationSuccess"] != "true") {
            readingBackendJson();
            await Task.Delay(1000);
        }
    }

    // Handling the actions after restart button has been pressed
    public void RestartClick(object sender, RoutedEventArgs e) {
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
        instructions.Inlines.Add(
            new Run("Click START READING"){FontWeight = FontWeights.Bold});
        instructions.Inlines.Add(new LineBreak());
        instructions.Inlines.Add(new Run("then begin reading"));
        instructions.Inlines.Add(new LineBreak());
        instructions.Inlines.Add(new Run("the text below"));

        foreach (var child in progress.Children) {
            if (child is Border anyBorder) {
                anyBorder.Background = Brushes.White;
            }

            if (progress.Children[progressCount] is Border border) {
                var colour =
                    (Color) ColorConverter.ConvertFromString("#097ffc");
                border.Background = new SolidColorBrush(colour);
            }
        }
        lovelyButton.Content = "START READING";
        lovelyButton.IsEnabled = true;  // reset the button
        lovelyButton.Opacity = 1;
        restartButton.Visibility = Visibility.Hidden;
        listenButton.Visibility = Visibility.Hidden;
        speakingLang.IsEnabled = true;
    }

    // Playing sound after listen recording button has been pressed
    public void ListenRecording(object sender, RoutedEventArgs e) {
        recorder.PlaySound();
    }

    // Changing the instructions at the top of the window
    public void ChangeInstructions() {
        if (this.buttonClickedCount == 1) {
            instructions.Inlines.Clear();
            instructions.Inlines.Add(
                new Run(" RECORDING STARTED"){FontWeight = FontWeights.Bold});
            instructions.Inlines.Add(new LineBreak());
            instructions.Inlines.Add(new Run("read through all pages"));
            instructions.Inlines.Add(new LineBreak());
            instructions.Inlines.Add(new Run("ps: small mistakes are fine!"){
                FontSize = 14, FontWeight = FontWeights.DemiBold,
                FontStyle = FontStyles.Italic});
        } else if (this.buttonClickedCount == 10) {
            instructions.Inlines.Clear();
            instructions.Inlines.Add(
                new Run("Click FINISH READING"){FontWeight = FontWeights.Bold});
            instructions.Inlines.Add(new LineBreak());
            instructions.Inlines.Add(new Run("to stop recording"));
        } else if (this.buttonClickedCount == 11) {
            instructions.Inlines.Clear();
            instructions.Inlines.Add(
                new Run("     Click ALL DONE!"){FontWeight = FontWeights.Bold});
            instructions.Inlines.Add(new LineBreak());
            instructions.Inlines.Add(new Run("     to continue"));
        }
    }

    // Changing the text that the user is reading
    public void ChangeText() {
        if (this.buttonClickedCount != 1) {
            if (stringList.Count > this.textCount) {
                pageText.Text = stringList[this.textCount];
            }
        } else {
            this.textCount--;
        }
    }

    private void SpeakerNameTextChanged(object sender, TextChangedEventArgs e) {
        if (speakerName.Text ==
            "")  // disable the button when the textbox is empty
        {
            lovelyButton.Opacity = 0.3;
            lovelyButton.IsEnabled = false;
        } else {
            lovelyButton.Opacity = 1;  // enable the button
            lovelyButton.IsEnabled = true;
        }
        if (speakerName != null &&
            speakerName.Visibility != Visibility.Hidden) {
            frontendJsonObject["speakerName"] = speakerName.Text;
            frontendJsonObject["nameOfCurrentUser"] =
                frontendJsonObject["speakerName"];
        }
    }

    // Changing the main button name appropriately
    public void ChangeButtonName() {
        if (this.buttonClickedCount == 1) {
            lovelyButton.Content = "READ NEXT PAGE";
        }
        if (this.buttonClickedCount == 10) {
            lovelyButton.Content = "FINISH READING";
        }
        if (this.buttonClickedCount == 11) {
            lovelyButton.Content = "ALL DONE!";
        }
    }

    // Changing the progress bar colors
    private void ChangeBorderColors() {
        if (progressCount >= 0 && progressCount < progress.Children.Count) {
            foreach (var child in progress.Children) {
                if (child is Border anyBorder) {
                    anyBorder.Background = Brushes.White;
                }
            }

            // Set the selected border color to blue
            if (progress.Children[progressCount] is Border border) {
                var colour =
                    (Color) ColorConverter.ConvertFromString("#097ffc");
                border.Background = new SolidColorBrush(colour);
            }
            this.progressCount++;
        }
    }

    private void SpeakingLanguageChanged(object sender,
                                         SelectionChangedEventArgs e) {
        string language;
        LanguageItem selectedItem = speakingLang.SelectedItem as LanguageItem;
        if (selectedItem != null) {
            language = selectedItem.text;
            switch (language) {
                case "English":
                    frontendJsonObject["language"] = "en";
                    break;
                case "French":
                    frontendJsonObject["language"] = "fr-fr";
                    break;
                case "Portuguese":
                    frontendJsonObject["language"] = "pt-br";
                    break;
            }

            frontendJsonContent = JsonConvert.SerializeObject(
                frontendJsonObject, Formatting.Indented);
            File.WriteAllText(frontendJsonFilePath, frontendJsonContent);
            string currentLanguage = frontendJsonObject["language"];
            string textFilePath = "";
            if (currentLanguage == "en") {
                textFilePath = "../../texts/VoiceBankingText-En.txt";
            } else if (currentLanguage == "fr-fr") {
                textFilePath = "../../texts/VoiceBankingText-Fr.txt";
            } else if (currentLanguage == "pt-br") {
                textFilePath = "../../texts/VoiceBankingText-Pt.txt";
            }

            // Read and separate text from the chosen file
            TextSeperator textSeperator = new TextSeperator(currentLanguage);
            stringList = textSeperator.ReadAndSeparateText();
            ChangeText();
        } else {
            return;
        }
    }

    private void manage_voices(object sender, RoutedEventArgs e) {
        int len = this.ExtractNames().Length;
        recorder.StopRecording();
        if (len > 0) {
            this.NavigationService.Navigate(new ManageVoicesWindow());
        } else {
            MessageBox.Show("You have not recorded any voice to manage!");
        }
    }

    private void text_to_speech(object sender, RoutedEventArgs e) {
        recorder.StopRecording();
        int len = this.ExtractNames().Length;
        if (len > 0) {
            this.NavigationService.Navigate(new TextToSpeechWindow());
        } else {
            MessageBox.Show("You have not recorded a voice to speak with!");
        }
    }

    private string DefaultLanguageSelected() {
        return frontendJsonObject["language"];
    }

    // Go to the home page
    private void GoHome(object sender, RoutedEventArgs e) {
        recorder.StopRecording();
        this.NavigationService.Navigate(new AnimaHomePage());
    }
}
}
