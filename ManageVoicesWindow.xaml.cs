using dotnetAnima.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace dotnetAnima {
/// <summary>
/// Interaction logic for ManageVoicesWindow.xaml
/// </summary>
public partial class ManageVoicesWindow : Page {
    // Back end
    string backendJsonFilePath = @"backend.json";
    private string backendJsonContent =
        File.ReadAllText("backend.json");
    private Dictionary<string, string> backendJsonObject;

    // Front end
    string frontendJsonFilePath = @"frontend.json";
    private string frontendJsonContent =
        File.ReadAllText("frontend.json");
    private Dictionary<string, string> frontendJsonObject;

    // Profile Languages
    string profileLanguagesJsonFilePath = @"profileLanguages.json";
    private string profileLanguagesJsonContent =
        File.ReadAllText("profileLanguages.json");
    private Dictionary<string, List<string>> profileLanguagesObject;

    public ManageVoicesWindow() {
        InitializeComponent();
        frontendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                frontendJsonContent);
        backendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                backendJsonContent);
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
        updateVoices();
    }

    private void readingBackendJson() {
        string updatedJsonContent = File.ReadAllText("backend.json");
        backendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                updatedJsonContent);
    }

    private void readingProfileLanguageJson() {
        string updatedJsonContent =
            File.ReadAllText("profileLanguages.json");
        profileLanguagesObject =
            JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                updatedJsonContent);
    }

    private void readingFrontendJson() {
        string frontendJsonContent = File.ReadAllText(frontendJsonFilePath);
        frontendJsonObject =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                frontendJsonContent);
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
        }
    }

    private string DefaultLanguageSelected() {
        return frontendJsonObject["language"];
    }

    private void updateVoices() {
        string[] listOfNames;
        listOfNames = ExtractNames();
        string currentUser = frontendJsonObject["nameOfCurrentUser"];
        yourVoiceRadioButton.Content = currentUser;

        foreach (string name in listOfNames) {
            if (name != currentUser) {
                RadioButton radioButton = new RadioButton();
                radioButton.Content = name;

                string cleanedName =
                    name.Replace(" ", "_");  // RadioButton.Name only supports
                                             // number, alphabet, and underline
                cleanedName = System.Text.RegularExpressions.Regex.Replace(
                    cleanedName, @"[^\w]", "");
                radioButton.Name = cleanedName;

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

    private string[] ExtractNames() {
        string[] filePaths =
            Directory.GetFiles("animaProfiles",
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

    private void RedoVoice(object sender, RoutedEventArgs e) {
        this.NavigationService.Navigate(new BankVoiceWindow());
    }

    // Create digital voice from selected .wav file
    private async void ImportVoice(object sender, RoutedEventArgs e) {
        Microsoft.Win32.OpenFileDialog dialog =
            new Microsoft.Win32.OpenFileDialog();
        bool ? response = dialog.ShowDialog();
        if (response == true) {
            ButtonHelper.DisableButton(importVoice, false);
            string filePath = dialog.FileName;
            frontendJsonObject["importFilePath"] = filePath;
            string updatedJsonContent = JsonConvert.SerializeObject(
                frontendJsonObject, Formatting.Indented);
            File.WriteAllText(frontendJsonFilePath, updatedJsonContent);
            await SendFileContentBackToFrontend();

            // Re-enable the button regardless of import success or failure
            ButtonHelper.DisableButton(importVoice, true);

            if (backendJsonObject["importSuccess"] == "false") {
                MessageBox.Show(
                    "Couldn't create a voice profile from the uploaded file, please upload a .wav file.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                frontendJsonObject["importFilePath"] = "";
            }
            if (backendJsonObject["importSuccess"] == "true") {
                foreach (var child in groupPanel.Children.OfType<RadioButton>()
                             .ToList()) {
                    if (child.Name != "yourVoiceRadioButton") {
                        groupPanel.Children.Remove(child);
                    }
                }
                frontendJsonObject["importFilePath"] = "";
                updateVoices();
            }
            backendJsonObject["importSuccess"] = "";

            UpdateFrontendJsonFile();
            UpdateBackendJsonFile();
        }
    }

    private async Task SendFileContentBackToFrontend() {
        while (backendJsonObject["importSuccess"] != "true") {
            readingBackendJson();
            await Task.Delay(1000);
            if (backendJsonObject["importSuccess"] == "false") {
                break;
            }
        }
    }

    // Delete Voice Button Functions

    private RadioButton FindCheckedRadioButton() {
        RadioButton checkedRadioButton = null;
        foreach (var child in groupPanel.Children) {
            if (child is RadioButton radioButton &&
                radioButton.IsChecked == true) {
                checkedRadioButton = radioButton;
                break;
            }
        }
        return checkedRadioButton;
    }
    private void UpdateFrontendJsonFile() {
        string updatedJsonContent = JsonConvert.SerializeObject(
            frontendJsonObject, Formatting.Indented);
        File.WriteAllText(frontendJsonFilePath, updatedJsonContent);
    }

    private void UpdateBackendJsonFile() {
        string updatedJsonContent =
            JsonConvert.SerializeObject(backendJsonObject, Formatting.Indented);
        File.WriteAllText(backendJsonFilePath, updatedJsonContent);
    }

    private void UpdateProfileLanguagesFile() {
        string updatedContent = JsonConvert.SerializeObject(
            profileLanguagesObject, Formatting.Indented);
        File.WriteAllText(profileLanguagesJsonFilePath, updatedContent);
    }
    private void DeleteVoice(object sender, RoutedEventArgs e) {
        RadioButton checkedRadioButton = FindCheckedRadioButton();

        if (checkedRadioButton != null) {
            string username = checkedRadioButton.Content.ToString();
            string animaProfileName = username + ".animaprofile";
            int radioButtonCount =
                groupPanel.Children.OfType<RadioButton>().Count();
            string animaProfilePath = System.IO.Path.Combine(
                "animaProfiles", animaProfileName);
            string valueToDeleteKey = "";
            readingProfileLanguageJson();
            foreach (var kvp in profileLanguagesObject) {
                if (kvp.Value.Contains(username)) {
                    valueToDeleteKey = kvp.Key.ToString();
                    break;  // Exit loop after removing first occurrence
                }
            }

            if (valueToDeleteKey != "") {
                profileLanguagesObject [valueToDeleteKey]
                    .Remove(username);
                Console.WriteLine(profileLanguagesObject.ToString());
            }

            UpdateProfileLanguagesFile();

            if (File.Exists(animaProfilePath) && radioButtonCount > 1) {
                File.Delete(animaProfilePath);

                if (checkedRadioButton.Content.ToString() !=
                    frontendJsonObject["nameOfCurrentUser"]) {
                    groupPanel.Children.Remove(checkedRadioButton);
                    yourVoiceRadioButton.IsChecked = true;
                } else {
                    RadioButton[] radioButtons =
                        groupPanel.Children.OfType<RadioButton>().ToArray();
                    foreach (RadioButton radioButton in radioButtons) {
                        if (radioButton.Content.ToString() !=
                            frontendJsonObject["nameOfCurrentUser"]) {
                            frontendJsonObject["nameOfCurrentUser"] =
                                radioButton.Content.ToString();
                            yourVoiceRadioButton.Content =
                                frontendJsonObject["nameOfCurrentUser"];
                            yourVoiceRadioButton.IsChecked = true;
                            groupPanel.Children.Remove(radioButton);
                            break;
                        }
                    }
                    UpdateFrontendJsonFile();
                }
            }

            else {
                MessageBox.Show("Record one more voice to delete this one!");
            }
        }
    }

    private void SelectVoice(object sender, RoutedEventArgs e) {
        string oldUser = frontendJsonObject["nameOfCurrentUser"];
        foreach (var child in groupPanel.Children) {
            if (child is RadioButton radioButton &&
                radioButton.IsChecked == true) {
                frontendJsonObject["nameOfCurrentUser"] =
                    radioButton.Content.ToString();
                break;
            }
        }
        string currentUser = frontendJsonObject["nameOfCurrentUser"];
        foreach (var child in groupPanel.Children) {
            if (child is RadioButton radioButton) {
                if (radioButton.Content == currentUser) {
                    radioButton.Content = oldUser;
                    break;
                }
            }
        }
        UpdateFrontendJsonFile();
        yourVoiceRadioButton.Content = currentUser;
        yourVoiceRadioButton.IsChecked = true;
    }

    private void Speak(object sender, RoutedEventArgs e) {
        this.NavigationService.Navigate(new TextToSpeechWindow());
    }

    // Go to the home page
    private void GoHome(object sender, RoutedEventArgs e) {
        this.NavigationService.Navigate(new AnimaHomePage());
    }
}
}
