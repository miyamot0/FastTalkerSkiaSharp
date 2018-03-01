using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using FastTalkerSkiaSharp.ViewModels;
using System.Diagnostics;
using SkiaSharp.Elements;
using Rg.Plugins.Popup.Extensions;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Storage;
using System.Linq;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Acr.UserDialogs;
using System.IO;
using FastTalkerSkiaSharp.Interfaces;
using Plugin.Media.Abstractions;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class SettingsPage : PopupPage
    {
        public enum SettingsAction
        {
            SaveBoard,
            InvalidateBoardFrame,
            InvalidateBoardIcon,
            ResumeOperation
        }

        public event Action<SettingsAction> SettingsActionEvent = delegate { };
        public event Action<ArgsSelectedIcon> SaveCommunicationIconEvent = delegate { };
        public event Action<SkiaSharp.Elements.Element> SaveCommunicationElementEvent = delegate { };

        private ElementsController ControllerReference;

        public SettingsPage(ElementsController _controllerReference)
        {
            InitializeComponent();

            BindingContext = new PopupUpViewModel()
            {
                Padding = new Thickness(100, 100, 100, 100),
                IsSystemPadding = true
            };

            ControllerReference = _controllerReference;

            RefreshSettingsStatus();
        }

        /// <summary>
        /// On appearing, set up the UI
        /// </summary>
		protected override void OnAppearing()
		{
            base.OnAppearing();
		}

        /// <summary>
        /// On disappearing, save state
        /// </summary>
		protected override void OnDisappearing()
		{
            base.OnDisappearing();

            ClosingBehavior(false);
		}

        /// <summary>
        /// Updates UI
        /// </summary>
        void RefreshSettingsStatus()
        {
            btnDeselect.Text = GetToggleMessage("Auto-Deselecting: ", ControllerReference.RequireDeselect);
            btnMode.Text = string.Concat("Mode: ", GetModeString());
        }

        /// <summary>
        /// Returns back to user operation
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Click_Resume_Operation(object sender, System.EventArgs e)
        {
            ControllerReference.UpdateSettings(isEditing: false,
                                               isInFrame: ControllerReference.InFramedMode,
                                               isAutoDeselecting: ControllerReference.RequireDeselect,
                                               isInIconModeAuto: ControllerReference.IconModeAuto);

            ControllerReference.BackgroundColor = ControllerReference.InEditMode ? SkiaSharp.SKColors.Orange : SkiaSharp.SKColors.DimGray;

            ClosingBehavior(true);
        }

        /// <summary>
        /// Saves current board/settings
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Click_Save_Board(object sender, System.EventArgs e)
        {
            SettingsActionEvent(SettingsAction.SaveBoard);
        }

        /// <summary>
        /// Adds local, embedded images to field as icons
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Click_Add_Local_Icon(object sender, System.EventArgs e)
        {
            await Navigation.PopAllPopupAsync();

            CommunicationIconPicker newCommunicationPage = new CommunicationIconPicker();
            newCommunicationPage.IconConstructed += SaveCommunicationIconEvent;

            await App.Current.MainPage.Navigation.PushAsync(newCommunicationPage);
        }

        /// <summary>
        /// Adds a local, downloaded image
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Click_Add_Downloaded_Icon(object sender, System.EventArgs e)
        {
            await Navigation.PopAllPopupAsync();

            await CrossMedia.Current.Initialize();

            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);

            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                {
                    await UserDialogs.Instance.AlertAsync("Need access to photos to make icon");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Photos });

                status = results[Permission.Photos];
            }

            if (status == PermissionStatus.Granted)
            {
                var userInput = await UserDialogs.Instance.PromptAsync("Name the icon to add");

                if (userInput == null || string.IsNullOrWhiteSpace(userInput.Text)) return;

                Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions 
                { 
                    CustomPhotoSize = 500, 
                    CompressionQuality = 80,
                    RotateImage = true,
                });

                if (file == null || file.Path == null)
                {
                    return;
                }

                if (File.Exists(@file.Path))
                { 
                    byte[] imageArray = null;

                    //if (Device.RuntimePlatform == Device.Android)
                    //{
                    //    imageArray = DependencyService.Get<InterfaceBitmapResize>().RotateImage(@file.Path);
                    //}
                    //else
                    //{
                        imageArray = File.ReadAllBytes(@file.Path);
                    //}

                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                    file.Dispose();

                    CommunicationIcon dynamicIcon = new CommunicationIcon()
                    {
                        Tag = (int)SkiaSharp.Elements.CanvasView.Role.Communication,
                        Text = userInput.Text,
                        Local = false,
                        IsStoredInFolder = false,
                        FolderContainingIcon = "",
                        Base64 = base64ImageRepresentation,
                        Scale = 1f,
                        X = -1,
                        Y = -1
                    };

                    SkiaSharp.Elements.Image testImage = null;

                    try
                    {
                        testImage = App.ImageBuilderInstance.BuildCommunicationIconDynamic(icon: dynamicIcon);
                    }
                    catch
                    {
                        return;
                    }

                    SaveCommunicationElementEvent(testImage);
                }
            }
            else if (status != PermissionStatus.Unknown)
            {
                await UserDialogs.Instance.AlertAsync("Can not continue, try again");
            }
        }

        /// <summary>
        /// Adds photos from the camera to the field, as icons
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Click_Add_Photo_Icon(object sender, System.EventArgs e)
        {
            await Navigation.PopAllPopupAsync();

            string[] base64 = await App.UserInputInstance.GetImageFromCameraCallAsync();

            if (base64 == null) return;

            CommunicationIcon dynamicIcon = new CommunicationIcon()
            {
                Tag = (int)SkiaSharp.Elements.CanvasView.Role.Communication,
                Text = base64[0],
                Local = false,
                IsStoredInFolder = false,
                FolderContainingIcon = "",
                Base64 = base64[1],
                Scale = 1f,
                X = -1,
                Y = -1
            };

            SkiaSharp.Elements.Image testImage = null;

            try
            {
                testImage = App.ImageBuilderInstance.BuildCommunicationIconDynamic(icon: dynamicIcon);
            }
            catch
            {
                return;
            }

            SaveCommunicationElementEvent(testImage);
        }

        /// <summary>
        /// Adds folders to the field from local images
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Click_Add_Folder(object sender, System.EventArgs e)
        {
            await Navigation.PopAllPopupAsync();

            IEnumerable<SkiaSharp.Elements.Element> foldersInField = ControllerReference.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder);

            FolderIconPicker newCommunicationPage = new FolderIconPicker(currentFolders: foldersInField);
            newCommunicationPage.FolderConstructed += SaveCommunicationIconEvent;

            await App.Current.MainPage.Navigation.PushAsync(newCommunicationPage);
        }

        /// <summary>
        /// Display help information
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Click_Help_Popup(object sender, System.EventArgs e)
        {
            HelpPopup mPopup = new HelpPopup();

            await App.Current.MainPage.Navigation.PushPopupAsync(mPopup);
        }

        /// <summary>
        /// Display about information
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void Click_About_Popup(object sender, System.EventArgs e)
        {
            AboutPagePopup page = new AboutPagePopup();

            await App.Current.MainPage.Navigation.PushPopupAsync(page);
        }

        /// <summary>
        /// Close view (button event)
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Click_Close_Button(object sender, System.EventArgs e)
        {
            ClosingBehavior(true);
        }

        /// <summary>
        /// Behavior to fire, with routing
        /// </summary>
        /// <param name="pop">If set to <c>true</c> pop.</param>
        async void ClosingBehavior(bool pop)
        {
            if (pop)
            {
                SettingsActionEvent(SettingsAction.SaveBoard);

                await Navigation.PopPopupAsync();
            }
        }

        /// <summary>
        /// Toggles deselect mode
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Click_Deselecting(object sender, System.EventArgs e)
        {
            if (!ControllerReference.RequireDeselect)
            {
                ControllerReference.UpdateSettings(isEditing: ControllerReference.InEditMode,
                                                   isInFrame: ControllerReference.InFramedMode,
                                                   isAutoDeselecting: true,
                                                   isInIconModeAuto: ControllerReference.IconModeAuto);                
            }
            else
            {
                ControllerReference.UpdateSettings(isEditing: ControllerReference.InEditMode,
                                                   isInFrame: ControllerReference.InFramedMode,
                                                   isAutoDeselecting: false,
                                                   isInIconModeAuto: ControllerReference.IconModeAuto);
            }

            RefreshSettingsStatus();
        }

        /// <summary>
        /// Toggle mode of operation
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Click_Mode(object sender, System.EventArgs e)
        {
            if (ControllerReference.InFramedMode)
            {
                ControllerReference.UpdateSettings(isEditing: ControllerReference.InEditMode,
                                                   isInFrame: false,
                                                   isAutoDeselecting: ControllerReference.RequireDeselect,
                                                   isInIconModeAuto: true);

                SettingsActionEvent(SettingsAction.InvalidateBoardIcon);
            }
            else
            {
                if (ControllerReference.IconModeAuto)
                {
                    ControllerReference.UpdateSettings(isEditing: ControllerReference.InEditMode,
                                                       isInFrame: false,
                                                       isAutoDeselecting: ControllerReference.RequireDeselect,
                                                       isInIconModeAuto: false);

                    SettingsActionEvent(SettingsAction.InvalidateBoardIcon);
                }
                else
                {
                    ControllerReference.UpdateSettings(isEditing: ControllerReference.InEditMode,
                                                       isInFrame: true,
                                                       isAutoDeselecting: ControllerReference.RequireDeselect,
                                                       isInIconModeAuto: true);

                    SettingsActionEvent(SettingsAction.InvalidateBoardFrame);
                }
            }

            RefreshSettingsStatus();
        }

        /// <summary>
        /// Get string for ui
        /// </summary>
        /// <returns>The toggle message.</returns>
        /// <param name="baseMessage">Base message.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        private string GetToggleMessage(string baseMessage, bool value)
        {
            string valueString = value ? "true" : "false";

            return string.Concat(baseMessage, value);
        }

        /// <summary>
        /// Get string for ui
        /// </summary>
        /// <returns>The mode string.</returns>
        private string GetModeString()
        {
            if (ControllerReference.InFramedMode)
            {
                return "Sentence Output";
            }
            else
            {
                if (ControllerReference.IconModeAuto)
                {
                    return "Auto Icon Output";
                }
                else
                {
                    return "Manual Icon Output";
                }
            }
        }
    }
}
