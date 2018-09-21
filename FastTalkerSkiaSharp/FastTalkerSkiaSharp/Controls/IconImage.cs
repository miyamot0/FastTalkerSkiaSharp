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
    public class IconImage : Icon
    {
        #region Constructors

        public IconImage(SKBitmap bitmap) : this()
        {
            _bitmap = bitmap;
            Size = new SKSize(bitmap.Width, bitmap.Height);
        }

        public IconImage()
        {
            _borderColor = SKColors.Transparent;
            _borderWidth = 1;
        }

        #endregion Constructors

        #region Properties

        private bool _drawBorder;

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

        private SKBitmap _bitmap;
        public SKBitmap Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                Invalidate();
            }
        }

        #endregion Properties

        #region Public methods

        public override void Draw(SKCanvas canvas)
        {
            if (_bitmap != null || _drawBorder)
            {
                DrawBefore(canvas);

                using (var paint = new SKPaint { IsAntialias = true })
                {
                    if (_bitmap != null)
                    {
                        if (IsPressed)
                        {
                            paint.ColorFilter = SKColorFilter.CreateLighting(SKColors.Black, SKColors.GreenYellow);
                        }

                        canvas.DrawBitmap(_bitmap, SKRect.Create(0, 0, _bitmap.Width, _bitmap.Height), Bounds, paint);
                    }

                    if (_drawBorder)
                    {
                        paint.Style = SKPaintStyle.Stroke;
                        paint.Color = BorderColor;
                        paint.StrokeWidth = BorderWidth;
                        canvas.DrawRect(Bounds, paint);
                    }
                }
            }
        }

        #endregion Public methods
    }
}