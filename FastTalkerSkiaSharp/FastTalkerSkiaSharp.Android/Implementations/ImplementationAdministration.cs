/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System;

using Android.App;
using Android.App.Admin;
using Android.Content;
using FastTalkerSkiaSharp.Droid.Base;
using FastTalkerSkiaSharp.Droid.Implementations;
using FastTalkerSkiaSharp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ImplementationAdministration))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
    class ImplementationAdministration : InterfaceAdministration
    {
        public bool IsAdmin()
        {
            DevicePolicyManager devicePolicyManager = (DevicePolicyManager)Application.Context.GetSystemService(Context.DevicePolicyService);
            ComponentName mDeviceAdminRcvr = new ComponentName(Application.Context, Java.Lang.Class.FromType(typeof(DeviceAdminReceiverClass)).Name);

            return devicePolicyManager.IsAdminActive(mDeviceAdminRcvr);
        }

        public void RequestAdmin(bool status)
        {
            DevicePolicyManager devicePolicyManager = (DevicePolicyManager)Application.Context.GetSystemService(Context.DevicePolicyService);
            ComponentName mDeviceAdminRcvr = new ComponentName(Application.Context, Java.Lang.Class.FromType(typeof(DeviceAdminReceiverClass)).Name);

            // This is the preferred, hard lock method with device as administrator
            try
            {
                if (devicePolicyManager.IsAdminActive(mDeviceAdminRcvr))
                {
                    devicePolicyManager.SetLockTaskPackages(mDeviceAdminRcvr, new String[] { Application.Context.PackageName });

                    if (devicePolicyManager.IsLockTaskPermitted(Application.Context.PackageName))
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
            catch
            {

            }

            // This is the fallback, pinning the device and overriding other keys
            try
            {
                ActivityManager activityManager = (ActivityManager)Application.Context.GetSystemService(Context.ActivityService);

                if (status)
                {
                    Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StartLockTask();
                }
                else
                {
                    Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StopLockTask();

                }
            }
            catch
            {

            }


        }
    }
}