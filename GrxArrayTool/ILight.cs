using System.Collections.Generic;
using System.IO;

namespace GrxArrayTool
{
    public interface ILight
    {
        uint EntryType { get; set; }
        uint EntryLength { get; set; }

        void Read(BinaryReader reader, Dictionary<uint, string> hashLookupTable, HashIdentifiedDelegate hashIdentifiedCallback);
        void Write(BinaryWriter writer);
    }
}
