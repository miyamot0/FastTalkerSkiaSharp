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

namespace FastTalkerSkiaSharp.Storage
{
    /// <summary>
    /// SQLite Model
    /// </summary>
    public class CommunicationSettings
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// Is the board in edit mode?
        /// </summary>
        /// <value><c>true</c> if in edit mode; otherwise, <c>false</c>.</value>
        public bool InEditMode { get; set; }

        /// <summary>
        /// Is the board in sentence frame mode?
        /// </summary>
        /// <value><c>true</c> if in framed mode; otherwise, <c>false</c>.</value>
        public bool InFramedMode { get; set; }

        /// <summary>
        /// Is auto de-select activated?
        /// </summary>
        /// <value><c>true</c> if require deselect; otherwise, <c>false</c>.</value>
        public bool RequireDeselect { get; set; }

        /// <summary>
        /// Is the icon automatically outputting
        /// </summary>
        /// <value><c>true</c> if in icon mode auto; otherwise, <c>false</c>.</value>
        public bool InIconModeAuto { get; set; }

        /// <summary>
        /// Is the board top oriented, in strip mode?
        /// </summary>
        /// <value><c>true</c> if is top oriented; otherwise, <c>false</c>.</value>
        public bool IsBottomOriented { get; set; }
    }
}
