/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

[assembly: Xamarin.Forms.Dependency(typeof(FastTalkerSkiaSharp.Droid.Implementations.ImplementationVersioning))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
    public class ImplementationVersioning : Interfaces.InterfaceVersioning
    {
        public string GetBuildString()
        {
            var context = global::Android.App.Application.Context;

            Android.Content.PM.PackageManager pm = context.PackageManager;
            Android.Content.PM.PackageInfo pinfo = pm.GetPackageInfo(context.PackageName, 0);

            return pinfo.VersionCode.ToString();
        }

        public string GetVersionString()
        {
            var context = global::Android.App.Application.Context;

            Android.Content.PM.PackageManager pm = context.PackageManager;
            Android.Content.PM.PackageInfo pinfo = pm.GetPackageInfo(context.PackageName, 0);

            return pinfo.VersionName;
        }
    }
}
