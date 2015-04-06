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
        public EnumCopyScanMode m_scanMode = EnumCopyScanMode.Photo; 
        public byte   m_docSize    = 1;
        public byte   m_outputSize = 1;
        public byte   m_nin1       = 1;
        public byte   m_dpi        = 1;
        public byte   m_mediaType  = 1;

        /// <summary>
        /// Flag used to specify the CopySetting has been loaded or not.
        /// </summary>
        private bool isWindowLoaded = false;

        /// <summary>
        /// Previous N in 1 value, used to recovery the selected item when N in 1 check box was rechecked.
        /// This value only change during copy setting window loading and N in 1 images selecting.
        /// </summary>
        private byte m_preNin1 = 1;
#endregion

#region Properties
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

        }

        private void cboResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cboOutputSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cboMediaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
            if ( isWindowLoaded )
            {
                if ( 4 == m_preNin1 || 9 == m_preNin1 )
                    m_nin1 = m_preNin1;
                else
                    m_nin1 = 2;

                InitNin1();
            }
        }

        private void chkNin1_Unchecked(object sender, RoutedEventArgs e)
        {
            if ( isWindowLoaded )
            {
                m_nin1 = 1;
                InitNin1();
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {

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

            InitCboDocSize();
            InitCboResolution();
            InitCboOutputSize();
            InitCboMediaType();
            InitNin1();
            m_preNin1 = m_nin1;

            isWindowLoaded = true;

        }


        /// <summary>
        /// Initialize original document size combobox.
        /// </summary>
        private void InitCboDocSize()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "A4 (210*297mm)" ;
            cboItem.IsSelected = true;
            cboItem.DataContext = EnumPaperSizeInput._A4; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "A5 (148*210mm)" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeInput._A5; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "B5 (182*257mm)" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeInput._B5; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Letter (8.5*11\")" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeInput._Letter; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Executive (7.25*10.5\")" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeInput._Executive; 
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboDocSize.Items.Add( cboItem );
        }

        /// <summary>
        /// Initialize resolution combobox.
        /// </summary>
        private void InitCboResolution()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "300*300" ;
            cboItem.IsSelected = true;
            cboItem.DataContext = enum_resln._300x300;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboResolution.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "600*600" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = enum_resln._600x600;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboResolution.Items.Add( cboItem );
        }

        /// <summary>
        /// Initialize output paper size combobox.
        /// </summary>
        private void InitCboOutputSize()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content="Letter" ;
            cboItem.IsSelected = true;
            cboItem.DataContext = EnumPaperSizeOutput._Letter;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="A4" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeOutput._A4;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="A5" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeOutput._A5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="A6" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeOutput._A6;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="B5" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeOutput._B5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="B6" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeOutput._B6;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="Executive" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeOutput._Executive;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content="16K" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumPaperSizeOutput._16K;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboOutputSize.Items.Add( cboItem );
        }

        /// <summary>
        /// Initialize media type combobox.
        /// </summary>
        private void InitCboMediaType()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "Plain Paper" ;
            cboItem.IsSelected = true;
            cboItem.DataContext = EnumMediaType.Plain;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Recycled Paper" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumMediaType.Recycled;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Thick Paper" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumMediaType.Thick;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Thin Paper" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumMediaType.Thin;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Label" ;
            cboItem.IsSelected = false;
            cboItem.DataContext = EnumMediaType.Label;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add( cboItem );

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

            if ( 1 == m_nin1 )
            {
                chkNin1.IsChecked = false;
            }
            else
            {
                chkNin1.IsChecked = true;

                if ( 2 == m_nin1 )
                {
                    rectNin1_2.Fill = brSelected;
                }
                else if ( 4 == m_nin1 )
                {
                    rectNin1_4.Fill = brSelected;
                }
                else if ( 9 == m_nin1 )
                {
                    rectNin1_9.Fill = brSelected;
                }
            }
        }

        private void imgNin1_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = 2;
                m_nin1 = 2;
                InitNin1();
            }
        }

        private void imgNin1_4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = 4;
                m_nin1 = 4;
                InitNin1();
            }
        }

        private void imgNin1_9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ( true == chkNin1.IsChecked ) 
            {
                m_preNin1 = 9;
                m_nin1 = 9;
                InitNin1();
            }
        }
	}
}
