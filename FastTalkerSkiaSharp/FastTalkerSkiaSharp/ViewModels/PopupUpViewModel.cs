/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

namespace FastTalkerSkiaSharp.ViewModels
{
    public class PopupUpViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        Xamarin.Forms.Thickness _padding;
        public Xamarin.Forms.Thickness Padding
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

        bool _isSystemPadding = true;
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
