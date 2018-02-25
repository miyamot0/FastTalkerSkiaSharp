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

using SkiaSharp.Elements;

namespace FastTalkerSkiaSharp.Constants
{
    public static class LanguageSettings
    {
        public const string ResourcePrefixPng = "FastTalkerSkiaSharp.Images.";
        public const string ResourceSuffixPng = ".png";

        public const string ResourcePrefixJson = "FastTalkerSkiaSharp.Data.StoredJson.json";

        public const string SettingsTitle = "Change setting or icons?";

        public const string SettingsClose = "Close";

        public const string SettingsResume = "Resume Child Operation";
        public const string SettingsServerStart = "Start Server";
        public const string SettingsSave = "Prompt Save";
        public const string SettingsDeselect = "Require Auto Deselect";
        public const string SettingsDeselectDisable = "Disable Auto Deselect";

        public const string SettingsMode = "Change to Framed Mode";
        public const string SettingsMode2 = "Change to Icon Mode";
        public const string SettingsAddIcon = "Add Icon";
        public const string SettingsTakePhoto = "Take a Photo";
        public const string SettingsAddFolder = "Add a Folder";

        public const string EditTextCancel = "Cancel";
        public const string EditTextOK = "OK";

        public static string[] SettingsMenu(ElementsController controller)
        {
            return new string[] {
                SettingsResume,
                //SettingsServerStart,
                SettingsSave,
                (controller.RequireDeselect) ? SettingsDeselectDisable : SettingsDeselect,
                (controller.InFramedMode) ? SettingsMode2 : SettingsMode,
                SettingsAddIcon,
                SettingsTakePhoto,
                SettingsAddFolder
            };
        }

        public const string EditTitle = "Edit Current Icon?";

        public const string EditClose = "Close";

        public const string EditShrink = "Shrink";
        public const string EditGrow = "Grow";
        public const string EditResetSize = "Scale Default";
        public const string EditShrink2 = "Shrink a Lot";
        public const string EditGrow2 = "Grow a Lot";
        public const string EditText = "Edit Text";
        public const string EditPinning = "Toggle Pinning";

        public static string[] EditMenu()
        {
            return new string[]
            {
                EditGrow2,
                EditGrow,
                EditResetSize,
                EditShrink,
                EditShrink2,
                EditText,
                EditPinning
            };
        }
    }
}
