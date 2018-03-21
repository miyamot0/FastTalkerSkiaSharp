using Rg.Plugins.Popup.Pages;
using FastTalkerSkiaSharp.ViewModels;
using SkiaSharp.Elements;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class SettingsPage : PopupPage
    {
        public SettingsPage(ElementsController _controllerReference, SettingsPageViewModel _viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel;
        }
    }
}
