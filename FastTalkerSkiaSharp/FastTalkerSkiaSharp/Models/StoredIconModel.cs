/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

namespace FastTalkerSkiaSharp.Models
{
    public class StoredIconModel
    {
        /// <summary>
        /// Text to be spoken
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Tags for each image
        /// </summary>
        /// <value>The tags.</value>
        public System.Collections.Generic.List<string> Tags { get; set; }

        /// <summary>
        /// Is the icon mature?
        /// </summary>
        /// <value>The mature.</value>
        public int Mature { get; set; }
    }
}
