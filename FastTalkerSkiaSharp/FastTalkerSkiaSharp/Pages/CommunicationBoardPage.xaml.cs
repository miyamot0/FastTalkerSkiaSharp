using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Interfaces;
using Rg.Plugins.Popup.Extensions;
using SkiaSharp;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class CommunicationBoardPage : ContentPage
    {
        private PanGestureRecognizer _panGestureRecognizer;
        private SkiaSharp.Elements.Element _currentElement;
        private SKPoint? _startLocation;

        private SkiaSharp.Elements.Element emitterReference,
                                           deleteReference,
                                           stripReference;

        private bool holdingEmitter = false;
        private DateTime emitterPressTime;

        private bool inInitialLoading = true;

        public CommunicationBoardPage()
        {
            InitializeComponent();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Padding = new Thickness(0, 20, 0, 0);

                    break;
            }

            App.ImageBuilderInstance = new ImageBuilder(canvas);

            _panGestureRecognizer = new PanGestureRecognizer();
            _panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;

            canvas.GestureRecognizers.Add(_panGestureRecognizer);

            canvas.Controller.OnElementsChanged += SaveCurrentBoard;
            canvas.Controller.OnSettingsChanged += SaveCurrentSettings;

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
            System.Diagnostics.Debug.WriteLine("Saving (event-based)");

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
                        Base64 = (element.LocalImage) ? "" :
                                                        element.ImageInformation,
                        ResourceLocation = (element.LocalImage) ? element.ImageInformation :
                                                                  "",
                        IsStoredInFolder = element.IsStoredInAFolder,
                        FolderContainingIcon = element.StoredFolderTag,
                        Scale = element.CurrentScale,
                        TextScale = 1f,
                        HashCode = element.GetHashCode()
                    });
                }

                int saveResult = await App.Database.InsertOrUpdateAsync(toInsert);

                System.Diagnostics.Debug.WriteLine("Results: Saved " + saveResult + " icons to db");
            }
        }

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void SaveCurrentSettings(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Saving settings (event-based)");

            App.BoardSettings.InEditMode = canvas.Controller.InEditMode;
            App.BoardSettings.InFramedMode = canvas.Controller.InFramedMode;
            App.BoardSettings.RequireDeselect = canvas.Controller.RequireDeselect;

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
                    Debug.WriteLine("waiting...");
                }

                Debug.WriteLine("GetSettingsAsync");

                GetSettingsAsync();

                Debug.WriteLine("AddStaticContent");

                AddStaticContent();

                Debug.WriteLine("GetIconsAsync");

                GetIconsAsync();

                Debug.WriteLine("Loading..");

                inInitialLoading = false;
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
                Debug.WriteLine("icon: " + icons.Count);

                foreach (var icon in icons)
                {
                    Debug.WriteLine("Tag: " + icon.Tag +
                                    " Name: " + icon.Text +
                                    " Scale: " + icon.Scale +
                                    " Local: " + icon.Local +
                                    " Stored Bool: " + icon.IsStoredInFolder +
                                    " FolderTag: " + icon.FolderContainingIcon);

                    if (icon.Local && icon.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication)
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationIconLocal(icon));
                    }
                    else if (icon.Local && icon.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                    {
                        canvas.Elements.Add(App.ImageBuilderInstance.BuildCommunicationFolderLocal(icon));
                    }
                }
            }
            else if (icons != null && icons.Count == 0)
            {
                Debug.WriteLine("No icons");
            }
            else
            {
                Debug.WriteLine("Null");
            }
        }

        /// <summary>
        /// Gets the settings async.
        /// </summary>
        private async void GetSettingsAsync()
        {
            App.BoardSettings = await App.Database.GetSettingsAsync();

            //App.BoardSettings.InEditMode = false;
            //canvas.Controller.BackgroundColor = SKColors.DimGray;

            App.BoardSettings.InEditMode = true;
            canvas.Controller.BackgroundColor = SKColors.DarkOrange;

            canvas.Controller.UpdateSettings(App.BoardSettings.InEditMode,
                                             App.BoardSettings.InFramedMode,
                                             App.BoardSettings.RequireDeselect,
                                             overridePrompt: true);
        }

        #region Panning Logic

        private bool IsInterfaceIconPanning()
        {
            return _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Control || _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Emitter;
        }

        private bool IsCommunicationIconPanning()
        {
            return _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication;
        }

        private bool IsFolderIconPanningEditMode()
        {
            return _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder && canvas.Controller.InEditMode;
        }

        private bool IsCommunicationIconPanningCompletedDeleteable(PanUpdatedEventArgs e)
        {
            return e.StatusType == GestureStatus.Completed && _currentElement != null &&
                                                                canvas.Controller.InEditMode &&
                                                                _currentElement.IsDeletable;
        }

        private bool IsCommunicationIconPanningCompletedInsertable(PanUpdatedEventArgs e)
        {
            return e.StatusType == GestureStatus.Completed && _currentElement != null &&
                                                                _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication &&
                                                                _currentElement.IsInsertableIntoFolder;
        }

        #endregion

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
        /// Pans the gesture recognizer pan updated.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            bool outputVerbose = true;

            System.Diagnostics.Debug.WriteLineIf(outputVerbose, e.StatusType.ToString());

            if (e.StatusType == GestureStatus.Running)
            {
                if (_currentElement != null)
                {
                    if (IsInterfaceIconPanning())
                    {
                        return;
                    }
                    else if (IsCommunicationIconPanning())
                    {
                        _currentElement.Location = new SKPoint(_startLocation.Value.X + (float)e.TotalX * App.DisplayScaleFactor,
                                                               _startLocation.Value.Y + (float)e.TotalY * App.DisplayScaleFactor);

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

                        _currentElement.IsDeletable = _currentElement.Bounds.IntersectsWith(deleteReference.Bounds);

                        if (_currentElement.IsDeletable && canvas.Controller.InEditMode)
                        {
                            //System.Diagnostics.Debug.WriteLine("can delete");
                        }

                        _currentElement.IsInsertableIntoFolder = canvas.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                                          .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds))
                                          .Any();
                    }
                    else if (IsFolderIconPanningEditMode())
                    {
                        _currentElement.Location = new SKPoint(_startLocation.Value.X + (float)e.TotalX * App.DisplayScaleFactor,
                                                               _startLocation.Value.Y + (float)e.TotalY * App.DisplayScaleFactor);

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

                        _currentElement.IsDeletable = _currentElement.Bounds.IntersectsWith(deleteReference.Bounds);

                        if (_currentElement.IsDeletable && canvas.Controller.InEditMode)
                        {
                            //System.Diagnostics.Debug.WriteLine("can delete folder");
                        }
                    }
                }
            }
            else if (IsCommunicationIconPanningCompletedDeleteable(e))
            {
                App.UserInputInstance.ConfirmRemoveIcon(canvas, _currentElement, deleteReference);
            }
            else if (IsCommunicationIconPanningCompletedInsertable(e))
            {
                var folderOfInterest = canvas.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder && !elem.IsStoredInAFolder)
                                                      .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds)).First();

                if (folderOfInterest != null)
                {
                    Debug.WriteLine("In Completed: Passed Insertable into folder. Tag: " + _currentElement.Tag +
                                    " Text: " + _currentElement.Text +
                                    " Folder name: " + folderOfInterest.Text);

                    _currentElement.IsStoredInAFolder = true;

                    _currentElement.StoredFolderTag = folderOfInterest.Text;

                    canvas.Elements.SendToBack(_currentElement);

                    canvas.Controller.PromptResave();

                    // Animation

                }
            }
        }

        #region Logic Canvas Press Handling

        /// <summary>
        /// Ises the canvas pressed.
        /// </summary>
        /// <returns><c>true</c>, if canvas pressed was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsCanvasPressed(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed;
        }

        /// <summary>
        /// Ises the sentence frame pressed.
        /// </summary>
        /// <returns><c>true</c>, if sentence frame pressed was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsSentenceFramePressed(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return _currentElement != null && _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.SentenceFrame;
        }

        /// <summary>
        /// Ises the speech emitter pressed.
        /// </summary>
        /// <returns><c>true</c>, if speech emitter pressed was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsSpeechEmitterPressed(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return _currentElement != null && _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Emitter;
        }

        /// <summary>
        /// Ises the speech emitter released.
        /// </summary>
        /// <returns><c>true</c>, if speech emitter released was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsSpeechEmitterReleased(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Released && holdingEmitter;
        }

        /// <summary>
        /// Ises the settings item pressed.
        /// </summary>
        /// <returns><c>true</c>, if settings item pressed was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsSettingsItemPressed(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return _currentElement != null &&
                canvas.Controller.InEditMode &&
                _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Settings;
        }

        /// <summary>
        /// Ises the delete pressed.
        /// </summary>
        /// <returns><c>true</c>, if delete pressed was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsDeletePressed(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return _currentElement != null && _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Delete;
        }

        /// <summary>
        /// Ises the icon tapped in edit mode.
        /// </summary>
        /// <returns><c>true</c>, if icon tapped in edit mode was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsIconTappedInEditMode(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Released &&
                     _currentElement != null &&
                     _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication &&
                    canvas.Controller.InEditMode;
        }

        /// <summary>
        /// Ises the folder pressed user mode.
        /// </summary>
        /// <returns><c>true</c>, if folder pressed user mode was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsFolderPressedUserMode(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Released &&
                    _currentElement != null &&
                    _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder &&
                    !canvas.Controller.InEditMode;
        }

        /// <summary>
        /// Ises the folder pressed edit mode.
        /// </summary>
        /// <returns><c>true</c>, if folder pressed edit mode was ised, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool IsFolderPressedEditMode(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            return e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Released &&
                    _currentElement != null &&
                    _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder &&
                    canvas.Controller.InEditMode;
        }

        #endregion

        /// <summary>
        /// Canvases the touch.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void Canvas_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            bool outputVerbose = true;

            Debug.WriteLine("e.ActionType = " + e.ActionType.ToString());
            Debug.WriteLineIf(outputVerbose, "e.InContact = " + e.InContact.ToString());

            //e.Handled = true;

            Debug.WriteLineIf(outputVerbose, "IsCanvasPressed: " + IsCanvasPressed(e));
            Debug.WriteLineIf(outputVerbose, "IsSentenceFramePressed: " + IsSentenceFramePressed(e));
            Debug.WriteLineIf(outputVerbose, "IsSpeechEmitterPressed: " + IsSpeechEmitterPressed(e));
            Debug.WriteLineIf(outputVerbose, "IsSettingsItemPressed: " + IsSettingsItemPressed(e));

            if (IsCanvasPressed(e))
            {
                _currentElement = canvas.GetElementAtPoint(e.Location);

                if (_currentElement != null)
                {
                    _startLocation = _currentElement.Location;

                    if (IsSentenceFramePressed(e))
                    {
                        Debug.WriteLineIf(outputVerbose, "Hit sentence frame");

                        return;
                    }
                    else if (IsSpeechEmitterPressed(e))
                    {
                        holdingEmitter = true;
                        emitterPressTime = DateTime.Now;

                        e.Handled = true;

                    }
                    else if (IsSettingsItemPressed(e))
                    {
                        App.UserInputInstance.QueryUserMainSettingsAsync(canvas);

                        e.Handled = true;
                    }
                    else if (IsDeletePressed(e))
                    {
                        e.Handled = true;

                        return;
                    }
                    else if (_currentElement != null &&
                             _currentElement.Tag == (int) SkiaSharp.Elements.CanvasView.Role.Communication &&
                             canvas.Controller.InEditMode)
                    {
                        ClearIconsInPlay();

                        canvas.Elements.BringToFront(_currentElement);

                        if (!canvas.Controller.InFramedMode)
                        {
                            _currentElement.IsMainIconInPlay = true;
                        }

                        e.Handled = true;
                    }
                    else if (_currentElement != null &&
                             _currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                    {
                        ClearIconsInPlay();

                        canvas.Elements.BringToFront(_currentElement);

                        e.Handled = true;
                    }
                    else
                    {
                        ClearIconsInPlay();

                        canvas.Elements.BringToFront(_currentElement);

                        if (!canvas.Controller.InFramedMode)
                        {
                            _currentElement.IsMainIconInPlay = true;
                        }
                    }
                }
            }
            else if (IsIconTappedInEditMode(e))
            {
                string userFeedback = await App.UserInputInstance.IconEditOptionsAsync();

                var item = App.ImageBuilderInstance.AmendIconImage(_currentElement, userFeedback);
                int index = canvas.Elements.IndexOf(_currentElement);

                if (item == null && index != -1)
                {
                    Debug.WriteLine("was null or unrefernced");
                }
                else
                {
                    canvas.Elements[index] = item;

                    canvas.InvalidateSurface();
                }

                //e.Handled = false;
            }
            else if (IsSpeechEmitterReleased(e))
            {
                holdingEmitter = false;

                //Debug.WriteLine("Seconds held: " + (DateTime.Now - emitterPressTime).TotalSeconds.ToString());

                if ((DateTime.Now - emitterPressTime).Seconds >= 3 && !canvas.Controller.InEditMode)
                {

                    canvas.Controller.UpdateSettings(!canvas.Controller.InEditMode,
                                                     canvas.Controller.InFramedMode,
                                                     canvas.Controller.RequireDeselect);

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

                            DependencyService.Get<InterfaceSpeechOutput>().SpeakText(output);
                        }
                    }
                    else
                    {
                        var selectedElements = canvas?.Elements
                                                      .Where(elem => elem.IsMainIconInPlay && elem.Tag != (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                                                      .Select(elem => elem.Text)
                                                      .FirstOrDefault();

                        if (selectedElements != null)
                        {
                            DependencyService.Get<InterfaceSpeechOutput>().SpeakText(selectedElements);
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
            else if (IsFolderPressedUserMode(e))
            {
                Debug.WriteLine("Hit a folder, in user mode: " + _currentElement.Text);

                // This is where the current item is the folder in question
                var itemsMatching = canvas.Controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == _currentElement.Text).ToList();

                // Leave if empty
                if (itemsMatching == null)
                {
                    e.Handled = true;

                    return;
                }

                var page = new StoredIconPopup(_currentElement.Text, itemsMatching);
                page.IconSelected += RestoreIcon;

                await App.Current.MainPage.Navigation.PushPopupAsync(page);

                e.Handled = true;
            }
            else if (IsFolderPressedEditMode(e))
            {
                Debug.WriteLine("Hit a folder, in edit mode");

                e.Handled = true;
            }

            //e.Handled = false;
        }

        /// <summary>
        /// Restores the icon.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void RestoreIcon(ArgsSelectedIcon obj)
        {
            Debug.WriteLine("RestoreIcon(ArgsSelectedIcon obj)");
            Debug.WriteLine("Name: " + obj.Name + " ImageSourceResource: " + obj.ImageSource);

            var check = canvas.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).Any();

            if (check)
            {
                Debug.WriteLine("Pass check? " + check);

                var item = canvas?.Elements.Where(elem => elem.IsStoredInAFolder && elem.Text == obj.Name).First();

                Debug.WriteLine("Text: " + item.Text);

                item.IsStoredInAFolder = false;
                item.StoredFolderTag = "";
                //item.Location = DeviceLayout.GetCenterPointWithJitter(canvas.CanvasSize);
                item.Location = DeviceLayout.GetCenterPoint(canvas.CanvasSize);

                canvas.Elements.Remove(item);
                canvas.Elements.Add(item);

                //canvas.Elements.BringToFront(item);

                canvas.InvalidateSurface();
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
        }

        /// <summary>
        /// Adds the content of the static.
        /// </summary>
        private void AddStaticContent()
        {
            Debug.WriteLine("Adding Static Content");
            Debug.WriteLine("Width: " + canvas.CanvasSize.Width);
            Debug.WriteLine("Height: " + canvas.CanvasSize.Height);

            Debug.WriteLine("Layout Width: " + hackLayout.Width);
            Debug.WriteLine("Layout Height: " + hackLayout.Height);

            // Sentence Strip
            stripReference = App.ImageBuilderInstance.BuildSentenceStrip();

            Debug.WriteLine("stripRef: " + stripReference.X);
            Debug.WriteLine("stripRef: " + stripReference.Y);
            Debug.WriteLine("stripRef: " + stripReference.Width);

            canvas.Elements.Add(stripReference);

            // Speech Emitter
            emitterReference = App.ImageBuilderInstance.BuildStaticElement("FastTalkerSkiaSharp.Images.Speaker.png",
                                                                           2f,
                                                                           1.5f,
                                                                           (int)SkiaSharp.Elements.CanvasView.Role.Emitter);
            canvas.Elements.Add(emitterReference);

            // Settings
            SkiaSharp.Elements.Element settingsElement = App.ImageBuilderInstance.BuildNamedIcon("FastTalkerSkiaSharp.Images.Settings.png",
                                                                                                 "Settings",
                                                                                                 canvas.CanvasSize.Width - Constants.DeviceLayout.Bezel,
                                                                                                 canvas.CanvasSize.Height - Constants.DeviceLayout.Bezel,
                                                                                                 (int)SkiaSharp.Elements.CanvasView.Role.Settings,
                                                                                                 alignRight: true,
                                                                                                 alignBottom: true,
                                                                                                 opaqueBackground: true);

            canvas.Elements.Add(settingsElement);

            // Delete zone
            deleteReference = App.ImageBuilderInstance.BuildNamedIcon("FastTalkerSkiaSharp.Images.Trash.png",
                                                                      "Delete",
                                                                      Constants.DeviceLayout.Bezel,
                                                                      canvas.CanvasSize.Height - Constants.DeviceLayout.Bezel,
                                                                      (int)SkiaSharp.Elements.CanvasView.Role.Delete,
                                                                      alignBottom: true,
                                                                      opaqueBackground: true);

            canvas.Elements.Add(deleteReference);
        }
    }
}