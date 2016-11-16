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
            "Scan To Ftp",
            "Scan To Cloud",
        };

        public ScanParameterView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitMatchListBox();
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

            settingWin.m_scanResln = MainWindow_Rufous.g_settingData.m_commonScanSettings.ScanResolution;
            settingWin.m_paperSize = MainWindow_Rufous.g_settingData.m_commonScanSettings.PaperSize;
            settingWin.m_color = MainWindow_Rufous.g_settingData.m_commonScanSettings.ColorType;
            settingWin.m_brightness = MainWindow_Rufous.g_settingData.m_commonScanSettings.Brightness;
            settingWin.m_contrast = MainWindow_Rufous.g_settingData.m_commonScanSettings.Contrast;

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_commonScanSettings.ScanResolution = settingWin.m_scanResln;
                MainWindow_Rufous.g_settingData.m_commonScanSettings.PaperSize = settingWin.m_paperSize;
                MainWindow_Rufous.g_settingData.m_commonScanSettings.ColorType = settingWin.m_color;
                MainWindow_Rufous.g_settingData.m_commonScanSettings.Brightness = settingWin.m_brightness;
                MainWindow_Rufous.g_settingData.m_commonScanSettings.Contrast = settingWin.m_contrast;
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
