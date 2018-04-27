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

        int currentItem = 0;
        float xPos;
        SkiaSharp.SKSize sizeOfStrip;

        SkiaSharp.Elements.Element _currentElement;

        uint animationLength = 300;

        public TitlePage ()
		{
            InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();

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
            SkiaSharp.SKSize sizeOfPanel = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 10f);

            xPos = sizeOfStrip.Width;

            foreach (string letter in title)
            {
                if (letter != "")
                {
                    tempImage = App.ImageBuilderInstance.BuildStaticElement(resource: "FastTalkerSkiaSharp.DisplayImages." + letter + ".png",
                                                                      xPercent: 0.9f,
                                                                      yPercent: 1f,
                                                                      tag: -1);

                    tempImage.X = (sizeOfPanel.Width / 2) - tempImage.Width / 2f;
                    tempImage.Y = sizeOfStrip.Height;

                    tempImage.BorderColor = SKColors.Black;
                    tempImage.BorderWidth = 2f;

                    canvasTitle.Controller.Elements.Add(tempImage);
                }  
            }

            Animater(canvasTitle.Controller.Elements[currentItem],
                     xPos - (float)(Rng.Next(0, 10) - 5),
                     (sizeOfStrip.Height / 3f) - (float)(Rng.Next(0, 40) - 20));
        }

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
                             xPos + canvasTitle.Controller.Elements[currentItem].Width * (float)currentItem - (float)(Rng.Next(0, 10) - 5),
                             (sizeOfStrip.Height / 3f) - (float)(Rng.Next(0, 40) - 20));
                }
                else if (currentItem == canvasTitle.Controller.Elements.Count)
                {
                    DrawText();
                }
            });
        }

        void DrawText()
        {
            var centerX = (DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 10f).Width / 2f);

            #region Top messages

            var topText = new SkiaSharp.Elements.Text("Developed by Shawn Gilroy (C) 2017")
            {
                Size = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 1f),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = 32f,
            };

            topText.X = centerX - topText.Width / 2f;
            topText.Y = topText.Height + 5f;

            canvasTitle.Controller.Elements.Add(topText);

            var subText = new SkiaSharp.Elements.Text("Released under the General Public License, Version 3.0")
            {
                Size = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 1f),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = 32f,
            };

            subText.X = centerX - subText.Width / 2f;
            subText.Y = topText.Bottom + subText.Height / 2f;

            canvasTitle.Controller.Elements.Add(subText);

            #endregion

            var iconsText = new SkiaSharp.Elements.Text("Visual symbols from \"Mulberry Symbol Set\" Copyright 2008-2012 Garry Paxton (CC-BY-SA 2.0). http://straight-street.com")
            {
                Size = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 1f),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = 32f,
            };

            iconsText.X = centerX - iconsText.Width / 2f;
            iconsText.Y = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 7f).Height - (iconsText.Height + 50);

            canvasTitle.Controller.Elements.Add(iconsText);
            
            var sizeBtn = DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 8f, 1.5f);

            var xOffset = (DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 7f).Width - sizeBtn.Width)/2f;

            SKRect rect = new SKRect(xOffset, 
                DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 4f).Height,
                xOffset + sizeBtn.Width,
                DeviceLayout.GetSizeByGrid(canvasTitle.CanvasSize, 10f, 4f).Height + sizeBtn.Height);

            var button = new SkiaSharp.Elements.RoundRectangle(rect)
            {
                BorderColor = SKColors.Black,
                FillColor = SKColors.IndianRed,
                BorderWidth = 10,
                CornerRadius = new SKPoint(25,25),
                Tag = 999
            };

            canvasTitle.Controller.Elements.Add(button);

            var btnText = new SkiaSharp.Elements.Text("Load Communication Board")
            {
                Size = new SKSize(button.Width, button.Height),
                ForeColor = SKColors.White,
                AutoSize = true,
                FontSize = 64f,
                Tag = 999
            };

            btnText.X = centerX - btnText.Width / 2f;
            btnText.Y = button.Top + (button.Height / 2f) - (btnText.Height / 2f) + 10;

            canvasTitle.Controller.Elements.Add(btnText);
        }

        private void Canvas_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed)
            {
                _currentElement = canvasTitle.GetElementAtPoint(e.Location);

                if (_currentElement != null && _currentElement.Tag == 999)
                {
                    App.Current.MainPage = new NavigationPage(new FastTalkerSkiaSharp.Pages.CommunicationBoardPage());
                }
            }

            e.Handled = true;
        }
    }
}