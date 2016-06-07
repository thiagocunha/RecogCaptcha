using AForge.Imaging.Filters;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tesseract;

namespace RecogCaptcha
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openImageButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            Bitmap image = new Bitmap(openFileDialog.FileName);
            if (image != null)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(openFileDialog.FileName);
                resultLabel.Text = reconhecerCaptcha(image);
            }
            image.Dispose();
        }

        private string reconhecerCaptcha(Image img)
        {
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
            FiltersSequence seq = new FiltersSequence(cc, inverter);
            pictureBox.Image = seq.Apply(grayImage);
            string reconhecido = OCR((Bitmap)pictureBox.Image);
            return reconhecido;
        }

        private string OCR(Bitmap b)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", "BCDEFGHIJKLMNPRSTUVWXYZ123456789");
                engine.SetVariable("tessedit_unrej_any_wd", true);
                
                using (var page = engine.Process(b, PageSegMode.SingleWord))
                    res = page.GetText();
            }
            return res;
        }
    }
}
