using System;
using System.Collections.Generic;
using System.IO;

namespace GrxArrayTool
{
    public enum LightType
    {
        DataSetDefinition00 = 0x30304D43,
        NullTerminator = 0x00000000,
        DirectLight00 = 0x30304C44, //Referenced in exe, never used
        PointLight01 = 0x31304C50, //PL01 and SL01 refereced in exe but never used, GZ uses 02, TPP uses 03, seemingly no difference
        PointLight02 = 0x32304C50,
        PointLight03 = 0x33304C50,
        SpotLight01 = 0x31304C53,
        SpotLight02 = 0x32304C53,
        SpotLight03 = 0x33304C53,
        UnknownLP00 = 0x3030504C, //"LP00" referenced in exe but never used
        LightProbe00 = 0x30305045, //"EP00" but actually the light probes
        Occluder00 = 0x3030434F, //"OC00".grxoc
    }
    public struct ExtraTransform
    {
        public Vector3 Scale { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Translation { get; set; }
    }
    public class GrxArrayFile
    {
        public ulong DataSetNameHash { get; set; }
        public string DataSetPath { get; set; }
        public List<LightTypePointLight> PointLights = new List<LightTypePointLight>();
        public List<LightTypeSpotLight> SpotLights = new List<LightTypeSpotLight>();
        public List<LightTypeLightProbe> LightProbes = new List<LightTypeLightProbe>();
        public List<LightTypeOccluder> Occluders = new List<LightTypeOccluder>();
        /// <summary>
        /// Reads and populates data from a binary lba file.
        /// </summary>
        public void Read(BinaryReader reader, HashManager hashManager)
        {
            // Read header
            uint signature = reader.ReadUInt32(); //FGxL or FGxO
            if (signature!=1282950982 && signature!=1333282630)
            {
                Console.WriteLine("Wrong signature!!! Not a FGx file?");
                throw new ArgumentOutOfRangeException();
            }
            //if (signature == 1333282630)

            reader.BaseStream.Position += 12;

            uint entryTypeDataSet = reader.ReadUInt32();
            reader.BaseStream.Position += 4; //nil
            DataSetNameHash = reader.ReadUInt64(); //Doesn't look like the PathCode64 of the .fox2?
            reader.BaseStream.Position += 8; //uint offsetToArray uint n2
            DataSetPath = reader.ReadCString();
            if (reader.BaseStream.Position % 0x4 != 0)
                reader.BaseStream.Position += 0x4 - reader.BaseStream.Position % 0x4;

            Console.WriteLine("Dataset entry");
            Console.WriteLine($"    NameHash {DataSetNameHash}");
            Console.WriteLine($"    DataSetPath {DataSetPath}");

            // Read locators
            while (reader.BaseStream.Position!=reader.BaseStream.Length-8) //Last 8 bytes is the terminator entry
            {
                uint entryType = reader.ReadUInt32();
                reader.BaseStream.Position += 4; //entrySize
                Console.WriteLine($"entryType {(LightType)entryType}");
                switch (entryType)
                {
                    case (uint)LightType.DataSetDefinition00:
                        throw new ArgumentOutOfRangeException(); //Should already be parsed
                    case (uint)LightType.NullTerminator:
                        Console.WriteLine("Null entry type should not be parsed !!!");
                        throw new ArgumentOutOfRangeException();
                    case (uint)LightType.DirectLight00:
                        Console.WriteLine("DL00 entry type is unknown !!!");
                        throw new ArgumentOutOfRangeException();
                    case (uint)LightType.PointLight01:
                    case (uint)LightType.PointLight02:
                    case (uint)LightType.PointLight03:
                        LightTypePointLight pl = new LightTypePointLight();
                        pl.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        PointLights.Add(pl);
                        break;
                    case (uint)LightType.SpotLight01:
                    case (uint)LightType.SpotLight02:
                    case (uint)LightType.SpotLight03:
                        LightTypeSpotLight sl = new LightTypeSpotLight();
                        sl.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        SpotLights.Add(sl);
                        break;
                    case (uint)LightType.UnknownLP00:
                        Console.WriteLine("LP00 entry type is unknown !!!");
                        throw new ArgumentOutOfRangeException();
                    case (uint)LightType.LightProbe00:
                        LightTypeLightProbe ep = new LightTypeLightProbe();
                        ep.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        LightProbes.Add(ep);
                        break;
                    case (uint)LightType.Occluder00:
                        LightTypeOccluder oc = new LightTypeOccluder();
                        oc.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        Occluders.Add(oc);
                        break;
                    default:
                        Console.WriteLine("Unrecognized entry type!!!");
                        throw new ArgumentOutOfRangeException();
                }
                if (entryType==(uint)LightType.PointLight02||entryType==(uint)LightType.SpotLight02)
                {
                    //is gz type
                }
            }
            
        }

        /// <summary>
        /// Writes data to a binary lba file.
        /// </summary>
        public void Write(BinaryWriter writer)
        {
            if (Occluders.Count == 0)
                writer.WriteCString("FGxL");
            else
                writer.WriteCString("FGxO");
            writer.WriteZeroes(4);
            writer.Write(0x10);
            writer.Write(1);

            // write dataset entry
            writer.Write(808471875);//CM00
            //calculate entry size
            int dataSetPathLength = (DataSetPath.Length + 1);
            if (dataSetPathLength % 0x4 != 0)
                dataSetPathLength += 0x4 - dataSetPathLength % 0x4;
            writer.Write(24 + dataSetPathLength); // entry size
            writer.Write(DataSetNameHash); //not actually the hash of the dataset path?
            writer.Write(8);
            writer.WriteZeroes(4);

            // write dataset string
            writer.WriteCString(DataSetPath);
            writer.WriteZeroes(1);//null byte for readcstring
            if (writer.BaseStream.Position % 0x4 != 0)
                writer.WriteZeroes(0x4 - (int)writer.BaseStream.Position % 0x4);

            // write lights
            if (PointLights.Count + SpotLights.Count > 0)
            {
                foreach (var light in PointLights)
                {
                    writer.WriteCString("PL03");
                    int entryLength = 0x60;
                    if (light.StringName != "")
                        entryLength += light.StringName.Length + 1;
                    if (entryLength % 0x4 != 0)
                        entryLength += (0x4 - entryLength % 0x4);
                    if (light.LightArea.Count > 0)
                        entryLength += 0x28;
                    if (light.IrradiationPoint.Count > 0)
                        entryLength += 0x28;
                    writer.Write(entryLength); //entry size
                    light.Write(writer);
                }
                foreach (var light in SpotLights)
                {
                    writer.WriteCString("SL03");
                    int entryLength = 0x88;
                    if (light.StringName !="")
                        entryLength += light.StringName.Length + 1;
                    if (entryLength % 0x4 != 0)
                        entryLength += (0x4 - entryLength % 0x4);
                    if (light.LightArea.Count > 0)
                        entryLength += 0x28;
                    if (light.IrradiationPoint.Count > 0)
                        entryLength += 0x28;
                    writer.Write(entryLength); //entry size
                    light.Write(writer);
                }
            }
            else if (LightProbes.Count > 0)
                foreach (var light in LightProbes)
                {
                    writer.WriteCString("EP00");
                    int entryLength = 0x68;
                    if (light.StringName != "")
                        entryLength += light.StringName.Length + 1;
                    if (entryLength % 0x4 != 0)
                        entryLength += (0x4 - entryLength % 0x4); 
                    writer.Write(entryLength); //entry size
                    light.Write(writer);
                }
            else if (Occluders.Count > 0)
                foreach (var light in Occluders)
                {
                    writer.WriteCString("OC00");
                    writer.Write(0x1C + (light.Faces.Length * 0x8) + (light.Node.Length * 0x10)); //entry size
                    light.Write(writer);
                }

            //write footer terminator entry
            writer.WriteZeroes(4);
            writer.Write(8);

        }
    }
}
