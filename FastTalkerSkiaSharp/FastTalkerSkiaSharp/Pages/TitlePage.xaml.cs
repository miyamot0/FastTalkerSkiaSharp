/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using FastTalkerSkiaSharp.Constants;
using FastTalkerSkiaSharp.Helpers;
using SkiaSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class TitlePage : ContentPage
	{
        public List<SKImage> ImageCollection;

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

        ImageBuilder Instance;

		bool isDrawing = true;

        int currentItem = 0;
        int jitterSpan = 80;
        int jitterSpanSide = 20;

        float xPos;

        SKSize sizeOfStrip;

        float textSize = 28f;
        float btnTextSize = 56f;

        SkiaSharp.Elements.Element _currentElement;

        uint animationLength = 300;

        public TitlePage ()
		{
            InitializeComponent ();
            
			isDrawing = true;
		}

        /// <summary>
        /// Fire event once layout is inflated
        /// </summary>
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            // Only animate once
			if (!isDrawing) return;

            while (canvasTitle.CanvasSize.Width == 0)
            {
                await Task.Delay(50);
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "waiting...");
            }

            Instance = new ImageBuilder(canvasTitle);

            canvasTitle.Controller.BackgroundColor = SKColors.BlueViolet;

            string[] title = { "F", "A", "S", "T", "", "T", "A", "L", "K", "E", "R" };

            SkiaSharp.Elements.Image tempImage;

            sizeOfStrip = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 0.05f, 5.0f);
            SKSize sizeOfPanel = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 10f);

            xPos = sizeOfStrip.Width;

            foreach (string letter in title)
            {
                if (letter != "")
                {
                    tempImage = App.ImageBuilderInstance.BuildStaticElement(resource: "FastTalkerSkiaSharp.DisplayImages." + letter + ".png",
                                                                      xPercent: 0.9f,
                                                                      yPercent: 1f,
                                                                      tag: -1);

                    tempImage.X = canvasTitle.CanvasSize.Width / 2f - tempImage.Width / 2f;
                    tempImage.Y = canvasTitle.CanvasSize.Height / 2f - tempImage.Height / 2f;

                    tempImage.BorderColor = SKColors.Black;
                    tempImage.BorderWidth = 2f;

                    canvasTitle.Controller.Elements.Add(tempImage);
                }  
            }

            Animater(canvasTitle.Controller.Elements[currentItem],
                     xPos - (float)(Rng.Next(0, jitterSpanSide) - jitterSpanSide / 2),
                     (sizeOfStrip.Height / 3f) - (float)(Rng.Next(0, jitterSpan) - jitterSpan / 2));
        }

        /// <summary>
        /// Animating figures
        /// </summary>
        /// <param name="item"></param>
        /// <param name="xEnd"></param>
        /// <param name="yEnd"></param>
        void Animater(SkiaSharp.Elements.Element item, float xEnd, float yEnd)
        {
            SKRect oldCenter = item.Bounds;

            float deltaX = oldCenter.Left - xEnd;
            float deltaY = oldCenter.Top - yEnd;

            new Animation((value) =>
            {
                item.Location = new SKPoint(oldCenter.Left - (deltaX * (float)value),
                                            oldCenter.Top - (deltaY * (float)value));
            })
            .Commit(this, "Anim", length: animationLength, easing: Easing.SpringOut, finished: (e,i) => 
            {
                currentItem = currentItem + 1;

                if (currentItem < canvasTitle.Controller.Elements.Count)
                {
                    Animater(canvasTitle.Controller.Elements[currentItem],
                             xPos + canvasTitle.Controller.Elements[currentItem].Width * (float)currentItem - (float)(Rng.Next(0, jitterSpanSide) - jitterSpanSide / 2),
                             (sizeOfStrip.Height / 3f) - (float)(Rng.Next(0, jitterSpan) - jitterSpan / 2));
                }
                else if (currentItem == canvasTitle.Controller.Elements.Count)
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

            #region Upper Text

            var topText = new SkiaSharp.Elements.Text("Developed by Shawn Gilroy (C) 2017")
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = textSize,
            };

            topText.X = centerX - topText.Width / 2f;
            topText.Y = topText.Height / 2f;

            canvasTitle.Controller.Elements.Add(topText);

            var subText = new SkiaSharp.Elements.Text("Released under the Mozilla Public License, Version 2.0")
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = textSize,
            };

            subText.X = centerX - subText.Width / 2f;
            subText.Y = topText.Bottom + subText.Height / 2f;

            canvasTitle.Controller.Elements.Add(subText);

            #endregion

            #region Lower Text

            var iconsText = new SkiaSharp.Elements.Text("Copyright 2008-2012 Garry Paxton (CC-BY-SA 2.0). http://straight-street.com")
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = textSize,
            };

            iconsText.X = centerX - iconsText.Width / 2f;
            iconsText.Y = canvasTitle.CanvasSize.Height - iconsText.Height;

            canvasTitle.Controller.Elements.Add(iconsText);

            var iconsText2 = new SkiaSharp.Elements.Text("Visual symbols from \"Mulberry Symbol Set\"")
            {
                Size = new SKSize(canvasTitle.CanvasSize.Width, canvasTitle.CanvasSize.Height / 10f),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = textSize,
            };

            iconsText2.X = centerX - iconsText2.Width / 2f;
            iconsText2.Y = iconsText.Top - (iconsText2.Height + 5f);

            canvasTitle.Controller.Elements.Add(iconsText2);

            #endregion

            #region Button

            var sizeBtn = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 8f, 2f);

            var xOffset = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 1, 7f).Width;

            SKRect rect = new SKRect(xOffset,
                                     sizeOfStrip.Height,
                                     sizeBtn.Width + xOffset, 
                                     sizeBtn.Height / 2f);

            var button = new SkiaSharp.Elements.RoundRectangle(rect)
            {
                BorderColor = SKColors.Black,
                FillColor = SKColors.IndianRed,
                BorderWidth = 6,
                CornerRadius = new SKPoint(25,25),
                Tag = 999,
            };

            button.Height = sizeBtn.Height / 2f;
            button.Y = ((canvasTitle.CanvasSize.Height / 2f) + (iconsText2.Top))/2f - button.Height/2f;

            canvasTitle.Controller.Elements.Add(button);

            #endregion

            #region Button Text

            var btnText = new SkiaSharp.Elements.Text("Load Communication Board")
            {
                Size = new SKSize(button.Width, button.Height),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = btnTextSize,
                Tag = 999
            };

            btnText.X = centerX - btnText.Width / 2f;
            btnText.Y = button.Top + (button.Height / 2f) - (btnText.Height / 2f) + 10;

            canvasTitle.Controller.Elements.Add(btnText);

            #endregion
        }

        /// <summary>
        /// Launch board if hit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed)
            {
                _currentElement = canvasTitle.GetElementAtPoint(e.Location);

                if (_currentElement != null && _currentElement.Tag == 999)
                {
					App.BoardPage = new NavigationPage(new FastTalkerSkiaSharp.Pages.CommunicationBoardPage());

					Application.Current.MainPage = App.BoardPage;
                }
            }

            e.Handled = true;
        }
    }
}