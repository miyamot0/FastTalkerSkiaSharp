﻿/* 
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

using FastTalkerSkiaSharp.Controls;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class ModifyPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public ModifyPage(Icon _currentIcon, FieldControl _controller)
        {
            InitializeComponent();

            BindingContext = new ViewModels.ModifyIconViewModel(_currentIcon, _controller)
            {
                Padding = new Xamarin.Forms.Thickness(50, 50, 50, 50),
                IsSystemPadding = false
            };
        }

        public void UpdateCurrentIcon(Icon _currentIcon)
        {
            ((ViewModels.ModifyIconViewModel)BindingContext).UpdateIcon(_currentIcon);
        }
    }
}
