﻿using System;
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

namespace VOP
{
    public partial class ScanParameterView : UserControl
    {
      
        public static string[] ScanToItems =
        {
            "Scan To Print",
            "Scan To File",
            "Scan To Application",
            "Scan To Email",
            "Scan To FTP",
            "Scan To Cloud",
        };

        public ScanParameterView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitMatchListBox();

            if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest == false)
            {
                QuickModeButton.IsChecked = true;
                cbDecodeLevel.IsEnabled = false;
            }
            else
            {
                HardModeButton.IsChecked = true;
                cbDecodeLevel.IsEnabled = true;
            }

            if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_Standard"))
            {
                cbDecodeLevel.SelectedIndex = 0;
            }
            else if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_Fine"))
            {
                cbDecodeLevel.SelectedIndex = 1;
            }
            else if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_SuperFine"))
            {
                cbDecodeLevel.SelectedIndex = 2;
            }
            else if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_HighestQuality"))
            {
                cbDecodeLevel.SelectedIndex = 3;
            }
        }

        private void cbDecodeLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDecodeLevel.SelectedIndex == 0)
            {
                MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level = (string)this.TryFindResource("ResStr_Faroe_Standard");
            }
            else if (cbDecodeLevel.SelectedIndex == 1)
            {
                MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level = (string)this.TryFindResource("ResStr_Faroe_Fine");
            }
            else if (cbDecodeLevel.SelectedIndex == 2)
            {
                MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level = (string)this.TryFindResource("ResStr_Faroe_SuperFine");
            }
            else if (cbDecodeLevel.SelectedIndex == 3)
            {
                MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level = (string)this.TryFindResource("ResStr_Faroe_HighestQuality");
            }
        }

        void InitMatchListBox()
        {
            MatchListBox.Items.Clear();
            for (int i = 0; i < SettingData.MaxShortCutNum; i++)
            {
                MatchListBox.Items.Add(CreateItem(i, MainWindow_Rufous.g_settingData.m_MatchList[i].Value));
            }

        }

        private ListBoxItem CreateItem(int id, int selectId)
        {

            TextBlock text = new TextBlock();

            text.Text = "Number " + (id + 1).ToString();

            text.Margin = new Thickness(10, 0, 0, 0);
            text.VerticalAlignment = VerticalAlignment.Center;
            text.FontSize = 16;
            text.Style = (Style)this.FindResource("DsDigital");

            SolidColorBrush txtbrush = new SolidColorBrush();
            txtbrush.Color = Colors.DodgerBlue;
            text.Foreground = txtbrush;

            ComboBox combobox = new ComboBox();
            combobox.Width = 200;

            if (id == 0)
            {
                combobox.Margin = new Thickness(44, 0, 0, 0);
            }
            else
            {
                combobox.Margin = new Thickness(40, 0, 0, 0);
            }
               

            foreach (string s in ScanToItems)
            {
                combobox.Items.Add(s);
            }
            combobox.SelectedIndex = selectId;
            combobox.SelectionChanged += new SelectionChangedEventHandler(cboListBoxItem_SelectionChanged);
            combobox.Tag = id;

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            stack.Children.Add(text);
            stack.Children.Add(combobox);

            ListBoxItem item = new ListBoxItem();
            SolidColorBrush bgbrush = new SolidColorBrush();
            bgbrush.Color = Colors.AliceBlue;
            item.Background = bgbrush;

            item.Content = stack;
            item.Height = 50;
            item.Tag = id;

            return item;
        }

        private void cboListBoxItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox item = sender as ComboBox;
            int id = (int)item.Tag;
            MainWindow_Rufous.g_settingData.m_MatchList[id] = new MatchListPair(id, item.SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_commonScanSettings.Clone();

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_commonScanSettings = (ScanParam)settingWin.m_scanParams.Clone();
            }
        }

        public void QRDecodeMode_click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "QuickModeButton")
                {
                    MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest = false;
                    cbDecodeLevel.IsEnabled = false;
                }
                else if (rdbtn.Name == "HardModeButton")
                {
                    MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest = true;
                    cbDecodeLevel.IsEnabled = true;
                }
            }
        }

        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if (null == _MainWin)
                {
                    return (MainWindow_Rufous)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }

     
    }
}
