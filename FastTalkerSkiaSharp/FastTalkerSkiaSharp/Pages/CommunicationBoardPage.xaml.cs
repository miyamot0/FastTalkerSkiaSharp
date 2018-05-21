/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using SkiaSharp;
using System.Linq;
using Rg.Plugins.Popup.Extensions;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class CommunicationBoardPage : Xamarin.Forms.ContentPage
    {
        SkiaSharp.Elements.Element _currentElement;
        SKPoint? _startLocation;

        SkiaSharp.Elements.Element emitterReference, stripReference;

        System.DateTime emitterPressTime;
        System.DateTime itemPressTime;

        bool holdingEmitter = false;
        bool inInitialLoading = true;
        bool hasMoved = false;

        int thresholdAdmin = 3;
        int thresholdReset = 10;

        Interfaces.InterfaceSpeechOutput commInterface;

        public CommunicationBoardPage()
        {
            InitializeComponent();

            App.ImageBuilderInstance = new Helpers.ImageBuilder(canvas);
            App.UserInputInstance = new Helpers.UserInput(canvas);

            canvas.Controller.OnElementsChanged += SaveCurrentBoard;
            canvas.Controller.OnSettingsChanged += SaveCurrentSettings;

            commInterface = Xamarin.Forms.DependencyService.Get<Interfaces.InterfaceSpeechOutput>();

            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            Xamarin.Forms.NavigationPage.SetHasBackButton(this, false);
        }

        /// <summary>
        /// Saves the current board.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void SaveCurrentBoard(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Saving (event-based)");

            System.Collections.Generic.List<Storage.CommunicationIcon> toInsert = new System.Collections.Generic.List<Storage.CommunicationIcon>();

            var currentItems = canvas.Elements?.Where(elem => elem.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication) ||
                                                      elem.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder));

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
        async void SaveCurrentSettings(object sender, System.EventArgs e)
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

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                Xamarin.Forms.DependencyService.Get<Interfaces.InterfaceAdministration>().RequestAdmin(!canvas.Controller.InEditMode);
            }

            await App.Database.InsertOrUpdateAsync(App.BoardSettings);

            ClearIconsInPlay();
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
                    await System.Threading.Tasks.Task.Delay(50);
                    System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "waiting...");
                }

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "GetSettingsAsync");

                GetSettingsAsync();

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "AddStaticContent");

                AddStaticContent();

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "GetIconsAsync");

                GetIconsAsync();

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Loading..");

                CheckPermissions();

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Requesting permissions..");

                inInitialLoading = false;

                await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(new HelpPopup());
            }
        }

        /// <summary>
        /// Checks the permissions.
        /// </summary>
        async void CheckPermissions()
        {
            var cameraStatus = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);
            var storageStatus = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

            if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted || storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var results = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(new[]
                {
                    Plugin.Permissions.Abstractions.Permission.Camera,
                    Plugin.Permissions.Abstractions.Permission.Storage
                });

                cameraStatus = results[Plugin.Permissions.Abstractions.Permission.Camera];
                storageStatus = results[Plugin.Permissions.Abstractions.Permission.Storage];
            }

            if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted || storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Permissions Denied", "Unable to take photos.");
            }
        }

        /// <summary>
        /// Gets the icons async.
        /// </summary>
        async void GetIconsAsync()
        {
            var icons = await App.Database.GetIconsAsync();

            if (icons != null && icons.Count > 0)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "icon: " + icons.Count);

                foreach (var icon in icons)
                {
                    System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose,
                                    "Tag: " + icon.Tag +
                                    " Name: " + icon.Text +
                                    " Scale: " + icon.Scale +
                                    " Local: " + icon.Local +
                                    " Stored Bool: " + icon.IsStoredInFolder +
                                    " Base64: " + icon.Base64 +
                                    " FolderTag: " + icon.FolderContainingIcon);

                    if (icon.Local && icon.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication))
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationIconLocal(icon));
                    }
                    else if (!icon.Local && icon.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication))
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationIconDynamic(icon));
                    }
                    else if (icon.Local && icon.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder))
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationFolderLocal(icon));
                    }
                }
            }
            else if (icons != null && icons.Count == 0)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "No icons");
            }
            else
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Null");
            }

            ClearIconsInPlay();
        }

        /// <summary>
        /// Gets the settings async.
        /// </summary>
        async void GetSettingsAsync()
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
        /// Keep icon within bounds of canvas
        /// </summary>
        void ClampCurrentIconToCanvasBounds()
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
        void Canvas_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            e.Handled = true;

            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "e.ActionType = " + e.ActionType.ToString());
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "e.InContact = " + e.InContact.ToString());

            switch (e.ActionType)
            {
                case SkiaSharp.Views.Forms.SKTouchAction.Pressed:
                    ProcessInitialTouchEvent(e, outputVerbose: App.OutputVerbose);
                    return;

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

            itemPressTime = System.DateTime.Now;

            // Get origin of element
            _startLocation = _currentElement.Location;

            switch (_currentElement.Tag)
            {
                case (int)Elements.ElementRoles.Role.SentenceFrame:
                    System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Hit sentence frame");

                    return;

                case (int)Elements.ElementRoles.Role.Emitter:
                    System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Hit speech emitter");
                    holdingEmitter = true;
                    emitterPressTime = System.DateTime.Now;

                    return;

                case (int)Elements.ElementRoles.Role.Settings:
                    System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Hit settings");

                    if (canvas.Controller.InEditMode)
                    {
                        App.UserInputInstance.QueryUserMainSettingsAsync();
                    }

                    return;

                case (int)Elements.ElementRoles.Role.Folder:
                    System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Hit Folder");
                    ClearIconsInPlay();
                    canvas.Elements.BringToFront(_currentElement);

                    return;

                default:
                    System.Diagnostics.Debug.WriteLineIf(outputVerbose, "In Default Hit");
                    ClearIconsInPlay();
                    canvas.Elements.BringToFront(_currentElement);

                    if (!canvas.Controller.InFramedMode && !canvas.Controller.IconModeAuto)
                    {
                        _currentElement.IsMainIconInPlay = true;
                    }
                    else if (!canvas.Controller.InFramedMode &&
                             _currentElement != null &&
                             _currentElement.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication) &&
                             !canvas.Controller.InEditMode &&
                             canvas.Controller.IconModeAuto)
                    {
                        commInterface.SpeakText(_currentElement.Text);

                        e.Handled = true;
                    }
                    /* Pinned icons in sentence mode */
                    else if (canvas.Controller.InFramedMode && 
                             !canvas.Controller.InEditMode &&
                             _currentElement.IsPinnedToSpot && 
                             !stripReference.Bounds.IntersectsWith(_currentElement.Bounds))
                    {
                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Hit a pinned icon in sentence mode, outside of frame");

                        e.Handled = true;

                        commInterface.SpeakText(text: _currentElement.Text);

                        _currentElement = null;
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
                case (int)Elements.ElementRoles.Role.Control:

                    return;

                case (int)Elements.ElementRoles.Role.Emitter:

                    return;

                case (int)Elements.ElementRoles.Role.Communication:
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

                    _currentElement.IsInsertableIntoFolder = canvas.Elements.Where(elem => elem.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder))
                                        .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds))
                                        .Any();

                    _startLocation = _currentElement.Location;

                    return;

                case (int)Elements.ElementRoles.Role.Folder:
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
                case (int)Elements.ElementRoles.Role.Communication:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Completed icon tap");

                        if (App.InstanceModificationPage == null)
                        {
                            App.InstanceModificationPage = new ModifyPage(_currentElement, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentElement);
                        }

                        await Navigation.PushPopupAsync(App.InstanceModificationPage);
                    }
                    else if (canvas.Controller.InEditMode &&
                             !_currentElement.IsInsertableIntoFolder &&
                             System.DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Completed icon held > 3s");

                        if (App.InstanceModificationPage == null)
                        {
                            App.InstanceModificationPage = new ModifyPage(_currentElement, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentElement);
                        }

                        await Navigation.PushPopupAsync(App.InstanceModificationPage);
                    }
                    else if (hasMoved && _currentElement.IsInsertableIntoFolder)
                    {
                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Icon completed, has moved");

                        System.Collections.Generic.IEnumerable<SkiaSharp.Elements.Element> folderOfInterest = canvas.Elements
                            .Where(elem => elem.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder) && !elem.IsStoredInAFolder)
                                      .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds));

                        App.UserInputInstance.InsertIntoFolder(_currentElement: _currentElement,
                                                               folderOfInterest: folderOfInterest);
                    }

                    e.Handled = true;

                    return;

                case (int)Elements.ElementRoles.Role.Folder:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Completed folder tap");

                        if (App.InstanceModificationPage == null)
                        {
                            App.InstanceModificationPage = new ModifyPage(_currentElement, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentElement);
                        }

                        await Navigation.PushPopupAsync(App.InstanceModificationPage);

                        e.Handled = true;
                    }
                    else if (canvas.Controller.InEditMode && System.DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Completed folder hold");

                        if (App.InstanceModificationPage == null)
                        {
                            App.InstanceModificationPage = new ModifyPage(_currentElement, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentElement);
                        }

                        await Navigation.PushPopupAsync(App.InstanceModificationPage);

                        e.Handled = true;
                    }

                    if (!canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Hit a folder, in user mode: " + _currentElement.Text);

                        // This is where the current item is the folder in question
                        System.Collections.Generic.List<SkiaSharp.Elements.Element> itemsMatching = canvas.Controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == _currentElement.Text).ToList();

                        // Leave if empty
                        if (itemsMatching == null)
                        {
                            e.Handled = true;

                            return;
                        }

                        if (App.InstanceStoredIconsViewModel == null)
                        {
                            App.InstanceStoredIconsViewModel = new ViewModels.StoredIconPopupViewModel
                            {
                                Padding = new Xamarin.Forms.Thickness(100, 100, 100, 100),
                                IsSystemPadding = true,
                                FolderWithIcons = _currentElement.Text,
                                ItemsMatching = itemsMatching,
                            };

                            App.InstanceStoredIconsViewModel.IconSelected += RestoreIcon;

                            App.InstanceStoredIconPage = new StoredIconPopup()
                            {
                                BindingContext = App.InstanceStoredIconsViewModel
                            };
                        }
                        else
                        {
                            App.InstanceStoredIconsViewModel.FolderWithIcons = _currentElement.Text;
                            App.InstanceStoredIconsViewModel.ItemsMatching = itemsMatching;
                        }

                        await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(App.InstanceStoredIconPage);

                        e.Handled = true;
                    }
                    else if (!canvas.Controller.InEditMode && System.DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Held a folder, in user mode: " + _currentElement.Text);

                        // This is where the current item is the folder in question
                        System.Collections.Generic.List<SkiaSharp.Elements.Element> itemsMatching = canvas.Controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == _currentElement.Text).ToList();

                        // Leave if empty
                        if (itemsMatching == null)
                        {
                            e.Handled = true;

                            return;
                        }

                        if (App.InstanceStoredIconsViewModel == null)
                        {
                            App.InstanceStoredIconsViewModel = new ViewModels.StoredIconPopupViewModel
                            {
                                Padding = new Xamarin.Forms.Thickness(100, 100, 100, 100),
                                IsSystemPadding = true,
                                FolderWithIcons = _currentElement.Text,
                                ItemsMatching = itemsMatching,
                            };

                            App.InstanceStoredIconsViewModel.IconSelected += RestoreIcon;

                            App.InstanceStoredIconPage = new StoredIconPopup()
                            {
                                BindingContext = App.InstanceStoredIconsViewModel
                            };
                        }
                        else
                        {
                            App.InstanceStoredIconsViewModel.FolderWithIcons = _currentElement.Text;
                            App.InstanceStoredIconsViewModel.ItemsMatching = itemsMatching;
                        }

                        await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(App.InstanceStoredIconPage);

                        e.Handled = true;
                    }

                    return;

                default:
                    // Emitter was tapped/held
                    if (holdingEmitter)
                    {
                        holdingEmitter = false;

                        System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Seconds held: " + (System.DateTime.Now - emitterPressTime).TotalSeconds.ToString());

                        if ((System.DateTime.Now - emitterPressTime).Seconds >= thresholdAdmin && !canvas.Controller.InEditMode)
                        {
                            canvas.Controller.UpdateSettings(isEditing: !canvas.Controller.InEditMode,
                                                             isInFrame: canvas.Controller.InFramedMode,
                                                             isAutoDeselecting: canvas.Controller.RequireDeselect,
                                                             isInIconModeAuto: canvas.Controller.IconModeAuto);

                            canvas.Controller.BackgroundColor = canvas.Controller.InEditMode ? SKColors.DarkOrange : SKColors.DimGray;

                            ClearIconsInPlay();
                        }
                        else if ((System.DateTime.Now - emitterPressTime).Seconds >= thresholdReset && canvas.Controller.InEditMode)
                        {
                            // TODO: Confirm message?

                            System.Diagnostics.Debug.WriteLineIf(outputVerbose, "In reset hook, returning to home");

                            Xamarin.Forms.Application.Current.MainPage = new TitlePage();
                        }
                        else
                        {
                            if (canvas.Controller.InFramedMode)
                            {
                                var mIntersectingElements = canvas?.Elements
                                                                   .Where(elem => elem.IsSpeakable && elem.Tag != Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder))
                                                                   .OrderBy(elem => elem.Left)
                                                                   .Select(elem => elem.Text);

                                if (mIntersectingElements != null && mIntersectingElements.Count() > 0)
                                {
                                    var output = System.String.Join(" ", mIntersectingElements);

                                    System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Verbal output (Frame): " + output);

                                    commInterface.SpeakText(text: output);
                                }
                            }
                            else if (!canvas.Controller.IconModeAuto)
                            {
                                var selectedElements = canvas?.Elements
                                                              .Where(elem => elem.IsMainIconInPlay && elem.Tag != Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder))
                                                              .Select(elem => elem.Text)
                                                              .FirstOrDefault();

                                if (selectedElements != null)
                                {
                                    System.Diagnostics.Debug.WriteLineIf(outputVerbose, "Verbal output (Icon): " + selectedElements);

                                    commInterface.SpeakText(text: selectedElements);
                                }

                                if (canvas.Controller.RequireDeselect)
                                {
                                    ClearIconsInPlay();
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
        void RestoreIcon(Helpers.ArgsSelectedIcon obj)
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "RestoreIcon(ArgsSelectedIcon obj) Name: " + obj.Name +
                                                " ImageSourceResource: " + obj.ImageSource);

            bool check = canvas.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).Any();

            if (check)
            {
                SkiaSharp.Elements.Element item = canvas?.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).First();

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Pass check? " + check + " Text: " + item.Text);

                item.IsInsertableIntoFolder = false;
                item.IsStoredInAFolder = false;
                item.StoredFolderTag = "";
                item.Location = Constants.DeviceLayout.GetCenterPointWithJitter(deviceSize: canvas.CanvasSize,
                                                                      iconReference: item.Size);

                canvas.Elements.BringToFront(item);

                canvas.Controller.PromptResave();
            }
        }

        /// <summary>
        /// Clears the icons in play.
        /// </summary>
        void ClearIconsInPlay()
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

            canvas.InvalidateSurface();
        }

        /// <summary>
        /// Adds the content of the static.
        /// </summary>
        void AddStaticContent()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Adding Static Content");
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Width: " + canvas.CanvasSize.Width);
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Height: " + canvas.CanvasSize.Height);

            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Layout Width: " + hackLayout.Width);
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Layout Height: " + hackLayout.Height);

            // Sentence Strip
            stripReference = App.ImageBuilderInstance.BuildSentenceStrip();

            canvas.Elements.Add(stripReference);

            // Speech Emitter
            emitterReference = App.ImageBuilderInstance.BuildStaticElement(resource: "FastTalkerSkiaSharp.Images.Speaker.png",
                                                                           xPercent: 2f,
                                                                           yPercent: 1.5f,
                                                                           tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));
            canvas.Elements.Add(emitterReference);

            // Settings
            SkiaSharp.Elements.Element settingsElement = App.ImageBuilderInstance.BuildNamedIcon(resource: "FastTalkerSkiaSharp.Images.Settings.png",
                                                                                                 text: "Settings",
                                                                                                 x: canvas.CanvasSize.Width - Constants.DeviceLayout.Bezel,
                                                                                                 y: canvas.CanvasSize.Height - Constants.DeviceLayout.Bezel,
                                                                                                 tagCode: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Settings),
                                                                                                 alignRight: true,
                                                                                                 alignBottom: true,
                                                                                                 opaqueBackground: true);

            canvas.Elements.Add(settingsElement);
        }
    }
}