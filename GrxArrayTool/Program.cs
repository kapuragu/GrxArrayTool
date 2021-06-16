using Newtonsoft.Json;
using System;
using System.IO;

namespace GrxArrayTool
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            foreach (var filePath in args)
            {
                if (File.Exists(filePath))
                {
                    // Read input file
                    string fileExtension = Path.GetExtension(filePath);
                    if (fileExtension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        GrxArrayFile file = ReadFromJson(filePath);
                        if (file.Occluders.Count > 0)
                            WriteToBinary(file, Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath)) + ".grxoc");
                        else
                            WriteToBinary(file, Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath)) + ".grxla");
                    }
                    else if (fileExtension.Equals(".grxla", StringComparison.OrdinalIgnoreCase))
                    {
                        GrxArrayFile file = ReadFromBinary(filePath);
                        File.WriteAllText(Path.GetFileNameWithoutExtension(filePath) + ".grxla.json", JsonConvert.SerializeObject(file, Formatting.Indented));
                    }
                    else if (fileExtension.Equals(".grxoc", StringComparison.OrdinalIgnoreCase))
                    {
                        GrxArrayFile file = ReadFromBinary(filePath);
                        File.WriteAllText(Path.GetFileNameWithoutExtension(filePath) + ".grxoc.json", JsonConvert.SerializeObject(file, Formatting.Indented));
                    }
                    else
                    {
                        throw new IOException("Unrecognized input type.");
                    }
                }
            }
        }

        public static void WriteToBinary(GrxArrayFile file, string path)
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                file.Write(writer);
            }
        }

        public static GrxArrayFile ReadFromBinary(string path)
        {
            GrxArrayFile file = new GrxArrayFile();
            using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                file.Read(reader);
            }
            return file;
        }

        public static GrxArrayFile ReadFromJson(string path)
        {
            var file = JsonConvert.DeserializeObject<GrxArrayFile>(File.ReadAllText(path));
            return file;
        }
    }
}
