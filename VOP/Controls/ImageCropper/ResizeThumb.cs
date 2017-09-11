using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace VOP.Controls
{
    public class ResizeThumb : Thumb
    {
        //private RotateTransform rotateTransform;
        //private double angle;
        //private Adorner adorner;
        //private Point transformOrigin;
        private ContentControl designerItem;
        private Canvas parentCanvas;
        double maxHeightToTop;
        double maxWidthToLeft;

        public ResizeThumb()
        {
            this.Loaded += new RoutedEventHandler(OnLoaded);
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            //DragCompleted += new DragCompletedEventHandler(this.ResizeThumb_DragCompleted);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if((string)this.Tag  == "SizeNWSE")
            {
                this.Cursor = Cursors.SizeNWSE;
            }
            else if ((string)this.Tag == "SizeNESW")
            {
                this.Cursor = Cursors.SizeNESW;
            }
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = this.DataContext as ContentControl;

            if (this.designerItem != null)
            {
                this.parentCanvas = VisualTreeHelper.GetParent(this.designerItem) as Canvas;

                if (this.parentCanvas != null)
                {
                    maxHeightToTop = Canvas.GetTop(this.designerItem) - ImageCropper.imageToTop + this.designerItem.ActualHeight - ImageCropper.thumbCornerWidth;
                    maxWidthToLeft = Canvas.GetLeft(this.designerItem) - ImageCropper.imageToLeft + this.designerItem.ActualWidth - ImageCropper.thumbCornerWidth;
                    //this.transformOrigin = this.designerItem.RenderTransformOrigin;

                    //this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                    //if (this.rotateTransform != null)
                    //{
                    //    this.angle = this.rotateTransform.Angle * Math.PI / 180.0;
                    //}
                    //else
                    //{
                    //    this.angle = 0.0d;
                    //}

                    //AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
                    //if (adornerLayer != null)
                    //{
                    //    this.adorner = new SizeAdorner(this.designerItem);
                    //    adornerLayer.Add(this.adorner);
                    //}
                }
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {

            if (this.designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                if(VerticalAlignment == System.Windows.VerticalAlignment.Bottom 
                  && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                {
                    deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                    this.designerItem.Height -= deltaVertical;
                    this.designerItem.Width -= deltaVertical * ImageCropper.designerItemWHRatio;

                    if ((Canvas.GetLeft(this.designerItem) - ImageCropper.imageToLeft + this.designerItem.Width + ImageCropper.thumbCornerWidth) > ImageCropper.imageWidth)
                    {
                        this.designerItem.Width = ImageCropper.imageWidth - (Canvas.GetLeft(this.designerItem) - ImageCropper.imageToLeft) - ImageCropper.thumbCornerWidth;
                        this.designerItem.Height = this.designerItem.Width / ImageCropper.designerItemWHRatio;
                    }

                    if ((Canvas.GetTop(this.designerItem) - ImageCropper.imageToTop + this.designerItem.Height + ImageCropper.thumbCornerWidth) > ImageCropper.imageHeight)
                    {
                        this.designerItem.Height = ImageCropper.imageHeight - (Canvas.GetTop(this.designerItem) - ImageCropper.imageToTop) - ImageCropper.thumbCornerWidth;
                        this.designerItem.Width = this.designerItem.Height * ImageCropper.designerItemWHRatio;
                    }
                }

                if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                 && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                {

                    deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
  
                    double setX = Canvas.GetLeft(this.designerItem) + deltaHorizontal;

                    if (setX < ImageCropper.thumbCornerWidth + ImageCropper.imageToLeft)
                    {
                        setX = ImageCropper.thumbCornerWidth + ImageCropper.imageToLeft;
                    }

                    Canvas.SetLeft(this.designerItem, setX);
                    this.designerItem.Width -= deltaHorizontal;

                    if (this.designerItem.Width > maxWidthToLeft)
                    {
                        this.designerItem.Width = maxWidthToLeft;
                    }
                    else
                    {
                        this.designerItem.Height -= deltaHorizontal / ImageCropper.designerItemWHRatio;                 
                    }

                    if ((Canvas.GetTop(this.designerItem) - ImageCropper.imageToTop + this.designerItem.Height + ImageCropper.thumbCornerWidth) > ImageCropper.imageHeight)
                    {
                        this.designerItem.Height = ImageCropper.imageHeight - (Canvas.GetTop(this.designerItem) - ImageCropper.imageToTop) - ImageCropper.thumbCornerWidth;                   
                    }
                    
                    if(  this.designerItem.Width > this.designerItem.Height * ImageCropper.designerItemWHRatio)
                    {
                        this.designerItem.Width = this.designerItem.Height * ImageCropper.designerItemWHRatio;
                    }
                }

                if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                  && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                {
                    deltaVertical = Math.Min(e.VerticalChange, this.designerItem.ActualHeight - this.designerItem.MinHeight);

                    double setX = Canvas.GetLeft(this.designerItem) + deltaVertical;
                    double setY = Canvas.GetTop(this.designerItem) + deltaVertical / ImageCropper.designerItemWHRatio;

                    if (setX < ImageCropper.thumbCornerWidth + ImageCropper.imageToLeft)
                    {
                        setX = ImageCropper.thumbCornerWidth + ImageCropper.imageToLeft;
                    }

                    if (setY < ImageCropper.thumbCornerWidth + ImageCropper.imageToTop)
                    {
                        setY = ImageCropper.thumbCornerWidth + ImageCropper.imageToTop;
                    }

                    Canvas.SetLeft(this.designerItem, setX);
                    Canvas.SetTop(this.designerItem, setY);

                    if (this.designerItem.Height > maxHeightToTop)
                    {
                        this.designerItem.Height = maxHeightToTop;
                    }
                    if (this.designerItem.Width > maxWidthToLeft)
                    {
                        this.designerItem.Width = maxWidthToLeft;
                    }

                    if(this.designerItem.Height < maxHeightToTop && this.designerItem.Width < maxWidthToLeft)
                    {
                        this.designerItem.Width -= deltaVertical;
                        this.designerItem.Height -= deltaVertical / ImageCropper.designerItemWHRatio;
                    }
                    else if (deltaVertical > 0)
                    {
                        this.designerItem.Width -= deltaVertical;
                        this.designerItem.Height -= deltaVertical / ImageCropper.designerItemWHRatio;
                    }
                }

                if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                    && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                {
                    deltaVertical = Math.Min(e.VerticalChange, this.designerItem.ActualHeight - this.designerItem.MinHeight);

                    double setY = Canvas.GetTop(this.designerItem) + deltaVertical;

                    if (setY < ImageCropper.thumbCornerWidth + ImageCropper.imageToTop)
                    {
                        setY = ImageCropper.thumbCornerWidth + ImageCropper.imageToTop;
                    }

                    Canvas.SetTop(this.designerItem, setY);
                    this.designerItem.Height -= deltaVertical;

                    if (this.designerItem.Height > maxHeightToTop)
                    {
                        this.designerItem.Height = maxHeightToTop;
                    }
                    else
                    {
                        this.designerItem.Width -= deltaVertical * ImageCropper.designerItemWHRatio;
                    }

                    if ((Canvas.GetLeft(this.designerItem) - ImageCropper.imageToLeft + this.designerItem.Width + ImageCropper.thumbCornerWidth) > ImageCropper.imageWidth)
                    {
                        this.designerItem.Width = ImageCropper.imageWidth - (Canvas.GetLeft(this.designerItem) - ImageCropper.imageToLeft) - ImageCropper.thumbCornerWidth;
                        this.designerItem.Height = this.designerItem.Width / ImageCropper.designerItemWHRatio;
                    }

                    if (this.designerItem.Height > this.designerItem.Width / ImageCropper.designerItemWHRatio)
                    {
                        this.designerItem.Height = this.designerItem.Width / ImageCropper.designerItemWHRatio;
                    }
                }
                //switch (VerticalAlignment)
                //{
                //    case System.Windows.VerticalAlignment.Bottom:
                //        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                //        designerItem.Height -= deltaVertical;

                //        if ((Canvas.GetTop(this.designerItem) + this.designerItem.Height + ImageCropper.thumbCornerWidth) > parentCan.ActualHeight)
                //        {
                //            this.designerItem.Height = parentCan.ActualHeight - Canvas.GetTop(this.designerItem) - ImageCropper.thumbCornerWidth;
                //        }

                //        break;
                //    case System.Windows.VerticalAlignment.Top:  
                //        deltaVertical = Math.Min(e.VerticalChange, this.designerItem.ActualHeight - this.designerItem.MinHeight);

                //        double setY = Canvas.GetTop(this.designerItem) + deltaVertical;

                //        if (setY < ImageCropper.thumbCornerWidth)
                //        {
                //            setY = ImageCropper.thumbCornerWidth;
                //        }

                //        Canvas.SetTop(this.designerItem, setY);
                //        this.designerItem.Height -= deltaVertical;

                //        if (this.designerItem.Height > maxHeightToTop)
                //        {
                //            this.designerItem.Height = maxHeightToTop;
                //        }

                //        break;
                //    default:
                //        break;
                //}

                //switch (HorizontalAlignment)
                //{
                //    case System.Windows.HorizontalAlignment.Left:
                //        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);

                //        double setX = Canvas.GetLeft(this.designerItem) + deltaHorizontal;

                //        if (setX < ImageCropper.thumbCornerWidth)
                //        {
                //            setX = ImageCropper.thumbCornerWidth;
                //        }

                //        Canvas.SetLeft(this.designerItem, setX);
                //        this.designerItem.Width -= deltaHorizontal;

                //        if (this.designerItem.Width > maxWidthToLeft)
                //        {
                //            this.designerItem.Width = maxWidthToLeft;
                //        }

                //        break;
                //    case System.Windows.HorizontalAlignment.Right:
                //        deltaHorizontal = Math.Min(-e.HorizontalChange, this.designerItem.ActualWidth - this.designerItem.MinWidth);
                //        this.designerItem.Width -= deltaHorizontal;

                //        if ((Canvas.GetLeft(this.designerItem) + this.designerItem.Width + ImageCropper.thumbCornerWidth) > parentCan.ActualWidth)
                //        {
                //            this.designerItem.Width = parentCan.ActualWidth - Canvas.GetLeft(this.designerItem) - ImageCropper.thumbCornerWidth;
                //        }

                //        break;
                //    default:
                //        break;
                    
                //}
         
            }

            e.Handled = true;
        }

        //private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        //{
        //    if (this.adorner != null)
        //    {
        //        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
        //        if (adornerLayer != null)
        //        {
        //            adornerLayer.Remove(this.adorner);
        //        }

        //        this.adorner = null;
        //    }
        //}
    }
}
