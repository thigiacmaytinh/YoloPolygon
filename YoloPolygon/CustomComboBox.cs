using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoloPolygon
{
    public class RoundedComboBox : UserControl
    {
        private ComboBox comboBox;
        private Color _borderColor = Color.MediumSlateBlue;
        private int _borderSize = 1;
        private int _cornerRadius = 10;

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        public int BorderSize
        {
            get { return _borderSize; }
            set { _borderSize = value; Invalidate(); }
        }

        public int CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = value; Invalidate(); }
        }

        public ComboBoxStyle DropDownStyle
        {
            get { return comboBox.DropDownStyle; }
            set { comboBox.DropDownStyle = value; }
        }

        public override string Text
        {
            get { return comboBox.Text; }
            set { comboBox.Text = value; }
        }

        public object DataSource
        {
            get { return comboBox.DataSource; }
            set { comboBox.DataSource = value; }
        }

        public object SelectedItem
        {
            get { return comboBox.SelectedItem; }
            set { comboBox.SelectedItem = value; }
        }

        public RoundedComboBox()
        {
            comboBox = new ComboBox();
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Location = new Point(_borderSize, _borderSize);
            comboBox.Size = this.ClientSize;
            this.Controls.Add(comboBox);

            this.Size = new Size(200, 30);
            this.Padding = new Padding(_borderSize);
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw the rounded rectangle
            using (GraphicsPath path = new GraphicsPath())
            {
                Rectangle rect = this.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                path.AddArc(rect.X, rect.Y, _cornerRadius, _cornerRadius, 180, 90);
                path.AddArc(rect.Right - _cornerRadius, rect.Y, _cornerRadius, _cornerRadius, 270, 90);
                path.AddArc(rect.Right - _cornerRadius, rect.Bottom - _cornerRadius, _cornerRadius, _cornerRadius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - _cornerRadius, _cornerRadius, _cornerRadius, 90, 90);
                path.CloseFigure();

                using (Pen pen = new Pen(_borderColor, _borderSize))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            comboBox.Size = this.ClientSize - new Size(_borderSize * 2, _borderSize * 2);
        }
    }
}
