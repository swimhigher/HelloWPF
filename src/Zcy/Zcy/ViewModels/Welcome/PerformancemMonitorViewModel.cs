using Caliburn.Micro;
using Core.Helper;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Zcy.ViewModels.Welcome
{
    public class PerformancemMonitorViewModel : Screen
    {

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += timer_Tick;
            timer.IsEnabled = true;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (UsageHelper._cpuUsageList.Count > 10)
            {
                CpuUseRateList = new ChartValues<float>(UsageHelper._cpuUsageList.Skip(UsageHelper._cpuUsageList.Count - 10).Take(10).ToList());
            }
            else
            {
                CpuUseRateList = new ChartValues<float>(UsageHelper._cpuUsageList.Take(10).ToList());
            }
            if (CpuUseRateList != null)
            {
                CpuUseRateChat = string.Join(',', CpuUseRateList);
            }
        }

        private ChartValues<float> _CpuUseRateList;

        public ChartValues<float> CpuUseRateList
        {
            get { return _CpuUseRateList; }
            set { _CpuUseRateList = value; NotifyOfPropertyChange(() => CpuUseRateList); }
        }
        private string _CpuUseRateChat;

        public string CpuUseRateChat
        {
            get { return _CpuUseRateChat; }
            set { _CpuUseRateChat = value; NotifyOfPropertyChange(() => CpuUseRateChat); }
        }


    }
}
