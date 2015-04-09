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
	/// Interaction logic for CopySetting.xaml
	/// </summary>
	public partial class CopySetting : Window
	{

#region Fields
        public EnumCopyScanMode    m_scanMode   = EnumCopyScanMode.Photo;
        public EnumPaperSizeInput  m_docSize    = EnumPaperSizeInput._A4;
        public EnumPaperSizeOutput m_outputSize = EnumPaperSizeOutput._Letter;
        public EnumResln           m_dpi        = EnumResln._300x300;
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

        private ushort _scaling = 100;
        public ushort m_scaling
        {
            set
            {
                if ( value >= 25 && value <= 400 )
                {
                    _scaling = value;
                }
            }

            get
            {
                return _scaling;
            }
        }

#endregion

		public CopySetting()
		{
			this.InitializeComponent();
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
            }
        }

        private void cboResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboResolution.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_dpi = (EnumResln)selItem.DataContext;
            }
        }

        private void cboOutputSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboOutputSize.SelectedItem as ComboBoxItem;

            if ( null != selItem && null != selItem.DataContext )
            {
                m_outputSize = (EnumPaperSizeOutput)selItem.DataContext;
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

        private void btnScalingDec_Click(object sender, RoutedEventArgs e)
        {
            m_scaling--;
            txtblkScaling.Text = m_scaling.ToString();
        }

        private void btnScalingInc_Click(object sender, RoutedEventArgs e)
        {
            m_scaling++;
            txtblkScaling.Text = m_scaling.ToString();
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
            }
        }

        private void chkNin1_Unchecked(object sender, RoutedEventArgs e)
        {
            if ( m_isWindowLoaded )
            {
                m_nin1 = EnumNin1._1up;
                InitNin1();
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            cboDocSize.Items.Clear();
            cboResolution.Items.Clear();
            cboOutputSize.Items.Clear();
            cboMediaType.Items.Clear();

            m_scaling    = 100;
            m_scanMode   = EnumCopyScanMode.Photo;
            m_docSize    = EnumPaperSizeInput._A4;
            m_outputSize = EnumPaperSizeOutput._Letter;
            m_nin1       = EnumNin1._1up;
            m_dpi        = EnumResln._300x300;
            m_mediaType  = EnumMediaType.Plain;

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

            txtblkScaling.Text = m_scaling.ToString();
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


            txtblkScaling.Text = m_scaling.ToString();

            if ( m_isIDCardCopy )
            {
                txtblkScaling.IsEnabled = false;
                btnScalingDec.IsEnabled = false;
                btnScalingInc.IsEnabled = false;
            }
                

            InitCboDocSize();
            InitCboResolution();
            InitCboOutputSize();
            InitCboMediaType();
            InitNin1();
            m_preNin1 = m_nin1;

            m_isWindowLoaded = true;

        }


        /// <summary>
        /// Initialize original document size combobox.
        /// </summary>
        private void InitCboDocSize()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "A4 (210*297mm)" ;
            cboItem.DataContext = EnumPaperSizeInput._A4; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "A5 (148*210mm)" ;
            cboItem.DataContext = EnumPaperSizeInput._A5; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "B5 (182*257mm)" ;
            cboItem.DataContext = EnumPaperSizeInput._B5; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Letter (8.5*11\")" ;
            cboItem.DataContext = EnumPaperSizeInput._Letter; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Executive (7.25*10.5\")" ;
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
            cboItem.Content = "300*300" ;
            cboItem.DataContext = EnumResln._300x300;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboResolution.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "600*600" ;
            cboItem.DataContext = EnumResln._600x600;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboResolution.Items.Add( cboItem );

            foreach ( ComboBoxItem obj in cboResolution.Items )
            {
                if ( null != obj.DataContext 
                        && (EnumResln)obj.DataContext == m_dpi )
                {
                    obj.IsSelected = true;
                }
            }

            if ( m_isIDCardCopy )
            {
                foreach ( ComboBoxItem obj in cboResolution.Items )
                {
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
            cboItem.Content="Letter" ;
            cboItem.DataContext = EnumPaperSizeOutput._Letter;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="A4" ;
            cboItem.DataContext = EnumPaperSizeOutput._A4;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="A5" ;
            cboItem.DataContext = EnumPaperSizeOutput._A5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="A6" ;
            cboItem.DataContext = EnumPaperSizeOutput._A6;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="B5" ;
            cboItem.DataContext = EnumPaperSizeOutput._B5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="B6" ;
            cboItem.DataContext = EnumPaperSizeOutput._B6;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="Executive" ;
            cboItem.DataContext = EnumPaperSizeOutput._Executive;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="16K" ;
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
            cboItem.Content = "Plain Paper" ;
            cboItem.DataContext = EnumMediaType.Plain;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Recycled Paper" ;
            cboItem.DataContext = EnumMediaType.Recycled;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Thick Paper" ;
            cboItem.DataContext = EnumMediaType.Thick;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Thin Paper" ;
            cboItem.DataContext = EnumMediaType.Thin;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Label" ;
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

            brSelected.Color = Colors.Gray;
            brUnselected.Color = Color.FromArgb(255, 168, 168, 168);

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
            }
        }

        private void imgNin1_4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = EnumNin1._4up;
                m_nin1 = EnumNin1._4up;
                InitNin1();
            }
        }

        private void imgNin1_9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = EnumNin1._9up;
                m_nin1 = EnumNin1._9up;
                InitNin1();
            }
        }
	}
}
