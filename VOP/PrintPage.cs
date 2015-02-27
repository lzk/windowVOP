using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage

namespace VOP
{
    public partial class PrintPage : UserControl
    {
        public PrintPage()
        {
            InitializeComponent();
        }

        #region AdjustValueEx_Ctrl  // For Test...
        int minValue_Ex = 0;
        int maxValue_Ex = 100;
        int curValue_Ex = 0;

        private void IncFuncCallbackEx()
        {
            curValue_Ex++;

            if (minValue_Ex <= curValue_Ex && (curValue_Ex <= maxValue_Ex))
            {
                adjustValueExCtrl.Text = curValue_Ex.ToString() + "%";
            }
        }

        private void DecFuncCallbackEx()
        {
            curValue_Ex--;

            if (minValue_Ex <= curValue_Ex && (curValue_Ex <= maxValue_Ex))
            {
               adjustValueExCtrl.Text = curValue_Ex.ToString() + "%";
            }
        }
        #endregion


        #region AmountOfPage  // For Test...
        int minValue_AmountOfPage = 0;
        int maxValue_AmountOfPage = 100;
        int curValue_AmountOfPage = 0;

        private void IncCallback_AmountOfPage()
        {
            curValue_AmountOfPage++;

            if (minValue_AmountOfPage <= curValue_AmountOfPage && (curValue_AmountOfPage <= maxValue_AmountOfPage))
            {
                AmountOfPage.Text = curValue_AmountOfPage.ToString();
            }

        }

        private void DecCallback_AmountOfPage()
        {
            curValue_AmountOfPage--;

            if (minValue_AmountOfPage <= curValue_AmountOfPage && (curValue_AmountOfPage <= maxValue_AmountOfPage))
            {
                AmountOfPage.Text = curValue_AmountOfPage.ToString();
            }
        }

        #endregion // AmountOfPage


        private void slider_ValueChanged(double oldValue, double newValue)
        {
            double test = oldValue;
            double test2 = newValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // for test.. 
          // imagePreviewCtrl.Path=@"F:\ben";
        }
    }
}