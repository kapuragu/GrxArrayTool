using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace GrxArrayTool
{
    [DebuggerDisplay("x = {X}, y = {Y}, z = {Z}")]
    public class HalfVector3
    {
        public Half X { get; set; }
        public Half Y { get; set; }
        public Half Z { get; set; }
        
        public virtual void Read(BinaryReader reader)
        {
            X = Half.ToHalf(reader.ReadUInt16());
            Y = Half.ToHalf(reader.ReadUInt16());
            Z = Half.ToHalf(reader.ReadUInt16());
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(Half.GetBytes(X));
            writer.Write(Half.GetBytes(Y));
            writer.Write(Half.GetBytes(Z));
        }
    }
}
