using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo.Headers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataDirectory
    {
        public UInt32 VirtualAddress;
        public UInt32 Size;
    }
}
