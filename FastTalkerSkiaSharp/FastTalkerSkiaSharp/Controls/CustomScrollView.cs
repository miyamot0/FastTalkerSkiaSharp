/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   Fast Talker is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, version 3.

   Fast Talker is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
   </copyright>

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Controls
{
    public class CustomScrollView : ScrollView
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource",
                                                                                              typeof(IEnumerable),
                                                                                              typeof(CustomScrollView),
                                                                                              default(IEnumerable),

                                                                                              BindingMode.Default,
                                                                                              null,
                                                                                              new BindableProperty.BindingPropertyChangedDelegate(HandleBindingPropertyChangedDelegate));

        private static object HandleBindingPropertyChangedDelegate(BindableObject bindable, object value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate",
                                                                                               typeof(DataTemplate),
                                                                                               typeof(CustomScrollView),
                                                                                               default(DataTemplate));

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        public event EventHandler<ItemTappedEventArgs> ItemSelected;

        public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create("SelectedCommand",
                                                                                                  typeof(ICommand),
                                                                                                  typeof(CustomScrollView), null);

        public ICommand SelectedCommand
        {
            get
            {
                return (ICommand)GetValue(SelectedCommandProperty);
            }
            set
            {
                SetValue(SelectedCommandProperty, value);
            }
        }

        public static readonly BindableProperty SelectedCommandParameterProperty = BindableProperty.Create("SelectedCommandParameter",
                                                                                                           typeof(object),
                                                                                                           typeof(CustomScrollView),
                                                                                                           null);

        public object SelectedCommandParameter
        {
            get
            {
                return GetValue(SelectedCommandParameterProperty);
            }
            set
            {
                SetValue(SelectedCommandParameterProperty, value);
            }
        }

        static void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            var isOldObservable = oldValue?.GetType().GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(INotifyCollectionChanged));

            var isNewObservable = newValue?.GetType().GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(INotifyCollectionChanged));

            var tl = (CustomScrollView)bindable;

            if (isOldObservable.GetValueOrDefault(false))
            {
                ((INotifyCollectionChanged)oldValue).CollectionChanged -= tl.HandleCollectionChanged;
            }

            if (isNewObservable.GetValueOrDefault(false))
            {
                ((INotifyCollectionChanged)newValue).CollectionChanged += tl.HandleCollectionChanged;
            }

            if (oldValue != newValue)
            {
                tl.Render();
            }
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Render();
        }

        public void Render()
        {
            if (ItemTemplate == null || ItemsSource == null)
            {
                Content = null;

                return;
            }

            var layout = new StackLayout();

            layout.Orientation = Orientation == ScrollOrientation.Vertical ? StackOrientation.Vertical :
                                                                             StackOrientation.Horizontal;

            foreach (var item in ItemsSource)
            {
                var command = SelectedCommand ?? new Command((obj) =>
                {
                    var args = new ItemTappedEventArgs(ItemsSource, item);
                    ItemSelected?.Invoke(this, args);
                });

                var commandParameter = SelectedCommandParameter ?? item;

                var viewCell = ItemTemplate.CreateContent() as ViewCell;

                viewCell.View.BindingContext = item;

                viewCell.View.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = command,
                    CommandParameter = commandParameter,
                    NumberOfTapsRequired = 1
                });

                layout.Children.Add(viewCell.View);
            }

            Content = layout;
        }

        public CustomScrollView()
        {
        }
    }
}
