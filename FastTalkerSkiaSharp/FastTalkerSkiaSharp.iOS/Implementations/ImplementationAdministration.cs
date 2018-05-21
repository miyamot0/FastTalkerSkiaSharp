/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

[assembly: Xamarin.Forms.Dependency(typeof(FastTalkerSkiaSharp.iOS.Implementations.ImplementationAdministration))]
namespace FastTalkerSkiaSharp.iOS.Implementations
{
    public class ImplementationAdministration : FastTalkerSkiaSharp.Interfaces.InterfaceAdministration
    {
        /// <summary>
        /// Hackish, guided access assumed available?
        /// </summary>
        /// <returns><c>true</c>, if admin was ised, <c>false</c> otherwise.</returns>
		public bool IsAdmin()
		{
            return UIKit.UIAccessibility.IsGuidedAccessEnabled;
		}

        /// <summary>
        /// Attempt to activate guided access
        /// </summary>
        /// <param name="status">If set to <c>true</c> status.</param>
        public void RequestAdmin(bool status)
		{
			UIKit.UIAccessibility.RequestGuidedAccessSession(status, (result) => {
				if (result)
                {
                    System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Locked -> true");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Locked -> false");
                }
            });
		}
    }
}
