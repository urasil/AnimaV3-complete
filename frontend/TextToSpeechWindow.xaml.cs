using dotnetAnima.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Runtime;

namespace dotnetAnima {
/// <summary>
/// Interaction logic for TextToSpeechWindow.xaml
/// </summary>
///
// Have 2 json files - 1 for the frontend 1 for the backend (avoid race
// conditions)
public partial class TextToSpeechWindow : Page {
    private string frontendJsonFilePath;
    private string backendJsonFilePath;
    private Dictionary<string, string> frontendJsonObject;
    private Dictionary<string, string> backendJsonObject;
    private string frontendJsonContent;
    private string backendJsonContent;

    private string nameOfUser;
    private bool speakingState;  // true means audio is playing

    public TextToSpeechWindow() {
        InitializeComponent();
        ButtonHelper.DisableButton(speakButton,
                                   false);  // disable speak button by default
        // Frontend JSON
        frontendJsonFilePath = @"../frontend.json";
        frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
        frontendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                frontendJsonContent);

        // Backend JSON
        backendJsonFilePath = @"../backend.json";
        backendJsonContent = File.ReadAllText(backendJsonFilePath);
        backendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                backendJsonContent);

        nameOfUser = frontendJsonObject["nameOfCurrentUser"];
        selectedVoice.Text = selectedVoice.Text + nameOfUser;

        speakingState = false;
    }

    private void readingBackendJson() {
        backendJsonContent = File.ReadAllText(backendJsonFilePath);
        backendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                backendJsonContent);
    }

    private void
    updateBackendJson()  // write the in-program backend object into json
    {
        backendJsonContent =
            JsonConvert.SerializeObject(backendJsonObject, Formatting.Indented);
        File.WriteAllText(backendJsonFilePath, backendJsonContent);
    }

    // Add this method to extract the highlighted text from the TextBox
    private string GetHighlightedText() {
        int selectionStart = myTextBox.SelectionStart;
        int selectionLength = myTextBox.SelectionLength;
        return myTextBox.Text.Substring(selectionStart, selectionLength);
    }

    // Send the content typed by the user via registering it to the Json file
    private async void Speak(object sender, RoutedEventArgs e) {
        if (!speakingState) {
            ButtonHelper.DisableButton(speakButton, false);
            frontendJsonObject["speakID"] =
                UUIDGenerator
                    .NewUUID();  // UUID to recognise the same content but
                                 // function call at diferent moment

            // Use highlighted text if available, otherwise use entire content
            string contentToSpeak = string.IsNullOrEmpty(GetHighlightedText())
                                        ? myTextBox.Text
                                        : GetHighlightedText();

            
            backendJsonObject["speechSuccess"] =
                "false";  // reset the value before sending the request
            updateBackendJson();

            frontendJsonObject["content"] = contentToSpeak;
            UpdateFrontendJsonFile();

            

            await WaitSpeech();
            if (backendJsonObject["speechSuccess"] == "false") {
                MessageBox.Show("Failed to create speech", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ButtonHelper.DisableButton(speakButton, true);
            changeSpeakState();
            await ResetSpeakButtonTimer(
                (int) double.Parse(backendJsonObject["audioLength"]) * 1000);
            backendJsonObject["speechSuccess"] = "false";  // reset the value
            // frontendJsonObject["content"] = "";
            updateBackendJson();
            UpdateFrontendJsonFile();
        } else {
            StopSpeak();
        }
    }

    // The timer used to automatically restore the speak button, based on the
    // length of audio
    private async Task ResetSpeakButtonTimer(int time) {
        await Task.Delay(time);
        if (speakingState) {
            changeSpeakState();
        }
    }

    // if speaking, then stop the speak
    private async void StopSpeak() {
        ButtonHelper.DisableButton(speakButton, false);
        backendJsonObject["stopSpeakSuccess"] =
            "false";  // reset the value before sending the request
        updateBackendJson();
        frontendJsonObject["stopSpeakTrigger"] = "true";
        UpdateFrontendJsonFile();

        await WaitStopSpeech();
        if (backendJsonObject["stopSpeakSuccess"] == "false") {
            MessageBox.Show("Failed to stop speech", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
        ButtonHelper.DisableButton(speakButton, true);
        changeSpeakState();
        backendJsonObject["stopSpeakSuccess"] = "false";  // reset the value
        frontendJsonObject["stopSpeakTrigger"] = "false";
        updateBackendJson();
        UpdateFrontendJsonFile();
    }

    private async Task WaitSpeech() {
        while (backendJsonObject ["speechSuccess"]
                   .ToString() != "true") {
            Console.WriteLine(backendJsonObject["speechSuccess"]);
            readingBackendJson();
            await Task.Delay(200);
        }
    }

    private async Task WaitStopSpeech() {
        while (backendJsonObject["stopSpeakSuccess"] != "true") {
            Console.WriteLine(backendJsonObject["stopSpeakSuccess"]);
            readingBackendJson();
            await Task.Delay(200);
        }
    }

    // Send user to Manage Voice Window
    private void ManageVoices(object sender, RoutedEventArgs e) {
        if (speakingState) {
            StopSpeak();
        }
        this.NavigationService.Navigate(new ManageVoicesWindow());
    }

    private void
    UpdateFrontendJsonFile()  // write the in-program frontend object into json
    {
        string updatedJsonContent = JsonConvert.SerializeObject(
            frontendJsonObject, Formatting.Indented);
        File.WriteAllText(frontendJsonFilePath, updatedJsonContent);
    }

    // Read from image button - Implement backend functionality for this to work
    private async void ReadFromImageButton(object sender, RoutedEventArgs e) {
        if (speakingState) {
            StopSpeak();
        }

        Microsoft.Win32.OpenFileDialog dialog =
            new Microsoft.Win32.OpenFileDialog();
        bool ? response = dialog.ShowDialog();
        if (response == true) {
            ButtonHelper.DisableButton(speakButton, false);

            string filePath = dialog.FileName;
            Console.WriteLine(filePath);
            frontendJsonObject["readFilePath"] = filePath;
            frontendJsonObject["readFileID"] = UUIDGenerator.NewUUID();
            UpdateFrontendJsonFile();

            backendJsonObject["readFileSuccess"] = "false";  // reset the value
            updateBackendJson();

            await SendFileContentBackToFrontend();
            if (backendJsonObject["readFileSuccess"] == "false") {
                MessageBox.Show(
                    "Failed to read file, make sure the extension is jpg or pdf, and make sure the quality is good enough",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ButtonHelper.DisableButton(speakButton, true);
            changeSpeakState();
            await ResetSpeakButtonTimer(
                (int) double.Parse(backendJsonObject["audioLength"]) * 1000);

            backendJsonObject["readFileSuccess"] = "false";  // reset the value
            frontendJsonObject["readFilePath"] = "";
            UpdateFrontendJsonFile();
            updateBackendJson();
        }
    }

    private async Task SendFileContentBackToFrontend() {
        while (backendJsonObject ["readFileSuccess"]
                   .ToString() != "true") {
            readingBackendJson();
            await Task.Delay(200);
        }
    }

    private void MyTextBoxTextChanged(object sender, TextChangedEventArgs e) {
        if (myTextBox != null) {
            frontendJsonObject["content"] = myTextBox.Text;
        }
        if (myTextBox.Text == "") {
            ButtonHelper.DisableButton(speakButton,
                                       false);  // empty text is not allowed
        } else {
            ButtonHelper.DisableButton(speakButton, true);
        }
    }

    private void myTextBox_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            TextBox textBox = sender as TextBox;
            if (textBox != null) {
                int caretIndex = textBox.CaretIndex;
                int lineNumber =
                    textBox.GetLineIndexFromCharacterIndex(caretIndex);
                textBox.Text =
                    textBox.Text.Insert(caretIndex, Environment.NewLine);
                textBox.CaretIndex =
                    textBox.GetCharacterIndexFromLineIndex(lineNumber + 1);

                e.Handled = true;  // Prevents the Enter key from inserting a
                                   // newline in the TextBox
            }
        }
    }

    // change the style of speak button between SPEAK and STOP
    private void changeSpeakState() {
        speakingState = !speakingState;
        var rectangleOverlay =
            speakButton.Template.FindName("RectangleOverlay", speakButton)
                as Rectangle;  // change the attribute in style file
        if (speakingState) {
            speakButton.Content = "STOP";
            if (rectangleOverlay != null) {
                rectangleOverlay.Fill = new SolidColorBrush(Colors.Red);
            }
        } else {
            speakButton.Content = "SPEAK";
            if (rectangleOverlay != null) {
                rectangleOverlay.Fill = new SolidColorBrush(Colors.LimeGreen);
            }
        }
    }

    // Go to the home page
    private void GoHome(object sender, RoutedEventArgs e) {
        this.NavigationService.Navigate(new AnimaHomePage());
    }

    private void bankvoice(object sender, RoutedEventArgs e) {
        if (speakingState) {
            StopSpeak();
        }
        Console.WriteLine("Bank Voice Navigation");
        this.NavigationService.Navigate(new BankVoiceWindow());
    }
}
}
