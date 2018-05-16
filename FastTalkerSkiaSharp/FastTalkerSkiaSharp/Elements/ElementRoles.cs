/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

namespace FastTalkerSkiaSharp.Elements
{
    static class ElementRoles
    {
        public enum Role
        {
            Control,        // Fixed, user behavior
            Communication,  // Dynamic, based on settings
            Display,        // Aesthetic, for decoration
            Emitter,        // For speech
            SentenceFrame,  // For framed speech
            Settings,       // Access settings
            Folder          // Folder icon
        }

        public static int GetRoleInt(Role role)
        {
            return (int)role;
        }
    }
}
