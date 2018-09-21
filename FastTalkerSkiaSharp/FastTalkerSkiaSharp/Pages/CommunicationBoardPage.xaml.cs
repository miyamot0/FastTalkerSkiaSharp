/* 
    The MIT License

    Copyright February 8, 2016 Shawn Gilroy. http://www.smallnstats.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using SkiaSharp;
using System.Linq;
using Rg.Plugins.Popup.Extensions;
using FastTalkerSkiaSharp.Controls;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class CommunicationBoardPage : Xamarin.Forms.ContentPage
    {
        Icon _currentIcon;
        SKPoint? _startLocation;

        Icon stripReference;
        IconImage emitterReference;

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

            canvas.Controller.OnIconsChanged += SaveCurrentBoard;
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
                    canvas.Icons.Remove(emitterReference);
                    emitterReference = App.ImageBuilderInstance.BuildStaticIconBottom(resource: resource,
                                                       xPercent: 2f,
                                                       yPercent: 1.5f,
                                                       tag: IconRoles.GetRoleInt(IconRoles.Role.Emitter));
                    canvas.Icons.Add(emitterReference);

                    canvas.Icons.Remove(stripReference);
                    stripReference = App.ImageBuilderInstance.BuildSentenceStripBottom();
                    canvas.Icons.Add(stripReference);
                }
                else
                {
                    canvas.Icons.Remove(emitterReference);
                    emitterReference = App.ImageBuilderInstance.BuildStaticIcon(resource: resource,
                                                       xPercent: 2f,
                                                       yPercent: 1.5f,
                                                       tag: IconRoles.GetRoleInt(IconRoles.Role.Emitter));
                    canvas.Icons.Add(emitterReference);

                    canvas.Icons.Remove(stripReference);
                    stripReference = App.ImageBuilderInstance.BuildSentenceStrip();
                    canvas.Icons.Add(stripReference);
                }

                SendReferenceToBack(stripReference);
                SendReferenceToBack(emitterReference);

                canvas.InvalidateSurface();
            }
            else
            {
                resource = App.BoardSettings.InEditMode ? "FastTalkerSkiaSharp.Images.Settings.png" : "FastTalkerSkiaSharp.Images.Speaker.png";

                canvas.Icons.Remove(emitterReference);
                emitterReference = App.ImageBuilderInstance.BuildStaticIcon(resource: resource,
                                                   xPercent: 2f,
                                                   yPercent: 1.5f,
                                                   tag: IconRoles.GetRoleInt(IconRoles.Role.Emitter));
                canvas.Icons.Add(emitterReference);

                SendReferenceToBack(emitterReference);
            }


            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                Xamarin.Forms.DependencyService.Get<Interfaces.InterfaceAdministration>().RequestAdmin(!canvas.Controller.InEditMode);
            }

            await App.Database.InsertOrUpdateAsync(App.BoardSettings);

            if (canvas.Icons[canvas.Icons.IndexOf(emitterReference)].IsPressed)
            {
                canvas.Icons[canvas.Icons.IndexOf(emitterReference)].IsPressed = false;
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

            if (inInitialLoading)
            {
                OutputDebug("GetSettingsAsync");
                await GetSettingsAsync();

                OutputDebug("AddStaticContent");
                AddStaticContent();

                OutputDebug("GetIconsAsync");
                await GetIconsAsync();

                OutputDebug("Requesting permissions..");
                await CheckPermissions();

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

                    if (icon.Local && icon.Tag == IconRoles.GetRoleInt(IconRoles.Role.Communication))
                    {
                        canvas.Icons.Add(App.ImageBuilderInstance.BuildCommunicationIconLocal(icon));
                    }
                    else if (!icon.Local && icon.Tag == IconRoles.GetRoleInt(IconRoles.Role.Communication))
                    {
                        canvas.Icons.Add(App.ImageBuilderInstance.BuildCommunicationIconDynamic(icon));
                    }
                    else if (icon.Local && icon.Tag == IconRoles.GetRoleInt(IconRoles.Role.Folder))
                    {
                        canvas.Icons.Add(App.ImageBuilderInstance.BuildCommunicationFolderLocal(icon));
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

        #region Icon Building

        /// <summary>
        /// Adds the content of the static.
        /// </summary>
        void AddStaticContent()
        {
            OutputDebug("Adding Static Content" +
                        " Width: " + canvas.CanvasSize.Width + " Height: " + canvas.CanvasSize.Height +
                        " Layout Width: " + hackLayout.Width +
                        " Layout Height: " + hackLayout.Height);

            resource = App.BoardSettings.InEditMode ? "FastTalkerSkiaSharp.Images.Settings.png" : "FastTalkerSkiaSharp.Images.Speaker.png";

            if (canvas.Controller.InFramedModeBottom)
            {
                stripReference = App.ImageBuilderInstance.BuildSentenceStripBottom();

                emitterReference = App.ImageBuilderInstance.BuildStaticIconBottom(resource: resource,
                                               xPercent: 2f,
                                               yPercent: 1.5f,
                                               tag: IconRoles.GetRoleInt(IconRoles.Role.Emitter));
            }
            else
            {
                stripReference = App.ImageBuilderInstance.BuildSentenceStrip();

                emitterReference = App.ImageBuilderInstance.BuildStaticIcon(resource: resource,
                                                               xPercent: 2f,
                                                               yPercent: 1.5f,
                                                               tag: IconRoles.GetRoleInt(IconRoles.Role.Emitter));


            }

            canvas.Icons.Add(stripReference);
            canvas.Icons.SendToBack(stripReference);

            canvas.Icons.Add(emitterReference);
            canvas.Icons.SendToBack(emitterReference);
        }

        #endregion

        #region Touch Interaction

        /// <summary>
        /// Keep icon within bounds of canvas
        /// </summary>
        void ClampCurrentIconToCanvasBounds()
        {
            if (_currentIcon.Top <= 0)
            {
                _currentIcon.Y -= _currentIcon.Top;
            }

            if (_currentIcon.Bottom >= canvas.CanvasSize.Height)
            {
                _currentIcon.Y -= (_currentIcon.Bottom - canvas.CanvasSize.Height);
            }

            if (_currentIcon.Left <= 0)
            {
                _currentIcon.X -= _currentIcon.Left;
            }

            if (_currentIcon.Right >= canvas.CanvasSize.Width)
            {
                _currentIcon.X -= (_currentIcon.Right - canvas.CanvasSize.Width);
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

            OutputDebug(" e.ActionType = " + e.ActionType.ToString() + 
                        " e.InContact = " + e.InContact.ToString() +
                        " Icons: " + canvas.Icons.Count);

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
                    _currentIcon = null;
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
            _currentIcon = canvas.GetIconAtPoint(e.Location);

            // Confirmation of movement
            hasMoved = false;

            // Fail out if null
            if (_currentIcon == null) return;

            itemPressTime = System.DateTime.Now;

            // Get origin of element
            _startLocation = _currentIcon.Location;

            switch (_currentIcon.Tag)
            {
                case (int)IconRoles.Role.SentenceFrame:
                    OutputDebug("Hit sentence frame");

                    return;

                case (int)IconRoles.Role.Emitter:

                    // Serves as Settings button in edit mode
                    if (canvas.Controller.InEditMode)
                    {
                        canvas.Icons[canvas.Icons.IndexOf(emitterReference)].IsPressed = true;
                        canvas.InvalidateSurface();

                        OutputDebug("Hit settings (speech emitter)");
                        App.UserInputInstance.QueryUserMainSettingsAsync();

                    }
                    // Serves as speech emitter as normal
                    else
                    {
                        canvas.Icons[canvas.Icons.IndexOf(emitterReference)].IsPressed = true;

                        canvas.InvalidateSurface();

                        OutputDebug("Hit speech emitter");
                        holdingEmitter = true;
                        emitterPressTime = System.DateTime.Now;
                    }

                    return;

                case (int)IconRoles.Role.Folder:
                    OutputDebug("Hit Folder");
                    ClearIconsInPlay();
                    canvas.Icons.BringToFront(_currentIcon);

                    return;

                default:
                    OutputDebug("In Default Hit");
                    ClearIconsInPlay();
                    canvas.Icons.BringToFront(_currentIcon);

                    if (!canvas.Controller.InFramedMode && !canvas.Controller.IconModeAuto)
                    {
                        _currentIcon.IsMainIconInPlay = true;
                    }
                    else if (!canvas.Controller.InFramedMode &&
                             _currentIcon != null &&
                             _currentIcon.Tag == IconRoles.GetRoleInt(IconRoles.Role.Communication) &&
                             !canvas.Controller.InEditMode &&
                             canvas.Controller.IconModeAuto)
                    {
                        commInterface.SpeakText(_currentIcon.Text);

                        e.Handled = true;
                    }
                    /* Pinned icons in sentence mode */
                    else if (canvas.Controller.InFramedMode &&
                             !canvas.Controller.InEditMode &&
                             _currentIcon.IsPinnedToSpot &&
                             !stripReference.Bounds.IntersectsWith(_currentIcon.Bounds))
                    {
                        OutputDebug("Hit a pinned icon in sentence mode, outside of frame");

                        e.Handled = true;

                        commInterface.SpeakText(text: _currentIcon.Text);

                        _currentIcon = null;
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
            if (_currentIcon == null) return;

            OutputDebug("Moving Event: " + e.ToString());

            switch (_currentIcon.Tag)
            {
                case (int)IconRoles.Role.Control:

                    return;

                case (int)IconRoles.Role.Emitter:

                    return;

                case (int)IconRoles.Role.Communication:
                    hasMoved = true;

                    // If pinned, prevent move
                    if (_currentIcon.IsPinnedToSpot && !canvas.Controller.InEditMode) return;

                    _currentIcon.Location = new SKPoint(e.Location.X - _currentIcon.Bounds.Width / 2f,
                                                           e.Location.Y - _currentIcon.Bounds.Height / 2f);

                    ClampCurrentIconToCanvasBounds();

                    if (canvas.Controller.InFramedMode)
                    {
                        _currentIcon.IsSpeakable = _currentIcon.Bounds.IntersectsWith(stripReference.Bounds);
                    }
                    else
                    {
                        for (int i = 0; i < canvas.Icons.Count; i++)
                        {
                            canvas.Icons[i].IsMainIconInPlay = false;
                        }

                        _currentIcon.IsMainIconInPlay = true;
                    }

                    _currentIcon.IsInsertableIntoFolder = canvas.Icons.Where(elem => elem.Tag == IconRoles.GetRoleInt(IconRoles.Role.Folder))
                                        .Where(folder => folder.Bounds.IntersectsWith(_currentIcon.Bounds))
                                        .Any();

                    _startLocation = _currentIcon.Location;

                    return;

                case (int)IconRoles.Role.Folder:
                    hasMoved = true;

                    if (!canvas.Controller.InEditMode) return;

                    _currentIcon.Location = new SKPoint(e.Location.X - _currentIcon.Bounds.Width / 2f,
                                        e.Location.Y - _currentIcon.Bounds.Height / 2f);

                    ClampCurrentIconToCanvasBounds();

                    _startLocation = _currentIcon.Location;

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
            if (canvas.Icons[canvas.Icons.IndexOf(emitterReference)].IsPressed)
            {
                canvas.Icons[canvas.Icons.IndexOf(emitterReference)].IsPressed = false;
                canvas.InvalidateSurface();
            }

            // If out of scope, return
            if (_currentIcon == null) return;

            switch (_currentIcon.Tag)
            {
                case (int)IconRoles.Role.Communication:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        OutputDebug("Completed icon tap");

                        if (App.InstanceModificationPage == null)
                        {
                            App.InstanceModificationPage = new ModifyPage(_currentIcon, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentIcon);
                        }

                        await Navigation.PushPopupAsync(App.InstanceModificationPage);
                    }
                    else if (canvas.Controller.InEditMode &&
                             !_currentIcon.IsInsertableIntoFolder &&
                             System.DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        OutputDebug("Completed icon held > 3s");

                        if (App.InstanceModificationPage == null)
                        {
                            App.InstanceModificationPage = new ModifyPage(_currentIcon, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentIcon);
                        }

                        await Navigation.PushPopupAsync(App.InstanceModificationPage);
                    }
                    else if (hasMoved && _currentIcon.IsInsertableIntoFolder)
                    {
                        OutputDebug("Icon completed, has moved");

                        var folderOfInterest = GetFoldersOfInterest();

                        App.UserInputInstance.InsertIntoFolder(_currentIcon: _currentIcon, folderOfInterest: folderOfInterest);
                    }

                    e.Handled = true;

                    return;

                case (int)IconRoles.Role.Folder:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        OutputDebug("Completed folder tap");

                        if (App.InstanceModificationPage == null)
                        {
                            App.InstanceModificationPage = new ModifyPage(_currentIcon, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentIcon);
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
                            App.InstanceModificationPage = new ModifyPage(_currentIcon, canvas.Controller);
                        }
                        else
                        {
                            App.InstanceModificationPage.UpdateCurrentIcon(_currentIcon);
                        }

                        await Navigation.PushPopupAsync(App.InstanceModificationPage);

                        e.Handled = true;
                    }

                    if (!canvas.Controller.InEditMode && !hasMoved)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        OutputDebug("Hit a folder, in user mode: " + _currentIcon.Text);

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
                                FolderWithIcons = _currentIcon.Text,
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
                            App.InstanceStoredIconsViewModel.FolderWithIcons = _currentIcon.Text;
                            App.InstanceStoredIconsViewModel.ItemsMatching = itemsMatching;
                        }

                        await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(App.InstanceStoredIconPage);

                        e.Handled = true;
                    }
                    else if (!canvas.Controller.InEditMode && System.DateTime.Now.Subtract(itemPressTime).Seconds > 3)
                    {
                        if (App.UserInputInstance.AreModalsOpen()) return;

                        OutputDebug("Held a folder, in user mode: " + _currentIcon.Text);

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
                                FolderWithIcons = _currentIcon.Text,
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
                            App.InstanceStoredIconsViewModel.FolderWithIcons = _currentIcon.Text;
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

                            if (canvas.Controller.InFramedModeBottom && canvas.Controller.InFramedMode)
                            {
                                var list = canvas.Icons.Where(elem => elem.Tag == IconRoles.GetRoleInt(IconRoles.Role.Emitter)).ToList();

                                foreach (var icon in list)
                                {
                                    canvas.Icons.Remove(icon);
                                }

                                emitterReference = App.ImageBuilderInstance.BuildStaticIconBottom(resource: resource,
                                                                   xPercent: 2f,
                                                                   yPercent: 1.5f,
                                                                   tag: IconRoles.GetRoleInt(IconRoles.Role.Emitter));
                                canvas.Icons.Add(emitterReference);
                            }
                            else
                            {
                                var list = canvas.Icons.Where(elem => elem.Tag == IconRoles.GetRoleInt(IconRoles.Role.Emitter)).ToList();

                                foreach (var icon in list)
                                {
                                    canvas.Icons.Remove(icon);
                                }

                                emitterReference = App.ImageBuilderInstance.BuildStaticIcon(resource: resource,
                                                                   xPercent: 2f,
                                                                   yPercent: 1.5f,
                                                                   tag: IconRoles.GetRoleInt(IconRoles.Role.Emitter));
                                canvas.Icons.Add(emitterReference);
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
                                var mIntersectingIcons = GetSentenceContent();

                                if (mIntersectingIcons != null && mIntersectingIcons.Count() > 0)
                                {
                                    var output = System.String.Join(" ", mIntersectingIcons);

                                    OutputDebug("Verbal output (Frame): " + output);

                                    commInterface.SpeakText(text: output);
                                }
                            }
                            else if (!canvas.Controller.IconModeAuto)
                            {
                                var selectedIcons = GetMainIconInPlay();

                                if (selectedIcons != null)
                                {
                                    OutputDebug("Verbal output (Icon): " + selectedIcons);

                                    commInterface.SpeakText(text: selectedIcons);
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

            bool check = canvas.Icons.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).Any();

            if (check)
            {
                Icon item = canvas?.Icons.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).First();

                OutputDebug("Pass check? " + check + " Text: " + item.Text);

                item.IsInsertableIntoFolder = false;
                item.IsStoredInAFolder = false;
                item.StoredFolderTag = "";
                item.Location = Constants.DeviceLayout.GetCenterPointWithJitter(deviceSize: canvas.CanvasSize, iconReference: item.Size);

                canvas.Icons.BringToFront(item);

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
                for (int i = 0; i < canvas.Icons.Count; i++)
                {
                    canvas.Icons[i].IsMainIconInPlay = false;
                }
            }
            else
            {
                var isGreenHighlighted = false;

                for (int i = 0; i < canvas.Icons.Count; i++)
                {
                    isGreenHighlighted = canvas.Icons[i].Bounds.IntersectsWith(stripReference.Bounds);

                    canvas.Icons[i].IsSpeakable = isGreenHighlighted;
                }
            }

            canvas.InvalidateSurface();
        }

        /// <summary>
        /// Method for sending a referenced element to the back
        /// </summary>
        /// <param name="item">Item.</param>
        void SendReferenceToBack(Icon item)
        {
            canvas.Icons.SendToBack(canvas.Icons.ElementAt(canvas.Icons.IndexOf(item)));
        }

        /// <summary>
        /// Get icons that are useful from storage
        /// </summary>
        /// <returns>The icons user interactable.</returns>
        System.Collections.Generic.List<Icon> GetIconsUserInteractable()
        {
            return canvas.Icons?.Where(elem => elem.Tag == IconRoles.GetRoleInt(IconRoles.Role.Communication) ||
                                          elem.Tag == IconRoles.GetRoleInt(IconRoles.Role.Folder)).ToList();
        }

        System.Collections.Generic.IEnumerable<Icon> GetFoldersOfInterest()
        {
            return canvas.Icons.Where(elem => elem.Tag == IconRoles.GetRoleInt(IconRoles.Role.Folder) && !elem.IsStoredInAFolder)
                                  .Where(folder => folder.Bounds.IntersectsWith(_currentIcon.Bounds));            
        }

        System.Collections.Generic.List<Icon> GetItemsMatching()
        {
            return canvas.Controller.Icons.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == _currentIcon.Text).ToList();
        }

        System.Collections.Generic.IEnumerable<string> GetSentenceContent()
        {
            return canvas?.Icons.Where(elem => elem.IsSpeakable && elem.Tag != IconRoles.GetRoleInt(IconRoles.Role.Folder))
                          .OrderBy(elem => elem.Left)
                          .Select(elem => elem.Text);
        }

        string GetMainIconInPlay()
        {
            return canvas?.Icons
                          .Where(elem => elem.IsMainIconInPlay && elem.Tag != IconRoles.GetRoleInt(IconRoles.Role.Folder))
                          .Select(elem => elem.Text)
                          .FirstOrDefault();
        }

        #endregion

    }
}