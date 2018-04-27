// Based on SkiaSharp.Elements
// Felipe Nicoletto
// https://github.com/FelipeNicoletto/SkiaSharp.Elements

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
