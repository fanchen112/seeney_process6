using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace seeney_process6
{
    public delegate void ShowLoadMsg(string msg, int ipos);//2019-10-23 增加进度状态条显示
    public partial class FormProgress : Form
    {

        public FormProgress()
        {
            InitializeComponent();
        }
        #region 信息显示
        private delegate void LogMsgCallBack(string msg, int ipos);
        private void LogMsg(string msg, int ipos)
        {
            if (InvokeRequired)
            {
                object[] pList = { msg, ipos };
                this.lbLoadMsg.BeginInvoke(new LogMsgCallBack(AddLogMsg), pList);
            }
            else
            {
                AddLogMsg(msg, ipos);
            }
        }
        private void AddLogMsg(string msg, int ipos)
        {

            this.MyProgressBar.Value = ipos;
            this.lbLoadMsg.Text = msg;
            if (ipos == 100)
                this.Close();
        }
        #endregion


        private void FormLoading_Load(object sender, EventArgs e)
        {
            MainForm.LoadingMsg += new ShowLoadMsg(LogMsg);
            MainForm.frmLoadingOpen = true;

        }

        private void FormLoading_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.frmLoadingOpen = false;
        }








    }
}
