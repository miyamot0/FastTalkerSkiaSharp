/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
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