using AdbTools.bean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
    /// PairWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PairWindow : Window
    {
        public string AdbPath { get; set; }

        public string Address { get; set; }

        public PairWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Address))
            {
                this.deviceAddress.Text = Address;
            }
        }

        private void pairDevice_Click(object sender, RoutedEventArgs e)
        {
            string address = deviceAddress.Text;
            address = address.Replace("：", ":");
            deviceAddress.Text = address;
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
            CmdExecutor.ExecuteCommandAndReturnAsync(new string[] { $"{AdbPath} pair {address}", code }, result =>
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

                    this.Close();
                }
            }, null);
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

    }
}
