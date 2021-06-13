using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GrxArrayTool
{
    internal static class Program
    {
        private const string DefaultNameDictionaryFileName = "lba name dictionary.txt";
        private const string DefaultDataSetDictionaryFileName = "lba dataset dictionary.txt";
        private const string DefaultHashMatchOutputFileName = "lba hash matches.txt";

        private static void Main(string[] args)
        {
            var hashManager = new HashManager();

            // Read hash dictionaries
            if (File.Exists(DefaultNameDictionaryFileName))
            {
                hashManager.StrCode32LookupTable = MakeHashLookupTableFromFile(DefaultNameDictionaryFileName, FoxHash.Type.StrCode32);
                hashManager.PathCode32LookupTable = MakeHashLookupTableFromFile(DefaultDataSetDictionaryFileName, FoxHash.Type.PathCode32);
            }

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
                        GrxArrayFile file = ReadFromBinary(filePath, hashManager);
                        File.WriteAllText(Path.GetFileNameWithoutExtension(filePath) + ".grxla.json", JsonConvert.SerializeObject(file, Formatting.Indented));
                    }
                    else if (fileExtension.Equals(".grxoc", StringComparison.OrdinalIgnoreCase))
                    {
                        GrxArrayFile file = ReadFromBinary(filePath, hashManager);
                        File.WriteAllText(Path.GetFileNameWithoutExtension(filePath) + ".grxoc.json", JsonConvert.SerializeObject(file, Formatting.Indented));
                    }
                    else
                    {
                        throw new IOException("Unrecognized input type.");
                    }
                }
            }

            // Write hash matches output
            //WriteHashMatchesToFile(DefaultHashMatchOutputFileName, hashManager);
        }

        public static void WriteToBinary(GrxArrayFile file, string path)
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                file.Write(writer);
            }
        }

        public static GrxArrayFile ReadFromBinary(string path, HashManager hashManager)
        {
            GrxArrayFile file = new GrxArrayFile();
            using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                file.Read(reader, hashManager);
            }
            return file;
        }

        public static GrxArrayFile ReadFromJson(string path)
        {
            var file = JsonConvert.DeserializeObject<GrxArrayFile>(File.ReadAllText(path));
            return file;
        }

        /// <summary>
        /// Opens a file containing one string per line, hashes each string, and adds each pair to a lookup table.
        /// </summary>
        private static Dictionary<uint, string> MakeHashLookupTableFromFile(string path, FoxHash.Type hashType)
        {
            ConcurrentDictionary<uint, string> table = new ConcurrentDictionary<uint, string>();

            // Read file
            List<string> stringLiterals = new List<string>();
            using (StreamReader file = new StreamReader(path))
            {
                // TODO multi-thread
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    stringLiterals.Add(line);
                }
            }

            // Hash entries
            Parallel.ForEach(stringLiterals, (string entry) =>
            {
                if (hashType == FoxHash.Type.StrCode32)
                {
                    uint hash = HashManager.StrCode32(entry);
                    table.TryAdd(hash, entry);
                }
                else
                {
                    uint hash = HashManager.PathCode32(entry);
                    table.TryAdd(hash, entry);
                }
            });

            return new Dictionary<uint, string>(table);
        }

        /// <summary>
        /// Outputs all hash matched strings to a file.
        /// </summary>
        private static void WriteHashMatchesToFile(string path, HashManager hashManager)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                foreach (var entry in hashManager.UsedHashes)
                {
                    file.WriteLine(entry.Value);
                }
            }
        }
    }
}
