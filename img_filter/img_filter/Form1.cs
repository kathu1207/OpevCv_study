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

namespace img_filter
{
    public partial class Form1 : Form
    {
        private Mat MyImage;
        private Mat Filter = new Mat();
        private Mat Threshold = new Mat();
        private Mat Edge = new Mat();

        private double threshold_Value;

        public Form1()
        {
            InitializeComponent();

            this.button1.Click += new System.EventHandler(this.button1_Click);

            this.comboBox1.Items.AddRange(new object[] {"Blur",
                        "Box",
                        "Median",
                        "Gaussian",
                        "Bilateral"});

            this.comboBox2.Items.AddRange(new object[] {"Canny",
                        "Sobel",
                        "Scharr",
                        "Laplacian"});
        }

        //블럭화
        private void button1_Click(object sender, EventArgs e)
        {
           
        }


        //Filter
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;

            switch (index)
            {
                case 0:
                    // 단순 흐림 
                    //입력, 출력, 커널크기, 앵커 (-1, -1)이 중심, 테두리 유형
                    Cv2.Blur(MyImage, Filter, new OpenCvSharp.Size(5, 5), new OpenCvSharp.Point(-1, -1), BorderTypes.Default);
                    break;
                case 1:
                    //박스 필터
                    Cv2.BoxFilter(MyImage, Filter, MatType.CV_8UC3, new OpenCvSharp.Size(7, 7), new OpenCvSharp.Point(-1, -1), true, BorderTypes.Default);
                    break;
                case 2:
                    //중간값 흐림
                    Cv2.MedianBlur(MyImage, Filter, 9);
                    break;
                case 3:
                    //가우시안 흐림
                    Cv2.GaussianBlur(MyImage, Filter, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);
                    break;
                case 4:
                    //쌍방 필터
                    //과도한 노이즈 필터링이 필요한 오프라인에는 d = 9를 사용하는 것이 좋습니다.
                    //시그마 값이 크면 클수록 만화 처럼 보입니다.(50 ,50)
                    Cv2.BilateralFilter(MyImage, Filter, 9, 50, 50, BorderTypes.Default);
                    break;
            }

            pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Filter);

        }

        //Threshold
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //textBox에서 숫자를 전부 지우면 ""값이 들어와서 오류가 발생함
            if(textBox1.Text == "")
            {
                threshold_Value = 0; 
            }
            else
            {
                threshold_Value = Convert.ToDouble(textBox1.Text);
            }

            Cv2.Threshold(Filter,Threshold, threshold_Value, 255, ThresholdTypes.Binary);

            pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Threshold);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MyImage = Cv2.ImRead("../../lena_Grayscale.png");

            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(MyImage);

        }
    }
}
