using System;
using System.IO;

namespace GrxArrayTool
{
    public class ExtraTransform
    {
        public Vector3 Scale { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Translation { get; set; }

        public virtual void Read(BinaryReader reader)
        {
            Scale = new Vector3();
            Rotation = new Vector4();
            Translation = new Vector3();
            Scale.Read(reader);
            Rotation.Read(reader);
            Translation.Read(reader);
        }

        public virtual void Write(BinaryWriter writer)
        {
            Scale.Write(writer);
            Rotation.Write(writer);
            Translation.Write(writer);
        }
        public virtual void Log()
        {
            Console.WriteLine($"         Scale X={Scale.X} Y={Scale.Y} Z={Scale.Z}");
            Console.WriteLine($"         Rotation X={Rotation.X} Y={Rotation.Y} Z={Rotation.Z} W={Rotation.W}");
            Console.WriteLine($"         Translation X={Translation.X} Y={Translation.Y} Z={Translation.Z}");
        }
    }
}
