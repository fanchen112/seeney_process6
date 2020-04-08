using ChaseDream.EnterpriseLibraries.DSkinEngine;
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

namespace seeney_process6
{

    public partial class ModelConfig : DSkinForm
    {
        Field field = new Field();
        modeljson onemodeltype = new modeljson();
        List<modeljson> modeljsons = new List<modeljson>();
        //定义一个委托
        public delegate void delegate_megCommit();
        //定义一个事件
        public event delegate_megCommit Event_megCommit;
        DataTable dataTable = new DataTable();
        public ModelConfig()
        {
            dataTable.Columns.Add("公司名");
            dataTable.Columns.Add("产品名");
            dataTable.Columns.Add("相机数");
            dataTable.Columns.Add("图片数量");
            dataTable.Columns.Add("已标记图片数量");
            InitializeComponent();
            if (field.IsJiCheng)
            {
                LAB_JICheng.Visible = true;
                COMB_JICheng.Visible = true;
                string strmodels;
                using (StreamReader reader = new StreamReader(field.NowproPath + "\\" + field.Finalmodel + "\\models.json"))
                {
                    strmodels = reader.ReadToEnd();
                }
                List<DateTime> models = new List<DateTime>();
                models = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(strmodels);
                if (models != null)
                {
                    foreach (var i in models)
                    {
                        COMB_JICheng.Items.Add(i);

                    }
                }


            }
            else
            {

            }
        }

        private void BTN_ADDList_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("是否确认", "将文字框的数据添加到表格", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                dataTable.Rows.Add(TBX_Company.Text, TBX_Product.Text, TBX_Came.Text, TBX_ImgCount.Text, TBX_LabCount.Text);
                ModelDatas.DataSource = dataTable;
            }
            else
            {

            }





        }

        private void BTN_Comit_Click(object sender, EventArgs e)
        {

            if (field.IsJiCheng)
            {
                if(COMB_JICheng.SelectedIndex!=-1)
                {
                    //field.SelectJiCheng = COMB_JICheng.SelectedItem.ToString();
                    modeljsons.Clear();
                    for (int i = 0; i < ModelDatas.Rows.Count; i++)
                    {
                        onemodeltype.company = ModelDatas[0, i].Value.ToString();
                        onemodeltype.product = ModelDatas[1, i].Value.ToString();
                        onemodeltype.came = ModelDatas[2, i].Value.ToString();
                        onemodeltype.imagecount = ModelDatas[3, i].Value.ToString();
                        onemodeltype.labelcount = ModelDatas[4, i].Value.ToString();
                        modeljsons.Add(onemodeltype);
                    }
                    // File.Create(field.ProPath + "\\model_" + field.Modelcreatetime.ToString("yyyyMMddHHmmss") + "\\model.json");
                    using (StreamWriter writer = new StreamWriter(field.NowproPath + "\\" + field.Finalmodel + "\\mode\\model.json"))
                    {
                        writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(modeljsons, Newtonsoft.Json.Formatting.Indented));
                    }
                    if (Event_megCommit != null)
                    {
                        Event_megCommit();
                    }
                    this.Close();

                }
                else
                {
                    MessageBox.Show("请选择继承的模型");
                }
            }else
            {
                modeljsons.Clear();
                for (int i = 0; i < ModelDatas.Rows.Count; i++)
                {
                    onemodeltype.company = ModelDatas[0, i].Value.ToString();
                    onemodeltype.product = ModelDatas[1, i].Value.ToString();
                    onemodeltype.came = ModelDatas[2, i].Value.ToString();
                    onemodeltype.imagecount = ModelDatas[3, i].Value.ToString();
                    onemodeltype.labelcount = ModelDatas[4, i].Value.ToString();
                    modeljsons.Add(onemodeltype);
                }
                // File.Create(field.ProPath + "\\model_" + field.Modelcreatetime.ToString("yyyyMMddHHmmss") + "\\model.json");
                using (StreamWriter writer = new StreamWriter(field.NowproPath + "\\" + field.Finalmodel + "\\model\\model.json"))
                {
                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(modeljsons, Newtonsoft.Json.Formatting.Indented));
                }
                if (Event_megCommit != null)
                {
                    Event_megCommit();
                }
                this.Close();
            }

           

        }
    }
    public class modeljson
    {
        public string company;
        public string product;
        public string came;
        public string imagecount;
        public string labelcount;

    }
}
