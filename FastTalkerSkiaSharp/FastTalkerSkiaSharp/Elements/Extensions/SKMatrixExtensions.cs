/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu

   =========================================================================================================
   
   Based on SkiaSharp.Elements
   Felipe Nicoletto
   https://github.com/FelipeNicoletto/SkiaSharp.Elements

*/

namespace SkiaSharp.Elements.Extensions
{
    public static class SKMatrixExtensions
    {
        public static SKMatrix Rotate(this SKMatrix m, float angle)
        {
            return m.Concat(SKMatrix.MakeRotation(angle));
        }

        public static SKMatrix RotateDegrees(this SKMatrix m, float andegrees)
        {
            return m.Concat(SKMatrix.MakeRotationDegrees(andegrees));
        }

        public static SKMatrix Scale(this SKMatrix m, float sx, float sy)
        {
            return m.Concat(SKMatrix.MakeScale(sx, sy));
        }

        public static SKMatrix Translate(this SKMatrix m, float dx, float dy)
        {
            return m.Concat(SKMatrix.MakeTranslation(dx, dy));
        }

        public static SKMatrix Invert(this SKMatrix m)
        {
            m.TryInvert(out var m2);
            return m2;
        }

        public static SKMatrix Concat(this SKMatrix m, SKMatrix m2)
        {
            var target = m;
            SKMatrix.PreConcat(ref target, m2);
            return target;
        }
    }
}
