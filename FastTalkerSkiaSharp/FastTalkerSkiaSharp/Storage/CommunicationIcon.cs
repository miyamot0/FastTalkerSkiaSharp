/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   Fast Talker is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, version 3.

   Fast Talker is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
   </copyright>

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

namespace FastTalkerSkiaSharp.Storage
{
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