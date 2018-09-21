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

using Xamarin.Forms;
using System.Reflection;

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

            // Lock if administrator
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                HasAdmin = DependencyService.Get<Interfaces.InterfaceAdministration>().IsAdmin();

                if (HasAdmin)
                {
                    DependencyService.Get<Interfaces.InterfaceAdministration>().RequestAdmin(HasAdmin);
                }
            }

            // Dynamically shape UI
            if (Device.RuntimePlatform == Device.iOS)
            {
                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    Application.Current.Resources["dynamicTextSize"] = Device.GetNamedSize(NamedSize.Large, typeof(Button));
                    Application.Current.Resources["dynamicFrameMargin"] = new Thickness(5, 25);
                }                
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                int size = DependencyService.Get<Interfaces.InterfaceScreenSize>().GetSizeIdentifier();

                switch (size)
                {
                    case 1:
                        Application.Current.Resources["dynamicTextSize"] = Device.GetNamedSize(NamedSize.Small, typeof(Button));
                        Application.Current.Resources["dynamicFrameMargin"] = new Thickness(5, 5);

                        break;

                    case 2:
                        Application.Current.Resources["dynamicTextSize"] = Device.GetNamedSize(NamedSize.Small, typeof(Button));
                        Application.Current.Resources["dynamicFrameMargin"] = new Thickness(5, 15);

                        break;

                    case 3:
                        Application.Current.Resources["dynamicTextSize"] = Device.GetNamedSize(NamedSize.Small, typeof(Button));
                        Application.Current.Resources["dynamicFrameMargin"] = new Thickness(5, 15);

                        break;

                    case 4:
                        Application.Current.Resources["dynamicTextSize"] = Device.GetNamedSize(NamedSize.Large, typeof(Button));
                        Application.Current.Resources["dynamicFrameMargin"] = new Thickness(5, 25);

                        break;
                }

            }

            MainPage = new Pages.TitlePage();
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
