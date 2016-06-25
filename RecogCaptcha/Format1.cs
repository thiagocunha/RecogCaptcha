using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecogCaptcha
{
    public partial class Format1 : Form
    {
        public Format1()
        {
            InitializeComponent();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(txtCaminho.Text);
            DirectoryInfo di2 = di.GetDirectories("format2")[0];

            var arqs = di.GetFiles("*.png");
            int i = 0;

            foreach (var arq in arqs)
            {
                i++;
                TransformCaptcha(arq.FullName, di2.FullName + @"\" + arq.Name);

                if (i % 1000 == 0)
                {
                    GC.Collect();
                }
            }
        }

        private void TransformCaptcha(string imgPath, string output)
        {
            Image img = Image.FromFile(imgPath);
            Bitmap imagem = new Bitmap(img);
            imagem = imagem.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Grayscale cinza = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = cinza.Apply(imagem);
            SISThreshold filter = new SISThreshold();
            filter.ApplyInPlace(grayImage);
            Invert inverter = new Invert();
            /*
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            cor.Red = new AForge.IntRange(200, 255);
            cor.Green = new AForge.IntRange(200, 255);
            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            bc.MinHeight = 10;
            Closing close = new Closing();
            GaussianSharpen gs = new GaussianSharpen();
            */
            ContrastCorrection cc = new ContrastCorrection();
            //ResizeBicubic filterResize = new ResizeBicubic(615, 195);
            FiltersSequence seq = new FiltersSequence(cc, inverter);
            seq.Apply(grayImage).Save(output);
        }
    }
}
