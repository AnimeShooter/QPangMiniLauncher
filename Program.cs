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
            Dictionary<uint, byte[]> offsets = new Dictionary<uint, byte[]>()
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


            string filePath = Directory.GetCurrentDirectory() + "\\QPangBin.exe";
            if (args == null || (args.Length == 0 && !File.Exists(filePath))) // carry the noobs that dont know CLI ;)
            {
                Console.WriteLine("No args specified!");
                return;
            }
            else
                filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Invalid file selected: " + filePath);
                return;
            }

            byte[] buffer = File.ReadAllBytes(filePath);

            // Make a backup if not already
            string backupFilename = filePath.Replace(".exe", "_original.exe");
            if (!File.Exists(backupFilename))
                File.WriteAllBytes(backupFilename, buffer);

            // Patch file
            uint textBase = 0x0000; //0x1000; // .text hardcoded
            foreach (var o in offsets)
            {
                if (buffer.Length < o.Key + textBase + o.Value.Length)
                {
                    Console.WriteLine("Error, target file to small!");
                    continue;
                }

                for (int i = 0; i < o.Value.Length; i++)
                    buffer[textBase + o.Key + i] = o.Value[i];
            }
            File.WriteAllBytes(filePath, buffer);

            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("File has been patched!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine("Don't forget to update QpangID.exe and to run QPangBin.exe using \"-fullscreen:0 -width:800 -height:600 -forcevsync:0 -locale:English\"");

            // Create shortcut if needed
            string shortcuteFilename = filePath.Replace(".exe", "_Launcher.bat");
            if (!File.Exists(shortcuteFilename))
            {
                string qargs = "-fullscreen:0 -width:800 -height:600 -forcevsync:0 -locale:English";
                File.WriteAllText(shortcuteFilename, "\"" + filePath + "\"" + " " + qargs);
                Console.WriteLine("Please use the _Launcher.Bat file that has been created to open QPang!");
            }

            Console.ForegroundColor = old;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}