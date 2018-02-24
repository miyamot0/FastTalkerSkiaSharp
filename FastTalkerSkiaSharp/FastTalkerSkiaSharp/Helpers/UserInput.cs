﻿/*
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Interfaces;
using FastTalkerSkiaSharp.Pages;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SkiaSharp;
using SkiaSharp.Elements;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Helpers
{
    public class UserInput
    {
        CanvasView canvasRef = null;

        public UserInput() { }

        /// <summary>
        /// Queries the user main settings async.
        /// </summary>
        /// <param name="canvasView">Canvas view.</param>
        /// <param name="saveSettingsAsync">Save settings async.</param>
        public async void QueryUserMainSettingsAsync(CanvasView canvasView)
        {
            if (canvasRef == null)
            {
                canvasRef = canvasView;
            }

            var userResponse = await UserDialogs.Instance.ActionSheetAsync(LanguageSettings.SettingsTitle,
                                                                           LanguageSettings.SettingsClose,
                                                                           LanguageSettings.SettingsClose,
                                                                           null,
                                                                           LanguageSettings.SettingsMenu(canvasView.Controller));

            ResponseToQuery(canvasView, userResponse);
        }

        /// <summary>
        /// Responses to query.
        /// </summary>
        /// <param name="canvasView">Canvas view.</param>
        /// <param name="userResponse">User response.</param>
        /// <param name="saveSettingsAsync">Save settings async.</param>
        public async void ResponseToQuery(CanvasView canvasView, string userResponse)
        {
            switch (userResponse)
            {
                #region Close out back to edit mode - Done

                case LanguageSettings.SettingsClose:
                    // Return
                    return;

                #endregion

                case LanguageSettings.SettingsServerStart:
                    // TODO

                    return;

                case LanguageSettings.SettingsSave:
                    canvasView.Controller.PromptResave();

                    return;

                #region Auto De-selecting options

                case LanguageSettings.SettingsDeselect:
                    canvasView.Controller.UpdateSettings(canvasView.Controller.InEditMode,
                                                         canvasView.Controller.InFramedMode,
                                                         true);

                    return;

                case LanguageSettings.SettingsDeselectDisable:
                    canvasView.Controller.UpdateSettings(canvasView.Controller.InEditMode,
                                                         canvasView.Controller.InFramedMode,
                                                         false);

                    return;

                #endregion

                #region Frame mode options

                case LanguageSettings.SettingsMode:
                    canvasView.Controller.UpdateSettings(canvasView.Controller.InEditMode,
                                                         true,
                                                         canvasView.Controller.RequireDeselect);

                    for (int i = 0; i < canvasView.Elements.Count; i++)
                    {
                        canvasView.Elements[i].IsMainIconInPlay = false;
                    }

                    canvasView.InvalidateSurface();

                    return;

                case LanguageSettings.SettingsMode2:
                    canvasView.Controller.UpdateSettings(canvasView.Controller.InEditMode,
                                                         false,
                                                         canvasView.Controller.RequireDeselect);

                    for (int i = 0; i < canvasView.Elements.Count; i++)
                    {
                        canvasView.Elements[i].IsMainIconInPlay = false;
                    }

                    canvasView.InvalidateSurface();

                    return;

                #endregion

                case LanguageSettings.SettingsAddIcon:

                    var newCommunicationPage = new CommunicationIconPicker();
                    newCommunicationPage.IconConstructed += SaveCommunicationIcon;

                    await App.Current.MainPage.Navigation.PushAsync(newCommunicationPage);

                    return;

                case LanguageSettings.SettingsTakePhoto:
                    var base64 = await GetImageFromCameraCallAsync();

                    return;

                case LanguageSettings.SettingsAddFolder:
                    var newFolderPage = new FolderIconPicker();
                    newFolderPage.FolderConstructed += SaveFolder;

                    await App.Current.MainPage.Navigation.PushAsync(newFolderPage);

                    return;

                #region Resume User Behavior - Done

                case LanguageSettings.SettingsResume:
                    canvasView.Controller.UpdateSettings(false,
                                                         canvasView.Controller.InFramedMode,
                                                         canvasView.Controller.RequireDeselect);

                    canvasView.Controller.BackgroundColor = canvasView.Controller.InEditMode ?
                                                            SKColors.Orange :
                                                            SKColors.DimGray;

                    return;

                #endregion

                default:

                    return;
            }
        }

        /// <summary>
        /// Get options from user
        /// </summary>
        /// <returns></returns>
        public async Task<string> IconEditOptionsAsync()
        {
            var userResponse = await UserDialogs.Instance.ActionSheetAsync(LanguageSettings.EditTitle, LanguageSettings.EditClose,
                                                                           LanguageSettings.EditClose, null, LanguageSettings.EditMenu());


            return userResponse;
        }

        /// <summary>
        /// Saves the communication icon.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void SaveCommunicationIcon(ArgsSelectedIcon obj)
        {
            canvasRef.Elements.Add(App.ImageBuilderInstance.BuildCommunicationIconLocal(obj));

            canvasRef.Controller.PromptResave();
        }

        /// <summary>
        /// Saves the folder.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void SaveFolder(ArgsSelectedIcon obj)
        {
            canvasRef.Elements.Add(App.ImageBuilderInstance.BuildCommunicationFolderLocal(obj));

            canvasRef.Controller.PromptResave();
        }

        /// <summary>
        /// Confirms the remove icon, with some animation.
        /// </summary>
        /// <param name="canvasView">Canvas view.</param>
        /// <param name="currentElement">Current element.</param>
        public async void ConfirmRemoveIcon(SkiaSharp.Elements.Element currentElement, SkiaSharp.Elements.Element deleteButton)
        {
            var response = await UserDialogs.Instance.ConfirmAsync("Delete this icon?");

            if (response)
            {
                var startPoint = currentElement.Location;

                float xDiff = (deleteButton.Location.X + deleteButton.Bounds.Width / 2f) - (startPoint.X + currentElement.Bounds.Width / 2f);
                float yDiff = (deleteButton.Location.Y + deleteButton.Bounds.Height / 2f) - (startPoint.Y + currentElement.Bounds.Height / 2f);

                new Xamarin.Forms.Animation((value) =>
                {
                    canvasRef.SuspendLayout();
                    currentElement.Location = new SKPoint((startPoint.X) + (xDiff * (float)value),
                                                          (startPoint.Y) + (yDiff * (float)value));
                    canvasRef.ResumeLayout(true);
                }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationMoveMillis, finished: (v, c) =>
                {
                    new Xamarin.Forms.Animation((value) =>
                    {
                        canvasRef.SuspendLayout();

                        currentElement.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);

                        canvasRef.ResumeLayout(true);
                    }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationShrinkMillis, finished: (v2, c2) =>
                    {
                        canvasRef.Elements.Remove(currentElement);
                        canvasRef.Controller.PromptResave();
                    });
                });
            }
        }

        /// <summary>
        /// Delete folder and icons within
        /// </summary>
        /// <param name="currentElement"></param>
        /// <param name="deleteButton"></param>
        public async void ConfirmDeleteFolder(SkiaSharp.Elements.Element currentElement, SkiaSharp.Elements.Element deleteButton)
        {
            var response = await UserDialogs.Instance.ConfirmAsync("Delete this folder and the icons within?");

            if (response)
            {
                var startPoint = currentElement.Location;

                float xDiff = (deleteButton.Location.X + deleteButton.Bounds.Width / 2f) - (startPoint.X + currentElement.Bounds.Width / 2f);
                float yDiff = (deleteButton.Location.Y + deleteButton.Bounds.Height / 2f) - (startPoint.Y + currentElement.Bounds.Height / 2f);

                new Xamarin.Forms.Animation((value) =>
                {
                    canvasRef.SuspendLayout();
                    currentElement.Location = new SKPoint((startPoint.X) + (xDiff * (float)value),
                                                          (startPoint.Y) + (yDiff * (float)value));
                    canvasRef.ResumeLayout(true);
                }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationMoveMillis, finished: (v, c) =>
                {
                    new Xamarin.Forms.Animation((value) =>
                    {
                        canvasRef.SuspendLayout();

                        currentElement.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);

                        canvasRef.ResumeLayout(true);
                    }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationShrinkMillis, finished: (v2, c2) =>
                    {
                        var containedIconColl = canvasRef.Elements.Where(elem => elem.IsStoredInAFolder && 
                                                                          elem.StoredFolderTag == currentElement.Text);

                        if (containedIconColl != null && containedIconColl.Any() && containedIconColl.Count() > 0)
                        {
                            List<int> indicesToRemove = new List<int>();

                            // Build a list of items to remove
                            foreach (var storedIcon in containedIconColl)
                            {
                                indicesToRemove.Add(canvasRef.Elements.IndexOf(storedIcon));
                            }

                            indicesToRemove = indicesToRemove.Where(i => i != -1)
                                                             .OrderByDescending(i => i)
                                                             .ToList();

                            foreach (var index in indicesToRemove)
                            {
                                canvasRef.Elements.RemoveAt(index);
                            }
                        }

                        canvasRef.Elements.Remove(currentElement);
                        canvasRef.Controller.PromptResave();
                    });
                });
            }
        }

        /// <summary>
        /// Modify icon text
        /// </summary>
        /// <param name="prevText"></param>
        /// <returns></returns>
        public async Task<string> ModifyIconTextAsync(string prevText)
        {
            var text = await UserDialogs.Instance.PromptAsync("Enter name for image", 
                title: LanguageSettings.EditTitle,
                okText: LanguageSettings.EditTextOK,
                cancelText: LanguageSettings.EditTextCancel,
                placeholder: prevText);

            return text.Text;
        }

        /// <summary>
        /// Make call to camera
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetImageFromCameraCallAsync()
        {
            if (!(CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported))
            {
                // <!-- If photo taking isn't supported, return with blank array -->
                await UserDialogs.Instance.AlertAsync("");                    
            }

            // <!-- Options related to image storage and formatting -->
            StoreCameraMediaOptions mediaOptions = new StoreCameraMediaOptions
            {
                Directory = "SGDPhotos",
                Name = $"{System.DateTime.UtcNow}.png",
                PhotoSize = PhotoSize.Small,
                CompressionQuality = 50,

                // <!-- These below largely have to be handled explicitly in Android
                SaveToAlbum = true,
                AllowCropping = true,
                RotateImage = true
                // --> 
            };

            return await GetImageAndCrop(mediaOptions);
        }

        /// <summary>
        /// Get saved image
        /// </summary>
        /// <param name="mediaOptions"></param>
        /// <returns></returns>
        public async Task<string> GetImageAndCrop(StoreCameraMediaOptions mediaOptions)
        {
            try
            {
                using (MediaFile file = await CrossMedia.Current.TakePhotoAsync(mediaOptions))
                {
                    string newPath = "";

                    Debug.WriteLineIf(App.OutputVerbose, "In Taker");

                    if (file == null || file.Path == null || file.Path == "")
                    {
                        return null;
                    }
                    else if (File.Exists(@file.Path))
                    {
                        var path = Path.GetDirectoryName(@file.Path);

                        if (Device.RuntimePlatform == Device.Android)
                        {
                            newPath = Path.Combine(path, Path.GetFileNameWithoutExtension(@file.Path) + "crop.jpg");

                            // <!-- Note: this crops an image to square, since not a default in Android -->
                            DependencyService.Get<InterfaceBitmapResize>().ResizeBitmaps(@file.Path, @newPath);

                        }
                        else if (Device.RuntimePlatform == Device.iOS)
                        {
                            // <!-- Note: iOS has a center crop option built in -->
                            newPath = Path.Combine(path, Path.GetFileName(@file.Path));
                        }

                        var getNamedImageInfo = await UserDialogs.Instance.PromptAsync("Name picture");
                        var getNamedImage = getNamedImageInfo.Text;

                        byte[] imageArray = File.ReadAllBytes(@newPath);
                        string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                        return base64ImageRepresentation;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
