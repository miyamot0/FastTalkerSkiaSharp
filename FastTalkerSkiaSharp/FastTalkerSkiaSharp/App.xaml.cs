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

namespace FastTalkerSkiaSharp
{
    public partial class App : Application
	{
        public static bool OutputVerbose = false;
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

        static Storage.CommunicationSettings _boardSettings;
        public static Storage.CommunicationSettings BoardSettings
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

        public static Helpers.UserInput UserInputInstance;
        public static Helpers.ImageBuilder ImageBuilderInstance;

        public static Models.StoredIconContainerModel storedIcons { get; set; }

        public static Pages.ModifyPage InstanceModificationPage;
        public static Pages.SettingsPage InstanceSettingsPage;
        public static Pages.StoredIconPopup InstanceStoredIconPage;

        public static ViewModels.SettingsPageViewModel InstanceSettingsPageViewModel;
        public static ViewModels.StoredIconPopupViewModel InstanceStoredIconsViewModel;

        public static NavigationPage BoardPage;

        public App()
        {
            InitializeComponent();

            MainAssembly = GetType().GetTypeInfo().Assembly;

            Database.Init();

            BoardPage = new NavigationPage(new Pages.CommunicationBoardPage());

			if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                HasAdmin = DependencyService.Get<Interfaces.InterfaceAdministration>().IsAdmin();

                if (HasAdmin)
                {
                    DependencyService.Get<Interfaces.InterfaceAdministration>().RequestAdmin(HasAdmin);
                }
            }

			if (Device.Idiom == TargetIdiom.Tablet)
			{
				App.Current.Resources["dynamicTextSize"] = Device.GetNamedSize(NamedSize.Large, typeof(Button));
				App.Current.Resources["dynamicFrameMargin"] = new Thickness(5, 25);
			}
            
            MainPage = new Pages.TitlePage();
        }

        protected override void OnStart () { }

		protected override void OnSleep () { }

		protected override void OnResume () { }
	}
}
