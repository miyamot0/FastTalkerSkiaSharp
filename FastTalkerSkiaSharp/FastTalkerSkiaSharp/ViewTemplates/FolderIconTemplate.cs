using System;
using Xamarin.Forms;
namespace FastTalkerSkiaSharp.ViewTemplates
{
    public class FolderIconTemplate : ViewCell
    {
        public FolderIconTemplate()
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

            View = grid;
        }
    }
}
