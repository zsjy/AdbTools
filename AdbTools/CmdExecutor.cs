using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace AdbTools
{
    public delegate void ExecuteResult(string result);
    public delegate bool ExecuteErrResult(string err);

    public class CmdExecutor
    {

        public static void ExecuteCommandAndReturnAsync(string command)
        {
            ExecuteCommandAndReturnAsync(new string[] { command }, null, null);
        }

        public static void ExecuteCommandAndReturnAsync(string command, ExecuteResult executeResult)
        {
            ExecuteCommandAndReturnAsync(new string[] { command }, executeResult, null);
        }

        public static void ExecuteCommandAndReturnAsync(string command, ExecuteResult executeResult, ExecuteErrResult errorResult)
        {
            ExecuteCommandAndReturnAsync(new string[] { command }, executeResult, errorResult);
        }

        public static void ExecuteCommandAndReturnAsync(string[] command, ExecuteResult executeResult, ExecuteErrResult errorResult, int timeout = 5)
        {
            new Thread(() =>
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    foreach (string com in command)
                    {
                        process.StandardInput.WriteLine(com);
                        Thread.Sleep(50);
                    }

                    process.StandardInput.WriteLine("exit");
                    process.StandardInput.Close();  // 关闭输入流

                    string ret = process.StandardOutput.ReadToEnd();
                    string err = process.StandardError.ReadToEnd();
                    Console.WriteLine($"result : {ret}");
                    Console.WriteLine($"err : {err}");

                    if (!process.WaitForExit(timeout * 1000))  // 等待5秒
                    {
                        process.Kill();  // 强制终止进程
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        bool dispose = false;
                        if (!string.IsNullOrWhiteSpace(err))
                        {
                            if (null != errorResult)
                            {
                                dispose = errorResult.Invoke(err);
                            }
                        }
                        if (!dispose)
                        {
                            executeResult?.Invoke(ret);
                        }
                    }));
                }
            }).Start();

        }

        public static void ExecuteCommandByShell(string command)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.Arguments = $"/c \"{command}\"";
                process.Start();

            }
        }

        public static void StartExe(string exePath, List<string> cmdArges)
        {
            string args = "";
            foreach (string a in cmdArges)
            {
                args += a + " ";
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动程序时出错: " + ex.Message);
            }
        }

    }
}
