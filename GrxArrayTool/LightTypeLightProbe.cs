using System;
using System.IO;

namespace GrxArrayTool
{
    public class LightTypeLightProbe
    {
        public ulong HashName { get; set; }
        public uint Flags1 { get; set; }
        public uint LightFlags { get; set; } //Bitfield: 0x1 is Enable
        public uint Flags2 { get; set; }
        public float InnerScaleXPositive { get; set; } //To-do: figure out which one is positive and which is negative
        public float InnerScaleYPositive { get; set; }
        public float InnerScaleZPositive { get; set; }
        public float InnerScaleXNegative { get; set; }
        public float InnerScaleYNegative { get; set; }
        public float InnerScaleZNegative { get; set; }
        public Vector3 Scale { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Translation { get; set; }
        public float Exposure { get; set; } // translation's w?
        public short Priority { get; set; }
        public short ShapeType { get; set; } // shapeType? 0 - square, 1 - triangular prism, 2 - semi-cylindiral, 3 - half-square
        public short RelatedLightIndex { get; set; } // relatedLights index
        public ushort SHDataIndex { get; set; }
        public float OcclusionModeOpenRate { get; set; } // most time is 1
        public float u5 { get; set; } //most time is 0
        public string StringName { get; set; }
        public void Read(BinaryReader reader)
        {
            HashName = reader.ReadUInt64();
            uint offsetToString = reader.ReadUInt32();
            Flags1 = reader.ReadUInt32();
            LightFlags = reader.ReadUInt32();
            Flags2 = reader.ReadUInt32();
            InnerScaleXPositive = Half.ToHalf(reader.ReadUInt16());
            InnerScaleYPositive = Half.ToHalf(reader.ReadUInt16());
            InnerScaleZPositive = Half.ToHalf(reader.ReadUInt16());
            InnerScaleXNegative = Half.ToHalf(reader.ReadUInt16());
            InnerScaleYNegative = Half.ToHalf(reader.ReadUInt16());
            InnerScaleZNegative = Half.ToHalf(reader.ReadUInt16());

            Scale = new Vector3();
            Scale.Read(reader);

            Rotation = new Vector4();
            Rotation.Read(reader);

            Translation = new Vector3();
            Translation.Read(reader);

            Exposure = reader.ReadSingle();
            Priority = reader.ReadInt16();
            ShapeType = reader.ReadInt16();
            RelatedLightIndex = reader.ReadInt16(); // related light id (TppLightProbeArray)
            SHDataIndex = reader.ReadUInt16(); // probe index used by shDatas and drawRejectionLevels (TppLightProbeArray)
            OcclusionModeOpenRate = reader.ReadSingle();
            u5 = reader.ReadSingle();

            StringName = string.Empty;
            if (offsetToString > 0)
            {
                StringName = reader.ReadCString();
                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }

            Log();
        }
        public void Write(BinaryWriter writer)
        {
            if (StringName != string.Empty)
            {
                writer.Write(HashManager.StrCode64(StringName));
                writer.Write(0x58);
            }
            else
            {
                writer.Write(HashName);
                writer.Write(0);
            }
            writer.Write(Flags1);
            writer.Write(LightFlags);
            writer.Write(Flags2);

            writer.Write(Half.GetBytes((Half)InnerScaleXPositive)); writer.Write(Half.GetBytes((Half)InnerScaleYPositive));
            writer.Write(Half.GetBytes((Half)InnerScaleZPositive)); writer.Write(Half.GetBytes((Half)InnerScaleXNegative));
            writer.Write(Half.GetBytes((Half)InnerScaleYNegative)); writer.Write(Half.GetBytes((Half)InnerScaleZNegative));

            Scale.Write(writer);
            Rotation.Write(writer);
            Translation.Write(writer);

            writer.Write(Exposure);
            writer.Write(Priority);
            writer.Write(ShapeType);
            writer.Write(RelatedLightIndex);
            writer.Write(SHDataIndex);
            writer.Write(OcclusionModeOpenRate);
            writer.Write(u5);
            if (StringName != string.Empty)
            {
                writer.WriteCString(StringName);
                writer.WriteZeroes(1);//null byte for readcstring
                if (writer.BaseStream.Position % 0x4 != 0)
                    writer.WriteZeroes(0x4 - (int)writer.BaseStream.Position % 0x4);
            }

            Log();
        }
        public void Log()
        {
            Console.WriteLine($"Light Probe entry StrCode64={HashName} StringName='{StringName}'");
            Console.WriteLine($"    LightProbeIndex={SHDataIndex} vals4_2={Flags1} LightFlags={LightFlags} vals4_4={Flags2}");
            Console.WriteLine($"    InnerScaleXPositive={InnerScaleXPositive} InnerScaleXNegative={InnerScaleYPositive}");
            Console.WriteLine($"    InnerScaleYPositive={InnerScaleZPositive} InnerScaleYNegative={InnerScaleXNegative}");
            Console.WriteLine($"    InnerScaleZPositive={InnerScaleYNegative} InnerScaleZNegative={InnerScaleZNegative}");
            Console.WriteLine($"    Scale X={Scale.X} Y={Scale.Y} Z={Scale.Z}");
            Console.WriteLine($"    Rotation X={Rotation.X} Y={Rotation.Y} Z={Rotation.Z} W={Rotation.W}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    vals16={Exposure} Priority={Priority} ShapeType={ShapeType} RelatedLightIndex={RelatedLightIndex}");
            Console.WriteLine($"    LightSize={OcclusionModeOpenRate} u5={u5}");
        }
    }
}
