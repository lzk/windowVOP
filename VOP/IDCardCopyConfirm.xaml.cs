﻿using System;
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
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for NIn1CopyConfirm.xaml
    /// </summary>
    public partial class NIn1CopyConfirm : Window
    {
        public bool m_popupDlg = true;

        public NIn1CopyConfirm()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
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

        private void chkPopupNextTime_Checked(object sender, RoutedEventArgs e)
        {
            m_popupDlg = false;
        }

        private void chkPopupNextTime_Unchecked(object sender, RoutedEventArgs e)
        {
            m_popupDlg = true;
        }
    }
}