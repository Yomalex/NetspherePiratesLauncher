using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetspherePiratesLauncher
{
    public partial class Patcher_s4 : Form
    {
        private Bitmap _bmpStart = Properties.Resources.start2;
        private Bitmap _bmpRegister = Properties.Resources.register1;
        private Bitmap _bmpCancel = Properties.Resources.cancel2;
        private Download manager;
        public readonly Dictionary<string, RadioButton> assoc;
 

        public Patcher_s4()
        {
            InitializeComponent();

            assoc = new Dictionary<string, RadioButton>()
            {
                { "ger", radioButton1 },
                { "eng", radioButton2 },
                { "fre", radioButton3 },
                { "kor", radioButton4 },
                { "spa", radioButton5 },
                { "rus", radioButton6 }
            };

            manager = new Download(label1, progressBar2, progressBar1);
            manager.StartUpdate();

            assoc[Program.options.Lang].Checked = true;
            webBrowser1.Url = new Uri(Program.options.News);

            Start.MouseEnter += (object sender, EventArgs e) => { _bmpStart = Properties.Resources.start3; };
            Start.MouseLeave += (object sender, EventArgs e) => { _bmpStart = Properties.Resources.start2; };
            Register.MouseEnter += (object sender, EventArgs e) => { _bmpRegister = Properties.Resources.register3; };
            Register.MouseLeave += (object sender, EventArgs e) => { _bmpRegister = Properties.Resources.register1; };
            button1.MouseEnter += (object sender, EventArgs e) => { _bmpCancel = Properties.Resources.cancel1; };
            button1.MouseLeave += (object sender, EventArgs e) => { _bmpCancel = Properties.Resources.cancel2; };
        }

        private void Start_Click(object sender, EventArgs e)
        {
            var tmp = assoc.FirstOrDefault(p => p.Value.Checked == true);            
            Program.options.Lang = tmp.Key;

            var runString = $"-rc:eu -lac:{Program.options.Lang} -auth_server_ip:{Program.options.AuthIP}";
            Process.Start("S4Client.exe", runString);
            Close();
        }

        private void Register_Click(object sender, EventArgs e)
        {
            Process.Start(Program.options.Register);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tmp = assoc.FirstOrDefault(p => p.Value.Checked == true);
            Program.options.Lang = tmp.Key;
            Close();
        }

        private void btStart_Paint(object sender, PaintEventArgs e)
        {
            if(manager.CanStart)
            {
                var bmp = _bmpStart;
                bmp.MakeTransparent(Color.FromArgb(240,0,255));
                e.Graphics.DrawImage(bmp, 0, 0);
            }else
            {
                var bmp = Properties.Resources.start1;
                bmp.MakeTransparent(Color.FromArgb(240, 0, 255));
                e.Graphics.DrawImage(bmp, 0, 0);
            }
        }

        private void Register_paint(object sender, PaintEventArgs e)
        {
            var bmp = _bmpRegister;
            bmp.MakeTransparent(Color.FromArgb(240, 0, 255));
            e.Graphics.DrawImage(bmp, 0, 0);
        }

        private void button1_paint(object sender, PaintEventArgs e)
        {
            var bmp = _bmpCancel;
            bmp.MakeTransparent(Color.FromArgb(240, 0, 255));
            e.Graphics.DrawImage(bmp, 0, 0);
        }
    }
}
