﻿/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   Fast Talker is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, version 3.

   Fast Talker is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
   </copyright>

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu

   =========================================================================================================
   
   Based on SkiaSharp.Elements
   Felipe Nicoletto
   https://github.com/FelipeNicoletto/SkiaSharp.Elements

*/

using FastTalkerSkiaSharp.Elements;
using SkiaSharp.Elements.Extensions;
using SkiaSharp.Elements.Interfaces;

namespace SkiaSharp.Elements
{
    public abstract class Element : IInvalidatable
    {
        #region Constructors

        public Element()
        {
            _transformationPivot = new SKPoint(.5f, .5f);

            IsPinnedToSpot = false;
        }

        #endregion Constructors

        #region Properties

        private int _suspendLayout;
        private int _suspendDrawBeforeAfter;
        private SKMatrix? _appliedTransformation;

        internal IElementContainer Parent { get; set; }

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

        public bool IsPinnedToSpot { get; set; }

        public float CurrentScale { get; set; }

        ElementsController _parentController = null;
        ElementsController ParentController
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

        /// <summary>
        /// Manage drawing state
        /// </summary>
        public void SuspendLayout()
        {
            _suspendLayout++;
        }

        /// <summary>
        /// Restore drawing state
        /// </summary>
        /// <param name="invalidate"></param>
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

        public void Invalidate()
        {
            if (_suspendLayout == 0)
            {
                Parent?.Invalidate();
            }
        }

        public virtual bool IsPointInside(SKPoint point)
        {
            var transformation = GetTransformation(true);
            if (transformation.HasValue && transformation.Value.TryInvert(out var invert))
            {
                point = invert.MapPoint(point);
            }
            
            return Bounds.Contains(point);
        }

        /// <summary>
        /// Bring item to front
        /// </summary>
        public void BringToFront()
        {
            var collector = Parent as IElementsCollector;
            var parentElement = Parent as Element;

            if (collector != null || parentElement != null)
            {
                var controller = GetController();
                //controller?.SuspendLayout();

                if (collector != null)
                {
                    collector.Elements.BringToFront(this);
                }

                if (parentElement != null)
                {
                    parentElement.BringToFront();
                }

                //controller?.ResumeLayout(true);
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

            if (Tag == ElementRoles.GetRoleInt(ElementRoles.Role.Settings) && !ParentController.InEditMode) return;

            if (Tag == ElementRoles.GetRoleInt(ElementRoles.Role.SentenceFrame) && !ParentController.InFramedMode) return;

            if (_suspendDrawBeforeAfter == 0)
            {
                if (Transformation != null)
                {
                    var transformation = GetTransformation(false).Value;

                    canvas.Concat(ref transformation);
                    _appliedTransformation = transformation;
                }

                // This is the communication tag, w/o access to the other projects
                if (this.Tag == ElementRoles.GetRoleInt(ElementRoles.Role.Communication))
                {
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
                else if (this.Tag == ElementRoles.GetRoleInt(ElementRoles.Role.Folder))
                {
                    canvas.DrawRect(_bounds, ParentController.PaintWhite);
                }
            }
        }

        protected void SuspendDrawBeforeAfter()
        {
            _suspendDrawBeforeAfter++;
        }

        protected void ResumeDrawBeforeAfter()
        {
            if (_suspendDrawBeforeAfter > 0)
            {
                _suspendDrawBeforeAfter--;
            }
        }

        internal SKMatrix? GetTransformation(bool concatParents)
        {
            SKMatrix? transformation = null;
            if (Transformation.HasValue)
            {
                var bounds = Bounds;

                var tx = bounds.Left + (bounds.Width * _transformationPivot.X);
                var ty = bounds.Top + (bounds.Height * _transformationPivot.Y);

                var anchor = SKMatrix.MakeTranslation(tx, ty);
                var anchorN = SKMatrix.MakeTranslation(-tx, -ty);
                
                transformation = anchor.Concat(Transformation.Value)
                                       .Concat(anchorN);
            }
            
            if (concatParents)
            {
                var parent = Parent as Element;
                if (parent != null)
                {
                    var t = parent.GetTransformation(true);

                    if (t.HasValue)
                    {
                        if(!transformation.HasValue)
                        {
                            transformation = SKMatrix.MakeIdentity();
                        }

                        transformation = t.Value.Concat(transformation.Value);
                    }
                }
            }
            
            return transformation;
        }
        
        #endregion Protected/Internal methods

        #region Private methods

        private ElementsController GetController()
        {
            var controller = Parent as ElementsController;
            if (controller != null)
            {
                return controller;
            }
            var parent = Parent as Element;

            while(parent != null)
            {
                controller = parent.Parent as ElementsController;
                if (controller != null)
                {
                    return controller;
                }

                parent = parent.Parent as Element;
            }

            return null;
        }

        #endregion Private methods
    }
}