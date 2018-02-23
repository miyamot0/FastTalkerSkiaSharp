using Xamarin.Forms;

[assembly: Dependency(typeof(FastTalkerSkiaSharp.Droid.Implementations.ImplementationSaveLoad))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
    public class ImplementationSaveLoad : Interfaces.InterfaceSaveLoad
    {
        public string GetDatabaseFilePath(string dbName)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return System.IO.Path.Combine(documentsPath, dbName);
        }
    }
}
