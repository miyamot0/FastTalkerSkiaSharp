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

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.Controls
{
    /// <summary>
    /// Custom scroll view, to handle horizontal scrolling
    /// </summary>
    public class CustomScrollView : ScrollView
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource",
                                                                                              typeof(IEnumerable),
                                                                                              typeof(CustomScrollView),
                                                                                              default(IEnumerable),
                                                                                              BindingMode.Default,
                                                                                              null,
                                                                                              new BindableProperty.BindingPropertyChangedDelegate(HandleBindingPropertyChangedDelegate));

        /// <summary>
        /// Err out
        /// </summary>
        /// <returns>The binding property changed delegate.</returns>
        /// <param name="bindable">Bindable.</param>
        /// <param name="value">Value.</param>
        static object HandleBindingPropertyChangedDelegate(BindableObject bindable, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Getter
        /// </summary>
        /// <value>The items source.</value>
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

        /// <summary>
        /// Template property
        /// </summary>
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate",
                                                                                               typeof(DataTemplate),
                                                                                               typeof(CustomScrollView),
                                                                                               default(DataTemplate));

        /// <summary>
        /// Getter
        /// </summary>
        /// <value>The item template.</value>
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

        /// <summary>
        /// Event
        /// </summary>
        public event EventHandler<ItemTappedEventArgs> ItemSelected;

        /// <summary>
        /// Selected property
        /// </summary>
        public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create("SelectedCommand",
                                                                                                  typeof(ICommand),
                                                                                                  typeof(CustomScrollView), null);

        /// <summary>
        /// Selected command
        /// </summary>
        /// <value>The selected command.</value>
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

        /// <summary>
        /// Command parameter property
        /// </summary>
        public static readonly BindableProperty SelectedCommandParameterProperty = BindableProperty.Create("SelectedCommandParameter",
                                                                                                           typeof(object),
                                                                                                           typeof(CustomScrollView),
                                                                                                           null);
        
        /// <summary>
        /// Getter/Setter Command property
        /// </summary>
        /// <value>The selected command parameter.</value>
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

        /// <summary>
        /// Bindings for objects
        /// </summary>
        /// <param name="bindable">Bindable.</param>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
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

        /// <summary>
        /// On collection change, call render
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Render();
        }

        /// <summary>
        /// Render listview
        /// </summary>
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

                var viewCell = ItemTemplate.CreateContent() as ViewCell;

                viewCell.View.BindingContext = item;

                viewCell.View.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = command,
                    CommandParameter = SelectedCommandParameter ?? item,
                    NumberOfTapsRequired = 1
                });

                layout.Children.Add(viewCell.View);
            }

            Content = layout;
        }
    }
}
