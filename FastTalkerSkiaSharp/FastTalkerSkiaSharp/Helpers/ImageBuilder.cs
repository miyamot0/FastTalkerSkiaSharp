﻿/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using SkiaSharp;
using System.Linq;

namespace FastTalkerSkiaSharp.Helpers
{
    public class ImageBuilder
    {
        static SkiaSharp.Elements.CanvasView canvasReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FastTalkerSkia.Helpers.ImageBuilder"/> class.
        /// </summary>
        /// <param name="_canvas">Canvas.</param>
        public ImageBuilder(SkiaSharp.Elements.CanvasView _canvas)
        {
            canvasReference = _canvas;
        }

        /// <summary>
        /// Builds the sentence strip.
        /// </summary>
        /// <returns>The sentence strip.</returns>
        public SkiaSharp.Elements.Rectangle BuildSentenceStrip()
        {
            SKSize sizeOfStrip = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                                          Constants.DeviceLayout.StripWidth,
                                                                                          Constants.DeviceLayout.StripHeight);

            return new SkiaSharp.Elements.Rectangle(SKRect.Create(Constants.DeviceLayout.Bezel,
                                                                            Constants.DeviceLayout.Bezel,
                                                                            sizeOfStrip.Width,
                                                                            sizeOfStrip.Height))
            {
                FillColor = new SKColor(SKColors.White.Red,
                                                  SKColors.White.Green,
                                                  SKColors.White.Blue, 200),
                BorderColor = new SKColor(SKColors.Black.Red,
                                                    SKColors.Black.Green,
                                                    SKColors.Black.Blue, 200),
                Tag = Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.SentenceFrame),
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

                SKSize sizeOfEmitter = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, xPercent, yPercent);

                SkiaSharp.Elements.Image emitterReference = new SkiaSharp.Elements.Image(SkiaSharp.SKBitmap.Decode(stream))
                {
                    Tag = tag
                };

                SKPoint centerPoint = Constants.DeviceLayout.GetEmitterPoint(canvasReference.CanvasSize, sizeOfEmitter);

                emitterReference.Bounds = SKRect.Create(centerPoint.X,
                                                                  centerPoint.Y,
                                                                  sizeOfEmitter.Height,
                                                                  sizeOfEmitter.Height);

                return emitterReference;
            }
        }

        public SkiaSharp.Elements.Image BuildStaticElement(string resource, float xPercent, float yPercent, int tag, float degrees)
        {
            using (var stream = App.MainAssembly.GetManifestResourceStream(resource))
            {

                SKSize size = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, xPercent, yPercent);

                SKPoint centerPoint = Constants.DeviceLayout.GetEmitterPoint(canvasReference.CanvasSize, size);

                SkiaSharp.Elements.Image emitterReference = new SkiaSharp.Elements.Image(SkiaSharp.SKBitmap.Decode(stream))
                {
                    Tag = tag,
                    Transformation = SKMatrix.MakeRotationDegrees(degrees)
                };

                emitterReference.Bounds = SKRect.Create(centerPoint.X,
                                                                  centerPoint.Y,
                                                                  size.Height,
                                                                  size.Height);

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

            SKSize loadedSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, 1f, 1f);

            using (System.IO.Stream stream = App.MainAssembly.GetManifestResourceStream(resource))
            {
                SKBitmap tempBitmapPre = SKBitmap.Decode(stream);
                SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                    SKBitmapResizeMethod.Lanczos3);
                //SkiaSharp.SKBitmap tempBitmap = SkiaSharp.SKBitmap.Decode(stream);

                SKBitmap returnBitmap = new SKBitmap((int)System.Math.Round(tempBitmap.Width * 1.5),
                                                                         (int)System.Math.Round(tempBitmap.Height * 1.5),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SKCanvas canvas2 = new SKCanvas(returnBitmap))
                {
                    if (opaqueBackground)
                    {
                        canvas2.Clear(SKColors.White);
                    }
                    else
                    {
                        canvas2.Clear(SKColors.Transparent);
                    }


                    canvas2.DrawBitmap(tempBitmap, SKRect.Create(System.Convert.ToInt16(tempBitmap.Width * 0.25),
                                                                           System.Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width,
                                                                           tempBitmap.Height));

                    using (SKPaint paint = new SKPaint())
                    {
                        paint.TextSize = Constants.DeviceLayout.TextSizeDefault * App.DisplayScaleFactor;
                        paint.IsAntialias = true;
                        paint.Color = SKColors.Black;
                        paint.TextAlign = SKTextAlign.Center;

                        canvas2.DrawText(text,
                                         (System.Convert.ToSingle(tempBitmap.Width) * 1.5f) / 2f,
                                         (System.Convert.ToSingle(tempBitmap.Height) * 1.35f),
                                         paint);
                    }

                    canvas2.Flush();

                    SKSize finalSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                                                Constants.DeviceLayout.InterfaceDimensionDefault,
                                                                                                Constants.DeviceLayout.InterfaceDimensionDefault);


                    SKPoint settingsPoint;

                    if ((int)x == -1 || (int)y == -1)
                    {
                        settingsPoint = Constants.DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                    }
                    else
                    {
                        settingsPoint = new SKPoint(x, y);
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
                        BorderColor = SKColors.Black,
                        LocalImage = true,
                        ImageInformation = resource,
                        IsInsertableIntoFolder = false,
                        StoredFolderTag = "",
                        BorderWidth = 2f,
                        Bounds = SKRect.Create(settingsPoint, finalSize)
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
        public SkiaSharp.Elements.Image BuildCommunicationIconLocal(Storage.CommunicationIcon icon)
        {
            SkiaSharp.Elements.Image image = null;

            SKSize loadedSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, icon.Scale, icon.Scale);

            using (System.IO.Stream stream = App.MainAssembly.GetManifestResourceStream(icon.ResourceLocation))
            {
                SKBitmap tempBitmapPre = SKBitmap.Decode(stream);
                SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                 SKBitmapResizeMethod.Lanczos3);

                SKBitmap returnBitmap = new SKBitmap((int)System.Math.Round(tempBitmap.Width * 1.4),
                                                                         (int)System.Math.Round(tempBitmap.Height * 1.4),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SKCanvas canvas2 = new SKCanvas(returnBitmap))
                {
                    canvas2.Clear(SKColors.Transparent);

                    canvas2.DrawBitmap(tempBitmap, SKRect.Create(System.Convert.ToInt16(tempBitmap.Width * 0.2),
                                                                           System.Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width,
                                                                           tempBitmap.Height));

                    using (SKPaint paint = new SKPaint())
                    {
                        paint.TextSize = Constants.DeviceLayout.TextSizeDefault * App.DisplayScaleFactor * icon.Scale;
                        paint.IsAntialias = true;
                        paint.Color = SKColors.Black;
                        paint.TextAlign = SKTextAlign.Center;

                        canvas2.DrawText(icon.Text,
                                        (System.Convert.ToSingle(tempBitmap.Width) * 1.4f) / 2f,
                                        (System.Convert.ToSingle(tempBitmap.Height) * 1.35f),
                                        paint);
                    }

                    canvas2.Flush();
                }

                SKSize finalSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                                            Constants.DeviceLayout.InterfaceDimensionDefault,
                                                                                            Constants.DeviceLayout.InterfaceDimensionDefault);

                SKPoint settingsPoint;

                if ((int)icon.X == -1 || (int)icon.Y == -1)
                {
                    settingsPoint = Constants.DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                }
                else
                {
                    settingsPoint = new SKPoint(icon.X, icon.Y);
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
                    BorderColor = SKColors.Black,
                    BorderWidth = 2f,
                    Bounds = SKRect.Create(settingsPoint, loadedSize)
                };

                return image;
            }
        }

        /// <summary>
        /// Build icon from instance of icon class
        /// </summary>
        /// <returns>The communication icon local.</returns>
        /// <param name="icon">Icon.</param>
        public SkiaSharp.Elements.Image BuildCommunicationIconDynamic(Storage.CommunicationIcon icon)
        {
            SkiaSharp.Elements.Image image = null;

            byte[] data = System.Convert.FromBase64String(icon.Base64);

            SKSize loadedSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, icon.Scale, icon.Scale);

            using (System.IO.Stream stream = new System.IO.MemoryStream(data))
            {
                SKBitmap tempBitmapPre = SKBitmap.Decode(stream);
                SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                 SKBitmapResizeMethod.Lanczos3);

                SKBitmap returnBitmap = new SKBitmap((int)System.Math.Round(tempBitmap.Width * 1.4),
                                                                         (int)System.Math.Round(tempBitmap.Height * 1.4),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SKCanvas canvas2 = new SKCanvas(returnBitmap))
                {
                    canvas2.Clear(SKColors.Transparent);

                    canvas2.DrawBitmap(tempBitmap, SKRect.Create(System.Convert.ToInt16(tempBitmap.Width * 0.2),
                                                                           System.Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width,
                                                                           tempBitmap.Height));

                    using (SKPaint paint = new SKPaint())
                    {
                        paint.TextSize = Constants.DeviceLayout.TextSizeDefault * App.DisplayScaleFactor * icon.Scale;
                        paint.IsAntialias = true;
                        paint.Color = SKColors.Black;
                        paint.TextAlign = SKTextAlign.Center;

                        canvas2.DrawText(icon.Text,
                                        (System.Convert.ToSingle(tempBitmap.Width) * 1.4f) / 2f,
                                        (System.Convert.ToSingle(tempBitmap.Height) * 1.35f),
                                        paint);
                    }

                    canvas2.Flush();
                }

                SKSize finalSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                                            Constants.DeviceLayout.InterfaceDimensionDefault,
                                                                                            Constants.DeviceLayout.InterfaceDimensionDefault);

                SKPoint settingsPoint;

                if ((int)icon.X == -1 || (int)icon.Y == -1)
                {
                    settingsPoint = Constants.DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                }
                else
                {
                    settingsPoint = new SKPoint(icon.X, icon.Y);
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
                    BorderColor = SKColors.Black,
                    BorderWidth = 2f,
                    Bounds = SKRect.Create(settingsPoint, loadedSize)
                };

                System.Array.Clear(data, 0, data.Length);

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
            var item = new Storage.CommunicationIcon()
            {
                Tag = Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication),
                Text = selectedIconArgs.Name,
                Local = true,
                IsStoredInFolder = false,
                IsPinned = false,
                FolderContainingIcon = "",
                ResourceLocation = Constants.LanguageSettings.ResourcePrefixPng + selectedIconArgs.ImageSource + Constants.LanguageSettings.ResourceSuffixPng,
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
        public SkiaSharp.Elements.Image BuildCommunicationFolderLocal(Storage.CommunicationIcon icon)
        {
            SkiaSharp.Elements.Image image = null;

            SKSize loadedSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize, icon.Scale, icon.Scale);

            using (System.IO.Stream stream = App.MainAssembly.GetManifestResourceStream(icon.ResourceLocation))
            {
                SKBitmap tempBitmapPre = SKBitmap.Decode(stream);
                SKBitmap tempBitmap = tempBitmapPre.Resize(new SKImageInfo((int)loadedSize.Width, (int)loadedSize.Width),
                                                                 SKBitmapResizeMethod.Lanczos3);

                SKBitmap returnBitmap = new SKBitmap((int)System.Math.Round(tempBitmap.Width * 1.5),
                                                                         (int)System.Math.Round(tempBitmap.Height * 1.5),
                                                                         tempBitmap.ColorType,
                                                                         tempBitmap.AlphaType);

                using (SKCanvas canvas2 = new SKCanvas(returnBitmap))
                {
                    canvas2.Clear(SKColors.Transparent);

                    canvas2.DrawBitmap(tempBitmap, SKRect.Create(System.Convert.ToInt16(tempBitmap.Width * 0.15),
                                                                           System.Convert.ToInt16(tempBitmap.Height * 0.1),
                                                                           tempBitmap.Width * 1.3f,
                                                                           tempBitmap.Height * 1.3f));

                    using (SKPaint paint = new SKPaint())
                    {
                        paint.TextSize = Constants.DeviceLayout.TextSizeDefault * App.DisplayScaleFactor * icon.Scale;
                        paint.IsAntialias = true;
                        paint.Color = SKColors.White;
                        paint.TextAlign = SKTextAlign.Center;

                        canvas2.DrawText(icon.Text,
                                        (System.Convert.ToSingle(tempBitmap.Width) * 1.5f) / 2f,
                                        (System.Convert.ToSingle(tempBitmap.Height) * 1.5f) - (System.Convert.ToSingle(tempBitmap.Height) * 1.5f) / 4f,
                                        paint);
                    }

                    canvas2.Flush();
                }

                SKSize finalSize = Constants.DeviceLayout.GetSizeByGrid(canvasReference.CanvasSize,
                                                                                            Constants.DeviceLayout.InterfaceDimensionDefault,
                                                                                            Constants.DeviceLayout.InterfaceDimensionDefault);

                SKPoint settingsPoint;

                if ((int)icon.X == -1 || (int)icon.Y == -1)
                {
                    settingsPoint = Constants.DeviceLayout.GetCenterPoint(canvasReference.CanvasSize);
                }
                else
                {
                    settingsPoint = new SKPoint(icon.X, icon.Y);
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
                    BorderColor = SKColors.Black,
                    BorderWidth = 2f,
                    Bounds = SKRect.Create(settingsPoint, loadedSize)
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
            var item = new Storage.CommunicationIcon()
            {
                Tag = Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder),
                Text = selectedIconArgs.Name,
                Local = true,
                IsStoredInFolder = false,
                IsPinned = false,
                FolderContainingIcon = "",
                ResourceLocation = Constants.LanguageSettings.ResourcePrefixPng + selectedIconArgs.ImageSource + Constants.LanguageSettings.ResourceSuffixPng,
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
        public async System.Threading.Tasks.Task<SkiaSharp.Elements.Element> AmendIconImage(SkiaSharp.Elements.Element element, string changeActions)
        {
            float newScale = element.CurrentScale;
            string newText = element.Text;
            bool isPinned = element.IsPinnedToSpot;

            switch (changeActions)
            {
                case Constants.LanguageSettings.EditGrow:
                    newScale = element.CurrentScale * 1.1f;
                    break;

                case Constants.LanguageSettings.EditShrink:
                    newScale = element.CurrentScale * 0.9f;
                    break;

                case Constants.LanguageSettings.EditGrow2:
                    newScale = element.CurrentScale * 1.5f;
                    break;

                case Constants.LanguageSettings.EditShrink2:
                    newScale = element.CurrentScale * 0.5f;
                    break;

                case Constants.LanguageSettings.EditResetSize:
                    newScale = 1.5f;
                    break;

                case Constants.LanguageSettings.EditText:
                    newText = await App.UserInputInstance.ModifyIconTextAsync(newText);

                    if (element.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder) &&
                        canvasReference.Elements.Where(elem => elem.Text == newText).Any())
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Please select a different name for the folder.", title: "Cannot rename folder.");

                        return element;
                    }

                    break;

                case Constants.LanguageSettings.EditPinning:
                    isPinned = !isPinned;
                    break;

                case Constants.LanguageSettings.EditClose:

                    return element;

                default:

                    return element;
            }

            var icon = new Storage.CommunicationIcon()
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

            if (element.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication) && icon.Local)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "CanvasView.Role.Communication && icon.Local");

                return BuildCommunicationIconLocal(icon);
            }
            else if (element.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Communication) && !icon.Local)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "CanvasView.Role.Communication && !icon.Local");

                return BuildCommunicationIconDynamic(icon);
            }
            else if (element.Tag == Elements.ElementRoles.GetRoleInt(Elements.ElementRoles.Role.Folder))
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
