/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu

   ====================================================

   Based on SkiaSharp.Elements
   Felipe Nicoletto

   https://github.com/FelipeNicoletto/SkiaSharp.Elements

*/

using System;
using SkiaSharp.Views.Forms;
using SkiaSharp.Elements.Collections;

namespace SkiaSharp.Elements
{
    public class CanvasView : SKCanvasView
    {
        public CanvasView()
		{
            _controller = new ElementsController();
            _controller.OnInvalidate += delegate (object sender, EventArgs e)
            {
                InvalidateSurface();
            };
        }
        
        private ElementsController _controller;
        public ElementsController Controller { get => _controller; }

        public ElementsCollection Elements => _controller.Elements;
        
        public Element GetElementAtPoint(SKPoint point)
        {
            return Elements.GetElementAtPoint(point);
        }

        public void SuspendLayout() => _controller.SuspendLayout();

        public void ResumeLayout(bool invalidate = false) => _controller.ResumeLayout(invalidate);

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            _controller.Clear(e.Surface.Canvas);

            base.OnPaintSurface(e);

            _controller.Draw(e.Surface.Canvas);
        }        
    }
}
