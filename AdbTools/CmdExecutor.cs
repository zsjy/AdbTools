using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdbTools
{
    public class CmdExecutor
    {
        public static string ExecuteCommandAndReturn(string command)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");

                process.WaitForExit(2000);

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
                process.WaitForExit();

                //process.StandardInput.WriteLine(command);
            }
        }

        public static void ExecuteCommandAndQuit(string command)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");

                process.WaitForExit(2000);
            }
        }
    }
}
