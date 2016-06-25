using Lattestrac.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecogCaptcha
{
    public partial class GetIDs : Form
    {
        HttpWebRequest webReq = null;
        CookieContainer cookieContainer = new CookieContainer();
        string searchURL = string.Empty;
        int pageSize = 400;
        StringBuilder sb = new StringBuilder();
        BackgroundWorker bw = new BackgroundWorker();
        int nQtdTotal = 0;
        int nBaixados = 0;

        public GetIDs()
        {
            InitializeComponent();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.WorkerReportsProgress = true;
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;

            if (e.UserState != null)
            {
                sb.AppendLine(e.UserState.ToString());
            }
            lblRegistros.Text = "Registros: " + nQtdTotal;
            lblBaixados.Text = "Baixados: " + nBaixados;
            //txtIDs.Text = sb.ToString();

            if (e.ProgressPercentage == 100)
            {
                File.WriteAllText(txtIDs.Text, sb.ToString());
                //txtIDs.Text = sb.ToString();
                //txtIDs.SelectAll();
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string pagina = this.GetWebReq(searchURL);
            Regex regQtdTotal = new Regex(@"intLTotReg.=.(?<total>\d.*?);");
            Match m = regQtdTotal.Match(pagina);
            nQtdTotal = Convert.ToInt32(m.Groups["total"].Value);
            int andamento = 0;
            int totalPages = nQtdTotal / pageSize;

            for (int i = 0; i <= totalPages; i++)
            {
                string paginaAtual = this.GetWebReq(this.MontaURLPagina(this.searchURL, i));
                if (totalPages >0)
                    andamento = (i*100) / totalPages;

                var listaCurriculos = CurriculumIDExtractor.GetPageCurriculums(paginaAtual);
                if (listaCurriculos.Count != pageSize)
                {
                    pageSize.ToString();
                }
                StringBuilder sb = new StringBuilder();
                foreach (string s in listaCurriculos)
                {
                    nBaixados++;
                    sb.AppendLine(s);
                    bw.ReportProgress(andamento, s);
                }
            }

            bw.ReportProgress(100, null);
        }

        private string MontaURLPagina(string url, int pagina)
        {
            return url.Replace("10;10", string.Format("{0};{1}", pagina * pageSize, pageSize));
        }

        private string GetWebReq(string url)
        {
            bool reqRealizada = false;
            int tentativas = 0;
            webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Timeout = 1000 * 60 * 2; // 2 minutos
            webReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.112 Safari/537.36";
            webReq.CookieContainer = cookieContainer;

            while (!reqRealizada && tentativas < 5)
            {
                try
                {
                    WebResponse wr = webReq.GetResponse();

                    Stream receiveStream = wr.GetResponseStream();
                    StreamReader sReader = new StreamReader(receiveStream, Encoding.UTF8);
                    string responseString = sReader.ReadToEnd();
                    return responseString;
                }
                catch
                {
                }
                tentativas++;
            }

            return "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.searchURL = txtURL.Text;
            bw.RunWorkerAsync();
        }
    }
}
