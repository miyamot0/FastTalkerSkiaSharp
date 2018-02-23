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

        public string Base64 { get; set; }
        public string ResourceLocation { get; set; }
        public string FolderContainingIcon { get; set; }

        public float Scale { get; set; }
        public float TextScale { get; set; }

        public int HashCode { get; set; }
    }
}