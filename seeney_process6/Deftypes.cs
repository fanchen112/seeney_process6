using ChaseDream.EnterpriseLibraries.DSkinEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace seeney_process6
{
    public partial class Deftypes : DSkinForm
    {
        Field field = new Field();
        //定义一个委托
        public delegate void delegate_CrackTypeshow();
        //定义一个事件
        public event delegate_CrackTypeshow Event_CrackTypeshow;
        public Deftypes()
        {
            InitializeComponent();
        }

        private void BTN_OK_Click(object sender, EventArgs e)
        {
            field.NowCrackType = (int)NUD_CrackType.DSkinValue;
            if (Event_CrackTypeshow != null)
            {
                Event_CrackTypeshow();
            }
            this.Close();
        }

        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
