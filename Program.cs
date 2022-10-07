using System;
using System.Collections.Generic;
using System.IO;

namespace QpangLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            // #=================#
            // #  Local Patcher  # 
            // #=================#

            // NOTE: you mmay want to comment out as you go!
            Dictionary<uint, byte[]> patches_qpangbin = new Dictionary<uint, byte[]>()
            {
                //{0x0242C80, new byte[] { 0x31, 0xC0, 0x90, 0x90 } }, // fix headshot
                //{0x0242C8E, new byte[] { 0x30, 0xC9, 0x90, 0x90, 0x90, 0x90 } }, // fix headshot 2
                //{0x01D003B, new byte[] { 0x90, 0x90 } }, // No Flashbang
                //{0x00E0181, new byte[] { 0xE9, 0x41, 0x03, 0x00, 0x00, 0x90 } }, // Godmode (client-side)
                {0x0001146, new byte[] { 0x6A, 0x00, 0x6A, 0x00, 0x68, 0x00, 0x00, 0x08, 0x10 } }, // Window mode
                {0x000143F, new byte[] { 0xEB } }, // Multi client bypass
                {0x00D6A53, new byte[] { 0x83, 0xBF, 0xF4, 0x06, 0x00, 0x00, 0x00, 0x83, 0xC1, 0x42, 0x0F, 0x85 } }, // Fix nameplates
                {0x01AC68E, new byte[] { 0x90, 0xC7, 0x46, 0x50, 0x00, 0x00, 0x80, 0x40, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90} }, // No stupid splash-screen
                {0x01AC6BC, new byte[] { 0x90, 0x90, 0x90} }, // Fix 3sec cursor
                {0x02350E0, new byte[] { 0xC3} }, // Block emailing error
                {0x002D2B0, new byte[] { 0xC6, 0x05, 0x1C, 0x1D, 0x7C, 0x00, 0x01, 0x33, 0xC0, 0xC2, 0x08, 0x00 } } // Patch Anti-Hackshield
            };

            Dictionary<uint, byte[]> patches_on3d = new Dictionary<uint, byte[]>()
            {
                // Increases the particle limit by allocating more buffer (0x4268 increased to 0x7FFFF) space and increasing the threshold checks.
                { 0x0089DAD, new byte[] { 0xFF, 0xFF, 0x07} },
                { 0x00A0271, new byte[] { 0xFF, 0xFF, 0x07} }, 
                { 0x00A042C, new byte[] { 0xFF, 0xFF, 0x07} },
                { 0x00A0575, new byte[] { 0xFF, 0xFF, 0x07} },
            };



            Dictionary<string, Dictionary<uint, byte[]>> patches = new Dictionary<string, Dictionary<uint, byte[]>>()
            {
                { "QPangBin.exe", patches_qpangbin },
                { "On3D.dll", patches_on3d },
            };

            // NOTE: index 0 is expected to be QPangBin
            string[] files = new string[2];
            files[0] = "QPangBin.exe";
            files[1] = "On3D.dll";

            string[] filePaths = new string[2];
            for (int i = 0; i < files.Length; i++)
            {
                filePaths[i] = Directory.GetCurrentDirectory() + "\\" + files[i];
            }

            if (args == null || (args.Length == 0 && !File.Exists(filePaths[0]))) // carry the noobs that dont know CLI ;)
            {
                Console.WriteLine("No args specified!");
                return;
            }
            else
            {
                // use argument path if any
                string newpath = args[0];
                string[] split = args[0].Split('\\');
                files = new string[1] { split[split.Length-1] };
                filePaths = new string[1] { newpath };
            }
                

            if (!File.Exists(filePaths[0]))
            {
                Console.WriteLine("Invalid file selected: " + filePaths[0]);
                return;
            }

            var old = Console.ForegroundColor;
            for (int i = 0; i < filePaths.Length; i++)
            {
                if(!patches.ContainsKey(files[i]))
                {
                    Console.WriteLine("No patches found for " + files[i]);
                }
                PatchFile(filePaths[i], patches[files[i]]);
            }

            // Create shortcut if target us QpangBin.exe and no existing shortcut yet
            if(filePaths[0].Contains(files[0]))
            {
                string shortcuteFilename = filePaths[0].Replace(".exe", "_Launcher.bat");
                if (!File.Exists(shortcuteFilename))
                {
                    string qargs = "-fullscreen:0 -width:800 -height:600 -forcevsync:0 -locale:English";
                    File.WriteAllText(shortcuteFilename, "\"" + filePaths[0] + "\"" + " " + qargs);
                    Console.WriteLine("Please use the _Launcher.Bat file that has been created to open QPang!");
                }
            }

            Console.ForegroundColor = old;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void PatchFile(string filePath, Dictionary<uint, byte[]> patches)
        {
            byte[] buffer = File.ReadAllBytes(filePath);

            // Make a backup if not already
            //string backupFilename = filePath.Replace(".exe", "_original.exe");
            string backupFilename = filePath + ".bak";

            if (!File.Exists(backupFilename))
                File.WriteAllBytes(backupFilename, buffer);

            // Patch file
            uint textBase = 0x0000; //0x1000; // .text hardcoded
            foreach (var o in patches)
            {
                if (buffer.Length < o.Key + textBase + o.Value.Length)
                {
                    Console.WriteLine("Error, target file to small!");
                    continue;
                }

                for (int j = 0; j < o.Value.Length; j++)
                    buffer[textBase + o.Key + j] = o.Value[j];

                Console.WriteLine($"Patched 0x{(o.Key + textBase).ToString("X8")}");
            }
            File.WriteAllBytes(filePath, buffer);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Finished patching: \"{filePath}\"");
            Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine("Don't forget to update QpangID.exe and to run QPangBin.exe using \"-fullscreen:0 -width:800 -height:600 -forcevsync:0 -locale:English\"");

        }
    }
}