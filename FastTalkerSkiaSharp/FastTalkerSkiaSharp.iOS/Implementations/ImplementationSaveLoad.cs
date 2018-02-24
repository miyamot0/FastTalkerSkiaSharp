/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   Fast Talker is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, version 3.

   Fast Talker is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
   </copyright>

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