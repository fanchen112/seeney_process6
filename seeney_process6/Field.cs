using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seeney_process6
{
    public class CrackConfig
    {
        public long crackTypes;
        public string batch;
        public string subdivisions;
        public string width;
        public string height;
        public string angle;
        public string saturation;
        public string exposure;
        public string hue;
        public string learning_rate;
        public long ShiJianBeiShu;


        public CrackConfig()
        {
            crackTypes = 1;
            batch = "64";
            subdivisions = "32";
            width = "640";
            height = "540";
            angle = "0";
            saturation = "1.5";
            exposure = "1.5";
            hue = "1";
            learning_rate = "0.001";
            ShiJianBeiShu = 4;
        }

    }

    public class SortConfig
    {

        public string angle;
        public string batch_size1;
        public string batch_size2;
        public string new_width;
        public string new_height;
        public string max_iter;
        public SortConfig()
        {
            angle = "90";
            batch_size1 = "64";
            batch_size2 = "32";
            new_width = "224";
            new_height = "224";
            max_iter = "600";
        }

    }

    public class OneProject
    {
        public string proname;
        public string createtime;
        public int protype;
    }

    public class Shapes
    {
        public List<SiPoint> shape = new List<SiPoint>();
    }

    public class SiPoint
    {
        public int cracktype;
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;
        public int X3;
        public int Y3;
        public int X4;
        public int Y4;
    }
    class Field
    {
        private static string nowProCreateTime;
        private static int nowProType = 3;//0：Crack 1:Sort 3：都不是
        private static long nowCrackType = 1;
        private static string proPath;
        private static DateTime modelcreatetime;
        private static bool isJiCheng;
        private static string selectJiCheng;

        public string NowProCreateTime
        {
            get
            {
                return nowProCreateTime;
            }

            set
            {
                nowProCreateTime = value;
            }
        }

        public int NowProType
        {
            get
            {
                return nowProType;
            }

            set
            {
                nowProType = value;
            }
        }

        public long NowCrackType
        {
            get
            {
                return nowCrackType;
            }

            set
            {
                nowCrackType = value;
            }
        }

        public string ProPath
        {
            get
            {
                return proPath;
            }

            set
            {
                proPath = value;
            }
        }

        public DateTime Modelcreatetime
        {
            get
            {
                return modelcreatetime;
            }

            set
            {
                modelcreatetime = value;
            }
        }

        public bool IsJiCheng
        {
            get
            {
                return isJiCheng;
            }

            set
            {
                isJiCheng = value;
            }
        }

        public string SelectJiCheng
        {
            get
            {
                return selectJiCheng;
            }

            set
            {
                selectJiCheng = value;
            }
        }
    }
}
