﻿using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace GrxArrayTool
{
    [DebuggerDisplay("x = {X}, y = {Y}, z = {Z}")]
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public virtual void Read(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }
}
