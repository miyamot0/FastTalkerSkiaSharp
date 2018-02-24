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

using System.Threading.Tasks;
using Acr.UserDialogs;
using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Pages;
using SkiaSharp;
using SkiaSharp.Elements;

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
        public void ResponseToQuery(CanvasView canvasView, string userResponse)
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

                    App.Current.MainPage.Navigation.PushAsync(newCommunicationPage);

                    return;

                case LanguageSettings.SettingsTakePhoto:
                    // TODO

                    return;

                case LanguageSettings.SettingsAddFolder:
                    var newFolderPage = new FolderIconPicker();
                    newFolderPage.FolderConstructed += SaveFolder;

                    App.Current.MainPage.Navigation.PushAsync(newFolderPage);

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

        public async Task<string> IconEditOptionsAsync()
        {
            var userResponse = await UserDialogs.Instance.ActionSheetAsync(LanguageSettings.EditTitle, LanguageSettings.EditClose,
                                                                           LanguageSettings.EditClose, null,
            new string[] 
            { 
                LanguageSettings.EditGrow2,
                LanguageSettings.EditGrow,
                LanguageSettings.EditResetSize,
                LanguageSettings.EditShrink,
                LanguageSettings.EditShrink2 
            });

            return userResponse;

            /* 
            if (userResponse == LanguageSettings.EditClose)
            {
                Debug.WriteLine("Closing");
            }
            else
            {

                var item = App.ImageBuilderInstance.AmendIconImage(currentItem, userResponse);
                int index = canvasRef.Elements.IndexOf(currentItem);

                if (item == null && index != -1)
                {
                    Debug.WriteLine("was null or unrefernced");
                }
                else
                {
                    Debug.WriteLine("was not null");

                    Debug.WriteLine("Index of OG: " + index);

                    canvasRef.Elements[index] = item;

                    canvasRef.InvalidateSurface();
                }

                //return App.ImageBuilderInstance.AmendIconImage(currentItem, userResponse);

                 


                //canvasRef.Controller.Elements.Remove(currentItem);
                //canvasRef.Controller.Elements.Add(newItem);
                //canvasRef.Controller.Elements.BringToFront(newItem);

                //canvasRef.Controller.Invalidate();
            }
            */
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
        public async void ConfirmRemoveIcon(CanvasView canvasView, Element currentElement, Element deleteButton)
        {
            var response = await UserDialogs.Instance.ConfirmAsync("Delete this icon?");

            if (response)
            {
                var startPoint = currentElement.Location;

                float xDiff = (deleteButton.Location.X + deleteButton.Bounds.Width / 2f) - (startPoint.X + currentElement.Bounds.Width / 2f);
                float yDiff = (deleteButton.Location.Y + deleteButton.Bounds.Height / 2f) - (startPoint.Y + currentElement.Bounds.Height / 2f);

                new Xamarin.Forms.Animation((value) =>
                {
                    canvasView.SuspendLayout();
                    currentElement.Location = new SKPoint((startPoint.X) + (xDiff * (float)value),
                                                          (startPoint.Y) + (yDiff * (float)value));
                    canvasView.ResumeLayout(true);
                }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationMoveMillis, finished: (v, c) =>
                {
                    new Xamarin.Forms.Animation((value) =>
                    {
                        canvasView.SuspendLayout();

                        currentElement.Transformation = SKMatrix.MakeScale(1 - (float)value, 1 - (float)value);

                        canvasView.ResumeLayout(true);
                    }).Commit(App.Current.MainPage, "Anim", length: DeviceLayout.AnimationShrinkMillis, finished: (v2, c2) =>
                    {
                        canvasView.Elements.Remove(currentElement);
                        canvasView.Controller.PromptResave();
                    });
                });
            }
        }
    }
}
