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

namespace VOP
{
    /// <summary>
    /// Interaction logic for UserDefinedSetting.xaml
    /// </summary>
    public partial class UserDefinedSetting : Window
    {
        public ObservableCollection<UserDefinedSizeItem> UserDefinedSizeItems { get; set; }

        public UserDefinedSetting(ObservableCollection<UserDefinedSizeItem> userDefinedSizeItems)
        {
            UserDefinedSizeItems = userDefinedSizeItems;
            DataContext = this;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            myStackPanel.AddHandler(RadioButton.CheckedEvent, new RoutedEventHandler(OnRadioChecked));

        }

        private void OnRadioChecked(object sender, RoutedEventArgs e)
        {
            RadioButton button = e.Source as RadioButton;
            if (button.Name == "RadioButtonMM")
            {
                tWidth.Text = "宽度（76.2-216.0）";
                tHeight.Text = "高度（116.0-355.6）";

                tbWidth.Text = "210.0";
                tbHeight.Text = "297.0";
            }
            else
            {
                tWidth.Text = "宽度（3.00-8.50）";
                tHeight.Text = "高度（4.57-14.00）";

                tbWidth.Text = "8.27";
                tbHeight.Text = "11.69";
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(myComboBox.Text.Trim() != "")
            {
                double w = 0;
                double h = 0;

                if(!double.TryParse(tbWidth.Text.Trim(), out w))
                {
                    w = 0;
                }

                if (!double.TryParse(tbHeight.Text.Trim(), out h))
                {
                    h = 0;
                }

                var result = (from item in UserDefinedSizeItems
                                         where item.UserDefinedName == myComboBox.Text.Trim()
                                         select item).ToList();

                if (result.Count == 0)
                {
                    UserDefinedSizeItems.Add(new UserDefinedSizeItem()
                    {
                        UserDefinedName = myComboBox.Text.Trim(),
                        IsMM = (bool)RadioButtonMM.IsChecked, 
                        Width = w, 
                        Height = h });
                }
                else if (result.Count == 1)
                {
                    UserDefinedSizeItem item = (UserDefinedSizeItem)result[0];
                    item.UserDefinedName = myComboBox.Text.Trim();
                    item.IsMM = (bool)RadioButtonMM.IsChecked;
                    item.Width = w;
                    item.Height = h;
                }
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

            if (element != null)
            {
                return element.Width.ToString();
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;
            if (element != null)
            {
                if(double.TryParse(value.ToString(), out result))
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

            if (element != null)
            {
                return element.Height.ToString();
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
