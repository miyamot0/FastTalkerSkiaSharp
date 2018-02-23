using System.Windows.Input;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Models
{
    public class DisplayImageRowModel
    {
        public string Name1 { get; set; }
        public ImageSource Image1 { get; set; }

        public string Name2 { get; set; }
        public ImageSource Image2 { get; set; }

        public string Name3 { get; set; }
        public ImageSource Image3 { get; set; }

        public double WidthRequest { get; set; }

        public ICommand TappedCommand { get; set; }
    }
}
