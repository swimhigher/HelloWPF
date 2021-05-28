using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Zcy.View
{
    public class ShellViewModel : ViewAware
    {
        public Button WinMaxBtn;
        public Button WinRestoreBtn;

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

            var frameworkElement = view as FrameworkElement;

            if (frameworkElement == null)
            {
                return;
            }

            WinMaxBtn = frameworkElement.FindName("WinMaxBtn") as Button;
            WinRestoreBtn = frameworkElement.FindName("WinRestoreBtn") as Button;
        }

        private readonly IWindowManager _windowManager;


        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }


        public bool CanSayHello
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public void SayHello()
        {
            MessageBox.Show(string.Format("Hello {0}!", Name)); //Don't do this in real life :)
        }

        public void Border_MouseDown(MouseButtonEventArgs e, object view)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (view as Window).DragMove();
            }
        }
        public void WinClose()
        {
            Environment.Exit(Environment.ExitCode);
        }
        Rect rcnormal;
        public void WinMax(object view)
        {
            WinMaxBtn.Visibility = Visibility.Collapsed;
            WinRestoreBtn.Visibility = Visibility.Visible;
            var window = (view as Window);
            rcnormal = new Rect(window.Left, window.Top, window.Width, window.Height);//保存下当前位置与大小
            window.Left = 0;//设置位置
            window.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            window.Width = rc.Width;
            window.Height = rc.Height;
        }
        public void WinRestore(object view)
        {
            WinMaxBtn.Visibility = Visibility.Visible;
            WinRestoreBtn.Visibility = Visibility.Collapsed;

            var window = (view as Window);
            window.Left = rcnormal.Left;
            window.Top = rcnormal.Top;
            window.Width = rcnormal.Width;
            window.Height = rcnormal.Height;

        }
        public void WinMin(object view)
        {
            var window = (view as Window);

            window.WindowState = System.Windows.WindowState.Minimized;
        }

    }
}
