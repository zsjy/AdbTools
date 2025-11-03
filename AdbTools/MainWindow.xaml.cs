using AdbTools.AccessInterface;
using AdbTools.bean;
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
using Update;

namespace AdbTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand CommandF5 = new RoutedCommand();
        public static RoutedCommand CommandF8 = new RoutedCommand();


        public static string adbPath = "";
        //private DispatcherTimer timer;
        private ObservableCollection<string> deviceAddressHistory;
        private ObservableCollection<Device> deviceAddressList;

        public MainWindow()
        {
            InitializeComponent();

            deviceAddressHistory = new ObservableCollection<string>();
            deviceAddressList = new ObservableCollection<Device>();

            // 创建 CommandBinding 并绑定到处理程序
            CommandBinding commandF5Binding = new CommandBinding(
                CommandF5,
                CommandF5_Executed,
                CommandF5_CanExecute);

            CommandBinding commandF8Binding = new CommandBinding(
                CommandF8,
                CommandF8_Executed,
                CommandF8_CanExecute);


            this.CommandBindings.Add(commandF5Binding);
            this.CommandBindings.Add(commandF8Binding);

        }

        private void CommandF5_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Timer_Tick(null, null);
        }

        private void CommandF5_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; // 始终可以执行
        }

        private void CommandF8_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            pairDevice_Click(null, null);
        }

        private void CommandF8_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; // 始终可以执行
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
            this.Title = $"{this.Title} V{App.Version}";
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
            if (!Globals.AppSettings.IS_GITHUB_PROXY)
            {
                Globals.AppSettings.GITHUB_PROXY = Globals.AppSettings.GITHUB_PROXY;
            }

            RequestJson.UpdateCheck(this, updateVersion);

            adbPath = $"\"{path}\"";

            deviceAddress.Text = Globals.AppSettings.LAST_DEVICE_ADDRESS;
            deviceAddress.ItemsSource = deviceAddressHistory;
            refreshDeviceAddressHistory();

            deviceList.ItemsSource = deviceAddressList;

            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(3);
            //timer.Tick += Timer_Tick;
            //timer.Start();
            Timer_Tick(null, null);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            waitAnimation(true);
            string command = $"{adbPath} devices";
            CmdExecutor.ExecuteCommandAndReturnAsync(command, content =>
            {
                // 输出所有匹配项
                int index = 0;
                List<string> list = new List<string>();
                string[] str = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < str.Length; ++i)
                {
                    if (str[i].EndsWith("device"))
                    {
                        string address = str[i].Replace("device", "").Trim();
                        list.Add(address);
                        Device d = null;
                        if (!deviceAddressList.Any(o =>
                        {
                            d = o;
                            return o.DeviceMark.Equals(address);
                        }))
                        {
                            Device device = new Device()
                            {
                                IsWifiConnect = IsValidIPEndPoint(address),
                                DeviceMark = address,
                                ShowDeviceName = address
                            };

                            deviceAddressList.Insert(index, device);
                        }
                        else
                        {
                            deviceAddressList.Remove(d);
                            deviceAddressList.Insert(index, d);
                        }
                        index++;
                    }
                }

                List<Device> removeList = deviceAddressList.Where(o => !list.Contains(o.DeviceMark)).ToList();
                removeList.ForEach(o => deviceAddressList.Remove(o));
                foreach (Device d in deviceAddressList)
                {
                    d.Refresh();
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
            address = address.Replace("：", ":");
            deviceAddress.Text = address;
            if (!IsValidIPEndPoint(address))
            {
                MessageBox.Show("设备连接地址输入错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            waitAnimation(true);
            CmdExecutor.ExecuteCommandAndReturnAsync($"{adbPath} connect {address}", result =>
            {
                if (string.IsNullOrWhiteSpace(result) || ContainsAny(result, new string[] { "(10060)", "(10061)", "failed to connect" }))
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

            }, (err) =>
            {
                waitAnimation(false);
                MessageBox.Show("设备连接失败！\r\n1.请检查IP和端口输入是否正确；\r\n2.设备与电脑是否完成配对。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            });
        }

        private void pairDevice_Click(object sender, RoutedEventArgs e)
        {
            PairWindow pairWindow = new PairWindow()
            {
                AdbPath = adbPath,
                Address = deviceAddress.Text,
            };
            pairWindow.Owner = this;
            pairWindow.ShowDialog();
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
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            if (MessageBoxResult.OK != MessageBox.Show($"确定要安装应用到设备【{device.ShowDeviceName}】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                return;
            }
            string path = SelectApkFile();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            waitAnimation(true);
            CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark} install \"{path}\"" }, result =>
            {
                waitAnimation(false);
                if (!string.IsNullOrWhiteSpace(result) && ContainsAny(result, new string[] { "Success" }))
                {
                    MessageBox.Show($"设备【{device.ShowDeviceName}】安装应用成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            },
            err =>
            {
                waitAnimation(false);
                if (!string.IsNullOrWhiteSpace(err) && ContainsAny(err, new string[] { "adb: failed to install", "device offline" }))
                {
                    MessageBox.Show($"设备【{device.ShowDeviceName}】安装应用失败！\r\n1.请确认安卓设备是否允许安装应用；\r\n2.应用签名是否不符；\r\n3.检查设备是否已离线。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                }
                return false;
            }, 600);
        }

        private void disconnectDeviceItem_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];

            if (MessageBoxResult.OK != MessageBox.Show($"确定要断开设备【{device.ShowDeviceName}】的连接吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                return;
            }
            waitAnimation(true);
            CmdExecutor.ExecuteCommandAndReturnAsync($"{adbPath} disconnect {device.DeviceMark}", result =>
            {
                Timer_Tick(null, null);
                waitAnimation(false);
            });
        }

        private void disconnectAllDeviceItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("确定要【断开所有设备】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
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
            if (ipPort.Contains("._adb-tls-connect._tcp"))
            {
                return true;
            }
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
            if (MessageBoxResult.OK == MessageBox.Show("确定要【清空所有历史纪录】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
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

        private void resetPortItem_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            if (MessageBoxResult.OK == MessageBox.Show($"确定要对设备【{device.ShowDeviceName}】执行【恢复默认TCP端口(5555)】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                waitAnimation(true);
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark} tcpip 5555 " }, result =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】恢复默认TCP端口(5555)成功", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                },
                err =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】恢复默认TCP端口(5555)失败！\r\n检查设备是否已离线。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                });
            }
        }

        private void resetPXItem_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            if (MessageBoxResult.OK == MessageBox.Show($"确定要对设备【{device.ShowDeviceName}】执行【恢复默认分辨率】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                waitAnimation(true);
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark} shell wm size reset " }, result =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】恢复默认分辨率成功", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                },
                err =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】恢复默认分辨率失败！\r\n检查设备是否已离线。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                });
            }
        }

        private void resetDPIItem_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            if (MessageBoxResult.OK == MessageBox.Show($"确定要对设备【{device.ShowDeviceName}】执行【恢复默认DPI】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                waitAnimation(true);
                CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark} shell wm density reset " }, result =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】恢复默认DPI成功", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                },
                err =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】恢复默认DPI失败！\r\n检查设备是否已离线。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                });
            }
        }

        private void logcatItem_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            CmdExecutor.ExecuteCommandByShell($"{adbPath} -s {device.DeviceMark} logcat");
        }

        private void logcatDebugItem_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            CmdExecutor.ExecuteCommandByShell($"{adbPath} -s {device.DeviceMark} logcat -s DEBUG");
        }



        #region 《版本更新》

        private void updateVersion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                GithubReleases githubReleases = (GithubReleases)updateVersion.Tag;
                UpdateVersion(githubReleases);
            }
        }

        private void UpdateVersion(GithubReleases githubReleases)
        {
            if (null == githubReleases) return;
            GithubReleasesAssets githubReleasesAssets = null;
            foreach (GithubReleasesAssets assets in githubReleases.Assets)
            {
                if (RequestJson.UpdateFileName.Equals(assets.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    githubReleasesAssets = assets;
                }
            }
            if (null == githubReleasesAssets)
            {
                return;
            }

            updateVersion.Visibility = Visibility.Visible;
            updateVersion.Tag = githubReleases;

            if (MessageBoxResult.OK != MessageBox.Show($"发现新版本【V{githubReleases.TagName}】是否更新？", "新版本", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                return;
            }

            List<string> cmdArges = new List<string>();
            //cmdArges.Add($"{Globals.AppSettings.GITHUB_PROXY}{githubReleasesAssets.browser_download_url}");
            cmdArges.Add($"{githubReleasesAssets.browser_download_url}");
            cmdArges.Add($"{AppDomain.CurrentDomain.BaseDirectory}");
            cmdArges.Add($"{AppDomain.CurrentDomain.BaseDirectory}{AppDomain.CurrentDomain.FriendlyName}");

            if (CmdExecutor.StartExe($"{AppDomain.CurrentDomain.BaseDirectory}Update.exe", cmdArges))
            {
                Application.Current.Shutdown();
            }
            else
            {

            }
        }

        #endregion

        #region 《关于菜单及控制》

        private void about_Click(object sender, RoutedEventArgs e)
        {
            about_popup.IsOpen = true;
        }

        private void projectAddress_Click(object sender, RoutedEventArgs e)
        {
            about_popup.IsOpen = false;
            CmdExecutor.StartExe("https://github.com/zsjy/AdbTools");
        }


        private void projectAddressMirror_Click(object sender, RoutedEventArgs e)
        {
            about_popup.IsOpen = false;
            CmdExecutor.StartExe("https://gitee.com/hcjike/AdbTools");
        }

        private void cheackUpdate_Click(object sender, RoutedEventArgs e)
        {
            about_popup.IsOpen = false;
            RequestJson.UpdateCheck(this, githubReleases =>
            {
                UpdateVersion(githubReleases);
            });
        }

        #endregion

        #region 《设置菜单及控制》

        private void setup_Click(object sender, RoutedEventArgs e)
        {
            setup_popup.IsOpen = true;
        }

        private void topmastCB_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = (bool)topmastCB.IsChecked;
            setup_popup.IsOpen = false;
        }
        #endregion



        private void customkEnable_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            InputWindow inputWindow = new InputWindow()
            {
                Title = "请输入包名"
            };
            inputWindow.Owner = this;
            if (inputWindow.ShowDialog() == true)
            {
                string package = inputWindow.Result;
                if (MessageBoxResult.OK == MessageBox.Show($"确定要对设备【{device.ShowDeviceName}】执行【启用包名{package}】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
                {
                    waitAnimation(true);
                    CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark} shell pm enable {package} " }, result =>
                    {
                        waitAnimation(false);
                        MessageBox.Show($"设备【{device.ShowDeviceName}】启用包名{package}成功", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    },
                    err =>
                    {
                        waitAnimation(false);
                        MessageBox.Show($"设备【{device.ShowDeviceName}】启用包名{package}失败！\r\n1.检查设备是否已离线；\r\n2.应用包名未找到！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return true;
                    });
                }
            }
        }

        private void customDisable_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            InputWindow inputWindow = new InputWindow()
            {
                Title = "请输入包名"
            };
            inputWindow.Owner = this;
            if (inputWindow.ShowDialog() == true)
            {
                string package = inputWindow.Result;
                if (MessageBoxResult.OK == MessageBox.Show($"确定要对设备【{device.ShowDeviceName}】执行【禁用包名{package}】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question))
                {
                    waitAnimation(true);
                    CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark} shell pm disable-user {package} " }, result =>
                    {
                        waitAnimation(false);
                        MessageBox.Show($"设备【{device.ShowDeviceName}】禁用包名{package}成功", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    },
                    err =>
                    {
                        waitAnimation(false);
                        MessageBox.Show($"设备【{device.ShowDeviceName}】禁用包名{package}失败！\r\n1.检查设备是否已离线；\r\n2.应用包名未找到！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return true;
                    });
                }
            }
        }

        private void systemAppPackagep_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark}  shell pm list packages  -s " }, result =>
            {
                // 1. 按行分割
                string[] lines = result.Replace("package:", "").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                // 2. 找到 "shell pm list packages" 所在行
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("shell pm list packages"))
                    {
                        startIndex = i + 1; // 下一行开始就是包名
                        break;
                    }
                }
                // 3. 提取包名（遇到空行就结束）
                List<string> packages = new List<string>();
                for (int i = startIndex; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    packages.Add(line);
                }
                waitAnimation(false);
                InfoShowWindow infoShowWindow = new InfoShowWindow() { Title = "系统安装应用包名", DescriptionContent = string.Join("\r\n", packages) };
                infoShowWindow.Show();
            },
                err =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】获取系统安装应用包名失败！\r\n检查设备是否已离线。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                });
        }

        private void thirdPartyAppPackage_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark}  shell pm list packages  -3 " }, result =>
            {
                // 1. 按行分割
                string[] lines = result.Replace("package:", "").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                // 2. 找到 "shell pm list packages" 所在行
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("shell pm list packages"))
                    {
                        startIndex = i + 1; // 下一行开始就是包名
                        break;
                    }
                }
                // 3. 提取包名（遇到空行就结束）
                List<string> packages = new List<string>();
                for (int i = startIndex; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    packages.Add(line);
                }
                waitAnimation(false);
                InfoShowWindow infoShowWindow = new InfoShowWindow() { Title = "第三方安装应用包名", DescriptionContent = string.Join("\r\n", packages) };
                infoShowWindow.Show();
            },
                err =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】获取第三方安装应用包名失败！\r\n检查设备是否已离线。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                });
        }

        private void allAppPackage_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{adbPath} -s {device.DeviceMark}  shell pm list packages" }, result =>
            {
                // 1. 按行分割
                string[] lines = result.Replace("package:", "").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                // 2. 找到 "shell pm list packages" 所在行
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("shell pm list packages"))
                    {
                        startIndex = i + 1; // 下一行开始就是包名
                        break;
                    }
                }
                // 3. 提取包名（遇到空行就结束）
                List<string> packages = new List<string>();
                for (int i = startIndex; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    packages.Add(line);
                }
                waitAnimation(false);
                InfoShowWindow infoShowWindow = new InfoShowWindow() { Title = "所有安装应用包名", DescriptionContent = string.Join("\r\n", packages) };
                infoShowWindow.Show();
            },
                err =>
                {
                    waitAnimation(false);
                    MessageBox.Show($"设备【{device.ShowDeviceName}】获取所有安装应用包名失败！\r\n检查设备是否已离线。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                });
        }

        private void disableAllAppPackage_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            if (!"alps k71v1_64_bsp".Equals(device.DeviceInfo, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show($"仅支持操作设备【alps k71v1_64_bsp】", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AlpsK71v1Window alpsK71V1Window = new AlpsK71v1Window()
            {
                Title = $"一键处理【{device.ShowDeviceName}】多余应用",
                AdbPath = adbPath,
                DeviceInfo = device,
            };
            alpsK71V1Window.Owner = this;
            alpsK71V1Window.ShowDialog();

        }

        private void openShell_Click(object sender, RoutedEventArgs e)
        {
            int index = deviceList.SelectedIndex;
            Device device = deviceAddressList[index];
            CmdExecutor.ExecuteCommandByShell($"{adbPath} -s {device.DeviceMark} shell ");
        }
    }
}
