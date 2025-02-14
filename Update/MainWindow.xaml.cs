using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Update.AccessInterface;

namespace Update
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread thread;
        private string updateFilePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            waitProcessBar.IsShowAnimation = true;
            updateFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}update.zip";
            updateTip("正在下载...");
            thread = new Thread(() =>
            {
                try
                {
                    if (RequestJson.DownloadFile(App.downloadUrl, updateFilePath))
                    {
                        updateTip("正在解压");
                        Thread.Sleep(1000);
                        ZipFileUtils.UnzipFile(updateFilePath, App.unZipPath);
                        updateTip("更新完成");

                        if (!string.IsNullOrWhiteSpace(App.startExePath))
                        {
                            updateTip("启动程序");
                            Thread.Sleep(1000);
                            StartExe(App.startExePath, "");
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            Quit();
                        }
                    }
                    else
                    {
                        updateTip("下载失败");
                    }
                }
                catch (Exception ex)
                {
                    updateTip("更新失败，请使用全量更新");
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        waitProcessBar.IsShowAnimation = false;
                    }));
                }
            });
            thread.Start();
        }

        private void updateTip(string tip)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                tipLabel.Content = tip;
            }));
        }

        private void StartExe(string exePath, string args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                // 创建ProcessStartInfo对象
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    UseShellExecute = true // 使用操作系统shell启动
                };

                try
                {
                    // 启动外部程序
                    Process.Start(startInfo);

                    ///退出当前新开进程，不走OnExit方法
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }));
        }

        private void Quit()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                ///退出当前新开进程，不走OnExit方法
                Environment.Exit(0);
            }));
        }

    }
}
