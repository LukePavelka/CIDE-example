using System;
using CideClient.Structures;
using CideClient.Api;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CideClient.Class
{
    public class AutoPilot
    {
        public static void SetJump(string systemName)
        {
            string path = @"pycode\jump.py";
            run_python(path, systemName);
        }
        public static void TransferFuel()
        {
            string path = @"pycode\CargoTransfer.py";
            run_python(path,"northing");
        }
        public static void OpenFcManagment()
        {
            string path = @"pycode\OpenFcManagment.py";
            run_python(path,"northing");
        }
        private static void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo("cmd.exe");
            start.FileName = "python.exe";
            start.Arguments = args;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

        private static void run_python(string cmd, string args)
        {
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo("cmd.exe"); 
            string progToRun = cmd;
            Process proc = new Process();
            proc.StartInfo.FileName = "python.exe";
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.Arguments = string.Concat(progToRun, " ", args);
            proc.StartInfo.Arguments = $"{progToRun} \"{args}\"";
            proc.Start();
            proc.WaitForExit();
        }

        internal static void SetJumpFromManegment(string nextSys)
        {
            string path = @"pycode\jumpFromManagment.py";
            run_python(path, nextSys);
        }

        internal static void ExitFcManagment()
        {
            string path = @"pycode\exitFcManagment.py";
            run_python(path, "northing");
        }

        internal static void AddFuelToCarrier()
        {
            string path = @"pycode\AddFuelToFC.py";
            run_python(path,"northing");
        }

        internal static void BuyFromMarketAndAdd()
        {
            string path = @"pycode\BuyFromMarketAndAdd.py";
            run_python(path,"northing");
        }
    }
}