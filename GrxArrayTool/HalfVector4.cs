using System;
using System.IO;

namespace GrxArrayTool
{
    public class HalfVector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public virtual void Read(BinaryReader reader)
        {
            X = Half.ToHalf(reader.ReadUInt16());
            Y = Half.ToHalf(reader.ReadUInt16());
            Z = Half.ToHalf(reader.ReadUInt16());
            W = Half.ToHalf(reader.ReadUInt16());
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(Half.GetBytes((Half)X));
            writer.Write(Half.GetBytes((Half)Y));
            writer.Write(Half.GetBytes((Half)Z));
            writer.Write(Half.GetBytes((Half)W));
        }
    }
}
