using Caliburn.Micro;
using Core.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using Zcy.ViewModels.Welcome;

namespace Zcy.Views
{
    public class ShellViewModel : Conductor<object>
    {
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            UserName = "Admin";
            MenuJsonHelper.Init();
            TreeMenus = MenuJsonHelper.Menus;
        }

        private readonly IWindowManager _windowManager;


        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            UsageHelper.Initialize();
            ActivateItemAsync(new PerformancemMonitorViewModel());
        }

        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }
        private List<MenuModel> _TreeMenus;

        public List<MenuModel> TreeMenus
        {
            get { return _TreeMenus; }
            set { _TreeMenus = value; NotifyOfPropertyChange(() => TreeMenus); }
        }

        public void MenuClick(object data)
        {


            try
            {
                var SelectedMenu = data as MenuModel;
                if (string.IsNullOrWhiteSpace(SelectedMenu.Url))
                {
                    return;
                }
                var pageName = $"Zcy.ViewModels.{SelectedMenu.Url}";
                if (pageName == ActiveItem.ToString())
                {
                    return;
                }
                //从程序集中获取指定对象类型;
                Type type = Assembly.GetExecutingAssembly().GetType(pageName); //程序集名称.类名
                Object obj = type.Assembly.CreateInstance(type.ToString());
                ActivateItemAsync(obj);

            }
            catch (Exception)
            {


            }
        }

        #region 弃用  改用windowsChrome实现
        //public Button WinMaxBtn;
        //public Button WinRestoreBtn;

        //protected override void OnViewAttached(object view, object context)
        //{
        //    base.OnViewAttached(view, context);

        //    var frameworkElement = view as FrameworkElement;

        //    if (frameworkElement == null)
        //    {
        //        return;
        //    }

        //    WinMaxBtn = frameworkElement.FindName("WinMaxBtn") as Button;
        //    WinRestoreBtn = frameworkElement.FindName("WinRestoreBtn") as Button;
        //}
        //public void Border_MouseDown(MouseButtonEventArgs e, object view)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        (view as Window).DragMove();
        //    }
        //}
        //public void WinClose()
        //{
        //    Environment.Exit(Environment.ExitCode);
        //}
        //Rect rcnormal;
        //public void WinMax(object view)
        //{
        //    WinMaxBtn.Visibility = Visibility.Collapsed;
        //    WinRestoreBtn.Visibility = Visibility.Visible;
        //    var window = (view as Window);
        //    rcnormal = new Rect(window.Left, window.Top, window.Width, window.Height);//保存下当前位置与大小
        //    window.Left = 0;//设置位置
        //    window.Top = 0;
        //    Rect rc = SystemParameters.WorkArea;//获取工作区大小
        //    window.Width = rc.Width;
        //    window.Height = rc.Height;
        //}
        //public void WinRestore(object view)
        //{
        //    WinMaxBtn.Visibility = Visibility.Visible;
        //    WinRestoreBtn.Visibility = Visibility.Collapsed;

        //    var window = (view as Window);
        //    window.Left = rcnormal.Left;
        //    window.Top = rcnormal.Top;
        //    window.Width = rcnormal.Width;
        //    window.Height = rcnormal.Height;

        //}
        //public void WinMin(object view)
        //{
        //    var window = (view as Window);

        //    window.WindowState = System.Windows.WindowState.Minimized;
        //}
        #endregion

    }
}
