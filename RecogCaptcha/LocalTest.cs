using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecogCaptcha
{
    public partial class LocalTest : Form
    {
        BackgroundWorker bw = new BackgroundWorker();
        Dictionary<int, string> imgs = new Dictionary<int, string>();

        public LocalTest()
        {
            InitializeComponent();
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.WorkerReportsProgress = true;
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnTeste.Enabled = true;
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            KeyValuePair<string, string> dados = (KeyValuePair<string, string>)e.UserState;
            imageList1.Images.Add(Image.FromFile(dados.Key));
            listView1.Items.Add(new ListViewItem()
            {
                Text = dados.Value,
                ImageIndex = e.ProgressPercentage
            });
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnTeste_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            imageList1.Images.Clear();
            imgs.Clear();

            btnTeste.Enabled = false;
            bw.RunWorkerAsync();


        }

        private string Quebrar(string arqName, int mode)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tesseract.exe",
                    Arguments = arqName + @" stdout -c tessedit_char_whitelist=BCDFGHIJKLMNPQRSTVWXYZ123456789 -psm " + mode + " -l lat2",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            return proc.StandardOutput.ReadLine();
        }

        private bool ValidaQuebra(string temp)
        {
            return !(temp == null || temp.Trim().Length < 3 || (temp.Trim().IndexOf(" ") >= 0 && temp.Trim().IndexOf(" ") < 4));
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(txtPath.Text);
            int qtd = 0;

            foreach (var arq in di.GetFiles())
            {
                string line = string.Empty;
                for (int i = 6; i < 9; i++)
                {
                    string temp = Quebrar(arq.FullName, i);
                    
                    if (ValidaQuebra(temp))
                    {
                        line = temp.Substring(0, 4);
                        break;
                    }
                }


                KeyValuePair<string, string> dados = new KeyValuePair<string, string>(arq.FullName, line);
                imgs.Add(qtd, arq.FullName);
                bw.ReportProgress(qtd, dados);
                qtd++;

                if (qtd > 102)
                {
                    break;
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].ImageIndex >= 0)
            {
                pictureBox1.Image = Image.FromFile(imgs[listView1.SelectedItems[0].ImageIndex]);
            }
        }
    }
}
