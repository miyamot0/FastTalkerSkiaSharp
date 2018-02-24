namespace FastTalkerSkiaSharp.Interfaces
{
    public interface InterfaceBitmapResize
    {
        void ResizeBitmaps(string photoPath, string newPhotoPath);

        byte[] RotateImage(string photoPath);
    }
}
