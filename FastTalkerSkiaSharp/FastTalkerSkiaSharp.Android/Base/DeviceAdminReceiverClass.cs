/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

namespace FastTalkerSkiaSharp.Droid.Base
{
    /// <summary>
    /// Device admin receiver class.
    /// </summary>
    [Android.Content.BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", 
                                       Name = "com.smallnstats.FastTalkerSkiaSharp.Base.DeviceAdminReceiverClass")]
    [Android.App.MetaData("android.app.device_admin", Resource = "@xml/admin")]
    [Android.App.IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", Android.Content.Intent.ActionMain })]
    public class DeviceAdminReceiverClass : Android.App.Admin.DeviceAdminReceiver { }
}