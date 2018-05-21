/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.ViewCell), typeof(FastTalkerSkiaSharp.iOS.Controls.ViewCellTransparent))]
namespace FastTalkerSkiaSharp.iOS.Controls
{
    public class ViewCellTransparent : Xamarin.Forms.Platform.iOS.ViewCellRenderer
    {
        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            if (cell != null)
            {
                cell.SelectionStyle = UIKit.UITableViewCellSelectionStyle.None;
            }

            return cell;
        }
    }
}