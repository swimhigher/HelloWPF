using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zcy.ViewModels.Welcome;
using Zcy.Services;
using Zcy.Services.Welcome;
using Zcy.Views;

namespace Zcy.Helpers
{
    static class ServiceRegister
    {
        public static void CustomerRegister(this SimpleContainer container)
        {
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();

            container.PerRequest<ShellViewModel>();
            container.PerRequest<IIntroduce, IntroduceViewModel>();
        }
    }
}
