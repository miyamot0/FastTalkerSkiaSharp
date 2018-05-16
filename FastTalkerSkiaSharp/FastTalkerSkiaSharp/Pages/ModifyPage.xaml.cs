/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using FastTalkerSkiaSharp.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class ModifyPage : PopupPage
    {
        public ModifyPage(SkiaSharp.Elements.Element _currentElement, SkiaSharp.Elements.ElementsController _controller)
        {
            InitializeComponent();

            BindingContext = new ModifyIconViewModel(_currentElement, _controller)
            {
                Padding = new Thickness(50, 50, 50, 50),
                IsSystemPadding = false
            };
        }

		public void UpdateCurrentIcon(SkiaSharp.Elements.Element _currentElement)
		{
			((ModifyIconViewModel)BindingContext).UpdateIcon(_currentElement);
		}
    }
}
