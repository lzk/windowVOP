using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using VOP.Controls;

namespace VOP
{
    /// <summary>
    /// Interaction logic for UserDefinedSetting.xaml
    /// </summary>
    public partial class UserDefinedSetting : Window
    {
        double MMWidthMin = 76.2;
        double MMWidthMax = 216.0;
        double MMWidthDefault = 210.0;
        double MMHeightMin = 116.0;
        double MMHeightMax = 355.6;
        double MMHeightDefault = 297.0;

        double InchWidthMin = 3.00;
        double InchWidthMax = 8.50;
        double InchWidthDefault = 8.27;
        double InchHeightMin = 4.57;
        double InchHeightMax = 14.00;
        double InchHeightDefault = 11.69;

        double widthTextValue = 0;
        double heightTextValue = 0;

        bool IsTextInputValid = true;
        public ObservableCollection<UserDefinedSizeItem> UserDefinedSizeItems { get; set; }

        public UserDefinedSetting(ObservableCollection<UserDefinedSizeItem> userDefinedSizeItems)
        {
            UserDefinedSizeItems = userDefinedSizeItems;
            DataContext = this;
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;

            tWidth.Text = (string)this.TryFindResource("ResStr_Width") + "(76.2-216.0)";
            tHeight.Text = (string)this.TryFindResource("ResStr_Length") + "(116.0-355.6)";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            myStackPanel.AddHandler(RadioButton.CheckedEvent, new RoutedEventHandler(OnRadioChecked));

            tbWidth.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput);
            tbWidth.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
            tbWidth.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
         
            tbHeight.PreviewTextInput += new TextCompositionEventHandler(TextBox_PreviewTextInput);
            tbHeight.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
            tbHeight.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);

            TextBox tb = myComboBox.Template.FindName("PART_EditableTextBox", myComboBox) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(ComboBoxTextBox_TextChanged);

            myComboBox.SelectedIndex = -1;

            SaveButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            SetWidthTextValue(MMWidthDefault);
            SetHeightTextValue(MMHeightDefault);
        }

        private void OnDropDownOpened(object sender, EventArgs e)
        {
            TextBox tb = myComboBox.Template.FindName("PART_EditableTextBox", myComboBox) as TextBox;
            tb.SelectionStart = tb.Text.Length;
        }

        private void OnDropDownClosed(object sender, EventArgs e)
        {
            TextBox tb = myComboBox.Template.FindName("PART_EditableTextBox", myComboBox) as TextBox;
            tb.SelectionStart = tb.Text.Length;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string strText = e.Text;
            if (strText.Length > 0 && !Char.IsDigit(strText, 0))
            {
                if (e.Text != ".")
                    e.Handled = true;      
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if(string.IsNullOrEmpty(tb.Text))
            {    
                if (tb.Name == "tbWidth")
                {
                    tbWidth.Text = RadioButtonMM.IsChecked == true ? MMWidthDefault.ToString() : InchWidthDefault.ToString();
                }
                else
                {
                    tbHeight.Text = RadioButtonMM.IsChecked == true ? MMHeightDefault.ToString() : InchHeightDefault.ToString();
                }
            }
        }

        private bool IsTextBoxValid(TextBox tb, ValidationRule rule)
        {
            ValidationResult result = rule.Validate(tb.Text, null);
            if (result.IsValid == false)
            {
                if (tb.Name == "tbWidth")
                {
                    tbWidthBorder.BorderThickness = new Thickness(2);
                }
                else
                {
                    tbHeightBorder.BorderThickness = new Thickness(2);
                }

                return false;
            }
            else
            {
                if (tb.Name == "tbWidth")
                {
                    tbWidthBorder.BorderThickness = new Thickness(0);
                }
                else
                {
                    tbHeightBorder.BorderThickness = new Thickness(0);
                }

                return true;
            }
        }

        private void ComboBoxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (IsTextInputValid)
            {
                if (tb.Text.Trim() == "")
                {
                    SaveButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                }
                else
                {
                    if (IsTextKey(tb.Text))
                    {
                        SaveButton.IsEnabled = true;
                        DeleteButton.IsEnabled = true;
                    }
                    else
                    {
                        SaveButton.IsEnabled = true;
                        DeleteButton.IsEnabled = false;
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (RadioButtonMM.IsChecked == true)
            {
                if (tb.Name == "tbWidth")
                {
                    UserDefinedSizeValidationRule ruleWidth = new UserDefinedSizeValidationRule();
                    ruleWidth.MinimumValue = MMWidthMin;
                    ruleWidth.MaximumValue = MMWidthMax;
                    ruleWidth.DecimalPlaces = 1;

                    IsTextInputValid = IsTextBoxValid(tb, ruleWidth);
                }
                else
                {

                    UserDefinedSizeValidationRule ruleHeight = new UserDefinedSizeValidationRule();
                    ruleHeight.MinimumValue = MMHeightMin;
                    ruleHeight.MaximumValue = MMHeightMax;
                    ruleHeight.DecimalPlaces = 1;

                    IsTextInputValid = IsTextBoxValid(tb, ruleHeight);
                }

            }
            else
            {
                if (tb.Name == "tbWidth")
                {
                    UserDefinedSizeValidationRule ruleWidth = new UserDefinedSizeValidationRule();
                    ruleWidth.MinimumValue = InchWidthMin;
                    ruleWidth.MaximumValue = InchWidthMax;
                    ruleWidth.DecimalPlaces = 2;

                    IsTextInputValid = IsTextBoxValid(tb, ruleWidth);
                }
                else
                {
                    UserDefinedSizeValidationRule ruleHeight = new UserDefinedSizeValidationRule();
                    ruleHeight.MinimumValue = InchHeightMin;
                    ruleHeight.MaximumValue = InchHeightMax;
                    ruleHeight.DecimalPlaces = 2;

                    IsTextInputValid = IsTextBoxValid(tb, ruleHeight);
                }  
            }

            if (IsTextInputValid)
            {
                OkButton.IsEnabled = true;

                if (myComboBox.Text.Trim() == "")
                {
                    SaveButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                }
                else
                {
                    if (IsTextKey(myComboBox.Text))
                    {
                        SaveButton.IsEnabled = true;
                        DeleteButton.IsEnabled = true;
                    }
                    else
                    {
                        SaveButton.IsEnabled = true;
                        DeleteButton.IsEnabled = false;
                    }    
                }

                if (tb.Name == "tbWidth")
                {
                    widthTextValue = double.Parse(tb.Text);
                }
                else
                {
                    heightTextValue = double.Parse(tb.Text);
                }

            }
            else
            { 
                OkButton.IsEnabled = false;
                SaveButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

        private void OnRadioChecked(object sender, RoutedEventArgs e)
        {

            RadioButton button = e.Source as RadioButton;
            
            if (button.Name == "RadioButtonMM")
            {  
                tWidth.Text = (string)this.TryFindResource("ResStr_Width") + "(76.2-216.0)";
                tHeight.Text = (string)this.TryFindResource("ResStr_Length") + "(116.0-355.6)";    
            }
            else
            {
                tWidth.Text = (string)this.TryFindResource("ResStr_Width") + "(3.00-8.50)";
                tHeight.Text = (string)this.TryFindResource("ResStr_Length") + "(4.57-14.00)";
            }

            ChangeMMInch();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void UserDefinedNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(myComboBox.SelectedIndex != -1)
            {
                UserDefinedSizeItem item = UserDefinedSizeItems[myComboBox.SelectedIndex];

                if (item.IsMM == true)
                {
                    RadioButtonMM.IsChecked = true;
                    RadioButtonInch.IsChecked = false;
                }
                else
                {
                    RadioButtonMM.IsChecked = false;
                    RadioButtonInch.IsChecked = true;
                }

                SetWidthTextValue(item.Width);
                SetHeightTextValue(item.Height);
 
            }
        }

        private void SetWidthTextValue(double value)
        {
            widthTextValue = value;
            tbWidth.Text = RadioButtonMM.IsChecked == true ? 
                           string.Format("{0:0.0}", widthTextValue)
                           : string.Format("{0:0.00}", widthTextValue);
        }

        private void SetHeightTextValue(double value)
        {
            heightTextValue = value;
            tbHeight.Text = RadioButtonMM.IsChecked == true ?
                           string.Format("{0:0.0}", heightTextValue)
                           : string.Format("{0:0.00}", heightTextValue);
        }

        private double GetWidthTextValue()
        {
            return widthTextValue;
        }

        private double GetHeightTextValue()
        {
            return heightTextValue;
        }

        private void ChangeMMInch()
        {

            SetWidthTextValue(RadioButtonMM.IsChecked == true ?
                            SizeConvert.InchToMM(GetWidthTextValue())
                            : SizeConvert.MMToInch(GetWidthTextValue()));

            SetHeightTextValue(RadioButtonMM.IsChecked == true ?
                            SizeConvert.InchToMM(GetHeightTextValue())
                            : SizeConvert.MMToInch(GetHeightTextValue()));     
          
        }

        private bool IsTextKey(string text)
        {
            var result = (from item in UserDefinedSizeItems
                          where item.UserDefinedName == text.Trim()
                          select item).ToList();

            if (result.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(myComboBox.Text.Trim() != "")
            {
                double w = GetWidthTextValue();
                double h = GetHeightTextValue();

                var result = (from item in UserDefinedSizeItems
                                         where item.UserDefinedName == myComboBox.Text.Trim()
                                         select item).ToList();

                if (result.Count == 1)
                {
                    UserDefinedSizeItem item = (UserDefinedSizeItem)result[0];
                    item.UserDefinedName = myComboBox.Text.Trim();
                    item.IsMM = (bool)RadioButtonMM.IsChecked;
                    item.Width = w;
                    item.Height = h;
                }
                else if (result.Count == 0)
                {
                    if (UserDefinedSizeItems.Count() < 20)
                    {
                        
                        UserDefinedSizeItems.Add(new UserDefinedSizeItem()
                        {
                            UserDefinedName = myComboBox.Text.Trim(),
                            IsMM = (bool)RadioButtonMM.IsChecked,
                            Width = w,
                            Height = h
                        });
                        
                    }
                    else
                    {
                        MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple(
                            (string)this.FindResource("ResStr_Warning_Custom_Exceed"),
                            (string)this.FindResource("ResStr_Warning_2"));
                        messageBox.Owner = App.Current.MainWindow;
                        messageBox.ShowDialog();
                    }
                }
                myComboBox.Text = "";
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = (from item in UserDefinedSizeItems
                                     where item.UserDefinedName == myComboBox.Text.Trim()
                                     select item).ToList();

            if (result.Count != 0)
            {
                UserDefinedSizeItems.Remove((UserDefinedSizeItem)result[0]);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    
        public int GetCurrentSelectedIndex()
        {
            return myComboBox.SelectedIndex;
        }

        private void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }

 
    public class UserDefinedSizeItem : DependencyObject
    {
        public string UserDefinedName
        {
            get { return (string)GetValue(UserDefinedNameProperty); }
            set { SetValue(UserDefinedNameProperty, value); }
        }

        public static readonly DependencyProperty UserDefinedNameProperty =
            DependencyProperty.Register("UserDefinedName", typeof(string), typeof(UserDefinedSizeItem));

        public bool IsMM
        {
            get { return (bool)GetValue(IsMMProperty); }
            set { SetValue(IsMMProperty, value); }
        }

        public static readonly DependencyProperty IsMMProperty =
                DependencyProperty.Register("IsMM", typeof(bool), typeof(UserDefinedSizeItem));

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
             DependencyProperty.Register("Width", typeof(double), typeof(UserDefinedSizeItem));

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty HeightProperty =
             DependencyProperty.Register("Height", typeof(double), typeof(UserDefinedSizeItem));
    }

    public class IsInchConverter : IValueConverter
    {
        UserDefinedSizeItem element;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string flag = parameter as string;
            element = value as UserDefinedSizeItem;

            if (element != null)
            {
                return (flag == "flip") ? !element.IsMM : element.IsMM;
            }
            else
            {
                return (flag == "flip") ? false : true;
            }
              
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string flag = parameter as string;

            if (element != null)
            {
                element.IsMM = (bool)value;
                return element;
            }
            else
            {
                return null;
            }
               
        }
    }

    public class WidthConverter : IValueConverter
    {
        UserDefinedSizeItem element;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            element = value as UserDefinedSizeItem;
            string flag = parameter as string;

            if (element != null)
            {
                return (flag == "1") ? string.Format("{0:0.0}", element.Width) : string.Format("{0:0.00}", element.Width);
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;
            if (element != null)
            {
                if (double.TryParse(value.ToString(), out result))
                {
                    element.Width = result;
                }

                return element;
            }
            else
                return null;
        }
    }

    public class HeightConverter : IValueConverter
    {
        UserDefinedSizeItem element;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            element = value as UserDefinedSizeItem;
            string flag = parameter as string;

            if (element != null)
            {
                return (flag == "1") ? string.Format("{0:0.0}", element.Height) : string.Format("{0:0.00}", element.Height);
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;
            if (element != null)
            {
                if (double.TryParse(value.ToString(), out result))
                {
                    element.Height = result;
                }

                return element;
            }
            else
                return null;
        }
    }
}
