using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ProductMonitor.UserControls
{
    /// <summary>
    /// RaderUC.xaml 的交互逻辑
    /// </summary>
    public partial class RaderUC : UserControl
    {
        public RaderUC()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InintRader();
        }

        public ObservableCollection<RaderModel> RanderList
        {
            get { return (ObservableCollection<RaderModel>)GetValue(RanderListProperty); }
            set { SetValue(RanderListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RanderList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RanderListProperty =
            DependencyProperty.Register(nameof(RanderList), typeof(ObservableCollection<RaderModel>), typeof(RaderUC));

        //画布方法
        private void InintRader()
        {
            //判断数据是否存在
            if (RanderList == null || RanderList.Count == null)
                return;
            //清除之前的画布
            mainCanvas.Children.Clear();
            P1.Points.Clear();
            P2.Points.Clear();
            P3.Points.Clear();
            P4.Points.Clear();
            P5.Points.Clear();
            //调整大小(正方体)
            double size = Math.Min(RenderSize.Width, RenderSize.Height);
            LayGrid.Width = size;
            LayGrid.Height = size;
            //半径
            double radius = size / 2;
            //步子跨度
            double step = 360.0 / RanderList.Count;
            for(int i = 0; i < RanderList.Count; i++)
            {
                double x = (radius - 5) * Math.Cos((step * i - 90) * Math.PI / 180);//X偏移量
                double y = (radius - 5) * Math.Sin((step * i - 90) * Math.PI / 180);//Y偏移量
                //X Y坐标
                P1.Points.Add(new Point(radius + x, radius + y));
                P2.Points.Add(new Point(radius + x * 0.75, radius + y * 0.75));
                P3.Points.Add(new Point(radius + x * 0.5, radius + y * 0.5));
                P4.Points.Add(new Point(radius + x * 0.25, radius + y * 0.25));
                P5.Points.Add(new Point(radius + x * RanderList[i].Value * 0.01, radius + y * RanderList[i].Value * 0.01));
                //文字内容Title
                TextBlock Title = new TextBlock();
                Title.FontSize = 10;
                Title.Width = 60;
                Title.TextAlignment = TextAlignment.Center;
                Title.Text = RanderList[i].ItemName;
                Title.Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
                //设置左边间距
                Title.SetValue(Canvas.LeftProperty, radius + (radius - 10) * Math.Cos((step * i - 90)*Math.PI/180)-20);
                Title.SetValue(Canvas.TopProperty, radius + (radius - 10) * Math.Sin((step * i - 90)*Math.PI/180)-7);
                //加入画布
                mainCanvas.Children.Add(Title);
            }
        }
    }
}
