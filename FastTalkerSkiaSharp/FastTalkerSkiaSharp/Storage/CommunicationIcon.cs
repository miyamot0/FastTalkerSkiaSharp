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
    public class CommunicationIcon
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// Spoken text
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        public float X { get; set; }
        public float Y { get; set; }

        /// <summary>
        /// Icon type
        /// </summary>
        /// <value>The tag.</value>
        public int Tag { get; set; }

        /// <summary>
        /// Is icon embedded?
        /// </summary>
        /// <value><c>true</c> if local; otherwise, <c>false</c>.</value>
        public bool Local { get; set; }

        /// <summary>
        /// Is text visible?
        /// </summary>
        /// <value><c>true</c> if text visible; otherwise, <c>false</c>.</value>
        public bool TextVisible { get; set; }

        /// <summary>
        /// Is the icon in a folder?
        /// </summary>
        /// <value><c>true</c> if is stored in folder; otherwise, <c>false</c>.</value>
        public bool IsStoredInFolder { get; set; }

        /// <summary>
        /// Is icon pinned in place?
        /// </summary>
        /// <value><c>true</c> if is pinned; otherwise, <c>false</c>.</value>
        public bool IsPinned { get; set; }

        /// <summary>
        /// Base64 encoded image
        /// </summary>
        /// <value>The base64.</value>
        public string Base64 { get; set; }

        /// <summary>
        /// ResourceName in embedded resources
        /// </summary>
        /// <value>The resource location.</value>
        public string ResourceLocation { get; set; }

        /// <summary>
        /// If in a folder, name of the folder?
        /// </summary>
        /// <value>The folder containing icon.</value>
        public string FolderContainingIcon { get; set; }

        /// <summary>
        /// Scale of the image
        /// </summary>
        /// <value>The scale.</value>
        public float Scale { get; set; }

        /// <summary>
        /// Scale of the text
        /// </summary>
        /// <value>The text scale.</value>
        public float TextScale { get; set; }

        /// <summary>
        /// Hashcode, for quick indexing
        /// </summary>
        /// <value>The hash code.</value>
        public int HashCode { get; set; }
    }
}