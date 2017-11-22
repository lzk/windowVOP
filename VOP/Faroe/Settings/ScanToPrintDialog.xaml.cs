using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanToPrintDialog.xaml
    /// </summary>
    public partial class ScanToPrintDialog : Window
    {
        public ScanToPrintParam m_scanToPrintParams = new ScanToPrintParam();
        public ScanParam m_scanParams = new ScanParam();
        public ScanToPrintDialog()
        {
            InitializeComponent();
        }
        private void ScanToPrintDialog_Loaded(object sender, RoutedEventArgs e)
        {
            int index = 0;

            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            cboPrinters.Items.Clear();

            for (int i = 0; i < MainWindow_Rufous.g_printerList.Count; i++)
            {
                if (MainWindow_Rufous.g_printerList[i] == m_scanToPrintParams.PrinterName)
                {
                    index = i;
                }

                cboPrinters.Items.Add(MainWindow_Rufous.g_printerList[i]);
            }

            cboPrinters.SelectedIndex = index;
            tbSettings.Focus();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)m_scanParams.Clone();

            if (settingWin.ShowDialog() == true)
            {
                m_scanParams = (ScanParam)settingWin.m_scanParams.Clone();
            }
        }
        private void cboPrinters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != cboPrinters.SelectedItem)
            {
                m_scanToPrintParams.PrinterName = this.cboPrinters.SelectedItem.ToString();
            }
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
        }
        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
