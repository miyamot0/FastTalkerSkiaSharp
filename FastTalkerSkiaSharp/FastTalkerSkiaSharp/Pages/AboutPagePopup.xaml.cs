/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

namespace FastTalkerSkiaSharp.Pages
{
    public partial class AboutPagePopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public AboutPagePopup()
        {
            InitializeComponent();

            BindingContext = new FastTalkerSkiaSharp.ViewModels.PopupUpViewModel()
            {
                Padding = new Xamarin.Forms.Thickness(50, 50, 50, 50),
                IsSystemPadding = true
            };
        }
    }
}
