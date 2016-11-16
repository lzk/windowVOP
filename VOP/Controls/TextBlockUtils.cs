using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using VOP;

namespace Util
{
  public class TextBlockUtils
  {
    private static bool isWordEllipsis = true;
    const double DBL_EPSILON = 0.5; 

    public static bool AreClose(double value1, double value2)
    {
        if (value1 == value2) return true;

        double delta = Math.Abs(value1 - value2);
        return delta <= DBL_EPSILON;
    }

    public static bool GetIsWordEllipsis(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsWordEllipsisProperty);
    }

    public static void SetIsWordEllipsis(DependencyObject obj, bool value)
    {
        obj.SetValue(IsWordEllipsisProperty, value);
    }

    public static readonly DependencyProperty IsWordEllipsisProperty = DependencyProperty.RegisterAttached("IsWordEllipsis",
            typeof(bool), typeof(TextBlockUtils), new PropertyMetadata(true, OnIsWordEllipsisPropertyChanged));


    private static void OnIsWordEllipsisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBlock textBlock = d as TextBlock;
        if (textBlock == null)
            return;

        if (e.NewValue.Equals(true))
        {
            isWordEllipsis = true;
        }
        else
        {
            isWordEllipsis = false;
        }
    }
    /// <summary>
    /// Gets the value of the AutoTooltipProperty dependency property
    /// </summary>
    public static bool GetAutoTooltip(DependencyObject obj)
    {
      return (bool)obj.GetValue(AutoTooltipProperty);
    }

    /// <summary>
    /// Sets the value of the AutoTooltipProperty dependency property
    /// </summary>
    public static void SetAutoTooltip(DependencyObject obj, bool value)
    {
      obj.SetValue(AutoTooltipProperty, value);
    }
   
    /// <summary>
    /// Identified the attached AutoTooltip property. When true, this will set the TextBlock TextTrimming
    /// property to WordEllipsis, and display a tooltip with the full text whenever the text is trimmed.
    /// </summary>
    public static readonly DependencyProperty AutoTooltipProperty = DependencyProperty.RegisterAttached("AutoTooltip",
            typeof(bool), typeof(TextBlockUtils), new PropertyMetadata(false, OnAutoTooltipPropertyChanged));

    private static void OnAutoTooltipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBlock textBlock = d as TextBlock;
      if (textBlock == null)
        return;

      if (e.NewValue.Equals(true))
      {
        textBlock.TextTrimming = isWordEllipsis ? TextTrimming.WordEllipsis : TextTrimming.CharacterEllipsis;
        ComputeAutoTooltip(textBlock);
        textBlock.SizeChanged += TextBlock_SizeChanged;
      }
      else
      {
        textBlock.SizeChanged -= TextBlock_SizeChanged;
      }
    }

    private static void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      TextBlock textBlock = sender as TextBlock;
      ComputeAutoTooltip(textBlock);
    }

    /// <summary>
    /// Assigns the ToolTip for the given TextBlock based on whether the text is trimmed
    /// </summary>
    private static void ComputeAutoTooltip(TextBlock textBlock)
    {



        //if (textBlock.Name == "tbTextx2x")
        //{
        //    string debug = "";
        //}

      FormattedText formattedText = new FormattedText(
         textBlock.Text,
         textBlock.Language.GetEquivalentCulture(),
         textBlock.FlowDirection,
         new Typeface(textBlock.FontFamily.Source),
         textBlock.FontSize,
         Brushes.Black);

      double comparedWidth = 0.0;

      if (double.IsNaN(textBlock.Width))
      {
          comparedWidth = textBlock.ActualWidth;
      }
      else
      {
          comparedWidth = textBlock.Width;
      }

      if (AreClose(comparedWidth, formattedText.Width))
      {
          ToolTipService.SetToolTip(textBlock, null);
      }
      else if (comparedWidth < formattedText.Width)
      {
        ToolTipService.SetToolTip(textBlock, textBlock.Text);
      }
      else
      {
        ToolTipService.SetToolTip(textBlock, null);
      }
    }
  }
}
