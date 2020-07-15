using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unpacker
{
    class Program
    {
        /// <summary>
        /// Finds byte sequence and returns offset of beginning of byte sequence, returns -1 if not found
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static long findByteSequence(BinaryReader stream, byte[] bytes)
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
                            return (stream.BaseStream.Position - buffer.Length - bytes.Length + i + 2);
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
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Unpacker.exe [PlantsVsZombies.exe] [output file]");
                Environment.Exit(1);
            }

            BinaryReader reader = new BinaryReader(File.OpenRead(args[0]));
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(args[1]));

            // find required byte sequences
            byte[] drmStartBytes = Encoding.ASCII.GetBytes("!popcapdrmprotect!");
            byte[] drmEndBytes = Encoding.ASCII.GetBytes("!popcapdrmprotend!");

            long drmStartOffset = findByteSequence(reader, drmStartBytes);
            long drmEndOffset = findByteSequence(reader, drmEndBytes);

            if (drmStartOffset == -1 || drmEndOffset == -1)
                throw new Exception("Required byte sequence not found!");

            reader.BaseStream.Position = drmStartOffset + drmStartBytes.Length;

            // skip first few useless 0 bytes (why do they exist at all?)
            // until we get something that isn't a 0 byte
            while (reader.ReadByte() == 0)
                continue;
            reader.BaseStream.Position--;

            // read until drmEndOffset
            byte[] buffer = new byte[1];
            while (reader.Read(buffer, 0, 1) > 0)
            {
                writer.Write(buffer);

                if (reader.BaseStream.Position == drmEndOffset)
                    break;
            }

            // flush & close
            writer.Flush();
            writer.Close();
            reader.Close();
        }
    }
}
