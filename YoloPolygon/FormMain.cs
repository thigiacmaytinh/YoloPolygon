﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGMTcs;

namespace YoloPolygon
{
    public partial class FormMain : Form
    {
        string m_imageDir = "";
        string m_labelDir = "";
        string m_classFile = "";
        string mCurrentImgName = "";

        double g_scaleX = 0;
        double g_scaleY = 0;
        double g_aspect = 0;

        //List<Polygon> m_scaledPolygons = new List<Polygon>();
        Polygon m_polygon = new Polygon();
        List<Polygon> m_polygons;
        //List<Rectangle> mRects = new List<Rectangle>();
        Size MAX_PICTURE_BOX_SIZE = new Size(800, 600);

        Image m_img;

        public enum Colision
        {
            None,
            NewRect,
            All,
            Top,
            Right,
            Bottom,
            Left,
            TopLeft,
            TopRight,
            BotLeft,
            BotRight
        }
        Colision g_colisionState = Colision.None;


        int ANCHOR_WIDTH = 10;
        Size ANCHOR_SIZE;
        Size mDistanceCurrentRectToMouse;
        Point mCurrentRectBottomRight;

        //public string textFilePath;
        Point currentPoint;
        Point startPoint;
        bool g_isMouseDown = false;


        int newPointIdx = -1;


        int mTotalRects = 0;
        bool m_isFirstLoading = true;

        //use for draw on picture box



        Font myFont;
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);

        string[] m_classes;
        string m_saveDir = "";
        int m_lastSearchIndex = 0;
        string m_lastSearch = "";

        bool m_isTextboxFocused = false;

        Pen blue_thick = new Pen(Color.Blue, 2);

        //Pen pen = new Pen(Color.Red, 2);
        Brush blue_brush = new SolidBrush(Color.FromArgb(100, Color.Blue));

        Pen red_pen = new Pen(Color.Red, 2);
        Brush red_brush = new SolidBrush(Color.FromArgb(100, Color.Red));
        Brush red_brushPoint = new SolidBrush(Color.FromArgb(255, Color.Red));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        public FormMain()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_polygons = new List<Polygon>();

            Control.CheckForIllegalCrossThreadCalls = false;
            ANCHOR_SIZE = new Size(ANCHOR_WIDTH, ANCHOR_WIDTH);

            LoadOption();

            txt_classFile.Text = TGMTregistry.GetInstance().ReadString("txt_classFile");
            txt_imageDir.Text = TGMTutil.CorrectPath(TGMTregistry.GetInstance().ReadString("txt_imageDir"));
            txt_labelDir.Text = TGMTutil.CorrectPath(TGMTregistry.GetInstance().ReadString("txt_labelDir"));
            


            this.KeyPreview = true;
            this.Text += " " + TGMTutil.GetVersion();

#if DEBUG
            this.Text += " (Debug)";
#endif

            LoadRecentDir();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                if(lstPolygon.Items.Count > 0)
                {
                    lstPolygon.SelectedIndex = 0;
                }
                if(lstImg.Items.Count > 0)
                {
                    lstImg.EnsureVisible(0);
                }
                
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void txt_imageDir_TextChanged(object sender, EventArgs e)
        {
            if (txt_imageDir.Text == "")
                return;

            m_imageDir = TGMTutil.CorrectPath(txt_imageDir.Text);
            if(chk_sameDir.Checked)
            {
                txt_labelDir.Text = txt_imageDir.Text;
            }
            LoadImage();

            TGMTregistry.GetInstance().SaveValue("txt_imageDir", txt_imageDir.Text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void txt_labelDir_TextChanged(object sender, EventArgs e)
        {
            if (txt_labelDir.Text == "")
                return;

            m_labelDir = TGMTutil.CorrectPath(txt_labelDir.Text);
            LoadImage();

            TGMTregistry.GetInstance().SaveValue("txt_labelDir", txt_labelDir.Text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void txt_classFile_TextChanged(object sender, EventArgs e)
        {
            if (txt_classFile.Text == "")
                return;

            m_classFile = txt_classFile.Text;
            LoadClasses();

            if(lstImg.Items.Count == 0)
            {
                LoadImage();
            }

            TGMTregistry.GetInstance().SaveValue("txt_classFile", txt_classFile.Text);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void chk_sameDir_CheckedChanged(object sender, EventArgs e)
        {
            txt_labelDir.Enabled = !chk_sameDir.Checked;

            if (chk_sameDir.Checked)
            {
                txt_labelDir.Text = txt_imageDir.Text;
            }

            TGMTregistry.GetInstance().SaveValue("chk_sameDir", chk_sameDir.Checked);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadRecentDir()
        {
            string recentDir = TGMTregistry.GetInstance().ReadString("recentDir");
            if (recentDir != "")
            {
                string[] recentDirs = recentDir.Split(';');

                if (recentDirs.Length > 0)
                {
                    
                }
            }
        }        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadOption()
        {
            int fontsize = TGMTregistry.GetInstance().ReadInt("numFontSize", 14);
            myFont = new Font("Arial", fontsize);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadClasses()
        {
            cb_classes.Items.Clear();
            cb_classID.Items.Clear();

            string ext = Path.GetExtension(m_classFile);
            if (ext != ".names" && ext != ".txt")
                return;


            if (File.Exists(m_classFile))
            {
                m_classes = File.ReadAllLines(m_classFile);
                if (m_classes.Length == 0)
                {
                    PrintError("File classes is empty");
                    return;
                }
                for (int i = 0; i < m_classes.Length; i++)
                {
                    cb_classes.Items.Add(m_classes[i]);
                    cb_classID.Items.Add(i.ToString());
                }
            }
            else
            {
                m_classes = new string[]{ "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train",
                    "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird",
                    "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella",
                    "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat",
                    "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork",
                    "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog",
                    "pizza", "donut", "cake", "chair", "couch", "potted plant", "bed", "dining table", "toilet", "tv",
                    "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink",
                    "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };
                cb_classes.Items.AddRange(m_classes);

                File.WriteAllLines(m_classFile, m_classes);
            }
            if(cb_classes.Items.Count > 0)
                cb_classes.SelectedIndex = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_isTextboxFocused)
                return;


            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveToFile();
            }
            else if (e.KeyCode == Keys.Space)
            {
                if (lstPolygon.Items.Count >= 1)
                {
                    lstPolygon.SelectedIndex = 0;
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (lstPolygon.SelectedIndex > -1)
                {
                    lstRect_KeyDown(sender, e);
                    return;
                }
            }
            else if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
            {
                if (lstPolygon.SelectedIndex > -1)
                {
                    int newClass = (int)e.KeyCode - 48;
                    if (newClass < cb_classes.Items.Count && newClass != cb_classes.SelectedIndex)
                    {
                        cb_classes.SelectedIndex = newClass;
                    }
                }
            }
            else if ((e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                if (lstPolygon.SelectedIndex > -1)
                {
                    int newClass = (int)e.KeyCode - 96;
                    if (newClass < cb_classes.Items.Count)
                    {
                        cb_classes.SelectedIndex = newClass;
                    }
                }
            }
            else if (e.KeyCode == Keys.A)
            {
                if (lstImg.SelectedIndices.Count > 0)
                {
                    int currentIdx = lstImg.SelectedIndices[0];
                    if (currentIdx > 0)
                    {

                        lstImg.Items[currentIdx - 1].Selected = true;
                        lstImg.EnsureVisible(currentIdx - 1);
                    }
                }
            }
            else if (e.KeyCode == Keys.D)
            {
                if (lstImg.SelectedIndices.Count > 0)
                {
                    int currentIdx = lstImg.SelectedIndices[0];
                    if (currentIdx < lstImg.Items.Count - 1)
                    {

                        lstImg.Items[currentIdx + 1].Selected = true;
                        lstImg.EnsureVisible(currentIdx + 1);
                    }
                }
            }
            else if (e.KeyCode == Keys.Q)
            {
                GotoLastNotAnnotated();
            }
            else if (e.KeyCode == Keys.E)
            {
                GotoNextNotAnnotated();
            }
            else if (e.KeyCode == Keys.F)
            {
                if (e.Control)
                {
                    SearchFile();
                }
            }
            else if (lstPolygon.SelectedIndex > -1)
            {
                //if (e.KeyCode == Keys.Up)
                //{
                //    if (e.Control) ResizeRect(0, -2); else MovePoint(0, -2);
                //}
                //else if (e.KeyCode == Keys.Down)
                //{
                //    if (e.Control) ResizeRect(0, 2); else MovePoint(0, 2);
                //}
                //else if (e.KeyCode == Keys.Left)
                //{
                //    if (e.Control) ResizeRect(-2, 0); else MovePoint(-2, 0);
                //}
                //else if (e.KeyCode == Keys.Right)
                //{
                //    if (e.Control) ResizeRect(2, 0); else MovePoint(2, 0);
                //}
                //else if (e.KeyCode == Keys.I)
                //{
                //    ResizeRect(0, -2);
                //}
                //else if (e.KeyCode == Keys.K)
                //{
                //    ResizeRect(0, 2);
                //}
                //else if (e.KeyCode == Keys.J)
                //{
                //    ResizeRect(-2, 0);
                //}
                //else if (e.KeyCode == Keys.L)
                //{
                //    ResizeRect(2, 0);
                //}
            }
            else
            {
                e.Handled = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //draw new rect
            if (g_isMouseDown)
            {
                if (g_colisionState == Colision.None)
                {
                    int w = currentPoint.X - startPoint.X;
                    int h = currentPoint.Y - startPoint.Y;
                    if (w > 0 && h > 0)
                    {
                        e.Graphics.DrawRectangle(Pens.Blue, startPoint.X, startPoint.Y, w, h);
                    }
                }
            }
            else
            {
                e.Graphics.DrawLine(Pens.Black, new Point(0, currentPoint.Y), new Point(pictureBox1.Width - 1, currentPoint.Y));
                e.Graphics.DrawLine(Pens.Black, new Point(currentPoint.X, 0), new Point(currentPoint.X, pictureBox1.Height - 1));
            }


            int offset = ANCHOR_WIDTH / 2;
            for (int i = 0; i < lstPolygon.Items.Count; i++)
            {
                string elements = lstPolygon.Items[i].ToString();
                int spaceIdx = elements.IndexOf(" ");
                int classID = Convert.ToInt32(elements.Substring(0, spaceIdx));
                string className = "Unknown";
                if (classID < cb_classes.Items.Count)
                    className = cb_classes.Items[classID].ToString();

                Polygon po = m_polygons[i];

                if (po.pointfs == null)
                    continue;

                className = cb_classes.Items[po.classID].ToString();


                if (i == lstPolygon.SelectedIndex)
                {
                    List<Point> drawPoints = ConvertToDrawPoint(po.pointfs);

                    

                    e.Graphics.DrawPolygon(blue_thick, drawPoints.ToArray());

                    e.Graphics.FillPolygon(blue_brush, drawPoints.ToArray());

                    for (int j = 0; j < drawPoints.Count; j++)
                    {
                        Point point = drawPoints[j];

                        e.Graphics.FillEllipse(j == lstPoint.SelectedIndex ? red_brushPoint : blue_brush, 
                            point.X - ANCHOR_WIDTH / 2, point.Y - ANCHOR_WIDTH / 2, ANCHOR_WIDTH, ANCHOR_WIDTH);
                
                    }

                    

                    e.Graphics.DrawString(className, myFont, redBrush, drawPoints[0].X, drawPoints[0].Y);
                }
                else
                {
                    List<Point> drawPoints = ConvertToDrawPoint(po.pointfs);
                    e.Graphics.DrawPolygon(blue_thick, drawPoints.ToArray());

                    e.Graphics.FillPolygon(blue_brush, drawPoints.ToArray());

                    e.Graphics.DrawString(className, myFont, redBrush, drawPoints[0].X, drawPoints[0].Y);

                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;

            timer1.Stop();

            currentPoint = Clamp(e.Location, new Point(0, 0), new Point(pictureBox1.Width, pictureBox1.Height));
            g_isMouseDown = false;
            //draw new rect
            if (g_colisionState == Colision.None)
            {
                int x = startPoint.X;
                int y = startPoint.Y;
                int w = Math.Abs(currentPoint.X - startPoint.X);
                int h = Math.Abs(currentPoint.Y - startPoint.Y);
                int cx = x + w / 2;
                int cy = y + h / 2;

                if (w <= 1 || h <= 1)
                {
                    return;
                }


                //if ((chkMinSize.Checked && Convert.ToInt32(w * g_scaleX) > numMinWidth.Value && Convert.ToInt32(h * g_scaleY) > numMinHeight.Value)
                //    || !chkMinSize.Checked)
                //{
                //mRects.Add(new Rectangle(x, y, w, h));
                lstPolygon.Items.Add(
                    cb_classes.SelectedIndex + " " +
                    cx * g_scaleX / m_img.Width + " " +
                    cy * g_scaleY / m_img.Height + " " +
                    w * g_scaleX / m_img.Width + " " +
                    h * g_scaleY / m_img.Height);
                lstPolygon.SelectedIndex = lstPolygon.Items.Count - 1;
                //}
                CompleteEdit();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;

            timer1.Start();
            startPoint = e.Location;
            g_isMouseDown = true;
            if (lstPolygon.SelectedIndex > -1)
            {
                Point point = GetCurrentPoint();
                mDistanceCurrentRectToMouse = new Size(e.Location.X - point.X, e.Location.Y - point.Y);
                //mCurrentRectBottomRight = new Point(rect.X + rect.Width, rect.Y + rect.Height);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;

            currentPoint = Clamp(e.Location, new Point(0, 0), new Point(pictureBox1.Width, pictureBox1.Height));
            int dx = currentPoint.X - startPoint.X;
            int dy = currentPoint.Y - startPoint.Y;


            if (g_isMouseDown)
            {
                if (g_colisionState == Colision.All)
                {
                    MovePoint(currentPoint);
                }
            }
            else //not mouse down
            {
                ChangeCursor(e.Location);
                pictureBox1.Refresh();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void GotoLastNotAnnotated()
        {
            if (lstImg.Items.Count == 0)
                return;

            int startIndex = lstImg.Items.Count - 1;
            if (lstImg.SelectedIndices.Count > 0)
                startIndex = lstImg.SelectedIndices[0] - 1;

            for (int i = startIndex; i > 0; i--)
            {
                string filePath = m_imageDir + lstImg.Items[i].Text;
                string txtPath = filePath.Replace(Path.GetExtension(filePath), ".txt");
                if (File.Exists(txtPath))
                {
                    string[] lines = File.ReadAllLines(txtPath);
                    if (lines.Length == 0)
                    {
                        lstImg.Items[i].Selected = true;
                        lstImg.EnsureVisible(i);
                        lstImg.Focus();
                        return;
                    }
                }
                else
                {
                    lstImg.Items[i].Selected = true;
                    lstImg.EnsureVisible(i);
                    lstImg.Focus();
                    return;
                }

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void GotoNextNotAnnotated()
        {
            int startIndex = 0;
            if (lstImg.SelectedIndices.Count > 0)
                startIndex = lstImg.SelectedIndices[0] + 1;
            for (int i = startIndex; i < lstImg.Items.Count; i++)
            {
                string filePath = TGMTutil.CorrectPath(m_imageDir) + lstImg.Items[i].Text;
                string txtPath = filePath.Replace(Path.GetExtension(filePath), ".txt");
                if (File.Exists(txtPath))
                {
                    string[] lines = File.ReadAllLines(txtPath);
                    if (lines.Length == 0)
                    {
                        lstImg.Items[i].Selected = true;
                        lstImg.EnsureVisible(i);
                        lstImg.Focus();
                        return;
                    }
                }
                else
                {
                    lstImg.Items[i].Selected = true;
                    lstImg.EnsureVisible(i);
                    lstImg.Focus();
                    return;
                }

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void DeleteFile()
        {
            if (lstImg.Items.Count == 0 || lstImg.SelectedIndices.Count == 0)
                return;

            int index = lstImg.SelectedIndices[0];

            string filePath = TGMTutil.CorrectPath(m_imageDir) + lstImg.Items[index].Text;
            string txtPath = filePath.Replace(Path.GetExtension(filePath), ".txt");
            TGMTfile.MoveFileToRecycleBin(filePath);
            TGMTfile.MoveFileToRecycleBin(txtPath);

            lstImg.Items[index].Remove();
            lstImg.Items[index].Selected = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void SearchFile()
        {
            if (txt_search.Text == "")
                return;

            if (txt_search.Text != m_lastSearch)
                m_lastSearchIndex = 0;
            m_lastSearch = txt_search.Text;
            if (m_lastSearchIndex >= lstImg.Items.Count)
                m_lastSearchIndex = 0;

            bool found = false;
            for (int i = m_lastSearchIndex; i < lstImg.Items.Count; i++)
            {
                if (lstImg.Items[i].Text.Contains(txt_search.Text))
                {
                    lstImg.Items[i].Selected = true;
                    lstImg.EnsureVisible(i);
                    found = true;
                    m_lastSearchIndex = i + 1;
                    break;
                }
            }

            if (!found)
            {
                m_lastSearchIndex = 0;
                MessageBox.Show("Not found");
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MovePoint(Point mouseLocation)
        {
            Point point = GetCurrentPoint();

            point.X = mouseLocation.X - mDistanceCurrentRectToMouse.Width;
            point.Y = mouseLocation.Y - mDistanceCurrentRectToMouse.Height;

            SetCurrentPoint(point);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MovePoint(int dx, int dy)
        {
            Point point = GetCurrentPoint();

            point.X += dx;
            point.Y += dy;

            SetCurrentPoint(point);
        }
       
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void CountTotalRect()
        {
            mTotalRects = lstPolygon.Items.Count;
            //for (int i = 0; i < lstImg.Items.Count; i++)
            //{
            //    string line = lstImg.Items[i].ToString();
            //    string[] lineSplit = line.Split(' ');
            //    if (lineSplit.Length > 1)
            //    {
            //        int count = 0;
            //        if (int.TryParse(lineSplit[1], out count))
            //            mTotalRects += count;
            //    }
            //}

            //lblTotalRect.Text = mTotalRects.ToString();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        Point Clamp(Point p, Point min, Point max)
        {
            if (p.X > max.X)
            {
                p.X = max.X;
            }
            else if (p.X < min.X)
            {
                p.X = min.X;
            }

            if (p.Y > max.Y)
            {
                p.Y = max.Y;
            }
            else if (p.Y < min.Y)
            {
                p.Y = min.Y;
            }
            return p;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void PrintError(string message)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = message;
            timerClear.Start();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void PrintSuccess(string message)
        {
            lblMessage.ForeColor = Color.Green;
            lblMessage.Text = message;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void PrintMessage(string message)
        {
            lblMessage.ForeColor = Color.Black;
            lblMessage.Text = message;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void CompleteEdit()
        {
            //write rect to list image
            if (lstImg.SelectedItems.Count == 0)
                return;

            lstImg.Items[lstImg.SelectedIndices[0]].ForeColor = Color.Black;
            CountTotalRect();
            SaveToFile();
            this.Cursor = Cursors.Default;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        //write list image to file
        void SaveToFile()
        {
            if (lstImg.Items.Count == 0 || lstImg.SelectedIndices.Count == 0)
                return;

            TGMTregistry.GetInstance().SaveValue("imageIdx", lstImg.SelectedIndices[0]);

            lblMessage.Text = "Saving...";

            string content = "";
            foreach (string item in lstPolygon.Items)
            {
                content += item + "\n";
            }
            if (content.Length > 0)
                content = content.Substring(0, content.Length - 1);

            if (content != "")
            {
                string txtPath = m_labelDir + Path.GetFileNameWithoutExtension(mCurrentImgName) + ".txt";
                File.WriteAllText(txtPath, content);
            }

            PrintSuccess("Saved");
            timerClear.Start();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void bgLoadFile_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(m_imageDir))
            {
                MessageBox.Show("Không tìm thấy folder:" + m_imageDir);
                return;
            }


            var allowedExtensions = new[] { ".jpg", ".png", ".bmp", ".JPG", ".PNG", ".BMP" };
            var fileList = Directory.GetFiles(m_imageDir)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith)).ToList();


            lstImg.Items.Clear();
            List<int> listNoTxt = new List<int>();
            int index = 0;

            lstImg.BeginUpdate();


            foreach (string filePath in fileList)
            {
                string fileName = Path.GetFileName(filePath);
                lstImg.Items.Add(fileName);

                string txtPath = m_labelDir + Path.GetFileNameWithoutExtension(filePath) + ".txt";

                if (!File.Exists(txtPath))
                {
                    listNoTxt.Add(index);
                    lstImg.Items[index].ForeColor = Color.Red;
                }
                index++;
            }


            lstImg.EndUpdate();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void bgLoadFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CountTotalRect();
            this.Enabled = true;
            timerLoading.Stop();
            progressBar1.Value = progressBar1.Minimum;
            lblMessage.Text = "";

            if (m_isFirstLoading && lstImg.Items.Count > 0)
            {
                int imageIdx = TGMTregistry.GetInstance().ReadInt("imageIdx");
                if (imageIdx >= 0 && imageIdx <= lstImg.Items.Count)
                {
                    lstImg.Items[imageIdx].Selected = true;
                    lstImg.EnsureVisible(imageIdx);
                }
                m_isFirstLoading = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Cursor == Cursors.SizeAll && newPointIdx < lstPoint.Items.Count)
            {
                lstPoint.SelectedIndex = newPointIdx;
                lstPoint.Focus();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void lstRect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (lstPolygon.Items.Count > 0 && lstPolygon.SelectedIndex > -1)
                {
                    int index = lstPolygon.SelectedIndex;
                    lstPolygon.Items.RemoveAt(index);
                    //mRects.RemoveAt(index);

                    if (index > -1 & index < lstPolygon.Items.Count)
                    {
                        lstPolygon.SelectedIndex = index;
                    }
                    else if (index == lstPolygon.Items.Count)
                    {
                        lstPolygon.SelectedIndex = lstPolygon.Items.Count - 1;
                    }
                    CompleteEdit();
                    pictureBox1.Refresh();
                }

                this.Cursor = Cursors.Default;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                CompleteEdit();
                lstPolygon.SelectedIndex = -1;
                if (lstImg.SelectedIndices.Count > 0)
                {
                    int nextIndex = lstImg.SelectedIndices[0] + 1;
                    if (nextIndex < lstImg.Items.Count)
                    {
                        lstImg.Items[nextIndex].Selected = true;
                        lstImg.EnsureVisible(nextIndex);
                    }
                    lstImg.Focus();
                    e.Handled = true;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_openFolder_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Select folder contain image";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                m_imageDir = TGMTutil.CorrectPath(folderPath);

                if (m_imageDir.Contains(" "))
                {
                    MessageBox.Show("Folder không được có khoảng trắng hay ký tự đặc biệt");
                    return;
                }


                if (!Directory.Exists(m_imageDir))
                {
                    MessageBox.Show("Folder không tồn tại");
                    return;
                }
                LoadImage();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadImage()
        {
            if (m_imageDir == "" || m_labelDir == "" || m_classFile == "")
                return;

            lstImg.Items.Clear();
            lstPolygon.Items.Clear();

            this.Enabled = false;
            timerLoading.Start();
            lblMessage.Text = "Loading file...";



            string recentDir = TGMTregistry.GetInstance().ReadString("recentDir");
            if (recentDir == "")
            {
                recentDir += m_imageDir;
            }                
            else
            {
                if (!recentDir.Contains(m_imageDir))
                    recentDir += ";" + m_imageDir;
            }


            TGMTregistry.GetInstance().SaveValue("recentDir", recentDir);


            if (!bgLoadFile.IsBusy)
            {
                bgLoadFile.RunWorkerAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadPolygon()
        {
            errorProvider1.Clear();
            if (lstImg.Items.Count == 0 || lstImg.SelectedIndices.Count == 0)
                return;

            if (string.IsNullOrEmpty(m_imageDir))
            {
                MessageBox.Show("Không tìm thấy folder");
                return;
            }


            string imgName = lstImg.SelectedItems[0].Text.ToString();
            if (imgName == mCurrentImgName)
                return;

            mCurrentImgName = imgName;


            string imgPath = m_imageDir + mCurrentImgName;
            if (!File.Exists(imgPath))
            {
                //ErrorProvider1.SetError(lblLstImg, "Image selected not exist");
                pictureBox1.Image = null;
                return;
            }




            //clear
            lstPolygon.Items.Clear();
            //mRects.Clear();


            m_img = TGMTimage.LoadBitmapWithoutLock(imgPath);
            pictureBox1.Image = m_img;
            PrintMessage("Image " + (lstImg.SelectedIndices[0] + 1) + " / " + lstImg.Items.Count);


            g_aspect = (double)m_img.Width / (double)m_img.Height;

            //resize
            if (g_aspect > 4.0 / 3.0)
            {
                pictureBox1.Width = MAX_PICTURE_BOX_SIZE.Width;
                pictureBox1.Height = (int)(MAX_PICTURE_BOX_SIZE.Width / g_aspect);
            }
            else if (g_aspect < 4.0 / 3.0)
            {
                pictureBox1.Height = MAX_PICTURE_BOX_SIZE.Height;
                pictureBox1.Width = (int)(MAX_PICTURE_BOX_SIZE.Height * g_aspect);
            }
            g_scaleX = (double)m_img.Width / pictureBox1.Width;
            g_scaleY = (double)m_img.Height / pictureBox1.Height;

            string txtPath = m_labelDir + mCurrentImgName.Replace(Path.GetExtension(mCurrentImgName), ".txt");

            if (File.Exists(txtPath))
            {
                string[] lines = File.ReadAllLines(txtPath);

                m_polygons.Clear();
                //m_scaledPolygons.Clear();

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line == "")
                        continue;

                    string[] lineSplit = line.Split(' ');

                    //add polygon with real data
                    Polygon po =  LoadPolygon(lineSplit, true);
                    m_polygons.Add(po);
                    
                    //add polygon multiple with scale ratio
                    //Polygon scaledPo = LoadPolygon(lineSplit, true);
                    //m_scaledPolygons.Add(scaledPo);


                    lstPolygon.Items.Add(line);   
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        Polygon LoadPolygon(string[] lineSplit, bool multipleScale)
        {
            Polygon po = new Polygon();

            if (lineSplit.Length <= 5)
                return po;

            double scaleX = 1;
            double scaleY = 1;
            double width = 1;
            double height = 1;

            if (multipleScale)
            {
                scaleX = g_scaleX;
                scaleY = g_scaleY;
                width = m_img.Width;
                height = m_img.Height;
            }

            int classID = int.Parse(lineSplit[0]);



            po.classID = classID;
            po.cxf = double.Parse(lineSplit[1]);
            po.cx = (int)(po.cxf * width / scaleX);
            po.cyf = double.Parse(lineSplit[2]);
            po.cy = (int)(po.cyf * height / scaleY);

            po.wf = double.Parse(lineSplit[3]);
            po.w =  (int)(po.wf * width / scaleX);
            po.hf = double.Parse(lineSplit[4]);
            po.h = (int)(po.hf *height / scaleY);


            po.drawPoints = new List<Point>();
            po.pointfs = new List<PointF>();

            for (int i = 5; i < lineSplit.Length; i += 2)
            {
                Point p = new Point();
                p.X = (int)(double.Parse(lineSplit[i]) * width / scaleX);
                p.Y = (int)(double.Parse(lineSplit[i + 1]) * height / scaleY);
                po.drawPoints.Add(p);


                PointF pf = new PointF();
                pf.X = (float)(double.Parse(lineSplit[i]));
                pf.Y = (float)(double.Parse(lineSplit[i + 1]));
                po.pointfs.Add(pf);
            }

            return po;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        Colision CheckColision(Point cursor, Point drawPoint)
        {
            double deltaX = cursor.X - drawPoint.X;
            double deltaY = cursor.Y - drawPoint.Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            double offset = ANCHOR_WIDTH / 2;

            if (distance <= offset)            
                return Colision.All;           
            
            
            return Colision.None;            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        Point GetCurrentPoint()
        {
            if (lstPoint.SelectedIndex == -1 && lstPoint.Items.Count > 0)
                lstPoint.SelectedIndex = 0;
            return m_polygon.drawPoints[lstPoint.SelectedIndex];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void SetCurrentPoint(Point point)
        {
            m_polygon.drawPoints[lstPoint.SelectedIndex] = point;
            m_polygon.pointfs[lstPoint.SelectedIndex] = ConvertToRealPoint(m_polygon.drawPoints[lstPoint.SelectedIndex]);

            lstPolygon.Items[lstPolygon.SelectedIndex] = m_polygon.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void ChangeCursor(Point p)
        {
            if (lstPolygon.Items.Count == 0)
            {
                g_colisionState = Colision.None;
                return;   
            }
            if(lstPolygon.SelectedIndex < 0)
            {
                g_colisionState = Colision.None;
                return;
            }
                

            if (p.X > pictureBox1.Width || p.Y > pictureBox1.Height)
            {
                this.Cursor = Cursors.Default;
                return;
            }

            Polygon selectedPolygon = m_polygons[lstPolygon.SelectedIndex];

            List<Point> drawPoints = ConvertToDrawPoint(selectedPolygon.pointfs);


            for (int i = 0; i < drawPoints.Count; i++)
            {
                
                g_colisionState = CheckColision(p, drawPoints[i]);
                if (g_colisionState == Colision.All)
                    this.Cursor = Cursors.SizeAll;
                else if (g_colisionState == Colision.TopLeft || g_colisionState == Colision.BotRight)
                    this.Cursor = Cursors.SizeNWSE;
                else if (g_colisionState == Colision.TopRight || g_colisionState == Colision.BotLeft)
                    this.Cursor = Cursors.SizeNESW;
                else if (g_colisionState == Colision.Left || g_colisionState == Colision.Right)
                    this.Cursor = Cursors.SizeWE;
                else if (g_colisionState == Colision.Top || g_colisionState == Colision.Bottom)
                    this.Cursor = Cursors.SizeNS;
                else if (g_colisionState == Colision.NewRect)
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.Default;


                if (g_colisionState != Colision.None)
                {
                    if (i != lstPoint.SelectedIndex)
                    {
                        this.Cursor = Cursors.SizeAll;
                    }
                    newPointIdx = i;
                    return;
                }
            }
            this.Cursor = Cursors.Default;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void lstRect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPolygon.SelectedIndex < 0 || lstPolygon.SelectedIndex > lstPolygon.Items.Count)
                return;

            if (pictureBox1.Image == null)
                return;

            string[] elements = lstPolygon.SelectedItem.ToString().Split(' ');
            int classIdx = Convert.ToInt32(elements[0]);
            if (classIdx < cb_classes.Items.Count)
            {
                cb_classes.SelectedIndex = classIdx;
                cb_classID.SelectedIndex = classIdx;
            }

            cb_classes.Enabled = true;
            cb_classID.Enabled = true;

            m_polygon = m_polygons[lstPolygon.SelectedIndex];

            ShowPoints(m_polygon);

            pictureBox1.Refresh();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void cb_classes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPolygon.SelectedIndex < 0)
                return;
            
            string elements = lstPolygon.Items[lstPolygon.SelectedIndex].ToString();
            int spaceIdx = elements.IndexOf(" ");

            string currentClass = elements.Substring(0, spaceIdx);
            string newClass = cb_classes.SelectedIndex.ToString();
            if (currentClass != newClass)
            {
                elements = newClass + elements.Substring(spaceIdx);
                lstPolygon.Items[lstPolygon.SelectedIndex] = elements;
                CompleteEdit();
            }

            lstPolygon.Focus();
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void cb_classID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPolygon.SelectedIndex < 0)
                return;

            
            string elements = lstPolygon.Items[lstPolygon.SelectedIndex].ToString();
            int spaceIdx = elements.IndexOf(" ");

            string currentClass = elements.Substring(0, spaceIdx);
            cb_classes.SelectedIndex = cb_classID.SelectedIndex;
            string newClassID = cb_classID.SelectedItem.ToString();
            if (currentClass != newClassID)
            {
                elements = newClassID + elements.Substring(spaceIdx);
                lstPolygon.Items[lstPolygon.SelectedIndex] = elements;
                CompleteEdit();
            }

            lstPolygon.Focus();            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void bgCrop_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < m_classes.Length; i++)
                {
                    string classDir = m_saveDir + m_classes[i];
                    if (!Directory.Exists(classDir))
                        Directory.CreateDirectory(classDir);
                }



                int count = 0;
                int totalImage = lstImg.Items.Count;
                for (int i = 0; i < totalImage; i++)
                {
                    string fileName = lstImg.Items[i].Text;
                    string filePath = m_imageDir + fileName;
                    string txtPath = filePath.Replace(Path.GetExtension(filePath), ".txt");

                    if (!File.Exists(txtPath))
                    {
                        continue;
                    }
                    string[] lines = File.ReadAllLines(txtPath);


                    Bitmap bmp = TGMTimage.LoadBitmapWithoutLock(filePath);

                    for (int j = 0; j < lines.Length; j++)
                    {
                        string line = lines[j];
                        string[] elements = line.Split(' ');
                        if (elements.Length != 5)
                            continue;

                        int classID = Convert.ToInt32(elements[0]);

                        if (classID >= m_classes.Length)
                        {
                            if (MessageBox.Show("File " + txtPath + " wrong class ID", "Open text file?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Process.Start(txtPath);
                            }
                            return;
                        }
                        string className = m_classes[classID];



                        double cx = Convert.ToDouble(elements[1]);
                        double cy = Convert.ToDouble(elements[2]);
                        double w = Convert.ToDouble(elements[3]);
                        double h = Convert.ToDouble(elements[4]);
                        double x = cx - w / 2;
                        double y = cy - h / 2;

                        int ix = Convert.ToInt32(x * bmp.Width);
                        int iy = Convert.ToInt32(y * bmp.Height);
                        int iw = Convert.ToInt32(w * bmp.Width);
                        int ih = Convert.ToInt32(h * bmp.Height);


                        if (iw > bmp.Width || ih > bmp.Height || ix + iw > bmp.Width || iy + ih > bmp.Height)
                        {
                            if (MessageBox.Show("File " + txtPath + " wrong size", "Open text file?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Process.Start(txtPath);
                            }
                            continue;
                        }

                        Rectangle cropRect = new Rectangle(ix, iy, iw, ih);
                        Bitmap cropBmp = TGMTimage.CropBitmap(bmp, cropRect);
                        string savePath = String.Format("{0}{1}\\{2}_{3}.jpg", m_saveDir, className, Path.GetFileNameWithoutExtension(fileName), j);
                        cropBmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        cropBmp.Dispose();
                        cropBmp = null;
                        count++;
                    }

                    bmp.Dispose();
                    bmp = null;

                    int percentComplete = (int)((float)i / (float)totalImage * 100);
                    bgCrop.ReportProgress(percentComplete);

                }
                e.Result = "Crop " + count + " objects on " + totalImage + " total image";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void lstImg_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb_classes.Enabled = false;
            cb_classID.Enabled = false;

            LoadPolygon();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void txt_search_TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchFile();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void lstImg_KeyDown(object sender, KeyEventArgs e)
        {
            int selectedIndex = lstImg.SelectedIndices[0];
            if (e.KeyCode == Keys.Delete)
            {
                if (lstImg.SelectedIndices.Count > 0 && lstPolygon.SelectedIndex == -1)
                {
                    int index = lstImg.SelectedIndices[0];
                    string imagePath = m_imageDir + lstImg.Items[index].Text;
                    string txtPath = m_labelDir + Path.GetFileNameWithoutExtension(imagePath) + ".txt";


                    lstImg.Items.RemoveAt(lstImg.SelectedIndices[0]);
                    lstPolygon.Items.Clear();

                    TGMTfile.MoveFileToRecycleBin(imagePath);
                    TGMTfile.MoveFileToRecycleBin(txtPath);

                    if (index < lstImg.Items.Count)
                    {
                        lstImg.Items[index].Selected = true;
                    }
                    else
                    {
                        lstImg.Items[lstImg.Items.Count - 1].Selected = true;
                    }
                }
                this.Cursor = Cursors.Default;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (lstPolygon.SelectedIndex > -1)
                {
                    e.Handled = true;
                    return;
                }

                if (selectedIndex > 0)
                {
                    lstImg.Items[selectedIndex - 1].Selected = true;
                    lstImg.EnsureVisible(selectedIndex - 1);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                return;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (lstPolygon.SelectedIndex > -1)
                {
                    e.Handled = true;
                    return;
                }

                if (selectedIndex < lstImg.Items.Count - 1)
                {
                    lstImg.Items[selectedIndex + 1].Selected = true;
                    lstImg.EnsureVisible(selectedIndex + 1);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                return;
            }
            else
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void timerLoading_Tick(object sender, EventArgs e)
        {
            progressBar1.Value++;
            if (progressBar1.Value >= progressBar1.Maximum)
                progressBar1.Value = progressBar1.Minimum;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void bgCrop_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            PrintMessage(e.ProgressPercentage + "%");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void bgCrop_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = progressBar1.Minimum;
            PrintMessage("");
            MessageBox.Show(e.Result.ToString(), "Crop success");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_search_Click(object sender, EventArgs e)
        {
            SearchFile();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_imageNoLabel_Click(object sender, EventArgs e)
        {
            bool found = false;

            for (int i = 0; i < lstImg.Items.Count; i++)
            {
                string txtPath = m_labelDir + lstImg.Items[i].Text.Replace(Path.GetExtension(mCurrentImgName), ".txt");
                if (!File.Exists(txtPath))
                {
                    found = true;
                    lstImg.EnsureVisible(i);
                    break;
                }
                string[] lines = File.ReadAllLines(txtPath);
                if (lines.Length == 0)
                {
                    found = true;
                    lstImg.EnsureVisible(i);
                    break;
                }
            }

            if (!found)
                PrintError("Not found");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_addClass_Click(object sender, EventArgs e)
        {
            string newClass = InputBox.Show("Add new class");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void timerClear_Tick(object sender, EventArgs e)
        {
            timerClear.Stop();
            lblMessage.Text = "";
        }

        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_about_Click(object sender, EventArgs e)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        void ShowPoints(Polygon po)
        {
            lstPoint.Items.Clear();

            for(int i=0; i<po.drawPoints.Count; i++)
            {
                PointF p = po.pointfs[i];
                lstPoint.Items.Add(p.X.ToString() + " " + p.Y.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<Point> ConvertToDrawPoint(List<PointF> pointfs)
        {

            List <Point> points = new List<Point>();
            for(int i=0; i<pointfs.Count; i++)
            {
                PointF pf = pointfs[i];
                Point p = new Point();
                p.X = (int)(pf.X * m_img.Width / g_scaleX);
                p.Y = (int)(pf.Y * m_img.Height / g_scaleY);


                points.Add(p);
            }

            return points;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        Point ConvertToDrawPoint(PointF pf)
        {
            Point p = new Point();
            p.X = (int)(pf.X * m_img.Width / g_scaleX);
            p.Y = (int)(pf.Y * m_img.Height / g_scaleY);

            return p;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        List<PointF> ConvertToRealPoint(List<Point> points)
        {

            List<PointF> pointfs = new List<PointF>();
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                PointF pf = new PointF();

                pf.X = (float)((double)p.X / m_img.Width * g_scaleX);
                pf.Y = (float)((double)p.Y / m_img.Height * g_scaleY);


                pointfs.Add(pf);
            }

            return pointfs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        PointF ConvertToRealPoint(Point point)
        {
            PointF pf = new PointF();

            pf.X = (float)((double)point.X / m_img.Width * g_scaleX);
            pf.Y = (float)((double)point.Y / m_img.Height * g_scaleY);

            return pf;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void lstPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }
    }
}

