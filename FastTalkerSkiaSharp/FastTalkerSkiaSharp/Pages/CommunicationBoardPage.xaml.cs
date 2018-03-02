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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Interfaces;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Extensions;
using SkiaSharp;
using Xamarin.Forms;
using FastTalkerSkiaSharp.ViewModels;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class CommunicationBoardPage : ContentPage
    {
        private SkiaSharp.Elements.Element _currentElement;
        private SKPoint? _startLocation;

        private SkiaSharp.Elements.Element emitterReference, stripReference;

        private bool holdingEmitter = false;
        private DateTime emitterPressTime;
        private DateTime itemPressTime;

        private bool inInitialLoading = true;
        private bool hasMoved = false;

        InterfaceSpeechOutput commInterface;

        public CommunicationBoardPage()
        {
            InitializeComponent();

            App.ImageBuilderInstance = new ImageBuilder(canvas);
            App.UserInputInstance = new UserInput(canvas);

            canvas.Controller.OnElementsChanged += SaveCurrentBoard;
            canvas.Controller.OnSettingsChanged += SaveCurrentSettings;

            commInterface = DependencyService.Get<InterfaceSpeechOutput>();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
        }

        /// <summary>
        /// Saves the current board.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void SaveCurrentBoard(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Saving (event-based)");

            List<Storage.CommunicationIcon> toInsert = new List<Storage.CommunicationIcon>();

            var currentItems = canvas.Elements?.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication ||
                                                              elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder);

            if (currentItems != null)
            {
                foreach (var element in currentItems)
                {
                    toInsert.Add(new Storage.CommunicationIcon()
                    {
                        Text = element.Text,
                        X = element.Left,
                        Y = element.Top,
                        Tag = element.Tag,
                        Local = element.LocalImage,
                        TextVisible = true,
                        Base64 = (element.LocalImage) ? "" : element.ImageInformation,
                        ResourceLocation = (element.LocalImage) ? element.ImageInformation : "",
                        IsStoredInFolder = element.IsStoredInAFolder,
                        FolderContainingIcon = element.StoredFolderTag,
                        Scale = element.CurrentScale,
                        IsPinned = element.IsPinnedToSpot,
                        TextScale = 1f,
                        HashCode = element.GetHashCode()
                    });
                }

                int saveResult = await App.Database.InsertOrUpdateAsync(toInsert);

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Results: Saved " + saveResult + " icons to db");
            }
        }

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void SaveCurrentSettings(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Saving settings (event-based)");

            App.BoardSettings.InEditMode = canvas.Controller.InEditMode;
            App.BoardSettings.InFramedMode = canvas.Controller.InFramedMode;
            App.BoardSettings.RequireDeselect = canvas.Controller.RequireDeselect;
            App.BoardSettings.InIconModeAuto = canvas.Controller.IconModeAuto;

            if (App.BoardSettings.InFramedMode)
            {
                App.BoardSettings.InIconModeAuto = false;

                canvas.InvalidateSurface();
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<InterfaceAdministration>().RequestAdmin(!canvas.Controller.InEditMode);
            }

            await App.Database.InsertOrUpdateAsync(App.BoardSettings);
        }

        /// <summary>
        /// Gives the views time to size up
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadingPrepAsync();
        }

        /// <summary>
        /// Buy some time if android needs to inflate layouts
        /// </summary>
        public async void LoadingPrepAsync()
        {
            if (inInitialLoading)
            {
                while (canvas.CanvasSize.Width == 0)
                {
                    await Task.Delay(50);
                    Debug.WriteLineIf(App.OutputVerbose, "waiting...");
                }

                Debug.WriteLineIf(App.OutputVerbose, "GetSettingsAsync");

                GetSettingsAsync();

                Debug.WriteLineIf(App.OutputVerbose, "AddStaticContent");

                AddStaticContent();

                Debug.WriteLineIf(App.OutputVerbose, "GetIconsAsync");

                GetIconsAsync();

                Debug.WriteLineIf(App.OutputVerbose, "Loading..");

                CheckPermissions();

                Debug.WriteLineIf(App.OutputVerbose, "Requesting permissions..");

                inInitialLoading = false;
            }
        }

        /// <summary>
        /// Checks the permissions.
        /// </summary>
        private async void CheckPermissions()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] 
                { 
                    Permission.Camera, 
                    Permission.Storage 
                });

                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                await UserDialogs.Instance.AlertAsync("Permissions Denied", "Unable to take photos.");
            }
        }

        /// <summary>
        /// Gets the icons async.
        /// </summary>
        private async void GetIconsAsync()
        {
            var icons = await App.Database.GetIconsAsync();

            if (icons != null && icons.Count > 0)
            {
                Debug.WriteLineIf(App.OutputVerbose, "icon: " + icons.Count);

                foreach (var icon in icons)
                {
                    Debug.WriteLineIf(App.OutputVerbose,
                                    "Tag: " + icon.Tag +
                                    " Name: " + icon.Text +
                                    " Scale: " + icon.Scale +
                                    " Local: " + icon.Local +
                                    " Stored Bool: " + icon.IsStoredInFolder +
                                    " Base64: " + icon.Base64 +
                                    " FolderTag: " + icon.FolderContainingIcon);

                    if (icon.Local && icon.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication)
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationIconLocal(icon));
                    }
                    else if (!icon.Local && icon.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication)
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationIconDynamic(icon));
                    }
                    else if (icon.Local && icon.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationFolderLocal(icon));
                    }
                }
            }
            else if (icons != null && icons.Count == 0)
            {
                Debug.WriteLineIf(App.OutputVerbose, "No icons");
            }
            else
            {
                Debug.WriteLineIf(App.OutputVerbose, "Null");
            }

            ClearIconsInPlay();

            canvas.InvalidateSurface();
        }

        /// <summary>
        /// Gets the settings async.
        /// </summary>
        private async void GetSettingsAsync()
        {
            App.BoardSettings = await App.Database.GetSettingsAsync();

            canvas.Controller.BackgroundColor = App.BoardSettings.InEditMode ? SKColors.DarkOrange : SKColors.DimGray;

            canvas.Controller.UpdateSettings(App.BoardSettings.InEditMode,
                                             App.BoardSettings.InFramedMode,
                                             App.BoardSettings.RequireDeselect,
                                             App.BoardSettings.InIconModeAuto,
                                             overridePrompt: true);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClampCurrentIconToCanvasBounds()
        {
            if (_currentElement.Top <= 0)
            {
                _currentElement.Y -= _currentElement.Top;
            }

            if (_currentElement.Bottom >= canvas.CanvasSize.Height)
            {
                _currentElement.Y -= (_currentElement.Bottom - canvas.CanvasSize.Height);
            }

            if (_currentElement.Left <= 0)
            {
                _currentElement.X -= _currentElement.Left;
            }

            if (_currentElement.Right >= canvas.CanvasSize.Width)
            {
                _currentElement.X -= (_currentElement.Right - canvas.CanvasSize.Width);
            }
        }

        /// <summary>
        /// Canvases the touch.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Canvas_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            e.Handled = true;

            Debug.WriteLineIf(App.OutputVerbose, "e.ActionType = " + e.ActionType.ToString());
            Debug.WriteLineIf(App.OutputVerbose, "e.InContact = " + e.InContact.ToString());

            switch (e.ActionType)
            {
                case SkiaSharp.Views.Forms.SKTouchAction.Pressed:
                    ProcessInitialTouchEvent(e, outputVerbose: App.OutputVerbose);

                    break;

                case SkiaSharp.Views.Forms.SKTouchAction.Moved:
                    ProcessMovedTouchEvent(e, outputVerbose: App.OutputVerbose);

                    return;

                case SkiaSharp.Views.Forms.SKTouchAction.Released:
                    ProcessCompletedTouchEvent(e, outputVerbose: App.OutputVerbose);

                    return;
                    
                default:
                    _currentElement = null;

                    return;
            }        
        }

        /// <summary>
        /// Process initial touches
        /// </summary>
        /// <param name="e"></param>
        /// <param name="outputVerbose"></param>
        void ProcessInitialTouchEvent(SkiaSharp.Views.Forms.SKTouchEventArgs e, bool outputVerbose = false)
        {
            _currentElement = canvas.GetElementAtPoint(e.Location);

            // Confirmation of movement
            hasMoved = false;

            // Fail out if null
            if (_currentElement == null) return;

            itemPressTime = DateTime.Now;

            // Get origin of element
            _startLocation = _currentElement.Location;

            switch (_currentElement.Tag)
            {
                case (int)SkiaSharp.Elements.CanvasView.Role.SentenceFrame:
                    Debug.WriteLineIf(outputVerbose, "Hit sentence frame");

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Emitter:
                    Debug.WriteLineIf(outputVerbose, "Hit speech emitter");
                    holdingEmitter = true;
                    emitterPressTime = DateTime.Now;

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Settings:
                    Debug.WriteLineIf(outputVerbose, "Hit settings");

                    if (canvas.Controller.InEditMode) 
                    {
                        App.UserInputInstance.QueryUserMainSettingsAsync();
                    }

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Folder:
                    Debug.WriteLineIf(outputVerbose, "Hit Folder");
                    ClearIconsInPlay();
                    canvas.Elements.BringToFront(_currentElement);

                    return;

                default:
                    Debug.WriteLineIf(outputVerbose, "In Default Hit");
                    ClearIconsInPlay();
                    canvas.Elements.BringToFront(_currentElement);

                    if (!canvas.Controller.InFramedMode && !canvas.Controller.IconModeAuto)
                    {
                        _currentElement.IsMainIconInPlay = true;
                    }
                    else if (!canvas.Controller.InFramedMode && 
                             _currentElement != null &&
                             _currentElement.Tag == (int) SkiaSharp.Elements.CanvasView.Role.Communication &&
                             !canvas.Controller.InEditMode &&
                             canvas.Controller.IconModeAuto)
                    {
                        commInterface.SpeakText(_currentElement.Text);

                        e.Handled = true;
                    }

                    return;
            }
        }

        /// <summary>
        /// Process moving touches
        /// </summary>
        /// <param name="e"></param>
        /// <param name="outputVerbose"></param>
        void ProcessMovedTouchEvent(SkiaSharp.Views.Forms.SKTouchEventArgs e, bool outputVerbose = false)
        {
            // If out of scope, return
            if (_currentElement == null) return;
            
            switch (_currentElement.Tag)
            {
                case (int)SkiaSharp.Elements.CanvasView.Role.Control:

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Emitter:

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Communication:
                    hasMoved = true;

                    // If pinned, prevent move
                    if (_currentElement.IsPinnedToSpot && !canvas.Controller.InEditMode) return;

                    _currentElement.Location = new SKPoint(e.Location.X - _currentElement.Bounds.Width / 2f,
                                                           e.Location.Y - _currentElement.Bounds.Height / 2f);

                    ClampCurrentIconToCanvasBounds();

                    if (canvas.Controller.InFramedMode)
                    {
                        _currentElement.IsSpeakable = _currentElement.Bounds.IntersectsWith(stripReference.Bounds);
                    }
                    else
                    {
                        for (int i = 0; i < canvas.Elements.Count; i++)
                        {
                            canvas.Elements[i].IsMainIconInPlay = false;
                        }

                        _currentElement.IsMainIconInPlay = true;
                    }

                    _currentElement.IsInsertableIntoFolder = canvas.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                                        .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds))
                                        .Any();

                    _startLocation = _currentElement.Location;

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Folder:
                    hasMoved = true;

                    if (!canvas.Controller.InEditMode) return;

                    _currentElement.Location = new SKPoint(e.Location.X - _currentElement.Bounds.Width / 2f,
                                        e.Location.Y - _currentElement.Bounds.Height / 2f);

                    ClampCurrentIconToCanvasBounds();

                    _startLocation = _currentElement.Location;

                    return;

                default:

                    return;

            }            
        }

        /// <summary>
        /// Process touch completed events
        /// </summary>
        /// <param name="e"></param>
        /// <param name="outputVerbose"></param>
        async void ProcessCompletedTouchEvent(SkiaSharp.Views.Forms.SKTouchEventArgs e, bool outputVerbose = false)
        {
            // If out of scope, return
            if (_currentElement == null) return;

            switch (_currentElement.Tag)
            {
                case (int)SkiaSharp.Elements.CanvasView.Role.Communication:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        Debug.WriteLineIf(outputVerbose, "Completed icon tap");

                        ModifyPage iconModificationPopup = new ModifyPage(_currentElement, canvas.Controller);

                        await Navigation.PushPopupAsync(iconModificationPopup);
                    }
                    else if (canvas.Controller.InEditMode && 
                             !_currentElement.IsInsertableIntoFolder && 
                             DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        Debug.WriteLineIf(outputVerbose, "Completed icon held > 3s");

                        ModifyPage iconModificationPopup = new ModifyPage(_currentElement, canvas.Controller);

                        await Navigation.PushPopupAsync(iconModificationPopup);
                    }
                    else if (hasMoved && _currentElement.IsInsertableIntoFolder)
                    {
                        Debug.WriteLineIf(outputVerbose, "Icon completed, has moved");

                        IEnumerable<SkiaSharp.Elements.Element> folderOfInterest = canvas.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder && 
                                                                                                         !elem.IsStoredInAFolder)
                                      .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds));

                        App.UserInputInstance.InsertIntoFolder(_currentElement: _currentElement, 
                                                               folderOfInterest: folderOfInterest);
                    }

                    e.Handled = true;

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Folder:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        Debug.WriteLineIf(outputVerbose, "Completed folder tap");

                        ModifyPage iconModificationPopup = new ModifyPage(_currentElement, canvas.Controller);

                        await Navigation.PushPopupAsync(iconModificationPopup);

                        e.Handled = true;
                    }
                    else if (canvas.Controller.InEditMode && DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        Debug.WriteLineIf(outputVerbose, "Completed folder hold");

                        ModifyPage iconModificationPopup = new ModifyPage(_currentElement, canvas.Controller);

                        await Navigation.PushPopupAsync(iconModificationPopup);

                        e.Handled = true;
                    }

                    if (!canvas.Controller.InEditMode && !hasMoved)
                    {
                        Debug.WriteLineIf(outputVerbose, "Hit a folder, in user mode: " + _currentElement.Text);

                        // This is where the current item is the folder in question
                        List<SkiaSharp.Elements.Element> itemsMatching = canvas.Controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == _currentElement.Text).ToList();

                        // Leave if empty
                        if (itemsMatching == null)
                        {
                            e.Handled = true;

                            return;
                        }

                        StoredIconPopupViewModel viewModel = new StoredIconPopupViewModel
                        {
                            Padding = new Thickness(100, 100, 100, 100),
                            IsSystemPadding = true,
                            FolderWithIcons = _currentElement.Text,
                            ItemsMatching = itemsMatching,
                        };

                        viewModel.IconSelected += RestoreIcon;

                        StoredIconPopup page = new StoredIconPopup()
                        {
                            BindingContext = viewModel
                        };

                        await App.Current.MainPage.Navigation.PushPopupAsync(page);

                        e.Handled = true;
                    }
                    else if (!canvas.Controller.InEditMode && DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        Debug.WriteLineIf(outputVerbose, "Held a folder, in user mode: " + _currentElement.Text);

                        // This is where the current item is the folder in question
                        List<SkiaSharp.Elements.Element> itemsMatching = canvas.Controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == _currentElement.Text).ToList();

                        // Leave if empty
                        if (itemsMatching == null)
                        {
                            e.Handled = true;

                            return;
                        }

                        StoredIconPopupViewModel viewModel = new StoredIconPopupViewModel
                        {
                            Padding = new Thickness(100, 100, 100, 100),
                            IsSystemPadding = true,
                            FolderWithIcons = _currentElement.Text,
                            ItemsMatching = itemsMatching,
                        };

                        viewModel.IconSelected += RestoreIcon;

                        StoredIconPopup page = new StoredIconPopup()
                        {
                            BindingContext = viewModel
                        };

                        await App.Current.MainPage.Navigation.PushPopupAsync(page);

                        e.Handled = true;
                    }

                    return;

                default:
                    // Emitter was tapped/held
                    if (holdingEmitter)
                    {
                        holdingEmitter = false;

                        Debug.WriteLineIf(outputVerbose, "Seconds held: " + (DateTime.Now - emitterPressTime).TotalSeconds.ToString());

                        if ((DateTime.Now - emitterPressTime).Seconds >= 3 && !canvas.Controller.InEditMode)
                        {

                            canvas.Controller.UpdateSettings(isEditing: !canvas.Controller.InEditMode,
                                                             isInFrame: canvas.Controller.InFramedMode,
                                                             isAutoDeselecting: canvas.Controller.RequireDeselect,
                                                             isInIconModeAuto: canvas.Controller.IconModeAuto);

                            canvas.Controller.BackgroundColor = canvas.Controller.InEditMode ? SKColors.DarkOrange : SKColors.DimGray;

                            ClearIconsInPlay();

                            canvas.InvalidateSurface();
                        }
                        else
                        {
                            if (canvas.Controller.InFramedMode)
                            {
                                var mIntersectingElements = canvas?.Elements
                                                                   .Where(elem => elem.IsSpeakable && elem.Tag != (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                                                                   .OrderBy(elem => elem.Left)
                                                                   .Select(elem => elem.Text);

                                if (mIntersectingElements != null && mIntersectingElements.Count() > 0)
                                {
                                    var output = String.Join(" ", mIntersectingElements);

                                    Debug.WriteLineIf(outputVerbose, "Verbal output (Frame): " + output);

                                    commInterface.SpeakText(text: output);
                                }
                            }
                            else if (!canvas.Controller.IconModeAuto)
                            {
                                var selectedElements = canvas?.Elements
                                                              .Where(elem => elem.IsMainIconInPlay && elem.Tag != (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                                                              .Select(elem => elem.Text)
                                                              .FirstOrDefault();

                                if (selectedElements != null)
                                {
                                    Debug.WriteLineIf(outputVerbose, "Verbal output (Icon): " + selectedElements);

                                    commInterface.SpeakText(text: selectedElements);
                                }

                                if (canvas.Controller.RequireDeselect)
                                {
                                    ClearIconsInPlay();

                                    canvas.InvalidateSurface();
                                }
                            }

                        }

                        e.Handled = true;
                    }

                    return;
            }
        }

        /// <summary>
        /// Restores the icon.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void RestoreIcon(ArgsSelectedIcon obj)
        {
            Debug.WriteLineIf(App.OutputVerbose, "RestoreIcon(ArgsSelectedIcon obj) Name: " + obj.Name + 
                                                " ImageSourceResource: " + obj.ImageSource);

            bool check = canvas.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).Any();

            if (check)
            {
                SkiaSharp.Elements.Element item = canvas?.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).First();

                Debug.WriteLineIf(App.OutputVerbose, "Pass check? " + check + " Text: " + item.Text);

                item.IsInsertableIntoFolder = false;
                item.IsStoredInAFolder = false;
                item.StoredFolderTag = "";
                item.Location = DeviceLayout.GetCenterPointWithJitter(deviceSize: canvas.CanvasSize, 
                                                                      iconReference: item.Size);

                canvas.Elements.BringToFront(item);
                canvas.InvalidateSurface();

                canvas.Controller.PromptResave();
            }
        }

        /// <summary>
        /// Clears the icons in play.
        /// </summary>
        private void ClearIconsInPlay()
        {
            if (!canvas.Controller.InFramedMode)
            {
                for (int i = 0; i < canvas.Elements.Count; i++)
                {
                    canvas.Elements[i].IsMainIconInPlay = false;
                }
            }
            else
            {
                var isGreenHighlighted = false;

                for (int i = 0; i < canvas.Elements.Count; i++)
                {
                    isGreenHighlighted = canvas.Elements[i].Bounds.IntersectsWith(stripReference.Bounds);

                    canvas.Elements[i].IsSpeakable = isGreenHighlighted;
                }
            }
        }

        /// <summary>
        /// Adds the content of the static.
        /// </summary>
        private void AddStaticContent()
        {
            Debug.WriteLineIf(App.OutputVerbose, "Adding Static Content");
            Debug.WriteLineIf(App.OutputVerbose, "Width: " + canvas.CanvasSize.Width);
            Debug.WriteLineIf(App.OutputVerbose, "Height: " + canvas.CanvasSize.Height);

            Debug.WriteLineIf(App.OutputVerbose, "Layout Width: " + hackLayout.Width);
            Debug.WriteLineIf(App.OutputVerbose, "Layout Height: " + hackLayout.Height);

            // Sentence Strip
            stripReference = App.ImageBuilderInstance.BuildSentenceStrip();

            canvas.Elements.Add(stripReference);

            // Speech Emitter
            emitterReference = App.ImageBuilderInstance.BuildStaticElement(resource: "FastTalkerSkiaSharp.Images.Speaker.png",
                                                                           xPercent: 2f,
                                                                           yPercent: 1.5f,
                                                                           tag: (int)SkiaSharp.Elements.CanvasView.Role.Emitter);
            canvas.Elements.Add(emitterReference);

            // Settings
            SkiaSharp.Elements.Element settingsElement = App.ImageBuilderInstance.BuildNamedIcon(resource: "FastTalkerSkiaSharp.Images.Settings.png",
                                                                                                 text: "Settings",
                                                                                                 x: canvas.CanvasSize.Width - Constants.DeviceLayout.Bezel,
                                                                                                 y: canvas.CanvasSize.Height - Constants.DeviceLayout.Bezel,
                                                                                                 tagCode: (int)SkiaSharp.Elements.CanvasView.Role.Settings,
                                                                                                 alignRight: true,
                                                                                                 alignBottom: true,
                                                                                                 opaqueBackground: true);

            canvas.Elements.Add(settingsElement);

            /*
            // Delete zone
            deleteReference = App.ImageBuilderInstance.BuildNamedIcon(resource: "FastTalkerSkiaSharp.Images.Trash.png",
                                                                      text: "Delete",
                                                                      x: Constants.DeviceLayout.Bezel,
                                                                      y: canvas.CanvasSize.Height - Constants.DeviceLayout.Bezel,
                                                                      tagCode: (int)SkiaSharp.Elements.CanvasView.Role.Delete,
                                                                      alignBottom: true,
                                                                      opaqueBackground: true);

            canvas.Elements.Add(deleteReference);
            */
        }
    }
}