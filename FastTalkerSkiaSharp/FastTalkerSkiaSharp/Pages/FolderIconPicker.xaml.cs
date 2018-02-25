/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   Fast Talker is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, version 3.

   Fast Talker is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
   </copyright>

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Models;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class FolderIconPicker : ContentPage
    {
        public event Action<ArgsSelectedIcon> FolderConstructed = delegate { };

        public List<DisplayImageModel> Images { get; set; }

        private bool needsImage = true;

        private string selectedIconString = "";
        private IEnumerable<SkiaSharp.Elements.Element> currentFolders;

        private List<string> currentFolderStrings;

        public FolderIconPicker(IEnumerable<SkiaSharp.Elements.Element> currentFolders)
        {
            InitializeComponent();

            this.currentFolders = currentFolders;

            currentFolderStrings = new List<string>();

            if (currentFolders.Any())
            {
                foreach (var folder in currentFolders)
                {
                    currentFolderStrings.Add(folder.Text);
                }
            }
        }

        /// <summary>
        /// On appearing event
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

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

            customScrollView.ItemsSource = Images;
        }

        /// <summary>
        /// Handles the item selected.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_ItemSelected(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            needsImage = false;

            selectedIconString = (e.Item as DisplayImageModel).Name;

            previewCurrent.Source = ImageSource.FromResource(string.Format(LanguageSettings.ResourcePrefixPng +
                                                                           "{0}" +
                                                                           LanguageSettings.ResourceSuffixPng, (e.Item as DisplayImageModel).Name));
        }

        /// <summary>
        /// Handles the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (needsImage || string.IsNullOrWhiteSpace(selectedFolderNaming.Text) || selectedFolderNaming.Text.Trim().Length < 2)
            {
                await UserDialogs.Instance.AlertAsync("Please select an imange and enter a folder name with at least three letters.");
            }
            else if (currentFolderStrings.Count > 0 && currentFolderStrings.Contains(selectedFolderNaming.Text.Trim()))
            {
                await UserDialogs.Instance.AlertAsync("Please pick a folder with a unique name (cannot have two folders with same name).");
            }
            else
            {
                FolderConstructed(new ArgsSelectedIcon
                {
                    Name = selectedFolderNaming.Text.Trim(),
                    ImageSource = selectedIconString
                });

                await App.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}