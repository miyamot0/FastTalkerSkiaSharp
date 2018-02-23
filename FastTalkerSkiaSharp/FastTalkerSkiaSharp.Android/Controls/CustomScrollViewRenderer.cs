using FastTalkerSkiaSharp.Controls;
using FastTalkerSkiaSharp.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomScrollView), typeof(CustomScrollViewRenderer))]
namespace FastTalkerSkiaSharp.Droid.Controls
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class CustomScrollViewRenderer : ScrollViewRenderer
#pragma warning restore CS0618 // Type or member is obsolete
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement as FastTalkerSkiaSharp.Controls.CustomScrollView;
            element?.Render();
        }
    }
}