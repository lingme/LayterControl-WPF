# LayterControl_WPF

![f](https://github.com/lingme/Picture_Bucket/raw/master/LayterControl/AlternateGif.gif)

##### data model binding 

```C{.line-numbers}
public class ZLayter
    {
        /// <summary>
        /// layer id
        /// </summary>
        public int LayterID { get; set; }

        /// <summary>
        /// layer is check
        /// </summary>
        public bool LayterCheck { get; set; } = false;

        /// <summary>
        /// layer visibility
        /// </summary>
        public bool ShowLayter { get; set; } = true;

        /// <summary>
        /// layer can drag
        /// </summary>
        public bool CanMove { get; set; } = true;

        /// <summary>
        /// layer image source
        /// </summary>
        public ImageSource LayterSource { get; set; }

        /// <summary>
        /// layer name
        /// </summary>
        public string LayterName { get; set; }

        /// <summary>
        /// layer height
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// layer width
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// layer x
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// layer y
        /// </summary>
        public double Y { get; set; }
    }
```
