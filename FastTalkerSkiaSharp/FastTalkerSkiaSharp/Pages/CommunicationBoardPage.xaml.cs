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

        SkiaSharp.Elements.Element stripReference;
        SkiaSharp.Elements.Image emitterReference;

        System.DateTime emitterPressTime;
        System.DateTime itemPressTime;

        bool holdingEmitter = false;
        bool inInitialLoading = true;
        bool hasMoved = false;

        int thresholdAdmin = 3;
        int thresholdReset = 10;

        string resource;

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
        /// Gives the views time to size up
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadingPrepAsync();
        }

        /// <summary>
        /// Override back button
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        #region Save Methods

        /// <summary>
        /// Saves the current board.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void SaveCurrentBoard(object sender, System.EventArgs e)
        {
            OutputDebug("Saving (event-based)");

            System.Collections.Generic.List<Storage.CommunicationIcon> toInsert = new System.Collections.Generic.List<Storage.CommunicationIcon>();

            var currentItems = GetIconsUserInteractable();

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

                OutputDebug("Results: Saved " + saveResult + " icons to db");

                toInsert.Clear();
                toInsert = null;

                currentItems.Clear();
                currentItems = null;
            }
        }

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void SaveCurrentSettings(object sender, System.EventArgs e)
        {
            OutputDebug("Saving settings (event-based)");

            CopySettingsFromController();

            if (App.BoardSettings.InFramedMode)
            {
                App.BoardSettings.InIconModeAuto = false;

                resource = App.BoardSettings.InEditMode ? "FastTalkerSkiaSharp.Images.Settings.png" : "FastTalkerSkiaSharp.Images.Speaker.png";

                if (App.BoardSettings.IsBottomOriented)
                {
                    canvas.Elements.Remove(emitterReference);
                    emitterReference = App.ImageBuilderInstance.BuildStaticElementBottom(resource: resource,
                                                       xPercent: 2f,
                                                       yPercent: 1.5f,
                                                       tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));
                    canvas.Elements.Add(emitterReference);

                    canvas.Elements.Remove(stripReference);
                    stripReference = App.ImageBuilderInstance.BuildSentenceStripBottom();
                    canvas.Elements.Add(stripReference);
                }
                else
                {
                    canvas.Elements.Remove(emitterReference);
                    emitterReference = App.ImageBuilderInstance.BuildStaticElement(resource: resource,
                                                       xPercent: 2f,
                                                       yPercent: 1.5f,
                                                       tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));
                    canvas.Elements.Add(emitterReference);

                    canvas.Elements.Remove(stripReference);
                    stripReference = App.ImageBuilderInstance.BuildSentenceStrip();
                    canvas.Elements.Add(stripReference);
                }

                SendReferenceToBack(stripReference);
                SendReferenceToBack(emitterReference);

                canvas.InvalidateSurface();
            }
            else
            {
                resource = App.BoardSettings.InEditMode ? "FastTalkerSkiaSharp.Images.Settings.png" : "FastTalkerSkiaSharp.Images.Speaker.png";

                canvas.Elements.Remove(emitterReference);
                emitterReference = App.ImageBuilderInstance.BuildStaticElement(resource: resource,
                                                   xPercent: 2f,
                                                   yPercent: 1.5f,
                                                   tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));
                canvas.Elements.Add(emitterReference);

                SendReferenceToBack(emitterReference);
            }


            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                Xamarin.Forms.DependencyService.Get<Interfaces.InterfaceAdministration>().RequestAdmin(!canvas.Controller.InEditMode);
            }

            await App.Database.InsertOrUpdateAsync(App.BoardSettings);

            if (canvas.Elements[canvas.Elements.IndexOf(emitterReference)].IsPressed)
            {
                canvas.Elements[canvas.Elements.IndexOf(emitterReference)].IsPressed = false;
                canvas.InvalidateSurface();
            }

            ClearIconsInPlay();
        }

        #endregion

        #region Loading Methods

        /// <summary>
        /// Buy some time if android needs to inflate layouts
        /// </summary>
        public async System.Threading.Tasks.Task LoadingPrepAsync()
        {
            while ((int)canvas.CanvasSize.Width == 0)
            {
                await System.Threading.Tasks.Task.Delay(50);
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "waiting...");
            }

            OutputDebug("GetSettingsAsync");
            await GetSettingsAsync();

            OutputDebug("AddStaticContent");
            AddStaticContent();

            OutputDebug("GetIconsAsync");
            await GetIconsAsync();

            OutputDebug("Requesting permissions..");
            await CheckPermissions();

            if (inInitialLoading)
            {
                inInitialLoading = false;

                await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(new HelpPopup());
            }
        }

        /// <summary>
        /// Gets the icons async.
        /// </summary>
        async System.Threading.Tasks.Task GetIconsAsync()
        {
            var icons = await App.Database.GetIconsAsync();

            if (icons != null && icons.Count > 0)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "icon: " + icons.Count);

                foreach (var icon in icons)
                {
                    OutputDebug("Tag: " + icon.Tag +
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
                OutputDebug("No icons");
            }
            else
            {
                OutputDebug("Null");
            }

            ClearIconsInPlay();
        }

        /// <summary>
        /// Gets the settings async.
        /// </summary>
        async System.Threading.Tasks.Task GetSettingsAsync()
        {
            App.BoardSettings = await App.Database.GetSettingsAsync();

            canvas.Controller.BackgroundColor = App.BoardSettings.InEditMode ? SKColors.DarkOrange : SKColors.DimGray;

            canvas.Controller.UpdateSettings(App.BoardSettings.InEditMode,
                                             App.BoardSettings.InFramedMode,
                                             App.BoardSettings.IsBottomOriented,
                                             App.BoardSettings.RequireDeselect,
                                             App.BoardSettings.InIconModeAuto,
                                             overridePrompt: true);
        }

        #endregion

        #region Element Building

        /// <summary>
        /// Adds the content of the static.
        /// </summary>
        void AddStaticContent()
        {
            OutputDebug("Adding Static Content");
            OutputDebug("Width: " + canvas.CanvasSize.Width);
            OutputDebug("Height: " + canvas.CanvasSize.Height);
            OutputDebug("Layout Width: " + hackLayout.Width);
            OutputDebug("Layout Height: " + hackLayout.Height);

            resource = App.BoardSettings.InEditMode ? "FastTalkerSkiaSharp.Images.Settings.png" : "FastTalkerSkiaSharp.Images.Speaker.png";

            if (canvas.Controller.InFramedModeBottom)
            {
                stripReference = App.ImageBuilderInstance.BuildSentenceStripBottom();

                emitterReference = App.ImageBuilderInstance.BuildStaticElementBottom(resource: resource,
                                               xPercent: 2f,
                                               yPercent: 1.5f,
                                               tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));
            }
            else
            {
                stripReference = App.ImageBuilderInstance.BuildSentenceStrip();

                emitterReference = App.ImageBuilderInstance.BuildStaticElement(resource: resource,
                                                               xPercent: 2f,
                                                               yPercent: 1.5f,
                                                               tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));


            }

            canvas.Elements.Add(stripReference);
            canvas.Elements.SendToBack(stripReference);

            canvas.Elements.Add(emitterReference);
            canvas.Elements.SendToBack(emitterReference);
        }

        #endregion

        #region Touch Interaction

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
        /// Initial routing method
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Canvas_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            e.Handled = true;

            OutputDebug("e.ActionType = " + e.ActionType.ToString());
            OutputDebug("e.InContact = " + e.InContact.ToString());

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
                    OutputDebug("Hit sentence frame");

                    return;

                case (int)Elements.ElementRoles.Role.Emitter:

                    // Serves as Settings button in edit mode
                    if (canvas.Controller.InEditMode)
                    {
                        canvas.Elements[canvas.Elements.IndexOf(emitterReference)].IsPressed = true;
                        canvas.InvalidateSurface();

                        OutputDebug("Hit settings (speech emitter)");
                        App.UserInputInstance.QueryUserMainSettingsAsync();

                    }
                    // Serves as speech emitter as normal
                    else
                    {
                        canvas.Elements[canvas.Elements.IndexOf(emitterReference)].IsPressed = true;

                        canvas.InvalidateSurface();

                        OutputDebug("Hit speech emitter");
                        holdingEmitter = true;
                        emitterPressTime = System.DateTime.Now;
                    }

                    return;

                case (int)Elements.ElementRoles.Role.Folder:
                    OutputDebug("Hit Folder");
                    ClearIconsInPlay();
                    canvas.Elements.BringToFront(_currentElement);

                    return;

                default:
                    OutputDebug("In Default Hit");
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
                        OutputDebug("Hit a pinned icon in sentence mode, outside of frame");

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

            OutputDebug("Moving Event: " + e.ToString());

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
            if (canvas.Elements[canvas.Elements.IndexOf(emitterReference)].IsPressed)
            {
                canvas.Elements[canvas.Elements.IndexOf(emitterReference)].IsPressed = false;
                canvas.InvalidateSurface();
            }

            // If out of scope, return
            if (_currentElement == null) return;

            switch (_currentElement.Tag)
            {
                case (int)Elements.ElementRoles.Role.Communication:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        OutputDebug("Completed icon tap");

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

                        OutputDebug("Completed icon held > 3s");

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
                        OutputDebug("Icon completed, has moved");

                        var folderOfInterest = GetFoldersOfInterest();

                        App.UserInputInstance.InsertIntoFolder(_currentElement: _currentElement, folderOfInterest: folderOfInterest);
                    }

                    e.Handled = true;

                    return;

                case (int)Elements.ElementRoles.Role.Folder:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        OutputDebug("Completed folder tap");

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

                        OutputDebug("Completed folder hold");

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

                        OutputDebug("Hit a folder, in user mode: " + _currentElement.Text);

                        // This is where the current item is the folder in question
                        var itemsMatching = GetItemsMatching();

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

                        OutputDebug("Held a folder, in user mode: " + _currentElement.Text);

                        // This is where the current item is the folder in question
                        var itemsMatching = GetItemsMatching();

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

                        OutputDebug("Seconds held: " + (System.DateTime.Now - emitterPressTime).TotalSeconds.ToString());
                        OutputDebug("Bottom Oriented: " + canvas.Controller.InFramedModeBottom);

                        // Enter into Edit Mode
                        if ((System.DateTime.Now - emitterPressTime).Seconds >= thresholdAdmin && !canvas.Controller.InEditMode)
                        {
                            canvas.Controller.UpdateSettings(isEditing: !canvas.Controller.InEditMode,
                                                             isInFrame: canvas.Controller.InFramedMode,
                                                             isFrameBottom: canvas.Controller.InFramedModeBottom,
                                                             isAutoDeselecting: canvas.Controller.RequireDeselect,
                                                             isInIconModeAuto: canvas.Controller.IconModeAuto);

                            canvas.Controller.BackgroundColor = canvas.Controller.InEditMode ? SKColors.DarkOrange : SKColors.DimGray;

                            resource = App.BoardSettings.InEditMode ? "FastTalkerSkiaSharp.Images.Settings.png" : "FastTalkerSkiaSharp.Images.Speaker.png";

                            canvas.Elements.Remove(emitterReference);

                            if (canvas.Controller.InFramedModeBottom && canvas.Controller.InFramedMode)
                            {
                                canvas.Elements.Remove(emitterReference);
                                emitterReference = App.ImageBuilderInstance.BuildStaticElementBottom(resource: resource,
                                                                   xPercent: 2f,
                                                                   yPercent: 1.5f,
                                                                   tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));
                                canvas.Elements.Add(emitterReference);
                            }
                            else
                            {
                                canvas.Elements.Remove(emitterReference);
                                emitterReference = App.ImageBuilderInstance.BuildStaticElement(resource: resource,
                                                                   xPercent: 2f,
                                                                   yPercent: 1.5f,
                                                                   tag: Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Emitter));
                                canvas.Elements.Add(emitterReference);
                            }

                            SendReferenceToBack(emitterReference);

                            ClearIconsInPlay();

                        }
                        // TODO: Fix exit program (changed since emitter fx altered)
                        else if ((System.DateTime.Now - emitterPressTime).Seconds >= thresholdReset && canvas.Controller.InEditMode)
                        {
                            // TODO: Confirm message?

                            OutputDebug("In reset hook, returning to home");

                            Xamarin.Forms.Application.Current.MainPage = new TitlePage();
                        }
                        // Speak output
                        else
                        {
                            if (canvas.Controller.InFramedMode)
                            {
                                var mIntersectingElements = GetSentenceContent();

                                if (mIntersectingElements != null && mIntersectingElements.Count() > 0)
                                {
                                    var output = System.String.Join(" ", mIntersectingElements);

                                    OutputDebug("Verbal output (Frame): " + output);

                                    commInterface.SpeakText(text: output);
                                }
                            }
                            else if (!canvas.Controller.IconModeAuto)
                            {
                                var selectedElements = GetMainIconInPlay();

                                if (selectedElements != null)
                                {
                                    OutputDebug("Verbal output (Icon): " + selectedElements);

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

        #endregion

        #region Helper Methods

        /// <summary>
        /// Simpler output in code
        /// </summary>
        /// <param name="msg">Message.</param>
        void OutputDebug(string msg)
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, msg);
        }

        /// <summary>
        /// Pull from controller into static container
        /// </summary>
        void CopySettingsFromController()
        {
            App.BoardSettings.InEditMode = canvas.Controller.InEditMode;
            App.BoardSettings.InFramedMode = canvas.Controller.InFramedMode;
            App.BoardSettings.RequireDeselect = canvas.Controller.RequireDeselect;
            App.BoardSettings.InIconModeAuto = canvas.Controller.IconModeAuto;
            App.BoardSettings.IsBottomOriented = canvas.Controller.InFramedModeBottom;            
        }

        /// <summary>
        /// Checks the permissions.
        /// </summary>
        async System.Threading.Tasks.Task CheckPermissions()
        {
            var cameraStatus = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);
            var storageStatus = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

            if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted || 
                storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var results = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(new[]
                {
                    Plugin.Permissions.Abstractions.Permission.Camera,
                    Plugin.Permissions.Abstractions.Permission.Storage
                });

                cameraStatus = results[Plugin.Permissions.Abstractions.Permission.Camera];
                storageStatus = results[Plugin.Permissions.Abstractions.Permission.Storage];
            }

            // 
            if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted || 
                storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Permissions Denied", "Unable to take photos.");
            }
        }

        /// <summary>
        /// Restores the icon.
        /// </summary>
        /// <param name="obj">Object.</param>
        void RestoreIcon(Helpers.ArgsSelectedIcon obj)
        {
            OutputDebug("RestoreIcon(ArgsSelectedIcon obj) Name: " + obj.Name + " ImageSourceResource: " + obj.ImageSource);

            bool check = canvas.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).Any();

            if (check)
            {
                SkiaSharp.Elements.Element item = canvas?.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).First();

                OutputDebug("Pass check? " + check + " Text: " + item.Text);

                item.IsInsertableIntoFolder = false;
                item.IsStoredInAFolder = false;
                item.StoredFolderTag = "";
                item.Location = Constants.DeviceLayout.GetCenterPointWithJitter(deviceSize: canvas.CanvasSize, iconReference: item.Size);

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
        /// Method for sending a referenced element to the back
        /// </summary>
        /// <param name="item">Item.</param>
        void SendReferenceToBack(SkiaSharp.Elements.Element item)
        {
            canvas.Elements.SendToBack(canvas.Elements.ElementAt(canvas.Elements.IndexOf(item)));
        }

        /// <summary>
        /// Get icons that are useful from storage
        /// </summary>
        /// <returns>The icons user interactable.</returns>
        System.Collections.Generic.List<SkiaSharp.Elements.Element> GetIconsUserInteractable()
        {
            return canvas.Elements?.Where(elem => elem.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication) ||
                                          elem.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder)).ToList();
        }

        System.Collections.Generic.IEnumerable<SkiaSharp.Elements.Element> GetFoldersOfInterest()
        {
            return canvas.Elements.Where(elem => elem.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder) && !elem.IsStoredInAFolder)
                                  .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds));            
        }

        System.Collections.Generic.List<SkiaSharp.Elements.Element> GetItemsMatching()
        {
            return canvas.Controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == _currentElement.Text).ToList();
        }

        System.Collections.Generic.IEnumerable<string> GetSentenceContent()
        {
            return canvas?.Elements.Where(elem => elem.IsSpeakable && elem.Tag != Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder))
                          .OrderBy(elem => elem.Left)
                          .Select(elem => elem.Text);
        }

        string GetMainIconInPlay()
        {
            return canvas?.Elements
                          .Where(elem => elem.IsMainIconInPlay && elem.Tag != Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder))
                          .Select(elem => elem.Text)
                          .FirstOrDefault();
        }

        #endregion

    }
}