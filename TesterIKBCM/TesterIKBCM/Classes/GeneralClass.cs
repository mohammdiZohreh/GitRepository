using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TesterIKBCM
{
    class GeneralClass
    {
    }
    public class swClass 
    {
        public string name { get; set; }
        public double value { get; set; }
        public bool check { get; set; } 
    }
        public static class TestInfoClass
    {
        public static string BCM_SoftwareVr_Rec { get; set; } = "";
        public static string BCM_HardWareVr_Rec { get; set; } = "";
        public static string BCM_BootloaderVr_Rec { get; set; } = "";
        public static string CAS_SoftwareVr_Rec { get; set; } = "";
        public static string CAS_HardWareVr_Rec { get; set; } = "";
        public static string CAS_BootloaderVr_Rec { get; set; } = "";

        public static string BCM_SoftwareVr_Exp { get; set; } = "02.44";
        public static string BCM_HardWareVr_Exp { get; set; } = "";
        public static string BCM_BootloaderVr_Exp { get; set; } = "";
        public static string CAS_SoftwareVr_Exp { get; set; } = "";
        public static string CAS_HardWareVr_Exp { get; set; } = "";
        public static string CAS_BootloaderVr_Exp{ get; set; } = "";

        public static string PartNum { get; set; } = "IK01627480";
        public static int TrackingNum { get; set; }
        public static string TrackingNumSt { get; set; }
        public static int ProductSerial { get; set; }
        public static int DayNum { get; set; }

        public static string TestLab { get; set; }
       
        public static string Year { get; set; } = "R";
        public static string DesignerCode { get; set; } = "ES";
        public static string DeviceCode { get; set; } = "U8";
        public static string ChangeTurn { get; set; } = "D4";
        public static byte TestStrategy { get; set; }
        public static byte InputTestStrategy { get; set; }
        public static string Barcode { get; set; } = "D4";
        public static int TotalTestPass { get; set; } = 0;
        public static int TotalTestFail { get; set; } = 0;

        public static List<string> savedReports = new List<string>();
        public static float[] outputThr = new float[84];

        public static string Description { get; set; }
        public static void UpdateTrackingSt(byte u)
        {
            // TrackingNumSt = PersianConverterDate.YearToShamsi(DateTime.Now).ToString() + PersianConverterDate.DeyNumber(DateTime.Now).ToString() + TrackingNum.ToString("0000");
            if (u == 1)
                TestInfoClass.TrackingNum++;
            TrackingNumSt = Year + PersianConverterDate.DeyNumber(DateTime.Now).ToString() + TrackingNum.ToString("0000");

            TrackingNumSt = Regex.Replace(TrackingNumSt, @"\s", "");
        }
        public static void UpdateTrackingSt()
        {
            TrackingNumSt = PersianConverterDate.YearToShamsi(DateTime.Now).ToString() + PersianConverterDate.DeyNumber(DateTime.Now).ToString() + TrackingNum.ToString("0000"); ;
            TrackingNumSt = Regex.Replace(TrackingNumSt, @"\s", "");
        }
        public static void loadReportList()
        {
            savedReports = new List<string>();
        }
        public static bool CheckNewDay()
        {
            int day = PersianConverterDate.DeyNumber(DateTime.Now);

            if (DayNum < day)
            {
                DayNum = day;
                TrackingNum = 0;
                ProductSerial = 0;
                TotalTestPass = 0;
                TotalTestFail = 0;

                return true;
            }
            return false;
        }
      


    }
    public static class LoginData
    {
        public static string UserName { get; set; }
        public static int UserID { get; set; }
        public static int accessLevel { get; set; }
        public static bool IsLogin { get; set; }
    }
    public static class SettingHouse
    {
        public static string UserName { get; set; }
        public static string loginLabel { get; set; }
        public static bool IsLogin { get; set; }

        public static int UserID { get; set; }
        public static int NewUserID { get; set; }


        public static int accessLevel { get; set; }
       // public static bool LogPermission { get; set; }
      //  public static bool SettingPermission { get; set; }
        public static Color ComColor { get; set; }

        public static bool MainFormIsOpen { get; set; }

        public static MainForm mainForm;//= new BCMTester();
        public static FormLogin loginForm { get; set; }

        public static void OpenLoginForm()
        {
            if (loginForm == null)
                loginForm = new FormLogin();
            if (loginForm.IsDisposed)
            {
                loginForm = new FormLogin();
            }
            loginForm.Show();
        }
        public static void OpenMainForm()//object sender, string user
        {
            if (mainForm == null || (mainForm.IsDisposed))
            {

            }
            if(loginForm !=null)
            loginForm.Close();
            MainFormIsOpen = true;
        }

    }
    public static class UserInfoClass
    {
        //public static int UserID { get; set; }
        //public static int NewUserID { get; set; }
        //public static string UserName { get; set; }
        //public static string Position { get; set; }
        //public static int accessLevel { get; set; }
        //public static Color ComColor { get; set; }
        //public static bool LogPermission { get; set; }
        //public static bool SettingPermission { get; set; }
        //public static bool IsLogin { get; set; }
        //public static string loginLabel { get; set; }
        //public static bool MainFormIsOpen { get; set; }

        //public static MainForm mainForm;//= new BCMTester();
        //public static FormLogin loginForm { get; set; }

        //public static void OpenLoginForm()
        //{
        //    if (loginForm == null)
        //        loginForm = new FormLogin();
        //    if (loginForm.IsDisposed)
        //    {
        //        loginForm = new FormLogin();
        //    }
        //    loginForm.Show();
        //}
        //public static void OpenMainForm()//object sender, string user
        //{
        //    if (mainForm == null || (mainForm.IsDisposed))
        //    {
                
        //    }

        //    loginForm.Close();
        //    MainFormIsOpen = true;
        //}
    }
    public static class TimingLimit
    {
        public static int NetworkTime { get; set; } = 40;
        public static int CASTime { get; set; } = 25;

        public static int NetworkTime2 { get; set; } = 10;
        public static int PowerTime { get; set; } = 10;
        public static int OutputTime { get; set; } = 50;
        public static int loadTest1Time { get; set; } = 200;
        public static int loadTest2Time { get; set; } = 70;
        public static int DigitalOutputTime { get; set; } = 40;
        public static int InputTime { get; set; } = 55;
        public static int AInputTime { get; set; } = 10;
        public static int waitForVersion { get; set; } = 11;


    }
}