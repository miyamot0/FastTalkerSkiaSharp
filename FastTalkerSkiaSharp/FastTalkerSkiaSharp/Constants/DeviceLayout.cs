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

        public static SKPoint GetCenterPoint(SKSize deviceSize)
        {
            return new SKPoint(deviceSize.Width / 2f, deviceSize.Height / 2f);
        }

        public static SKPoint GetCenterPointWithJitter(SKSize deviceSize, SKSize iconReference)
        {
            Random random = new Random();

            SKSize standardSize = GetSizeByGrid(deviceSize, 1f, 1f);

            int deltaX = random.Next((int)standardSize.Width * 2) - (int)standardSize.Width;
            int deltaY = random.Next((int)standardSize.Height * 2) - (int)standardSize.Height;

            return new SKPoint((deviceSize.Width / 2f) - (iconReference.Width/2f) - deltaX, 
                               (deviceSize.Height / 2f - (iconReference.Height / 2f) - deltaY));
        }

        public static SKSize GetSizeByGrid(SKSize deviceSize, float x, float y)
        {
            float widthNoBezel = (deviceSize.Width - Bezel * 2) / UnitWidth;
            float heightNoBezel = (deviceSize.Width - Bezel * 2) / UnitHeight;

            return new SKSize(widthNoBezel * x, heightNoBezel * y);
        }

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
