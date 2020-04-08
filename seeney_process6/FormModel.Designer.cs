namespace seeney_process6
{
    partial class FormModel
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
            this.LAB_ProType = new System.Windows.Forms.Label();
            this.LAB_ProName = new System.Windows.Forms.Label();
            this.LAB_ProCreTime = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dSkinListBox1 = new ChaseDream.EnterpriseLibraries.DSkinEngine.DSkinListBox();
            this.Col_Company = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Product = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Came = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_imagecount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Labelcount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // LAB_ProType
            // 
            this.LAB_ProType.AutoSize = true;
            this.LAB_ProType.Location = new System.Drawing.Point(23, 37);
            this.LAB_ProType.Name = "LAB_ProType";
            this.LAB_ProType.Size = new System.Drawing.Size(53, 12);
            this.LAB_ProType.TabIndex = 0;
            this.LAB_ProType.Text = "项目类型";
            // 
            // LAB_ProName
            // 
            this.LAB_ProName.AutoSize = true;
            this.LAB_ProName.Location = new System.Drawing.Point(23, 77);
            this.LAB_ProName.Name = "LAB_ProName";
            this.LAB_ProName.Size = new System.Drawing.Size(41, 12);
            this.LAB_ProName.TabIndex = 1;
            this.LAB_ProName.Text = "项目名";
            // 
            // LAB_ProCreTime
            // 
            this.LAB_ProCreTime.AutoSize = true;
            this.LAB_ProCreTime.Location = new System.Drawing.Point(23, 122);
            this.LAB_ProCreTime.Name = "LAB_ProCreTime";
            this.LAB_ProCreTime.Size = new System.Drawing.Size(77, 12);
            this.LAB_ProCreTime.TabIndex = 2;
            this.LAB_ProCreTime.Text = "项目创建时间";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_Company,
            this.Col_Product,
            this.Col_Came,
            this.Col_imagecount,
            this.Col_Labelcount});
            this.dataGridView1.Location = new System.Drawing.Point(145, 37);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(546, 328);
            this.dataGridView1.TabIndex = 3;
            // 
            // dSkinListBox1
            // 
            this.dSkinListBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dSkinListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.dSkinListBox1.DSkinItemBackground = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.dSkinListBox1.DSkinItemBackground2 = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.dSkinListBox1.DSkinItemSelected = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.dSkinListBox1.DSkinTextDisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(129)))), ((int)(((byte)(129)))));
            this.dSkinListBox1.DSkinTextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            this.dSkinListBox1.Font = new System.Drawing.Font("Verdana", 10F);
            this.dSkinListBox1.IntegralHeight = false;
            this.dSkinListBox1.ItemHeight = 18;
            this.dSkinListBox1.Location = new System.Drawing.Point(25, 154);
            this.dSkinListBox1.Name = "dSkinListBox1";
            this.dSkinListBox1.Size = new System.Drawing.Size(87, 150);
            this.dSkinListBox1.TabIndex = 4;
            this.dSkinListBox1.SelectedIndexChanged += new System.EventHandler(this.dSkinListBox1_SelectedIndexChanged);
            // 
            // Col_Company
            // 
            this.Col_Company.HeaderText = "公司";
            this.Col_Company.Name = "Col_Company";
            this.Col_Company.ReadOnly = true;
            // 
            // Col_Product
            // 
            this.Col_Product.HeaderText = "产品";
            this.Col_Product.Name = "Col_Product";
            this.Col_Product.ReadOnly = true;
            // 
            // Col_Came
            // 
            this.Col_Came.HeaderText = "相机";
            this.Col_Came.Name = "Col_Came";
            this.Col_Came.ReadOnly = true;
            // 
            // Col_imagecount
            // 
            this.Col_imagecount.HeaderText = "图片数";
            this.Col_imagecount.Name = "Col_imagecount";
            this.Col_imagecount.ReadOnly = true;
            // 
            // Col_Labelcount
            // 
            this.Col_Labelcount.HeaderText = "标记数";
            this.Col_Labelcount.Name = "Col_Labelcount";
            this.Col_Labelcount.ReadOnly = true;
            // 
            // FormModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dSkinListBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.LAB_ProCreTime);
            this.Controls.Add(this.LAB_ProName);
            this.Controls.Add(this.LAB_ProType);
            this.DSkinBorderInstance.DSkinColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.DSkinBorderInstance.DSkinHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(183)))), ((int)(((byte)(230)))));
            this.DSkinBorderInstance.DSkinHoverVisible = true;
            this.DSkinBorderInstance.DSkinRounding = 6;
            this.DSkinBorderInstance.DSkinShape = ChaseDream.EnterpriseLibraries.DSkinEngine.DSkinBorderShapes.Rounded;
            this.DSkinBorderInstance.DSkinThickness = 1;
            this.DSkinBorderInstance.DSkinVisible = true;
            this.DSkinHeaderBackColor = System.Drawing.Color.Black;
            this.DSkinHeaderTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Name = "FormModel";
            this.Text = "模型管理";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LAB_ProType;
        private System.Windows.Forms.Label LAB_ProName;
        private System.Windows.Forms.Label LAB_ProCreTime;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Company;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Product;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Came;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_imagecount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Labelcount;
        private ChaseDream.EnterpriseLibraries.DSkinEngine.DSkinListBox dSkinListBox1;
    }
}