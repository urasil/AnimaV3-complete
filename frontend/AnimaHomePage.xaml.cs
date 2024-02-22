using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dotnetAnima
{
    /// <summary>
    /// Interaction logic for AnimaHomePage.xaml
    /// </summary>
    public partial class AnimaHomePage : Page
    {
        bool profileExists;
        private string frontendJsonFilePath = "../../../frontend.json";
        private string backendJsonFilePath = "../../../backend.json";
        private Dictionary<string, string> frontendJsonObject;
        private Dictionary<string, string> backendJsonObject;
        private string frontendJsonContent;
        private string backendJsonContent;
        public AnimaHomePage()
        {
            string path = @"../../../animaProfiles";
            frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
            backendJsonContent = File.ReadAllText(backendJsonFilePath);
            frontendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(frontendJsonContent);
            backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonContent);
            profileExists = false;
            InitializeComponent();


            startButton.IsEnabled = false;  // disable button before the backend is ready
            startButton.Opacity = 0.3;

            if (Directory.Exists(path))
            {
                string[] directoriesWithinPath = Directory.GetFiles(path, "*.animaprofile");
                if (directoriesWithinPath.Length > 0)
                {
                    string userName = frontendJsonObject["nameOfCurrentUser"];
                    if(File.Exists(path + "/" + userName + ".animaprofile"))
                    {
                        int userNameLength = userName.Length;
                        string spaces = String.Concat(Enumerable.Repeat(" ", 52 - userNameLength));
                        desc.Inlines.Clear();
                        desc.Inlines.Add(new Run(spaces + "Welcome Back " + userName + "!") { FontWeight = FontWeights.Bold });
                        startButton.Content = "Text-to-Speech";
                        startButton.Margin = new Thickness(136, 319, 282, 47);
                        profileExists = true;
                    }
                    
                }
                InitialiseFrontendJson();
                InitialiseBackendJson();
            }
            else
            {
                Console.WriteLine("Error: Missed animaProfiles directory");
                MessageBox.Show("Error: Missed animaProfiles directory", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (profileExists)
            {
                this.NavigationService.Navigate(new TextToSpeechWindow());
            }
            else
            {
                this.NavigationService.Navigate(new BankVoiceWindow());
            }

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await WaitForBackendReady();
        }
        private void InitialiseBackendJson()  // used to initialise all values in backend json file
        {
            backendJsonObject["profileCreationSuccess"] = "false";
            backendJsonObject["readFileSuccess"] = "false";
            backendJsonObject["speechSuccess"] = "false";
            backendJsonObject["readContentSuccess"] = "false";
            backendJsonObject["importSuccess"] = "NULL";
            backendJsonObject["backendReady"] = "false";
            backendJsonObject["stopSpeakSuccess"] = "false";
            backendJsonObject["audioLength"] = "";

            backendJsonContent = JsonConvert.SerializeObject(backendJsonObject, Formatting.Indented);
            File.WriteAllText(backendJsonFilePath, backendJsonContent);
        }
        private void InitialiseFrontendJson()
        {
            frontendJsonObject["stopSpeakTrigger"] = "false";
            frontendJsonObject["content"] = "";
            frontendJsonObject["readFilePath"] = "";
            frontendJsonObject["importFilePath"] = "";

            frontendJsonContent = JsonConvert.SerializeObject(frontendJsonObject, Formatting.Indented);
            File.WriteAllText(frontendJsonFilePath, frontendJsonContent);
        }
        private async Task WaitForBackendReady()
        {
            bool conditionMet = false;
            while (!conditionMet)
            {
                try
                {
                    backendJsonContent = File.ReadAllText(backendJsonFilePath);
                    backendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(backendJsonContent);
                    if (backendJsonObject["backendReady"] == "true")
                    {
                        conditionMet = true;
                        startButton.IsEnabled = true;
                        startButton.Opacity = 1;
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
