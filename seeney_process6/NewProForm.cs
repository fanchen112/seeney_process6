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
    public partial class NewProForm : DSkinForm
    {
        //定义一个委托 

        public delegate void delegatProshow();
        //定义一个事件
        public event delegatProshow eventProShow;
        Field field = new Field();
        ProDB prodb = new ProDB();
        public NewProForm()
        {
            InitializeComponent();
        }

        private void BTN_SkirDirPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "请选择项目路径";

            if (folder.ShowDialog() == DialogResult.OK)
            {
                TBX_ProDirPath.Text = folder.SelectedPath;
            }
        }

        private void BTN_NewProOK_Click(object sender, EventArgs e)
        {
            if (TBX_ProName.Text == "")
            {
                MessageBox.Show("项目名不为空");
            }
            else
            {
                string dirpath = TBX_ProDirPath.Text + "\\" + TBX_ProName.Text;

                if (Directory.Exists(dirpath))
                {
                    MessageBox.Show("存在同名文件夹");
                }
                else
                {
                    Directory.CreateDirectory(dirpath);
                    Directory.CreateDirectory(dirpath + "\\1");
                    Directory.CreateDirectory(dirpath + "\\1\\image");

                    OneProject oneproject = new OneProject();



                    oneproject.proname = TBX_ProName.Text;
                    oneproject.createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    field.NowProCreateTime = oneproject.createtime;

                    string strconfig = "";
                    if (CBX_CrackType.Checked)
                    {
                        CrackConfig crackConfig = new CrackConfig();

                        strconfig = Newtonsoft.Json.JsonConvert.SerializeObject(crackConfig, Newtonsoft.Json.Formatting.Indented);

                        field.NowProType = 0;
                        oneproject.protype = 0;
                        prodb.AddOnepro(oneproject.proname, dirpath, oneproject.createtime, 0);
                    }
                    else if (CBX_SortType.Checked)
                    {

                        SortConfig sortConfig = new SortConfig();
                        Directory.CreateDirectory(dirpath + "\\1");
                        Directory.CreateDirectory(dirpath + "\\1\\image\\DefectImages");
                        Directory.CreateDirectory(dirpath + "\\1\\image\\NormalImages");

                        strconfig = Newtonsoft.Json.JsonConvert.SerializeObject(sortConfig, Newtonsoft.Json.Formatting.Indented);

                        field.NowProType = 1;
                        oneproject.protype = 1;

                        prodb.AddOnepro(oneproject.proname, dirpath, oneproject.createtime, 1);

                    }
                    using (StreamWriter writer = new StreamWriter(dirpath + "\\config.json"))
                    {
                        writer.Write(strconfig);
                    }
                    File.Create(dirpath + "\\models.json");

                    string projson = Newtonsoft.Json.JsonConvert.SerializeObject(oneproject, Newtonsoft.Json.Formatting.Indented);
                    using (StreamWriter writer = new StreamWriter(dirpath + "\\project.json"))
                    {
                        writer.Write(projson);
                    }
                    if (eventProShow != null)
                    {
                        eventProShow();
                    }
                    this.Close();

                }
            }
        }

        private void BTN_NewProCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CBX_CrackType_Click(object sender, EventArgs e)
        {
            CBX_CrackType.Checked = true;
            CBX_SortType.Checked = false;
        }

        private void CBX_SortType_Click(object sender, EventArgs e)
        {
            CBX_CrackType.Checked = false;
            CBX_SortType.Checked = true;
        }
    }
}
