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

#region Properties
        public byte   m_scanMode   = 1;
        public byte   m_docSize    = 1;
        public byte   m_outputSize = 1;
        public byte   m_nin1       = 1;
        public byte   m_dpi        = 1;
        public ushort m_scaling    = 1;
        public byte   m_mediaType  = 1;
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

        }

        private void btnScalingInc_Click(object sender, RoutedEventArgs e)
        {

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
	}
}
