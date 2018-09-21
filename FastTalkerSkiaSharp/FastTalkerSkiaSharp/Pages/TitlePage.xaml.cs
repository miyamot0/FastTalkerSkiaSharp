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

using FastTalkerSkiaSharp.Controls;
using SkiaSharp;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class TitlePage : Xamarin.Forms.ContentPage
    {
        SKPaint iconBackground;
        SKPaint IconBackground
        {
            get
            {
                if (iconBackground == null)
                {
                    iconBackground = new SKPaint
                    {
                        Color = SKColors.White,
                    };
                }

                return iconBackground;
            }
        }

        SKPaint iconOutline;
        SKPaint IconOutline
        {
            get
            {
                if (iconOutline == null)
                {
                    iconOutline = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsStroke = true,
                        IsAntialias = true,
                        StrokeWidth = 5f,
                        FilterQuality = SKFilterQuality.High
                    };
                }

                return iconOutline;
            }
        }

        System.Random rng;
        System.Random Rng
        {
            get
            {
                if (rng == null)
                {
                    rng = new System.Random();
                }

                return rng;
            }
        }

        Helpers.ImageBuilder Instance;

        bool isDrawing = true;

        int currentItem = 0;
        int jitterSpan = 120;
        int jitterSpanSide = 20;

        float xPos;

        SKSize sizeOfStrip;
        SKSize sizeOfPanel;

        float textSize = 28f;
        float btnTextSize = 56f;

        Icon _currentIcon;
        IconImage tempImage;

        uint animationLength = 300;

        public TitlePage()
        {
            InitializeComponent();

            isDrawing = true;
        }

        /// <summary>
        /// Fire event once layout is inflated
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!isDrawing) return;

            await Waiter();

            Instance = new Helpers.ImageBuilder(canvasTitle);

            canvasTitle.Controller.BackgroundColor = SKColors.BlueViolet;

            string[] title = { "F", "A", "S", "T", "", "T", "A", "L", "K", "E", "R" };

            sizeOfStrip = Constants.DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 0.05f, 5.0f);
            sizeOfPanel = Constants.DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 10f);

            xPos = sizeOfStrip.Width;

            foreach (string letter in title)
            {
                if (letter != "")
                {
                    tempImage = App.ImageBuilderInstance.BuildStaticIcon(resource: "FastTalkerSkiaSharp.DisplayImages." + letter + ".png",
                                                                      xPercent: 0.9f,
                                                                      yPercent: 1f,
                                                                      tag: -1);

                    tempImage.X = canvasTitle.CanvasSize.Width / 2f - tempImage.Width / 2f - (float)(Rng.Next(0, jitterSpan) - jitterSpan / 2);
                    tempImage.Y = canvasTitle.CanvasSize.Height / 2f - tempImage.Height / 2f - (float)(Rng.Next(0, jitterSpan) - jitterSpan / 2);

                    tempImage.BorderColor = SKColors.Black;
                    tempImage.BorderWidth = 2f;

                    canvasTitle.Controller.Icons.Add(tempImage);
                }
            }

            Animater(canvasTitle.Controller.Icons[currentItem],
                     xPos - (float)(Rng.Next(0, jitterSpanSide) - jitterSpanSide / 2),
                     (sizeOfStrip.Height / 3f) - (float)(Rng.Next(0, jitterSpan) - jitterSpan / 2));
        }

        /// <summary>
        /// Clean up
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            IconBackground.Dispose();
            iconBackground = null;

            IconOutline.Dispose();
            iconOutline = null;

            _currentIcon = null;

            tempImage.Bitmap.Dispose();
            tempImage = null;

            rng = null;

            Instance = null;

            canvasTitle.Icons.Clear();
        }

        async System.Threading.Tasks.Task Waiter()
        {
            while ((int) canvasTitle.CanvasSize.Width == 0)
            {
                await System.Threading.Tasks.Task.Delay(50);
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "waiting...");
            }
        }

        /// <summary>
        /// Animating figures
        /// </summary>
        /// <param name="item"></param>
        /// <param name="xEnd"></param>
        /// <param name="yEnd"></param>
        void Animater(Icon item, float xEnd, float yEnd)
        {
            SKRect oldCenter = item.Bounds;

            float deltaX = oldCenter.Left - xEnd;
            float deltaY = oldCenter.Top - yEnd;

            new Xamarin.Forms.Animation((value) =>
            {
                item.Location = new SKPoint(oldCenter.Left - (deltaX * (float)value),
                                            oldCenter.Top - (deltaY * (float)value));
            })
            .Commit(this, "Anim", length: animationLength, easing: Xamarin.Forms.Easing.SpringOut, finished: (e, i) =>
            {
                currentItem = currentItem + 1;

                if (currentItem < canvasTitle.Controller.Icons.Count)
                {
                    Animater(canvasTitle.Controller.Icons[currentItem],
                             xPos + canvasTitle.Controller.Icons[currentItem].Width * (float)currentItem - (float)(Rng.Next(0, jitterSpanSide) - jitterSpanSide / 2),
                             (sizeOfStrip.Height / 3f) - (float)(Rng.Next(0, jitterSpan) - jitterSpan / 2));
                }
                else if (currentItem == canvasTitle.Controller.Icons.Count)
                {
                    DrawText();

                    isDrawing = false;
                }
            });
        }

        /// <summary>
        /// Draw text on board
        /// </summary>
        void DrawText()
        {
            var centerX = canvasTitle.CanvasSize.Width / 2f;
            var centerY = canvasTitle.CanvasSize.Height / 2f;

            string versionString = "";

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                versionString = Xamarin.Forms.DependencyService.Get<Interfaces.InterfaceVersioning>().GetBuildString();
            }
            else if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                versionString = Xamarin.Forms.DependencyService.Get<Interfaces.InterfaceVersioning>().GetVersionString();
            }

            #region Upper Text

            var topText = new IconText("Developed by Shawn Gilroy (C) 2017 " + string.Format("({0})", versionString))
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                FontSize = textSize,
            };

            topText.X = centerX - topText.Width / 2f;
            topText.Y = topText.Height / 2f;

            canvasTitle.Controller.Icons.Add(topText);

            var subText = new IconText("Released under the Mozilla Public License, Version 2.0")
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                FontSize = textSize,
            };

            subText.X = centerX - subText.Width / 2f;
            subText.Y = topText.Bottom + subText.Height / 2f;

            canvasTitle.Controller.Icons.Add(subText);

            #endregion

            #region Lower Text

            var iconsText = new IconText("Copyright 2008-2012 Garry Paxton (CC-BY-SA 2.0). http://straight-street.com")
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                FontSize = textSize,
            };

            iconsText.X = centerX - iconsText.Width / 2f;
            iconsText.Y = canvasTitle.CanvasSize.Height - iconsText.Height;

            canvasTitle.Controller.Icons.Add(iconsText);

            var iconsText2 = new IconText("Visual symbols from \"Mulberry Symbol Set\"")
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                FontSize = textSize,
            };

            iconsText2.X = centerX - iconsText2.Width / 2f;
            iconsText2.Y = iconsText.Top - (iconsText2.Height + 5f);

            canvasTitle.Controller.Icons.Add(iconsText2);

            #endregion

            #region Button

            var sizeBtn = Constants.DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 8f, 2f);

            var xOffset = Constants.DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 1, 7f).Width;

            SKRect rect = new SKRect(xOffset,
                                     sizeOfStrip.Height,
                                     sizeBtn.Width + xOffset,
                                     sizeBtn.Height / 2f);

            var button = new IconSquareRounded(rect)
            {
                BorderColor = SKColors.Black,
                FillColor = SKColors.IndianRed,
                BorderWidth = 6,
                CornerRadius = new SKPoint(25, 25),
                Tag = 999,
            };

            button.Height = sizeBtn.Height / 2f;
            button.Y = ((canvasTitle.CanvasSize.Height / 2f) + (iconsText2.Top)) / 2f - button.Height / 2f;

            canvasTitle.Controller.Icons.Add(button);

            #endregion

            #region Button Text

            var btnText = new IconText("Load Communication Board")
            {
                Size = new SKSize(button.Width, button.Height),
                ForeColor = SKColors.White,
                FontSize = btnTextSize,
                Tag = 999
            };

            btnText.X = centerX - btnText.Width / 2f;
            btnText.Y = button.Top + (button.Height / 2f) - (btnText.Height / 2f) + 10;

            canvasTitle.Controller.Icons.Add(btnText);

            #endregion

            // Cleanup 
            topText.Dispose();
            topText = null;

            subText.Dispose();
            subText = null;

            iconsText.Dispose();
            iconsText = null;

            iconsText2.Dispose();
            iconsText2 = null;

            button = null;

            btnText.Dispose();
            btnText = null;
        }

        /// <summary>
        /// Launch board if hit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Canvas_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed)
            {
                _currentIcon = canvasTitle.GetIconAtPoint(e.Location);

                if (_currentIcon != null && _currentIcon.Tag == 999)
                {
                    App.BoardPage = new Xamarin.Forms.NavigationPage(new FastTalkerSkiaSharp.Pages.CommunicationBoardPage());

                    Xamarin.Forms.Application.Current.MainPage = App.BoardPage;
                }
            }

            e.Handled = true;
        }
    }
}