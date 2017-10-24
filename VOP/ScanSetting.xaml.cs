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

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanSetting.xaml
    /// </summary>
    public partial class ScanSetting : Window
    {

#region scan parameters
        public EnumScanDocType   m_docutype   = EnumScanDocType.Photo;
        public EnumScanResln     m_scanResln  = EnumScanResln._300x300;
        public EnumPaperSizeScan m_paperSize  = EnumPaperSizeScan._A4;
        public EnumColorType     m_color      = EnumColorType.color_24bit;
        public int               m_brightness = 50;
        public int               m_contrast   = 50;
#endregion

        public ScanSetting()
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

            TextBox tb = spinCtlConstrast.Template.FindName("tbTextBox", spinCtlConstrast) as TextBox;
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            TextBox tb1 = spinCtlBrightness.Template.FindName("tbTextBox", spinCtlBrightness) as TextBox;
            tb1.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb1.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

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
            switch(m_docutype)
            {
                case EnumScanDocType.Photo:
                    docutype_photo.IsChecked = true;
                    break;
                case EnumScanDocType.Text:
                    docutype_text.IsChecked = true;
                    break;
                case EnumScanDocType.Graphic:
                    docutype_graphic.IsChecked = true;
                    break;                   
            }

            switch(m_color)
            {
                case EnumColorType.color_24bit:
                    Color.IsChecked = true;
                    break;
                case EnumColorType.grayscale_8bit:
                    Grayscale.IsChecked = true;
                    break;
                case EnumColorType.black_white:
                    BlackAndWhite.IsChecked = true;
                    break;     
            }

            sldr_brightness.Value = m_brightness;
            sldr_contrast.Value = m_contrast;
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


        public void docutype_click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "docutype_photo")
                {
                    m_docutype = EnumScanDocType.Photo;
                }
                else if (rdbtn.Name == "docutype_text")
                {
                    m_docutype = EnumScanDocType.Text;
                }
                else if (rdbtn.Name == "docutype_graphic")
                {
                    m_docutype = EnumScanDocType.Graphic;
                }
            }
        }


        private void cbo_selchg_scansize(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboScanSize.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_paperSize = (EnumPaperSizeScan)selItem.DataContext;
            }
        }

        private void combo_sel_change(object sender, SelectionChangedEventArgs e)
        {    
            ComboBoxItem selItem = cboScanResln.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_scanResln = (EnumScanResln)selItem.DataContext;
            }

        }

        private void colorMode_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "Color")
                {
                    m_color = EnumColorType.color_24bit;
                }
                else if (rdbtn.Name == "Grayscale")
                {
                    m_color = EnumColorType.grayscale_8bit;
                }
                else if (rdbtn.Name == "BlackAndWhite")
                {
                    m_color = EnumColorType.black_white;
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
                    m_brightness = val;
                }
                else if (sldr.Name == "sldr_contrast")
                {
                    m_contrast = val;
                }
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            m_docutype   = EnumScanDocType.Photo;
            m_scanResln  = EnumScanResln._300x300;
            m_paperSize  = EnumPaperSizeScan._A4;
            m_color      = EnumColorType.color_24bit;
            m_brightness = 50;
            m_contrast   = 50;

            InitControls();
            InitScanResln();
            InitScanSize();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void InitScanResln()
        {
            cboScanResln.Items.Clear();

            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "100 x 100dpi";
            cboItem.DataContext = EnumScanResln._100x100;
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

            cboItem = new ComboBoxItem();
            cboItem.Content = "1200 x 1200dpi" ;
            cboItem.DataContext = EnumScanResln._1200x1200;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            foreach ( ComboBoxItem obj in cboScanResln.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumScanResln)obj.DataContext == m_scanResln )
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
            cboItem.Content = (string)this.TryFindResource("ResStr_A4_210_297mm_");           
            cboItem.DataContext = EnumPaperSizeScan._A4;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_A5_148_x_210mm_");
            cboItem.DataContext = EnumPaperSizeScan._A5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_B5_182_x_257mm_");
            cboItem.DataContext = EnumPaperSizeScan._B5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_Letter_8_5_x_11");
            cboItem.DataContext = EnumPaperSizeScan._Letter;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.TryFindResource("ResStr_4_x_6_");
            cboItem.DataContext = EnumPaperSizeScan._4x6Inch;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            foreach ( ComboBoxItem obj in cboScanSize.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumPaperSizeScan)obj.DataContext == m_paperSize )
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
                Color.FontSize = Grayscale.FontSize = BlackAndWhite.FontSize = 8.0;
                btnDefault.FontSize = btnOk.FontSize = 14;
            }
        }

        private void OnValidationHasErrorChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            btnOk.IsEnabled = ( false == spinCtlBrightness.ValidationHasError
                    && false == spinCtlConstrast.ValidationHasError );
        }
    }
}
