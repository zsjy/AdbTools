using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AdbTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string adbPath = "";
        private DispatcherTimer timer;
        private ObservableCollection<string> deviceAddressHistory;
        private ObservableCollection<string> deviceAddressList;

        public MainWindow()
        {
            InitializeComponent();

            deviceAddressHistory = new ObservableCollection<string>();
            deviceAddressList = new ObservableCollection<string>();
        }

        private void refreshDeviceAddressHistory()
        {
            deviceAddressHistory.Clear();
            Globals.AppSettings.DEVICE_ADDRESS_HISTORY.ForEach(s =>
            {
                deviceAddressHistory.Add(s);
            });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //adb相关资源文件
            string path = AppDomain.CurrentDomain.BaseDirectory + "adb.exe";
            string adbDll1 = AppDomain.CurrentDomain.BaseDirectory + "AdbWinApi.dll";
            string adbDll2 = AppDomain.CurrentDomain.BaseDirectory + "AdbWinUsbApi.dll";
            if (!File.Exists(path) || !File.Exists(adbDll1) || !File.Exists(adbDll2))
            {
                MessageBox.Show("adb工具集丢失，无法运行！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                return;
            }
            adbPath = $"\"{path}\"";

            deviceAddress.Text = Globals.AppSettings.LAST_DEVICE_ADDRESS;
            deviceAddress.ItemsSource = deviceAddressHistory;
            refreshDeviceAddressHistory();

            deviceList.ItemsSource = deviceAddressList;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick; 
            timer.Start(); 

            Timer_Tick(null, null);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            deviceAddressList.Clear();
            string content = CmdExecutor.ExecuteCommandAndReturn($"{adbPath} devices");
            // 定义正则表达式模式
            string pattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d+\b";
            // 创建 Regex 对象
            Regex regex = new Regex(pattern);
            // 查找所有匹配项
            MatchCollection matches = regex.Matches(content);
            // 输出所有匹配项
            foreach (Match match in matches)
            {
                deviceAddressList.Add(match.Value.Trim());
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void connectDevice_Click(object sender, RoutedEventArgs e)
        {
            string address = deviceAddress.Text;
            if (!IsValidIPEndPoint(address))
            {
                MessageBox.Show("设备连接地址输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CmdExecutor.ExecuteCommandAndQuit($"{adbPath} connect {address}");
            Globals.AppSettings.LAST_DEVICE_ADDRESS = $"{address}";
            if (!Globals.AppSettings.DEVICE_ADDRESS_HISTORY.Contains(address))
            {
                Globals.AppSettings.DEVICE_ADDRESS_HISTORY.Insert(0, address);
                Globals.AppSettings.DEVICE_ADDRESS_HISTORY = Globals.AppSettings.DEVICE_ADDRESS_HISTORY;
                refreshDeviceAddressHistory();
            }
            Timer_Tick(null, null);
        }

        private void pairDevice_Click(object sender, RoutedEventArgs e)
        {
            string address = deviceAddress.Text;
            if (!IsValidIPEndPoint(address))
            {
                MessageBox.Show("设备配对地址输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CmdExecutor.ExecuteCommandByShell($"{adbPath} pair {address}");
        }

        private string SelectApkFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "APK Files (*.apk)|*.apk";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                // 获取选中的文件路径
                return openFileDialog.FileName;
            }
            return null;
        }

        private void installApkItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var textBlock = (TextBlock)contextMenu.PlacementTarget;
            if (MessageBoxResult.OK != MessageBox.Show($"确定要安装应用到【{textBlock.Text}】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                return;
            }
            string path = SelectApkFile();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            CmdExecutor.ExecuteCommandByShell($"{adbPath} -s {textBlock.Text} install \"{path}\"");
        }

        private void disconnectDeviceItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var textBlock = (TextBlock)contextMenu.PlacementTarget;
            if (MessageBoxResult.OK != MessageBox.Show($"确定要断开设备【{textBlock.Text}】的连接吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                return;
            }
            CmdExecutor.ExecuteCommandAndQuit($"{adbPath} disconnect {textBlock.Text}");
            Timer_Tick(null, null);
        }

        private void disconnectAllDeviceItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("确定要断开所有设备吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                CmdExecutor.ExecuteCommandAndQuit($"{adbPath} disconnect");
                Timer_Tick(null, null);
            }
        }

        public static bool IsValidIPEndPoint(string ipPort)
        {
            string[] parts = ipPort.Split(':');
            if (parts.Length > 2)
            {
                return false;
            }
            if (parts.Length == 1)
            {
                if (!IPAddress.TryParse(ipPort, out _))
                {
                    return false;
                }
                return true;
            }

            string ipAddress = parts[0];
            string port = parts[1];

            // 验证IP地址
            if (!IPAddress.TryParse(ipAddress, out _))
            {
                return false;
            }

            // 验证端口号
            if (!int.TryParse(port, out int portNumber))
            {
                return false;
            }

            if (portNumber <= 0 || portNumber > 65535)
            {
                return false;
            }

            return true;
        }

        private void deleteAllHistory_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("确定要清空所有历史纪录吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                Globals.AppSettings.DEVICE_ADDRESS_HISTORY.Clear();
                Globals.AppSettings.DEVICE_ADDRESS_HISTORY = Globals.AppSettings.DEVICE_ADDRESS_HISTORY;
                refreshDeviceAddressHistory();
            }
        }

    }
}
