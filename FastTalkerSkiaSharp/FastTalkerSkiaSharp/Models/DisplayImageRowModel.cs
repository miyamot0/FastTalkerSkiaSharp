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

using System.Windows.Input;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Models
{
    public class DisplayImageRowModel
    {
        /// <summary>
        /// Column 1
        /// </summary>
        /// <value>The name1.</value>
        public string Name1 { get; set; }
        public ImageSource Image1 { get; set; }
		public int Rotation1 { get; set; }

        /// <summary>
        /// Column 2
        /// </summary>
        /// <value>The name2.</value>
        public string Name2 { get; set; }
		public ImageSource Image2 { get; set; }
        public int Rotation2 { get; set; }

        /// <summary>
        /// Column 3
        /// </summary>
        /// <value>The name3.</value>
        public string Name3 { get; set; }
		public ImageSource Image3 { get; set; }
        public int Rotation3 { get; set; }

        /// <summary>
        /// Width request, for scaling
        /// </summary>
        /// <value>The width request.</value>
        public double WidthRequest { get; set; }

        /// <summary>
        /// Tapped command, one per row (with ID)
        /// </summary>
        /// <value>The tapped command.</value>
        public ICommand TappedCommand { get; set; }
    }
}
