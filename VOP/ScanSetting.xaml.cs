﻿using System;
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
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
            // Insert code required on object creation below this point.
        }

        public void handler_loaded( object sender, RoutedEventArgs e )
        {
            InitControls();
            InitScanResln();
            InitScanSize();
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

        private void InitScanResln()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "100 x 100" ;
            cboItem.DataContext = EnumScanResln._100x100;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "200 x 200" ;
            cboItem.DataContext = EnumScanResln._200x200;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "300 x 300" ;
            cboItem.DataContext = EnumScanResln._300x300;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "600 x 600" ;
            cboItem.DataContext = EnumScanResln._600x600;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanResln.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "1200 x 1200" ;
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
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = "A4 (210 x 297mm)";
            cboItem.DataContext = EnumPaperSizeScan._A4;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "A5 (148 x 210mm)";
            cboItem.DataContext = EnumPaperSizeScan._A5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "B5 (182 x 257mm)";
            cboItem.DataContext = EnumPaperSizeScan._B5;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "Letter (8.5 x 11\")";
            cboItem.DataContext = EnumPaperSizeScan._Letter;
            cboItem.MinWidth = 145;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboScanSize.Items.Add( cboItem );

            cboItem = new ComboBoxItem();
            cboItem.Content = "(4 x 6\")";
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
    }
}
