using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo.Headers
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PESign
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] signature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] nulls;
    }
}
