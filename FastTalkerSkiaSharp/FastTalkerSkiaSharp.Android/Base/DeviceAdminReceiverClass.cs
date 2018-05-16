/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using Android.App;
using Android.App.Admin;
using Android.Content;

namespace FastTalkerSkiaSharp.Droid.Base
{
    /// <summary>
    /// Device admin receiver class.
    /// </summary>
    [BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", 
                       Name = "com.smallnstats.FastTalkerSkiaSharp.Base.DeviceAdminReceiverClass")]
    [MetaData("android.app.device_admin", Resource = "@xml/admin")]
    [IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", Intent.ActionMain })]
    public class DeviceAdminReceiverClass : DeviceAdminReceiver { }
}