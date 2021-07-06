using System;
using System.Collections.Generic;
using System.IO;

namespace GrxArrayTool
{
    public class LightTypePointLight
    {
        public ulong HashName { get; set; }
        public string StringName { get; set; }
        public uint Flags1 { get; set; } // Different in GZ
        public uint LightFlags { get; set; }
        public uint Flags2 { get; set; } // Sometimes different in GZ?
        public Vector3 Translation { get; set; }
        public HalfVector3 ReachPoint { get; set; }
        public HalfVector4 Color { get; set; }
        public float Temperature { get; set; }
        public float ColorDeflection { get; set; }
        public float Lumen { get; set; }
        public float LightSize { get; set; }
        public float Dimmer { get; set; }
        public float ShadowBias { get; set; }
        public float LodFarSize { get; set; }
        public float LodNearSize { get; set; }
        public float LodShadowDrawRate { get; set; }
        public uint LodRadiusLevel { get; set; }
        public uint LodFadeType { get; set; }
        public ExtraTransform LightArea { get; set; }
        public ExtraTransform IrradiationPoint { get; set; }
        public void Read(BinaryReader reader)
        {
            HashName = reader.ReadUInt64(); //Doesn't look like the PathCode64 of the .fox2?
            uint offsetToString = reader.ReadUInt32();
            Flags1 = reader.ReadUInt32();
            LightFlags = reader.ReadUInt32();
            Flags2 = reader.ReadUInt32();
            uint offsetToLightArea = reader.ReadUInt32();

            Translation = new Vector3();
            Translation.Read(reader);

            ReachPoint = new HalfVector3();
            ReachPoint.Read(reader);

            Color = new HalfVector4();
            Color.Read(reader);

            Temperature = Half.ToHalf(reader.ReadUInt16());
            ColorDeflection = reader.ReadSingle();
            Lumen = reader.ReadSingle();
            LightSize = Half.ToHalf(reader.ReadUInt16());
            Dimmer = Half.ToHalf(reader.ReadUInt16());
            ShadowBias = Half.ToHalf(reader.ReadUInt16());
            LodFarSize = Half.ToHalf(reader.ReadUInt16());
            LodNearSize = Half.ToHalf(reader.ReadUInt16());
            LodShadowDrawRate = Half.ToHalf(reader.ReadUInt16());
            LodRadiusLevel = reader.ReadUInt32();
            LodFadeType = reader.ReadUInt32();
            uint offsetToIrraditationTransform = reader.ReadUInt32();

            StringName = string.Empty;
            //PS3 files don't use strings for light objects (however there's no way to tell byte sex apart so tool won't parse them anyway)
            if (offsetToString > 0)
            {
                StringName = reader.ReadCString();
                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }

            Console.WriteLine($"Point light entry name: StrCode64={HashName} StringName='{StringName}'");
            Console.WriteLine($"    vals4_2={Flags1} LightFlags={LightFlags} vals4_4={Flags2}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    ReachPoint X={ReachPoint.X} Y={ReachPoint.Y} Z={ReachPoint.Z}");
            Console.WriteLine($"    Color X={Color.X} Y={Color.Y} Z={Color.Z} W={Color.W}");
            Console.WriteLine($"    Temperature={Temperature} ColorDeflection={ColorDeflection} Lumen={Lumen}");
            Console.WriteLine($"    vals5_3={LightSize} vals5_4={Dimmer} vals3_1={ShadowBias}");
            Console.WriteLine($"    vals3_2={LodFarSize} vals6={LodNearSize} vals13={LodShadowDrawRate}");
            Console.WriteLine($"    vals7_1={LodRadiusLevel} vals7_2={LodFadeType}");

            if (offsetToLightArea > 0)
            {
                LightArea = new ExtraTransform();
                LightArea.Read(reader);
            }
            else
                LightArea = null;
            if (offsetToIrraditationTransform > 0)
            {
                IrradiationPoint = new ExtraTransform();
                IrradiationPoint.Read(reader);
            }
            else
                IrradiationPoint = null;

            Log();
        }
        public void Write(BinaryWriter writer)
        {
            int offsetToTransforms = 0x50;
            if (StringName != string.Empty)
            {
                writer.Write(HashManager.StrCode64(StringName));
                writer.Write(offsetToTransforms);
                offsetToTransforms += StringName.Length + 1;
                if (offsetToTransforms % 0x4 != 0)
                    offsetToTransforms += (0x4 - offsetToTransforms % 0x4);
            }
            else
            {
                writer.Write(HashName);
                writer.Write(0);
            }
            writer.Write(Flags1);
            writer.Write(LightFlags);
            writer.Write(Flags2);
            if (LightArea!=null)
                writer.Write(offsetToTransforms-0x10);
            else
                writer.Write(0);
            Translation.Write(writer);
            ReachPoint.Write(writer);
            Color.Write(writer);

            writer.Write(Half.GetBytes((Half)Temperature));
            writer.Write(ColorDeflection);
            writer.Write(Lumen);
            writer.Write(Half.GetBytes((Half)LightSize));
            writer.Write(Half.GetBytes((Half)Dimmer));
            writer.Write(Half.GetBytes((Half)ShadowBias));
            writer.Write(Half.GetBytes((Half)LodFarSize));
            writer.Write(Half.GetBytes((Half)LodNearSize));
            writer.Write(Half.GetBytes((Half)LodShadowDrawRate));
            writer.Write(LodRadiusLevel);
            writer.Write(LodFadeType);

            if (IrradiationPoint!=null)
                writer.Write((offsetToTransforms + 0x28) - 0x4C);
            else
                writer.Write(0);

            if (StringName != string.Empty)
            {
                writer.WriteCString(StringName); writer.WriteZeroes(1);
                if (writer.BaseStream.Position % 0x4 != 0)
                    writer.WriteZeroes(0x4 - (int)writer.BaseStream.Position % 0x4);
            }

            if (LightArea!=null)
                LightArea.Write(writer);

            if (IrradiationPoint!=null)
                IrradiationPoint.Write(writer);

            Log();
        }
        public void Log()
        {
            Console.WriteLine($"Point light entry StrCode64={HashName} StringName='{StringName}'");
            Console.WriteLine($"    vals4_2={Flags1} LightFlags={LightFlags} vals4_4={Flags2}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    ReachPoint X={ReachPoint.X} Y={ReachPoint.Y} Z={ReachPoint.Z}");
            Console.WriteLine($"    Color X={Color.X} Y={Color.Y} Z={Color.Z} W={Color.W}");
            Console.WriteLine($"    Temperature={Temperature} ColorDeflection={ColorDeflection} Lumen={Lumen}");
            Console.WriteLine($"    vals5_3={LightSize} vals5_4={Dimmer} vals3_1={ShadowBias}");
            Console.WriteLine($"    vals3_2={LodFarSize} vals6={LodNearSize} vals13={LodShadowDrawRate}");
            Console.WriteLine($"    vals7_1={LodRadiusLevel} vals7_2={LodFadeType}");
            if (LightArea != null)
            {
                Console.WriteLine("        LightArea");
                LightArea.Log();
            }
            if (IrradiationPoint != null)
            {
                Console.WriteLine("        IrradiationPoint");
                IrradiationPoint.Log();
            }
        }
    }
}
