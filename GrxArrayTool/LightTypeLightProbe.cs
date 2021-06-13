using System;
using System.Collections.Generic;
using System.IO;

namespace GrxArrayTool
{
    public class LightTypeLightProbe
    {
        public ulong HashName { get; set; }
        public uint vals4_2 { get; set; } // Different in GZ
        public uint vals4_3 { get; set; }
        public uint vals4_4 { get; set; } // Sometimes different in GZ?
        public float InnerScaleXPositive { get; set; }
        public float InnerScaleXNegative { get; set; }
        public float InnerScaleYPositive { get; set; }
        public float InnerScaleYNegative { get; set; }
        public float InnerScaleZPositive { get; set; }
        public float InnerScaleZNegative { get; set; }
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
        public string StringName { get; set; }
        public void Read(BinaryReader reader, Dictionary<uint, string> hashLookupTable, HashIdentifiedDelegate hashIdentifiedCallback)
        {
            HashName = reader.ReadUInt64();
            uint offsetToString = reader.ReadUInt32();
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

            StringName = "";
            if (offsetToString > 0)
            {
                StringName = reader.ReadCString();
                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }

            Console.WriteLine($"Light Probe entry NameHash={HashName} LightName='{StringName}'");
            Console.WriteLine($"    ProbeIndex={ProbeIndex} vals4_2={vals4_2} vals4_3={vals4_3} vals4_4={vals4_4}");
            Console.WriteLine($"    InnerScaleXPositive={InnerScaleXPositive} InnerScaleXNegative={InnerScaleXNegative}");
            Console.WriteLine($"    InnerScaleYPositive={InnerScaleYPositive} InnerScaleYNegative={InnerScaleYNegative}");
            Console.WriteLine($"    InnerScaleZPositive={InnerScaleZPositive} InnerScaleZNegative={InnerScaleZNegative}");
            Console.WriteLine($"    Scale X={Scale.X} Y={Scale.Y} Z={Scale.Z}");
            Console.WriteLine($"    Rotation X={Rotation.X} Y={Rotation.Y} Z={Rotation.Z} W={Rotation.W}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    vals16={vals16} Priority={Priority} u2={u2} n3={n3}");
            Console.WriteLine($"    LightSize={LightSize} u5={u5}");
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(HashName);
            if (StringName != "")
                writer.Write(0x58);
            else
                writer.Write(0);
            writer.Write(vals4_2);
            writer.Write(vals4_3);
            writer.Write(vals4_4);

            writer.Write(Half.GetBytes((Half)InnerScaleXPositive)); writer.Write(Half.GetBytes((Half)InnerScaleXNegative));
            writer.Write(Half.GetBytes((Half)InnerScaleYPositive)); writer.Write(Half.GetBytes((Half)InnerScaleYNegative));
            writer.Write(Half.GetBytes((Half)InnerScaleZPositive)); writer.Write(Half.GetBytes((Half)InnerScaleZNegative));

            Scale.Write(writer);
            Rotation.Write(writer);
            Translation.Write(writer);

            writer.Write(vals16);
            writer.Write(Priority);
            writer.Write(u2);
            writer.Write(n3);
            writer.Write(ProbeIndex);
            writer.Write(LightSize);
            writer.Write(u5);
            if (StringName != "")
            {
                writer.WriteCString(StringName);
                writer.WriteZeroes(1);//null byte for readcstring
                if (writer.BaseStream.Position % 0x4 != 0)
                    writer.WriteZeroes(0x4 - (int)writer.BaseStream.Position % 0x4);
            }

            Console.WriteLine($"Light Probe entry NameHash={HashName} LightName='{StringName}'");
            Console.WriteLine($"    ProbeIndex={ProbeIndex} vals4_2={vals4_2} vals4_3={vals4_3} vals4_4={vals4_4}");
            Console.WriteLine($"    InnerScaleXPositive={InnerScaleXPositive} InnerScaleXNegative={InnerScaleXNegative}");
            Console.WriteLine($"    InnerScaleYPositive={InnerScaleYPositive} InnerScaleYNegative={InnerScaleYNegative}");
            Console.WriteLine($"    InnerScaleZPositive={InnerScaleZPositive} InnerScaleZNegative={InnerScaleZNegative}");
            Console.WriteLine($"    Scale X={Scale.X} Y={Scale.Y} Z={Scale.Z}");
            Console.WriteLine($"    Rotation X={Rotation.X} Y={Rotation.Y} Z={Rotation.Z} W={Rotation.W}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    vals16={vals16} Priority={Priority} u2={u2} n3={n3}");
            Console.WriteLine($"    LightSize={LightSize} u5={u5}");
        }
    }
}
