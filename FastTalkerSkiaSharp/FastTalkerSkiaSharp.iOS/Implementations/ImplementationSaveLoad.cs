/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

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