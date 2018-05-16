/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using Android.App;
using Android.Content;
using Android;
using Android.AccessibilityServices;
using Android.Views.Accessibility;
using Java.Lang;

namespace FastTalkerSkiaSharp.Droid
{
    [Service(Label = "Override Default Launcher with FastTalker", Permission = Manifest.Permission.BindAccessibilityService)]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    public class BindLauncher : AccessibilityService
    {
        private const int delay = 500;

        Intent intent = new Intent(Android.Content.Intent.ActionMain)
                            .AddCategory(Android.Content.Intent.CategoryHome)
                            .SetPackage("com.smallnstats.FastTalkerSkiaSharp")
                            .AddFlags(ActivityFlags.NewTask |
                                    ActivityFlags.ExcludeFromRecents |
                                    ActivityFlags.ClearTop |
                                    ActivityFlags.ReorderToFront);

        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            if (e.PackageName.Contains("com.amazon.firelauncher"))
            {
                PerformGlobalAction(GlobalAction.Recents);

                try
                {
                    Thread.Sleep(delay);
                }
                catch { }

                PerformGlobalAction(GlobalAction.Back);
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

            AccessibilityServiceInfo info = new AccessibilityServiceInfo();
            info.Flags = AccessibilityServiceFlags.Default;
            info.EventTypes = EventTypes.WindowStateChanged;
            info.FeedbackType = FeedbackFlags.Generic;
            info.PackageNames = new string[] { "com.amazon.firelauncher" };

            SetServiceInfo(info);

            PerformGlobalAction(GlobalAction.Recents);

            try
            {
                Thread.Sleep(delay);
            }
            catch { }

            PerformGlobalAction(GlobalAction.Back);

            StartActivity(intent);
        }
    }
}