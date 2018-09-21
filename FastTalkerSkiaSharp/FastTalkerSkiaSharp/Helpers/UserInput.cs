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

using System.Linq;
using SkiaSharp;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using FastTalkerSkiaSharp.Controls;

namespace FastTalkerSkiaSharp.Helpers
{
    public class UserInput
    {
        FieldView canvasRef = null;

        public UserInput(FieldView _canvasRef)
        {
            canvasRef = _canvasRef;
        }

        /// <summary>
        /// Queries the user main settings async.
        /// </summary>
        public async void QueryUserMainSettingsAsync()
        {
            if (AreModalsOpen()) return;

            if (App.InstanceSettingsPageViewModel == null)
            {
                App.InstanceSettingsPageViewModel = new ViewModels.SettingsPageViewModel(canvasRef.Controller)
                {
                    Padding = new Xamarin.Forms.Thickness(50, 50, 50, 50),
                    IsSystemPadding = false
                };

                App.InstanceSettingsPageViewModel.SaveCommunicationIconEvent += SettingsIconInteraction;
                App.InstanceSettingsPageViewModel.SaveCommunicationSelectionEvent += SettingsSelectInteraction;
                App.InstanceSettingsPageViewModel.SaveFolderEvent += SettingsFolderInteraction;
            }

            if (App.InstanceSettingsPage == null)
            {
                App.InstanceSettingsPage = new Pages.SettingsPage(canvasRef.Controller, App.InstanceSettingsPageViewModel);
            }

            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushPopupAsync(App.InstanceSettingsPage);
        }

        /// <summary>
        /// Add icon from local
        /// </summary>
        /// <param name="obj">Object.</param>
        void SettingsSelectInteraction(ArgsSelectedIcon obj)
        {
            canvasRef.Icons.Add(App.ImageBuilderInstance.BuildCommunicationIconLocal(obj));

            canvasRef.Controller.PromptResave();
        }

        /// <summary>
        /// Add icon from image/base64
        /// </summary>
        /// <param name="obj">Object.</param>
        private void SettingsIconInteraction(Icon obj)
        {
            try
            {
                canvasRef.Icons.Add(obj);

                canvasRef.InvalidateSurface();
            }
            catch
            {
                return;
            }

            canvasRef.Controller.PromptResave();
        }

        /// <summary>
        /// Add folder from local
        /// </summary>
        /// <param name="obj">Object.</param>
        private void SettingsFolderInteraction(ArgsSelectedIcon obj)
        {
            canvasRef.Icons.Add(App.ImageBuilderInstance.BuildCommunicationFolderLocal(obj));

            canvasRef.Controller.PromptResave();
        }

        /// <summary>
        /// There there any modals popped
        /// </summary>
        /// <returns><c>true</c>, if modals open was ared, <c>false</c> otherwise.</returns>
        public bool AreModalsOpen()
        {
            return PopupNavigation.Instance.PopupStack.Count > 0;
        }

        /// <summary>
        /// Confirms the remove icon, with some animation.
        /// </summary>
        /// <param name="currentIcon">Current element.</param>
        public async void ConfirmRemoveIcon(Icon currentIcon)
        {
            var response = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete this icon?");

            if (response)
            {
                new Xamarin.Forms.Animation((value) =>
                {
                    currentIcon.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);

                }).Commit(Xamarin.Forms.Application.Current.MainPage, "Anim", length: Constants.DeviceLayout.AnimationShrinkMillis, finished: async (v2, c2) =>
                {
                    canvasRef.Icons.Remove(currentIcon);
                    canvasRef.Controller.PromptResave();

#pragma warning disable CS0618 // Type or member is obsolete
                    await PopupNavigation.PopAsync();
#pragma warning restore CS0618 // Type or member is obsolete

                });
            }
        }

        /// <summary>
        /// Delete folder and icons within
        /// </summary>
        /// <param name="currentIcon"></param>
        public async void ConfirmDeleteFolder(Icon currentIcon)
        {
            var response = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete this folder and the icons within?");

            if (response && currentIcon != null)
            {
                new Xamarin.Forms.Animation((value) =>
                {
                    try
                    {
                        if (currentIcon != null)
                        {
                            currentIcon.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);
                        }
                    }
                    catch (System.Exception e)
                    {
                        System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, e.ToString());
                    }

                }).Commit(Xamarin.Forms.Application.Current.MainPage, "Anim", length: Constants.DeviceLayout.AnimationShrinkMillis, finished: async (v2, c2) =>
                {
                    try
                    {
                        var containedIconColl = canvasRef.Icons.Where(elem => elem.IsStoredInAFolder &&
                                                                      elem.StoredFolderTag == currentIcon.Text);

                        if (containedIconColl != null && containedIconColl.Any() && containedIconColl.Count() > 0)
                        {
                            System.Collections.Generic.List<int> indicesToRemove = new System.Collections.Generic.List<int>();

                            // Build a list of items to remove
                            foreach (var storedIcon in containedIconColl)
                            {
                                indicesToRemove.Add(canvasRef.Icons.IndexOf(storedIcon));
                            }

                            indicesToRemove = indicesToRemove.Where(i => i != -1)
                                                             .OrderByDescending(i => i)
                                                             .ToList();

                            foreach (var index in indicesToRemove)
                            {
                                canvasRef.Icons.RemoveAt(index);
                            }
                        }

                        if (currentIcon != null)
                        {
                            canvasRef.Icons.Remove(currentIcon);
                            canvasRef.Controller.PromptResave();

#pragma warning disable CS0618 // Type or member is obsolete
                            await PopupNavigation.PopAsync();
#pragma warning restore CS0618 // Type or member is obsolete

                        }
                    }
                    catch (System.Exception e)
                    {
                        System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, e.ToString());
                    }
                });
            }
        }

        /// <summary>
        /// Insert an icon into a specific folder
        /// </summary>
        /// <param name="_currentIcon"></param>
        /// <param name="folderOfInterest"></param>
        public void InsertIntoFolder(Icon _currentIcon, System.Collections.Generic.IEnumerable<Icon> folderOfInterest)
        {
            if (folderOfInterest != null && _currentIcon != null && folderOfInterest.Count() > 0)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "In Completed: Insertable into folder: " + _currentIcon.Tag);

                var folderToInsertInto = folderOfInterest.First();

                var startPoint = _currentIcon.Location;

                float xDiff = (folderToInsertInto.Location.X + folderToInsertInto.Bounds.Width / 2f) - (startPoint.X + _currentIcon.Bounds.Width / 2f);
                float yDiff = (folderToInsertInto.Location.Y + folderToInsertInto.Bounds.Height / 2f) - (startPoint.Y + _currentIcon.Bounds.Height / 2f);

                new Xamarin.Forms.Animation((value) =>
                {
                    if (_currentIcon != null)
                    {
                        try
                        {
                            _currentIcon.Location = new SKPoint((startPoint.X) + (xDiff * (float)value),
                                                                  (startPoint.Y) + (yDiff * (float)value));
                        }
                        catch (System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, e.ToString());
                        }
                    }

                }).Commit(Xamarin.Forms.Application.Current.MainPage, "Anim", length: Constants.DeviceLayout.AnimationMoveMillis, finished: (v, c) =>
                {
                    new Xamarin.Forms.Animation((value) =>
                    {
                        try
                        {
                            if (_currentIcon != null)
                            {
                                _currentIcon.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);
                            }
                        }
                        catch (System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, e.ToString());
                        }

                    }).Commit(Xamarin.Forms.Application.Current.MainPage, "Anim", length: Constants.DeviceLayout.AnimationShrinkMillis, finished: (v2, c2) =>
                    {
                        try
                        {
                            if (_currentIcon != null)
                            {
                                _currentIcon.IsStoredInAFolder = true;
                                _currentIcon.Transformation = SKMatrix.MakeScale(1, 1);
                                _currentIcon.StoredFolderTag = folderToInsertInto.Text;

                                canvasRef.Icons.SendToBack(_currentIcon);
                                canvasRef.Controller.PromptResave();
                            }
                        }
                        catch (System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, e.ToString());
                        }
                    });
                });
            }
        }

        /// <summary>
        /// Modify icon text
        /// </summary>
        /// <param name="prevText"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<string> ModifyIconTextAsync(string prevText)
        {
            var text = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("Enter name for image",
                                                              title: Constants.LanguageSettings.EditTitle,
                                                              okText: Constants.LanguageSettings.EditTextOK,
                                                              cancelText: Constants.LanguageSettings.EditTextCancel,
                                                              placeholder: prevText);

            return text.Text;
        }

        /// <summary>
        /// Make call to camera
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<string[]> GetImageFromCameraCallAsync()
        {
            if (!(Plugin.Media.CrossMedia.Current.IsCameraAvailable && Plugin.Media.CrossMedia.Current.IsTakePhotoSupported))
            {
                // <!-- If photo taking isn't supported, return with blank array -->
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("");
            }

            // <!-- Options related to image storage and formatting -->
            Plugin.Media.Abstractions.StoreCameraMediaOptions mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "SGDPhotos",
                Name = $"{System.DateTime.UtcNow}.png",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
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
        public static async System.Threading.Tasks.Task<string[]> GetImageAndCrop(Plugin.Media.Abstractions.StoreCameraMediaOptions mediaOptions)
        {
            try
            {
                using (Plugin.Media.Abstractions.MediaFile file = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(mediaOptions))
                {
                    string newPath = "";

                    System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "In Taker");

                    if (file == null || file.Path == null || file.Path == "")
                    {
                        return null;
                    }

                    if (System.IO.File.Exists(@file.Path))
                    {
                        var path = System.IO.Path.GetDirectoryName(@file.Path);

                        if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                        {
                            newPath = System.IO.Path.Combine(path, System.IO.Path.GetFileNameWithoutExtension(@file.Path) + "crop.jpg");

                            // <!-- Note: this crops an image to square, since not a default in Android -->
                            Xamarin.Forms.DependencyService.Get<Interfaces.InterfaceBitmapResize>().ResizeBitmaps(@file.Path, @newPath);

                        }
                        else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                        {
                            // <!-- Note: iOS has a center crop option built in -->
                            newPath = System.IO.Path.Combine(path, System.IO.Path.GetFileName(@file.Path));
                        }

                        var getNamedImageInfo = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("Name picture");
                        var getNamedImage = getNamedImageInfo.Text;

                        byte[] imageArray = System.IO.File.ReadAllBytes(@newPath);

                        string base64ImageRepresentation = System.Convert.ToBase64String(imageArray);

                        imageArray = null;

                        return new string[] { getNamedImage, base64ImageRepresentation };
                    }

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
