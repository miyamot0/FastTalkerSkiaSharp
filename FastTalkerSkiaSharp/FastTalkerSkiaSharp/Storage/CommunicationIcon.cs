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

        public string Text { get; set; }

        public float X { get; set; }
        public float Y { get; set; }

        public int Tag { get; set; }

        public bool Local { get; set; }
        public bool TextVisible { get; set; }
        public bool IsStoredInFolder { get; set; }
        public bool IsPinned { get; set; }

        public string Base64 { get; set; }
        public string ResourceLocation { get; set; }
        public string FolderContainingIcon { get; set; }

        public float Scale { get; set; }
        public float TextScale { get; set; }

        public int HashCode { get; set; }
    }
}