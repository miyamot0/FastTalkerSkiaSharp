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

            Images = new List<DisplayImageModel>();

            customScrollView.ItemsSource = Images;
        }

        /// <summary>
        /// Handles the selected index changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            List<string> checkList = new List<string>() { categoryPicker.SelectedItem.ToString() };

            var mIcons = App.storedIcons.StoredIcons.Where(icons => icons.Tags.Intersect(checkList).Any())
                                                    .Select(icons => icons.Name)
                                                    .ToList();

            customScrollView.ItemsSource = null;

            Images.Clear();

            foreach (var iconName in mIcons)
            {
                Images.Add(new DisplayImageModel
                {
                    Image = ImageSource.FromResource(string.Format("FastTalkerSkiaSharp.Images.{0}.png", iconName)),
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

            selectedIconNaming.Text = selectedIconString;

            previewCurrent.Source = ImageSource.FromResource(string.Format("FastTalkerSkiaSharp.Images.{0}.png", (e.Item as DisplayImageModel).Name));
        }

        /// <summary>
        /// Handles the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (!needsImage && selectedIconNaming.Text.Trim().Length < 2)
            {
                await UserDialogs.Instance.AlertAsync("Please enter valid speech to output");
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