﻿// Copyright 2012 lapthorn.net.
//
// This software is provided "as is" without a warranty of any kind. All
// express or implied conditions, representations and warranties, including
// any implied warranty of merchantability, fitness for a particular purpose
// or non-infringement, are hereby excluded. lapthorn.net and its licensors
// shall not be liable for any damages suffered by licensee as a result of
// using the software. In no event will lapthorn.net be liable for any
// lost revenue, profit or data, or for direct, indirect, special,
// consequential, incidental or punitive damages, however caused and regardless
// of the theory of liability, arising out of the use of or inability to use
// software, even if lapthorn.net has been advised of the possibility of
// such damages.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VOP.Controls
{
    public class SpinnerControlOnlySupportUpDowmKey : Control
    {
        public bool IsPercentFormat { get; set; }
        public SpinnerControlOnlySupportUpDowmKey()
        {
            IsPercentFormat = false;
        }

        static SpinnerControlOnlySupportUpDowmKey()
        {
            InitializeCommands();

            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpinnerControlOnlySupportUpDowmKey), new FrameworkPropertyMetadata(typeof(SpinnerControlOnlySupportUpDowmKey)));
        }

        #region FormattedValue property

        /// <summary>
        /// Dependency property identifier for the formatted value with limited 
        /// write access to the underlying read-only dependency property:  we
        /// can only use SetValue on this, not on the property itself.
        /// </summary>
        private static readonly DependencyProperty FormattedValueProperty =
            DependencyProperty.Register("FormattedValue", typeof(string), typeof(SpinnerControlOnlySupportUpDowmKey),
            new PropertyMetadata(DefaultValue.ToString(), OnFormattedValueChanged));

        private static void OnFormattedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpinnerControlOnlySupportUpDowmKey control = obj as SpinnerControlOnlySupportUpDowmKey;
            Decimal number;
            if (control != null)
            {
                string newValue = (string)args.NewValue;
                string oldValue = (string)args.OldValue;

                if (Decimal.TryParse(newValue, out number))
                {
                    control.Value = number;
                }
            }
        }
        /// <summary>
        /// Returns the formatted version of the value, with the specified
        /// number of DecimalPlaces.
        /// </summary>
        public string FormattedValue
        {
            set
            {
                SetValue(FormattedValueProperty, value);
            }
            get
            {
                return (string)GetValue(FormattedValueProperty);
            }
        }

        /// <summary>
        /// Update the formatted value.
        /// </summary>
        /// <param name="newValue"></param>
        protected void UpdateFormattedValue(decimal newValue)
        {
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo() { NumberDecimalDigits = DecimalPlaces };
            //  use fixed point, and the built-in NumberFormatInfo
            //  implementation of IFormatProvider

            var formattedValue = "";
            if (IsPercentFormat)
            {
                formattedValue = newValue.ToString("f", numberFormatInfo) + "%";
            }
            else
            {
                formattedValue = newValue.ToString("f", numberFormatInfo);
            }


            //  Set the value of the FormattedValue property via its property key
            SetValue(FormattedValueProperty, formattedValue);
        }
        #endregion


        #region Value property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControlOnlySupportUpDowmKey")]
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(SpinnerControlOnlySupportUpDowmKey),
            new FrameworkPropertyMetadata(DefaultValue,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged,
                CoerceValue
                ));

        /// <summary>
        /// If the value changes, update the text box that displays the Value 
        /// property to the consumer.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpinnerControlOnlySupportUpDowmKey control = obj as SpinnerControlOnlySupportUpDowmKey;
            if (control != null)
            {
                var newValue = (decimal)args.NewValue;
                var oldValue = (decimal)args.OldValue;

                control.UpdateFormattedValue(newValue);

                RoutedPropertyChangedEventArgs<decimal> e =
                    new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue, ValueChangedEvent);

                control.OnValueChanged(e);
            }
        }

        /// <summary>
        /// Raise the ValueChanged event.  Derived classes can use this.
        /// </summary>
        /// <param name="e"></param>
        virtual protected void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> e)
        {
            RaiseEvent(e);
        }

        private static decimal LimitValueByBounds(decimal newValue, SpinnerControlOnlySupportUpDowmKey control)
        {
            newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));
            //  then ensure the number of decimal places is correct.
            newValue = Decimal.Round(newValue, control.DecimalPlaces);
            return newValue;
        }

        private static object CoerceValue(DependencyObject obj, object value)
        {
            decimal newValue = (decimal)value;
            SpinnerControlOnlySupportUpDowmKey control = obj as SpinnerControlOnlySupportUpDowmKey;

            if (control != null)
            {
                //  ensure that the value stays within the bounds of the minimum and
                //  maximum values that we define.
                newValue = LimitValueByBounds(newValue, control);
            }

            return newValue;
        }


        #endregion


        #region MinimumValue property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControlSupportUpDowmKey")]
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        private static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.Register("Minimum", typeof(decimal), typeof(SpinnerControlOnlySupportUpDowmKey),
            new PropertyMetadata(DefaultMinimumValue));
        #endregion


        #region MaximumValue property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControlSupportUpDowmKey")]
        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        private static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("Maximum", typeof(decimal), typeof(SpinnerControlOnlySupportUpDowmKey),
            new PropertyMetadata(DefaultMaximumValue));

        #endregion


        #region DecimalPlaces property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControlSupportUpDowmKey")]
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        private static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(SpinnerControlOnlySupportUpDowmKey),
            new PropertyMetadata(DefaultDecimalPlaces));

        #endregion


        #region Change property
        /// <summary>
        /// This is the Control property that we expose to the user.
        /// </summary>
        [Category("SpinnerControlSupportUpDowmKey")]
        public decimal Change
        {
            get { return (decimal)GetValue(ChangeProperty); }
            set { SetValue(ChangeProperty, value); }
        }

        private static readonly DependencyProperty ChangeProperty =
            DependencyProperty.Register("Change", typeof(decimal), typeof(SpinnerControlOnlySupportUpDowmKey),
            new PropertyMetadata(DefaultChange));

        #endregion


        #region Default values

        /// <summary>
        /// Define the min, max and starting value, which we then expose 
        /// as dependency properties.
        /// </summary>
        private const Decimal DefaultMinimumValue = 0,
            DefaultMaximumValue = 100,
            DefaultValue = DefaultMinimumValue,
            DefaultChange = 1;

        /// <summary>
        /// The default number of decimal places, i.e. 0, and show the
        /// spinner control as an int initially.
        /// </summary>
        private const int DefaultDecimalPlaces = 0;
        #endregion


        #region Command Stuff
        public static RoutedCommand IncreaseCommand { get; set; }

        protected static void OnIncreaseCommand(Object sender, ExecutedRoutedEventArgs e)
        {
            SpinnerControlOnlySupportUpDowmKey control = sender as SpinnerControlOnlySupportUpDowmKey;

            if (control != null)
            {
                control.OnIncrease();
            }
        }

        protected void OnIncrease()
        {
            //  see https://connect.microsoft.com/VisualStudio/feedback/details/489775/
            //  for why we do this.
            Value = LimitValueByBounds(Value + Change, this);
        }

        public static RoutedCommand DecreaseCommand { get; set; }

        protected static void OnDecreaseCommand(Object sender, ExecutedRoutedEventArgs e)
        {
            SpinnerControlOnlySupportUpDowmKey control = sender as SpinnerControlOnlySupportUpDowmKey;

            if (control != null)
            {
                control.OnDecrease();
            }
        }

        protected void OnDecrease()
        {
            //  see https://connect.microsoft.com/VisualStudio/feedback/details/489775/
            //  for why we do this.
            Value = LimitValueByBounds(Value - Change, this);
        }

        /// <summary>
        /// Since we're using RoutedCommands for the up/down buttons, we need to
        /// register them with the command manager so we can tie the events
        /// to callbacks in the control.
        /// </summary>
        private static void InitializeCommands()
        {
            //  create instances
            IncreaseCommand = new RoutedCommand("IncreaseCommand", typeof(SpinnerControlOnlySupportUpDowmKey));
            DecreaseCommand = new RoutedCommand("DecreaseCommand", typeof(SpinnerControlOnlySupportUpDowmKey));

            //  register the command bindings - if the buttons get clicked, call these methods.
            CommandManager.RegisterClassCommandBinding(typeof(SpinnerControlOnlySupportUpDowmKey), new CommandBinding(IncreaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassCommandBinding(typeof(SpinnerControlOnlySupportUpDowmKey), new CommandBinding(DecreaseCommand, OnDecreaseCommand));

            CommandManager.RegisterClassInputBinding(typeof(SpinnerControlOnlySupportUpDowmKey), new InputBinding(IncreaseCommand, new KeyGesture(Key.Up)));
            CommandManager.RegisterClassInputBinding(typeof(SpinnerControlOnlySupportUpDowmKey), new InputBinding(DecreaseCommand, new KeyGesture(Key.Down)));
        }
        #endregion


        #region Events

        /// <summary>
        /// The ValueChangedEvent, raised if  the value changes.
        /// </summary>
        private static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(SpinnerControlOnlySupportUpDowmKey));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion

        #region ValidationHasError property
        public static readonly DependencyProperty ValidationHasErrorProperty =
                            DependencyProperty.Register("ValidationHasError",
                            typeof(bool),
                            typeof(SpinnerControlOnlySupportUpDowmKey),
                            new PropertyMetadata(false, new PropertyChangedCallback(OnValidationHasErrorPropertyChanged)));

        public bool ValidationHasError
        {
            get { return (bool)GetValue(ValidationHasErrorProperty); }
            set { SetValue(ValidationHasErrorProperty, value); }
        }

        private static void OnValidationHasErrorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SpinnerControlOnlySupportUpDowmKey control = sender as SpinnerControlOnlySupportUpDowmKey;
            if (control != null)
            {
                var newValue = (bool)args.NewValue;
                var oldValue = (bool)args.OldValue;

                RoutedPropertyChangedEventArgs<bool> e =
                    new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue, ValidationHasErrorEvent);

                control.OnValidationHasErrorPropertyChanged(e);
            }
        }

        virtual protected void OnValidationHasErrorPropertyChanged(RoutedPropertyChangedEventArgs<bool> e)
        {
            RaiseEvent(e);
        }

        public static readonly RoutedEvent ValidationHasErrorEvent =
                                   EventManager.RegisterRoutedEvent("ValidationHasErrorChanged",
                                   RoutingStrategy.Bubble,
                                   typeof(RoutedPropertyChangedEventHandler<bool>), typeof(SpinnerControlOnlySupportUpDowmKey));


        public event RoutedPropertyChangedEventHandler<bool> ValidationHasErrorChanged
        {
            add { AddHandler(ValidationHasErrorEvent, value); }
            remove { RemoveHandler(ValidationHasErrorEvent, value); }
        }
        #endregion
    }

}