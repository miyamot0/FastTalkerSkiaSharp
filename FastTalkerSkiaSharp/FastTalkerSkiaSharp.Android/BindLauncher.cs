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