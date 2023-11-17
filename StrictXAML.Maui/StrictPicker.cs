using System.Diagnostics.CodeAnalysis;

namespace StrictXAML.Maui
{
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

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static readonly BindableProperty StrictSelectedItemProperty =
            BindableProperty.Create(
                nameof(StrictSelectedItem),
                typeof(object),
                typeof(StrictPicker), null, BindingMode.TwoWay,
                propertyChanged: StrictSelectedItemChanged);

        protected override void OnPropertyChanged(string propertyName = null!)
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

        private static void StrictSelectedItemChanged(BindableObject d, object oldValue, object eNewValue)
        {
            var @this = (StrictPicker)d;
            if (@this._userInitiated) return;
            try
            {
                @this._applicationInitiated = true;
                @this.SelectedItem = eNewValue;
            }
            finally
            {
                @this._applicationInitiated = false;
            }
        }

        [SuppressMessage("ReSharper", "RedundantDelegateCreation")]
        private void QueueRestoreValidState()
        {
            Dispatcher.Dispatch(new Action(() =>
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