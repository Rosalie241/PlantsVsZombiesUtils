using System;
using System.IO;

namespace Packer
{
    class Program
    {
        static void Main(string[] args)
        {
            // packed executable starts at 0x1A71A2
            // packed executable ends at   0x50BB21

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Packer.exe [PlantsVsZombies.exe] [executable to inject]");
                Environment.Exit(1);
            }

            string gameExecutable = args[0];
            string injectExecutable = args[1];

            if (new FileInfo(injectExecutable).Length > (0x50BB21 - 0x1A71A2))
            {
                Console.WriteLine("executable too big to inject!");
                Environment.Exit(1);
            }

            BinaryWriter gameExecutableStream = new BinaryWriter(File.OpenWrite(gameExecutable));
            byte[] injectExecutableBytes = File.ReadAllBytes(injectExecutable);

            // set starting position
            gameExecutableStream.BaseStream.Position = 0x1A71A2;

            // write executable into stream
            gameExecutableStream.Write(injectExecutableBytes);

            // fill with 0 until we've reached the end
            while (gameExecutableStream.BaseStream.Position <= 0x50BB21)
                gameExecutableStream.Write(new byte[] { 0 });

            // flush & close stream
            gameExecutableStream.Flush();
            gameExecutableStream.Close();
        }
    }
}
