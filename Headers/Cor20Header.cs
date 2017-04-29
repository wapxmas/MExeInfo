using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo.Headers
{
    [Flags]
    public enum Cor20HeaderFlags
    {
        COMIMAGE_FLAGS_ILONLY = 0x00000001,
        COMIMAGE_FLAGS_32BITREQUIRED = 0x00000002,
        COMIMAGE_FLAGS_IL_LIBRARY = 0x00000004,
        COMIMAGE_FLAGS_STRONGNAMESIGNED = 0x00000008,
        COMIMAGE_FLAGS_NATIVE_ENTRYPOINT = 0x00000010,
        COMIMAGE_FLAGS_TRACKDEBUGDATA = 0x00010000,
        COMIMAGE_FLAGS_32BITPREFERRED = 0x00002000
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Cor20Header
    {
        [FieldOffset(0)]
        public UInt32 cb;

        [FieldOffset(4)]
        public ushort MajorRuntimeVersion;

        [FieldOffset(6)]
        public ushort MinorRuntimeVersion;

        [FieldOffset(8)]
        public DataDirectory MetaData;

        [FieldOffset(16)]
        public UInt32 Flags;

        [FieldOffset(20)]
        public UInt32 EntryPointToken;

        [FieldOffset(20)]
        public UInt32 EntryPointRVA;

        [FieldOffset(24)]
        public DataDirectory Resources;

        [FieldOffset(32)]
        public DataDirectory StrongNameSignature;

        [FieldOffset(40)]
        public DataDirectory CodeManagerTable;

        [FieldOffset(48)]
        public DataDirectory VTableFixups;

        [FieldOffset(56)]
        public DataDirectory ExportAddressTableJumps;

        [FieldOffset(64)]
        public DataDirectory ManagedNativeHeader;
    }
}
