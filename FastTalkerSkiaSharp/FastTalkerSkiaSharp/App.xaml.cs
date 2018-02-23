using Xamarin.Forms;
using System.Reflection;
using FastTalkerSkiaSharp.Storage;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Models;

namespace FastTalkerSkiaSharp
{
    public partial class App : Application
	{
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

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
