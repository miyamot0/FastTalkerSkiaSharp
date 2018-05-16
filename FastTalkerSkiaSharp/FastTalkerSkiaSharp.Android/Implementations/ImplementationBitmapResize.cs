/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using FastTalkerSkiaSharp.Droid.Implementations;
using FastTalkerSkiaSharp.Interfaces;
using Xamarin.Forms;

using Android.Graphics;
using System.IO;
using Android.Media;

[assembly: Dependency(typeof(ImplementationBitmapResize))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
    class ImplementationBitmapResize : InterfaceBitmapResize
    {
        /// <summary>
        /// Resize image bitmap
        /// </summary>
        /// <param name="photoPath"></param>
        /// <param name="newPhotoPath"></param>
        public void ResizeBitmaps(string photoPath, string newPhotoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPath, options);
            Bitmap croppedBitmap = null;

            if (bitmap.Width >= bitmap.Height)
            {
                croppedBitmap = Bitmap.CreateBitmap(
                   bitmap,
                   bitmap.Width / 2 - bitmap.Height / 2,
                   0,
                   bitmap.Height,
                   bitmap.Height);
            }
            else
            {
                croppedBitmap = Bitmap.CreateBitmap(
                   bitmap,
                   0,
                   bitmap.Height / 2 - bitmap.Width / 2,
                   bitmap.Width,
                   bitmap.Width);
            }

            FileStream stream = null;

            try
            {
                stream = new FileStream(newPhotoPath, FileMode.Create);
                croppedBitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            }
            catch
            {
                //System.Diagnostics.Debug.WriteLineIf(App.Debugging, "Failed to close: " + ex.ToString());
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }

                    croppedBitmap.Recycle();
                    croppedBitmap.Dispose();
                    croppedBitmap = null;

                    bitmap.Recycle();
                    bitmap.Dispose();
                    bitmap = null;
                }
                catch 
                {
                    //Debug.WriteLineIf(App.Debugging, "Failed to close: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Rotate via EXIF information
        /// </summary>
        /// <param name="photoPath"></param>
        /// <returns></returns>
        public byte[] RotateImage(string photoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPath, options);

            try
            {
                ExifInterface exifInterface = new ExifInterface(photoPath);
                int orientation = exifInterface.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

                int rotate = 0;

                switch (orientation)
                {
                    case (int)Android.Media.Orientation.Normal:
                        rotate = 0;
                        break;

                    case (int)Android.Media.Orientation.Rotate90:
                        rotate = 90;
                        break;

                    case (int)Android.Media.Orientation.Rotate270:
                        rotate = 270;
                        break;

                    case (int)Android.Media.Orientation.Rotate180:
                        rotate = 180;
                        break;

                    default:
                        rotate = 0;
                        break;
                }

                using (var ms = new MemoryStream())
                {
                    Bitmap croppedBitmap = null;

                    Matrix mtx = new Matrix();
                    mtx.PreRotate(rotate);

                    if (bitmap.Width >= bitmap.Height)
                    {
                        croppedBitmap = Bitmap.CreateBitmap(
                           bitmap,
                           bitmap.Width / 2 - bitmap.Height / 2,
                           0,
                           bitmap.Height,
                           bitmap.Height,
                           mtx,
                           false);
                    }
                    else
                    {
                        croppedBitmap = Bitmap.CreateBitmap(
                           bitmap,
                           0,
                           bitmap.Height / 2 - bitmap.Width / 2,
                           bitmap.Width,
                           bitmap.Width,
                           mtx,
                           false);
                    }

                    croppedBitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);

                    croppedBitmap.Recycle();
                    croppedBitmap.Dispose();
                    croppedBitmap = null;

                    mtx.Dispose();
                    mtx = null;

                    bitmap.Recycle();
                    bitmap.Dispose();
                    bitmap = null;

                    return ms.ToArray();
                }
            }
            catch
            {
                // <!-- Fail out -->
            }

            return null;
        }
    }
}