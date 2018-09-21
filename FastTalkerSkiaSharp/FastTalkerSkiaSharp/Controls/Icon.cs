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

namespace FastTalkerSkiaSharp.Controls
{
    public abstract class Icon : InterfaceRedraw
    {
        #region Constructors

        public Icon()
        {
            _transformationPivot = new SKPoint(.5f, .5f);

            IsPinnedToSpot = false;
        }

        #endregion Constructors

        #region Properties

        internal IconsCollection Parent { get; set; }

        public SKPoint Location
        {
            get => Bounds.Location;
            set { Bounds = SKRect.Create(value, Size); }
        }

        public float X
        {
            get => Location.X;
            set { Location = new SKPoint(value, Y); }
        }

        public float Y
        {
            get => Location.Y;
            set { Location = new SKPoint(X, value); }
        }

        public SKSize Size
        {
            get => Bounds.Size;
            set { Bounds = SKRect.Create(Location, value); }
        }

        public float Width
        {
            get => Size.Width;
            set { Size = new SKSize(value, Height); }
        }

        public float Height
        {
            get => Size.Height;
            set { Size = new SKSize(Width, value); }
        }

        public float Left => Bounds.Left;
        public float Top => Bounds.Top;
        public float Right => Bounds.Right;
        public float Bottom => Bounds.Bottom;

        // Can this be spoken/colored green?
        public bool IsSpeakable = false;

        // Can we put this into a folder?
        public bool IsInsertableIntoFolder = false;

        // Is this stored on the device already?
        public bool LocalImage { get; set; }

        // This is the information related to the string
        public string ImageInformation { get; set; }

        // Is this stored in a folder?
        public bool IsStoredInAFolder { get; set; }

        // Has this been moved since press?
        public bool IsThisInMotion { get; set; }

        // What folder is this stored in?
        public string StoredFolderTag { get; set; }

        // Is this icon kept in a constant space?
        public bool IsPinnedToSpot { get; set; }

        // Current scale of icon
        public float CurrentScale { get; set; }

        FieldControl _parentController = null;
        FieldControl ParentController
        {
            get
            {
                if (_parentController == null)
                {
                    _parentController = GetController();
                }

                return _parentController;
            }
        }

        private int _tag;
        public int Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        private bool _isMainIconInPlay;
        public bool IsMainIconInPlay
        {
            get
            {
                return _isMainIconInPlay;
            }
            set
            {
                _isMainIconInPlay = value;
            }
        }

        public SKPoint Center
        {
            get
            {
                var bounds = Bounds;
                return new SKPoint(bounds.MidX, bounds.MidY);
            }
            set { Location = new SKPoint(value.X - (Width / 2), value.Y - (Height / 2)); }
        }

        private SKRect _bounds;
        public virtual SKRect Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                Invalidate();
            }
        }

        private SKMatrix? _transformation;
        public SKMatrix? Transformation
        {
            get => _transformation;
            set
            {
                _transformation = value;
                Invalidate();
            }
        }

        private SKPoint _transformationPivot;
        public SKPoint TransformationPivot
        {
            get => _transformationPivot;
            set
            {
                _transformationPivot = value;
                Invalidate();
            }
        }

        bool _isPressed = false;
        public bool IsPressed
        {
            get => _isPressed;
            set
            {
                _isPressed = value;
            }
        }

        public virtual bool EnableDrag { get; set; }

        #endregion Properties

        #region Public methods

        /// <summary>
        /// Overrride draw
        /// </summary>
        /// <param name="canvas"></param>
        public virtual void Draw(SKCanvas canvas)
        {

        }

        public void Invalidate()
        {
            Parent?.Invalidate();
        }

        public virtual bool IsPointInside(SKPoint point)
        {
            return Bounds.Contains(point);
        }

        /// <summary>
        /// Bring item to front
        /// </summary>
        public void BringToFront()
        {
            var parentIcon = Parent as Icon;

            if (ParentController != null || parentIcon != null)
            {
                if (ParentController != null)
                {
                    ParentController.Icons.BringToFront(this);
                }

                if (parentIcon != null)
                {
                    parentIcon.BringToFront();
                }
            }
        }

        #endregion Public methods

        #region Protected/Internal methods

        /// <summary>
        /// Draws the before.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        protected void DrawBefore(SKCanvas canvas)
        {
            if (this.IsStoredInAFolder) return;

            if (Tag == IconRoles.GetRoleInt(IconRoles.Role.SentenceFrame) && !ParentController.InFramedMode) return;

            // This is the communication tag, w/o access to the other projects
            if (this.Tag == IconRoles.GetRoleInt(IconRoles.Role.Communication))
            {
                // Draws highlight
                if (ParentController.InFramedMode)
                {
                    if (IsSpeakable)
                    {
                        canvas.DrawRect(_bounds, ParentController.PaintGreen);
                    }
                    else if (IsInsertableIntoFolder)
                    {
                        canvas.DrawRect(_bounds, ParentController.PaintOrange);
                    }
                    else
                    {
                        canvas.DrawRect(_bounds, ParentController.PaintWhite);
                    }
                }
                else
                {
                    if (IsInsertableIntoFolder)
                    {
                        canvas.DrawRect(_bounds, ParentController.PaintOrange);
                    }
                    else if (IsMainIconInPlay && !ParentController.IconModeAuto)
                    {
                        canvas.DrawRect(_bounds, ParentController.PaintGreen);
                    }
                    else
                    {
                        canvas.DrawRect(_bounds, ParentController.PaintWhite);
                    }
                }

                // Draws to specified location
                if (IsPinnedToSpot)
                {
                    var cirleWidth = _bounds.Width * 0.1f;
                    var circleOffset = cirleWidth / 2f;
                    var circleRadius = cirleWidth / 3f;

                    canvas.DrawCircle(_bounds.Right - circleOffset,
                                        _bounds.Top + circleOffset,
                                        circleRadius,
                                        ParentController.PaintGray);

                    canvas.DrawCircle(_bounds.Right - circleOffset,
                                        _bounds.Top + circleOffset,
                                        circleRadius,
                                        ParentController.PaintBlackStroke);
                }
            }
            else if (this.Tag == IconRoles.GetRoleInt(IconRoles.Role.Folder))
            {
                canvas.DrawRect(_bounds, ParentController.PaintWhite);
            }
        }

        #endregion Protected/Internal methods

        #region Private methods

        private FieldControl GetController()
        {
            var controller = Parent as FieldControl;
            if (controller != null)
            {
                return controller;
            }
            var parent = Parent as Icon;

            while (parent != null)
            {
                controller = parent.Parent as FieldControl;
                if (controller != null)
                {
                    return controller;
                }

                parent = parent.Parent as Icon;
            }

            return null;
        }

        #endregion Private methods
    }
}
