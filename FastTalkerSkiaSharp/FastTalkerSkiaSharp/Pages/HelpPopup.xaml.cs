using System;
using System.Collections.Generic;
using FastTalkerSkiaSharp.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class HelpPopup : PopupPage
    {
        public HelpPopup()
        {
            InitializeComponent();

            BindingContext = new PopupUpViewModel()
            {
                Padding = new Thickness(100, 100, 100, 100),
                IsSystemPadding = true
            };
        }
    }
}
