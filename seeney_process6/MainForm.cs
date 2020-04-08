using ChaseDream.EnterpriseLibraries.DSkinEngine;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;




/*  */



namespace seeney_process6
{
    public partial class MainForm : DSkinForm
    {

        Field field = new Field();
        ProDB prodb = new ProDB();



        bool CansellistCag = true;
        // bool candraw = false;
        // bool RADOK_EVENT = true;

        int oldindex = 0;
        int selectindex = 0;
        string selecturl = null;
        string selectjson = null;
        Image selectimage;
        Graphics g;
        /*
        Graphics g1;
        Graphics g2;
        Graphics g3;
        Graphics g4;
        Graphics g5;
        Graphics g6;
        Graphics g7;
        Graphics g8;
        Graphics g9;
        Graphics g10;
        Graphics g11;
        Graphics g12;
        Graphics g13;
        Graphics g14;
        */
        
        int totalpage = 1;

        //2019-12-13 增加训练状态变量的输出和控制训练停止的状态量
        public struct Train_Status
        {
            public float avg_loss;             //训练实时当前平均loss值
            public float max_img_loss;         //建议绘制Loss曲线轴的显示图像最大值
            public float average_precision;    //训练实时当前识别率数值
            public int max_batches;            //建议绘制曲线叠代次数轴的最大值
            public int stop_flag; //初始化时为0，当需要中断训练时，该变量赋值1
            public int now_batches;
        };

        SiPoint sipoint = new SiPoint();


        int clickcount = 1;

        Shapes shapes = new Shapes();

        [DllImport("Augm_HZ.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Seeney_GetTrainValLists(string argv);

        [DllImport("Augm_HZ.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Seeney_GetYOLOv3LabeltoDefectsImage(string src_path, string dst_path);

        [DllImport("Augm_HZ.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Seeney_GetProgress();

        //[DllImport("train.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int Seeney_Train(string datacfg, string cfgfile, string _pathWeights);
        //是否继承
        [DllImport("train.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Seeney_Train(string datacfg, string cfgfile, string _pathWeights, bool Enable_Inherit);

        [DllImport("train.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Train_Status Get_Seeney_train_status(int stop_flag);//2019-12-13 增加訓練的狀態監控
                                                                                 //Control[] pictureboxs;


        Region kuangregion = new Region();
        List<string[]> picsAdresses = new List<string[]>();
        int nowpagenum = 1;
        int nowboxnum = 1;
        CrackConfig crackConfig = new CrackConfig();
        SortConfig sortConfig = new SortConfig();

        string strlabel = "";

        FileInfo[] json;

        int preangle = 90;

        public MainForm()
        {

            InitializeComponent();
            //this.Width = 1920;
            //this.Height = 1024;
            kuangregion.MakeEmpty();
            Panel_Kuang.Region = kuangregion;
            g = canvas.CreateGraphics();
            /*
            g1 = canvas.CreateGraphics();
            g2 = canvas.CreateGraphics();
            g3 = canvas.CreateGraphics();
            g4 = canvas.CreateGraphics();
            g5 = canvas.CreateGraphics();
            g6 = canvas.CreateGraphics();
            g7 = canvas.CreateGraphics();
            g8 = canvas.CreateGraphics();
            g9 = canvas.CreateGraphics();
            g10 = canvas.CreateGraphics();
            g11 = canvas.CreateGraphics();
            g12 = canvas.CreateGraphics();
            g13 = canvas.CreateGraphics();
            g14 = canvas.CreateGraphics();
            
            */
        }

        //读取txt文件中总行数的方法
        private int requestMethod(string _fileName)
        {
            System.Diagnostics.Stopwatch sw = new Stopwatch();
            var path = _fileName;
            int lines = 0;
            //按行读取
            sw.Restart();
            using (var sr = new StreamReader(path))
            {
                //use StreamReader 完了 自动dispose
                var ls = "";
                while ((ls = sr.ReadLine()) != null)
                {
                    lines++;
                }
            }
            sw.Stop();
            return lines;
        }






        public RectangleF GetWaiKuang(int Num)
        {
            RectangleF rectangleF = new RectangleF((Num - 1) * 102, 3, 104, 79);
            return rectangleF;
        }
        public RectangleF GetNeiKuang(int Num)
        {
            RectangleF rectangleF = new RectangleF(2 + (Num - 1) * 102, 5, 100, 75);
            return rectangleF;
        }

        public void DrawView()
        {

            if (picsAdresses[nowpagenum - 1][0] != "")
            {
                Graphics g = pictureBox1.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][0]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][0].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox1.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][1] != "")
            {
                Graphics g = pictureBox2.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][1]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][1].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();
            }
            else
            {
                Graphics g = pictureBox2.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][2] != "")
            {
                Graphics g = pictureBox3.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][2]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][2].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox3.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][3] != "")
            {
                Graphics g = pictureBox4.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][3]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][3].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox4.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][4] != "")
            {
                Graphics g = pictureBox5.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][4]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][4].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox5.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][5] != "")
            {
                Graphics g = pictureBox6.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][5]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][5].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox6.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][6] != "")
            {
                Graphics g = pictureBox7.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][6]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][6].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox7.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][7] != "")
            {
                Graphics g = pictureBox8.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][7]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][7].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox8.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][8] != "")
            {
                Graphics g = pictureBox9.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][8]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][8].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox9.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][9] != "")
            {
                Graphics g = pictureBox10.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][9]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][9].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox10.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][10] != "")
            {
                Graphics g = pictureBox11.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][10]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][10].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox11.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][11] != "")
            {
                Graphics g = pictureBox12.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][11]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][11].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox12.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][12] != "")
            {
                Graphics g = pictureBox13.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][12]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][12].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox13.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][13] != "")
            {
                Graphics g = pictureBox14.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][13]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                string selectjsonview = picsAdresses[nowpagenum - 1][13].Split('.')[0] + ".json";
                if (File.Exists(selectjsonview))
                {
                    Shapes shapesview = new Shapes();
                    using (StreamReader reader = new StreamReader(selectjsonview))
                    {
                        shapesview = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                    }

                    foreach (var j in shapesview.shape)
                    {
                        //  labelregion.Union()
                        g.DrawLine(Pens.Red, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X2 * 100 / nowimageview.Width, j.Y2 * 100 / nowimageview.Width, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X3 * 100 / nowimageview.Width, j.Y3 * 100 / nowimageview.Width, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width);
                        g.DrawLine(Pens.Red, j.X4 * 100 / nowimageview.Width, j.Y4 * 100 / nowimageview.Width, j.X1 * 100 / nowimageview.Width, j.Y1 * 100 / nowimageview.Width);
                    }
                    shapesview.shape.Clear();
                }
                else
                {

                }
                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox14.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
        }

        public PictureBox ImageLocation(Image image, Panel panel)
        {
            //picturebox位置 大小
            PictureBox picturebox = new PictureBox();

            if (image.Width <= panel.Width && image.Height <= panel.Height)
            {
                picturebox.Left = panel.Width / 2 - image.Width / 2;
                picturebox.Top = panel.Height / 2 - image.Height / 2;
                picturebox.Width = image.Width;
                picturebox.Height = image.Height;
            }
            else if (image.Width > panel.Width && image.Height <= panel.Height)
            {
                picturebox.Left = 0;

                picturebox.Width = panel.Width;
                picturebox.Height = panel.Width * image.Height / image.Width;
                picturebox.Top = panel.Height / 2 - picturebox.Height / 2;
            }
            else if (image.Width <= panel.Width && image.Height > panel.Height)
            {
                picturebox.Top = 0;
                picturebox.Height = panel.Height;
                picturebox.Width = panel.Height * image.Width / image.Height;
                picturebox.Left = panel.Width / 2 - picturebox.Width / 2;

            }
            else
            {
                if (image.Width * panel.Height > image.Height * panel.Width)
                {
                    picturebox.Left = 0;
                    picturebox.Width = panel.Width;
                    picturebox.Height = panel.Width * image.Height / image.Width;
                    picturebox.Top = panel.Height / 2 - picturebox.Height / 2;
                }
                else
                {
                    picturebox.Top = 0;
                    picturebox.Height = panel.Height;
                    picturebox.Width = panel.Height * image.Width / image.Height;
                    picturebox.Left = panel.Width / 2 - picturebox.Width / 2;
                }
            }
            return picturebox;
        }

        private void modelConfiged()
        {
            //缺陷不继承
            Thread Crack_childThread = new Thread(new ThreadStart(Crack_CallToChildThread));
            Crack_childThread.IsBackground = true;
            Crack_childThread.Start();
            
        }
        

        private void Sort_CallToChildThread()
        {
            // augm.Enabled = false;
            Directory.CreateDirectory( field.NowproPath + "\\NormalImages");
            Directory.CreateDirectory( field.NowproPath + "\\DefectImages");

            DirectoryInfo dirsrcNorpath = new DirectoryInfo( field.NowproPath + "\\image\\NormalImages");
            DirectoryInfo dirsrcDefpath = new DirectoryInfo( field.NowproPath + "\\image\\DefectImages");
            //  int sum = dirsrcNorpath.GetFiles("*.bmp").Length + dirsrcDefpath.GetFiles("*.bmp").Length;
            //  int procount = 0;
            foreach (FileInfo srcnorfile in dirsrcNorpath.GetFiles("*.bmp"))
            {
                Mat src = Cv2.ImRead(srcnorfile.FullName);
                Mat dst = new Mat(src.Size(), MatType.CV_8UC3);
                Point2f center = new Point2f(src.Cols / 2, src.Rows / 2);
                for (int angle = 0; angle < 360; angle += preangle)
                {
                    Mat M = Cv2.GetRotationMatrix2D(center, angle, 1);
                    Cv2.WarpAffine(src, dst, M, src.Size());
                    Cv2.ImWrite( field.NowproPath + "\\NormalImages\\" + srcnorfile.Name.Split('.')[0] + "_" + angle + ".bmp", dst);
                }
                //    procount++;
                //  SortcutProgress = procount / sum * 100;
                //  LoadingMsg("正在剪切", SortcutProgress);

            }
            foreach (FileInfo srcdeffile in dirsrcDefpath.GetFiles("*.bmp"))
            {
                Mat src = Cv2.ImRead(srcdeffile.FullName);
                Mat dst = new Mat(src.Size(), MatType.CV_8UC3);
                Point2f center = new Point2f(src.Cols / 2, src.Rows / 2);
                for (int angle = 0; angle < 360; angle += preangle)
                {
                    Mat M = Cv2.GetRotationMatrix2D(center, angle, 1);
                    Cv2.WarpAffine(src, dst, M, src.Size());
                    Cv2.ImWrite( field.NowproPath + "\\DefectImages\\" + srcdeffile.Name.Split('.')[0] + "_" + angle + ".bmp", dst);
                }

                //   procount++;
                //  SortcutProgress = procount / sum * 100;
                //  LoadingMsg("正在剪切", SortcutProgress);
            }
            DirectoryInfo dirdstNorpath = new DirectoryInfo( field.NowproPath + "\\NormalImages");
            DirectoryInfo dirdstDefpath = new DirectoryInfo( field.NowproPath + "\\DefectImages");
            List<string> imgmesg = new List<string>();
            List<string> miximgmesg = new List<string>();
            Random random = new Random();
            foreach (FileInfo srcNorfile in dirdstNorpath.GetFiles("*.bmp"))
            {
                imgmesg.Add("\\NormalImages\\" + srcNorfile.Name + " 1\r\n");
            }
            foreach (FileInfo srcDeffile in dirdstDefpath.GetFiles("*.bmp"))
            {
                imgmesg.Add("\\DefectImages\\" + srcDeffile.Name + " 0\r\n");
            }

            //1920*1080







            foreach (string i in imgmesg)
            {
                miximgmesg.Insert(random.Next(miximgmesg.Count), i);
            }
            string strtrainlist = "";
            string strvallist = "";
            int Count = 0;
            foreach (string j in miximgmesg)
            {
                Count++;
                if (Count < miximgmesg.Count * 7 / 8)
                {
                    strtrainlist += j;
                }
                else
                {
                    strvallist += j;
                }
            }

            StreamWriter trainwrite = new StreamWriter( field.NowproPath + "\\train_list.txt");
            trainwrite.Write(strtrainlist);
            trainwrite.Dispose();
            StreamWriter valwrite = new StreamWriter( field.NowproPath + "\\val_list.txt");
            valwrite.Write(strvallist);
            valwrite.Dispose();

            //  Thread.Sleep(100);

            // LoadingMsg("正在剪切", 100);
            // th.Abort();

            //  MessageBox.Show("剪切成功");
            // MessageBox.Show("剪切成功");
            //重写Crack_ResNet18_train_val.prototxt
            
            string strreplace =  field.NowproPath.Replace("\\", "/");
            File.Copy("Crack_ResNet18_train_val.prototxt", field.NowproPath + "\\" + field.Finalmodel + "\\model\\Crack_ResNet18_train_val.prototxt");
            StreamReader reader = new StreamReader(field.NowproPath + "\\" + field.Finalmodel + "\\model\\Crack_ResNet18_train_val.prototxt");
            string strread = reader.ReadToEnd();
            reader.Dispose();
            string[] strresul = Regex.Split(strread, "bottom: \"data\"");
            string strprototxt = "name: \"ResNet-18\"\r\n" +
"layer {\r\n" +
"    name: \"data\"\r\n" +
"    type: \"ImageData\"\r\n" +
"    top: \"data\"\r\n" +
"    top: \"label\"\r\n" +
"    include {\r\n" +
"        phase: TRAIN\r\n" +
"    }\r\n" +
"    transform_param {\r\n" +
"        mean_value: 104\r\n" +
"        mean_value: 117\r\n" +
"        mean_value: 123\r\n" +
"    }\r\n" +
"    image_data_param {\r\n" +
"        source: \"" + strreplace + "/train_list.txt\"\r\n" +
"        root_folder: \"" + strreplace + "\"\r\n" +
"        batch_size: " + sortConfig.batch_size1 + "\r\n" +
"        new_height: " + sortConfig.new_height + "\r\n" +
"        new_width: " + sortConfig.new_width + "\r\n" +
"    }\r\n" +
"}\r\n" +
"layer {\r\n" +
"    name: \"data\"\r\n" +
"    type: \"ImageData\"\r\n" +
"    top: \"data\"\r\n" +
"    top: \"label\"\r\n" +
"    include {\r\n" +
"        phase: TEST\r\n" +
"    }\r\n" +
"    transform_param {\r\n" +
"        mean_value: 104\r\n" +
"        mean_value: 117\r\n" +
"        mean_value: 123\r\n" +
"    }\r\n" +
"    image_data_param {\r\n" +
"        source: \"" + strreplace + "/val_list.txt\"\r\n" +
"        root_folder: \"" + strreplace + "\"\r\n" +
"        batch_size: " + sortConfig.batch_size2 + "\r\n" +
"        new_height: " + sortConfig.new_height + "\r\n" +
"        new_width: " + sortConfig.new_width + "\r\n" +
"   }\r\n" +
"}\r\n" +
"layer {\r\n" +
"    bottom: \"data\"";
            strprototxt = strprototxt + strresul[1];
            StreamWriter prototxtwriter = new StreamWriter(field.NowproPath + "\\" + field.Finalmodel + "\\model\\Crack_ResNet18_train_val.prototxt");
            prototxtwriter.Write(strprototxt);
            prototxtwriter.Dispose();

            //重写solver.prototxt
            File.Copy("solver.prototxt", field.NowproPath + "\\" + field.Finalmodel + "\\model\\solver.prototxt");
            StreamWriter solverprot = new StreamWriter(field.NowproPath + "\\" + field.Finalmodel + "\\model\\solver.prototxt");
            string str_solverprot = "net: \"Crack_ResNet18_train_val.prototxt\"\r\n" +
"# 10.1 = 162/16\r\n" +
"test_iter: 10\r\n" +
"test_interval: 100\r\n" +
"test_initialization: false\r\n" +
"\r\n" +
"\r\n" +
"lr_policy: \"multistep\"\r\n" +
"# 45.8125 = 1466 / 32\r\n" +
"stepvalue: 100\r\n" +
"stepvalue: 150\r\n" +
"max_iter: " + sortConfig.max_iter + "\r\n" +
"iter_size: 2\r\n" +
"\r\n" +
"\r\n" +
"gamma: 0.1\r\n" +
"base_lr: 0.001\r\n" +
"momentum: 0.9\r\n" +
"weight_decay: 0.0005\r\n" +
"\r\n" +
"display: 50\r\n" +
"snapshot: 50\r\n" +
"snapshot_prefix: \"ResNet18_DefectType02\"\r\n" +
"solver_mode: GPU";
            solverprot.Write(str_solverprot);
            solverprot.Dispose();
            Seeney_Train("", field.NowproPath + "\\" + field.Finalmodel + "\\model\\solver.prototxt", "resnet-18_pretrained.caffemodel", false);
            MessageBox.Show("分类完成");

        }

        #region  进度条初始化信息  2019-10-23 进度滚动显示
        public static event ShowLoadMsg LoadingMsg;
        public static bool frmLoadingOpen = false;
        // 打开进度条界面
        private void Loading()
        {
            FormProgress frmloading;
            frmloading = new FormProgress();
            frmloading.ShowDialog();
        }
        #endregion
        int max_batches;
        private void VisionWork1()
        {
            //缺陷剪切进度条

            while (true)
            {
                LoadingMsg("正在剪切", Seeney_GetProgress());

                Thread.Sleep(300);

            }




        }
        private void Crack_CallToChildThread()
        {
            foreach (FileInfo i in json)
            {
                StreamReader reader = new StreamReader(i.FullName);
                Shapes readshapes = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());
                reader.Dispose();
                strlabel = strlabel + i.Name.Split('.')[0] + ".bmp " + readshapes.shape.Count + " ";
                foreach (var j in readshapes.shape)
                {
                    strlabel = strlabel + j.cracktype + " " + j.X1 + " " + j.Y1 + " " + j.X2 + " " + j.Y2 + " " + j.X3 + " " + j.Y3 + " " + j.X4 + " " + j.Y4 + " ";
                }
                strlabel = strlabel + "\r\n";


            }
            using (StreamWriter writer = new StreamWriter( field.NowproPath + "\\image\\LabelInfos.txt"))
            {
                writer.Write(strlabel);
            }

            Directory.CreateDirectory( field.NowproPath + "\\train");

            // MessageBox.Show("开始剪切");
            //模型加载状态以滚动条方式显示

            Thread th = new Thread(new ThreadStart(Loading));
            th.Start();

            while (!frmLoadingOpen)
            {
                Thread.Sleep(10);
            }
            Thread mWorkThread1 = new Thread(new ThreadStart(VisionWork1));
            mWorkThread1.Priority = ThreadPriority.AboveNormal;
            mWorkThread1.IsBackground = true;
            mWorkThread1.Start();
            Seeney_GetYOLOv3LabeltoDefectsImage( field.NowproPath + "\\image\\",  field.NowproPath + "\\train");
            Seeney_GetTrainValLists( field.NowproPath + "\\train");
            Thread.Sleep(100);
            mWorkThread1.Abort();
            LoadingMsg("正在剪切", 100);
            th.Abort();
            //  MessageBox.Show("剪切成功");

            // MessageBox.Show("剪切完成");

            Directory.CreateDirectory( field.NowproPath + "\\backup");
            // Directory.CreateDirectory( field.NowproPath + "\\model");

            /*
             *  //Crack
        int QueTypescount = 1;
        int batch = 64;
        int subdivisions = 64;
        string crackwidth = "640";
        string crackheight = "640";
        string crackangle = "0";
        string saturation = "1.5";
        string exposure = "1.5";
        string hue = "1";
        string learningrate = "0.001";
        long timebeishu = 4;
        */


            //查询缺陷个数
            //写crack.data
            // field.NowproPath + "\\model_" + field.Modelcreatetime.ToString("yyyyMMddHHmmss")
            StreamWriter crackdata = new StreamWriter(field.NowproPath + "\\" + field.Finalmodel + "\\model\\crack.data");
            string strcrackdata = "classes= " + crackConfig.crackTypes + "\r\ntrain  = " +  field.NowproPath + "\\traintrain_crack.txt\r\nvalid  = " +  field.NowproPath + "\\trainval_crack.txt\r\nnames = crack.names\r\nbackup = " +  field.NowproPath + "\\backup\\";
            crackdata.Write(strcrackdata);
            crackdata.Close();
            int QueTypescount = Convert.ToInt32(crackConfig.crackTypes);
            int batch = Convert.ToInt32(crackConfig.batch);

            int filters = QueTypescount * 3 + 15;

            int hangcount = requestMethod( field.NowproPath + "\\traintrain_crack.txt");
            int burn_in = hangcount / batch * (int)crackConfig.ShiJianBeiShu;
            max_batches = burn_in * 10;
            int step1 = burn_in * 5;
            int step2 = burn_in * 8;
            //写crack.cfg          
            // File.Copy("crack.cfg",  field.NowproPath + "\\model_" + field.Modelcreatetime.ToString("yyyyMMddHHmmss") + "\\crack.cfg");
            StreamWriter crackcfg = new StreamWriter("crack.cfg");
            string strcrackcfg = "[net]\r\n" +
"# Testing\r\n" +
"#batch=1\r\n" +
"#subdivisions=" + crackConfig.subdivisions + "\r\n" +
"# Training\r\n" +
"batch =" + batch + "\r\n" +
"subdivisions = " + crackConfig.subdivisions + "\r\n" +
"width = " + crackConfig.width + "\r\n" +
"height = " + crackConfig.height + "\r\n" +
"channels = 3\r\n" +
"momentum = 0.9\r\n" +
"decay = 0.0005\r\n" +
"angle = " + crackConfig.angle + "\r\n" +
"saturation = " + crackConfig.saturation + "\r\n" +
"exposure = " + crackConfig.exposure + "\r\n" +
"hue = ." + crackConfig.hue + "\r\n" +
"\r\n" +
"learning_rate = " + crackConfig.learning_rate + "\r\n" +
"burn_in = " + burn_in + "\r\n" +
"max_batches = " + max_batches + "\r\n" +
"policy = steps\r\n" +
"steps =" + step1 + "," + step2 + "\r\n" +
"scales = .1,.1\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 16\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[maxpool]\r\n" +
"size = 2\r\n" +
"stride = 2\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 32\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[maxpool]\r\n" +
"size = 2\r\n" +
"stride = 2\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 64\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[maxpool]\r\n" +
"size = 2\r\n" +
"stride = 2\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 128\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[maxpool]\r\n" +
"size = 2\r\n" +
"stride = 2\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 256\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[maxpool]\r\n" +
"size = 2\r\n" +
"stride = 2\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 512\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[maxpool]\r\n" +
"size = 2\r\n" +
"stride = 1\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 1024\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"###########\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 256\r\n" +
"size = 1\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 512\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"size = 1\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"filters = " + filters + "\r\n" +
"activation = linear\r\n" +
"\r\n" +
"\r\n" +
"\r\n" +
"[yolo]\r\n" +
"mask = 3,4,5\r\n" +
"anchors = 10,14,  23,27,  37,58,  81,82,  135,169,  344,319\r\n" +
"classes = " + QueTypescount + "\r\n" +
"num = 6\r\n" +
"jitter = .3\r\n" +
"ignore_thresh = .7\r\n" +
"truth_thresh = 1\r\n" +
"random = 1\r\n" +
"\r\n" +
"[route]\r\n" +
"layers = -4\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 128\r\n" +
"size = 1\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[upsample]\r\n" +
"stride = 2\r\n" +
"\r\n" +
"[route]\r\n" +
"layers = -1, 8\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"batch_normalize = 1\r\n" +
"filters = 256\r\n" +
"size = 3\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"activation = leaky\r\n" +
"\r\n" +
"[convolutional]\r\n" +
"size = 1\r\n" +
"stride = 1\r\n" +
"pad = 1\r\n" +
"filters = " + filters + "\r\n" +
"activation = linear\r\n" +
"\r\n" +
"[yolo]\r\n" +
"mask = 0,1,2\r\n" +
"anchors = 10,14,  23,27,  37,58,  81,82,  135,169,  344,319\r\n" +
"classes = " + QueTypescount + "\r\n" +
"num = 6\r\n" +
"jitter = .3\r\n" +
"ignore_thresh = .7\r\n" +
"truth_thresh = 1\r\n" +
"random = 1\r\n";

            /*
              //2019-12-13 增加训练状态变量的输出和控制训练停止的状态量
        public struct Train_Status
        {
            public float avg_loss;             //训练实时当前平均loss值
            public float max_img_loss;         //建议绘制Loss曲线轴的显示图像最大值
            public float average_precision;    //训练实时当前识别率数值
            public int max_batches;            //建议绘制曲线叠代次数轴的最大值
            public int stop_flag;              //初始化时为0，当需要中断训练时，该变量赋值1
        };
             
             */
            crackcfg.Write(strcrackcfg);
            crackcfg.Close();

            canvas.Left = 0;
            canvas.Top = 0;
            canvas.Width = 1430;
            canvas.Height = 840;
            canvas.Visible = false;





            timer1.Start();
            canstop = true;
            Seeney_Train(field.NowproPath + "\\" + field.Finalmodel + "\\model\\crack.data", "crack.cfg", "pretrain_tiny.15", false);
            timer1.Stop();

            FileInfo crackcfgfile = new FileInfo("crack.cfg");
            crackcfgfile.CopyTo(field.NowproPath + "\\" + field.Finalmodel + "\\model\\crack.cfg");
            FileInfo crackweightfile = new FileInfo(field.NowproPath + "\\" + field.Finalmodel + "\\backup\\crack_final.weights");
            crackweightfile.CopyTo(field.NowproPath + "\\" + field.Finalmodel + "\\model\\crack.weights");
            //  using (StreamWriter writer = new StreamWriter( field.NowproPath + "\\dst.txt"))
            // {
            //     writer.Write(txt);
            // }


        }

        public void SaveConfig(int finalmodel)
        {
            if (LAB_ProType.Text == "弱像素分割项目")
            {
                crackConfig.crackTypes = NUD_CTypeCount.DSkinValue;
                crackConfig.batch = TBX_Batch.Text;
                crackConfig.subdivisions = TBX_Sub.Text;
                crackConfig.width = TBX_Width.Text;
                crackConfig.height = TBX_Height.Text;
                crackConfig.angle = TBX_Angle.Text;
                crackConfig.saturation = TBX_Saturat.Text;
                crackConfig.exposure = TBX_Expos.Text;
                crackConfig.hue = TBX_Hue.Text;
                crackConfig.learning_rate = TBX_LearRat.Text;
                crackConfig.ShiJianBeiShu = NUD_TimeMul.DSkinValue;
                using (StreamWriter writer = new StreamWriter( field.NowproPath + "\\"+finalmodel+"\\config.json"))
                {
                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(crackConfig, Newtonsoft.Json.Formatting.Indented));
                }
            }
            else if (LAB_ProType.Text == "大分类项目")
            {
                sortConfig.angle = CMB_PreAngle.Text;
                sortConfig.batch_size1 = CMB_Batch1.Text;
                sortConfig.batch_size2 = CMB_Batch2.Text;
                sortConfig.new_width = CMB_Width.Text;
                LAB_height.Text = sortConfig.new_width;
                sortConfig.new_height = LAB_height.Text;
                sortConfig.max_iter = CMB_Max_Iter.Text;
                using (StreamWriter writer = new StreamWriter( field.NowproPath + "\\" + finalmodel + "\\config.json"))
                {
                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(sortConfig, Newtonsoft.Json.Formatting.Indented));
                }
            }
        }

        private void BTN_NewPro_Click(object sender, EventArgs e)
        {

            SaveConfig(field.Finalmodel);
            //三清
            DrawViewClear();
            g.Clear(Color.White);
            g.Dispose();
            checkedListBox1.Items.Clear();

            NewProForm newProForm = new NewProForm();
            newProForm.eventProShow += new NewProForm.delegatProshow(newproshow);
            newProForm.ShowDialog();
        }

        private void DrawViewClear()
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();
            Panel_Kuang.Region = kuangregion;
            Graphics g1 = pictureBox1.CreateGraphics();
            g1.Clear(Color.White);
            g1.Dispose();
            Graphics g2 = pictureBox2.CreateGraphics();
            g2.Clear(Color.White);
            g2.Dispose();
            Graphics g3 = pictureBox3.CreateGraphics();
            g3.Clear(Color.White);
            g3.Dispose();
            Graphics g4 = pictureBox4.CreateGraphics();
            g4.Clear(Color.White);
            g4.Dispose();
            Graphics g5 = pictureBox5.CreateGraphics();
            g5.Clear(Color.White);
            g5.Dispose();
            Graphics g6 = pictureBox6.CreateGraphics();
            g6.Clear(Color.White);
            g6.Dispose();
            Graphics g7 = pictureBox7.CreateGraphics();
            g7.Clear(Color.White);
            g7.Dispose();
            Graphics g8 = pictureBox8.CreateGraphics();
            g8.Clear(Color.White);
            g8.Dispose();
            Graphics g9 = pictureBox9.CreateGraphics();
            g9.Clear(Color.White);
            g9.Dispose();
            Graphics g10 = pictureBox10.CreateGraphics();
            g10.Clear(Color.White);
            g10.Dispose();
            Graphics g11 = pictureBox11.CreateGraphics();
            g11.Clear(Color.White);
            g11.Dispose();
            Graphics g12 = pictureBox12.CreateGraphics();
            g12.Clear(Color.White);
            g12.Dispose();
            Graphics g13 = pictureBox13.CreateGraphics();
            g13.Clear(Color.White);
            g13.Dispose();
            Graphics g14 = pictureBox14.CreateGraphics();
            g14.Clear(Color.White);
            g14.Dispose();

        }

        private void newproshow()
        {
            LAB_ProName.Text = prodb.QueryPromesg(field.NowProType, field.NowProCreateTime).proname;
            

            NUD_CTypeCount.DSkinValue = 1;
            TBX_Batch.Text = "64";
            TBX_Sub.Text = "32";
            TBX_Width.Text = "640";

            TBX_Height.Text = "540";
            TBX_Angle.Text = "0";
            TBX_Saturat.Text = "1.5";
            TBX_Expos.Text = "1.5";
            TBX_Hue.Text = "1";
            TBX_LearRat.Text = "0.001";
            NUD_TimeMul.DSkinValue = 4;
            CMB_PreAngle.Text = "90";
            CMB_Batch1.Text = "64";
            CMB_Batch2.Text = "32";
            CMB_Width.Text = "224";
            LAB_height.Text = "224";
            CMB_Max_Iter.Text = "600";

            if (field.NowProType == 0)
            {
                LAB_ProType.Text = "弱像素分割项目";

                CBX_NG.Visible = false;
                CBX_OK.Visible = false;
                LAB_CanDrawTips.Visible = true;
                TOG_CanLabel.Visible = true;
                dSkinTabControl1.SelectedIndex = 0;

            }
            else if (field.NowProType == 1)
            {
                LAB_ProType.Text = "大分类项目";
                CBX_NG.Visible = true;
                CBX_OK.Visible = true;
                LAB_CanDrawTips.Visible = false;
                TOG_CanLabel.Visible = false;
                dSkinTabControl1.SelectedIndex = 1;

            }
           


        }

        string[] paistr = new string[14];

        private void BTN_ImpPro_Click(object sender, EventArgs e)
        {
            SaveConfig(field.Finalmodel);
            OpenFileDialog projson = new OpenFileDialog();
            projson.Filter = "|project.json";
            if (projson.ShowDialog() == DialogResult.OK)
            {
                OneProject onproject = new OneProject();
                using (StreamReader reader = new StreamReader(projson.FileName))
                {
                    onproject = Newtonsoft.Json.JsonConvert.DeserializeObject<OneProject>(reader.ReadToEnd());
                }

                 field.NowProType = onproject.protype;
                 field.NowProCreateTime = onproject.createtime;
                 field.NowproPath = prodb.QueryPromesg(field.NowProType, field.NowProCreateTime).propath;
                 field.Finalmodel = onproject.totalmodelnum;
                DirectoryInfo imagepath = new DirectoryInfo(field.NowproPath + "\\"+field.Finalmodel+"\\image");
                if (onproject.protype == 0)
                {
                    CBX_NG.Visible = false;
                    CBX_OK.Visible = false;
                    LAB_CanDrawTips.Visible = true;
                    TOG_CanLabel.Visible = true;
                    dSkinTabControl1.SelectedIndex = 0;
                    LAB_ProType.Text = "弱像素分割项目";
                    CansellistCag = false;
                    using (StreamReader reader = new StreamReader(field.NowproPath + "\\" + field.Finalmodel + "\\config.json"))
                    {
                        crackConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<CrackConfig>(reader.ReadToEnd());
                    }
                    NUD_CTypeCount.DSkinValue = crackConfig.crackTypes;
                    TBX_Batch.Text = crackConfig.batch;
                    TBX_Sub.Text = crackConfig.subdivisions;
                    TBX_Width.Text = crackConfig.width;
                    TBX_Height.Text = crackConfig.height;
                    TBX_Angle.Text = crackConfig.angle;
                    TBX_Saturat.Text = crackConfig.saturation;
                    TBX_Expos.Text = crackConfig.exposure;
                    TBX_LearRat.Text = crackConfig.learning_rate;
                    NUD_TimeMul.DSkinValue = crackConfig.ShiJianBeiShu;




                    FileInfo[] cbmpfiles = imagepath.GetFiles("*.bmp");
                    int picnum = cbmpfiles.Length;
                    if (picnum == 0)
                    {

                    }
                    else
                    {
                        if (picnum % 14 == 0)
                        {
                            totalpage = picnum / 14;
                        }
                        else
                        {
                            totalpage = picnum / 14 + 1;
                        }
                        picsAdresses.Clear();
                        //int count = 0;

                        if (totalpage > 1)
                        {

                            for (int i = 0; i < totalpage - 1; i++)
                            {
                                paistr = new string[14];
                                paistr[0] = cbmpfiles[14 * i].FullName;
                                paistr[1] = cbmpfiles[14 * i + 1].FullName;
                                paistr[2] = cbmpfiles[14 * i + 2].FullName;
                                paistr[3] = cbmpfiles[14 * i + 3].FullName;
                                paistr[4] = cbmpfiles[14 * i + 4].FullName;
                                paistr[5] = cbmpfiles[14 * i + 5].FullName;
                                paistr[6] = cbmpfiles[14 * i + 6].FullName;
                                paistr[7] = cbmpfiles[14 * i + 7].FullName;
                                paistr[8] = cbmpfiles[14 * i + 8].FullName;
                                paistr[9] = cbmpfiles[14 * i + 9].FullName;
                                paistr[10] = cbmpfiles[14 * i + 10].FullName;
                                paistr[11] = cbmpfiles[14 * i + 11].FullName;
                                paistr[12] = cbmpfiles[14 * i + 12].FullName;
                                paistr[13] = cbmpfiles[14 * i + 13].FullName;
                                picsAdresses.Add(paistr);
                            }
                            paistr = new string[14];
                            for (int i = 0; i < 14; i++)
                            {

                                if ((totalpage - 1) * 14 + i < picnum)
                                {
                                    paistr[i] = cbmpfiles[(totalpage - 1) * 14 + i].FullName;
                                }
                                else
                                {
                                    paistr[i] = "";
                                }
                            }
                            picsAdresses.Add(paistr);
                        }
                        else if (totalpage == 1)
                        {
                            paistr = new string[14];
                            for (int i = 0; i < 14; i++)
                            {
                                if (i < picnum)
                                {
                                    paistr[i] = cbmpfiles[i].FullName;
                                }
                                else
                                {
                                    paistr[i] = "";
                                }
                            }
                            picsAdresses.Add(paistr);

                        }
                        else if (totalpage == 0)
                        {

                        }

                        DrawView();

                        kuangregion = new Region();
                        kuangregion.MakeEmpty();

                        kuangregion.Union(GetWaiKuang(1));
                        kuangregion.Xor(GetNeiKuang(1));

                        Panel_Kuang.Region = kuangregion;


                        canTextEvent = false;
                        TBX_NowPageNum.Text = "1";
                        canTextEvent = true;
                        LAB_TolPage.Text = "共" + ((cbmpfiles.Length - 1) / 14 + 1) + "页";

                    }
                    checkedListBox1.Items.Clear();
                    foreach (var i in cbmpfiles)
                    {
                        if (File.Exists(i.FullName.Split('.')[0] + ".json"))
                        {
                            checkedListBox1.Items.Add(i.Name, true);
                        }
                        else
                        {
                            checkedListBox1.Items.Add(i.Name, false);
                        }
                    }
                    if (checkedListBox1.Items.Count != 0)
                    {
                        candraw = true;
                        selectindex = 0;
                        checkedListBox1.SelectedItem = checkedListBox1.Items[selectindex];



                        oldindex = selectindex;
                        selecturl = field.NowproPath + "\\"+field.Finalmodel+"\\image\\" + checkedListBox1.SelectedItem;
                        selectimage = Image.FromFile(selecturl);
                        canvas.Left = ImageLocation(selectimage, Panel_mid).Left;
                        canvas.Top = ImageLocation(selectimage, Panel_mid).Top;
                        canvas.Width = ImageLocation(selectimage, Panel_mid).Width;
                        canvas.Height = ImageLocation(selectimage, Panel_mid).Height;
                        // canvas.Image = selectimage;
                        g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        selectjson = field.NowproPath + "\\" + field.Finalmodel + "\\image\\" + checkedListBox1.SelectedItem.ToString().Split('.')[0] + ".json";
                        if (File.Exists(selectjson))
                        {
                            using (StreamReader reader = new StreamReader(selectjson))
                            {
                                shapes = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                            }
                            // labelregion.MakeEmpty();
                            foreach (var i in shapes.shape)
                            {
                                //  labelregion.Union()
                                g.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);
                            }
                        }
                        else
                        {

                        }
                        //  lastone.Enabled = true;
                        //  nextone.Enabled = true;
                        // save.Enabled = true;
                        //  cancel.Enabled = true;
                        //  augm.Enabled = true;


                        LAB_TotalNum.Text = "总" + new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image").GetFiles("*.bmp").Length + "张";
                        LAB_LabelNum.Text = "标记" + new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image").GetFiles("*.json").Length + "张";
                        CansellistCag = true;
                    }


                }
                else if (onproject.protype == 1)
                {
                    LAB_ProType.Text = "大分类项目";
                    candraw = false;
                    CBX_NG.Visible = true;
                    CBX_OK.Visible = true;
                    LAB_CanDrawTips.Visible = false;
                    TOG_CanLabel.Visible = false;
                    dSkinTabControl1.SelectedIndex = 1;
  
                    CansellistCag = false;
                    //ff

                    using (StreamReader reader = new StreamReader(field.NowproPath + "\\" + field.Finalmodel + "\\config.json"))
                    {

                        sortConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<SortConfig>(reader.ReadToEnd());

                        // NUD_CTypeCount.DSkinValue = Convert.ToInt64(Regex.Match(reader.ReadToEnd(), "(?<=classes=\\ )\\d+").Value);

                    }
                    CMB_PreAngle.Text = sortConfig.angle;
                    CMB_Batch1.Text = sortConfig.batch_size1;
                    CMB_Batch2.Text = sortConfig.batch_size2;
                    CMB_Width.Text = sortConfig.new_width;
                    LAB_height.Text = sortConfig.new_height;
                    CMB_Max_Iter.Text = sortConfig.max_iter;


                    //
                    List<FileInfo> SortAllImage = new List<FileInfo>();

                    checkedListBox1.Items.Clear();
                    spath = new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image");
                    FileInfo[] files = spath.GetFiles("*.bmp");

                    foreach (var i in files)
                    {
                        SortAllImage.Add(i);
                        checkedListBox1.Items.Add(i.Name, CheckState.Unchecked);
                    }
                    npath = new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image\\NormalImages");
                    FileInfo[] nfiles = npath.GetFiles("*.bmp");
                    foreach (var i in nfiles)
                    {
                        SortAllImage.Add(i);
                        checkedListBox1.Items.Add(i.Name, CheckState.Checked);
                    }
                    dpath = new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\DefectImages");
                    FileInfo[] dfiles = dpath.GetFiles("*.bmp");
                    foreach (var i in dfiles)
                    {
                        SortAllImage.Add(i);
                        checkedListBox1.Items.Add(i.Name, CheckState.Indeterminate);
                    }
                    int picnum = SortAllImage.Count;
                    if (picnum == 0)
                    {

                    }
                    else
                    {
                        if (picnum % 14 == 0)
                        {
                            totalpage = picnum / 14;
                        }
                        else
                        {
                            totalpage = picnum / 14 + 1;
                        }
                        picsAdresses.Clear();
                        //int count = 0;

                        if (totalpage > 1)
                        {

                            for (int i = 0; i < totalpage - 1; i++)
                            {
                                paistr = new string[14];
                                paistr[0] = SortAllImage[14 * i].FullName;
                                paistr[1] = SortAllImage[14 * i + 1].FullName;
                                paistr[2] = SortAllImage[14 * i + 2].FullName;
                                paistr[3] = SortAllImage[14 * i + 3].FullName;
                                paistr[4] = SortAllImage[14 * i + 4].FullName;
                                paistr[5] = SortAllImage[14 * i + 5].FullName;
                                paistr[6] = SortAllImage[14 * i + 6].FullName;
                                paistr[7] = SortAllImage[14 * i + 7].FullName;
                                paistr[8] = SortAllImage[14 * i + 8].FullName;
                                paistr[9] = SortAllImage[14 * i + 9].FullName;
                                paistr[10] = SortAllImage[14 * i + 10].FullName;
                                paistr[11] = SortAllImage[14 * i + 11].FullName;
                                paistr[12] = SortAllImage[14 * i + 12].FullName;
                                paistr[13] = SortAllImage[14 * i + 13].FullName;
                                picsAdresses.Add(paistr);
                            }
                            paistr = new string[14];
                            for (int i = 0; i < 14; i++)
                            {

                                if ((totalpage - 1) * 14 + i < picnum)
                                {
                                    paistr[i] = SortAllImage[(totalpage - 1) * 14 + i].FullName;
                                }
                                else
                                {
                                    paistr[i] = "";
                                }
                            }
                            picsAdresses.Add(paistr);
                        }
                        else if (totalpage == 1)
                        {
                            paistr = new string[14];
                            for (int i = 0; i < 14; i++)
                            {
                                if (i < picnum)
                                {
                                    paistr[i] = SortAllImage[i].FullName;
                                }
                                else
                                {
                                    paistr[i] = "";
                                }
                            }
                            picsAdresses.Add(paistr);

                        }
                        else if (totalpage == 0)
                        {

                        }




                        DrawViewSort();

                        kuangregion = new Region();
                        kuangregion.MakeEmpty();

                        kuangregion.Union(GetWaiKuang(1));
                        kuangregion.Xor(GetNeiKuang(1));

                        Panel_Kuang.Region = kuangregion;


                        canTextEvent = false;
                        TBX_NowPageNum.Text = "1";
                        canTextEvent = true;
                        LAB_TolPage.Text = "共" + ((SortAllImage.Count - 1) / 14 + 1) + "页";

                    }





                    //画第一张
                    if (checkedListBox1.Items.Count != 0)
                    {
                        //  candraw = false;
                        // RADOK_EVENT = false;
                        selectindex = 0;
                        checkedListBox1.SelectedItem = checkedListBox1.Items[selectindex];



                        oldindex = selectindex;
                        if (checkedListBox1.GetItemCheckState(selectindex) == CheckState.Indeterminate)
                        {
                            selecturl = field.NowproPath + "\\" + field.Finalmodel + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem;
                            CBX_NG.Checked = true;
                            CBX_OK.Checked = false;

                            selectimage = Image.FromFile(selecturl);
                            canvas.Left = ImageLocation(selectimage, Panel_mid).Left;
                            canvas.Top = ImageLocation(selectimage, Panel_mid).Top;
                            canvas.Width = ImageLocation(selectimage, Panel_mid).Width;
                            canvas.Height = ImageLocation(selectimage, Panel_mid).Height;
                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
                        }
                        else if (checkedListBox1.GetItemCheckState(selectindex) == CheckState.Checked)
                        {
                            CBX_NG.Checked = false;
                            CBX_OK.Checked = true;
                            selecturl = field.NowproPath + "\\" + field.Finalmodel + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem;
                            selectimage = Image.FromFile(selecturl);
                            canvas.Left = ImageLocation(selectimage, Panel_mid).Left;
                            canvas.Top = ImageLocation(selectimage, Panel_mid).Top;
                            canvas.Width = ImageLocation(selectimage, Panel_mid).Width;
                            canvas.Height = ImageLocation(selectimage, Panel_mid).Height;
                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
                        }
                        else if (checkedListBox1.GetItemCheckState(selectindex) == CheckState.Unchecked)
                        {

                            CBX_NG.Checked = false;
                            CBX_OK.Checked = false;
                            selecturl = field.NowproPath + "\\" + field.Finalmodel + "\\image\\" + checkedListBox1.SelectedItem;
                            selectimage = Image.FromFile(selecturl);
                            canvas.Left = ImageLocation(selectimage, Panel_mid).Left;
                            canvas.Top = ImageLocation(selectimage, Panel_mid).Top;
                            canvas.Width = ImageLocation(selectimage, Panel_mid).Width;
                            canvas.Height = ImageLocation(selectimage, Panel_mid).Height;
                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        }
                        // RADOK_EVENT = true;

                    }

                    LAB_TotalNum.Text = "未标" + new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image").GetFiles("*.bmp").Length + "张";
                    LAB_LabelNum.Text = "良品" + new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张 不良" + new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";

                    CansellistCag = true;

                }





                LAB_ProName.Text = prodb.QueryPromesg(field.NowProType, field.NowProCreateTime).proname;




            }

        }
        DirectoryInfo spath;
        DirectoryInfo npath;
        DirectoryInfo dpath;
        public void DrawViewSort()
        {

            if (picsAdresses[nowpagenum - 1][0] != "")
            {
                Graphics g = pictureBox1.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][0]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][0].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][0].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][0].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox1.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][1] != "")
            {
                Graphics g = pictureBox2.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][1]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][1].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][1].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][1].Contains("\\image"))
                {

                }

                g.Dispose();
            }
            else
            {
                Graphics g = pictureBox2.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][2] != "")
            {
                Graphics g = pictureBox3.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][2]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][2].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][2].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][2].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox3.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][3] != "")
            {
                Graphics g = pictureBox4.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][3]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][3].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][3].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][3].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox4.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][4] != "")
            {
                Graphics g = pictureBox5.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][4]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][4].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][4].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][4].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox5.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][5] != "")
            {
                Graphics g = pictureBox6.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][5]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][5].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][5].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][5].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox6.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][6] != "")
            {
                Graphics g = pictureBox7.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][6]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][6].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][6].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][6].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox7.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][7] != "")
            {
                Graphics g = pictureBox8.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][7]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][7].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][7].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][7].Contains("\\image"))
                {

                }

                g.Dispose();
            }
            else
            {
                Graphics g = pictureBox8.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][8] != "")
            {
                Graphics g = pictureBox9.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][8]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][8].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][8].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][8].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox9.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][9] != "")
            {
                Graphics g = pictureBox10.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][9]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][9].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][9].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][9].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox10.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][10] != "")
            {
                Graphics g = pictureBox11.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][10]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][10].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][10].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][10].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox11.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][11] != "")
            {
                Graphics g = pictureBox12.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][11]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][11].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][11].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][11].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox12.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][12] != "")
            {
                Graphics g = pictureBox13.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][12]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][12].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][12].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][12].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox13.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
            if (picsAdresses[nowpagenum - 1][13] != "")
            {
                Graphics g = pictureBox14.CreateGraphics();
                Image nowimageview = Image.FromFile(picsAdresses[nowpagenum - 1][13]);
                g.DrawImage(nowimageview, 0, 0, 100, 75);
                if (picsAdresses[nowpagenum - 1][13].Contains("\\image\\DefectImages"))
                {
                    g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][13].Contains("\\image\\NormalImages"))
                {
                    g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 5, 5);
                }
                else if (picsAdresses[nowpagenum - 1][13].Contains("\\image"))
                {

                }

                g.Dispose();

            }
            else
            {
                Graphics g = pictureBox14.CreateGraphics();
                g.Clear(Color.White);
                g.Dispose();
            }
        }

        private void BTN_ImpPic_Click(object sender, EventArgs e)
        {
            //导入图片pic listbox viewpics暂不变
            if (LAB_ProType.Text == "弱像素分割项目" || LAB_ProType.Text == "大分类项目")
            {
                CansellistCag = false;
                bool oldisEmpty = false;
                if (field.NowProType == 0)
                {
                    oldisEmpty = new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length == 0;
                }
                else if (field.NowProType == 1)
                {
                    oldisEmpty = new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length == 0 && new DirectoryInfo( field.NowproPath + "\\image\\NormalImages").GetFiles("*.bmp").Length == 0 && new DirectoryInfo( field.NowproPath + "\\image\\DefectImages").GetFiles("*.bmp").Length == 0;
                }

                OpenFileDialog importimages = new OpenFileDialog();
                importimages.Filter = "|*.bmp";
                importimages.Multiselect = true;
                List<FileInfo> impimgsf = new List<FileInfo>();
                int count = 0;
                if (importimages.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;//等待 


                    foreach (var i in importimages.FileNames)
                    {
                        count++;
                        string afterpicname = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + count + ".bmp";
                        new FileInfo(i).CopyTo( field.NowproPath + "\\image\\" + afterpicname);
                        checkedListBox1.Items.Add(afterpicname);
                    }

                    this.Cursor = Cursors.Default;//正常
                }

                if (oldisEmpty && count > 0)
                {
                    //显示第一张
                    candraw = true;
                    selectindex = 0;
                    checkedListBox1.SelectedItem = checkedListBox1.Items[selectindex];



                    oldindex = selectindex;
                    selecturl =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem;
                    selectimage = Image.FromFile(selecturl);
                    canvas.Left = ImageLocation(selectimage, Panel_mid).Left;
                    canvas.Top = ImageLocation(selectimage, Panel_mid).Top;
                    canvas.Width = ImageLocation(selectimage, Panel_mid).Width;
                    canvas.Height = ImageLocation(selectimage, Panel_mid).Height;
                    g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);


                }

                if (field.NowProType == 0)
                {

                    LAB_TotalNum.Text = "总" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                }
                else if (field.NowProType == 1)
                {
                    candraw = false;
                    LAB_TotalNum.Text = "未标" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                }



                CansellistCag = true;
                MessageBox.Show("向" + LAB_ProName.Text + "导入" + count + "张图片");
            }
            else
            {
                MessageBox.Show("请先新建或导入项目");
            }
        }

        private void BTN_OpenProFile_Click(object sender, EventArgs e)
        {
            if (LAB_ProType.Text == "弱像素分割项目" || LAB_ProType.Text == "大分类项目")
            {


                System.Diagnostics.Process.Start("explorer.exe",  field.NowproPath);


            }
            else
            {
                MessageBox.Show("请先新建或导入项目");
            }
        }

        private void BTN_LastOne_Click(object sender, EventArgs e)
        {

            CansellistCag = false;
            if (checkedListBox1.Items.Count != 0)
            {
                if (selectindex == 0)
                {
                    MessageBox.Show("这是第一张");

                }
                else
                {
                    if (LAB_ProType.Text == "弱像素分割项目")
                    {
                        selectjson =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString().Split('.')[0] + ".json";
                        if (shapes.shape.Count != 0)
                        {
                            using (StreamWriter writer = new StreamWriter(selectjson))
                            {
                                writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(shapes));
                            }

                            checkedListBox1.SetItemChecked(oldindex, true);

                        }
                        else
                        {
                            checkedListBox1.SetItemChecked(oldindex, false);
                            if (File.Exists(selectjson))
                            {
                                File.Delete(selectjson);
                            }
                        }
                        shapes.shape.Clear();
                        selectindex--;
                        oldindex = selectindex;
                        checkedListBox1.SelectedItem = checkedListBox1.Items[selectindex];
                        selecturl =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem;
                        selectimage = Image.FromFile(selecturl);
                        // canvas.Left = ImageLocation(selectimage, panel3).Left;
                        // canvas.Top = ImageLocation(selectimage, panel3).Top;
                        // canvas.Width = ImageLocation(selectimage, panel3).Width;
                        // canvas.Height = ImageLocation(selectimage, panel3).Height;
                        g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        selectjson =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString().Split('.')[0] + ".json";
                        if (File.Exists(selectjson))
                        {
                            using (StreamReader reader = new StreamReader(selectjson))
                            {
                                shapes = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                            }

                            foreach (var i in shapes.shape)
                            {
                                g.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);
                            }
                        }
                        else
                        {

                        }

                        nowpagenum = checkedListBox1.SelectedIndex / 14 + 1;
                        nowboxnum = checkedListBox1.SelectedIndex - (nowpagenum - 1) * 14 + 1;
                        DrawView();
                        kuangregion = new Region();
                        kuangregion.MakeEmpty();

                        kuangregion.Union(GetWaiKuang(nowboxnum));
                        kuangregion.Xor(GetNeiKuang(nowboxnum));

                        Panel_Kuang.Region = kuangregion;
                        canTextEvent = false;
                        TBX_NowPageNum.Text = nowpagenum.ToString();
                        canTextEvent = true;
                        LAB_TotalNum.Text = "总" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                        LAB_LabelNum.Text = "标记" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.json").Length + "张";


                    }
                    else if (LAB_ProType.Text == "大分类项目")
                    {

                        selectimage.Dispose();
                        if (CBX_OK.Checked)
                        {
                            if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString());
                            }
                            else if (File.Exists( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString());
                            }

                            checkedListBox1.SetItemCheckState(selectindex, CheckState.Checked);


                        }
                        else if (CBX_OK.Checked)
                        {
                            if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString());
                            }
                            else if (File.Exists( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString());
                            }
                            checkedListBox1.SetItemCheckState(selectindex, CheckState.Indeterminate);
                        }
                        else
                        {
                            //原图没有标记

                        }




                        selectindex--;
                        oldindex = selectindex;
                        checkedListBox1.SelectedItem = checkedListBox1.Items[selectindex];

                        if (File.Exists( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString()))
                        {
                            selecturl =  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString();
                            selectimage = Image.FromFile(selecturl);
                            // canvas.Left = ImageLocation(selectimage, panel3).Left;
                            //  canvas.Top = ImageLocation(selectimage, panel3).Top;
                            //  canvas.Width = ImageLocation(selectimage, panel3).Width;
                            //   canvas.Height = ImageLocation(selectimage, panel3).Height;
                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
                            CBX_OK.Checked = true;
                            CBX_NG.Checked = false;

                        }
                        else if (File.Exists( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString()))
                        {
                            selecturl =  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem;
                            selectimage = Image.FromFile(selecturl);
                            // canvas.Left = ImageLocation(selectimage, panel3).Left;
                            //  canvas.Top = ImageLocation(selectimage, panel3).Top;
                            //  canvas.Width = ImageLocation(selectimage, panel3).Width;
                            //  canvas.Height = ImageLocation(selectimage, panel3).Height;
                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);

                            CBX_NG.Checked = true;


                            CBX_OK.Checked = false;

                        }
                        else if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString()))
                        {
                            selecturl =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem;
                            selectimage = Image.FromFile(selecturl);
                            // canvas.Left = ImageLocation(selectimage, panel3).Left;
                            // canvas.Top = ImageLocation(selectimage, panel3).Top;
                            // canvas.Width = ImageLocation(selectimage, panel3).Width;
                            //canvas.Height = ImageLocation(selectimage, panel3).Height;

                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            CBX_NG.Checked = false;
                            CBX_OK.Checked = false;


                        }

                        nowpagenum = checkedListBox1.SelectedIndex / 14 + 1;
                        nowboxnum = checkedListBox1.SelectedIndex - (nowpagenum - 1) * 14 + 1;
                        DrawViewSort();

                        kuangregion = new Region();
                        kuangregion.MakeEmpty();

                        kuangregion.Union(GetWaiKuang(nowboxnum));
                        kuangregion.Xor(GetNeiKuang(nowboxnum));

                        Panel_Kuang.Region = kuangregion;
                        canTextEvent = false;
                        TBX_NowPageNum.Text = nowpagenum.ToString();
                        canTextEvent = true;



                        LAB_TotalNum.Text = "未标" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                        LAB_LabelNum.Text = "良品" + new DirectoryInfo( field.NowproPath + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张 不良" + new DirectoryInfo( field.NowproPath + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";




                    }



                }
            }

            CansellistCag = true;

        }

        private void BTN_NextOne_Click(object sender, EventArgs e)
        {

            CansellistCag = false;
            if (checkedListBox1.Items.Count != 0)
            {
                if (selectindex == checkedListBox1.Items.Count - 1)
                {
                    MessageBox.Show("这是最后一张");

                }
                else
                {
                    if (LAB_ProType.Text == "弱像素分割项目")
                    {
                        selectjson =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString().Split('.')[0] + ".json";
                        if (shapes.shape.Count != 0)
                        {
                            using (StreamWriter writer = new StreamWriter(selectjson))
                            {
                                writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(shapes));
                            }

                            checkedListBox1.SetItemChecked(oldindex, true);

                        }
                        else
                        {
                            checkedListBox1.SetItemChecked(oldindex, false);
                            if (File.Exists(selectjson))
                            {
                                File.Delete(selectjson);
                            }
                        }
                        shapes.shape.Clear();
                        selectindex++;
                        oldindex = selectindex;
                        checkedListBox1.SelectedItem = checkedListBox1.Items[selectindex];
                        selecturl =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem;
                        selectimage = Image.FromFile(selecturl);
                        // canvas.Left = ImageLocation(selectimage, panel3).Left;
                        // canvas.Top = ImageLocation(selectimage, panel3).Top;
                        // canvas.Width = ImageLocation(selectimage, panel3).Width;
                        // canvas.Height = ImageLocation(selectimage, panel3).Height;
                        g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        selectjson =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString().Split('.')[0] + ".json";
                        if (File.Exists(selectjson))
                        {
                            using (StreamReader reader = new StreamReader(selectjson))
                            {
                                shapes = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());


                            }

                            foreach (var i in shapes.shape)
                            {
                                g.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                                g.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);
                            }
                        }
                        else
                        {

                        }
                        nowpagenum = checkedListBox1.SelectedIndex / 14 + 1;
                        nowboxnum = checkedListBox1.SelectedIndex - (nowpagenum - 1) * 14 + 1;
                        DrawView();
                        kuangregion = new Region();
                        kuangregion.MakeEmpty();

                        kuangregion.Union(GetWaiKuang(nowboxnum));
                        kuangregion.Xor(GetNeiKuang(nowboxnum));

                        Panel_Kuang.Region = kuangregion;
                        canTextEvent = false;
                        TBX_NowPageNum.Text = nowpagenum.ToString();
                        canTextEvent = true;
                        LAB_TotalNum.Text = "总" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                        LAB_LabelNum.Text = "标记" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.json").Length + "张";
                    }
                    else if (LAB_ProType.Text == "大分类项目")
                    {

                        selectimage.Dispose();
                        if (CBX_OK.Checked)
                        {
                            if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString());
                            }
                            else if (File.Exists( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString());
                            }

                            checkedListBox1.SetItemCheckState(selectindex, CheckState.Checked);


                        }
                        else if (CBX_NG.Checked)
                        {
                            if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString());
                            }
                            else if (File.Exists( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString()))
                            {
                                File.Move( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString(),  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString());
                            }
                            checkedListBox1.SetItemCheckState(selectindex, CheckState.Indeterminate);
                        }
                        else
                        {
                            //原图没有标记

                        }




                        selectindex++;
                        oldindex = selectindex;
                        checkedListBox1.SelectedItem = checkedListBox1.Items[selectindex];

                        if (File.Exists( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString()))
                        {
                            selecturl =  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString();
                            selectimage = Image.FromFile(selecturl);
                            // canvas.Left = ImageLocation(selectimage, panel3).Left;
                            //  canvas.Top = ImageLocation(selectimage, panel3).Top;
                            //  canvas.Width = ImageLocation(selectimage, panel3).Width;
                            //   canvas.Height = ImageLocation(selectimage, panel3).Height;
                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
                            CBX_OK.Checked = true;
                            CBX_NG.Checked = false;

                        }
                        else if (File.Exists( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString()))
                        {
                            selecturl =  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem;
                            selectimage = Image.FromFile(selecturl);
                            // canvas.Left = ImageLocation(selectimage, panel3).Left;
                            //  canvas.Top = ImageLocation(selectimage, panel3).Top;
                            //  canvas.Width = ImageLocation(selectimage, panel3).Width;
                            //  canvas.Height = ImageLocation(selectimage, panel3).Height;
                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
                            CBX_NG.Checked = true;
                            CBX_OK.Checked = false;

                        }
                        else if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString()))
                        {
                            selecturl =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem;
                            selectimage = Image.FromFile(selecturl);
                            // canvas.Left = ImageLocation(selectimage, panel3).Left;
                            // canvas.Top = ImageLocation(selectimage, panel3).Top;
                            // canvas.Width = ImageLocation(selectimage, panel3).Width;
                            //canvas.Height = ImageLocation(selectimage, panel3).Height;

                            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                            CBX_NG.Checked = false;
                            CBX_OK.Checked = false;



                        }
                        nowpagenum = checkedListBox1.SelectedIndex / 14 + 1;
                        nowboxnum = checkedListBox1.SelectedIndex - (nowpagenum - 1) * 14 + 1;
                        DrawViewSort();

                        kuangregion = new Region();
                        kuangregion.MakeEmpty();

                        kuangregion.Union(GetWaiKuang(nowboxnum));
                        kuangregion.Xor(GetNeiKuang(nowboxnum));

                        Panel_Kuang.Region = kuangregion;
                        canTextEvent = false;
                        TBX_NowPageNum.Text = nowpagenum.ToString();
                        canTextEvent = true;



                        LAB_TotalNum.Text = "未标" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                        LAB_LabelNum.Text = "良品" + new DirectoryInfo( field.NowproPath + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张 不良" + new DirectoryInfo( field.NowproPath + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";



                    }



                }
            }

            CansellistCag = true;

        }

        private void BTN_Save_Click(object sender, EventArgs e)
        {

            if (LAB_ProType.Text == "弱像素分割项目")
            {
                if (shapes.shape.Count != 0)
                {
                    selectjson =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString().Split('.')[0] + ".json";
                    using (StreamWriter writer = new StreamWriter(selectjson))
                    {
                        writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(shapes));
                    }
                    checkedListBox1.SetItemChecked(oldindex, true);





                }
                else
                {
                    checkedListBox1.SetItemChecked(oldindex, false);
                    if (File.Exists(selectjson))
                    {
                        File.Delete(selectjson);
                    }
                }
                LAB_TotalNum.Text = "总" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                LAB_LabelNum.Text = "标记" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.json").Length + "张";
                MessageBox.Show("已保存");
            }



        }

        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            if (LAB_ProType.Text == "弱像素分割项目")
            {
                if (shapes.shape.Count != 0)
                {
                    shapes.shape.RemoveAt(shapes.shape.Count - 1);

                    g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                    foreach (var i in shapes.shape)
                    {
                        g.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                        g.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                        g.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                        g.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);
                    }
                }
                LAB_TotalNum.Text = "总" + new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image").GetFiles("*.bmp").Length + "张";
                LAB_LabelNum.Text = "标记" + new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image").GetFiles("*.json").Length + "张";
            }
        }

        OneModel onemodel = new OneModel();
        private void BTN_Train_Click(object sender, EventArgs e)
        {
            //models = new List<DateTime>();
            //models.Clear();
            onemodel.modelcreatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            field.Finalmodel++;
            //field. field.NowproPath =  field.NowproPath;
            //field. = DateTime.Now;
            /*
            if (field.NowProType==0||field.NowProType ==1)
            {
                string strmodels;
                using (StreamReader reader = new StreamReader(field.NowproPath + "\\" + field.Finalmodel + "\\models.json"))
                {
                    strmodels = reader.ReadToEnd();
                }
                models = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(strmodels);
                if (models == null)
                {
                    models = new List<DateTime>();
                }
            }
           
    */




            CheckForIllegalCrossThreadCalls = false;
             
            BTN_NewPro.Enabled = false;
            BTN_NextOne.Enabled = false;

            BTN_Save.Enabled = false;
            BTN_Cancel.Enabled = false;
            BTN_ImpPic.Enabled = false;
            BTN_ImpPro.Enabled = false;
            BTN_LastOne.Enabled = false;
            BTN_ProManage.Enabled = false;
            BTN_ModelManage.Enabled = false;
            if (checkedListBox1.Items.Count > 0)
            {
                //this.Cursor = Cursors.WaitCursor()
                // augm.Enabled = false;
                // path = dataGridView1[1, dataGridView1.CurrentCellAddress.Y].Value.ToString();






                if (LAB_ProType.Text == "弱像素分割项目")
                {
                    IsCrack = true;
                    DialogResult jichengresult = MessageBox.Show("是否继承", "弱像素分割继承训练", MessageBoxButtons.YesNo);
                    if (jichengresult == DialogResult.Yes)
                    {
                        field.IsJiCheng = true;
                        DialogResult result1 = MessageBox.Show("确认开始", "即将开始剪切训练", MessageBoxButtons.OKCancel);
                        if (result1 == DialogResult.OK)
                        {
                            // BTN_StopTrain.Visible = true;
                            crackConfig.crackTypes = NUD_CTypeCount.DSkinValue;
                            crackConfig.batch = TBX_Batch.Text;
                            crackConfig.subdivisions = TBX_Sub.Text;
                            crackConfig.width = TBX_Width.Text;
                            crackConfig.height = TBX_Height.Text;
                            crackConfig.angle = TBX_Angle.Text;
                            crackConfig.saturation = TBX_Saturat.Text;
                            crackConfig.exposure = TBX_Expos.Text;
                            crackConfig.hue = TBX_Hue.Text;
                            crackConfig.learning_rate = TBX_LearRat.Text;
                            crackConfig.ShiJianBeiShu = NUD_TimeMul.DSkinValue;

                            kuangregion = new Region();
                            kuangregion.MakeEmpty();
                            Panel_Kuang.Region = kuangregion;


                            if (Directory.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\train"))
                            {
                                Directory.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\train", true);
                            }
                            if (Directory.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\backup"))
                            {
                                Directory.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\backup", true);
                            }


                            Directory.CreateDirectory(field.NowproPath + "\\" + field.Finalmodel + "\\model");
                           // models.Add(field.Modelcreatetime);
                          //  using (StreamWriter writer1 = new StreamWriter( field.NowproPath + "\\models.json"))
                           // {
                            //    writer1.Write(Newtonsoft.Json.JsonConvert.SerializeObject(models, Newtonsoft.Json.Formatting.Indented));
                           // }

                            //if (Directory.Exists( field.NowproPath + "\\model"))
                            //{
                            //  Directory.Delete( field.NowproPath + "\\model", true);
                            //}

                            if (File.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\traintrain_crack.txt"))
                            {
                                File.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\traintrain_crack.txt");
                            }
                            if (File.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\trainval_crack.txt"))
                            {
                                File.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\trainval_crack.txt");
                            }
                            DirectoryInfo info = new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image");
                            json = info.GetFiles("*.json");
                            if (json.Length == 0)
                            {
                                MessageBox.Show("无标记图片");
                            }
                            else
                            {
                                using (StreamWriter writer = new StreamWriter(field.NowproPath + "\\" + field.Finalmodel + "\\config.json"))
                                {
                                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(crackConfig, Newtonsoft.Json.Formatting.Indented));
                                }

                                ModelConfig modelConfig1 = new ModelConfig();
                                modelConfig1.Event_megCommit += new ModelConfig.delegate_megCommit(modelConfiged1);
                                //deftypes.Event_CrackTypeshow += new Deftypes.delegate_CrackTypeshow(CrackTypeshow);
                                modelConfig1.ShowDialog();






                            }
                        }
                        else if (result1 == DialogResult.Cancel)
                        {

                        }

                    }
                    else if (jichengresult == DialogResult.No)
                    {
                        field.IsJiCheng = false;
                        DialogResult result2 = MessageBox.Show("确认开始", "即将开始剪切训练", MessageBoxButtons.OKCancel);
                        if (result2 == DialogResult.OK)
                        {
                            // BTN_StopTrain.Visible = true;
                            crackConfig.crackTypes = NUD_CTypeCount.DSkinValue;
                            crackConfig.batch = TBX_Batch.Text;
                            crackConfig.subdivisions = TBX_Sub.Text;
                            crackConfig.width = TBX_Width.Text;
                            crackConfig.height = TBX_Height.Text;
                            crackConfig.angle = TBX_Angle.Text;
                            crackConfig.saturation = TBX_Saturat.Text;
                            crackConfig.exposure = TBX_Expos.Text;
                            crackConfig.hue = TBX_Hue.Text;
                            crackConfig.learning_rate = TBX_LearRat.Text;
                            crackConfig.ShiJianBeiShu = NUD_TimeMul.DSkinValue;

                            kuangregion = new Region();
                            kuangregion.MakeEmpty();
                            Panel_Kuang.Region = kuangregion;


                            if (Directory.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\train"))
                            {
                                Directory.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\train", true);
                            }
                            if (Directory.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\backup"))
                            {
                                Directory.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\backup", true);
                            }


                            Directory.CreateDirectory(field.NowproPath + "\\" + field.Finalmodel + "\\model");
                            //models.Add(field.Modelcreatetime);
                            //using (StreamWriter writer1 = new StreamWriter( field.NowproPath + "\\models.json"))
                            //{
                            //    writer1.Write(Newtonsoft.Json.JsonConvert.SerializeObject(models, Newtonsoft.Json.Formatting.Indented));
                            //}

                            //if (Directory.Exists( field.NowproPath + "\\model"))
                            //{
                            //  Directory.Delete( field.NowproPath + "\\model", true);
                            //}

                            if (File.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\traintrain_crack.txt"))
                            {
                                File.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\traintrain_crack.txt");
                            }
                            if (File.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\trainval_crack.txt"))
                            {
                                File.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\trainval_crack.txt");
                            }
                            DirectoryInfo info = new DirectoryInfo(field.NowproPath + "\\" + field.Finalmodel + "\\image");
                            json = info.GetFiles("*.json");
                            if (json.Length == 0)
                            {
                                MessageBox.Show("无标记图片");
                            }
                            else
                            {
                                using (StreamWriter writer = new StreamWriter(field.NowproPath + "\\" + field.Finalmodel + "\\config.json"))
                                {
                                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(crackConfig, Newtonsoft.Json.Formatting.Indented));
                                }

                                ModelConfig modelConfig = new ModelConfig();
                                modelConfig.Event_megCommit += new ModelConfig.delegate_megCommit(modelConfiged);
                                //deftypes.Event_CrackTypeshow += new Deftypes.delegate_CrackTypeshow(CrackTypeshow);
                                modelConfig.ShowDialog();






                            }
                        }
                        else if (result2 == DialogResult.Cancel)
                        {

                        }
                    }


                }
                else if (LAB_ProType.Text == "大分类项目")
                {
                    IsCrack = false;
                    DialogResult jichengresult = MessageBox.Show("是否继承", "大分类训练", MessageBoxButtons.YesNo);
                    if (jichengresult == DialogResult.Yes)
                    {
                        field.IsJiCheng = true;
                        DialogResult result1 = MessageBox.Show("确认开始", "即将开始剪切训练", MessageBoxButtons.OKCancel);
                        if (result1 == DialogResult.OK)
                        {   
                            sortConfig.angle = CMB_PreAngle.Text;
                            sortConfig.batch_size1 = CMB_Batch1.Text;
                            sortConfig.batch_size2 = CMB_Batch2.Text;
                            sortConfig.new_width = CMB_Width.Text;
                            LAB_height.Text = sortConfig.new_width;
                            sortConfig.new_height = LAB_height.Text;
                            sortConfig.max_iter = CMB_Max_Iter.Text;
                            int preangle = Convert.ToInt32(sortConfig.angle);
                            if (preangle > 0 && preangle < 180)
                            {
                                if (Directory.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\NormalImages"))
                                {
                                    Directory.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\NormalImages", true);
                                }
                                if (Directory.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\DefectImages"))
                                {
                                    Directory.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\DefectImages", true);
                                }
                                if (File.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\train_list.txt"))
                                {
                                    File.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\train_list.txt");
                                }
                                if (File.Exists(field.NowproPath + "\\" + field.Finalmodel + "\\val_list.txt"))
                                {
                                    File.Delete(field.NowproPath + "\\" + field.Finalmodel + "\\val_list.txt");
                                }
                                using (StreamWriter writer = new StreamWriter( field.NowproPath + "\\config.json"))
                                {
                                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(sortConfig, Newtonsoft.Json.Formatting.Indented));
                                }
                                Directory.CreateDirectory(field.NowproPath + "\\" + field.Finalmodel + "\\model");
                                //models.Add(field.Modelcreatetime);
                                //using (StreamWriter writer1 = new StreamWriter( field.NowproPath + "\\models.json"))
                               // {
                               //     writer1.Write(Newtonsoft.Json.JsonConvert.SerializeObject(models, Newtonsoft.Json.Formatting.Indented));
                              //  }
                                ModelConfig modelConfig4 = new ModelConfig();
                                modelConfig4.Event_megCommit += new ModelConfig.delegate_megCommit(modelConfiged4);
                                //deftypes.Event_CrackTypeshow += new Deftypes.delegate_CrackTypeshow(CrackTypeshow);
                                modelConfig4.ShowDialog();



                            }
                            else
                            {
                                MessageBox.Show("角度设置范围应为0~180");
                            }
                        }
                        else if (result1 == DialogResult.Cancel)
                        {

                        }

                    }
                    else if (jichengresult == DialogResult.No)
                    {
                        field.IsJiCheng = false;
                        DialogResult result1 = MessageBox.Show("确认开始", "即将开始剪切训练", MessageBoxButtons.OKCancel);
                        if (result1 == DialogResult.OK)
                        {
                            sortConfig.angle = CMB_PreAngle.Text;
                            sortConfig.batch_size1 = CMB_Batch1.Text;
                            sortConfig.batch_size2 = CMB_Batch2.Text;
                            sortConfig.new_width = CMB_Width.Text;
                            LAB_height.Text = sortConfig.new_width;
                            sortConfig.new_height = LAB_height.Text;
                            sortConfig.max_iter = CMB_Max_Iter.Text;
                            int preangle = Convert.ToInt32(sortConfig.angle);
                            if (preangle > 0 && preangle < 180)
                            {
                                if (Directory.Exists( field.NowproPath + "\\NormalImages"))
                                {
                                    Directory.Delete( field.NowproPath + "\\NormalImages", true);
                                }
                                if (Directory.Exists( field.NowproPath + "\\DefectImages"))
                                {
                                    Directory.Delete( field.NowproPath + "\\DefectImages", true);
                                }
                                if (File.Exists( field.NowproPath + "\\train_list.txt"))
                                {
                                    File.Delete( field.NowproPath + "\\train_list.txt");
                                }
                                if (File.Exists( field.NowproPath + "\\val_list.txt"))
                                {
                                    File.Delete( field.NowproPath + "\\val_list.txt");
                                }
                                using (StreamWriter writer = new StreamWriter( field.NowproPath + "\\config.json"))
                                {
                                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(sortConfig, Newtonsoft.Json.Formatting.Indented));
                                }
                                Directory.CreateDirectory(field.NowproPath + "\\" + field.Finalmodel + "\\model");
                              //  models.Add(field.Modelcreatetime);
                               // using (StreamWriter writer1 = new StreamWriter( field.NowproPath + "\\models.json"))
                               // {
                               //     writer1.Write(Newtonsoft.Json.JsonConvert.SerializeObject(models, Newtonsoft.Json.Formatting.Indented));
                              //  }
                                ModelConfig modelConfig2 = new ModelConfig();
                                modelConfig2.Event_megCommit += new ModelConfig.delegate_megCommit(modelConfiged2);
                                //deftypes.Event_CrackTypeshow += new Deftypes.delegate_CrackTypeshow(CrackTypeshow);
                                modelConfig2.ShowDialog();



                            }
                            else
                            {
                                MessageBox.Show("角度设置范围应为0~180");
                            }


                        }
                        else if (result1 == DialogResult.Cancel)
                        {

                        }
                    }



                }
                else
                {
                    MessageBox.Show("请新建或导入项目");
                    BTN_NewPro.Enabled = true;
                    BTN_NextOne.Enabled = true;

                    BTN_Save.Enabled = true;
                    BTN_Cancel.Enabled = true;
                    BTN_ImpPic.Enabled = true;
                    BTN_ImpPro.Enabled = true;
                    BTN_LastOne.Enabled = true;
                    BTN_ProManage.Enabled = true;
                    BTN_ModelManage.Enabled = true;

                }
            }
            else
            {


                MessageBox.Show("请新建或导入项目");
                BTN_NewPro.Enabled = true;
                BTN_NextOne.Enabled = true;

                BTN_Save.Enabled = true;
                BTN_Cancel.Enabled = true;
                BTN_ImpPic.Enabled = true;
                BTN_ImpPro.Enabled = true;
                BTN_LastOne.Enabled = true;
                BTN_ProManage.Enabled = true;
                BTN_ModelManage.Enabled = true;

            }



            //augm.Enabled = true;



        }

        private void modelConfiged1()
        {
            //缺陷继承
            Thread Crack_childThread = new Thread(new ThreadStart(Crack_CallToChildThread2));
            Crack_childThread.IsBackground = true;
            Crack_childThread.Start();

        }

        private void Crack_CallToChildThread2()
        {
            
        }

        private void modelConfiged4()
        {
            //分类继承
            Thread Sort_childThread = new Thread(new ThreadStart(Sort_CallToChildThread2));
            Sort_childThread.IsBackground = true;
            Sort_childThread.Start();
        }

        private void Sort_CallToChildThread2()
        {
            
        }

        private void modelConfiged2()
        {
            //分类不继承
            Thread Sort_childThread = new Thread(new ThreadStart(Sort_CallToChildThread));
            Sort_childThread.IsBackground = true;
            Sort_childThread.Start();
        }

        private void BTN_StopTrain_Click(object sender, EventArgs e)
        {
            if (canstop)
            {
                Get_Seeney_train_status(1);
            }
            else
            {
                MessageBox.Show("还不能中止");
            }
        }

        private void BTN_ProManage_Click(object sender, EventArgs e)
        {
            // 项目管理
            ManagePros managepros = new ManagePros();
            managepros.ShowDialog();
        }

        private void BTN_ModelManage_Click(object sender, EventArgs e)
        {
            FormModel formModel = new FormModel();
            formModel.ShowDialog();

        }

        bool candraw = false;
        private void CrackTypeshow()
        {
            sipoint.cracktype = (int)field.NowCrackType;
            shapes.shape.Add(sipoint);

        }



        private void canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (candraw)
            {
                if (TOG_CanLabel.DSkinToggled)
                {
                    if (clickcount == 1)
                    {
                        sipoint = new SiPoint();

                        sipoint.X1 = e.X * selectimage.Width / canvas.Width;
                        sipoint.Y1 = e.Y * selectimage.Width / canvas.Width;
                        clickcount = 2;
                    }
                    else if (clickcount == 2)
                    {

                        sipoint.X2 = e.X * selectimage.Width / canvas.Width;
                        sipoint.Y2 = e.Y * selectimage.Width / canvas.Width;
                        clickcount = 3;
                    }
                    else if (clickcount == 3)
                    {

                        sipoint.X3 = e.X * selectimage.Width / canvas.Width;
                        sipoint.Y3 = e.Y * selectimage.Width / canvas.Width;
                        clickcount = 4;
                    }
                    else
                    {
                        sipoint.X4 = e.X * selectimage.Width / canvas.Width;
                        sipoint.Y4 = e.Y * selectimage.Width / canvas.Width;

                        clickcount = 1;

                        Deftypes deftypes = new Deftypes();
                        deftypes.Event_CrackTypeshow += new Deftypes.delegate_CrackTypeshow(CrackTypeshow);
                        deftypes.ShowDialog();
                        //不能画 选label

                    }
                }




            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (candraw)
            {
                if (TOG_CanLabel.DSkinToggled)
                {
                    //可标记控件打开
                    if (clickcount == 2)
                    {
                        Bitmap bitmap = new Bitmap(canvas.Width, canvas.Height);
                        Graphics bitg = Graphics.FromImage(bitmap);
                        bitg.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        foreach (var i in shapes.shape)
                        {
                            bitg.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);


                        }
                        bitg.DrawLine(Pens.Red, sipoint.X1 * canvas.Width / selectimage.Width, sipoint.Y1 * canvas.Width / selectimage.Width, e.X, e.Y);
                        //  Graphics g = pictureBox1.CreateGraphics();
                        g.DrawImage(bitmap, 0, 0);
                        // g.Dispose();
                        bitg.Dispose();
                        bitmap.Dispose();



                    }
                    else if (clickcount == 3)
                    {
                        Bitmap bitmap = new Bitmap(canvas.Width, canvas.Height);
                        Graphics bitg = Graphics.FromImage(bitmap);
                        bitg.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        foreach (var i in shapes.shape)
                        {
                            bitg.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);


                        }
                        bitg.DrawLine(Pens.Red, sipoint.X1 * canvas.Width / selectimage.Width, sipoint.Y1 * canvas.Width / selectimage.Width, sipoint.X2 * canvas.Width / selectimage.Width, sipoint.Y2 * canvas.Width / selectimage.Width);
                        bitg.DrawLine(Pens.Red, sipoint.X2 * canvas.Width / selectimage.Width, sipoint.Y2 * canvas.Width / selectimage.Width, e.X, e.Y);
                        //  Graphics g = pictureBox1.CreateGraphics();
                        g.DrawImage(bitmap, 0, 0);
                        //  g.Dispose();
                        bitg.Dispose();
                        bitmap.Dispose();

                    }
                    else if (clickcount == 4)
                    {
                        Bitmap bitmap = new Bitmap(canvas.Width, canvas.Height);
                        Graphics bitg = Graphics.FromImage(bitmap);
                        bitg.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        foreach (var i in shapes.shape)
                        {
                            bitg.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                            bitg.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);


                        }
                        bitg.DrawLine(Pens.Red, sipoint.X1 * canvas.Width / selectimage.Width, sipoint.Y1 * canvas.Width / selectimage.Width, sipoint.X2 * canvas.Width / selectimage.Width, sipoint.Y2 * canvas.Width / selectimage.Width);
                        bitg.DrawLine(Pens.Red, sipoint.X2 * canvas.Width / selectimage.Width, sipoint.Y2 * canvas.Width / selectimage.Width, sipoint.X3 * canvas.Width / selectimage.Width, sipoint.Y3 * canvas.Width / selectimage.Width);
                        bitg.DrawLine(Pens.Red, sipoint.X3 * canvas.Width / selectimage.Width, sipoint.Y3 * canvas.Width / selectimage.Width, e.X, e.Y);
                        bitg.DrawLine(Pens.Red, sipoint.X1 * canvas.Width / selectimage.Width, sipoint.Y1 * canvas.Width / selectimage.Width, e.X, e.Y);
                        //  Graphics g = pictureBox1.CreateGraphics();
                        g.DrawImage(bitmap, 0, 0);
                        //  g.Dispose();
                        bitg.Dispose();
                        bitmap.Dispose();

                    }
                    else
                    {

                    }

                }



            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CansellistCag)
            {
                if (LAB_ProType.Text == "弱像素分割项目")
                {

                    selectjson =  field.NowproPath + "\\image\\" + checkedListBox1.Items[oldindex].ToString().Split('.')[0] + ".json";
                    if (shapes.shape.Count != 0)
                    {
                        using (StreamWriter writer = new StreamWriter(selectjson))
                        {
                            writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(shapes));
                        }

                        checkedListBox1.SetItemChecked(oldindex, true);

                    }
                    else
                    {
                        if (File.Exists(selectjson))
                        {
                            File.Delete(selectjson);
                        }
                        checkedListBox1.SetItemChecked(oldindex, false);
                    }
                    shapes.shape.Clear();
                    selectindex = checkedListBox1.SelectedIndex;
                    oldindex = selectindex;
                    selecturl =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem;
                    selectimage = Image.FromFile(selecturl);
                    // canvas.Left = ImageLocation(selectimage, panel3).Left;
                    // canvas.Top = ImageLocation(selectimage, panel3).Top;
                    // canvas.Width = ImageLocation(selectimage, panel3).Width;
                    // canvas.Height = ImageLocation(selectimage, panel3).Height;
                    g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                    selectjson =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString().Split('.')[0] + ".json";
                    if (File.Exists(selectjson))
                    {
                        using (StreamReader reader = new StreamReader(selectjson))
                        {
                            shapes = Newtonsoft.Json.JsonConvert.DeserializeObject<Shapes>(reader.ReadToEnd());

                        }

                        foreach (var i in shapes.shape)
                        {
                            g.DrawLine(Pens.Red, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width);
                            g.DrawLine(Pens.Red, i.X2 * canvas.Width / selectimage.Width, i.Y2 * canvas.Width / selectimage.Width, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width);
                            g.DrawLine(Pens.Red, i.X3 * canvas.Width / selectimage.Width, i.Y3 * canvas.Width / selectimage.Width, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width);
                            g.DrawLine(Pens.Red, i.X4 * canvas.Width / selectimage.Width, i.Y4 * canvas.Width / selectimage.Width, i.X1 * canvas.Width / selectimage.Width, i.Y1 * canvas.Width / selectimage.Width);
                        }
                    }
                    else
                    {

                    }
                    nowpagenum = checkedListBox1.SelectedIndex / 14 + 1;
                    nowboxnum = checkedListBox1.SelectedIndex - (nowpagenum - 1) * 14 + 1;
                    DrawView();
                    kuangregion = new Region();
                    kuangregion.MakeEmpty();

                    kuangregion.Union(GetWaiKuang(nowboxnum));
                    kuangregion.Xor(GetNeiKuang(nowboxnum));

                    Panel_Kuang.Region = kuangregion;
                    canTextEvent = false;
                    TBX_NowPageNum.Text = nowpagenum.ToString();
                    canTextEvent = true;

                    LAB_TotalNum.Text = "总" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                    LAB_LabelNum.Text = "标记" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.json").Length + "张";


                }
                else if (LAB_ProType.Text == "大分类项目")
                {

                    selectimage.Dispose();
                    if (CBX_OK.Checked)
                    {
                        if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.Items[oldindex]))
                        {
                            File.Move( field.NowproPath + "\\image\\" + checkedListBox1.Items[oldindex],  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.Items[oldindex]);
                        }
                        else if (File.Exists( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.Items[oldindex]))
                        {
                            File.Move( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.Items[oldindex],  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.Items[oldindex]);
                        }

                        checkedListBox1.SetItemCheckState(oldindex, CheckState.Checked);


                    }
                    else if (CBX_NG.Checked)
                    {
                        if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.Items[oldindex]))
                        {
                            File.Move( field.NowproPath + "\\image\\" + checkedListBox1.Items[oldindex],  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.Items[oldindex]);
                        }
                        else if (File.Exists( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.Items[oldindex]))
                        {
                            File.Move( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.Items[oldindex],  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.Items[oldindex]);
                        }
                        checkedListBox1.SetItemCheckState(oldindex, CheckState.Indeterminate);
                    }




                    selectindex = checkedListBox1.SelectedIndex;
                    oldindex = selectindex;


                    if (File.Exists( field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString()))
                    {
                        selecturl =  field.NowproPath + "\\image\\NormalImages\\" + checkedListBox1.SelectedItem.ToString();
                        selectimage = Image.FromFile(selecturl);
                        // canvas.Left = ImageLocation(selectimage, panel3).Left;
                        // canvas.Top = ImageLocation(selectimage, panel3).Top;
                        // canvas.Width = ImageLocation(selectimage, panel3).Width;
                        // canvas.Height = ImageLocation(selectimage, panel3).Height;
                        g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
                        CBX_OK.Checked = true;
                        CBX_NG.Checked = false;

                    }
                    else if (File.Exists( field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem.ToString()))
                    {
                        selecturl =  field.NowproPath + "\\image\\DefectImages\\" + checkedListBox1.SelectedItem;
                        selectimage = Image.FromFile(selecturl);
                        //  canvas.Left = ImageLocation(selectimage, panel3).Left;
                        // canvas.Top = ImageLocation(selectimage, panel3).Top;
                        //  canvas.Width = ImageLocation(selectimage, panel3).Width;
                        //  canvas.Height = ImageLocation(selectimage, panel3).Height;
                        g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
                        CBX_NG.Checked = true;
                        CBX_OK.Checked = false;

                    }
                    else if (File.Exists( field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem.ToString()))
                    {
                        selecturl =  field.NowproPath + "\\image\\" + checkedListBox1.SelectedItem;
                        selectimage = Image.FromFile(selecturl);
                        // canvas.Left = ImageLocation(selectimage, panel3).Left;
                        // canvas.Top = ImageLocation(selectimage, panel3).Top;
                        //  canvas.Width = ImageLocation(selectimage, panel3).Width;
                        //  canvas.Height = ImageLocation(selectimage, panel3).Height;

                        g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
                        CBX_NG.Checked = false;
                        CBX_OK.Checked = false;


                    }
                    LAB_TotalNum.Text = "未标" + new DirectoryInfo( field.NowproPath + "\\image").GetFiles("*.bmp").Length + "张";
                    LAB_LabelNum.Text = "良品" + new DirectoryInfo( field.NowproPath + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张 不良" + new DirectoryInfo( field.NowproPath + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";

                }


            }


        }

        private void BTN_LastPage_Click(object sender, EventArgs e)
        {
            if (nowpagenum > 1)
            {
                nowpagenum--;
                TBX_NowPageNum.Text = nowpagenum.ToString();
                DrawView();

                checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14;
                kuangregion = new Region();
                kuangregion.MakeEmpty();

                kuangregion.Union(GetWaiKuang(1));
                kuangregion.Xor(GetNeiKuang(1));

                Panel_Kuang.Region = kuangregion;


            }
            else
            {
                MessageBox.Show("已经是第一页");
            }
        }

        private void BTN_NextPage_Click(object sender, EventArgs e)
        {
            if (nowpagenum < totalpage)
            {
                nowpagenum++;
                TBX_NowPageNum.Text = nowpagenum.ToString();
                DrawView();

                checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14;
                kuangregion = new Region();
                kuangregion.MakeEmpty();

                kuangregion.Union(GetWaiKuang(1));
                kuangregion.Xor(GetNeiKuang(1));

                Panel_Kuang.Region = kuangregion;


            }
            else
            {
                MessageBox.Show("已经是最后一页");
            }
        }

        private void CBX_NG_Click(object sender, EventArgs e)
        {
            CBX_OK.Checked = false;
            CBX_NG.Checked = true;
            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
            g.DrawString("不良", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
        }

        private void CBX_OK_Click(object sender, EventArgs e)
        {
            CBX_OK.Checked = true;
            CBX_NG.Checked = false;
            g.DrawImage(selectimage, 0, 0, canvas.Width, canvas.Height);
            g.DrawString("良品", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 50, 50);
        }
        bool canTextEvent = true;
        private void TBX_NowPageNum_TextChanged(object sender, EventArgs e)
        {


            if (canTextEvent)
            {
                if (TBX_NowPageNum.Text.Length != 0)
                {
                    int a = 0;
                    try
                    {
                        a = Convert.ToInt32(TBX_NowPageNum.Text);

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                        canTextEvent = false;
                        TBX_NowPageNum.Text = nowpagenum.ToString();
                        canTextEvent = true;

                    }
                    if (a > 0 && a <= totalpage)
                    {
                        nowpagenum = a;
                        TBX_NowPageNum.Text = nowpagenum.ToString();

                        DrawView();

                        checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14;
                        kuangregion = new Region();
                        kuangregion.MakeEmpty();

                        kuangregion.Union(GetWaiKuang(1));
                        kuangregion.Xor(GetNeiKuang(1));

                        Panel_Kuang.Region = kuangregion;



                    }
                    else
                    {
                        MessageBox.Show("页数数量不正确");
                        canTextEvent = false;
                        TBX_NowPageNum.Text = nowpagenum.ToString();
                        canTextEvent = true;
                    }

                }
            }



        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(1));
            kuangregion.Xor(GetNeiKuang(1));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 1;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(2));
            kuangregion.Xor(GetNeiKuang(2));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 2;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(3));
            kuangregion.Xor(GetNeiKuang(3));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 3;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(4));
            kuangregion.Xor(GetNeiKuang(4));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 4;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(5));
            kuangregion.Xor(GetNeiKuang(5));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 5;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(6));
            kuangregion.Xor(GetNeiKuang(6));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 6;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(7));
            kuangregion.Xor(GetNeiKuang(7));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 7;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(8));
            kuangregion.Xor(GetNeiKuang(8));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 8;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(9));
            kuangregion.Xor(GetNeiKuang(9));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 9;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(10));
            kuangregion.Xor(GetNeiKuang(10));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 10;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(11));
            kuangregion.Xor(GetNeiKuang(11));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 11;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(12));
            kuangregion.Xor(GetNeiKuang(12));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 12;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(13));
            kuangregion.Xor(GetNeiKuang(13));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 13;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            kuangregion = new Region();
            kuangregion.MakeEmpty();

            kuangregion.Union(GetWaiKuang(14));
            kuangregion.Xor(GetNeiKuang(14));

            Panel_Kuang.Region = kuangregion;
            nowboxnum = 14;
            checkedListBox1.SelectedIndex = (nowpagenum - 1) * 14 + nowboxnum - 1;


        }

        //  string txt = "";
        int second = 0;
        // List<Train_Status> trainmesg = new List<Train_Status>();
        bool canstop = false;

        bool IsCrack = true;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsCrack)
            {
                if (second < 5)
                {
                    second++;
                }
                else if (second == 5)
                {
                    second++;
                    string[] text = new string[11];
                    text[0] = "max_batches";
                    text[1] = (max_batches / 10).ToString();
                    text[2] = (max_batches / 10 * 2).ToString();
                    text[3] = (max_batches / 10 * 3).ToString();
                    text[4] = (max_batches / 10 * 4).ToString();
                    text[5] = (max_batches / 10 * 5).ToString();
                    text[6] = (max_batches / 10 * 6).ToString();
                    text[7] = (max_batches / 10 * 7).ToString();
                    text[8] = (max_batches / 10 * 8).ToString();
                    text[9] = (max_batches / 10 * 9).ToString();
                    text[10] = (max_batches).ToString();



                    Train_Status ts = Get_Seeney_train_status(0);
                    //userCurve1.Width = 1430;
                    // userCurve1.Height = 840;
                    userCurve1.ValueMaxLeft = ts.max_img_loss;
                    userCurve1.ValueMaxRight = 99;
                    userCurve1.ValueMinRight = -1;
                    userCurve1.StrechDataCountMax = max_batches;
                    userCurve1.SetCurveText(text);
                    userCurve1.SetLeftCurve("loss", new float[] { }, Color.Red);
                    userCurve1.SetRightCurve("precision", new float[] { }, Color.Blue);
                    userCurve1.Visible = true;


                }
                else
                {
                    Train_Status ts = Get_Seeney_train_status(0);
                    userCurve1.AddCurveData("loss", ts.avg_loss);
                    userCurve1.AddCurveData("precision", ts.average_precision);
                    // string nowtest = "max_img_loss:" + ts.max_img_loss + "\r\nmax_batches:" + ts.max_batches + "\r\nnow_batches:" + ts.now_batches + "\r\navg_loss:" + ts.avg_loss.ToString() + "\r\nprecision:" + ts.average_precision.ToString() + "\r\n\r\n";
                    //  txt += nowtest;
                    //dSkinRichTextBox1.Text = nowtest;
                }
            }
            else
            {

            }


        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConfig(field.Finalmodel);
        }

       
    }
}
