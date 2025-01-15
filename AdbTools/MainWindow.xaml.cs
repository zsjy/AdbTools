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


            Timer_Tick(null, null);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            waitAnimation(true);
            deviceAddressList.Clear();
            CmdExecutor.ExecuteCommandAndReturnAsync($"{adbPath} devices", content =>
            {
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

                waitAnimation(false);
            });
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void connectDevice_Click(object sender, RoutedEventArgs e)
        {
            string address = deviceAddress.Text;
            if (!IsValidIPEndPoint(address))
            {
                MessageBox.Show("设备连接地址输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            waitAnimation(true);
            CmdExecutor.ExecuteCommandAndReturnAsync($"{adbPath} connect {address}", result =>
            {
                _ = Application.Current.Dispatcher.Invoke(new Action(() =>
                  {
                      if (string.IsNullOrWhiteSpace(result) || ContainsAny(result, new string[] { "(10060)", "(10061)" , "failed to connect" }))
                      {
                          waitAnimation(false);
                          MessageBox.Show("设备连接失败！\r\n1.请检查IP和端口输入是否正确；\r\n2.设备与电脑是否完成配对。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                          return;
                      }
                      Globals.AppSettings.LAST_DEVICE_ADDRESS = $"{address}";

                      string ip = address.Split(':')[0];
                      List<string> list = Globals.AppSettings.DEVICE_ADDRESS_HISTORY.Where(item => !item.StartsWith(ip)).ToList();
                      list.Insert(0, address);
                      Globals.AppSettings.DEVICE_ADDRESS_HISTORY = list;
                      refreshDeviceAddressHistory();
                      deviceAddress.Text = Globals.AppSettings.LAST_DEVICE_ADDRESS;

                      Timer_Tick(null, null);
                      waitAnimation(false);
                  }));
            });
        }

        private void pairDevice_Click(object sender, RoutedEventArgs e)
        {
            string address = deviceAddress.Text;
            string code = pairCode.Text;
            if (!IsValidIPEndPoint(address))
            {
                MessageBox.Show("设备配对地址输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("请输入配对码！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            waitAnimation(true);
            CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} pair {address}", code }, result =>
            {
                waitAnimation(false);
                if (string.IsNullOrWhiteSpace(result) || !ContainsAny(result, new string[] { "Successfully" }))
                {
                    MessageBox.Show("设备配对失败！\r\n1.确认IP地址和端口是否处于配对状态；\r\n2.配对码输入是否正确。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (ContainsAny(result, new string[] { "Successfully" }))
                {
                    MessageBox.Show("设备配对成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }, null);
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
            waitAnimation(true);
            CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {textBlock.Text} install \"{path}\"" }, result =>
            {
                waitAnimation(false);
                if (!string.IsNullOrWhiteSpace(result) && ContainsAny(result, new string[] { "Success" }))
                {
                    MessageBox.Show("安装应用成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            },
            err =>
            {
                waitAnimation(false);
                if (!string.IsNullOrWhiteSpace(err) && ContainsAny(err, new string[] { "adb: failed to install" }))
                {
                    MessageBox.Show("安装应用失败！\r\n1.请确认安卓设备是否允许安装应用；\r\n2.应用签名是否不符。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
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
            waitAnimation(true);
            CmdExecutor.ExecuteCommandAndReturnAsync($"{adbPath} disconnect {textBlock.Text}", result =>
            {
                Timer_Tick(null, null);
                waitAnimation(false);
            });
        }

        private void disconnectAllDeviceItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("确定要断开所有设备吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                waitAnimation(true);
                CmdExecutor.ExecuteCommandAndReturnAsync($"{adbPath} disconnect", result =>
                {
                    Timer_Tick(null, null);
                    waitAnimation(false);
                });
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

        private void deleteHistory_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var textBlock = (TextBlock)contextMenu.PlacementTarget;
            if (MessageBoxResult.OK != MessageBox.Show($"确定要删除记录【{textBlock.Text}】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                return;
            }
            Globals.AppSettings.DEVICE_ADDRESS_HISTORY.Remove(textBlock.Text);
            Globals.AppSettings.DEVICE_ADDRESS_HISTORY = Globals.AppSettings.DEVICE_ADDRESS_HISTORY;
            refreshDeviceAddressHistory();
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

        public bool ContainsAny(string source, params string[] substrings)
        {
            return substrings.Any(substring => source.Contains(substring));
        }

        private void waitAnimation(bool show)
        {
            if (show)
            {
                processBarGrid.Visibility = Visibility.Visible;
                waitProcessBar.IsShowAnimation = show;
            }
            else
            {
                processBarGrid.Visibility = Visibility.Collapsed;
                waitProcessBar.IsShowAnimation = show;
            }
        }

        private void refreshList_Click(object sender, RoutedEventArgs e)
        {
            Timer_Tick(null, null);
        }

    }
}
