using System;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;
using StrictComboBox = StrictXAML.Xamarin.Forms.StrictPicker;
using Dispatcher = Xamarin.Forms.Device;
using ComboBox = Xamarin.Forms.Picker;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using DependencyObject = Xamarin.Forms.BindableObject;
using SelectionChangedEventArgs = System.EventArgs;

namespace StrictXAML.Xamarin.Forms
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class StrictPicker : Picker
    {
        private bool _userInitiated;
        private bool _applicationInitiated;

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public object StrictSelectedItem
        {
            get => GetValue(StrictSelectedItemProperty);
            set => SetValue(StrictSelectedItemProperty, value);
        }


        public static readonly DependencyProperty StrictSelectedItemProperty =
            DependencyProperty.Create(
                nameof(StrictSelectedItem),
                typeof(object),
                typeof(StrictComboBox), null, BindingMode.TwoWay,
                propertyChanged: StrictSelectedItemChanged);

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (!_applicationInitiated && propertyName == nameof(SelectedItem))
            {
                if (SelectedItem != null)
                {
                    try
                    {
                        _userInitiated = true;
                        StrictSelectedItem = SelectedItem;
                    }
                    finally
                    {
                        _userInitiated = false;
                    }
                }
                else
                {
                    QueueRestoreValidState();
                }
            }

            base.OnPropertyChanged(propertyName);
        }

        private static void StrictSelectedItemChanged(DependencyObject d, object oldValue, object eNewValue)
        {
            var _this = (StrictComboBox)d;
            if (_this._userInitiated) return;
            try
            {
                _this._applicationInitiated = true;
                _this.SelectedItem = eNewValue;
            }
            finally
            {
                _this._applicationInitiated = false;
            }
        }

        [SuppressMessage("ReSharper", "RedundantDelegateCreation")]
        private void QueueRestoreValidState()
        {
            Dispatcher.BeginInvokeOnMainThread(new Action(() =>
            {
                try
                {
                    _applicationInitiated = true;
                    SelectedItem = StrictSelectedItem;
                }
                finally
                {
                    _applicationInitiated = false;
                }
            }));
        }
    }
}
