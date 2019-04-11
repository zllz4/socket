namespace socket
{
    partial class MainForm
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
            this.severStartButton = new System.Windows.Forms.Button();
            this.clientStartButton = new System.Windows.Forms.Button();
            this.ipLabel = new System.Windows.Forms.Label();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.infoTitleLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // severStartButton
            // 
            this.severStartButton.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.severStartButton.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.severStartButton.Location = new System.Drawing.Point(58, 162);
            this.severStartButton.Name = "severStartButton";
            this.severStartButton.Size = new System.Drawing.Size(68, 57);
            this.severStartButton.TabIndex = 0;
            this.severStartButton.Text = "服务端";
            this.severStartButton.UseVisualStyleBackColor = false;
            this.severStartButton.Click += new System.EventHandler(this.serverStartButtonClick);
            // 
            // clientStartButton
            // 
            this.clientStartButton.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.clientStartButton.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.clientStartButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clientStartButton.Location = new System.Drawing.Point(159, 137);
            this.clientStartButton.Name = "clientStartButton";
            this.clientStartButton.Size = new System.Drawing.Size(151, 103);
            this.clientStartButton.TabIndex = 1;
            this.clientStartButton.Text = "客户端";
            this.clientStartButton.UseVisualStyleBackColor = false;
            this.clientStartButton.Click += new System.EventHandler(this.clientStartButton_Click);
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ipLabel.Location = new System.Drawing.Point(63, 40);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(83, 20);
            this.ipLabel.TabIndex = 2;
            this.ipLabel.Text = "输入ip地址";
            // 
            // ipTextBox
            // 
            this.ipTextBox.Location = new System.Drawing.Point(169, 35);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(100, 25);
            this.ipTextBox.TabIndex = 3;
            this.ipTextBox.Text = "127.0.0.1";
            this.ipTextBox.TextChanged += new System.EventHandler(this.IpTextBox_TextChanged);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.portLabel.Location = new System.Drawing.Point(63, 95);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(84, 20);
            this.portLabel.TabIndex = 4;
            this.portLabel.Text = "输入端口号";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(170, 90);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(100, 25);
            this.portTextBox.TabIndex = 5;
            this.portTextBox.Text = "3423";
            // 
            // infoTitleLabel
            // 
            this.infoTitleLabel.AutoSize = true;
            this.infoTitleLabel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.infoTitleLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.infoTitleLabel.Location = new System.Drawing.Point(424, 17);
            this.infoTitleLabel.Name = "infoTitleLabel";
            this.infoTitleLabel.Size = new System.Drawing.Size(91, 20);
            this.infoTitleLabel.TabIndex = 6;
            this.infoTitleLabel.Text = ">连接情况<";
            this.infoTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.infoTitleLabel.Click += new System.EventHandler(this.InfoTitleLabel_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.infoLabel.Location = new System.Drawing.Point(316, 50);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(306, 165);
            this.infoLabel.TabIndex = 7;
            this.infoLabel.Text = "服务器未上线";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(653, 262);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.infoTitleLabel);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.ipLabel);
            this.Controls.Add(this.clientStartButton);
            this.Controls.Add(this.severStartButton);
            this.Name = "MainForm";
            this.Text = "Socket Test";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button severStartButton;
        private System.Windows.Forms.Button clientStartButton;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label infoTitleLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        //private System.ComponentModel.BackgroundWorker backgroundWorker1;
        //private System.ComponentModel.BackgroundWorker backgroundWorker2;
    }
}

