using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GrxArrayTool
{
    public enum LightType
    {
        DataSetDefinition00 = 0x30304D43,
        NullTerminator = 0x00000000,
        DirectLight00 = 0x444C3030, //Referenced in exe, never used
        PointLight01 = 0x504C3031, //PL01 and SL01 refereced in exe but never used, GZ uses 02, TPP uses 03
        PointLight02 = 0x504C3032,
        PointLight03 = 0x504C3033,
        SpotLight01 = 0x534C3031,
        SpotLight02 = 0x534C3032,
        SpotLight03 = 0x33304C53,
        UnknownLP00 = 0x3030504C, //"LP00" but never used
        LightProbe00 = 0x30305045, //"EP00" but light probes
        Occluder00 = 0x4F433030, //.grxoc
    }
    
    public class GrxArrayFile
    {
        public LightType Type;
        public List<ILight> Lights = new List<ILight>();

        /// <summary>
        /// Reads and populates data from a binary lba file.
        /// </summary>
        public void Read(BinaryReader reader, HashManager hashManager)
        {
            // Read header
            uint signature = reader.ReadUInt32(); //FGxL
            reader.BaseStream.Position += 4;
            uint offsetToArray = reader.ReadUInt32();
            uint n2 = reader.ReadUInt32();

            // Read locators
            while (reader.BaseStream.Position!=reader.BaseStream.Length-8)
            {
                uint entryType = reader.ReadUInt32();
                uint entrySize = reader.ReadUInt32();
                Console.WriteLine($"entryType {entryType}");
                switch (entryType)
                {
                    case (uint)LightType.DataSetDefinition00:
                        ILight cm00 = new LightTypeCommon();
                        cm00.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        Lights.Add(cm00);
                        break;
                    case (uint)LightType.NullTerminator:
                        return;
                    case (uint)LightType.DirectLight00:
                        throw new ArgumentOutOfRangeException();
                    case (uint)LightType.PointLight01:
                    case (uint)LightType.PointLight02:
                    case (uint)LightType.PointLight03:
                        ILight pl03 = new LightTypePointLight();
                        pl03.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        Lights.Add(pl03);
                        break;
                    case (uint)LightType.SpotLight01:
                    case (uint)LightType.SpotLight02:
                    case (uint)LightType.SpotLight03:
                        ILight sl03 = new LightTypeSpotLight();
                        sl03.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        Lights.Add(sl03);
                        break;
                    case (uint)LightType.LightProbe00:
                        ILight ep00 = new LightTypeLightProbe();
                        ep00.Read(reader, hashManager.StrCode32LookupTable, hashManager.OnHashIdentified);
                        Lights.Add(ep00);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
        }

        /// <summary>
        /// Writes data to a binary lba file.
        /// </summary>
        public void Write(BinaryWriter writer)
        {
            // Write header
            writer.Write(Lights.Count);
            writer.Write((uint)Type);
            writer.Write(0);
            writer.Write(0);

            // Write locators
            foreach (var locator in Lights)
            {
                locator.Write(writer);
            }
        }
        
    }
}
