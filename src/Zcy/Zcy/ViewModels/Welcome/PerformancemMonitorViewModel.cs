using Caliburn.Micro;
using Core.Helper;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Zcy.ViewModels.Welcome
{
    public class PerformancemMonitorViewModel : Screen
    {

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var nowDate = DateTime.Now;
            CpuUseRateList = new ChartValues<float>();
            MemoryUseRateList = new ChartValues<float>();
            PhysicalMemoryUseRateList = new ChartValues<float>();
            SurplusPhysicalMemoryUseRateList = new ChartValues<float>();
            MemoryUse = 0;
            // CPU_AxisX = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                CpuUseRateList.Add(0);
                MemoryUseRateList.Add(0);
                // CPU_AxisX.Add(nowDate.AddSeconds(i - 9).ToString("mm-ss"));
            }
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.IsEnabled = true;
            timer.Start();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (UsageHelper._cpuUsageList.Count > 0)
                {
                    CpuUseRateList.RemoveAt(0);
                    CpuUseRateList.Add((float)Math.Round(UsageHelper._cpuUsageList.Last().value, 2));
                    // CPU_AxisX.RemoveAt(0);
                    // CPU_AxisX.Add(UsageHelper._cpuUsageList.Last().datetime.ToString("mm-ss"));
                }
                if (UsageHelper._MemoryUsageList.Count > 0)
                {
                    MemoryUse = (float)Math.Round(UsageHelper._MemoryUsageList.Last().value / UsageHelper.PhysicalMemory * 100, 2);
                    MemoryUseRateList.RemoveAt(0);
                    MemoryUseRateList.Add((float)Math.Round(UsageHelper._MemoryUsageList.Last().value / UsageHelper.PhysicalMemory * 100, 2));

                }
                PhysicalMemoryUseRateList.Clear();
                SurplusPhysicalMemoryUseRateList.Clear();
                var Phy = UsageHelper.GetLogicalDrives_Use();
                PhysicalMemoryUseRateList.AddRange(Phy.Select(p => p.Item3).ToList());
                SurplusPhysicalMemoryUseRateList.AddRange(Phy.Select(p => p.Item2).ToList());
                PhysicalAxis = Phy.Select(p => p.Item1).ToList();
            });
        }

        private ChartValues<float> _CpuUseRateList;

        public ChartValues<float> CpuUseRateList
        {
            get { return _CpuUseRateList; }
            set { _CpuUseRateList = value; NotifyOfPropertyChange(() => CpuUseRateList); }
        }

        private ChartValues<float> _MemoryUseRateList;

        public ChartValues<float> MemoryUseRateList
        {
            get { return _MemoryUseRateList; }
            set { _MemoryUseRateList = value; NotifyOfPropertyChange(() => MemoryUseRateList); }
        }

        private ChartValues<float> _PhysicalMemoryUseRateList;

        public ChartValues<float> PhysicalMemoryUseRateList
        {
            get { return _PhysicalMemoryUseRateList; }
            set { _PhysicalMemoryUseRateList = value; NotifyOfPropertyChange(() => PhysicalMemoryUseRateList); }
        }
        private ChartValues<float> _SurplusPhysicalMemoryUseRateList;

        public ChartValues<float> SurplusPhysicalMemoryUseRateList
        {
            get { return _SurplusPhysicalMemoryUseRateList; }
            set { _SurplusPhysicalMemoryUseRateList = value; NotifyOfPropertyChange(() => SurplusPhysicalMemoryUseRateList); }
        }

        private double _MemoryUse;

        public double MemoryUse
        {
            get { return _MemoryUse; }
            set { _MemoryUse = value; NotifyOfPropertyChange(() => MemoryUse); }
        }

        public List<string> PhysicalAxis { get => _PhysicalAxis; set { _PhysicalAxis = value; NotifyOfPropertyChange(() => PhysicalAxis); } }

        private List<string> _PhysicalAxis;





        #region 仪表盘色彩 弃用
        //public SolidColorBrush NeedleBrush
        //{
        //    get { return _NeedleBrush; }
        //    set { _NeedleBrush = value; NotifyOfPropertyChange(() => NeedleBrush); }
        //}
        //public SolidColorBrush NormalBrush { get => normalBrush; set { normalBrush = value; NotifyOfPropertyChange(() => NormalBrush); } }
        //public SolidColorBrush WarnBrush { get => warnBrush; set { warnBrush = value; NotifyOfPropertyChange(() => WarnBrush); } }
        //public SolidColorBrush ErrorBrush { get => errorBrush; set { errorBrush = value; NotifyOfPropertyChange(() => ErrorBrush); } }

        //private SolidColorBrush _NeedleBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 118, 33, 255));
        //private SolidColorBrush normalBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(167, 16, 183, 3));
        //private SolidColorBrush warnBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 152, 0));
        //private SolidColorBrush errorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 244, 67, 54)); 
        #endregion

    }
}
