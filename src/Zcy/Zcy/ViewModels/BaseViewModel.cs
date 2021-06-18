using Caliburn.Micro;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zcy.Services;

namespace Zcy.ViewModels
{
    public class BaseViewModel : Conductor<object>, IScreenService
    {

        IEventAggregator events;
     
     
        public BaseViewModel(IEventAggregator events)
        {
            this.events = events;
          
        }

        private string _Title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { _Title = value; NotifyOfPropertyChange(() => Title); }
        }
        /// <summary>
        /// 正常提示
        /// </summary>
        /// <param name="Message"></param>
        public void Info(string Message)
        {
            Growl.Info(Message, "InfoMsg");
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="Message"></param>
        public void Warn(string Message)
        {
            Growl.Warning(Message, "InfoMsg");
        }
        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="Message"></param>
        public void Error(string Message)
        {
            Growl.Error(Message, "InfoMsg");
        }

    }
}
