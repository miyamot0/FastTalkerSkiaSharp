using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Pages;
using FastTalkerSkiaSharp.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class SettingsPageViewModel : PopupUpViewModel
    {
        SkiaSharp.Elements.ElementsController controller;

        public ICommand CommandSaveBoard { get; set; }
        public ICommand CommandDeselecting { get; set; }
        public ICommand CommandOperatingMode { get; set; }
        public ICommand CommandAddIconLocal { get; set; }
        public ICommand CommandAddIconPhoto { get; set; }
        public ICommand CommandAddIconStored { get; set; }
        public ICommand CommandAddFolder { get; set; }
        public ICommand CommandHelp { get; set; }
        public ICommand CommandAbout { get; set; }

        public ICommand CommandClose { get; set; }

        public event Action<ArgsSelectedIcon> SaveCommunicationIconEvent = delegate { };
        public event Action<SkiaSharp.Elements.Element> SaveCommunicationElementEvent = delegate { };

        string _deselectText;
        public string DeselectText
        {
            get
            {
                return _deselectText;
            }
            set
            {
                _deselectText = value;
                OnPropertyChanged("DeselectText");
            }
        }

        string _modeText;
        public string ModeText
        {
            get
            {
                return _modeText;
            }
            set
            {
                _modeText = value;
                OnPropertyChanged("ModeText");
            }
        }

        public SettingsPageViewModel(SkiaSharp.Elements.ElementsController _controller)
        {
            controller = _controller;

            CommandSaveBoard = new Command(ResumeOperation);
            CommandDeselecting = new Command(DeselectOperation);
            CommandOperatingMode = new Command(ChangeMode);
            CommandAddIconLocal = new Command(AddIconLocal);
            CommandAddIconPhoto = new Command(AddIconPhoto);
            CommandAddIconStored = new Command(AddIconStored);
            CommandAddFolder = new Command(AddFolder);

            CommandHelp = new Command(ShowHelpPopup);
            CommandAbout = new Command(ShowAboutPopup);

            CommandClose = new Command(Close);

            RefreshSettingsStatus();
        }

        /// <summary>
        /// Resume operation and save
        /// </summary>
        void ResumeOperation()
        {
            controller.UpdateSettings(isEditing: false,
                                      isInFrame: controller.InFramedMode,
                                      isAutoDeselecting: controller.RequireDeselect,
                                      isInIconModeAuto: controller.IconModeAuto);

            controller.BackgroundColor = controller.InEditMode ? SkiaSharp.SKColors.Orange : SkiaSharp.SKColors.DimGray;

            ClosingBehavior(true);
        }

        /// <summary>
        /// Toggle de-selecting
        /// </summary>
        void DeselectOperation()
        {
            if (!controller.RequireDeselect)
            {
                controller.UpdateSettings(isEditing: controller.InEditMode,
                                          isInFrame: controller.InFramedMode,
                                          isAutoDeselecting: true,
                                          isInIconModeAuto: controller.IconModeAuto);
            }
            else
            {
                controller.UpdateSettings(isEditing: controller.InEditMode,
                                          isInFrame: controller.InFramedMode,
                                          isAutoDeselecting: false,
                                          isInIconModeAuto: controller.IconModeAuto);
            }

            RefreshSettingsStatus();
        }

        /// <summary>
        /// Change operating mode
        /// </summary>
        void ChangeMode()
        {
            if (controller.InFramedMode)
            {
                controller.UpdateSettings(isEditing: controller.InEditMode,
                                          isInFrame: false,
                                          isAutoDeselecting: controller.RequireDeselect,
                                          isInIconModeAuto: true);

                SettingsInteraction(SettingsAction.InvalidateBoardIcon);
            }
            else
            {
                if (controller.IconModeAuto)
                {
                    controller.UpdateSettings(isEditing: controller.InEditMode,
                                              isInFrame: false,
                                              isAutoDeselecting: controller.RequireDeselect,
                                              isInIconModeAuto: false);

                    SettingsInteraction(SettingsAction.InvalidateBoardIcon);
                }
                else
                {
                    controller.UpdateSettings(isEditing: controller.InEditMode,
                                              isInFrame: true,
                                              isAutoDeselecting: controller.RequireDeselect,
                                              isInIconModeAuto: true);

                    SettingsInteraction(SettingsAction.InvalidateBoardFrame);
                }
            }

            RefreshSettingsStatus();
        }

        /// <summary>
        /// Add local icons
        /// </summary>
        async void AddIconLocal()
        {
            await App.Current.MainPage.Navigation.PopAllPopupAsync();

            CommunicationIconPicker newCommunicationPage = new CommunicationIconPicker();
            newCommunicationPage.IconConstructed += SaveCommunicationIconEvent;

            await App.Current.MainPage.Navigation.PushAsync(newCommunicationPage);
        }

        /// <summary>
        /// Add an icon from the camera
        /// </summary>
        async void AddIconPhoto()
        {
            await App.Current.MainPage.Navigation.PopAllPopupAsync();

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
        /// Adds an icon from storage
        /// </summary>
        async void AddIconStored()
        {
            await App.Current.MainPage.Navigation.PopAllPopupAsync();

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
        /// Adds an icon representing a folder
        /// </summary>
        async void AddFolder()
        {
            await App.Current.MainPage.Navigation.PopAllPopupAsync();

            IEnumerable<SkiaSharp.Elements.Element> foldersInField = controller.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder);

            FolderIconPickerViewModel viewModel = new FolderIconPickerViewModel(foldersInField);
            viewModel.FolderConstructed += SaveCommunicationIconEvent;

            FolderIconPicker folderPicker = new FolderIconPicker()
            {
                BindingContext = viewModel
            };

            await App.Current.MainPage.Navigation.PushAsync(folderPicker);
        }

        /// <summary>
        /// Show help popup
        /// </summary>
        async void ShowHelpPopup()
        {
            HelpPopup mPopup = new HelpPopup();

            await App.Current.MainPage.Navigation.PushPopupAsync(mPopup);
        }

        /// <summary>
        /// Show about popup
        /// </summary>
        async void ShowAboutPopup()
        {
            AboutPagePopup page = new AboutPagePopup();

            await App.Current.MainPage.Navigation.PushPopupAsync(page);
        }

        /// <summary>
        /// Close the dialog
        /// </summary>
        void Close()
        {
            ClosingBehavior(true);
        }

        /// <summary>
        /// Routing for how to close app
        /// </summary>
        /// <param name="pop">If set to <c>true</c> pop.</param>
        async void ClosingBehavior(bool pop)
        {
            if (pop)
            {
                SettingsInteraction(SettingsAction.SaveBoard);

                await App.Current.MainPage.Navigation.PopPopupAsync();
            }
        }

        /// <summary>
        /// Update bindings
        /// </summary>
        void RefreshSettingsStatus()
        {
            DeselectText = GetToggleMessage("Auto-Deselecting: ", controller.RequireDeselect);
            ModeText = string.Concat("Mode: ", GetModeString());
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
            if (controller.InFramedMode)
            {
                return "Sentence Output";
            }
            else
            {
                if (controller.IconModeAuto)
                {
                    return "Auto Icon Output";
                }
                else
                {
                    return "Manual Icon Output";
                }
            }
        }

        public enum SettingsAction
        {
            SaveBoard,
            InvalidateBoardFrame,
            InvalidateBoardIcon,
            ResumeOperation
        }

        private void SettingsInteraction(SettingsAction obj)
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, obj);

            switch (obj)
            {
                case SettingsAction.SaveBoard:
                    controller.PromptResave();

                    break;

                case SettingsAction.InvalidateBoardIcon:
                    for (int i = 0; i < controller.Elements.Count; i++)
                    {
                        controller.Elements[i].IsMainIconInPlay = false;
                    }

                    controller.Invalidate();

                    break;

                case SettingsAction.InvalidateBoardFrame:
                    for (int i = 0; i < controller.Elements.Count; i++)
                    {
                        controller.Elements[i].IsMainIconInPlay = false;
                    }

                    controller.Invalidate();

                    break;
            }
        }
    }
}
