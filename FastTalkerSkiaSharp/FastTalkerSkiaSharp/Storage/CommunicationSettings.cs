namespace FastTalkerSkiaSharp.Storage
{
    public class CommunicationSettings
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ID { get; set; }

        public bool InEditMode { get; set; }

        public bool InFramedMode { get; set; }

        public bool RequireDeselect { get; set; }

    }
}
