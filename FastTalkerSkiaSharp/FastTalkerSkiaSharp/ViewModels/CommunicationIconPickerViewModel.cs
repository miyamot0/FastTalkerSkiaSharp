/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System.Linq;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class CommunicationIconPickerViewModel : PopupUpViewModel
    {
        public event System.Action<Helpers.ArgsSelectedIcon> IconConstructed = delegate { };

        System.Collections.ObjectModel.ObservableCollection<string> _categories;
        public System.Collections.ObjectModel.ObservableCollection<string> Categories
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

        System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageModel> _images;
        public System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageModel> Images
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

        Xamarin.Forms.ImageSource _previewCurrentIcon;
        public Xamarin.Forms.ImageSource PreviewCurrentIcon
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

        public System.Windows.Input.ICommand IconSelectedFromList { get; set; }
        public System.Windows.Input.ICommand CommandSaveClicked { get; set; }

        bool inInitialLoading = true;
        bool needsImage = true;

        string selectedIconString = "";

        public CommunicationIconPickerViewModel()
        {
            IconSelectedFromList = new Xamarin.Forms.Command(ItemSelected);
            CommandSaveClicked = new Xamarin.Forms.Command(SaveClicked);
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

            Images = new System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageModel>();
        }

        /// <summary>
        /// Load JSON categories into memory
        /// </summary>
        void LoadingInitialJson()
        {
            using (var dlg = Acr.UserDialogs.UserDialogs.Instance.Progress("Loading icon categories"))
            {
                if (App.storedIcons == null || App.storedIcons.StoredIcons.Count == 0)
                {
                    using (System.IO.Stream stream = App.MainAssembly.GetManifestResourceStream(Constants.LanguageSettings.ResourcePrefixJson))
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                            App.storedIcons = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.StoredIconContainerModel>(reader.ReadToEnd());
                        }
                    }
                }

                Categories = new System.Collections.ObjectModel.ObservableCollection<string>(App.storedIcons.StoredIcons
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
            using (var dlg = Acr.UserDialogs.UserDialogs.Instance.Progress("Loading..."))
            {
                System.Collections.Generic.List<string> checkList = new System.Collections.Generic.List<string>() { newValue };

                var mIcons = App.storedIcons.StoredIcons.Where(icons => icons.Tags.Intersect(checkList).Any())
                                                        .Select(icons => icons.Name)
                                                        .ToList();

                System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageModel> _tempImages = new System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageModel>();

                double counter = 0d;
                double total = mIcons.Count;

                int current = 0;
                int saved = 0;

                foreach (var iconName in mIcons)
                {
                    _tempImages.Add(new Models.DisplayImageModel
                    {
                        Image = Xamarin.Forms.ImageSource.FromResource(string.Format(Constants.LanguageSettings.ResourcePrefixPng +
                                                                       "{0}" +
                                                                       Constants.LanguageSettings.ResourceSuffixPng, iconName)),
                        Name = iconName
                    });

                    await System.Threading.Tasks.Task.Delay(50);

                    counter += 1d;

                    current = (int)System.Math.Floor(((counter / total) * 100) / 5);

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

            PreviewCurrentIcon = Xamarin.Forms.ImageSource.FromResource(string.Format(Constants.LanguageSettings.ResourcePrefixPng +
                                                                        "{0}" +
                                                                        Constants.LanguageSettings.ResourceSuffixPng, selectedIconString));

            IconNameText = selectedIconString;
        }

        /// <summary>
        /// Saved icon clicked
        /// </summary>
        async public void SaveClicked()
        {
            if (needsImage || string.IsNullOrWhiteSpace(IconNameText) || IconNameText.Trim().Length < 2)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Please select an imange and enter a folder name with at least three letters.");
            }
            else
            {
                IconConstructed(new Helpers.ArgsSelectedIcon
                {
                    Name = IconNameText,
                    ImageSource = selectedIconString
                });

                await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}
