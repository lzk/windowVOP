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
    public partial class ScanSettingDialog : Window
    {

//#region scan parameters
//        public bool   m_adfMode   = true;
//        public EnumScanResln     m_scanResln  = EnumScanResln._300x300;
//        public EnumPaperSizeScan m_paperSize  = EnumPaperSizeScan._Auto;
//        public EnumColorType     m_color      = EnumColorType.color_24bit;
//        public int               m_brightness = 50;
//        public int               m_contrast   = 50;
//        public bool m_MultiFeed = true;
//        public bool m_AutoCrop = true;
//        #endregion

        public ScanParam m_scanParams = new ScanParam();
        private RepeatButton btnConstrastDecrease;
        private RepeatButton btnConstrastIncrease;
        private RepeatButton btnBrightnessDecrease;
        private RepeatButton btnBrightnessIncrease;

        public ScanSettingDialog()
        {
            this.InitializeComponent();

            this.Width = this.Width*  App.gScalingRate;
            this.Height = this.Height*App.gScalingRate;

            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
            // Insert code required on object creation below this point.
        }

        public void handler_loaded( object sender, RoutedEventArgs e )
        {
            InitControls();
            InitScanResln();
            InitScanSize();
            InitFontSize();
            InitMediaType();

            TextBox tb = spinCtlConstrast.Template.FindName("tbTextBox", spinCtlConstrast) as TextBox;
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            btnConstrastDecrease = spinCtlConstrast.Template.FindName("btnDecrease", spinCtlConstrast) as RepeatButton;
            btnConstrastIncrease = spinCtlConstrast.Template.FindName("btnIncrease", spinCtlConstrast) as RepeatButton;
            btnBrightnessDecrease = spinCtlConstrast.Template.FindName("btnDecrease", spinCtlBrightness) as RepeatButton;
            btnBrightnessIncrease = spinCtlConstrast.Template.FindName("btnIncrease", spinCtlBrightness) as RepeatButton;

            CheckContrastValue(); 
            CheckBrightnessValue();
            TextBox tb1 = spinCtlBrightness.Template.FindName("tbTextBox", spinCtlBrightness) as TextBox;
            tb1.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb1.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
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

        private void SpinnerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int textValue = 0;

            if (!int.TryParse(e.Text, out textValue))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Init document type, color type, brightness and contrast.
        /// </summary>
        private void InitControls()
        {
            //switch(m_docutype)
            //{
            //    case EnumScanDocType.Photo:
            //        docutype_photo.IsChecked = true;
            //        break;
            //    case EnumScanDocType.Text:
            //        docutype_text.IsChecked = true;
            //        break;
            //    case EnumScanDocType.Graphic:
            //        docutype_graphic.IsChecked = true;
            //        break;                   
            //}

            if (m_scanParams.ADFMode == true)
            {
                twoSideButton.IsChecked = true;
            }
            else
            {
                oneSideButton.IsChecked = true;
            }

            if (m_scanParams.MultiFeed == true)
            {
                MultiFeedOnButton.IsChecked = true;
            }
            else
            {
                MultiFeedOffButton.IsChecked = true;
            }

            if (m_scanParams.AutoCrop == true)
            {
                AutoCropOnButton.IsChecked = true;
            }
            else
            {
                AutoCropOffButton.IsChecked = true;
            }

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

            sldr_brightness.Value = m_scanParams.Brightness;
            sldr_contrast.Value = m_scanParams.Contrast;
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

        public void MultiFeed_click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "MultiFeedOnButton")
                {
                    m_scanParams.MultiFeed = true;
                }
                else if (rdbtn.Name == "MultiFeedOffButton")
                {
                    m_scanParams.MultiFeed = false;
                }
            }
        }

        public void AutoCrop_click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "AutoCropOnButton")
                {
                    m_scanParams.AutoCrop = true;
                }
                else if (rdbtn.Name == "AutoCropOffButton")
                {
                    m_scanParams.AutoCrop = false;
                }
            }
        }

        private void cbo_selchg_scansize(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboScanSize.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_scanParams.PaperSize = (EnumPaperSizeScan)selItem.DataContext;
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


        private void comboType_sel_change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboMediaType.SelectedItem as ComboBoxItem;

            if (null != selItem && null != selItem.DataContext)
            {
                m_scanParams.ScanMediaType = (EnumScanMediaType)selItem.DataContext;
            }

            if (m_scanParams.ScanMediaType == EnumScanMediaType._BankBook ||
                m_scanParams.ScanMediaType == EnumScanMediaType._Card)
            {
                InitScanSize();

                MultiFeedOffButton.IsChecked = true;
                MultiFeedOnButton.IsEnabled = false;
                MultiFeedOffButton.IsEnabled = false;
            }
            else
            {
                InitScanSize();

                if (m_scanParams.MultiFeed == true)
                {
                    MultiFeedOnButton.IsChecked = true;
                    MultiFeedOffButton.IsChecked = false;
                }
                else
                {
                    MultiFeedOnButton.IsChecked = false;
                    MultiFeedOffButton.IsChecked = true;
                }

                MultiFeedOnButton.IsEnabled = true;
                MultiFeedOffButton.IsEnabled = true;
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
        
        private void SpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VOP.Controls.SpinnerControl spinnerCtl = sender as VOP.Controls.SpinnerControl;
            TextBox tb = spinnerCtl.Template.FindName("tbTextBox", spinnerCtl) as TextBox;
            int textValue = 0;
            if (!spinnerCtl.IsFocused)
            {
                if ("spinCtlBrightness" == spinnerCtl.Name
                        || "spinCtlConstrast" == spinnerCtl.Name)
                {
                    if (int.TryParse(tb.Text, out textValue))
                    {
                        if (textValue > 100)
                            tb.Text = "100";
                        else if (textValue <= 0)
                            tb.Text = "0";
                    }
                    else
                    {
                        tb.Text = "50";
                    }
                }
            }
        }

        public void sldr_value_change<T>(Object sender, RoutedPropertyChangedEventArgs<T> e)
        {
            Slider sldr = sender as Slider;

            // tb_brightness maybe null. Because the tb_brightness was defined
            // after sldr in xaml.
            if (null != sldr)
            {
                int val = (int)(sldr.Value+0.5);
                if (sldr.Name == "sldr_brightness")
                {
                    m_scanParams.Brightness = val;
                    if (btnBrightnessDecrease != null && btnBrightnessIncrease != null)
                    {
                        CheckBrightnessValue();
                    }
                }
                else if (sldr.Name == "sldr_contrast")
                {
                    m_scanParams.Contrast = val;
                    if (btnConstrastDecrease != null && btnConstrastIncrease != null)
                    {
                        CheckContrastValue();
                    }
                }
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            //m_adfMode    = true;
            //m_scanResln  = EnumScanResln._300x300;
            //m_paperSize  = EnumPaperSizeScan._Auto;
            //m_color      = EnumColorType.color_24bit;
            //m_brightness = 50;
            //m_contrast   = 50;
            //m_MultiFeed = true;
            //m_AutoCrop = true;

            m_scanParams = new ScanParam();

            InitControls();
            InitScanResln();
            InitScanSize();
            InitMediaType();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        private void CheckContrastValue() // BMS #1195
        {
            if (m_scanParams.Contrast == spinCtlConstrast.Minimum)
            {
                btnConstrastDecrease.IsEnabled = false;
                btnConstrastIncrease.IsEnabled = true;
            }
            else if (m_scanParams.Contrast == spinCtlConstrast.Maximum)
            {
                btnConstrastIncrease.IsEnabled = false;
                btnConstrastDecrease.IsEnabled = true;
            }
            else
            {
                btnConstrastIncrease.IsEnabled = true;
                btnConstrastDecrease.IsEnabled = true;
            }
        }
        private void CheckBrightnessValue() // BMS #1195
        {
            if (m_scanParams.Brightness == spinCtlBrightness.Minimum)
            {
                btnBrightnessDecrease.IsEnabled = false;
                btnBrightnessIncrease.IsEnabled = true;
            }
            else if (m_scanParams.Brightness == spinCtlBrightness.Maximum)
            {
                btnBrightnessIncrease.IsEnabled = false;
                btnBrightnessDecrease.IsEnabled = true;
            }
            else
            {
                btnBrightnessIncrease.IsEnabled = true;
                btnBrightnessDecrease.IsEnabled = true;
            }
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

            cboItem = new ComboBoxItem();
            cboItem.Content = "600 x 600dpi" ;
            cboItem.DataContext = EnumScanResln._600x600;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            //cboItem = new ComboBoxItem();
            //cboItem.Content = "1200 x 1200dpi" ;
            //cboItem.DataContext = EnumScanResln._1200x1200;
            //cboItem.MinWidth = 145;
            //cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            //cboScanResln.Items.Add( cboItem );

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

            if (m_scanParams.ScanMediaType != EnumScanMediaType._BankBook &&
            m_scanParams.ScanMediaType != EnumScanMediaType._Card)
            {
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

                //cboItem = new ComboBoxItem();
                //cboItem.Content = (string)this.TryFindResource("ResStr_4_x_6_");
                //cboItem.DataContext = EnumPaperSizeScan._4x6Inch;
                //cboItem.MinWidth = 145;
                //cboItem.Style = this.FindResource("customComboBoxItem") as Style;
                //cboScanSize.Items.Add( cboItem );
            }

            foreach ( ComboBoxItem obj in cboScanSize.Items )
            {
                if ( null != obj.DataContext 
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

            foreach (ComboBoxItem obj in cboMediaType.Items)
            {
                if (null != obj.DataContext
                        && (EnumScanMediaType)obj.DataContext == m_scanParams.ScanMediaType)
                {
                    obj.IsSelected = true;
                }
            }
        }

        private void OnValidationHasErrorChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {          
            btnOk.IsEnabled = ( false == spinCtlBrightness.ValidationHasError
                    && false == spinCtlConstrast.ValidationHasError );
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
