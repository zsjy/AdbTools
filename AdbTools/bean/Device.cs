using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

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

        private string _ShowDeviceName;
        public string ShowDeviceName
        {
            get => _ShowDeviceName;
            set
            {
                _ShowDeviceName = value;
                OnPropertyChanged(string.Empty);
            }
        }

        public void Refresh()
        {
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
            s += string.IsNullOrWhiteSpace(s) ? $"{DeviceMark}" : $" ({DeviceMark})";

            ShowDeviceName = s;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
