namespace MovingANPR
{
    partial class FormCamera
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCamera));
            this.ctxtMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._recordNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._takePhotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._cropToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelResult = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnCamera8 = new System.Windows.Forms.Panel();
            this.pnCamera4 = new System.Windows.Forms.Panel();
            this.pnCamera7 = new System.Windows.Forms.Panel();
            this.pnCamera6 = new System.Windows.Forms.Panel();
            this.pnCamera3 = new System.Windows.Forms.Panel();
            this.pnCamera5 = new System.Windows.Forms.Panel();
            this.pnCamera2 = new System.Windows.Forms.Panel();
            this.pnCamera1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.circle1 = new AltoControls.ProcessingControl();
            this.chk_autoANPR = new System.Windows.Forms.CheckBox();
            this.btn_connect = new AltoControls.AltoButton();
            this.progress_ANPR = new System.Windows.Forms.ProgressBar();
            this.ctxtMnu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxtMnu
            // 
            this.ctxtMnu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxtMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._recordNowToolStripMenuItem,
            this._takePhotoToolStripMenuItem,
            this._cropToolStripMenuItem});
            this.ctxtMnu.Name = "_ctxtMnu";
            this.ctxtMnu.Size = new System.Drawing.Size(144, 82);
            // 
            // _recordNowToolStripMenuItem
            // 
            this._recordNowToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("_recordNowToolStripMenuItem.Image")));
            this._recordNowToolStripMenuItem.Name = "_recordNowToolStripMenuItem";
            this._recordNowToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this._recordNowToolStripMenuItem.Text = "Record Now";
            this._recordNowToolStripMenuItem.Visible = false;
            // 
            // _takePhotoToolStripMenuItem
            // 
            this._takePhotoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("_takePhotoToolStripMenuItem.Image")));
            this._takePhotoToolStripMenuItem.Name = "_takePhotoToolStripMenuItem";
            this._takePhotoToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this._takePhotoToolStripMenuItem.Text = "Take Picture";
            this._takePhotoToolStripMenuItem.Click += new System.EventHandler(this._takePhotoToolStripMenuItem_Click);
            // 
            // _cropToolStripMenuItem
            // 
            this._cropToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("_cropToolStripMenuItem.Image")));
            this._cropToolStripMenuItem.Name = "_cropToolStripMenuItem";
            this._cropToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this._cropToolStripMenuItem.Text = "&Set crop";
            this._cropToolStripMenuItem.Click += new System.EventHandler(this._cropToolStripMenuItem_Click);
            // 
            // panelResult
            // 
            this.panelResult.AutoScroll = true;
            this.panelResult.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelResult.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelResult.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.panelResult.Location = new System.Drawing.Point(900, 0);
            this.panelResult.Name = "panelResult";
            this.panelResult.Size = new System.Drawing.Size(302, 740);
            this.panelResult.TabIndex = 28;
            this.panelResult.WrapContents = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(240)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.pnCamera8);
            this.panel1.Controls.Add(this.pnCamera4);
            this.panel1.Controls.Add(this.pnCamera7);
            this.panel1.Controls.Add(this.pnCamera6);
            this.panel1.Controls.Add(this.pnCamera3);
            this.panel1.Controls.Add(this.pnCamera5);
            this.panel1.Controls.Add(this.pnCamera2);
            this.panel1.Controls.Add(this.pnCamera1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 61);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 679);
            this.panel1.TabIndex = 29;
            // 
            // pnCamera8
            // 
            this.pnCamera8.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera8.Location = new System.Drawing.Point(256, 342);
            this.pnCamera8.Name = "pnCamera8";
            this.pnCamera8.Size = new System.Drawing.Size(186, 162);
            this.pnCamera8.TabIndex = 25;
            // 
            // pnCamera4
            // 
            this.pnCamera4.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera4.Location = new System.Drawing.Point(0, 174);
            this.pnCamera4.Name = "pnCamera4";
            this.pnCamera4.Size = new System.Drawing.Size(237, 162);
            this.pnCamera4.TabIndex = 22;
            // 
            // pnCamera7
            // 
            this.pnCamera7.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera7.Location = new System.Drawing.Point(7, 342);
            this.pnCamera7.Name = "pnCamera7";
            this.pnCamera7.Size = new System.Drawing.Size(213, 162);
            this.pnCamera7.TabIndex = 26;
            // 
            // pnCamera6
            // 
            this.pnCamera6.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera6.Location = new System.Drawing.Point(493, 174);
            this.pnCamera6.Name = "pnCamera6";
            this.pnCamera6.Size = new System.Drawing.Size(224, 162);
            this.pnCamera6.TabIndex = 24;
            // 
            // pnCamera3
            // 
            this.pnCamera3.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera3.Location = new System.Drawing.Point(486, 6);
            this.pnCamera3.Name = "pnCamera3";
            this.pnCamera3.Size = new System.Drawing.Size(213, 162);
            this.pnCamera3.TabIndex = 22;
            // 
            // pnCamera5
            // 
            this.pnCamera5.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera5.Location = new System.Drawing.Point(243, 174);
            this.pnCamera5.Name = "pnCamera5";
            this.pnCamera5.Size = new System.Drawing.Size(237, 162);
            this.pnCamera5.TabIndex = 23;
            // 
            // pnCamera2
            // 
            this.pnCamera2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera2.Location = new System.Drawing.Point(256, 6);
            this.pnCamera2.Name = "pnCamera2";
            this.pnCamera2.Size = new System.Drawing.Size(224, 162);
            this.pnCamera2.TabIndex = 21;
            // 
            // pnCamera1
            // 
            this.pnCamera1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera1.Location = new System.Drawing.Point(3, 6);
            this.pnCamera1.Name = "pnCamera1";
            this.pnCamera1.Size = new System.Drawing.Size(237, 162);
            this.pnCamera1.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(240)))), ((int)(((byte)(250)))));
            this.panel2.Controls.Add(this.circle1);
            this.panel2.Controls.Add(this.chk_autoANPR);
            this.panel2.Controls.Add(this.btn_connect);
            this.panel2.Controls.Add(this.progress_ANPR);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(900, 61);
            this.panel2.TabIndex = 41;
            // 
            // circle1
            // 
            this.circle1.BackColor = System.Drawing.Color.Transparent;
            this.circle1.IndexColor = System.Drawing.Color.Gray;
            this.circle1.Interval = 50;
            this.circle1.Location = new System.Drawing.Point(402, 13);
            this.circle1.Name = "circle1";
            this.circle1.NCircle = 8;
            this.circle1.Others = System.Drawing.Color.LightGray;
            this.circle1.Radius = 3;
            this.circle1.Size = new System.Drawing.Size(31, 36);
            this.circle1.TabIndex = 33;
            this.circle1.Text = "processingControl1";
            this.circle1.Visible = false;
            // 
            // chk_autoANPR
            // 
            this.chk_autoANPR.AutoSize = true;
            this.chk_autoANPR.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_autoANPR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.chk_autoANPR.Location = new System.Drawing.Point(14, 17);
            this.chk_autoANPR.Name = "chk_autoANPR";
            this.chk_autoANPR.Size = new System.Drawing.Size(63, 23);
            this.chk_autoANPR.TabIndex = 32;
            this.chk_autoANPR.Text = "ANPR";
            this.chk_autoANPR.UseVisualStyleBackColor = true;
            this.chk_autoANPR.CheckedChanged += new System.EventHandler(this.chk_autoANPR_CheckedChanged);
            // 
            // btn_connect
            // 
            this.btn_connect.Active1 = System.Drawing.Color.DodgerBlue;
            this.btn_connect.Active2 = System.Drawing.Color.DeepSkyBlue;
            this.btn_connect.BackColor = System.Drawing.Color.Transparent;
            this.btn_connect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_connect.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btn_connect.ForeColor = System.Drawing.Color.White;
            this.btn_connect.Inactive1 = System.Drawing.Color.DeepSkyBlue;
            this.btn_connect.Inactive2 = System.Drawing.Color.DodgerBlue;
            this.btn_connect.Location = new System.Drawing.Point(283, 13);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Radius = 10;
            this.btn_connect.Size = new System.Drawing.Size(110, 33);
            this.btn_connect.Stroke = false;
            this.btn_connect.StrokeColor = System.Drawing.Color.Gray;
            this.btn_connect.TabIndex = 31;
            this.btn_connect.Text = "Connect";
            this.btn_connect.Transparency = false;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // progress_ANPR
            // 
            this.progress_ANPR.Location = new System.Drawing.Point(83, 17);
            this.progress_ANPR.Maximum = 40;
            this.progress_ANPR.Name = "progress_ANPR";
            this.progress_ANPR.Size = new System.Drawing.Size(191, 23);
            this.progress_ANPR.TabIndex = 30;
            // 
            // FormCamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1202, 740);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelResult);
            this.Name = "FormCamera";
            this.Text = "FormCamera";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCamera_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCamera_FormClosed);
            this.Load += new System.EventHandler(this.FormCamera_Load);
            this.SizeChanged += new System.EventHandler(this.FormCamera_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.FormCamera_VisibleChanged);
            this.ctxtMnu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip ctxtMnu;
        private System.Windows.Forms.ToolStripMenuItem _recordNowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _takePhotoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _cropToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel panelResult;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnCamera1;
        private System.Windows.Forms.Panel pnCamera4;
        private System.Windows.Forms.Panel pnCamera3;
        private System.Windows.Forms.Panel pnCamera2;
        private System.Windows.Forms.Panel pnCamera8;
        private System.Windows.Forms.Panel pnCamera7;
        private System.Windows.Forms.Panel pnCamera6;
        private System.Windows.Forms.Panel pnCamera5;
        private System.Windows.Forms.Panel panel2;
        private AltoControls.ProcessingControl circle1;
        private System.Windows.Forms.CheckBox chk_autoANPR;
        private AltoControls.AltoButton btn_connect;
        private System.Windows.Forms.ProgressBar progress_ANPR;
    }
}