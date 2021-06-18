using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Zcy;
using Zcy.Helpers;
using Zcy.Views;

namespace Zcy
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
            Application.DispatcherUnhandledException += Application_DispatcherUnhandledException; 
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; 
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            var msg = string.Format("捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}\r\nCLR即将退出：{3}", ex.GetType(), ex.Message, ex.StackTrace, e.IsTerminating);

            // Log.Error(msg);
            MessageBox.Show("系统异常");
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Log.Error(e.Exception, "系统异常");
            MessageBox.Show("系统异常");
            e.Handled = true;
        }

        private SimpleContainer container;

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
        protected override void Configure()
        {
            container = new SimpleContainer();
            container.CustomerRegister();
            
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }



        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetExecutingAssembly() };
        }
    }
}
