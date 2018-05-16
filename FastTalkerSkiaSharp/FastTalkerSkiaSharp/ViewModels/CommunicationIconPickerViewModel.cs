/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System;
using FastTalkerSkiaSharp.Helpers;
using System.Collections.ObjectModel;
using FastTalkerSkiaSharp.Models;
using Acr.UserDialogs;
using System.IO;
using FastTalkerSkiaSharp.Constants;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class CommunicationIconPickerViewModel : PopupUpViewModel
    {
        public event Action<ArgsSelectedIcon> IconConstructed = delegate { };

        ObservableCollection<string> _categories;
        public ObservableCollection<string> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        string _selectedCategory;
        public string SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory");

                CategoryChanged(value);
            }
        }

        ObservableCollection<DisplayImageModel> _images;
        public ObservableCollection<DisplayImageModel> Images 
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

        string _iconNameText;
        public string IconNameText
        {
            get
            {
                return _iconNameText;
            }
            set
            {
                _iconNameText = value;
                OnPropertyChanged("IconNameText");
            }
        }

        public ICommand IconSelectedFromList { get; set; }
        public ICommand CommandSaveClicked { get; set; }

        private bool inInitialLoading = true;
        private bool needsImage = true;

        private string selectedIconString = "";

        public CommunicationIconPickerViewModel()
        {
            IconSelectedFromList = new Command(ItemSelected);
            CommandSaveClicked = new Command(SaveClicked);
        }

        /// <summary>
        /// Initial loading call
        /// </summary>
        public void InitialLoading()
        {
            if (inInitialLoading)
            {
                LoadingInitialJson();
            }

            Images = new ObservableCollection<DisplayImageModel>();
        }

        /// <summary>
        /// Load JSON categories into memory
        /// </summary>
        void LoadingInitialJson()
        {
            using (var dlg = UserDialogs.Instance.Progress("Loading icon categories"))
            {
                if (App.storedIcons == null || App.storedIcons.StoredIcons.Count == 0)
                {
                    using (Stream stream = App.MainAssembly.GetManifestResourceStream(LanguageSettings.ResourcePrefixJson))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            App.storedIcons = JsonConvert.DeserializeObject<StoredIconContainerModel>(reader.ReadToEnd());
                        }
                    }
                }

                Categories = new ObservableCollection<string>(App.storedIcons.StoredIcons
                                                                 .SelectMany(m => m.Tags)
                                                                 .Distinct()
                                                                 .Where(m => m.Length > 0)
                                                                 .OrderBy(m => m));
            }
        }

        /// <summary>
        /// Changed event
        /// </summary>
        /// <param name="newValue">New value.</param>
        async void CategoryChanged(string newValue)
        {
            using (var dlg = UserDialogs.Instance.Progress("Loading..."))
            {
                List<string> checkList = new List<string>() { newValue };

                var mIcons = App.storedIcons.StoredIcons.Where(icons => icons.Tags.Intersect(checkList).Any())
                                                        .Select(icons => icons.Name)
                                                        .ToList();

                ObservableCollection<DisplayImageModel> _tempImages = new ObservableCollection<DisplayImageModel>();

                double counter = 0d;
                double total = mIcons.Count;

                int current = 0;
                int saved = 0;

                foreach (var iconName in mIcons)
                {
                    _tempImages.Add(new DisplayImageModel
                    {
                        Image = ImageSource.FromResource(string.Format(LanguageSettings.ResourcePrefixPng +
                                                                       "{0}" +
                                                                       LanguageSettings.ResourceSuffixPng, iconName)),
                        Name = iconName
                    });

                    await Task.Delay(50);

                    counter += 1d;

                    current = (int)Math.Floor(((counter / total) * 100) / 5);

                    if (current != saved)
                    {
                        saved = current;

                        dlg.PercentComplete = saved * 20;
                    }

                }

                dlg.Title = "Drawing...";

                Images = _tempImages;
            }
        }

        /// <summary>
        /// Image selected
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

            IconNameText = selectedIconString;
        }

        /// <summary>
        /// Saved icon clicked
        /// </summary>
        async public void SaveClicked()
        {
            if (needsImage || string.IsNullOrWhiteSpace(IconNameText) || IconNameText.Trim().Length < 2)
            {
                await UserDialogs.Instance.AlertAsync("Please select an imange and enter a folder name with at least three letters.");
            }
            else
            {
                IconConstructed(new ArgsSelectedIcon
                {
                    Name = IconNameText,
                    ImageSource = selectedIconString
                });

                await App.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}
