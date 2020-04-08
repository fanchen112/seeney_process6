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
    public partial class FormModel : DSkinForm
    {
        Field field = new Field();
        ProDB proDB = new ProDB();
        string propath;
        List<DateTime> models = new List<DateTime>();
        public FormModel()
        {
            InitializeComponent();
            if (field.NowProType == 0)
            {
                LAB_ProCreTime.Text = field.NowProCreateTime;
                promesg promesg =
                proDB.QueryPromesg(0, field.NowProCreateTime);
                LAB_ProName.Text = promesg.proname;
                propath = promesg.propath;
                LAB_ProType.Text = "弱像素分割项目";
                using (StreamReader reader = new StreamReader(propath + "\\models.json"))
                {
                    models = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(reader.ReadToEnd());

                }
                foreach (DateTime i in models)
                {
                    dSkinListBox1.Items.Add(i.ToString("yyyyMMddHHmmss"));
                }




            }
            else if (field.NowProType == 1)
            {
                LAB_ProCreTime.Text = field.NowProCreateTime;
                promesg promesg =
                proDB.QueryPromesg(0, field.NowProCreateTime);
                LAB_ProName.Text = promesg.proname;
                propath = promesg.propath;
                LAB_ProType.Text = "大分类项目";
                using (StreamReader reader = new StreamReader(propath + "\\models.json"))
                {
                    models = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(reader.ReadToEnd());

                }
                foreach (DateTime i in models)
                {
                    dSkinListBox1.Items.Add(i.ToString("yyyyMMddHHmmss"));
                }

            }
            else if (field.NowProType == 2)
            {
                MessageBox.Show("没有导入项目");
                // this.Close();
            }
            //if(dSkinListBox1)


        }

        modeljson onemodeltype = new modeljson();
        List<modeljson> modeljsons = new List<modeljson>();
        private void dSkinListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            modeljsons = new List<modeljson>();

            string modelspath = propath + "\\model_" + dSkinListBox1.SelectedItem.ToString() + "\\models.json";
            if (File.Exists(modelspath))
            {
                using (StreamReader reader = new StreamReader(modelspath))
                {
                    modeljsons = Newtonsoft.Json.JsonConvert.DeserializeObject<List<modeljson>>(reader.ReadToEnd());

                }

                foreach (var i in modeljsons)
                {
                    dataGridView1.Rows.Add(i.company, i.product, i.came, i.imagecount, i.labelcount);
                }
            }
            else
            {
                MessageBox.Show("无配置信息");
            }


        }
    }
}
