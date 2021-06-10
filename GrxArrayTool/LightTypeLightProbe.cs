using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GrxArrayTool
{
    public class LightTypeLightProbe : ILight
    {
        public uint EntryType { get; set; }
        public uint EntryLength { get; set; }
        public ulong LightNameHash { get; set; }
        public bool HasString { get; set; }
        public uint vals4_2 { get; set; } // Different in GZ
        public uint vals4_3 { get; set; }
        public uint vals4_4 { get; set; } // Sometimes different in GZ?
        public Half InnerScaleXPositive { get; set; }
        public Half InnerScaleXNegative { get; set; }
        public Half InnerScaleYPositive { get; set; }
        public Half InnerScaleYNegative { get; set; }
        public Half InnerScaleZPositive { get; set; }
        public Half InnerScaleZNegative { get; set; }
        public Vector3 Scale { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Translation { get; set; }
        public float vals16 { get; set; }
        public short Priority { get; set; }
        public short u2 { get; set; }
        public short n3 { get; set; } // some other kind of index? sometimes it increases with entries, sometimes skips
        public ushort ProbeIndex { get; set; }
        public float LightSize { get; set; }
        public float u5 { get; set; }
        public string LightName { get; set; }
        public void Read(BinaryReader reader, Dictionary<uint, string> hashLookupTable, HashIdentifiedDelegate hashIdentifiedCallback)
        {
            LightNameHash = reader.ReadUInt64(); //Doesn't look like the PathCode64 of the .fox2?
            uint offsetToString = reader.ReadUInt32();
            if (offsetToString > 0)
            {
                HasString = true;
            }
            vals4_2 = reader.ReadUInt32();
            vals4_3 = reader.ReadUInt32();
            vals4_4 = reader.ReadUInt32();

            InnerScaleXPositive = Half.ToHalf(reader.ReadUInt16());
            InnerScaleXNegative = Half.ToHalf(reader.ReadUInt16());
            InnerScaleYPositive = Half.ToHalf(reader.ReadUInt16());
            InnerScaleYNegative = Half.ToHalf(reader.ReadUInt16());
            InnerScaleZPositive = Half.ToHalf(reader.ReadUInt16());
            InnerScaleZNegative = Half.ToHalf(reader.ReadUInt16());

            Scale = new Vector3();
            Scale.Read(reader);

            Rotation = new Vector4();
            Rotation.Read(reader);

            Translation = new Vector3();
            Translation.Read(reader);

            vals16 = reader.ReadSingle();

            Priority = reader.ReadInt16();
            u2 = reader.ReadInt16();
            n3 = reader.ReadInt16();

            ProbeIndex = reader.ReadUInt16();

            LightSize = reader.ReadSingle();

            u5 = reader.ReadSingle();

            if (HasString)
            {
                LightName = reader.ReadCString();
                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }
        }
        public void Write(BinaryWriter writer)
        {

        }
    }
}
