/* 
    The MIT License

    Copyright February 8, 2016 Shawn Gilroy. http://www.smallnstats.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using SkiaSharp;

namespace FastTalkerSkiaSharp.Controls
{
    public class IconSquare : Icon
    {
        #region Constructors

        public IconSquare(SKRect bounds) : this()
        {
            Bounds = bounds;
        }

        private IconSquare()
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

                using (var paint = new SKPaint { IsAntialias = true })
                {
                    if (_drawFill)
                    {
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