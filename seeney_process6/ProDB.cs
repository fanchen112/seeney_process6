using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seeney_process6
{
    public class promesg
    {
        public string proname;
        public string propath;
    }
    class ProDB
    {
        public Dictionary<string, promesg> readjson(int proType)
        {
            Dictionary<string, promesg> Protable = new Dictionary<string, promesg>();
            if (proType == 0)
            {
                using (StreamReader reader = new StreamReader("CProDB.json"))
                {
                    Protable =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, promesg>>(reader.ReadToEnd());
                }
            }
            else if (proType == 1)
            {
                using (StreamReader reader = new StreamReader("SProDB.json"))
                {
                    Protable =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, promesg>>(reader.ReadToEnd());
                }
            }else if(proType == 3)
            {
                Protable = null;
            }
            return Protable;

        }
        public void writejson(Dictionary<string, promesg> protable, int proType)
        {
            if (proType == 0)
            {
                using (StreamWriter writer = new StreamWriter("CProDB.json"))
                {

                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(protable, Newtonsoft.Json.Formatting.Indented));


                }
            }
            else if (proType == 1)
            {
                using (StreamWriter writer = new StreamWriter("SProDB.json"))
                {
                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(protable));
                }
            }
        }

        /// <summary>
        /// 查找时间段项目
        /// </summary>
        /// <param name="proType">类型</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <returns></returns> 

        public DataTable QueryTime(int proType, string starttime, string endtime)
        {
            Dictionary<string, promesg> Protable = new Dictionary<string, promesg>();
            // List<string> creattimes = new List<string>();
            DataTable datatable = new DataTable();
            datatable.Columns.Add("项目名");
            datatable.Columns.Add("创建时间");
            datatable.Columns.Add("项目地址");
            datatable.Columns.Add("项目类型");
            datatable.Columns.Add("图片量");
            datatable.Columns.Add("标记信息");
            string labelmesg;
            string sumcount;
            if (proType == 0)
            {
                Protable = readjson(proType);
                if (Protable == null)
                {


                }
                else
                {

                    DateTime DT_starttime = Convert.ToDateTime(starttime);
                    DateTime DT_endtime = Convert.ToDateTime(endtime);
                    foreach (string i in Protable.Keys)
                    {
                        DateTime DT_i = Convert.ToDateTime(i);
                        if (DT_i >= DT_starttime && DT_i <= DT_endtime)
                        {
                            //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                            sumcount = new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length.ToString();
                            labelmesg = "已标" + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.json").Length + "张";
                            datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "缺陷", sumcount, labelmesg);
                        }
                    }

                }

            }
            else if (proType == 1)
            {

                Protable = readjson(proType);


                if (Protable == null)
                {


                }
                else
                {

                    DateTime DT_starttime = Convert.ToDateTime(starttime);
                    DateTime DT_endtime = Convert.ToDateTime(endtime);
                    foreach (string i in Protable.Keys)
                    {
                        DateTime DT_i = Convert.ToDateTime(i);
                        if (DT_i >= DT_starttime && DT_i <= DT_endtime)
                        {
                            //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                            sumcount = (new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length).ToString();
                            labelmesg = "良品" + new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张不良" + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";
                            datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "分类", sumcount, labelmesg);
                        }
                    }

                }

            }
            else if (proType == 2)
            {
                Protable = readjson(0);


                if (Protable == null)
                {


                }
                else
                {
                    DateTime DT_starttime = Convert.ToDateTime(starttime);
                    DateTime DT_endtime = Convert.ToDateTime(endtime);
                    foreach (string i in Protable.Keys)
                    {
                        DateTime DT_i = Convert.ToDateTime(i);
                        if (DT_i >= DT_starttime && DT_i <= DT_endtime)
                        {
                            //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                            sumcount = new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length.ToString();
                            labelmesg = "已标" + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.json").Length + "张";
                            datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "缺陷", sumcount, labelmesg);
                        }
                    }

                }

                Protable = readjson(1);


                if (Protable == null)
                {


                }
                else
                {

                    DateTime DT_starttime = Convert.ToDateTime(starttime);
                    DateTime DT_endtime = Convert.ToDateTime(endtime);
                    foreach (string i in Protable.Keys)
                    {
                        DateTime DT_i = Convert.ToDateTime(i);
                        if (DT_i >= DT_starttime && DT_i <= DT_endtime)
                        {
                            //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                            sumcount = (new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length).ToString();
                            labelmesg = "良品" + new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张不良" + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";
                            datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "分类", sumcount, labelmesg);
                        }
                    }

                }


            }

            return datatable;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="proType"></param>
        /// <returns></returns>
        public DataTable QueryAllTime(int proType)
        {
            Dictionary<string, promesg> Protable = new Dictionary<string, promesg>();
            // List<string> creattimes = new List<string>();
            DataTable datatable = new DataTable();
            datatable.Columns.Add("项目名");
            datatable.Columns.Add("创建时间");
            datatable.Columns.Add("项目地址");
            datatable.Columns.Add("项目类型");
            datatable.Columns.Add("图片量");
            datatable.Columns.Add("标记信息");
            string labelmesg;
            string sumcount;


            if (proType == 0)
            {
                Protable = readjson(proType);


                if (Protable == null)
                {


                }
                else
                {


                    foreach (string i in Protable.Keys)
                    {

                        //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                        sumcount = new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length.ToString();
                        labelmesg = "已标" + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.json").Length + "张";
                        datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "缺陷", sumcount, labelmesg);

                    }

                }

            }
            else if (proType == 1)
            {

                Protable = readjson(proType);


                if (Protable == null)
                {


                }
                else
                {

                    foreach (string i in Protable.Keys)
                    {

                        //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                        sumcount = (new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length).ToString();
                        labelmesg = "良品" + new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张不良" + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";
                        datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "分类", sumcount, labelmesg);

                    }

                }

            }
            else if (proType == 2)
            {
                Protable = readjson(0);


                if (Protable == null)
                {


                }
                else
                {


                    foreach (string i in Protable.Keys)
                    {

                        //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                        sumcount = new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length.ToString();
                        labelmesg = "已标" + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.json").Length + "张";
                        datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "缺陷", sumcount, labelmesg);

                    }

                }

                Protable = readjson(1);


                if (Protable == null)
                {


                }
                else
                {


                    foreach (string i in Protable.Keys)
                    {

                        //creattimes.Add(DT_i.ToString("yyyy-MM-dd HH:mm:ss"));
                        sumcount = (new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + new DirectoryInfo(Protable[i].propath + "\\image").GetFiles("*.bmp").Length).ToString();
                        labelmesg = "良品" + new DirectoryInfo(Protable[i].propath + "\\image\\NormalImages").GetFiles("*.bmp").Length + "张不良" + new DirectoryInfo(Protable[i].propath + "\\image\\DefectImages").GetFiles("*.bmp").Length + "张";
                        datatable.Rows.Add(Protable[i].proname, i, Protable[i].propath, "分类", sumcount, labelmesg);

                    }

                }


            }

            return datatable;

        }




        public promesg QueryPromesg(int proType, string createtime)
        {

            Dictionary<string, promesg> Protable = new Dictionary<string, promesg>();
            Protable = readjson(proType);
            promesg mesg = new promesg();
            if (Protable == null)
            {




            }
            else
            {


                mesg.proname = Protable[createtime].proname;
                mesg.propath = Protable[createtime].propath;

            }
            return mesg;
        }


        public void AddOnepro(string ProName, string ProPath, string datetime, int proType)
        {
            Dictionary<string, promesg> Protable = new Dictionary<string, promesg>();


            Protable = readjson(proType);

            if (Protable == null)
            {
                Dictionary<string, promesg> protable = new Dictionary<string, promesg>();
                promesg mesg = new promesg();

                mesg.proname = ProName;
                mesg.propath = ProPath;

                protable.Add(datetime, mesg);

                writejson(protable, proType);

            }
            else
            {
                promesg mesg = new promesg();

                mesg.proname = ProName;
                mesg.propath = ProPath;
                Protable.Add(datetime, mesg);

                writejson(Protable, proType);

            }



        }

        public void deleteOnePro(string datetime, int proType)
        {
            Dictionary<string, promesg> Protable = new Dictionary<string, promesg>();
            Protable = readjson(proType);
            if (Protable.ContainsKey(datetime))
            {
                Protable.Remove(datetime);
                writejson(Protable, proType);

            }


        }


    }

}