/* 
    The MIT License

    Copyright February 8, 2016 Shawn Gilroy. http://www.smallnstats.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using FastTalkerSkiaSharp.Controls;
using System.Linq;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class FolderIconPickerViewModel : PopupUpViewModel
    {
        public event System.Action<Helpers.ArgsSelectedIcon> FolderConstructed = delegate { };

        System.Collections.Generic.List<Models.DisplayImageModel> _images;
        public System.Collections.Generic.List<Models.DisplayImageModel> Images
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

        System.Collections.Generic.List<string> _currentFolderStrings;
        public System.Collections.Generic.List<string> CurrentFolderStrings
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

        public System.Windows.Input.ICommand IconSelectedFromList { get; set; }
        public System.Windows.Input.ICommand CommandSaveClicked { get; set; }

        bool needsImage = true;
        string selectedIconString;

        /// <summary>
        /// Constructor for VM
        /// </summary>
        /// <param name="currentFolders">Current folders.</param>
        public FolderIconPickerViewModel(System.Collections.Generic.IEnumerable<Icon> currentFolders)
        {
            CurrentFolderStrings = new System.Collections.Generic.List<string>();

            if (currentFolders.Any())
            {
                foreach (var folder in currentFolders)
                {
                    CurrentFolderStrings.Add(folder.Text);
                }
            }

            IconSelectedFromList = new Xamarin.Forms.Command(ItemSelected);
            CommandSaveClicked = new Xamarin.Forms.Command(SaveClicked);
        }

        /// <summary>
        /// Event fired on appearing
        /// </summary>
        public void LoadImagesOnLoad()
        {
            Images = new System.Collections.Generic.List<Models.DisplayImageModel>();

            System.Collections.Generic.List<string> mFolderIcons = new System.Collections.Generic.List<string>() {    "Dark Blue",
                                                                "Dark Pink",
                                                                "Dark Purple",
                                                                "Green",
                                                                "Light Blue",
                                                                "Red" };

            foreach (var iconName in mFolderIcons)
            {
                Images.Add(new Models.DisplayImageModel
                {
                    Image = Xamarin.Forms.ImageSource.FromResource(string.Format(Constants.LanguageSettings.ResourcePrefixPng +
                                                                   "FolderOpen{0}" +
                                                                   Constants.LanguageSettings.ResourceSuffixPng,
                                                                   RemoveWhitespace(iconName))),
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

            selectedIconString = RemoveWhitespace(selectedIconString);

            if (string.IsNullOrWhiteSpace(selectedIconString)) return;

            needsImage = false;

            PreviewCurrentIcon = Xamarin.Forms.ImageSource.FromResource(string.Format(Constants.LanguageSettings.ResourcePrefixPng +
                                                                        "FolderOpen{0}" +
                                                                        Constants.LanguageSettings.ResourceSuffixPng, selectedIconString));
        }

        /// <summary>
        /// Save button clicked
        /// </summary>
        async public void SaveClicked()
        {
            if (needsImage || string.IsNullOrWhiteSpace(FolderNameText) || FolderNameText.Trim().Length < 2)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Please select an imange and enter a folder name with at least three letters.");
            }
            else if (CurrentFolderStrings.Count > 0 && CurrentFolderStrings.Contains(FolderNameText.Trim()))
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Please pick a folder with a unique name (cannot have two folders with same name).");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("FolderOpen{0}", selectedIconString));

                FolderConstructed(new FastTalkerSkiaSharp.Helpers.ArgsSelectedIcon
                {
                    Name = FolderNameText,
                    ImageSource = string.Format("FolderOpen{0}", selectedIconString)
                });

                await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        /// <summary>
        /// Removes the whitespace.
        /// </summary>
        /// <returns>The whitespace.</returns>
		/// <param name="imageString">Input.</param>
		public string RemoveWhitespace(string imageString)
        {
            return new string(imageString.Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
        }
    }
}
