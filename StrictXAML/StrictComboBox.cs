using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace StrictXAML
{
    public class StrictComboBox : ComboBox
    {
        private bool _userInitiated = false;
        private bool _applicationInitiated = false;

        public object StrictSelectedItem
        {
            get { return GetValue(StrictSelectedItemProperty); }
            set { SetValue(StrictSelectedItemProperty, value); }
        }

        public static readonly DependencyProperty StrictSelectedItemProperty =
            DependencyProperty.Register("StrictSelectedItem", typeof(object), typeof(StrictComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, StrictSelectedItemChanged));

        public IEnumerable StrictItemsSource
        {
            get { return (IEnumerable)GetValue(StrictItemsSourceProperty); }
            set { SetValue(StrictItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty StrictItemsSourceProperty =
            DependencyProperty.Register("StrictItemsSource", typeof(IEnumerable), typeof(StrictComboBox), new PropertyMetadata(null, StrictItemsSourceChanged));


        static StrictComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StrictComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
        }

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
            if (!_this._userInitiated)
            {
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
        }

        private static void StrictItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = (StrictComboBox)d;
            _this.ItemsSource = (IEnumerable)e.NewValue;
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
