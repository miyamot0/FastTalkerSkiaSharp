/* 
    The MIT License

    Copyright February 8, 2016 Shawn Gilroy. http://www.smallnstats.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System.Linq;

namespace FastTalkerSkiaSharp.Storage
{
    public class ApplicationDatabase
    {
        static SQLite.SQLiteAsyncConnection database;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbFilePath"></param>
        public ApplicationDatabase(string dbFilePath)
        {
            database = new SQLite.SQLiteAsyncConnection(dbFilePath);
            database.CreateTableAsync<CommunicationSettings>().Wait();
            database.CreateTableAsync<CommunicationIcon>().Wait();
        }

        /// <summary>
        /// Force initialization
        /// </summary>
        public void Init() { }

        /// <summary>
        /// Insert or update settings
        /// </summary>
        /// <returns>The or update async.</returns>
        /// <param name="item">Item.</param>
        public async System.Threading.Tasks.Task<int> InsertOrUpdateAsync(CommunicationSettings item)
        {
            return await database.InsertOrReplaceAsync(item);
        }

        /// <summary>
        /// Get the single settings object
        /// </summary>
        /// <returns>The settings async.</returns>
        public async System.Threading.Tasks.Task<CommunicationSettings> GetSettingsAsync()
        {
            var settings = await database.Table<CommunicationSettings>()?.ToListAsync();

            if (settings != null && settings.Count == 1)
            {
                return settings.First();
            }

            return new CommunicationSettings()
            {
                InEditMode = false,
                InFramedMode = false,
                RequireDeselect = false,
                InIconModeAuto = false,
                IsBottomOriented = false
            };
        }

        /// <summary>
        /// Inserts the or update icons
        /// </summary>
        /// <returns>The or update async.</returns>
        /// <param name="item">Item.</param>
        public async System.Threading.Tasks.Task<int> InsertOrUpdateAsync(CommunicationIcon item)
        {
            return await database.InsertOrReplaceAsync(item);
        }

        /// <summary>
        /// Inserts items, as lists
        /// </summary>
        /// <returns>The or update async.</returns>
        /// <param name="items">Items.</param>
        public async System.Threading.Tasks.Task<int> InsertOrUpdateAsync(System.Collections.Generic.List<CommunicationIcon> items)
        {
            database.DropTableAsync<CommunicationIcon>().Wait();
            database.CreateTableAsync<CommunicationIcon>().Wait();

            return await database?.InsertAllAsync(items);
        }

        /// <summary>
        /// Gets the icons async
        /// </summary>
        /// <returns>The icons async.</returns>
        public async System.Threading.Tasks.Task<System.Collections.Generic.List<CommunicationIcon>> GetIconsAsync()
        {
            var icons = await database.Table<CommunicationIcon>().ToListAsync();

            if (icons != null)
            {
                return icons;
            }

            return new System.Collections.Generic.List<CommunicationIcon>();
        }
    }
}
