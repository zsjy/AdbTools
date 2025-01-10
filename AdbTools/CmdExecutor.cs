using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace AdbTools
{
    public delegate void ExecuteResult(string result);

    public class CmdExecutor
    {
        public static string ExecuteCommandAndReturn(string command)
        {
            return ExecuteCommandAndReturn(new string[] { command });
        }

        public static string ExecuteCommandAndReturn(string[] command)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                foreach (string com in command)
                {
                    process.StandardInput.WriteLine(com);
                    Thread.Sleep(150);
                }

                process.StandardInput.WriteLine("exit");

                return process.StandardOutput.ReadToEnd();
            }
        }

        public static void ExecuteCommandAndReturnAsync(string command)
        {
            ExecuteCommandAndReturnAsync(new string[] { command }, null, null);
        }

        public static void ExecuteCommandAndReturnAsync(string command, ExecuteResult executeResult)
        {
            ExecuteCommandAndReturnAsync(new string[] { command }, executeResult, null);
        }

        public static void ExecuteCommandAndReturnAsync(string[] command, ExecuteResult executeResult, ExecuteResult errorResult)
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
                        Thread.Sleep(150);
                    }

                    process.StandardInput.WriteLine("exit");

                    string ret = process.StandardOutput.ReadToEnd();
                    string err = process.StandardError.ReadToEnd();

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (!string.IsNullOrWhiteSpace(err))
                        {
                            errorResult?.Invoke(err);
                        }
                        executeResult?.Invoke(ret);
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

    }
}
