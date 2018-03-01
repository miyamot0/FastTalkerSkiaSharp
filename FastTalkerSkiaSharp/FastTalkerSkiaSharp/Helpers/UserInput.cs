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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Interfaces;
using FastTalkerSkiaSharp.Pages;
using FastTalkerSkiaSharp.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Extensions;
using SkiaSharp;
using SkiaSharp.Elements;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Helpers
{
    public class UserInput
    {
        CanvasView canvasRef = null;

        public UserInput(CanvasView _canvasRef) 
        {
            canvasRef = _canvasRef;
        }

        /// <summary>
        /// Queries the user main settings async.
        /// </summary>
        /// <param name="canvasView">Canvas view.</param>
        /// <param name="saveSettingsAsync">Save settings async.</param>
        public async void QueryUserMainSettingsAsync()
        {
            SettingsPage settingsPopupPage = new SettingsPage(canvasRef.Controller);
            settingsPopupPage.SettingsActionEvent += SettingsInteraction;
            settingsPopupPage.SaveCommunicationIconEvent += SettingsIconInteraction;
            settingsPopupPage.SaveCommunicationElementEvent += SettingsElementInteraction;

            await App.Current.MainPage.Navigation.PushPopupAsync(settingsPopupPage);

            /*
            string userResponse = await UserDialogs.Instance.ActionSheetAsync(LanguageSettings.SettingsTitle,
                                                                              LanguageSettings.SettingsClose,
                                                                              LanguageSettings.SettingsClose,
                                                                              null,
                                                                              LanguageSettings.SettingsMenu(canvasRef.Controller));

            ResponseToQuery(userResponse);
            */
        }

        /// <summary>
        /// Add icon from local
        /// </summary>
        /// <param name="obj">Object.</param>
        private void SettingsIconInteraction(ArgsSelectedIcon obj)
        {
            canvasRef.Elements.Add(App.ImageBuilderInstance.BuildCommunicationIconLocal(obj));

            canvasRef.Controller.PromptResave();
        }

        /// <summary>
        /// Add icon from image/base64
        /// </summary>
        /// <param name="obj">Object.</param>
        private void SettingsElementInteraction(SkiaSharp.Elements.Element obj)
        {
            try
            {
                canvasRef.Elements.Add(obj);

                canvasRef.InvalidateSurface();
            }
            catch
            {
                return;
            }

            canvasRef.Controller.PromptResave();
        }

        /// <summary>
        /// Delegate for settings changes
        /// </summary>
        /// <param name="obj">Object.</param>
        private void SettingsInteraction(SettingsPage.SettingsAction obj)
        {
            Debug.WriteLine(obj);

            switch(obj)
            {
                case SettingsPage.SettingsAction.SaveBoard:
                    canvasRef.Controller.PromptResave();

                    UserDialogs.Instance.Toast("Saved Current Board");

                    break;

                case SettingsPage.SettingsAction.InvalidateBoardIcon:
                    for (int i = 0; i < canvasRef.Elements.Count; i++)
                    {
                        canvasRef.Elements[i].IsMainIconInPlay = false;
                    }

                    canvasRef.InvalidateSurface();

                    break;

                case SettingsPage.SettingsAction.InvalidateBoardFrame:
                    for (int i = 0; i < canvasRef.Elements.Count; i++)
                    {
                        canvasRef.Elements[i].IsMainIconInPlay = false;
                    }

                    canvasRef.InvalidateSurface();

                    break;
            }
        }

        /*

        /// <summary>
        /// Responses to query.
        /// </summary>
        /// <param name="canvasView">Canvas view.</param>
        /// <param name="userResponse">User response.</param>
        /// <param name="saveSettingsAsync">Save settings async.</param>
        public async void ResponseToQuery(string userResponse)
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
                    canvasRef.Controller.PromptResave();

                    return;

                #region Auto De-selecting options

                case LanguageSettings.SettingsDeselect:
                    canvasRef.Controller.UpdateSettings(isEditing: canvasRef.Controller.InEditMode,
                                                        isInFrame: canvasRef.Controller.InFramedMode,
                                                        isAutoDeselecting: true,
                                                        isInIconModeAuto: canvasRef.Controller.IconModeAuto);

                    return;

                case LanguageSettings.SettingsDeselectDisable:
                    canvasRef.Controller.UpdateSettings(isEditing: canvasRef.Controller.InEditMode,
                                                        isInFrame: canvasRef.Controller.InFramedMode,
                                                        isAutoDeselecting: false,
                                                        isInIconModeAuto: canvasRef.Controller.IconModeAuto);

                    return;

                #endregion

                #region Frame mode options

                case LanguageSettings.SettingsModeQuery:

                    string responseToModeQuery = await UserDialogs.Instance.ActionSheetAsync(title: "Select Mode",
                                                                                             cancel: LanguageSettings.SettingsClose,
                                                                                             destructive: LanguageSettings.SettingsClose,
                                                                                             cancelToken: null,
                                                                                             buttons: LanguageSettings.GetSpeechOutputModes());

                    if (!string.IsNullOrWhiteSpace(responseToModeQuery) && responseToModeQuery != LanguageSettings.SettingsClose)
                    {
                        switch (responseToModeQuery)
                        {
                            case LanguageSettings.SettingsIconManual:
                                canvasRef.Controller.UpdateSettings(isEditing: canvasRef.Controller.InEditMode,
                                                                    isInFrame: false,
                                                                    isAutoDeselecting: canvasRef.Controller.RequireDeselect,
                                                                    isInIconModeAuto: false);

                                for (int i = 0; i < canvasRef.Elements.Count; i++)
                                {
                                    canvasRef.Elements[i].IsMainIconInPlay = false;
                                }

                                canvasRef.InvalidateSurface();

                                return;

                            case LanguageSettings.SettingsIconAuto:
                                canvasRef.Controller.UpdateSettings(isEditing: canvasRef.Controller.InEditMode,
                                                                    isInFrame: false,
                                                                    isAutoDeselecting: canvasRef.Controller.RequireDeselect,
                                                                    isInIconModeAuto: true);

                                for (int i = 0; i < canvasRef.Elements.Count; i++)
                                {
                                    canvasRef.Elements[i].IsMainIconInPlay = false;
                                }

                                canvasRef.InvalidateSurface();

                                return;

                            case LanguageSettings.SettingsFramedMode:
                                canvasRef.Controller.UpdateSettings(isEditing: canvasRef.Controller.InEditMode,
                                                                    isInFrame: true,
                                                                    isAutoDeselecting: canvasRef.Controller.RequireDeselect,
                                                                    isInIconModeAuto: canvasRef.Controller.IconModeAuto);

                                for (int i = 0; i < canvasRef.Elements.Count; i++)
                                {
                                    canvasRef.Elements[i].IsMainIconInPlay = true;
                                }

                                canvasRef.InvalidateSurface();

                                return;
                        }
                    }

                    break;

                #endregion

                #region Adding icons

                case LanguageSettings.SettingsAddIcon:

                    CommunicationIconPicker newCommunicationPage = new CommunicationIconPicker();
                    newCommunicationPage.IconConstructed += SaveCommunicationIcon;

                    await App.Current.MainPage.Navigation.PushAsync(newCommunicationPage);

                    return;

                case LanguageSettings.SettingsTakePhoto:
                    string[] base64 = await GetImageFromCameraCallAsync();

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

                        canvasRef.Elements.Add(testImage);

                        canvasRef.InvalidateSurface();
                    }
                    catch
                    {
                        return;
                    }

                    canvasRef.Controller.PromptResave();

                    return;

                case LanguageSettings.SettingsAddFolder:

                    IEnumerable<SkiaSharp.Elements.Element> foldersInField = canvasRef.Elements.Where(elem => elem.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder);

                    FolderIconPicker newFolderPage = new FolderIconPicker(currentFolders: foldersInField);
                    newFolderPage.FolderConstructed += SaveFolder;

                    await App.Current.MainPage.Navigation.PushAsync(newFolderPage);

                    return;

                #endregion

                #region Resume User Behavior - Done

                case LanguageSettings.SettingsResume:
                    canvasRef.Controller.UpdateSettings(isEditing: false,
                                                        isInFrame: canvasRef.Controller.InFramedMode,
                                                        isAutoDeselecting: canvasRef.Controller.RequireDeselect,
                                                        isInIconModeAuto: canvasRef.Controller.IconModeAuto);

                    canvasRef.Controller.BackgroundColor = canvasRef.Controller.InEditMode ? SKColors.Orange : SKColors.DimGray;

                    return;

                #endregion

                case LanguageSettings.SettingsHelp:
                    HelpPopup mPopup = new HelpPopup();

                    await App.Current.MainPage.Navigation.PushPopupAsync(mPopup);

                    return;

                case LanguageSettings.SettingsAbout:
                    AboutPagePopup page = new AboutPagePopup();

                    await App.Current.MainPage.Navigation.PushPopupAsync(page);

                    return;

                default:

                    return;
            }
        }

        */

        /// <summary>
        /// Get options from user
        /// </summary>
        /// <returns></returns>
        public async Task<string> IconEditOptionsAsync()
        {
            var userResponse = await UserDialogs.Instance.ActionSheetAsync(LanguageSettings.EditTitle, 
                                                                           LanguageSettings.EditClose,
                                                                           LanguageSettings.EditClose, 
                                                                           null, 
                                                                           LanguageSettings.EditMenu());

            return userResponse;
        }

        /*

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

        */

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

            if (response && currentElement != null)
            {
                var startPoint = currentElement.Location;

                float xDiff = (deleteButton.Location.X + deleteButton.Bounds.Width / 2f) - (startPoint.X + currentElement.Bounds.Width / 2f);
                float yDiff = (deleteButton.Location.Y + deleteButton.Bounds.Height / 2f) - (startPoint.Y + currentElement.Bounds.Height / 2f);

                new Xamarin.Forms.Animation((value) =>
                {
                    try
                    {
                        if (currentElement != null)
                        {
                            canvasRef.SuspendLayout();
                            currentElement.Location = new SKPoint((startPoint.X) + (xDiff * (float)value),
                                                                  (startPoint.Y) + (yDiff * (float)value));
                            canvasRef.ResumeLayout(true);
                        }
                    }
                    catch { }

                }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationMoveMillis, finished: (v, c) =>
                {
                    new Xamarin.Forms.Animation((value) =>
                    {
                        try
                        {
                            if (currentElement != null)
                            {
                                canvasRef.SuspendLayout();
                                currentElement.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);
                                canvasRef.ResumeLayout(true);
                            }
                        } catch { }

                    }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationShrinkMillis, finished: (v2, c2) =>
                    {
                        try
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

                            if (currentElement != null)
                            {
                                canvasRef.Elements.Remove(currentElement);
                                canvasRef.Controller.PromptResave();
                            }
                        }
                        catch { }

                    });
                });
            }
        }

        /// <summary>
        /// Insert an icon into a specific folder
        /// </summary>
        /// <param name="_currentElement"></param>
        /// <param name="folderOfInterest"></param>
        public void InsertIntoFolder(SkiaSharp.Elements.Element _currentElement, IEnumerable<SkiaSharp.Elements.Element> folderOfInterest)
        {
            if (folderOfInterest != null && _currentElement != null && folderOfInterest.Count() > 0)
            {
                Debug.WriteLineIf(App.OutputVerbose, "In Completed: Insertable into folder: " + _currentElement.Tag);

                var folderToInsertInto = folderOfInterest.First();

                var startPoint = _currentElement.Location;

                float xDiff = (folderToInsertInto.Location.X + folderToInsertInto.Bounds.Width / 2f) - (startPoint.X + _currentElement.Bounds.Width / 2f);
                float yDiff = (folderToInsertInto.Location.Y + folderToInsertInto.Bounds.Height / 2f) - (startPoint.Y + _currentElement.Bounds.Height / 2f);

                new Xamarin.Forms.Animation((value) =>
                {
                    if (_currentElement != null)
                    {
                        try
                        {
                            canvasRef.SuspendLayout();
                            _currentElement.Location = new SKPoint((startPoint.X) + (xDiff * (float)value),
                                                                  (startPoint.Y) + (yDiff * (float)value));
                            canvasRef.ResumeLayout(true);
                        }
                        catch { }
                    }

                }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationMoveMillis, finished: (v, c) =>
                {
                    new Xamarin.Forms.Animation((value) =>
                    {
                        try
                        {
                            if (_currentElement != null)
                            {
                                canvasRef.SuspendLayout();

                                _currentElement.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);

                                canvasRef.ResumeLayout(true);
                            }
                        }
                        catch { }

                    }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationShrinkMillis, finished: (v2, c2) =>
                    {
                        try
                        {
                            if (_currentElement != null)
                            {
                                _currentElement.IsStoredInAFolder = true;
                                _currentElement.Transformation = SKMatrix.MakeScale(1, 1);
                                _currentElement.StoredFolderTag = folderToInsertInto.Text;

                                canvasRef.Elements.SendToBack(_currentElement);
                                canvasRef.Controller.PromptResave();
                            }
                        }
                        catch { }
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
        public async Task<string[]> GetImageFromCameraCallAsync()
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

        /*

        public async Task<string[]> GetImageFromStorageCallAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);

                Debug.WriteLine("Status: " + status);

                if (status != PermissionStatus.Granted)
                {
                    // Request permissions
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                    {
                        await Application.Current.MainPage.DisplayAlert("Need photos", "Need access to photos to make icon", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Photos });

                    status = results[Permission.Photos];
                }

                if (status == PermissionStatus.Granted)
                {
                    Debug.WriteLine("Status: granted");
                    // If permissions granted, query selection and call back

                    var results = await GetsFileBase64();

                    return results;
                    //mLayer.CallBackIcon(results[0], results[1], results[2], null);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Debug.WriteLine("Status: Not granted");

                    await UserDialogs.Instance.AlertAsync("Can not continue, try again");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(App.OutputVerbose, ex.ToString());
            }

            return null;
        }

        */

        /*

        public async Task<string[]> GetsFileBase64()
        {
            //await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await UserDialogs.Instance.AlertAsync("Permissions Not Granted");

                return null;
            }
            else
            {
                Debug.WriteLine("is supported");
            }

            try
            {
                Plugin.Media.Abstractions.MediaFile file = await CrossMedia.Current.PickPhotoAsync();

                if (file == null || file.Path == null) 
                {
                    Debug.WriteLineIf(App.OutputVerbose, "File ref null");

                    return null;
                } 

                if (File.Exists(@file.Path))
                {
                    // If the photo can be found, query user for icon label

                    Debug.WriteLine("File exists");

                    var nameString = await UserDialogs.Instance.PromptAsync("Please name the icon");

                    Debug.WriteLine("past prompt async");

                    if (nameString == null || string.IsNullOrWhiteSpace(nameString.Text)) return null;

                    Debug.WriteLine("past null check");

                    byte[] imageArray = null;

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        imageArray = DependencyService.Get<InterfaceBitmapResize>().RotateImage(@file.Path);
                    }
                    else
                    {
                        imageArray = File.ReadAllBytes(@file.Path);
                    }

                    Debug.WriteLine("got byte array");

                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                    Debug.WriteLine("got base 64");

                    file.Dispose();

                    return new string[] { nameString.Text, base64ImageRepresentation };
                }
                else
                {
                    Debug.WriteLineIf(App.OutputVerbose, "File doesnt exist");

                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(App.OutputVerbose, ex.ToString());

                return null;
            }
        }

        */

        /// <summary>
        /// Get saved image
        /// </summary>
        /// <param name="mediaOptions"></param>
        /// <returns></returns>
        public static async Task<string[]> GetImageAndCrop(StoreCameraMediaOptions mediaOptions)
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

                        return new string[] { getNamedImage, base64ImageRepresentation };
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
