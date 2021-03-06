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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VOP.Controls;

namespace VOP.Controls
{
    public class GroupBoxDecorator : ContentControl
    {
        private List<Brush> colorList = new List<Brush>();
        private int textElementIndex = 0;

        static GroupBoxDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBoxDecorator), new FrameworkPropertyMetadata(typeof(GroupBoxDecorator)));
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
           DependencyProperty.Register("Header", typeof(string), typeof(GroupBoxDecorator));


        public GroupBoxDecorator()
        {
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(IsEnabledValueChanged);
        }

        private void EnableAllVisual(Visual myVisual, bool enable)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
               
                // Do processing of the child visual object. 
                if (childVisual is SpinnerControl)
                {
                    SpinnerControl control = childVisual as SpinnerControl;

                    if (enable)
                    {
                        control.IsEnabled = true;
                    }
                    else
                    {
                        control.IsEnabled = false;
                    }

                    return;
                }
                else if (childVisual is CheckBox)
                {
                    CheckBox control = childVisual as CheckBox;

                    if (enable)
                    {
                        control.IsEnabled = true;
                    }
                    else
                    {
                        control.IsEnabled = false;
                    }

                    return;
                }
                else if (childVisual is TextBlock)
                {
                    TextBlock textBlock = childVisual as TextBlock;
                    if(enable)
                    {
                        SolidColorBrush brush = new SolidColorBrush();
                        Color c = new Color();
                        c.A = 255;
                        c.R = 14;
                        c.B = 14;
                        c.G = 14;

                        brush.Color = c;
                        textBlock.Foreground = brush;

                        if (textElementIndex < colorList.Count)
                        {
                            textBlock.Foreground = colorList[textElementIndex];
                        }
                    }
                    else
                    {
                        //save enable color
                        if (textElementIndex < colorList.Count)
                        {
                            colorList[textElementIndex] = textBlock.Foreground;
                        }
                        else
                        {
                            colorList.Add(textBlock.Foreground);
                        }
                    
                        SolidColorBrush brush = new SolidColorBrush();
                        Color c = new Color();
                        c.A = 100;
                        c.R = 14;
                        c.B = 14;
                        c.G = 14;

                        brush.Color = c;
                        textBlock.Foreground = brush;
                    }

                    textElementIndex++;
                }
                else if (childVisual is ComboBox)
                {
                    ComboBox combobox = childVisual as ComboBox;
                    if (enable)
                    {
                        SolidColorBrush brush = new SolidColorBrush();
                        Color c = new Color();
                        c.A = 255;
                        c.R = 14;
                        c.B = 14;
                        c.G = 14;

                        brush.Color = c;
                        combobox.Foreground = brush;
                    }
                    else
                    {
                        SolidColorBrush brush = new SolidColorBrush();
                        Color c = new Color();
                        c.A = 100;
                        c.R = 14;
                        c.B = 14;
                        c.G = 14;

                        brush.Color = c;
                        combobox.Foreground = brush;
                    }
                }

                // Enumerate children of the child visual object.
                EnableAllVisual(childVisual, enable);
              
            }
        }

        private void IsEnabledValueChanged(Object obj, DependencyPropertyChangedEventArgs args)
        {
            textElementIndex = 0;
            EnableAllVisual(this, (bool)args.NewValue);
        }
    }
}
