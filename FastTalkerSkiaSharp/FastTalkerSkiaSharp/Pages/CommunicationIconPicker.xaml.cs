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
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms;
using FastTalkerSkiaSharp.Models;
using System.Linq;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Constants;
using System.Threading.Tasks;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class CommunicationIconPicker : ContentPage
    {
        public event Action<ArgsSelectedIcon> IconConstructed = delegate { };

        public List<string> Categories { get; set; }
        public List<DisplayImageModel> Images { get; set; }

        private bool inInitialLoading = true;
        private bool needsImage = true;

        private string selectedIconString = "";

        public CommunicationIconPicker()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ons the appearing.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (inInitialLoading)
            {
                LoadingInitialJson();
            }

            Images = new List<DisplayImageModel>();

            customScrollView.ItemsSource = Images;
        }

        /// <summary>
        /// Loading json
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

                Categories = App.storedIcons.StoredIcons
                                            .SelectMany(m => m.Tags)
                                            .Distinct()
                                            .OrderBy(m => m)
                                            .ToList();

                foreach (var category in Categories)
                {
                    if (category.Trim().Length == 0)
                    {
                        continue;
                    }

                    categoryPicker.Items.Add(category);
                }

                categoryPicker.SelectedItem = Categories.First();
            }
        }

        /// <summary>
        /// Handles the selected index changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            using (var dlg = UserDialogs.Instance.Progress("Loading icon images"))
            {
                List<string> checkList = new List<string>() { categoryPicker.SelectedItem.ToString() };

                var mIcons = App.storedIcons.StoredIcons.Where(icons => icons.Tags.Intersect(checkList).Any())
                                                        .Select(icons => icons.Name)
                                                        .ToList();

                customScrollView.ItemsSource = null;

                Images.Clear();

                int count = 0;

                foreach (var iconName in mIcons)
                {
                    Images.Add(new DisplayImageModel
                    {
                        Image = ImageSource.FromResource(string.Format(LanguageSettings.ResourcePrefixPng +
                                                                           "{0}" +
                                                                           LanguageSettings.ResourceSuffixPng, iconName)),
                        Name = iconName
                    });

                    await Task.Delay(50);

                    count++;

                    dlg.PercentComplete = (int)(((double)count / (double)mIcons.Count)*100);
                }

                customScrollView.ItemsSource = Images;
            }
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

            selectedIconNaming.Text = selectedIconString;
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
            if (needsImage || string.IsNullOrWhiteSpace(selectedIconNaming.Text) || selectedIconNaming.Text.Trim().Length < 2)
            {
                await UserDialogs.Instance.AlertAsync("Please select an image and enter speech to output");
            }
            else
            {
                IconConstructed(new ArgsSelectedIcon
                {
                    Name = selectedIconNaming.Text.Trim(),
                    ImageSource = selectedIconString
                });

                await App.Current.MainPage.Navigation.PopAsync();
            }
        }
    }
}