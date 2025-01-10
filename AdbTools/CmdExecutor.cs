using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace AdbTools
{
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
