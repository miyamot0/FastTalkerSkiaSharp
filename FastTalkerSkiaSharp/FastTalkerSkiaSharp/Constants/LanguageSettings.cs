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

using FastTalkerSkiaSharp.Controls;

namespace FastTalkerSkiaSharp.Constants
{
    /// <summary>
    /// This class is largely deprecated, in favor of pop-up windows over lists
    /// </summary>
    public static class LanguageSettings
    {
        public const string ResourcePrefixPng = "FastTalkerSkiaSharp.Images.";
        public const string ResourceSuffixPng = ".png";

        public const string ResourcePrefixJson = "FastTalkerSkiaSharp.Data.StoredJson.json";

        public const string SettingsTitle = "Change setting or icons?";

        public const string SettingsClose = "Close";

        public const string SettingsAbout = "About";
        public const string SettingsHelp = "Help";

        public const string SettingsResume = "Resume User Operation";
        public const string SettingsServerStart = "Start Server";
        public const string SettingsSave = "Prompt Save";
        public const string SettingsDeselect = "Require Auto Deselect";
        public const string SettingsDeselectDisable = "Disable Auto Deselect";

        public const string SettingsModeQuery = "Change Mode";
        public const string SettingsFramedMode = "Change to Framed Mode";
        public const string SettingsIconManual = "Change to Icon Mode (Manual Speaking)";
        public const string SettingsIconAuto = "Change to Icon Mode (Auto Speaking)";
        public const string SettingsAddIcon = "Add Icon";
        public const string SettingsTakePhoto = "Take a Photo";
        public const string SettingsAddFolder = "Add a Folder";

        public const string EditTextCancel = "Cancel";
        public const string EditTextOK = "OK";

        /// <summary>
        /// DEPRECATED: settings option when originally as actionlist
        /// </summary>
        /// <returns>The menu.</returns>
        /// <param name="controller">Controller.</param>
        public static string[] SettingsMenu(FieldControl controller)
        {
            return new string[] {
                SettingsResume,
                //SettingsServerStart,
                SettingsSave,
                (controller.RequireDeselect) ? SettingsDeselectDisable : SettingsDeselect,
                SettingsModeQuery,
                SettingsAddIcon,
                SettingsTakePhoto,
                SettingsAddFolder,
                SettingsHelp,
                SettingsAbout
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

        /// <summary>
        /// Gets the speech output for icon editing
        /// </summary>
        /// <returns>The menu.</returns>
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

        /// <summary>
        /// Get the speech output modes for settings menu
        /// </summary>
        /// <returns>The speech output modes.</returns>
        public static string[] GetSpeechOutputModes()
        {
            return new string[] {
                SettingsIconManual,
                SettingsIconAuto,
                SettingsFramedMode
            };
        }
    }
}
