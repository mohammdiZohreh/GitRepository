using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Printing;

namespace TesterIKBCM
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }
        float[] outputThr = new float[42];
        float[] outputThrMx = new float[42];
        int[] waitTimes = new int[42];

        Printer printer = new Printer();
        int barCuntr = 1, barCuntrF = 1;
        private void Setting_Load(object sender, EventArgs e)
        {
            tsDisplayDefaultBtn();
            thDisplay();
            DisplayPrinterParams();
            LoadUsers();

            CheckPrinterName();
            repPath.Text = AutoTest.SaveReportPath;

            //loadPrinterParams();
            //var server = new PrintServer();
            //var queues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            //foreach (var queue in queues)
            //{
            //    printrName.Properties.Items.Add(queue.Name);
            //}
            //repPath.Text = AutoTest.SaveReportPath;
            //reportNumberCb.Items.AddRange(TestInfoClass.savedReports.ToArray());
            //reportNumberCb.Text = TestInfoClass.TrackingNumSt;
            //if (reportNumberCb.Items.Count > 50)
            //{
            //    reportNumberCb.Items.Clear();
            //}

        }
        public void CheckPrinterName()
        {
            var server = new PrintServer();
            var queues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            foreach (var queue in queues)
            {
                printrName.Properties.Items.Add(queue.Name);

            }

            foreach (var pr in printrName.Properties.Items)
            {
                string name = pr.ToString();
                if (name.Substring(0, 3) == "TSC")
                {
                    AutoTest.printrFlag = true;
                    break;
                }

            }
        }
        private void btnRefreshPr_Click(object sender, EventArgs e)
        {
            var server = new PrintServer();
            var queues = server.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            printrName.Properties.Items.Clear();
            foreach (var queue in queues)
            {
                printrName.Properties.Items.Add(queue.Name);
            }
        }
        ToolTip rs = new ToolTip();
        private void restorDefaultBtn_MouseHover(object sender, EventArgs e)
        {
            rs.Show("Restore Default", (Button)sender);
        }
        private void tsSaveBtn_Click(object sender, EventArgs e)
        {
            TestInfoClass.BCM_SoftwareVr_Exp = bcmSoftVrCb.Text.ToString();
            TestInfoClass.BCM_HardWareVr_Exp = bcmHardVrCb.Text.ToString();
            TestInfoClass.BCM_BootloaderVr_Exp = bcmBootVrCb.Text.ToString();
            TestInfoClass.CAS_SoftwareVr_Exp = casSoftVrCb.Text.ToString();
            TestInfoClass.CAS_HardWareVr_Exp = casHardVrCb.Text.ToString();
            TestInfoClass.CAS_BootloaderVr_Exp = casBootVrCb.Text.ToString();
            // TestInfoClass.TrackingNum=
            TestInfoClass.TestStrategy = (byte)testStrategyCb.Items.IndexOf(testStrategyCb.Text.ToString());
            TestInfoClass.InputTestStrategy = (byte)inputTestStrategyCb.Items.IndexOf(inputTestStrategyCb.Text.ToString());

            TestInfoClass.Year = yearCb.Text.ToString();


            if (tsSetAsDefault.Checked)
            {
                DataBase.SaveTestSpecTblInDb();

            }
            else
            {
                //DialogResult d = MessageBox.Show("Data has been saved", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SettingHouse.OpenMainForm();
        }
        private void tsLoadDefaultBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DataBase.LoadTestSpecTblInDb();
                tsDisplayDefaultBtn();


            }
            catch
            {

            }
        }
        private void tsDisplayDefaultBtn()
        {
            try
            {
                bcmSoftVrCb.Text = TestInfoClass.BCM_SoftwareVr_Exp.ToString();
                bcmHardVrCb.Text = TestInfoClass.BCM_HardWareVr_Exp.ToString();
                bcmBootVrCb.Text = TestInfoClass.BCM_BootloaderVr_Exp.ToString();
                casSoftVrCb.Text = TestInfoClass.CAS_SoftwareVr_Exp.ToString();
                casHardVrCb.Text = TestInfoClass.CAS_HardWareVr_Exp.ToString();
                casBootVrCb.Text = TestInfoClass.CAS_BootloaderVr_Exp.ToString();

                testStrategyCb.Text = TestInfoClass.TestStrategy == 0 ? "StopWithError" : "GoToEnd";
                yearCb.Text = TestInfoClass.Year.ToString();
                inputTestStrategyCb.Text = TestInfoClass.InputTestStrategy == 0 ? "One by One" : "by Group";


            }
            catch
            {

            }
        }
        private void tsRefreshBtn_Click(object sender, EventArgs e)
        {

            bcmSoftVrCb.Text = "";
            bcmHardVrCb.Text = "";
            bcmBootVrCb.Text = "";
            casSoftVrCb.Text = "";
            casHardVrCb.Text = "";
            casBootVrCb.Text = "";

            testStrategyCb.Text = "";
            inputTestStrategyCb.Text = "";
            yearCb.Text = "";
        }

        private void usAddBtn_Click(object sender, EventArgs e)
        {
            if (cbUser.Text != "" && tbUserId.Text != "" && tbConId.Text != "" && tbUserId.Text == tbConId.Text)
            {
                string userName = cbUser.Text;
                int userID = int.Parse(tbUserId.Text);
                int accessLevel = (accessLevelCb.Text == "Manager") ? 1 : 0;

                bool res = DataBase.AddUser(userName, userID, accessLevel);
                if (res)
                {
                    cbUser.Text = "";
                    tbUserId.Text = "";
                    tbConId.Text = "";
                    accessLevelCb.Text = "";
                    LoadUsers();
                    AutoClosingMessageBox.Show("New User Added succesfully", "Ok", 2000, MessageBoxIcon.None);
                }
                else
                {
                    AutoClosingMessageBox.Show("User Name Exist!", "Problem!", 2000, MessageBoxIcon.Error);
                }
            }
            else
            {
                AutoClosingMessageBox.Show("Please Fill The Form Correctly!", "Warning", 2000, MessageBoxIcon.None);

            }
        }
        private void LoadUsers()
        {
            cbUser.Items.Clear();
            // cbUser.Items.Clear();

            bool res = DataBase.LoadAllUsers();
            if (res)
            {
                foreach (var s in DataBase.Users)
                {
                    cbUser.Items.Add(s);
                    //  cbUser.Items.Add(s);

                }

                // cbUser.Text = DataBase.Users[0];
            }
        }
        //try
        //{
        //    DataBase.con.Open();

        //    string sql = "Select UserName from Registertbl  ";
        //    DataBase.sql_command = new SQLiteCommand();
        //    DataBase.sql_command.Connection = DataBase.con;
        //    DataBase.sql_command.CommandText = sql;

        //    SQLiteDataReader reader1;
        //    reader1 = DataBase.sql_command.ExecuteReader();
        //    while (reader1.Read())
        //    {
        //        if (reader1["UserName"].ToString() == userNameCb.Text.ToString())
        //        {
        //            //MessageBox.Show("User Name Exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            AutoClosingMessageBox.Show("User Name Exist", "Warning!", 2000);

        //            DataBase.con.Close();
        //            return;
        //        }
        //    }
        //    reader1.Close();

        //    sql = "INSERT INTO Registertbl (UserName,UserID,Position,LogP,SettingP) VALUES('" + userNameCb.Text.ToString() + "','" + userIdCb.Text.ToString() + "','" + positionCb.Text.ToString() + "','" + (logPermission.Checked ? 1 : 0) + "','" + (settingPermission.Checked ? 1 : 0) + "')";

        //    DataBase.sql_command.CommandText = sql;
        //    int result = DataBase.sql_command.ExecuteNonQuery();
        //    if (result > 0)
        //    {
        //        // XtraMessageBox.Show("User Added Successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //        AutoClosingMessageBox.Show("User Added Successfuly", "Success", 1000);
        //    }
        //    else
        //    {
        //        //MessageBox.Show("Error!");
        //        AutoClosingMessageBox.Show("Error!", "Warning", 1000);
        //    }

        //    DataBase.con.Close();

        //}
        //catch (Exception ex)
        //{
        //    //MessageBox.Show(ex.Message);
        //    // MessageBox.Show("This UserID Exist!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    AutoClosingMessageBox.Show("This UserID Exist!", "Warning", 1000);
        //    //string_con.Close();
        //}
        //finally
        //{
        //    DataBase.con.Close();
        //}


        private void usLoadBtn_Click(object sender, EventArgs e)
        {
            if (cbUser.Text != "")
            {
                SettingHouse.UserName = cbUser.Text;
                SettingHouse.UserID = 0;
                bool res = DataBase.LoadUser();
                if (res)
                {
                    cbUser.Text = SettingHouse.UserName;
                    tbUserId.Text = SettingHouse.UserID.ToString();
                    tbConId.Text = tbUserId.Text;
                    accessLevelCb.Text = (SettingHouse.accessLevel == 1) ? "Manager" : "Operator";
                    // comboUser.Items.Add(SettingHouse.UserName);

                }
                else
                {
                    AutoClosingMessageBox.Show("User Info Not Loaded!", "Problem!", 2000, MessageBoxIcon.Error);
                }
            }
            else if (tbUserId.Text != "")
            {
                SettingHouse.UserID = int.Parse(tbUserId.Text);
                SettingHouse.UserName = "";
                bool res = DataBase.LoadUser();
                if (res)
                {
                    cbUser.Text = SettingHouse.UserName;
                    tbUserId.Text = SettingHouse.UserID.ToString();
                    tbConId.Text = tbUserId.Text;
                    accessLevelCb.Text = (SettingHouse.accessLevel == 1) ? "Manager" : "Operator";
                    //comboUser.Items.Add(SettingHouse.UserName);

                }
                else
                {
                    AutoClosingMessageBox.Show("User Info Not Loaded!", "Problem!", 2000, MessageBoxIcon.Error);
                }
            }
            else
            {
                LoadUsers();


            }
        }
        bool isUser = false;
        private void usEditBtn_Click(object sender, EventArgs e)
        {
            if (cbUser.Text != "" && tbUserId.Text != "" && tbConId.Text != "" && tbUserId.Text == tbConId.Text)
            {
                SettingHouse.UserName = cbUser.Text;
                SettingHouse.NewUserID = int.Parse(tbUserId.Text);
                SettingHouse.accessLevel = (accessLevelCb.Text == "Manager") ? 1 : 0;
                if (SettingHouse.UserName == LoginData.UserName && SettingHouse.UserID == LoginData.UserID)
                {
                    isUser = true;
                }
                bool res = DataBase.EditUser();
                if (res)
                {
                    cbUser.Text = "";
                    tbUserId.Text = "";
                    tbConId.Text = "";
                    accessLevelCb.Text = "";
                    LoadUsers();
                    if (isUser)
                    {
                        LoginData.UserID = SettingHouse.UserID;
                        LoginData.UserName = SettingHouse.UserName;
                        LoginData.accessLevel = SettingHouse.accessLevel;
                        isUser = false;
                    }
                    AutoClosingMessageBox.Show(" User Info Edited succesfully", "Ok", 2000, MessageBoxIcon.None);
                }
                else
                {
                    AutoClosingMessageBox.Show("User Info Not Edited!", "Problem!", 2000, MessageBoxIcon.Error);
                }
            }
        }

        private void usRemoveBtn_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Delete This User Permenently?", "Result", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d == DialogResult.Yes)
            {
                if (cbUser.Text != "" && tbUserId.Text != "")
                {
                    SettingHouse.UserName = cbUser.Text;
                    SettingHouse.UserID = int.Parse(tbUserId.Text);
                    if (SettingHouse.UserName != LoginData.UserName && SettingHouse.UserID != LoginData.UserID)
                    {
                        bool res = DataBase.RemoveUser();
                        if (res)
                        {
                            cbUser.Text = "";
                            tbUserId.Text = "";
                            tbConId.Text = "";
                            accessLevelCb.Text = "";
                            LoadUsers();
                            AutoClosingMessageBox.Show(" User Removed succesfully", "Ok", 2000, MessageBoxIcon.None);
                        }
                        else
                        {
                            AutoClosingMessageBox.Show("User Not Removed!", "Problem!", 2000, MessageBoxIcon.Error);
                        }
                    }
                    else
                        AutoClosingMessageBox.Show("You Can't Remove Yourself!", "Problem!", 2000);

                }
            }
        }

        private void thSaveBtn_Click(object sender, EventArgs e)
        {
            try
            {

                thr.outOne1 = float.Parse(outOne1C.Text.ToString());
                thr.outOne2 = float.Parse(outOne2C.Text.ToString());
                thr.outOne3 = float.Parse(outOne3C.Text.ToString());
                thr.outOne4 = float.Parse(outOne4C.Text.ToString());
                thr.outOne5 = float.Parse(outOne5C.Text.ToString());
                thr.outOne6 = float.Parse(outOne6C.Text.ToString());
                thr.outOne7 = float.Parse(outOne7C.Text.ToString());
                thr.outOne8 = float.Parse(outOne8C.Text.ToString());
                thr.outOne9 = float.Parse(outOne9C.Text.ToString());
                thr.outOne10 = float.Parse(outOne10C.Text.ToString());
                thr.outOne11 = float.Parse(outOne11C.Text.ToString());
                thr.outOne12 = float.Parse(outOne12C.Text.ToString());
                thr.outOne13 = float.Parse(outOne13C.Text.ToString());
                thr.outOne14 = float.Parse(outOne14C.Text.ToString());

                thr.outOne1Mx = float.Parse(outOne1CMx.Text.ToString());
                thr.outOne2Mx = float.Parse(outOne2CMx.Text.ToString());
                thr.outOne3Mx = float.Parse(outOne3CMx.Text.ToString());
                thr.outOne4Mx = float.Parse(outOne4CMx.Text.ToString());
                thr.outOne5Mx = float.Parse(outOne5CMx.Text.ToString());
                thr.outOne6Mx = float.Parse(outOne6CMx.Text.ToString());
                thr.outOne7Mx = float.Parse(outOne7CMx.Text.ToString());
                thr.outOne8Mx = float.Parse(outOne8CMx.Text.ToString());
                thr.outOne9Mx = float.Parse(outOne9CMx.Text.ToString());
                thr.outOne10Mx = float.Parse(outOne10CMx.Text.ToString());
                thr.outOne11Mx = float.Parse(outOne11CMx.Text.ToString());
                thr.outOne12Mx = float.Parse(outOne12CMx.Text.ToString());
                thr.outOne13Mx = float.Parse(outOne13CMx.Text.ToString());
                thr.outOne14Mx = float.Parse(outOne14CMx.Text.ToString());

                //
                thr.outTwo1 = float.Parse(outTwo1C.Text.ToString());
                thr.outTwo2 = float.Parse(outTwo2C.Text.ToString());
                thr.outTwo3 = float.Parse(outTwo3C.Text.ToString());
                thr.outTwo4 = float.Parse(outTwo4C.Text.ToString());
                thr.outTwo5 = float.Parse(outTwo5C.Text.ToString());
                thr.outTwo6 = float.Parse(outTwo6C.Text.ToString());
                thr.outTwo7 = float.Parse(outTwo7C.Text.ToString());
                thr.outTwo8 = float.Parse(outTwo8C.Text.ToString());
                thr.outTwo9 = float.Parse(outTwo9C.Text.ToString());
                thr.outTwo10 = float.Parse(outTwo10C.Text.ToString());
                thr.outTwo11 = float.Parse(outTwo11C.Text.ToString());
                thr.outTwo12 = float.Parse(outTwo12C.Text.ToString());
                thr.outTwo13 = float.Parse(outTwo13C.Text.ToString());
                thr.outTwo14 = float.Parse(outTwo14C.Text.ToString());

                thr.outTwo1Mx = float.Parse(outTwo1CMx.Text.ToString());
                thr.outTwo2Mx = float.Parse(outTwo2CMx.Text.ToString());
                thr.outTwo3Mx = float.Parse(outTwo3CMx.Text.ToString());
                thr.outTwo4Mx = float.Parse(outTwo4CMx.Text.ToString());
                thr.outTwo5Mx = float.Parse(outTwo5CMx.Text.ToString());
                thr.outTwo6Mx = float.Parse(outTwo6CMx.Text.ToString());
                thr.outTwo7Mx = float.Parse(outTwo7CMx.Text.ToString());
                thr.outTwo8Mx = float.Parse(outTwo8CMx.Text.ToString());
                thr.outTwo9Mx = float.Parse(outTwo9CMx.Text.ToString());
                thr.outTwo10Mx = float.Parse(outTwo10CMx.Text.ToString());
                thr.outTwo11Mx = float.Parse(outTwo11CMx.Text.ToString());
                thr.outTwo12Mx = float.Parse(outTwo12CMx.Text.ToString());
                thr.outTwo13Mx = float.Parse(outTwo13CMx.Text.ToString());
                thr.outTwo14Mx = float.Parse(outTwo14CMx.Text.ToString());
                //
                thr.outThree1 = float.Parse(outThree1C.Text.ToString());
                thr.outThree2 = float.Parse(outThree2C.Text.ToString());
                thr.outThree3 = float.Parse(outThree3C.Text.ToString());
                thr.outThree4 = float.Parse(outThree4C.Text.ToString());
                thr.outThree5 = float.Parse(outThree5C.Text.ToString());
                thr.outThree6 = float.Parse(outThree6C.Text.ToString());
                thr.outThree7 = float.Parse(outThree7C.Text.ToString());
                thr.outThree8 = float.Parse(outThree8C.Text.ToString());
                //thr.outThree9 = float.Parse(outThree9C.Text.ToString());
                //thr.outThree10 = float.Parse(outThree10C.Text.ToString());
                //thr.outThree11 = float.Parse(outThree11C.Text.ToString());
                //thr.outThree12 = float.Parse(outThree12C.Text.ToString());
                //thr.outThree13 = float.Parse(outThree13C.Text.ToString());
                //thr.outThree14 = float.Parse(outThree14C.Text.ToString());

                thr.outThree1Mx = float.Parse(outThree1CMx.Text.ToString());
                thr.outThree2Mx = float.Parse(outThree2CMx.Text.ToString());
                thr.outThree3Mx = float.Parse(outThree3CMx.Text.ToString());
                thr.outThree4Mx = float.Parse(outThree4CMx.Text.ToString());
                thr.outThree5Mx = float.Parse(outThree5CMx.Text.ToString());
                thr.outThree6Mx = float.Parse(outThree6CMx.Text.ToString());
                thr.outThree7Mx = float.Parse(outThree7CMx.Text.ToString());
                thr.outThree8Mx = float.Parse(outThree8CMx.Text.ToString());
                //thr.outThree9Mx = float.Parse(outThree9Mx.Text.ToString());
                //thr.outThree10Mx = float.Parse(outThree10Mx.Text.ToString());
                //thr.outThree11Mx = float.Parse(outThree11Mx.Text.ToString());
                //thr.outThree12Mx = float.Parse(outThree12Mx.Text.ToString());
                //thr.outThree13Mx = float.Parse(outThree13Mx.Text.ToString());
                //thr.outThree14Mx = float.Parse(outThree14Mx.Text.ToString());

                int indx = 0;
                if (thSetAsDefault.Checked)
                {
                    outputThr[indx++] = thr.outOne1;
                    outputThr[indx++] = thr.outOne2;
                    outputThr[indx++] = thr.outOne3;
                    outputThr[indx++] = thr.outOne4;
                    outputThr[indx++] = thr.outOne5;
                    outputThr[indx++] = thr.outOne6;
                    outputThr[indx++] = thr.outOne7;
                    outputThr[indx++] = thr.outOne8;
                    outputThr[indx++] = thr.outOne9;
                    outputThr[indx++] = thr.outOne10;
                    outputThr[indx++] = thr.outOne11;
                    outputThr[indx++] = thr.outOne12;
                    outputThr[indx++] = thr.outOne13;
                    outputThr[indx++] = thr.outOne14;

                    outputThr[indx++] = thr.outTwo1;
                    outputThr[indx++] = thr.outTwo2;
                    outputThr[indx++] = thr.outTwo3;
                    outputThr[indx++] = thr.outTwo4;
                    outputThr[indx++] = thr.outTwo5;
                    outputThr[indx++] = thr.outTwo6;
                    outputThr[indx++] = thr.outTwo7;
                    outputThr[indx++] = thr.outTwo8;
                    outputThr[indx++] = thr.outTwo9;
                    outputThr[indx++] = thr.outTwo10;
                    outputThr[indx++] = thr.outTwo11;
                    outputThr[indx++] = thr.outTwo12;
                    outputThr[indx++] = thr.outTwo13;
                    outputThr[indx++] = thr.outTwo14;

                    outputThr[indx++] = thr.outThree1;
                    outputThr[indx++] = thr.outThree2;
                    outputThr[indx++] = thr.outThree3;
                    outputThr[indx++] = thr.outThree4;
                    outputThr[indx++] = thr.outThree5;
                    outputThr[indx++] = 0;  //thr.outThree6;
                    outputThr[indx++] = 0;  //thr.outThree7;
                    outputThr[indx++] = 0;  //thr.outThree8;
                    outputThr[indx++] = 0;  //thr.outThree9;
                    outputThr[indx++] = 0;  //thr.outThree10;
                    outputThr[indx++] = 0;  //thr.outThree11;
                    outputThr[indx++] = 0;  //thr.outThree12;
                    outputThr[indx++] = 0;  //thr.outThree13;
                    outputThr[indx++] = 0; //thr.outThree14;
                    //Max
                    indx = 0;
                    outputThrMx[indx++] = thr.outOne1Mx;
                    outputThrMx[indx++] = thr.outOne2Mx;
                    outputThrMx[indx++] = thr.outOne3Mx;
                    outputThrMx[indx++] = thr.outOne4Mx;
                    outputThrMx[indx++] = thr.outOne5Mx;
                    outputThrMx[indx++] = thr.outOne6Mx;
                    outputThrMx[indx++] = thr.outOne7Mx;
                    outputThrMx[indx++] = thr.outOne8Mx;
                    outputThrMx[indx++] = thr.outOne9Mx;
                    outputThrMx[indx++] = thr.outOne10Mx;
                    outputThrMx[indx++] = thr.outOne11Mx;
                    outputThrMx[indx++] = thr.outOne12Mx;
                    outputThrMx[indx++] = thr.outOne13Mx;
                    outputThrMx[indx++] = thr.outOne14;

                    outputThrMx[indx++] = thr.outTwo1Mx;
                    outputThrMx[indx++] = thr.outTwo2Mx;
                    outputThrMx[indx++] = thr.outTwo3Mx;
                    outputThrMx[indx++] = thr.outTwo4Mx;
                    outputThrMx[indx++] = thr.outTwo5Mx;
                    outputThrMx[indx++] = thr.outTwo6Mx;
                    outputThrMx[indx++] = thr.outTwo7Mx;
                    outputThrMx[indx++] = thr.outTwo8Mx;
                    outputThrMx[indx++] = thr.outTwo9Mx;
                    outputThrMx[indx++] = thr.outTwo10Mx;
                    outputThrMx[indx++] = thr.outTwo11Mx;
                    outputThrMx[indx++] = thr.outTwo12Mx;
                    outputThrMx[indx++] = thr.outTwo13Mx;
                    outputThrMx[indx++] = thr.outTwo14Mx;

                    outputThrMx[indx++] = thr.outThree1Mx;
                    outputThrMx[indx++] = thr.outThree2Mx;
                    outputThrMx[indx++] = thr.outThree3Mx;
                    outputThrMx[indx++] = thr.outThree4Mx;
                    outputThrMx[indx++] = thr.outThree5Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree6Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree7Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree8Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree9Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree10Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree11Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree12Mx;
                    outputThrMx[indx++] = 0;    //thr.outThree13Mx;
                    outputThrMx[indx++] = 0;    // thr.outThree14Mx;


                    DataBase.SaveOutputsThresholdsInDb(outputThr, outputThrMx);
                }
            }
            catch (Exception ex) { }
        }

        private void thLoadDefaultBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DataBase.LoadOutputsThresholdsFromSqlDatabase();
                thDisplay();


            }
            catch (Exception ex)
            {

            }

        }
        private void thDisplay()
        {
            try
            {
                //float[] read = TestInfoClass.outputThr;                            //LoadOutputsThresholdsFromSqlDatabase();
                outOne1C.Text = thr.outOne1.ToString();  //read[ind++].ToString();
                outOne2C.Text = thr.outOne2.ToString();  //read[ind++].ToString();
                outOne3C.Text = thr.outOne3.ToString();  //read[ind++].ToString();
                outOne4C.Text = thr.outOne4.ToString();
                outOne5C.Text = thr.outOne5.ToString();
                outOne6C.Text = thr.outOne6.ToString();
                outOne7C.Text = thr.outOne7.ToString();
                outOne8C.Text = thr.outOne8.ToString();
                outOne9C.Text = thr.outOne9.ToString();
                outOne10C.Text = thr.outOne10.ToString();
                outOne11C.Text = thr.outOne11.ToString();
                outOne12C.Text = thr.outOne12.ToString();
                outOne13C.Text = thr.outOne13.ToString();
                outOne14C.Text = thr.outOne14.ToString();

                outTwo1C.Text = thr.outTwo1.ToString();
                outTwo2C.Text = thr.outTwo2.ToString();
                outTwo3C.Text = thr.outTwo3.ToString();
                outTwo4C.Text = thr.outTwo4.ToString();
                outTwo5C.Text = thr.outTwo5.ToString();
                outTwo6C.Text = thr.outTwo6.ToString();
                outTwo7C.Text = thr.outTwo7.ToString();
                outTwo8C.Text = thr.outTwo8.ToString();
                outTwo9C.Text = thr.outTwo9.ToString();
                outTwo10C.Text = thr.outTwo10.ToString();
                outTwo11C.Text = thr.outTwo11.ToString();
                outTwo12C.Text = thr.outTwo12.ToString();
                outTwo13C.Text = thr.outTwo13.ToString();
                outTwo14C.Text = thr.outTwo14.ToString();

                outThree1C.Text = thr.outThree1.ToString();
                outThree2C.Text = thr.outThree2.ToString();
                outThree3C.Text = thr.outThree3.ToString();
                outThree4C.Text = thr.outThree4.ToString();
                outThree5C.Text = thr.outThree5.ToString();
                //outThree6C.Text = thr.outThree6.ToString();
                //outThree7C.Text = thr.outThree7.ToString();
                //outThree8C.Text = thr.outThree8.ToString();
                //outThree9C.Text = thr.outThree9.ToString();
                //outThree10C.Text = thr.outThree10.ToString();
                //outThree11C.Text = thr.outThree11.ToString();
                //outThree12C.Text = thr.outThree12.ToString();
                //outThree13C.Text = thr.outThree13.ToString();
                //outThree14C.Text = thr.outThree14.ToString();

                outOne1CMx.Text = thr.outOne1Mx.ToString();
                outOne2CMx.Text = thr.outOne2Mx.ToString();
                outOne3CMx.Text = thr.outOne3Mx.ToString();
                outOne4CMx.Text = thr.outOne4Mx.ToString();
                outOne5CMx.Text = thr.outOne5Mx.ToString();
                outOne6CMx.Text = thr.outOne6Mx.ToString();
                outOne7CMx.Text = thr.outOne7Mx.ToString();
                outOne8CMx.Text = thr.outOne8Mx.ToString();
                outOne9CMx.Text = thr.outOne9Mx.ToString();
                outOne10CMx.Text = thr.outOne10Mx.ToString();
                outOne11CMx.Text = thr.outOne11Mx.ToString();
                outOne12CMx.Text = thr.outOne12Mx.ToString();
                outOne13CMx.Text = thr.outOne13Mx.ToString();
                outOne14CMx.Text = thr.outOne14Mx.ToString();

                outTwo1CMx.Text = thr.outTwo1Mx.ToString();
                outTwo2CMx.Text = thr.outTwo2Mx.ToString();
                outTwo3CMx.Text = thr.outTwo3Mx.ToString();
                outTwo4CMx.Text = thr.outTwo4Mx.ToString();
                outTwo5CMx.Text = thr.outTwo5Mx.ToString();
                outTwo6CMx.Text = thr.outTwo6Mx.ToString();
                outTwo7CMx.Text = thr.outTwo7Mx.ToString();
                outTwo8CMx.Text = thr.outTwo8Mx.ToString();
                outTwo9CMx.Text = thr.outTwo9Mx.ToString();
                outTwo10CMx.Text = thr.outTwo10Mx.ToString();
                outTwo11CMx.Text = thr.outTwo11Mx.ToString();
                outTwo12CMx.Text = thr.outTwo12Mx.ToString();
                outTwo13CMx.Text = thr.outTwo13Mx.ToString();
                outTwo14CMx.Text = thr.outTwo14Mx.ToString();


                outThree1CMx.Text = thr.outThree1Mx.ToString();
                outThree2CMx.Text = thr.outThree2Mx.ToString();
                outThree3CMx.Text = thr.outThree3Mx.ToString();
                outThree4CMx.Text = thr.outThree4Mx.ToString();
                outThree5CMx.Text = thr.outThree5Mx.ToString();
                //outThree6CMx.Text = thr.outThree6Mx.ToString();
                //outThree7CMx.Text = thr.outThree7Mx.ToString();
                //outThree8CMx.Text = thr.outThree8Mx.ToString();
                //outThree9CMx.Text = thr.outThree9Mx.ToString();
                //outThree10Mx.Text = thr.outThree10Mx.ToString();
                //outThree11Mx.Text = thr.outThree11Mx.ToString();
                //outThree12Mx.Text = thr.outThree12Mx.ToString();
                //outThree13Mx.Text = thr.outThree13Mx.ToString();
                //outThree14Mx.Text = thr.outThree14Mx.ToString();


            }
            catch (Exception ex)
            {

            }
        }

        private void restorDefaultBtn_Click(object sender, EventArgs e)
        {
            //float[] read = DataBase.LoadDefaultOutputsThresholdsFromSqlDatabase();
            //outOne1C.Text = read[0].ToString();
            //outOne2C.Text = read[1].ToString();
        }

        private void userRefreshBtn_Click(object sender, EventArgs e)
        {
            cbUser.Text = "";
            tbUserId.Text = "";
            tbConId.Text = "";
            accessLevelCb.Text = "";
        }



        private void loadReportBtn_Click(object sender, EventArgs e)
        {
            //XtraReport rp = TestInfoClass.LoadReport(reportNumberCb.EditValue.ToString());
            //if (rp != null)
            //{
            //    ReportPrintTool printTool = new ReportPrintTool(rp);
            //    printTool.ShowRibbonPreview();
            //}
            //else
            //{
            //    MessageBox.Show("This Report Number Dosnt Exist!");
            //}
        }

        private void refreshReportBtn_Click(object sender, EventArgs e)
        {
            reportNumberCb.Properties.Items.Clear();
            Setting_Load(null, null);
        }
        int r;
        private void deleteReportBtn_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Delete This Report?", "Result", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d == DialogResult.Yes)
                // r = DataBase.DeleteReportInDb(reportNumberCb.Text.ToString());
                if (r == 1)
                {
                    MessageBox.Show("Report Deleted Successfuly");
                }
                else
                {
                    MessageBox.Show("Report Not Deleted!");
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
        private void btnTestPrint_Click(object sender, EventArgs e)
        {
            try
            {
                AutoTest.AutoTestPass = true;

                TestInfoClass.Barcode = MakeBarcode();
                var labelText = "EKS Co.";

                TestInfoClass.TotalTestPass++;

                printer.TSCPrint(TestInfoClass.Barcode, labelText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void testFailPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //printerSetting_Click(null, null);
                // AutoTest.AutoTestPass = true;
                //TestInfoClass.UpdateTrackingSt(0);
                var labelText = wff_content.Text;// + (barCuntrF++).ToString("0000");// TestInfoClass.TrackingNumSt;
                printer.TSCPrintFail(labelText);
                Console.WriteLine(labelText);   //for test...
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void rArrow1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void rArrow2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }
        private void rightArrowBtn1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }
        private void rArrow4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }

        private void thRefreshBtn_Click(object sender, EventArgs e)
        {
            outOne1C.Text = "";
            outOne2C.Text = "";
            outOne3C.Text = "";
            outOne4C.Text = "";
            outOne5C.Text = "";
            outOne6C.Text = "";
            outOne7C.Text = "";
            outOne8C.Text = "";
            outOne9C.Text = "";
            outOne10C.Text = "";
            outOne11C.Text = "";
            outOne12C.Text = "";
            outOne13C.Text = "";
            outOne14C.Text = "";

            outOne1CMx.Text = "";
            outOne2CMx.Text = "";
            outOne3CMx.Text = "";
            outOne4CMx.Text = "";
            outOne5CMx.Text = "";
            outOne6CMx.Text = "";
            outOne7CMx.Text = "";
            outOne8CMx.Text = "";
            outOne9CMx.Text = "";
            outOne10CMx.Text = "";
            outOne11CMx.Text = "";
            outOne12CMx.Text = "";
            outOne13CMx.Text = "";
            outOne14CMx.Text = "";
        }

        private void thDeleteBtn_Click(object sender, EventArgs e)
        {
            thRefreshBtn_Click(null, null);
        }
        private void btnLoadPr_Click(object sender, EventArgs e)
        {
            bool r = DataBase.LoadPrintrSetting();
            DisplayPrinterParams();
        }
        private void DisplayPrinterParams()
        {
            //bool r = DataBaseLite.LoadPrintrSetting();
            // if (r)
            {
                printrName.Text = lbl.Printer_Name;
                lbl_w.Text = lbl.label_Width;
                lbl_h.Text = lbl.label_Height;
                lbl_d.Text = lbl.label_Density;
                lbl_v.Text = lbl.label_Vertical;
                lbl_o.Text = lbl.label_Offset;

                lbl_sp.Text = lbl.label_Speed;
                lbl_se.SelectedIndex = int.Parse(lbl.label_Sensor); //???
                printer_dpi.Text = lbl.printr_dpi.ToString();

                if (lbl.printr_dpi == 200) dot = 8;
                else if (lbl.printr_dpi == 300) dot = 12;
                br_x.Text = (float.Parse(lbl.barcd_x.ToString()) / dot).ToString();
                br_y.Text = (float.Parse(lbl.barcd_y.ToString()) / dot).ToString();
                br_h.Text = (float.Parse(lbl.barcd_Height.ToString()) / dot).ToString();

                br_c.Text = lbl.barcd_content;
                br_re.SelectedIndex = int.Parse(lbl.barcd_readble);//???
                br_t.Text = lbl.barcd_type;
                br_ro.Text = lbl.barcd_rotation;
                br_n.Text = lbl.barcd_narrow;
                br_w.Text = lbl.barcd_wide;

                wfp_x.Text = (float.Parse(lbl.wfp_x.ToString()) / dot).ToString();
                wfp_y.Text = (float.Parse(lbl.wfp_y.ToString()) / dot).ToString();
                wfp_fHeight.Text = (float.Parse(lbl.wfp_fontHeight.ToString()) / dot).ToString();

                wfp_content.Text = lbl.wfp_content;
                wfp_r.Text = lbl.wfp_rotation.ToString();
                wfp_under.Text = lbl.wfp_underline.ToString();

                wfp_fs.SelectedIndex = lbl.wfp_fontstyle;//???
                wfp_f.Text = lbl.wfp_font;
                //
                wff_x.Text = (float.Parse(lbl.wff_x.ToString()) / dot).ToString();
                wff_y.Text = (float.Parse(lbl.wff_y.ToString()) / dot).ToString();
                wff_fHeight.Text = (float.Parse(lbl.wff_fontHeight.ToString()) / dot).ToString();

                wff_content.Text = lbl.wff_content;
                wff_r.Text = lbl.wff_rotation.ToString();
                wff_under.Text = lbl.wff_underline.ToString();

                wff_fs.SelectedIndex = lbl.wff_fontstyle;//???
                wff_f.Text = lbl.wff_font;
            }
        }
        int dot = 8;

        private void btnSaveReports_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                AutoTest.SaveReportPath = folderBrowserDialog1.SelectedPath;// + @"\Reports\";
                repPath.Text = AutoTest.SaveReportPath;
                DataBase.SaveReportPath();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lbl_w.Text = 55.ToString();
            lbl_h.Text = "15";// 15.ToString();
            lbl_d.Text = "8";
            lbl_v.Text = "3";
            lbl_o.Text = "0";
            lbl_sp.Text = "4";
            lbl_se.Text = " Vertical gap";
            printer_dpi.Text = "200";
            //
            br_x.Text = "7";
            br_y.Text = "4";
            br_h.Text = "6";
            br_c.Text = "RU8K1ES";
            br_c.Text = "R_Center";
            br_t.Text = "128";
            var i = int.Parse(br_x.Text);
            br_ro.Text = "0";
            br_n.Text = "2";
            br_w.Text = "2";
            //
            wfp_x.Text = "7";
            wfp_y.Text = "1";
            wfp_fHeight.Text = "2.5";
            wfp_content.Text = "IKBCM - EKS Co.";
            wfp_r.Text = "0";
            wfp_under.Text = "0";
            wfp_fs.Text = "Bold";
            wfp_f.Text = "Arial";

            wff_x.Text = "7";
            wff_y.Text = "4";
            wff_fHeight.Text = "4";
            wff_content.Text = "IKBCM - EKS Co.";
            wff_r.Text = "0";
            wff_under.Text = "1";
            wff_fs.Text = "Bold";
            wff_f.Text = "Arial";
        }

        private void btnMorSting_Click(object sender, EventArgs e)
        {
            Pannel4.Visible = !(Pannel4.Visible);
            Panel5.Visible = !(Panel5.Visible);
            Panel6.Visible = !(Panel6.Visible);
            Panel7.Visible = !(Panel7.Visible);
            Panel8.Visible = !(Panel8.Visible);
            Panel12.Visible = !(Panel12.Visible);
            Panel13.Visible = !(Panel13.Visible);
        }

        private void printerSetting_Click(object sender, EventArgs e)
        {
            try
            {
                lbl.Printer_Name = printrName.Text;
                lbl.label_Width = lbl_w.Text;
                lbl.label_Height = lbl_h.Text;
                var density = int.Parse(lbl_d.Text);
                if (density >= 0 && density <= 15) lbl.label_Density = density.ToString();
                else
                {
                    lbl.label_Density = "8";
                    lbl_d.Text = "8";
                }
                lbl.label_Vertical = lbl_v.Text;
                lbl.label_Offset = lbl_o.Text;

                lbl.label_Speed = lbl_sp.Text;
                lbl.label_Sensor = lbl_se.SelectedIndex.ToString();
                lbl.printr_dpi = int.Parse(printer_dpi.Text);
                if (lbl.printr_dpi == 200) dot = 8;
                else if (lbl.printr_dpi == 300) dot = 12;
                lbl.barcd_x = (float.Parse(br_x.Text.ToString()) * dot).ToString();
                lbl.barcd_y = (float.Parse(br_y.Text) * dot).ToString();
                lbl.barcd_Height = (float.Parse(br_h.Text) * dot).ToString();
                lbl.barcd_content = br_c.Text;

                lbl.barcd_readble = br_re.SelectedIndex.ToString();
                lbl.barcd_type = br_t.Text;
                lbl.barcd_rotation = br_ro.Text;
                lbl.barcd_narrow = br_n.Text;
                lbl.barcd_wide = br_w.Text;

                lbl.wfp_x = (int)(float.Parse(wfp_x.Text.ToString()) * dot);
                lbl.wfp_y = (int)(float.Parse(wfp_y.Text.ToString()) * dot);
                lbl.wfp_fontHeight = (int)(float.Parse(wfp_fHeight.Text.ToString()) * dot);
                lbl.wfp_content = wfp_content.Text;

                lbl.wfp_rotation = int.Parse(wfp_r.Text.ToString());
                lbl.wfp_underline = int.Parse(wfp_under.Text.ToString());
                lbl.wfp_fontstyle = wfp_fs.SelectedIndex;
                lbl.wfp_font = wfp_f.Text;

                lbl.wff_x = (int)(float.Parse(wff_x.Text.ToString()) * dot);
                lbl.wff_y = (int)(float.Parse(wff_y.Text.ToString()) * dot);
                lbl.wff_fontHeight = (int)(float.Parse(wff_fHeight.Text.ToString()) * dot);
                lbl.wff_content = wff_content.Text;

                lbl.wff_rotation = int.Parse(wff_r.Text.ToString());
                lbl.wff_underline = int.Parse(wff_under.Text.ToString());
                lbl.wff_fontstyle = wff_fs.SelectedIndex;
                lbl.wff_font = wff_f.Text;
                //if (lbl.wff_font == "B Nazanin")
                //{
                //    lbl.wff_x = int.Parse(lbl.label_Width) - int.Parse(wff_x.Text);
                //    lbl.wff_x *= dot;
                //    lbl.wff_y = int.Parse(lbl.label_Height) - int.Parse(wff_y.Text);
                //    lbl.wff_y *= dot;
                //}
                if (printerSettingDef.Checked)
                    DataBase.SavePrintrSetting();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
