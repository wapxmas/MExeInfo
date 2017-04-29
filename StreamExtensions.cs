using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo
{
    public static class StreamExtensions
    {
        public static T? ReadStructure<T>(this Stream stream) where T : struct
        {
            if (stream == null)
            {
                return null;
            }

            int size = Marshal.SizeOf(typeof(T));

            byte[] bytes = new byte[size];

            if (stream.Read(bytes, 0, size) != size)
            {
                return null;
            }

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
