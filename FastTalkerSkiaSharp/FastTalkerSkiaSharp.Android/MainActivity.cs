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
        public static Activity MainApplicationActivity;

        protected override void OnCreate(Bundle bundle)
        {
            MainApplicationActivity = this;

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
    }
}

