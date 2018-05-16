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
        public string Name1 { get; set; }
        public ImageSource Image1 { get; set; }
		public int Rotation1 { get; set; }

        public string Name2 { get; set; }
		public ImageSource Image2 { get; set; }
        public int Rotation2 { get; set; }

        public string Name3 { get; set; }
		public ImageSource Image3 { get; set; }
        public int Rotation3 { get; set; }

        public double WidthRequest { get; set; }

        public ICommand TappedCommand { get; set; }
    }
}
