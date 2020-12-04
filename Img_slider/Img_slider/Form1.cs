using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.IO;

namespace Img_slider
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.button1.Click += new System.EventHandler(this.button1_Click);

            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);

            trackBar1.Maximum = 255;
            trackBar1.TickFrequency = 10;
            trackBar1.LargeChange = 3;
            trackBar1.SmallChange = 2;

        }
       
        private Mat MyImage;
        private Mat MyImage_Binary;

        private void button1_Click(object sender, EventArgs e)
        {
            MyImage = Cv2.ImRead("../../cat.jpg");
            MyImage_Binary = Cv2.ImRead("../../cat.jpg");
            
            // 'OpenCvSharp.Mat' 형식을 'System.Drawing.Image' 형식으로 변환할 수 없습니다.
            //  해결하기 위해서 사용
            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(MyImage);

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(MyImage != null)
            {
                Cv2.Threshold(MyImage, MyImage_Binary, trackBar1.Value, 255, ThresholdTypes.Binary);

                pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(MyImage_Binary);

                textBox1.Text = "" + trackBar1.Value;
            }
            else
            {
                MessageBox.Show("이미지를 불러와주세요.");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
