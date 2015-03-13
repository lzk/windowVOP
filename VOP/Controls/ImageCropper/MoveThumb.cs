using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace VOP.Controls
{
    public class MoveThumb : Thumb
    {
        //private RotateTransform rotateTransform;
        private ContentControl designerItem;

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as ContentControl;

            //if (this.designerItem != null)
            //{
            //    this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
            //}
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ContentControl designerItem = DataContext as ContentControl;
            Canvas parentCan = VisualTreeHelper.GetParent(designerItem) as Canvas;

            if (this.designerItem != null)
            {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                //if (this.rotateTransform != null)
                //{
                //    dragDelta = this.rotateTransform.Transform(dragDelta);
                //}

                double setX = Canvas.GetLeft(designerItem) + dragDelta.X;
                double setY = Canvas.GetTop(designerItem) + dragDelta.Y;


                if (setX < ImageCropper.thumbCornerWidth + ImageCropper.imageToLeft)
                {
                    setX = ImageCropper.thumbCornerWidth + ImageCropper.imageToLeft;
                }

                if (setX > (parentCan.ActualWidth - designerItem.ActualWidth - ImageCropper.thumbCornerWidth - ImageCropper.imageToLeft))
                {
                    setX = parentCan.ActualWidth - designerItem.ActualWidth - ImageCropper.thumbCornerWidth - ImageCropper.imageToLeft;
                }

                if (setY < ImageCropper.thumbCornerWidth + ImageCropper.imageToTop)
                {
                    setY = ImageCropper.thumbCornerWidth + ImageCropper.imageToTop;
                }

                if (setY > (parentCan.ActualHeight - designerItem.ActualHeight - ImageCropper.thumbCornerWidth - ImageCropper.imageToTop))
                {
                    setY = parentCan.ActualHeight - designerItem.ActualHeight - ImageCropper.thumbCornerWidth - ImageCropper.imageToTop;
                }

                Canvas.SetLeft(designerItem, setX);
                Canvas.SetTop(designerItem, setY);
            }
        }
    }
}
