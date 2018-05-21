/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
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