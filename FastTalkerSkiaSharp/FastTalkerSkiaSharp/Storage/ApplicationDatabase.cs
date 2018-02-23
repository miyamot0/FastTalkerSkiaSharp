﻿using System.Threading.Tasks;
using System.Linq;

namespace FastTalkerSkiaSharp.Storage
{
    public class ApplicationDatabase
    {
        private static SQLite.SQLiteAsyncConnection database;

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
        public async Task<int> InsertOrUpdateAsync(CommunicationSettings item)
        {
            return await database.InsertOrReplaceAsync(item);
        }

        /// <summary>
        /// Get the single settings object
        /// </summary>
        /// <returns>The settings async.</returns>
        public async Task<CommunicationSettings> GetSettingsAsync()
        {
            var settings = await database.Table<CommunicationSettings>()?.ToListAsync();

            if (settings != null && settings.Count == 1)
            {
                return settings.First();
            }
            else
            {
                return new CommunicationSettings()
                {
                    InEditMode = false,
                    InFramedMode = false,
                    RequireDeselect = false,
                };
            }
        }

        /// <summary>
        /// Inserts the or update icons
        /// </summary>
        /// <returns>The or update async.</returns>
        /// <param name="item">Item.</param>
        public async Task<int> InsertOrUpdateAsync(CommunicationIcon item)
        {
            return await database.InsertOrReplaceAsync(item);
        }

        public async Task<int> InsertOrUpdateAsync(System.Collections.Generic.List<CommunicationIcon> items)
        {
            System.Diagnostics.Debug.WriteLine("Fired insert all");

            database.DropTableAsync<CommunicationIcon>().Wait();
            database.CreateTableAsync<CommunicationIcon>().Wait();

            return await database?.InsertAllAsync(items);
        }

        /// <summary>
        /// Gets the icons async
        /// </summary>
        /// <returns>The icons async.</returns>
        /// <param name="instance">Instance.</param>
        public async Task<System.Collections.Generic.List<CommunicationIcon>> GetIconsAsync()
        {
            var icons = await database.Table<CommunicationIcon>().ToListAsync();

            if (icons != null)
            {
                return icons;

            }
            else
            {
                return new System.Collections.Generic.List<CommunicationIcon>();
            }
        }
    }
}