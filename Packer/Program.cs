using System;
using System.IO;
using System.Text;

namespace Packer
{
    class Program
    {
        /// <summary>
        /// Finds byte sequence and returns offset of beginning of byte sequence, returns -1 if not found
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static long findByteSequence(FileStream stream, byte[] bytes)
        {
            byte[] buffer = new byte[1024];
            byte[] tmpBuffer = new byte[bytes.Length];
            int tmpBufferIndex = 0;
            int readCount;

            while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < readCount; i++)
                {
                    if (buffer[i] == bytes[tmpBufferIndex])
                    {
                        tmpBufferIndex++;
                        tmpBuffer[tmpBufferIndex] = buffer[i];

                        // found the bytes, return offset
                        if (tmpBufferIndex == (bytes.Length - 1))
                            return (stream.Position - buffer.Length - bytes.Length + i + 2);
                    }
                    else
                    {
                        tmpBufferIndex = 0;
                    }
                }
            }

            return -1;
        }

        static void Main(string[] args)
        {
            // packed executable starts at 0x1A71A2
            // packed executable ends at   0x50BB21

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Packer.exe [PlantsVsZombies.exe] [executable to inject]");
                Environment.Exit(1);
            }

            FileStream writer = File.Open(args[0], FileMode.Open);
            FileStream reader = File.OpenRead(args[1]);


            // find required byte sequences
            byte[] drmStartBytes = Encoding.ASCII.GetBytes("!popcapdrmprotect!");
            byte[] drmEndBytes = Encoding.ASCII.GetBytes("!popcapdrmprotend!");

            long drmStartOffset = findByteSequence(writer, drmStartBytes);
            long drmEndOffset = findByteSequence(writer, drmEndBytes);

            if (drmStartOffset == -1 || drmEndOffset == -1)
                throw new Exception("Required byte sequence not found!");

            writer.Position = drmStartOffset + drmStartBytes.Length;

            // the game needs 400 zero bytes after the drmStartBytes offset for some reason,
            // so let's respect that
            writer.Position += 400;

            if (reader.Length > (drmEndOffset - writer.Position))
                throw new Exception("Executable too big to inject!");

            // write executable into the game executable
            byte[] buffer = new byte[1024];
            int bufferCount;
            while ((bufferCount = reader.Read(buffer, 0, buffer.Length)) > 0)
                writer.Write(buffer, 0, bufferCount);

            // fill rest with 0 bytes until we've reached the end
            buffer = new byte[] { 0 };
            while (writer.Position < drmEndOffset)
                writer.Write(buffer, 0, buffer.Length);

            // flush & close
            writer.Flush();
            writer.Close();
            reader.Close();
        }
    }
}
