using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Text.RegularExpressions;

namespace VOP
{
    public partial class PrintPage : UserControl
    {
        public PrintPage()
        {
            InitializeComponent();
        }

        public bool ValidateText(string str)
        {
            if (0 != str.Length)
            {
                Regex rg = new Regex("^([0-9]{1,3})$");
                if (rg.IsMatch(str))
                {
                    int value = int.Parse(str);

                    if (!(0 <= value && value <= 100))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

    }
}