using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private Dictionary<string, string> frontendJsonObject;
        public AnimaHomePage()
        {
            string path = @"../../../animaProfiles";
            string frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
            frontendJsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(frontendJsonContent);
            profileExists = false;
            InitializeComponent();
            if (Directory.Exists(path))
            {
                string[] directoriesWithinPath = Directory.GetFiles(path);
                if (directoriesWithinPath.Length > 0)
                {
                    string userName = frontendJsonObject["nameOfCurrentUser"];
                    int userNameLength = userName.Length;
                    string spaces = String.Concat(Enumerable.Repeat(" ", 52 - userNameLength));
                    desc.Inlines.Clear();
                    desc.Inlines.Add(new Run(spaces + "Welcome Back " + userName + "!") { FontWeight = FontWeights.Bold });
                    startButton.Content = "Text-to-Speech";
                    startButton.Margin = new Thickness(136, 319, 282, 47);
                    profileExists = true;
                }
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
    }
}
