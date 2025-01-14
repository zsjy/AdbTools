using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AdbTools
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                Process current = Process.GetCurrentProcess();
                Process[] createMeetingProcess = Process.GetProcessesByName(current.ProcessName);
                if (createMeetingProcess.Count() > 1)
                {
                    ///退出当前新开进程，不走OnExit方法
                    Environment.Exit(0);
                    //Application.Current.Shutdown();
                    return;
                }
            }
            catch { }


            //UI线程未捕获异常处理事件（UI主线程）
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }


        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show("未捕获Task线程异常：" + e.ToString() + "\r\n异常消息：" + e.Exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Environment.Exit(0);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show("未捕获非UI线程异常：" + e.ToString() + "\r\n对象：" + e.ExceptionObject, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var comException = e.Exception as System.Runtime.InteropServices.COMException;
            if (comException != null && comException.ErrorCode == -2147221040)
            {
                e.Handled = true;
                return;
            }

            e.Handled = true;

            Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show("未捕获UI线程异常：" + e.ToString() + "\r\n异常消息：" + e.Exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }
    }
}
