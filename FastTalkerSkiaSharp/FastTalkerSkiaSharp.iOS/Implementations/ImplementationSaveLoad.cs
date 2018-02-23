using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(FastTalkerSkiaSharp.iOS.Implementations.ImplementationSaveLoad))]
namespace FastTalkerSkiaSharp.iOS.Implementations
{
    public class ImplementationSaveLoad : FastTalkerSkiaSharp.Interfaces.InterfaceSaveLoad
    {
        /// <summary>
        /// Gets the database file path.
        /// </summary>
        /// <returns>The database file path.</returns>
        /// <param name="dbName">Db name.</param>
        public string GetDatabaseFilePath(string dbName)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            return System.IO.Path.Combine(docFolder, dbName);
        }
    }
}