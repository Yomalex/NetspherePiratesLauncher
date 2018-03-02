using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DamienG.Security.Cryptography;
using NetspherePiratesLauncher.Xml;

namespace PatchMaker
{
    class Program
    {
        public static string versionXml = @".\version.xml";
        static void Main(string[] args)
        {
            Console.Title = "Patch maker";
            if(args.Count()==0)
            {
                Console.WriteLine("Please pass as argument the client folder");
                Console.WriteLine("Example: dotnet.exe PatchMaker.dll \"C:\\FumbiClient\"");
                Console.ReadKey();
                return;
            }

            char key;
            do
            {
                Console.Write($"\rClient folder is '{args[0]}'? (Y/N):");
                key = Console.ReadKey().KeyChar;
                if (key == 'n' || key == 'N')
                    return;

            } while (key != 'Y' && key != 'y');

            Console.WriteLine("\nLoading patch info...");

            //var currentFolder = Path.Combine(args[0], @".\");
            VersionDto version;
            var fileList = new List<FileInfoDto>();
            var changeList = new List<FileInfoDto>();
            var curPatch = 0;
            var d = new XmlSerializer(typeof(VersionDto));

            try
            {
                using (var fr = File.OpenRead(versionXml))
                {
                    version = d.Deserialize(fr) as VersionDto;
                }

            }catch(FileNotFoundException e)
            {
                version = new VersionDto();
                version.Patchs = Array.Empty<PatchDto>();
            }
            
            foreach (var patch in version.Patchs)
            {
                foreach (var file in patch.Files)
                {
                    if (fileList.Exists(p => p.Name == file.Name))
                    {
                        var sfile = fileList.Find(p => p.Name == file.Name);
                        sfile.CRC = file.CRC;
                        continue;
                    }

                    fileList.Add(new FileInfoDto { Name = file.Name, CRC = file.CRC });// copy
                }

                curPatch++;
            }

            Console.WriteLine($"Version.xml have {curPatch} patchs.");
            Console.WriteLine("Making a file tree...");

            var files = Directory.EnumerateFiles(args[0], "*.*", SearchOption.AllDirectories);
            var crc = new Crc32();
            string hash;
            var counter = 0;

            foreach (var filei in files)
            {
                var file = Path.GetRelativePath(args[0], filei);

                if (file.ToLower() == versionXml.ToLower()
                    || Path.GetExtension(file).ToLower() == ".s4")
                    continue;

                hash = String.Empty;
                using (FileStream fs = File.OpenRead(filei))
                    foreach (byte b in crc.ComputeHash(fs)) hash += b.ToString("x2").ToLower();

                counter++;
                Console.Write($"\rScanned files {counter}, changes found {changeList.Count}");
                if (fileList.Exists(p => p.Name.ToLower() == file.ToLower())
                    && fileList.Find(p => p.Name.ToLower() == file.ToLower()).CRC == hash)
                    continue;

                changeList.Add(new FileInfoDto { Name = file, CRC = hash });
            }

            Console.WriteLine($"\nFound {changeList.Count} changes on files.");

            if(changeList.Any())
            {
                var patches = version.Patchs.ToList();
                var patch = new PatchDto();
                patch.Version = curPatch.ToString();
                patch.Files = changeList.ToArray();
                patches.Add(patch);
                version.Patchs = patches.ToArray();
            }


            Console.WriteLine("Saving changes...");

            using (var fs = File.OpenWrite(versionXml))
                d.Serialize(fs, version);

            Console.WriteLine("Changes saved.");
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
