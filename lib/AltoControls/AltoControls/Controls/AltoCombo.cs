﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace AltoControls
{
    [DefaultEvent("TextChanged")]

    public class AltoCombo : Control
    {
        int radius = 4;
        public ComboBox box = new ComboBox();
        GraphicsPath shape;
        GraphicsPath innerRect;
        Color br;

        Color m_borderColor = Color.FromArgb(232, 240, 250);


        public AltoCombo()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            box.Parent = this;
            Controls.Add(box);

            //box.BorderStyle = BorderStyle.None;
            //box.TextAlign = HorizontalAlignment.Left;
            box.Font = Font;

            BackColor = Color.Transparent;
            ForeColor = SystemColors.WindowText;
            br = Color.White;
            box.BackColor = br;
            Padding = new Padding(5);
            Text = null;
            Font = new Font("Segoe UI", 10);
            Size = new Size(135, 30);
            DoubleBuffered = true;
            box.KeyDown += box_KeyDown;
            box.TextChanged += box_TextChanged;
            box.MouseDoubleClick += box_MouseDoubleClick;
            //box.PasswordChar = '\0';

          
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            this.Text = "";

            OpenFileDialog folderBrowser = new OpenFileDialog();
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Select folder";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                this.Text = folderPath;
            }
        }

        void box_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            box.SelectAll();
        }

        void box_TextChanged(object sender, EventArgs e)
        {
            Text = box.Text;
        }
        public void SelectAll()
        {
            box.SelectAll();
        }
        void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                box.SelectionStart = 0;
                box.SelectionLength = Text.Length;
            }
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            box.Text = Text;
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            box.Font = Font;
            Invalidate();
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            box.ForeColor = ForeColor;
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            shape = new RoundedRectangleF(Width, Height, radius).Path;


            innerRect = new RoundedRectangleF(Width - 0.5f, Height - 0.5f, radius, 0.5f, 0.5f).Path;
            if (box.Height >= Height - 4)
                Height = box.Height + 4;
            box.Location = new Point(radius , Height / 2 - box.Font.Height / 2);
            box.Width = Width - (int)(radius * 1.5) - this.Padding.Right;

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Bitmap bmp = new Bitmap(Width, Height);
            Graphics grp = Graphics.FromImage(bmp);

            Pen p = new Pen(m_borderColor);
            e.Graphics.DrawPath(p, shape);
            using (SolidBrush brush = new SolidBrush(br))
                e.Graphics.FillPath(brush, innerRect);
            Transparenter.MakeTransparent(this, e.Graphics);

            base.OnPaint(e);
        }
        public Color BackgroundColor
        {
            get
            {
                return br;
            }
            set
            {
                br = value;
                if (br != Color.Transparent)
                    box.BackColor = br;
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                m_borderColor = value;                
                Invalidate();
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = Color.Transparent;
            }
        }
    }
}
