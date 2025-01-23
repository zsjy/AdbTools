using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace AdbTools.bean
{
    public class Device : INotifyPropertyChanged
    {

        public string DeviceMark
        {
            get;
            set;
        }

        public string DeviceBrand
        {
            get;
            set;
        }

        public string DeviceModel
        {
            get;
            set;
        }

        public bool IsWifiConnect
        {
            get;
            set;
        }

        public string ShowDeviceName
        {
            get;
            set;
        }

        public string ConnectWay
        {
            get;
            set;
        }

        public Brush ConnectWayColor
        {
            get;
            set;
        }

        public string DeviceInfo
        {
            get;
            set;
        }


        public void Refresh()
        {
            UpdateShow();
            new Thread(() =>
            {
                if (string.IsNullOrWhiteSpace(DeviceBrand))
                {
                    CmdExecutor.ExecuteCommandAndReturnAsync($"{MainWindow.adbPath} -s {DeviceMark} shell getprop ro.product.brand ", result =>
                    {
                        string[] str = result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        string s = "";
                        for (int i = 0; i < str.Length; ++i)
                        {
                            if (str[i].Contains("shell getprop ro.product.brand") && i < str.Length)
                            {
                                s = str[i + 1];
                            }
                        }

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            DeviceBrand = s;
                            UpdateShow();
                        }));
                    });
                }

                if (string.IsNullOrWhiteSpace(DeviceModel))
                {
                    CmdExecutor.ExecuteCommandAndReturnAsync($"{MainWindow.adbPath} -s {DeviceMark} shell getprop ro.product.model ", result =>
                    {
                        string[] str = result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        string s = "";
                        for (int i = 0; i < str.Length; ++i)
                        {
                            if (str[i].Contains("shell getprop ro.product.model") && i < str.Length)
                            {
                                s = str[i + 1];
                            }
                        }

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            DeviceModel = s;
                            UpdateShow();
                        }));
                    });
                }
            }).Start();
        }

        private void UpdateShow()
        {
            string s = "";
            s += DeviceBrand;
            s += (string.IsNullOrWhiteSpace(s) ? "" : " ") + DeviceModel;
            DeviceInfo = s;
            ConnectWay = IsWifiConnect ? "WiFi" : "USB";
            ConnectWayColor = IsWifiConnect ? Brushes.Green : Brushes.Orange;

            s += $" {ConnectWay}";
            s += string.IsNullOrWhiteSpace(s) ? $"{DeviceMark}" : $" ({DeviceMark})";

            ShowDeviceName = s;
            OnPropertyChanged(string.Empty);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
