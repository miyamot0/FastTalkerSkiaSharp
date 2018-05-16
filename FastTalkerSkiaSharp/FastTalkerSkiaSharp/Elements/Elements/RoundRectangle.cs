/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu

   =========================================================================================================
   
   Based on SkiaSharp.Elements
   Felipe Nicoletto
   https://github.com/FelipeNicoletto/SkiaSharp.Elements

*/

namespace SkiaSharp.Elements
{
    public class RoundRectangle : Element
    {
        #region Constructors

        public RoundRectangle(SKRect bounds) : this()
        {
            Bounds = bounds;
        }

        private RoundRectangle()
        {
            _fillColor = SKColors.Transparent;
            _borderColor = SKColors.Black;
            _borderWidth = 1;
            _drawBorder = true;
        }

        #endregion Constructors

        #region Properties

        private bool _drawBorder;
        private bool _drawFill;

        private SKColor _fillColor;
        public SKColor FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                _drawFill = value != SKColors.Transparent;
                Invalidate();
            }
        }

        private SKColor _borderColor;
        public SKColor BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                _drawBorder = _borderWidth > 0 && _borderColor != SKColors.Transparent;
                Invalidate();
            }
        }

        private float _borderWidth;
        public float BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                _drawBorder = _borderWidth > 0 && _borderColor != SKColors.Transparent;
                Invalidate();
            }
        }

        private SKPoint _cornerRadius;
        public SKPoint CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                Invalidate();
            }
        }

        #endregion Properties

        #region Public methods

        public override void Draw(SKCanvas canvas)
        {
            if (_drawFill || _drawBorder)
            {
                DrawBefore(canvas);

                using (var paint = new SKPaint
                {
                    IsAntialias = true
                })
                {
                    if (_drawFill)
                    {
                        paint.Style = SKPaintStyle.Fill;
                        paint.Color = FillColor;
                        canvas.DrawRoundRect(Bounds, CornerRadius.X, CornerRadius.Y, paint);
                    }

                    if (_drawBorder)
                    {
                        paint.Style = SKPaintStyle.Stroke;
                        paint.Color = BorderColor;
                        paint.StrokeWidth = BorderWidth;
                        canvas.DrawRoundRect(Bounds, CornerRadius.X, CornerRadius.Y, paint);
                    }
                }

                //DrawAfter(canvas);
            }
        }

        #endregion Public methods
    }
}
