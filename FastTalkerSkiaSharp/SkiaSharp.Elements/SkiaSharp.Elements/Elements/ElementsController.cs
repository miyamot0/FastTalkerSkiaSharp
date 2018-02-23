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

        public ElementsCollection Elements { get; }
        
        #endregion Properties

        #region Public methods

        public void Invalidate()
        {
            if (_suspendLayout == 0)
            {
                OnInvalidate?.Invoke(this, EventArgs.Empty);
            }
        }

        public void PromptResave()
        {
            OnElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateSettings(bool isEditing, bool isInFrame, bool isAutoDeselecting, bool overridePrompt = false)
        {
            _inEditMode = isEditing;
            _inFramedMode = isInFrame;
            _requireDeselect = isAutoDeselecting;

            if (overridePrompt)
            {
                return;
            }

            OnSettingsChanged?.Invoke(this, EventArgs.Empty);            
        }

        public void Clear(SKCanvas canvas)
        {
            canvas.Clear(BackgroundColor);
        }

        public void Draw(SKCanvas canvas)
        {
            foreach (var element in Elements)
            {
                if (element.Tag == (int) CanvasView.Role.Settings && !InEditMode)
                {
                    // Pass if not needed

                    continue;
                }
                else if (element.Tag == (int)CanvasView.Role.Delete && !InEditMode)
                {
                    // Pass if not needed

                    continue;
                }
                else if (element.Tag == (int)CanvasView.Role.SentenceFrame && !InFramedMode)
                {
                    // Pass if not needed

                    continue;
                }
                else if (element.Tag == (int)CanvasView.Role.Communication && element.IsStoredInAFolder)
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
