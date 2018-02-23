using SkiaSharp.Elements;

namespace FastTalkerSkiaSharp.Constants
{
    public static class LanguageSettings
    {
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

        public static string[] SettingsMenu(ElementsController controller)
        {
            return new string[] {
                SettingsResume,
                SettingsServerStart,
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
    }
}
