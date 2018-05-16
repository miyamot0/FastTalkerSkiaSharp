/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using Xamarin.Forms;
using FastTalkerSkiaSharp.ViewModels;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class CommunicationIconPicker : ContentPage
    {
        public CommunicationIconPicker()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ons the appearing.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            (BindingContext as CommunicationIconPickerViewModel).InitialLoading();
        }
    }
}