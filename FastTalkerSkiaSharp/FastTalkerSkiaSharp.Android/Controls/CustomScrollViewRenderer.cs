/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using FastTalkerSkiaSharp.Controls;
using FastTalkerSkiaSharp.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomScrollView), typeof(CustomScrollViewRenderer))]
namespace FastTalkerSkiaSharp.Droid.Controls
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class CustomScrollViewRenderer : ScrollViewRenderer
#pragma warning restore CS0618 // Type or member is obsolete
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement as FastTalkerSkiaSharp.Controls.CustomScrollView;
            element?.Render();
        }
    }
}