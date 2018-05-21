/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu

   =====================================================================================================

   Based on LauncherHijack

   Copyright (c) 2017 Ethan Nelson-Moore
   https://github.com/parrotgeek1/LauncherHijack

   You can do whatever you want with this code, but you have to credit me somewhere in your project, including a link to this repo. The modified version doesn't have to be open source. You have to send me a link to the modified version if it is ever public.
*/

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Views;

namespace FastTalkerSkiaSharp.Droid
{
    [Activity(Label = "Fast Talker",
              AlwaysRetainTaskState = true,
              Icon = "@drawable/icon",
              ScreenOrientation = ScreenOrientation.SensorLandscape,
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleInstance,
              Theme = "@style/MainTheme",
              ConfigurationChanges = ConfigChanges.Orientation |
                                     ConfigChanges.ScreenSize |
                                     ConfigChanges.Keyboard |
                                     ConfigChanges.KeyboardHidden)]
    [IntentFilter(new[] { Intent.ActionMain },
        Categories = new[]
        {
            Intent.CategoryHome,
            Android.Content.Intent.CategoryDefault
        })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Acr.UserDialogs.UserDialogs.Init(this);

            App.DisplayScreenWidth = Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density;
            App.DisplayScreenHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density;
            App.DisplayScaleFactor = Resources.DisplayMetrics.Density;

            LoadApplication(new App());

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            AudioManager audioManager = (AudioManager)Application.Context.GetSystemService(Context.AudioService);
            audioManager.SetStreamVolume(Stream.System, audioManager.GetStreamMaxVolume(Stream.System), 0);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

