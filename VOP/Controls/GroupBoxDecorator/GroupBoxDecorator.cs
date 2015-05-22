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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP.Controls
{
    public class GroupBoxDecorator : ContentControl
    {

        static GroupBoxDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBoxDecorator), new FrameworkPropertyMetadata(typeof(GroupBoxDecorator)));
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
           DependencyProperty.Register("Header", typeof(string), typeof(GroupBoxDecorator));


        public GroupBoxDecorator()
        {
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(IsEnabledValueChanged);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void IsEnabledValueChanged(Object obj, DependencyPropertyChangedEventArgs args)
        {
         //   foreach (DependencyObject tb in FindVisualChildren<DependencyObject>(this.Content))
            {
                // do something with tb here
            }
        }
    }
}
