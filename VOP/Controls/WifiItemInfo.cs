using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VOP.Controls
{
    public class WifiItemInfo : DependencyObject
    {
        public static readonly DependencyProperty SSIDTextProperty =
         DependencyProperty.Register("SSIDText", typeof(string), typeof(WifiItemInfo));

        public static readonly DependencyProperty EncryptionTextProperty =
         DependencyProperty.Register("EncryptionText", typeof(string), typeof(WifiItemInfo));

        public static readonly DependencyProperty EncryptTypeProperty =
         DependencyProperty.Register("EncryptType", typeof(EnumEncryptType), typeof(WifiItemInfo));

        public static readonly DependencyProperty WifiSignalLevelProperty =
         DependencyProperty.Register("WifiSignalLevel", typeof(EnumWifiSignalLevel), typeof(WifiItemInfo));

        public string SSIDText
        {
            get { return (string)GetValue(SSIDTextProperty); }
            set { SetValue(SSIDTextProperty, value); }
        }

        public string EncryptionText
        {
            get { return (string)GetValue(EncryptionTextProperty); }
            set { SetValue(EncryptionTextProperty, value); }
        }

        public EnumEncryptType EncryptType
        {
            get { return (EnumEncryptType)GetValue(EncryptTypeProperty); }
            set { SetValue(EncryptTypeProperty, value); }
        }

        public EnumWifiSignalLevel WifiSignalLevel
        {
            get { return (EnumWifiSignalLevel)GetValue(WifiSignalLevelProperty); }
            set { SetValue(WifiSignalLevelProperty, value); }
        }
    }

}
