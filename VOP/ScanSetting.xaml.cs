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
        enum_docutype m_docutype = enum_docutype.docutype_photo;
        public EnumResln m_resln = EnumResln._300x300;
        private EnumPaperSizeScan m_paperSize = EnumPaperSizeScan._A4;
        public enum_color m_color = enum_color.color_24bit;
        public int m_brightness = 50;
        public int m_contrast = 50;

        public ScanSetting()
        {
            this.InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
            // Insert code required on object creation below this point.
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
                    //if (true == is_default)
                    {
                        //txtblk_dpi.Text = "300x300 dpi";
                        //txtblk_color.Text = "Color";
                        m_resln = EnumResln._300x300;
                        m_color = enum_color.color_24bit;
                    }
                    m_docutype = enum_docutype.docutype_photo;
                }
                else if (rdbtn.Name == "docutype_text")
                {
                    //if (true == is_default)
                    {
                        //txtblk_dpi.Text = "300x300 dpi";
                        //txtblk_color.Text = "Grayscale";
                        m_resln = EnumResln._300x300;
                        m_color = enum_color.grayscale_8bit;
                    }
                    m_docutype = enum_docutype.docutype_text;
                }
                else if (rdbtn.Name == "docutype_graphic")
                {
                    //if (true == is_default)
                    {
                        //txtblk_dpi.Text = "300x300 dpi";
                        //txtblk_color.Text = "Color";
                        m_resln = EnumResln._300x300;
                        m_color = enum_color.color_24bit;
                    }
                    m_docutype = enum_docutype.docutype_graphic;
                }
            }
        }


        private void cbo_selchg_scansize(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbo = sender as ComboBox;
            ComboBoxItem item = cbo.SelectedItem as ComboBoxItem;

            if (null != item && null != item.DataContext)
            {
                m_paperSize = (EnumPaperSizeScan)item.DataContext;
            }
        }

        private void combo_sel_change(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            if (null != combo)
            {
                ComboBoxItem item = combo.SelectedItem as ComboBoxItem;

                if (null != item)
                {
                    string txt_data = (string)item.DataContext;

                    if (combo.Name == "combo_dpi")
                    {
                        switch (txt_data)
                        {
                            case "0":
                                m_resln = EnumResln._300x300;
                                break;
                            case "1":
                                m_resln = EnumResln._600x600;
                                break;
                            case "2":
                                m_resln = EnumResln._1200x1200;
                                break;
                        }
                    }
                }
            }
        }

        private void colorMode_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rdbtn = sender as RadioButton;

            if (null != rdbtn)
            {
                if (rdbtn.Name == "Color")
                {
                    m_color = enum_color.color_24bit;
                }
                else if (rdbtn.Name == "Grayscale")
                {
                    m_color = enum_color.grayscale_8bit;
                }
                else if (rdbtn.Name == "BlackAndWhite")
                {
                    m_color = enum_color.black_white;
                }
            }
        }


        public void TextValueChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (null != tb)
            {
                if ("tb_brightness" == tb.Name)
                {
                    int nVal = m_brightness;
                    try
                    {
                        if (tb.Text.Length > 0)
                            nVal = Convert.ToInt32(tb.Text);
                        else
                            nVal = 0;
                    }
                    catch
                    {
                    }

                    if (nVal < 0)
                        nVal = 0;

                    if (nVal > 100)
                        nVal = 100;

                    tb.Text = nVal.ToString();
                    tb.CaretIndex = tb.Text.Length;
                    sldr_brightness.Value = nVal;
                }
                else if ("tb_contrast" == tb.Name)
                {
                    int nVal = m_contrast;
                    try
                    {
                        if (tb.Text.Length > 0)
                            nVal = Convert.ToInt32(tb.Text);
                        else
                            nVal = 0;
                    }
                    catch
                    {
                    }

                    if (nVal < 0)
                        nVal = 0;

                    if (nVal > 100)
                        nVal = 100;

                    tb.Text = nVal.ToString();
                    tb.CaretIndex = tb.Text.Length;
                    sldr_contrast.Value = nVal;
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
                int val = (int)sldr.Value;
                if (sldr.Name == "sldr_brightness" && null != tb_brightness)
                {
                    tb_brightness.Text = val.ToString();
                    m_brightness = val;
                }
                else if (sldr.Name == "sldr_contrast" && null != tb_contrast)
                {
                    tb_contrast.Text = val.ToString();
                    m_contrast = val;
                }
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            SetupDefault();
        }

        private void SetupDefault()
        {
            docutype_photo.IsChecked = true;
            combo_dpi.SelectedIndex = 0;
            Color.IsChecked = true;
            cbo_scansize.SelectedIndex = 0;

            sldr_brightness.Value = 50;
            sldr_contrast.Value = 50;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Adjust_Click(object sender, RoutedEventArgs e)
        {
             Button btn = sender as Button;

             int brightnesstMin = (int)sldr_brightness.Minimum;
             int brightnessMax = (int)sldr_brightness.Maximum;
             int brightnessCurValue = (int)sldr_brightness.Value;

             int contrastMin = (int)sldr_contrast.Minimum;
             int contrastMax = (int)sldr_contrast.Maximum;
             int contrastCurValue = (int)sldr_contrast.Value;




             if (null != btn)
             {
                 if ("btnReduce_Brightness" == btn.Name)
                 {
                     if( (brightnessCurValue-1) >= brightnesstMin )
                     {
                         sldr_brightness.Value = brightnessCurValue - 1;
                     }
                 }
                 else if ("btnAdd_Brightness" == btn.Name)
                 {
                     if ((brightnessCurValue + 1) <= brightnessMax)
                     {
                         sldr_brightness.Value = brightnessCurValue + 1;
                     }
                 }
                 else if ("btnReduce_Constrast" == btn.Name)
                 {
                     if ((contrastCurValue - 1) >= contrastMin)
                     {
                         sldr_contrast.Value = contrastCurValue - 1;
                     }
                 }
                 else if ("btnAdd_Constrast" == btn.Name)
                 {
                     if ((contrastCurValue + 1) <= contrastMax)
                     {
                         sldr_contrast.Value = contrastCurValue + 1;
                     }
                 }
             }
        }
    }
}
