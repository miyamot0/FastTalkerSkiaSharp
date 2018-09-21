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

using FastTalkerSkiaSharp.Interfaces;
using SkiaSharp;
using System;

namespace FastTalkerSkiaSharp.Controls
{
    public class FieldControl : IconsCollection
    {
        #region Events

        public event EventHandler OnInvalidate;

        public event EventHandler OnIconsChanged;

        public event EventHandler OnSettingsChanged;

        #endregion Events

        #region Constructors

        public FieldControl()
        {
            Icons = new IconCollection(this);
            BackgroundColor = SKColors.White;
        }

        #endregion Constructors

        #region Properties

        public IconCollection Icons { get; }

        private int _suspendLayout;

        private SKColor _backgroundColor;
        public SKColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                Invalidate();
            }
        }

        private bool _inEditMode;
        public bool InEditMode
        {
            get
            {
                return _inEditMode;
            }
            private set
            {
                _inEditMode = value;
            }
        }

        private bool _inFramedMode;
        public bool InFramedMode
        {
            get
            {
                return _inFramedMode;
            }
            private set
            {
                _inFramedMode = value;
            }
        }

        private bool _inFramedModeBottom;
        public bool InFramedModeBottom
        {
            get
            {
                return _inFramedModeBottom;
            }
            private set
            {
                _inFramedModeBottom = value;
            }
        }

        private bool _requireDeselect;
        public bool RequireDeselect
        {
            get
            {
                return _requireDeselect;
            }
            private set
            {
                _requireDeselect = value;
            }
        }

        private bool _iconModeAuto;
        public bool IconModeAuto
        {
            get
            {
                return _iconModeAuto;
            }
            set
            {
                _iconModeAuto = value;
            }
        }

        #endregion Properties

        #region Coloring

        SKPaint _paintWhite;
        public SKPaint PaintWhite
        {
            get
            {
                if (_paintWhite == null)
                {
                    _paintWhite = new SKPaint()
                    {
                        Color = SKColors.White
                    };
                }

                return _paintWhite;
            }
        }

        SKPaint _paintOrange;
        public SKPaint PaintOrange
        {
            get
            {
                if (_paintOrange == null)
                {
                    _paintOrange = new SKPaint()
                    {
                        Color = SKColors.Orange
                    };
                }

                return _paintOrange;
            }
        }

        SKPaint _paintGreen;
        public SKPaint PaintGreen
        {
            get
            {
                if (_paintGreen == null)
                {
                    _paintGreen = new SKPaint()
                    {
                        Color = SKColors.GreenYellow
                    };
                }

                return _paintGreen;
            }
        }

        SKPaint _paintGray;
        public SKPaint PaintGray
        {
            get
            {
                if (_paintGray == null)
                {
                    _paintGray = new SKPaint()
                    {
                        Color = SKColors.LightGray
                    };
                }

                return _paintGray;
            }
        }

        SKPaint _paintBlackStroke;
        public SKPaint PaintBlackStroke
        {
            get
            {
                if (_paintBlackStroke == null)
                {
                    _paintBlackStroke = new SKPaint()
                    {
                        Color = SKColors.Black,
                        IsStroke = true,
                        StrokeWidth = 2f,
                        IsAntialias = true
                    };
                }

                return _paintBlackStroke;
            }
        }

        #endregion 

        #region Public methods

        /// <summary>
        /// Invalidate layout event
        /// </summary>
        public void Invalidate()
        {
            if (_suspendLayout == 0)
            {
                OnInvalidate?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Save elements event
        /// </summary>
        public void PromptResave()
        {
            OnIconsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Edit settings in underlying controller
        /// </summary>
        /// <param name="isEditing"></param>
        /// <param name="isInFrame"></param>
        /// <param name="isAutoDeselecting"></param>
        /// <param name="overridePrompt"></param>
        public void UpdateSettings(bool isEditing, bool isInFrame, bool isFrameBottom,
                                   bool isAutoDeselecting, bool isInIconModeAuto,
                                   bool overridePrompt = false)
        {
            _inEditMode = isEditing;
            _inFramedMode = isInFrame;
            _inFramedModeBottom = isFrameBottom;
            _requireDeselect = isAutoDeselecting;
            _iconModeAuto = isInIconModeAuto;

            if (overridePrompt)
            {
                return;
            }

            OnSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Background color
        /// </summary>
        /// <param name="canvas"></param>
        public void Clear(SKCanvas canvas)
        {
            canvas.Clear(BackgroundColor);
        }

        /// <summary>
        /// Drawing of elements, pass over certain events if they're not necessary (or presented to user)
        /// </summary>
        /// <param name="canvas"></param>
        public void Draw(SKCanvas canvas)
        {
            foreach (var element in Icons)
            {
                if (element.Tag == IconRoles.GetRoleInt(IconRoles.Role.SentenceFrame) && !InFramedMode)
                {
                    // Pass if not needed

                    continue;
                }
                else if (element.Tag == IconRoles.GetRoleInt(IconRoles.Role.Communication) && element.IsStoredInAFolder)
                {
                    // Pass if not needed

                    continue;
                }
                else
                {
                    element.Draw(canvas);
                }
            }
        }

        public void SuspendLayout()
        {
            _suspendLayout++;
        }

        public void ResumeLayout(bool invalidate = false)
        {
            if (_suspendLayout > 0)
            {
                _suspendLayout--;
            }
            if (invalidate)
            {
                Invalidate();
            }
        }

        #endregion Public methods
    }
}
