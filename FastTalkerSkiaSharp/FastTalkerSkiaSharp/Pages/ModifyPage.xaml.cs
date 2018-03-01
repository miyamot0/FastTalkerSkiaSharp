using System;
using System.Collections.Generic;
using FastTalkerSkiaSharp.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class ModifyPage : PopupPage
    {
        public ModifyPage(SkiaSharp.Elements.Element _currentElement, SkiaSharp.Elements.ElementsController _controller)
        {
            InitializeComponent();

            BindingContext = new ModifyIconViewModel(_currentElement, _controller)
            {
                Padding = new Thickness(100, 100, 100, 100),
                IsSystemPadding = true
            };
        }
    }
}
