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
using System;

namespace FastTalkerSkiaSharp.Controls
{
    public class IconText : IconSquare, IDisposable
    {
        #region Constructors

        public IconText(string content) : this()
        {
            _content = content;
        }

        private IconText() : base(new SKRect())
        {
            _foreColor = SKColors.Black;
            _fontSize = 15f;
        }

        #endregion Constructors

        #region Properties

        private static SKTypeface _defaultFont;
        public static SKTypeface DefaultFont { get => _defaultFont = _defaultFont ?? SKTypeface.FromFamilyName("Helvetica"); }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                Invalidate();
            }
        }

        private SKColor _foreColor;
        public SKColor ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = value;
                Invalidate();
            }
        }

        public string FontFamily
        {
            get => Font?.FamilyName ?? DefaultFont.FamilyName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Font?.Dispose();
                    Font = null;
                }
                else if (Font == null || !Font.FamilyName.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    Font = SKTypeface.FromFamilyName(value);
                }
            }
        }

        private SKTypeface _font;
        public SKTypeface Font
        {
            get => _font;
            set
            {
                _font = value;
                Invalidate();
            }
        }

        private float _fontSize;
        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                Invalidate();
            }
        }

        private SKPoint _location;
        private SKRect? _bounds;
        public override SKRect Bounds
        {
            get
            {
                if (!_bounds.HasValue)
                {
                    using (var paint = CreatePaint())
                    {
                        var b = new SKRect();
                        paint.MeasureText(Content, ref b);
                        _bounds = SKRect.Create(_location, b.Size);
                    }
                }
                return _bounds.Value;
            }
            set
            {
                _location = value.Location;
                _bounds = null;

                Invalidate();
            }
        }

        #endregion Properties

        #region Public methods

        public override void Draw(SKCanvas canvas)
        {
            DrawBefore(canvas);

            if (!string.IsNullOrWhiteSpace(Content))
            {
                using (var paint = CreatePaint())
                {
                    canvas.DrawText(Content, _location.X, _location.Y + Bounds.Height - paint.FontMetrics.Descent, paint);
                }
            }
        }

        public void Dispose()
        {
            Font?.Dispose();
            Font = null;
        }

        #endregion Public methods

        #region Private methods

        private SKPaint CreatePaint()
        {
            return new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor,
                Typeface = Font ?? DefaultFont,
                TextSize = FontSize
            };
        }

        #endregion Private methods
    }
}