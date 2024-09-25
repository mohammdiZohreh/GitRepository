
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraEditors;
using System.IO.Ports;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DevExpress.XtraPrinting;
using System.Diagnostics;

namespace TesterIKBCM
{
    public partial class MainForm : Form
    {
        private int _blink = 0;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer_SerialCheck = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer autoTest_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer updateUI_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer NewDay_timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer checkHealth_timer = new System.Windows.Forms.Timer();
        //System.Windows.Forms.Timer AutoClose_timer = new System.Windows.Forms.Timer();



        public SerialManager serialManager { get; set; }
        public InputParams inputParam { get; set; }
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BCMTester));
        string path = Directory.GetCurrentDirectory() + @"\AllUsers";
        string path2 = Directory.GetCurrentDirectory();//+ @"\Reports";

        private bool firstTimeFlag = true;
        bool bcmVersionReceived = false;
        int sendSum = 0;
        byte sendChecksum = 0;
        int autoTSt = 0, autoTcounter = 0, inerTSt = 0, i = 0, j = 0, loadTcounter = 0;
        double OutPutBytes1 = Math.Pow(2, 12);//11
        double OutPutBytes2 = Math.Pow(2, 9);//12
        double OutPutBytes3 = Math.Pow(2, 13);
        double OutPutBytesD = Math.Pow(2, 8);

        //int RecivInputSt = 0;
        double PowerBytes = Math.Pow(2, 22);

        int output1Divid = 1000, output3Divid = 1000;
        Color outputColor;
        Color testGreen = Color.FromArgb(153, 255, 54);
        Color testRed = Color.Red; //FromArgb(235, 141, 161);

        Color connectPanelColor = Color.Transparent;
        //  XtraReport1 report;

        byte[] netReport = new byte[4] { 0, 0, 0, 0 };
        byte[] powerReport = new byte[22];
        string[] powerCurReport = new string[22];
        string[] powerName = new string[22];
        string[] inputName = new string[49];
        string[] outputName = new string[53];
        byte[] inputReport = new byte[49];
        byte[] outputReport = new byte[53];
        string[] outputCurReport = new string[53];
        string[] inputVoltReport = new string[2];

        int MAXPOWERNUM = 22;
        int MAXOUTPUT1 = 12, MAXOUTPUT2 = 9, MAXOUTPUT3 = 13, ALLOUTPUTCNT = 34, LOADTEST1WAIT = 16, LOADTEST2WAIT = 5, LOADTEST2CNT = 3;
        public delegate void UpdateControlsDelegate();
        public delegate void UpdateControlsDelegate2(OutPutParams op);
        public delegate void UpdateControlsDelegate3(int param);

        public delegate void UpdateControlsDel(int st, int loc, Color c);
        public delegate void UpdateControlsDel2(int st, Color c);
        System.Random random = new System.Random();


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
            // HwVer.Text = TestInfoClass.HardWareVersion;
            BCM_SoftwareVer.Text = TestInfoClass.BCM_SoftwareVr;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 800;
            timer.Start();

            timer_SerialCheck.Tick += new EventHandler(timerSerialCheck_Tick);
            timer_SerialCheck.Interval = 1200;

            autoTest_timer.Tick += new EventHandler(autoTest_timer_Tick);
            autoTest_timer.Interval = 500;

            updateUI_timer.Tick += new EventHandler(updateUI_timer_Tick);
            updateUI_timer.Interval = 1300;// 500;

            checkHealth_timer.Tick += new EventHandler(checkHealth_Tick);
            checkHealth_timer.Interval = 1000 * 60;


            NewDay_timer.Tick += new EventHandler(NewDay_timer_Tick);
            NewDay_timer.Interval = 1000 * 24 * 60 * 60;


            serialConnectionCheck();
            initParams();
            if (UserInfoClass.UserName != null)
                userLbl.Text = UserInfoClass.UserName;

            addParamNamesToReport();
            //TestInfoClass.LoadTestSpecTblInDb();
            // MessageBox.Show("System is Checking Boards Health,Please Wait..");
        }

        private void BCMTester_Load(object sender, EventArgs e)
        {
            //  startReport.Checked = true;
            CheckLogin();

            TestInfoClass.LoadTestSpecTblInDb();
            TestInfoClass.LoadOutputsThresholdsFromSqlDatabase();

            if (UserInfoClass.IsLogin)
            {
                btnLogout.Text = " LogOut ";
            }
            else
            {
                btnLogout.Text = " Login ";
            }

            testspecLoadData();
            updateUI_timer_Tick(null, null);
            //
            roundPannelCorner();

            //
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

        }
        private void testspecLoadData()
        {

            //Load
            TestInfoClass.CheckNewDay();
            TestInfoClass.UpdateTrackingSt();
            TestInfoClass.loadReportList();

        }
        private void roundPannelCorner()
        {
            // roundCorner(inputpannel1);

        }

        private int CheckLogin()
        {
            if (UserInfoClass.IsLogin)
            {
                btnLogout.Text = "  LogOut  ";
            }
            else
            {
                btnLogout.Text = "  Login  ";

            }
            if ((UserInfoClass.IsLogin) && (UserInfoClass.SettingPermission))
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
        private void roundCorner(GroupControl gc)
        {
            gc.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, gc.Width + 1,
                     gc.Height + 1, 35, 35));
        }
        #region Timers
        private void timer_Tick(object sender, EventArgs e)
        {
            timeLbl.Text = DateTime.Now.ToString("HH:mm:ss tt");
            if (UserInfoClass.UserName != null)
                userLbl.Text = UserInfoClass.UserName;
            // CheckLogin();   //for Test!!!!!!!!!!!!



        }
        private void timerSerialCheck_Tick(object sender, EventArgs e)
        {
            if (serialManager != null)
                if (!serialManager.IsOpen())
                {
                    timer_SerialCheck.Stop();
                    btnConnect.BackColor = Color.Pink;

                }

        }
        private void updateUI_timer_Tick(object sender, EventArgs e)
        {
            trackingNum.Text = TestInfoClass.TrackingNumSt;
            totalPass.Text = TestInfoClass.TotalTestPass.ToString();
            totalFail.Text = TestInfoClass.TotalTestFail.ToString();
            BCM_SoftwareVer.Text = TestInfoClass.SoftwareVersion;

            CheckLogin();

        }
        private void checkHealth_Tick(object sender, EventArgs e)
        {
            //PowerTestCmd();
            //SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.Welcome);

        }
        private void NewDay_timer_Tick(object sender, EventArgs e)
        {
            TestInfoClass.CheckNewDay();
            TestInfoClass.UpdateTrackingSt();
            TestInfoClass.TotalTestPass = 0;
            TestInfoClass.TotalTestFail = 0;
        }
        int waitforVersion = 7;
        private void autoTest_timer_Tick(object sender, EventArgs e)
        {

            switch (autoTSt)
            {

                case 0:        //init
                    switch (inerTSt)
                    {
                        case 0:
                            bcmPowerSw.Checked = true; ////
                            autoTcounter++;
                            if (autoTcounter > waitforVersion)
                            {
                                inerTSt++;
                                autoTcounter = 0;
                            }

                            break;
                        case 1:
                            swVersionBtn_Click(null, null);
                            inerTSt++;
                            break;
                        case 2:
                            if (bcmVersionReceived)
                            {
                                if ((TestInfoClass.SoftwareVersion == TestInfoClass.SoftwareVersion2))
                                {
                                    //if (TestInfoClass.SoftwareVersion == TestInfoClass.SoftwareVersion2)
                                    //{
                                    BCM_SoftwareVer.Text = TestInfoClass.SoftwareVersion;
                                    updateUI_timer.Stop();
                                    //  feedbackPannel.BackColor = Color.Khaki;
                                    //  tabControl1.SelectedIndex = tabP3;

                                    feedbackTb.Text = " Auto Test Started" + "\r\n";

                                    progressBar1.Visible = true;
                                    progressBar1.Value = 0;

                                    connectPanelColor = connectPanel.BackColor;

                                    connectPanel.BackColor = Color.WhiteSmoke;

                                    //autoTcounter++;
                                    // if (autoTcounter > 7)

                                    //inerTSt = 0;
                                    // autoTSt++;
                                    inerTSt++;

                                }
                                else
                                {
                                    StopAutoTest();
                                    MessageBox.Show(string.Format(" The BCM Software Version differs! Recived version is : {0} while user set version is: {1}", TestInfoClass.SoftwareVersion, TestInfoClass.SoftwareVersion2), "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    // AutoClosingMessageBox.Show(" The BCM Software Version differs from wich you set! Recived version is:{0}", "Attention!", 5000);
                                }
                            }
                            autoTcounter++;
                            if (autoTcounter > 5)
                            {
                                swVersionBtn_Click(null, null);
                            }
                            if (autoTcounter > 40)
                            {
                                StopAutoTest();
                                MessageBox.Show(" The BCM Software Version Not Received");
                                autoTcounter = 0;
                            }

                            break;
                        case 3:
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
                            feedbackTb.Text = "Can_High Test Started..";
                            //TabControl1.SelectedTabPage = netTab;
                            feedbackPannel.BackColor = Color.PapayaWhip;
                            AutoTest.CanHRec = 0;
                            netParams.Mode = 1;
                            netParams.randomNum = 0xaabbccdd;// random.Next();
                            netParams.DataCANH = new byte[5] { netParams.Mode, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                            SendSerialTestCmd(netParams.DataCANH.ToList(), ID.Network_Board, MsType.CANMessage);
                            inerTSt++;
                            break;
                        case 1:
                            if (AutoTest.CanHRec == 1)
                            {
                                feedbackTb.Text = "Can_High Test Done.";
                                inerTSt++;
                            }
                            //else if ((AutoTest.CanHRec == 2) || (autoTcounter > TimingLimit.NetworkTime2))
                            //{

                            //    StopAutoTest();
                            //    //MessageBox.Show("CAN High Is Currupt!  AutoTest Stopped");
                            //    AutoClosingMessageBox.Show(" CAN High Is Currupt!  AutoTest Stopped!", "Attention!", 1000);
                            //}
                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_CANHS, 0);
                                netReport[0] = 0;
                                feedbackTb.Text = "Can_High Not Received!";
                                inerTSt++;

                            }
                            autoTcounter++;
                            break;
                        case 2:
                            AutoTest.NetworkRec = false;
                            feedbackTb.Text = "Can_Low Test Started..";
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
                                feedbackTb.Text = "Can_Low Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                ledBulb_Click(BCM_CANLS, 0);
                                netReport[1] = 0;
                                feedbackTb.Text = "Can_Low Not Received!";
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
                                feedbackTb.Text = "LIN_Front Test Done.";
                                inerTSt++;
                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
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
                                feedbackTb.Text = "LIN_Rear Test Done.";
                                inerTSt = 0;
                                autoTSt++;
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false;
                                progressBar1.Value++;
                            }

                            else if (autoTcounter > TimingLimit.NetworkTime)
                            {
                                feedbackTb.Text = "LIN_Rear Not Received!";
                                inerTSt = 0;
                                autoTSt++;
                                AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false;
                                progressBar1.Value++;
                            }
                            autoTcounter++;

                            break;


                    }

                    break;
                case 2:                 //Power
                    switch (inerTSt)
                    {
                        case 0:
                            if (loadTest2.Checked)
                            {

                                // TabControl1.SelectedTabPage = powerTab;
                                // feedbackPannel.BackColor = Color.Bisque;

                                feedbackTb.Text = "Power Board Test Started..";
                                i = 1;
                                autoTcounter = 0;
                                inerTSt++;
                                progressBar1.Value++;
                            }
                            else
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            break;
                        case 1:
                            powerParams.PowerList.Clear();
                            AutoTest.PowerRec = false;
                            powerParams.PowerList.Add((byte)(i >> 8));
                            powerParams.PowerList.Add((byte)i);

                            SendSerialTestCmd(powerParams.PowerList, ID.Power_Board, MsType.BCMCurrent);
                            inerTSt++;
                            progressBar1.Value++;

                            break;
                        case 2:
                            if (AutoTest.PowerRecev >= MAXPOWERNUM)
                            {
                                autoTSt++;
                                inerTSt = 0;
                                autoTcounter = 0;
                            }
                            if (AutoTest.PowerRec)
                            {
                                feedbackTb.Text = "Power Board Test, Please Wait... ";//+ findRoot(i) + " Rec";
                                autoTcounter = 0;
                                AutoTest.PowerRec = false;
                                progressBar1.Value++;

                            }

                            else if (autoTcounter > TimingLimit.PowerTime)
                            {
                                feedbackTb.Text = "Power Board Current Not Received!";// + findRoot(i);
                                autoTcounter = 0;
                                progressBar1.Value++;
                                autoTSt++;
                                inerTSt = 0;

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
                            inerTSt++;

                            break;
                        case 1:
                            if (loadTest2.Checked)//(loadPreTest.Checked)
                                inerTSt++;
                            else
                            {
                                inerTSt = 4;
                                // autoTSt++;
                            }
                            break;
                        case 2:
                            //resetOutputsFeedback();
                            SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.OutputTest2);
                            inerTSt++;
                            break;

                        case 3:
                            if (AutoTest.LoadTest2B3Rec)
                            {
                                if (AutoTest.LoadTest2Pass == LOADTEST2CNT)
                                {
                                    AutoTest.LoadTest2Passed = true;
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
                                inerTSt++;
                                autoTcounter = 0;
                                loadTcounter = 0;
                            }
                            autoTcounter++;
                            break;

                        case 4:
                            if (loadTest.Checked)
                            {
                                resetOutputsFeedback();
                                inerTSt++;
                            }
                            else
                            {
                                inerTSt = 0;
                                autoTSt++;
                            }
                            break;
                        case 5:
                            SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.OutputTest1);
                            inerTSt++;
                            break;
                        case 6:
                            if (AutoTest.LoadTest1B3Cnt >= MAXOUTPUT3)
                            {
                                if (AutoTest.LoadTest1Pass == ALLOUTPUTCNT)
                                {
                                    AutoTest.LoadTest1Passed = true;
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
                        case 7:
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

                            feedbackTb.Text = "OutPut Board1 Test Started..";
                            i = 1; j = 0;
                            autoTcounter = 0;
                            inerTSt++;
                            progressBar1.Value++;
                            outputParam.OutputList1.Clear();
                            resetOutputsFeedback();

                            break;

                        case 1:
                            outputParam.OutputList1.Clear();
                            AutoTest.Output1Rec = false;
                            if (i < OutPutBytes1)
                            {
                                outputParam.OutputList1.Add((byte)(i >> 8));
                                outputParam.OutputList1.Add((byte)i);
                                //Thread.Sleep(300);
                                SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.SwitchONOff);
                                //var t = thr.outputsTiming1[j];
                                //Thread.Sleep(thr.outputsTiming1[j]);

                                i *= 2;
                                //j++;
                                inerTSt++;
                                progressBar1.Value++;
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;
                                progressBar1.Value++;
                            }

                            break;
                        case 2:
                            if (AutoTest.Output1Rec)
                            {
                                feedbackTb.Text = "OutPut Board1 Current " + findRoot(i) + " Received.";
                                inerTSt--;
                                autoTcounter = 0;
                                progressBar1.Value++;
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board1 Current" + findRoot(i) + "Not Received!";
                                inerTSt--;
                                autoTcounter = 0;
                                progressBar1.Value++;

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
                            progressBar1.Value++;
                            outputParam.OutputList2.Clear();
                            break;

                        case 1:
                            outputParam.OutputList2.Clear();
                            AutoTest.Output2Rec = false;
                            if ((i < OutPutBytes2))
                            {
                                // if (i != 256)
                                {
                                    outputParam.OutputList2.Add((byte)(i >> 8));
                                    outputParam.OutputList2.Add((byte)i);
                                    //Thread.Sleep(250);
                                    SendSerialTestCmd(outputParam.OutputList2, ID.OutputB2, MsType.SwitchONOff);
                                    // Thread.Sleep(thr.outputsTiming2[j++]);

                                    i *= 2;
                                    inerTSt++;
                                    progressBar1.Value++;
                                }
                                //else
                                //{
                                //    i *= 2;

                                //}
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
                                inerTSt--;
                                autoTcounter = 0;
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board2 Current " + findRoot(i) + " Not Received";
                                inerTSt--;
                                autoTcounter = 0;
                            }
                            autoTcounter++;

                            break;
                    }

                    break;
                case 6:         //Output3
                    switch (inerTSt)
                    {
                        case 0:
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

                                i *= 2;

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
                                inerTSt--;
                                autoTcounter = 0;
                            }

                            else if (autoTcounter > TimingLimit.OutputTime)
                            {
                                feedbackTb.Text = "OutPut Board3  Current " + findRoot(i) + " Not Received";
                                inerTSt--;
                                autoTcounter = 0;
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
                            inerTSt++;
                            break;
                        case 1:
                            outputParam.DigitalOutputList.Clear();
                            AutoTest.OutputDRec = false;
                            if (i < 8)   //2^3
                            {
                                outputParam.DigitalOutputList.Add(0x80);
                                outputParam.DigitalOutputList.Add((byte)i);

                                SendSerialTestCmd(outputParam.DigitalOutputList, ID.DigitalOutput, MsType.SwitchONOff);

                                i *= 2;
                                inerTSt = 3;
                            }
                            else
                            {
                                inerTSt = 2;
                                i = 1;// i = 1;  //remove window lock
                                Thread.Sleep(900);

                            }

                            break;
                        case 2:
                            outputParam.DigitalOutputList.Clear();
                            AutoTest.OutputDRec = false;
                            if (i < OutPutBytesD)//2^8=256  //2^7=128
                            {
                                //if (i != 64)
                                {
                                    outputParam.DigitalOutputList.Add((byte)(i >> 8));
                                    outputParam.DigitalOutputList.Add((byte)i);

                                    SendSerialTestCmd(outputParam.DigitalOutputList, ID.DigitalOutput, MsType.SwitchONOff);
                                }
                                i *= 2;

                                inerTSt = 4;
                            }
                            else
                            {
                                autoTSt++;
                                inerTSt = 0;

                            }
                            break;
                        case 3:
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
                                feedbackTb.Text = "Digital OutPut Board Not Received!" + findRoot(i);
                                inerTSt = 1;
                                autoTcounter = 0;


                            }
                            autoTcounter++;
                            break;
                        case 4:
                            if (AutoTest.OutputDRec)
                            {
                                feedbackTb.Text = "Digital OutPut Board Test " + findRoot(i) + " Received";
                                if (autoTcounter > 3)
                                {
                                    autoTcounter = 0;
                                    inerTSt = 2;
                                }
                            }

                            else if (autoTcounter > TimingLimit.DigitalOutputTime)
                            {
                                feedbackTb.Text = "Digital OutPut Board " + findRoot(i) + " Not Received!";
                                inerTSt = 2;
                                autoTcounter = 0;


                            }
                            autoTcounter++;
                            break;
                    }
                    break;
                case 8:       //Input
                    //lbl17.Text = AutoTest.InputRecevd.ToString();
                    if (inputTest.Checked)
                    {

                        switch (inerTSt)
                        {


                            case 0:
                                tabControl1.SelectedTab = inputTab;
                                feedbackPannel.BackColor = Color.MistyRose;
                                inputParam.InputList1.Clear();
                                byte[] inputs1 = { 0x01, 0x8c, 0x59, 0x99, 0x99 };    //89 to 99 byte3
                                inputParam.InputList1.AddRange(inputs1);
                                SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);

                                inerTSt++;
                                feedbackTb.Text = "InPut Board Test Started..";
                                //autoTcounter++;
                                progressBar1.Value++;
                                break;
                            case 1:
                                autoTcounter++;
                                if (autoTcounter > 9)
                                {
                                    inerTSt++;
                                    autoTcounter = 0;
                                }
                                break;
                            //case 2:

                            //    if ((AutoTest.InputRecevd == 5) || (autoTcounter > TimingLimit.InputTime))
                            //    {
                            //        inputParam.InputList1.Clear();
                            //        byte[] inputs5 = { 0x00, 0x00, 0x00, 0x10, 0x00 };  //0x33
                            //        inputParam.InputList1.AddRange(inputs5);
                            //        SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);
                            //        feedbackTb.Text = "InPut1_2 sent";
                            //        inerTSt++;
                            //        autoTcounter = 0;
                            //        progressBar1.Value++;
                            //        AutoTest.InputRecevd = 5;
                            //    }


                            //    autoTcounter++;

                            //    break;
                            //case 3:
                            //    autoTcounter++;
                            //    if (autoTcounter > 10)
                            //    {
                            //        inerTSt++;
                            //        autoTcounter = 0;
                            //    }
                            //    break;
                            //case 4:

                            //    if ((AutoTest.InputRecevd == 6) || (autoTcounter > TimingLimit.InputTime))
                            //    {
                            //        inputParam.InputList1.Clear();
                            //        byte[] inputs5 = { 0x00, 0x00, 0x00, 0x20, 0x00 };  //0x33
                            //        inputParam.InputList1.AddRange(inputs5);
                            //        SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);
                            //        feedbackTb.Text = "InPut1_3 sent";
                            //        inerTSt++;
                            //        autoTcounter = 0;
                            //        progressBar1.Value++;
                            //        AutoTest.InputRecevd = 6;
                            //    }


                            //    autoTcounter++;

                            //    break;
                            //case 5:
                            //    autoTcounter++;
                            //    if (autoTcounter > 10)
                            //    {
                            //        inerTSt++;
                            //        autoTcounter = 0;
                            //    }
                            //    break;
                            case 2:

                                if ((AutoTest.InputRecevd == 1) || (autoTcounter > TimingLimit.InputTime))
                                {
                                    inputParam.InputList1.Clear();
                                    byte[] inputs2 = { 0x00, 0x33, 0x86, 0x66, 0x66 };  //0x46 to 0x66 byte3
                                    inputParam.InputList1.AddRange(inputs2);
                                    SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);
                                    feedbackTb.Text = "InPut1_4 sent";
                                    inerTSt++;
                                    autoTcounter = 0;
                                    progressBar1.Value++;
                                    AutoTest.InputRecevd = 1;
                                }


                                autoTcounter++;

                                break;
                            case 3:
                                autoTcounter++;
                                if (autoTcounter > 10)
                                {
                                    inerTSt++;
                                    autoTcounter = 0;
                                }
                                break;
                            //***

                            //**
                            case 4:
                                if ((AutoTest.InputRecevd == 2) || (autoTcounter > TimingLimit.InputTime))
                                {
                                    inputParam.InputList2.Clear();
                                    byte[] inputs3 = { 00, 00, 00, 0x19, 0x99 };
                                    inputParam.InputList2.AddRange(inputs3);
                                    SendSerialTestCmd(inputParam.InputList2, ID.InputB2, MsType.SwitchONOff);
                                    feedbackTb.Text = "InPut2_1 sent";
                                    inerTSt++;
                                    autoTcounter = 0;
                                    progressBar1.Value++;
                                    AutoTest.InputRecevd = 2;
                                }

                                autoTcounter++;
                                break;
                            case 5:
                                autoTcounter++;
                                if (autoTcounter > 10)
                                {
                                    inerTSt++;
                                    autoTcounter = 0;
                                }
                                break;
                            case 6:
                                if ((AutoTest.InputRecevd == 3) || (autoTcounter > TimingLimit.InputTime))
                                {
                                    inputParam.InputList2.Clear();
                                    byte[] inputs4 = { 00, 00, 00, 0x66, 0x66 };
                                    inputParam.InputList2.AddRange(inputs4);
                                    SendSerialTestCmd(inputParam.InputList2, ID.InputB2, MsType.SwitchONOff);
                                    feedbackTb.Text = "InPut2_2 sent";
                                    inerTSt++;
                                    autoTcounter = 0;
                                    progressBar1.Value++;
                                    AutoTest.InputRecevd = 3;
                                }

                                autoTcounter++;
                                break;

                            case 7:
                                if ((AutoTest.InputRecevd == 4) || (autoTcounter > TimingLimit.InputTime))
                                {
                                    autoTSt++;
                                    feedbackTb.Text = "InPut Board Test Done.";
                                    inerTSt = 0;
                                    progressBar1.Value++;
                                }

                                autoTcounter++;

                                break;

                        }
                    }
                    else
                    {
                        autoTSt++;
                    }

                    break;
                case 9:
                    feedbackTb.Text = "Auto Test Completed.";
                    progressBar1.Value = 100;
                    bcmPowerSw.Checked = false;
                    TestInfoClass.CheckNewDay();
                    TestInfoClass.TrackingNum++;
                    TestInfoClass.UpdateTrackingSt();
                    //updateUI_timer_Tick(null, null);
                    StopAutoTest();
                    //
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;

                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}", //:{2:00}
                        ts.Minutes, ts.Seconds);   //, ts.Milliseconds / 10

                    testTime.Text = elapsedTime + " min";
                    //
                    if ((UserInfoClass.IsLogin) && (UserInfoClass.LogPermission))
                        checkTestAndPrint();
                    else //if(!UserInfoClass.LogPermission)
                         //MessageBox.Show(" You Dont Have Log Permission!");
                        AutoClosingMessageBox.Show(" You Dont Have Log Permission!", "Attention!", 1000);


                    break;
            }
        }
        private void StopAutoTest()
        {
            //StartStopList.Clear();
            //StartStopList.Add(2);
            //SendSerialTestCmd(StartStopList, ID.DigitalOutput, MsType.StartStopCmd);


            autoTest_timer.Stop();
            updateUI_timer.Start();
            tabControl1.SelectedTab = inputTab;
            autoTSt = 0;
            inerTSt = 0;
            autoTcounter = 0;
            PannelLock(false);
            feedbackPannel.BackColor = Color.Transparent;
            // connectPanel.BackColor = connectPanelColor;
            feedbackTb.Text = "";
            progressBar1.Visible = false;
            // CheckSerialBox.Visible = true;
            //  boardsStatusPannel.Visible = true;

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


        private void checkTestAndPrint()
        {
            //startReport.Checked = true;
            startLog_CheckedChanged(null, null);
            bool powerFlag = powerTest.Checked ? true : false;
            report.reportGetData2(netReport, powerReport, powerCurReport, inputReport, outputReport, outputCurReport, inputVoltReport, powerFlag);
            TestMode.Auto = false;
            PannelLock(false);

            TestInfoClass.SaveReport(TestInfoClass.TrackingNumSt, report);

            // if (XtraMessageBox.Show("Do you want to Show Auto Test Result?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            if (startReport.Checked)
            {
                showLogBtn_ItemClick(null, null);    //Dont Show Report Every Test
            }
            bool testPass;
            if (!powerTest.Checked)
            {
                AutoTest.PowerAllPass = true;
            }
            //
            testPass = AutoTest.Output1Pass & AutoTest.Output2Pass & AutoTest.Output3Pass & AutoTest.OutputDPass & AutoTest.NetworkPass;
            if (InputTestToggle.Checked)
                testPass &= AutoTest.InputPass & AutoTest.Input2Pass;
            if (powerTest.Checked)
                testPass &= AutoTest.PowerAllPass;
            if (loadTest1.Checked)
                testPass &= AutoTest.LoadTest1Passed;
            if (loadTest2.Checked)
                testPass &= AutoTest.LoadTest2Passed;
            MessageBox.Show("testPass:" + testPass + " ,output1:" + AutoTest.Output1Pass + ", output2:" + AutoTest.Output2Pass + ", output3:" + AutoTest.Output3Pass + ", Digitaloutput:" + AutoTest.OutputDPass +

             ", Network:" + AutoTest.NetworkPass + ", Input1:" + AutoTest.InputPass + ", Input2:" + AutoTest.Input2Pass);
            //

            if (testPass)
            {
                try
                {
                    AutoTest.AutoTestPass = true;
                    Printer printer = new Printer();
                    TestInfoClass.Barcode = MakeBarcode();
                    var labelText = "EKS Co.";

                    TestInfoClass.TotalTestPass++;

                    printer.TSCPrint(TestInfoClass.Barcode, labelText);
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

                report.ExportToPdf(path2 + @"\Reports\" + $"{TestInfoClass.TrackingNumSt}.pdf", pdfOptions);//
                                                                                                            //Save Report in A Folder
                                                                                                            // XtraMessageBox.Show("Some Tests Failed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private string MakeBarcode()
        {
            string year, deviceCode, changeTurn, designerCode, productDay, productSerial;
            var y = PersianConverterDate.YearToShamsi(DateTime.Now);

            //switch (y)
            //{
            //    case "1403":
            //        year = "R";
            //        break;
            //    case "1402":
            //        year = "P";
            //        break;
            //    case "1401":
            //        year = "N";
            //        break;
            //    case "1400":
            //        year = "M";
            //        break;
            //}

            TestInfoClass.DayNum = PersianConverterDate.DeyNumber(DateTime.Now);
            TestInfoClass.ProductSerial++;

            year = TestInfoClass.Year;
            designerCode = TestInfoClass.DesignerCode;
            deviceCode = TestInfoClass.DeviceCode;
            changeTurn = TestInfoClass.ChangeTurn;
            productDay = TestInfoClass.DayNum.ToString("000");
            productSerial = TestInfoClass.ProductSerial.ToString("0000");    //(++barcodeCounter).ToString("0000");
            string bar = year + deviceCode + changeTurn + designerCode + productDay + productSerial;
            bar = Regex.Replace(bar, @"\s", "");
            return bar;
            //save in database
        }
        #endregion timers
        #region led
        private void ledBulb_Click(object sender)//, EventArgs e
        {
            ((LedBulb)sender).On = !((LedBulb)sender).On;
        }
        private void ledBulb_Click(object sender, byte val)//, EventArgs e
        {
            if (val == 0)
            {
                ((LedBulb)sender).Color = Color.Red;
            }
            else
            {
                ((LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
          ((LedBulb)sender).On = true;

        }
        private void SetOverTempFeedback(TextBox tb, double temp)//, EventArgs e
        {
            envTemp.Text = temp.ToString();

            if (temp > 33)
            {
                tb.BackColor = Color.Red;
                tempPanel.BackColor = Color.Red;

                //XtraMessageBox.Show("Over Temprature!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (temp < 33 & temp > 26)
            {
                tb.BackColor = Color.Orange;
                tempPanel.BackColor = Color.Orange;

            }
            else if (temp > 0 & temp < 33)
            {
                tb.BackColor = Color.Lime;
                tempPanel.BackColor = Color.Lime;
            }
        }
        private void SetInputsFeedback(object sender, byte val, CheckButton c)//, EventArgs e
        {
            if (val == 0)
            {
                ((LedBulb)sender).Color = Color.Red;
                AutoTest.InputFail++;
            }
            else
            {
                ((LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
            ((LedBulb)sender).On = true;

        }

        private void SetInputsFeedbackReverse(object sender, byte val, CheckButton c)//, EventArgs e
        {
            if (val == 1)
            {
                ((LedBulb)sender).Color = Color.Red;
                AutoTest.InputFail++;
            }
            else
            {
                ((LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
            ((LedBulb)sender).On = true;

        }
        private void SetDigitalOutputsFeedback(object sender, byte val)//, EventArgs e
        {
            if (val == 0)
            {
                ((LedBulb)sender).Color = Color.Red;
                AutoTest.DigitalOutputFail++;
            }
            else
            {
                ((LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
            }
         ((LedBulb)sender).On = true;

        }
        private void SetDigitalOutputsFeedbackManual(object sender, byte val)//, EventArgs e
        {
            if (val == 0)
            {
                // ((LedBulb)sender).Color = Color.Red;
                // AutoTest.DigitalOutputFail++;
                ((LedBulb)sender).On = false;
            }
            else
            {
                ((LedBulb)sender).Color = Color.FromArgb(153, 255, 54); // Color.LightGreen;
                ((LedBulb)sender).On = true;
            }


        }
        private void ledBulb_Off(object sender)//, EventArgs e
        {
            ((LedBulb)sender).On = false;
        }
        private void ledBulb7_Click(object sender, EventArgs e)
        {
            if (_blink == 0) _blink = 500;
            else _blink = 0;
            ((LedBulb)sender).Blink(_blink);
        }
        #endregion

        #region RegisterRibbon


        public void WriteToJson(List<string> data)
        {
            string fileName = path + @"\UserInfo.txt";
            if (firstTimeFlag)
            {
                if (File.Exists(fileName))
                {
                    var jsonData = System.IO.File.ReadAllText(path + @"\UserInfo.txt");
                    // De-serialize to object or create new list
                    var employeeList = JsonConvert.DeserializeObject<List<string>>(jsonData) ?? new List<string>();

                    foreach (var item in employeeList)
                    {

                        data.Add(item);
                    }

                    jsonData = JsonConvert.SerializeObject(data.ToArray());
                    System.IO.File.WriteAllText(path + @"\UserInfo.txt", jsonData);
                }
                else
                {

                    var jsonData = JsonConvert.SerializeObject(data.ToArray());
                    System.IO.File.WriteAllText(path + @"\UserInfo.txt", jsonData);
                }
                firstTimeFlag = false;
            }
            else
            {

                var jsonData = JsonConvert.SerializeObject(data.ToArray());
                System.IO.File.WriteAllText(path + @"\UserInfo.txt", jsonData);
            }

        }
        private void Loadfile()
        {
            //UsersList.Add(operatorNameCombo.EditValue?.ToString());
            var read = LoadFromFile();
            if (read != null)
                foreach (var item in read)
                {
                    UsersList.Add(item.ToString());
                    operatorNameCb.Items.Add(item.ToString());

                }
        }

        private List<string> LoadFromFile()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return null;
            }
            // File.SetAttributes(path, FileAttributes.Normal);
            string fileName = path + @"\UserInfo.txt";
            if (File.Exists(fileName))
            {
                using (var file = File.OpenText(Directory.GetCurrentDirectory() + @"\AllUsers\UserInfo.txt"))
                {
                    var serializer = new JsonSerializer();
                    return (List<string>)serializer.Deserialize(file, typeof(List<string>));
                }
            }
            else return null;
        }

        #endregion


        #region Tests
        Setting st = new Setting();
        private void settingBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (st == null || (st.IsDisposed))
                st = new Setting();
            st.Show();


        }
        private void logoutBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UserInfoClass.OpenSettingForm();
        }

        private void resetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AutoTest.ResetResult();
            resetInputs();
            resetInputsFeedback();
            resetOutputs();
            resetOutputsFeedback();
            netParamsReset();
            netParamsResetFeedback();
            powerReset();
            autoTest_timer.Stop();
            reportListsReset();
            StopAutoTest();
            powerParamsFeedback = null;
            TestInfoClass.SoftwareVersion = "";
            TestInfoClass.HardWareVersion = "";
            // HwVer.Text = "";
            SoftwVer.Text = "";


            tempPanel.BackColor = Color.Transparent;
            OverTempLed.BackColor = Color.LightGray;
            bcmVersionReceived = false;
        }
        private void reportListsReset()
        {
            for (int i = 0; i < 22; i++)
            {
                powerReport[i] = 0;
                powerCurReport[i] = "";
            }
            for (int i = 0; i < 4; i++)
                netReport[i] = 0;
            for (int i = 0; i < 48; i++)
            {
                inputReport[i] = 0;
            }
            for (int i = 0; i < 49; i++)
            {
                outputReport[i] = 0;
                outputCurReport[i] = "";
            }
            for (int i = 0; i < 2; i++)
                inputVoltReport[i] = "";

        }
        private void resetInputs()
        {
            AutoTest.InputRec = false;
            AutoTest.InputRecevd = 0;
            AutoTest.Input2Rec = false;
            inputParam.InputList1.Clear();
            inputParam.InputList2.Clear();

            input1.Checked = false;
            input2.Checked = false;
            input3.Checked = false;
            input4.Checked = false;
            input5.Checked = false;
            input6.Checked = false;
            input7.Checked = false;

            input1.Checked = false;
            input2.Checked = false;
            input3.Checked = false;
            input4.Checked = false;
            input5.Checked = false;
            input6.Checked = false;
            input7.Checked = false;
            input8.Checked = false;
            input9.Checked = false;
            input10.Checked = false;
            input11.Checked = false;
            input12.Checked = false;
            input13.Checked = false;
            input14.Checked = false;
            input15.Checked = false;
            input16.Checked = false;
            input17.Checked = false;
            input18.Checked = false;
            input19.Checked = false;
            input20.Checked = false;
            input21.Checked = false;
            input49.Checked = false;
            input23.Checked = false;
            input24.Checked = false;
            input25.Checked = false;
            input26.Checked = false;
            input27.Checked = false;
            input28.Checked = false;
            input29.Checked = false;
            input30.Checked = false;
            input31.Checked = false;
            input32.Checked = false;
            input33.Checked = false;


            //Board2
            input34.Checked = false;
            input35.Checked = false;
            input36.Checked = false;
            input37.Checked = false;
            input38.Checked = false;
            input39.Checked = false;
            input40.Checked = false;
            input41.Checked = false;
            input42.Checked = false;
            input43.Checked = false;
            input44.Checked = false;
            input45.Checked = false;
            input46.Checked = false;
            input47.Checked = false;
            input48.Checked = false;
            input49.Checked = false;


        }
        private void resetInputsFeedback()
        {
            ledBulb_Off(input1Led);
            ledBulb_Off(input2Led);
            ledBulb_Off(input3Led);
            ledBulb_Off(input4Led);
            ledBulb_Off(input5Led);
            ledBulb_Off(input6Led);
            ledBulb_Off(input7Led);
            ledBulb_Off(input8Led);
            ledBulb_Off(input9Led);
            ledBulb_Off(input10Led);
            ledBulb_Off(input11Led);
            ledBulb_Off(input12Led);
            ledBulb_Off(input13Led);
            ledBulb_Off(input14Led);
            ledBulb_Off(input15Led);
            ledBulb_Off(input16Led);
            ledBulb_Off(input17Led);
            ledBulb_Off(input18Led);
            ledBulb_Off(input19Led);
            ledBulb_Off(input20Led);
            ledBulb_Off(input21Led);
            ledBulb_Off(input49Led);
            ledBulb_Off(input23Led);
            ledBulb_Off(input24Led);
            ledBulb_Off(input25Led);
            ledBulb_Off(input26Led);
            ledBulb_Off(input27Led);
            ledBulb_Off(input28Led);
            ledBulb_Off(input29Led);
            ledBulb_Off(input30Led);
            ledBulb_Off(input31Led);
            ledBulb_Off(input32Led);
            ledBulb_Off(input33Led);
            //
            ledBulb_Off(input34Led);
            ledBulb_Off(input35Led);
            ledBulb_Off(input36Led);
            ledBulb_Off(input37Led);
            ledBulb_Off(input38Led);
            ledBulb_Off(input39Led);
            ledBulb_Off(input40Led);
            ledBulb_Off(input41Led);
            ledBulb_Off(input42Led);
            ledBulb_Off(input43Led);
            ledBulb_Off(input44Led);
            ledBulb_Off(input45Led);
            ledBulb_Off(input46Led);
            ledBulb_Off(input47Led);
            ledBulb_Off(input48Led);
            ledBulb_Off(input49Led);
        }
        private void netParamsReset()
        {
            AutoTest.NetworkRec = false;
            //AutoTest.CanHRec = 0;
            CANHS.Checked = false;
            CANLS.Checked = false;
            LinRear.Checked = false;
            LinFront.Checked = false;
        }
        private void netParamsResetFeedback()
        {
            ledBulb_Off(CANHSF);
            ledBulb_Off(CANLSF);
            ledBulb_Off(LinRearF);
            ledBulb_Off(LinFrontF);
        }
        private void powerReset()
        {
            AutoTest.PowerRec = false;
            AutoTest.PowerRecev = 0;

            Color color = Color.WhiteSmoke;
            p1.Text = 0.ToString();
            p2.Text = 0.ToString();
            p3.Text = 0.ToString();
            p4.Text = 0.ToString();
            p5.Text = 0.ToString();
            p6.Text = 0.ToString();
            p7.Text = 0.ToString();
            p8.Text = 0.ToString();
            p9.Text = 0.ToString();
            p10.Text = 0.ToString();
            p11.Text = 0.ToString();
            p12.Text = 0.ToString();
            p13.Text = 0.ToString();
            p14.Text = 0.ToString();
            p15.Text = 0.ToString();
            p16.Text = 0.ToString();
            p17.Text = 0.ToString();
            p18.Text = 0.ToString();
            p19.Text = 0.ToString();
            p20.Text = 0.ToString();
            p21.Text = 0.ToString();
            p22.Text = 0.ToString();

            p1.BackColor = color;
            p2.BackColor = color;
            p3.BackColor = color;
            p4.BackColor = color;
            p5.BackColor = color;
            p6.BackColor = color;
            p7.BackColor = color;
            p8.BackColor = color;
            p9.BackColor = color;
            p10.BackColor = color;
            p11.BackColor = color;
            p12.BackColor = color;
            p13.BackColor = color;
            p14.BackColor = color;
            p15.BackColor = color;
            p16.BackColor = color;
            p17.BackColor = color;
            p18.BackColor = color;
            p19.BackColor = color;
            p20.BackColor = color;
            p21.BackColor = color;
            p22.BackColor = color;

        }
        private void resetOutputs()
        {
            AutoTest.Output1Rec = false;
            AutoTest.Output2Rec = false;
            AutoTest.Output3Rec = false;
            AutoTest.OutputDRec = false;

            outputParam.OutputList1.Clear();
            outputParam.OutputList2.Clear();
            outputParam.OutputList3.Clear();
            outputParam.DigitalOutputList.Clear();


            outOne1.Checked = false;
            outOne2.Checked = false;
            outOne3.Checked = false;
            outOne4.Checked = false;
            outOne5.Checked = false;
            outOne6.Checked = false;
            outOne7.Checked = false;
            outOne8.Checked = false;
            outOne9.Checked = false;
            outOne10.Checked = false;
            outOne11.Checked = false;
            outOne12.Checked = false;


            //B2
            outTwo1.Checked = false;
            outTwo2.Checked = false;
            outTwo3.Checked = false;
            outTwo4.Checked = false;
            outTwo5.Checked = false;
            outTwo6.Checked = false;
            outTwo7.Checked = false;
            outTwo8.Checked = false;
            outTwo9.Checked = false;
            outTwo10.Checked = false;
            outTwo11.Checked = false;

            //B3
            outThree1.Checked = false;
            outThree2.Checked = false;
            outThree3.Checked = false;
            outThree4.Checked = false;
            outThree5.Checked = false;
            outThree6.Checked = false;
            outThree7.Checked = false;
            outThree8.Checked = false;
            outThree9.Checked = false;
            outThree10.Checked = false;
            outThree11.Checked = false;
            outThree12.Checked = false;
            outThree13.Checked = false;
            //DigitalOutput
            outD1.Checked = false;
            outD2.Checked = false;
            outD3.Checked = false;

            outDL1.Checked = false;
            outDL2.Checked = false;
            outDL3.Checked = false;
            outDL4.Checked = false;
            outDL5.Checked = false;
            outDL6.Checked = false;
            outDL7.Checked = false;
            outDL8.Checked = false;
        }
        Color colr = Color.WhiteSmoke;
        private void resetOutputsFeedbackBoard1()
        {

            outOne1C.BackColor = colr;
            outOne2C.BackColor = colr;
            outOne3C.BackColor = colr;
            outOne4C.BackColor = colr;
            outOne5C.BackColor = colr;
            outOne6C.BackColor = colr;
            outOne7C.BackColor = colr;
            outOne8C.BackColor = colr;
            outOne9C.BackColor = colr;
            outOne10C.BackColor = colr;
            outOne11C.BackColor = colr;
            outOne12C.BackColor = colr;
            outOne13C.BackColor = colr;
            outOne14C.BackColor = colr;

            outOne1C.Text = 0.ToString();
            outOne2C.Text = 0.ToString();
            outOne3C.Text = 0.ToString();
            outOne4C.Text = 0.ToString();
            outOne5C.Text = 0.ToString();
            outOne6C.Text = 0.ToString();
            outOne7C.Text = 0.ToString();
            outOne8C.Text = 0.ToString();
            outOne9C.Text = 0.ToString();
            outOne10C.Text = 0.ToString();
            outOne11C.Text = 0.ToString();
            outOne12C.Text = 0.ToString();
            outOne13C.Text = 0.ToString();
            outOne14C.Text = 0.ToString();
        }
        private void resetOutputsFeedbackBoard2()
        {
            outTwo1C.BackColor = colr;
            outTwo2C.BackColor = colr;
            outTwo3C.BackColor = colr;
            outTwo4C.BackColor = colr;
            outTwo5C.BackColor = colr;
            outTwo6C.BackColor = colr;
            outTwo7C.BackColor = colr;
            outTwo8C.BackColor = colr;
            outTwo9C.BackColor = colr;
            outTwo10C.BackColor = colr;
            outTwo11C.BackColor = colr;
            //outTwo12C.BackColor = colr;
            //outTwo13C.BackColor = colr;
            //outTwo14C.BackColor = colr;

            outTwo1C.Text = 0.ToString();
            outTwo2C.Text = 0.ToString();
            outTwo3C.Text = 0.ToString();
            outTwo4C.Text = 0.ToString();
            outTwo5C.Text = 0.ToString();
            outTwo6C.Text = 0.ToString();
            outTwo7C.Text = 0.ToString();
            outTwo8C.Text = 0.ToString();
            outTwo9C.Text = 0.ToString();
            outTwo10C.Text = 0.ToString();
            outTwo11C.Text = 0.ToString();
            //outTwo12C.Text = 0.ToString();
            //outTwo13C.Text = 0.ToString();
            //outTwo14C.Text = 0.ToString();
        }
        private void resetOutputsFeedbackBoard3()
        {
            outThree1C.BackColor = colr;
            outThree2C.BackColor = colr;
            outThree3C.BackColor = colr;
            outThree4C.BackColor = colr;
            outThree5C.BackColor = colr;
            outThree6C.BackColor = colr;
            outThree7C.BackColor = colr;
            outThree8C.BackColor = colr;
            outThree9C.BackColor = colr;
            outThree10C.BackColor = colr;
            outThree11C.BackColor = colr;
            outThree12C.BackColor = colr;
            outThree13C.BackColor = colr;
            outThree14C.BackColor = colr;

            outThree1C.Text = 0.ToString();
            outThree2C.Text = 0.ToString();
            outThree3C.Text = 0.ToString();
            outThree4C.Text = 0.ToString();
            outThree5C.Text = 0.ToString();
            outThree6C.Text = 0.ToString();
            outThree7C.Text = 0.ToString();
            outThree8C.Text = 0.ToString();
            outThree9C.Text = 0.ToString();
            outThree10C.Text = 0.ToString();
            outThree11C.Text = 0.ToString();
            outThree12C.Text = 0.ToString();
            outThree13C.Text = 0.ToString();
            outThree14C.Text = 0.ToString();
        }
        private void resetOutputsFeedbackBoardD()
        {
            ledBulb_Off(outDled1);
            ledBulb_Off(outDled2);
            ledBulb_Off(outDled3);

            ledBulb_Off(outDLled9);
            ledBulb_Off(outDLled2);
            ledBulb_Off(outDLled3);
            ledBulb_Off(outDLled4);
            ledBulb_Off(outDLled5);
            ledBulb_Off(outDLled6);
            ledBulb_Off(outDLled7);
            ledBulb_Off(outDLled8);

            digitalOutputStatus = 0;
        }
        private void resetOutputsFeedback()
        {
            resetOutputsFeedbackBoardD();
            resetOutputsFeedbackBoard1();
            resetOutputsFeedbackBoard2();
            resetOutputsFeedbackBoard3();
        }



        //byte[] autoInput1 = new byte[5] { 0xff, 0xff, 0xff, 0xff, 0xff };
        //byte[] autoInput2 = new byte[5] { 0xff, 0xff, 0xff, 0xff, 0xff };
        byte[] Oauto = new byte[2] { 0xff, 0xff };
        Stopwatch stopWatch;
        private void autoTestBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AutoTestStart();
        }
        private void AutoTestStart()
        {
            resetBtn_ItemClick(null, null);
            AutoTest.ResetResult();
            startReport.Checked = true;
            if (serialManager != null)
                if (serialManager.IsOpen())
                {
                    outputParam.DigitalOutputList.AddRange(Oauto);
                    TestMode.Auto = true;
                    PannelLock(true);
                    autoTcounter = 0;

                    autoTest_timer.Start();
                    //updateUI_timer.Stop();
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                    testTime.Text = "0";
                }
                else
                {
                    //MessageBox.Show("Please Connect First!");
                    AutoClosingMessageBox.Show("Please Connect First!", "Attention!", 1000);


                }
            else
            {
                //MessageBox.Show("Please Connect First!");
                AutoClosingMessageBox.Show(" Please Connect First!", "Attention!", 1000);
            }
        }
        private void SetInputsOnForAutoTest()
        {
            input1.Checked = true;
            input2.Checked = true;
            input3.Checked = true;
            input4.Checked = true;
            input5.Checked = true;
            input6.Checked = true;
            input7.Checked = true;
            input8.Checked = true;
            input9.Checked = true;
            input10.Checked = true;
            input11.Checked = true;
            input12.Checked = true;
            input13.Checked = true;
            input14.Checked = true;
            input15.Checked = true;
            input16.Checked = true;
            input17.Checked = true;
            input18.Checked = true;
            input19.Checked = true;
            input20.Checked = true;
            input21.Checked = true;
            input49.Checked = true;
            input23.Checked = true;
            input24.Checked = true;
            input25.Checked = true;
            input26.Checked = true;
            input27.Checked = true;
            input28.Checked = true;
            input29.Checked = true;
            input30.Checked = true;

            input31.Checked = true;
            input32.Checked = true;
            input33.Checked = true;
            //input2
            input34.Checked = true;
            input35.Checked = true;
            input36.Checked = true;
            input37.Checked = true;
            input38.Checked = true;
            input39.Checked = true;
            input40.Checked = true;
            input41.Checked = true;
            input42.Checked = true;
            input43.Checked = true;
            input44.Checked = true;
            input45.Checked = true;
            input46.Checked = true;
            input47.Checked = true;
        }

        private void PannelLock(bool state)
        {
            if (state)
            {

                inputpannel1.Enabled = false;
                inputpannel2.Enabled = false;
                inputpannel3.Enabled = false;
                outputPannel1.Enabled = false;
                outputPannel2.Enabled = false;
                outputPannel3.Enabled = false;
                outputPannel4.Enabled = false;
                CANBox.Enabled = false;
                linBox.Enabled = false;
                powerPanel1.Enabled = false;
                powerPanel2.Enabled = false;
                outDL8label.ForeColor = Color.Gray;
                powerSwitch.Enabled = false;

            }
            else
            {

                inputpannel1.Enabled = true;
                inputpannel2.Enabled = true;
                inputpannel3.Enabled = true;
                outputPannel1.Enabled = true;
                outputPannel2.Enabled = true;
                outputPannel3.Enabled = true;
                outputPannel4.Enabled = true;
                CANBox.Enabled = true;
                linBox.Enabled = true;
                powerPanel1.Enabled = true;
                powerPanel2.Enabled = true;
                outDL8label.ForeColor = Color.Black;
                powerSwitch.Enabled = true;


            }

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


        //Send Process

        private void startTestBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TestMode.Auto = false;
            TestMode.Manual = true;

            if ((serialManager != null) && (serialManager.IsOpen()))
            {

                if (TabControl1.SelectedTabPage == inputTab)
                {
                    InputTestCmd();
                }
                else if (TabControl1.SelectedTabPage == outputTab)
                {
                    OutputTestCmd();

                }
                if (TabControl1.SelectedTabPage == netTab)
                {
                    NetworkTestCmd();
                }
                if (TabControl1.SelectedTabPage == powerTab)
                {
                    PowerTestCmd();
                }
                //if (loadTest1.Checked)
                //{
                //    loadTest1_Test();
                //}
                //if (loadTest2.Checked)
                //{
                //    loadTest2_Test();
                //}
            }
            else
            {
                //XtraMessageBox.Show(" Please Open The Port ");
                AutoClosingMessageBox.Show("  Please Open The Port", "Attention!", 1000);

            }

            //string author ="V1.1";// "1400/01/22";
            //byte[] bytes = Encoding.ASCII.GetBytes(author);
            //string str = Encoding.ASCII.GetString(bytes);
        }

        private void InputTestCmd()
        {
            InputTestB1Cmd();
            InputTestB2Cmd();
            inputParam.SetInputParamsB1();
            inputParam.SetInputParamsB2();

            foreach (var item in inputParam.InputList1)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.SwitchONOff);
                    break;
                }

            }
            Thread.Sleep(100);
            foreach (var item in inputParam.InputList2)
            {
                if (item > 0)
                {
                    SendSerialTestCmd(inputParam.InputList2, ID.InputB2, MsType.SwitchONOff);
                    break;
                }
            }


            resetInputs();
        }
        private void InputTestB1Cmd()
        {
            //int i = 0;
            //inputParam.b = (byte)(FrontWashPumpSW.Checked ? Math.Pow(2,i++) : 0);
            //inputParam.b += (byte)(FrontHSWiperSW.Checked ? Math.Pow(2, i++) : 0);
            //inputParam.b += (byte)(FrontLSWiperSW.Checked ? Math.Pow(2, i++) : 0);
            //inputParam.b += (byte)(FrontIntWiperSW.Checked ? Math.Pow(2, i++) : 0);
            //inputParam.b += (byte)(FrontWiperZeroPosition.Checked ? Math.Pow(2, i++) : 0);


            //Board1
            inputParam.b0 = (byte)(input1.Checked ? 1 : 0);
            inputParam.b1 = (byte)(input2.Checked ? 1 : 0);
            inputParam.b2 = (byte)(input3.Checked ? 1 : 0);
            inputParam.b3 = (byte)(input4.Checked ? 1 : 0);
            inputParam.b4 = (byte)(input5.Checked ? 1 : 0);
            inputParam.b5 = (byte)(input6.Checked ? 1 : 0);
            inputParam.b6 = (byte)(input7.Checked ? 1 : 0);
            inputParam.b7 = (byte)(input8.Checked ? 1 : 0);
            inputParam.b8 = (byte)(input9.Checked ? 1 : 0);
            inputParam.b9 = (byte)(input10.Checked ? 1 : 0);
            inputParam.b10 = (byte)(input11.Checked ? 1 : 0);
            inputParam.b11 = (byte)(input12.Checked ? 1 : 0);
            inputParam.b12 = (byte)(input13.Checked ? 1 : 0);
            inputParam.b13 = (byte)(input14.Checked ? 1 : 0);
            //if ((inputParam.b12 == 1) || (inputParam.b13 == 1))
            //    inputParam.b9 = 1;
            inputParam.b14 = (byte)(input15.Checked ? 1 : 0);
            inputParam.b15 = (byte)(input16.Checked ? 1 : 0);
            inputParam.b16 = (byte)(input17.Checked ? 1 : 0);
            inputParam.b17 = (byte)(input18.Checked ? 1 : 0);
            inputParam.b18 = (byte)(input19.Checked ? 1 : 0);
            inputParam.b19 = (byte)(input20.Checked ? 1 : 0);
            inputParam.b20 = (byte)(input21.Checked ? 1 : 0);
            inputParam.b21 = (byte)(input49.Checked ? 1 : 0);
            inputParam.b22 = (byte)(input23.Checked ? 1 : 0);
            inputParam.b23 = (byte)(input24.Checked ? 1 : 0);
            inputParam.b24 = (byte)(input25.Checked ? 1 : 0);
            inputParam.b25 = (byte)(input26.Checked ? 1 : 0);
            inputParam.b26 = (byte)(input27.Checked ? 1 : 0);
            inputParam.b27 = (byte)(input28.Checked ? 1 : 0);
            inputParam.b28 = (byte)(input29.Checked ? 1 : 0);
            inputParam.b29 = (byte)(input30.Checked ? 1 : 0);
            //High
            // inputParam.b30 = (byte)(input31.Checked ? 1 : 0);
            inputParam.b31 = (byte)(input32.Checked ? 1 : 0);
            inputParam.b32 = (byte)(input33.Checked ? 1 : 0);



            //reserve 33-35
        }
        private void InputTestB2Cmd()
        {
            //Board2
            inputParam.b30 = (byte)(input31.Checked ? 1 : 0); //Ac Sw
            inputParam.b36 = (byte)(input34.Checked ? 1 : 0);
            inputParam.b37 = (byte)(input35.Checked ? 1 : 0);
            inputParam.b38 = (byte)(input36.Checked ? 1 : 0);
            inputParam.b39 = (byte)(input37.Checked ? 1 : 0);
            inputParam.b40 = (byte)(input38.Checked ? 1 : 0);
            inputParam.b41 = (byte)(input39.Checked ? 1 : 0);
            inputParam.b42 = (byte)(input40.Checked ? 1 : 0);
            inputParam.b43 = (byte)(input41.Checked ? 1 : 0);
            inputParam.b44 = (byte)(input42.Checked ? 1 : 0);
            inputParam.b45 = (byte)(input43.Checked ? 1 : 0);
            inputParam.b46 = (byte)(input44.Checked ? 1 : 0);
            inputParam.b47 = (byte)(input45.Checked ? 1 : 0);
            inputParam.b48 = (byte)(input46.Checked ? 1 : 0);
            inputParam.b49 = (byte)(input47.Checked ? 1 : 0);


        }
        byte hs = 0;
        private void OutputTestCmd()
        {
            OutputTestB1Cmd();
            OutputTestB2Cmd();
            OutputTestB3Cmd();
            OutputTestBDCmd();

            outputParam.SetOutputParamsO1();
            outputParam.SetOutputParamsO2();
            outputParam.SetOutputParamsO3();
            outputParam.SetDigitalOutput(hs);

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
            //resetOutputs();
        }
        private void OutputTestB1Cmd()
        {
            //Board1
            outputParam.b0 = (byte)(outOne1.Checked ? 1 : 0);
            outputParam.b1 = (byte)(outOne2.Checked ? 1 : 0);
            outputParam.b2 = (byte)(outOne3.Checked ? 1 : 0);
            outputParam.b3 = (byte)(outOne4.Checked ? 1 : 0);
            outputParam.b4 = (byte)(outOne5.Checked ? 1 : 0);
            outputParam.b5 = (byte)(outOne6.Checked ? 1 : 0);
            outputParam.b6 = (byte)(outOne7.Checked ? 1 : 0);
            outputParam.b7 = (byte)(outOne8.Checked ? 1 : 0);
            outputParam.b8 = (byte)(outOne9.Checked ? 1 : 0);
            outputParam.b9 = (byte)(outOne10.Checked ? 1 : 0);
            outputParam.b10 = (byte)(outOne11.Checked ? 1 : 0);
            outputParam.b11 = (byte)(outOne12.Checked ? 1 : 0);
            //outputParam.b12 = (byte)(outDL7.Checked ? 1 : 0);
            //outputParam.b13 = (byte)(outDL8.Checked ? 1 : 0);

        }
        private void OutPutSendOneByOne()
        {
            if (outputParam.b0 >= 1)
            {
                SendSerialTestCmd(outputParam.OutputList1, ID.OutputB1, MsType.SwitchONOff);
            }
        }
        private void OutputTestB2Cmd()
        {
            outputParam.b14 = (byte)(outTwo1.Checked ? 1 : 0);
            outputParam.b15 = (byte)(outTwo2.Checked ? 1 : 0);
            outputParam.b16 = (byte)(outTwo3.Checked ? 1 : 0);
            outputParam.b17 = (byte)(outTwo4.Checked ? 1 : 0);
            outputParam.b18 = (byte)(outTwo5.Checked ? 1 : 0);
            outputParam.b19 = (byte)(outTwo6.Checked ? 1 : 0);
            outputParam.b20 = (byte)(outTwo7.Checked ? 1 : 0);
            outputParam.b21 = (byte)(outTwo8.Checked ? 1 : 0);
            outputParam.b22 = (byte)(outTwo9.Checked ? 1 : 0);
            outputParam.b23 = (byte)(outTwo10.Checked ? 1 : 0);
            outputParam.b24 = 0;// (byte)(outTwo11.Checked ? 1 : 0);
            outputParam.b25 = 0;
            outputParam.b26 = 0;
            outputParam.b27 = 0;
        }
        private void OutputTestB3Cmd()
        {
            outputParam.b28 = (byte)(outThree1.Checked ? 1 : 0);
            outputParam.b29 = (byte)(outThree2.Checked ? 1 : 0);
            outputParam.b30 = (byte)(outThree3.Checked ? 1 : 0);
            outputParam.b31 = (byte)(outThree4.Checked ? 1 : 0);
            outputParam.b32 = (byte)(outThree5.Checked ? 1 : 0);
            outputParam.b33 = (byte)(outThree6.Checked ? 1 : 0);
            outputParam.b34 = (byte)(outThree7.Checked ? 1 : 0);
            outputParam.b35 = (byte)(outThree8.Checked ? 1 : 0);
            outputParam.b36 = (byte)(outThree9.Checked ? 1 : 0);
            outputParam.b37 = (byte)(outThree10.Checked ? 1 : 0);
            outputParam.b38 = (byte)(outThree11.Checked ? 1 : 0);
            outputParam.b39 = (byte)(outThree12.Checked ? 1 : 0);
            outputParam.b40 = (byte)(outThree13.Checked ? 1 : 0);
            outputParam.b41 = 0;

            //outputParam.b39 = (byte)(LHFrontMainlamp.Checked ? 1 : 0);
            //outputParam.b40 = (byte)(ACCompressorClutch.Checked ? 1 : 0);
            //outputParam.b41 = (byte)(LHDippedLamp.Checked ? 1 : 0);
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
            outputParam.db11 = 0;
            outputParam.db12 = 0;
            outputParam.db13 = 0;
            outputParam.db14 = 0;
        }
        private void OutputTestBDCmd()
        {
            resetOutputsFeedbackBoardD();
            OutputTestBDReset();
            if (outD1.Checked || outD2.Checked || outD3.Checked)
            {
                hs = 1;
                outputParam.db0 = (byte)(outD1.Checked ? 1 : 0);
                outputParam.db1 = (byte)(outD2.Checked ? 1 : 0);
                outputParam.db2 = (byte)(outD3.Checked ? 1 : 0);

            }



            if (outDL1.Checked || outDL2.Checked || outDL3.Checked || outDL4.Checked || outDL5.Checked || outDL6.Checked || outDL7.Checked || outDL8.Checked)
            {
                hs = 0;
                outputParam.db0 = (byte)(outDL1.Checked ? 1 : 0);
                outputParam.db1 = (byte)(outDL2.Checked ? 1 : 0);
                outputParam.db2 = (byte)(outDL3.Checked ? 1 : 0);
                outputParam.db3 = (byte)(outDL4.Checked ? 1 : 0);
                outputParam.db4 = (byte)(outDL5.Checked ? 1 : 0);
                outputParam.db5 = (byte)(outDL6.Checked ? 1 : 0);
                outputParam.db6 = (byte)(outDL7.Checked ? 1 : 0);
                outputParam.db7 = (byte)(outDL8.Checked ? 1 : 0);

            }
        }
        private void PowerTestCmd()
        {
            //powerParams.PowerList.Add((byte)1);
            SendSerialTestCmd(powerParams.PowerList, ID.Power, MsType.BCMCurrent);

        }
        private void NetworkTestCmd()
        {
            netParams.CANHS = (byte)(CANHS.Checked ? 1 : 0);
            netParams.CANLS = (byte)(CANLS.Checked ? 2 : 0);

            netParams.LinFront = (byte)(LinFront.Checked ? 1 : 0);
            netParams.LinRear = (byte)(LinRear.Checked ? 2 : 0);



            if (netParams.CANHS >= 1)
            {
                netParams.randomNum = 0xaabbccdd;// random.Next();
                netParams.DataCANH = new byte[5] { netParams.CANHS, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                SendSerialTestCmd(netParams.DataCANH.ToList(), ID.Network, MsType.CANMessage);
            }
            if (netParams.CANLS >= 1)
            {
                netParams.randomNum = 0x22334455;// random.Next();
                netParams.DataCANL = new byte[5] { netParams.CANLS, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                SendSerialTestCmd(netParams.DataCANL.ToList(), ID.Network, MsType.CANMessage);
            }
            if (netParams.LinFront >= 1)
            {
                netParams.randomNum = 0x66778899;// random.Next();
                netParams.DataLinF = new byte[5] { netParams.LinFront, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };

                SendSerialTestCmd(netParams.DataLinF.ToList(), ID.Network, MsType.LINMessage);
            }
            if (netParams.LinRear >= 1)
            {
                netParams.randomNum = 0x98765432;// random.Next();
                netParams.DataLinR = new byte[5] { netParams.LinRear, (byte)(netParams.randomNum >> 24 & 0xff), (byte)(netParams.randomNum >> 16 & 0xff), (byte)(netParams.randomNum >> 8 & 0xff), (byte)(netParams.randomNum >> 0 & 0xff) };
                SendSerialTestCmd(netParams.DataLinR.ToList(), ID.Network, MsType.LINMessage);
            }
            //Analog Input
            AnalogInput.p = 0;//  (byte)(A1.Checked ? 1 : 0);
            AnalogInput.p += 0;//(byte)(A2.Checked ? 2 : 0);
            AnalogInput.p += 0;//(byte)(A3.Checked ? 4 : 0);
            AnalogInput.p += 0;//(byte)(A4.Checked ? 8 : 0);
            AnalogInput.AnInputList.Add(AnalogInput.p);
            if (AnalogInput.p > 0)
            {
                SendSerialTestCmd(AnalogInput.AnInputList, ID.Network, MsType.BCMAnalogInputStatus);
            }

            //
            //netParamsReset();
        }



        #endregion


        #region OutputPannel



        #endregion


        #region connection
        string detectedPort;
        private void serialConnectionCheck()
        {
            try
            {
                comPortCombo.Items.Clear();
                string[] ArrayComPortsNames = null;
                List<string> ArrayComPortsName = new List<string>();
                ArrayComPortsNames = SerialPort.GetPortNames();
                ArrayComPortsName = SerialPort.GetPortNames().Distinct().ToList();

                if (ArrayComPortsName.Count > 0)
                {
                    foreach (var item in ArrayComPortsName)
                    {
                        comPortCombo.Items.Add(item);

                    }
                    comPortCb.EditValue = ArrayComPortsNames[0];
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
            testbuf[0] = 0x11;
            testbuf[1] = 0xAA;
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

                        //testSerial.SerialManagerDataReceive += (sndr, args) =>
                        //{
                        if (testSerial.IncomingBytes != null)
                            if ((testSerial.IncomingBytes[0] == 0x11) & (testSerial.IncomingBytes[1] == 0xAA))
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
                        //if (testSerial.IncomingBytes != null)
                        //WelcomCheck(testSerial.IncomingBytes[2]);
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
                comPortCb.EditValue = com;
                serialManager = new SerialManager(com.ToString(), 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One, false);
                serialManager.OpenPort();
                if (serialManager.IsOpen())
                {
                    serialManager.SerialManagerDataReceive += ReadSerialEventHispeed;
                    connectBtn.Appearance.BackColor = Color.MediumSpringGreen;
                    timer_SerialCheck.Start();
                    updateUI_timer.Start();
                    // checkHealth_timer.Start();
                    CheckSerialBox.BackColor = Color.Lime;
                    connectPanel.BackColor = Color.Lime;
                    SerialConnection.Text = comPortCb.EditValue.ToString();
                    connectBtn.Enabled = false;
                    swVersionBtn_Click(null, null);


                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                AutoClosingMessageBox.Show(ex.ToString(), "Warning!", 2000);

            }
        }
        private void connectBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var com = comPortCb.EditValue;
                comPortCb.EditValue = com;
                serialManager = new SerialManager(com.ToString(), int.Parse(baudRateCb.EditValue.ToString()), System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One, false);

                serialManager.OpenPort();
                if (serialManager.IsOpen())
                {
                    connectBtn.Enabled = false;
                    serialManager.SerialManagerDataReceive += ReadSerialEventHispeed;
                    connectBtn.Appearance.BackColor = Color.MediumSpringGreen;
                    timer_SerialCheck.Start();
                    updateUI_timer.Start();
                    CheckSerialBox.BackColor = Color.Lime;   //MediumSpringGreen;
                    connectPanel.BackColor = Color.Lime;
                    // this.pictureBox1.Image = global::MainProject.Properties.Resources.tara2;
                    SerialConnection.Text = comPortCb.EditValue.ToString();
                    //welcome
                    inputParam.InputList1.Clear();
                    SendSerialTestCmd(inputParam.InputList1, ID.InputB1, MsType.Welcome);

                    //checkHealth_timer.Start();

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                AutoClosingMessageBox.Show(ex.ToString(), "Warning!", 2000);
            }
        }

        private void refreshBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (serialManager != null && (serialManager.IsOpen()))
            {
                serialManager.ClosePort();
            }
            conectedCom = null;
            timer_SerialCheck.Stop();
            updateUI_timer.Stop();
            comPortCombo.Items.Clear();
            comPortCb.EditValue = "";
            connectBtn.Appearance.BackColor = Color.Transparent;
            CheckSerialBox.BackColor = Color.LightGray;
            connectPanel.BackColor = Color.Transparent;
            ResetWelcom();
            connectBtn.Enabled = true;
            serialConnectionCheck();

            // checkHealth_timer.Stop();
            //this.pictureBox1.Image = global::MainProject.Properties.Resources.tara4;

        }
        #endregion Connection
        //Read********************************************
        private void ShowPacket()
        {
            packetLabel.Text = PacketView.data;
        }
        private void ReadSerialEventHispeed(object sender, EventArgs e)
        {
            var serialString = serialManager.IncomingBytes;
            if ((serialString == null))
                return;
            serialManager.IncomingBytes = null;
            var dataList = new List<List<byte>>();


            serialManager.ExctractDataFromSerialByte(dataList, serialString);


            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateControlsDelegate(ShowPacket));
            }
            else
            {
                ShowPacket();
            }



            IncomingSerialDataDetachment(dataList);
        }
        int loadStatus, loadStatus2, loadLoac;
        bool powerShortFlag = false;
        int powerShortParam;
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
                                SetInputBoardCurrent(InputBoard.Input1, readData[0].ToArray());           //?
                                break;
                            case MsType.Welcome:
                                WelcomExtract(readData[0][0]);
                                if (InvokeRequired)
                                    Invoke(new UpdateControlsDelegate(WelcomCheck));
                                else
                                    WelcomCheck();
                                break;

                        }

                        break;
                    case ID.InputB2:
                        switch (message)
                        {
                            case MsType.Current:
                                SetInputBoardCurrent(InputBoard.Input2, readData[0].ToArray());
                                break;
                        }
                        break;
                    case ID.OutputB1:
                        switch (message)
                        {
                            case MsType.Current:
                                outputFeedback = new OutPutParams();
                                Parsers.ReceiveParser.ExctractOutputData(readData[0].ToArray(), outputFeedback, output1Divid);
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
                                Parsers.ReceiveParser.ExctractOutputData(readData[0].ToArray(), outputFeedback, output1Divid);
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
                                Parsers.ReceiveParser.ExctractOutputData(readData[0].ToArray(), outputFeedback, output3Divid);
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
                                Parsers.ReceiveParser.ExctractOutputDataDigitalOutputBoard(readData[0].ToArray(), digitalOutputFeedback);
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
                                        this.Invoke(new UpdateControlsDelegate(StopAutoTest));
                                    }
                                    else
                                    {
                                        StopAutoTest();
                                    }
                                    //XtraMessageBox.Show("Auto Test Stopped By User");
                                    powerSwitch.Checked = false;
                                    feedbackTb.Text += "Auto Test Stopped By User";


                                }
                                break;
                        }
                        break;
                    case ID.Power:
                        //powerShortFlag = false;??
                        switch (message)
                        {
                            case MsType.BCMCurrent:
                                powerParamsFeedback = new PowerParams();
                                Parsers.ReceiveParser.ExctractPowerData(readData[0].ToArray(), powerParamsFeedback);
                                AutoTest.PowerRec = true;
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate(UpdateUIPowerElement));
                                }
                                else
                                {
                                    UpdateUIPowerElement();
                                }
                                break;
                            case MsType.Temp:
                                powerParamsFeedback = new PowerParams();
                                var temp = readData[0][1] + (readData[0][0] << 8);// BitConverter.ToInt16(readData[0].ToArray(), 0);
                                powerParamsFeedback.temprature = temp / 10.0;
                                break;
                            case MsType.PowerShort:
                                powerShortParam = readData[0][0];

                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new UpdateControlsDelegate3(PowerShortState), powerShortParam);
                                }
                                else
                                {
                                    PowerShortState(powerShortParam);
                                }
                                break;
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
                    case ID.Network:
                        switch (message)
                        {
                            case MsType.CANMessage:
                                netParamsFeedback = new NetParams();
                                Parsers.ReceiveParser.ExctractNetDataCan(readData[0].ToArray(), netParamsFeedback);
                                SetNetFeedBacksCan();
                                break;
                            case MsType.LINMessage:
                                netParamsFeedback = new NetParams();
                                Parsers.ReceiveParser.ExctractNetDataLin(readData[0].ToArray(), netParamsFeedback);
                                SetNetFeedBacksLin();
                                break;
                            case MsType.BCMAnalogInputStatus:  //?

                                Parsers.ReceiveParser.ExctractAnalogInputData(readData[0].ToArray());
                                SetAnalogInputFeedBacks();
                                break;
                            case MsType.BCMInputStatus:
                                inputParamFeedback = new InputParams();
                                Parsers.ReceiveParser.ExctractInputDataInputBoard1(readData[0].ToArray(), inputParamFeedback);
                                SetInputCheckB1(inputParamFeedback);
                                //SetInputFeedBacksBoard1(inputParamFeedback);
                                if (TestMode.Auto)
                                    AutoTest.InputRec = true;
                                break;

                            case MsType.BCM_Version:
                                var bcmVersion = readData[0][1] + (readData[0][0] << 8);
                                //data = new byte[2];
                                //data[0] = readData[0][1];
                                //data[1] = readData[0][0];

                                string s = readData[0][0].ToString("X") + "." + readData[0][1].ToString("X"); //Encoding.ASCII.GetString(data);
                                TestInfoClass.SoftwareVersion = s;// bcmVersion.ToString();
                                bcmVersionReceived = true;
                                break;

                        }
                        if (TestMode.Auto)
                            AutoTest.NetworkRec = true;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void SetInputBoardCurrent(InputBoard BoardNum, byte[] data)
        {
            //byte Mode;
            //Mode = data[0];         //?
            int currentHigh = data[1] + (data[0] << 8);
            int currentLow = data[3] + (data[2] << 8);
            switch (BoardNum)
            {
                case InputBoard.Input1:
                    currentHigh = currentHigh / 1;
                    currentLow = currentLow / 1;

                    //p15.Text = current.ToString();
                    //if (current > 300)
                    //    p15.BackColor = Color.Red;
                    break;
                case InputBoard.Input2:
                    currentHigh = currentHigh / 1;
                    currentLow = currentLow / 1;

                    //p16.Text = current.ToString();
                    //if (current > 300)
                    //    p16.BackColor = Color.Red;
                    break;
            }
        }

        byte InputB1St;
        byte InputB2St;
        byte OutputB1St;
        byte OutputB2St;
        byte OutputB3St;
        byte DOutputBSt;
        byte NetworkBSt;
        byte PowerBSt;
        Color welcomeColor = Color.LightGray;
        private void ResetWelcom()
        {
            InputB1St = 0;
            InputB2St = 0;
            OutputB1St = 0;
            OutputB2St = 0;
            OutputB3St = 0;
            DOutputBSt = 0;
            NetworkBSt = 0;
            PowerBSt = 0;
            inpB1.BackColor = welcomeColor;
            inpB2.BackColor = welcomeColor;
            outB1.BackColor = welcomeColor;
            outB2.BackColor = welcomeColor;
            outB3.BackColor = welcomeColor;
            dOut.BackColor = welcomeColor;
            netB.BackColor = welcomeColor;
            powB.BackColor = welcomeColor;
        }
        private void WelcomExtract(byte data)
        {
            InputB1St = (byte)(data & 0x1);
            InputB2St = (byte)(data >> 1 & 0x1);
            OutputB1St = (byte)(data >> 2 & 0x1);
            OutputB2St = (byte)(data >> 3 & 0x1);
            OutputB3St = (byte)(data >> 4 & 0x1);
            DOutputBSt = (byte)(data >> 5 & 0x1);
            NetworkBSt = (byte)(data >> 6 & 0x1);
            PowerBSt = (byte)(data >> 7 & 0x1);

            // MessageBox.Show("System is Checking Boards Health,Please Wait..", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Hand);

        }
        private void WelcomCheck()
        {
            inpB1.BackColor = InputB1St == 1 ? testGreen : testRed;
            inpB2.BackColor = InputB2St == 1 ? testGreen : testRed;
            outB1.BackColor = OutputB1St == 1 ? testGreen : testRed;
            outB2.BackColor = OutputB2St == 1 ? testGreen : testRed;
            outB3.BackColor = OutputB3St == 1 ? testGreen : testRed;
            dOut.BackColor = DOutputBSt == 1 ? testGreen : testRed;
            netB.BackColor = NetworkBSt == 1 ? testGreen : testRed;
            powB.BackColor = PowerBSt == 1 ? testGreen : testRed;

            PannelLock(false);
        }
        private void SetInputCheckB1(InputParams data)
        {
            if (TestMode.Auto)
            {
                switch (AutoTest.InputRecevd)
                {
                    case 0:
                        SetInputFeedBacksBoard1(data);
                        AutoTest.InputRecevd = 1;

                        break;
                    //case 5:
                    //    SetInputFeedBacksBoard1TwoParam(data);

                    //    AutoTest.InputRecevd = 6;

                    //    break;
                    //case 6:
                    //    SetInputFeedBacksBoard1TwoParam2(data);

                    //    AutoTest.InputRecevd = 1;

                    //    break;
                    case 1:
                        SetInputFeedBacksBoard1part2(data);
                        AutoTest.InputRecevd = 2;

                        break;
                    case 2:
                        SetInputFeedBacksBoard2(data);
                        AutoTest.InputRecevd = 3;

                        break;
                    case 3:
                        SetInputFeedBacksBoard2part2(data);
                        AutoTest.InputRecevd = 4;

                        break;
                }
            }
            else
            {
                SetInputFeedBacksBoard1(data);
                SetInputFeedBacksBoard2(data);
                // SetInputFeedBacksBoard1TwoParam(data);
                // SetInputFeedBacksBoard1TwoParam2(data);
                SetInputFeedBacksBoard1part2(data);
                SetInputFeedBacksBoard2part2(data);
            }


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
        public void SetInputFeedBacksBoard1TwoParam(InputParams data)
        {
            AutoTest.InputFail = 0;
            SetInputsFeedback(input13Led, data.b13, input13); addInputReport(12, data.b13);
            //SetInputsFeedback(input14Led, data.b14, input14); addInputReport(13, data.b14);
            //SetInputsFeedback(input10Led, data.b10, input10); addInputReport(9, data.b10);

        }
        public void SetInputFeedBacksBoard1TwoParam2(InputParams data)
        {
            AutoTest.InputFail = 0;
            //SetInputsFeedback(input13Led, data.b13, input13); addInputReport(12, data.b13);
            SetInputsFeedback(input14Led, data.b14, input14); addInputReport(13, data.b14);
            //SetInputsFeedback(input10Led, data.b10, input10); addInputReport(9, data.b10);

        }
        public void SetInputFeedBacksBoard1(InputParams data)
        {
            AutoTest.InputFail = 0;
            SetInputsFeedback(input1Led, data.b1, input1); addInputReport(0, data.b1);
            SetInputsFeedback(input4Led, data.b4, input4); addInputReport(3, data.b4);
            SetInputsFeedbackReverse(input5Led, data.b5, input5); addInputReportReverse(4, data.b5);
            SetInputsFeedback(input8Led, data.b8, input8); addInputReport(7, data.b8);
            SetInputsFeedback(input9Led, data.b9, input9); addInputReport(8, data.b9);
            SetInputsFeedback(input12Led, data.b12, input12); addInputReport(11, data.b12);
            SetInputsFeedback(input13Led, data.b13, input13); addInputReport(12, data.b13);//
            SetInputsFeedback(input16Led, data.b16, input16); addInputReport(15, data.b16);
            SetInputsFeedback(input17Led, data.b17, input17); addInputReport(16, data.b17);
            SetInputsFeedback(input20Led, data.b20, input20); addInputReport(19, data.b20);
            SetInputsFeedback(input21Led, data.b21, input21); addInputReport(20, data.b21);
            SetInputsFeedback(input23Led, data.b23, input23); addInputReport(22, data.b23);
            SetInputsFeedback(input27Led, data.b27, input27); addInputReport(26, data.b27);
            SetInputsFeedback(input28Led, data.b28, input28); addInputReport(27, data.b28);

            SetInputsFeedback(input32Led, data.b32, input32); addInputReport(31, data.b32);
            SetInputsFeedback(input33Led, data.b33, input33); addInputReport(32, data.b33);

            //SetInputsFeedbackReverse(input49Led, data.b22, input49); addInputReportReverse(21, data.b22);   //?////Ambient Temp


            if (AutoTest.InputFail == 0)
            {
                AutoTest.InputPass = true;
            }
        }
        public void SetInputFeedBacksBoard1part2(InputParams data)
        {
            int input1Fail = AutoTest.InputFail;
            AutoTest.InputFail = 0;
            SetInputsFeedback(input2Led, data.b2, input2); addInputReport(1, data.b2);
            SetInputsFeedback(input3Led, data.b3, input3); addInputReport(2, data.b3);
            SetInputsFeedback(input6Led, data.b6, input6); addInputReport(5, data.b6);
            SetInputsFeedback(input7Led, data.b7, input7); addInputReport(6, data.b7);
            SetInputsFeedback(input10Led, data.b10, input10); addInputReport(9, data.b10);
            SetInputsFeedback(input11Led, data.b11, input11); addInputReport(10, data.b11);
            SetInputsFeedback(input14Led, data.b14, input14); addInputReport(13, data.b14);//
            SetInputsFeedback(input15Led, data.b15, input15); addInputReport(14, data.b15);
            SetInputsFeedback(input18Led, data.b18, input18); addInputReport(17, data.b18);
            SetInputsFeedback(input19Led, data.b19, input19); addInputReport(18, data.b19);
            SetInputsFeedback(input24Led, data.b24, input24); addInputReport(23, data.b24);
            SetInputsFeedback(input25Led, data.b25, input25); addInputReport(24, data.b25);
            SetInputsFeedback(input26Led, data.b26, input26); addInputReport(25, data.b26);
            SetInputsFeedback(input29Led, data.b29, input29); addInputReport(28, data.b29);
            SetInputsFeedback(input30Led, data.b30, input30); addInputReport(29, data.b30);


            if ((AutoTest.InputFail == 0) && (input1Fail == 0))
            {
                AutoTest.InputPass = true;
            }
        }
        public void SetInputFeedBacksBoard2(InputParams data)
        {
            AutoTest.InputFail = 0;
            SetInputsFeedback(input34Led, data.b34, input34); addInputReport(33, data.b34);
            SetInputsFeedback(input37Led, data.b37, input37); addInputReport(36, data.b37);
            SetInputsFeedback(input38Led, data.b38, input38); addInputReport(37, data.b38);
            SetInputsFeedback(input41Led, data.b41, input41); addInputReport(40, data.b41);
            SetInputsFeedback(input42Led, data.b42, input42); addInputReport(41, data.b42);
            SetInputsFeedback(input45Led, data.b45, input45); addInputReport(44, data.b45);
            SetInputsFeedback(input46Led, data.b46, input46); addInputReport(45, data.b46);


            if (AutoTest.InputFail == 0)
            {
                AutoTest.Input2Pass = true;
            }
        }
        public void SetInputFeedBacksBoard2part2(InputParams data)
        {
            int input2Fail = AutoTest.InputFail;
            AutoTest.InputFail = 0;

            SetInputsFeedback(input31Led, data.b31, input31); addInputReport(30, data.b31);
            SetInputsFeedback(input35Led, data.b35, input35); addInputReport(34, data.b35);
            SetInputsFeedback(input36Led, data.b36, input36); addInputReport(35, data.b36);
            SetInputsFeedback(input39Led, data.b39, input39); addInputReport(38, data.b39);
            SetInputsFeedback(input40Led, data.b40, input40); addInputReport(39, data.b40);
            SetInputsFeedback(input43Led, data.b43, input43); addInputReport(42, data.b43);
            SetInputsFeedback(input44Led, data.b44, input44); addInputReport(43, data.b44);
            SetInputsFeedbackReverse(input47Led, data.b47, input47); addInputReportReverse(46, data.b47);    //true

            if ((AutoTest.InputFail == 0) && (input2Fail == 0))
            {
                AutoTest.Input2Pass = true;
            }
        }
        public void SetInputFeedBacksBoard1Manual(InputParams data)
        {
            if (inputParam.b0 == 1)
                SetInputsFeedback(input1Led, data.b20, input1);
            if (inputParam.b1 == 1)
                SetInputsFeedback(input2Led, data.b18, input2);
            if (inputParam.b2 == 1)
                SetInputsFeedback(input3Led, data.b17, input3);
            if (inputParam.b3 == 1)
                SetInputsFeedback(input4Led, data.b19, input4);
            if (inputParam.b4 == 1)
                SetInputsFeedback(input5Led, data.b21, input5);
            if (inputParam.b5 == 1)
                SetInputsFeedback(input6Led, data.b5, input6);//?
            if (inputParam.b6 == 1)
                SetInputsFeedback(input7Led, data.b23, input7);
            if (inputParam.b7 == 1)
                SetInputsFeedback(input8Led, data.b35, input8);
            if (inputParam.b8 == 1)
                SetInputsFeedback(input9Led, data.b40, input9);
            if (inputParam.b9 == 1)
                SetInputsFeedback(input10Led, data.b48, input10);
            if (inputParam.b10 == 1)
                SetInputsFeedback(input11Led, data.b47, input11);
            if (inputParam.b11 == 1)
                SetInputsFeedback(input12Led, data.b46, input12);
            if (inputParam.b12 == 1)
                SetInputsFeedback(input13Led, data.b45, input13);
            if (inputParam.b13 == 1)
                SetInputsFeedback(input14Led, data.b44, input14);
            if (inputParam.b14 == 1)
                SetInputsFeedback(input15Led, data.b43, input15);
            if (inputParam.b15 == 1)
                SetInputsFeedback(input16Led, data.b42, input16);
            if (inputParam.b16 == 1)
                SetInputsFeedback(input17Led, data.b41, input17);
            if (inputParam.b17 == 1)
                SetInputsFeedback(input18Led, data.b37, input18);
            if (inputParam.b18 == 1)
                SetInputsFeedback(input19Led, data.b28, input19);
            if (inputParam.b19 == 1)
                SetInputsFeedback(input20Led, data.b27, input20);
            if (inputParam.b20 == 1)
                SetInputsFeedback(input21Led, data.b26, input21);
            if (inputParam.b21 == 1)
                SetInputsFeedback(input49Led, data.b21, input49);//?
            if (inputParam.b22 == 1)
                SetInputsFeedback(input23Led, data.b8, input23);
            if (inputParam.b23 == 1)
                SetInputsFeedback(input24Led, data.b9, input24);
            if (inputParam.b24 == 1)
                SetInputsFeedback(input25Led, data.b55, input25);
            if (inputParam.b25 == 1)
                SetInputsFeedback(input26Led, data.b53, input26);
            if (inputParam.b26 == 1)
                SetInputsFeedback(input27Led, data.b56, input27);
            if (inputParam.b27 == 1)
                SetInputsFeedback(input28Led, data.b54, input28);
            if (inputParam.b28 == 1)
                SetInputsFeedback(input29Led, data.b51, input29);
            if (inputParam.b29 == 1)
                SetInputsFeedback(input30Led, data.b49, input30);
            if (inputParam.b30 == 1)


                SetInputsFeedback(input31Led, data.b13, input31);
            if (inputParam.b31 == 1)
                SetInputsFeedback(input32Led, data.b12, input32);
            if (inputParam.b32 == 1)
                SetInputsFeedback(input33Led, data.b38, input33);
            //
            if (inputParam.b36 == 1)
                SetInputsFeedback(input34Led, data.b52, input34);
            if (inputParam.b37 == 1)
                SetInputsFeedback(input35Led, data.b50, input35);
            if (inputParam.b38 == 1)
                SetInputsFeedback(input36Led, data.b34, input36);
            if (inputParam.b39 == 1)
                SetInputsFeedback(input37Led, data.b33, input37);
            if (inputParam.b40 == 1)
                SetInputsFeedback(input38Led, data.b32, input38);
            if (inputParam.b41 == 1)
                SetInputsFeedback(input39Led, data.b31, input39);
            if (inputParam.b42 == 1)
                SetInputsFeedback(input40Led, data.b29, input40);
            if (inputParam.b43 == 1)
                SetInputsFeedback(input41Led, data.b30, input41);
            if (inputParam.b44 == 1)
                SetInputsFeedback(input42Led, data.b44, input42);//?
            if (inputParam.b45 == 1)
                SetInputsFeedback(input43Led, data.b14, input43);
            if (inputParam.b46 == 1)
                SetInputsFeedback(input44Led, data.b24, input44);
            if (inputParam.b47 == 1)
                SetInputsFeedback(input45Led, data.b11, input45);
            if (inputParam.b48 == 1)
                SetInputsFeedback(input46Led, data.b7, input46);
            if (inputParam.b49 == 1)
                SetInputsFeedback(input47Led, data.b10, input47);
        }


        public void SetInputFeedBacksBoard2Manual(InputParams data)
        {
            if (inputParam.b36 == 1)
                SetInputsFeedback(input34Led, data.b36, input34);
            if (inputParam.b37 == 1)
                SetInputsFeedback(input35Led, data.b37, input35);
            if (inputParam.b38 == 1)
                SetInputsFeedback(input36Led, data.b38, input36);
            if (inputParam.b39 == 1)
                SetInputsFeedback(input37Led, data.b39, input37);
            if (inputParam.b40 == 1)
                SetInputsFeedback(input38Led, data.b40, input38);
            if (inputParam.b41 == 1)
                SetInputsFeedback(input39Led, data.b41, input39);
            if (inputParam.b42 == 1)
                SetInputsFeedback(input40Led, data.b42, input40);
            if (inputParam.b43 == 1)
                SetInputsFeedback(input41Led, data.b43, input41);
            if (inputParam.b44 == 1)
                SetInputsFeedback(input42Led, data.b44, input42);
            if (inputParam.b45 == 1)
                SetInputsFeedback(input43Led, data.b45, input43);
            if (inputParam.b46 == 1)
                SetInputsFeedback(input44Led, data.b46, input44);
            if (inputParam.b47 == 1)
                SetInputsFeedback(input45Led, data.b47, input45);
            if (inputParam.b48 == 1)
                SetInputsFeedback(input46Led, data.b48, input46);
            if (inputParam.b49 == 1)
                SetInputsFeedback(input47Led, data.b49, input47);
        }
        public void SetAnalogInputFeedBacks()
        {
            if (AnalogInput.InputNumber < AnalogInput.Data.Length)
            {
                AnalogInput.Data[AnalogInput.InputNumber] = AnalogInput.Voltage;
                inputVoltReport[0] = AnalogInput.Data[1].ToString();


            }
        }



        private void addParamNamesToReport()
        {
            powerName[0] = power1.Text;
            powerName[1] = power2.Text;
            powerName[2] = power3.Text;
            powerName[3] = power4.Text;
            powerName[4] = power5.Text;
            powerName[5] = power6.Text;
            powerName[6] = power7.Text;
            powerName[7] = power8.Text;
            powerName[8] = power9.Text;
            powerName[9] = power10.Text;
            powerName[10] = power11.Text;
            powerName[11] = power12.Text;
            powerName[12] = power13.Text;
            powerName[13] = power14.Text;
            powerName[14] = bpower1.Text;
            powerName[15] = bpower2.Text;
            powerName[16] = bpower3.Text;
            powerName[17] = bpower4.Text;
            powerName[18] = bpower5.Text;
            powerName[19] = bpower6.Text;
            powerName[20] = bpower7.Text;
            powerName[21] = bpower8.Text;
            //
            inputName[0] = input1.Text;
            inputName[1] = input2.Text;
            inputName[2] = input3.Text;
            inputName[3] = input4.Text;
            inputName[4] = input5.Text;
            inputName[5] = input6.Text;
            inputName[6] = input7.Text;
            inputName[7] = input8.Text;
            inputName[8] = input9.Text;
            inputName[9] = input10.Text;
            inputName[10] = input11.Text;
            inputName[11] = input12.Text;
            inputName[12] = input13.Text;
            inputName[13] = input14.Text;
            inputName[14] = input15.Text;
            inputName[15] = input16.Text;
            inputName[16] = input17.Text;
            inputName[17] = input18.Text;
            inputName[18] = input19.Text;
            inputName[19] = input20.Text;
            inputName[20] = input21.Text;
            inputName[21] = input23.Text;       //input49.Text;
            inputName[22] = input24.Text;
            inputName[23] = input25.Text;
            inputName[24] = input26.Text;
            inputName[25] = input27.Text;
            inputName[26] = input28.Text;
            inputName[27] = input29.Text;
            inputName[28] = input30.Text;
            inputName[29] = input31.Text;
            inputName[30] = input32.Text;
            inputName[31] = input33.Text;
            inputName[32] = input34.Text;
            inputName[33] = input35.Text;

            inputName[34] = input36.Text;
            inputName[35] = input37.Text;
            inputName[36] = input38.Text;
            inputName[37] = input39.Text;
            inputName[38] = input40.Text;
            inputName[39] = input41.Text;
            inputName[40] = input42.Text;
            inputName[41] = input43.Text;
            inputName[42] = input44.Text;
            inputName[43] = input45.Text;
            inputName[44] = input46.Text;
            inputName[45] = input47.Text;
            inputName[46] = input48.Text;
            inputName[47] = input49.Text;

            //
            int j = 0;
            outputName[j++] = outOne1.Text;
            outputName[j++] = outOne2.Text;
            outputName[j++] = outOne3.Text;
            outputName[j++] = outOne4.Text;
            outputName[j++] = outOne5.Text;
            outputName[j++] = outOne6.Text;
            outputName[j++] = outOne7.Text;
            outputName[j++] = outOne8.Text;
            outputName[j++] = outOne9.Text;
            outputName[j++] = outOne10.Text;
            outputName[j++] = outOne11.Text;
            outputName[j++] = outOne12.Text;
            //outputName[12] = "";
            //outputName[13] = "";

            outputName[j++] = outTwo1.Text;
            outputName[j++] = outTwo2.Text;
            outputName[j++] = outTwo3.Text;
            outputName[j++] = outTwo4.Text;
            outputName[j++] = outTwo5.Text;
            outputName[j++] = outTwo6.Text;
            outputName[j++] = outTwo7.Text;
            outputName[j++] = outTwo8.Text;
            outputName[j++] = outTwo9.Text;
            // outputName[j++] = outTwo10.Text;
            // outputName[j++] = outTwo11.Text;
            //outputName[25] ="";
            //outputName[26] ="";
            //outputName[27] = "";

            outputName[j++] = outThree1.Text;
            outputName[j++] = outThree2.Text;
            outputName[j++] = outThree3.Text;
            outputName[j++] = outThree4.Text;
            outputName[j++] = outThree5.Text;
            outputName[j++] = outThree6.Text;
            outputName[j++] = outThree7.Text;
            outputName[j++] = outThree8.Text;
            outputName[j++] = outThree9.Text;
            outputName[j++] = outThree10.Text;
            outputName[j++] = outThree11.Text;
            outputName[j++] = outThree12.Text;
            outputName[j++] = outThree13.Text;
            //outputName[40] = "";
            //outputName[41] = "";


            outputName[j++] = outD1.Text;
            outputName[j++] = outD2.Text;
            outputName[j++] = outD3.Text;

            // outputName[j++] = outDL1.Text;
            outputName[j++] = outDL2.Text;
            outputName[j++] = outDL3.Text;
            outputName[j++] = outDL4.Text;
            outputName[j++] = outDL5.Text;
            outputName[j++] = outDL6.Text;
            outputName[j++] = outDL7.Text;
            outputName[j++] = outDL8.Text;


        }
        private void addOutputReport(int i, string current)
        {
            outputCurReport[i] = current;

            if (AutoTest.OutputPass == false)

                outputReport[i] = 0;
            else
                outputReport[i] = 1;
        }
        private void addPowerFailReport(SimpleButton c, int i, string current)
        {
            powerCurReport[i] = current;

            if (AutoTest.PowerPass == false)

                powerReport[i] = 0;
            else
                powerReport[i] = 1;
        }
        private void addPowerFailReport2(SimpleButton c, int i, string current)
        {
            powerCurReport[i] = "";// current;

            powerReport[i] = 0;

        }
        Color loadsError = Color.Orange;
        Color loadsErrorT2 = Color.OrangeRed;
        //Color loadsPass = Color.LightCyan;
        private void LoadTest1B1(int status, int loc, Color loadsPass)
        {
            switch (loc)
            {
                case 1:
                    if ((status >> 0 & 0x1) == 1)
                    {
                        outOne1C.BackColor = loadsError;

                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne1C.BackColor = loadsPass;
                    }

                    break;
                case 2:
                    if ((status >> 1 & 0x1) == 1)
                    {
                        outOne2C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne2C.BackColor = loadsPass;
                    }

                    break;
                case 4:
                    if ((status >> 2 & 0x1) == 1)
                    {
                        outOne3C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne3C.BackColor = loadsPass;
                    }

                    break;
                case 8:
                    if ((status >> 3 & 0x1) == 1)
                    {
                        outOne4C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne4C.BackColor = loadsPass;
                    }
                    break;
                case 16:
                    if ((status >> 4 & 0x1) == 1)
                    {
                        outOne5C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne5C.BackColor = loadsPass;
                    }
                    break;
                case 32:
                    if ((status >> 5 & 0x1) == 1)
                    {
                        outOne6C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne6C.BackColor = loadsPass;
                    }
                    break;
                case 64:
                    if ((status >> 6 & 0x1) == 1)
                    {
                        outOne7C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne7C.BackColor = loadsPass;
                    }
                    break;
                case 128:
                    if ((status >> 7 & 0x1) == 1)
                    {
                        outOne8C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne8C.BackColor = loadsPass;
                    }
                    break;
                case 256:
                    if ((status >> 8 & 0x1) == 1)
                    {
                        outOne9C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne9C.BackColor = loadsPass;
                    }
                    break;
                case 512:
                    if ((status >> 9 & 0x1) == 1)
                    {
                        outOne10C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne10C.BackColor = loadsPass;
                    }
                    break;
                case 1024:
                    if ((status >> 10 & 0x1) == 1)
                    {
                        outOne11C.BackColor = loadsPass;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne11C.BackColor = loadsPass;
                    }
                    break;
                case 2048:
                    if ((status >> 11 & 0x1) == 1)
                    {
                        outOne12C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outOne12C.BackColor = loadsPass;
                    }
                    break;
                case 4096:
                    //if ((status >> 12 & 0x1) == 1)
                    //{
                    //    outOne13C.BackColor = loadsError;
                    //}
                    //else
                    //{
                    //    AutoTest.LoadTestPass++;
                    //    outOne13C.BackColor = loadsPass;
                    //}
                    break;
                case 8192:

                    break;
            }
        }
        private void LoadTest2B1(int status, Color loadsPass)
        {
            if (status == 0) AutoTest.LoadTest2Pass++;

            if ((status >> 0 & 0x1) == 1) outOne1C.BackColor = loadsErrorT2; else outOne1C.BackColor = loadsPass;
            if ((status >> 1 & 0x1) == 1) outOne2C.BackColor = loadsErrorT2; else outOne2C.BackColor = loadsPass;
            if ((status >> 2 & 0x1) == 1) outOne3C.BackColor = loadsErrorT2; else outOne3C.BackColor = loadsPass;
            if ((status >> 3 & 0x1) == 1) outOne4C.BackColor = loadsErrorT2; else outOne4C.BackColor = loadsPass;
            if ((status >> 4 & 0x1) == 1) outOne5C.BackColor = loadsErrorT2; else outOne5C.BackColor = loadsPass;
            if ((status >> 5 & 0x1) == 1) outOne6C.BackColor = loadsErrorT2; else outOne6C.BackColor = loadsPass;
            if ((status >> 6 & 0x1) == 1) outOne7C.BackColor = loadsErrorT2; else outOne7C.BackColor = loadsPass;
            if ((status >> 7 & 0x1) == 1) outOne8C.BackColor = loadsErrorT2; else outOne8C.BackColor = loadsPass;
            if ((status >> 8 & 0x1) == 1) outOne9C.BackColor = loadsErrorT2; else outOne9C.BackColor = loadsPass;
            if ((status >> 9 & 0x1) == 1) outOne10C.BackColor = loadsErrorT2; else outOne10C.BackColor = loadsPass;
            if ((status >> 10 & 0x1) == 1) outOne11C.BackColor = loadsPass; else outOne11C.BackColor = loadsPass;   //Eco Relay Dosnt Turn Off so we Pass it Maqnually
            if ((status >> 11 & 0x1) == 1) outOne12C.BackColor = loadsErrorT2; else outOne12C.BackColor = loadsPass;
        }
        //*****************B2**************************************
        private void LoadTest1B2(int status, int loc, Color loadsPass)
        {
            switch (loc)
            {
                case 1:
                    if ((status >> 0 & 0x1) == 1)
                    {
                        outTwo1C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo1C.BackColor = loadsPass;
                    }

                    break;
                case 2:
                    if ((status >> 1 & 0x1) == 1)
                    {
                        outTwo2C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo2C.BackColor = loadsPass;
                    }
                    break;
                case 4:
                    if ((status >> 2 & 0x1) == 1)
                    {
                        outTwo3C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo3C.BackColor = loadsPass;
                    }
                    break;
                case 8:
                    if ((status >> 3 & 0x1) == 1)
                    {
                        outTwo4C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo4C.BackColor = loadsPass;
                    }
                    break;
                case 16:
                    if ((status >> 4 & 0x1) == 1)
                    {
                        outTwo5C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo5C.BackColor = loadsPass;
                    }
                    break;
                case 32:
                    if ((status >> 5 & 0x1) == 1)
                    {
                        outTwo6C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo6C.BackColor = loadsPass;
                    }
                    break;
                case 64:
                    if ((status >> 6 & 0x1) == 1)
                    {
                        outTwo7C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo7C.BackColor = loadsPass;
                    }
                    break;
                case 128:
                    if ((status >> 7 & 0x1) == 1)
                    {
                        outTwo8C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo8C.BackColor = loadsPass;
                    }
                    break;
                case 256:
                    if ((status >> 8 & 0x1) == 1)
                    {
                        outTwo9C.BackColor = loadsPass;// loadsError;// because Alternator isnt load
                        AutoTest.LoadTest1Pass++; //added
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outTwo9C.BackColor = loadsPass;


                    }
                    break;
                case 512:
                    //if ((status >> 9 & 0x1) == 1)
                    //{
                    //    outTwo10C.BackColor = loadsError;


                    //}
                    //else
                    //{
                    //    AutoTest.LoadTest1Pass++;
                    //    outTwo10C.BackColor = loadsPass;


                    //}
                    break;
                case 1024:
                    //if ((status >> 10 & 0x1) == 1)
                    //{
                    //    outTwo11C.BackColor = loadsError;
                    //}
                    //else
                    //{
                    //    AutoTest.LoadTest1Pass++;
                    //    outTwo11C.BackColor = loadsPass;
                    //}
                    break;
                case 2048:

                    break;
                case 4096:

                    break;
                case 8192:

                    break;
            }
        }
        private void LoadTest2B2(int status, Color loadsPass)
        {
            if (status == 0) AutoTest.LoadTest2Pass++;
            if ((status >> 0 & 0x1) == 1) outTwo1C.BackColor = loadsErrorT2; else outTwo1C.BackColor = loadsPass;
            if ((status >> 1 & 0x1) == 1) outTwo2C.BackColor = loadsErrorT2; else outTwo2C.BackColor = loadsPass;
            if ((status >> 2 & 0x1) == 1) outTwo3C.BackColor = loadsErrorT2; else outTwo3C.BackColor = loadsPass;
            if ((status >> 3 & 0x1) == 1) outTwo4C.BackColor = loadsErrorT2; else outTwo4C.BackColor = loadsPass;
            if ((status >> 4 & 0x1) == 1) outTwo5C.BackColor = loadsErrorT2; else outTwo5C.BackColor = loadsPass;
            if ((status >> 5 & 0x1) == 1) outTwo6C.BackColor = loadsErrorT2; else outTwo6C.BackColor = loadsPass;
            if ((status >> 6 & 0x1) == 1) outTwo7C.BackColor = loadsErrorT2; else outTwo7C.BackColor = loadsPass;
            if ((status >> 7 & 0x1) == 1) outTwo8C.BackColor = loadsErrorT2; else outTwo8C.BackColor = loadsPass;
            if ((status >> 8 & 0x1) == 1) outTwo9C.BackColor = loadsPass; else outTwo9C.BackColor = loadsPass;   //because alternatore isnt load
                                                                                                                 //if ((status >> 9 & 0x1) == 1) outTwo10C.BackColor = loadsErrorT2; else outTwo10C.BackColor = loadsPass;
                                                                                                                 //if ((status >> 10 & 0x1) == 1) outTwo11C.BackColor = loadsErrorT2; else outTwo11C.BackColor = loadsPass;

        }
        //***********************B3***********************************************
        private void LoadTest1B3(int status, int loc, Color loadsPass)
        {
            switch (loc)
            {
                case 1:
                    if ((status >> 0 & 0x1) == 1)
                    {
                        outThree1C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree1C.BackColor = loadsPass;
                    }
                    break;
                case 2:
                    if ((status >> 1 & 0x1) == 1)
                    {
                        outThree2C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree2C.BackColor = loadsPass;
                    }
                    break;
                case 4:
                    if ((status >> 2 & 0x1) == 1)
                    {
                        outThree3C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree3C.BackColor = loadsPass;
                    }
                    break;
                case 8:
                    if ((status >> 3 & 0x1) == 1)
                    {
                        outThree4C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree4C.BackColor = loadsPass;
                    }
                    break;
                case 16:
                    if ((status >> 4 & 0x1) == 1)
                    {
                        outThree5C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree5C.BackColor = loadsPass;
                    }
                    break;
                case 32:
                    if ((status >> 5 & 0x1) == 1)
                    {
                        outThree6C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree6C.BackColor = loadsPass;
                    }
                    break;
                case 64:
                    if ((status >> 6 & 0x1) == 1)
                    {
                        outThree7C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree7C.BackColor = loadsPass;
                    }
                    break;
                case 128:
                    if ((status >> 7 & 0x1) == 1)
                    {
                        outThree8C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree8C.BackColor = loadsPass;
                    }
                    break;
                case 256:
                    if ((status >> 8 & 0x1) == 1)
                    {
                        outThree9C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree9C.BackColor = loadsPass;
                    }
                    break;
                case 512:
                    if ((status >> 9 & 0x1) == 1)
                    {
                        outThree10C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree10C.BackColor = loadsPass;
                    }
                    break;
                case 1024:
                    if ((status >> 10 & 0x1) == 1)
                    {
                        outThree11C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree11C.BackColor = loadsPass;
                    }
                    break;
                case 2048:
                    if ((status >> 11 & 0x1) == 1)
                    {
                        outThree12C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree12C.BackColor = loadsPass;
                    }
                    break;
                case 4096:
                    if ((status >> 12 & 0x1) == 1)
                    {
                        outThree13C.BackColor = loadsError;
                    }
                    else
                    {
                        AutoTest.LoadTest1Pass++;
                        outThree13C.BackColor = loadsPass;
                    }
                    break;
                case 8192:

                    break;
            }
        }
        private void LoadTest2B3(int status, Color loadsPass)
        {
            if (status == 0) AutoTest.LoadTest2Pass++;
            if ((status >> 0 & 0x1) == 1) outThree1C.BackColor = loadsErrorT2; else outThree1C.BackColor = loadsPass;
            if ((status >> 1 & 0x1) == 1) outThree2C.BackColor = loadsErrorT2; else outThree2C.BackColor = loadsPass;
            if ((status >> 2 & 0x1) == 1) outThree3C.BackColor = loadsErrorT2; else outThree3C.BackColor = loadsPass;
            if ((status >> 3 & 0x1) == 1) outThree4C.BackColor = loadsErrorT2; else outThree4C.BackColor = loadsPass;
            if ((status >> 4 & 0x1) == 1) outThree5C.BackColor = loadsErrorT2; else outThree5C.BackColor = loadsPass;
            if ((status >> 5 & 0x1) == 1) outThree6C.BackColor = loadsErrorT2; else outThree6C.BackColor = loadsPass;
            if ((status >> 6 & 0x1) == 1) outThree7C.BackColor = loadsErrorT2; else outThree7C.BackColor = loadsPass;
            if ((status >> 7 & 0x1) == 1) outThree8C.BackColor = loadsErrorT2; else outThree8C.BackColor = loadsPass;
            if ((status >> 8 & 0x1) == 1) outThree9C.BackColor = loadsErrorT2; else outThree9C.BackColor = loadsPass;
            if ((status >> 9 & 0x1) == 1) outThree10C.BackColor = loadsErrorT2; else outThree10C.BackColor = loadsPass;
            if ((status >> 10 & 0x1) == 1) outThree11C.BackColor = loadsErrorT2; else outThree11C.BackColor = loadsPass;
            if ((status >> 11 & 0x1) == 1) outThree12C.BackColor = loadsErrorT2; else outThree12C.BackColor = loadsPass;
            if ((status >> 12 & 0x1) == 1) outThree13C.BackColor = loadsErrorT2; else outThree13C.BackColor = loadsPass;
            AutoTest.LoadTest2B3Rec = true;
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

            //if (AutoTest.Output1Passed == output1Count)
            //{
            //    AutoTest.Output1Pass = true;
            //}

        }
        public void SetOutputFeedBacksBoard1()
        {
            string current = string.Format("{0:F1} ", outputFeedback.current);
            //TestMode.Auto = true;
            switch (outputFeedback.outputNumber)
            {
                case 1:

                    if ((outputParam.b0 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne1Mx, thr.outOne1);
                        outOne1C.Text = current;
                        outOne1C.BackColor = outputColor;
                        addOutputReport(0, current);
                        //addOutputFailReport(outOne1);
                    }
                    break;
                case 2:
                    if ((outputParam.b1 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne2Mx, thr.outOne2);
                        outOne2C.Text = current;
                        outOne2C.BackColor = outputColor;
                        addOutputReport(1, current);
                        //addOutputFailReport(outOne2);
                    }
                    break;
                case 3:
                    if ((outputParam.b2 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne3Mx, thr.outOne3);
                        outOne3C.Text = current;
                        outOne3C.BackColor = outputColor;
                        addOutputReport(2, current);
                        //addOutputFailReport(outOne3);
                    }
                    break;
                case 4:
                    if ((outputParam.b3 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne4Mx, thr.outOne4);
                        outOne4C.Text = current;
                        outOne4C.BackColor = outputColor;
                        addOutputReport(3, current);
                        //addOutputFailReport(outOne4);
                    }
                    break;
                case 5:
                    if ((outputParam.b4 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne5Mx, thr.outOne5);
                        outOne5C.Text = current;
                        outOne5C.BackColor = outputColor;
                        addOutputReport(4, current);
                        //addOutputFailReport(outOne5);
                    }
                    break;
                case 6:
                    if ((outputParam.b5 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne6Mx, thr.outOne6);
                        outOne6C.Text = current;
                        outOne6C.BackColor = outputColor;
                        addOutputReport(5, current);

                    }
                    break;
                case 7:
                    if ((outputParam.b6 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne7Mx, thr.outOne7);
                        outOne7C.Text = current;
                        outOne7C.BackColor = outputColor;
                        addOutputReport(6, current);

                    }
                    break;
                case 8:
                    if ((outputParam.b7 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne8Mx, thr.outOne8);
                        outOne8C.Text = current;
                        outOne8C.BackColor = outputColor;
                        addOutputReport(7, current);
                    }
                    break;
                case 9:
                    if ((outputParam.b8 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne9Mx, thr.outOne9);
                        outOne9C.Text = current;
                        outOne9C.BackColor = outputColor;
                        addOutputReport(8, current);
                    }
                    break;
                case 10:
                    if ((outputParam.b9 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne10Mx, thr.outOne10);
                        outOne10C.Text = current;
                        outOne10C.BackColor = outputColor;
                        addOutputReport(9, current);
                    }
                    break;
                case 11:
                    if ((outputParam.b10 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne11Mx, thr.outOne11);
                        outOne11C.Text = current;
                        outOne11C.BackColor = outputColor;
                        addOutputReport(10, current);
                    }
                    break;
                case 12:
                    if ((outputParam.b11 == 1) || (TestMode.Auto))
                    {
                        CheckOutput(thr.outOne12Mx, thr.outOne12);
                        outOne12C.Text = current;
                        outOne12C.BackColor = outputColor;
                        addOutputReport(11, current);

                        if (AutoTest.Output1Failed == 0)
                            AutoTest.Output1Pass = true;

                    }
                    break;
                    //case 13:
                    //    if ((outputParam.b12 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput(thr.outOne13);
                    //        outOne13C.Text = current;
                    //        outOne13C.BackColor = outputColor;
                    //        addOutputReport(12, current);
                    //    }
                    //    break;
                    //case 14:
                    //    if ((outputParam.b13 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput(thr.outOne14);
                    //        outOne14C.Text = current;
                    //        outOne14C.BackColor = outputColor;
                    //        addOutputFailReport(outOne13);
                    //    }
                    //    break;
            }

            resetOutputs();

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
                        outTwo1C.Text = current;
                        outTwo1C.BackColor = outputColor;
                        addOutputReport(12, current);
                        //addOutputFailReport(outTwo1);
                    }
                    break;
                case 2:
                    if ((outputParam.b15 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo2Mx, thr.outTwo2);
                        outTwo2C.Text = current;
                        outTwo2C.BackColor = outputColor;
                        addOutputReport(13, current);
                        //addOutputFailReport(outTwo2);
                    }
                    break;
                case 3:
                    if ((outputParam.b16 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo3Mx, thr.outTwo3);
                        outTwo3C.Text = current;
                        outTwo3C.BackColor = outputColor;
                        addOutputReport(14, current);
                    }
                    break;
                case 4:
                    if ((outputParam.b17 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo4Mx, thr.outTwo4);
                        outTwo4C.Text = current;
                        outTwo4C.BackColor = outputColor;
                        addOutputReport(15, current);
                    }
                    break;
                case 5:
                    if ((outputParam.b18 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo5Mx, thr.outTwo5);
                        outTwo5C.Text = current;
                        outTwo5C.BackColor = outputColor;
                        addOutputReport(16, current);
                    }
                    break;
                case 6:
                    if ((outputParam.b19 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo6Mx, thr.outTwo6);
                        outTwo6C.Text = current;
                        outTwo6C.BackColor = outputColor;
                        addOutputReport(17, current);
                    }
                    break;
                case 7:
                    if ((outputParam.b20 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo7Mx, thr.outTwo7);
                        outTwo7C.Text = current;
                        outTwo7C.BackColor = outputColor;
                        addOutputReport(18, current);
                    }
                    break;
                case 8:
                    if ((outputParam.b21 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo8Mx, thr.outTwo8);
                        outTwo8C.Text = current;
                        outTwo8C.BackColor = outputColor;
                        addOutputReport(19, current);
                    }
                    break;
                case 9:
                    if ((outputParam.b22 == 1) || (TestMode.Auto))
                    {
                        CheckOutput2(thr.outTwo9Mx, thr.outTwo9);
                        outTwo9C.Text = current;
                        outTwo9C.BackColor = outputColor;
                        addOutputReport(20, current);
                        if (AutoTest.Output2Failed == 0)
                        {
                            AutoTest.Output2Pass = true;
                        }
                    }
                    break;
                    //case 10:
                    //    if ((outputParam.b23 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput2(thr.outTwo10Mx, thr.outTwo10);
                    //        outTwo10C.Text = current;
                    //        outTwo10C.BackColor = outputColor;
                    //        // addOutputReport(21, current);
                    //    }
                    //    break;
                    //case 11:
                    //    if ((outputParam.b24 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput2(thr.outTwo11Mx, thr.outTwo11);
                    //        outTwo11C.Text = current;
                    //        outTwo11C.BackColor = outputColor;
                    //        // addOutputReport(21, current);
                    //        //
                    //        if (AutoTest.Output2Failed == 0)
                    //        {
                    //            AutoTest.Output2Pass = true;
                    //        }
                    //    }
                    //  break;
                    //case 12:
                    //    if ((outputParam.b25 == 1) || (TestMode.Auto))
                    //    {
                    //        outTwo12C.Text = current;
                    //        outTwo12C.BackColor = outputColor;
                    //        addOutputFailReport(outTwo12);
                    //    }
                    //    break;
                    //case 13:
                    //    if ((outputParam.b26 == 1) || (TestMode.Auto))
                    //    {
                    //        outTwo13C.Text = current;
                    //        outTwo13C.BackColor = outputColor;
                    //        addOutputFailReport(outTwo13);
                    //    }
                    //    break;
                    //case 14:
                    //    if ((outputParam.b27 == 1) || (TestMode.Auto))
                    //    {
                    //        outTwo14C.Text = current;
                    //        outTwo14C.BackColor = outputColor;
                    //        addOutputFailReport(outTwo14);
                    //    }
                    //    break;
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

            //if (AutoTest.Output3Passed == 14)
            //{
            //    AutoTest.Output3Pass = true;
            //}
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
                        outThree1C.Text = current;
                        outThree1C.BackColor = outputColor;
                        addOutputReport(21, current);
                        //addOutputFailReport(outThree1);
                    }
                    break;
                case 2:
                    if ((outputParam.b29 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree2Mx, thr.outThree2);
                        outThree2C.Text = current;
                        outThree2C.BackColor = outputColor;
                        addOutputReport(22, current);
                    }
                    break;
                case 3:
                    if ((outputParam.b30 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree3Mx, thr.outThree3);
                        outThree3C.Text = current;
                        outThree3C.BackColor = outputColor;
                        addOutputReport(23, current);
                    }
                    break;
                case 4:
                    if ((outputParam.b31 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree4Mx, thr.outThree4);
                        outThree4C.Text = current;
                        outThree4C.BackColor = outputColor;
                        addOutputReport(24, current);
                    }
                    break;
                case 5:
                    if ((outputParam.b32 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree5Mx, thr.outThree5);
                        outThree5C.Text = current;
                        outThree5C.BackColor = outputColor;
                        addOutputReport(25, current);
                    }
                    break;
                case 6:
                    if ((outputParam.b33 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree6Mx, thr.outThree6);
                        outThree6C.Text = current;
                        outThree6C.BackColor = outputColor;
                        addOutputReport(26, current);
                    }
                    break;
                case 7:
                    if ((outputParam.b34 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree7Mx, thr.outThree7);
                        outThree7C.Text = current;
                        outThree7C.BackColor = outputColor;
                        addOutputReport(27, current);
                    }
                    break;
                case 8:
                    if ((outputParam.b35 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree8Mx, thr.outThree8);
                        outThree8C.Text = current;
                        outThree8C.BackColor = outputColor;
                        addOutputReport(28, current);
                    }
                    break;
                case 9:
                    if ((outputParam.b36 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree9Mx, thr.outThree9);
                        outThree9C.Text = current;
                        outThree9C.BackColor = outputColor;
                        addOutputReport(29, current);
                    }
                    break;
                case 10:
                    if ((outputParam.b37 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree10Mx, thr.outThree10);
                        outThree10C.Text = current;
                        outThree10C.BackColor = outputColor;
                        addOutputReport(30, current);
                    }
                    break;
                case 11:
                    if ((outputParam.b38 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree11Mx, thr.outThree11);
                        outThree11C.Text = current;
                        outThree11C.BackColor = outputColor;
                        addOutputReport(31, current);
                    }
                    break;
                case 12:
                    if ((outputParam.b39 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree12Mx, thr.outThree12);
                        outThree12C.Text = current;
                        outThree12C.BackColor = outputColor;
                        addOutputReport(32, current);
                        //
                        if (AutoTest.Output3Failed == 0)
                        {
                            AutoTest.Output3Pass = true;
                        }
                    }
                    break;
                case 13:
                    if ((outputParam.b40 == 1) || (TestMode.Auto))
                    {
                        CheckOutput3(thr.outThree13Mx, thr.outThree13);
                        outThree13C.Text = current;
                        outThree13C.BackColor = outputColor;
                        addOutputReport(33, current);
                        if (AutoTest.Output3Failed == 0)
                        {
                            AutoTest.Output3Pass = true;
                        }
                    }
                    break;
                    //case 14:
                    //    if ((outputParam.b41 == 1) || (TestMode.Auto))
                    //    {
                    //        CheckOutput3(thr.outThree14);
                    //        outThree14C.Text = current;
                    //        outThree14C.BackColor = outputColor;
                    //        addOutputReport(37, current);
                    //    }
                    //    break;

            }
            resetOutputs();

        }

        private void addDigitalOutputReport(int i, int val)
        {
            outputReport[i] = (byte)val;
        }
        public void SetDigitalOutputFeedBacksBoard()
        {
            SetDigitalOutputsFeedbackManual(outDled1, digitalOutputFeedback.db0);    //addDigitalOutputReport(38, digitalOutputFeedback.db0);
            SetDigitalOutputsFeedbackManual(outDled2, digitalOutputFeedback.db1);    // addDigitalOutputReport(39, digitalOutputFeedback.db1);
            SetDigitalOutputsFeedbackManual(outDled3, digitalOutputFeedback.db2);    // addDigitalOutputReport(40, digitalOutputFeedback.db2);

            SetDigitalOutputsFeedbackManual(outDLled9, digitalOutputFeedback.db15);   //addDigitalOutputReport(41, digitalOutputFeedback.db15);
            SetDigitalOutputsFeedbackManual(outDLled2, digitalOutputFeedback.db16);   //addDigitalOutputReport(42, digitalOutputFeedback.db16);
            SetDigitalOutputsFeedbackManual(outDLled3, digitalOutputFeedback.db17);   //addDigitalOutputReport(43, digitalOutputFeedback.db17);
            SetDigitalOutputsFeedbackManual(outDLled4, digitalOutputFeedback.db18);   //addDigitalOutputReport(44, digitalOutputFeedback.db18);
            SetDigitalOutputsFeedbackManual(outDLled5, digitalOutputFeedback.db19);   //addDigitalOutputReport(45, digitalOutputFeedback.db19);
            SetDigitalOutputsFeedbackManual(outDLled6, digitalOutputFeedback.db20);   //addDigitalOutputReport(46, digitalOutputFeedback.db20);
            SetDigitalOutputsFeedbackManual(outDLled7, digitalOutputFeedback.db21);   //addDigitalOutputReport(47, digitalOutputFeedback.db21);
            SetDigitalOutputsFeedbackManual(outDLled8, digitalOutputFeedback.db22);   //addDigitalOutputReport(48, digitalOutputFeedback.db22);
            resetOutputs();

        }
        int digitalOutputStatus = 0;
        public void SetDigitalOutputFeedBacksBoardAuto()
        {
            switch (digitalOutputStatus)
            {
                case 0:
                    SetDigitalOutputsFeedback(outDled1, digitalOutputFeedback.db0); addDigitalOutputReport(38, digitalOutputFeedback.db0);
                    digitalOutputStatus++;
                    break;
                case 1:
                    SetDigitalOutputsFeedback(outDled2, digitalOutputFeedback.db1); addDigitalOutputReport(39, digitalOutputFeedback.db1);
                    digitalOutputStatus++;
                    break;
                case 2:
                    SetDigitalOutputsFeedback(outDled3, digitalOutputFeedback.db2); addDigitalOutputReport(40, digitalOutputFeedback.db2);
                    digitalOutputStatus++;
                    break;
                case 3:
                    // SetDigitalOutputsFeedback(outDLled9, digitalOutputFeedback.db15); //addDigitalOutputReport(41, digitalOutputFeedback.db15);

                    digitalOutputStatus++;
                    break;
                case 4:
                    SetDigitalOutputsFeedback(outDLled2, digitalOutputFeedback.db16); addDigitalOutputReport(41, digitalOutputFeedback.db16);
                    digitalOutputStatus++;
                    break;
                case 5:
                    SetDigitalOutputsFeedback(outDLled3, digitalOutputFeedback.db17); addDigitalOutputReport(42, digitalOutputFeedback.db17);
                    digitalOutputStatus++;
                    break;
                case 6:
                    SetDigitalOutputsFeedback(outDLled4, digitalOutputFeedback.db18); addDigitalOutputReport(43, digitalOutputFeedback.db18);
                    digitalOutputStatus++;
                    break;
                case 7:
                    SetDigitalOutputsFeedback(outDLled5, digitalOutputFeedback.db19); addDigitalOutputReport(44, digitalOutputFeedback.db19);
                    digitalOutputStatus++;
                    break;
                case 8:
                    SetDigitalOutputsFeedback(outDLled6, digitalOutputFeedback.db20); addDigitalOutputReport(45, digitalOutputFeedback.db20);
                    digitalOutputStatus++;
                    break;
                case 9:
                    SetDigitalOutputsFeedback(outDLled7, digitalOutputFeedback.db21); addDigitalOutputReport(46, digitalOutputFeedback.db21);
                    // SetDigitalOutputssFeedback(outDLled8, digitalOutputFeedback.db22); addDigitalOutputReport(47, digitalOutputFeedback.db22);
                    digitalOutputStatus++;

                    break;
                case 10:
                    SetDigitalOutputsFeedback(outDLled8, digitalOutputFeedback.db22); addDigitalOutputReport(47, digitalOutputFeedback.db22);
                    digitalOutputStatus = 0;
                    if (AutoTest.DigitalOutputFail == 0)
                        AutoTest.OutputDPass = true;
                    break;

            }

        }
        public void SetDigitalOutputFeedBacksBoardManual()
        {
            if (outputParam.db0 == 1)
                SetDigitalOutputsFeedback(outDled1, digitalOutputFeedback.db0);
            if (outputParam.db1 == 1)
                SetDigitalOutputsFeedback(outDled2, digitalOutputFeedback.db1);
            if (outputParam.db2 == 1)
                SetDigitalOutputsFeedback(outDled3, digitalOutputFeedback.db2);


            if (outputParam.db8 == 1)
                SetDigitalOutputsFeedback(outDLled9, digitalOutputFeedback.db15);
            if (outputParam.db1 == 1)
                SetDigitalOutputsFeedback(outDLled2, digitalOutputFeedback.db16);
            if (outputParam.db2 == 1)
                SetDigitalOutputsFeedback(outDLled3, digitalOutputFeedback.db17);
            if (outputParam.db3 == 1)
                SetDigitalOutputsFeedback(outDLled4, digitalOutputFeedback.db18);
            if (outputParam.db4 == 1)
                SetDigitalOutputsFeedback(outDLled5, digitalOutputFeedback.db19);
            if (outputParam.db5 == 1)
                SetDigitalOutputsFeedback(outDLled6, digitalOutputFeedback.db20);
            if (outputParam.db6 == 1)
                SetDigitalOutputsFeedback(outDLled7, digitalOutputFeedback.db21);
            if (outputParam.db7 == 1)
                SetDigitalOutputsFeedback(outDLled8, digitalOutputFeedback.db22);
            resetOutputs();
        }

        public void SetDigitalOutputFeedBacksBoardManual2()
        {
            if (outputParam.db0 == 1)
                SetDigitalOutputsFeedback(outDled1, digitalOutputFeedback.db0);
            if (outputParam.db1 == 1)
                SetDigitalOutputsFeedback(outDled2, digitalOutputFeedback.db1);
            if (outputParam.db2 == 1)
                SetDigitalOutputsFeedback(outDled3, digitalOutputFeedback.db2);
            if (outputParam.db3 == 1)
                SetDigitalOutputsFeedback(outDLled5, digitalOutputFeedback.db3);
            if (outputParam.db4 == 1)
                SetDigitalOutputsFeedback(outDLled4, digitalOutputFeedback.db4);
            if (outputParam.db5 == 1)
                SetDigitalOutputsFeedback(outDLled9, digitalOutputFeedback.db5);
            if (outputParam.db6 == 1)
                SetDigitalOutputsFeedback(outDLled2, digitalOutputFeedback.db6);
            if (outputParam.db7 == 1)
                SetDigitalOutputsFeedback(outDLled3, digitalOutputFeedback.db7);
            resetOutputs();
        }

        private void CheckPower(float max, float min)
        {
            AutoTest.PowerPass = false;
            if (powerParamsFeedback != null)
            {
                if ((powerParamsFeedback.current < max) & (powerParamsFeedback.current >= min))
                {
                    //AutoTest.PowerPassed++;
                    outputColor = testGreen;
                    AutoTest.PowerPass = true;
                }
                else
                {
                    AutoTest.PowerFail++;
                    outputColor = testRed;
                    AutoTest.PowerPass = false;
                }
            }
        }
        private void CheckPower(float max)
        {
            AutoTest.PowerPass = false;
            if (powerParamsFeedback != null)
            {
                if (powerParamsFeedback.current < max)    //& (powerParamsFeedback.current > min
                {
                    //AutoTest.PowerPassed++;
                    outputColor = testGreen;
                    AutoTest.PowerPass = true;
                }
                else
                {
                    AutoTest.PowerFail++;
                    outputColor = testRed;
                    AutoTest.PowerPass = false;
                }
            }
        }
        private void UpdateUIPowerElement()
        {

            string current = string.Format("{0:F2} ", powerParamsFeedback.current);
            //AutoTest.PowerAllPass = AutoTest.PowerPassed == 14 ? true : false;
            AutoTest.PowerRecev++;
            switch (powerParamsFeedback.Number)
            {
                case 0:
                    break;
                case 1:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p1.Text = "";
                        p1.BackColor = testRed;
                        addPowerFailReport2(power1, 0, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power1, 0);
                        p1.Text = current;// powerParamsFeedback.current.ToString();
                        p1.BackColor = outputColor;

                        addPowerFailReport(power1, 0, current);
                    }
                    powerParamsFeedback.Number = 0;

                    break;
                case 2:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p2.Text = "";
                        p2.BackColor = testRed;
                        addPowerFailReport2(power2, 1, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power2, 0);
                        p2.Text = current;
                        p2.BackColor = outputColor;
                        addPowerFailReport(power2, 1, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 3:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p3.Text = "";
                        p3.BackColor = testRed;
                        addPowerFailReport2(power3, 2, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power3);
                        p3.Text = current;
                        p3.BackColor = outputColor;
                        addPowerFailReport(power3, 2, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 4:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p4.Text = "";
                        p4.BackColor = testRed;
                        addPowerFailReport2(power4, 3, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power4);
                        p4.Text = current;
                        p4.BackColor = outputColor;
                        addPowerFailReport(power4, 3, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 5:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p5.Text = "";
                        p5.BackColor = testRed;
                        addPowerFailReport2(power5, 4, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power5);
                        p5.Text = current;
                        p5.BackColor = outputColor;
                        addPowerFailReport(power5, 4, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 6:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p6.Text = "";
                        p6.BackColor = testRed;
                        addPowerFailReport2(power6, 5, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power6);
                        p6.Text = current;
                        p6.BackColor = outputColor;
                        addPowerFailReport(power6, 5, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 7:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p7.Text = "";
                        p7.BackColor = testRed;
                        addPowerFailReport2(power7, 6, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power7);
                        p7.Text = current;
                        p7.BackColor = outputColor;
                        addPowerFailReport(power7, 6, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 8:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p8.Text = "";
                        p8.BackColor = testRed;
                        addPowerFailReport2(power8, 7, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power8);
                        p8.Text = current;
                        p8.BackColor = outputColor;
                        addPowerFailReport(power8, 7, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 9:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p9.Text = "";
                        p9.BackColor = testRed;
                        addPowerFailReport2(power9, 8, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power9, 0);
                        p9.Text = current;
                        p9.BackColor = outputColor;
                        addPowerFailReport(power9, 8, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 10:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p10.Text = "";
                        p10.BackColor = testRed;
                        addPowerFailReport2(power10, 9, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power10, 0);
                        p10.Text = current;
                        p10.BackColor = outputColor;
                        addPowerFailReport(power10, 9, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 11:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p11.Text = "";
                        p11.BackColor = testRed;
                        addPowerFailReport2(power11, 10, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power11);
                        p11.Text = current;
                        p11.BackColor = outputColor;
                        addPowerFailReport(power11, 10, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 12:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p12.Text = "";
                        p12.BackColor = testRed;
                        addPowerFailReport2(power12, 11, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power12, 0);
                        p12.Text = current;
                        p12.BackColor = outputColor;
                        addPowerFailReport(power12, 11, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 13:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p13.Text = "";
                        p13.BackColor = testRed;
                        addPowerFailReport2(power13, 12, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power13);
                        p13.Text = current;
                        p13.BackColor = outputColor;
                        addPowerFailReport(power13, 12, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 14:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p14.Text = "";
                        p14.BackColor = testRed;
                        addPowerFailReport2(power14, 13, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power14);
                        p14.Text = current;
                        p14.BackColor = outputColor;
                        addPowerFailReport(power14, 13, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;

                case 15:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p15.Text = "";
                        p15.BackColor = testRed;
                        addPowerFailReport2(bpower1, 14, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power15, 0);
                        p15.Text = current;
                        p15.BackColor = outputColor;
                        addPowerFailReport(bpower1, 14, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 16:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p16.Text = "";
                        p16.BackColor = testRed;
                        addPowerFailReport2(bpower2, 15, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power16, 0);
                        p16.Text = current;
                        p16.BackColor = outputColor;
                        addPowerFailReport(bpower2, 15, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 17:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p17.Text = "";
                        p17.BackColor = testRed;
                        addPowerFailReport2(bpower3, 16, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power17, 0);
                        p17.Text = current;
                        p17.BackColor = outputColor;
                        addPowerFailReport(bpower3, 16, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 18:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p18.Text = "";
                        p18.BackColor = testRed;
                        addPowerFailReport2(bpower4, 17, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power18, 0);
                        p18.Text = current;
                        p18.BackColor = outputColor;
                        addPowerFailReport(bpower4, 17, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 19:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p19.Text = "";
                        p19.BackColor = testRed;
                        addPowerFailReport2(bpower5, 18, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power19, 0);
                        p19.Text = current;
                        p19.BackColor = outputColor;
                        addPowerFailReport(bpower5, 18, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 20:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p20.Text = "";
                        p20.BackColor = testRed;
                        addPowerFailReport2(bpower6, 19, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power20, 0);
                        p20.Text = current;
                        p20.BackColor = outputColor;
                        addPowerFailReport(bpower6, 19, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 21:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p21.Text = "";
                        p21.BackColor = testRed;
                        addPowerFailReport2(bpower7, 20, current);
                        AutoTest.PowerPass = false;
                    }
                    else
                    {
                        CheckPower(thr.power21, 0);
                        p21.Text = current;
                        p21.BackColor = outputColor;
                        addPowerFailReport(bpower7, 20, current);
                    }
                    powerParamsFeedback.Number = 0;
                    break;
                case 22:
                    if ((powerShortFlag) && (powerShortParam == powerParamsFeedback.Number))
                    {
                        p22.Text = "";
                        p22.BackColor = testRed;
                        addPowerFailReport2(bpower8, 21, current);
                        AutoTest.PowerPass = false;


                    }
                    else
                    {
                        CheckPower(thr.power22, 0);
                        p22.Text = current;
                        p22.BackColor = outputColor;
                        addPowerFailReport(bpower8, 21, current);
                        //MessageBox.Show("Danger!!!! Short State In Power Params! Stop Test! ", "Danger!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //powerShortFlag = false;
                    }
                    powerParamsFeedback.Number = 0;
                    //
                    if (AutoTest.PowerFail == 0)
                        AutoTest.PowerAllPass = true;
                    break;

            }
        }
        private void ShowMessage(string name)
        {
            //if(!TestMode.Auto)
            MessageBox.Show(string.Format("Danger!!! Short State In : {0} Power Params! Unplug BCM! ", name), "Danger!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        private void BCMPowerON(int data)
        {
            if (data == 0x10)
            {
                if (!powerSwitch.Checked)
                    powerSwitch.Checked = true;
            }
        }
        private void PowerShortState(int p)
        {
            powerShortFlag = true;
            //PowerTestCmd();
            swBoardPower.Checked = false;
            swBoardPower.Enabled = true;
            powerSwitch.Checked = false;
            StopAutoTest();
            // MessageBox.Show(string.Format("Danger!!!! Short State In : {0} Power Params! Stop Test! ", powerShortFlag), "Danger!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //     powerSwitch.Checked = false;
            switch (p)
            {
                case 0:
                    break;
                case 1:
                    ShowMessage(power1.Text);
                    p1.BackColor = testRed;
                    p1.Text = "";
                    break;
                case 2:
                    ShowMessage(power2.Text);
                    p2.BackColor = testRed;
                    p2.Text = "";
                    break;
                case 3:
                    p3.BackColor = testRed;
                    p3.Text = "";
                    ShowMessage(power3.Text);
                    break;
                case 4:
                    p4.BackColor = testRed;
                    p4.Text = "";
                    ShowMessage(power4.Text);
                    break;
                case 5:
                    p5.BackColor = testRed;
                    p5.Text = "";
                    ShowMessage(power5.Text);
                    break;

                case 6:
                    p6.BackColor = testRed;
                    p6.Text = "";
                    ShowMessage(power6.Text);
                    break;
                case 7:
                    p7.BackColor = testRed;
                    p7.Text = "";
                    ShowMessage(power7.Text);
                    break;
                case 8:
                    p8.BackColor = testRed;
                    p8.Text = "";
                    ShowMessage(power8.Text);
                    break;
                case 9:
                    p9.BackColor = testRed;
                    p9.Text = "";
                    ShowMessage(power9.Text);
                    break;
                case 10:
                    p10.BackColor = testRed;
                    p10.Text = "";
                    ShowMessage(power10.Text);
                    break;
                case 11:
                    p11.BackColor = testRed;
                    p11.Text = "";
                    ShowMessage(power11.Text);
                    break;
                case 12:
                    p12.BackColor = testRed;
                    p12.Text = "";
                    ShowMessage(power12.Text);
                    break;
                case 13:
                    p13.BackColor = testRed;
                    p13.Text = "";
                    ShowMessage(power13.Text);
                    break;
                case 14:
                    p14.BackColor = testRed;
                    p14.Text = "";
                    ShowMessage(power14.Text);
                    break;
                case 15:
                    p15.BackColor = testRed;
                    p15.Text = "";
                    ShowMessage(bpower1.Text);
                    break;
                case 16:
                    p16.BackColor = testRed;
                    p16.Text = "";
                    ShowMessage(bpower2.Text);
                    break;
                case 17:
                    p17.BackColor = testRed;
                    p17.Text = "";
                    ShowMessage(bpower3.Text);
                    break;
                case 18:
                    p18.BackColor = testRed;
                    p18.Text = "";
                    ShowMessage(bpower4.Text);
                    break;
                case 19:
                    p19.BackColor = testRed;
                    p19.Text = "";
                    ShowMessage(bpower5.Text);
                    break;
                case 20:
                    p20.BackColor = testRed;
                    p20.Text = "";
                    ShowMessage(bpower6.Text);
                    break;
                case 21:
                    p21.BackColor = testRed;
                    p21.Text = "";
                    ShowMessage(bpower7.Text);
                    break;
                case 22:
                    p22.BackColor = testRed;
                    p22.Text = "";
                    ShowMessage(bpower8.Text);
                    break;
                case 0x19:
                    AutoClosingMessageBox.Show(" Please Eject The BCM Module ! ", "Attention!", 3000);
                    break;


            }
            //MessageBox.Show("Danger!!!! Short State In Power Params! Stop Test! ", "Danger!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void SetTemperature()
        {
            if (powerParamsFeedback != null)
            {
                envTemp.Text = powerParamsFeedback.temprature.ToString();
                SetOverTempFeedback(OverTempLed, powerParamsFeedback.temprature);
            }
        }


        private void startLog_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (startReport.Checked) //& UserInfoClass.LogPermission)
            {

                report = new XtraReport1();

                var date = PersianConverterDate.ToShamsi(DateTime.Now);
                var time = DateTime.Now.ToString("HH:mm:ss tt");
                report.ReportGetData(date, time, UserInfoClass.UserName);
                report.loadTestSpecification(TestInfoClass.PartNum, TestInfoClass.TrackingNumSt, TestInfoClass.TestLab, TestInfoClass.HardWareVersion, TestInfoClass.SoftwareVersion, TestInfoClass.Description);
                report.SetReportNames(powerName, inputName, outputName);

                //test
                // Save the report to a stream.
                MemoryStream stream = new MemoryStream();
                report.SaveLayout(stream);

                // Prepare the stream for reading.
                stream.Position = 0;

                // Insert the report to a database.
                using (StreamReader sr = new StreamReader(stream))
                {
                    // Read the report from the stream to a string variable.
                    string s = sr.ReadToEnd();

                    // Add a row to a table.
                    //DataTable dt = dataSet1.Tables["Reports"];
                    //DataRow row = dt.NewRow();
                    //row["Report"] = s;
                    //dt.Rows.Add(row);
                }



            }
            else
            {
                // MessageBox.Show("You Dont Have Log Permission", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void showLogBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
            { }
        }

        private void buzerTestBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //PowerShortState(1);
            //AutoClosingMessageBox.Show("Text", "Caption", 1000);
            //var userResult = AutoClosingMessageBox.Show("Yes or No?", "Caption", 1000, MessageBoxButtons.YesNo);
            //if (userResult == System.Windows.Forms.DialogResult.Yes)
            //{
            //    // do something
            //}
            //checkTestAndPrint();

        }
        public List<byte> StartStopList = new List<byte>();
        public bool stopFlag;
        public void StopCmd()
        {
            StartStopList.Clear();
            StartStopList.Add(2);
            stopFlag = true;
            SendSerialTestCmd(StartStopList, ID.DigitalOutput, MsType.StartStopCmd);
        }
        private void stopTestBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                StartStopList.Clear();
                StartStopList.Add(2);
                stopFlag = true;
                SendSerialTestCmd(StartStopList, ID.DigitalOutput, MsType.StartStopCmd);

                Thread.Sleep(200);
                powerSwitch.Checked = false;
                StopAutoTest();
                //
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}", //:{2:00}
                           ts.Minutes, ts.Seconds);   //, ts.Milliseconds / 10

                testTime.Text = elapsedTime + " min";
                //
                //SendSerialTestCmd(StartStopList, ID.DigitalOutput, MsType.StartStopCmd);

                // XtraMessageBox.Show("Auto Test Stopped By User");
                // feedbackTb.Text += "Auto Test Stopped By User";
                TestMode.Auto = false;
                // powerSwitch.Checked = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void TabControl1_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            //if ((TabControl1.SelectedTabPage == powerTab) & (TestMode.Auto == false))
            //{
            //    powerParams.PowerList.Clear();
            //    SendSerialTestCmd(powerParams.PowerList, ID.Power, MsType.BCMCurrent);

            //}
        }

        private void BCMTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            TestInfoClass.SaveTestSpecTblInDb();
            // StopCmd();
            stopTestBtn_ItemClick(null, null);
        }

        public void SetNetFeedBacksCan()
        {
            if (netParamsFeedback.Mode == 1)
            {
                if (netParams.CANHS > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataCANH[i + 1] != netParamsFeedback.DataCANH[i])
                        {
                            netParamsFeedback.CanHFail++;
                        }

                    if (netParamsFeedback.CanHFail == 0)
                    {
                        ledBulb_Click(CANHSF, 1);
                        AutoTest.NetworkPassed++;
                        AutoTest.CanHRec = 1;
                        netReport[0] = 1;

                    }
                    else
                    {
                        ledBulb_Click(CANHSF, 0);
                        netReport[0] = 0;
                        AutoTest.CanHRec = 2;
                        //addNetworkFailReport(CANHS);

                    }
                }

            }
            if (netParamsFeedback.Mode == 2)
            {
                if (netParams.CANLS > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataCANL[i + 1] != netParamsFeedback.DataCANL[i])
                        {
                            netParamsFeedback.CanLFail++;
                        }
                    if (netParamsFeedback.CanLFail == 0)
                    {
                        ledBulb_Click(CANLSF, 1);
                        AutoTest.NetworkPassed++;
                        netReport[1] = 1;
                    }
                    else
                    {
                        ledBulb_Click(CANLSF, 0);
                        netReport[1] = 0;

                        // addNetworkFailReport(CANLS);
                    }
                }

            }
            netParamsReset();
        }

        private void powerSwitch_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            byte[] data = new byte[1];
            if (powerSwitch.Checked)
            {
                data[0] = 1;
            }
            else
            {
                data[0] = 0;
            }
            if (!powerShortFlag && !stopFlag)
                SendSerialTestCmd(data, ID.Power, MsType.PowerVoltSw);



            //Thread.Sleep(1000);
            powerShortFlag = false;
            stopFlag = false;
        }

        private void LoadTest1Start_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TabControl1.SelectedTabPage = outputTab;
            resetOutputsFeedbackBoard1();
            resetOutputsFeedbackBoard2();
            resetOutputsFeedbackBoard3();

            ID id = ID.OutputB1;
            //var board = LoadTest1SelectCb.Items.IndexOf(LoadTest1Select.EditValue.ToString());
            //switch (board)
            //{
            //    case 0:
            //        id = ID.OutputB1;
            //        resetOutputsFeedbackBoard1();
            //        break;
            //    case 1:
            //        id = ID.OutputB2;
            //        resetOutputsFeedbackBoard2();
            //        break;
            //    case 2:
            //        id = ID.OutputB3;
            //        resetOutputsFeedbackBoard3();
            //        break;
            //}
            SendSerialTestCmd(outputParam.OutputList1, id, MsType.OutputTest1);

        }
        private void LoadTest2Start_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TabControl1.SelectedTabPage = outputTab;
            resetOutputsFeedbackBoard1();
            resetOutputsFeedbackBoard2();
            resetOutputsFeedbackBoard3();
            ID id = ID.OutputB1;
            //var board = LoadTest2SelectCb.Items.IndexOf(LoadTest2Select.EditValue.ToString());
            //switch (board)
            //{
            //    case 0:
            //        id = ID.OutputB1;
            //        resetOutputsFeedbackBoard1();
            //        break;
            //    case 1:
            //        id = ID.OutputB2;
            //        resetOutputsFeedbackBoard2();
            //        break;
            //    case 2:
            //        id = ID.OutputB3;
            //        resetOutputsFeedbackBoard3();
            //        break;
            //}
            SendSerialTestCmd(outputParam.OutputList1, id, MsType.OutputTest2);
            //resetOutputsFeedback();
        }
        private void loadTest1_Test()
        {
            // if (loadTest1.Checked)
            {
                TabControl1.SelectedTabPage = outputTab;
                resetOutputsFeedbackBoard1();
                resetOutputsFeedbackBoard2();
                resetOutputsFeedbackBoard3();

                ID id = ID.OutputB1;
                SendSerialTestCmd(outputParam.OutputList1, id, MsType.OutputTest1);
            }
        }

        private void loadTest2_Test()
        {
            // if (loadTest2.Checked)
            {
                TabControl1.SelectedTabPage = outputTab;
                resetOutputsFeedbackBoard1();
                resetOutputsFeedbackBoard2();
                resetOutputsFeedbackBoard3();
                ID id = ID.OutputB1;

                SendSerialTestCmd(outputParam.OutputList1, id, MsType.OutputTest2);
            }
        }
        private void outOne1_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne1.Checked)
            {
                outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked = outOne8.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne2_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne2.Checked)
            {
                outOne1.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked = outOne8.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne3_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne3.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked = outOne8.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne4_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne4.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked = outOne8.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne5_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne5.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne6.Checked = outOne7.Checked = outOne8.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne6_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne6.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne7.Checked = outOne8.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne7_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne7.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne8.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne8_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne8.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
                outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne9_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne9.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
                outOne8.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne10_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne10.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
                outOne8.Checked = outOne9.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne11_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne11.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
                outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outOne12_CheckedChanged(object sender, EventArgs e)
        {
            if (outOne12.Checked)
            {
                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
                outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
             outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
               outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo1_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo1.Checked)
            {
                outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
               outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo2_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo2.Checked)
            {
                outTwo1.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo3_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo3.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo4_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo4.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo5_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo5.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo6_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo6.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo7_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo7.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo8_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo8.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked =
                 outTwo7.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }
        private void outTwo9_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo9.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked =
                 outTwo7.Checked = outTwo8.Checked = outTwo10.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }
        private void outTwo10_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo10.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked =
                 outTwo7.Checked = outTwo8.Checked = outTwo9.Checked = outTwo11.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outTwo11_CheckedChanged(object sender, EventArgs e)
        {
            if (outTwo11.Checked)
            {
                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = false;

                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
              outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;
            }
        }

        private void outThree1_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree1.Checked)
            {
                outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree2_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree2.Checked)
            {
                outThree1.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree3_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree3.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree4_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree4.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree5_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree5.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree6_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree6.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree7_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree7.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree8_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree8.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }



        private void outThree9_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree9.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
           outThree10.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }



        private void outThree10_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree10.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree11.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void labelControl16_Click(object sender, EventArgs e)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        private void labelControl17_Click(object sender, EventArgs e)
        {
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", //.{3:00}
                ts.Hours, ts.Minutes, ts.Seconds);   //,ts.Milliseconds / 10

            testTime.Text = elapsedTime;
        }

        private void timeBar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            checkTestAndPrint();
        }

        private void tb1_TextChanged(object sender, EventArgs e)
        {
            // waitforVersion = int.Parse(tb1.Text.ToString());
        }

        private void outThree11_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree11.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree12.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }

        private void outThree12_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree12.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree13.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }
        private void outThree13_CheckedChanged(object sender, EventArgs e)
        {
            if (outThree13.Checked)
            {
                outThree1.Checked = outThree2.Checked = outThree3.Checked = outThree4.Checked = outThree5.Checked = outThree6.Checked = outThree7.Checked = outThree8.Checked =
            outThree9.Checked = outThree10.Checked = outThree11.Checked = outThree12.Checked = false;


                outOne1.Checked = outOne2.Checked = outOne3.Checked = outOne4.Checked = outOne5.Checked = outOne6.Checked = outOne7.Checked =
         outOne8.Checked = outOne9.Checked = outOne10.Checked = outOne11.Checked = outOne12.Checked = false;

                outTwo1.Checked = outTwo2.Checked = outTwo3.Checked = outTwo4.Checked = outTwo5.Checked = outTwo6.Checked = outTwo7.Checked =
                 outTwo8.Checked = outTwo9.Checked = outTwo10.Checked = outTwo11.Checked = false;

            }
        }
        byte[] data;
        private void swBoardPower_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            data = new byte[1];
            if (swBoardPower.Checked)
            {
                data[0] = 1;
                swBoardPower.Enabled = false;
            }
            else
            {
                data[0] = 0;
            }
            if (!powerShortFlag)
                SendSerialTestCmd(data, ID.Power, MsType.BoardPowerSw);
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

            sendPacket.Visible = !sendPacket.Visible;
            packetPannel.Visible = !packetPannel.Visible;

        }
        private void swVersionBtn_Click(object sender, EventArgs e)
        {
            SoftwVer.Text = "";
            data = new byte[1];
            data[0] = 0;
            SendSerialTestCmd(data, ID.Network, MsType.BCM_Version);
        }
        public void SetNetFeedBacksLin()
        {
            if (netParamsFeedback.Mode == 1)
            {
                if (netParams.LinFront > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataLinF[i + 1] != netParamsFeedback.DataLinF[i])
                        {
                            netParamsFeedback.LinFFail++;
                        }


                    if (netParamsFeedback.LinFFail == 0)
                    {
                        ledBulb_Click(LinFrontF, 1);
                        AutoTest.NetworkPassed++;
                        netReport[2] = 1;

                    }

                    else
                    {
                        ledBulb_Click(LinFrontF, 0);
                        netReport[2] = 0;
                        //addNetworkFailReport(LinFront);
                    }
                }
            }
            if (netParamsFeedback.Mode == 2)
            {
                if (netParams.LinRear > 0 || (TestMode.Auto))
                {
                    for (int i = 0; i < 4; i++)
                        if (netParams.DataLinR[i + 1] != netParamsFeedback.DataLinR[i])
                        {
                            netParamsFeedback.LinRFail++;
                        }
                    if (netParamsFeedback.LinRFail == 0)
                    {
                        ledBulb_Click(LinRearF, 1);
                        AutoTest.NetworkPassed++;
                        netReport[3] = 1;

                    }
                    else
                    {
                        ledBulb_Click(LinRearF, 0);
                        netReport[3] = 0;
                        //addNetworkFailReport(LinRear);
                    }
                    AutoTest.NetworkPass = AutoTest.NetworkPassed == 4 ? true : false;
                }
            }
            netParamsReset();
        }
        public void SetInfoFeedback()
        {
            EcuType.Text = infoParams.ECUType.ToString();
            // HwVer.Text = infoParams.HardwareVersion;
            SoftwVer.Text = infoParams.SoftwareVersion;

            // envTemp.Text = infoParams.EnvironmentTemp.ToString();
            // ledBulb_Click(OverTempLed, (infoParams.OverTemp));

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

            sendLabel.Text = BitConverter.ToString(sendList);
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

            sendLabel.Text = BitConverter.ToString(sendList);
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

        //***************************************************

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics v = e.Graphics;
            DrawRoundRect(v, Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1, 10);
            //Without rounded corners
            //e.Graphics.DrawRectangle(Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            base.OnPaint(e);


        }
        public void DrawRoundRect(Graphics g, Pen p, float X, float Y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            //Upper-right arc:
            gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
            //Lower-right arc:
            gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
            //Lower-left arc:
            gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
            //Upper-left arc:
            gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
            gp.CloseFigure();
            g.DrawPath(p, gp);
            gp.Dispose();
        }

        private void panelControl2_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.FillRoundedRectangle(new SolidBrush(Color.White), 10, 10, this.Width - 40, this.Height - 60, 10);
            //SolidBrush brush = new SolidBrush(
            //    Color.White
            //    );
            //g.FillRoundedRectangle(brush, 12, 12, this.Width - 44, this.Height - 64, 10);
            //g.DrawRoundedRectangle(new Pen(ControlPaint.Light(Color.White, 0.00f)), 12, 12, this.Width - 44, this.Height - 64, 10);
            //g.FillRoundedRectangle(new SolidBrush(Color.White), 12, 12 + ((this.Height - 64) / 2), this.Width - 44, (this.Height - 64) / 2, 10);

            //Graphics v = e.Graphics;
            //DrawRoundRect(v, Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1, 10);
            ////Without rounded corners
            ////e.Graphics.DrawRectangle(Pens.Blue, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            //base.OnPaint(e);
        }














        //***************************************
    }
    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        DialogResult _result;
        DialogResult _timerResult;
        AutoClosingMessageBox(string text, string caption, int timeout, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            _timerResult = timerResult;
            using (_timeoutTimer)
                _result = MessageBox.Show(text, caption, buttons);
        }
        public static DialogResult Show(string text, string caption, int timeout, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
        {
            return new AutoClosingMessageBox(text, caption, timeout, buttons, timerResult)._result;
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
            _result = _timerResult;
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
