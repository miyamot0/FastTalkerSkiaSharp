﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using FastTalkerSkiaSharp.Models;
using System.Diagnostics;
using FastTalkerSkiaSharp.Helpers;

namespace FastTalkerSkiaSharp.Pages
{
    public partial class StoredIconPopup : PopupPage
    {
        public event Action<ArgsSelectedIcon> IconSelected = delegate { };

        private DataTemplate customDataTemplate;
        private List<DisplayImageRowModel> rows;

        private string FolderWithIcons;
        private List<SkiaSharp.Elements.Element> itemsMatching;

        public StoredIconPopup(string _folder, List<SkiaSharp.Elements.Element> itemsMatching)
        {
            InitializeComponent();

            this.FolderWithIcons = _folder;
            this.itemsMatching = itemsMatching;

            rows = new List<DisplayImageRowModel>();

            BindingContext = new PopupUpViewModel()
            {
                Padding = new Thickness(100, 100, 100, 100),
                IsSystemPadding = true
            };
        }

        /// <summary>
        /// Ons the appearing.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            var recommendedWidth = this.Width / 4;

            int totalCount = itemsMatching.Count;

            string tempName1, tempName2, tempName3;
            ImageSource source1, source2, source3;

            int i = 0, j = 0;

            for (i = 0; (j + i) < totalCount;)
            {
                tempName1 = tempName2 = tempName3 = null;
                source1 = source2 = source3 = null;

                Debug.WriteLine(itemsMatching[i].ImageInformation);

                for (j = 0; j < 3 && (j + i) < totalCount; j++)
                {
                    if (j == 0)
                    {
                        tempName1 = itemsMatching[j + i].Text;
                        source1 = ImageSource.FromResource(itemsMatching[j + i].ImageInformation);
                    }
                    else if (j == 1)
                    {
                        tempName2 = itemsMatching[j + i].Text;
                        source2 = ImageSource.FromResource(itemsMatching[j + i].ImageInformation);
                    }
                    else if (j == 2)
                    {
                        tempName3 = itemsMatching[j + i].Text;
                        source3 = ImageSource.FromResource(itemsMatching[j + i].ImageInformation);
                    }
                }

                j = 0;

                rows.Add(CreateModel(tempName1, source1, tempName2, source2, tempName3, source3, recommendedWidth));

                i += 3;
            }

            customDataTemplate = new DataTemplate(() =>
            {
                Grid grid = new Grid();
                grid.GestureRecognizers.Clear();

                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Xamarin.Forms.Image image1 = new Xamarin.Forms.Image();
                image1.Aspect = Aspect.AspectFit;
                image1.SetBinding(Xamarin.Forms.Image.HeightRequestProperty, "WidthRequest");
                image1.SetBinding(Xamarin.Forms.Image.SourceProperty, "Image1");

                TapGestureRecognizer tapGestureRecognizer1 = new TapGestureRecognizer();
                tapGestureRecognizer1.SetBinding(TapGestureRecognizer.CommandProperty, "TappedCommand");
                tapGestureRecognizer1.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Name1");

                image1.GestureRecognizers.Add(tapGestureRecognizer1);

                Label nameLabel1 = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                nameLabel1.SetBinding(Label.TextProperty, "Name1");

                Xamarin.Forms.Image image2 = new Xamarin.Forms.Image();
                image2.Aspect = Aspect.AspectFit;
                image2.SetBinding(Xamarin.Forms.Image.HeightRequestProperty, "WidthRequest");
                image2.SetBinding(Xamarin.Forms.Image.SourceProperty, "Image2");

                TapGestureRecognizer tapGestureRecognizer2 = new TapGestureRecognizer();
                tapGestureRecognizer2.SetBinding(TapGestureRecognizer.CommandProperty, "TappedCommand");
                tapGestureRecognizer2.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Name2");

                image2.GestureRecognizers.Add(tapGestureRecognizer2);

                Label nameLabel2 = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                nameLabel2.SetBinding(Label.TextProperty, "Name2");

                Xamarin.Forms.Image image3 = new Xamarin.Forms.Image();
                image3.Aspect = Aspect.AspectFit;
                image3.SetBinding(Xamarin.Forms.Image.HeightRequestProperty, "WidthRequest");
                image3.SetBinding(Xamarin.Forms.Image.SourceProperty, "Image3");


                TapGestureRecognizer tapGestureRecognizer3 = new TapGestureRecognizer();
                tapGestureRecognizer3.SetBinding(TapGestureRecognizer.CommandProperty, "TappedCommand");
                tapGestureRecognizer3.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Name3");

                image3.GestureRecognizers.Add(tapGestureRecognizer3);

                Label nameLabel3 = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                nameLabel3.SetBinding(Label.TextProperty, "Name3");

                grid.Children.Add(image1, 0, 0);
                grid.Children.Add(image2, 1, 0);
                grid.Children.Add(image3, 2, 0);

                grid.Children.Add(nameLabel1, 0, 1);
                grid.Children.Add(nameLabel2, 1, 1);
                grid.Children.Add(nameLabel3, 2, 1);

                return new ViewCell
                {
                    View = grid
                };
            });

            dynamicListView.ItemTemplate = customDataTemplate;
            dynamicListView.ItemsSource = rows;
        }

        /// <summary>
        /// Send back to canvas
        /// </summary>
        /// <param name="obj">Object.</param>
        private void TappedCommand(object obj)
        {
            string iconToReintroduce = obj as string;

            Debug.WriteLine("Tapped Command: " + obj as string);

            IconSelected(new ArgsSelectedIcon
            {
                Name = iconToReintroduce,
                ImageSource = null
            });

            OnClose(null, null);
        }

        /// <summary>
        /// Creates the model.
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="name1">Name1.</param>
        /// <param name="res1">Res1.</param>
        /// <param name="name2">Name2.</param>
        /// <param name="res2">Res2.</param>
        /// <param name="name3">Name3.</param>
        /// <param name="res3">Res3.</param>
        /// <param name="recommendedWidth">Recommended width.</param>
        private DisplayImageRowModel CreateModel(string name1, ImageSource res1,
                                                 string name2, ImageSource res2,
                                                 string name3, ImageSource res3,
                                                 double recommendedWidth)
        {
            return new DisplayImageRowModel()
            {
                Name1 = name1,
                Image1 = res1,

                Name2 = name2,
                Image2 = res2,

                Name3 = name3,
                Image3 = res3,

                WidthRequest = recommendedWidth,

                TappedCommand = new Command(TappedCommand)
            };
        }

        /// <summary>
        /// Ons the close.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void OnClose(object sender, EventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await PopupNavigation.PopAsync();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    public class PopupUpViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        private Thickness _padding;
        public Thickness Padding
        {
            get
            {
                return _padding;
            }
            set
            {
                _padding = value;
                OnPropertyChanged("Padding");
            }
        }

        private bool _isSystemPadding = true;
        public bool IsSystemPadding
        {
            get
            {
                return _isSystemPadding;
            }
            set
            {
                _isSystemPadding = value;
                OnPropertyChanged("IsSystemPadding");
            }
        }
    }
}