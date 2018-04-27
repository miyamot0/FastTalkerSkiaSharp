/*
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
using SkiaSharp.Elements.Collections;
using SkiaSharp.Elements.Interfaces;
using System;

namespace SkiaSharp.Elements
{
    public class ElementsController : IElementsCollector
    {
        #region Events

        public event EventHandler OnInvalidate;

        public event EventHandler OnElementsChanged;

        public event EventHandler OnSettingsChanged;

        #endregion Events

        #region Constructors

        public ElementsController()
        {
            Elements = new ElementsCollection(this);
            BackgroundColor = SKColors.White;
        }

        #endregion Constructors

        #region Properties

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

        public ElementsCollection Elements { get; }

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
            OnElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Edit settings in underlying controller
        /// </summary>
        /// <param name="isEditing"></param>
        /// <param name="isInFrame"></param>
        /// <param name="isAutoDeselecting"></param>
        /// <param name="overridePrompt"></param>
        public void UpdateSettings(bool isEditing, bool isInFrame, 
                                   bool isAutoDeselecting, bool isInIconModeAuto, 
                                   bool overridePrompt = false)
        {
            _inEditMode = isEditing;
            _inFramedMode = isInFrame;
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
            foreach (var element in Elements)
            {
                if (element.Tag == ElementRoles.GetRoleInt(ElementRoles.Role.Settings) && !InEditMode)
                {
                    // Pass if not needed

                    continue;
                }
                else if (element.Tag == ElementRoles.GetRoleInt(ElementRoles.Role.SentenceFrame) && !InFramedMode)
                {
                    // Pass if not needed

                    continue;
                }
                else if (element.Tag == ElementRoles.GetRoleInt(ElementRoles.Role.Communication) && element.IsStoredInAFolder)
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
