using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using NetspherePiratesLauncher.Xml;

namespace NetspherePiratesLauncher
{
    static class Program
    {
        public static OptionsDto options;
        public static string optionsPath = "./patcher_s4.option.s4";
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            options = StartUp();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Patcher_s4());
            ShutDown();
        }

        static OptionsDto StartUp()
        {
            var serializer = new XmlSerializer(typeof(OptionsDto));

            try
            {
                using (var r = new StreamReader(optionsPath))
                    return (OptionsDto)serializer.Deserialize(r);
            }catch(Exception e)
            {
                var options = new OptionsDto();
                options.Version = "0";
                options.Lang = "eng";
                options.AuthIP = "127.0.0.1";
                options.Register = "http://localhost/Register";
                options.News = "http://localhost/News";
                options.FTP = new FtpInfo();
                options.FTP.URL = "ftp://localhost/";
                options.FTP.User = "";
                options.FTP.Password = "";

                using (var r = new StreamWriter(optionsPath))
                    serializer.Serialize(r, options);

                return options;
            }
        }

        static void ShutDown()
        {
            var serializer = new XmlSerializer(typeof(OptionsDto));

            using (var r = new StreamWriter(optionsPath))
                serializer.Serialize(r, options);
        }
    }
}
