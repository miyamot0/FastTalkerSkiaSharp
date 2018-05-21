/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
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
        /// <param name="instance">Instance.</param>
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
