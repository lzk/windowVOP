using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for StatusPanel.xaml
    /// </summary>
    public partial class StatusPanel : UserControl
    {
        /// <summary>
        /// Current selected printer name. Assign empty, if nothing selected.
        /// </summary>
        public string m_selectedPrinter = "";

        public StatusPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            List<string> printers = new List<string>();
            common.GetSupportPrinters( printers );

            for ( int i=0; i<printers.Count; i++ )
            {
                cboPrinters.Items.Add( printers[i] );
            }

            if ( cboPrinters.Items.Count > 0 )
                cboPrinters.SelectedIndex = 0;
        }

        private void cboPrinters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_selectedPrinter = this.cboPrinters.SelectedItem.ToString();
        }
    }
}
