using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Windows.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace LayterControl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int KernelSum = 0;                                //卷积核之和
        private int effsize = 10;                                    //卷积核半径
        private int[,] BlurKn;                                         //模糊卷积集合
        private List<Point> pts = new List<Point>();      //画点集合
        private Rectangle minrect;                                //最小包围矩形

        private Bitmap smobitmap;                              //模糊图

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var list = inkCanvas.Strokes;
            foreach (var item in list)
            {
                pts = ((Point[])item.StylusPoints).ToList();
            }


            BlurKn = CreateKernel(effsize * 2 + 1);

            Rectangle effrect;
            Bitmap mainbmp;

            smobitmap = new Bitmap( (int)editRegion.ActualWidth, (int)editRegion.ActualHeight);

            smobitmap = createMaskbmp(pts, out minrect, (int)editRegion.ActualWidth, (int)editRegion.ActualHeight);//获取掩码

            effrect = extendRect(minrect, effsize);    //扩展区域

            cutPolygonbmp(BitmapImageToBitmap(new BitmapImage(new Uri("pack://application:,,,/Images/humen_bg.jpg"))), effrect, smobitmap,out mainbmp);

            //cutPolygonbmp(BitmapImageToBitmap(new BitmapImage(new Uri("pack://application:,,,/Images/humen_bg.jpg"))), effrect, smobitmap);




            //image.Source = BitmapToBitmapImage(mainbmp);

        }

        /// <summary>
        /// BitmapImage --> Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        /// <summary>
        /// Bitmap --> BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
            int i, j;
            for (i = 0; i < bitmap.Width; i++)
                for (j = 0; j < bitmap.Height; j++)
                {
                    Color pixelColor = bitmap.GetPixel(i, j);
                    Color newColor = Color.FromArgb(pixelColor.A, pixelColor.R, pixelColor.G, pixelColor.B);
                    bitmapSource.SetPixel(i, j, newColor);
                }
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private void limitRect(ref Rectangle rt, Size s)
        {
            if (rt.X < 0) rt.X = 0; if (rt.Y < 0) rt.Y = 0;
            if (rt.Right > s.Width) rt.Width = s.Width - rt.X;
            if (rt.Bottom > s.Height) rt.Height = s.Height - rt.Y;
        }

        /// <summary>
        /// 扩展矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="effsize"></param>
        /// <returns></returns>
        private Rectangle extendRect(Rectangle rect, int effsize)
        {
            rect.Location = new System.Drawing.Point(rect.X - effsize, rect.Y - effsize);
            rect.Size = new Size(rect.Width + 2 * effsize, rect.Height + 2 * effsize);
            return rect;
        }

        /// <summary>
        /// 生成N*N的矩阵，作为模糊卷积核
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        private int[,] CreateKernel(int N)
        {
            if (N <= 0) N = 1;
            int i, j;
            int[,] kn = new int[N, N];
            KernelSum = 0;
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < N; j++)
                {
                    int g = (Math.Abs(i - N / 2) > Math.Abs(j - N / 2)) ? Math.Abs(i - N / 2) : Math.Abs(j - N / 2);
                    kn[i, j] = N / 2 - g + 1;
                    KernelSum += kn[i, j];
                }
            }
            return kn;
        }


        /// <summary>
        /// 生成二值掩码图
        /// </summary>
        /// <param name="ptl"></param>
        /// <param name="rect"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private Bitmap createMaskbmp(List<Point> ptl, out Rectangle rect, int w, int h)
        {
            rect = seek_minRect(ptl);
            Bitmap tmbitmap = new Bitmap(w, h);
            BitmapData maskbitmapdata = tmbitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                UInt32* dstbyte = (UInt32*)(maskbitmapdata.Scan0.ToPointer());
                for (int y = 0; y < rect.Height; y++)
                {
                    for (int x = 0; x < rect.Width; x++)
                    {
                        Point pt = new Point(x + (int)rect.X, y + (int)rect.Y);
                        if (IsInPolygon(pt, ptl))
                        {
                            dstbyte[(x + rect.X) + (y + rect.Y) * w] = 0xffffffff;
                        }
                    }
                }
            }

            tmbitmap.UnlockBits(maskbitmapdata);

            image.Source = BitmapToBitmapImage(tmbitmap);

            return tmbitmap;
        }

        /// <summary>
        /// 寻找最小包含矩形
        /// </summary>
        /// <param name="ptl"></param>
        /// <returns></returns>
        private Rectangle seek_minRect(List<Point> ptl)
        {
            double minx = ptl[0].X, miny = ptl[0].Y;
            double maxx = ptl[0].X, maxy = ptl[0].Y;
            for (int i = 1; i < ptl.Count; i++)
            {
                if (ptl[i].X < minx)
                    minx = ptl[i].X;
                if (ptl[i].Y < miny)
                    miny = ptl[i].Y;
                if (ptl[i].X > maxx)
                    maxx = ptl[i].X;
                if (ptl[i].Y > maxy)
                    maxy = ptl[i].Y;
            }
            //RectangleF rect = new RectangleF(minx, miny, maxx, maxy);
            return new Rectangle((int)minx, (int)miny, (int)(maxx - minx + 1), (int)(maxy - miny + 1));
        }

        /// <summary>
        /// 判断点是否在多边形内.
        /// </summary>
        /// <param name="checkPoint"></param>
        /// <param name="polygonPoints"></param>
        /// <returns></returns>
        public static bool IsInPolygon(Point checkPoint, List<Point> polygonPoints)
        {
            bool inside = false;
            int pointCount = polygonPoints.Count;
            Point p1, p2;
            for (int i = 0, j = pointCount - 1; i < pointCount; j = i, i++)
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                if (checkPoint.Y < p2.Y)
                {
                    if (p1.Y <= checkPoint.Y)
                    {
                        if ((float)(checkPoint.Y - p1.Y) * (float)(p2.X - p1.X) > (float)(checkPoint.X - p1.X) * (float)(p2.Y - p1.Y))
                        {
                            inside = (!inside);
                        }
                    }
                }
                else if (checkPoint.Y < p1.Y)
                {
                    if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) < (checkPoint.X - p1.X) * (p2.Y - p1.Y))
                    {
                        inside = (!inside);
                    }
                }
            }
            return inside;
        }

        /// <summary>
        /// 获取多边形剪裁
        /// </summary>
        /// <param name="srcbmp"></param>
        /// <param name="rect"></param>
        /// <param name="maskbitmap"></param>
        /// <param name="dstbitmap"></param>
        /// <returns></returns>
        private Bitmap cutPolygonbmp(Bitmap srcbmp, Rectangle rect, Bitmap maskbitmap, out Bitmap dstbitmap)
        {
            dstbitmap = srcbmp.Clone() as Bitmap;
            int srcw = srcbmp.Width;
            Bitmap tmbitmap = new Bitmap((int)rect.Width, (int)rect.Height);
            BitmapData dstbitmapdata = dstbitmap.LockBits(new Rectangle(new System.Drawing.Point(0, 0), dstbitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData tmbitmapdata = tmbitmap.LockBits(new Rectangle(new System.Drawing.Point(0, 0), tmbitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                UInt32* srcbyte = (UInt32*)(dstbitmapdata.Scan0.ToPointer());
                UInt32* dstbyte = (UInt32*)(tmbitmapdata.Scan0.ToPointer());
                Bitmap tmaskbitmap = blurred(maskbitmap); //对二值掩码图模糊化




                for (int y = 0; y < rect.Height; y++)
                {
                    for (int x = 0; x < rect.Width; x++)
                    {
                        UInt32 mal = tmaskbitmap.GetPixel(x + (int)rect.X, y + (int)rect.Y).A; //获取掩码值
                        UInt32 c = srcbyte[(x + (int)rect.X) + (y + (int)rect.Y) * srcw];//获取原图颜色值
                        UInt32 bc = (c & 0x00ffffff);//基色
                        UInt32 al = (UInt32)c >> 24;//透明度

                        UInt32 dal = al * mal / 255;
                        UInt32 sal = al * (255 - mal) / 255;

                        dstbyte[x + y * rect.Width] = bc + ((dal) << 24);//填充
                        srcbyte[(x + (int)rect.X) + (y + (int)rect.Y) * srcw] = bc + ((sal) << 24);  //挖原图  
                    }
                }
            }
            dstbitmap.UnlockBits(dstbitmapdata);
            tmbitmap.UnlockBits(tmbitmapdata);
            return tmbitmap;
        }


        /// <summary>
        /// 获取多边形剪裁
        /// </summary>
        /// <param name="srcimg"></param>
        /// <param name="rect"></param>
        /// <param name="maskbitmap"></param>
        /// <returns></returns>
        private Bitmap cutPolygonbmp(Bitmap srcimg, Rectangle rect, Bitmap maskbitmap)
        {
            Bitmap srcbitmap = srcimg as Bitmap;
            int srcw = srcbitmap.Width;
            Bitmap dstbitmap = new Bitmap((int)rect.Width, (int)rect.Height);
            BitmapData srcbitmapdata = (srcbitmap).LockBits(new Rectangle(new System.Drawing.Point(0, 0), srcbitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData dstbitmapdata = dstbitmap.LockBits(new Rectangle(new System.Drawing.Point(0, 0), dstbitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                UInt32* srcbyte = (UInt32*)(srcbitmapdata.Scan0.ToPointer());
                UInt32* dstbyte = (UInt32*)(dstbitmapdata.Scan0.ToPointer());
                Bitmap tmaskbitmap = blurred(maskbitmap); //对二值掩码图模糊化
                for (int y = 0; y < rect.Height; y++)
                {
                    for (int x = 0; x < rect.Width; x++)
                    {
                        UInt32 mal = tmaskbitmap.GetPixel(x + rect.X, y + rect.Y).A; //获取掩码值
                        UInt32 c = srcbyte[(x + rect.X) + (y + rect.Y) * srcw];//获取原图颜色值
                        UInt32 bc = (c & 0x00ffffff);//基色
                        UInt32 al = (UInt32)c >> 24;//透明度

                        UInt32 dal = al * mal / 255;
                        UInt32 sal = al * (255 - mal) / 255;

                        dstbyte[x + y * rect.Width] = bc + ((dal) << 24);//填充
                        srcbyte[(x + rect.X) + (y + rect.Y) * srcw] = bc + ((sal) << 24);  //挖原图  
                    }
                }
            }
            srcbitmap.UnlockBits(srcbitmapdata);
            dstbitmap.UnlockBits(dstbitmapdata);
            return dstbitmap;
        }


        /// <summary>
        /// 开始模糊卷积运算
        /// </summary>
        /// <param name="srcbmp"></param>
        /// <returns></returns>
        private Bitmap blurred(Bitmap srcbmp)
        {


            if (srcbmp == null) return null;
            int Width = srcbmp.Width, Height = srcbmp.Height;
            int[,] InputPicbuf = new int[Width, Height];
            Color color = new Color();
            //只获取R通道
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    color = srcbmp.GetPixel(i, j);
                    InputPicbuf[i, j] = color.R;
                }
            }

            Bitmap blurbmp = new Bitmap(Width, Height);//创建新位图，保存模糊位图数据
            BitmapData blurbitmapdata = blurbmp.LockBits(new Rectangle(new System.Drawing.Point(0, 0), blurbmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                int* dstbyte = (int*)(blurbitmapdata.Scan0.ToPointer());
                Color c;
                for (int j = 0; j < Height; j++)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        c = integrateColorPoint(i, j, BlurKn, InputPicbuf, Width, Height);
                        dstbyte[i + j * Width] = c.ToArgb();
                    }
                }

            }
            blurbmp.UnlockBits(blurbitmapdata);


            //image.Source = BitmapToBitmapImage(blurbmp);


            return blurbmp;
        }

        /// <summary>
        /// 获取某点颜色积分
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="kn"></param>
        /// <param name="picbuf"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private Color integrateColorPoint(int i, int j, int[,] kn, int[,] picbuf, int w, int h)
        {
            int cr = 0;
            int KnSize = kn.GetLength(0);
            for (int r = 0; r < KnSize; r++)
            {
                int row = i - KnSize / 2 + r;
                for (int f = 0; f < KnSize; f++)
                {
                    int index = j - KnSize / 2 + f;
                    row = row < 0 ? 0 : row;
                    index = index < 0 ? 0 : index;
                    row = row >= w ? w - 1 : row;
                    index = index >= h ? h - 1 : index;
                    cr += kn[r, f] * picbuf[row, index];
                }
            }
            cr /= KernelSum;
            cr = cr > 255 ? 255 : cr;

            return Color.FromArgb(cr, 0, 0, 0);
        }
    }
}
