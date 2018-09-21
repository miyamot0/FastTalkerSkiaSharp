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
*/

[assembly: Xamarin.Forms.Dependency(typeof(FastTalkerSkiaSharp.Droid.Implementations.ImplementationAdministration))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
    class ImplementationAdministration : FastTalkerSkiaSharp.Interfaces.InterfaceAdministration
    {
        /// <summary>
        /// Check if permissions are even possible
        /// </summary>
        /// <returns><c>true</c>, if admin was ised, <c>false</c> otherwise.</returns>
        public bool IsAdmin()
        {
            Android.App.Admin.DevicePolicyManager devicePolicyManager = (Android.App.Admin.DevicePolicyManager) Android.App.Application.Context.GetSystemService(Android.Content.Context.DevicePolicyService);
            Android.Content.ComponentName mDeviceAdminRcvr = new Android.Content.ComponentName(Android.App.Application.Context, 
                                                               Java.Lang.Class.FromType(typeof(FastTalkerSkiaSharp.Droid.Base.DeviceAdminReceiverClass)).Name);
            
            return devicePolicyManager.IsAdminActive(mDeviceAdminRcvr);
        }

        /// <summary>
        /// Set screen pinning, with user-level privileges used as a backup if necessary
        /// </summary>
        /// <param name="status">If set to <c>true</c> status.</param>
        public void RequestAdmin(bool status)
        {
            Android.App.Admin.DevicePolicyManager devicePolicyManager = (Android.App.Admin.DevicePolicyManager) Android.App.Application.Context.GetSystemService(Android.Content.Context.DevicePolicyService);
            Android.Content.ComponentName mDeviceAdminRcvr = new Android.Content.ComponentName(Android.App.Application.Context, 
                                                               Java.Lang.Class.FromType(typeof(FastTalkerSkiaSharp.Droid.Base.DeviceAdminReceiverClass)).Name);

            // This is the preferred, hard lock method with device as administrator
            try
            {
                if (devicePolicyManager.IsAdminActive(mDeviceAdminRcvr))
                {
                    devicePolicyManager.SetLockTaskPackages(mDeviceAdminRcvr, new System.String[] { Android.App.Application.Context.PackageName });

                    if (devicePolicyManager.IsLockTaskPermitted(Android.App.Application.Context.PackageName))
                    {
                        if (status)
                        {
                            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StartLockTask();

                            return;
                        }
                        else
                        {
                            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StopLockTask();

                            return;
                        }
                    }
                }
            }
            catch  { }

            // This is the fallback, pinning the device and overriding other keys
            try
            {
                Android.App.ActivityManager activityManager = (Android.App.ActivityManager) Android.App.Application.Context.GetSystemService(Android.Content.Context.ActivityService);

                if (status)
                {
                    Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StartLockTask();
                }
                else
                {
                    Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StopLockTask();

                }
            }
            catch { }
        }
    }
}