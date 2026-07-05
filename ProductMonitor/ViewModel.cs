using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml.Linq;
using ProductMonitor.MVVMTemplate;
using ProductMonitor.UserControls;

namespace ProductMonitor
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            InitDateTimer();
            InitWindowButtonCommand();
            InintEnviroMentModel();
            InintAlarm();
            InitDeviceModel();
            InintRaderModelList();
            InintStaffOutWorkModelList();
            InintWorkShopModelList();
            InintMaChineModel();
            InintSettingCommand();
            InintSettingCloseCommand();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        #region 用户控件; --------------------------通知--------------------------
        private UserControl? _monitorUC;
        public UserControl MonitorUC
        {
            get
            {
                _monitorUC ??= new MonitorUC();//核心
                return _monitorUC;
            }
            set
            {
                _monitorUC = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MonitorUC)));//简写//简写
            }
        }
        #endregion

        #region 用户控件: --------------------------命令--------------------------
        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;
        public ICommand? CloseCommand { get; set; }
        public ICommand? MaximizedCommand { get; set; }
        public ICommand? MinimizedCommand { get; set; }
        public void InitWindowButtonCommand()
        {
            CloseCommand = new RelayCommand(Close);
            MaximizedCommand = new RelayCommand(MaxWindow);
            MinimizedCommand = new RelayCommand(MinWindow);

        }
        private void MinWindow()
        {
            if (mainWindow != null)
                mainWindow.WindowState = WindowState.Minimized;
        }
        //由于自定义窗口更改了边框使实际尺寸要不同，但是WPF不能同时和先执行放大化和设置窗口大小WoekArea
        //故放弃全屏(WindowState)方案改为无边框,还需加上ResizeMode = "CanMinimize"完全禁用最大化
        #region 废案
        //if (mainWindow.WindowState == WindowState.Maximized)
        //    mainWindow.WindowState = WindowState.Normal;
        //else
        //mainWindow.WindowState = WindowState.Maximized;
        //mainWindow.WindowState = mainWindow.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal; 
        #endregion
        #region 最大化和还原方法版
        private Rect bounds { get; set; }
        private bool _Maxbool;//用来切换最大化页面和还原页面
        private void MaxWindow()
        {
            if (_Maxbool)
            {
                //mainWindow.RestoreBounds;//这是保存最大化页面状态的方法
                //还原最大化之前的页面状态(Left,Top,Width,Height);
                mainWindow.Left = bounds.Left;
                mainWindow.Top = bounds.Top;
                mainWindow.Width = bounds.Width;
                mainWindow.Height = bounds.Height;
                _Maxbool = false;
            }
            else
            {
                //保存最大化之前的页面状态(Left,Top,Width,Height等)
                bounds = mainWindow.RestoreBounds;
                //最大化
                var workArea = SystemParameters.WorkArea;
                mainWindow.Left = workArea.Left;
                mainWindow.Top = workArea.Top;
                mainWindow.Width = workArea.Width;
                mainWindow.Height = workArea.Height;
                _Maxbool = true;
            }
        }
        #endregion
        private static void Close()
        {
            //MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;
            if (Application.Current.MainWindow is MainWindow mainWindow)//模式匹配
                //mainWindow.Close();
                Environment.Exit(0);//强制退出所有窗口
        }
        #endregion

        #region 用户控件; --------------------------行一列一: 时间-年月-星期[静态]--------------------------
        public string TimeStr { get { return DateTime.Now.ToString("HH:mm:ss"); } }
        public string DateStr { get { return DateTime.Now.ToString("yyyy:MM:dd"); } }
        //public string WeekStr { get { return DateTime.Now.DayOfWeek.ToString(); } }只有英文名
        public string WeekStr
        {
            get
            {
                int index = (int)DateTime.Now.DayOfWeek;
                string[] week = new string[7] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
                return week[index];
            }
        }
        #endregion

        #region 计时器: --------------------------行一列一: 时间-年月-星期[动态]--------------------------
        //DispatcherTimer: 定好间隔时间,触发时通过绑定的后台事件持续更新UI

        public DispatcherTimer dispatcherTimerTime { get; } = new DispatcherTimer();//时间
        public DispatcherTimer dispatcherTimerData { get; } = new DispatcherTimer();//日期
        #region 星期: 写多余了的
        //public DispatcherTimer dispatcherTimerWeek { get; } = new DispatcherTimer();//星期 
        #endregion
        public TimeSpan TwoTime => new TimeSpan(1, 0, 0, 0) - DateTime.Now.TimeOfDay;//=>是get{return  ;}简写
        //时间
        private string? _datetimeDT;
        public string DatetimeDT
        {
            get { return _datetimeDT ?? ""; }
            set
            {
                if (_datetimeDT != value)//当改变时触发,当不为空时触发[有点区别但不多]
                    _datetimeDT = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DatetimeDT)));
            }
        }
        //日期
        private string? _dateDataDT;
        public string DateDataDT
        {
            get { return _dateDataDT ?? ""; }
            set
            {
                if (_dateDataDT != value)//当改变时触发,当不为空时触发[有点区别但不多]
                    _dateDataDT = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DateDataDT)));
            }
        }
        //星期
        private string? _dateWeekDT;
        public string DateWeekDT
        {
            get { return _dateWeekDT ?? ""; }
            set
            {
                if (_dateWeekDT != value)//当改变时触发,当不为空时触发[有点区别但不多]
                    _dateWeekDT = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DateWeekDT)));//!: 可以不为空[会直接变为null的已改回?]
            }
        }
        public void InitDateTimer()
        {
            DatetimeDT = DateTime.Now.ToString("HH:mm:ss");//时间: 初始化显示一次
            dispatcherTimerTime.Interval = TimeSpan.FromSeconds(1);//时间: 触发间隔为每秒
            dispatcherTimerTime.Tick += DispatcherTimerTime_Tick;//绑定事件
            dispatcherTimerTime.Start();//启动定时器

            #region 日期: 未简化版
            //TimeSpan timeSpa= new TimeSpan(1, 0, 0, 0);
            //Double ts = timeSpan.TotalSeconds;//日期: TimeSpan转换为总秒数
            //Double df = DateTime.Now.TimeOfDay.TotalSeconds;//日期: 获取今天从零点开始到现在的总秒数
            //Double twoTime = ts - df;//日期: 得到现在到下一天0点的秒数 
            #endregion
            DateDataDT = DateTime.Now.ToString("yyyy:MM:dd");//日期: 初始化时显示一次
            dispatcherTimerData.Interval = TwoTime;//日期: 触发间隔为每天[搞错了这里本来需要的就是TimeSpan类型不用Total转换][每次执行时都会重复访问属性]
            dispatcherTimerData.Tick += DispatcherTimerDate_Tick;//绑定事件
            dispatcherTimerData.Start();//启动定时器

            #region 星期: 写多余了的
            //DateWeekDT = DateTime.Now.DayOfWeek.ToString();//星期: 初始化时显示一次
            //DateWeekDT = WeekStr;//因为ToString不支持[借用一下静态写的星期]
            //dispatcherTimerWeek.Interval = TwoTime;//星期: 触发间隔为每天
            //dispatcherTimerWeek.Tick += DispatcherTimerWeek_Tick;//绑定事件
            //dispatcherTimerWeek.Start();//启动定时器 
            #endregion
            DateWeekDT = WeekStr;//星期: 初始化时显示一次[WeekStr有写获取当前星期来着]

        }
        //时间
        private void DispatcherTimerTime_Tick(object? sender, EventArgs e)
        {
            DatetimeDT = DateTime.Now.ToString("HH:mm:ss");//时间
        }
        //日期 and 星期
        private void DispatcherTimerDate_Tick(object? sender, EventArgs e)
        {
            //日期
            DateDataDT = DateTime.Now.ToString("yyyy:MM:dd");//日期
            //星期
            DateWeekDT = WeekStr;
            //日期 or 星期: 重新替换时间间隔
            dispatcherTimerData.Interval = TwoTime;

            //当你关闭该窗口时可以在其他地方写个Stop()节省资源(包括时间)
        }
        #region 星期: 写多余了的
        ////星期
        //private void DispatcherTimerWeek_Tick(object? sender, EventArgs e)
        //{
        //    DateWeekDT = DateTime.Now.DayOfWeek.ToString(WeekStr);
        //    dispatcherTimerWeek.Interval = TwoTime;
        //} 
        #endregion
        #endregion

        #region 计数: --------------------------行一列二: 通知属性绑定[静态]--------------------------
        //能实现自动补位

        private string _maChineCount = "0298";
        public string MaChineCount
        {
            get { return _maChineCount; }
            set
            {
                if (_maChineCount != value)
                    _maChineCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaChineCount)));
            }
        }

        private string _productCount = "1643";
        public string ProductCount
        {
            get { return _productCount; }
            set
            {
                if (_productCount != value)
                    _productCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProductCount)));
            }
        }

        private string _badCount = "1643";
        public string BadCount
        {
            get { return _badCount; }
            set
            {
                if (_badCount != value)
                    _badCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BadCount)));
            }
        }
        #endregion

        #region 环境监控数据: --------------------------行二列一: 通知集合[静态]--------------------------

        private List<EnviroMentModel> _enviroMentModels;

        public List<EnviroMentModel> EnviroMentModelsList
        {
            get { return _enviroMentModels; }
            set
            {
                _enviroMentModels = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(EnviroMentModelsList)));//可简写删除EnviroMentModels
            }
        }
        public void InintEnviroMentModel()
        {
            EnviroMentModel enviroMentModel = new EnviroMentModel();
            EnviroMentModelsList = new List<EnviroMentModel>();
            EnviroMentModelsList.Add(new EnviroMentModel { EMName = "光照(Lux)", EMValue = 123 });
            EnviroMentModelsList.Add(new EnviroMentModel { EMName = "噪音(db)", EMValue = 55 });
            EnviroMentModelsList.Add(new EnviroMentModel { EMName = "温度(℃)", EMValue = 80 });
            EnviroMentModelsList.Add(new EnviroMentModel { EMName = "湿度(%)", EMValue = 43 });
            EnviroMentModelsList.Add(new EnviroMentModel { EMName = "PM2.5(m³)", EMValue = 20 });
            EnviroMentModelsList.Add(new EnviroMentModel { EMName = "硫化氢(PPM)", EMValue = 15 });
            EnviroMentModelsList.Add(new EnviroMentModel { EMName = "氮气(PPM)", EMValue = 18 });
            //MessageBox.Show(EnviroMentModelsList.ToString());
            //foreach(var index in EnviroMentModelsList)
            //{
            //    MessageBox.Show(index.EMName.ToString());
            //    MessageBox.Show(index.EMValue.ToString());
            //}
            //MessageBox.Show(EnviroMentModelsList[0].EMName.ToString())
        }
        #endregion

        #region 报警属性: --------------------------行二列二: 通知集合[静态]--------------------------
        private List<AlarmModel> _alarmList;

        public List<AlarmModel> AlarmList
        {
            get { return _alarmList; }
            set
            {
                if (_alarmList != value)
                    _alarmList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlarmList)));
            }
        }

        private void InintAlarm()
        {
            AlarmModel alarmModel = new AlarmModel();
            AlarmList = new List<AlarmModel>();
            AlarmList.Add(new AlarmModel { Num = "01", Message = "设备温度过高", Time = "2023-11-23 18:34:56", Duration = 7 });
            AlarmList.Add(new AlarmModel { Num = "02", Message = "车间温度过高", Time = "2023-12-08 20:40:59", Duration = 10 });
            AlarmList.Add(new AlarmModel { Num = "03", Message = "设备转速过快", Time = "2024-01-05 12:24:34", Duration = 12 });
            AlarmList.Add(new AlarmModel { Num = "04", Message = "设备气压偏低", Time = "2024-02-04 19:58:00", Duration = 90 });
        }
        #endregion

        #region 设备监控数据: --------------------------行二列三行一: 通知属性绑定[静态]--------------------------
        public ObservableCollection<DeviceModel> deviceModelList { get; } = new ObservableCollection<DeviceModel>();
        private void InitDeviceModel()
        {
            deviceModelList.Add(new DeviceModel { DeviceName = "60.8", DeviceValue = "电能(Kw.h)" });
            deviceModelList.Add(new DeviceModel { DeviceName = "390", DeviceValue = "电压(V)" });
            deviceModelList.Add(new DeviceModel { DeviceName = "5", DeviceValue = "电流(A)" });
            deviceModelList.Add(new DeviceModel { DeviceName = "13", DeviceValue = "压差(Kpa)" });
            deviceModelList.Add(new DeviceModel { DeviceName = "36", DeviceValue = "温度(℃)" });
            deviceModelList.Add(new DeviceModel { DeviceName = "40.1", DeviceValue = "振动(mm/s)" });
            deviceModelList.Add(new DeviceModel { DeviceName = "2600", DeviceValue = "转速(r/min)" });
            deviceModelList.Add(new DeviceModel { DeviceName = "0.5", DeviceValue = "气压(Kpa)" });
        }
        #endregion

        #region 能耗: --------------------------行二列三行二: 雷达--------------------------
        public ObservableCollection<RaderModel> ObservableRaderModelList { get; } = new ObservableCollection<RaderModel>();
        private void InintRaderModelList()
        {
            ObservableRaderModelList.Add(new RaderModel { ItemName = "排烟风机", Value = 90 });
            ObservableRaderModelList.Add(new RaderModel { ItemName = "客梯", Value = 30.00 });
            ObservableRaderModelList.Add(new RaderModel { ItemName = "供水机", Value = 34.89 });
            ObservableRaderModelList.Add(new RaderModel { ItemName = "喷淋水泵", Value = 69.59 });
            ObservableRaderModelList.Add(new RaderModel { ItemName = "稳压设备", Value = 20 });
        }
        #endregion

        #region 人力: --------------------------行二列三行三: 缺岗统计--------------------------
        public ObservableCollection<StaffOutWorkModel> observableStaffOutWorkModelList { get; } = new ObservableCollection<StaffOutWorkModel>();
        private void InintStaffOutWorkModelList()
        {
            observableStaffOutWorkModelList.Add(new StaffOutWorkModel { StaffName = "张晓婷", Positon = "技术员", OutWorkCount = 123 });
            observableStaffOutWorkModelList.Add(new StaffOutWorkModel { StaffName = "李晓", Positon = "操作员", OutWorkCount = 23 });
            observableStaffOutWorkModelList.Add(new StaffOutWorkModel { StaffName = "王克俭", Positon = "技术员", OutWorkCount = 134 });
            observableStaffOutWorkModelList.Add(new StaffOutWorkModel { StaffName = "陈家栋", Positon = "统计员", OutWorkCount = 143 });
            observableStaffOutWorkModelList.Add(new StaffOutWorkModel { StaffName = "杨过", Positon = "技术员", OutWorkCount = 12 });
        }
        #endregion

        #region 车间: --------------------------行三: 车间信息--------------------------
        public ObservableCollection<WorkShopModel> workShopModelList { get; } = new ObservableCollection<WorkShopModel>();
        private void InintWorkShopModelList()
        {
            workShopModelList.Add(new WorkShopModel { WorkShopName = "贴片车间", TotalCount = 44, WorkingCount = 32, WaitCount = 8, WrongCount = 4, StopCount = 0 });
            workShopModelList.Add(new WorkShopModel { WorkShopName = "封装车间", TotalCount = 32, WorkingCount = 20, WaitCount = 8, WrongCount = 4, StopCount = 0 });
            workShopModelList.Add(new WorkShopModel { WorkShopName = "焊接车间", TotalCount = 56, WorkingCount = 32, WaitCount = 10, WrongCount = 4, StopCount = 10 });
            workShopModelList.Add(new WorkShopModel { WorkShopName = "贴片车间", TotalCount = 80, WorkingCount = 68, WaitCount = 8, WrongCount = 4, StopCount = 0 });
        }
        #endregion

        //-------------------------------------------------------------------------------------------------------

        #region 车间详情页: --------------------------控件: 弹出车间--------------------------
        /// <summary>
        /// 显示车间页
        /// </summary>
        private void WorkShopUC()
        {
            WorkShopUserControl workShopUserControl = new WorkShopUserControl();
            MonitorUC = workShopUserControl;
        }
        public ICommand WorkShopCommand { get => new RelayCommand(WorkShopUC); set { } }
        #endregion

        #region 监控页: --------------------------控件: 返回监控页--------------------------
        public ICommand MonitorUCCommand { get => new RelayCommand(MonitorUCRetrun); set { } }
        private void MonitorUCRetrun()
        {
            MonitorUC monitorUC = new MonitorUC();
            MonitorUC = monitorUC;
        }
        #endregion

        #region 机台: --------------------------控件: 机台集合属性--------------------------
        private List<MaChineModel> _maChineModelCollection;

        public List<MaChineModel> MaChineModelCollection
        {
            get { return _maChineModelCollection; }
            set
            {
                if (value != _maChineModelCollection)
                    _maChineModelCollection = value;
                if (_maChineModelCollection != null)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaChineModelCollection)));
            }
        }
        private void InintMaChineModel()
        {
            List<MaChineModel> MachineList = new List<MaChineModel>();
            Random random = new Random();
            for (int i = 0; i < 20; i++)
            {
                int plan = random.Next(100, 1000);//计划量 随机数
                int finished = random.Next(0, plan);//已完成量
                MachineList.Add(new MaChineModel
                {
                    MachineName = "焊接机-" + (i + 1),
                    FinishedCount = finished,
                    PlanCount = plan,
                    Status = "作业中",
                    OrderNo = "H202212345678"
                });
            }
            MaChineModelCollection = MaChineModelCollection = MachineList;//同步
        }
        #endregion

        #region 配置页: --------------------------控件: 弹出配置界面--------------------------
        public ICommand settingCommand { get; set; }
        private Window _settingWindow;
        private void InintSettingCommand() 
        {
            settingCommand = new RelayCommand(Setting);
        }
        private void Setting()
        {
            //| 事件 | 触发时机 |
            //| ------| ----------|
            //| Loaded | 窗口加载完成 |
            //| Closing | 窗口关闭前（可取消） |
            //| Closed | 窗口关闭后 |
            if (_settingWindow == null)
            {
                _settingWindow = new SettingWindow
                { 
                    //Content = new SettingUC(),//如果你不使用自定义窗口就可以使用UC[使用UC看你需不需要在一个板块复用(这里不需要)]
                    //WindowStartupLocation = WindowStartupLocation.CenterScreen//如果不设置Owner的话,点击主窗口时弹出的窗口会自动隐藏
                    Owner = mainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                _settingWindow.Show();
                //_settingWindow.ShowDialog();//关闭前无法操作其他窗口[也可以用它来解决重复触发的问题,但更多是用于像登录后才能跳转页面的场景]
                //事件订阅的lamda必须写参数(sender,EventArgs)or匿名委托,or(_,_)[没什么区别就写你喜欢的的风格]
                _settingWindow.Closed += (s, e) => _settingWindow = null;//如果你手动关闭窗口时触发
                // _settingWindow.Closed += delegate { _settingWindow = null; };
                //_settingWindow.Closed += (_, _) =>  _settingWindow = null;
            }
            else
            {
                _settingWindow.Activate();//触发之前的窗口
            }
        }

        #endregion

        #region 配置: --------------------------命令: 关闭配置界面--------------------------
        public ICommand SettingCloseCommand { get; set; }
        private void InintSettingCloseCommand()
        {
            SettingCloseCommand = new RelayCommand(SettingClose);
        }
        private void SettingClose()
        {
            #region 获取特定窗口[不建议,因为是通过遍历查找的费性能(和获取控件一样),更推荐直接在View里写Click或用事件回调]
            Window? settingWindow = Application.Current.Windows
                    .OfType<SettingWindow>()
                    .FirstOrDefault();
            if (settingWindow != null)
            settingWindow.Close();
            #endregion
        }
        #endregion
    }
}
