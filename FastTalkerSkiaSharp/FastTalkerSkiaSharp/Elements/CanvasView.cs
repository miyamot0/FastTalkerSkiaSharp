﻿// Based on SkiaSharp.Elements
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

        #region Constructors

        public CanvasView()
		{
            _controller = new ElementsController();
            _controller.OnInvalidate += delegate (object sender, EventArgs e)
            {
                InvalidateSurface();
            };
        }

        #endregion Constructors

        #region Properties
        
        private ElementsController _controller;
        public ElementsController Controller { get => _controller; }

        public ElementsCollection Elements => _controller.Elements;
        
        #endregion Properties

        #region Public methods
        
        public Element GetElementAtPoint(SKPoint point)
        {
            return Elements.GetElementAtPoint(point);
        }
        
        public void SuspendLayout() => _controller.SuspendLayout();
        
        public void ResumeLayout(bool invalidate = false) => _controller.ResumeLayout(invalidate);
        
        #endregion Public methods

        #region Private methods

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            _controller.Clear(e.Surface.Canvas);

            base.OnPaintSurface(e);

            _controller.Draw(e.Surface.Canvas);

            PaintSurfaceAfter?.Invoke(this, e);
        }
        
        #endregion Private methods

        #region Events

        public event EventHandler<SKPaintSurfaceEventArgs> PaintSurfaceAfter;

        #endregion Events
    }
}
