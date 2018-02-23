﻿using SkiaSharp;
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

        public static SKPoint GetCenterPointWithJitter(SKSize deviceSize)
        {
            var random = new Random();

            int deltaX = random.Next((int)deviceSize.Width / 10) - (int)deviceSize.Width / 5;
            int deltaY = random.Next((int)deviceSize.Height / 10) - (int)deviceSize.Height / 5;

            return new SKPoint(deviceSize.Width / 2f - deltaX, deviceSize.Height / 2f - deltaY);
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