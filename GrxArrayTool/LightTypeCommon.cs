using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GrxArrayTool
{
    public class LightTypeCommon : ILight
    {
        public uint EntryType { get; set; }
        public uint EntryLength { get; set; }
        public ulong NameHash { get; set; }
        public string DataSetPath { get; set; }

        public void Read(BinaryReader reader, Dictionary<uint, string> hashLookupTable, HashIdentifiedDelegate hashIdentifiedCallback)
        {
            NameHash = reader.ReadUInt64(); //Doesn't look like the PathCode64 of the .fox2?
            reader.BaseStream.Position += 8;
            DataSetPath = reader.ReadCString();
            if (reader.BaseStream.Position % 0x4 != 0)
                reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;

            Console.WriteLine("Dataset entry");
            Console.WriteLine($"NameHash {NameHash}");
            Console.WriteLine($"DataSetPath {DataSetPath}");
        }

        public void Write(BinaryWriter writer)
        {

        }
    }
}
