using System;
using System.Collections.Generic;
using System.IO;

namespace GrxArrayTool
{
    public class LightTypePointLight
    {
        public ulong HashName { get; set; }
        public string StringName { get; set; }
        public uint vals4_2 { get; set; } // Different in GZ
        public uint vals4_3 { get; set; }
        public uint vals4_4 { get; set; } // Sometimes different in GZ?
        public Vector3 Translation { get; set; }
        public HalfVector3 ReachPoint { get; set; }
        public HalfVector4 Color { get; set; }
        public float Temperature { get; set; }
        public float ColorDeflection { get; set; }
        public float Lumen { get; set; }
        public float vals5_3 { get; set; }
        public float vals5_4 { get; set; }
        public float vals3_1 { get; set; }
        public float vals3_2 { get; set; }
        public float vals6 { get; set; }
        public float vals13 { get; set; }
        public uint vals7_1 { get; set; }
        public uint vals7_2 { get; set; }
        public List<ExtraTransform> LightArea = new List<ExtraTransform>();
        public List<ExtraTransform> IrradiationPoint = new List<ExtraTransform>();
        public void Read(BinaryReader reader, Dictionary<uint, string> hashLookupTable, HashIdentifiedDelegate hashIdentifiedCallback)
        {
            HashName = reader.ReadUInt64(); //Doesn't look like the PathCode64 of the .fox2?
            uint offsetToString = reader.ReadUInt32();
            vals4_2 = reader.ReadUInt32();
            vals4_3 = reader.ReadUInt32();
            vals4_4 = reader.ReadUInt32();
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
            vals5_3 = Half.ToHalf(reader.ReadUInt16());
            vals5_4 = Half.ToHalf(reader.ReadUInt16());
            vals3_1 = Half.ToHalf(reader.ReadUInt16());
            vals3_2 = Half.ToHalf(reader.ReadUInt16());
            vals6 = Half.ToHalf(reader.ReadUInt16());
            vals13 = Half.ToHalf(reader.ReadUInt16());
            vals7_1 = reader.ReadUInt32();
            vals7_2 = reader.ReadUInt32();
            uint offsetToIrraditationTransform = reader.ReadUInt32();

            StringName = "";
            //PS3 files don't use strings for light objects (however there's no way to tell byte sex apart so tool won't parse them anyway)
            if (offsetToString > 0)
            {
                StringName = reader.ReadCString();
                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }

            Console.WriteLine($"Point light entry NameHash={HashName} LightName='{StringName}'");
            Console.WriteLine($"    vals4_2={vals4_2} vals4_3={vals4_3} vals4_4={vals4_4}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    ReachPoint X={ReachPoint.X} Y={ReachPoint.Y} Z={ReachPoint.Z}");
            Console.WriteLine($"    Color X={Color.X} Y={Color.Y} Z={Color.Z} W={Color.W}");
            Console.WriteLine($"    Temperature={Temperature} ColorDeflection={ColorDeflection} Lumen={Lumen}");
            Console.WriteLine($"    vals5_3={vals5_3} vals5_4={vals5_4} vals3_1={vals3_1}");
            Console.WriteLine($"    vals3_2={vals3_2} vals6={vals6} vals13={vals13}");
            Console.WriteLine($"    vals7_1={vals7_1} vals7_2={vals7_2}");

            ExtraTransform LightAreaTrasform = new ExtraTransform
            {
                Scale = new Vector3(),
                Rotation = new Vector4(),
                Translation = new Vector3()
            };
            if (offsetToLightArea > 0)
            {
                LightAreaTrasform.Scale.Read(reader);
                LightAreaTrasform.Rotation.Read(reader);
                LightAreaTrasform.Translation.Read(reader);
                LightArea.Add(LightAreaTrasform);
                Console.WriteLine($"        LightAreaScale X={LightAreaTrasform.Scale.X} Y={LightAreaTrasform.Scale.Y} Z={LightAreaTrasform.Scale.Z}");
                Console.WriteLine($"        LightAreaRotation X={LightAreaTrasform.Rotation.X} Y={LightAreaTrasform.Rotation.Y} Z={LightAreaTrasform.Rotation.Z} W={LightAreaTrasform.Rotation.W}");
                Console.WriteLine($"        LightAreaTranslation X={LightAreaTrasform.Translation.X} Y={LightAreaTrasform.Translation.Y} Z={LightAreaTrasform.Translation.Z}");
            }
            ExtraTransform IrradiationPointTransform = new ExtraTransform
            {
                Scale = new Vector3(),
                Rotation = new Vector4(),
                Translation = new Vector3()
            };
            if (offsetToIrraditationTransform > 0)
            {
                IrradiationPointTransform.Scale.Read(reader);
                IrradiationPointTransform.Rotation.Read(reader);
                IrradiationPointTransform.Translation.Read(reader);
                IrradiationPoint.Add(IrradiationPointTransform);
                Console.WriteLine($"        IrradiationPointScale X={IrradiationPointTransform.Scale.X} Y={IrradiationPointTransform.Scale.Y} Z={IrradiationPointTransform.Scale.Z}");
                Console.WriteLine($"        IrradiationPointRotation X={IrradiationPointTransform.Rotation.X} Y={IrradiationPointTransform.Rotation.Y} Z={IrradiationPointTransform.Rotation.Z} W={IrradiationPointTransform.Rotation.W}");
                Console.WriteLine($"        IrradiationPointTranslation X={IrradiationPointTransform.Translation.X} Y={IrradiationPointTransform.Translation.Y} Z={IrradiationPointTransform.Translation.Z}");
            }
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(HashName);
            int offsetToTransforms = 0x50;
            if (StringName != "")
            {
                writer.Write(offsetToTransforms);
                offsetToTransforms += StringName.Length + 1;
                if (offsetToTransforms % 0x4 != 0)
                    offsetToTransforms += (0x4 - offsetToTransforms % 0x4);
            }
            else
                writer.Write(0);
            writer.Write(vals4_2);
            writer.Write(vals4_3);
            writer.Write(vals4_4);
            if (LightArea.Count > 0)
                writer.Write(offsetToTransforms-0x10);
            else
                writer.Write(0);
            Translation.Write(writer);
            ReachPoint.Write(writer);
            Color.Write(writer);

            writer.Write(Half.GetBytes((Half)Temperature));
            writer.Write(ColorDeflection);
            writer.Write(Lumen);
            writer.Write(Half.GetBytes((Half)vals5_3));
            writer.Write(Half.GetBytes((Half)vals5_4));
            writer.Write(Half.GetBytes((Half)vals3_1));
            writer.Write(Half.GetBytes((Half)vals3_2));
            writer.Write(Half.GetBytes((Half)vals6));
            writer.Write(Half.GetBytes((Half)vals13));
            writer.Write(vals7_1);
            writer.Write(vals7_2);

            if (IrradiationPoint.Count > 0)
                writer.Write((offsetToTransforms + 0x28) - 0x4C);
            else
                writer.Write(0);

            if (StringName != "")
            {
                writer.WriteCString(StringName);
                writer.WriteZeroes(1);//null byte for readcstring
                if (writer.BaseStream.Position % 0x4 != 0)
                    writer.WriteZeroes(0x4 - (int)writer.BaseStream.Position % 0x4);
            }

            Console.WriteLine($"Point light entry NameHash={HashName} LightName='{StringName}'");
            Console.WriteLine($"    vals4_2={vals4_2} vals4_3={vals4_3} vals4_4={vals4_4}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    ReachPoint X={ReachPoint.X} Y={ReachPoint.Y} Z={ReachPoint.Z}");
            Console.WriteLine($"    Color X={Color.X} Y={Color.Y} Z={Color.Z} W={Color.W}");
            Console.WriteLine($"    Temperature={Temperature} ColorDeflection={ColorDeflection} Lumen={Lumen}");
            Console.WriteLine($"    vals5_3={vals5_3} vals5_4={vals5_4} vals3_1={vals3_1}");
            Console.WriteLine($"    vals3_2={vals3_2} vals6={vals6} vals13={vals13}");
            Console.WriteLine($"    vals7_1={vals7_1} vals7_2={vals7_2}");

            foreach (var lightArea in LightArea)
            {
                lightArea.Scale.Write(writer);
                lightArea.Rotation.Write(writer);
                lightArea.Translation.Write(writer);
                Console.WriteLine($"        LightAreaScale X={lightArea.Scale.X} Y={lightArea.Scale.Y} Z={lightArea.Scale.Z}");
                Console.WriteLine($"        LightAreaRotation X={lightArea.Rotation.X} Y={lightArea.Rotation.Y} Z={lightArea.Rotation.Z} W={lightArea.Rotation.W}");
                Console.WriteLine($"        LightAreaTranslation X={lightArea.Translation.X} Y={lightArea.Translation.Y} Z={lightArea.Translation.Z}");
            }

            foreach (var lightArea in IrradiationPoint)
            {
                lightArea.Scale.Write(writer);
                lightArea.Rotation.Write(writer);
                lightArea.Translation.Write(writer);
                Console.WriteLine($"        IrradiationPointScale X={lightArea.Scale.X} Y={lightArea.Scale.Y} Z={lightArea.Scale.Z}");
                Console.WriteLine($"        IrradiationPointRotation X={lightArea.Rotation.X} Y={lightArea.Rotation.Y} Z={lightArea.Rotation.Z} W={lightArea.Rotation.W}");
                Console.WriteLine($"        IrradiationPointTranslation X={lightArea.Translation.X} Y={lightArea.Translation.Y} Z={lightArea.Translation.Z}");
            }
        }
    }
}
