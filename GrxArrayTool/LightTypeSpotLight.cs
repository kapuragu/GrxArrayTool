using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GrxArrayTool
{
    public class LightTypeSpotLight : ILight
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
        public Vector4 Rotation { get; set; }
        public Half OuterRange { get; set; }
        public Half InnerRange { get; set; }
        public Half UmbraAngle { get; set; }
        public Half PenumbraAngle { get; set; }
        public Half AttenuationExponent { get; set; }
        public Half vals14_6 { get; set; }
        public HalfVector4 Color { get; set; }
        public Half Temperature { get; set; }
        public Half ColorDeflection { get; set; } // inconsistency with pointlight having it as a float makes me doubt this is colordeflection too
        public float Lumen { get; set; }
        public Half vals10 { get; set; }
        public Half ShadowUmbraAngle { get; set; }
        public Half ShadowPenumbraAngle { get; set; }
        public Half ShadowAttenuationExponent { get; set; }
        public Half Dimmer { get; set; }
        public Half ShadowBias { get; set; }
        public Half ViewBias { get; set; }
        public Half vals11_1 { get; set; }
        public Half vals11_2 { get; set; }
        public Half vals11_3 { get; set; }
        public uint LodRadiusLevel { get; set; }
        public uint vals12_2 { get; set; }
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
            if (offsetToString > 0)
            {
                HasString = true;
            }
            vals4_2 = reader.ReadUInt32();
            vals4_3 = reader.ReadUInt32();
            vals4_4 = reader.ReadUInt32();

            uint offsetToLightArea = reader.ReadUInt32();
            if (offsetToLightArea > 0)
            {
                HasLightArea = true;
            }

            Translation = new Vector3();
            Translation.Read(reader);

            ReachPoint = new Vector3();
            ReachPoint.Read(reader);

            Rotation = new Vector4();
            Rotation.Read(reader);

            OuterRange = Half.ToHalf(reader.ReadUInt16());
            InnerRange = Half.ToHalf(reader.ReadUInt16());
            UmbraAngle = Half.ToHalf(reader.ReadUInt16());
            PenumbraAngle = Half.ToHalf(reader.ReadUInt16());
            AttenuationExponent = Half.ToHalf(reader.ReadUInt16());
            vals14_6 = Half.ToHalf(reader.ReadUInt16());

            Color = new HalfVector4();
            Color.Read(reader);

            Temperature = Half.ToHalf(reader.ReadUInt16());

            ColorDeflection = Half.ToHalf(reader.ReadUInt16());

            Lumen = reader.ReadSingle();

            vals10 = Half.ToHalf(reader.ReadUInt16());
            ShadowUmbraAngle = Half.ToHalf(reader.ReadUInt16());
            ShadowPenumbraAngle = Half.ToHalf(reader.ReadUInt16());
            ShadowAttenuationExponent = Half.ToHalf(reader.ReadUInt16());
            Dimmer = Half.ToHalf(reader.ReadUInt16());
            ShadowBias = Half.ToHalf(reader.ReadUInt16());
            ViewBias = Half.ToHalf(reader.ReadUInt16());
            vals11_1 = Half.ToHalf(reader.ReadUInt16());
            vals11_2 = Half.ToHalf(reader.ReadUInt16());
            vals11_3 = Half.ToHalf(reader.ReadUInt16());

            LodRadiusLevel = reader.ReadUInt32();
            vals12_2 = reader.ReadUInt32();

            uint offsetToIrraditationTransform = reader.ReadUInt32();
            if (offsetToIrraditationTransform > 0)
            {
                HasIrradiationPoint = true;
            }

            if (HasString)
            {
                LightName = reader.ReadCString();

                if (reader.BaseStream.Position % 0x4 != 0)
                    reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;
            }

            if (HasLightArea)
            {
                LightAreaScale = new Vector3();
                LightAreaScale.Read(reader);
                LightAreaRotation = new Vector4();
                LightAreaRotation.Read(reader);
                LightAreaTranslation = new Vector3();
                LightAreaTranslation.Read(reader);
            }

            if (HasIrradiationPoint)
            {
                IrradiationPointScale = new Vector3();
                IrradiationPointScale.Read(reader);
                IrradiationPointRotation = new Vector4();
                IrradiationPointRotation.Read(reader);
                IrradiationPointTranslation = new Vector3();
                IrradiationPointTranslation.Read(reader);
            }

            Console.WriteLine("Spotlight entry");
            Console.WriteLine($"NameHash {LightNameHash}");
            Console.WriteLine($"HasString {HasString}");
            Console.WriteLine($"vals4_2 {vals4_2} vals4_3 {vals4_3} vals4_4 {vals4_4}");
            Console.WriteLine($"Translation {Translation.X} {Translation.Y} {Translation.Z}");
            Console.WriteLine($"ReachPoint {ReachPoint.X} {ReachPoint.Y} {ReachPoint.Z}");
            Console.WriteLine($"Rotation {Rotation.X} {Rotation.Y} {Rotation.Z} {Rotation.W}");
            Console.WriteLine($"OuterRange {OuterRange} InnerRange {InnerRange} UmbraAngle {UmbraAngle} PenumbraAngle {PenumbraAngle} AttenuationExponent {AttenuationExponent} vals14_6 {vals14_6}");
            Console.WriteLine($"HasLightArea {HasLightArea}");
            Console.WriteLine($"HasIrradiationPoint {HasIrradiationPoint}");
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
