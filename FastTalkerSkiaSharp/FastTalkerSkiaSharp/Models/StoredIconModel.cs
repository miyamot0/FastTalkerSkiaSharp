using System.Collections.Generic;

namespace FastTalkerSkiaSharp.Models
{
    public class StoredIconModel
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public int Mature { get; set; }

        public StoredIconModel() { }
    }
}
