using MExeInfo.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MExeInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Helpers.SetInvariantCulture();
            new Program().MExeInfo(args);
        }

        private void MExeInfo(string[] args)
        {
            if(args.Length == 0)
            {
                return;
            }

            using (FileStream stream = new FileStream(args[0], FileMode.Open))
            {
                var msDosHdr = stream.ReadStructure<MsDosHeader>();

                if(!msDosHdr.HasValue)
                {
                    Console.WriteLine("File don't have a MsDos header.");
                    return;
                }

                var msDosSig = new string(msDosHdr.Value.signature);

                if(msDosSig != "MZ")
                {
                    Console.WriteLine("File is containing an incorrect MsDos header signature.");
                    return;
                }

                var peOffset = msDosHdr.Value.e_lfanew - Marshal.SizeOf(typeof(MsDosHeader));

                if(peOffset < 0)
                {
                    Console.WriteLine("File is containing an incorrect offset to the PE header.");
                    return;
                }

                stream.Seek(peOffset, SeekOrigin.Current);

                var peSign = stream.ReadStructure<PESign>();

                if(!peSign.HasValue)
                {
                    Console.WriteLine("File don't have a PE signature header.");
                    return;
                }

                var peSignStr = new string(peSign.Value.signature);

                if(peSignStr != "PE")
                {
                    Console.WriteLine("File is containing an incorrect PE signature header.");
                    return;
                }

                var coffHdr = stream.ReadStructure<CoffHeader>();

                if(!coffHdr.HasValue)
                {
                    Console.WriteLine("File don't have a COFF header.");
                    return;
                }

                switch(coffHdr.Value.Machine)
                {
                    case CoffHeaderMachineValues.IMAGE_FILE_MACHINE_I386:
                        Console.WriteLine($"Machine: x86");
                        break;
                    case CoffHeaderMachineValues.IMAGE_FILE_MACHINE_IA64:
                        Console.WriteLine($"Machine: Intel Itanium");
                        break;
                    case CoffHeaderMachineValues.IMAGE_FILE_MACHINE_AMD64:
                        Console.WriteLine($"Machine: x64");
                        break;
                    case CoffHeaderMachineValues.IMAGE_FILE_MACHINE_ARM:
                        Console.WriteLine($"Machine: ARM");
                        break;
                    default:
                        Console.WriteLine($"Unknown machine: 0x{coffHdr.Value.Machine:x4}");
                        return;
                }

                var coffFlags = (CoffHeaderCharacteristicsFlags)coffHdr.Value.Characteristics;

                Console.WriteLine($"Characteristics: {coffFlags.ToString()}");

                var peMagic = stream.ReadStructure<PEMagicHeader>();

                if(!peMagic.HasValue)
                {
                    Console.WriteLine("File is containing an incorrect optional PE magic header.");
                    return;
                }

                int peOptHdrSize = 0;
                UInt32 peOptDataSize = 0;
                switch(peMagic.Value.Magic)
                {
                    case PEMagicHeaderValues.IMAGE_NT_OPTIONAL_HDR32_MAGIC:
                        Console.WriteLine("PE opt. type: x32");
                        peOptHdrSize = Marshal.SizeOf(typeof(PE32Header));
                        var pe32Header = stream.ReadStructure<PE32Header>();
                        if (!pe32Header.HasValue)
                        {
                            Console.WriteLine("File is containing an incorrect optional PE header.");
                            return;
                        }
                        ShowPEOptInfo(pe32Header.Value);
                        peOptDataSize = pe32Header.Value.NumberOfRvaAndSizes;
                        break;
                    case PEMagicHeaderValues.IMAGE_NT_OPTIONAL_HDR64_MAGIC:
                        Console.WriteLine("PE opt. type: x64");
                        peOptHdrSize = Marshal.SizeOf(typeof(PE64Header));
                        var pe64Header = stream.ReadStructure<PE64Header>();
                        if (!pe64Header.HasValue)
                        {
                            Console.WriteLine("File is containing an incorrect optional PE header.");
                            return;
                        }
                        ShowPEOptInfo(pe64Header.Value);
                        peOptDataSize = pe64Header.Value.NumberOfRvaAndSizes;
                        break;
                    default:
                        Console.WriteLine($"File is containing an optional PE magic header that is not suitable for CLR executables: 0x{peMagic.Value.Magic:x3}");
                        return;
                }

                if(peOptDataSize < 16)
                {
                    Console.WriteLine("File is containing incorrect number of IMAGE_DATA_DIRECTORY.");
                    return;
                }

                var dataDirs = new DataDirectory[peOptDataSize];

                for(int i = 0; i < peOptDataSize; i++)
                {
                    var d = stream.ReadStructure<DataDirectory>();
                    if(!d.HasValue)
                    {
                        Console.WriteLine("Can't read an element of IMAGE_DATA_DIRECTORY.");
                        return;
                    }
                    dataDirs[i] = d.Value;
                }

                Console.WriteLine($"CLR RVA and Size: 0x{dataDirs[14].VirtualAddress:x} {dataDirs[14].Size}");

                UInt32 rvaOfCLRHeader = dataDirs[14].VirtualAddress;

                Console.WriteLine($"Total sections number: {coffHdr.Value.NumberOfSections}\n");

                Console.WriteLine("Sections:");

                SectionHeader[] sections = new SectionHeader[coffHdr.Value.NumberOfSections];

                for (int i = 0; i < coffHdr.Value.NumberOfSections; i++)
                {
                    var s = stream.ReadStructure<SectionHeader>();

                    if (!s.HasValue)
                    {
                        Console.WriteLine($"Can't read section #{i}");
                        return;
                    }

                    sections[i] = s.Value;

                    var name = new string(sections[i].Name.TakeWhile(c => c != 0).ToArray());
                    Console.WriteLine($"{i + 1}) {name}");

                    var characteristics = (SectionHeaderCharacteristicsFlags)sections[i].Characteristics;
                    Console.WriteLine($"Characteristics: {characteristics.ToString()}");
                    Console.WriteLine($"Offset: 0x{sections[i].PointerToRawData:x4}");
                    Console.WriteLine($"VirtualAddress: 0x{sections[i].VirtualAddress:x}");

                    if (name == ".text")
                    {
                        var offsetToClrHeader = rvaOfCLRHeader - sections[i].VirtualAddress + sections[i].PointerToRawData;
                        Console.WriteLine($"Offset CLR Header: 0x{offsetToClrHeader:x4}");
                        long old = stream.Seek(0, SeekOrigin.Current);
                        stream.Seek(offsetToClrHeader, SeekOrigin.Begin);
                        var cor20Header = stream.ReadStructure<Cor20Header>();
                        if(!cor20Header.HasValue)
                        {
                            Console.WriteLine("File is containing incorrect CLR header.");
                            return;
                        }
                        Console.WriteLine($"CLR runtime ver.: {cor20Header.Value.MajorRuntimeVersion}.{cor20Header.Value.MinorRuntimeVersion}");
                        Console.WriteLine($"CLR flags: {((Cor20HeaderFlags)cor20Header.Value.Flags).ToString()}");
                        stream.Seek(old, SeekOrigin.Begin);
                    }
                }
            }
        }

        private void ShowPEOptInfo(dynamic info)
        {
            Console.WriteLine($"PE opt. compiler ver.: {info.MajorLinkerVersion}.{info.MinorLinkerVersion}");
            Console.WriteLine($"PE opt. os ver.: {info.MajorOperatingSystemVersion}.{info.MinorOperatingSystemVersion}");
            Console.WriteLine($"PE opt. subs ver.: {info.MajorSubsystemVersion}.{info.MinorSubsystemVersion}");
            Console.WriteLine($"PE opt. image ver.: {info.MajorImageVersion}.{info.MinorImageVersion}");
            Console.WriteLine($"PE opt. win32 ver.: {info.Win32VersionValue}");
            Console.WriteLine($"PE opt. num. data dirs.: {info.NumberOfRvaAndSizes}");
        }
    }
}
