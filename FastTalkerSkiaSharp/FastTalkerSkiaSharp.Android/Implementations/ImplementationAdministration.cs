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

            if (devicePolicyManager.IsAdminActive(mDeviceAdminRcvr))
            {
                devicePolicyManager.SetLockTaskPackages(mDeviceAdminRcvr, new String[] { Application.Context.PackageName });

                if (devicePolicyManager.IsLockTaskPermitted(Application.Context.PackageName))
                {
                    if (status)
                    {
                        Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StartLockTask();
                    }
                    else
                    {
                        Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.StopLockTask();
                    }
                }
            }
        }
    }
}