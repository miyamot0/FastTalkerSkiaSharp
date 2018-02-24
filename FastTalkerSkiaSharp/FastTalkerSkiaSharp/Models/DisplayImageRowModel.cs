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

using System.Windows.Input;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Models
{
    public class DisplayImageRowModel
    {
        public string Name1 { get; set; }
        public ImageSource Image1 { get; set; }

        public string Name2 { get; set; }
        public ImageSource Image2 { get; set; }

        public string Name3 { get; set; }
        public ImageSource Image3 { get; set; }

        public double WidthRequest { get; set; }

        public ICommand TappedCommand { get; set; }
    }
}
