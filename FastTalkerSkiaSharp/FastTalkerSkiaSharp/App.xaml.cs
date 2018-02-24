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

using Xamarin.Forms;
using System.Reflection;
using FastTalkerSkiaSharp.Storage;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Models;

namespace FastTalkerSkiaSharp
{
    public partial class App : Application
	{
        public static bool OutputVerbose = true;

        public static float DisplayScreenWidth;
        public static float DisplayScreenHeight;
        public static float DisplayScaleFactor;

        static Storage.ApplicationDatabase database;
        public static Storage.ApplicationDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new Storage.ApplicationDatabase(DependencyService.Get<Interfaces.InterfaceSaveLoad>().GetDatabaseFilePath("database.db3"));
                }

                return database;
            }
        }

        static CommunicationSettings _boardSettings;
        public static CommunicationSettings BoardSettings
        {
            get
            {
                if (_boardSettings == null)
                {
                    _boardSettings = Database.GetSettingsAsync().Result;
                }

                return _boardSettings;
            }
            set
            {
                _boardSettings = value;
            }
        }

        public static Assembly MainAssembly;

        public static UserInput UserInputInstance;

        public static ImageBuilder ImageBuilderInstance;

        public static StoredIconContainerModel storedIcons { get; set; }

        public App()
        {
            InitializeComponent();

            MainAssembly = GetType().GetTypeInfo().Assembly;

            UserInputInstance = new UserInput();

            Database.Init();

            MainPage = new NavigationPage(new FastTalkerSkiaSharp.Pages.CommunicationBoardPage());
        }

        protected override void OnStart () { }

		protected override void OnSleep () { }

		protected override void OnResume () { }
	}
}
