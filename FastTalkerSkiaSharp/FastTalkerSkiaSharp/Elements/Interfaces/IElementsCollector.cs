// Based on SkiaSharp.Elements
// Felipe Nicoletto
// https://github.com/FelipeNicoletto/SkiaSharp.Elements

using SkiaSharp.Elements.Collections;

namespace SkiaSharp.Elements.Interfaces
{
    public interface IElementsCollector : IElementContainer
    {
        ElementsCollection Elements { get; }
    }
}
