using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seeney_process6
{
    /// <summary>
    /// 缺陷参数配置
    /// </summary>
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

    /// <summary>
    /// 分类参数配置
    /// </summary>
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
        public int protype;//0:弱像素分割；1：大分类
        public int totalmodelnum;
        public OneProject()
        {
            totalmodelnum = 1;
        }
    }

    public class OneModel
    {
        public int CurrentModel;
        public string procreatetime;//对应 OneProject.createtime
        public string modelcreatetime;
        public string trainbegintime;
        public string trainendtime;
    }

    
    /// <summary>
    /// 缺陷标记json对象
    /// </summary>
    #region
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
    #endregion

    /// <summary>
    /// 全项目字段
    /// </summary>
    class Field
    {
        private static string nowProCreateTime;
        private static int nowProType = 3;//0：Crack 1:Sort 3：都不是
        private static long nowCrackType = 1;
        private static string nowproPath;
        
        private static bool isJiCheng;
        private static int selectmodel;//1、2、3.。。
        private static int finalmodel;//最后一个model，也是在用的

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

        public string NowproPath
        {
            get
            {
                return nowproPath;
            }

            set
            {
                nowproPath = value;
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

        public int Selectmodel
        {
            get
            {
                return selectmodel;
            }

            set
            {
                selectmodel = value;
            }
        }

        public int Finalmodel
        {
            get
            {
                return finalmodel;
            }

            set
            {
                finalmodel = value;
            }
        }
    }
}
