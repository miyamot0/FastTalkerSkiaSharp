using System;
using System.Collections.Generic;
using System.Linq;
using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Models;
using Xamarin.Forms;
using System.Windows.Input;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Helpers;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class FolderIconPickerViewModel : PopupUpViewModel
    {
        public event Action<ArgsSelectedIcon> FolderConstructed = delegate { };

        List<DisplayImageModel> _images;
        public List<DisplayImageModel> Images 
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                OnPropertyChanged("Images");
            }
        }

        List<string> _currentFolderStrings;
        public List<string> CurrentFolderStrings
        {
            get
            {
                return _currentFolderStrings;
            }
            set
            {
                _currentFolderStrings = value;
                OnPropertyChanged("CurrentFolderStrings");
            }
        }

        ImageSource _previewCurrentIcon;
        public ImageSource PreviewCurrentIcon
        {
            get
            {
                return _previewCurrentIcon;
            }
            set
            {
                _previewCurrentIcon = value;
                OnPropertyChanged("PreviewCurrentIcon");
            }
        }

        string _folderNameText;
        public string FolderNameText
        {
            get
            {
                return _folderNameText;
            }
            set
            {
                _folderNameText = value;
                OnPropertyChanged("FolderNameText");
            }
        }

        public ICommand IconSelectedFromList { get; set; }
        public ICommand CommandSaveClicked { get; set; }

        bool needsImage = true;
        string selectedIconString;

        /// <summary>
        /// Constructor for VM
        /// </summary>
        /// <param name="currentFolders">Current folders.</param>
        public FolderIconPickerViewModel(IEnumerable<SkiaSharp.Elements.Element> currentFolders)
        {
            CurrentFolderStrings = new List<string>();

            if (currentFolders.Any())
            {
                foreach (var folder in currentFolders)
                {
                    CurrentFolderStrings.Add(folder.Text);
                }
            }

            IconSelectedFromList = new Command(ItemSelected);
            CommandSaveClicked = new Command(SaveClicked);
        }

        /// <summary>
        /// Event fired on appearing
        /// </summary>
        public void LoadImagesOnLoad()
        {
            Images = new List<DisplayImageModel>();

            List<string> mFolderIcons = new List<string>() {    "FolderOpenDarkBlue",
                                                                "FolderOpenDarkPink",
                                                                "FolderOpenDarkPurple",
                                                                "FolderOpenGreen",
                                                                "FolderOpenLightBlue",
                                                                "FolderOpenRed" };

            foreach (var iconName in mFolderIcons)
            {
                Images.Add(new DisplayImageModel
                {
                    Image = ImageSource.FromResource(string.Format(LanguageSettings.ResourcePrefixPng +
                                                                   "{0}" +
                                                                   LanguageSettings.ResourceSuffixPng, iconName)),
                    Name = iconName
                });
            }
        }

        /// <summary>
        /// Update bindings for selected folder
        /// </summary>
        /// <param name="obj">Object.</param>
        public void ItemSelected(object obj)
        {
            selectedIconString = obj as string;

            if (string.IsNullOrWhiteSpace(selectedIconString)) return;

            needsImage = false;

            PreviewCurrentIcon = ImageSource.FromResource(string.Format(LanguageSettings.ResourcePrefixPng +
                                                                        "{0}" +
                                                                        LanguageSettings.ResourceSuffixPng, selectedIconString));
        }

        /// <summary>
        /// Save button clicked
        /// </summary>
        async public void SaveClicked()
        {
            if (needsImage || string.IsNullOrWhiteSpace(FolderNameText) || FolderNameText.Trim().Length < 2)
            {
                await UserDialogs.Instance.AlertAsync("Please select an imange and enter a folder name with at least three letters.");
            }
            else if (CurrentFolderStrings.Count > 0 && CurrentFolderStrings.Contains(FolderNameText.Trim()))
            {
                await UserDialogs.Instance.AlertAsync("Please pick a folder with a unique name (cannot have two folders with same name).");
            }
            else
            {
                FolderConstructed(new ArgsSelectedIcon
                {
                    Name = FolderNameText,
                    ImageSource = selectedIconString
                });

                await App.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}
