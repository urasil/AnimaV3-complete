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
        private string frontendJsonContent;
        private string backendJsonContent;

        private string nameOfUser;

        public TextToSpeechWindow()
        {
            InitializeComponent();
            // Frontend JSON 
            frontendJsonFilePath = @"../../../frontend.json";
            frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
            frontendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(frontendJsonContent);

            // Backend JSON 
            backendJsonFilePath = @"../../../backend.json";
            backendJsonContent = File.ReadAllText(backendJsonFilePath);
            backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonContent);

            nameOfUser = frontendJsonObject["nameOfCurrentUser"];
            selectedVoice.Text = selectedVoice.Text + nameOfUser;
        }

        private void readingBackendJson()
        {
            backendJsonContent = File.ReadAllText(backendJsonFilePath);
            backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonContent);
        }

        private void updateBackendJson()   // write the in-program backend object into json
        {
            backendJsonContent = JsonConvert.SerializeObject(backendJsonObject, Formatting.Indented);
            File.WriteAllText(backendJsonFilePath, backendJsonContent);
        }

        // Send the content typed by the user via registering it to the Json file
        private async void Speak(object sender, RoutedEventArgs e)
        {
            UpdateFrontendJsonFile();
            await WaitSpeech();
            if (backendJsonObject["speechSuccess"] == "false")
            {
                MessageBox.Show("Failed to create speech", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            backendJsonObject["speechSuccess"] = "";
            frontendJsonObject["content"] = "";
            updateBackendJson();
            UpdateFrontendJsonFile();
        }

        private async Task WaitSpeech()
        {
            int count = 0;
            while (backendJsonObject["speechSuccess"].ToString() != "true")
            {
                Console.WriteLine(backendJsonObject["speechSuccess"]);
                readingBackendJson();
                await Task.Delay(1000);
                count++;
                if(count > 20) 
                {
                    backendJsonObject["speechSuccess"] = "false";
                    break;
                }
            }
        }

        // Send user to Manage Voice Window
        private void ManageVoices(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ManageVoicesWindow());
        }

        private void UpdateFrontendJsonFile()   // write the in-program frontend object into json
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
                Console.WriteLine(filePath);
                frontendJsonObject["readFilePath"] = filePath;
                UpdateFrontendJsonFile();
                await SendFileContentBackToFrontend();
                if (backendJsonObject["readFileSuccess"] == "false")
                {
                    MessageBox.Show("Failed to read file, make sure the extension is jpg or pdf, and make sure the quality is good enough", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                backendJsonObject["readFileSuccess"] = "";
                frontendJsonObject["readFilePath"] = "";
                UpdateFrontendJsonFile();
                updateBackendJson();
            }
        }

        private async Task SendFileContentBackToFrontend()
        {
            int count = 0;
            while (backendJsonObject["readFileSuccess"].ToString() != "true")
            {
                readingBackendJson();
                await Task.Delay(1000);
            }
            
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
