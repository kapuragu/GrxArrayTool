using System;
using System.Collections.Generic;
using System.IO;

namespace GrxArrayTool
{
    public class LightTypeOccluder
    {
        public uint valsOcc_1 { get; set; } // Different in GZ
        public Vector4[] Node { get; set; }
        public struct Face
        {
            public short value1 { get; set; }
            public short value2 { get; set; }
            public short VertexIndex { get; set; }
            public short Size { get; set; }
        }
        public Face[] Faces { get; set; }
        public void Read(BinaryReader reader, Dictionary<uint, string> hashLookupTable, HashIdentifiedDelegate hashIdentifiedCallback)
        {
            valsOcc_1 = reader.ReadUInt32();
            reader.BaseStream.Position += 4;
            uint facesCount = reader.ReadUInt32();
            reader.BaseStream.Position += 4;
            uint nodesCount = reader.ReadUInt32();
            Console.WriteLine($"Occluder entry");
            Console.WriteLine($"    valsOcc_1={valsOcc_1}");
            Console.WriteLine($"    edgesCount={facesCount}");
            Console.WriteLine($"    nodesCount={nodesCount}");
            Node = new Vector4[nodesCount];
            for (int i = 0; i < nodesCount; i++)
            {
                Node[i] = new Vector4();
                Node[i].Read(reader);
                Console.WriteLine($"    Node#{i} X={Node[i].X}, Y={Node[i].Y}, Z={Node[i].Z}, W={Node[i].W}");
            }
            Faces = new Face[facesCount];
            for (int i = 0; i < facesCount; i++)
            {
                Faces[i].value1 = reader.ReadInt16();
                Faces[i].value2 = reader.ReadInt16();
                Faces[i].VertexIndex = reader.ReadInt16();
                Faces[i].Size = reader.ReadInt16();
                Console.WriteLine($"    Face#{i} value1={Faces[i].value1}, value2={Faces[i].value2}, VertexIndex={Faces[i].VertexIndex}, Size={Faces[i].Size}");
            }
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(valsOcc_1);
            int nodeCount = Node.Length;
            writer.Write(0x10 * (nodeCount + 1));
            writer.Write(Faces.Length);
            writer.Write(8);
            writer.Write(nodeCount);
            for (int i=0; i<nodeCount;i++)
            {
                Node[i].Write(writer);
            }
            for (int i = 0; i < Faces.Length; i++)
            {
                writer.Write(Faces[i].value1);
                writer.Write(Faces[i].value2);
                writer.Write(Faces[i].VertexIndex);
                writer.Write(Faces[i].Size);
            }
        }
    }
}
