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
        public uint LightFlags { get; set; }
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
        public void Read(BinaryReader reader)
        {
            HashName = reader.ReadUInt64(); //Doesn't look like the PathCode64 of the .fox2?
            uint offsetToString = reader.ReadUInt32();
            vals4_2 = reader.ReadUInt32();
            LightFlags = reader.ReadUInt32();
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

            StringName = string.Empty;
            //PS3 files don't use strings for light objects (however there's no way to tell byte sex apart so tool won't parse them anyway)
            if (offsetToString > 0)
            {
                StringName = reader.ReadCString();
                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }

            Console.WriteLine($"Point light entry name: StrCode64={HashName} StringName='{StringName}'");
            Console.WriteLine($"    vals4_2={vals4_2} LightFlags={LightFlags} vals4_4={vals4_4}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    ReachPoint X={ReachPoint.X} Y={ReachPoint.Y} Z={ReachPoint.Z}");
            Console.WriteLine($"    Color X={Color.X} Y={Color.Y} Z={Color.Z} W={Color.W}");
            Console.WriteLine($"    Temperature={Temperature} ColorDeflection={ColorDeflection} Lumen={Lumen}");
            Console.WriteLine($"    vals5_3={vals5_3} vals5_4={vals5_4} vals3_1={vals3_1}");
            Console.WriteLine($"    vals3_2={vals3_2} vals6={vals6} vals13={vals13}");
            Console.WriteLine($"    vals7_1={vals7_1} vals7_2={vals7_2}");

            ExtraTransform LightAreaTrasform = new ExtraTransform();
            if (offsetToLightArea > 0)
            {
                LightAreaTrasform.Read(reader);
                LightArea.Add(LightAreaTrasform);
            }
            ExtraTransform IrradiationPointTransform = new ExtraTransform();
            if (offsetToIrraditationTransform > 0)
            {
                IrradiationPointTransform.Read(reader);
                IrradiationPoint.Add(IrradiationPointTransform);
            }

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
            writer.Write(vals4_2);
            writer.Write(LightFlags);
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

            if (StringName != string.Empty)
            {
                writer.WriteCString(StringName); writer.WriteZeroes(1);
                if (writer.BaseStream.Position % 0x4 != 0)
                    writer.WriteZeroes(0x4 - (int)writer.BaseStream.Position % 0x4);
            }

            foreach (var lightArea in LightArea)
            {
                lightArea.Write(writer);
            }

            foreach (var lightArea in IrradiationPoint)
            {
                lightArea.Write(writer);
            }

            Log();
        }
        public void Log()
        {
            Console.WriteLine($"Point light entry StrCode64={HashName} StringName='{StringName}'");
            Console.WriteLine($"    vals4_2={vals4_2} LightFlags={LightFlags} vals4_4={vals4_4}");
            Console.WriteLine($"    Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
            Console.WriteLine($"    ReachPoint X={ReachPoint.X} Y={ReachPoint.Y} Z={ReachPoint.Z}");
            Console.WriteLine($"    Color X={Color.X} Y={Color.Y} Z={Color.Z} W={Color.W}");
            Console.WriteLine($"    Temperature={Temperature} ColorDeflection={ColorDeflection} Lumen={Lumen}");
            Console.WriteLine($"    vals5_3={vals5_3} vals5_4={vals5_4} vals3_1={vals3_1}");
            Console.WriteLine($"    vals3_2={vals3_2} vals6={vals6} vals13={vals13}");
            Console.WriteLine($"    vals7_1={vals7_1} vals7_2={vals7_2}");
            foreach (var lightArea in LightArea)
            {
                Console.WriteLine("        LightArea");
                lightArea.Log();
            }
            foreach (var irradiationPoint in IrradiationPoint)
            {
                Console.WriteLine("        IrradiationPoint");
                irradiationPoint.Log();
            }
        }
    }
}
