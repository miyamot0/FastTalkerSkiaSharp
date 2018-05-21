/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Models
{
    public class DisplayImageModel
    {
        /// <summary>
        /// Name, which is displayed
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// ImageSource, location of image from embedded resource
        /// </summary>
        /// <value>The image.</value>
		public ImageSource Image { get; set; }

        /// <summary>
        /// Rotation
        /// </summary>
        /// <value>The rotation.</value>
        public int Rotation { get; set; } = 0;
    }
}
