/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using Xamarin.Forms;
using System.Reflection;
using FastTalkerSkiaSharp.Storage;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Models;
using FastTalkerSkiaSharp.Interfaces;
using FastTalkerSkiaSharp.Pages;
using FastTalkerSkiaSharp.ViewModels;

namespace FastTalkerSkiaSharp
{
    public partial class App : Application
	{
        public static bool OutputVerbose = true;

        public static bool HasAdmin = false;

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

		public static ModifyPage InstanceModificationPage = null;

		public static SettingsPageViewModel InstanceSettingsPageViewModel;
		public static SettingsPage InstanceSettingsPage;

		public static StoredIconPopupViewModel InstanceStoredIconsViewModel;
		public static StoredIconPopup InstanceStoredIconPage;

        public static NavigationPage BoardPage;

        public App()
        {
            InitializeComponent();

            MainAssembly = GetType().GetTypeInfo().Assembly;

            Database.Init();

            BoardPage = new NavigationPage(new FastTalkerSkiaSharp.Pages.CommunicationBoardPage());

            if (Device.RuntimePlatform == Device.Android)
            {
                HasAdmin = DependencyService.Get<InterfaceAdministration>().IsAdmin();
                DependencyService.Get<InterfaceAdministration>().RequestAdmin(HasAdmin);
            }

			if (Device.Idiom == TargetIdiom.Tablet)
			{
				App.Current.Resources["dynamicTextSize"] = Device.GetNamedSize(NamedSize.Large, typeof(Button));
				App.Current.Resources["dynamicFrameMargin"] = new Thickness(5, 25);
			}
            
			MainPage = new NavigationPage(new TitlePage());
        }

        protected override void OnStart () { }

		protected override void OnSleep () { }

		protected override void OnResume () { }
	}
}
