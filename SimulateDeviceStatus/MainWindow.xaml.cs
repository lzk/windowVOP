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

namespace SimulateDeviceStatus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileName = (@"DeviceStatus.xml");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = (int)(sld.Value + 0.5);
            txtToner.Text = value.ToString();

            SaveXmlDoc();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> printers = new List<string>();
            VOP.common.GetSupportPrinters(printers);

            for (int i = 0; i < printers.Count; i++)
            {
                cboPrinters.Items.Add(printers[i]);
            }

            if (cboPrinters.Items.Count <= 2)
            {
                cboPrinters.Items.Add("Lenovo test...");
            }



            if (cboPrinters.Items.Count > 0)
                cboPrinters.SelectedIndex = 0;

            updateUI();
        }
        
        private void updateUI()
        {        
            if (cboPrinters.HasItems)
            {
                string strSelPrtName = "";
                string deviceStatus = "";
                string machineJob = "";
                string tonerCapacity = "";

                string str = cboPrinters.SelectedValue as string;
                if (null != str)
                {
                    strSelPrtName = str;
                }
                else
                {
                    ComboBoxItem cboItem = cboPrinters.SelectedItem as ComboBoxItem;
                    strSelPrtName = (string)cboItem.Content;
                }

               // StatusXmlHelper xml = new StatusXmlHelper(fileName);

               // if (xml.CreateXmlDocument("DeviceStatus", "1.0", "gb2312", "yes"))
                {
                    if (StatusXmlHelper.GetPrinterInfo(strSelPrtName, out deviceStatus, out machineJob, out tonerCapacity, fileName))
                    {
                        SetComboBoxByContent(comboStatus, deviceStatus);
                        SetComboBoxByContent(comboJob, machineJob);

                        if (null != sld)
                            sld.Value = Convert.ToDouble(tonerCapacity);
                    }           
                }
           
            }
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            SaveXmlDoc();

            this.Close();
        }

        private void SaveXmlDoc()
        {
            if (cboPrinters.HasItems && (null != sld))
            {
                ComboBoxItem cboItem = null;
                string strSelPrtName = "";
                string strStatus;
                string strMachineJob;
                int tonerValue = (int)(sld.Value + 0.5);

                string str = cboPrinters.SelectedValue as string;
                if (null != str)
                {
                    strSelPrtName = str;
                }
                else
                {
                    cboItem = cboPrinters.SelectedItem as ComboBoxItem;
                    strSelPrtName = (string)cboItem.Content;
                }

                cboItem = comboStatus.SelectedItem as ComboBoxItem;
                strStatus = (string)cboItem.Content;

                cboItem = comboJob.SelectedItem as ComboBoxItem;
                strMachineJob = (string)cboItem.Content;

                if (!StatusXmlHelper.SavePrinterInfo(strSelPrtName, strStatus, strMachineJob, tonerValue.ToString(), fileName))
                {
                    System.Windows.MessageBox.Show("Save xml Document Failed..  ");
                }
            }        

        }



        private bool SetComboBoxByContent(ComboBox cbo, string content)
        {
            if (cbo == null) return false;

            ItemCollection items = cbo.Items;

            foreach (ComboBoxItem item in items)
            {
                string str = item.Content as string;

                if (content == str)
                {
                    cbo.SelectedItem = (item);
                }
            }

            return false;
        }

        private void cboPrinters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateUI();
        }

        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveXmlDoc();
        }       
    }
}
