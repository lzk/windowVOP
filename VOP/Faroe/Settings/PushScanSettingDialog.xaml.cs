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
    public partial class PushScanSettingDialog : Window
    {
        public ScanParamShort m_scanParams = new ScanParamShort();
        private EnumPaperSizeScan m_lastPaperSize = EnumPaperSizeScan._Auto;
        private EnumPaperSizeScan m_lastPaperSize1 = EnumPaperSizeScan._Auto;
        private EnumScanResln m_lastRes = EnumScanResln._200x200;
        private int m_mode = 0;

        public PushScanSettingDialog(int mode)
        {
            this.InitializeComponent();

            this.Width = this.Width*  App.gScalingRate;
            this.Height = this.Height*App.gScalingRate;

            m_mode = mode;

            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
            // Insert code required on object creation below this point.            
        }

        public void handler_loaded( object sender, RoutedEventArgs e )
        {
            m_lastPaperSize = m_scanParams.PaperSize;
            m_lastPaperSize1 = m_scanParams.PaperSize;
            m_lastRes = m_scanParams.ScanResolution;

            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            ScanParametersRecord m_rec = new ScanParametersRecord(0, 0, 0, 0, 0);
            if (worker.InvokeMethod<ScanParametersRecord>("", ref m_rec, DllMethodType.GetScanParameters, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    switch (m_rec.Size)
                    {
                        case 0:
                            m_scanParams.PaperSize = EnumPaperSizeScan._Auto;
                            break;
                        case 1:
                            m_scanParams.PaperSize = EnumPaperSizeScan._A4;
                            break;

                        case 2:
                            m_scanParams.PaperSize = EnumPaperSizeScan._Letter;
                            break;

                        case 3:
                            m_scanParams.PaperSize = EnumPaperSizeScan._Legal;
                            break;

                        case 4:
                            m_scanParams.PaperSize = EnumPaperSizeScan._B5;
                            break;

                        case 5:
                            m_scanParams.PaperSize = EnumPaperSizeScan._A5;
                            break;

                        case 6:
                            m_scanParams.PaperSize = EnumPaperSizeScan._A6;
                            break;

                        default:
                            m_scanParams.PaperSize = EnumPaperSizeScan._Auto;
                            break;

                    }

                    if (m_rec.Duplex == 0)
                        m_scanParams.ADFMode = true;
                    else
                        m_scanParams.ADFMode = false;

                    switch (m_rec.Resolution)
                    {
                        case 1:
                            m_scanParams.ScanResolution = EnumScanResln._200x200;
                            break;

                        case 0:
                            m_scanParams.ScanResolution = EnumScanResln._150x150;
                            break;

                        case 2:
                            m_scanParams.ScanResolution = EnumScanResln._300x300;
                            break;

                        case 3:
                            m_scanParams.ScanResolution = EnumScanResln._600x600;
                            break;

                        default:
                            m_scanParams.ScanResolution = EnumScanResln._200x200;
                            break;
                    }

                    if (m_rec.ColorMode == 0)
                        m_scanParams.ColorType = EnumColorType.color_24bit;
                    else
                        m_scanParams.ColorType = EnumColorType.grayscale_8bit;

                    if (m_rec.FileFormat == 0)
                        m_scanParams.FileFormat = EnumFileFormat.JPEG;
                    else
                        m_scanParams.FileFormat = EnumFileFormat.PDF;
                }
            }

            InitControls();
            InitScanResln();
            InitScanSize();
            InitFontSize();

            twoSideButton.Focus();

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

            switch (m_scanParams.ColorType)
            {
                case EnumColorType.color_24bit:
                    Color.IsChecked = true;
                    break;
                case EnumColorType.grayscale_8bit:
                    Grayscale.IsChecked = true;
                    break;
                case EnumColorType.black_white:
                  //  BlackAndWhite.IsChecked = true;
                    break;     
            }

            if (m_scanParams.ADFMode == true)
            {
                twoSideButton.IsChecked = true;
            }
            else
            {
                oneSideButton.IsChecked = true;
            }

            if (m_mode == 0)
            {
                JPEGOnButton.IsChecked = true;
                PDFOnButton.IsEnabled = false;

            }
            else
            {
                if (m_scanParams.FileFormat == EnumFileFormat.JPEG)
                    JPEGOnButton.IsChecked = true;
                else
                    PDFOnButton.IsChecked = true;

                PDFOnButton.IsEnabled = true;
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


        public void ADFMode_click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "twoSideButton")
                {
                    m_scanParams.ADFMode = true;
                }
                else if (rdbtn.Name == "oneSideButton")
                {
                    m_scanParams.ADFMode = false;
                }
            }
        }

        private void cbo_selchg_scansize(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboScanSize.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                EnumPaperSizeScan size = (EnumPaperSizeScan)selItem.DataContext;

                if (size != m_lastPaperSize1)
                {
                    if (m_scanParams.PaperSize != EnumPaperSizeScan._LongPage)
                    {
                        m_lastRes = m_scanParams.ScanResolution;
                    }

                    m_scanParams.PaperSize = (EnumPaperSizeScan)selItem.DataContext;

                    if (m_scanParams.PaperSize == EnumPaperSizeScan._LongPage)
                    {
                        m_scanParams.ScanResolution = EnumScanResln._200x200;

                        InitScanResln();
 
                    }
                    else
                    {
                        m_scanParams.ScanResolution = m_lastRes;

                        InitScanResln();
                    }

                    m_lastPaperSize1 = size;
                }
                               
            }
        }

        private void combo_sel_change(object sender, SelectionChangedEventArgs e)
        {    
            ComboBoxItem selItem = cboScanResln.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_scanParams.ScanResolution = (EnumScanResln)selItem.DataContext;
            }

        }

       
        private void colorMode_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "Color")
                {
                    m_scanParams.ColorType = EnumColorType.color_24bit;
                }
                else if (rdbtn.Name == "Grayscale")
                {
                    m_scanParams.ColorType = EnumColorType.grayscale_8bit;
                }
                else if (rdbtn.Name == "BlackAndWhite")
                {
                    m_scanParams.ColorType = EnumColorType.black_white;
                }
            }
        }


        private void FileFormat_click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "JPEGOnButton")
                {
                    m_scanParams.FileFormat = EnumFileFormat.JPEG;
                }
                else 
                {
                    m_scanParams.FileFormat = EnumFileFormat.PDF;
                }
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {

            m_scanParams = new ScanParamShort();

            InitControls();
            InitScanResln();
            InitScanSize();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (m_scanParams.PaperSize == EnumPaperSizeScan._LongPage)
            {               
            }

            this.DialogResult = true;
            this.Close();
        }

        private void InitScanResln()
        {
            cboScanResln.Items.Clear();

            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "150 x 150dpi";
            cboItem.DataContext = EnumScanResln._150x150;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = "200 x 200dpi" ;
            cboItem.DataContext = EnumScanResln._200x200;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "300 x 300dpi" ;
            cboItem.DataContext = EnumScanResln._300x300;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            if (m_scanParams.PaperSize != EnumPaperSizeScan._LongPage)
            {
                cboItem = new ComboBoxItem();
                cboItem.Content = "600 x 600dpi";
                cboItem.DataContext = EnumScanResln._600x600;
                cboItem.MinWidth = 145;
                cboItem.Style = this.FindResource("customComboBoxItem") as Style;
                cboScanResln.Items.Add(cboItem);
            }

            foreach ( ComboBoxItem obj in cboScanResln.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumScanResln)obj.DataContext == m_scanParams.ScanResolution)
                {
                    obj.IsSelected = true;
                }
            }
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

        private void InitFontSize()
        {            
            if (App.LangId == 0x804) // zh-CN
            {
                btnDefault.FontSize = btnOk.FontSize = 17.87;
            }
            else// en-US
            {
                Color.FontSize = Grayscale.FontSize = 8.0;
                btnDefault.FontSize = btnOk.FontSize = 14;
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
