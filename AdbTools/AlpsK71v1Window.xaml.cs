using AdbTools.bean;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdbTools
{
    /// <summary>
    /// AlpsK71v1Window.xaml 的交互逻辑
    /// </summary>
    public partial class AlpsK71v1Window : Window
    {
        private ObservableCollection<OperateLog> operateLogList;

        public string AdbPath { get; set; }
        public Device DeviceInfo { get; set; }


        public AlpsK71v1Window()
        {
            InitializeComponent();

            operateLogList = new ObservableCollection<OperateLog>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logs.ItemsSource = operateLogList;

            Operate1();
        }

        private void PrintSucessLog(string appName, string package, string cmd)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                OperateLog operateLog = new OperateLog();
                operateLog.Message = $"应用【{appName}】处理结果";
                operateLog.Status = "停用";
                operateLog.StatusColor = Brushes.Green;
                operateLog.Cmd = cmd;
                operateLogList.Add(operateLog);
            }));
        }

        private void PrintUninstallLog(string appName, string package, string cmd)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                OperateLog operateLog = new OperateLog();
                operateLog.Message = $"应用【{appName}】处理结果";
                operateLog.Status = "卸载";
                operateLog.StatusColor = Brushes.Green;
                operateLog.Cmd = cmd;
                operateLogList.Add(operateLog);
            }));
        }

        private void PrintFailLog(string appName, string package, string cmd)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                OperateLog operateLog = new OperateLog();
                operateLog.Message = $"应用【{appName}】处理结果";
                operateLog.Status = "失败";
                operateLog.StatusColor = Brushes.Red;
                operateLog.Cmd = cmd;
                operateLogList.Add(operateLog);
            }));
        }

        private void PrintEndLog()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                OperateLog operateLog = new OperateLog();
                operateLog.Message = $"执行完成";
                operateLog.Status = "";
                operateLog.StatusColor = Brushes.Black;
                operateLog.Cmd = "";
                operateLogList.Add(operateLog);
            }));
        }


        private void Operate1()
        {
            new Thread(() =>
            {
                string appName = "电话";
                string package = "com.android.dialer";
                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user {package} ";
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate2();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate2();
                    return true;
                });
            }).Start();
        }

        private void Operate2()
        {
            new Thread(() =>
            {
                string appName = "通讯录";
                string package = "com.android.contacts";

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user {package} ";
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate3();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate3();
                    return true;
                });
            }).Start();
        }

        private void Operate3()
        {
            new Thread(() =>
            {
                string appName = "短信";
                string package = "com.android.mms";

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user {package} ";
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate4();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate4();
                    return true;
                });
            }).Start();
        }

        private void Operate4()
        {
            new Thread(() =>
            {
                string appName = "SIM卡工具包";
                string package = "com.android.stk".Trim().Replace("\0", "");

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user " + package;
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate5();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate5();
                    return true;
                });
            }).Start();
        }

        private void Operate5()
        {
            new Thread(() =>
            {
                string appName = "FM电台";
                string package = "com.android.music".Trim().Replace("\0", "");

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user " + package;
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate6();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate6();
                    return true;
                });
            }).Start();
        }

        private void Operate6()
        {
            new Thread(() =>
            {
                string appName = "音乐";
                string package = "com.android.fmradio".Trim().Replace("\0", "");

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user " + package;
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate7();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate7();
                    return true;
                });
            }).Start();
        }

        private void Operate7()
        {
            new Thread(() =>
            {
                string appName = "MTK（联发科）非框架定位服务";
                string package = "com.mediatek.gnss.nonframeworklbs";

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user " + package;
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate8();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate8();
                    return true;
                });
            }).Start();
        }

        private void Operate8()
        {
            new Thread(() =>
            {
                string appName = "Debug调试记录";
                string package = "com.debug.loggerui".Trim().Replace("\0", "");

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user " + package;
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate9();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate9();
                    return true;
                });
            }).Start();
        }

        private void Operate9()
        {
            new Thread(() =>
            {
                string appName = "时钟";
                string package = "com.android.deskclock".Trim().Replace("\0", "");
                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} shell pm disable-user " + package;
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintSucessLog(appName, package, cmd);
                    Operate10();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate10();
                    return true;
                });
            }).Start();
        }


        private void Operate10()
        {
            new Thread(() =>
            {
                string appName = "电子邮件";
                string package = "com.android.email".Trim().Replace("\0", "");
                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} uninstall {package} ";
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintUninstallLog(appName, package, cmd);
                    Operate11();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    Operate11();
                    return true;
                });
            }).Start();
        }

        private void Operate11()
        {
            new Thread(() =>
            {
                string appName = "Search";
                string package = "com.android.quicksearchbox".Trim().Replace("\0", "");

                string cmd = $"{AdbPath} -s {DeviceInfo.DeviceMark} uninstall {package} ";
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { cmd }, result =>
                {
                    PrintUninstallLog(appName, package, cmd);
                    PrintEndLog();
                },
                err =>
                {
                    PrintFailLog(appName, package, cmd);
                    PrintEndLog();
                    return true;
                });
            }).Start();
        }

        private void retry_Click(object sender, RoutedEventArgs e)
        {
            int index = logs.SelectedIndex;
            OperateLog operateLog = operateLogList[index];

            Clipboard.SetText(operateLog.Cmd);
        }
    }
}
