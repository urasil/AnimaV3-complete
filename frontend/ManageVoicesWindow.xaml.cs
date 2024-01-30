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

// What is left to do?
// Import my voice

namespace dotnetAnima
{
    /// <summary>
    /// Interaction logic for ManageVoicesWindow.xaml
    /// </summary>
    public partial class ManageVoicesWindow : Page
    {
        // Back end
        string backendJsonFilePath = @"../../../backend.json";
        private string backendJsonContent = File.ReadAllText("../../../backend.json");
        private Dictionary<string, string> backendJsonObject;

        // Front end
        string frontendJsonFilePath = @"../../../frontend.json";
        private string frontendJsonContent = File.ReadAllText("../../../frontend.json");
        private Dictionary<string, string> frontendJsonObject;
        string[] listOfNames;
        public ManageVoicesWindow()
        {
            InitializeComponent();
            frontendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(frontendJsonContent);
            listOfNames = ExtractNames();
            string currentUser = frontendJsonObject["nameOfCurrentUser"];
            yourVoiceRadioButton.Content = currentUser;


            foreach (string name in listOfNames)
            {
                if(name != currentUser)
                {
                    RadioButton radioButton = new RadioButton();
                    radioButton.Content = name;
                    radioButton.Name = name;
                    radioButton.Foreground = Brushes.AntiqueWhite;
                    radioButton.Margin = new Thickness(0, 0, 0, 10);
                    radioButton.FontSize = 25;
                    radioButton.FontWeight = FontWeights.DemiBold;
                    radioButton.FontStyle = FontStyles.Italic;
                    radioButton.FontFamily = new FontFamily("Times New Roman");
                    radioButton.GroupName = "VoiceSelection";
                    groupPanel.Children.Add(radioButton);
                }
                
            }

        }

        private string[] ExtractNames()
        {
            string[] filePaths = Directory.GetFiles("../../../animaProfiles");
            string[] namesList = new string[filePaths.Length];

            for (int i = 0; i < filePaths.Length; i++)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(filePaths[i]);
                namesList[i] = fileName;
            }

            return namesList;
        }


        private void RedoVoice(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BankVoiceWindow());
        }

        // Needs backend functionality
        private async void ImportVoice(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            bool? response = dialog.ShowDialog();
            if (response == true)
            {
                string filePath = dialog.FileName;
                frontendJsonObject["importFilePath"] = filePath;
                string updatedJsonContent = JsonConvert.SerializeObject(frontendJsonObject, Formatting.Indented);
                File.WriteAllText(frontendJsonFilePath, updatedJsonContent);
                await SendFileContentBackToFrontend();
                if(backendJsonObject["importSuccess"] == "false")
                {
                    MessageBox.Show("Coudln't create a voice profile from uploaded file");
                }
                
            }
        }

        private async Task SendFileContentBackToFrontend()
        {
            while (backendJsonObject["importSuccess"] != "true" || backendJsonObject["importSuccess"] != "false")
            {
                await Task.Delay(1000);
            }
        }

        // Delete Voice Button Functions

        private RadioButton FindCheckedRadioButton()
        {
            RadioButton checkedRadioButton = null;
            foreach (var child in groupPanel.Children)
            {
                if (child is RadioButton radioButton && radioButton.IsChecked == true)
                {
                    checkedRadioButton = radioButton;
                    break;
                }
            }
            return checkedRadioButton;
        }
        private void UpdateFrontendJsonFile()
        {
            string updatedJsonContent = JsonConvert.SerializeObject(frontendJsonObject, Formatting.Indented);
            File.WriteAllText(frontendJsonFilePath, updatedJsonContent);
        }
        private void DeleteVoice(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRadioButton = FindCheckedRadioButton();

            if (checkedRadioButton != null)
            {
                string animaProfileName = checkedRadioButton.Content.ToString() + ".animaprofile";
                int radioButtonCount = groupPanel.Children.OfType<RadioButton>().Count();
                string animaProfilePath = System.IO.Path.Combine("../../../animaProfiles", animaProfileName);

                if (File.Exists(animaProfilePath) && radioButtonCount > 1)
                {
                    File.Delete(animaProfilePath);

                    if (checkedRadioButton.Content.ToString() != frontendJsonObject["nameOfCurrentUser"])
                    {
                        groupPanel.Children.Remove(checkedRadioButton);
                        yourVoiceRadioButton.IsChecked = true;
                    }
                    else
                    {

                        RadioButton[] radioButtons = groupPanel.Children.OfType<RadioButton>().ToArray();
                        foreach (RadioButton radioButton in radioButtons)
                        {
                            if (radioButton.Content.ToString() != frontendJsonObject["nameOfCurrentUser"])
                            {
                                frontendJsonObject["nameOfCurrentUser"] = radioButton.Content.ToString();
                                yourVoiceRadioButton.Content = frontendJsonObject["nameOfCurrentUser"];
                                yourVoiceRadioButton.IsChecked = true;
                                groupPanel.Children.Remove(radioButton);
                                break;
                            }
                        }
                        UpdateFrontendJsonFile();

                    }
                }

                else
                {
                    MessageBox.Show("Record one more voice to delete this one!");
                }
                    
            }
            
        }

        private void SelectVoice(object sender, RoutedEventArgs e)
        {
            string oldUser = frontendJsonObject["nameOfCurrentUser"];
            foreach (var child in groupPanel.Children)
            {
                if(child is RadioButton radioButton &&  radioButton.IsChecked == true)
                {
                    frontendJsonObject["nameOfCurrentUser"] = radioButton.Content.ToString();
                    break;
                }
            }
            string currentUser = frontendJsonObject["nameOfCurrentUser"];
            foreach (var child in groupPanel.Children)
            {
                if (child is RadioButton radioButton)
                {
                    if (radioButton.Content == currentUser) 
                    {
                        radioButton.Content = oldUser;
                        break;
                    }
                }

            }
            UpdateFrontendJsonFile();
            yourVoiceRadioButton.Content = currentUser;
            yourVoiceRadioButton.IsChecked = true;

        }

        private void Speak(object sender, RoutedEventArgs e)
        {

            this.NavigationService.Navigate(new TextToSpeechWindow());
        }
    }
}
