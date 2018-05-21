/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System.Linq;
using Rg.Plugins.Popup.Extensions;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class SettingsPageViewModel : PopupUpViewModel
    {
        SkiaSharp.Elements.ElementsController controller;

        public System.Windows.Input.ICommand CommandSaveBoard { get; set; }
        public System.Windows.Input.ICommand CommandDeselecting { get; set; }
        public System.Windows.Input.ICommand CommandOperatingMode { get; set; }
        public System.Windows.Input.ICommand CommandAddIconLocal { get; set; }
        public System.Windows.Input.ICommand CommandAddIconPhoto { get; set; }
        public System.Windows.Input.ICommand CommandAddIconStored { get; set; }
        public System.Windows.Input.ICommand CommandAddFolder { get; set; }
        public System.Windows.Input.ICommand CommandHelp { get; set; }
        public System.Windows.Input.ICommand CommandAbout { get; set; }

        public System.Windows.Input.ICommand CommandClose { get; set; }

        public event System.Action<FastTalkerSkiaSharp.Helpers.ArgsSelectedIcon> SaveCommunicationIconEvent = delegate { };
        public event System.Action<FastTalkerSkiaSharp.Helpers.ArgsSelectedIcon> SaveFolderEvent = delegate { };
        public event System.Action<SkiaSharp.Elements.Element> SaveCommunicationElementEvent = delegate { };

		public enum SettingsAction
        {
            SaveBoard,
            InvalidateBoardFrame,
            InvalidateBoardIcon,
            ResumeOperation
        }

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

        /// <summary>
        /// Set up VM
        /// </summary>
        /// <param name="_controller">Controller.</param>
        public SettingsPageViewModel(SkiaSharp.Elements.ElementsController _controller)
        {
            controller = _controller;

            CommandSaveBoard = new Xamarin.Forms.Command(ResumeOperation);
            CommandDeselecting = new Xamarin.Forms.Command(DeselectOperation);
            CommandOperatingMode = new Xamarin.Forms.Command(ChangeMode);
            CommandAddIconLocal = new Xamarin.Forms.Command(AddIconLocal);
            CommandAddIconPhoto = new Xamarin.Forms.Command(AddIconPhoto);
            CommandAddIconStored = new Xamarin.Forms.Command(AddIconStored);
            CommandAddFolder = new Xamarin.Forms.Command(AddFolder);

            CommandHelp = new Xamarin.Forms.Command(ShowHelpPopup);
            CommandAbout = new Xamarin.Forms.Command(ShowAboutPopup);

            CommandClose = new Xamarin.Forms.Command(Close);

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
			System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "AddIconLocal()");

            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAllPopupAsync();

			System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "AddIconLocal() post");

            CommunicationIconPickerViewModel viewModel = new CommunicationIconPickerViewModel();
            viewModel.IconConstructed += SaveCommunicationIconEvent;

            FastTalkerSkiaSharp.Pages.CommunicationIconPicker newCommunicationPage = new FastTalkerSkiaSharp.Pages.CommunicationIconPicker()
            {
                BindingContext = viewModel
            };

			System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "AddIconLocal() end");

            //await App.Current.MainPage.Navigation.PushAsync(newCommunicationPage);
			await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(newCommunicationPage);

			System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "AddIconLocal() finished");

        }

        /// <summary>
        /// Add an icon from the camera
        /// </summary>
        async void AddIconPhoto()
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAllPopupAsync();

            string[] base64 = await App.UserInputInstance.GetImageFromCameraCallAsync();

            if (base64 == null) return;

            FastTalkerSkiaSharp.Storage.CommunicationIcon dynamicIcon = new FastTalkerSkiaSharp.Storage.CommunicationIcon()
            {
                Tag = FastTalkerSkiaSharp.Elements.ElementRoles.GetRoleInt(FastTalkerSkiaSharp.Elements.ElementRoles.Role.Communication),
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
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAllPopupAsync();

            await Plugin.Media.CrossMedia.Current.Initialize();

            var status = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Photos);

            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                if (await Plugin.Permissions.CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Photos))
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Need access to photos to make icon");
                }

                var results = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Photos });

                status = results[Plugin.Permissions.Abstractions.Permission.Photos];
            }

            if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var userInput = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("Name the icon to add");

                if (userInput == null || string.IsNullOrWhiteSpace(userInput.Text)) return;

                Plugin.Media.Abstractions.MediaFile file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    CustomPhotoSize = 500,
                    CompressionQuality = 80,
                    RotateImage = true,
                });

                if (file == null || file.Path == null)
                {
                    return;
                }

                if (System.IO.File.Exists(@file.Path))
                {
                    byte[] imageArray = null;

                    //if (Device.RuntimePlatform == Device.Android)
                    //{
                    //    imageArray = DependencyService.Get<InterfaceBitmapResize>().RotateImage(@file.Path);
                    //}
                    //else
                    //{
                    imageArray = System.IO.File.ReadAllBytes(@file.Path);
                    //}

                    string base64ImageRepresentation = System.Convert.ToBase64String(imageArray);

                    file.Dispose();

                    FastTalkerSkiaSharp.Storage.CommunicationIcon dynamicIcon = new FastTalkerSkiaSharp.Storage.CommunicationIcon()
                    {
                        Tag = FastTalkerSkiaSharp.Elements.ElementRoles.GetRoleInt(FastTalkerSkiaSharp.Elements.ElementRoles.Role.Communication),
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
            else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Can not continue, try again");
            }
        }

        /// <summary>
        /// Adds an icon representing a folder
        /// </summary>
        async void AddFolder()
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAllPopupAsync();

            System.Collections.Generic.IEnumerable<SkiaSharp.Elements.Element> foldersInField = controller.Elements.Where(elem => elem.Tag == FastTalkerSkiaSharp.Elements.ElementRoles.GetRoleInt(FastTalkerSkiaSharp.Elements.ElementRoles.Role.Folder));

            FolderIconPickerViewModel viewModel = new FolderIconPickerViewModel(foldersInField);
            viewModel.FolderConstructed += SaveFolderEvent;

            FastTalkerSkiaSharp.Pages.FolderIconPicker folderPicker = new FastTalkerSkiaSharp.Pages.FolderIconPicker()
            {
                BindingContext = viewModel
            };

            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(folderPicker);
        }

        /// <summary>
        /// Show help popup
        /// </summary>
        async void ShowHelpPopup()
        {
            FastTalkerSkiaSharp.Pages.HelpPopup mPopup = new FastTalkerSkiaSharp.Pages.HelpPopup();

            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(mPopup);
        }

        /// <summary>
        /// Show about popup
        /// </summary>
        async void ShowAboutPopup()
        {
            FastTalkerSkiaSharp.Pages.AboutPagePopup page = new FastTalkerSkiaSharp.Pages.AboutPagePopup();

            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(page);
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

                await Xamarin.Forms.Application.Current.MainPage.Navigation.PopPopupAsync();
            }
        }

        /// <summary>
        /// Update bindings
        /// </summary>
        void RefreshSettingsStatus()
        {
            DeselectText = GetToggleMessage("Auto-Deselect: ", controller.RequireDeselect);
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
                return "Sentence";
            }
            else
            {
                if (controller.IconModeAuto)
                {
                    return "Auto Icon";
                }
                else
                {
                    return "Manual Icon";
                }
            }
        }

        /// <summary>
        /// Settings interaction.
        /// </summary>
        /// <param name="obj">Object.</param>
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
