# LayterControl_WPF
WPF模拟Ps图层简单操作，完全基于MVVM数据驱动。
![f](https://github.com/lingme/Picture_Bucket/raw/master/LayterControl/AlternateGif.gif)

##### 数据模型

```C{.line-numbers}
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
        public bool ShowLayter { get; set; } = true;

        /// <summary>
        /// 图层允许移动
        /// </summary>
        public bool CanMove { get; set; } = true;

        /// <summary>
        /// 图层
        /// </summary>
        public ImageSource LayterSource { get; set; }

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
```
