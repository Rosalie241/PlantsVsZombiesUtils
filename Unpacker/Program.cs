using System;
using System.IO;

namespace Unpacker
{
    class Program
    {
        static void Main(string[] args)
        {
            // packed executable starts at 0x1A71A2
            // packed executable ends at   0x50BB22

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Unpacker.exe [PlantsVsZombies.exe] [output file]");
                Environment.Exit(1);
            }

            BinaryReader reader = new BinaryReader(File.OpenRead(args[0]));
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(args[1]));

            reader.BaseStream.Position = 0x1A71A2;

            byte[] buffer = new byte[1];

            // read until 0x50BB22
            while (reader.Read(buffer, 0, 1) > 0)
            {
                writer.Write(buffer);

                if (reader.BaseStream.Position == 0x50BB21)
                    break;
            }

            // close & flush
            writer.Flush();
            writer.Close();
            reader.Close();
        }
    }
}
