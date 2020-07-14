using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patcher
{
    public static class Patches
    {
        public static readonly Dictionary<string, List<KeyValuePair<int, byte[]>>> PatchList = new Dictionary<string, List<KeyValuePair<int, byte[]>>>()
        {
            {
                "Disable Signature Checks",
                new List<KeyValuePair<int, byte[]>>()
                {
                    { new KeyValuePair<int, byte[]>(0x5969, new byte[] { 0xEB } ) },
                    { new KeyValuePair<int, byte[]>(0x38403D, new byte[] { 0xE9, 0x82, 0x01, 0x00 } ) }
                }
            },
            {
                "Disable Packed Executable Unlock Marker Checks",
                new List<KeyValuePair<int, byte[]>>()
                {
                    { new KeyValuePair<int, byte[]>(0x52EE, new byte[] { 0xEB } ) }
                }
            }
        };
    }
}
