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

        }

        private void chkNin1_Unchecked(object sender, RoutedEventArgs e)
        {

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
	}
}
