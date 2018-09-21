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

using SkiaSharp;
using System;

namespace FastTalkerSkiaSharp.Constants
{
    public static class DeviceLayout
    {
        public const uint AnimationMoveMillis = 250;
        public const uint AnimationShrinkMillis = 250;

        public const float Bezel = 20;
        public const float UnitWidth = 10f;
        public const float UnitHeight = 10f;
        public const float StripWidth = 7.5f;
        public const float StripHeight = 1.5f;
        public const float InterfaceDimensionDefault = 1f;
        public const float TextSizeDefault = 18f;

        /// <summary>
        /// Return center of SKSize object
        /// </summary>
        /// <returns>The center point.</returns>
        /// <param name="deviceSize">Device size.</param>
        public static SKPoint GetCenterPoint(SKSize deviceSize)
        {
            return new SKPoint(deviceSize.Width / 2f, deviceSize.Height / 2f);
        }

        /// <summary>
        /// Return center point with some level of random jitter
        /// </summary>
        /// <returns>The center point with jitter.</returns>
        /// <param name="deviceSize">Device size.</param>
        /// <param name="iconReference">Icon reference.</param>
        public static SKPoint GetCenterPointWithJitter(SKSize deviceSize, SKSize iconReference)
        {
            Random random = new Random();

            SKSize standardSize = GetSizeByGrid(deviceSize, 1f, 1f);

            int deltaX = random.Next((int)standardSize.Width * 2) - (int)standardSize.Width;
            int deltaY = random.Next((int)standardSize.Height * 2) - (int)standardSize.Height;

            return new SKPoint((deviceSize.Width / 2f) - (iconReference.Width/2f) - deltaX, 
                               (deviceSize.Height / 2f - (iconReference.Height / 2f) - deltaY));
        }

        /// <summary>
        /// Returns SKSize with relative proportion, given the units provided
        /// </summary>
        /// <returns>The size by grid.</returns>
        /// <param name="deviceSize">Device size.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public static SKSize GetSizeByGrid(SKSize deviceSize, float x, float y)
        {
            float widthNoBezel = (deviceSize.Width - Bezel * 2) / UnitWidth;
            float heightNoBezel = (deviceSize.Width - Bezel * 2) / UnitHeight;

            return new SKSize(widthNoBezel * x, heightNoBezel * y);
        }

        /// <summary>
        /// Gets the location for the programmed emitter (at its default)
        /// </summary>
        /// <returns>The emitter point.</returns>
        /// <param name="deviceSize">Device size.</param>
        /// <param name="emitterSize">Emitter size.</param>
        public static SKPoint GetEmitterPoint(SKSize deviceSize, SKSize emitterSize)
        {
            SKSize sizeStrip = GetSizeByGrid(deviceSize, StripWidth, StripHeight);

            float sectorWidth = (deviceSize.Width + (sizeStrip.Width + Bezel)) / 2f;

            // Working from top
            float sectorHeight = (emitterSize.Height / 2f) + Bezel;

            // Scaled based on height alone, for aspect
            return new SKPoint(sectorWidth - emitterSize.Height / 2f,
                               sectorHeight - emitterSize.Height / 2f);
        }
    }
}
