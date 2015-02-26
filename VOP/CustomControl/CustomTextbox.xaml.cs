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
    /// Interaction logic for CustomTextbox.xaml
    /// </summary>
    public partial class CustomTextbox : UserControl
    {
        public CustomTextbox()
        {
            InitializeComponent();



            canvas.Loaded += new RoutedEventHandler(canvas_Loaded);
        }

        void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            Size canvasSize = canvas.RenderSize;

            double radius = (canvasSize.Width > canvasSize.Height) ? canvasSize.Height / 2 : canvasSize.Width / 2;

            textBox.Width = canvasSize.Width / 2.0 - radius;
            textBox.Height = canvasSize.Height * (2 / 3.0);

            Canvas.SetLeft(textBox, radius);
            Canvas.SetTop(textBox, canvasSize.Height * (1 / 6.0));

            ImageRight.Width = canvasSize.Width / 2.0;
            ImageRight.Height = canvasSize.Height;
            Canvas.SetLeft(ImageRight, canvasSize.Width / 2.0);

            CustomButton();
        }


        public void CustomButton()
        {

            GeometryGroup clipGeometry3 = new GeometryGroup();
            clipGeometry3.FillRule = FillRule.Nonzero;

            RectangleGeometry rectGeometry = new RectangleGeometry();

            Size canvasSize = canvas.RenderSize;


            double radius = (canvasSize.Width > canvasSize.Height) ? canvasSize.Height / 2 : canvasSize.Width / 2;

            rectGeometry.RadiusX = rectGeometry.RadiusY = radius;

            rectGeometry.Rect = new Rect(0, 0, canvasSize.Width, canvasSize.Height);

            clipGeometry3.Children.Add(rectGeometry);

            canvas.Clip = clipGeometry3;
        }






    }
}
