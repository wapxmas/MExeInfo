using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo.Headers
{
    public static class PEMagicHeaderValues
    {
        public const int IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        public const int IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PEMagicHeader
    {
        public ushort Magic;
    }
}
