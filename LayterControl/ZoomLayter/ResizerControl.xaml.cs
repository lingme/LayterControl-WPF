using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LayterControl.ZoomLayter
{
    /// <summary>
    /// ResizerControl.xaml 的交互逻辑
    /// </summary>
    public partial class ResizerControl : UserControl
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(ResizerControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(ResizerControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(ResizerControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(ResizerControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public ResizerControl()
        {
            InitializeComponent();
        }

        private void UpperLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            X = X + e.HorizontalChange;
            Y = Y + e.VerticalChange;

            ItemHeight = ItemHeight + e.VerticalChange * -1;
            ItemWidth = ItemWidth + e.HorizontalChange * -1;
        }

        private void UpperRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Y = Y + e.VerticalChange;

            ItemHeight = ItemHeight + e.VerticalChange * -1;
            ItemWidth = ItemWidth + e.HorizontalChange;
        }

        private void LowerLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            X = X + e.HorizontalChange;

            ItemHeight = ItemHeight + e.VerticalChange;
            ItemWidth = ItemWidth + e.HorizontalChange * -1;
        }

        private void LowerRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ItemHeight = ItemHeight + e.VerticalChange;
            ItemWidth = ItemWidth + e.HorizontalChange;
        }

        private void Center_DragDelta(object sender, DragDeltaEventArgs e)
        {
            X = X + e.HorizontalChange;
            Y = Y + e.VerticalChange;
        }
    }
}
