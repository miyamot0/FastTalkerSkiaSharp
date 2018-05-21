/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

namespace FastTalkerSkiaSharp.Droid
{
    [Android.App.Service(Label = "Override Default Launcher with FastTalker", Permission = Android.Manifest.Permission.BindAccessibilityService)]
    [Android.App.IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    public class BindLauncher : Android.AccessibilityServices.AccessibilityService
    {
        const int delay = 500;

        Android.Content.Intent intent = new Android.Content.Intent(Android.Content.Intent.ActionMain)
                            .AddCategory(Android.Content.Intent.CategoryHome)
                            .SetPackage("com.smallnstats.FastTalkerSkiaSharp")
                            .AddFlags(Android.Content.ActivityFlags.NewTask |
                                      Android.Content.ActivityFlags.ExcludeFromRecents |
                                      Android.Content.ActivityFlags.ClearTop |
                                      Android.Content.ActivityFlags.ReorderToFront);

        public override void OnAccessibilityEvent(Android.Views.Accessibility.AccessibilityEvent e)
        {
            if (e.PackageName.Contains("com.amazon.firelauncher"))
            {
                PerformGlobalAction(Android.AccessibilityServices.GlobalAction.Recents);

                try
                {
                    Java.Lang.Thread.Sleep(delay);
                }
                catch { }

                PerformGlobalAction(Android.AccessibilityServices.GlobalAction.Back);
                StartActivity(intent);
            }
        }

        public override void OnInterrupt()
        { }

        /// <summary>
        /// Service connected event, override default launcher
        /// </summary>
        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();

            Android.AccessibilityServices.AccessibilityServiceInfo info = new Android.AccessibilityServices.AccessibilityServiceInfo();
            info.Flags = Android.AccessibilityServices.AccessibilityServiceFlags.Default;
            info.EventTypes = Android.Views.Accessibility.EventTypes.WindowStateChanged;
            info.FeedbackType = Android.AccessibilityServices.FeedbackFlags.Generic;
            info.PackageNames = new string[] { "com.amazon.firelauncher" };

            SetServiceInfo(info);

            PerformGlobalAction(Android.AccessibilityServices.GlobalAction.Recents);

            try
            {
                Java.Lang.Thread.Sleep(delay);
            }
            catch { }

            PerformGlobalAction(Android.AccessibilityServices.GlobalAction.Back);

            StartActivity(intent);
        }
    }
}