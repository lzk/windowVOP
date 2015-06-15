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

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for IdCardTypeSelectWindow.xaml
    /// </summary>
    public partial class IdCardTypeSelectWindow : Window
    {
        private bool _helpCanExecute = true;
        public IdCardTypeItem SelectedTypeItem { get; set; }

        public IdCardTypeSelectWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            //this.Width = this.Width * App.gScalingRate;
            //this.Height = this.Height * App.gScalingRate;
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void verticalListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = e.Source as ListBox;
            SelectedTypeItem = lb.SelectedItem as IdCardTypeItem;
            this.DialogResult = true;
        }

        private void ListBox_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            ListBox lb = e.Source as ListBox;
            SelectedTypeItem = lb.SelectedItem as IdCardTypeItem;
            this.DialogResult = true;
        }

        private void HelpCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (listBox.SelectedIndex == -1)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }

            e.Handled = true;
        }

        private void HelpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            listBox.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.PreviewMouseDownEvent,
                Source = this,
            });
        }
    }
}
