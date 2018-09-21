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

namespace FastTalkerSkiaSharp.Droid.Base
{
    /// <summary>
    /// Device admin receiver class.
    /// </summary>
    [Android.Content.BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", 
                                       Name = "com.smallnstats.FastTalkerSkiaSharp.Base.DeviceAdminReceiverClass")]
    [Android.App.MetaData("android.app.device_admin", Resource = "@xml/admin")]
    [Android.App.IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", Android.Content.Intent.ActionMain })]
    public class DeviceAdminReceiverClass : Android.App.Admin.DeviceAdminReceiver { }
}