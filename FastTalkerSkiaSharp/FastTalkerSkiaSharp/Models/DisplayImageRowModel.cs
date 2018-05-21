/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System.Windows.Input;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Models
{
    public class DisplayImageRowModel
    {
        /// <summary>
        /// Column 1
        /// </summary>
        /// <value>The name1.</value>
        public string Name1 { get; set; }
        public ImageSource Image1 { get; set; }
		public int Rotation1 { get; set; }

        /// <summary>
        /// Column 2
        /// </summary>
        /// <value>The name2.</value>
        public string Name2 { get; set; }
		public ImageSource Image2 { get; set; }
        public int Rotation2 { get; set; }

        /// <summary>
        /// Column 3
        /// </summary>
        /// <value>The name3.</value>
        public string Name3 { get; set; }
		public ImageSource Image3 { get; set; }
        public int Rotation3 { get; set; }

        /// <summary>
        /// Width request, for scaling
        /// </summary>
        /// <value>The width request.</value>
        public double WidthRequest { get; set; }

        /// <summary>
        /// Tapped command, one per row (with ID)
        /// </summary>
        /// <value>The tapped command.</value>
        public ICommand TappedCommand { get; set; }
    }
}
