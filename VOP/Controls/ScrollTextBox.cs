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

using System.Windows.Threading;

namespace VOP
{
    public class ScrollTextBox : TextBox
    {
        DispatcherTimer timer = new DispatcherTimer();

        private double curSetOffset = 0;
        private double maxScrollOffset = 0;
        private int scrollStep = 1;

        private int defaultMilliseconds = 100;

       

        public int ScrollTextBox_milliseconds
        {
            set
            {
                defaultMilliseconds = value;
                timer.Interval = new TimeSpan(0, 0, 0, 0, value);
            }
        }

        public ScrollTextBox()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, defaultMilliseconds);
            timer.Tick += new EventHandler(timer_Tick);

            this.Loaded += new RoutedEventHandler(ScrollTextBox_Loaded);
            this.TextChanged += new TextChangedEventHandler(ScrollTextBox_TextChanged);
        }

        void ScrollTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            setSrollTextBox();
        }

        void ScrollTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            setSrollTextBox();
        }

        double GetMaxOffset()
        {
            this.ScrollToHorizontalOffset(0); // Don't forget to add to this statement

            int lineLen = this.Text.Length;
            if (lineLen <= 0)
            {
                return 0;
            }

            Rect EndChar_Rect = this.GetRectFromCharacterIndex(lineLen - 1, true);

            Size textSize = this.RenderSize;

            double retMaxScrollOffset = EndChar_Rect.Right - (this.RenderSize.Width);

            return retMaxScrollOffset;
        }

        void setSrollTextBox()
        {
            curSetOffset = 0;
            maxScrollOffset = 0;

            this.ScrollToHorizontalOffset(0);

            StopScroll();

            int lineLen = this.Text.Length;
            if (lineLen <= 0)
            {
                return;
            }

            maxScrollOffset = GetMaxOffset();

            if (maxScrollOffset >= 0.0)
                StartScroll();
        }

        void StartScroll()
        {
            timer.Start();
        }

        void StopScroll()
        {
            timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (0 == curSetOffset)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, defaultMilliseconds);
            }

            if (maxScrollOffset <= 0.0)
            {
                timer.Stop();
            }

            this.ScrollToHorizontalOffset(curSetOffset);

            double CurSetOffset = curSetOffset;

            if (curSetOffset >= maxScrollOffset)
            {
                curSetOffset = 0;

                timer.Interval = new TimeSpan(0, 0, 0, 0, 2000);
            }               
            else
                curSetOffset += scrollStep;
        }
    }
}
