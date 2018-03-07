using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanSetting.xaml
    /// </summary>
    public partial class DecodeScanSettingDialog : Window
    {
        public ScanParamDecode m_scanParams = new ScanParamDecode();
        private EnumPaperSizeScan m_lastPaperSize = EnumPaperSizeScan._Auto;
        private EnumPaperSizeScan m_lastPaperSize1 = EnumPaperSizeScan._Auto;
        private EnumScanMediaType m_lastPaperType = EnumScanMediaType._Normal;
        private static Byte m_powermode = 0;

        public DecodeScanSettingDialog()
        {
            this.InitializeComponent();

            this.Width = this.Width*  App.gScalingRate;
            this.Height = this.Height*App.gScalingRate;

            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
            // Insert code required on object creation below this point.            
        }

        public void handler_loaded( object sender, RoutedEventArgs e )
        {
            m_lastPaperSize = m_scanParams.PaperSize;
            m_lastPaperSize1 = m_scanParams.PaperSize;
            m_lastPaperType = m_scanParams.ScanMediaType;

            InitControls();
            InitScanSize();
            InitMediaType();

            cboScanSize.Focus();

            byte power = 0;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            PowerModeRecord m_rec = new PowerModeRecord();
            if (worker.InvokeMethod<PowerModeRecord>("", ref m_rec, DllMethodType.GetPowerSupply, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    if (m_rec.Mode != 0)
                        power = m_rec.Mode;
                }
            }

            if (power == 1)
                tbTitle.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_setting1");
            else if (power == 3)
                tbTitle.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_setting2");
            else if (power == 2)
                tbTitle.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_setting3");
            else
                tbTitle.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_setting4");

            if (m_powermode != power)
            {
                m_powermode = power;

                InitControls();
                InitScanSize();
                InitMediaType();
            }

        }

        private void InitMediaType()
        {
            cboMediaType.Items.Clear();

            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_Type_Normal");
            cboItem.DataContext = EnumScanMediaType._Normal;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add(cboItem);

            if (m_scanParams.PaperSize != EnumPaperSizeScan._LongPage &&
                m_powermode <= 1)
            {
                cboItem = new ComboBoxItem();
                cboItem.Content = (string)this.TryFindResource("ResStr_Bankbook");
                cboItem.DataContext = EnumScanMediaType._BankBook;
                cboItem.MinWidth = 145;
                cboItem.Style = this.FindResource("customComboBoxItem") as Style;
                cboMediaType.Items.Add(cboItem);

                cboItem = new ComboBoxItem();
                cboItem.Content = (string)this.TryFindResource("ResStr_Card");
                cboItem.DataContext = EnumScanMediaType._Card;
                cboItem.MinWidth = 145;
                cboItem.Style = this.FindResource("customComboBoxItem") as Style;
                cboMediaType.Items.Add(cboItem);
            }

            foreach (ComboBoxItem obj in cboMediaType.Items)
            {
                if (null != obj.DataContext
                        && (EnumScanMediaType)obj.DataContext == m_scanParams.ScanMediaType)
                {
                    obj.IsSelected = true;
                }
            }
        }

        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
                this.Close();
                e.Handled = true;
            }
        }

        private void ControlBtnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;

            if (null != btn)
            {
                if ("btnClose" == btn.Name)
                {
                    this.Close();
                }
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        /// <summary>
        /// Init document type, color type, brightness and contrast.
        /// </summary>
        private void InitControls()
        {
            if (m_scanParams.PaperSize == EnumPaperSizeScan._LongPage ||
                m_scanParams.ScanMediaType == EnumScanMediaType._BankBook ||
                m_scanParams.ScanMediaType == EnumScanMediaType._Card ||
                m_scanParams.PaperSize == EnumPaperSizeScan._A6 || 
                m_scanParams.PaperSize == EnumPaperSizeScan._Auto1)
            {
                MultiFeedOnButton.IsChecked = false;
                MultiFeedOnButton.IsEnabled = false;
                MultiFeedOffButton.IsEnabled = false;
                MultiFeedOffButton.IsChecked = true;
            }
            else
            {
                if (m_scanParams.MultiFeed == true)
                {
                    MultiFeedOnButton.IsChecked = true;
                }
                else
                {
                    MultiFeedOffButton.IsChecked = true;
                }
                MultiFeedOnButton.IsEnabled = true;
                MultiFeedOffButton.IsEnabled = true;
            }
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            double y = e.GetPosition(LayoutRoot).Y;
            bool isAtTitle = false;

            foreach (RowDefinition rd in LayoutRoot.RowDefinitions)
            {
                if (y <= rd.ActualHeight)
                {
                    isAtTitle = true;
                }

                break; // Only check the 1st row.
            }

            if (true == isAtTitle)
                this.DragMove();
        }

        private void cbo_selchg_scansize(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboScanSize.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                EnumPaperSizeScan size = (EnumPaperSizeScan)selItem.DataContext;

                if (size != m_lastPaperSize1)
                {
                    m_scanParams.PaperSize = (EnumPaperSizeScan)selItem.DataContext;

                    if (m_scanParams.PaperSize == EnumPaperSizeScan._LongPage)
                    {
                        InitMediaType();
                        
                    }
                    else
                    {
                        if (m_lastPaperSize1 == EnumPaperSizeScan._LongPage)
                            InitMediaType();
                    }
                    InitControls();

                    m_lastPaperSize1 = size;
                }
                               
            }
        }

        private void comboType_sel_change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboMediaType.SelectedItem as ComboBoxItem;

            if (null != selItem && null != selItem.DataContext)
            {
                EnumScanMediaType type = (EnumScanMediaType)selItem.DataContext;

                if (type != m_lastPaperType)
                {
                    if (m_scanParams.ScanMediaType == EnumScanMediaType._Normal)
                        m_lastPaperSize = m_scanParams.PaperSize;

                    m_scanParams.ScanMediaType = (EnumScanMediaType)selItem.DataContext;

                    if (m_scanParams.ScanMediaType == EnumScanMediaType._BankBook ||
                        m_scanParams.ScanMediaType == EnumScanMediaType._Card)
                    {
                        if (m_scanParams.PaperSize != EnumPaperSizeScan._Auto &&
                            m_scanParams.PaperSize != EnumPaperSizeScan._Auto1)
                            m_scanParams.PaperSize = EnumPaperSizeScan._Auto;

                        InitScanSize();
                        InitControls();

                        Window dlg = new MediaTypePrompt("Please switch to the Card mode on the machine!",
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Prompt"));
                        dlg.Owner = Application.Current.MainWindow;
                        dlg.ShowDialog();
                    }
                    else
                    {
                        m_scanParams.PaperSize = m_lastPaperSize;
                        InitScanSize();
                        InitControls();
                    }

                    m_lastPaperType = type;
                }
            }

        }


        public void MultiFeed_click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "MultiFeedOnButton")
                {
                    m_scanParams.MultiFeed = true;
                    InitScanSize();
                }
                else if (rdbtn.Name == "MultiFeedOffButton")
                {
                    m_scanParams.MultiFeed = false;
                    InitScanSize();
                }
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {

            m_scanParams = new ScanParamDecode();

            InitControls();
            InitScanSize();
            InitMediaType();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (m_scanParams.ScanMediaType == EnumScanMediaType._BankBook ||
                m_scanParams.ScanMediaType == EnumScanMediaType._Card ||
                m_scanParams.PaperSize == EnumPaperSizeScan._A6 ||
                m_scanParams.PaperSize == EnumPaperSizeScan._Auto1)
            {
                m_scanParams.MultiFeed = false;
            }

            if (m_scanParams.PaperSize == EnumPaperSizeScan._LongPage)
            {
                m_scanParams.ScanMediaType = EnumScanMediaType._Normal;
                m_scanParams.MultiFeed = false;
            }

            if (m_powermode > 1)
            {
                m_scanParams.ScanMediaType = EnumScanMediaType._Normal;

                if (m_powermode > 1 && m_scanParams.PaperSize == EnumPaperSizeScan._LongPage)
                {
                    if (m_scanParams.MultiFeed == false)
                        m_scanParams.PaperSize = EnumPaperSizeScan._Auto1;
                    else
                        m_scanParams.PaperSize = EnumPaperSizeScan._Auto;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

     
        private void InitScanSize()
        {
            cboScanSize.Items.Clear();

            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "Auto";
            cboItem.DataContext = EnumPaperSizeScan._Auto;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);

  
            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_A4_210_297mm_");
            cboItem.DataContext = EnumPaperSizeScan._A4;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_A5_148_x_210mm_");
            cboItem.DataContext = EnumPaperSizeScan._A5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_B5_182_x_257mm_");
            cboItem.DataContext = EnumPaperSizeScan._B5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_A6_105_x_148mm_");
            cboItem.DataContext = EnumPaperSizeScan._A6;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_Letter_8_5_x_11");
            cboItem.DataContext = EnumPaperSizeScan._Letter;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);


            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_Legal_8_5_x_14");
            cboItem.DataContext = EnumPaperSizeScan._Legal;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);
     
            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_LongPage_");
            cboItem.DataContext = EnumPaperSizeScan._LongPage;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add(cboItem);                     

            foreach (ComboBoxItem obj in cboScanSize.Items)
            {
                if (null != obj.DataContext
                        && (EnumPaperSizeScan)obj.DataContext == m_scanParams.PaperSize)
                {
                    obj.IsSelected = true;
                }
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string strText = e.Text;
 
            if (strText.Length > 0 && !Char.IsDigit(strText, 0))
            {
                e.Handled = true;
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
