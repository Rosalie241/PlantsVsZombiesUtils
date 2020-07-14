using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patcher
{
    static class Patcher
    {
        public static bool ApplyPatch(string executable, List<KeyValuePair<int, byte[]>> patch)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(executable)))
                {
                    foreach (var bytePatch in patch)
                    {
                        writer.BaseStream.Position = bytePatch.Key;
                        writer.Write(bytePatch.Value);
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

    }
}
