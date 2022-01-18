using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace StrictXAML
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class StrictComboBox : ComboBox
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
            DependencyProperty.Register(
                "StrictSelectedItem",
                typeof(object),
                typeof(StrictComboBox),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    StrictSelectedItemChanged));


        static StrictComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StrictComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
            SelectedItemProperty.OverrideMetadata(typeof(StrictComboBox), new FrameworkPropertyMetadata(null, null, CoerceValueCallback));
        }

        private static object CoerceValueCallback(DependencyObject o, object value) => value;

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (!_applicationInitiated)
            {
                if (e.AddedItems.Count > 0)
                {
                    try
                    {
                        _userInitiated = true;
                        StrictSelectedItem = e.AddedItems[0];
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
            base.OnSelectionChanged(e);
        }

        private static void StrictSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = (StrictComboBox)d;
            if (_this._userInitiated) return;
            try
            {
                _this._applicationInitiated = true;
                _this.SelectedItem = e.NewValue;
            }
            finally
            {
                _this._applicationInitiated = false;
            }
        }

        private void QueueRestoreValidState()
        {
            Dispatcher.BeginInvoke(new Action(() =>
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
