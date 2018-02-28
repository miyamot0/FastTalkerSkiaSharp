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
using System.ComponentModel;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using FastTalkerSkiaSharp.Models;
using System.Diagnostics;
using FastTalkerSkiaSharp.Helpers;
using System.Threading.Tasks;
using FastTalkerSkiaSharp.ViewTemplates;
using FastTalkerSkiaSharp.ViewModels;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class StoredIconPopup : PopupPage
    {
        public event Action<ArgsSelectedIcon> IconSelected = delegate { };

        private List<DisplayImageRowModel> rows;

        private string FolderWithIcons;

        private List<SkiaSharp.Elements.Element> ItemsMatching;

        private double recommendedWidth;

        private int i, j;

        private string tempName1, tempName2, tempName3;

        private ImageSource source1, source2, source3;

        public StoredIconPopup(string folder, List<SkiaSharp.Elements.Element> itemsMatching)
        {
            InitializeComponent();

            FolderWithIcons = folder;
            ItemsMatching = itemsMatching;

            rows = new List<DisplayImageRowModel>();

            BindingContext = new PopupUpViewModel()
            {
                Padding = new Thickness(100, 100, 100, 100),
                IsSystemPadding = true
            };
        }

        /// <summary>
        /// Ons the appearing.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadInformationAsync();
        }

        /// <summary>
        /// Load information into bindings
        /// </summary>
        private async void LoadInformationAsync()
        {
            // Wait for android
            while (coreLayout.Width == 0 || coreLayout.Width == -1)
            {
                await Task.Delay(50);
                Debug.WriteLineIf(App.OutputVerbose, "waiting...");
            }

            recommendedWidth = this.Width / 4;

            i = 0;
            j = 0;

            for (i = 0; (j + i) < ItemsMatching.Count;)
            {
                tempName1 = tempName2 = tempName3 = null;
                source1 = source2 = source3 = null;

                Debug.WriteLineIf(App.OutputVerbose, "ResourceName: " + ItemsMatching[i].ImageInformation);
                Debug.WriteLineIf(App.OutputVerbose, "Width: " + ItemsMatching[i].ImageInformation);

                for (j = 0; j < 3 && (j + i) < ItemsMatching.Count; j++)
                {
                    if (j == 0)
                    {
                        tempName1 = ItemsMatching[j + i].Text;
                        source1 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                    }
                    else if (j == 1)
                    {
                        tempName2 = ItemsMatching[j + i].Text;
                        source2 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                    }
                    else if (j == 2)
                    {
                        tempName3 = ItemsMatching[j + i].Text;
                        source3 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                    }
                }

                j = 0;

                rows.Add(CreateModel(tempName1, source1, tempName2, source2, tempName3, source3, recommendedWidth));

                i += 3;
            }

            dynamicListView.ItemTemplate = new DataTemplate(typeof(FolderIconTemplate));
            dynamicListView.ItemsSource = rows;
        }

        /// <summary>
        /// Send back to canvas
        /// </summary>
        /// <param name="obj">Object.</param>
        private void TappedCommand(object obj)
        {
            string iconToReintroduce = obj as string;

            Debug.WriteLineIf(App.OutputVerbose, "Tapped Command: " + obj as string);

            IconSelected(new ArgsSelectedIcon
            {
                Name = iconToReintroduce,
                ImageSource = null
            });

            OnClose(null, null);
        }

        /// <summary>
        /// Creates the model.
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="name1">Name1.</param>
        /// <param name="res1">Res1.</param>
        /// <param name="name2">Name2.</param>
        /// <param name="res2">Res2.</param>
        /// <param name="name3">Name3.</param>
        /// <param name="res3">Res3.</param>
        /// <param name="recommendedWidth">Recommended width.</param>
        private DisplayImageRowModel CreateModel(string name1, ImageSource res1,
                                                 string name2, ImageSource res2,
                                                 string name3, ImageSource res3,
                                                 double recommendedWidth)
        {
            return new DisplayImageRowModel()
            {
                Name1 = name1,
                Image1 = res1,

                Name2 = name2,
                Image2 = res2,

                Name3 = name3,
                Image3 = res3,

                WidthRequest = recommendedWidth,

                TappedCommand = new Command(TappedCommand)
            };
        }

        /// <summary>
        /// Ons the close.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void OnClose(object sender, EventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await PopupNavigation.PopAsync();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}