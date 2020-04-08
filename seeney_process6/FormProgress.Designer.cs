using System;

namespace seeney_process6
{
    partial class FormProgress
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbLoadMsg = new System.Windows.Forms.Label();
            this.MyProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lbLoadMsg
            // 
            this.lbLoadMsg.AutoSize = true;
            this.lbLoadMsg.BackColor = System.Drawing.Color.Transparent;
            this.lbLoadMsg.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbLoadMsg.Location = new System.Drawing.Point(165, 9);
            this.lbLoadMsg.Name = "lbLoadMsg";
            this.lbLoadMsg.Size = new System.Drawing.Size(76, 21);
            this.lbLoadMsg.TabIndex = 0;
            this.lbLoadMsg.Text = "label1";
            // 
            // MyProgressBar
            // 
            this.MyProgressBar.Location = new System.Drawing.Point(0, 33);
            this.MyProgressBar.Name = "MyProgressBar";
            this.MyProgressBar.Size = new System.Drawing.Size(400, 38);
            this.MyProgressBar.TabIndex = 1;
            // 
            // FormProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 71);
            this.Controls.Add(this.MyProgressBar);
            this.Controls.Add(this.lbLoadMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormProgress";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLoading_FormClosed);
            this.Load += new System.EventHandler(this.FormLoading_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.Label lbLoadMsg;
        private System.Windows.Forms.ProgressBar MyProgressBar;
    }
}