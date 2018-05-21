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
    public partial class StoredIconPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public StoredIconPopup()
        {
            InitializeComponent();
        }

        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();

            (BindingContext as FastTalkerSkiaSharp.ViewModels.StoredIconPopupViewModel).LoadInformationAsync(coreLayout);
        }

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

            (BindingContext as FastTalkerSkiaSharp.ViewModels.StoredIconPopupViewModel).UnloadInformation();
		}
	}
}