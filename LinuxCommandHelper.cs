using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;

namespace DotNetTool
{
    public class LinuxCommandHelper
    {
        public static void RunCommandWithoutRedirects(string command, string args)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                }
            };
            process.Start();
            //string output = process.StandardOutput.ReadToEnd();
            //string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
        }

        public static void RunScriptInNewTerminal(string script)
        {
            var cmd = "gnome-terminal";
            var param = $"-x bash -c \"{script}; exec bash; \"";
            RunCommandWithoutRedirects(cmd, param);
        }


        public static string ReadFromFIFO(string fifo)
        {
            var pipe = new NamedPipeClientStream(".", fifo, PipeDirection.InOut, PipeOptions.None);

            pipe.Connect();

            var bytes = new List<byte>();
            while (true)
            {
                var b = pipe.ReadByte();
                if (b == -1 && bytes.Count > 0)
                {
                    break;
                }
                bytes.Add(Convert.ToByte(b));
            }

            return TextEncodeHelper.ByteArray2String(bytes.ToArray());
        }

        public static int RunCommand(string command, string args)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return process.ExitCode;
        }
    }
}
