using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using NetspherePiratesLauncher.Xml;
using System.Windows.Forms;
using DamienG.Security.Cryptography;

namespace NetspherePiratesLauncher
{
    public class Download
    {
        public Task Process;
        private readonly string versionFile = "./version.xml";
        private WebClient request;
        private string CurrentFile;
        private ProgressBar p, g;
        private Label l;
        private List<FileInfoDto> fileList;
        private bool Error;

        public bool CanStart => !fileList.Any() && !request.IsBusy && !Error;

        public Download(Label label, ProgressBar partial, ProgressBar global)
        {
            fileList = new List<FileInfoDto>();
            request = new WebClient();
            p = partial;
            g = global;
            l = label;
        }
                
        public void StartUpdate()
        {
            request.DownloadDataCompleted -= PartialComplete;
            request.DownloadDataCompleted += VersionComplete;
            DownloadFile("version.xml");
        }

        public void StartRepair()
        {
            request.DownloadDataCompleted -= PartialComplete;
            request.DownloadDataCompleted += VersionComplete;
            Program.options.Version = "-1";
            DownloadFile("version.xml");
        }

        private void DownloadFile(string file)
        {
            try
            {
                l.Text = $"Downloading {file}...";
                CurrentFile = file;
                var version = Path.Combine(Program.options.FTP.URL, CurrentFile);

                var server = new Uri(version);

                if (server.Scheme != Uri.UriSchemeFtp)
                {
                    throw new ArgumentException("URL isn't FTP");
                }

                request.DownloadProgressChanged += CurrentProgress;
                request.Credentials = new NetworkCredential(Program.options.FTP.User, Program.options.FTP.Password);
                request.DownloadDataAsync(server);
            }catch(WebException e)
            {
                MessageBox.Show(e.Message + e.Source);
            }
        }

        private void CurrentProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            p.Value = e.ProgressPercentage;
        }
        
        public void PartialComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                l.Text = e.Error.Message;
                MessageBox.Show(e.Error.Message);
                Error = true;
                return;
            }

            g.Value = g.Value + 1;

            if (!fileList.Any())
            {
                l.Text = "Start Game!";
                return;
            }

            var file = fileList.First();
            DownloadFile(file.Name);
            fileList.Remove(file);
        }

        public void VersionComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            request.DownloadDataCompleted -= VersionComplete;
            request.DownloadDataCompleted += PartialComplete;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                Error = true;
                return;
            }

            using (var w = File.Create(versionFile))
            {
                w.Write(e.Result,0,e.Result.Length);
            }

            VersionDto xml;
            var s = new XmlSerializer(typeof(VersionDto));
            using (var r = new MemoryStream(e.Result))
                xml = s.Deserialize(r) as VersionDto;

            var curVersion = int.Parse(Program.options.Version);
            var newVersions = from p in xml.Patchs
                              where int.Parse(p.Version) > curVersion
                              select p;


            foreach (var patch in newVersions)
            {
                foreach (var file in patch.Files)
                {
                    if (fileList.Exists(p => p.Name.ToLower() == file.Name.ToLower()))
                    {
                        var slot = fileList.Find(p => p.Name.ToLower() == file.Name.ToLower());
                        slot.CRC = file.CRC;
                    }else
                    {
                        fileList.Add(file);
                    }
                }
            }

            var crc = new Crc32();
            string hash;
            foreach (var file in fileList.ToList())
            {
                hash = String.Empty;
                using (FileStream fs = File.OpenRead(file.Name))
                    foreach (byte b in crc.ComputeHash(fs)) hash += b.ToString("x2").ToLower();

                if (hash == file.CRC)
                    fileList.Remove(file);
            }


            if (!fileList.Any())
            {
                l.Text = "Start Game!";
                g.Value = 100;
                return;
            }

            g.Maximum = fileList.Count;
            g.Value = 0;

            var f = fileList.First();
            DownloadFile(f.Name);
            fileList.Remove(f);
        }
    }
}
