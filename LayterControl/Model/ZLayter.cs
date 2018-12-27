namespace LayterControl.Model
{
    using PropertyChanged;
    using System.Windows;
    using System.Windows.Media;
    using System.ComponentModel;

    [AddINotifyPropertyChangedInterface]
    public class ZLayter
    {
        /// <summary>
        /// 图层唯一标识
        /// </summary>
        public int LayterID { get; set; }

        /// <summary>
        /// 图层是否选中
        /// </summary>
        public bool LayterCheck { get; set; } = false;

        /// <summary>
        /// 图层是否可见
        /// </summary>
        public bool ShowLayter { get; set; } = false;

        /// <summary>
        /// 编辑模式
        /// </summary>
        public EditModeType EditMode { get; set; } = EditModeType.Move;

        /// <summary>
        /// 源图层
        /// </summary>
        public ImageSource LayterSource { get; set; }

        /// <summary>
        /// 绘制层
        /// </summary>
        public ImageSource RenderLayterSource { get; set; }

        /// <summary>
        /// 图层名
        /// </summary>
        public string LayterName { get; set; }

        /// <summary>
        /// 图层高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 图层宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 左顶点横坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 左顶点纵坐标
        /// </summary>
        public double Y { get; set; }
    }


    public enum EditModeType
    {
        [Description("画笔")]
        Brush,

        [Description("橡皮擦")]
        Eraser,

        [Description("移动")]
        Move
    }
}
