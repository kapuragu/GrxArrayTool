using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GrxArrayTool
{
    public class LightTypePointLight : ILight
    {
        public uint EntryType { get; set; }
        public uint EntryLength { get; set; }
        public ulong LightNameHash { get; set; }
        public bool HasString { get; set; }
        public uint vals4_2 { get; set; } // Different in GZ
        public uint vals4_3 { get; set; }
        public uint vals4_4 { get; set; } // Sometimes different in GZ?
        public bool HasLightArea { get; set; }
        public Vector3 Translation { get; set; }
        public Vector3 ReachPoint { get; set; }
        public HalfVector4 Color { get; set; }
        public Half Temperature { get; set; }
        public float ColorDeflection { get; set; }
        public float Lumen { get; set; }
        public Half vals5_3 { get; set; }
        public Half vals5_4 { get; set; }
        public Half vals3_1 { get; set; }
        public Half vals3_2 { get; set; }
        public Half vals6 { get; set; }
        public Half vals13 { get; set; }
        public uint vals7_1 { get; set; }
        public uint vals7_2 { get; set; }
        public bool HasIrradiationPoint { get; set; }
        public string LightName { get; set; }
        public Vector3 LightAreaScale { get; set; }
        public Vector4 LightAreaRotation { get; set; }
        public Vector3 LightAreaTranslation { get; set; }
        public Vector3 IrradiationPointScale { get; set; }
        public Vector4 IrradiationPointRotation { get; set; }
        public Vector3 IrradiationPointTranslation { get; set; }

        public void Read(BinaryReader reader, Dictionary<uint, string> hashLookupTable, HashIdentifiedDelegate hashIdentifiedCallback)
        {
            LightNameHash = reader.ReadUInt64(); //Doesn't look like the PathCode64 of the .fox2?
            uint offsetToString = reader.ReadUInt32();
            HasString = false;
            if (offsetToString > 0)
            {
                HasString = true;
            }
            vals4_2 = reader.ReadUInt32();
            vals4_3 = reader.ReadUInt32();
            vals4_4 = reader.ReadUInt32();

            HasLightArea = false;
            uint offsetToLightArea = reader.ReadUInt32();
            if (offsetToLightArea > 0)
            {
                HasLightArea = true;
            }
            Translation = new Vector3();
            Translation.Read(reader);

            ReachPoint = new Vector3();
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

            HasIrradiationPoint = false;
            uint offsetToIrraditationTransform = reader.ReadUInt32();
            if (offsetToIrraditationTransform > 0)
            {
                HasIrradiationPoint = true;
            }

            LightName = "";
            if (HasString)
            {
                LightName = reader.ReadCString();
                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }

            LightAreaScale.X = 0; LightAreaScale.Y = 0; LightAreaScale.Z = 0;
            LightAreaRotation.X = 0; LightAreaRotation.Y = 0; LightAreaRotation.Z = 0; LightAreaRotation.W = 0;
            LightAreaTranslation.X = 0; LightAreaTranslation.Y = 0; LightAreaTranslation.Z = 0;
            if (HasLightArea)
            {
                LightAreaScale = new Vector3();
                LightAreaScale.Read(reader);
                LightAreaRotation = new Vector4();
                LightAreaRotation.Read(reader);
                LightAreaTranslation = new Vector3();
                LightAreaTranslation.Read(reader);
            }

            IrradiationPointScale.X = 0; IrradiationPointScale.Y = 0; IrradiationPointScale.Z = 0;
            IrradiationPointRotation.X = 0; IrradiationPointRotation.Y = 0; IrradiationPointRotation.Z = 0; IrradiationPointRotation.W = 0;
            IrradiationPointTranslation.X = 0; IrradiationPointTranslation.Y = 0; IrradiationPointTranslation.Z = 0;
            if (HasIrradiationPoint)
            {
                IrradiationPointScale = new Vector3();
                IrradiationPointScale.Read(reader);
                IrradiationPointRotation = new Vector4();
                IrradiationPointRotation.Read(reader);
                IrradiationPointTranslation = new Vector3();
                IrradiationPointTranslation.Read(reader);
            }
        }

        public void Write(BinaryWriter writer)
        {

        }

        public void ReadJson()
        {
        }

        public void WriteJson()
        {
        }
    }
}
