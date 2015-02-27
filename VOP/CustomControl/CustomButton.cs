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
    public class CustomButton : Button
    {
        private string backgroundImagePath;
        public string BackgroundImagePath
        {
            set { backgroundImagePath = value; }
            get { return backgroundImagePath; }
        }

        private Color textColor = Colors.Black;
        public Color TextColor
        {
            set { textColor = value; }
            get { return textColor; }
        }

        public CustomButton()
        {   
            this.Loaded += new RoutedEventHandler(CustomButton_Loaded);
        }

        void CustomButton_Loaded(object sender, RoutedEventArgs e)
        {
            InitCtrl();
        }

        void InitCtrl()
        {
            ControlTemplate myButtonTemplate = new ControlTemplate();

            FrameworkElementFactory rootGrid = new FrameworkElementFactory(typeof(Grid), "Grid");

            FrameworkElementFactory rect = new FrameworkElementFactory(typeof(Rectangle), "Rectangle");
            rect.SetValue(Rectangle.WidthProperty, this.RenderSize.Width);
            rect.SetValue(Rectangle.WidthProperty, this.RenderSize.Width);
            {
                ImageBrush imgBrush = new ImageBrush();

                imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/" + BackgroundImagePath, UriKind.RelativeOrAbsolute));
                rect.SetValue(Rectangle.FillProperty, imgBrush);
            }

            FrameworkElementFactory text = new FrameworkElementFactory(typeof(TextBlock), "TextBlock");
            text.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            text.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            text.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
            text.SetValue(TextBlock.TextProperty, this.Content);
            text.SetValue(TextBlock.FontSizeProperty, this.FontSize);          
            text.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(textColor));
            text.SetValue(TextBlock.ForceCursorProperty, true);            

            rootGrid.AppendChild(rect);
            rootGrid.AppendChild(text);

            myButtonTemplate.VisualTree = rootGrid;
            this.Template = myButtonTemplate;
        }    
    }
}