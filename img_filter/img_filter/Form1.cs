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

        public Form1()
        {
            InitializeComponent();

            this.button1.Click += new System.EventHandler(this.button1_Click);

            this.comboBox1.Items.AddRange(new object[] {"Blur",
                        "Box",
                        "Median",
                        "Gaussian",
                        "Bilateral"});

            DataTable table = new DataTable();

            table.Columns.Add("Index", typeof(string));
            table.Columns.Add("X, Y", typeof(string)); //중심점
            table.Columns.Add("Size", typeof(string)); //넓이
            table.Columns.Add("GD", typeof(string));   //절대값(바깥과 안쪽의 차이)
            table.Columns.Add("Area", typeof(string)); //면적 
            table.Columns.Add("Max", typeof(string));  //면적중 가장긴거
            table.Columns.Add("Main", typeof(string)); //면적중 가장 짧은것
            table.Columns.Add("Mean", typeof(string)); //면적의 평균

            dataGridView1.DataSource = table;

        }

        //블럭화
        private void button1_Click(object sender, EventArgs e)
        {
            Mat dst = Threshold.Clone();

            OpenCvSharp.Point[][] contours; //윤곽선의 실제 값
            HierarchyIndex[] hierarchy;    // 윤곽선들의 계층 구조

            Mat preprocess_Value = new Mat();

            Cv2.InRange(Threshold, new Scalar(127, 127, 127), new Scalar(255, 255, 255), preprocess_Value);

            //Cv2.FindContours(원본 배열, 검출된 윤곽선, 계층 구조, 검색 방법, 근사 방법, 오프셋)
            Cv2.FindContours(preprocess_Value, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

            foreach (OpenCvSharp.Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                double area = Cv2.ContourArea(p, true);

                if (length < 100 && area < 1000 && p.Length < 5) continue;

                bool convex = Cv2.IsContourConvex(p);
                OpenCvSharp.Point[] hull = Cv2.ConvexHull(p, true);
                Moments moments = Cv2.Moments(p, false);

                //Cv2.FillConvexPoly(dst, hull, Scalar.White);
                //Cv2.Polylines(dst, new Point[][] { hull }, true, Scalar.White, 1);
                Cv2.DrawContours(dst, new OpenCvSharp.Point[][] { hull }, -1, Scalar.Red, 1);
                Cv2.Circle(dst, (int)(moments.M10 / moments.M00), (int)(moments.M01 / moments.M00), 5, Scalar.Black, -1);
            }
            Cv2.ImShow("dst", dst);
            pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
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
            double threshold_Value;

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
