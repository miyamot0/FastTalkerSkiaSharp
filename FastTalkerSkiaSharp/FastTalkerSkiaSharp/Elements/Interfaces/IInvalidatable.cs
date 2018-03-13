// Based on SkiaSharp.Elements
// Felipe Nicoletto
// https://github.com/FelipeNicoletto/SkiaSharp.Elements

namespace SkiaSharp.Elements.Interfaces
{
    public interface IInvalidatable
    {
        void Invalidate();
        void SuspendLayout();
        void ResumeLayout(bool invalidate = false);
    }
}
