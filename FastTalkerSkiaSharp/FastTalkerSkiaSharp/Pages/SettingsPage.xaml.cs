using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using FastTalkerSkiaSharp.ViewModels;
using System.Diagnostics;
using SkiaSharp.Elements;
using Rg.Plugins.Popup.Extensions;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Storage;
using System.Linq;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Acr.UserDialogs;
using System.IO;
using FastTalkerSkiaSharp.Interfaces;
using Plugin.Media.Abstractions;

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
