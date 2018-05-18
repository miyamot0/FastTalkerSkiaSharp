/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using Foundation;
using UIKit;

namespace FastTalkerSkiaSharp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();

            global::Xamarin.Forms.Forms.Init();

            App.DisplayScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
            App.DisplayScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;
            App.DisplayScaleFactor = (float)UIScreen.MainScreen.Scale;

            LoadApplication(new App());

			KeyboardOverlap.Forms.Plugin.iOSUnified.KeyboardOverlapRenderer.Init();

            UIApplication.SharedApplication.SetStatusBarHidden(true, true);
            UIApplication.SharedApplication.StatusBarHidden = true;

            return base.FinishedLaunching(app, options);
        }
    }
}
