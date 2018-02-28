using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace FastTalkerSkiaSharp.ViewModels
{
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
