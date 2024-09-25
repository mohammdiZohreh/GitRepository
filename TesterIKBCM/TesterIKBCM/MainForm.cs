using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.IO.Ports;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using System.Diagnostics;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using MainProject;
using System.Printing;

namespace TesterIKBCM
{
    public partial class MainForm : Form
    {
        public static MainForm instance2;
        System.Windows.Forms.Timer clock_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer serialCheck_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer autoTest_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer autoTest_timer2 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer updateUI_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer NewDay_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer loadForm_timer = new System.Windows.Forms.Timer();

        System.Windows.Forms.Timer checkHealth_timer = new System.Windows.Forms.Timer();
        //System.Windows.Forms.Timer AutoClose_timer = new System.Windows.Forms.Timer();

        Printer printer = new Printer();

        public SerialManager serialManager { get; set; }
        public InputParams inputParam { get; set; }
        public CASInputParam casInputParam { get; set; }


        public InputParams inputParamFeedback { get; set; }

        public OutPutParams outputParam { get; set; }
        public OutPutParams outputFeedback { get; set; }
        public OutPutParams digitalOutputFeedback { get; set; }
        public PowerParams powerParams { get; set; }
        public PowerParams powerParamsFeedback { get; set; }
        public NetParams netParams { get; set; }
        public NetParams netParamsFeedback { get; set; }
        public InfoParams infoParams { get; set; }

        private List<string> UsersList = new List<string>();
        public List<byte> StartStopList = new List<byte>();
        //  System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        string path = Directory.GetCurrentDirectory() + @"\AllUsers";
        string path2 = Directory.GetCurrentDirectory();//+ @"\Reports";

        //private bool firstTimeFlag = true;
        bool bcmVersionReceived = false, casVersionReceived = false;
        int sendSum = 0;
        byte sendChecksum = 0;
        int autoTSt = 0, autoTcounter = 0, inerTSt = 0, i = 0, j = 0, loadTcounter = 0;
        double OutPutBytes1 = Math.Pow(2, 14);
        double OutPutBytes2 = Math.Pow(2, 14);
        double OutPutBytes3 = Math.Pow(2, 5);
        double OutPutBytesD = Math.Pow(2, 14);

        //int RecivInputSt = 0;
        double PowerBytes = Math.Pow(2, 22);
        public bool stopFlag;

        int output1Divid = 1000, output3Divid = 1000;
        Color outputColor;
        Color testGreen = Color.FromArgb(153, 255, 54);
        Color testRed = Color.Red; //FromArgb(235, 141, 161);

        Color connectPanelColor = Color.Transparent;
        XtraReport1 report;

        byte[] netReport = new byte[14];
        byte[] powerReport = new byte[22];
        string[] powerCurReport = new string[22];
        string[] netName = new string[22];
        string[] inputName = new string[49];
        string[] outputName = new string[53];
        byte[] inputReport = new byte[49];
        byte[] outputReport = new byte[53];
        string[] outputCurReport = new string[53];
        string[] inputVoltReport = new string[2];

        int MAXPOWERNUM = 22;
        int MAXOUTPUT1 = 14, MAXOUTPUT2 = 14, MAXOUTPUT3 = 5, ALLOUTPUTCNT = 33, LOADTEST1WAIT = 16, LOADTEST2WAIT = 5, LOADTEST2CNT = 3;
        int loadStatus, loadStatus2, loadLoac;
        bool powerShortFlag = false;
        int powerShortParam;
        // swClass flagBCMPower, flagBoardPower, flagStartReport;
        Color loadsError = Color.Orange;
        Color loadsErrorT2 = Color.OrangeRed;

        public delegate void UpdateControlsDelegate();
        public delegate void UpdateControlsDelegate2(OutPutParams op);
        public delegate void UpdateControlsDelegate3(int param);
        public delegate void UpdateCASDelegate(MsType ms, List<byte> data);
        public delegate void UpdateControlsDel(int st, int loc, Color c);
        public delegate void UpdateControlsDel2(int st, Color c);
        public delegate void DispSendPack(byte[] data);
        public delegate void CASInputExtractDel(List<byte> data);
        System.Random random = new System.Random();

        byte[] Oauto = new byte[2] { 0xff, 0xff };
        Stopwatch stopWatch;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
         (
             int nLeftRect,     // x-coordinate of upper-left corner
             int nTopRect,      // y-coordinate of upper-left corner
             int nRightRect,    // x-coordinate of lower-right corner
             int nBottomRect,   // y-coordinate of lower-right corner
             int nWidthEllipse, // height of ellipse
             int nHeightEllipse // width of ellipse
         );
        public MainForm()
        {
            InitializeComponent();
            dateLbl.Text = PersianConverterDate.ToShamsi(DateTime.Now);
            timeLbl.Text = DateTime.Now.ToString("HH:mm:ss tt");
            BCM_SoftwareVr.Text = TestInfoClass.BCM_SoftwareVr_Rec;


            loadForm_timer.Tick += new EventHandler(loadForm_timer_Tick);
            loadForm_timer.Interval = 1;
            loadForm_timer.Start();

            clock_timer.Tick += new EventHandler(clock_timer_Tick);
            clock_timer.Interval = 800;
            clock_timer.Start();

            serialCheck_timer.Tick += new EventHandler(SerialCheck_timer_Tick);
            serialCheck_timer.Interval = 1200;

            autoTest_timer.Tick += new EventHandler(autoTest_timer_Tick);
            autoTest_timer.Interval = 500;

            autoTest_timer2.Tick += new EventHandler(autoTest_timer2_Tick);
            autoTest_timer2.Interval = 500;

            updateUI_timer.Tick += new EventHandler(updateUI_timer_Tick);
            updateUI_timer.Interval = 1300;// 500;

            //checkHealth_timer.Tick += new EventHandler(checkHealth_Tick);
            //checkHealth_timer.Interval = 1000 * 60;

            //NewDay_timer.Tick += new EventHandler(NewDay_timer_Tick);
            //NewDay_timer.Interval = 1000 * 24 * 60 * 60;
            initParams();//?
            serialConnectionCheck();//?


            if (LoginData.UserName != null)// && SettingHouse.IsLogin)
                userLbl.Text = LoginData.UserName;

            addParamNamesToReport();    //*

        }

        private void BCMTester_Load(object sender, EventArgs e)
        {
            CheckLogin();
            DataBase.LoadTestSpecTblInDb();
            DataBase.LoadOutputsThresholdsFromSqlDatabase();
            DataBase.LoadPrintrSetting();
            DataBase.LoadReportPath();

            testspecLoadData();

            updateUI_timer_Tick(null, null);
            //
            roundPannelCorner();

            string folderPath = path2 + @"\Reports";

            try
            {
                if (Directory.Exists(folderPath))
                {
                    //The code will execute if the folder exists
                }
                else
                {
                    //The below code will create a folder if the folder is not exists in C#.Net.            
                    DirectoryInfo folder = Directory.CreateDirectory(folderPath);
                }
            }
            catch
            {

            }
            updateUI_timer.Start();
            CheckPrinterName();
        }
        public void CheckPrinterName()
        {
            var server = new PrintServer();
            var queues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });

            foreach (var pr in queues)
            {
                string name = pr.Name.ToString();
                if (name.Substring(0, 3) == "TSC")
                {
                    AutoTest.printrFlag = true;
                    break;
                }

            }
        }
        private void testspecLoadData()
        {

            //Load
            TestInfoClass.CheckNewDay();
            TestInfoClass.UpdateTrackingSt(0);
            TestInfoClass.loadReportList();

        }
        private void gitTest()
        {
            MessageBox.Show("Test 1");
        }
        private void loadForm_timer_Tick(object sender, EventArgs e)
        {
            initParams();
            serialConnectionCheck();
            loadForm_timer.Stop();
        }

        private void roundPannelCorner()
        {
            roundCorner(panelDate);
            //    roundCorner(btnStart);

        }
        private void roundCorner(Panel gc)
        {
            gc.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gc.Width + 1,
                     gc.Height + 1, 35, 35));
        }
        private void roundCorner(Button gc)
        {
            gc.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gc.Width + 1,
                     gc.Height + 1, 35, 35));
        }
        ToolTip casLs = new ToolTip();
        ToolTip casHs = new ToolTip();
        ToolTip bcmLs = new ToolTip();
        ToolTip bcmHs = new ToolTip();

        ToolTip rdhsLock = new ToolTip();
        ToolTip rdhsunLock = new ToolTip();
        ToolTip ldhsLock = new ToolTip();
        ToolTip ldhsunLock = new ToolTip();

        ToolTip immo = new ToolTip();
        ToolTip kick = new ToolTip();
        ToolTip rke = new ToolTip();
        ToolTip lf = new ToolTip();
        ToolTip tpms = new ToolTip();
        ToolTip ldhs = new ToolTip();
        ToolTip rdhs = new ToolTip();
        ToolTip TB1 = new ToolTip();
        ToolTip TB2 = new ToolTip();
        ToolTip TB3 = new ToolTip();
        ToolTip TB4 = new ToolTip();
        ToolTip TB5 = new ToolTip();
        ToolTip TB6 = new ToolTip();
        ToolTip TB7 = new ToolTip();
        ToolTip TB8 = new ToolTip();
        ToolTip TB9 = new ToolTip();




        private void inpB1_MouseHover(object sender, EventArgs e)
        {
            TB1.Show("Input_Board1", (TextBox)sender);
            gitTest();
        }
        private void inpB2_MouseHover(object sender, EventArgs e)
        {
            TB2.Show("Input_Board2", (TextBox)sender);
        }
        private void OutB1_MouseHover(object sender, EventArgs e)
        {
            TB3.Show("Output_Board1", (TextBox)sender);
        }
        private void OutB2_MouseHover(object sender, EventArgs e)
        {
            TB4.Show("Output_Board2", (TextBox)sender);
        }
        private void OutB3_MouseHover(object sender, EventArgs e)
        {
            TB5.Show("Output_Board3", (TextBox)sender);
        }
        private void OutBD_MouseHover(object sender, EventArgs e)
        {
            TB6.Show("Digital Output_Board", (TextBox)sender);
        }
        private void NetB_MouseHover(object sender, EventArgs e)
        {
            TB7.Show("Net_Board", (TextBox)sender);
        }
        private void CASB_MouseHover(object sender, EventArgs e)
        {
            TB9.Show("CAS_Board", (TextBox)sender);
        }
        private void PowB_MouseHover(object sender, EventArgs e)
        {
            TB8.Show("Power_Board", (TextBox)sender);
        }


        private void Immo_MouseHover(object sender, EventArgs e)
        {
            immo.Show("IMMO Tx Test", (CheckBox)sender);
        }

        private void LDHS_MouseHover(object sender, EventArgs e)
        {
            ldhs.Show("Driver Door Handel ANT + Touch", (CheckBox)sender);
        }
        private void RDHS_MouseHover(object sender, EventArgs e)
        {
            rdhs.Show("Front Passenger Door Handel ANT + Touch", (CheckBox)sender);
        }

        private void CasLs_MouseHover(object sender, EventArgs e)
        {
            casLs.Show("CAS_Low Side_Input", (CheckBox)sender);
        }
        private void CasHs_MouseHover(object sender, EventArgs e)
        {
            casHs.Show("CAS_High Side_Input", (CheckBox)sender);
        }
        private void BcmLs_MouseHover(object sender, EventArgs e)
        {
            bcmLs.Show("BCM_Low Side_Input", (CheckBox)sender);
        }
        private void BcmHs_MouseHover(object sender, EventArgs e)
        {
            bcmHs.Show("BCM_High Side_Input", (CheckBox)sender);
        }
        private void Rdhslock_MouseHover(object sender, EventArgs e)
        {
            rdhsLock.Show("Right_Door Handle Switch_ Lock", (CheckBox)sender);
        }
        private void RdhsUnlock_MouseHover(object sender, EventArgs e)
        {
            rdhsunLock.Show("Right_Door Handle Switch_ UnLock", (CheckBox)sender);
        }
        private void Ldhslock_MouseHover(object sender, EventArgs e)
        {
            ldhsLock.Show("Left_Door Handle Switch_ Lock", (CheckBox)sender);
        }
        private void LdhsUnlock_MouseHover(object sender, EventArgs e)
        {
            ldhsunLock.Show("Left_Door Handle Switch_ UnLock", (CheckBox)sender);
        }


        #region Timers
        private void clock_timer_Tick(object sender, EventArgs e)
        {
            timeLbl.Text = DateTime.Now.ToString("HH:mm:ss tt");

            if (LoginData.UserName != null && LoginData.IsLogin) //?
                userLbl.Text = LoginData.UserName;
            // UpdateUI();
            updateUI_timer.Start();



        }
        private void SerialCheck_timer_Tick(object sender, EventArgs e)
        {
            if (serialManager != null)
                if (!serialManager.IsOpen())
                {
                    serialCheck_timer.Stop();
                    btnConnect.BackColor = Color.Pink;

                }

        }
        public void UpdateUI()
        {
            updateUI_timer_Tick(null, null);
        }
        private void updateUI_timer_Tick(object sender, EventArgs e)
        {
            trackingNum.Text = TestInfoClass.TrackingNumSt;
            totalPass.Text = TestInfoClass.TotalTestPass.ToString();
            totalFail.Text = TestInfoClass.TotalTestFail.ToString();
            BCM_SoftwareVr.Text = TestInfoClass.BCM_SoftwareVr_Rec;
            BCM_HardwareVr.Text = TestInfoClass.BCM_HardWareVr_Rec;
            BCM_BootloaderVr.Text = TestInfoClass.BCM_BootloaderVr_Rec;
            CAS_SoftwareVr.Text = TestInfoClass.CAS_SoftwareVr_Rec;
            CAS_HardwareVr.Text = TestInfoClass.CAS_HardWareVr_Rec;
            CAS_BootloaderVr.Text = TestInfoClass.CAS_BootloaderVr_Rec;

            CheckLogin();

        }
        private int CheckLogin()
        {
            if (LoginData.IsLogin && LoginData.accessLevel == 1)
            {
                btnSetting.Enabled = true;
                btnShowReport.Enabled = true;
                baudRateCb.Enabled = true;

                //btnLogout.Text = "  LogOut  ";
                //btnLogout.Image = global::TesterIKBCM.Properties.Resources.AutoStop;
            }
            else
            {
                btnSetting.Enabled = false;
                btnShowReport.Enabled = false;
                baudRateCb.Enabled = false;

                //btnLogout.Text = "  Login  ";
                //btnLogout.Image = global::TesterIKBCM.Properties.Resources.logout2;

            }
            if (LoginData.IsLogin)
            {
                //settingPage.Visible = true;
                //testPage.Visible = true;
                //SerialConnection.Visible = true;
                //LoadTest.Visible = true;
                return 1;
            }
            else
            {
                //settingPage.Visible = false;
                //testPage.Visible = false;
                //SerialConnection.Visible = false;
                //LoadTest.Visible = false;
                return 0;
            }

        }
        private void checkHealth_Tick(object sender, EventArgs e)
        {
            //PowerTestCmd();
            //SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.Welcome);

        }
        private void IncProgressVal()
        {
            if (progressBar1.Value < 100)
                progressBar1.Value++;
        }
        private void IncProgressVal(int val)
        {
            if (progressBar1.Value < (100 - val))
                progressBar1.Value += val;
        }
        private void NewDay_timer_Tick(object sender, EventArgs e)
        {
            TestInfoClass.CheckNewDay();
            TestInfoClass.UpdateTrackingSt(0);
            TestInfoClass.TotalTestPass = 0;
            TestInfoClass.TotalTestFail = 0;
        }



        private void autoTest_timer2_Tick(object sender, EventArgs e)
        {
            switch (autoTSt)
            {
                case 0:
                    switch (inerTSt)
                    {
                        case 0:
                            BCMPowerSw.Checked = true;

                            autoTcounter++;
                            if (autoTcounter > TimingLimit.waitForVersion)
                            {
                                inerTSt++;
                                autoTcounter = 0;
                            }

                            break;
                        case 1:
                            refreshVersion();
                            inerTSt++;
                            break;

                        case 2:
                            if (bcmVersionReceived)
                            {
                                if ((TestInfoClass.BCM_SoftwareVr_Rec == TestInfoClass.BCM_SoftwareVr_Exp))    //Add CAS_Soft_vr
                                {
                                    BCM_SoftwareVr.Text = TestInfoClass.BCM_SoftwareVr_Rec;

                                    btnCASSoftVr_Click(null, null);
                                    autoTcounter = 0;
                                    inerTSt++;

                                }
                                else
                                {
                                    TestEndProcedure();

                                    MessageBox.Show(string.Format(" The BCM Software Version differs! Recived version is : {0} while user set version is: {1}", TestInfoClass.BCM_SoftwareVr_Rec, TestInfoClass.BCM_SoftwareVr_Exp), "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    // AutoClosingMessageBox.Show(" The BCM Software Version differs from wich you set! Recived version is:{0}", "Attention!", 5000);
                                }
                            }
                            autoTcounter++;
                            if (autoTcounter > 5)
                            {
                                refreshVersion();
                            }
                            if (autoTcounter > 40)
                            {
                                TestEndProcedure();
                                MessageBox.Show(" The BCM Software Version Not Received");
                                autoTcounter = 0;
                            }

                            break;
                        case 3:
                            if (casVersionReceived)
                            {
                                if ((TestInfoClass.CAS_SoftwareVr_Rec == TestInfoClass.CAS_SoftwareVr_Exp))    // CAS_Soft_vr
                                {
                                    CAS_SoftwareVr.Text = TestInfoClass.CAS_SoftwareVr_Rec;

                                    updateUI_timer.Stop();
                                    feedbackPannel.BackColor = Color.Khaki;
                                    tabControl1.SelectedTab = netTab;

                                    feedbackTb.Text = " Auto Test Started" + "\r\n";

                                    progressBar1.Visible = true;
                                    progressBar1.Value = 0;
                                    autoTcounter = 0;
                                    inerTSt++;
                                    //  autoTSt++;
                                }
                                else
                                {
                                    TestEndProcedure();
                                    MessageBox.Show(string.Format(" The CAS Software Version differs! Recived version is : {0} while user set version is: {1}", TestInfoClass.CAS_SoftwareVr_Rec, TestInfoClass.CAS_SoftwareVr_Exp), "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                            }
                            autoTcounter++;

                            if (autoTcounter > 40)
                            {
                                TestEndProcedure();
                                MessageBox.Show(" The CAS Software Version Not Received");
                                autoTcounter = 0;
                                inerTSt = 0;
                                autoTSt = 0;
                            }
                            break;
                        case 4:
                            autoTcounter++;
                            if (autoTcounter > 7)
                            {
                                inerTSt = 0;
                                autoTSt++;
                                autoTcounter = 0;
                            }
                            break;

                    }
                    break;
                case 1:                  //Network
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "BCM_Can_High Test Started..";
                            //TabControl1.SelectedTabPage = netTab;
                            feedbackPannel.BackColor = Color.PapayaWhip;
                            AutoTest.NetworkRec = false;
                            AutoTest.NetworkPass = false;
                            netParams.Mode = 1;
                            netParams.randomNum = 0xaabbccdd;// random.Next();
                            netParams.DataCANH = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                            SendSerialTestCmd(netParams.DataCANH.ToList(), ID.Network_Board, MsType.CANMessage);
                            inerTSt++;
                            break;
                        case 1:
                            if (AutoTest.NetworkRec)
                            {
                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "BCM_Can_High Test Pass.";
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "BCM_Can_High Test Failed!.";
                                    TestCompleteProcedure(feedbackTb.Text);

                                }

                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_CANHS_led, 0);
                                netReport[0] = 0;
                                feedbackTb.Text = "BCM_Can_High Not Received!";
                                TestCompleteProcedure(feedbackTb.Text);

                            }
                            autoTcounter++;
                            break;
                        case 2:
                            AutoTest.NetworkRec = false;
                            AutoTest.NetworkPass = false;
                            feedbackTb.Text = "BCM_Can_Low Test Started..";
                            netParams.Mode = 2;
                            netParams.randomNum = 0x22334455;// random.Next();
                            netParams.DataCANL = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataCANL.ToList(), ID.Network_Board, MsType.CANMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 3:
                            if (AutoTest.NetworkRec)
                            {
                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "BCM_Can_Low Test Pass.";
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "BCM_Can_Low Test Failed!.";
                                    TestCompleteProcedure(feedbackTb.Text); // TestEndProcedure();
                                                                            //  ShowTestFail();
                                }
                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_CANLS_led, 0);
                                netReport[1] = 0;
                                feedbackTb.Text = "BCM_Can_Low Not Received!";
                                TestCompleteProcedure(feedbackTb.Text);

                            }
                            autoTcounter++;
                            break;
                        case 4:
                            feedbackTb.Text = "LIN_Front Test Started..";
                            AutoTest.NetworkRec = false;
                            AutoTest.NetworkPass = false;
                            netParams.Mode = 1;
                            netParams.randomNum = 0x66778899;// random.Next();
                            netParams.DataLinF = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataLinF.ToList(), ID.Network_Board, MsType.LINMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 5:
                            if (AutoTest.NetworkRec)
                            {
                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "LIN_Front Test Pass.";
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "LIN_Front Test Failed!.";
                                    TestCompleteProcedure(feedbackTb.Text); //TestEndProcedure();
                                                                            // ShowTestFail();
                                }

                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_LINF_led, 0);
                                netReport[2] = 0;
                                feedbackTb.Text = "LIN_Front Not Received!";
                                TestCompleteProcedure(feedbackTb.Text);
                            }
                            autoTcounter++;
                            break;
                        case 6:
                            feedbackTb.Text = "LIN_Rear Test Started..";
                            AutoTest.NetworkRec = false;
                            AutoTest.NetworkPass = false;
                            netParams.Mode = 2;
                            netParams.randomNum = 0x98765432;// random.Next();
                            netParams.DataLinR = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataLinR.ToList(), ID.Network_Board, MsType.LINMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 7:
                            if (AutoTest.NetworkRec)
                            {
                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "LIN_Rear Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "LIN_Rear Test Failed!.";
                                    TestCompleteProcedure(feedbackTb.Text); //  TestEndProcedure();
                                                                            // ShowTestFail();
                                }

                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false; //?
                                IncProgressVal();
                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_LINR_led, 0);
                                netReport[3] = 0;
                                feedbackTb.Text = "LIN_Rear Not Received!";
                                TestCompleteProcedure(feedbackTb.Text);
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false;
                                IncProgressVal();
                            }
                            autoTcounter++;

                            break;
                        case 8:
                            AutoTest.NetworkRec = false;
                            AutoTest.NetworkPass = false;
                            feedbackTb.Text = "CAS_Can_Low Test Started..";
                            netParams.Mode = 3;
                            netParams.randomNum = 0xABCDEF89;// random.Next();
                            netParams.DataCASCANL = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataCASCANL.ToList(), ID.Network_Board, MsType.CANMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 9:
                            if (AutoTest.NetworkRec)
                            {

                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "CAS_Can_Low Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_Can_Low Test Failed!.";
                                    TestCompleteProcedure(feedbackTb.Text); //  TestEndProcedure();
                                                                            // ShowTestFail();
                                }
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 5 ? true : false;
                                IncProgressVal();
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(CAS_CANLS_led, 0);
                                netReport[4] = 0;
                                feedbackTb.Text = "CAS_Can_Low Not Received!";
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 5 ? true : false;
                                IncProgressVal();
                                TestCompleteProcedure(feedbackTb.Text);

                            }
                            autoTcounter++;
                            break;
                        case 10:
                            if (CASTest.Checked)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            else
                            {
                                inerTSt = 0;
                                autoTSt += 2;
                            }
                            break;

                    }

                    break;
                case 2:    //CAS

                    netParams.Data = new List<byte>();
                    switch (inerTSt)
                    {
                        case 0:         //IMMO_Test
                            feedbackTb.Text = "CAS_IMMO Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.Immo);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 1:        //IMMO_Test
                            if (AutoTest.CASRec)
                            {
                                if (AutoTest.CASTestPass)
                                {
                                    feedbackTb.Text = "CAS_IMMO Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_IMMO Test Failed!.";
                                    TestCompleteProcedure(); //  TestEndProcedure();
                                                             // ShowTestFail();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(ImmoTest_led, 0);
                                netReport[5] = 0;
                                feedbackTb.Text = "CAS_IMMO Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;

                        case 2:         //KICK_Test
                            feedbackTb.Text = "CAS_KICK Sensor Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.KickSensor);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 3:        //KICK_Test
                            if (AutoTest.CASRec)
                            {
                                //    feedbackTb.Text = "CAS_KICK Sensor Test Done.";
                                //    inerTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_KICK Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_KICK Test Failed!.";
                                    TestCompleteProcedure(); //  TestEndProcedure();
                                                             // ShowTestFail();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(KickTest_Led, 0);
                                netReport[6] = 0;
                                feedbackTb.Text = "CAS_KICK Sensor Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;
                        case 4:         //TPMS_Test
                            feedbackTb.Text = "CAS_TPMS Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.TPMS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 5:        //TPMS_Test
                            if (AutoTest.CASRec)
                            {
                                //feedbackTb.Text = "CAS_TPMS Test Done.";
                                //inerTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_TPMS Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_TPMS Test Failed!.";
                                    TestCompleteProcedure(); //  TestEndProcedure();
                                                             // ShowTestFail();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(TPMSTest_led, 0);
                                netReport[7] = 0;
                                feedbackTb.Text = "CAS_TPMS Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;
                        case 6:         //RKE_Test
                            feedbackTb.Text = "CAS_RKE Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.RKE);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 7:        //RKE_Test
                            if (AutoTest.CASRec)
                            {
                                //feedbackTb.Text = "CAS_RKE Test Done.";
                                //inerTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_RKE Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_RKE Test Failed!.";
                                    TestCompleteProcedure(); //  TestEndProcedure();
                                                             // ShowTestFail();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(RKETest_led, 0);
                                netReport[8] = 0;
                                feedbackTb.Text = "CAS_RKE Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;
                        case 8:         //LF_Test
                            feedbackTb.Text = "CAS_LF Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.LF);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 9:        //LF_Test
                            if (AutoTest.CASRec)
                            {
                                //feedbackTb.Text = "CAS_LF Test Done.";
                                //inerTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_LF Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_LF Test Failed!.";
                                    TestCompleteProcedure();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(LFTest_led, 0);
                                netReport[9] = 0;
                                feedbackTb.Text = "CAS_LF Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;
                        case 10:         //RDHS_Lock_Test
                            feedbackTb.Text = "CAS_R_DHS_Lock Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(1);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 11:        //RDHS_Lock_Test
                            if (AutoTest.CASRec)
                            {
                                //feedbackTb.Text = "CAS_R_DHS_Lock Test Done.";
                                //inerTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_R_DHS_Lock Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_R_DHS_Lock Test Failed!.";
                                    TestCompleteProcedure();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(RDHSLock_led, 0);
                                netReport[10] = 0;
                                feedbackTb.Text = "CAS_R_DHS_Lock Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;
                        case 12:         //RDHS_UnLock_Test
                            feedbackTb.Text = "CAS_R_DHS_UnLock Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(2);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 13:        //RDHS_UnLock_Test
                            if (AutoTest.CASRec)
                            {
                                //feedbackTb.Text = "CAS_R_DHS_UnLock Test Done.";
                                //inerTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_R_DHS_UnLock Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_R_DHS_UnLock Test Failed!.";
                                    TestCompleteProcedure();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(RDHSUnlock_led, 0);
                                netReport[11] = 0;
                                feedbackTb.Text = "CAS_R_DHS_UnLock Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;
                        case 14:         //LDHS_Lock_Test
                            feedbackTb.Text = "CAS_L_DHS_Lock Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(3);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 15:        //LDHS_Lock_Test
                            if (AutoTest.CASRec)
                            {
                                //feedbackTb.Text = "CAS_L_DHS_Lock Test Done.";
                                //inerTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_L_DHS_UnLock Test Pass.";
                                    IncProgressVal();
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_L_DHS_UnLock Test Failed!.";
                                    TestCompleteProcedure();
                                }
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(LDHSLock_led, 0);
                                netReport[12] = 0;
                                feedbackTb.Text = "CAS_L_DHS_Lock Not Received!";
                                TestCompleteProcedure();

                            }
                            autoTcounter++;
                            break;
                        case 16:         //LDHS_UnLock_Test
                            feedbackTb.Text = "CAS_L_DHS_UnLock Test Started..";
                            AutoTest.CASRec = false;
                            AutoTest.CASTestPass = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(4);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 17:        //LDHS_UnLock_Test
                            if (AutoTest.CASRec)
                            {
                                //feedbackTb.Text = "CAS_L_DHS_UnLock Test Done.";
                                //inerTSt = 0;
                                //autoTSt++;
                                if (AutoTest.CASTestPass) //??
                                {
                                    feedbackTb.Text = "CAS_L_DHS_UnLock Test Pass.";
                                    IncProgressVal();
                                    inerTSt = 0;
                                    autoTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "CAS_L_DHS_UnLock Test Failed!.";
                                    TestCompleteProcedure();
                                }
                                if (AutoTest.CasTestCountr == 9)   //take it there
                                    AutoTest.CasTestPassed = true;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(LDHSUnlock_led, 0);
                                netReport[13] = 0;
                                feedbackTb.Text = "CAS_L_DHS_UnLock Not Received!";
                                // inerTSt = 0;
                                // autoTcounter = 0;
                                // autoTSt++;
                                //if (AutoTest.CasTestCountr == 9)
                                //    AutoTest.CasTestPass = true;
                                //else
                                //    AutoTest.CasTestPass = false;
                                TestCompleteProcedure();
                            }
                            autoTcounter++;
                            break;
                    }
                    break;

                case 3:      //loadTest
                    switch (inerTSt)
                    {
                        case 0:
                            tabControl1.SelectedTab = outputTab;
                            AutoTest.LoadTest2Rec = false;
                            AutoTest.LoadTest2Pass = false;
                            AutoTest.LoadTest2Passed = 0;
                            feedbackTb.Text = "Load Test Started..";
                            autoTcounter = 0;
                            loadTcounter = 0;
                            if (loadTest2.Checked)//(loadPreTest.Checked)
                            {
                                inerTSt++;
                            }

                            else
                            {
                                inerTSt = 3;
                            }
                            break;
                        case 1:

                            SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.OutputTest2);
                            inerTSt++;
                            break;

                        case 2:
                            if (AutoTest.LoadTest2B3Rec)
                            {
                                if (AutoTest.LoadTest2Pass)
                                {
                                    feedbackTb.Text = "Load Test Done.";
                                    loadTcounter++;
                                    if (loadTcounter > LOADTEST2WAIT) //for delay 
                                    {
                                        inerTSt = 0;
                                        autoTSt++;
                                        autoTcounter = 0;
                                        loadTcounter = 0;
                                        // AutoTest.LoadTest2Pass = false;
                                    }
                                    else
                                    {
                                        feedbackTb.Text = "Load Test Failed!";
                                        TestCompleteProcedure(); // TestEndProcedure();
                                                                 //   ShowTestFail();
                                    }
                                }
                                loadTcounter++;
                                if (loadTcounter > LOADTEST2WAIT) //for delay 
                                {
                                    inerTSt++;
                                    autoTcounter = 0;
                                    loadTcounter = 0;
                                }
                            }
                            else if (autoTcounter > TimingLimit.loadTest2Time)
                            {
                                TestCompleteProcedure();
                            }
                            autoTcounter++;
                            break;

                        case 3:
                            //if (loadTest1.Checked)
                            //{
                            //    resetOutputsFeedback();
                            //    inerTSt++;
                            //    autoTcounter = 0;
                            //}
                            //else
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            break;
                        case 4:
                            SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.OutputTest1);
                            inerTSt++;
                            break;
                        case 5:
                            if (AutoTest.LoadTest1B3Cnt >= MAXOUTPUT3)
                            {
                                if (AutoTest.LoadTest1Passed == ALLOUTPUTCNT)
                                {
                                    AutoTest.LoadTest1Pass = true;
                                }
                                inerTSt++;
                                autoTcounter = 0;
                                AutoTest.LoadTest1B3Cnt = 0;
                            }
                            if (autoTcounter > TimingLimit.loadTest1Time)
                            {
                                //inerTSt++;
                                //autoTcounter = 0;
                                //AutoTest.LoadTest1B3Cnt = 0;
                                TestCompleteProcedure();
                            }
                            autoTcounter++;

                            break;
                        case 6:
                            if (loadTcounter > LOADTEST1WAIT) //for delay 
                            {
                                inerTSt = 0;
                                autoTSt++;
                                loadTcounter = 0;
                            }
                            loadTcounter++;
                            break;

                    }
                    break;
                case 4:        //Output1
                    switch (inerTSt)
                    {
                        case 0:
                            tabControl1.SelectedTab = outputTab;
                            feedbackPannel.BackColor = Color.Bisque;
                            if (outputTestSw.Checked)
                            {
                                feedbackTb.Text = "OutPut Board1 Test Started..";
                                i = 1; j = 0;
                                autoTcounter = 0;
                                inerTSt++;
                                IncProgressVal();
                                outputParam.OutputList1.Clear();
                                resetOutputsFeedback();
                            }
                            else
                            {
                                autoTSt = 8;    //go to Input Test
                            }
                            break;

                        case 1:
                            AutoTest.Output1Rec = false;
                            outputParam.OutputList1.Clear();
                            if (i < OutPutBytes1)
                            {
                                outputParam.OutputList1.Add((byte)(i >> 8));
                                outputParam.OutputList1.Add((byte)i);
                                SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.SwitchONOff);
                                // i *= 2;
                                //j++;
                                inerTSt++;
                                IncProgressVal();
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;
                                IncProgressVal();
                            }

                            break;
                        case 2:
                            if (AutoTest.Output1Rec)
                            {
                                feedbackTb.Text = "OutPut Board1 Current " + findRoot(i) + " Received.";
                                if (autoTcounter >= 5)
                                {
                                    inerTSt--;
                                    autoTcounter = 0;
                                    IncProgressVal();

                                    if (AutoTest.Output1_Ok || AutoTest.TstRepeat)
                                    {
                                        i *= 2;
                                        AutoTest.TstRepeat = false;
                                    }
                                    else
                                    {
                                        AutoTest.TstRepeat = true;
                                        // i = i;
                                    }

                                }
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board1 Current" + findRoot(i) + "Not Received!";
                                inerTSt--;
                                autoTcounter = 0;
                                IncProgressVal();
                                i *= 2;
                            }
                            autoTcounter++;

                            break;
                    }
                    break;
                case 5:                            //Output2
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "OutPut Board2 Test Started..";
                            i = 1; j = 0;
                            autoTcounter = 0;
                            inerTSt++;
                            IncProgressVal();
                            outputParam.OutputList2.Clear();
                            break;

                        case 1:
                            outputParam.OutputList2.Clear();
                            AutoTest.Output2Rec = false;
                            if ((i < OutPutBytes2))
                            {
                                outputParam.OutputList2.Add((byte)(i >> 8));
                                outputParam.OutputList2.Add((byte)i);
                                SendSerialTestCmd(outputParam.OutputList2, ID.OutputB2, MsType.SwitchONOff);
                                // i *= 2;
                                inerTSt++;
                                IncProgressVal();

                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;
                            }

                            break;
                        case 2:
                            if (AutoTest.Output2Rec)
                            {
                                feedbackTb.Text = "OutPut Board2 Current " + findRoot(i) + " Received";
                                if (autoTcounter >= 5)
                                {
                                    inerTSt--;
                                    autoTcounter = 0;
                                    IncProgressVal();
                                    if (AutoTest.Output2_Ok || AutoTest.TstRepeat)
                                    {
                                        i *= 2;
                                        AutoTest.TstRepeat = false;
                                    }
                                    else
                                    {
                                        AutoTest.TstRepeat = true;

                                    }
                                }
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board2 Current " + findRoot(i) + " Not Received";
                                inerTSt--;
                                autoTcounter = 0;
                                i *= 2;
                            }
                            autoTcounter++;

                            break;
                    }

                    break;
                case 6:         //Output3
                    switch (inerTSt)
                    {
                        case 0:
                            tabControl1.SelectedTab = output2Tab;
                            feedbackTb.Text = "OutPut Board3 Test Started..";
                            i = 1; j = 0;
                            autoTcounter = 0;
                            inerTSt++;
                            outputParam.OutputList3.Clear();
                            break;

                        case 1:
                            outputParam.OutputList3.Clear();
                            AutoTest.Output3Rec = false;
                            if (i < OutPutBytes3)
                            {
                                outputParam.OutputList3.Add((byte)(i >> 8));
                                outputParam.OutputList3.Add((byte)i);
                                //Thread.Sleep(250);
                                SendSerialTestCmd(outputParam.OutputList3, ID.OutputB3, MsType.SwitchONOff);
                                //Thread.Sleep(thr.outputsTiming3[j++]);

                                // i *= 2;

                                inerTSt++;
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;

                            }

                            break;
                        case 2:
                            if (AutoTest.Output3Rec)
                            {
                                feedbackTb.Text = "OutPut Board3 Current " + findRoot(i) + " Received";
                                if (autoTcounter >= 5)
                                {
                                    inerTSt--;
                                    autoTcounter = 0;
                                    IncProgressVal();
                                    if (AutoTest.Output3_Ok || AutoTest.TstRepeat)
                                    {
                                        i *= 2;
                                        AutoTest.TstRepeat = false;
                                    }
                                    else
                                    {
                                        AutoTest.TstRepeat = true;

                                    }
                                }
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board3  Current " + findRoot(i) + " Not Received";
                                inerTSt--;
                                autoTcounter = 0;
                                i *= 2;
                            }
                            autoTcounter++;
                            break;
                    }
                    break;
                case 7:              //Digital Output
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "Digital OutPut Board Test Started..";
                            i = 1; j = 0;
                            autoTcounter = 0;
                            AutoTest.OutputD_Ok = false;
                            inerTSt++;
                            break;

                        case 1:
                            outputParam.DigitalOutputList.Clear();
                            AutoTest.OutputDRec = false;
                            if (i < OutPutBytesD)
                            {
                                {
                                    outputParam.DigitalOutputList.Add((byte)(i >> 8));
                                    outputParam.DigitalOutputList.Add((byte)i);

                                    SendSerialTestCmd(outputParam.DigitalOutputList, ID.DigitalOutput, MsType.SwitchONOff);
                                }
                                i *= 2;

                                inerTSt = 2;
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;

                            }
                            break;

                        case 2:
                            if (AutoTest.OutputDRec)
                            {
                                feedbackTb.Text = "Digital OutPut Board Test " + findRoot(i) + " Received";
                                if (autoTcounter > 3)
                                {
                                    autoTcounter = 0;
                                    inerTSt = 1;
                                }
                            }

                            else if (autoTcounter > TimingLimit.DigitalOutputTime)
                            {
                                feedbackTb.Text = "Digital OutPut Board " + findRoot(i) + " Not Received!";
                                inerTSt = 1;
                                autoTcounter = 0;


                            }
                            autoTcounter++;
                            break;
                    }
                    break;
                case 8:         //Input
                    if (inputTestSw.Checked)
                    {
                        tabControl1.SelectedTab = inputTab;
                        feedbackPannel.BackColor = Color.MistyRose;

                        if (TestInfoClass.InputTestStrategy == 0)   //OnebyOne
                            InputTestOnebyOne2();
                        else if (TestInfoClass.InputTestStrategy == 1)    //byGroup
                            InputTestbyGroup2();


                        //***

                    }
                    else
                    {
                        autoTSt = 9;       // autoTSt++; //because crash unlock moved to end procedure
                    }

                    break;
                case 9:      //CAS Input 

                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "CAS InPut Test";
                            CasInpStReset();
                            autoTcounter = 0;

                            In2_H5.Checked = true;
                            CASInputTestCmd();
                            inerTSt++;
                            break;
                        case 1:
                            if ((CASInput.InpSt1 == 1) || (autoTcounter > 15))
                            {
                                In2_H2.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }

                            autoTcounter++;
                            break;
                        case 2:
                            if ((CASInput.InpSt2 == 1) || (autoTcounter > 15))
                            {
                                In2_H3.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 3:
                            if ((CASInput.InpSt3 == 1) || (autoTcounter > 15))
                            {
                                In2_H4.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 4:
                            if ((CASInput.InpSt4 == 1) || (autoTcounter > 15))
                            {
                                In2_L12.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 5:
                            if ((CASInput.InpSt5 == 1) || (autoTcounter > 15))
                            {
                                In2_L13.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 6:
                            if ((CASInput.InpSt6 == 1) || (autoTcounter > 15))
                            {
                                In2_L14.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 7:
                            if ((CASInput.InpSt7 == 1) || (autoTcounter > 15))
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                    }

                    break;
                case 10:                 //Analoge Input
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "Analoge InPut Test";
                            autoTcounter = 0;
                            if (AnInputTest.Checked)
                            {
                                AnalogInput.AnInputList.Clear();
                                FuelResponse.Full_Recv = false;
                                AnalogInput.AnInputList.Add((byte)FuelLvl.Full);
                                SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                                FuelResponse.Full_Sent = true;
                                inerTSt++;
                            }
                            else
                            {
                                autoTSt++;
                            }

                            break;
                        case 1:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 2:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.ThreeQuarters);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.ThreeQuarters_Sent = true;
                            //if (FuelResponse.ThreeQuarters_Recv)
                            inerTSt++;
                            break;
                        case 3:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0; //? if not answered in time stop analog Test
                                autoTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 4:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.Half);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.Half_Sent = true;
                            //if (FuelResponse.Half_Recv)
                            inerTSt++;
                            break;
                        case 5:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 6:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.Quarter);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.Quarter_Sent = true;
                            //if (FuelResponse.Quarter_Recv)
                            inerTSt++;
                            break;
                        case 7:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 8:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.Empty);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.Empty_Sent = true;
                            //if (FuelResponse.Empty_Recv)
                            inerTSt++;
                            //fuelTestThread.Abort();
                            break;
                        case 9:
                            if (FuelResponse.Full_Recv || autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                    }
                    break;

                case 11:
                    feedbackTb.Text = "Auto Test Completed.";
                    progressBar1.Value = 100;
                    TestCompleteProcedure();
                    autoTSt = 0;
                    inerTSt = 0;
                    // TestInfoClass.CheckNewDay();
                    //TestInfoClass.TrackingNum++;
                    //TestInfoClass.UpdateTrackingSt();
                    ////updateUI_timer_Tick(null, null);
                    //StopAutoTest();
                    ////
                    //stopWatch.Stop();
                    //TimeSpan ts = stopWatch.Elapsed;

                    //// Format and display the TimeSpan value.
                    //string elapsedTime = String.Format("{0:00}:{1:00}", //:{2:00}
                    //    ts.Minutes, ts.Seconds);   //, ts.Milliseconds / 10

                    //testTime.Text = elapsedTime + " min";
                    ////

                    //if (SettingHouse.IsLogin)
                    //    checkTestAndPrint();
                    //else //if(!UserInfoClass.LogPermission)
                    //     //MessageBox.Show(" You Dont Have Log Permission!");
                    //    AutoClosingMessageBox.Show(" You Dont Have Log Permission!", "Attention!", 1000);


                    break;
            }
        }

        private void autoTest_timer_Tick(object sender, EventArgs e)
        {
            switch (autoTSt)
            {
                case 0:
                    switch (inerTSt)
                    {
                        case 0:
                            BCMPowerSw.Checked = true; ////
                            //for test
                            //autoTSt = 8;
                            //inerTSt = 0;
                            //
                            autoTcounter++;
                            if (autoTcounter > TimingLimit.waitForVersion)
                            {
                                inerTSt++;
                                autoTcounter = 0;
                            }

                            break;
                        case 1:
                            refreshVersion();
                            inerTSt++;
                            break;

                        case 2:
                            if (bcmVersionReceived)
                            {
                                if ((TestInfoClass.BCM_SoftwareVr_Rec == TestInfoClass.BCM_SoftwareVr_Exp))    //Add CAS_Soft_vr
                                {
                                    BCM_SoftwareVr.Text = TestInfoClass.BCM_SoftwareVr_Rec;    //next receive cas version
                                    btnCASSoftVr_Click(null, null);
                                    autoTcounter = 0;
                                    inerTSt++;

                                }
                                else
                                {
                                    TestEndProcedure();
                                    MessageBox.Show(string.Format(" The BCM Software Version differs! Recived version is : {0} while user set version is: {1}", TestInfoClass.BCM_SoftwareVr_Rec, TestInfoClass.BCM_SoftwareVr_Exp), "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    // AutoClosingMessageBox.Show(" The BCM Software Version differs from wich you set! Recived version is:{0}", "Attention!", 5000);
                                }
                            }
                            autoTcounter++;
                            if (autoTcounter > 5)
                            {
                                refreshVersion();
                            }
                            if (autoTcounter > 40)
                            {
                                TestEndProcedure();
                                MessageBox.Show(" The BCM Software Version Not Received");
                                autoTcounter = 0;
                            }

                            break;
                        case 3:
                            if (casVersionReceived)
                            {
                                if ((TestInfoClass.CAS_SoftwareVr_Rec == TestInfoClass.CAS_SoftwareVr_Exp))    // CAS_Soft_vr
                                {
                                    CAS_SoftwareVr.Text = TestInfoClass.CAS_SoftwareVr_Rec;

                                    updateUI_timer.Stop();
                                    feedbackPannel.BackColor = Color.Khaki;
                                    tabControl1.SelectedTab = netTab;

                                    feedbackTb.Text = " Auto Test Started" + "\r\n";

                                    progressBar1.Visible = true;
                                    progressBar1.Value = 0;
                                    autoTcounter = 0;
                                    inerTSt++;
                                    //  autoTSt++;
                                }
                                else
                                {
                                    TestEndProcedure();
                                    MessageBox.Show(string.Format(" The CAS Software Version differs! Recived version is : {0} while user set version is: {1}", TestInfoClass.CAS_SoftwareVr_Rec, TestInfoClass.CAS_SoftwareVr_Exp), "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                            }
                            autoTcounter++;
                            //if (autoTcounter > 5)
                            //{
                            //    refreshVersion();
                            //}
                            if (autoTcounter > 40)
                            {
                                TestEndProcedure();
                                MessageBox.Show(" The CAS Software Version Not Received");
                                autoTcounter = 0;
                                inerTSt = 0;
                                autoTSt = 0;
                            }
                            break;
                        case 4:
                            autoTcounter++;
                            if (autoTcounter > 7)
                            {
                                inerTSt = 0;
                                autoTSt++;
                                autoTcounter = 0;
                            }
                            break;

                    }
                    break;
                case 1:                  //Network
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "BCM_Can_High Test Started..";
                            //TabControl1.SelectedTabPage = netTab;
                            feedbackPannel.BackColor = Color.PapayaWhip;
                            AutoTest.NetworkRec = false;
                            AutoTest.NetworkPass = false;
                            netParams.Mode = 1;
                            netParams.randomNum = 0xaabbccdd;// random.Next();
                            netParams.DataCANH = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                            SendSerialTestCmd(netParams.DataCANH.ToList(), ID.Network_Board, MsType.CANMessage);
                            inerTSt++;
                            break;
                        case 1:
                            if (AutoTest.NetworkRec)
                            {
                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "Can_High Test Pass.";
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "Can_High Test Failed!.";
                                    inerTSt++;

                                }

                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_CANHS_led, 0);
                                netReport[0] = 0;
                                feedbackTb.Text = "BCM_Can_High Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 2:
                            AutoTest.NetworkRec = false;
                            feedbackTb.Text = "BCM_Can_Low Test Started..";
                            netParams.Mode = 2;
                            netParams.randomNum = 0x22334455;// random.Next();
                            netParams.DataCANL = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataCANL.ToList(), ID.Network_Board, MsType.CANMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 3:
                            if (AutoTest.NetworkRec)
                            {
                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "Can_Low Test Pass.";
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "Can_Low Test Failed!.";
                                    inerTSt++;

                                }
                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_CANLS_led, 0);
                                netReport[1] = 0;
                                feedbackTb.Text = "BCM_Can_Low Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 4:
                            feedbackTb.Text = "LIN_Front Test Started..";
                            AutoTest.NetworkRec = false;
                            netParams.Mode = 1;
                            netParams.randomNum = 0x66778899;// random.Next();
                            netParams.DataLinF = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataLinF.ToList(), ID.Network_Board, MsType.LINMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 5:
                            if (AutoTest.NetworkRec)
                            {
                                // feedbackTb.Text = "LIN_Front Test Done.";
                                // inerTSt++;
                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "LIN_Front Test Pass.";
                                    inerTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "LIN_Front Test Failed!.";
                                    inerTSt++;

                                }

                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_LINF_led, 0);
                                netReport[2] = 0;
                                feedbackTb.Text = "LIN_Front Not Received!";
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 6:
                            feedbackTb.Text = "LIN_Rear Test Started..";
                            AutoTest.NetworkRec = false;
                            netParams.Mode = 2;
                            netParams.randomNum = 0x98765432;// random.Next();
                            netParams.DataLinR = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataLinR.ToList(), ID.Network_Board, MsType.LINMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 7:
                            if (AutoTest.NetworkRec)
                            {

                                if (AutoTest.NetworkPass)
                                {
                                    feedbackTb.Text = "LIN_Rear Test Pass.";
                                    inerTSt++;
                                    // autoTSt++;
                                }
                                else
                                {
                                    feedbackTb.Text = "LIN_Rear Test Failed!.";
                                    inerTSt++;
                                    //autoTSt++;

                                }

                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false;
                                IncProgressVal();
                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_LINR_led, 0);
                                netReport[3] = 0;
                                feedbackTb.Text = "LIN_Rear Not Received!";
                                //inerTSt = 0;
                                //autoTSt++;
                                inerTSt++;
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false;
                                IncProgressVal();
                            }
                            autoTcounter++;

                            break;
                        case 8:
                            AutoTest.NetworkRec = false;
                            feedbackTb.Text = "CAS_Can_Low Test Started..";
                            netParams.Mode = 3;
                            netParams.randomNum = 0xABCDEF89;// random.Next();
                            netParams.DataCASCANL = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                            SendSerialTestCmd(netParams.DataCASCANL.ToList(), ID.Network_Board, MsType.CANMessage);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 9:
                            if (AutoTest.NetworkRec)
                            {
                                feedbackTb.Text = "CAS_Can_Low Test Done.";
                                inerTSt++;
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 5 ? true : false;
                                IncProgressVal();
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(CAS_CANLS_led, 0);
                                netReport[4] = 0;
                                feedbackTb.Text = "CAS_Can_Low Not Received!";
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 5 ? true : false;
                                IncProgressVal();
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 10:
                            if (CASTest.Checked)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            else
                            {
                                inerTSt = 0;
                                autoTSt += 2;
                            }
                            break;

                    }

                    break;
                case 2:    //CAS

                    netParams.Data = new List<byte>();
                    switch (inerTSt)
                    {
                        case 0:         //IMMO_Test
                            feedbackTb.Text = "CAS_IMMO Test Started..";
                            AutoTest.CASRec = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.Immo);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 1:        //IMMO_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_IMMO Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(ImmoTest_led, 0);
                                netReport[5] = 0;
                                feedbackTb.Text = "CAS_IMMO Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;

                        case 2:         //KICK_Test
                            feedbackTb.Text = "CAS_KICK Sensor Test Started..";
                            AutoTest.CASRec = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.KickSensor);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 3:        //KICK_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_KICK Sensor Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(KickTest_Led, 0);
                                netReport[6] = 0;
                                feedbackTb.Text = "CAS_KICK Sensor Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 4:         //TPMS_Test
                            feedbackTb.Text = "CAS_TPMS Test Started..";
                            AutoTest.CASRec = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.TPMS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 5:        //TPMS_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_TPMS Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(TPMSTest_led, 0);
                                netReport[7] = 0;
                                feedbackTb.Text = "CAS_TPMS Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 6:         //RKE_Test
                            feedbackTb.Text = "CAS_RKE Test Started..";
                            AutoTest.CASRec = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.RKE);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 7:        //RKE_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_RKE Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(RKETest_led, 0);
                                netReport[8] = 0;
                                feedbackTb.Text = "CAS_RKE Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 8:         //LF_Test
                            feedbackTb.Text = "CAS_LF Test Started..";
                            AutoTest.CASRec = false;
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.LF);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 9:        //LF_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_LF Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(LFTest_led, 0);
                                netReport[9] = 0;
                                feedbackTb.Text = "CAS_LF Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 10:         //RDHS_Lock_Test
                            feedbackTb.Text = "CAS_R_DHS_Lock Test Started..";
                            AutoTest.CASRec = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(1);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 11:        //RDHS_Lock_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_R_DHS_Lock Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(RDHSLock_led, 0);
                                netReport[10] = 0;
                                feedbackTb.Text = "CAS_R_DHS_Lock Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 12:         //RDHS_UnLock_Test
                            feedbackTb.Text = "CAS_R_DHS_UnLock Test Started..";
                            AutoTest.CASRec = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(2);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 13:        //RDHS_UnLock_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_R_DHS_UnLock Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(RDHSUnlock_led, 0);
                                netReport[11] = 0;
                                feedbackTb.Text = "CAS_R_DHS_UnLock Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 14:         //LDHS_Lock_Test
                            feedbackTb.Text = "CAS_L_DHS_Lock Test Started..";
                            AutoTest.CASRec = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(3);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 15:        //LDHS_Lock_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_L_DHS_Lock Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(LDHSLock_led, 0);
                                netReport[12] = 0;
                                feedbackTb.Text = "CAS_L_DHS_Lock Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 16:         //LDHS_UnLock_Test
                            feedbackTb.Text = "CAS_L_DHS_UnLock Test Started..";
                            AutoTest.CASRec = false;
                            netParams.Data.Clear();
                            netParams.Data.Add(4);
                            SendSerialTestCmd(netParams.Data, ID.CAS_tBoard, MsType.DHS);
                            autoTcounter = 0;
                            inerTSt++;
                            break;
                        case 17:        //LDHS_UnLock_Test
                            if (AutoTest.CASRec)
                            {
                                feedbackTb.Text = "CAS_L_DHS_UnLock Test Done.";
                                inerTSt = 0;
                                autoTSt++;
                                if (AutoTest.CasTestCountr == 9)
                                    AutoTest.CasTestPassed = true;
                            }

                            else if (autoTcounter > TimingLimit.CASTime)
                            {
                                ledBulb_Click(LDHSUnlock_led, 0);
                                netReport[13] = 0;
                                feedbackTb.Text = "CAS_L_DHS_UnLock Not Received!";
                                inerTSt = 0;
                                autoTcounter = 0;
                                autoTSt++;
                                if (AutoTest.CasTestCountr == 9)
                                    AutoTest.CasTestPassed = true;
                                else
                                    AutoTest.CasTestPassed = false;

                            }
                            autoTcounter++;
                            break;
                    }
                    break;

                case 3:      //loadTest
                    switch (inerTSt)
                    {
                        case 0:
                            tabControl1.SelectedTab = outputTab;
                            feedbackTb.Text = "Load Test Started..";

                            if (loadTest2.Checked)
                            {

                                inerTSt++;
                            }

                            else
                            {
                                inerTSt = 3;

                            }
                            break;
                        case 1:

                            SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.OutputTest2);
                            inerTSt++;
                            break;

                        case 2:
                            if (AutoTest.LoadTest2B3Rec)
                            {
                                loadTcounter++;
                                if (loadTcounter > LOADTEST2WAIT) //for delay 
                                {
                                    inerTSt++;
                                    autoTcounter = 0;
                                    loadTcounter = 0;
                                }
                            }
                            else if (autoTcounter > TimingLimit.loadTest2Time)
                            {
                                inerTSt++;
                                autoTcounter = 0;
                                loadTcounter = 0;
                            }
                            autoTcounter++;
                            break;

                        case 3:
                            //if (loadTest1.Checked)
                            //{
                            //    resetOutputsFeedback();
                            //    inerTSt++;
                            //    autoTcounter = 0;
                            //}
                            //else
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            break;
                        case 4:
                            SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.OutputTest1);
                            inerTSt++;
                            break;
                        case 5:
                            if (AutoTest.LoadTest1B3Cnt >= MAXOUTPUT3)
                            {
                                if (AutoTest.LoadTest1Passed == ALLOUTPUTCNT)
                                {
                                    AutoTest.LoadTest1Pass = true;
                                }
                                inerTSt++;
                                autoTcounter = 0;
                                AutoTest.LoadTest1B3Cnt = 0;
                            }
                            if (autoTcounter > TimingLimit.loadTest1Time)
                            {
                                inerTSt++;
                                autoTcounter = 0;
                                AutoTest.LoadTest1B3Cnt = 0;
                            }
                            autoTcounter++;

                            break;
                        case 6:
                            if (loadTcounter > LOADTEST1WAIT) //for delay 
                            {
                                inerTSt = 0;
                                autoTSt++;
                                loadTcounter = 0;
                            }
                            loadTcounter++;
                            break;

                    }
                    break;
                case 4:        //Output1
                    switch (inerTSt)
                    {
                        case 0:

                            if (outputTestSw.Checked)
                            {
                                tabControl1.SelectedTab = outputTab;
                                feedbackPannel.BackColor = Color.Bisque;
                                feedbackTb.Text = "OutPut Board1 Test Started..";
                                i = 1; j = 0;
                                autoTcounter = 0;
                                inerTSt++;
                                IncProgressVal();
                                outputParam.OutputList1.Clear();
                                resetOutputsFeedback();
                            }
                            else
                            {
                                autoTSt = 8;    //go to Input Test
                            }
                            break;

                        case 1:
                            AutoTest.Output1Rec = false;
                            outputParam.OutputList1.Clear();
                            if (i < OutPutBytes1)
                            {
                                outputParam.OutputList1.Add((byte)(i >> 8));
                                outputParam.OutputList1.Add((byte)i);
                                SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.SwitchONOff);
                                // i *= 2;
                                //j++;
                                inerTSt++;
                                IncProgressVal();
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;
                                IncProgressVal();
                            }

                            break;
                        case 2:
                            if (AutoTest.Output1Rec)
                            {
                                feedbackTb.Text = "OutPut Board1 Current " + findRoot(i) + " Received.";
                                if (autoTcounter >= 5)
                                {
                                    inerTSt--;
                                    autoTcounter = 0;
                                    IncProgressVal();

                                    if (AutoTest.Output1_Ok || AutoTest.TstRepeat)
                                    {
                                        i *= 2;
                                        AutoTest.TstRepeat = false;
                                    }
                                    else
                                    {
                                        AutoTest.TstRepeat = true;
                                        // i = i;
                                    }

                                }
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board1 Current" + findRoot(i) + "Not Received!";
                                inerTSt--;
                                autoTcounter = 0;
                                IncProgressVal();
                                i *= 2;
                            }
                            autoTcounter++;

                            break;
                    }
                    break;
                case 5:                            //Output2
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "OutPut Board2 Test Started..";
                            i = 1; j = 0;
                            autoTcounter = 0;
                            inerTSt++;
                            IncProgressVal();
                            outputParam.OutputList2.Clear();
                            break;

                        case 1:
                            outputParam.OutputList2.Clear();
                            AutoTest.Output2Rec = false;
                            if ((i < OutPutBytes2))
                            {
                                outputParam.OutputList2.Add((byte)(i >> 8));
                                outputParam.OutputList2.Add((byte)i);
                                SendSerialTestCmd(outputParam.OutputList2, ID.OutputB2, MsType.SwitchONOff);
                                // i *= 2;
                                inerTSt++;
                                IncProgressVal();

                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;
                            }

                            break;
                        case 2:
                            if (AutoTest.Output2Rec)
                            {
                                feedbackTb.Text = "OutPut Board2 Current " + findRoot(i) + " Received";
                                if (autoTcounter >= 5)
                                {
                                    inerTSt--;
                                    autoTcounter = 0;
                                    IncProgressVal();
                                    if (AutoTest.Output2_Ok || AutoTest.TstRepeat)
                                    {
                                        i *= 2;
                                        AutoTest.TstRepeat = false;
                                    }
                                    else
                                    {
                                        AutoTest.TstRepeat = true;

                                    }
                                }
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board2 Current " + findRoot(i) + " Not Received";
                                inerTSt--;
                                autoTcounter = 0;
                                i *= 2;
                            }
                            autoTcounter++;

                            break;
                    }

                    break;
                case 6:         //Output3
                    switch (inerTSt)
                    {
                        case 0:
                            tabControl1.SelectedTab = output2Tab;
                            feedbackTb.Text = "OutPut Board3 Test Started..";
                            i = 1; j = 0;
                            autoTcounter = 0;
                            inerTSt++;
                            outputParam.OutputList3.Clear();
                            break;

                        case 1:
                            outputParam.OutputList3.Clear();
                            AutoTest.Output3Rec = false;
                            if (i < OutPutBytes3)
                            {
                                outputParam.OutputList3.Add((byte)(i >> 8));
                                outputParam.OutputList3.Add((byte)i);
                                //Thread.Sleep(250);
                                SendSerialTestCmd(outputParam.OutputList3, ID.OutputB3, MsType.SwitchONOff);
                                //Thread.Sleep(thr.outputsTiming3[j++]);

                                // i *= 2;

                                inerTSt++;
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;

                            }

                            break;
                        case 2:
                            if (AutoTest.Output3Rec)
                            {
                                feedbackTb.Text = "OutPut Board3 Current " + findRoot(i) + " Received";
                                if (autoTcounter >= 5)
                                {
                                    inerTSt--;
                                    autoTcounter = 0;
                                    IncProgressVal();
                                    if (AutoTest.Output3_Ok || AutoTest.TstRepeat)
                                    {
                                        i *= 2;
                                        AutoTest.TstRepeat = false;
                                    }
                                    else
                                    {
                                        AutoTest.TstRepeat = true;

                                    }
                                }
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board3  Current " + findRoot(i) + " Not Received";
                                inerTSt--;
                                autoTcounter = 0;
                                i *= 2;
                            }
                            autoTcounter++;
                            break;
                    }
                    break;
                case 7:              //Digital Output
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "Digital OutPut Board Test Started..";
                            i = 1; j = 0;
                            autoTcounter = 0;
                            AutoTest.OutputD_Ok = false;
                            inerTSt++;
                            break;

                        case 1:
                            outputParam.DigitalOutputList.Clear();
                            AutoTest.OutputDRec = false;
                            if (i < OutPutBytesD)
                            {
                                {
                                    outputParam.DigitalOutputList.Add((byte)(i >> 8));
                                    outputParam.DigitalOutputList.Add((byte)i);

                                    SendSerialTestCmd(outputParam.DigitalOutputList, ID.DigitalOutput, MsType.SwitchONOff);
                                }
                                i *= 2;

                                inerTSt = 2;
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;

                            }
                            break;

                        case 2:
                            if (AutoTest.OutputDRec)
                            {
                                feedbackTb.Text = "Digital OutPut Board Test " + findRoot(i) + " Received";
                                if (autoTcounter > 3)
                                {
                                    autoTcounter = 0;
                                    inerTSt = 1;
                                }
                            }

                            else if (autoTcounter > TimingLimit.DigitalOutputTime)
                            {
                                feedbackTb.Text = "Digital OutPut Board " + findRoot(i) + " Not Received!";
                                inerTSt = 1;
                                autoTcounter = 0;


                            }
                            autoTcounter++;
                            break;
                    }
                    break;
                case 8:         //Input

                    if (inputTestSw.Checked)
                    {
                        tabControl1.SelectedTab = inputTab;
                        feedbackPannel.BackColor = Color.MistyRose;

                        if (TestInfoClass.InputTestStrategy == 0)   //OnebyOne
                            InputTestOnebyOne();
                        else if (TestInfoClass.InputTestStrategy == 1)    //byGroup
                            InputTestbyGroup();
                    }
                    else
                    {
                        autoTcounter = 0;
                        autoTSt += 1;
                    }


                    break;
                case 9:      //CAS Input 

                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "CAS InPut Test";
                            CasInpStReset();
                            autoTcounter = 0;

                            In2_H5.Checked = true;
                            CASInputTestCmd();
                            inerTSt++;
                            break;
                        case 1:
                            if ((CASInput.InpSt1 == 1) || (autoTcounter > 15))
                            {
                                In2_H2.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }

                            autoTcounter++;
                            break;
                        case 2:
                            if ((CASInput.InpSt2 == 1) || (autoTcounter > 15))
                            {
                                In2_H3.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 3:
                            if ((CASInput.InpSt3 == 1) || (autoTcounter > 15))
                            {
                                In2_H4.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 4:
                            if ((CASInput.InpSt4 == 1) || (autoTcounter > 15))
                            {
                                In2_L12.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 5:
                            if ((CASInput.InpSt5 == 1) || (autoTcounter > 15))
                            {
                                In2_L13.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 6:
                            if ((CASInput.InpSt6 == 1) || (autoTcounter > 15))
                            {
                                In2_L14.Checked = true;
                                CASInputTestCmd();
                                autoTcounter = 0;
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 7:
                            if ((CASInput.InpSt7 == 1) || (autoTcounter > 15))
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                    }

                    break;
                case 10:                 //Analoge Input
                    switch (inerTSt)
                    {
                        case 0:
                            feedbackTb.Text = "Analoge InPut Test";
                            autoTcounter = 0;
                            if (AnInputTest.Checked)
                            {
                                AnalogInput.AnInputList.Clear();
                                FuelResponse.Full_Recv = false;
                                AnalogInput.AnInputList.Add((byte)FuelLvl.Full);
                                SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                                FuelResponse.Full_Sent = true;
                                inerTSt++;
                            }
                            else
                            {
                                autoTSt++;
                            }

                            break;
                        case 1:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 2:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.ThreeQuarters);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.ThreeQuarters_Sent = true;
                            //if (FuelResponse.ThreeQuarters_Recv)
                            inerTSt++;
                            break;
                        case 3:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0; //? if not answered in time stop analog Test
                                autoTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 4:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.Half);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.Half_Sent = true;
                            //if (FuelResponse.Half_Recv)
                            inerTSt++;
                            break;
                        case 5:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 6:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.Quarter);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.Quarter_Sent = true;
                            //if (FuelResponse.Quarter_Recv)
                            inerTSt++;
                            break;
                        case 7:
                            if (FuelResponse.Full_Recv)
                            {
                                inerTSt++;
                            }
                            else if (autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                        case 8:
                            AnalogInput.AnInputList.Clear();
                            autoTcounter = 0;
                            FuelResponse.Full_Recv = false;
                            AnalogInput.AnInputList.Add((byte)FuelLvl.Empty);
                            SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                            FuelResponse.Empty_Sent = true;
                            //if (FuelResponse.Empty_Recv)
                            inerTSt++;
                            //fuelTestThread.Abort();
                            break;
                        case 9:
                            if (FuelResponse.Full_Recv || autoTcounter > TimingLimit.AInputTime)
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            autoTcounter++;
                            break;
                    }
                    break;

                case 11:
                    feedbackTb.Text = "Auto Test Completed.";
                    progressBar1.Value = 100;
                    TestCompleteProcedure();
                    autoTSt = 0;
                    inerTSt = 0;
                    // TestInfoClass.CheckNewDay();
                    //TestInfoClass.TrackingNum++;
                    //TestInfoClass.UpdateTrackingSt();
                    ////updateUI_timer_Tick(null, null);
                    //StopAutoTest();
                    ////
                    //stopWatch.Stop();
                    //TimeSpan ts = stopWatch.Elapsed;

                    //// Format and display the TimeSpan value.
                    //string elapsedTime = String.Format("{0:00}:{1:00}", //:{2:00}
                    //    ts.Minutes, ts.Seconds);   //, ts.Milliseconds / 10

                    //testTime.Text = elapsedTime + " min";
                    ////

                    //if (SettingHouse.IsLogin)
                    //    checkTestAndPrint();
                    //else //if(!UserInfoClass.LogPermission)
                    //     //MessageBox.Show(" You Dont Have Log Permission!");
                    //    AutoClosingMessageBox.Show(" You Dont Have Log Permission!", "Attention!", 1000);


                    break;
            }
        }

        private void InputTestInit(CheckBox c, LedBulb le, int t = 5)
        {
            AutoTest.InputCheckd = c.Name.ToString();// + ":" + c.Text;
            cselect = le;
            if (autoTcounter > t)
            {
                AutoTest.InputRec2 = false;
                inputParam.InputList1.Clear();
                InputTestCmd();
                inerTSt++;
                autoTcounter = 0;
            }
        }
        private void Input2TestInit(CheckBox c, LedBulb le, int t = 5)
        {
            AutoTest.InputCheckd = c.Name.ToString();//+ ":" + c.Text;
            cselect = le;
            if (autoTcounter > t)
            {
                AutoTest.InputRec2 = false;
                inputParam.InputList2.Clear();
                Input2TestCmd();
                inerTSt++;
                autoTcounter = 0;
            }
        }
        private void InputTestbyGroup2()
        {
            switch (inerTSt)
            {
                case 0:

                    if (inputTestSw.Checked)
                    {
                        inputParam.InputList1.Clear();
                        //new
                        In1_L1.Checked = In1_L3.Checked = In1_L5.Checked = In1_L7.Checked = In1_L9.Checked = In1_L11.Checked = In1_L13.Checked = In1_L15.Checked = true;
                        In1_L17.Checked = In1_L19.Checked = In1_L21.Checked = In1_L23.Checked = In1_L25.Checked = In1_L27.Checked = In1_L29.Checked = true;
                        InputTestCmd();
                        //

                        //byte[] inputs1 = { 0x01, 0x8c, 0x59, 0x99, 0x99 };    //89 to 99 byte3
                        //inputParam.InputList1.AddRange(inputs1);
                        //SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);

                        inerTSt++;
                        feedbackTb.Text = "InPut Board Test Started..";
                        //autoTcounter++;
                        progressBar1.Value++;
                    }
                    else
                    {
                        autoTcounter = 0;
                        autoTSt += 2;
                    }
                    break;
                case 1:
                    if (AutoTest.InputRecevd == 1 || autoTcounter > TimingLimit.InputTime)
                    {
                        inerTSt++;
                        autoTcounter = 0;
                        IncProgressVal();
                    }

                    autoTcounter++;
                    break;

                case 2:

                    //if ((AutoTest.InputRecevd == 1) || (autoTcounter > TimingLimit.InputTime))
                    //{
                    inputParam.InputList1.Clear();
                    //new
                    In1_L2.Checked = In1_L4.Checked = In1_L6.Checked = In1_L8.Checked = In1_L10.Checked = In1_L12.Checked = In1_L14.Checked = In1_L16.Checked = true;
                    In1_L18.Checked = In1_L20.Checked = In1_L22.Checked = In1_L24.Checked = In1_L26.Checked = In1_L28.Checked = true;
                    InputTestCmd();
                    //              
                    //byte[] inputs2 = { 0x00, 0x33, 0x86, 0x66, 0x66 };  //0x46 to 0x66 byte3
                    //inputParam.InputList1.AddRange(inputs2);
                    //SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);
                    feedbackTb.Text = "InPut1_2 sent";
                    inerTSt++;
                    autoTcounter = 0;
                    IncProgressVal();
                    AutoTest.InputRecevd = 1;
                    //}


                    autoTcounter++;

                    break;
                case 3:
                    if (AutoTest.InputRecevd == 2 || autoTcounter > TimingLimit.InputTime)
                    {
                        inerTSt++;
                        autoTcounter = 0;
                        IncProgressVal();
                    }
                    autoTcounter++;
                    break;
                //autoTcounter++;
                //if (autoTcounter > 10)
                //{
                //    inerTSt++;
                //    autoTcounter = 0;
                //}
                //break;
                //***

                //**
                case 4:
                    tabControl1.SelectedTab = input2Tab;

                    //  if ((AutoTest.InputRecevd == 2) || (autoTcounter > TimingLimit.InputTime))
                    //  {
                    inputParam.InputList2.Clear();
                    //
                    In2_L1.Checked = In2_L3.Checked = In2_L5.Checked = In2_L7.Checked = In2_L9.Checked = In2_L11.Checked = true;
                    Input2TestCmd();
                    //
                    //byte[] inputs3 = { 00, 00, 00, 0x19, 0x99 };
                    //inputParam.InputList2.AddRange(inputs3);
                    //SendSerialTestCmd(inputParam.InputList2, ID.InputB2, MsType.SwitchONOff);
                    feedbackTb.Text = "InPut2_1 sent";
                    inerTSt++;
                    //   autoTcounter = 0;
                    IncProgressVal();

                    //  }

                    //autoTcounter++;
                    break;
                case 5:
                    if (AutoTest.InputRecevd == 3 || autoTcounter > TimingLimit.InputTime)
                    {
                        inerTSt++;
                        autoTcounter = 0;
                        IncProgressVal();
                    }

                    autoTcounter++;
                    break;
                case 6:

                    inputParam.InputList2.Clear();
                    In2_L2.Checked = In2_L4.Checked = In2_L6.Checked = In2_L8.Checked = In2_L10.Checked = In2_H1.Checked = true;
                    Input2TestCmd();
                    feedbackTb.Text = "InPut2_2 sent";
                    inerTSt++;


                    break;

                case 7:
                    if ((AutoTest.InputRecevd == 0) || (autoTcounter > TimingLimit.InputTime))
                    {
                        feedbackTb.Text = "InPut Board Test Done.";
                        inerTSt = 0;
                        IncProgressVal();
                        autoTSt++;
                        //if (AnInputTest.Checked)
                        //{
                        //    autoTSt++;
                        //}
                        //else
                        //{
                        //    autoTSt += 2;
                        //}
                    }

                    autoTcounter++;

                    break;

            }
        }
        private void InputTestbyGroup()
        {
            switch (inerTSt)
            {

                case 0:

                    // if (inputTest.Checked)
                    // {
                    inputParam.InputList1.Clear();
                    //new
                    In1_L1.Checked = In1_L3.Checked = In1_L5.Checked = In1_L7.Checked = In1_L9.Checked = In1_L11.Checked = In1_L13.Checked = In1_L15.Checked = true;
                    In1_L17.Checked = In1_L19.Checked = In1_L21.Checked = In1_L23.Checked = In1_L25.Checked = In1_L27.Checked = In1_L29.Checked = true;
                    InputTestCmd();
                    //

                    //byte[] inputs1 = { 0x01, 0x8c, 0x59, 0x99, 0x99 };    //89 to 99 byte3
                    //inputParam.InputList1.AddRange(inputs1);
                    //SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);

                    inerTSt++;
                    feedbackTb.Text = "InPut Board Test Started..";
                    //autoTcounter++;
                    progressBar1.Value++;
                    // }
                    //else
                    //{
                    //    autoTcounter = 0;
                    //    autoTSt += 2;
                    //}
                    break;
                case 1:
                    if (AutoTest.InputRecevd == 1 || autoTcounter > TimingLimit.InputTime)
                    {
                        inerTSt++;
                        autoTcounter = 0;
                        IncProgressVal();
                    }

                    autoTcounter++;
                    break;

                case 2:

                    //if ((AutoTest.InputRecevd == 1) || (autoTcounter > TimingLimit.InputTime))
                    //{
                    inputParam.InputList1.Clear();
                    //new
                    In1_L2.Checked = In1_L4.Checked = In1_L6.Checked = In1_L8.Checked = In1_L10.Checked = In1_L12.Checked = In1_L14.Checked = In1_L16.Checked = true;
                    In1_L18.Checked = In1_L20.Checked = In1_L22.Checked = In1_L24.Checked = In1_L26.Checked = In1_L28.Checked = true;
                    InputTestCmd();
                    //              
                    //byte[] inputs2 = { 0x00, 0x33, 0x86, 0x66, 0x66 };  //0x46 to 0x66 byte3
                    //inputParam.InputList1.AddRange(inputs2);
                    //SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);
                    feedbackTb.Text = "InPut1_2 sent";
                    inerTSt++;
                    autoTcounter = 0;
                    IncProgressVal();
                    AutoTest.InputRecevd = 1;
                    //}


                    autoTcounter++;

                    break;
                case 3:
                    if (AutoTest.InputRecevd == 2 || autoTcounter > TimingLimit.InputTime)
                    {
                        inerTSt++;
                        autoTcounter = 0;
                        IncProgressVal();
                    }
                    autoTcounter++;
                    break;
                //autoTcounter++;
                //if (autoTcounter > 10)
                //{
                //    inerTSt++;
                //    autoTcounter = 0;
                //}
                //break;
                //***

                //**
                case 4:
                    tabControl1.SelectedTab = input2Tab;

                    //  if ((AutoTest.InputRecevd == 2) || (autoTcounter > TimingLimit.InputTime))
                    //  {
                    inputParam.InputList2.Clear();
                    //
                    In2_L1.Checked = In2_L3.Checked = In2_L5.Checked = In2_L7.Checked = In2_L9.Checked = In2_L11.Checked = true;
                    Input2TestCmd();
                    //
                    //byte[] inputs3 = { 00, 00, 00, 0x19, 0x99 };
                    //inputParam.InputList2.AddRange(inputs3);
                    //SendSerialTestCmd(inputParam.InputList2, ID.InputB2, MsType.SwitchONOff);
                    feedbackTb.Text = "InPut2_1 sent";
                    inerTSt++;
                    //   autoTcounter = 0;
                    IncProgressVal();

                    //  }

                    //autoTcounter++;
                    break;
                case 5:
                    if (AutoTest.InputRecevd == 3 || autoTcounter > TimingLimit.InputTime)
                    {
                        inerTSt++;
                        autoTcounter = 0;
                        IncProgressVal();
                    }

                    autoTcounter++;
                    break;
                case 6:

                    inputParam.InputList2.Clear();
                    In2_L2.Checked = In2_L4.Checked = In2_L6.Checked = In2_L8.Checked = In2_L10.Checked = In2_H1.Checked = true;
                    Input2TestCmd();
                    feedbackTb.Text = "InPut2_2 sent";
                    inerTSt++;


                    break;

                case 7:
                    if ((AutoTest.InputRecevd == 0) || (autoTcounter > TimingLimit.InputTime))
                    {
                        feedbackTb.Text = "InPut Board Test Done.";
                        inerTSt = 0;
                        IncProgressVal();
                        autoTSt++;
                        //if (AnInputTest.Checked)
                        //{
                        //    autoTSt++;
                        //}
                        //else
                        //{
                        //    autoTSt += 2;
                        //}
                    }

                    autoTcounter++;

                    break;

            }
        }

        private void InputTestOnebyOne()
        {
            switch (inerTSt)
            {
                case 0:
                    In1_L1.Checked = true;
                    InputTestInit(In1_L1, In1_L1_led);
                    feedbackTb.Text = "InPut Board Test Started..";
                    autoTcounter++;
                    break;
                case 1:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L2.Checked = true;
                        InputTestInit(In1_L2, In1_L2_led);
                    }

                    autoTcounter++;
                    break;

                case 2:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L3.Checked = true;
                        InputTestInit(In1_L3, In1_L3_led);
                    }
                    autoTcounter++;

                    break;
                case 3:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L4.Checked = true;
                        InputTestInit(In1_L4, In1_L4_led);
                    }
                    autoTcounter++;
                    break;

                case 4:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L5.Checked = true;
                        InputTestInit(In1_L5, In1_L5_led);
                    }
                    autoTcounter++;

                    break;
                case 5:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L6.Checked = true;
                        InputTestInit(In1_L6, In1_L6_led);
                        // IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 6:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L7.Checked = true;
                        InputTestInit(In1_L7, In1_L7_led);
                    }
                    autoTcounter++;
                    break;
                case 7:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L8.Checked = true;
                        InputTestInit(In1_L8, In1_L8_led);
                    }
                    autoTcounter++;
                    break;
                case 8:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L9.Checked = true;
                        InputTestInit(In1_L9, In1_L9_led);
                    }
                    autoTcounter++;
                    break;
                case 9:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L10.Checked = true;
                        InputTestInit(In1_L10, In1_L10_led);
                    }
                    autoTcounter++;
                    break;
                case 10:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L11.Checked = true;
                        InputTestInit(In1_L11, In1_L11_led);
                        // IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 11:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L12.Checked = true;
                        InputTestInit(In1_L12, In1_L12_led);
                    }
                    autoTcounter++;
                    break;
                case 12:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L13.Checked = true;
                        InputTestInit(In1_L13, In1_L13_led);
                    }
                    autoTcounter++;
                    break;
                case 13:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L14.Checked = true;
                        InputTestInit(In1_L14, In1_L14_led);
                    }
                    autoTcounter++;
                    break;
                case 14:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L15.Checked = true;
                        InputTestInit(In1_L15, In1_L15_led);
                    }
                    autoTcounter++;
                    break;
                case 15:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L16.Checked = true;
                        InputTestInit(In1_L16, In1_L16_led);
                        // IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 16:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L17.Checked = true;
                        InputTestInit(In1_L17, In1_L17_led);
                    }
                    autoTcounter++;
                    break;
                case 17:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L18.Checked = true;
                        InputTestInit(In1_L18, In1_L18_led);
                    }
                    autoTcounter++;
                    break;
                case 18:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L19.Checked = true;
                        InputTestInit(In1_L19, In1_L19_led);
                    }
                    autoTcounter++;
                    break;
                case 19:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L20.Checked = true;
                        InputTestInit(In1_L20, In1_L20_led);
                    }
                    autoTcounter++;
                    break;
                case 20:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L21.Checked = true;
                        InputTestInit(In1_L21, In1_L21_led);
                        //IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 21:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L22.Checked = true;
                        InputTestInit(In1_L22, In1_L22_led);
                    }
                    autoTcounter++;
                    break;
                case 22:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L23.Checked = true;
                        InputTestInit(In1_L23, In1_L23_led);
                    }
                    autoTcounter++;
                    break;
                case 23:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L24.Checked = true;
                        InputTestInit(In1_L24, In1_L24_led);
                    }
                    autoTcounter++;
                    break;
                case 24:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L25.Checked = true;
                        InputTestInit(In1_L25, In1_L25_led);
                    }
                    autoTcounter++;
                    break;
                case 25:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L26.Checked = true;
                        InputTestInit(In1_L26, In1_L26_led);
                        //IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 26:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L27.Checked = true;
                        InputTestInit(In1_L27, In1_L27_led);
                    }
                    autoTcounter++;
                    break;
                case 27:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L28.Checked = true;
                        InputTestInit(In1_L28, In1_L28_led);
                    }
                    autoTcounter++;
                    break;
                case 28:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In1_L29.Checked = true;
                        InputTestInit(In1_L29, In1_L29_led);
                    }
                    autoTcounter++;
                    break;
                case 29:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L1.Checked = true;
                        Input2TestInit(In2_L1, In2_L1_led);
                    }
                    autoTcounter++;
                    break;
                case 30:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L2.Checked = true;
                        Input2TestInit(In2_L2, In2_L2_led);
                        // IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 31:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L3.Checked = true;
                        Input2TestInit(In2_L3, In2_L3_led);
                    }
                    autoTcounter++;
                    break;
                case 32:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L4.Checked = true;
                        Input2TestInit(In2_L4, In2_L4_led);
                    }
                    autoTcounter++;
                    break;
                case 33:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L5.Checked = true;
                        Input2TestInit(In2_L5, In2_L5_led);
                    }
                    autoTcounter++;
                    break;
                case 34:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L6.Checked = true;
                        Input2TestInit(In2_L6, In2_L6_led);
                    }
                    autoTcounter++;
                    break;
                case 35:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L7.Checked = true;
                        Input2TestInit(In2_L7, In2_L7_led);
                        // IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 36:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L8.Checked = true;
                        Input2TestInit(In2_L8, In2_L8_led);
                    }
                    autoTcounter++;
                    break;
                case 37:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L9.Checked = true;
                        Input2TestInit(In2_L9, In2_L9_led);
                    }
                    autoTcounter++;
                    break;
                case 38:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L10.Checked = true;
                        Input2TestInit(In2_L10, In2_L10_led);
                    }
                    autoTcounter++;
                    break;
                case 39:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L11.Checked = true;
                        Input2TestInit(In2_L11, In2_L11_led);
                    }
                    autoTcounter++;
                    break;
                case 40:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L12.Checked = true;
                        Input2TestInit(In2_L12, In2_L12_led);
                        //  IncProgressVal(1);
                    }
                    autoTcounter++;
                    break;
                case 41:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L13.Checked = true;
                        Input2TestInit(In2_L3, In2_L3_led);
                    }
                    autoTcounter++;
                    break;
                case 42:
                    if (AutoTest.InputRec2 || autoTcounter > TimingLimit.InputTime)
                    {
                        In2_L14.Checked = true;
                        Input2TestInit(In2_L14, In2_L14_led);
                    }
                    autoTcounter++;
                    break;

                case 43:
                    if (AutoTest.InputRec2 || (autoTcounter > TimingLimit.InputTime))
                    {
                        autoTSt++; //?
                        feedbackTb.Text = "InPut Board Test Done.";
                        inerTSt = 0;
                        IncProgressVal(5);
                    }
                    autoTcounter++;

                    break;

            }
        }

        private void InputTestOnebyOne2()
        {
            switch (inerTSt)
            {
                case 0:
                    In1_L1.Checked = true;
                    InputTestInit(In1_L1, In1_L1_led);
                    feedbackTb.Text = "InPut Board Test Started..";
                    autoTcounter++;
                    break;
                case 1:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L2.Checked = true;
                            InputTestInit(In1_L2, In1_L2_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 1 Failed!";
                            TestCompleteProcedure(feedbackTb.Text); //  TestEndProcedure();
                                                                    //ShowTestFail();
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 1 Timeout!";
                        TestCompleteProcedure(feedbackTb.Text); // TestEndProcedure();
                                                                // ShowTestFail();
                    }

                    autoTcounter++;
                    break;

                case 2:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L3.Checked = true;
                            InputTestInit(In1_L3, In1_L3_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 2 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);

                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 2 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 3:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L4.Checked = true;
                            InputTestInit(In1_L4, In1_L4_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 3 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 3 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 4:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L5.Checked = true;
                            InputTestInit(In1_L5, In1_L5_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 4 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 4 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 5:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L6.Checked = true;
                            InputTestInit(In1_L6, In1_L6_led);
                            // IncProgressVal(1);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 5 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 5 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 6:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L7.Checked = true;
                            InputTestInit(In1_L7, In1_L7_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 6 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 6 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 7:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L8.Checked = true;
                            InputTestInit(In1_L8, In1_L8_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 7 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 7 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 8:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L9.Checked = true;
                            InputTestInit(In1_L9, In1_L9_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 8 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 8 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 9:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L10.Checked = true;
                            InputTestInit(In1_L10, In1_L10_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 9 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 9 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 10:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L11.Checked = true;
                            InputTestInit(In1_L11, In1_L11_led);
                            //IncProgressVal(1);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 10 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 10 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 11:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L12.Checked = true;
                            InputTestInit(In1_L12, In1_L12_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 11 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 11 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 12:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L13.Checked = true;
                            InputTestInit(In1_L13, In1_L13_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 12 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 12 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 13:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L14.Checked = true;
                            InputTestInit(In1_L14, In1_L14_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 13 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 13 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 14:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L15.Checked = true;
                            InputTestInit(In1_L15, In1_L15_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 14 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 14 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 15:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L16.Checked = true;
                            InputTestInit(In1_L16, In1_L16_led);
                            //IncProgressVal(1);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 15 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 15 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 16:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L17.Checked = true;
                            InputTestInit(In1_L17, In1_L17_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 16 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 16 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 17:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L18.Checked = true;
                            InputTestInit(In1_L18, In1_L18_led);
                        }
                        else
                        {

                            feedbackTb.Text = "InPut Board1,input 17 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 17 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }
                    autoTcounter++;
                    break;
                case 18:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L19.Checked = true;
                            InputTestInit(In1_L19, In1_L19_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 18 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 18 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }
                    autoTcounter++;
                    break;

                case 19:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L20.Checked = true;
                            InputTestInit(In1_L20, In1_L20_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 19 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 19 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 20:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L21.Checked = true;
                            InputTestInit(In1_L21, In1_L21_led);
                            //  IncProgressVal(1);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 20 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 20 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 21:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L22.Checked = true;
                            InputTestInit(In1_L22, In1_L22_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 21 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 21 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 22:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L23.Checked = true;
                            InputTestInit(In1_L23, In1_L23_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 22 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 22 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 23:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L24.Checked = true;
                            InputTestInit(In1_L24, In1_L24_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 23 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 23 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 24:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L25.Checked = true;
                            InputTestInit(In1_L25, In1_L25_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 24 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 24 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 25:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L26.Checked = true;
                            InputTestInit(In1_L26, In1_L26_led);
                            // IncProgressVal(1);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 25 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 25 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 26:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L27.Checked = true;
                            InputTestInit(In1_L27, In1_L27_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 26 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 26 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 27:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L28.Checked = true;
                            InputTestInit(In1_L28, In1_L28_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 27 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 27 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 28:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In1_L29.Checked = true;
                            InputTestInit(In1_L29, In1_L29_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 28 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 28 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 29:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In2_L1.Checked = true;
                            Input2TestInit(In2_L1, In2_L1_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board1,input 29 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board1,input 29 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 30:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In2_L2.Checked = true;
                            Input2TestInit(In2_L2, In2_L2_led);

                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 1 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 1 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 31:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.InputPass)
                        {
                            In2_L3.Checked = true;
                            Input2TestInit(In2_L3, In2_L3_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 2 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 2 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 32:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L4.Checked = true;
                            Input2TestInit(In2_L4, In2_L4_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 3 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 3 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 33:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L5.Checked = true;
                            Input2TestInit(In2_L5, In2_L5_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 4 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 4 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 34:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L6.Checked = true;
                            Input2TestInit(In2_L6, In2_L6_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 5 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 5 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 35:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L7.Checked = true;
                            Input2TestInit(In2_L7, In2_L7_led);
                            // IncProgressVal(1);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 6 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 6 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 36:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L8.Checked = true;
                            Input2TestInit(In2_L8, In2_L8_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 7 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 7 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 37:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L9.Checked = true;
                            Input2TestInit(In2_L9, In2_L9_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 8 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 8 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 38:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L10.Checked = true;
                            Input2TestInit(In2_L10, In2_L10_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 9 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 9 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 39:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L11.Checked = true;
                            Input2TestInit(In2_L11, In2_L11_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 10 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 10 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 40:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L12.Checked = true;
                            Input2TestInit(In2_L12, In2_L12_led);
                            //  IncProgressVal(1);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 11 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 11 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 41:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L13.Checked = true;
                            Input2TestInit(In2_L13, In2_L13_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 12 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 12 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
                case 42:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            In2_L14.Checked = true;
                            Input2TestInit(In2_L14, In2_L14_led);
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 13 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);
                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 13 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;

                case 43:
                    if (AutoTest.InputRec2)
                    {
                        if (AutoTest.Input2Pass)
                        {
                            inerTSt = 0;
                            autoTSt++;  //?
                            autoTcounter = 0;
                            IncProgressVal(3);
                            feedbackTb.Text = "InPut Board Test Done.";
                        }
                        else
                        {
                            feedbackTb.Text = "InPut Board2,input 14 Failed!";
                            TestCompleteProcedure(feedbackTb.Text);

                        }
                    }
                    else if (autoTcounter > TimingLimit.InputTime)
                    {
                        feedbackTb.Text = "InPut Board2,input 14 Failed!";
                        TestCompleteProcedure(feedbackTb.Text);
                    }

                    autoTcounter++;
                    break;
            }
        }
        byte[] crUnlockData = new byte[1] { 0 };
        private void StopAutoCmd()
        {
            TestEndProcedure();
        }

        private void TestCompleteProcedure(string result)
        {
            AutoTest.AutoTestResult = result;
            if (LoginData.IsLogin && serialManager != null)
            {
                checkTestAndPrint();
            }
            TestEndProcedure();

        }
        private void TestCompleteProcedure()
        {
            AutoTest.AutoTestResult = feedbackTb.Text;
            if (LoginData.IsLogin && serialManager != null)
            {
                checkTestAndPrint();
            }
            TestEndProcedure();

        }
        private void TestEndProcedure()
        {
            SendSerialTestCmd(crUnlockData, ID.Network_Board, MsType.CrashUnlock_Cancling);
            Thread.Sleep(600);

            AutoTest.CrashUnlockRec = false;
            AutoTest.TstRepeat = false;
            autoTest_timer.Stop();
            autoTest_timer2.Stop();

            updateUI_timer.Start();
            tabControl1.SelectedTab = netTab;
            autoTSt = 0;
            inerTSt = 0;
            autoTcounter = 0;
            PannelLock(false);
            feedbackPannel.BackColor = Color.Transparent;
            btnConnect.BackColor = connectPanelColor;
            feedbackTb.Text = "";
            progressBar1.Visible = false;
            //CheckSerialBox.Visible = true;
            boardsStatusPannel.Visible = true;
            if (stopWatch != null)     //Dena
            {
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}", //:{2:00}
                    ts.Minutes, ts.Seconds);   //, ts.Milliseconds / 10

                testTime.Text = elapsedTime + " min";
            }
            //TestInfoClass.CheckNewDay();
            TestMode.Auto = false;
            BCMPowerSw.Checked = false;

        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                StartStopList.Clear();
                StartStopList.Add(2);
                SendSerialTestCmd(crUnlockData, ID.Network_Board, MsType.CrashUnlock_Cancling);
                Thread.Sleep(800);
                SendSerialTestCmd(StartStopList, ID.DigitalOutput, MsType.StartStopCmd);
                Thread.Sleep(200);
                TestEndProcedure();

                //stopFlag = true;
                //SendSerialTestCmd(StartStopList, ID.DigitalOutput, MsType.StartStopCmd);

                //Thread.Sleep(200);
                //BCMPower.Checked = false;
                //StopAutoTest();
                //stopWatch.Stop();
                //TimeSpan ts = stopWatch.Elapsed;
                //string elapsedTime = String.Format("{0:00}:{1:00}", //:{2:00}
                //ts.Minutes, ts.Seconds);   //, ts.Milliseconds / 10

                //testTime.Text = elapsedTime + " min";

                //TestMode.Auto = false;

            }
            catch (Exception ex)
            {

            }
        }
        private int findRoot(long i)
        {
            int c = 0;
            while (i > 1)
            {
                i = i / 2;
                c++;
            }
            return c;
        }
        resultMBox ms = new resultMBox(); //testPass, Output1Pass, Output2Pass, Output3Pass, OutputDPass, NetworkPass, InputPass, Input2Pass

        private void ShowTestPass()
        {
            AutoTest.resultBoxColor = Color.Lime;
            AutoTest.resultBoxText = "Test Pass";
            ms = new resultMBox("Test Pass", Color.Lime); //
            ms.Show();
        }
        private void ShowTestFail()
        {
            AutoTest.resultBoxColor = Color.Red;
            AutoTest.resultBoxText = "Test Fail";
            ms = new resultMBox("Test Fail", Color.Red); //

            ms.Show();
        }
        private void ShowPartTestPass()
        {
            AutoTest.resultBoxColor = Color.SkyBlue;
            AutoTest.resultBoxText = "Part Test Pass";
            ms = new resultMBox("Test Pass", Color.SkyBlue);
            ms.Show();
        }
        bool testPass;
        int TST_ELEMENT_CNT = 5;
        private void checkTestAndPrint()
        {
            //CreateReport();
            testPass = AutoTest.NetworkPass;
            AutoTest.AutoTestResult += "Network: " + AutoTest.NetworkPass;
            AutoTest.tstCnt = 1;
            if (outputTestSw.Checked)
            {
                testPass = AutoTest.Output1Pass & AutoTest.Output2Pass & AutoTest.Output3Pass & AutoTest.OutputDPass;
                AutoTest.AutoTestResult += ", output1: " + AutoTest.Output1Pass + " , output2: " + AutoTest.Output2Pass + " , output3: " + AutoTest.Output3Pass + " , Digitaloutput: " + AutoTest.OutputDPass;
                AutoTest.tstCnt++;
            }
            if (inputTestSw.Checked)
            {
                testPass &= AutoTest.InputPass & AutoTest.Input2Pass;

                //AutoTest.AutoTestResult += "  Input1:" + AutoTest.InputPass + ", Input2:" + AutoTest.Input2Pass;
                AutoTest.AutoTestResult += " , Input: " + (AutoTest.InputPass & AutoTest.Input2Pass);
                AutoTest.tstCnt++;
            }
            if (loadTest2.Checked)
            {
                testPass &= AutoTest.LoadTest2Pass;
                AutoTest.AutoTestResult += " , LoadTestPass: " + AutoTest.LoadTest2Pass;
                AutoTest.tstCnt++;
            }
            if (CASTest.Checked)
            {
                testPass &= AutoTest.CasTestPassed;
                AutoTest.AutoTestResult += " , CASTestPass: " + AutoTest.CASTestPass;
                AutoTest.tstCnt++;
            }
            //
            AutoTest.AutoTestPass = testPass;
            CreateReport();
            if (testPass && AutoTest.tstCnt == TST_ELEMENT_CNT)
            {

                if (printPassSw.Checked)
                {
                    TestInfoClass.Barcode = MakeBarcode();
                    TestInfoClass.TotalTestPass++;
                    TestInfoClass.UpdateTrackingSt(1);
                    if (AutoTest.printrFlag)
                        printer.TSCPrint(TestInfoClass.Barcode);
                }
                AutoTest.AutoTestResult = "All Tests Passed.";
                ShowTestPass();
                Thread.Sleep(250);

            }
            else if (testPass && AutoTest.tstCnt != TST_ELEMENT_CNT)
            {
                // AutoTest.AutoTestResult = "?";
                ShowPartTestPass();
                Thread.Sleep(250);
            }
            else if (!testPass)
            {

                if (printFailSw.Checked && AutoTest.tstCnt == TST_ELEMENT_CNT)
                {
                    TestInfoClass.TotalTestFail++;
                    var labelText = "Test Fail :  " + TestInfoClass.TrackingNumSt;
                    TestInfoClass.UpdateTrackingSt(1);
                    if (AutoTest.printrFlag)
                        printer.TSCPrintFail(labelText);
                    ExportPDF();
                }

                ShowTestFail();
                Thread.Sleep(250);



            }
            DataBase.SaveTestSpecTblInDb();


        }

        private void checkTestAndPrint2()
        {
            try
            {
                TestInfoClass.UpdateTrackingSt(1);
                CreateReport();

                testPass = AutoTest.NetworkPass;
                if (outputTestSw.Checked)
                    testPass &= AutoTest.Output1Pass & AutoTest.Output2Pass & AutoTest.Output3Pass & AutoTest.OutputDPass;
                if (inputTestSw.Checked)
                    testPass &= AutoTest.InputPass & AutoTest.Input2Pass;

                //if (loadTest1.Checked)
                //    testPass &= AutoTest.LoadTest1Passed;
                if (loadTest2.Checked)
                    testPass &= AutoTest.LoadTest2Pass;
                if (CASTest.Checked)
                    testPass &= AutoTest.CasTestPassed;
                AutoTest.AutoTestPass = testPass;
                //MessageBox.Show("testPass:" + testPass + " ,output1:" + AutoTest.Output1Pass + ", output2:" + AutoTest.Output2Pass + ", output3:" + AutoTest.Output3Pass + ", Digitaloutput:" + AutoTest.OutputDPass +

                // ", Network:" + AutoTest.NetworkPass + ", Input1:" + AutoTest.InputPass + ", Input2:" + AutoTest.Input2Pass + ", Cas Test:" + AutoTest.CasTestPass);
                ////

                if (testPass)
                {
                    ShowTestPass();
                    Thread.Sleep(250);
                    try
                    {

                        TestInfoClass.Barcode = MakeBarcode();
                        TestInfoClass.TotalTestPass++;
                        if (AutoTest.printrFlag & printPassSw.Checked)
                            printer.TSCPrint(TestInfoClass.Barcode);
                        else if (printPassSw.Checked)
                            AutoClosingMessageBox.Show("TSC Printer Not Found!", "Attention!", 3000);

                        // printer.TSCPrint(TestInfoClass.Barcode, labelText);
                        //printer.Print(TestInfoClass.Barcode, labelText);
                        //printer.print_Click(TestInfoClass.Barcode);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    TestInfoClass.TotalTestFail++;
                    var labelText = "Test Fail :  " + TestInfoClass.TrackingNumSt;
                    if (AutoTest.printrFlag & printPassSw.Checked)
                        printer.TSCPrintFail(labelText);
                    else if (printPassSw.Checked)
                        AutoClosingMessageBox.Show("TSC Printer Not Found!", "Attention!", 3000);
                    ShowTestFail();
                    Thread.Sleep(250);
                    //
                    PdfExportOptions pdfOptions = report.ExportOptions.Pdf;

                    // Specify the pages to export.
                    //pdfOptions.PageRange = "1, 3-5";

                    // Specify the quality of exported images.
                    pdfOptions.ConvertImagesToJpeg = false;
                    pdfOptions.ImageQuality = PdfJpegImageQuality.Medium;

                    // Specify the PDF/A-compatibility.
                    pdfOptions.PdfACompatibility = PdfACompatibility.PdfA3b;

                    // Specify the document options.
                    pdfOptions.DocumentOptions.Application = "Test Application";
                    pdfOptions.DocumentOptions.Author = "DX Documentation Team";
                    pdfOptions.DocumentOptions.Keywords = "DevExpress, Reporting, PDF";
                    pdfOptions.DocumentOptions.Producer = Environment.UserName.ToString();
                    pdfOptions.DocumentOptions.Subject = "Document Subject";
                    pdfOptions.DocumentOptions.Title = "Document Title";
                    //

                    // report.ExportToPdf(path2 + @"\Reports\" + $"{TestInfoClass.TrackingNumSt}.pdf", pdfOptions);//
                    var date = PersianConverterDate.ToShamsi(DateTime.Now);
                    date = Regex.Replace(date, @"\/", "_");
                    string dir = AutoTest.SaveReportPath + "\\" + date + "\\";

                    // If directory does not exist, create it
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    report.ExportToPdf(dir + $"{TestInfoClass.TrackingNumSt}.pdf", pdfOptions);
                    DataBase.SaveTestSpecTblInDb();
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void ExportPDF()
        {
            PdfExportOptions pdfOptions = report.ExportOptions.Pdf;

            // Specify the pages to export.
            //pdfOptions.PageRange = "1, 3-5";

            // Specify the quality of exported images.
            pdfOptions.ConvertImagesToJpeg = false;
            pdfOptions.ImageQuality = PdfJpegImageQuality.Medium;

            // Specify the PDF/A-compatibility.
            pdfOptions.PdfACompatibility = PdfACompatibility.PdfA3b;

            // Specify the document options.
            pdfOptions.DocumentOptions.Application = "Test Application";
            pdfOptions.DocumentOptions.Author = "DX Documentation Team";
            pdfOptions.DocumentOptions.Keywords = "DevExpress, Reporting, PDF";
            pdfOptions.DocumentOptions.Producer = Environment.UserName.ToString();
            pdfOptions.DocumentOptions.Subject = "Document Subject";
            pdfOptions.DocumentOptions.Title = "Document Title";
            //

            // report.ExportToPdf(path2 + @"\Reports\" + $"{TestInfoClass.TrackingNumSt}.pdf", pdfOptions);//
            //string dir = AutoTest.SaveReportPath + @"\Reports\";
            //string dir = AutoTest.SaveReportPath+ "\\" + @"Reports" + "\\" ;
            var date = PersianConverterDate.ToShamsi(DateTime.Now);
            date = Regex.Replace(date, @"\/", "_");
            string dir = AutoTest.SaveReportPath + "\\" + date + "\\";

            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            report.ExportToPdf(dir + $"{TestInfoClass.TrackingNumSt}.pdf", pdfOptions);
        }
        private string MakeBarcode()
        {
            string productDay, productSerial;
            TestInfoClass.DayNum = PersianConverterDate.DeyNumber(DateTime.Now);
            TestInfoClass.ProductSerial++;
            productDay = TestInfoClass.DayNum.ToString("000");
            productSerial = TestInfoClass.ProductSerial.ToString("0000");    //(++barcodeCounter).ToString("0000");
                                                                             //string bar = year + deviceCode + changeTurn + designerCode + productDay + productSerial;
            string bar = lbl.barcd_content + productDay + productSerial;
            bar = Regex.Replace(bar, @"\s", "");
            return bar;
        }
        string[] vr = new string[4];
        private void CreateReport()
        {
            report = new XtraReport1();

            var date = PersianConverterDate.ToShamsi(DateTime.Now);
            var time = DateTime.Now.ToString("HH:mm:ss tt");
            report.ReportGetData(date, time, SettingHouse.UserName);
            string res = AutoTest.AutoTestPass == true ? "Pass" : "Fail";
            vr[0] = TestInfoClass.BCM_SoftwareVr_Exp;
            vr[1] = TestInfoClass.CAS_SoftwareVr_Exp;
            vr[2] = TestInfoClass.BCM_HardWareVr_Exp;
            report.loadTestSpecification(TestInfoClass.PartNum, TestInfoClass.TrackingNumSt, res, vr, TestInfoClass.Description);

            report.SetReportNames(netName, inputName, outputName);
            report.reportGetData2(netReport, inputReport, outputReport, outputCurReport, inputVoltReport);
            report.reportGetData3(AutoTest.AutoTestResult, AutoTest.AutoTestResult2, AutoTest.InputMsg);

        }
        //private void btnStartReport_Func()
        //{
        //    if (btnStartReport.Checked)
        //    {
        //        btnStartReport.Image = Properties.Resources.onState;
        //        report = new XtraReport1();

        //        var date = PersianConverterDate.ToShamsi(DateTime.Now);
        //        var time = DateTime.Now.ToString("HH:mm:ss tt");
        //        report.ReportGetData(date, time, SettingHouse.UserName);
        //        vr[0] = TestInfoClass.BCM_SoftwareVr_Exp;
        //        vr[1] = TestInfoClass.CAS_SoftwareVr_Exp;
        //        vr[2] = TestInfoClass.BCM_HardWareVr_Exp;


        //        report.loadTestSpecification(TestInfoClass.PartNum, TestInfoClass.TrackingNumSt, TestInfoClass.TestLab, vr, TestInfoClass.Description);
        //        report.SetReportNames(netName, inputName, outputName);

        //        //test
        //        // Save the report to a stream.
        //        MemoryStream stream = new MemoryStream();
        //        report.SaveLayout(stream);

        //        // Prepare the stream for reading.
        //        stream.Position = 0;

        //        // Insert the report to a database.
        //        using (StreamReader sr = new StreamReader(stream))
        //        {
        //            // Read the report from the stream to a string variable.
        //            string s = sr.ReadToEnd();

        //            // Add a row to a table.
        //            //DataTable dt = dataSet1.Tables["Reports"];
        //            //DataRow row = dt.NewRow();
        //            //row["Report"] = s;
        //            //dt.Rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        btnStartReport.Image = Properties.Resources.ofState;
        //        // MessageBox.Show("You Dont Have Log Permission", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (report != null)
                {
                    ReportPrintTool printTool = new ReportPrintTool(report);
                    printTool.ShowRibbonPreview();
                    //report.ExportToPdf(path2);//path2, pdfOptions
                    //if (TestInfoClass.TrackingNumStYear != null)
                    //    TestInfoClass.SaveReport(TestInfoClass.TrackingNumStYear, report);
                }
                else
                {
                    //MessageBox.Show("Report Not Created, Please Check Your Log Permission");
                    AutoClosingMessageBox.Show("Report Not Created, Please Check Your Log Permission", "Warning!", 2000);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
        #region report
        private void addParamNamesToReport()
        {
            netName[0] = BCM_CANHS.Text;
            netName[1] = BCM_CANLS.Text;
            netName[2] = BCM_LINF.Text;
            netName[3] = BCM_LINR.Text;
            netName[4] = CAS_CANLS.Text;
            netName[5] = ImmoTest.Text;
            netName[6] = KickTest.Text;
            netName[7] = TPMSTest.Text;
            netName[8] = RKETest.Text;
            netName[9] = LFTest.Text;
            netName[10] = RDHSLock1.Text;
            netName[11] = RDHSUnlock1.Text;
            netName[12] = LDHSLock1.Text;
            netName[13] = LDHSUnlock1.Text;

            //
            inputName[0] = In1_L1.Text;
            inputName[1] = In1_L2.Text;
            inputName[2] = In1_L3.Text;
            inputName[3] = In1_L4.Text;
            inputName[4] = In1_L5.Text;
            inputName[5] = In1_L6.Text;
            inputName[6] = In1_L7.Text;
            inputName[7] = In1_L8.Text;
            inputName[8] = In1_L9.Text;
            inputName[9] = In1_L10.Text;
            inputName[10] = In1_L11.Text;
            inputName[11] = In1_L12.Text;
            inputName[12] = In1_L13.Text;
            inputName[13] = In1_L14.Text;
            inputName[14] = In1_L15.Text;
            inputName[15] = In1_L16.Text;
            inputName[16] = In1_L17.Text;
            inputName[17] = In1_L18.Text;
            inputName[18] = In1_L19.Text;
            inputName[19] = In1_L20.Text;
            inputName[20] = In1_L21.Text;
            inputName[21] = In1_L22.Text;
            inputName[22] = In1_L23.Text;
            inputName[23] = In1_L24.Text;
            inputName[24] = In1_L25.Text;
            inputName[25] = In1_L26.Text;
            inputName[26] = In1_L27.Text;
            inputName[27] = In1_L28.Text;
            inputName[28] = In1_L29.Text;

            inputName[29] = In2_L1.Text;
            inputName[30] = In2_L2.Text;
            inputName[31] = In2_L3.Text;
            inputName[32] = In2_L4.Text;
            inputName[33] = In2_L5.Text;
            inputName[34] = In2_L6.Text;
            inputName[35] = In2_L7.Text;
            inputName[36] = In2_L8.Text;
            inputName[37] = In2_L9.Text;
            inputName[38] = In2_L10.Text;
            inputName[39] = In2_L11.Text;
            inputName[40] = In2_H1.Text;
            inputName[41] = In2_H5.Text;
            inputName[42] = In2_H2.Text;
            inputName[43] = In2_H3.Text;
            inputName[44] = In2_H4.Text;
            inputName[45] = In2_L12.Text;
            inputName[46] = In2_L13.Text;
            inputName[47] = In2_L14.Text;
            inputName[48] = In1_An1.Text;

            //
            int j = 0;
            outputName[j++] = OutOne1.Text;
            outputName[j++] = OutOne2.Text;
            outputName[j++] = OutOne3.Text;
            outputName[j++] = OutOne4.Text;
            outputName[j++] = OutOne5.Text;
            outputName[j++] = OutOne6.Text;
            outputName[j++] = OutOne7.Text;
            outputName[j++] = OutOne8.Text;
            outputName[j++] = OutOne9.Text;
            outputName[j++] = OutOne10.Text;
            outputName[j++] = OutOne11.Text;
            outputName[j++] = OutOne12.Text;
            outputName[j++] = OutOne13.Text;
            outputName[j++] = OutOne14.Text;

            outputName[j++] = OutTwo1.Text;
            outputName[j++] = OutTwo2.Text;
            outputName[j++] = OutTwo3.Text;
            outputName[j++] = OutTwo4.Text;
            outputName[j++] = OutTwo5.Text;
            outputName[j++] = OutTwo6.Text;
            outputName[j++] = OutTwo7.Text;
            outputName[j++] = OutTwo8.Text;
            outputName[j++] = OutTwo9.Text;
            outputName[j++] = OutTwo10.Text;
            outputName[j++] = OutTwo11.Text;
            outputName[j++] = OutTwo12.Text;
            outputName[j++] = OutTwo13.Text;
            outputName[j++] = OutTwo14.Text;   //27

            outputName[j++] = OutThree1.Text;
            outputName[j++] = OutThree2.Text;
            outputName[j++] = OutThree3.Text;

            outputName[j++] = OutThree4.Text; //cas
            outputName[j++] = OutThree5.Text;

            outputName[j++] = OutDL1.Text; //33
            outputName[j++] = OutDL2.Text;
            outputName[j++] = OutDL3.Text;
            outputName[j++] = OutDL4.Text;

            outputName[j++] = OutDH1.Text;   //37
            outputName[j++] = OutDH2.Text;
            outputName[j++] = OutDH3.Text;
            outputName[j++] = OutDH4.Text;
            outputName[j++] = OutDH5.Text;
            outputName[j++] = OutDH6.Text;   //42
            outputName[j++] = OutDH7.Text;
            outputName[j++] = OutDH8.Text;
            outputName[j++] = OutDH9.Text;
            outputName[j++] = OutDH10.Text;
        }
        #endregion
        #region connection
        string detectedPort;
        private void serialConnectionCheck()
        {
            try
            {
                comPortCb.Items.Clear();
                string[] ArrayComPortsNames = null;
                List<string> ArrayComPortsName = new List<string>();
                ArrayComPortsNames = SerialPort.GetPortNames();
                ArrayComPortsName = SerialPort.GetPortNames().Distinct().ToList();

                if (ArrayComPortsName.Count > 0)
                {
                    foreach (var item in ArrayComPortsName)
                    {
                        comPortCb.Items.Add(item);

                    }
                    comPortCb.Text = ArrayComPortsNames[0];
                    detectedPort = DetctConectedPort(ArrayComPortsName);
                    if (detectedPort != null)
                    {
                        AutoConnectPort(detectedPort);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                AutoClosingMessageBox.Show(ex.ToString(), "Warning!", 2000);

            }
        }
        int detectCounter = 0;
        string conectedCom = null;



        private string DetctConectedPort(List<string> ArrayComPortsName)
        {
            SerialManager testSerial;
            string com;

            byte[] testbuf = new byte[4];
            testbuf[0] = (byte)ID.InputB1;// 0x11;
            testbuf[1] = (byte)MsType.CheckConnection;  //AA
            testbuf[2] = 0x0;
            testbuf[3] = 0x55;
            for (int i = 0; i < ArrayComPortsName.Count; i++)
            {
                com = ArrayComPortsName[i];

                testSerial = new SerialManager(com.ToString(), 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One, false);

                try
                {
                    testSerial.OpenPort();
                }
                catch (Exception e)
                {
                    testSerial.ClosePort();
                    continue;
                }

                if (testSerial.IsOpen())
                {

                    bool result = testSerial.Write(testbuf);
                    detectCounter = 0;
                    do
                    {
                        if (testSerial.IncomingBytes != null)
                            if ((testSerial.IncomingBytes[0] == 0x11) & (testSerial.IncomingBytes[1] == 0xAA)) //0xAA
                            {
                                conectedCom = com;
                                detectCounter = 1000000000 - 1;
                            }
                        //};
                        detectCounter++;
                    } while (detectCounter < 1000000000);
                    if (conectedCom != null)
                    {
                        testSerial.ClosePort();
                        return com;
                    }
                }
                testSerial.ClosePort();
            }

            return null;
        }



        private void AutoConnectPort(string com)
        {
            try
            {
                comPortCb.Text = com;
                serialManager = new SerialManager(com.ToString(), 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One, false);
                serialManager.OpenPort();
                if (serialManager.IsOpen())
                {
                    serialManager.SerialManagerDataReceive += ReadSerialEventHispeed;
                    btnConnect.BackColor = Color.MediumSpringGreen;
                    serialCheck_timer.Start();
                    updateUI_timer.Start();
                    //CheckSerialBox.BackColor = Color.Lime;
                    //connectPanel.BackColor = Color.Lime;
                    //  SerialConnection.Text = comPortCb.EditValue.ToString();
                    btnConnect.Enabled = false;



                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                AutoClosingMessageBox.Show(ex.ToString(), "Warning!", 2000);

            }
        }
        byte[] data = new byte[0];
        private void refreshVersion()
        {
            BCM_SoftwareVr.Text = "";
            data = new byte[1];
            data[0] = (byte)version.BCM_Software;
            SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);
            TestInfoClass.BCM_SoftwareVr_Rec = "";
        }
        private void refreshVersions(byte select)
        {
            // BCM_SoftwareVer.Text = "";
            data = new byte[1];
            data[0] = select;
            SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);
        }
        private void btnRefreshVr_Click(object sender, EventArgs e)
        {
            // refreshVersion();
            // refreshVersions((byte)version.BCM_Software);
            // data = new byte[0];
            SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);


        }
        private void btnBCMHardVr_Click(object sender, EventArgs e)
        {
            // refreshVersions((byte)version.BCM_Hardware);
            //  data = new byte[0];
            SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_HardwarVersion);

        }

        private void btnBCMBootVr_Click(object sender, EventArgs e)
        {
            //  refreshVersions((byte)version.BCM_BootLoader);
            SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_BootloadrVersion);

        }

        private void btnCASSoftVr_Click(object sender, EventArgs e)
        {
            // refreshVersions((byte)version.CAS_Software);
            SendSerialTestCmd(data, ID.Network_Board, MsType.CAS_SoftwarVersion);

        }

        private void btnCASHardtVr_Click(object sender, EventArgs e)
        {
            // refreshVersions((byte)version.CAS_Hardware);
            SendSerialTestCmd(data, ID.Network_Board, MsType.CAS_HardwarVersion);

        }

        private void btnCASBootVr_Click(object sender, EventArgs e)
        {
            //refreshVersions((byte)version.CAS_BootLoader);
            SendSerialTestCmd(data, ID.Network_Board, MsType.CAS_BootloadrVersion);

        }
        private void ReadSerialEventHispeed(object sender, EventArgs e)
        {
            var serialString = serialManager.IncomingBytes;
            if ((serialString == null))
                return;
            serialManager.IncomingBytes = null;
            var dataList = new List<List<byte>>();


            serialManager.ExctractDataFromSerialByte(dataList, serialString);

            if (dataList.Count != 0)
                IncomingSerialDataDetachment(dataList);

            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateControlsDelegate(ShowPacket));
            }
            else
            {
                ShowPacket();
            }

        }
        public void IncomingSerialDataDetachment(List<List<byte>> readData)
        {
            //swBoardPower
            try
            {
                ID id = (ID)serialManager.id;
                MsType message = (MsType)serialManager.messageType;
                switch (id)
                {
                    case ID.InputB1:
                        switch (message)
                        {
                            case MsType.Current:
                                //  SetInputBoardCurrent(InputBoard.Input1, readData[0].ToArray());           //?
                                break;
                            case MsType.Welcome:
                                WelcomExtract(readData[0][0], readData[0][1]);
                                if (InvokeRequired)
                                    Invoke(new UpdateControlsDelegate(WelcomCheck_Pre));
                                else
                                    WelcomCheck_Pre();
                                break;

                        }

                        break;
                    case ID.InputB2:
                        switch (message)
                        {
                            case MsType.Current:
                                // SetInputBoardCurrent(InputBoard.Input2, readData[0].ToArray());
                                break;
                        }
                        break;
                    case ID.OutputB1:
                        switch (message)
                        {
                            case MsType.Current:
                                outputFeedback = new OutPutParams();
                                ReceiveParser.ExctractOutputData(readData[0].ToArray(), outputFeedback, output1Divid);
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate(SetOutputFeedBacksBoard1));
                                }
                                else
                                {
                                    SetOutputFeedBacksBoard1();
                                }
                                if (TestMode.Auto)
                                    AutoTest.Output1Rec = true;

                                break;
                            case MsType.OutputStatus1:
                                loadLoac = readData[0][1] + (readData[0][0] << 8);
                                loadStatus = readData[0][3] + (readData[0][2] << 8);
                                // AutoTest.LoadTest1B1Rec = true;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDel(LoadTest1B1), loadStatus, loadLoac, Color.YellowGreen);
                                }
                                else
                                {
                                    LoadTest1B1(loadStatus, loadLoac, Color.YellowGreen);
                                }

                                break;
                            case MsType.OutputStatus2:
                                loadStatus2 = readData[0][1] + (readData[0][0] << 8);
                                //AutoTest.LoadTest2B1Rec = true;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDel2(LoadTest2B1), loadStatus2, Color.SpringGreen);
                                }
                                else
                                {
                                    LoadTest2B1(loadStatus2, Color.SpringGreen);
                                }
                                break;
                        }
                        break;
                    case ID.OutputB2:
                        switch (message)
                        {
                            case MsType.Current:
                                outputFeedback = new OutPutParams();
                                ReceiveParser.ExctractOutputData(readData[0].ToArray(), outputFeedback, output1Divid);
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate(SetOutputFeedBacksBoard2));
                                }
                                else
                                {
                                    SetOutputFeedBacksBoard2();
                                }
                                if (TestMode.Auto)
                                    AutoTest.Output2Rec = true;

                                break;
                            case MsType.OutputStatus1:
                                loadLoac = readData[0][1] + (readData[0][0] << 8);
                                loadStatus = readData[0][3] + (readData[0][2] << 8);
                                //AutoTest.LoadTest1B2Rec = true;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDel(LoadTest1B2), loadStatus, loadLoac, Color.YellowGreen);
                                }
                                else
                                {
                                    LoadTest1B2(loadStatus, loadLoac, Color.YellowGreen);
                                }

                                break;
                            case MsType.OutputStatus2:
                                loadStatus2 = readData[0][1] + (readData[0][0] << 8);
                                // AutoTest.LoadTest2B2Rec = true;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDel2(LoadTest2B2), loadStatus2, Color.SpringGreen);
                                }
                                else
                                {
                                    LoadTest2B2(loadStatus2, Color.SpringGreen);
                                }
                                break;

                        }
                        break;
                    case ID.OutputB3:
                        switch (message)
                        {
                            case MsType.Current:
                                outputFeedback = new OutPutParams();
                                ReceiveParser.ExctractOutputData(readData[0].ToArray(), outputFeedback, output3Divid);
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate(SetOutputFeedBacksBoard3));
                                }
                                else
                                {
                                    SetOutputFeedBacksBoard3();
                                }
                                if (TestMode.Auto)
                                    AutoTest.Output3Rec = true;
                                break;
                            case MsType.OutputStatus1:
                                loadLoac = readData[0][1] + (readData[0][0] << 8);
                                loadStatus = readData[0][3] + (readData[0][2] << 8);
                                // AutoTest.LoadTest1B3Rec = true;
                                AutoTest.LoadTest1B3Cnt++;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDel(LoadTest1B3), loadStatus, loadLoac, Color.YellowGreen);
                                }
                                else
                                {
                                    LoadTest1B3(loadStatus, loadLoac, Color.YellowGreen);
                                }

                                break;
                            case MsType.OutputStatus2:
                                loadStatus2 = readData[0][1] + (readData[0][0] << 8);
                                // AutoTest.LoadTest2B3Rec = true;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDel2(LoadTest2B3), loadStatus2, Color.SpringGreen);
                                }
                                else
                                {
                                    LoadTest2B3(loadStatus2, Color.SpringGreen);
                                }
                                break;
                        }
                        break;
                    case ID.DigitalOutput:
                        switch (message)
                        {
                            case MsType.DigitalOutputBStatus:
                                digitalOutputFeedback = new OutPutParams();
                                ReceiveParser.ExctractOutputDataDigitalOutputBoard(readData[0].ToArray(), digitalOutputFeedback);
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate2(SetDigitalOutputCheck), digitalOutputFeedback);
                                }
                                else
                                {
                                    SetDigitalOutputCheck(digitalOutputFeedback);
                                }

                                // SetDigitalOutputFeedBacksBoard();
                                if (TestMode.Auto)
                                    AutoTest.OutputDRec = true;
                                break;
                            case MsType.StartStopCmd:
                                if (readData[0][0] == 1)
                                {
                                    if (this.InvokeRequired)
                                    {
                                        this.Invoke(new UpdateControlsDelegate(AutoTestStart));
                                    }
                                    else
                                    {
                                        AutoTestStart();
                                    }

                                }
                                else if (readData[0][0] == 2)
                                {
                                    if (this.InvokeRequired)
                                    {
                                        this.Invoke(new UpdateControlsDelegate(StopAutoCmd));
                                    }
                                    else
                                    {
                                        StopAutoCmd();
                                        // StopAutoTest();
                                    }
                                    //XtraMessageBox.Show("Auto Test Stopped By User");
                                    // BCMPowerSw.Checked = false;
                                    feedbackTb.Text += "Auto Test Stopped By User";


                                }
                                break;
                        }
                        break;

                    case ID.CAS_tBoard:
                        if (this.InvokeRequired)

                            this.Invoke(new UpdateCASDelegate(CAS_Extract), message, readData[0]);

                        else

                            CAS_Extract(message, readData[0]);



                        break;
                    case ID.Network_Board:
                        switch (message)
                        {
                            case MsType.CANMessage:
                                netParamsFeedback = new NetParams();
                                ReceiveParser.ExctractNetDataCan(readData[0].ToArray(), netParamsFeedback);
                                SetNetFeedBacksCan();
                                break;
                            case MsType.LINMessage:
                                netParamsFeedback = new NetParams();
                                ReceiveParser.ExctractNetDataLin(readData[0].ToArray(), netParamsFeedback);
                                SetNetFeedBacksLin();
                                break;
                            case MsType.BCMAnalogInputStatus:

                                ReceiveParser.ExctractAnalogInputData(readData[0].ToArray());
                                if (this.InvokeRequired)

                                    this.Invoke(new UpdateControlsDelegate(SetAnalogInputFeedBacks));
                                else
                                    SetAnalogInputFeedBacks();
                                break;
                            case MsType.BCMInputStatus:
                                inputParamFeedback = new InputParams();
                                ReceiveParser.ExctractInputDataInputBoard(readData[0].ToArray(), inputParamFeedback);
                                SetInputCheck(inputParamFeedback);

                                //if (TestMode.Auto)
                                //    AutoTest.InputRec = true;
                                break;
                            case MsType.CASInputStatus:
                                if (this.InvokeRequired)

                                    this.Invoke(new CASInputExtractDel(CASInputExtract), readData[0]);
                                else
                                    CASInputExtract(readData[0]);
                                break;

                            case MsType.BCM_SoftwarVersion:
                                //SetVersion(ver, sb.ToString());
                                string s = ChangeToAscii(readData[0]);
                                TestInfoClass.BCM_SoftwareVr_Rec = s;
                                bcmVersionReceived = true;
                                //  SendSerialTestCmd(data, ID.Network_Board, MsType.CAS_SoftwarVersion);
                                break;
                            case MsType.CAS_SoftwarVersion:
                                s = ChangeToAscii(readData[0]);
                                TestInfoClass.CAS_SoftwareVr_Rec = s;
                                casVersionReceived = true;
                                // SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_HardwarVersion);
                                break;
                            case MsType.BCM_HardwarVersion:
                                s = ChangeToAscii(readData[0]);
                                TestInfoClass.BCM_HardWareVr_Rec = s;
                                //   SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_BootloadrVersion);
                                break;
                            case MsType.BCM_BootloadrVersion:
                                s = ExtractBootloaderVrsion(readData[0]);
                                TestInfoClass.BCM_BootloaderVr_Rec = s;
                                //  SendSerialTestCmd(data, ID.Network_Board, MsType.CAS_HardwarVersion);
                                break;
                            case MsType.CAS_HardwarVersion:
                                s = ChangeToAscii(readData[0]);
                                TestInfoClass.CAS_HardWareVr_Rec = s;
                                //  SendSerialTestCmd(data, ID.Network_Board, MsType.CAS_BootloadrVersion);
                                break;
                            case MsType.CAS_BootloadrVersion:
                                s = ChangeToAscii(readData[0]);
                                TestInfoClass.CAS_BootloaderVr_Rec = s;
                                break;
                            case MsType.CrashUnlock_Cancling:
                                var crData = readData[0][0];
                                AutoTest.CrashUnlockRec = true;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate3(CrashUnlockExtract), crData);
                                }
                                else
                                {
                                    CrashUnlockExtract(crData);
                                }
                                break;

                        }
                        if (TestMode.Auto)
                            AutoTest.NetworkRec = true;
                        break;
                    case ID.Power_Board:
                        switch (message)
                        {
                            case MsType.NoMessage:
                                var data = readData[0][0];
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate3(BCMPowerON), data);
                                }
                                else
                                {
                                    BCMPowerON(data);
                                }
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void BCMPowerON(int data)
        {
            if (data == 0x10)
            {
                if (!BCMPowerSw.Checked)
                    BCMPowerSw.Checked = true;
            }
        }
        //private void PowerShortState(int p)
        //{
        //    powerShortFlag = true;
        //    //PowerTestCmd();
        //    swBoardPower.Checked = false;
        //    swBoardPower.Enabled = true;
        //    powerSwitch.Checked = false;
        //    TestEndProcedure();
        //    // MessageBox.Show(string.Format("Danger!!!! Short State In : {0} Power Params! Stop Test! ", powerShortFlag), "Danger!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //    //     powerSwitch.Checked = false;
        //    switch (p)
        //    {
        //        case 0:
        //            break;
        //        case 1:
        //            ShowMessage(power1.Text);
        //            p1.BackColor = testRed;
        //            p1.Text = "";
        //            break;
        //        case 2:
        //            ShowMessage(power2.Text);
        //            p2.BackColor = testRed;
        //            p2.Text = "";
        //            break;
        //        case 3:
        //            p3.BackColor = testRed;
        //            p3.Text = "";
        //            ShowMessage(power3.Text);
        //            break;
        //        case 4:
        //            p4.BackColor = testRed;
        //            p4.Text = "";
        //            ShowMessage(power4.Text);
        //            break;
        //        case 5:
        //            p5.BackColor = testRed;
        //            p5.Text = "";
        //            ShowMessage(power5.Text);
        //            break;

        //        case 6:
        //            p6.BackColor = testRed;
        //            p6.Text = "";
        //            ShowMessage(power6.Text);
        //            break;
        //        case 7:
        //            p7.BackColor = testRed;
        //            p7.Text = "";
        //            ShowMessage(power7.Text);
        //            break;
        //        case 8:
        //            p8.BackColor = testRed;
        //            p8.Text = "";
        //            ShowMessage(power8.Text);
        //            break;
        //        case 9:
        //            p9.BackColor = testRed;
        //            p9.Text = "";
        //            ShowMessage(power9.Text);
        //            break;
        //        case 10:
        //            p10.BackColor = testRed;
        //            p10.Text = "";
        //            ShowMessage(power10.Text);
        //            break;
        //        case 11:
        //            p11.BackColor = testRed;
        //            p11.Text = "";
        //            ShowMessage(power11.Text);
        //            break;
        //        case 12:
        //            p12.BackColor = testRed;
        //            p12.Text = "";
        //            ShowMessage(power12.Text);
        //            break;
        //        case 13:
        //            p13.BackColor = testRed;
        //            p13.Text = "";
        //            ShowMessage(power13.Text);
        //            break;
        //        case 14:
        //            p14.BackColor = testRed;
        //            p14.Text = "";
        //            ShowMessage(power14.Text);
        //            break;
        //        case 15:
        //            p15.BackColor = testRed;
        //            p15.Text = "";
        //            ShowMessage(bpower1.Text);
        //            break;
        //        case 16:
        //            p16.BackColor = testRed;
        //            p16.Text = "";
        //            ShowMessage(bpower2.Text);
        //            break;
        //        case 17:
        //            p17.BackColor = testRed;
        //            p17.Text = "";
        //            ShowMessage(bpower3.Text);
        //            break;
        //        case 18:
        //            p18.BackColor = testRed;
        //            p18.Text = "";
        //            ShowMessage(bpower4.Text);
        //            break;
        //        case 19:
        //            p19.BackColor = testRed;
        //            p19.Text = "";
        //            ShowMessage(bpower5.Text);
        //            break;
        //        case 20:
        //            p20.BackColor = testRed;
        //            p20.Text = "";
        //            ShowMessage(bpower6.Text);
        //            break;
        //        case 21:
        //            p21.BackColor = testRed;
        //            p21.Text = "";
        //            ShowMessage(bpower7.Text);
        //            break;
        //        case 22:
        //            p22.BackColor = testRed;
        //            p22.Text = "";
        //            ShowMessage(bpower8.Text);
        //            break;
        //        case 0x19:
        //            AutoClosingMessageBox.Show(" Please Eject The BCM Module ! ", "Attention!", 3000);
        //            break;


        //    }
        //    //MessageBox.Show("Danger!!!! Short State In Power Params! Stop Test! ", "Danger!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //}
        private void CrashUnlockExtract(int data)
        {
            if (data == 0x1)
            {
                feedbackTb.Text = "Crash Unlock Cancling Ok";
            }
            else if (data == 0x0)
            {
                feedbackTb.Text = "Crash Unlock Cancling Fail!";

            }
        }
        byte cas_In_state = 0;
        private void CASInputExtract(List<byte> data)
        {
            byte number = data[0];
            switch (number)
            {
                case 1:
                    switch (cas_In_state)
                    {
                        case 0:
                            if (data[1] == 0)
                            {
                                casData[0] = 1;
                                casData[1] = 1;
                                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
                                cas_In_state++;
                            }
                            else
                            {
                                ledBulb_Click(In2_H5_led, 0);
                                CASInput.InpSt1 = 1;
                            }

                            break;

                        case 1:
                            if (data[1] == 1)
                            {
                                ledBulb_Click(In2_H5_led, 1);
                                AutoTest.CASInputOk++;
                            }
                            else
                            {
                                ledBulb_Click(In2_H5_led, 0);
                            }
                            cas_In_state = 0;
                            CASInput.InpSt1 = 1;
                            In2_H5.Checked = false;
                            break;
                    }

                    break;
                case 2:
                    switch (cas_In_state)
                    {
                        case 0:
                            if (data[1] == 0)
                            {
                                casData[0] = 2;
                                casData[1] = 1;
                                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
                                cas_In_state++;
                            }
                            else
                            {
                                ledBulb_Click(In2_H2_led, 0);
                                CASInput.InpSt2 = 1;
                            }

                            break;

                        case 1:
                            if (data[1] == 1)
                            {
                                ledBulb_Click(In2_H2_led, 1);
                                AutoTest.CASInputOk++;

                            }
                            else
                            {
                                ledBulb_Click(In2_H2_led, 0);
                            }
                            cas_In_state = 0;
                            In2_H2.Checked = false;
                            CASInput.InpSt2 = 1;
                            break;
                    }

                    break;
                case 3:
                    switch (cas_In_state)
                    {
                        case 0:
                            if (data[1] == 0)
                            {
                                casData[0] = 3;
                                casData[1] = 1;
                                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
                                cas_In_state++;
                            }
                            else
                            {
                                ledBulb_Click(In2_H3_led, 0);
                                CASInput.InpSt3 = 1;
                            }

                            break;

                        case 1:
                            if (data[1] == 1)
                            {
                                ledBulb_Click(In2_H3_led, 1);
                                AutoTest.CASInputOk++;

                            }
                            else
                            {
                                ledBulb_Click(In2_H3_led, 0);
                            }
                            cas_In_state = 0;
                            In2_H3.Checked = false;
                            CASInput.InpSt3 = 1;
                            break;
                    }

                    break;
                case 4:
                    switch (cas_In_state)
                    {
                        case 0:
                            if (data[1] == 0)
                            {
                                casData[0] = 4;
                                casData[1] = 1;
                                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
                                cas_In_state++;
                            }
                            else
                            {
                                ledBulb_Click(In2_H4_led, 0);
                                CASInput.InpSt4 = 1;
                            }

                            break;

                        case 1:
                            if (data[1] == 1)
                            {
                                ledBulb_Click(In2_H4_led, 1);
                                AutoTest.CASInputOk++;

                            }
                            else
                            {
                                ledBulb_Click(In2_H4_led, 0);

                            }
                            cas_In_state = 0;
                            In2_H4.Checked = false;
                            CASInput.InpSt4 = 1;
                            break;
                    }

                    break;
                case 5:
                    switch (cas_In_state)
                    {
                        case 0:
                            if (data[1] == 0)
                            {
                                casData[0] = 5;
                                casData[1] = 1;
                                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
                                cas_In_state++;
                            }
                            else
                            {
                                ledBulb_Click(In2_L12_led, 0);
                                CASInput.InpSt5 = 1;

                            }

                            break;

                        case 1:
                            if (data[1] == 1)
                            {
                                ledBulb_Click(In2_L12_led, 1);
                                AutoTest.CASInputOk++;

                            }
                            else
                            {
                                ledBulb_Click(In2_L12_led, 0);

                            }
                            cas_In_state = 0;
                            In2_L12.Checked = false;
                            CASInput.InpSt5 = 1;
                            break;
                    }

                    break;
                case 6:
                    switch (cas_In_state)
                    {
                        case 0:
                            if (data[1] == 0)
                            {
                                casData[0] = 6;
                                casData[1] = 1;
                                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
                                cas_In_state++;
                            }
                            else
                            {
                                ledBulb_Click(In2_L13_led, 0);
                                CASInput.InpSt6 = 1;

                            }

                            break;

                        case 1:
                            if (data[1] == 1)
                            {
                                ledBulb_Click(In2_L13_led, 1);
                                AutoTest.CASInputOk++;

                            }
                            else
                            {
                                ledBulb_Click(In2_L13_led, 0);

                            }
                            cas_In_state = 0;
                            In2_L13.Checked = false;
                            CASInput.InpSt6 = 1;
                            break;
                    }

                    break;
                case 7:
                    switch (cas_In_state)
                    {
                        case 0:
                            if (data[1] == 0)
                            {
                                casData[0] = 7;
                                casData[1] = 1;
                                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
                                cas_In_state++;
                            }
                            else
                            {
                                ledBulb_Click(In2_L14_led, 0);
                                CASInput.InpSt7 = 1;

                            }

                            break;

                        case 1:
                            if (data[1] == 1)
                            {
                                ledBulb_Click(In2_L14_led, 1);
                                AutoTest.CASInputOk++;
                            }
                            else
                            {
                                ledBulb_Click(In2_L14_led, 0);

                            }
                            cas_In_state = 0;
                            In2_L14.Checked = false;
                            CASInput.InpSt7 = 1;

                            if (AutoTest.CASInputOk == 7)
                            {
                                AutoTest.CasInputPass = true;
                            }
                            else
                            {
                                AutoTest.CasInputPass = false;

                            }
                            AutoTest.CASInputOk = 0;
                            break;
                    }

                    break;
            }

        }
        private void CasInpStReset()
        {
            CASInput.InpSt1 = 0;
            CASInput.InpSt2 = 0;
            CASInput.InpSt3 = 0;
            CASInput.InpSt4 = 0;
            CASInput.InpSt5 = 0;
            CASInput.InpSt6 = 0;
            CASInput.InpSt7 = 0;
        }
        private string ChangeToAscii(List<byte> Data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Data.Count; i++)
                if (Data[i] != 0xFF)
                    sb.Append((char)Data[i]);
            return sb.ToString();
        }
        private string ExtractBootloaderVrsion(List<byte> Data)
        {
            short bootMajor = (short)(Data[3] + (Data[2] << 8));
            var bootMinor = Data[5] + (Data[4] << 8);
            string s = bootMajor.ToString() + "." + bootMinor.ToString();
            return s;
        }
        private void SetVersion(byte ver, string sb)
        {
            data = new byte[1];
            switch (ver)
            {
                case 1:
                    TestInfoClass.BCM_SoftwareVr_Rec = sb;
                    data[0] = (byte)version.BCM_Hardware;
                    SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);
                    break;
                case 2:
                    TestInfoClass.BCM_HardWareVr_Rec = sb;
                    data[0] = (byte)version.BCM_BootLoader;
                    SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);
                    break;
                case 3:
                    TestInfoClass.BCM_BootloaderVr_Rec = sb;
                    data[0] = (byte)version.CAS_Software;
                    SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);
                    break;
                case 4:
                    TestInfoClass.CAS_SoftwareVr_Rec = sb;
                    data[0] = (byte)version.CAS_Hardware;
                    SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);
                    break;
                case 5:
                    TestInfoClass.CAS_HardWareVr_Rec = sb;
                    data[0] = (byte)version.CAS_BootLoader;
                    SendSerialTestCmd(data, ID.Network_Board, MsType.BCM_SoftwarVersion);
                    break;
                case 6:
                    TestInfoClass.CAS_BootloaderVr_Rec = sb;
                    break;
            }
        }
        private void CAS_Extract(MsType ms, List<byte> data)
        {
            AutoTest.CASRec = true;
            switch (ms)
            {
                case MsType.Immo:
                    if (ImmoTest.Checked)
                    {
                        if (data[0] == 1)
                        {
                            ledBulb_Click(ImmoTest_led, 1);
                            AutoTest.CasTestCountr++;   //1
                            AutoTest.CASTestPass = true;
                        }


                        else
                        {
                            ledBulb_Click(ImmoTest_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        ImmoTest.Checked = false;
                    }
                    break;
                case MsType.KickSensor:
                    if (KickTest.Checked)
                    {
                        if (data[0] == 1)
                        {
                            ledBulb_Click(KickTest_Led, 1);
                            AutoTest.CasTestCountr++;       //2
                            AutoTest.CASTestPass = true;
                        }

                        else
                        {
                            ledBulb_Click(KickTest_Led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        KickTest.Checked = false;
                    }
                    break;
                case MsType.TPMS:
                    if (TPMSTest.Checked)
                    {
                        if (data[0] == 1)
                        {
                            ledBulb_Click(TPMSTest_led, 1);
                            AutoTest.CasTestCountr++;       //3
                            AutoTest.CASTestPass = true;
                        }
                        else
                        {
                            ledBulb_Click(TPMSTest_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        TPMSTest.Checked = false;
                    }
                    break;
                case MsType.LF:
                    if (LFTest.Checked)
                    {
                        if (data[0] == 1)
                        {
                            ledBulb_Click(LFTest_led, 1);
                            AutoTest.CasTestCountr++;          //4
                            AutoTest.CASTestPass = true;
                        }

                        else
                        {
                            ledBulb_Click(LFTest_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        LFTest.Checked = false;
                    }
                    break;
                case MsType.RKE:
                    if (RKETest.Checked)
                    {
                        if (data[0] == 1)
                        {
                            ledBulb_Click(RKETest_led, 1);
                            AutoTest.CasTestCountr++;        //5
                            AutoTest.CASTestPass = true;
                        }

                        else
                        {
                            ledBulb_Click(RKETest_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        RKETest.Checked = false;
                    }
                    break;
                case MsType.DHS:
                    DHS_Extract(data);
                    break;
            }
        }
        private void DHS_Extract(List<byte> data)
        {
            switch (data[0])
            {
                case 1:
                    if (RDHSLock1.Checked)
                    {
                        if (data[1] == 1)
                        {
                            ledBulb_Click(RDHSLock_led, 1);
                            AutoTest.CasTestCountr++;       //6
                            AutoTest.CASTestPass = true;
                        }

                        else
                        {
                            ledBulb_Click(RDHSLock_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        RDHSLock1.Checked = false;
                    }
                    break;
                case 2:
                    if (RDHSUnlock1.Checked)
                    {
                        if (data[1] == 1)
                        {
                            ledBulb_Click(RDHSUnlock_led, 1);
                            AutoTest.CasTestCountr++;       //7
                            AutoTest.CASTestPass = true;
                        }

                        else
                        {
                            ledBulb_Click(RDHSUnlock_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        RDHSUnlock1.Checked = false;
                    }
                    break;
                case 3:
                    if (LDHSLock1.Checked)
                    {
                        if (data[1] == 1)
                        {
                            ledBulb_Click(LDHSLock_led, 1);
                            AutoTest.CasTestCountr++;        //8
                            AutoTest.CASTestPass = true;
                        }

                        else
                        {
                            ledBulb_Click(LDHSLock_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        LDHSLock1.Checked = false;
                    }
                    break;
                case 4:
                    if (LDHSUnlock1.Checked)
                    {
                        if (data[1] == 1)
                        {
                            ledBulb_Click(LDHSUnlock_led, 1);
                            AutoTest.CasTestCountr++;         //9
                            AutoTest.CASTestPass = true;
                        }

                        else
                        {
                            ledBulb_Click(LDHSUnlock_led, 0);
                            AutoTest.CASTestPass = false;
                        }

                        LDHSUnlock1.Checked = false;
                    }
                    break;
            }
        }
        private void LoadTest2B1(int status, Color loadsPass)
        {
            if (status == 0)
                AutoTest.LoadTest2Passed++;    //filter Eco relay??

            if ((status >> 0 & 0x1) == 1) OutOne1C.BackColor = loadsErrorT2; else OutOne1C.BackColor = loadsPass;
            if ((status >> 1 & 0x1) == 1) OutOne2C.BackColor = loadsErrorT2; else OutOne2C.BackColor = loadsPass;
            if ((status >> 2 & 0x1) == 1) OutOne3C.BackColor = loadsErrorT2; else OutOne3C.BackColor = loadsPass;
            if ((status >> 3 & 0x1) == 1) OutOne4C.BackColor = loadsErrorT2; else OutOne4C.BackColor = loadsPass;
            if ((status >> 4 & 0x1) == 1) OutOne5C.BackColor = loadsErrorT2; else OutOne5C.BackColor = loadsPass;
            if ((status >> 5 & 0x1) == 1) OutOne6C.BackColor = loadsErrorT2; else OutOne6C.BackColor = loadsPass;
            if ((status >> 6 & 0x1) == 1) OutOne7C.BackColor = loadsErrorT2; else OutOne7C.BackColor = loadsPass;
            if ((status >> 7 & 0x1) == 1) OutOne8C.BackColor = loadsErrorT2; else OutOne8C.BackColor = loadsPass;
            if ((status >> 8 & 0x1) == 1) OutOne9C.BackColor = loadsErrorT2; else OutOne9C.BackColor = loadsPass;
            if ((status >> 9 & 0x1) == 1) OutOne10C.BackColor = loadsErrorT2; else OutOne10C.BackColor = loadsPass;
            if ((status >> 10 & 0x1) == 1) OutOne11C.BackColor = loadsPass; else OutOne11C.BackColor = loadsPass;   //Eco Relay Dosnt Turn Off so we Pass it Manually
            if ((status >> 11 & 0x1) == 1) OutOne12C.BackColor = loadsErrorT2; else OutOne12C.BackColor = loadsPass;

        }
        private void LoadTest1B1(int status, int loc, Color loadsPass)
        {
            switch (loc)
            {
                case 1:
                    if ((status >> 0 & 0x1) == 1)
                    {
                        OutOne1C.BackColor = loadsError;

                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne1C.BackColor = loadsPass;
                    }

                    break;
                case 2:
                    if ((status >> 1 & 0x1) == 1)
                    {
                        OutOne2C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne2C.BackColor = loadsPass;
                    }

                    break;
                case 4:
                    if ((status >> 2 & 0x1) == 1)
                    {
                        OutOne3C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne3C.BackColor = loadsPass;
                    }

                    break;
                case 8:
                    if ((status >> 3 & 0x1) == 1)
                    {
                        OutOne4C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne4C.BackColor = loadsPass;
                    }
                    break;
                case 16:
                    if ((status >> 4 & 0x1) == 1)
                    {
                        OutOne5C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne5C.BackColor = loadsPass;
                    }
                    break;
                case 32:
                    if ((status >> 5 & 0x1) == 1)
                    {
                        OutOne6C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne6C.BackColor = loadsPass;
                    }
                    break;
                case 64:
                    if ((status >> 6 & 0x1) == 1)
                    {
                        OutOne7C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne7C.BackColor = loadsPass;
                    }
                    break;
                case 128:
                    if ((status >> 7 & 0x1) == 1)
                    {
                        OutOne8C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne8C.BackColor = loadsPass;
                    }
                    break;
                case 256:
                    if ((status >> 8 & 0x1) == 1)
                    {
                        OutOne9C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne9C.BackColor = loadsPass;
                    }
                    break;
                case 512:
                    if ((status >> 9 & 0x1) == 1)
                    {
                        OutOne10C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne10C.BackColor = loadsPass;
                    }
                    break;
                case 1024:
                    if ((status >> 10 & 0x1) == 1)
                    {
                        OutOne11C.BackColor = loadsPass;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne11C.BackColor = loadsPass;
                    }
                    break;
                case 2048:
                    if ((status >> 11 & 0x1) == 1)
                    {
                        OutOne12C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne12C.BackColor = loadsPass;
                    }
                    break;
                case 4096:
                    if ((status >> 12 & 0x1) == 1)
                    {
                        OutOne13C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne13C.BackColor = loadsPass;
                    }
                    break;
                case 8192:
                    if ((status >> 13 & 0x1) == 1)
                    {
                        OutOne14C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutOne14C.BackColor = loadsPass;
                    }
                    break;
            }
        }

        //*****************B2**************************************
        private void LoadTest1B2(int status, int loc, Color loadsPass)
        {
            switch (loc)
            {
                case 1:
                    if ((status >> 0 & 0x1) == 1)
                    {
                        OutTwo1C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo1C.BackColor = loadsPass;
                    }

                    break;
                case 2:
                    if ((status >> 1 & 0x1) == 1)
                    {
                        OutTwo2C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo2C.BackColor = loadsPass;
                    }
                    break;
                case 4:
                    if ((status >> 2 & 0x1) == 1)
                    {
                        OutTwo3C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo3C.BackColor = loadsPass;
                    }
                    break;
                case 8:
                    if ((status >> 3 & 0x1) == 1)
                    {
                        OutTwo4C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo4C.BackColor = loadsPass;
                    }
                    break;
                case 16:
                    if ((status >> 4 & 0x1) == 1)
                    {
                        OutTwo5C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo5C.BackColor = loadsPass;
                    }
                    break;
                case 32:
                    if ((status >> 5 & 0x1) == 1)
                    {
                        OutTwo6C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo6C.BackColor = loadsPass;
                    }
                    break;
                case 64:
                    if ((status >> 6 & 0x1) == 1)
                    {
                        OutTwo7C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo7C.BackColor = loadsPass;
                    }
                    break;
                case 128:
                    if ((status >> 7 & 0x1) == 1)
                    {
                        OutTwo8C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo8C.BackColor = loadsPass;
                    }
                    break;
                case 256:
                    if ((status >> 8 & 0x1) == 1)
                    {
                        OutTwo9C.BackColor = loadsPass;// loadsError;// because Alternator isnt load
                        AutoTest.LoadTest1Passed++; //added
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo9C.BackColor = loadsPass;


                    }
                    break;
                case 512:
                    if ((status >> 9 & 0x1) == 1)
                    {
                        OutTwo10C.BackColor = loadsError;


                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo10C.BackColor = loadsPass;


                    }
                    break;
                case 1024:
                    if ((status >> 10 & 0x1) == 1)
                    {
                        OutTwo11C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo11C.BackColor = loadsPass;
                    }
                    break;
                case 2048:
                    if ((status >> 11 & 0x1) == 1)
                    {
                        OutTwo12C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo12C.BackColor = loadsPass;
                    }
                    break;
                case 4096:
                    if ((status >> 13 & 0x1) == 1)
                    {
                        OutTwo13C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo13C.BackColor = loadsPass;
                    }
                    break;
                case 8192:
                    if ((status >> 14 & 0x1) == 1)
                    {
                        OutTwo14C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutTwo14C.BackColor = loadsPass;
                    }
                    break;
            }
        }
        private void LoadTest2B2(int status, Color loadsPass)
        {
            if (status == 0) AutoTest.LoadTest2Passed++;
            if ((status >> 0 & 0x1) == 1) OutTwo1C.BackColor = loadsErrorT2; else OutTwo1C.BackColor = loadsPass;
            if ((status >> 1 & 0x1) == 1) OutTwo2C.BackColor = loadsErrorT2; else OutTwo2C.BackColor = loadsPass;
            if ((status >> 2 & 0x1) == 1) OutTwo3C.BackColor = loadsErrorT2; else OutTwo3C.BackColor = loadsPass;
            if ((status >> 3 & 0x1) == 1) OutTwo4C.BackColor = loadsErrorT2; else OutTwo4C.BackColor = loadsPass;
            if ((status >> 4 & 0x1) == 1) OutTwo5C.BackColor = loadsErrorT2; else OutTwo5C.BackColor = loadsPass;
            if ((status >> 5 & 0x1) == 1) OutTwo6C.BackColor = loadsErrorT2; else OutTwo6C.BackColor = loadsPass;
            if ((status >> 6 & 0x1) == 1) OutTwo7C.BackColor = loadsErrorT2; else OutTwo7C.BackColor = loadsPass;
            if ((status >> 7 & 0x1) == 1) OutTwo8C.BackColor = loadsErrorT2; else OutTwo8C.BackColor = loadsPass;
            if ((status >> 8 & 0x1) == 1) OutTwo9C.BackColor = loadsErrorT2; else OutTwo9C.BackColor = loadsPass;
            if ((status >> 9 & 0x1) == 1) OutTwo10C.BackColor = loadsErrorT2; else OutTwo10C.BackColor = loadsPass;
            if ((status >> 10 & 0x1) == 1) OutTwo11C.BackColor = loadsErrorT2; else OutTwo11C.BackColor = loadsPass;
            if ((status >> 11 & 0x1) == 1) OutTwo12C.BackColor = loadsErrorT2; else OutTwo12C.BackColor = loadsPass;
            if ((status >> 12 & 0x1) == 1) OutTwo13C.BackColor = loadsErrorT2; else OutTwo13C.BackColor = loadsPass;
            if ((status >> 13 & 0x1) == 1) OutTwo14C.BackColor = loadsErrorT2; else OutTwo14C.BackColor = loadsPass;

        }
        //***********************B3***********************************************
        private void LoadTest1B3(int status, int loc, Color loadsPass)
        {
            switch (loc)
            {
                case 1:
                    if ((status >> 0 & 0x1) == 1)
                    {
                        OutThree1C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutThree1C.BackColor = loadsPass;
                    }
                    break;
                case 2:
                    if ((status >> 1 & 0x1) == 1)
                    {
                        OutThree2C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutThree2C.BackColor = loadsPass;
                    }
                    break;
                case 4:
                    if ((status >> 2 & 0x1) == 1)
                    {
                        OutThree3C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutThree3C.BackColor = loadsPass;
                    }
                    break;
                case 8:
                    if ((status >> 3 & 0x1) == 1)
                    {
                        OutThree4C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutThree4C.BackColor = loadsPass;
                    }
                    break;
                case 16:
                    if ((status >> 4 & 0x1) == 1)
                    {
                        OutThree5C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Passed++;
                        OutThree5C.BackColor = loadsPass;
                    }
                    break;
                    //case 32:
                    //    if ((status >> 5 & 0x1) == 1)
                    //    {
                    //        OutThree6C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        OutThree6C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 64:
                    //    if ((status >> 6 & 0x1) == 1)
                    //    {
                    //        OutThree7C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        OutThree7C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 128:
                    //    if ((status >> 7 & 0x1) == 1)
                    //    {
                    //        OutThree5C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        OutThree5C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 256:
                    //    if ((status >> 8 & 0x1) == 1)
                    //    {
                    //        OutThree9C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        OutThree9C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 512:
                    //    if ((status >> 9 & 0x1) == 1)
                    //    {
                    //        OutThree10C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        outThree10C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 1024:
                    //    if ((status >> 10 & 0x1) == 1)
                    //    {
                    //        outThree11C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        outThree11C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 2048:
                    //    if ((status >> 11 & 0x1) == 1)
                    //    {
                    //        outThree12C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        outThree12C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 4096:
                    //    if ((status >> 12 & 0x1) == 1)
                    //    {
                    //        outThree13C.BackColor = loadsError;
                    //    }
                    //    else
                    //    {
                    //        AutoTest.LoadTest1Pass++;
                    //        outThree13C.BackColor = loadsPass;
                    //    }
                    //    break;
                    //case 8192:

                    //    break;
            }
        }
        private void LoadTest2B3(int status, Color loadsPass)
        {
            if (status == 0) AutoTest.LoadTest2Passed++;

            if ((status >> 0 & 0x1) == 1) OutThree1C.BackColor = loadsErrorT2; else OutThree1C.BackColor = loadsPass;
            if ((status >> 1 & 0x1) == 1) OutThree2C.BackColor = loadsErrorT2; else OutThree2C.BackColor = loadsPass;
            if ((status >> 2 & 0x1) == 1) OutThree3C.BackColor = loadsErrorT2; else OutThree3C.BackColor = loadsPass;
            if ((status >> 3 & 0x1) == 1) OutThree4C.BackColor = loadsErrorT2; else OutThree4C.BackColor = loadsPass;
            if ((status >> 4 & 0x1) == 1) OutThree5C.BackColor = loadsErrorT2; else OutThree5C.BackColor = loadsPass;
            //if ((status >> 5 & 0x1) == 1) OutThree6C.BackColor = loadsErrorT2; else OutThree6C.BackColor = loadsPass;
            //if ((status >> 6 & 0x1) == 1) OutThree7C.BackColor = loadsErrorT2; else OutThree7C.BackColor = loadsPass;
            //if ((status >> 7 & 0x1) == 1) OutThree8C.BackColor = loadsErrorT2; else OutThree8C.BackColor = loadsPass;
            //if ((status >> 8 & 0x1) == 1) OutThree9C.BackColor = loadsErrorT2; else OutThree9C.BackColor = loadsPass;
            //if ((status >> 9 & 0x1) == 1) OutThree10C.BackColor = loadsErrorT2; else OutThree10C.BackColor = loadsPass;
            //if ((status >> 10 & 0x1) == 1) OutThree11C.BackColor = loadsErrorT2; else OutThree11C.BackColor = loadsPass;
            //if ((status >> 11 & 0x1) == 1) OutThree12C.BackColor = loadsErrorT2; else OutThree12C.BackColor = loadsPass;
            //if ((status >> 12 & 0x1) == 1) OutThree13C.BackColor = loadsErrorT2; else OutThree13C.BackColor = loadsPass;
            AutoTest.LoadTest2B3Rec = true;
        }
        //##
        public void SetOutputFeedBacksBoard1()
        {
            string current = string.Format("{0:F1} ", outputFeedback.current);
            switch (outputFeedback.outputNumber)
            {
                case 1:

                    if ((outputParam.b0 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne1Mx, thr.outOne1);
                        OutOne1C.Text = current;
                        OutOne1C.BackColor = outputColor;
                        addOutputReport(0, current);

                    }
                    break;
                case 2:
                    if ((outputParam.b1 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne2Mx, thr.outOne2);
                        OutOne2C.Text = current;
                        OutOne2C.BackColor = outputColor;
                        addOutputReport(1, current);

                    }
                    break;
                case 3:
                    if ((outputParam.b2 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne3Mx, thr.outOne3);
                        OutOne3C.Text = current;
                        OutOne3C.BackColor = outputColor;
                        addOutputReport(2, current);

                    }
                    break;
                case 4:
                    if ((outputParam.b3 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne4Mx, thr.outOne4);
                        OutOne4C.Text = current;
                        OutOne4C.BackColor = outputColor;
                        addOutputReport(3, current);

                    }
                    break;
                case 5:
                    if ((outputParam.b4 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne5Mx, thr.outOne5);
                        OutOne5C.Text = current;
                        OutOne5C.BackColor = outputColor;
                        addOutputReport(4, current);

                    }
                    break;
                case 6:
                    if ((outputParam.b5 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne6Mx, thr.outOne6);
                        OutOne6C.Text = current;
                        OutOne6C.BackColor = outputColor;
                        addOutputReport(5, current);

                    }
                    break;
                case 7:
                    if ((outputParam.b6 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne7Mx, thr.outOne7);
                        OutOne7C.Text = current;
                        OutOne7C.BackColor = outputColor;
                        addOutputReport(6, current);

                    }
                    break;
                case 8:
                    if ((outputParam.b7 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne8Mx, thr.outOne8);
                        OutOne8C.Text = current;
                        OutOne8C.BackColor = outputColor;
                        addOutputReport(7, current);
                    }
                    break;
                case 9:
                    if ((outputParam.b8 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne9Mx, thr.outOne9);
                        OutOne9C.Text = current;
                        OutOne9C.BackColor = outputColor;
                        addOutputReport(8, current);
                    }
                    break;
                case 10:
                    if ((outputParam.b9 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne10Mx, thr.outOne10);
                        OutOne10C.Text = current;
                        OutOne10C.BackColor = outputColor;
                        addOutputReport(9, current);
                    }
                    break;
                case 11:
                    if ((outputParam.b10 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne11Mx, thr.outOne11);
                        OutOne11C.Text = current;
                        OutOne11C.BackColor = outputColor;
                        addOutputReport(10, current);
                    }
                    break;
                case 12:
                    if ((outputParam.b11 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne12Mx, thr.outOne12);
                        OutOne12C.Text = current;
                        OutOne12C.BackColor = outputColor;
                        addOutputReport(11, current);
                    }
                    break;
                case 13:
                    if ((outputParam.b12 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne13Mx, thr.outOne13);
                        OutOne13C.Text = current;
                        OutOne13C.BackColor = outputColor;
                        addOutputReport(12, current);
                    }
                    break;
                case 14:
                    if ((outputParam.b13 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne14Mx, thr.outOne14);
                        OutOne14C.Text = current;
                        OutOne14C.BackColor = outputColor;
                        addOutputReport(13, current);

                        if (AutoTest.Output1Failed == 0)
                            AutoTest.Output1Pass = true;
                        else
                        {
                            AutoTest.Output1Pass = false;
                            AutoTest.Output1Failed = 0;
                        }



                    }
                    break;
            }
        }
        public void SetOutputFeedBacksBoard2()
        {
            string current = string.Format("{0:F1} ", outputFeedback.current);
            //TestMode.Auto = true;
            switch (outputFeedback.outputNumber)
            {
                case 1:
                    if ((outputParam.b14 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo1Mx, thr.outTwo1);
                        OutTwo1C.Text = current;
                        OutTwo1C.BackColor = outputColor;
                        addOutputReport(14, current);
                        //addOutputFailReport(outTwo1);
                    }
                    break;
                case 2:
                    if ((outputParam.b15 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo2Mx, thr.outTwo2);
                        OutTwo2C.Text = current;
                        OutTwo2C.BackColor = outputColor;
                        addOutputReport(15, current);
                        //addOutputFailReport(outTwo2);
                    }
                    break;
                case 3:
                    if ((outputParam.b16 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo3Mx, thr.outTwo3);
                        OutTwo3C.Text = current;
                        OutTwo3C.BackColor = outputColor;
                        addOutputReport(16, current);
                    }
                    break;
                case 4:
                    if ((outputParam.b17 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo4Mx, thr.outTwo4);
                        OutTwo4C.Text = current;
                        OutTwo4C.BackColor = outputColor;
                        addOutputReport(17, current);
                    }
                    break;
                case 5:
                    if ((outputParam.b18 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo5Mx, thr.outTwo5);
                        OutTwo5C.Text = current;
                        OutTwo5C.BackColor = outputColor;
                        addOutputReport(18, current);
                    }
                    break;
                case 6:
                    if ((outputParam.b19 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo6Mx, thr.outTwo6);
                        OutTwo6C.Text = current;
                        OutTwo6C.BackColor = outputColor;
                        addOutputReport(19, current);
                    }
                    break;
                case 7:
                    if ((outputParam.b20 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo7Mx, thr.outTwo7);
                        OutTwo7C.Text = current;
                        OutTwo7C.BackColor = outputColor;
                        addOutputReport(20, current);
                    }
                    break;
                case 8:
                    if ((outputParam.b21 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo8Mx, thr.outTwo8);
                        OutTwo8C.Text = current;
                        OutTwo8C.BackColor = outputColor;
                        addOutputReport(21, current);
                    }
                    break;
                case 9:
                    if ((outputParam.b22 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo9Mx, thr.outTwo9);
                        OutTwo9C.Text = current;
                        OutTwo9C.BackColor = outputColor;
                        addOutputReport(22, current);

                    }
                    break;
                case 10:
                    if ((outputParam.b23 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo10Mx, thr.outTwo10);
                        OutTwo10C.Text = current;
                        OutTwo10C.BackColor = outputColor;
                        addOutputReport(23, current);
                    }
                    break;
                case 11:
                    if ((outputParam.b24 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo11Mx, thr.outTwo11);
                        OutTwo11C.Text = current;
                        OutTwo11C.BackColor = outputColor;
                        addOutputReport(24, current);

                    }
                    break;
                case 12:
                    if ((outputParam.b25 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo12Mx, thr.outTwo12);
                        OutTwo12C.Text = current;
                        OutTwo12C.BackColor = outputColor;
                        addOutputReport(25, current);
                    }
                    break;
                case 13:
                    if ((outputParam.b26 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo13Mx, thr.outTwo13);
                        OutTwo13C.Text = current;
                        OutTwo13C.BackColor = outputColor;
                        addOutputReport(26, current);
                    }
                    break;
                case 14:
                    if ((outputParam.b27 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo14Mx, thr.outTwo14);
                        OutTwo14C.Text = current;
                        OutTwo14C.BackColor = outputColor;
                        addOutputReport(27, current);

                        if (AutoTest.Output2Failed == 0)
                        {
                            AutoTest.Output2Pass = true;
                        }
                        else
                        {
                            AutoTest.Output2Pass = false;
                            AutoTest.Output2Failed = 0;
                        }


                    }
                    break;
            }
            //TestMode.Auto = false;
            resetOutputs();
        }
        private void CheckOutput3(float max, float min)
        {
            AutoTest.OutputPass = false;

            if ((outputFeedback.current < max) && (outputFeedback.current > min))
            {
                //AutoTest.Output3Passed++;
                outputColor = testGreen; //green
                AutoTest.OutputPass = true;
            }
            else
            {
                AutoTest.Output3Failed++;
                outputColor = testRed; //red
                AutoTest.OutputPass = false;
            }

        }
        public void SetOutputFeedBacksBoard3()
        {
            string current = string.Format("{0:F1} ", outputFeedback.current);
            //TestMode.Auto = true;
            switch (outputFeedback.outputNumber)
            {
                case 1:
                    if ((outputParam.b28 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree1Mx, thr.outThree1);
                        OutThree1C.Text = current;
                        OutThree1C.BackColor = outputColor;
                        addOutputReport(28, current);
                        //addOutputFailReport(outThree1);
                    }
                    break;
                case 2:
                    if ((outputParam.b29 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree2Mx, thr.outThree2);
                        OutThree2C.Text = current;
                        OutThree2C.BackColor = outputColor;
                        addOutputReport(29, current);
                    }
                    break;
                case 3:
                    if ((outputParam.b30 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree3Mx, thr.outThree3);
                        OutThree3C.Text = current;
                        OutThree3C.BackColor = outputColor;
                        addOutputReport(30, current);
                    }
                    break;
                case 4:
                    if ((outputParam.b31 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree4Mx, thr.outThree4);
                        OutThree4C.Text = current;
                        OutThree4C.BackColor = outputColor;
                        addOutputReport(31, current);
                    }
                    break;
                case 5:
                    if ((outputParam.b32 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree5Mx, thr.outThree5);
                        OutThree5C.Text = current;
                        OutThree5C.BackColor = outputColor;
                        addOutputReport(32, current);

                        if (AutoTest.Output3Failed == 0)
                        {
                            AutoTest.Output3Pass = true;
                        }
                        else
                        {
                            AutoTest.Output3Pass = false;
                            AutoTest.Output3Failed = 0;
                        }

                    }
                    break;
                    //case 6:
                    //    if ((outputParam.b33 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput3(thr.outThree6Mx, thr.outThree6);
                    //        OutThree6C.Text = current;
                    //        OutThree6C.BackColor = outputColor;
                    //        addOutputReport(26, current);
                    //    }
                    //    break;
                    //case 7:
                    //    if ((outputParam.b34 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput3(thr.outThree7Mx, thr.outThree7);
                    //        OutThree7C.Text = current;
                    //        OutThree7C.BackColor = outputColor;
                    //        addOutputReport(27, current);
                    //    }
                    //    break;
                    //case 8:
                    //    if ((outputParam.b35 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput3(thr.outThree8Mx, thr.outThree8);
                    //        OutThree8C.Text = current;
                    //        OutThree8C.BackColor = outputColor;
                    //        addOutputReport(28, current);
                    //    }
                    //    break;

            }
            resetOutputs();

        }
        private void CheckOutput(float max, float min)
        {
            AutoTest.OutputPass = false;

            if ((outputFeedback.current < max) && (outputFeedback.current > min))
            {
                outputColor = testGreen;
                if (TestMode.Auto)
                {
                    //AutoTest.Output1Passed++;
                    AutoTest.OutputPass = true;
                }

            }
            else
            {
                AutoTest.Output1Failed++;
                outputColor = testRed;
                AutoTest.OutputPass = false;
            }



        }
        private void CheckOutput2(float max, float min)
        {
            AutoTest.OutputPass = false;

            if ((outputFeedback.current < max) && (outputFeedback.current > min))
            {
                //AutoTest.Output2Passed++;
                outputColor = testGreen; //green
                AutoTest.OutputPass = true;

            }
            else
            {
                AutoTest.Output2Failed++;
                outputColor = testRed; //red
                AutoTest.OutputPass = false;
            }

            //if (AutoTest.Output2Passed == 11)
            //{
            //    AutoTest.Output2Pass = true;
            //}
        }
        private void SetInputCheck(InputParams data)
        {
            if (TestMode.Auto && TestInfoClass.InputTestStrategy == 1)
            {
                switch (AutoTest.InputRecevd)
                {
                    case 0:
                        SetInputFeedBacksBoard1Part1(data);
                        AutoTest.InputRecevd = 1;
                        resetInputs();
                        break;
                    case 1:
                        SetInputFeedBacksBoard1part2(data); //????
                        AutoTest.InputRecevd = 2;
                        resetInputs();
                        break;
                    case 2:
                        SetInputFeedBacksBoard2Part1(data);
                        AutoTest.InputRecevd = 3;
                        resetInputs2();
                        break;
                    case 3:
                        SetInputFeedBacksBoard2part2(data);
                        AutoTest.InputRecevd = 4;
                        resetInputs2();
                        break;
                }
                AutoTest.InputRec2 = true;
            }
            else if (TestMode.Auto && TestInfoClass.InputTestStrategy == 0)
            {
                switch (AutoTest.InputRecevd)
                {
                    case 0:
                        SetInputFeedBacksBoard1Part1(data);
                        SetInputFeedBacksBoard1part2(data);
                        // AutoTest.InputRecevd = 1;
                        resetInputs();
                        break;

                    case 1:
                        SetInputFeedBacksBoard2Part1(data);
                        SetInputFeedBacksBoard2part2(data);
                        //  AutoTest.InputRecevd = 3;
                        resetInputs2();
                        break;
                }
                AutoTest.InputRec2 = true;
            }
            else                               //  manual
            {
                SetInputFeedBacksBoard1(data);
                SetInputFeedBacksBoard2(data);
                resetInputs();
                resetInputs2();
                AutoTest.InputRec2 = true;
            }
            //else
            //{
            //    SetInputFeedBacksBoards(data);
            //}


        }
        byte recvState = 0;
        //byte fuelT_Res = 0;
        byte FUELTOLERANCE = 5;
        public void SetAnalogInputFeedBacks()
        {
            if (AnalogInput.Value == 254)
                AnalogInput.Value -= 254;
            tbFuel.Text = AnalogInput.Value.ToString();
            FuelResponse.Full_Recv = true;
            aGauge9.Value = (AnalogInput.Value);

            Thread.Sleep(100);

            switch (recvState)
            {
                case 0:
                    if (AnalogInput.Value > (byte)FuelVal.Full - FUELTOLERANCE && AnalogInput.Value < (byte)FuelVal.Full + FUELTOLERANCE)
                    {
                        // fuelT_Res++;
                        AutoTest.AnalogInputOk++;
                    }
                    recvState = 1;
                    FuelTest();
                    break;
                case 1:
                    if (AnalogInput.Value > (byte)FuelVal.ThreeQuarters - FUELTOLERANCE && AnalogInput.Value < (byte)FuelVal.ThreeQuarters + FUELTOLERANCE)
                    {
                        //fuelT_Res++;
                        AutoTest.AnalogInputOk++;

                    }
                    recvState = 2;
                    FuelTest();
                    break;
                case 2:
                    if (AnalogInput.Value > (byte)FuelVal.Half - FUELTOLERANCE && AnalogInput.Value < (byte)FuelVal.Half + FUELTOLERANCE)
                    {
                        // fuelT_Res++;
                        AutoTest.AnalogInputOk++;

                    }
                    recvState = 3;
                    FuelTest();
                    break;
                case 3:
                    if (AnalogInput.Value > (byte)FuelVal.Quarter - FUELTOLERANCE && AnalogInput.Value < (byte)FuelVal.Quarter + FUELTOLERANCE)
                    {
                        //fuelT_Res++;
                        AutoTest.AnalogInputOk++;

                    }
                    recvState = 4;
                    FuelTest();
                    break;
                case 4:
                    if (AnalogInput.Value > (byte)FuelVal.Empty - FUELTOLERANCE && AnalogInput.Value < (byte)FuelVal.Empty + FUELTOLERANCE)
                    {
                        //fuelT_Res++;
                        AutoTest.AnalogInputOk++;

                    }
                    if (AutoTest.AnalogInputOk == 5)
                    {
                        FuelResponse.Full_Result = 1;
                        AutoTest.AnalogInpPass = true;
                    }

                    else
                    {
                        FuelResponse.Full_Result = 0;
                        AutoTest.AnalogInpPass = false;

                    }
                    //  fuelT_Res = 0;
                    AutoTest.AnalogInputOk = 0;
                    recvState = 0;
                    SetInputsFeedback(fuelGauge_led, FuelResponse.Full_Result, In1_An1, 48);//
                    In1_An1.Checked = false;


                    break;
            }

        }


        private void AnalogInputReset()
        {
            AnalogInput.fuelResult = 0;
            fuelGauge_led.On = false;
            In1_An1.Checked = false;
            aGauge9.Value = 0;

        }
        public void SetInputFeedBacksBoards(InputParams data)   //All
        {
            //AutoTest.InputFail = 0;
            //SetInputsFeedback(In1_L1_led, data.b1, In1_L1, 0);      //addInputReport(0, data.b1);
            //SetInputsFeedback(In1_L2_led, data.b2, In1_L2, 1);      //addInputReport(1, data.b2);
            //SetInputsFeedback(In1_L3_led, data.b3, In1_L3, 2);      //addInputReport(2, data.b3);
            //SetInputsFeedback(In1_L4_led, data.b4, In1_L4, 3);      //addInputReport(3, data.b4);
            //SetInputsFeedback(In1_L5_led, data.b5, In1_L5, 4);      //addInputReport(4, data.b5);
            //SetInputsFeedback(In1_L6_led, data.b6, In1_L6, 5);      //addInputReport(5, data.b6);
            //SetInputsFeedback(In1_L7_led, data.b7, In1_L7, 6);      //addInputReport(6, data.b7);
            //SetInputsFeedback(In1_L8_led, data.b8, In1_L8, 7);      //addInputReport(7, data.b8);

            //SetInputsFeedback(In1_L9_led, data.b9, In1_L9, 8);       //addInputReport(8, data.b9);
            //SetInputsFeedback(In1_L10_led, data.b10, In1_L10, 9);     // addInputReport(9, data.b10);
            //SetInputsFeedback(In1_L11_led, data.b11, In1_L11, 10);    // addInputReport(10, data.b11);
            //SetInputsFeedback(In1_L12_led, data.b12, In1_L12, 11);    // addInputReport(11, data.b12);
            //SetInputsFeedback(In1_L13_led, data.b13, In1_L13, 12);    // addInputReport(12, data.b13);
            //SetInputsFeedback(In1_L14_led, data.b14, In1_L14, 13);    // addInputReport(13, data.b14);
            //SetInputsFeedback(In1_L15_led, data.b15, In1_L15, 14);    // addInputReport(14, data.b15);
            //SetInputsFeedback(In1_L16_led, data.b16, In1_L16, 15);    // addInputReport(15, data.b16);

            //SetInputsFeedback(In1_L17_led, data.b17, In1_L17, 16);     //addInputReport(16, data.b17);
            //SetInputsFeedback(In1_L18_led, data.b18, In1_L18, 17);     //addInputReport(17, data.b18);
            //SetInputsFeedback(In1_L19_led, data.b19, In1_L19, 18);     //addInputReport(18, data.b19);
            //SetInputsFeedback(In1_L20_led, data.b20, In1_L20, 19);     //addInputReport(19, data.b20);
            //SetInputsFeedback(In1_L21_led, data.b21, In1_L21, 20);     //addInputReport(20, data.b21);
            //SetInputsFeedback(In1_L22_led, data.b22, In1_L22, 21);     //addInputReport(21, data.b22);
            //SetInputsFeedback(In1_L23_led, data.b23, In1_L23, 22);     //addInputReport(22, data.b23);
            //SetInputsFeedback(In1_L24_led, data.b24, In1_L24, 23);     //addInputReport(23, data.b24);

            //SetInputsFeedback(In1_L25_led, data.b25, In1_L25, 24);      //addInputReport(24, data.b25);
            //SetInputsFeedback(In1_L26_led, data.b26, In1_L26, 25);      //addInputReport(25, data.b26);
            //SetInputsFeedback(In1_L27_led, data.b27, In1_L27, 26);      //addInputReport(26, data.b27);
            //SetInputsFeedback(In1_L28_led, data.b28, In1_L28, 27);      //addInputReport(27, data.b28);
            //SetInputsFeedback(In1_L29_led, data.b29, In1_L29, 28);      //addInputReport(28, data.b29);


            //SetInputsFeedback(In2_L1_led, data.b2_1, In2_L1, 29);        //addInputReport(29, data.b2_1);
            //SetInputsFeedback(In2_L2_led, data.b2_2, In2_L2, 30);        //addInputReport(30, data.b2_2);
            //SetInputsFeedback(In2_L3_led, data.b2_3, In2_L3, 31);        //addInputReport(31, data.b2_3);
            //SetInputsFeedback(In2_L4_led, data.b2_4, In2_L4, 32);        //addInputReport(32, data.b2_4);
            //SetInputsFeedback(In2_L5_led, data.b2_5, In2_L5, 33);        //addInputReport(33, data.b2_5);
            //SetInputsFeedback(In2_L6_led, data.b2_6, In2_L6, 34);        //addInputReport(34, data.b2_6);
            //SetInputsFeedback(In2_L7_led, data.b2_7, In2_L7, 35);        //addInputReport(35, data.b2_7);
            //SetInputsFeedback(In2_L8_led, data.b2_8, In2_L8, 36);        //addInputReport(36, data.b2_8);

            //SetInputsFeedback(In2_L9_led, data.b2_9, In2_L9, 37);           //addInputReport(37, data.b2_9);
            //SetInputsFeedback(In2_L10_led, data.b2_10, In2_L10, 38);        //addInputReport(38, data.b2_10);
            //SetInputsFeedback(In2_L11_led, data.b2_11, In2_L11, 39);        //addInputReport(39, data.b2_11);

            //SetInputsFeedbackReverse(In2_H1_led, data.b2_31, In2_H1, 40);      //addInputReportReverse(40, data.b2_31);
            //SetInputsFeedbackReverse(In2_H5_led, data.b2_35, In2_H5, 41);      //addInputReportReverse(41, data.b2_35);
            //SetInputsFeedbackReverse(In2_H2_led, data.b2_32, In2_H2, 42);      //addInputReportReverse(42, data.b2_32);
            //SetInputsFeedbackReverse(In2_H3_led, data.b2_33, In2_H3, 43);      //addInputReportReverse(43, data.b2_33);
            //SetInputsFeedbackReverse(In2_H4_led, data.b2_34, In2_H4, 44);      //addInputReportReverse(44, data.b2_34);

            //SetInputsFeedback(In2_L12_led, data.b2_12, In2_L12, 45);      //addInputReport(45, data.b2_12);
            //SetInputsFeedback(In2_L13_led, data.b2_13, In2_L13, 46);      //addInputReport(46, data.b2_13);
            //SetInputsFeedback(In2_L14_led, data.b2_14, In2_L14, 47);      //addInputReport(47, data.b2_14);


            //if (AutoTest.InputFail == 0)
            //{
            //    AutoTest.InputPass = true;
            //}
        }
        public void SetInputFeedBacksBoard1(InputParams data)
        {
            SetInputsFeedback(In1_L1_led, data.b1, In1_L1, 0);
            SetInputsFeedback(In1_L2_led, data.b2, In1_L2, 1);
            SetInputsFeedback(In1_L3_led, data.b3, In1_L3, 2);
            SetInputsFeedback(In1_L4_led, data.b4, In1_L4, 3);
            SetInputsFeedback(In1_L5_led, data.b5, In1_L5, 4);
            SetInputsFeedback(In1_L6_led, data.b6, In1_L6, 5);
            SetInputsFeedback(In1_L7_led, data.b7, In1_L7, 6);
            SetInputsFeedback(In1_L8_led, data.b8, In1_L8, 7);

            SetInputsFeedback(In1_L9_led, data.b9, In1_L9, 8);
            SetInputsFeedback(In1_L10_led, data.b10, In1_L10, 9);
            SetInputsFeedback(In1_L11_led, data.b11, In1_L11, 10);
            SetInputsFeedback(In1_L12_led, data.b12, In1_L12, 11);
            SetInputsFeedback(In1_L13_led, data.b13, In1_L13, 12);
            SetInputsFeedback(In1_L14_led, data.b14, In1_L14, 13);
            SetInputsFeedback(In1_L15_led, data.b15, In1_L15, 14);
            SetInputsFeedback(In1_L16_led, data.b16, In1_L16, 15);

            SetInputsFeedback(In1_L17_led, data.b17, In1_L17, 16);
            SetInputsFeedback(In1_L18_led, data.b18, In1_L18, 17);
            SetInputsFeedback(In1_L19_led, data.b19, In1_L19, 18);
            SetInputsFeedback(In1_L20_led, data.b20, In1_L20, 19);
            SetInputsFeedback(In1_L21_led, data.b21, In1_L21, 20);
            SetInputsFeedback(In1_L22_led, data.b22, In1_L22, 21);
            SetInputsFeedback(In1_L23_led, data.b23, In1_L23, 22);
            SetInputsFeedback(In1_L24_led, data.b24, In1_L24, 23);

            SetInputsFeedback(In1_L25_led, data.b25, In1_L25, 24);
            SetInputsFeedback(In1_L26_led, data.b26, In1_L26, 25);
            SetInputsFeedback(In1_L27_led, data.b27, In1_L27, 26);
            SetInputsFeedback(In1_L28_led, data.b28, In1_L28, 27);
            SetInputsFeedback(In1_L29_led, data.b29, In1_L29, 28);

            if (AutoTest.InputFail == 0)
            {
                AutoTest.InputPass = true;
            }
            else
                AutoTest.InputPass = false;
        }
        public void SetInputFeedBacksBoard1Part1(InputParams data)
        {
            SetInputsFeedback(In1_L1_led, data.b1, In1_L1, 0);
            SetInputsFeedback(In1_L4_led, data.b4, In1_L4, 3);
            SetInputsFeedback(In1_L5_led, data.b5, In1_L5, 4);
            SetInputsFeedback(In1_L8_led, data.b8, In1_L8, 7);
            SetInputsFeedback(In1_L9_led, data.b9, In1_L9, 8);
            SetInputsFeedback(In1_L12_led, data.b12, In1_L12, 11);
            SetInputsFeedback(In1_L13_led, data.b13, In1_L13, 12);
            SetInputsFeedback(In1_L16_led, data.b16, In1_L16, 15);
            SetInputsFeedback(In1_L17_led, data.b17, In1_L17, 16);
            SetInputsFeedback(In1_L20_led, data.b20, In1_L20, 19);
            SetInputsFeedback(In1_L21_led, data.b21, In1_L21, 20);
            SetInputsFeedback(In1_L22_led, data.b22, In1_L22, 21);
            SetInputsFeedback(In1_L26_led, data.b26, In1_L26, 25);
            SetInputsFeedback(In1_L27_led, data.b27, In1_L27, 26);


            if (AutoTest.InputFail == 0)
            {
                AutoTest.InputPass = true;
            }
            else
                AutoTest.InputPass = false;
        }
        public void SetInputFeedBacksBoard1part2(InputParams data)
        {
            int input1Fail = AutoTest.InputFail;

            SetInputsFeedback(In1_L2_led, data.b2, In1_L2, 1);
            SetInputsFeedback(In1_L3_led, data.b3, In1_L3, 2);
            SetInputsFeedback(In1_L6_led, data.b6, In1_L6, 5);
            SetInputsFeedback(In1_L7_led, data.b7, In1_L7, 6);
            SetInputsFeedback(In1_L10_led, data.b10, In1_L10, 9);
            SetInputsFeedback(In1_L11_led, data.b11, In1_L11, 10);
            SetInputsFeedback(In1_L14_led, data.b14, In1_L14, 13);
            SetInputsFeedback(In1_L15_led, data.b15, In1_L15, 14);
            SetInputsFeedback(In1_L18_led, data.b18, In1_L18, 17);
            SetInputsFeedback(In1_L19_led, data.b19, In1_L19, 18);
            SetInputsFeedback(In1_L23_led, data.b23, In1_L23, 22);
            SetInputsFeedback(In1_L24_led, data.b24, In1_L24, 23);
            SetInputsFeedback(In1_L25_led, data.b25, In1_L25, 24);
            SetInputsFeedback(In1_L28_led, data.b28, In1_L28, 27);
            SetInputsFeedback(In1_L29_led, data.b29, In1_L29, 28);

            if ((AutoTest.InputFail == 0) && (input1Fail == 0))
            {
                AutoTest.InputPass = true;
            }
            else
            {
                AutoTest.InputPass = false;
            }
        }
        public void SetInputFeedBacksBoard2(InputParams data)
        {
            AutoTest.InputFail = 0;

            SetInputsFeedback(In2_L1_led, data.b2_1, In2_L1, 29);
            SetInputsFeedback(In2_L2_led, data.b2_2, In2_L2, 30);
            SetInputsFeedback(In2_L3_led, data.b2_3, In2_L3, 31);
            SetInputsFeedback(In2_L4_led, data.b2_4, In2_L4, 32);
            SetInputsFeedback(In2_L5_led, data.b2_5, In2_L5, 33);
            SetInputsFeedback(In2_L6_led, data.b2_6, In2_L6, 34);
            SetInputsFeedback(In2_L7_led, data.b2_7, In2_L7, 35);
            SetInputsFeedback(In2_L8_led, data.b2_8, In2_L8, 36);
            SetInputsFeedback(In2_L9_led, data.b2_9, In2_L9, 37);

            SetInputsFeedback(In2_L10_led, data.b2_10, In2_L10, 38);
            SetInputsFeedback(In2_L11_led, data.b2_11, In2_L11, 39);

            SetInputsFeedbackReverse(In2_H1_led, data.b2_31, In2_H1, 40); //??? CAS
            SetInputsFeedbackReverse(In2_H5_led, data.b2_35, In2_H5, 41);
            SetInputsFeedbackReverse(In2_H2_led, data.b2_32, In2_H2, 42);
            SetInputsFeedbackReverse(In2_H3_led, data.b2_33, In2_H3, 43);
            SetInputsFeedbackReverse(In2_H4_led, data.b2_34, In2_H4, 44);

            SetInputsFeedback(In2_L12_led, data.b2_12, In2_L12, 45);
            SetInputsFeedback(In2_L13_led, data.b2_13, In2_L13, 46);
            SetInputsFeedback(In2_L14_led, data.b2_14, In2_L14, 47);



            if (AutoTest.InputFail == 0)
            {
                AutoTest.Input2Pass = true;
            }
            else
                AutoTest.Input2Pass = false;
        }
        public void SetInputFeedBacksBoard2Part1(InputParams data)
        {
            AutoTest.InputFail = 0;

            SetInputsFeedback(In2_L1_led, data.b2_1, In2_L1, 29);
            SetInputsFeedback(In2_L4_led, data.b2_4, In2_L4, 32);
            SetInputsFeedback(In2_L5_led, data.b2_5, In2_L5, 33);
            SetInputsFeedback(In2_L8_led, data.b2_8, In2_L8, 36);
            SetInputsFeedback(In2_L9_led, data.b2_9, In2_L9, 37);


            SetInputsFeedbackReverse(In2_H1_led, data.b2_31, In2_H1, 40);
            SetInputsFeedbackReverse(In2_H5_led, data.b2_35, In2_H5, 41);
            SetInputsFeedbackReverse(In2_H2_led, data.b2_32, In2_H2, 42);


            SetInputsFeedback(In2_L12_led, data.b2_12, In2_L12, 45);
            SetInputsFeedback(In2_L13_led, data.b2_13, In2_L13, 46);

            if (AutoTest.InputFail == 0)
            {
                AutoTest.Input2Pass = true;
            }
            else
                AutoTest.Input2Pass = false;
        }
        public void SetInputFeedBacksBoard2part2(InputParams data)
        {
            int input2Fail = AutoTest.InputFail;

            SetInputsFeedback(In2_L2_led, data.b2_2, In2_L2, 30);
            SetInputsFeedback(In2_L3_led, data.b2_3, In2_L3, 31);
            SetInputsFeedback(In2_L6_led, data.b2_6, In2_L6, 34);
            SetInputsFeedback(In2_L7_led, data.b2_7, In2_L7, 35);
            SetInputsFeedback(In2_L10_led, data.b2_10, In2_L10, 38);
            SetInputsFeedback(In2_L11_led, data.b2_11, In2_L11, 39);

            //SetInputsFeedbackReverse(In2_H1_led, data.b2_31, In2_H1, 40);
            //SetInputsFeedbackReverse(In2_H5_led, data.b2_35, In2_H5, 41);
            //SetInputsFeedbackReverse(In2_H2_led, data.b2_32, In2_H2, 42);
            SetInputsFeedbackReverse(In2_H3_led, data.b2_33, In2_H3, 43);
            SetInputsFeedbackReverse(In2_H4_led, data.b2_34, In2_H4, 44);


            SetInputsFeedback(In2_L14_led, data.b2_14, In2_L14, 47);


            if ((AutoTest.InputFail == 0) && (input2Fail == 0))
            {
                AutoTest.Input2Pass = true;
            }
            else
                AutoTest.Input2Pass = false;
        }
        LedBulb cselect;
        private void SetInputsFeedback(object sender, byte val, CheckBox c, int j)//, EventArgs e
        {
            if (val == 0)
            {

                if (c.Checked)
                {
                    ((LedBulb)sender).Color = Color.Red;
                    ((LedBulb)sender).On = true;
                    AutoTest.InputFail++;
                    inputReport[j] = 0;
                    AutoTest.InputMsg += c.Name.ToString() + " Failed!";
                }

            }
            else
            {

                if (c.Checked)
                {
                    //if (((LedBulb)sender).Color != Color.Magenta)
                    ((LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
                    ((LedBulb)sender).On = true;
                    inputReport[j] = 1;

                }
                else
                {
                    ((LedBulb)sender).Color = Color.Magenta;
                    ((LedBulb)sender).On = true;
                    AutoTest.InputFail++;
                    inputReport[j] = 0;

                    AutoTest.InputShort = c.Name.ToString();// + ":" + c.Text;
                    if (AutoTest.InputCheckd != null)
                        AutoTest.InputMsg += "Short Circuit ! " + AutoTest.InputCheckd + " Conected to " + AutoTest.InputShort + "!  ";
                    if (cselect != null)
                        ledBulb_SetColor(cselect, Color.Magenta);
                }
            }


        }

        private void SetInputsFeedbackReverse(object sender, byte val, CheckBox c, int j)//, EventArgs e
        {
            if (val == 1)
            {

                if (c.Checked)
                {
                    ((LedBulb)sender).Color = Color.Red;
                    ((LedBulb)sender).On = true;
                    AutoTest.InputFail++;
                    inputReport[j] = 0;
                    AutoTest.InputMsg += c.Name.ToString() + " Failed!";
                }
                else
                {

                }

            }
            else
            {

                if (c.Checked)
                {
                    //if (((LedBulb)sender).Color != Color.Magenta)
                    ((LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
                    ((LedBulb)sender).On = true;
                    inputReport[j] = 1;
                }
                else
                {
                    ((LedBulb)sender).Color = Color.Magenta;
                    ((LedBulb)sender).On = true;
                    AutoTest.InputFail++;
                    inputReport[j] = 0;
                    AutoTest.InputShort = c.Name.ToString();// + ":" + c.Text;
                    if (AutoTest.InputCheckd != null)
                        AutoTest.InputMsg += "Short Circuit !:" + AutoTest.InputCheckd + " Conected to " + AutoTest.InputShort + "!  ";
                    if (cselect != null)
                        ledBulb_SetColor(cselect, Color.Magenta);
                }
            }


        }
        private void SetInputsFeedback2(object sender, byte val)//, EventArgs e
        {
            if (val == 0)
            {
                ((MainProject.LedBulb)sender).Color = Color.Red;
                AutoTest.InputFail++;
            }
            else
            {
                ((MainProject.LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
          ((MainProject.LedBulb)sender).On = true;

        }

        private void SetInputsFeedbackReverse2(object sender, byte val)
        {
            if (val == 1)
            {
                ((MainProject.LedBulb)sender).Color = Color.Red;
                AutoTest.InputFail++;
            }
            else
            {
                ((MainProject.LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
            ((MainProject.LedBulb)sender).On = true;

        }
        private void SetDigitalOutputsFeedback(object sender, byte val)
        {
            if (val == 0)
            {
                ((MainProject.LedBulb)sender).Color = Color.Red;
                AutoTest.DigitalOutputFail++;
            }
            else
            {
                ((MainProject.LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
         ((MainProject.LedBulb)sender).On = true;

        }
        private void addInputReport(int i, int val)
        {
            if (val == 1)
                inputReport[i] = 1;
            else if (val == 0)
                inputReport[i] = 0;
        }
        private void addInputReportReverse(int i, int val)
        {
            if (val == 0)
                inputReport[i] = 1;
            else if (val == 1)
                inputReport[i] = 0;
        }
        private void addOutputReport(int i, string current)
        {
            outputCurReport[i] = current;

            if (AutoTest.OutputPass == false)

                outputReport[i] = 0;
            else
                outputReport[i] = 1;
        }
        private void SetDigitalOutputCheck(OutPutParams data)

        {

            if (TestMode.Auto)
            {
                SetDigitalOutputFeedBacksBoardAuto();
            }
            else
            {
                SetDigitalOutputFeedBacksBoard();
                //SetDigitalOutputFeedBacksBoardManual();
            }

        }
        int digitalOutputStatus = 0;
        public void SetDigitalOutputFeedBacksBoardAuto()  //Check Not Correct
        {
            switch (digitalOutputStatus)
            {
                case 0:
                    SetDigitalOutputsFeedback(OutDL1_led, digitalOutputFeedback.db0); addDigitalOutputReport(33, digitalOutputFeedback.db0);
                    digitalOutputStatus++;
                    break;
                case 1:
                    SetDigitalOutputsFeedback(OutDL2_led, digitalOutputFeedback.db1); addDigitalOutputReport(34, digitalOutputFeedback.db1);
                    digitalOutputStatus++;
                    break;
                case 2:
                    SetDigitalOutputsFeedback(OutDL3_led, digitalOutputFeedback.db2); addDigitalOutputReport(35, digitalOutputFeedback.db2);
                    digitalOutputStatus++;
                    break;
                case 3:
                    SetDigitalOutputsFeedback(OutDL4_led, digitalOutputFeedback.db3); addDigitalOutputReport(36, digitalOutputFeedback.db3);
                    digitalOutputStatus++;
                    break;
                case 4:
                    SetDigitalOutputsFeedback(OutDH1_led, digitalOutputFeedback.db4); addDigitalOutputReport(37, digitalOutputFeedback.db4);
                    digitalOutputStatus++;
                    break;
                case 5:
                    SetDigitalOutputsFeedback(OutDH2_led, digitalOutputFeedback.db5); addDigitalOutputReport(38, digitalOutputFeedback.db5);
                    digitalOutputStatus++;
                    break;
                case 6:
                    SetDigitalOutputsFeedback(OutDH3_led, digitalOutputFeedback.db6); addDigitalOutputReport(39, digitalOutputFeedback.db6);
                    digitalOutputStatus++;
                    break;
                case 7:
                    SetDigitalOutputsFeedback(OutDH4_led, digitalOutputFeedback.db7); addDigitalOutputReport(40, digitalOutputFeedback.db7);
                    digitalOutputStatus++;
                    break;
                case 8:
                    SetDigitalOutputsFeedback(OutDH5_led, digitalOutputFeedback.db8); addDigitalOutputReport(41, digitalOutputFeedback.db8);
                    digitalOutputStatus++;
                    break;
                case 9:
                    SetDigitalOutputsFeedback(OutDH6_led, digitalOutputFeedback.db9); addDigitalOutputReport(42, digitalOutputFeedback.db9);
                    digitalOutputStatus++;

                    break;
                case 10:
                    SetDigitalOutputsFeedback(OutDH7_led, digitalOutputFeedback.db10); addDigitalOutputReport(43, digitalOutputFeedback.db10);
                    digitalOutputStatus++;

                    break;
                case 11:
                    SetDigitalOutputsFeedback(OutDH8_led, digitalOutputFeedback.db11); addDigitalOutputReport(44, digitalOutputFeedback.db11);
                    digitalOutputStatus++;
                    break;
                case 12:
                    SetDigitalOutputsFeedback(OutDH9_led, digitalOutputFeedback.db12); addDigitalOutputReport(45, digitalOutputFeedback.db12);
                    digitalOutputStatus++;
                    break;
                case 13:
                    SetDigitalOutputsFeedback(OutDH10_led, digitalOutputFeedback.db13); addDigitalOutputReport(46, digitalOutputFeedback.db13);
                    digitalOutputStatus = 0;
                    if (AutoTest.DigitalOutputFail == 0)
                        AutoTest.OutputDPass = true;
                    break;

            }

        }
        private void addDigitalOutputReport(int i, int val)
        {
            outputReport[i] = (byte)val;
        }
        public void SetDigitalOutputFeedBacksBoard()
        {
            SetDigitalOutputsFeedbackManual(OutDL1_led, digitalOutputFeedback.db0);    //addDigitalOutputReport(38, digitalOutputFeedback.db0);
            SetDigitalOutputsFeedbackManual(OutDL2_led, digitalOutputFeedback.db1);    // addDigitalOutputReport(39, digitalOutputFeedback.db1);
            SetDigitalOutputsFeedbackManual(OutDL3_led, digitalOutputFeedback.db2);    // addDigitalOutputReport(40, digitalOutputFeedback.db2);
            SetDigitalOutputsFeedbackManual(OutDL4_led, digitalOutputFeedback.db3);   //addDigitalOutputReport(41, digitalOutputFeedback.db15);
            SetDigitalOutputsFeedbackManual(OutDH1_led, digitalOutputFeedback.db4);   //addDigitalOutputReport(42, digitalOutputFeedback.db16);
            SetDigitalOutputsFeedbackManual(OutDH2_led, digitalOutputFeedback.db5);   //addDigitalOutputReport(43, digitalOutputFeedback.db17);
            SetDigitalOutputsFeedbackManual(OutDH3_led, digitalOutputFeedback.db6);   //addDigitalOutputReport(44, digitalOutputFeedback.db18);

            SetDigitalOutputsFeedbackManual(OutDH4_led, digitalOutputFeedback.db7);   //addDigitalOutputReport(45, digitalOutputFeedback.db19);
            SetDigitalOutputsFeedbackManual(OutDH5_led, digitalOutputFeedback.db8);   //addDigitalOutputReport(46, digitalOutputFeedback.db20);
            SetDigitalOutputsFeedbackManual(OutDH6_led, digitalOutputFeedback.db9);   //addDigitalOutputReport(47, digitalOutputFeedback.db21);
            SetDigitalOutputsFeedbackManual(OutDH7_led, digitalOutputFeedback.db10);   //addDigitalOutputReport(48, digitalOutputFeedback.db22);
            SetDigitalOutputsFeedbackManual(OutDH8_led, digitalOutputFeedback.db11);
            SetDigitalOutputsFeedbackManual(OutDH9_led, digitalOutputFeedback.db12);
            SetDigitalOutputsFeedbackManual(OutDH10_led, digitalOutputFeedback.db13);
            resetOutputs();

        }
        private void SetDigitalOutputsFeedbackManual(object sender, byte val)//, EventArgs e
        {
            if (val == 0)
            {
                // ((LedBulb)sender).Color = Color.Red;
                // AutoTest.DigitalOutputFail++;
                ((MainProject.LedBulb)sender).On = false;
            }
            else
            {
                ((MainProject.LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
                ((MainProject.LedBulb)sender).On = true;
            }
        }
        public void SetNetFeedBacksCan()
        {
            if (netParamsFeedback.Mode == 1)
            {
                if (netParams.BCM_CANHS > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataCANH[i + 1] != netParamsFeedback.DataCANH[i])
                        {
                            netParamsFeedback.CanHFail++;
                        }

                    if (netParamsFeedback.CanHFail == 0)
                    {
                        ledBulb_Click(BCM_CANHS_led, 1);
                        //BCM_CANHS.Checked = false;
                        AutoTest.NetworkPassed++;
                        AutoTest.CanHRec = 1;
                        netReport[0] = 1;

                    }
                    else
                    {
                        ledBulb_Click(BCM_CANHS_led, 0);
                        netReport[0] = 0;
                        AutoTest.CanHRec = 2;


                    }
                }

            }
            if (netParamsFeedback.Mode == 2)
            {
                if (netParams.BCM_CANLS > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataCANL[i + 1] != netParamsFeedback.DataCANL[i])
                        {
                            netParamsFeedback.CanLFail++;
                        }
                    if (netParamsFeedback.CanLFail == 0)
                    {
                        ledBulb_Click(BCM_CANLS_led, 1);
                        AutoTest.NetworkPassed++;
                        netReport[1] = 1;
                    }
                    else
                    {
                        ledBulb_Click(BCM_CANLS_led, 0);
                        netReport[1] = 0;


                    }
                }

            }
            //
            if (netParamsFeedback.Mode == 3)
            {
                if (netParams.CAS_CANLS > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataCASCANL[i + 1] != netParamsFeedback.DataCASCANL[i])
                        {
                            netParamsFeedback.CanLFail++;
                        }
                    if (netParamsFeedback.CanLFail == 0)
                    {
                        ledBulb_Click(CAS_CANLS_led, 1);
                        AutoTest.NetworkPassed++;
                        netReport[4] = 1;
                    }
                    else
                    {
                        ledBulb_Click(BCM_CANLS_led, 0);
                        netReport[4] = 0;


                    }
                }

            }
            netParamsReset();
        }
        public void SetNetFeedBacksLin()
        {
            if (netParamsFeedback.Mode == 1)
            {
                if (netParams.BCM_LinFront > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataLinF[i + 1] != netParamsFeedback.DataLinF[i])
                        {
                            netParamsFeedback.LinFFail++;
                        }


                    if (netParamsFeedback.LinFFail == 0)
                    {
                        ledBulb_Click(BCM_LINF_led, 1);
                        AutoTest.NetworkPassed++;
                        netReport[2] = 1;

                    }

                    else
                    {
                        ledBulb_Click(BCM_LINF_led, 0);
                        netReport[2] = 0;

                    }
                }
            }
            if (netParamsFeedback.Mode == 2)
            {
                if (netParams.BCM_LinRear > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataLinR[i + 1] != netParamsFeedback.DataLinR[i])
                        {
                            netParamsFeedback.LinRFail++;
                        }
                    if (netParamsFeedback.LinRFail == 0)
                    {
                        ledBulb_Click(BCM_LINR_led, 1);
                        AutoTest.NetworkPassed++;
                        netReport[3] = 1;

                    }
                    else
                    {
                        ledBulb_Click(BCM_LINR_led, 0);
                        netReport[3] = 0;

                    }
                    AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false;
                }
            }
            netParamsReset();
        }
        #region reset
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetAll();
        }
        private void ResetAll()
        {
            AutoTest.ResetResult();
            AutoTest.CrashUnlockRec = false;
            resetInputs();
            resetInputs2();
            resetCASInputs();
            AnalogInputReset();
            resetInputsFeedback();
            resetOutputs();
            resetOutputs2();
            resetOutputsFeedback();
            netParamsReset();
            netParamsResetFeedback();
            // powerReset();
            autoTest_timer.Stop();
            autoTest_timer2.Stop();
            reportListsReset();

            powerParamsFeedback = null;


            BCM_SoftwareVr.Text = "";
            TestInfoClass.BCM_SoftwareVr_Rec = "";
            bcmVersionReceived = false;
            casVersionReceived = false;
            feedbackTb.Text = "";
            AutoTest.InputMsg = "";
            testTime.Text = "0";
        }
        private void PannelLock(bool state)
        {
            if (state)
            {
                //panelSetting.Enabled = false;
                //panelAutoTest.Enabled = false;
                groupBoxSetting.Enabled = false;
                groupBoxIn1.Enabled = false;
                groupBoxIn2.Enabled = false;
                groupBoxIn3.Enabled = false;
                groupBoxIn4.Enabled = false;
                groupBoxIn5.Enabled = false;
                groupBoxIn6.Enabled = false;
                groupBoxOut1.Enabled = false;
                groupBoxOut2.Enabled = false;
                groupBoxOut3.Enabled = false;
                groupBoxOut4.Enabled = false;
                groupBoxOut5.Enabled = false;
                groupBoxNet1.Enabled = false;
                groupBoxNet2.Enabled = false;

                //outDL8label.ForeColor = Color.Gray;

            }
            else
            {
                //panelSetting.Enabled = true;
                //panelAutoTest.Enabled = true;
                groupBoxSetting.Enabled = true;
                groupBoxIn1.Enabled = true;
                groupBoxIn2.Enabled = true;
                groupBoxIn3.Enabled = true;
                groupBoxIn4.Enabled = true;
                groupBoxIn5.Enabled = true;
                groupBoxIn6.Enabled = true;
                groupBoxOut1.Enabled = true;
                groupBoxOut2.Enabled = true;
                groupBoxOut3.Enabled = true;
                groupBoxOut4.Enabled = true;
                groupBoxOut5.Enabled = true;
                groupBoxNet1.Enabled = true;
                groupBoxNet2.Enabled = true;
                // outDL8label.ForeColor = Color.Black;
            }
        }
        private void netParamsReset()
        {
            AutoTest.NetworkRec = false;
            BCM_CANHS.Checked = false;
            BCM_CANLS.Checked = false;
            BCM_LINF.Checked = false;
            BCM_LINR.Checked = false;
            CAS_CANLS.Checked = false;
            ImmoTest.Checked = false;
            KickTest.Checked = false;
            TPMSTest.Checked = false;
            RKETest.Checked = false;
            LFTest.Checked = false;
            RDHSLock1.Checked = false;
            RDHSUnlock1.Checked = false;
            LDHSLock1.Checked = false;
            LDHSUnlock1.Checked = false;
        }

        private void netParamsResetFeedback()
        {
            ledBulb_Off(BCM_CANHS_led);
            ledBulb_Off(BCM_CANLS_led);
            ledBulb_Off(BCM_LINF_led);
            ledBulb_Off(BCM_LINR_led);
            ledBulb_Off(CAS_CANLS_led);
            ledBulb_Off(ImmoTest_led);
            ledBulb_Off(KickTest_Led);
            ledBulb_Off(TPMSTest_led);
            ledBulb_Off(RKETest_led);
            ledBulb_Off(LFTest_led);

            ledBulb_Off(RDHSLock_led);
            ledBulb_Off(RDHSUnlock_led);
            ledBulb_Off(LDHSLock_led);
            ledBulb_Off(LDHSUnlock_led);
        }
        private void resetOutputs()
        {
            AutoTest.Output1Rec = false;
            AutoTest.Output2Rec = false;

            outputParam.OutputList1.Clear();
            outputParam.OutputList2.Clear();

            OutOne1.Checked = false;
            OutOne2.Checked = false;
            OutOne3.Checked = false;
            OutOne4.Checked = false;
            OutOne5.Checked = false;
            OutOne6.Checked = false;
            OutOne7.Checked = false;
            OutOne8.Checked = false;
            OutOne9.Checked = false;
            OutOne10.Checked = false;
            OutOne11.Checked = false;
            OutOne12.Checked = false;
            OutOne13.Checked = false;
            OutOne14.Checked = false;


            //B2
            OutTwo1.Checked = false;
            OutTwo2.Checked = false;
            OutTwo3.Checked = false;
            OutTwo4.Checked = false;
            OutTwo5.Checked = false;
            OutTwo6.Checked = false;
            OutTwo7.Checked = false;
            OutTwo8.Checked = false;
            OutTwo9.Checked = false;
            OutTwo10.Checked = false;
            OutTwo11.Checked = false;
            OutTwo12.Checked = false;
            OutTwo13.Checked = false;
            OutTwo14.Checked = false;

        }

        private void resetOutputs2()
        {
            AutoTest.Output3Rec = false;
            AutoTest.OutputDRec = false;

            outputParam.OutputList3.Clear();
            outputParam.DigitalOutputList.Clear();

            //B3
            OutThree1.Checked = false;
            OutThree2.Checked = false;
            OutThree3.Checked = false;
            OutThree4.Checked = false;
            OutThree5.Checked = false;
            //OutThree6.Checked = false;
            //OutThree7.Checked = false;
            //OutThree8.Checked = false;
            //outThree9.Checked = false;
            //outThree10.Checked = false;
            //outThree11.Checked = false;
            //outThree12.Checked = false;
            //outThree13.Checked = false;
            //DigitalOutput
            OutDL1.Checked = false;
            OutDL2.Checked = false;
            OutDL3.Checked = false;
            OutDL4.Checked = false;

            OutDH1.Checked = false;
            OutDH2.Checked = false;
            OutDH3.Checked = false;
            OutDH4.Checked = false;
            OutDH5.Checked = false;
            OutDH6.Checked = false;
            OutDH7.Checked = false;
            OutDH8.Checked = false;
            OutDH9.Checked = false;
            OutDH10.Checked = false;



        }
        private void ledBulb_Off(object sender)//, EventArgs e
        {
            ((MainProject.LedBulb)sender).On = false;
        }
        private void resetInputsFeedback()
        {
            ledBulb_Off(In1_L1_led);
            ledBulb_Off(In1_L2_led);
            ledBulb_Off(In1_L3_led);
            ledBulb_Off(In1_L4_led);
            ledBulb_Off(In1_L5_led);
            ledBulb_Off(In1_L6_led);
            ledBulb_Off(In1_L7_led);
            ledBulb_Off(In1_L8_led);
            ledBulb_Off(In1_L9_led);
            ledBulb_Off(In1_L10_led);
            ledBulb_Off(In1_L11_led);
            ledBulb_Off(In1_L12_led);
            ledBulb_Off(In1_L13_led);
            ledBulb_Off(In1_L14_led);
            ledBulb_Off(In1_L15_led);
            ledBulb_Off(In1_L16_led);
            ledBulb_Off(In1_L17_led);
            ledBulb_Off(In1_L18_led);
            ledBulb_Off(In1_L19_led);
            ledBulb_Off(In1_L20_led);
            ledBulb_Off(In1_L21_led);
            ledBulb_Off(In1_L22_led);
            ledBulb_Off(In1_L23_led);
            ledBulb_Off(In1_L24_led);
            ledBulb_Off(In1_L25_led);
            ledBulb_Off(In1_L26_led);
            ledBulb_Off(In1_L27_led);
            ledBulb_Off(In1_L28_led);
            ledBulb_Off(In1_L29_led);

            ledBulb_Off(In2_L1_led);
            ledBulb_Off(In2_L2_led);
            ledBulb_Off(In2_L3_led);
            ledBulb_Off(In2_L4_led);
            ledBulb_Off(In2_L5_led);
            ledBulb_Off(In2_L6_led);
            ledBulb_Off(In2_L7_led);
            ledBulb_Off(In2_L8_led);
            ledBulb_Off(In2_L9_led);
            ledBulb_Off(In2_L10_led);
            ledBulb_Off(In2_L11_led);
            ledBulb_Off(In2_L12_led);
            ledBulb_Off(In2_L13_led);
            ledBulb_Off(In2_L14_led);
            ledBulb_Off(In2_H1_led);
            ledBulb_Off(In2_H2_led);
            ledBulb_Off(In2_H3_led);
            ledBulb_Off(In2_H4_led);
            ledBulb_Off(In2_H5_led);
        }
        private void resetOutputsFeedback()
        {
            resetOutputsFeedbackBoardD();
            resetOutputsFeedbackBoard1();
            resetOutputsFeedbackBoard2();
            resetOutputsFeedbackBoard3();
        }
        Color colr = Color.WhiteSmoke;
        private void resetOutputsFeedbackBoard1()
        {
            OutOne1C.BackColor = colr;
            OutOne2C.BackColor = colr;
            OutOne3C.BackColor = colr;
            OutOne4C.BackColor = colr;
            OutOne5C.BackColor = colr;
            OutOne6C.BackColor = colr;
            OutOne7C.BackColor = colr;
            OutOne8C.BackColor = colr;
            OutOne9C.BackColor = colr;
            OutOne10C.BackColor = colr;
            OutOne11C.BackColor = colr;
            OutOne12C.BackColor = colr;
            OutOne13C.BackColor = colr;
            OutOne14C.BackColor = colr;

            OutOne1C.Text = 0.ToString();
            OutOne2C.Text = 0.ToString();
            OutOne3C.Text = 0.ToString();
            OutOne4C.Text = 0.ToString();
            OutOne5C.Text = 0.ToString();
            OutOne6C.Text = 0.ToString();
            OutOne7C.Text = 0.ToString();
            OutOne8C.Text = 0.ToString();
            OutOne9C.Text = 0.ToString();
            OutOne10C.Text = 0.ToString();
            OutOne11C.Text = 0.ToString();
            OutOne12C.Text = 0.ToString();
            OutOne13C.Text = 0.ToString();
            OutOne14C.Text = 0.ToString();
        }
        private void resetOutputsFeedbackBoard2()
        {
            OutTwo1C.BackColor = colr;
            OutTwo2C.BackColor = colr;
            OutTwo3C.BackColor = colr;
            OutTwo4C.BackColor = colr;
            OutTwo5C.BackColor = colr;
            OutTwo6C.BackColor = colr;
            OutTwo7C.BackColor = colr;
            OutTwo8C.BackColor = colr;
            OutTwo9C.BackColor = colr;
            OutTwo10C.BackColor = colr;
            OutTwo11C.BackColor = colr;
            OutTwo12C.BackColor = colr;
            OutTwo13C.BackColor = colr;
            OutTwo14C.BackColor = colr;

            OutTwo1C.Text = 0.ToString();
            OutTwo2C.Text = 0.ToString();
            OutTwo3C.Text = 0.ToString();
            OutTwo4C.Text = 0.ToString();
            OutTwo5C.Text = 0.ToString();
            OutTwo6C.Text = 0.ToString();
            OutTwo7C.Text = 0.ToString();
            OutTwo8C.Text = 0.ToString();
            OutTwo9C.Text = 0.ToString();
            OutTwo10C.Text = 0.ToString();
            OutTwo11C.Text = 0.ToString();
            OutTwo12C.Text = 0.ToString();
            OutTwo13C.Text = 0.ToString();
            OutTwo14C.Text = 0.ToString();
        }
        private void resetOutputsFeedbackBoard3()
        {
            OutThree1C.BackColor = colr;
            OutThree2C.BackColor = colr;
            OutThree3C.BackColor = colr;
            OutThree4C.BackColor = colr;
            OutThree5C.BackColor = colr;
            //OutThree6C.BackColor = colr;
            //OutThree7C.BackColor = colr;
            //OutThree8C.BackColor = colr;
            //OutThree9C.BackColor = colr;
            //OutThree10C.BackColor = colr;
            //OutThree11C.BackColor = colr;
            //OutThree12C.BackColor = colr;
            //OutThree13C.BackColor = colr;
            //OutThree14C.BackColor = colr;

            OutThree1C.Text = 0.ToString();
            OutThree2C.Text = 0.ToString();
            OutThree3C.Text = 0.ToString();
            OutThree4C.Text = 0.ToString();
            OutThree5C.Text = 0.ToString();
            //OutThree6C.Text = 0.ToString();
            //OutThree7C.Text = 0.ToString();
            //OutThree8C.Text = 0.ToString();
            //outThree9C.Text = 0.ToString();
            //outThree10C.Text = 0.ToString();
            //outThree11C.Text = 0.ToString();
            //outThree12C.Text = 0.ToString();
            //outThree13C.Text = 0.ToString();
            //outThree14C.Text = 0.ToString();
        }
        private void reportListsReset()
        {
            for (int i = 0; i < 22; i++)
            {
                powerReport[i] = 0;
                powerCurReport[i] = "0";
            }
            for (int i = 0; i < 14; i++)
                netReport[i] = 0;
            for (int i = 0; i < 48; i++)
            {
                inputReport[i] = 0;
            }
            for (int i = 0; i < 49; i++)
            {
                outputReport[i] = 0;
                outputCurReport[i] = "0";
            }
            for (int i = 0; i < 2; i++)
                inputVoltReport[i] = "0";

        }
        //private void StopAutoTest()
        //{
        //    autoTest_timer.Stop();
        //    updateUI_timer.Start();
        //    tabControl1.SelectedTab = mainTab;
        //    autoTSt = 0;
        //    inerTSt = 0;
        //    autoTcounter = 0;
        //    PannelLock(false);
        //    feedbackPannel.BackColor = Color.Transparent;
        //    feedbackTb.Text = "";
        //    progressBar1.Visible = false;


        //}
        #endregion
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                var com = comPortCb.Text;
                comPortCb.Text = com;
                serialManager = new SerialManager(com.ToString(), int.Parse(baudRateCb.Text.ToString()), System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One, false);

                serialManager.OpenPort();
                if (serialManager.IsOpen())
                {
                    btnConnect.Enabled = false;
                    serialManager.SerialManagerDataReceive += ReadSerialEventHispeed;
                    btnConnect.BackColor = Color.MediumSpringGreen;
                    serialCheck_timer.Start();
                    updateUI_timer.Start();

                    // this.pictureBox1.Image = global::MainProject.Properties.Resources.tara2;
                    //welcome
                    inputParam.InputList1.Clear();
                    //  SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.Welcome);   //commented for test

                    //checkHealth_timer.Start();

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                AutoClosingMessageBox.Show(ex.ToString(), "Warning!", 2000);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {

            if (serialManager != null && (serialManager.IsOpen()))
            {
                serialManager.ClosePort();
            }
            conectedCom = null;
            serialCheck_timer.Stop();
            updateUI_timer.Stop();
            comPortCb.Items.Clear();
            comPortCb.Text = "";
            btnConnect.BackColor = Color.Transparent;
            //CheckSerialBox.BackColor = Color.LightGray;
            //connectPanel.BackColor = Color.Transparent;
            ResetWelcom();
            btnConnect.Enabled = true;
            serialConnectionCheck();

            // checkHealth_timer.Stop();
            //this.pictureBox1.Image = global::MainProject.Properties.Resources.tara4;

        }
        byte B1;
        byte B2;
        byte B3;
        byte B4;
        byte B5;
        byte B6;
        byte B7;
        byte B8;
        byte B9;
        Color welcomeColor = Color.LightGray;
        private void ResetWelcom()
        {
            B1 = 0;
            B2 = 0;
            B3 = 0;
            B4 = 0;
            B5 = 0;
            B6 = 0;
            B7 = 0;
            B8 = 0;
            B9 = 0;
            Board1.BackColor = welcomeColor;
            Board2.BackColor = welcomeColor;
            Board3.BackColor = welcomeColor;
            Board4.BackColor = welcomeColor;
            Board5.BackColor = welcomeColor;
            Board6.BackColor = welcomeColor;
            Board7.BackColor = welcomeColor;
            Board8.BackColor = welcomeColor;
            Board9.BackColor = welcomeColor;

        }
        private void WelcomExtract(byte data, byte data2)
        {
            B1 = (byte)(data & 0x1);
            B2 = (byte)(data >> 1 & 0x1);
            B3 = (byte)(data >> 2 & 0x1);
            B4 = (byte)(data >> 3 & 0x1);
            B5 = (byte)(data >> 4 & 0x1);
            B6 = (byte)(data >> 5 & 0x1);
            B7 = (byte)(data >> 6 & 0x1);
            B8 = (byte)(data >> 7 & 0x1);
            B9 = (byte)(data2 >> 0 & 0x1);

            // MessageBox.Show("System is Checking Boards Health,Please Wait..", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Hand);

        }
        private void WelcomCheck_Pre()
        {
            BCMPowerSw.Checked = true;
            Thread.Sleep(130);
            WelcomCheck();
        }
        private void WelcomCheck()
        {
            Board1.BackColor = B1 == 1 ? testGreen : testRed;
            Board2.BackColor = B2 == 1 ? testGreen : testRed;
            Board3.BackColor = B3 == 1 ? testGreen : testRed;
            Board4.BackColor = B4 == 1 ? testGreen : testRed;
            Board5.BackColor = B5 == 1 ? testGreen : testRed;
            Board6.BackColor = B6 == 1 ? testGreen : testRed;
            Board7.BackColor = B7 == 1 ? testGreen : testRed;
            Board8.BackColor = B8 == 1 ? testGreen : testRed;
            Board9.BackColor = B9 == 1 ? testGreen : testRed;

            PannelLock(false);
        }
        #endregion Connection
        #region led
        private void ledBulb_Click(object sender)//, EventArgs e
        {
            ((MainProject.LedBulb)sender).On = !((MainProject.LedBulb)sender).On;
        }
        private void ledBulb_Click(object sender, byte val)//, EventArgs e
        {
            if (val == 0)
            {
                ((MainProject.LedBulb)sender).Color = Color.Red;
            }
            else
            {
                ((MainProject.LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
          ((MainProject.LedBulb)sender).On = true;

        }
        private void ledBulb_SetColor(object sender, Color co)//, EventArgs e
        {

            ((MainProject.LedBulb)sender).Color = co;

            ((MainProject.LedBulb)sender).On = true;

        }
        #endregion
        #region InputPannel

        private void initParams()
        {
            inputParam = new InputParams();
            outputParam = new OutPutParams();
            netParams = new NetParams();
            digitalOutputFeedback = new OutPutParams();
            powerParams = new PowerParams();
        }
        #endregion
        //Read********************************************
        private void ShowPacket()
        {
            packetLabel.Text = PacketView.data;
        }
        //Write****************************************************



        public void SendSerialTestCmd(List<byte> iData, ID id, MsType mt)
        {
            var len = iData.Count + 3;
            byte[] sendList = new byte[len + 1];
            sendSum = 0;
            //var dataLen = iData.Count;
            sendList[0] = (byte)id;
            sendList[1] = (byte)mt;
            sendList[2] = (byte)iData.Count;
            for (int i = 0; i < iData.Count; i++)
            {
                sendList[3 + i] = iData[i];
            }
            for (int i = 1; i < sendList.Length; i++)
            {
                sendSum += sendList[i];
            }

            if (sendSum >= 0xff)
            {
                while (sendSum >= 0xff)
                {
                    sendSum -= 0xff;
                }

            }
            sendChecksum = (byte)(0xff - sendSum);
            sendList[len] = sendChecksum;  // sendChecksum;

            if (this.InvokeRequired)
                this.Invoke(new DispSendPack(DisplaySendPacket), sendList);
            else
                DisplaySendPacket(sendList);

            //sendLabel.Text = BitConverter.ToString(sendList);
            if (serialManager != null)
            {

                bool result = serialManager.Write(sendList.ToArray());//sendingData
                //sm.Write(sendList.ToArray());//sendingData

                if (!result)
                {

                    AutoClosingMessageBox.Show("  Please Open The Port", "Attention!", 1000);
                    return;
                }
            }
            else
                AutoClosingMessageBox.Show("  Please Open The Port", "Attention!", 1000);
        }

        public void SendSerialTestCmd(byte[] Data, ID id, MsType mt)
        {
            var len = Data.Length + 3;
            byte[] sendList = new byte[len + 1];
            sendSum = 0;
            //var dataLen = iData.Count;
            sendList[0] = (byte)id;
            sendList[1] = (byte)mt;
            sendList[2] = (byte)Data.Length;
            for (int i = 0; i < Data.Length; i++)
            {
                sendList[3 + i] = Data[i];
            }
            for (int i = 1; i < sendList.Length; i++)
            {
                sendSum += sendList[i];
            }

            if (sendSum >= 0xff)
            {
                while (sendSum >= 0xff)
                {
                    sendSum -= 0xff;
                }

            }
            sendChecksum = (byte)(0xff - sendSum);
            sendList[len] = sendChecksum;  // sendChecksum;

            if (this.InvokeRequired)
                this.Invoke(new DispSendPack(DisplaySendPacket), sendList);
            else
                DisplaySendPacket(sendList);
            //sendLabel.Text = BitConverter.ToString(sendList);
            if (serialManager != null)
            {

                bool result = serialManager.Write(sendList.ToArray());//sendingData
                //sm.Write(sendList.ToArray());//sendingData

                if (!result)
                {
                    AutoClosingMessageBox.Show("  Please Open The Port", "Attention!", 1000);
                    return;
                }
            }
            else
                AutoClosingMessageBox.Show("  Please Open The Port", "Attention!", 1000);

        }

        private void DisplaySendPacket(byte[] slist)
        {
            sendLabel.Text = BitConverter.ToString(slist);
        }
        //private bool SwBtnClick(swClass op, Button btn)
        //{
        //    if (op.check)
        //    {
        //        btn.Image = Properties.Resources.ofState;
        //        op.check = false;
        //        return false;
        //    }
        //    else
        //    {
        //        btn.Image = Properties.Resources.onState;
        //        op.check = true;
        //        return true;

        //    }
        //}

        //***************************************************
        private void btnStart_Click(object sender, EventArgs e)
        {
            TestMode.Auto = false;
            TestMode.Manual = true;

            if ((serialManager != null) && (serialManager.IsOpen()))
            {

                if (tabControl1.SelectedTab == inputTab)
                {
                    InputTestCmd();
                }
                else if (tabControl1.SelectedTab == input2Tab)
                {
                    Input2TestCmd();
                    CASInputTestCmd();
                }
                else if (tabControl1.SelectedTab == outputTab)
                {
                    OutputTestCmd();

                }
                else if (tabControl1.SelectedTab == output2Tab)
                {
                    OutputTestCmd2();

                }
                if (tabControl1.SelectedTab == netTab)
                {
                    NetworkTestCmd();
                }


            }
            else
            {

                AutoClosingMessageBox.Show("  Please Open The Port", "Attention!", 1000);

            }
        }

        // Thread fuelTestThread;
        private void Input2TestCmd()
        {
            InputTestB2Cmd();
            inputParam.SetInputParamsB2();

            //Thread.Sleep(100);
            foreach (var item in inputParam.InputList2)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(inputParam.InputList2, ID.InputB2, MsType.SwitchONOff);
                    break;
                }
            }

            if (!TestMode.Auto)
            {
                resetInputsFeedback();
            }
            // resetInputs2();
            AnalogInput.p = (byte)(In1_An1.Checked ? 1 : 0);
            if (AnalogInput.p > 0)
            {
                fState = 0;
                //In1_An1.Checked = false;
                //Thread fuelTestThread = new Thread(new ThreadStart(FuelTest));
                //fuelTestThread.Start();
                FuelTest();

            }

        }
        byte casInputs;//= new byte[7];
        byte[] casData = new byte[2];
        //private void CASInputTestBCmd() //????
        //{

        //    casInputParam.b1 = (byte)(In2_H5.Checked ? 1 : 0);
        //    casInputParam.b2 = (byte)(In2_H2.Checked ? 1 : 0);
        //    casInputParam.b3 = (byte)(In2_H3.Checked ? 1 : 0);
        //    casInputParam.b4 = (byte)(In2_H4.Checked ? 1 : 0);

        //    casInputParam.b5 = (byte)(In2_L12.Checked ? 1 : 0);
        //    casInputParam.b6 = (byte)(In2_L13.Checked ? 1 : 0);
        //    casInputParam.b7 = (byte)(In2_L14.Checked ? 1 : 0);
        //}
        private void CASInputTestCmd()
        {
            //H1
            casInputs = (byte)(In2_H5.Checked ? 1 : 0);

            if (casInputs > 0)
            {
                casData[0] = 1;
                casData[1] = 0;
                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);

            }
            //H2
            casInputs = (byte)(In2_H2.Checked ? 1 : 0);
            if (casInputs > 0)
            {
                casData[0] = 2;
                casData[1] = 0;
                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
            }
            //H3
            casInputs = (byte)(In2_H3.Checked ? 1 : 0);
            if (casInputs > 0)
            {
                casData[0] = 3;
                casData[1] = 0;
                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
            }
            //H4
            casInputs = (byte)(In2_H4.Checked ? 1 : 0);
            if (casInputs > 0)
            {
                casData[0] = 4;
                casData[1] = 0;
                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
            }
            //L12
            casInputs = (byte)(In2_L12.Checked ? 1 : 0);
            if (casInputs > 0)
            {
                casData[0] = 5;
                casData[1] = 0;
                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
            }
            //L13
            casInputs = (byte)(In2_L13.Checked ? 1 : 0);
            if (casInputs > 0)
            {
                casData[0] = 6;
                casData[1] = 0;
                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
            }
            //L14
            casInputs = (byte)(In2_L14.Checked ? 1 : 0);
            if (casInputs > 0)
            {
                casData[0] = 7;
                casData[1] = 0;
                SendSerialTestCmd(casData, ID.Network_Board, MsType.CASInputStatus);
            }
        }

        byte fState = 0;

        private void FuelTest()
        {
            switch (fState)
            {
                case 0:
                    AnalogInput.AnInputList.Clear();
                    AnalogInput.AnInputList.Add((byte)FuelLvl.Full);
                    SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                    FuelResponse.Full_Sent = true;
                    //if (FuelResponse.Full_Recv)
                    fState = 1;
                    break;
                case 1:
                    AnalogInput.AnInputList.Clear();
                    AnalogInput.AnInputList.Add((byte)FuelLvl.ThreeQuarters);
                    SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                    FuelResponse.ThreeQuarters_Sent = true;
                    //if (FuelResponse.ThreeQuarters_Recv)
                    fState = 2;
                    break;
                case 2:
                    AnalogInput.AnInputList.Clear();
                    AnalogInput.AnInputList.Add((byte)FuelLvl.Half);
                    SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                    FuelResponse.Half_Sent = true;
                    //if (FuelResponse.Half_Recv)
                    fState = 3;
                    break;
                case 3:
                    AnalogInput.AnInputList.Clear();
                    AnalogInput.AnInputList.Add((byte)FuelLvl.Quarter);
                    SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                    FuelResponse.Quarter_Sent = true;
                    //if (FuelResponse.Quarter_Recv)
                    fState = 4;
                    break;
                case 4:
                    AnalogInput.AnInputList.Clear();
                    AnalogInput.AnInputList.Add((byte)FuelLvl.Empty);
                    SendSerialTestCmd(AnalogInput.AnInputList, ID.InputB1, MsType.BCMAnalogInputStatus);
                    FuelResponse.Empty_Sent = true;
                    //if (FuelResponse.Empty_Recv)
                    fState = 0;
                    //fuelTestThread.Abort();
                    break;
            }
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            AutoTestStart();
        }
        private void AutoTestStart()
        {
            ResetAll();

            if (serialManager != null)
                if (serialManager.IsOpen())
                {
                    // outputParam.DigitalOutputList.AddRange(Oauto);
                    TestMode.Auto = true;
                    PannelLock(true);
                    autoTcounter = 0;

                    if (TestInfoClass.TestStrategy == 1)// strategy.GoToEnd)
                        autoTest_timer.Start();
                    else if (TestInfoClass.TestStrategy == 0)//strategy.StopWithError)
                        autoTest_timer2.Start();

                    stopWatch = new Stopwatch();
                    stopWatch.Start();

                }
                else
                {
                    AutoClosingMessageBox.Show("Please Connect First!", "Attention!", 1000);


                }
            else
            {
                AutoClosingMessageBox.Show(" Please Connect First!", "Attention!", 1000);
            }
        }



        private void btnBCMPower_CheckedChanged(object sender, EventArgs e)
        {
            byte[] data = new byte[1];
            if (BCMPowerSw.Checked)
            {
                BCMPowerSw.Image = Properties.Resources.onState;
                data[0] = 1;

            }
            else
            {
                BCMPowerSw.Image = Properties.Resources.ofState;
                data[0] = 0;
            }

            if (!powerShortFlag && !stopFlag)
                SendSerialTestCmd(data, ID.Power_Board, MsType.PowerVoltSw);

            powerShortFlag = false;
            stopFlag = false;
        }

        private void btnBoardPower_CheckedChanged(object sender, EventArgs e)
        {
            data = new byte[1];
            if (btnBoardPower.Checked)
            {
                btnBoardPower.Image = Properties.Resources.onState;
                data[0] = 1;
                btnBoardPower.Enabled = false;
            }
            else
            {
                btnBoardPower.Image = Properties.Resources.ofState;
                data[0] = 0;
            }
            if (!powerShortFlag)
                SendSerialTestCmd(data, ID.Power_Board, MsType.BoardPowerSw);
        }

        private void NetworkTestCmd()
        {
            netParams.BCM_CANHS = (byte)(BCM_CANHS.Checked ? 1 : 0);
            netParams.BCM_CANLS = (byte)(BCM_CANLS.Checked ? 2 : 0);

            netParams.BCM_LinFront = (byte)(BCM_LINF.Checked ? 1 : 0);
            netParams.BCM_LinRear = (byte)(BCM_LINR.Checked ? 2 : 0);
            //CAS
            netParams.CAS_CANLS = (byte)(CAS_CANLS.Checked ? 3 : 0);
            netParams.IMMO_LIN = (byte)(ImmoTest.Checked ? 1 : 0);
            netParams.Kick_LIN = (byte)(KickTest.Checked ? 1 : 0);
            netParams.TPMS = (byte)(TPMSTest.Checked ? 1 : 0);
            netParams.RKE = (byte)(RKETest.Checked ? 1 : 0);
            netParams.LF = (byte)(LFTest.Checked ? 1 : 0);

            netParams.R_DHS_Lock = (byte)(RDHSLock1.Checked ? 1 : 0);
            netParams.R_DHS_UnLock = (byte)(RDHSUnlock1.Checked ? 2 : 0);
            netParams.L_DHS_Lock = (byte)(LDHSLock1.Checked ? 3 : 0);
            netParams.L_DHS_UnLock = (byte)(LDHSUnlock1.Checked ? 4 : 0);


            if (netParams.BCM_CANHS >= 1)
            {
                netParams.randomNum = 0xaabbccdd;// random.Next();
                netParams.DataCANH = new byte[5] { netParams.BCM_CANHS, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                SendSerialTestCmd(netParams.DataCANH.ToList(), ID.Network_Board, MsType.CANMessage);
            }
            if (netParams.BCM_CANLS >= 1)
            {
                netParams.randomNum = 0x22334455;// random.Next();
                netParams.DataCANL = new byte[5] { netParams.BCM_CANLS, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                SendSerialTestCmd(netParams.DataCANL.ToList(), ID.Network_Board, MsType.CANMessage);
            }
            if (netParams.BCM_LinFront >= 1)
            {
                netParams.randomNum = 0x66778899;// random.Next();
                netParams.DataLinF = new byte[5] { netParams.BCM_LinFront, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                SendSerialTestCmd(netParams.DataLinF.ToList(), ID.Network_Board, MsType.LINMessage);
            }
            if (netParams.BCM_LinRear >= 1)
            {
                netParams.randomNum = 0x98765432;// random.Next();
                netParams.DataLinR = new byte[5] { netParams.BCM_LinRear, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                SendSerialTestCmd(netParams.DataLinR.ToList(), ID.Network_Board, MsType.LINMessage);
            }
            //CAS
            if (netParams.CAS_CANLS >= 1)
            {
                netParams.randomNum = 0xabcdef89;// random.Next();
                netParams.DataCASCANL = new byte[5] { netParams.CAS_CANLS, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                SendSerialTestCmd(netParams.DataCASCANL.ToList(), ID.Network_Board, MsType.CANMessage);
            }
            if (netParams.IMMO_LIN >= 1)
            {
                data = new byte[0];
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.Immo);
            }
            if (netParams.Kick_LIN >= 1)
            {
                data = new byte[0];
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.KickSensor);
            }
            if (netParams.TPMS >= 1)
            {
                data = new byte[0];
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.TPMS);
            }
            if (netParams.RKE >= 1)
            {
                data = new byte[0];
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.RKE);
            }
            if (netParams.LF >= 1)
            {
                data = new byte[0];
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.LF);
            }
            if (netParams.R_DHS_Lock >= 1)
            {
                data = new byte[1];
                data[0] = netParams.R_DHS_Lock;
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.DHS);
            }
            if (netParams.R_DHS_UnLock >= 1)
            {
                data = new byte[1];
                data[0] = netParams.R_DHS_UnLock;
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.DHS);
            }
            if (netParams.L_DHS_Lock >= 1)
            {
                data = new byte[1];
                data[0] = netParams.L_DHS_Lock;
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.DHS);
            }
            if (netParams.L_DHS_UnLock >= 1)
            {
                data = new byte[1];
                data[0] = netParams.L_DHS_UnLock;
                SendSerialTestCmd(data, ID.CAS_tBoard, MsType.DHS);
            }

            //netParamsReset();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (LoginData.IsLogin)
            {
                btnLogout.Text = "  LogOut  ";
                // btnLogout.Image = global::TesterIKBCM.Properties.Resources.AutoStop;
                LoginData.IsLogin = false;
                LoginData.UserName = "..";
                //  SettingHouse.UserName = "..";

            }
            else
            {
                btnLogout.Text = "  Login  ";
                // btnLogout.Image = global::TesterIKBCM.Properties.Resources.logout2;

            }

            SettingHouse.OpenLoginForm();
        }


        private void btnLoadTest1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = outputTab;
            resetOutputsFeedbackBoard1();
            resetOutputsFeedbackBoard2();
            resetOutputsFeedbackBoard3();

            ID id = ID.OutputB1;
            SendSerialTestCmd(outputParam.OutputList1, id, MsType.OutputTest1);
        }

        private void btnLoadTest2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = outputTab;
            resetOutputsFeedbackBoard1();
            resetOutputsFeedbackBoard2();
            resetOutputsFeedbackBoard3();
            ID id = ID.OutputB1;
            SendSerialTestCmd(outputParam.OutputList1, id, MsType.OutputTest2);
        }
        Setting st = new Setting();
        private void btnSetting_Click(object sender, EventArgs e)
        {
            DataBase.SaveTestSpecTblInDb();
            if (st == null || (st.IsDisposed))
                st = new Setting();
            st.Show();
        }



        byte hs = 0;



        private void OutputTestCmd()
        {
            OutputTestB1Cmd();
            OutputTestB2Cmd();

            outputParam.SetOutputParamsO1();
            outputParam.SetOutputParamsO2();


            foreach (var item in outputParam.OutputList1)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.SwitchONOff);
                    break;
                }
            }

            foreach (var item in outputParam.OutputList2)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(outputParam.OutputList2, ID.OutputB2, MsType.SwitchONOff);
                    break;
                }
            }

            resetOutputs();
        }

        private void OutputTestCmd2()
        {
            OutputTestB3Cmd();
            OutputTestBDCmd();
            outputParam.SetOutputParamsO3();
            outputParam.SetDigitalOutput(hs);


            foreach (var item in outputParam.OutputList3)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(outputParam.OutputList3, ID.OutputB3, MsType.SwitchONOff);
                    break;
                }
            }
            //if (outputParam.OBoard1 > 0)
            //    SendSerialStringInputList(outputParam.OutputList1, ID.OutputB1, MsType.SwitchONOff);

            foreach (var item in outputParam.DigitalOutputList)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(outputParam.DigitalOutputList, ID.DigitalOutput, MsType.SwitchONOff);
                    hs = 0;
                    break;
                }
            }
            resetOutputs2();
        }
        private void OutputTestB1Cmd()
        {
            //Board1
            outputParam.b0 = (byte)(OutOne1.Checked ? 1 : 0);
            outputParam.b1 = (byte)(OutOne2.Checked ? 1 : 0);
            outputParam.b2 = (byte)(OutOne3.Checked ? 1 : 0);
            outputParam.b3 = (byte)(OutOne4.Checked ? 1 : 0);
            outputParam.b4 = (byte)(OutOne5.Checked ? 1 : 0);
            outputParam.b5 = (byte)(OutOne6.Checked ? 1 : 0);
            outputParam.b6 = (byte)(OutOne7.Checked ? 1 : 0);
            outputParam.b7 = (byte)(OutOne8.Checked ? 1 : 0);
            outputParam.b8 = (byte)(OutOne9.Checked ? 1 : 0);
            outputParam.b9 = (byte)(OutOne10.Checked ? 1 : 0);
            outputParam.b10 = (byte)(OutOne11.Checked ? 1 : 0);
            outputParam.b11 = (byte)(OutOne12.Checked ? 1 : 0);
            outputParam.b12 = (byte)(OutOne13.Checked ? 1 : 0);
            outputParam.b13 = (byte)(OutOne14.Checked ? 1 : 0);

        }
        private void OutputTestB2Cmd()
        {
            outputParam.b14 = (byte)(OutTwo1.Checked ? 1 : 0);
            outputParam.b15 = (byte)(OutTwo2.Checked ? 1 : 0);
            outputParam.b16 = (byte)(OutTwo3.Checked ? 1 : 0);
            outputParam.b17 = (byte)(OutTwo4.Checked ? 1 : 0);
            outputParam.b18 = (byte)(OutTwo5.Checked ? 1 : 0);
            outputParam.b19 = (byte)(OutTwo6.Checked ? 1 : 0);
            outputParam.b20 = (byte)(OutTwo7.Checked ? 1 : 0);
            outputParam.b21 = (byte)(OutTwo8.Checked ? 1 : 0);
            outputParam.b22 = (byte)(OutTwo9.Checked ? 1 : 0);
            outputParam.b23 = (byte)(OutTwo10.Checked ? 1 : 0);
            outputParam.b24 = (byte)(OutTwo11.Checked ? 1 : 0);
            outputParam.b25 = (byte)(OutTwo12.Checked ? 1 : 0);
            outputParam.b26 = (byte)(OutTwo13.Checked ? 1 : 0);
            outputParam.b27 = (byte)(OutTwo14.Checked ? 1 : 0);
        }
        private void OutputTestB3Cmd()
        {
            outputParam.b28 = (byte)(OutThree1.Checked ? 1 : 0);
            outputParam.b29 = (byte)(OutThree2.Checked ? 1 : 0);
            outputParam.b30 = (byte)(OutThree3.Checked ? 1 : 0);
            outputParam.b31 = (byte)(OutThree4.Checked ? 1 : 0);
            outputParam.b32 = (byte)(OutThree5.Checked ? 1 : 0);
            outputParam.b33 = 0;// (byte)(OutDH9.Checked ? 1 : 0);
            outputParam.b34 = 0;//(byte)(OutDH10.Checked ? 1 : 0);
            outputParam.b35 = 0;//(byte)(OutThree5.Checked ? 1 : 0);
            outputParam.b36 = 0;
            outputParam.b37 = 0;
            outputParam.b38 = 0;
            outputParam.b39 = 0;
            outputParam.b40 = 0;
            outputParam.b41 = 0;

        }
        private void OutputTestBDCmd()
        {
            resetOutputsFeedbackBoardD();
            OutputTestBDReset();

            outputParam.db0 = (byte)(OutDL1.Checked ? 1 : 0);
            outputParam.db1 = (byte)(OutDL2.Checked ? 1 : 0);
            outputParam.db2 = (byte)(OutDL3.Checked ? 1 : 0);
            outputParam.db3 = (byte)(OutDL4.Checked ? 1 : 0);
            outputParam.db4 = (byte)(OutDH1.Checked ? 1 : 0);
            outputParam.db5 = (byte)(OutDH2.Checked ? 1 : 0);
            outputParam.db6 = (byte)(OutDH3.Checked ? 1 : 0);

            outputParam.db7 = (byte)(OutDH4.Checked ? 1 : 0);
            outputParam.db8 = (byte)(OutDH5.Checked ? 1 : 0);
            outputParam.db9 = (byte)(OutDH6.Checked ? 1 : 0);
            outputParam.db10 = (byte)(OutDH7.Checked ? 1 : 0);
            outputParam.db11 = (byte)(OutDH8.Checked ? 1 : 0);
            outputParam.db12 = (byte)(OutDH9.Checked ? 1 : 0);
            outputParam.db13 = (byte)(OutDH10.Checked ? 1 : 0);

        }
        private void resetOutputsFeedbackBoardD()
        {
            ledBulb_Off(OutDL1_led);
            ledBulb_Off(OutDL2_led);
            ledBulb_Off(OutDL3_led);
            ledBulb_Off(OutDL4_led);
            ledBulb_Off(OutDH1_led);
            ledBulb_Off(OutDH2_led);
            ledBulb_Off(OutDH3_led);
            ledBulb_Off(OutDH4_led);
            ledBulb_Off(OutDH5_led);
            ledBulb_Off(OutDH6_led);
            ledBulb_Off(OutDH7_led);
            ledBulb_Off(OutDH8_led);
            ledBulb_Off(OutDH9_led);
            ledBulb_Off(OutDH10_led);


        }
        private void OutputTestBDReset()
        {
            outputParam.db0 = 0;
            outputParam.db1 = 0;
            outputParam.db2 = 0;
            outputParam.db3 = 0;
            outputParam.db4 = 0;
            outputParam.db5 = 0;
            outputParam.db6 = 0;
            outputParam.db7 = 0;
            outputParam.db8 = 0;
            outputParam.db9 = 0;
            outputParam.db10 = 0;
        }

        private void comPortCb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void In2_H5_CheckedChanged(object sender, EventArgs e)
        {
            In2_H2.Checked = false;
            In2_H3.Checked = false;
            In2_H4.Checked = false;
            In2_L12.Checked = false;
            In2_L13.Checked = false;
            In2_L14.Checked = false;
        }

        private void In2_H2_CheckedChanged(object sender, EventArgs e)
        {
            In2_H5.Checked = false;
            In2_H3.Checked = false;
            In2_H4.Checked = false;
            In2_L12.Checked = false;
            In2_L13.Checked = false;
            In2_L14.Checked = false;
        }

        private void In2_H3_CheckedChanged(object sender, EventArgs e)
        {
            In2_H5.Checked = false;
            In2_H2.Checked = false;
            In2_H4.Checked = false;
            In2_L12.Checked = false;
            In2_L13.Checked = false;
            In2_L14.Checked = false;
        }

        private void In2_H4_CheckedChanged(object sender, EventArgs e)
        {
            In2_H5.Checked = false;
            In2_H2.Checked = false;
            In2_H3.Checked = false;
            In2_L12.Checked = false;
            In2_L13.Checked = false;
            In2_L14.Checked = false;
        }

        private void In2_L12_CheckedChanged(object sender, EventArgs e)
        {
            In2_H5.Checked = false;
            In2_H2.Checked = false;
            In2_H3.Checked = false;
            In2_H4.Checked = false;
            In2_L13.Checked = false;
            In2_L14.Checked = false;
        }

        private void In2_L13_CheckedChanged(object sender, EventArgs e)
        {
            In2_H5.Checked = false;
            In2_H2.Checked = false;
            In2_H3.Checked = false;
            In2_H4.Checked = false;
            In2_L12.Checked = false;
            In2_L14.Checked = false;
        }

        private void In2_L14_CheckedChanged(object sender, EventArgs e)
        {
            In2_H5.Checked = false;
            In2_H2.Checked = false;
            In2_H3.Checked = false;
            In2_H4.Checked = false;
            In2_L12.Checked = false;
            In2_L13.Checked = false;
        }

        private void InputTestCmd()
        {
            InputTestB1Cmd();
            inputParam.SetInputParamsB1();
            //InputTestB2Cmd();
            //inputParam.SetInputParamsB2();

            foreach (var item in inputParam.InputList1)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);
                    break;
                }

            }
            Thread.Sleep(100);

            if (!TestMode.Auto)
                resetInputsFeedback();

            // resetInputs();
        }

        //private void In2_H5Check_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (In2_H5Check.Checked)
        //    {
        //        In2_H5Check.Image = Properties.Resources.onState;
        //    }
        //    else
        //    {
        //        In2_H5Check.Image = Properties.Resources.ofState;
        //    }
        //}



        private void InputTestB1Cmd()
        {
            //Board1
            inputParam.b1 = (byte)(In1_L1.Checked ? 1 : 0);
            //if (In1_L12.Checked)
            //{
            //    AutoTest.InputCheckd = In1_L12.Name;
            //    cselect= In1_L12_led;
            //}
            inputParam.b2 = (byte)(In1_L2.Checked ? 1 : 0);
            inputParam.b3 = (byte)(In1_L3.Checked ? 1 : 0);
            inputParam.b4 = (byte)(In1_L4.Checked ? 1 : 0);
            inputParam.b5 = (byte)(In1_L5.Checked ? 1 : 0);
            inputParam.b6 = (byte)(In1_L6.Checked ? 1 : 0);
            inputParam.b7 = (byte)(In1_L7.Checked ? 1 : 0);
            inputParam.b8 = (byte)(In1_L8.Checked ? 1 : 0);
            inputParam.b9 = (byte)(In1_L9.Checked ? 1 : 0);
            inputParam.b10 = (byte)(In1_L10.Checked ? 1 : 0); //
            inputParam.b11 = (byte)(In1_L11.Checked ? 1 : 0);
            inputParam.b12 = (byte)(In1_L12.Checked ? 1 : 0);
            inputParam.b13 = (byte)(In1_L13.Checked ? 1 : 0);
            inputParam.b14 = (byte)(In1_L14.Checked ? 1 : 0);
            inputParam.b15 = (byte)(In1_L15.Checked ? 1 : 0);
            inputParam.b16 = (byte)(In1_L16.Checked ? 1 : 0);
            inputParam.b17 = (byte)(In1_L17.Checked ? 1 : 0);
            inputParam.b18 = (byte)(In1_L18.Checked ? 1 : 0);
            inputParam.b19 = (byte)(In1_L19.Checked ? 1 : 0);
            inputParam.b20 = (byte)(In1_L20.Checked ? 1 : 0);
            inputParam.b21 = (byte)(In1_L21.Checked ? 1 : 0);
            inputParam.b22 = (byte)(In1_L22.Checked ? 1 : 0);
            inputParam.b23 = (byte)(In1_L23.Checked ? 1 : 0);
            inputParam.b24 = (byte)(In1_L24.Checked ? 1 : 0);
            inputParam.b25 = (byte)(In1_L25.Checked ? 1 : 0);
            inputParam.b26 = (byte)(In1_L26.Checked ? 1 : 0);
            inputParam.b27 = (byte)(In1_L27.Checked ? 1 : 0);
            inputParam.b28 = (byte)(In1_L28.Checked ? 1 : 0);
            inputParam.b29 = (byte)(In1_L29.Checked ? 1 : 0);
            inputParam.b30 = 0;// (byte)(In1_L30.Checked ? 1 : 0);
            //High
            inputParam.b31 = 0;     //byte)(In1_An1.Checked ? 1 : 0);
            inputParam.b32 = 0;     //byte)(In1_An1.Checked ? 1 : 0); //???
            inputParam.b33 = 0;     //byte)(In1_An1.Checked ? 1 : 0);
            inputParam.b34 = 0;     //byte)(In1_An1.Checked ? 1 : 0);
            inputParam.b35 = 0;     //byte)(In1_An1.Checked ? 1 : 0);
            inputParam.b36 = 0;     //(byte)(In1_An1.Checked ? 1 : 0);


        }

        private void inputTest_CheckedChanged(object sender, EventArgs e)
        {
            if (inputTestSw.Checked)
            {
                inputTestSw.Image = Properties.Resources.onState;
            }
            else
            {
                inputTestSw.Image = Properties.Resources.ofState;
            }
        }

        private void outputTest_CheckedChanged(object sender, EventArgs e)
        {
            if (outputTestSw.Checked)
            {
                outputTestSw.Image = Properties.Resources.onState;
            }
            else
            {
                outputTestSw.Image = Properties.Resources.ofState;
            }
        }

        private void CASTest_CheckedChanged(object sender, EventArgs e)
        {
            if (CASTest.Checked)
            {
                CASTest.Image = Properties.Resources.onState;
            }
            else
            {
                CASTest.Image = Properties.Resources.ofState;
            }
        }

        private void loadTest2_CheckedChanged(object sender, EventArgs e)
        {
            if (loadTest2.Checked)
            {
                loadTest2.Image = Properties.Resources.onState;
            }
            else
            {
                loadTest2.Image = Properties.Resources.ofState;
            }
        }

        private void AnInputTest_CheckedChanged(object sender, EventArgs e)
        {
            if (AnInputTest.Checked)
            {
                AnInputTest.Image = Properties.Resources.onState;
            }
            else
            {
                AnInputTest.Image = Properties.Resources.ofState;
            }
        }

        private void loadTest1_CheckedChanged(object sender, EventArgs e)
        {
            //if (loadTest1.Checked)
            //{
            //    loadTest1.Image = Properties.Resources.onState;
            //}
            //else
            //{
            //    loadTest1.Image = Properties.Resources.ofState;
            //}
        }



        private void InputTestB2Cmd()
        {
            //Board2
            inputParam.b2_1 = (byte)(In2_L1.Checked ? 1 : 0);
            inputParam.b2_2 = (byte)(In2_L2.Checked ? 1 : 0);
            inputParam.b2_3 = (byte)(In2_L3.Checked ? 1 : 0);
            inputParam.b2_4 = (byte)(In2_L4.Checked ? 1 : 0);
            inputParam.b2_5 = (byte)(In2_L5.Checked ? 1 : 0);
            inputParam.b2_6 = (byte)(In2_L6.Checked ? 1 : 0);
            inputParam.b2_7 = (byte)(In2_L7.Checked ? 1 : 0);
            inputParam.b2_8 = (byte)(In2_L8.Checked ? 1 : 0);
            inputParam.b2_9 = (byte)(In2_L9.Checked ? 1 : 0);
            inputParam.b2_10 = (byte)(In2_L10.Checked ? 1 : 0);
            inputParam.b2_11 = (byte)(In2_L11.Checked ? 1 : 0);
            //inputParam.b2_12 = (byte)(In2_L12.Checked ? 1 : 0);
            //inputParam.b2_13 = (byte)(In2_L13.Checked ? 1 : 0);
            //inputParam.b2_14 = (byte)(In2_L14.Checked ? 1 : 0);

            inputParam.b2_31 = (byte)(In2_H1.Checked ? 1 : 0);
            //inputParam.b2_32 = (byte)(In2_H2.Checked ? 1 : 0); //b2_15 ???
            //inputParam.b2_33 = (byte)(In2_H3.Checked ? 1 : 0);
            //inputParam.b2_34 = (byte)(In2_H4.Checked ? 1 : 0);
            //inputParam.b2_35 = (byte)(In2_H5.Checked ? 1 : 0);


        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataBase.SaveTestSpecTblInDb();
        }

        private void userLbl_Click(object sender, EventArgs e)
        {
            if (barPassword.Text.ToString() != "") //&& txtID.Text != "")
            {
                int UID = int.Parse(barPassword.Text.ToString());
                string user = userLbl.Text;
                SettingHouse.UserName = "";
                SettingHouse.UserID = UID;

                bool res = DataBase.LoadUser();

                if (res && (UID == SettingHouse.UserID)) //(user == SettingHouse.UserName) && 
                {
                    //TabControlVisible(true);
                    // lblInfo.Text = "????";
                    //panelLogin.Visible = false;
                    SettingHouse.loginLabel = $"Welcome {user}";
                    //PersonName.Text = $"Welcome {user}";
                    // PersonName.ForeColor = Color.SteelBlue;
                    LoginData.UserID = SettingHouse.UserID;
                    LoginData.UserName = SettingHouse.UserName;
                    LoginData.accessLevel = SettingHouse.accessLevel;
                    LoginData.IsLogin = true;
                    barPassword.Text = "";
                    //this.Close();

                }
                else
                {
                    //lblwelcom.ForeColor = Color.Tomato;
                    //  lblInfo.Text = "Username Or UserID Is Incorect";
                    // PersonName.Text = "Username Or UserID Is Incorect";
                    LoginData.IsLogin = false;
                    AutoClosingMessageBox.Show(" The Password is wrong!", "Attention!", 3000);

                }
            }
        }
        byte[] suplier = new byte[1] { 0 };
        private void btnSuplier_Click(object sender, EventArgs e)
        {
            suplier[0] = (byte)suplierCodeCb.SelectedIndex;
            SendSerialTestCmd(suplier, ID.Network_Board, MsType.SupplierCode);
        }

        private void suplierCodeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            //suplier[0] = (byte)suplierCodeCb.SelectedIndex;
            //SendSerialTestCmd(suplier, ID.Network_Board, MsType.SupplierCode);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = global::TesterIKBCM.Properties.Resources.WhiteBackground;

        }


        private void button9_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = global::TesterIKBCM.Properties.Resources.IMG_1299;

        }

        private void button10_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = global::TesterIKBCM.Properties.Resources._20240506_144253;

        }

        private void OutOne1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                 OutOne2.Checked = OutOne3.Checked = OutOne4.Checked = OutOne5.Checked = OutOne6.Checked = OutOne7.Checked = OutOne8.Checked =
                                OutOne9.Checked = OutOne10.Checked = OutOne11.Checked = OutOne12.Checked = false;

                OutTwo1.Checked = OutTwo2.Checked = OutTwo3.Checked = OutTwo4.Checked = OutTwo5.Checked = OutTwo6.Checked = OutTwo7.Checked =
                 OutTwo8.Checked = OutTwo9.Checked = OutTwo10.Checked = OutTwo11.Checked = false;

                OutThree1.Checked = OutThree2.Checked = OutThree3.Checked = OutThree4.Checked = OutThree5.Checked = false;
              //  ((CheckBox)sender).Checked = true;
            }catch(Exception ex)
            {

            }
        }

        private void resetInputs()
        {
            //AutoTest.InputRec = false;
            //AutoTest.InputRecevd = 0;
            //AutoTest.Input2Rec = false;
            inputParam.InputList1.Clear();
            inputParam.InputList2.Clear();

            In1_L1.Checked = false;
            In1_L2.Checked = false;
            In1_L3.Checked = false;
            In1_L4.Checked = false;
            In1_L5.Checked = false;
            In1_L6.Checked = false;
            In1_L7.Checked = false;
            In1_L8.Checked = false;
            In1_L9.Checked = false;
            In1_L10.Checked = false;
            In1_L11.Checked = false;
            In1_L12.Checked = false;
            In1_L13.Checked = false;
            In1_L14.Checked = false;
            In1_L15.Checked = false;
            In1_L16.Checked = false;
            In1_L17.Checked = false;
            In1_L18.Checked = false;
            In1_L19.Checked = false;
            In1_L20.Checked = false;
            In1_L21.Checked = false;
            In1_L22.Checked = false;
            In1_L23.Checked = false;
            In1_L24.Checked = false;
            In1_L25.Checked = false;
            In1_L26.Checked = false;
            In1_L27.Checked = false;
            In1_L28.Checked = false;
            In1_L29.Checked = false;
            // In1_L30.Checked = false;
            In1_An1.Checked = false;

        }
        private void resetInputs2()
        {
            //AutoTest.InputRecevd = 0;
            //AutoTest.Input2Rec = false;
            inputParam.InputList2.Clear();
            //Board2
            In2_L1.Checked = false;
            In2_L2.Checked = false;
            In2_L3.Checked = false;
            In2_L4.Checked = false;
            In2_L5.Checked = false;
            In2_L6.Checked = false;
            In2_L7.Checked = false;
            In2_L8.Checked = false;
            In2_L9.Checked = false;
            In2_L10.Checked = false;
            In2_L11.Checked = false;

            In2_H1.Checked = false;

        }
        private void resetCASInputs()
        {
            In2_H2.Checked = false;
            In2_H3.Checked = false;
            In2_H4.Checked = false;
            In2_H5.Checked = false;

            In2_L12.Checked = false;
            In2_L13.Checked = false;
            In2_L14.Checked = false;
        }

        private void rArrow1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = netTab;
        }
        private void rArrow2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = input2Tab;
        }
        private void rArrow3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = mainTab;
        }
        private void rArrow4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = output2Tab;
        }
        private void rArrow5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = inputTab;
        }
        private void rArrow6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = outputTab;
        }
        //

        private void lArrow2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = output2Tab;
        }
        private void lArrow3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = inputTab;
        }
        private void lArrow4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = netTab;
        }
        private void lArrow5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = outputTab;
        }
        private void lArrow6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = mainTab;
        }
    }

}