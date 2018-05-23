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
