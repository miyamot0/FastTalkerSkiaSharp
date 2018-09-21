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

[assembly: Xamarin.Forms.Dependency(typeof(FastTalkerSkiaSharp.Droid.Implementations.ImplementationBitmapResize))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
    class ImplementationBitmapResize : FastTalkerSkiaSharp.Interfaces.InterfaceBitmapResize
    {
        /// <summary>
        /// Resize image bitmap
        /// </summary>
        /// <param name="photoPath"></param>
        /// <param name="newPhotoPath"></param>
        public void ResizeBitmaps(string photoPath, string newPhotoPath)
        {
            Android.Graphics.BitmapFactory.Options options = new Android.Graphics.BitmapFactory.Options();
            options.InPreferredConfig = Android.Graphics.Bitmap.Config.Argb8888;
            Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeFile(photoPath, options);
            Android.Graphics.Bitmap croppedBitmap = null;

            if (bitmap.Width >= bitmap.Height)
            {
                croppedBitmap = Android.Graphics.Bitmap.CreateBitmap(
                   bitmap,
                   bitmap.Width / 2 - bitmap.Height / 2,
                   0,
                   bitmap.Height,
                   bitmap.Height);
            }
            else
            {
                croppedBitmap = Android.Graphics.Bitmap.CreateBitmap(
                   bitmap,
                   0,
                   bitmap.Height / 2 - bitmap.Width / 2,
                   bitmap.Width,
                   bitmap.Width);
            }

            System.IO.FileStream stream = null;

            try
            {
                stream = new System.IO.FileStream(newPhotoPath, System.IO.FileMode.Create);
                croppedBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);
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
            Android.Graphics.BitmapFactory.Options options = new Android.Graphics.BitmapFactory.Options();
            options.InPreferredConfig = Android.Graphics.Bitmap.Config.Argb8888;
            Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeFile(photoPath, options);

            try
            {
                Android.Media.ExifInterface exifInterface = new Android.Media.ExifInterface(photoPath);
                int orientation = exifInterface.GetAttributeInt(Android.Media.ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

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

                using (var ms = new System.IO.MemoryStream())
                {
                    Android.Graphics.Bitmap croppedBitmap = null;

                    Android.Graphics.Matrix mtx = new Android.Graphics.Matrix();
                    mtx.PreRotate(rotate);

                    if (bitmap.Width >= bitmap.Height)
                    {
                        croppedBitmap = Android.Graphics.Bitmap.CreateBitmap(
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
                        croppedBitmap = Android.Graphics.Bitmap.CreateBitmap(
                           bitmap,
                           0,
                           bitmap.Height / 2 - bitmap.Width / 2,
                           bitmap.Width,
                           bitmap.Width,
                           mtx,
                           false);
                    }

                    croppedBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);

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