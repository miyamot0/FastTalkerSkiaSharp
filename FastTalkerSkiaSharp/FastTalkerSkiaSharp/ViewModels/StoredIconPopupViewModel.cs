﻿/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using Rg.Plugins.Popup.Services;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class StoredIconPopupViewModel : PopupUpViewModel
    {
        public event System.Action<Helpers.ArgsSelectedIcon> IconSelected = delegate { };

        System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageRowModel> _rows;
        public System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageRowModel> Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
                OnPropertyChanged("Rows");
            }
        }

        public System.Collections.Generic.List<SkiaSharp.Elements.Element> ItemsMatching { get; set; }

        public string FolderWithIcons { get; set; }

        public void UnloadInformation()
        {
            Rows.Clear();

            ItemsMatching.Clear();
        }

        public void LoadInformationAsync(Xamarin.Forms.StackLayout coreLayout)
        {
            if (Rows == null)
            {
                Rows = new System.Collections.ObjectModel.ObservableCollection<Models.DisplayImageRowModel>();
            }
            else
            {
                Rows.Clear();
            }

            int i = 0, j = 0;

            Xamarin.Forms.ImageSource source1, source2, source3;
            string tempName1, tempName2, tempName3;
            int rotation1, rotation2, rotation3;

            rotation1 = rotation2 = rotation3 = 0;

            for (i = 0; (j + i) < ItemsMatching.Count;)
            {
                tempName1 = tempName2 = tempName3 = null;
                source1 = source2 = source3 = "";

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "ResourceName: " + ItemsMatching[i].ImageInformation);
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Width: " + ItemsMatching[i].ImageInformation);

                for (j = 0; j < 3 && (j + i) < ItemsMatching.Count; j++)
                {
                    System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "ResourceName: " + ItemsMatching[j + i].Text +
                                                         " Local: " + ItemsMatching[j + i].LocalImage);

                    if (j == 0)
                    {
                        tempName1 = ItemsMatching[j + i].Text;

                        if (ItemsMatching[j + i].LocalImage)
                        {
                            source1 = Xamarin.Forms.ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                            rotation1 = 0;
                        }
                        else
                        {
                            byte[] data = System.Convert.FromBase64String(ItemsMatching[j + i].ImageInformation);
                            source1 = Xamarin.Forms.ImageSource.FromStream(() => new System.IO.MemoryStream(data));

                            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                            {
                                rotation1 = 180;
                            }
                        }
                    }
                    else if (j == 1)
                    {
                        tempName2 = ItemsMatching[j + i].Text;

                        if (ItemsMatching[j + i].LocalImage)
                        {
                            source2 = Xamarin.Forms.ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                            rotation2 = 0;
                        }
                        else
                        {
                            byte[] data = System.Convert.FromBase64String(ItemsMatching[j + i].ImageInformation);
                            source2 = Xamarin.Forms.ImageSource.FromStream(() => new System.IO.MemoryStream(data));

                            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                            {
                                rotation2 = 180;
                            }
                        }
                    }
                    else if (j == 2)
                    {
                        tempName3 = ItemsMatching[j + i].Text;

                        if (ItemsMatching[j + i].LocalImage)
                        {
                            source3 = Xamarin.Forms.ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                            rotation3 = 0;
                        }
                        else
                        {
                            byte[] data = System.Convert.FromBase64String(ItemsMatching[j + i].ImageInformation);
                            source3 = Xamarin.Forms.ImageSource.FromStream(() => new System.IO.MemoryStream(data));

                            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                            {
                                rotation3 = 180;
                            }
                        }
                    }
                }

                j = 0;

                Rows.Add(CreateModel(tempName1, source1, rotation1,
                                     tempName2, source2, rotation2,
                                     tempName3, source3, rotation3,
                                     coreLayout.Width / 4));

                i += 3;
            }
        }

        /// <summary>
        /// Creates a model for collection based on existing references
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="name1">Name1.</param>
        /// <param name="res1">Res1.</param>
        /// <param name="name2">Name2.</param>
        /// <param name="res2">Res2.</param>
        /// <param name="name3">Name3.</param>
        /// <param name="res3">Res3.</param>
        /// <param name="recommendedWidth">Recommended width.</param>
        private Models.DisplayImageRowModel CreateModel(string name1, Xamarin.Forms.ImageSource res1, int rot1,
                                                 string name2, Xamarin.Forms.ImageSource res2, int rot2,
                                                 string name3, Xamarin.Forms.ImageSource res3, int rot3,
                                                 double recommendedWidth)
        {
            return new Models.DisplayImageRowModel()
            {
                Name1 = name1,
                Image1 = res1,
                Rotation1 = rot1,

                Name2 = name2,
                Image2 = res2,
                Rotation2 = rot2,

                Name3 = name3,
                Image3 = res3,
                Rotation3 = rot3,

                WidthRequest = recommendedWidth,

                TappedCommand = new Xamarin.Forms.Command(TappedCommand)
            };
        }

        /// <summary>
        /// Icon in listview was clicked, add to field and close
        /// </summary>
        /// <param name="obj">Object.</param>
        void TappedCommand(object obj)
        {
            string iconToReintroduce = obj as string;

            if (string.IsNullOrWhiteSpace(iconToReintroduce)) return;

            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Tapped Command: " + obj as string);

            IconSelected(new Helpers.ArgsSelectedIcon
            {
                Name = iconToReintroduce,
                ImageSource = null
            });

            OnClose(null, null);
        }

        /// <summary>
        /// Call to close
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void OnClose(object sender, System.EventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await PopupNavigation.PopAsync();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
