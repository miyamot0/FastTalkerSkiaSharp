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
        private SkiaSharp.Elements.Element _currentElement;
        private SKPoint? _startLocation;

        private SkiaSharp.Elements.Element emitterReference,
                                           deleteReference,
                                           stripReference;

        private bool holdingEmitter = false;
        private DateTime emitterPressTime;

        private bool inInitialLoading = true;
        private bool hasMoved = false;

        public CommunicationBoardPage()
        {
            InitializeComponent();

            App.ImageBuilderInstance = new ImageBuilder(canvas);

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

            Debug.WriteLine(App.OutputVerbose, "e.ActionType = " + e.ActionType.ToString() + "e.InContact = " + e.InContact.ToString());

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
                    if (canvas.Controller.InEditMode) App.UserInputInstance.QueryUserMainSettingsAsync(canvas);

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Delete:
                    Debug.WriteLineIf(outputVerbose, "Hit Delete");

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

                    if (!canvas.Controller.InFramedMode)
                    {
                        _currentElement.IsMainIconInPlay = true;
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

                    _currentElement.IsDeletable = _currentElement.Bounds.IntersectsWith(deleteReference.Bounds);

                    if (_currentElement.IsDeletable && canvas.Controller.InEditMode)
                    {
                        Debug.WriteLineIf(outputVerbose, "Can delete communication icon");
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

                    _currentElement.IsDeletable = _currentElement.Bounds.IntersectsWith(deleteReference.Bounds);

                    if (_currentElement.IsDeletable && canvas.Controller.InEditMode)
                    {
                        Debug.WriteLineIf(outputVerbose, "Can delete folder icon");
                    }

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

                        string userFeedback = await App.UserInputInstance.IconEditOptionsAsync();

                        Debug.WriteLineIf(outputVerbose, "User Feedback: " + userFeedback);

                        var item = App.ImageBuilderInstance.AmendIconImage(_currentElement, userFeedback);

                        int index = canvas.Elements.IndexOf(_currentElement);

                        if (item == null || index == -1)
                        {
                            Debug.WriteLineIf(outputVerbose, "was null or unrefernced");
                        }
                        else
                        {
                            canvas.Elements[index] = item;

                            canvas.InvalidateSurface();
                        }
                    }
                    else if (hasMoved && _currentElement.IsInsertableIntoFolder)
                    {
                        Debug.WriteLineIf(outputVerbose, "Icon completed, has moved");

                        var folderOfInterest = canvas.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder && !elem.IsStoredInAFolder)
                                      .Where(folder => folder.Bounds.IntersectsWith(_currentElement.Bounds));

                        if (folderOfInterest != null || folderOfInterest.Count() > 0)
                        {
                            Debug.WriteLineIf(outputVerbose, "In Completed: Insertable into folder: " + _currentElement.Tag);

                            _currentElement.IsStoredInAFolder = true;
                            _currentElement.StoredFolderTag = folderOfInterest.First().Text;

                            canvas.Elements.SendToBack(_currentElement);
                            canvas.Controller.PromptResave();

                            Debug.WriteLineIf(outputVerbose, "TODO: animation entry");
                        }
                    }
                    else if (hasMoved && _currentElement.IsDeletable)
                    {
                        App.UserInputInstance.ConfirmRemoveIcon(canvas, _currentElement, deleteReference);
                    }

                    e.Handled = true;

                    return;

                case (int)SkiaSharp.Elements.CanvasView.Role.Folder:
                    if (canvas.Controller.InEditMode && !hasMoved)
                    {
                        Debug.WriteLineIf(outputVerbose, "Hit a folder, in edit mode, ADD EDIT");

                        e.Handled = true;
                    }
                    if (canvas.Controller.InEditMode && _currentElement.IsDeletable)
                    {
                        Debug.WriteLineIf(outputVerbose, "Can delete a folder, TODO");

                        e.Handled = true;
                    }
                    else if (!canvas.Controller.InEditMode && !hasMoved)
                    {
                        Debug.WriteLineIf(outputVerbose, "Hit a folder, in user mode: " + _currentElement.Text);

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

                    return;

                default:
                    // Emitter was tapped/held
                    if (holdingEmitter)
                    {
                        holdingEmitter = false;

                        Debug.WriteLineIf(outputVerbose, "Seconds held: " + (DateTime.Now - emitterPressTime).TotalSeconds.ToString());

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

                                    Debug.WriteLineIf(outputVerbose, "Verbal output (Frame): " + output);

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
                                    Debug.WriteLineIf(outputVerbose, "Verbal output (Icon): " + selectedElements);

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
                    // Item is deleteable
                    else if (canvas.Controller.InEditMode && _currentElement.IsDeletable)
                    {
                        App.UserInputInstance.ConfirmRemoveIcon(canvas, _currentElement, deleteReference);
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