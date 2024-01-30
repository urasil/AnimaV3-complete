using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace dotnetAnima
{
    /// <summary>
    /// Interaction logic for TextToSpeechWindow.xaml
    /// </summary>
    /// 
    // Have 2 json files - 1 for the frontend 1 for the backend (avoid race conditions)
    public partial class TextToSpeechWindow : Page
    {
        private string frontendJsonFilePath;
        private string backendJsonFilePath;
        private Dictionary<string, string> frontendJsonObject;
        private Dictionary<string, string> backendJsonObject;
        public TextToSpeechWindow()
        {
            InitializeComponent();
            // Frontend JSON 
            frontendJsonFilePath = @"../../../frontend.json";
            string frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
            frontendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(frontendJsonContent);

            // Backend JSON 
            backendJsonFilePath = @"../../../backend.json";
            string backendJsonContent = File.ReadAllText(backendJsonFilePath);
            backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonContent);

            string nameOfUser = frontendJsonObject["nameOfCurrentUser"];
            string newText = selectedVoice.Text + nameOfUser;
            selectedVoice.Text = newText;
        }

        private void readingBackendJson()
        {
            string backendJsonContent = File.ReadAllText(backendJsonFilePath);
            backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonContent);
        }

        private void updateBackendJson()
        {
            string jsonData = JsonConvert.SerializeObject(backendJsonObject, Formatting.Indented);
            File.WriteAllText(backendJsonFilePath, jsonData);
        }

        // Send the content typed by the user via registering it to the Json file
        private async void Speak(object sender, RoutedEventArgs e)
        {
            string updatedJsonContent = JsonConvert.SerializeObject(frontendJsonObject, Formatting.Indented);
            File.WriteAllText(frontendJsonFilePath, updatedJsonContent);
            await WaitSpeech();
        }

        private async Task WaitSpeech()
        {
            while (backendJsonObject["speechSuccess"].ToString() == "true")
            {
                readingBackendJson();
                await Task.Delay(1000);
            }
            backendJsonObject["speechSuccess"] = "";
            updateBackendJson();
        }

        // Send user to Manage Voice Window
        private void ManageVoices(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ManageVoicesWindow());
        }

        private void UpdateFrontendJsonFile()
        {
            string updatedJsonContent = JsonConvert.SerializeObject(frontendJsonObject, Formatting.Indented);
            File.WriteAllText(frontendJsonFilePath, updatedJsonContent);
        }

        // Read from image button - Implement backend functionality for this to work
        private async void ReadFromImageButton(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            bool? response = dialog.ShowDialog();
            if(response == true)
            {
                string filePath = dialog.FileName;
                frontendJsonObject["readFilePath"] = filePath;
                UpdateFrontendJsonFile();
                await SendFileContentBackToFrontend();
            }
        }

        private async Task SendFileContentBackToFrontend()
        {
            while (backendJsonObject["readFileSuccess"].ToString() == "true")
            {
                readingBackendJson();
                await Task.Delay(1000);
            }
            backendJsonObject["readContentSuccess"] = "";
            updateBackendJson();
        }

        private void MyTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if(myTextBox != null && myTextBox.Text != "")
            {
                frontendJsonObject["content"] = myTextBox.Text;
            }
        }

        private void myTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    int caretIndex = textBox.CaretIndex;
                    int lineNumber = textBox.GetLineIndexFromCharacterIndex(caretIndex);
                    textBox.Text = textBox.Text.Insert(caretIndex, Environment.NewLine);
                    textBox.CaretIndex = textBox.GetCharacterIndexFromLineIndex(lineNumber + 1);

                    e.Handled = true; // Prevents the Enter key from inserting a newline in the TextBox
                }
            }
        }
    }
}
