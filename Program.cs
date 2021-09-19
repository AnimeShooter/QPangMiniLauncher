using System;
using System.Collections.Generic;
using System.IO;

namespace QpangPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            // #=================#
            // #  Local Patcher  # 
            // #=================#

            //Dictionary<uint, Dictionary<uint, byte[]>> offsets = new Dictionary<uint, Dictionary<uint, byte[]>> // TODO: identify the binary?
            Dictionary<uint, byte[]> offsets = new Dictionary<uint, byte[]>()
            {
                {0x002D2B0, new byte[] { 0xC6, 0x05, 0x1C, 0x1D, 0x7C, 0x00, 0x01, 0x33, 0xC0, 0xC2, 0x08, 0x00 } }
            };

            if (args.Length == 0)
            {
                Console.WriteLine("No args specified!");
                return;
            }

            if(!File.Exists(args[0]))
            {
                Console.WriteLine("Invalid file selected: " + args[0]);
                return;
            }

            byte[] buffer = File.ReadAllBytes(args[0]);

            // Make a backup if not already
            string backupFilename = args[0].Replace(".exe", "_original.exe");
            if(!File.Exists(backupFilename))
                File.WriteAllBytes(backupFilename, buffer);

            // Patch file
            uint textBase = 0x0000; //0x1000; // .text hardcoded
            foreach(var o in offsets)
            {
                if(buffer.Length < o.Key + textBase)
                {
                    Console.WriteLine("Error, target file to small!");
                    continue;
                }

                for(int i = 0; i < o.Value.Length; i++)
                    buffer[textBase + o.Key + i] = o.Value[i];
            }
            File.WriteAllBytes(args[0], buffer);

            Console.WriteLine("File has been patched!");
        }
    }
}
