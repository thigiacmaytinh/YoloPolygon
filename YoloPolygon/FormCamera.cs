using MovingANPR.DBmgr;
using MovingANPR.UC;
using MovingANPR.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TGMT;
using TGMTcs;
using TGMTplayer.Controls;

namespace MovingANPR
{
    public partial class FormCamera : Form
    {
        static FormCamera m_instance;

        CameraPanel m_cameraPanel1;
        CameraPanel m_cameraPanel2;
        CameraPanel m_cameraPanel3;
        CameraPanel m_cameraPanel4;
        CameraPanel m_cameraPanel5;
        CameraPanel m_cameraPanel6;
        CameraPanel m_cameraPanel7;
        CameraPanel m_cameraPanel8;
        

        CameraPanel m_currentCameraPanel;
        public bool m_autoANPR = false;

        public PictureBox[] m_ImageBoxs;
        float m_aspect = -1.0f;


        List<string> m_alphanumerics = new List<string>();
        Quad[] m_quads = new Quad[8];

        private readonly object _bitmapLock1 = new object();
        private readonly object _bitmapLock2 = new object();
        private readonly object _bitmapLock3 = new object();
        private readonly object _bitmapLock4 = new object();
        private readonly object _bitmapLock5 = new object();
        private readonly object _bitmapLock6 = new object();
        private readonly object _bitmapLock7 = new object();
        private readonly object _bitmapLock8 = new object();

        private readonly object _panelLock = new object();
        private readonly object _lastTimeDetectLock = new object();
        private readonly object _dbLock = new object();

        bool m_isConnecting = false;
        DateTime m_lastTimeDetect = DateTime.Now;

        public bool m_isBusy1 = false;
        public bool m_isBusy2 = false;
        public bool m_isBusy3 = false;
        public bool m_isBusy4 = false;
        public bool m_isBusy5 = false;
        public bool m_isBusy6 = false;
        public bool m_isBusy7 = false;
        public bool m_isBusy8 = false;

        bool m_formClosed = false;

        BackgroundWorker m_worker1 = new BackgroundWorker();
        BackgroundWorker m_worker2 = new BackgroundWorker();
        BackgroundWorker m_worker3 = new BackgroundWorker();
        BackgroundWorker m_worker4 = new BackgroundWorker();
        BackgroundWorker m_worker5 = new BackgroundWorker();
        BackgroundWorker m_worker6 = new BackgroundWorker();
        BackgroundWorker m_worker7 = new BackgroundWorker();
        BackgroundWorker m_worker8 = new BackgroundWorker();

        public DateTime LastTimeDetect
        {
            get {
                DateTime lastTimeDetect;
                lock (_lastTimeDetectLock) {
                    lastTimeDetect = m_lastTimeDetect;
                }
                return lastTimeDetect;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public FormCamera()
        {
            InitializeComponent();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static FormCamera GetInstance()
        {
            if (m_instance == null)
                m_instance = new FormCamera();
            return m_instance;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormCamera_Load(object sender, EventArgs e)
        {
            chk_autoANPR.Checked = TGMTregistry.GetInstance().ReadBool("chk_autoStart");           

            m_autoANPR = chk_autoANPR.Checked;

            Program.enableCamera1 = TGMTregistry.GetInstance().ReadBool("enableCamera1", Program.enableCamera1);
            Program.enableCamera2 = TGMTregistry.GetInstance().ReadBool("enableCamera2", Program.enableCamera2);
            Program.enableCamera3 = TGMTregistry.GetInstance().ReadBool("enableCamera3", Program.enableCamera3);
            Program.enableCamera4 = TGMTregistry.GetInstance().ReadBool("enableCamera4", Program.enableCamera4);
            Program.enableCamera5 = TGMTregistry.GetInstance().ReadBool("enableCamera5", Program.enableCamera5);
            Program.enableCamera6 = TGMTregistry.GetInstance().ReadBool("enableCamera6", Program.enableCamera6);
            Program.enableCamera7 = TGMTregistry.GetInstance().ReadBool("enableCamera7", Program.enableCamera7);
            Program.enableCamera8 = TGMTregistry.GetInstance().ReadBool("enableCamera8", Program.enableCamera8);

            Program.cameras = new List<string>(8);
            for (int i = 0; i < Program.cameras.Capacity; i++)
            {
                Program.cameras.Add(TGMTregistry.GetInstance().ReadString("camera" + i.ToString()));
            }

            InitCameraPanels();

            m_worker1.WorkerSupportsCancellation = true;
            m_worker1.DoWork += worker_DoWork1;
            m_worker1.RunWorkerCompleted += worker_RunWorkerCompleted1;

            m_worker2.WorkerSupportsCancellation = true;
            m_worker2.DoWork += worker_DoWork2;
            m_worker2.RunWorkerCompleted += worker_RunWorkerCompleted2;

            m_worker3.WorkerSupportsCancellation = true;
            m_worker3.DoWork += worker_DoWork3;
            m_worker3.RunWorkerCompleted += worker_RunWorkerCompleted3;

            m_worker4.WorkerSupportsCancellation = true;
            m_worker4.DoWork += worker_DoWork4;
            m_worker4.RunWorkerCompleted += worker_RunWorkerCompleted4;

            m_worker5.WorkerSupportsCancellation = true;
            m_worker5.DoWork += worker_DoWork5;
            m_worker5.RunWorkerCompleted += worker_RunWorkerCompleted5;

            m_worker6.WorkerSupportsCancellation = true;
            m_worker6.DoWork += worker_DoWork6;
            m_worker6.RunWorkerCompleted += worker_RunWorkerCompleted6;

            m_worker7.WorkerSupportsCancellation = true;
            m_worker7.DoWork += worker_DoWork7;
            m_worker7.RunWorkerCompleted += worker_RunWorkerCompleted7;

            m_worker8.WorkerSupportsCancellation = true;
            m_worker8.DoWork += worker_DoWork8;
            m_worker8.RunWorkerCompleted += worker_RunWorkerCompleted8;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormCamera_VisibleChanged(object sender, EventArgs e)
        {
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void FormCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_formClosed = true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormCamera_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisconnectCameras();

            m_cameraPanel1 = null;
            m_cameraPanel2 = null;
            m_cameraPanel3 = null;
            m_cameraPanel4 = null;
            m_cameraPanel5 = null;
            m_cameraPanel6 = null;
            m_cameraPanel7 = null;
            m_cameraPanel8 = null;            
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (btn_connect.Text == "Connect")
            {
                ConnectCameras();
                chk_autoANPR.Enabled = true;
                m_formClosed = false;
            }
            else
            {
                DisconnectCameras();

                circle1.Visible = false;
                btn_connect.Text = "Connect";

                chk_autoANPR.Enabled = false;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void chk_autoANPR_CheckedChanged(object sender, EventArgs e)
        {
            m_autoANPR = chk_autoANPR.Checked;
            TGMTregistry.GetInstance().SaveValue("chk_autoStart", chk_autoANPR.Checked);

            if (m_autoANPR)
            {
                m_formClosed = false;
                m_isBusy1 = false;
                m_isBusy2 = false;
                m_isBusy3 = false;
                m_isBusy4 = false;
                m_isBusy5 = false;
                m_isBusy6 = false;
                m_isBusy7 = false;
                m_isBusy8 = false;

                for (int i = 0; i < Program.readers.Count; i++)
                {
                    Program.readers[i].ResetReader();
                }
            }
            else
            {
                progress_ANPR.Value = progress_ANPR.Minimum;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormCamera_SizeChanged(object sender, EventArgs e)
        {
            int newWidth = (int)(panel1.Width / 3.0);
            int newHeight = (int)(newWidth / 16.0 * 9);

            if (m_aspect != -1)
            {
                newHeight = (int)(newWidth / m_aspect);
            }


            pnCamera1.Width = pnCamera2.Width = pnCamera3.Width = pnCamera4.Width =
            pnCamera5.Width = pnCamera6.Width = pnCamera7.Width = pnCamera8.Width = newWidth;

            pnCamera1.Height = pnCamera2.Height = pnCamera3.Height = pnCamera4.Height =
            pnCamera5.Height = pnCamera6.Height = pnCamera7.Height = pnCamera8.Height = newHeight;

            pnCamera2.Location = new Point(pnCamera1.Location.X + pnCamera1.Width, pnCamera1.Location.Y);
            pnCamera3.Location = new Point(pnCamera2.Location.X + pnCamera2.Width, pnCamera1.Location.Y);

            pnCamera4.Location = new Point(pnCamera1.Location.X, pnCamera1.Location.Y + pnCamera1.Height);
            pnCamera5.Location = new Point(pnCamera4.Location.X + pnCamera2.Width, pnCamera4.Location.Y);
            pnCamera6.Location = new Point(pnCamera5.Location.X + pnCamera5.Width, pnCamera4.Location.Y);

            pnCamera7.Location = new Point(pnCamera1.Location.X, pnCamera4.Location.Y + pnCamera4.Height);
            pnCamera8.Location = new Point(pnCamera7.Location.X + pnCamera7.Width, pnCamera7.Location.Y);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitCameraPanels()
        {
            try
            {
                string resolution = "1280x720";// TGMTini.GetInstance().Read("resolution", "TGMTcamera", "1280x720");

                m_cameraPanel1 = InitCameraPanel(Program.cameras[0], resolution, pnCamera1, 1);
                m_cameraPanel2 = InitCameraPanel(Program.cameras[1], resolution, pnCamera2, 2);
                m_cameraPanel3 = InitCameraPanel(Program.cameras[2], resolution, pnCamera3, 3);
                m_cameraPanel4 = InitCameraPanel(Program.cameras[3], resolution, pnCamera4, 4);
                m_cameraPanel5 = InitCameraPanel(Program.cameras[4], resolution, pnCamera5, 5);
                m_cameraPanel6 = InitCameraPanel(Program.cameras[5], resolution, pnCamera6, 6);
                m_cameraPanel7 = InitCameraPanel(Program.cameras[6], resolution, pnCamera7, 7);
                m_cameraPanel8 = InitCameraPanel(Program.cameras[7], resolution, pnCamera8, 8);

                LoadRois();

                ConnectCameras();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public CameraPanel InitCameraPanel( string url, string resolution, Panel panelDisplay, int camIndex)
        {
            if (url == "")
                return null;

            CameraPanel cameraPanel = null;
            if (cameraPanel != null)
            {
                panelDisplay.Controls.Clear();

                cameraPanel.Stop();
                cameraPanel.Dispose();
                cameraPanel = null;

            }
            cameraPanel = new CameraPanel(url, resolution);
            cameraPanel.CamIndex = camIndex;


            cameraPanel.parent = panelDisplay;
            cameraPanel.Click += cameraWindows_Click;


            string strRoi = "";// TGMTini.GetInstance().Read("ROI", "CAM" + camIndex);
            if (strRoi != "")
            {
                string[] strrois = strRoi.Split(',');
                if (strrois.Length == 4)
                {
                    int x = int.Parse(strrois[0]);
                    int y = int.Parse(strrois[1]);
                    int w = int.Parse(strrois[2]);
                    int h = int.Parse(strrois[3]);
                    Rectangle rect = new Rectangle(x, y, w, h);
                    cameraPanel.ROI = rect;
                }
            }

            panelDisplay.Controls.Add(cameraPanel);
            return cameraPanel;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void ConnectCameras()
        {
            if (m_isConnecting)
                return;

            TGMTsound.PlaySoundAsync("comedy_pop_finger_in_mouth_001.mp3");
            m_isConnecting = true;

            this.Invoke(new Action(() =>
            {
                circle1.Visible = true;
            }));
            

            BackgroundWorker workerLoadCamera = new BackgroundWorker();
            workerLoadCamera.WorkerReportsProgress = true;
            workerLoadCamera.DoWork += workerLoadCamera_DoWork;
            workerLoadCamera.RunWorkerCompleted += workerLoadCamera_RunWorkerCompleted;
            workerLoadCamera.RunWorkerAsync();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void DisconnectCameras()
        {
            if (m_cameraPanel1 != null)
            {
                m_cameraPanel1.Stop();
                //m_cameraPanel1.Dispose();
            }
            if (m_cameraPanel2 != null)
            {
                m_cameraPanel2.Stop();
                //m_cameraPanel2.Dispose();
            }
            if (m_cameraPanel3 != null)
            {
                m_cameraPanel3.Stop();
                //m_cameraPanel3.Dispose();
            }
            if (m_cameraPanel4 != null)
            {
                m_cameraPanel4.Stop();
                //m_cameraPanel4.Dispose();
            }
            if (m_cameraPanel5 != null)
            {
                m_cameraPanel5.Stop();
                //m_cameraPanel5.Dispose();
            }
            if (m_cameraPanel6 != null)
            {
                m_cameraPanel6.Stop();
                //m_cameraPanel6.Dispose();
            }
            if (m_cameraPanel7 != null)
            {
                m_cameraPanel7.Stop();
                //m_cameraPanel7.Dispose();
            }
            if (m_cameraPanel8 != null)
            {
                m_cameraPanel8.Stop();
                //m_cameraPanel8.Dispose();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void cameraWindows_Click(object sender, EventArgs e)
        {
            m_currentCameraPanel = (CameraPanel)sender;
            MouseEventArgs ee = (MouseEventArgs)e;

            if (ee.Button == MouseButtons.Right)
            {
                Point tl = FormMain.GetInstance().Location;
                Panel parent = m_currentCameraPanel.parent;
                tl.X += parent.Location.X;
                tl.Y += parent.Location.Y + panel2.Height + 50;
                ctxtMnu.Show(new Point(tl.X + ee.Location.X, tl.Y + ee.Location.Y));
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void workerLoadCamera_DoWork(object sender, DoWorkEventArgs e)
        {
            if(Program.enableCamera1)
            {
                m_cameraPanel1.NewFrame -= M_cameraPanel_NewFrame1;
                m_cameraPanel1.NewFrame += M_cameraPanel_NewFrame1;
                m_cameraPanel1.Start();
            }

            if (Program.enableCamera2)
            {
                m_cameraPanel2.NewFrame -= M_cameraPanel_NewFrame2;
                m_cameraPanel2.NewFrame += M_cameraPanel_NewFrame2;
                m_cameraPanel2.Start();
            }

            if (Program.enableCamera3)
            {
                m_cameraPanel3.NewFrame -= M_cameraPanel_NewFrame3;
                m_cameraPanel3.NewFrame += M_cameraPanel_NewFrame3;
                m_cameraPanel3.Start();
            }
            
            if (Program.enableCamera4)
            {
                m_cameraPanel4.NewFrame -= M_cameraPanel_NewFrame4;
                m_cameraPanel4.NewFrame += M_cameraPanel_NewFrame4;
                m_cameraPanel4.Start();
            }
            
            if (Program.enableCamera5)
            {
                m_cameraPanel5.NewFrame -= M_cameraPanel_NewFrame5;
                m_cameraPanel5.NewFrame += M_cameraPanel_NewFrame5;
                m_cameraPanel5.Start();
            }
           
            if (Program.enableCamera6)
            {
                m_cameraPanel6.NewFrame -= M_cameraPanel_NewFrame6;
                m_cameraPanel6.NewFrame += M_cameraPanel_NewFrame6;
                m_cameraPanel6.Start();
            }
            
            if (Program.enableCamera7)
            {
                m_cameraPanel7.NewFrame -= M_cameraPanel_NewFrame7;
                m_cameraPanel7.NewFrame += M_cameraPanel_NewFrame7;
                m_cameraPanel7.Start();
            }
            
            if (Program.enableCamera8)
            {
                m_cameraPanel8.NewFrame -= M_cameraPanel_NewFrame8;    
                m_cameraPanel8.NewFrame += M_cameraPanel_NewFrame8;
                m_cameraPanel8.Start();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void workerLoadCamera_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                FormMain.GetInstance().PrintMessage(""); //clear disconnect message

                btn_connect.Text = "Disconnect";
                circle1.Visible = false;

            }));

            m_isConnecting = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void M_cameraPanel_NewFrame1(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy1 || !Program.inited || m_formClosed)
                return;

            m_isBusy1 = true;
            lock (_bitmapLock1)
            {
                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker1.RunWorkerAsync(bmp);
            }
        }

        private void M_cameraPanel_NewFrame2(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy2 || !Program.inited || m_formClosed)
                return;

            m_isBusy2 = true;

            lock (_bitmapLock2)
            {
                CameraPanel panel = (CameraPanel)sender;

                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker2.RunWorkerAsync(bmp);
            }
        }

        private void M_cameraPanel_NewFrame3(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy3 || !Program.inited || m_formClosed)
                return;

            m_isBusy3 = true;

            lock (_bitmapLock3)
            {
                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker3.RunWorkerAsync(bmp);
            }
        }

        private void M_cameraPanel_NewFrame4(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy4 || !Program.inited || m_formClosed)
                return;

            m_isBusy4 = true;

            lock (_bitmapLock4)
            {
                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker4.RunWorkerAsync(bmp);
            }
        }

        private void M_cameraPanel_NewFrame5(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy5 || !Program.inited || m_formClosed)
                return;

            m_isBusy5 = true;

            lock (_bitmapLock5)
            {
                CameraPanel panel = (CameraPanel)sender;

                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker5.RunWorkerAsync(bmp);
            }
        }

        private void M_cameraPanel_NewFrame6(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy6 || !Program.inited || m_formClosed)
                return;

            m_isBusy6 = true;

            lock (_bitmapLock6)
            {
                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker6.RunWorkerAsync(bmp);
            }
        }

        private void M_cameraPanel_NewFrame7(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy7 || !Program.inited || m_formClosed)
                return;

            m_isBusy7 = true;

            lock (_bitmapLock7)
            {
                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker7.RunWorkerAsync(bmp);
            }
        }

        private void M_cameraPanel_NewFrame8(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!m_autoANPR || m_isBusy8 || !Program.inited || m_formClosed)
                return;

            m_isBusy8 = true;

            lock (_bitmapLock8)
            {
                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                m_worker8.RunWorkerAsync(bmp);
            }
        }

        private void worker_DoWork1(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 1, m_quads[1 - 1]);
        }

        private void worker_DoWork2(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 2, m_quads[2 - 1]);
        }

        private void worker_DoWork3(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 3, m_quads[3 - 1]);
        }

        private void worker_DoWork4(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 4, m_quads[4 - 1]);
        }

        private void worker_DoWork5(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 5, m_quads[5 - 1]);
        }

        private void worker_DoWork6(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 6, m_quads[6 - 1]);
        }

        private void worker_DoWork7(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 7, m_quads[7 - 1]);
        }

        private void worker_DoWork8(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Detect(bmp, 8, m_quads[8 - 1]);
        }

        private void worker_RunWorkerCompleted1(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy1 = false;
        }

        private void worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy2 = false;
        }

        private void worker_RunWorkerCompleted3(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy3 = false;
        }

        private void worker_RunWorkerCompleted4(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy4 = false;
        }

        private void worker_RunWorkerCompleted5(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy5 = false;
        }

        private void worker_RunWorkerCompleted6(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy6 = false;
        }

        private void worker_RunWorkerCompleted7(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy7 = false;
        }

        private void worker_RunWorkerCompleted8(object sender, RunWorkerCompletedEventArgs e)
        {
            m_isBusy8 = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HandleProcessCorruptedStateExceptions]
        void Detect(Bitmap bmp, int camID, Quad quad)
        {
            if(bmp == null)
            {
                SetNotBusy(camID);
                return;
            }
            if (m_formClosed)
                return;

            lock (_lastTimeDetectLock)
            {
                m_lastTimeDetect = DateTime.Now;
            }

            if(TGMTimage.GetNumChannels(bmp) == 4)
            {
                bmp = TGMTimage.ConvertRGBA2RGB(bmp);
            }

            if(quad != null)
            {
                bmp = TGMTimage.CropToPolygon(bmp, quad.points);
            }

            try
            {
                this.Invoke(new Action(() =>
                {
                    if (progress_ANPR.Value >= progress_ANPR.Maximum)
                    {
                        progress_ANPR.Value = progress_ANPR.Minimum;
                    }
                    progress_ANPR.Value++;
                }));
            }
            catch
            {

            }

            PlateInfo[] plates = null;
            try
            {
                plates = Program.readers[camID - 1].Reads(bmp);
                lock (_lastTimeDetectLock)
                {
                    m_lastTimeDetect = DateTime.Now;
                }                
            }
            catch (System.Runtime.InteropServices.SEHException ex)
            {
#if DEBUG
                this.Invoke(new Action(() =>
                {
                    FormMain.GetInstance().PrintError($"SEHException at {camID}: {ex.Message}");
                }));
#endif
                SetNotBusy(camID);
                for (int i = 0; i < Program.readers.Count; i++)
                {
                    Program.readers[i].ResetReader();
                }
                return;
            }
            catch (AccessViolationException ex)
            {
#if DEBUG
                this.Invoke(new Action(() =>
                {
                    FormMain.GetInstance().PrintError($"AccessViolation at {camID}: {ex.Message}");
                }));

#endif
                SetNotBusy(camID);
                for (int i = 0; i < Program.readers.Count; i++)
                {
                    Program.readers[i].ResetReader();
                }
                return;
            }


            if (m_formClosed)
                return;
            

            if (plates == null)
            {
                SetNotBusy(camID);
                return;
            }
                
            

            List<PlateInfo> platesResult = new List<PlateInfo>();

            for (int i = 0; i < plates.Length; i++)
            {
                PlateInfo plate = plates[i];

                if (plate == null || plate.text == "" || plate.text == "Not found" || plate.rect.Width == 0 || plate.rect.Height == 0)
                {
                    continue;
                }

                if (plate.text.Length < 4)
                    continue;

                if (plate.rect.Width < 40)
                    continue;

                if (Program.minScore > 0 && plate.score < Program.minScore)
                    continue;

                if(plate.bitmap == null)
                {
                    Bitmap bmpPlate = TGMTimage.CropBitmap(bmp, plate.rect);
                    plate.bitmap = bmpPlate;
                }

                if (m_alphanumerics.Contains(plate.alphanumeric))
                    continue;

                if (m_alphanumerics.Count > 1000)
                    m_alphanumerics.Clear();

                m_alphanumerics.Add(plate.alphanumeric);

                platesResult.Add(plate);
            }

            

            for (int i = 0; i < platesResult.Count; i++)
            {
                PlateInfo plate = platesResult[i];                

                UC.UCplate ucPlate = new UC.UCplate();
                string filePath = ParkingUtil.GenFilePath(camID, plate.alphanumeric);

                if (plate.bitmap != null)
                {
                    ucPlate.picResult.Image = (Bitmap)plate.bitmap.Clone();
                    plate.bitmap.Save(filePath);
                }


                ucPlate.lblPlate.Text = "[" + camID + "] " + plate.text + " (" + String.Format("{0:0.00}", plate.score) + ")";
                ucPlate.lblPlate.ForeColor = plate.isValid ? Color.Green : Color.Red;


                if (Program.isLicensed)
                {
                    Session s = new Session();
                    s.Text = plate.text;
                    s.Alphanumeric = plate.alphanumeric;
                    s.Score = plate.score;
                    s.ImagePath = filePath;
                    s.CameraID = camID.ToString();

                    lock(_dbLock)
                    {
                        s.Insert();
                    }                    
                }

                lock (_panelLock)
                {
                    this.Invoke(new Action(() =>
                    {
                        if(panelResult.Controls.Count > 1000)
                        {
                            panelResult.Controls.Clear();
                        }
                        panelResult.Controls.Add(ucPlate);
                        panelResult.Refresh();
                    }));
                }
            }

            Thread.Sleep(100);
            SetNotBusy(camID);
            GC.Collect();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void _takePhotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string filePath = String.Format(@"{0}\{1}\{2}{3}", Application.StartupPath, "input", now.ToString("yyyy-MM-dd__hh-mm-ss"), ".jpg");
            Bitmap bmp = m_currentCameraPanel.GetFrame();
            bmp.Save(filePath);
            FormMain.GetInstance().PrintSuccess(filePath);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void _cropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = m_currentCameraPanel.GetFrame();

            if (bmp == null)
            {
                MsgBox.Show("Không đọc được camera");
            }

            FormROI form = new FormROI();
            form.FormClosed += FormROI_FormClosed;
            form.ShowDialog(bmp, m_currentCameraPanel.CamIndex);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormROI_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadRois();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadRois()
        {
            string txtPath = "location.txt";
            if (!File.Exists(txtPath))
                return;


            string[] lines = File.ReadAllLines(txtPath);
            if (lines.Length == 0)
                return;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                string[] lineSplit = line.Split(' ');
                if (lineSplit.Length != 9)
                    continue;

                int cameraID = int.Parse(lineSplit[0]);



                int x1 = int.Parse(lineSplit[1]);
                int y1 = int.Parse(lineSplit[2]);
                int x2 = int.Parse(lineSplit[3]);
                int y2 = int.Parse(lineSplit[4]);
                int x3 = int.Parse(lineSplit[5]);
                int y3 = int.Parse(lineSplit[6]);
                int x4 = int.Parse(lineSplit[7]);
                int y4 = int.Parse(lineSplit[8]);

                Quad quad = new Quad(x1, y1, x2, y2, x3, y3, x4, y4);
                if (cameraID == 1)
                {
                    m_quads[0] = quad;
                }
                else if (cameraID == 2)
                {
                    m_quads[1] = quad;
                }
                else if (cameraID == 3)
                {
                    m_quads[2] = quad;
                }
                else if (cameraID == 4)
                {
                    m_quads[3] = quad;
                }
                else if (cameraID == 5)
                {
                    m_quads[4] = quad;
                }
                else if (cameraID == 6)
                {
                    m_quads[5] = quad;
                }
                else if (cameraID == 7)
                {
                    m_quads[6] = quad;
                }
                else if (cameraID == 8)
                {
                    m_quads[7] = quad;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void SetNotBusy(int cameraID)
        {
            if (cameraID == 1)
                m_isBusy1 = false;
            else if (cameraID == 2)
                m_isBusy2 = false;
            else if (cameraID == 3)
                m_isBusy3 = false;
            else if (cameraID == 4)
                m_isBusy4 = false;
            else if (cameraID == 5)
                m_isBusy5 = false;
            else if (cameraID == 6)
                m_isBusy6 = false;
            else if (cameraID == 7)
                m_isBusy7 = false;
            else if (cameraID == 8)
                m_isBusy8 = false;
        }

        
    }
}
