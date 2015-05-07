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
    public class PaperSizeInputST
    {
        public EnumPaperSizeInput m_papersize_id;
        public double m_width;
        public double m_height;

        public PaperSizeInputST( EnumPaperSizeInput papersize_id, double width, double height )
        {
            m_papersize_id = papersize_id;
            m_width = width;
            m_height = height;
        }
    }

    public class PaperSizeOutputST
    {
        public EnumPaperSizeOutput m_papersize_id;
        public double m_width;
        public double m_height;

        public PaperSizeOutputST( EnumPaperSizeOutput papersize_id, double width, double height )
        {
            m_papersize_id = papersize_id;
            m_width = width;
            m_height = height;
        }
    }

	/// <summary>
	/// Interaction logic for CopySetting.xaml
	/// </summary>
	public partial class CopySetting : Window
	{

#region Fields
        public EnumCopyScanMode    m_scanMode   = EnumCopyScanMode.Photo;
        public EnumPaperSizeInput  m_docSize    = EnumPaperSizeInput._A4;
        public EnumPaperSizeOutput m_outputSize = EnumPaperSizeOutput._Letter;
        public EnumCopyResln       m_dpi        = EnumCopyResln._300x300;
        public EnumMediaType       m_mediaType  = EnumMediaType.Plain;

        /// <summary>
        /// Flag used to specify the CopySetting has been loaded or not.
        /// </summary>
        private bool m_isWindowLoaded = false;

        /// <summary>
        /// Previous N in 1 value, used to recovery the selected item when N in 1 check box was rechecked.
        /// This value only change during copy setting window loading and N in 1 images selecting.
        /// </summary>
        private EnumNin1 m_preNin1 = EnumNin1._1up;

        public bool m_isIDCardCopy = false; // flag present is id card copy mode or not.
#endregion

#region Properties
        private EnumNin1 _nin1 = EnumNin1._1up;
        public EnumNin1 m_nin1
        {
            get
            {
                return _nin1;
            }

            set
            {
                _nin1 = value;

                // Make sure the cboOutputSize is ready.
                if ( m_isWindowLoaded && false == m_isIDCardCopy )
                {
                    bool isNeedReselect = false;
                    foreach ( ComboBoxItem obj in cboOutputSize.Items )
                    {
                        if ( null != obj.DataContext )
                        {
                            EnumPaperSizeOutput s = (EnumPaperSizeOutput)obj.DataContext;

                            switch ( s )
                            {
                                case EnumPaperSizeOutput._Letter    :
                                    obj.IsEnabled = true;
                                    break;
                                case EnumPaperSizeOutput._A4        :
                                    obj.IsEnabled = true;
                                    break;
                                case EnumPaperSizeOutput._A5        :
                                    obj.IsEnabled = (_nin1 == EnumNin1._1up || _nin1 == EnumNin1._2up);
                                    break;
                                case EnumPaperSizeOutput._A6        :
                                    obj.IsEnabled = (_nin1 == EnumNin1._1up);
                                    break;
                                case EnumPaperSizeOutput._B5        :
                                    obj.IsEnabled = (_nin1 == EnumNin1._1up || _nin1 == EnumNin1._2up);
                                    break;
                                case EnumPaperSizeOutput._B6        :
                                    obj.IsEnabled = (_nin1 == EnumNin1._1up);
                                    break;
                                case EnumPaperSizeOutput._Executive :
                                    obj.IsEnabled = (_nin1 == EnumNin1._1up || _nin1 == EnumNin1._2up);
                                    break;
                                case EnumPaperSizeOutput._16K       :
                                    obj.IsEnabled = (_nin1 == EnumNin1._1up || _nin1 == EnumNin1._2up);
                                    break;
                                default:
                                    obj.IsEnabled = true;
                                    break;
                            }

                            if ( true == obj.IsSelected && false == obj.IsEnabled )
                            {
                                isNeedReselect = true;
                            }
                        }
                    }

                    if ( isNeedReselect )
                    {
                        // select default item.
                        cboOutputSize.SelectedIndex = 0;
                    }
                }
            }
        }

#endregion

		public CopySetting()
		{
			this.InitializeComponent();

            this.Width = this.Width*  App.gScalingRate;
            this.Height = this.Height*App.gScalingRate;

            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
			// Insert code required on object creation below this point.
		}

        public void MyMouseButtonEventHandler( Object sender, MouseButtonEventArgs e)
        {
            double y = e.GetPosition(LayoutRoot).Y;
            bool isAtTitle = false;

            foreach(RowDefinition rd in LayoutRoot.RowDefinitions)
            {
                if ( y <= rd.ActualHeight )
                {
                    isAtTitle = true;
                }

                break; // Only check the 1st row.
            }

            if ( true == isAtTitle )
                this.DragMove();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cboDocSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboDocSize.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_docSize = (EnumPaperSizeInput)selItem.DataContext;
                CalculateScaling();
            }
        }

        private void cboResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboResolution.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_dpi = (EnumCopyResln)selItem.DataContext;
            }
        }

        private void cboOutputSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboOutputSize.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_outputSize = (EnumPaperSizeOutput)selItem.DataContext;
                CalculateScaling();
            }

        }

        private void cboMediaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboMediaType.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_mediaType = (EnumMediaType)selItem.DataContext;
            }
        }

        private void chkNin1_Checked(object sender, RoutedEventArgs e)
        {
            if ( m_isWindowLoaded )
            {
                if ( EnumNin1._4up == m_preNin1 || EnumNin1._9up == m_preNin1 )
                    m_nin1 = m_preNin1;
                else
                    m_nin1 = EnumNin1._2up;

                InitNin1();
                spinnerScaling.IsEnabled = EnumNin1._1up == m_nin1;
            }
        }

        private void chkNin1_Unchecked(object sender, RoutedEventArgs e)
        {
            if ( m_isWindowLoaded )
            {
                m_nin1 = EnumNin1._1up;
                spinnerScaling.IsEnabled = EnumNin1._1up == m_nin1;
                InitNin1();
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            cboDocSize.Items.Clear();
            cboResolution.Items.Clear();
            cboOutputSize.Items.Clear();
            cboMediaType.Items.Clear();

            bool bIsMetrice = dll.IsMetricCountry();

            spinnerScaling.Value = 100;
            m_scanMode   = EnumCopyScanMode.Photo;

            if ( bIsMetrice )
            {
                m_docSize    = EnumPaperSizeInput._A4;
                m_outputSize = EnumPaperSizeOutput._A4;
            }
            else
            {
                m_docSize    = EnumPaperSizeInput._Letter;
                m_outputSize = EnumPaperSizeOutput._Letter;
            }

            m_nin1       = EnumNin1._1up;
            m_dpi        = m_isIDCardCopy ? EnumCopyResln._600x600 : EnumCopyResln._300x300;
            m_mediaType  = EnumMediaType.Plain;
            m_preNin1 = EnumNin1._2up;

            InitCboDocSize();
            InitCboResolution();
            InitCboOutputSize();
            InitCboMediaType();
            InitNin1();

            if ( EnumCopyScanMode.Text == m_scanMode )
            {
                rdBtnScanModeTxt.IsChecked = true;
                rdBtnScanModePhoto.IsChecked = false;
            }
            else if ( EnumCopyScanMode.Photo == m_scanMode )
            {
                rdBtnScanModeTxt.IsChecked = false;
                rdBtnScanModePhoto.IsChecked = true;
            }

        }

        private void rdBtnScanModePhoto_Checked(object sender, RoutedEventArgs e)
        {
            m_scanMode = EnumCopyScanMode.Photo;
        }

        private void rdBtnScanModeTxt_Checked(object sender, RoutedEventArgs e)
        {
            m_scanMode = EnumCopyScanMode.Text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ( EnumCopyScanMode.Text == m_scanMode )
            {
                rdBtnScanModeTxt.IsChecked = true;
                rdBtnScanModePhoto.IsChecked = false;
            }
            else if ( EnumCopyScanMode.Photo == m_scanMode )
            {
                rdBtnScanModeTxt.IsChecked = false;
                rdBtnScanModePhoto.IsChecked = true;
            }


            decimal nOriginalScaling = spinnerScaling.Value;
            
            InitCboDocSize();
            InitCboResolution();
            InitCboOutputSize();
            InitCboMediaType();
            InitNin1();
            m_preNin1 = m_nin1;

            m_isWindowLoaded = true;
            spinnerScaling.Value = nOriginalScaling;

            if ( m_isIDCardCopy )
            {
                spinnerScaling.IsEnabled = false;
                spinnerScaling.Value = 100;
            }

            if ( EnumNin1._1up != m_nin1 )
                spinnerScaling.IsEnabled = false;

            TextBox tb = spinnerScaling.Template.FindName("tbTextBox", spinnerScaling) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
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

        private void SpinnerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            int textValue = 0;

            if (int.TryParse(textBox.Text, out textValue))
            {

            }

        }
        /// <summary>
        /// Initialize original document size combobox.
        /// </summary>
        private void InitCboDocSize()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_A4" );
            cboItem.DataContext = EnumPaperSizeInput._A4; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_A5" );
            cboItem.DataContext = EnumPaperSizeInput._A5; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_B5" );
            cboItem.DataContext = EnumPaperSizeInput._B5; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Letter" );
            cboItem.DataContext = EnumPaperSizeInput._Letter; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Executive" );
            cboItem.DataContext = EnumPaperSizeInput._Executive; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            foreach ( ComboBoxItem obj in cboDocSize.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumPaperSizeInput)obj.DataContext == m_docSize )
                {
                    obj.IsSelected = true;
                }
            }

            if ( m_isIDCardCopy )
            {
                foreach ( ComboBoxItem obj in cboDocSize.Items )
                {
                    obj.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Initialize resolution combobox.
        /// </summary>
        private void InitCboResolution()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "300 x 300dpi" ;
            cboItem.DataContext = EnumCopyResln._300x300;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboResolution.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "600 x 600dpi" ;
            cboItem.DataContext = EnumCopyResln._600x600;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboResolution.Items.Add( cboItem );

            foreach ( ComboBoxItem obj in cboResolution.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumCopyResln)obj.DataContext == m_dpi )
                {
                    obj.IsSelected = true;
                }
            }

            if ( m_isIDCardCopy )
            {
                foreach ( ComboBoxItem obj in cboResolution.Items )
                {
                    if ( (EnumCopyResln)obj.DataContext != EnumCopyResln._600x600 )
                        obj.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Initialize output paper size combobox.
        /// </summary>
        private void InitCboOutputSize()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Letter" );
            cboItem.DataContext = EnumPaperSizeOutput._Letter;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_A4" );
            cboItem.DataContext = EnumPaperSizeOutput._A4;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_A5" );
            cboItem.DataContext = EnumPaperSizeOutput._A5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_A6" );
            cboItem.DataContext = EnumPaperSizeOutput._A6;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_B5" );
            cboItem.DataContext = EnumPaperSizeOutput._B5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_B6" );
            cboItem.DataContext = EnumPaperSizeOutput._B6;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Executive" );
            cboItem.DataContext = EnumPaperSizeOutput._Executive;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_16K" );
            cboItem.DataContext = EnumPaperSizeOutput._16K;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            foreach ( ComboBoxItem obj in cboOutputSize.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumPaperSizeOutput)obj.DataContext == m_outputSize )
                {
                    obj.IsSelected = true;
                }
            }

            if ( m_isIDCardCopy )
            {
                foreach ( ComboBoxItem obj in cboOutputSize.Items )
                {
                    if ( null != obj.DataContext )
                    {
                        EnumPaperSizeOutput s = (EnumPaperSizeOutput)obj.DataContext;

                        if ( EnumPaperSizeOutput._A6 == s || EnumPaperSizeOutput._B6 == s )
                        {
                            obj.IsEnabled = false;
                        }
                        else
                        {
                            obj.IsEnabled = true;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Initialize media type combobox.
        /// </summary>
        private void InitCboMediaType()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Plain_Paper" );
            cboItem.DataContext = EnumMediaType.Plain;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Recycled_Paper" );
            cboItem.DataContext = EnumMediaType.Recycled;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Thick_Paper" );
            cboItem.DataContext = EnumMediaType.Thin;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Thin_Paper" );
            cboItem.DataContext = EnumMediaType.Thick;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource( "ResStr_Label" );
            cboItem.DataContext = EnumMediaType.Label;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            foreach ( ComboBoxItem obj in cboMediaType.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumMediaType)obj.DataContext == m_mediaType )
                {
                    obj.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Initialize items for n in 1.
        /// </summary>
        private void InitNin1()
        {
            SolidColorBrush brSelected = new SolidColorBrush();
            SolidColorBrush brUnselected = new SolidColorBrush();

            brSelected.Color = Color.FromArgb( 255, 168, 168, 168 );
            brUnselected.Color = Color.FromArgb(255, 235, 235, 235);

            rectNin1_2.Fill = brUnselected;
            rectNin1_4.Fill = brUnselected;
            rectNin1_9.Fill = brUnselected;

            if ( EnumNin1._1up == m_nin1 )
            {
                chkNin1.IsChecked = false;
            }
            else
            {
                chkNin1.IsChecked = true;

                if ( EnumNin1._2up == m_nin1 )
                {
                    rectNin1_2.Fill = brSelected;
                }
                else if ( EnumNin1._4up == m_nin1 )
                {
                    rectNin1_4.Fill = brSelected;
                }
                else if ( EnumNin1._9up == m_nin1 )
                {
                    rectNin1_9.Fill = brSelected;
                }
            }

            if ( m_isIDCardCopy )
                chkNin1.IsEnabled = false;
        }

        private void imgNin1_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = EnumNin1._2up;
                m_nin1 = EnumNin1._2up;
                InitNin1();
                spinnerScaling.IsEnabled = EnumNin1._1up == m_nin1;
            }
        }

        private void imgNin1_4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = EnumNin1._4up;
                m_nin1 = EnumNin1._4up;
                InitNin1();
                spinnerScaling.IsEnabled = EnumNin1._1up == m_nin1;
            }
        }

        private void imgNin1_9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = EnumNin1._9up;
                m_nin1 = EnumNin1._9up;
                InitNin1();
                spinnerScaling.IsEnabled = EnumNin1._1up == m_nin1;
            }
        }

        private void OnScalingValidationHasError(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            btnApply.IsEnabled = ( false == spinnerScaling.ValidationHasError );
        }

        private void SpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        { 
            VOP.Controls.SpinnerControl spinnerCtl = sender as VOP.Controls.SpinnerControl;
            TextBox tb = spinnerCtl.Template.FindName("tbTextBox", spinnerCtl) as TextBox;
            int textValue = 0;
            if (!spinnerCtl.IsFocused)
            {
                if ( "spinnerScaling" == spinnerCtl.Name ) 
                {
                    if (int.TryParse(tb.Text, out textValue))
                    {
                        if ( textValue > 400 )
                            tb.Text = "400";
                        else if ( textValue < 25 )
                            tb.Text = "25";
                    }
                    else
                    {
                        tb.Text = "100";
                    }
                }
            }
        }

        /// <summary>
        /// Calculate and assign the scaling value to spinner control. 
        /// </summary>
        private void CalculateScaling()
        {
            if ( null != cboDocSize 
                    && null != cboOutputSize
                    && true != m_isIDCardCopy )
            {

                PaperSizeInputST[] input_papersize_infos = 
                {
                    new PaperSizeInputST( EnumPaperSizeInput._A4        , 210 , 297 ) ,
                    new PaperSizeInputST( EnumPaperSizeInput._A5        , 148 , 210 ) ,
                    new PaperSizeInputST( EnumPaperSizeInput._B5        , 182 , 257 ) ,
                    new PaperSizeInputST( EnumPaperSizeInput._Letter    , 216 , 279 ) ,
                    new PaperSizeInputST( EnumPaperSizeInput._Executive , 184 , 267 ) ,
                };

                PaperSizeOutputST[] output_papersize_infos = 
                {
                    new PaperSizeOutputST( EnumPaperSizeOutput._A4        , 210 , 297 ) ,
                    new PaperSizeOutputST( EnumPaperSizeOutput._A5        , 148 , 210 ) ,
                    new PaperSizeOutputST( EnumPaperSizeOutput._B5        , 182 , 257 ) ,
                    new PaperSizeOutputST( EnumPaperSizeOutput._A6        , 105 , 148 ) ,
                    new PaperSizeOutputST( EnumPaperSizeOutput._B6        , 128 , 182 ) ,
                    new PaperSizeOutputST( EnumPaperSizeOutput._Letter    , 216 , 279 ) ,
                    new PaperSizeOutputST( EnumPaperSizeOutput._Executive , 184 , 267 ) ,
                    new PaperSizeOutputST( EnumPaperSizeOutput._16K       , 185 , 260 ) ,
                };

                int idxInput = 0;
                int idxOutput = 0;

                // get selected item of input paper size
                EnumPaperSizeInput valInput = EnumPaperSizeInput._A4;
                ComboBoxItem selectedItemInput = (ComboBoxItem)cboDocSize.SelectedItem;
                if ( null != selectedItemInput && null != selectedItemInput.DataContext )
                    valInput = (EnumPaperSizeInput)selectedItemInput.DataContext;

                // get selected item of output paper size
                EnumPaperSizeOutput valOutput = EnumPaperSizeOutput._A4;
                ComboBoxItem selectedItemOutput = (ComboBoxItem)cboOutputSize.SelectedItem;
                if ( null != selectedItemOutput && null != selectedItemOutput.DataContext )
                    valOutput = (EnumPaperSizeOutput)selectedItemOutput.DataContext;

                // get the index of input paper size
                for (int i = 0; i < input_papersize_infos.Length; i++)
                {
                    if ( valInput == input_papersize_infos[i].m_papersize_id )
                    {
                        idxInput = i;
                        break;
                    }
                }

                // get the index of output paper size
                for (int i = 0; i < output_papersize_infos.Length; i++)
                {
                    if ( valOutput == output_papersize_infos[i].m_papersize_id )
                    {
                        idxOutput = i;
                        break;
                    }
                }

                // according the phone call from Beijing, the margin is 4.2 mm
                double scaling1 = (output_papersize_infos[idxOutput].m_width-4.2*2)/(input_papersize_infos[idxInput].m_width-4.2*2);
                double scaling2 = (output_papersize_infos[idxOutput].m_height-4.2*2)/(input_papersize_infos[idxInput].m_height-4.2*2);
                double scaling = scaling1 < scaling2 ? scaling1 : scaling2;

                spinnerScaling.Value = (decimal)(scaling*100);
            }

        }
	}
}
