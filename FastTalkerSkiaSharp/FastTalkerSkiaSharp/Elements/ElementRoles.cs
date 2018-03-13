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
