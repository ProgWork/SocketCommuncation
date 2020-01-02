namespace ClientTerminal
{
    partial class Client
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.MsgtextBox = new System.Windows.Forms.TextBox();
            this.Sendbutton = new System.Windows.Forms.Button();
            this.ReConnectbutton = new System.Windows.Forms.Button();
            this.SendMsgtextBox = new System.Windows.Forms.TextBox();
            this.ClientcomboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // MsgtextBox
            // 
            this.MsgtextBox.Location = new System.Drawing.Point(12, 12);
            this.MsgtextBox.Multiline = true;
            this.MsgtextBox.Name = "MsgtextBox";
            this.MsgtextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MsgtextBox.Size = new System.Drawing.Size(557, 338);
            this.MsgtextBox.TabIndex = 0;
            // 
            // Sendbutton
            // 
            this.Sendbutton.Location = new System.Drawing.Point(494, 379);
            this.Sendbutton.Name = "Sendbutton";
            this.Sendbutton.Size = new System.Drawing.Size(75, 23);
            this.Sendbutton.TabIndex = 1;
            this.Sendbutton.Text = "发送";
            this.Sendbutton.UseVisualStyleBackColor = true;
            this.Sendbutton.Click += new System.EventHandler(this.Sendbutton_Click);
            // 
            // ReConnectbutton
            // 
            this.ReConnectbutton.Location = new System.Drawing.Point(646, 280);
            this.ReConnectbutton.Name = "ReConnectbutton";
            this.ReConnectbutton.Size = new System.Drawing.Size(75, 23);
            this.ReConnectbutton.TabIndex = 2;
            this.ReConnectbutton.Text = "重新连接";
            this.ReConnectbutton.UseVisualStyleBackColor = true;
            // 
            // SendMsgtextBox
            // 
            this.SendMsgtextBox.Location = new System.Drawing.Point(12, 360);
            this.SendMsgtextBox.Multiline = true;
            this.SendMsgtextBox.Name = "SendMsgtextBox";
            this.SendMsgtextBox.Size = new System.Drawing.Size(412, 60);
            this.SendMsgtextBox.TabIndex = 3;
            // 
            // ClientcomboBox
            // 
            this.ClientcomboBox.FormattingEnabled = true;
            this.ClientcomboBox.Location = new System.Drawing.Point(592, 12);
            this.ClientcomboBox.Name = "ClientcomboBox";
            this.ClientcomboBox.Size = new System.Drawing.Size(196, 20);
            this.ClientcomboBox.TabIndex = 4;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ClientcomboBox);
            this.Controls.Add(this.MsgtextBox);
            this.Controls.Add(this.SendMsgtextBox);
            this.Controls.Add(this.ReConnectbutton);
            this.Controls.Add(this.Sendbutton);
            this.Name = "Client";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Client_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MsgtextBox;
        private System.Windows.Forms.Button Sendbutton;
        private System.Windows.Forms.Button ReConnectbutton;
        private System.Windows.Forms.TextBox SendMsgtextBox;
        private System.Windows.Forms.ComboBox ClientcomboBox;
    }
}

