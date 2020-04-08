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
using ChaseDream.EnterpriseLibraries.DSkinEngine;

namespace seeney_process6
{
    public partial class ManagePros : DSkinForm
    {
        ProDB prodb = new ProDB();
        public ManagePros()
        {
            InitializeComponent();
        }
        #region 实现提示文字
        Boolean textboxHasText = false;//判断输入框是否有文本 
        private void keywords_Enter(object sender, EventArgs e)
        {
            if (textboxHasText == false)
                keywords.Text = "";
            keywords.ForeColor = Color.Black;

        }

        private void keywords_Leave(object sender, EventArgs e)
        {
            if (keywords.Text == "")
            {
                keywords.Text = "请输入项目名关键字";
                keywords.ForeColor = Color.LightGray;
                textboxHasText = false;
            }
            else
                textboxHasText = true;

        }
        #endregion

        private void BTN_Query_Click(object sender, EventArgs e)
        {


            if (CMB_Types.Text == "所有类型")
            {


                if (TOG_IsSelTime.DSkinToggled)
                {
                    //所有类型:加入时间筛选
                    dataGridView2.DataSource = prodb.QueryTime(2, StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    dataGridView2.DataSource = prodb.QueryAllTime(2);
                }
            }
            else if (CMB_Types.Text == "缺陷")
            {
                if (TOG_IsSelTime.DSkinToggled)
                {
                    dataGridView2.DataSource = prodb.QueryTime(0, StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {

                    dataGridView2.DataSource = prodb.QueryAllTime(0);
                }
            }
            else if (CMB_Types.Text == "分类")
            {


                if (TOG_IsSelTime.DSkinToggled)
                {
                    dataGridView2.DataSource = prodb.QueryTime(1, StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    dataGridView2.DataSource = prodb.QueryAllTime(1);
                }
            }
            else
            {

                MessageBox.Show("选择类型有误");
            }



        }

        private void BTN_DeletePro_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows == null || dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("请选择项目");
            }
            else
            {
                DialogResult result =
                MessageBox.Show("确认删除" + dataGridView2[0, dataGridView2.SelectedRows.Count].Value, "删除", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    try
                    {

                        //可能bug  :image正在使用
                        Directory.Delete(dataGridView2[2, dataGridView2.SelectedRows.Count].Value.ToString(), true);
                        if (dataGridView2[3, dataGridView2.SelectedRows.Count].Value.ToString() == "缺陷")
                        {
                            prodb.deleteOnePro(dataGridView2[1, dataGridView2.SelectedRows.Count].Value.ToString(), 0);
                        }
                        else if (dataGridView2[3, dataGridView2.SelectedRows.Count].Value.ToString() == "分类")
                        {
                            prodb.deleteOnePro(dataGridView2[1, dataGridView2.SelectedRows.Count].Value.ToString(), 1);
                        }
                        dataGridView2.Rows.Remove(dataGridView2.Rows[dataGridView2.SelectedRows.Count]);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }
        private void BTN_OpenFile_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows == null || dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("请选择项目");
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", dataGridView2[2, dataGridView2.SelectedRows.Count].Value.ToString());
            }
        }
    }
}
