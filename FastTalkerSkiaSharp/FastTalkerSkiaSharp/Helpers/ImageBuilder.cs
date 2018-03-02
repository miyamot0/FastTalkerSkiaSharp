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
*/

using System;
using FastTalkerSkiaSharp.Constants;
using SkiaSharp;
using SkiaSharp.Elements;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Acr.UserDialogs;

namespace FastTalkerSkiaSharp.Helpers
{
    public class ImageBuilder
    {
        static CanvasView canvasReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FastTalkerSkia.Helpers.ImageBuilder"/> class.
        /// </summary>
        /// <param name="_canvas">Canvas.</param>
        public ImageBuilder(CanvasView _canvas)
        {
            canvasReference = _canvas;
        }

        /// <summary>
        /// Builds the sentence strip.
        /// </summary>
        /// <returns>The sentence strip.</returns>
        /// <param name="canvas">Canvas.</param>
        public SkiaSharp.Elements.Rectangle BuildSentenceStrip()
        {
            SkiaSharp.SKSize sizeOfStrip = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                      DeviceLayout.StripWidth, DeviceLayout.StripHeight);

            return new SkiaSharp.Elements.Rectangle(SkiaSharp.SKRect.Create(DeviceLayout.Bezel,
                                                                            DeviceLayout.Bezel,
                                                                            sizeOfStrip.Width,
                                                                            sizeOfStrip.Height))
            {
                FillColor = new SkiaSharp.SKColor(SkiaSharp.SKColors.White.Red,
                                                  SkiaSharp.SKColors.White.Green,
                                                  SkiaSharp.SKColors.White.Blue, 200),
                BorderColor = new SkiaSharp.SKColor(SkiaSharp.SKColors.Black.Red,
                                                    SkiaSharp.SKColors.Black.Green,
                                                    SkiaSharp.SKColors.Black.Blue, 200),
                Tag = (int)SkiaSharp.Elements.CanvasView.Role.SentenceFrame,
                IsStoredInAFolder = false,
            };
        }

        /// <summary>
        /// Construct the display-related content
        /// </summary>
        /// <returns>The static element.</returns>
        /// <param name="resource">Resource.</param>
        /// <param name="xPercent">X percent.</param>
        /// <param name="yPercent">Y percent.</param>
        /// <param name="tag">Tag.</param>
        public SkiaSharp.Elements.Image BuildStaticElement(string resource, float xPercent, float yPercent, int tag)
        {
            using (var stream = App.MainAssembly.GetManifestResourceStream(resource))
            {

                SkiaSharp.SKSize sizeOfEmitter = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, xPercent, yPercent);

                SkiaSharp.Elements.Image emitterReference = new SkiaSharp.Elements.Image(SkiaSharp.SKBitmap.Decode(stream))
                {
                    Tag = tag
                };

                SkiaSharp.SKPoint centerPoint = DeviceLayout.GetEmitterPoint(canvasReference.CanvasSize, sizeOfEmitter);

                emitterReference.Bounds = SkiaSharp.SKRect.Create(centerPoint.X,
                                                                  centerPoint.Y,
                                                                  sizeOfEmitter.Height,
                                                                  sizeOfEmitter.Height);

                return emitterReference;
            }
        }

        /// <summary>
        /// Builds user interface icons
        /// </summary>
        /// <returns>The named icon.</returns>
        /// <param name="resource">Resource.</param>
        /// <param name="text">Text.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="tagCode">Tag code.</param>
        /// <param name="alignRight">If set to <c>true</c> align right.</param>
        /// <param name="alignBottom">If set to <c>true</c> align bottom.</param>
        /// <param name="opaqueBackground">If set to <c>true</c> opaque background.</param>
        public SkiaSharp.Elements.Image BuildNamedIcon(string resource, string text, float x, float y, int tagCode,
                                                       bool alignRight = false,
                                                       bool alignBottom = false,
                                                       bool opaqueBackground = false)
        {
            SkiaSharp.Elements.Image image = null;

            SKSize loadedSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, 1f, 1f);

            using (Stream stream = App.MainAssembly.GetManifestResourceStream(resource))
            {
                SkiaSharp.SKBitmap tempBitmapPre = SkiaSharp.SKBitmap.Decode(stream);
                SkiaSharp.SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                    SKBitmapResizeMethod.Lanczos3);
                //SkiaSharp.SKBitmap tempBitmap = SkiaSharp.SKBitmap.Decode(stream);


                SkiaSharp.SKBitmap returnBitmap = new SkiaSharp.SKBitmap((int)Math.Round(tempBitmap.Width * 1.5),
                                                                         (int)Math.Round(tempBitmap.Height * 1.5),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SkiaSharp.SKCanvas canvas2 = new SkiaSharp.SKCanvas(returnBitmap))
                {
                    if (opaqueBackground)
                    {
                        canvas2.Clear(SkiaSharp.SKColors.White);
                    }
                    else
                    {
                        canvas2.Clear(SkiaSharp.SKColors.Transparent);
                    }


                    canvas2.DrawBitmap(tempBitmap, SkiaSharp.SKRect.Create(Convert.ToInt16(tempBitmap.Width * 0.25),
                                                                           Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width,
                                                                           tempBitmap.Height));

                    using (SkiaSharp.SKPaint paint = new SkiaSharp.SKPaint())
                    {
                        paint.TextSize = DeviceLayout.TextSizeDefault * App.DisplayScaleFactor;
                        paint.IsAntialias = true;
                        paint.Color = SkiaSharp.SKColors.Black;
                        paint.TextAlign = SkiaSharp.SKTextAlign.Center;

                        canvas2.DrawText(text,
                                        (Convert.ToSingle(tempBitmap.Width) * 1.5f) / 2f,
                                        (Convert.ToSingle(tempBitmap.Height) * 1.35f),
                                        paint);
                    }

                    canvas2.Flush();

                    SkiaSharp.SKSize finalSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                        DeviceLayout.InterfaceDimensionDefault,
                                        DeviceLayout.InterfaceDimensionDefault);


                    SkiaSharp.SKPoint settingsPoint;

                    if (x == -1 || y == -1)
                    {
                        settingsPoint = DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                    }
                    else
                    {
                        settingsPoint = new SkiaSharp.SKPoint(x, y);
                    }

                    if (alignRight)
                    {
                        settingsPoint.X -= finalSize.Width;
                    }

                    if (alignBottom)
                    {
                        settingsPoint.Y -= finalSize.Height;
                    }


                    image = new SkiaSharp.Elements.Image(returnBitmap)
                    {
                        Tag = tagCode,
                        Text = text,
                        BorderColor = SkiaSharp.SKColors.Black,
                        LocalImage = true,
                        ImageInformation = resource,
                        IsInsertableIntoFolder = false,
                        StoredFolderTag = "",
                        BorderWidth = 2f,
                        Bounds = SkiaSharp.SKRect.Create(settingsPoint, finalSize)
                    };

                    return image;
                }
            }
        }

        /// <summary>
        /// Build icon from instance of icon class
        /// </summary>
        /// <returns>The communication icon local.</returns>
        /// <param name="icon">Icon.</param>
        public SkiaSharp.Elements.Image BuildCommunicationIconLocal(FastTalkerSkiaSharp.Storage.CommunicationIcon icon)
        {
            SkiaSharp.Elements.Image image = null;

            SKSize loadedSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, icon.Scale, icon.Scale);

            using (Stream stream = App.MainAssembly.GetManifestResourceStream(icon.ResourceLocation))
            {
                SkiaSharp.SKBitmap tempBitmapPre = SkiaSharp.SKBitmap.Decode(stream);
                SkiaSharp.SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                 SKBitmapResizeMethod.Lanczos3);

                SkiaSharp.SKBitmap returnBitmap = new SkiaSharp.SKBitmap((int)Math.Round(tempBitmap.Width * 1.4),
                                                                         (int)Math.Round(tempBitmap.Height * 1.4),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SkiaSharp.SKCanvas canvas2 = new SkiaSharp.SKCanvas(returnBitmap))
                {
                    canvas2.Clear(SkiaSharp.SKColors.Transparent);

                    canvas2.DrawBitmap(tempBitmap, SkiaSharp.SKRect.Create(Convert.ToInt16(tempBitmap.Width * 0.2),
                                                                           Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width,
                                                                           tempBitmap.Height));

                    using (SkiaSharp.SKPaint paint = new SkiaSharp.SKPaint())
                    {
                        paint.TextSize = DeviceLayout.TextSizeDefault * App.DisplayScaleFactor * icon.Scale;
                        paint.IsAntialias = true;
                        paint.Color = SkiaSharp.SKColors.Black;
                        paint.TextAlign = SkiaSharp.SKTextAlign.Center;

                        canvas2.DrawText(icon.Text,
                                        (Convert.ToSingle(tempBitmap.Width) * 1.4f) / 2f,
                                        (Convert.ToSingle(tempBitmap.Height) * 1.35f),
                                        paint);
                    }

                    canvas2.Flush();
                }

                SkiaSharp.SKSize finalSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                        DeviceLayout.InterfaceDimensionDefault,
                                                                        DeviceLayout.InterfaceDimensionDefault);

                SkiaSharp.SKPoint settingsPoint;

                if (icon.X == -1 || icon.Y == -1)
                {
                    settingsPoint = DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                }
                else
                {
                    settingsPoint = new SkiaSharp.SKPoint(icon.X, icon.Y);
                }

                image = new SkiaSharp.Elements.Image(returnBitmap)
                {
                    Tag = icon.Tag,
                    Text = icon.Text,
                    ImageInformation = icon.ResourceLocation,
                    IsPinnedToSpot = icon.IsPinned,
                    LocalImage = true,
                    IsInsertableIntoFolder = false,
                    StoredFolderTag = icon.FolderContainingIcon,
                    IsStoredInAFolder = icon.IsStoredInFolder,
                    CurrentScale = icon.Scale,
                    BorderColor = SkiaSharp.SKColors.Black,
                    BorderWidth = 2f,
                    Bounds = SkiaSharp.SKRect.Create(settingsPoint, loadedSize)
                };

                return image;
            }
        }

        /// <summary>
        /// Build icon from instance of icon class
        /// </summary>
        /// <returns>The communication icon local.</returns>
        /// <param name="icon">Icon.</param>
        public SkiaSharp.Elements.Image BuildCommunicationIconDynamic(FastTalkerSkiaSharp.Storage.CommunicationIcon icon)
        {
            SkiaSharp.Elements.Image image = null;

            byte[] data = System.Convert.FromBase64String(icon.Base64);

            SKSize loadedSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, icon.Scale, icon.Scale);

            using (Stream stream = new MemoryStream(data))
            {
                SkiaSharp.SKBitmap tempBitmapPre = SkiaSharp.SKBitmap.Decode(stream);
                SkiaSharp.SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                 SKBitmapResizeMethod.Lanczos3);

                SkiaSharp.SKBitmap returnBitmap = new SkiaSharp.SKBitmap((int)Math.Round(tempBitmap.Width * 1.4),
                                                                         (int)Math.Round(tempBitmap.Height * 1.4),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SkiaSharp.SKCanvas canvas2 = new SkiaSharp.SKCanvas(returnBitmap))
                {
                    canvas2.Clear(SkiaSharp.SKColors.Transparent);

                    canvas2.DrawBitmap(tempBitmap, SkiaSharp.SKRect.Create(Convert.ToInt16(tempBitmap.Width * 0.2),
                                                                           Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width,
                                                                           tempBitmap.Height));

                    using (SkiaSharp.SKPaint paint = new SkiaSharp.SKPaint())
                    {
                        paint.TextSize = DeviceLayout.TextSizeDefault * App.DisplayScaleFactor * icon.Scale;
                        paint.IsAntialias = true;
                        paint.Color = SkiaSharp.SKColors.Black;
                        paint.TextAlign = SkiaSharp.SKTextAlign.Center;

                        canvas2.DrawText(icon.Text,
                                        (Convert.ToSingle(tempBitmap.Width) * 1.4f) / 2f,
                                        (Convert.ToSingle(tempBitmap.Height) * 1.35f),
                                        paint);
                    }

                    canvas2.Flush();
                }

                SkiaSharp.SKSize finalSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                        DeviceLayout.InterfaceDimensionDefault,
                                                                        DeviceLayout.InterfaceDimensionDefault);

                SkiaSharp.SKPoint settingsPoint;

                if (icon.X == -1 || icon.Y == -1)
                {
                    settingsPoint = DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                }
                else
                {
                    settingsPoint = new SkiaSharp.SKPoint(icon.X, icon.Y);
                }

                image = new SkiaSharp.Elements.Image(returnBitmap)
                {
                    Tag = icon.Tag,
                    Text = icon.Text,
                    ImageInformation = icon.Base64,
                    LocalImage = false,
                    IsInsertableIntoFolder = false,
                    IsPinnedToSpot = icon.IsPinned,
                    StoredFolderTag = icon.FolderContainingIcon,
                    IsStoredInAFolder = icon.IsStoredInFolder,
                    CurrentScale = icon.Scale,
                    BorderColor = SkiaSharp.SKColors.Black,
                    BorderWidth = 2f,
                    Bounds = SkiaSharp.SKRect.Create(settingsPoint, loadedSize)
                };

                Array.Clear(data, 0, data.Length);

                return image;
            }
        }

        /// <summary>
        /// Caller for building icon from user selection
        /// </summary>
        /// <returns>The communication icon local.</returns>
        /// <param name="selectedIconArgs">Selected icon arguments.</param>
        public SkiaSharp.Elements.Image BuildCommunicationIconLocal(ArgsSelectedIcon selectedIconArgs)
        {
            var item = new FastTalkerSkiaSharp.Storage.CommunicationIcon()
            {
                Tag = (int)SkiaSharp.Elements.CanvasView.Role.Communication,
                Text = selectedIconArgs.Name,
                Local = true,
                IsStoredInFolder = false,
                IsPinned = false,
                FolderContainingIcon = "",
                ResourceLocation = LanguageSettings.ResourcePrefixPng + selectedIconArgs.ImageSource + LanguageSettings.ResourceSuffixPng,
                Scale = 1f,
                X = -1,
                Y = -1
            };

            return BuildCommunicationIconLocal(item);
        }

        /// <summary>
        /// Builds the communication folder local.
        /// </summary>
        /// <returns>The communication folder local.</returns>
        /// <param name="icon">Icon.</param>
        public SkiaSharp.Elements.Image BuildCommunicationFolderLocal(FastTalkerSkiaSharp.Storage.CommunicationIcon icon)
        {
            SkiaSharp.Elements.Image image = null;

            SKSize loadedSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, icon.Scale, icon.Scale);

            using (Stream stream = App.MainAssembly.GetManifestResourceStream(icon.ResourceLocation))
            {
                SkiaSharp.SKBitmap tempBitmapPre = SkiaSharp.SKBitmap.Decode(stream);
                SkiaSharp.SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                 SKBitmapResizeMethod.Lanczos3);

                SkiaSharp.SKBitmap returnBitmap = new SkiaSharp.SKBitmap((int)Math.Round(tempBitmap.Width * 1.5),
                                                                         (int)Math.Round(tempBitmap.Height * 1.5),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SkiaSharp.SKCanvas canvas2 = new SkiaSharp.SKCanvas(returnBitmap))
                {
                    canvas2.Clear(SkiaSharp.SKColors.Transparent);

                    canvas2.DrawBitmap(tempBitmap, SkiaSharp.SKRect.Create(Convert.ToInt16(tempBitmap.Width * 0.15),
                                                                           Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width * 1.3f,
                                                                           tempBitmap.Height * 1.3f));

                    using (SkiaSharp.SKPaint paint = new SkiaSharp.SKPaint())
                    {
                        paint.TextSize = DeviceLayout.TextSizeDefault * App.DisplayScaleFactor * icon.Scale;
                        paint.IsAntialias = true;
                        paint.Color = SkiaSharp.SKColors.White;
                        paint.TextAlign = SkiaSharp.SKTextAlign.Center;

                        canvas2.DrawText(icon.Text,
                                        (Convert.ToSingle(tempBitmap.Width) * 1.5f) / 2f,
                                        (Convert.ToSingle(tempBitmap.Height) * 1.5f) - (Convert.ToSingle(tempBitmap.Height) * 1.5f) / 4f,
                                        paint);
                    }

                    canvas2.Flush();
                }

                SkiaSharp.SKSize finalSize = DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                        DeviceLayout.InterfaceDimensionDefault,
                                                                        DeviceLayout.InterfaceDimensionDefault);

                SkiaSharp.SKPoint settingsPoint;

                if (icon.X == -1 || icon.Y == -1)
                {
                    settingsPoint = DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                }
                else
                {
                    settingsPoint = new SkiaSharp.SKPoint(icon.X, icon.Y);
                }

                image = new SkiaSharp.Elements.Image(returnBitmap)
                {
                    Tag = icon.Tag,
                    Text = icon.Text,
                    ImageInformation = icon.ResourceLocation,
                    LocalImage = true,
                    IsInsertableIntoFolder = false,
                    IsPinnedToSpot = icon.IsPinned,
                    StoredFolderTag = icon.FolderContainingIcon,
                    IsStoredInAFolder = icon.IsStoredInFolder,
                    CurrentScale = icon.Scale,
                    BorderColor = SkiaSharp.SKColors.Black,
                    BorderWidth = 2f,
                    Bounds = SkiaSharp.SKRect.Create(settingsPoint, loadedSize)
                };

                return image;
            }
        }

        /// <summary>
        /// Builds the communication folder local.
        /// </summary>
        /// <returns>The communication folder local.</returns>
        /// <param name="selectedIconArgs">Selected icon arguments.</param>
        public SkiaSharp.Elements.Image BuildCommunicationFolderLocal(ArgsSelectedIcon selectedIconArgs)
        {
            var item = new FastTalkerSkiaSharp.Storage.CommunicationIcon()
            {
                Tag = (int)SkiaSharp.Elements.CanvasView.Role.Folder,
                Text = selectedIconArgs.Name,
                Local = true,
                IsStoredInFolder = false,
                IsPinned = false,
                FolderContainingIcon = "",
                ResourceLocation = LanguageSettings.ResourcePrefixPng + selectedIconArgs.ImageSource + LanguageSettings.ResourceSuffixPng,
                Scale = 1f,
                X = -1,
                Y = -1
            };

            return BuildCommunicationFolderLocal(item);
        }

        /// <summary>
        /// Amends the icon image.
        /// </summary>
        /// <returns>The icon image.</returns>
        /// <param name="element">Element.</param>
        /// <param name="changeActions">Change actions.</param>
        public async Task<SkiaSharp.Elements.Element> AmendIconImage(SkiaSharp.Elements.Element element, string changeActions)
        {
            float newScale = element.CurrentScale;
            string newText = element.Text;
            bool isPinned = element.IsPinnedToSpot;

            switch (changeActions)
            {
                case LanguageSettings.EditGrow:
                    newScale = element.CurrentScale * 1.1f;
                    break;

                case LanguageSettings.EditShrink:
                    newScale = element.CurrentScale * 0.9f;
                    break;

                case LanguageSettings.EditGrow2:
                    newScale = element.CurrentScale * 1.5f;
                    break;

                case LanguageSettings.EditShrink2:
                    newScale = element.CurrentScale * 0.5f;
                    break;

                case LanguageSettings.EditResetSize:
                    newScale = 1.5f;
                    break;

                case LanguageSettings.EditText:
                    newText = await App.UserInputInstance.ModifyIconTextAsync(newText);

                    if (element.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder &&  canvasReference.Elements.Where(elem => elem.Text == newText).Any())
                    {
                        await UserDialogs.Instance.AlertAsync("Please select a different name for the folder.", title: "Cannot rename folder.");

                        return element;
                    }

                    break;

                case LanguageSettings.EditPinning:
                    isPinned = !isPinned;
                    break;

                case LanguageSettings.EditClose:

                    return element;

                default:

                    return element;
            }
            
            var icon = new FastTalkerSkiaSharp.Storage.CommunicationIcon()
            {
                Text = newText,
                X = element.Left,
                Y = element.Top,
                Tag = element.Tag,
                Local = element.LocalImage,
                TextVisible = true,
                IsPinned = isPinned,
                Base64 = (element.LocalImage) ? "" : element.ImageInformation,
                ResourceLocation = (element.LocalImage) ? element.ImageInformation : "",
                IsStoredInFolder = element.IsStoredInAFolder,
                FolderContainingIcon = element.StoredFolderTag,
                Scale = newScale,
                TextScale = 1f,
                HashCode = element.GetHashCode()
            };

            if (element.Tag == (int) SkiaSharp.Elements.CanvasView.Role.Communication && icon.Local)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "CanvasView.Role.Communication && icon.Local");

                return BuildCommunicationIconLocal(icon);
            }
            else if (element.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication && !icon.Local)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "CanvasView.Role.Communication && !icon.Local");

                return BuildCommunicationIconDynamic(icon);
            }
            else if (element.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "CanvasView.Role.Folder");

                return BuildCommunicationFolderLocal(icon);
            }
            else
            {
                return null;
            }
        }
    }
}
