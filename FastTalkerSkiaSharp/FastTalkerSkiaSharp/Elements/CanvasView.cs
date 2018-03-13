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
        public enum Role
        {
            Control,        // Fixed, user behavior
            Communication,  // Dynamic, based on settings
            Display,        // Aesthetic, for decoration
            Emitter,        // For speech
            SentenceFrame,  // For framed speech
            Settings,       // Access settings
            Folder          // Folder icon
        }

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
                
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            _controller.Clear(e.Surface.Canvas);

            base.OnPaintSurface(e);

            _controller.Draw(e.Surface.Canvas);
        }        
    }
}
