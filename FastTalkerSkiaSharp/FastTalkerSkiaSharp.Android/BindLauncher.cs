/* 
    The MIT License

    Copyright February 8, 2016 Shawn Gilroy. http://www.smallnstats.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

    =====================================================================================================

    Based on LauncherHijack

    Copyright (c) 2017 Ethan Nelson-Moore
    https://github.com/parrotgeek1/LauncherHijack

    You can do whatever you want with this code, but you have to credit me somewhere in your project, including a link to this repo. The modified version doesn't have to be open source. You have to send me a link to the modified version if it is ever public.
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