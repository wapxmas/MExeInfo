using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo.Headers
{
    [Flags]
    public enum SectionHeaderCharacteristicsFlags : UInt32
    {
        SCALE_INDEX = 0x00000001,
        CNT_CODE = 0x00000020,
        CNT_INITIALIZED_DATA = 0x00000040,
        CNT_UNINITIALIZED_DATA = 0x00000080,
        LNK_INFO = 0x00000200,
        NO_DEFER_SPEC_EXC = 0x00004000,
        LNK_NRELOC_OVFL = 0x01000000,
        MEM_DISCARDABLE = 0x02000000,
        MEM_NOT_CACHED = 0x04000000,
        MEM_NOT_PAGED = 0x08000000,
        MEM_SHARED = 0x10000000,
        MEM_EXECUTE = 0x20000000,
        MEM_READ = 0x40000000,
        MEM_WRITE = 0x80000000
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SectionHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [FieldOffset(0)]
        public char[] Name;

        [FieldOffset(8)]
        public UInt32 PhysicalAddress;

        [FieldOffset(8)]
        public UInt32 VirtualSize;

        [FieldOffset(12)]
        public UInt32 VirtualAddress;

        [FieldOffset(16)]
        public UInt32 SizeOfRawData;

        [FieldOffset(20)]
        public UInt32 PointerToRawData;

        [FieldOffset(24)]
        public UInt32 PointerToRelocations;

        [FieldOffset(28)]
        public UInt32 PointerToLinenumbers;

        [FieldOffset(32)]
        public ushort NumberOfRelocations;

        [FieldOffset(34)]
        public ushort NumberOfLinenumbers;

        [FieldOffset(36)]
        public UInt32 Characteristics;
    }
}
