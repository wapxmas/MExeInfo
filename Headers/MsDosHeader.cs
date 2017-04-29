using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo.Headers
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MsDosHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] signature;
        public ushort lastsize;
        public ushort nblocks;
        public ushort nreloc;
        public ushort hdrsize;
        public ushort minalloc;
        public ushort maxalloc;
        public ushort ss;
        public ushort sp;
        public ushort checksum;
        public ushort ip;
        public ushort cs;
        public ushort relocpos;
        public ushort noverlay;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] reserved1;
        public ushort oem_id;
        public ushort oem_info;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] reserved2;
        public UInt32 e_lfanew;
    }
}
