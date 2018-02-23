using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Models;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class FolderIconPicker : ContentPage
    {
        public event Action<ArgsSelectedIcon> FolderConstructed = delegate { };

        public List<DisplayImageModel> Images { get; set; }

        private bool inInitialLoading = true;
        private bool needsImage = true;

        private string selectedIconString = "";

        public FolderIconPicker()
        {
            InitializeComponent();
        }

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

            previewCurrent.Source = ImageSource.FromResource(string.Format("FastTalkerSkiaSharp.Images.{0}.png", (e.Item as DisplayImageModel).Name));
        }

        /// <summary>
        /// Handles the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (!needsImage && selectedFolderNaming.Text.Trim().Length < 2)
            {
                await UserDialogs.Instance.AlertAsync("Please enter at least three letters.");
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