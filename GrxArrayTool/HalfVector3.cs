using System;
using System.Diagnostics;
using System.IO;

namespace GrxArrayTool
{
    [DebuggerDisplay("x = {X}, y = {Y}, z = {Z}")]
    public class HalfVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public virtual void Read(BinaryReader reader)
        {
            X = Half.ToHalf(reader.ReadUInt16());
            Y = Half.ToHalf(reader.ReadUInt16());
            Z = Half.ToHalf(reader.ReadUInt16());
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(Half.GetBytes((Half)X));
            writer.Write(Half.GetBytes((Half)Y));
            writer.Write(Half.GetBytes((Half)Z));
        }
    }
}
