using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterIKBCM

{
    public enum InputBoard
    {
        Input1,
        Input2
    }
    public enum FuelLvl
    {
        Full = 32,
        ThreeQuarters = 35,
        Half = 33,
        Quarter = 34,
        Empty = 31,
    }
    public enum FuelVal
    {
        Full = 100,
        ThreeQuarters = 72,
        Half = 52,
        Quarter = 24,
        Empty = 0,
    }
    public static class FuelResponse
    {
        public static bool Full_Recv { get; set; }
        public static byte Full_Result { get; set; }

        public static bool ThreeQuarters_Recv { get; set; }
        public static bool Half_Recv { get; set; }
        public static bool Quarter_Recv { get; set; }
        public static bool Empty_Recv { get; set; }

        public static bool Full_Sent { get; set; }
        public static bool ThreeQuarters_Sent { get; set; }
        public static bool Half_Sent { get; set; }
        public static bool Quarter_Sent { get; set; }
        public static bool Empty_Sent { get; set; }


    }
    public static class AnalogInput
    {
        public static byte p { get; set; }
        public static byte fuelResult { get; set; }

        //public static byte p1 { get; set; }
        //public static byte p2 { get; set; }
        //public static byte p3 { get; set; }
        //public static byte InputNumber { get; set; }
        //public static byte RelayNum { get; set; }

        public static float Value { get; set; }
        public static short[] Data = new short[4];
        public static List<byte> AnInputList = new List<byte>();

    }
    public class CASInputParam
    {
        public byte b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19, b20;
    }
    public class InputParams //: IEnumerable
    {
        public int b;
        public byte b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19, b20, b21, b22, b23, b24, b25, b26, b27, b28, b29, b30, b31, b32, b33, b34, b35, b36; //for board1
                                                                                                                                                                                               // public byte b36, b37, b38, b39, b40, b41, b42, b43, b44, b45, b46, b47, b48, b49, b50, b51, b52, b53, b54, b55, b56;
        public byte b2_1, b2_2, b2_3, b2_4, b2_5, b2_6, b2_7, b2_8, b2_9, b2_10, b2_11, b2_12, b2_13, b2_14, b2_30, b2_31, b2_32, b2_33, b2_34, b2_35, b2_36;
        public static byte IBoard1 = 0, IBoard2 = 0;
        public List<byte> InputList1 = new List<byte>();
        public List<byte> InputList2 = new List<byte>();
        public void SetInputParamsB1()
        {
            byte B0 = (byte)((b1 & 0x1) + ((b2 << 1) & 0x2) + ((b3 << 2) & 0x4) + ((b4 << 3) & 0x8) + ((b5 << 4) & 0x10) + ((b6 << 5) & 0x20) + ((b7 << 6) & 0x40) + ((b8 << 7) & 0x80));
            byte B1 = (byte)((b9 & 0x1) + ((b10 << 1) & 0x2) + ((b11 << 2) & 0x4) + ((b12 << 3) & 0x8) + ((b13 << 4) & 0x10) + ((b14 << 5) & 0x20) + ((b15 << 6) & 0x40) + ((b16 << 7) & 0x80));
            byte B2 = (byte)((b17 & 0x1) + ((b18 << 1) & 0x2) + ((b19 << 2) & 0x4) + ((b20 << 3) & 0x8) + ((b21 << 4) & 0x10) + ((b22 << 5) & 0x20) + ((b23 << 6) & 0x40) + ((b24 << 7) & 0x80));
            byte B3 = (byte)((b25 & 0x1) + ((b26 << 1) & 0x2) + ((b27 << 2) & 0x4) + ((b28 << 3) & 0x8) + ((b29 << 4) & 0x10) + ((b30 << 5) & 0x20) + ((b31 << 6) & 0x40) + ((b32 << 7) & 0x80));
            byte B4 = (byte)((b33 & 0x1) + ((b34 << 1) & 0x2) + ((b35 << 2) & 0x4) + ((b36 << 3) & 0x8));

            InputList1.Clear();
            InputList1.Add(B4);
            InputList1.Add(B3);
            InputList1.Add(B2);
            InputList1.Add(B1);
            InputList1.Add(B0);



        }
        public void SetInputParamsB2()
        {
            byte B0 = (byte)((b2_1 & 0x1) + ((b2_2 << 1) & 0x2) + ((b2_3 << 2) & 0x4) + ((b2_4 << 3) & 0x8) + ((b2_5 << 4) & 0x10) + ((b2_6 << 5) & 0x20) + ((b2_7 << 6) & 0x40) + ((b2_8 << 7) & 0x80));
            byte B1 = (byte)((b2_9 & 0x1) + ((b2_10 << 1) & 0x2) + ((b2_11 << 2) & 0x4) + ((b2_12 << 3) & 0x8) + ((b2_13 << 4) & 0x10) + ((b2_14 << 5) & 0x20) + ((b2_31 << 6) & 0x40) + ((b2_32 << 7) & 0x80));
            byte B2 = (byte)((b2_33 & 0x1) + ((b2_34 << 1) & 0x2) + ((b2_35 << 2) & 0x4));
            byte B3 = 0;
            byte B4 = 0;

            InputList2.Clear();
            InputList2.Add(B4);
            InputList2.Add(B3);
            InputList2.Add(B2);
            InputList2.Add(B1);
            InputList2.Add(B0);
        }


    }

    public class OutPutParams
    {

        public byte b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13,       //OutPut1
                    b14, b15, b16, b17, b18, b19, b20, b21, b22, b23, b24, b25, b26, b27,   //Output2
                    b28, b29, b30, b31, b32, b33, b34, b35, b36, b37, b38, b39, b40, b41;  //Output3
        public byte db0, db1, db2, db3, db4, db5, db6, db7, db8, db9, db10, db11, db12, db13,      //Digital
                 db14, db15, db16, db17, db18, db19, db20, db21, db22, db23, db24, db25, db26, db27,
                 db28, db29, db30, db31, hs;

        public byte OBoard1 = 0, OBoard2 = 0, OBoard3 = 0;

        public List<byte> OutputList1 = new List<byte>();
        public List<byte> OutputList2 = new List<byte>();
        public List<byte> OutputList3 = new List<byte>();
        public List<byte> DigitalOutputList = new List<byte>();

        public List<byte> OutputTimingList1 = new List<byte>();
        public List<byte> OutputTimingList2 = new List<byte>();
        public List<byte> OutputTimingList3 = new List<byte>();



        public byte outputNumber { get; set; }
        public float current { get; set; }
        public void SetOutputParamsO1()
        {
            byte B0 = (byte)((b0 & 0x1) + ((b1 << 1) & 0x2) + ((b2 << 2) & 0x4) + ((b3 << 3) & 0x8) + ((b4 << 4) & 0x10) + ((b5 << 5) & 0x20) + ((b6 << 6) & 0x40) + ((b7 << 7) & 0x80));
            byte B1 = (byte)((b8 & 0x1) + ((b9 << 1) & 0x2) + ((b10 << 2) & 0x4) + ((b11 << 3) & 0x8) + ((b12 << 4) & 0x10) + ((b13 << 5) & 0x20));// + ((b14 << 6) & 0x40) + ((b15 << 7) & 0x80));

            OutputList1.Clear();
            OutputList1.Add(B1);
            OutputList1.Add(B0);

        }
        public void SetOutputParamsO2()
        {
            byte B0 = (byte)((b14 & 0x1) + ((b15 << 1) & 0x2) + ((b16 << 2) & 0x4) + ((b17 << 3) & 0x8) + ((b18 << 4) & 0x10) + ((b19 << 5) & 0x20) + ((b20 << 6) & 0x40) + ((b21 << 7) & 0x80));
            byte B1 = (byte)((b22 & 0x1) + ((b23 << 1) & 0x2) + ((b24 << 2) & 0x4) + ((b25 << 3) & 0x8) + ((b26 << 4) & 0x10) + ((b27 << 5) & 0x20));// + ((b14 << 6) & 0x40) + ((b15 << 7) & 0x80));

            OutputList2.Clear();
            OutputList2.Add(B1);
            OutputList2.Add(B0);

        }
        public void SetOutputParamsO3()
        {
            byte B0 = (byte)((b28 & 0x1) + ((b29 << 1) & 0x2) + ((b30 << 2) & 0x4) + ((b31 << 3) & 0x8) + ((b32 << 4) & 0x10) + ((b33 << 5) & 0x20) + ((b34 << 6) & 0x40) + ((b35 << 7) & 0x80));
            byte B1 = (byte)((b36 & 0x1) + ((b37 << 1) & 0x2) + ((b38 << 2) & 0x4) + ((b39 << 3) & 0x8) + ((b40 << 4) & 0x10) + ((b41 << 5) & 0x20));// + ((b14 << 6) & 0x40) + ((b15 << 7) & 0x80));

            OutputList3.Clear();
            OutputList3.Add(B1);
            OutputList3.Add(B0);

        }

        public void SetDigitalOutput(byte highSide)
        {
            //hs = (byte)(highSide ? 1 : 0);
            byte B0 = (byte)((db0 & 0x1) + ((db1 << 1) & 0x2) + ((db2 << 2) & 0x4) + ((db3 << 3) & 0x8) + ((db4 << 4) & 0x10) + ((db5 << 5) & 0x20) + ((db6 << 6) & 0x40) + ((db7 << 7) & 0x80));
            byte B1 = (byte)((db8 & 0x1) + ((db9 << 1) & 0x2) + ((db10 << 2) & 0x4) + ((db11 << 3) & 0x8) + ((db12 << 4) & 0x10) + ((db13 << 5) & 0x20) + ((db14 << 6) & 0x40) + ((highSide << 7) & 0x80));

            //byte B2 = (byte)((db16 & 0x1) + ((db17 << 1) & 0x2) + ((db18 << 2) & 0x4) + ((db19 << 3) & 0x8) + ((db20 << 4) & 0x10) + ((db21 << 5) & 0x20) + ((db22 << 6) & 0x40) + ((db23 << 7) & 0x80));
            //byte B3 = (byte)((db24 & 0x1) + ((db25 << 1) & 0x2) + ((db26 << 2) & 0x4) + ((db27 << 3) & 0x8) + ((db28 << 4) & 0x10) + ((db29 << 5) & 0x20) + ((db30 << 6) & 0x40) + ((db31 << 7) & 0x80));

            DigitalOutputList.Clear();
            DigitalOutputList.Add(B1);
            DigitalOutputList.Add(B0);

        }
    }

    public class PowerParams
    {
        public byte Number { get; set; }
        public float current { get; set; }
        public double temprature { get; set; }
        public List<byte> PowerList = new List<byte>();
    }
    public static class TestMode
    {
        public static bool Auto { get; set; }
        public static bool Manual { get; set; }

    }
    public static class CASInput
    {
        public static byte InpSt1 { get; set; }
        public static byte InpSt2 { get; set; }
        public static byte InpSt3 { get; set; }
        public static byte InpSt4 { get; set; }
        public static byte InpSt5 { get; set; }
        public static byte InpSt6 { get; set; }
        public static byte InpSt7 { get; set; }
    }
    public static class AutoTest
    {
        public static bool InputRec { get; set; }
        public static byte InputRecevd { get; set; }
        public static bool Input2Rec { get; set; }
        public static bool Output1Rec { get; set; }
        public static bool Output2Rec { get; set; }
        public static bool Output3Rec { get; set; }
        public static bool OutputDRec { get; set; }
        public static bool PowerRec { get; set; }
        public static bool NetworkRec { get; set; }
        public static byte CanHRec { get; set; }
        public static bool CASRec { get; set; }
        public static bool CASTestPass { get; set; }
        public static bool CasTestPassed { get; set; }
        public static bool InputRec2 { get; set; }
       
        public static int LoadTest1B3Cnt { get; set; }
        public static bool LoadTest2B3Rec { get; set; }


        //
        public static bool InputPass { get; set; }
        public static bool Input2Pass { get; set; }
        public static bool OutputPass { get; set; }
        public static bool Output1Pass { get; set; }
        public static bool Output2Pass { get; set; }
        public static bool Output3Pass { get; set; }
        public static bool OutputDPass { get; set; }
        public static bool PowerPass { get; set; }
        public static bool PowerAllPass { get; set; } = false;
        public static bool NetworkPass { get; set; }
    
        public static bool AnalogInpPass { get; set; }
        public static bool CasInputPass { get; set; }
        public static bool AutoTestPass { get; set; }
        public static int LoadTest1Passed { get; set; }
        public static int LoadTest2Passed { get; set; }
        public static bool Output1_Ok { get; set; }
        public static bool Output2_Ok { get; set; }
        public static bool Output3_Ok { get; set; }
        public static bool OutputD_Ok { get; set; }

        //
        public static int InputFail { get; set; }
        public static int CASInputOk { get; set; }
        public static int CasTestCountr { get; set; }

        public static int DigitalOutputFail { get; set; }
        public static int AnalogInputOk { get; set; }

        public static int Output1Failed { get; set; }
        public static int Output2Failed { get; set; }
        public static int Output3Failed { get; set; }
        public static int PowerFail { get; set; }

        public static int NetworkPassed { get; set; }
        public static int PowerRecev { get; set; }
        public static bool LoadTest2Rec { get; set; }
        public static bool LoadTest1Pass { get; set; }
        public static bool LoadTest2Pass { get; set; }
        public static bool printrFlag { get; set; } = false;
        public static Color resultBoxColor { get; set; }
        public static string resultBoxText { get; set; }
        public static int DigitalOutputOk { get; set; }
        public static string SaveReportPath { get; set; } = @" E:\IKBCM_Tester";// Directory.GetCurrentDirectory();//+ @"\Reports";
        public static bool CrashUnlockRec { get; set; }
        public static byte AutoManRecv { get; set; }
        public static bool TstRepeat { get; set; }
        public static string AutoTestResult { get; set; }
        public static string AutoTestResult2 { get; set; }
        public static string InputCheckd { get; set; }
        public static string InputShort { get; set; }
        public static string InputMsg { get; set; }
        public static int tstCnt { get; set; }

        public static void ResetResult()
        {
            InputPass = false;
            Input2Pass = false;

            Output1_Ok = false;
            Output2_Ok = false;
            Output3_Ok = false;
            OutputD_Ok = false;

            PowerPass = false;
            NetworkPass = false;
            AutoTestPass = false;
            
            OutputPass = false; //
            Output1Pass = false;
            Output2Pass = false;
            Output3Pass = false;
            OutputDPass = false;

            LoadTest1Pass = false;
            LoadTest2Pass = false;
            LoadTest2B3Rec = false;

            CasInputPass = false;
            CasTestPassed = false;
            AnalogInpPass = false;

           

            NetworkPassed = 0;
            Output1Failed = 0;
            Output2Failed = 0;
            Output3Failed = 0;
            DigitalOutputFail = 0;
            LoadTest1Passed = 0;
            LoadTest2Passed = 0;
            LoadTest1B3Cnt = 0;

            InputFail = 0;
            CASInputOk = 0;
            CasTestCountr = 0;
            DigitalOutputFail = 0;
            AnalogInputOk = 0;

            InputMsg = "";
            AutoTest.AutoTestResult = "";
            AutoTest.AutoTestResult2 = "";
           
            AutoManRecv = 0;
        }

    }

    public static class lbl
    {
        public static string Printer_Name { get; set; } = "TSC TE200";

        public static string label_Width { get; set; } = "55";
        public static string label_Height { get; set; } = "15";
        public static string label_Speed { get; set; } = "4";
        public static string label_Density { get; set; } = "8";
        public static string label_Sensor { get; set; } = "0";
        public static string label_Vertical { get; set; } = "3";
        public static string label_Offset { get; set; } = "0";
        public static int printr_dpi { get; set; } = 200;

        //
        public static string barcd_x { get; set; } = "56";
        public static string barcd_y { get; set; } = "32";
        public static string barcd_type { get; set; } = "128";
        public static string barcd_Height { get; set; } = "48";
        public static string barcd_readble { get; set; } = "1";
        public static string barcd_rotation { get; set; } = "0";
        public static string barcd_narrow { get; set; } = "2";
        public static string barcd_wide { get; set; } = "2";
        public static string barcd_content { get; set; } = "RU8K1ES";

        //windows font pass
        public static int wfp_x { get; set; } = 56;
        public static int wfp_y { get; set; } = 8;
        public static int wfp_fontHeight { get; set; } = 20;
        public static int wfp_rotation { get; set; } = 0;
        public static int wfp_fontstyle { get; set; } = 2;
        public static int wfp_underline { get; set; } = 0;
        public static string wfp_font { get; set; } = "Arial";
        public static string wfp_content { get; set; } = "IKBCM - EKS co.";
        //windows font fail
        public static int wff_x { get; set; } = 56;
        public static int wff_y { get; set; } = 32;
        public static int wff_fontHeight { get; set; } = 32;
        public static int wff_rotation { get; set; } = 0;
        public static int wff_fontstyle { get; set; } = 2;
        public static int wff_underline { get; set; } = 1;
        public static string wff_font { get; set; } = "Arial";
        public static string wff_content { get; set; } = "Test Fail : ";
        //

    }
    public class NetParams
    {
        public byte BCM_CANHS { get; set; }
        public byte BCM_CANLS { get; set; }
        public byte CAS_CANLS { get; set; }
        public byte BCM_LinRear { get; set; }
        public byte BCM_LinFront { get; set; }

        public byte IMMO_LIN { get; set; }
        public byte Kick_LIN { get; set; }
        public byte TPMS { get; set; }
        public byte RKE { get; set; }
        public byte LF { get; set; }
        public byte R_DHS_Lock { get; set; }
        public byte R_DHS_UnLock { get; set; }
        public byte L_DHS_Lock { get; set; }
        public byte L_DHS_UnLock { get; set; }


        public long randomNum { get; set; }
        public byte[] DataCANH { get; set; }
        public byte[] DataCANL { get; set; }
        public byte[] DataCASCANL { get; set; }

        public byte[] DataLinF { get; set; }
        public byte[] DataLinR { get; set; }
        public byte CanHFail { get; set; }
        public byte CanLFail { get; set; }
        public byte LinFFail { get; set; }
        public byte LinRFail { get; set; }
        public byte Mode { get; set; }
        public List<byte> Data { get; set; }





    }
    public class InfoParams
    {
        public EcuType ECUType { get; set; }
        public byte EnvironmentTemp { get; set; }
        public byte OverTemp { get; set; }
        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public string ProductionDate { get; set; }



    }
    public static class PacketView
    {
        public static string data;
        // public static List<string> packet=new List<string>();
    }
    public static class thr
    {
        public static List<int> outputsTiming1 = new List<int>() { 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, };
        public static List<int> outputsTiming2 = new List<int>() { 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, };
        public static List<int> outputsTiming3 = new List<int>() { 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, 250, };
        //min
        public static float outOne1 { get; set; } = 0;
        public static float outOne2 { get; set; } = 0;
        public static float outOne3 { get; set; } = 0;
        public static float outOne4 { get; set; } = 0;
        public static float outOne5 { get; set; } = 0;
        public static float outOne6 { get; set; } = 0;
        public static float outOne7 { get; set; } = 0;
        public static float outOne8 { get; set; } = 0;
        public static float outOne9 { get; set; } = 0;
        public static float outOne10 { get; set; } = 0;
        public static float outOne11 { get; set; } = 0;
        public static float outOne12 { get; set; } = 0;
        public static float outOne13 { get; set; } = 0;
        public static float outOne14 { get; set; } = 0;
        //
        public static float outOne1Mx { get; set; } = 3;
        public static float outOne2Mx { get; set; } = 3;
        public static float outOne3Mx { get; set; } = 3;
        public static float outOne4Mx { get; set; } = 3;
        public static float outOne5Mx { get; set; } = 3;
        public static float outOne6Mx { get; set; } = 3;
        public static float outOne7Mx { get; set; } = 3;
        public static float outOne8Mx { get; set; } = 3;
        public static float outOne9Mx { get; set; } = 3;
        public static float outOne10Mx { get; set; } = 3;
        public static float outOne11Mx { get; set; } = 3;
        public static float outOne12Mx { get; set; } = 3;
        public static float outOne13Mx { get; set; } = 3;
        public static float outOne14Mx { get; set; } = 3;
        //
        public static float outTwo1 { get; set; } = 0;
        public static float outTwo2 { get; set; } = 0;
        public static float outTwo3 { get; set; } = 0;
        public static float outTwo4 { get; set; } = 0;
        public static float outTwo5 { get; set; } = 0;
        public static float outTwo6 { get; set; } = 0;
        public static float outTwo7 { get; set; } = 0;
        public static float outTwo8 { get; set; } = 0;
        public static float outTwo9 { get; set; } = 0;
        public static float outTwo10 { get; set; } = 0;
        public static float outTwo11 { get; set; } = 0;
        public static float outTwo12 { get; set; } = 0;
        public static float outTwo13 { get; set; } = 0;
        public static float outTwo14 { get; set; } = 0;
        //
        public static float outTwo1Mx { get; set; } = 6;
        public static float outTwo2Mx { get; set; } = 6;
        public static float outTwo3Mx { get; set; } = 6;
        public static float outTwo4Mx { get; set; } = 6;
        public static float outTwo5Mx { get; set; } = 6;
        public static float outTwo6Mx { get; set; } = 6;
        public static float outTwo7Mx { get; set; } = 6;
        public static float outTwo8Mx { get; set; } = 6;
        public static float outTwo9Mx { get; set; } = 6;
        public static float outTwo10Mx { get; set; } = 6;
        public static float outTwo11Mx { get; set; } = 6;
        public static float outTwo12Mx { get; set; } = 6;
        public static float outTwo13Mx { get; set; } = 6;
        public static float outTwo14Mx { get; set; } = 6;
        //                                          
        public static float outThree1 { get; set; } = 0;
        public static float outThree2 { get; set; } = 0;
        public static float outThree3 { get; set; } = 0;
        public static float outThree4 { get; set; } = 0;
        public static float outThree5 { get; set; } = 0;
        public static float outThree6 { get; set; } = 0;
        public static float outThree7 { get; set; } = 0;
        public static float outThree8 { get; set; } = 0;
        public static float outThree9 { get; set; } = 0;
        public static float outThree10 { get; set; } = 0;
        public static float outThree11 { get; set; } = 0;
        public static float outThree12 { get; set; } = 0;
        public static float outThree13 { get; set; } = 0;
        public static float outThree14 { get; set; } = 0;
        //
        public static float outThree1Mx { get; set; } = 16;
        public static float outThree2Mx { get; set; } = 20;
        public static float outThree3Mx { get; set; } = 10;
        public static float outThree4Mx { get; set; } = 10;
        public static float outThree5Mx { get; set; } = 10;
        public static float outThree6Mx { get; set; } = 10;
        public static float outThree7Mx { get; set; } = 10;
        public static float outThree8Mx { get; set; } = 10;
        public static float outThree9Mx { get; set; } = 10;
        public static float outThree10Mx { get; set; } = 10;
        public static float outThree11Mx { get; set; } = 10;
        public static float outThree12Mx { get; set; } = 10;
        public static float outThree13Mx { get; set; } = 10;
        public static float outThree14Mx { get; set; } = 10;
        //
        public static float power1 { get; set; } = 4;
        public static float power2 { get; set; } = 4;
        public static float power3 { get; set; } = 4;
        public static float power4 { get; set; } = 4;
        public static float power5 { get; set; } = 4;
        public static float power6 { get; set; } = 4;
        public static float power7 { get; set; } = 4;
        public static float power8 { get; set; } = 4;
        public static float power9 { get; set; } = 4;
        public static float power10 { get; set; } = 4;
        public static float power11 { get; set; } = 4;
        public static float power12 { get; set; } = 4;
        public static float power13 { get; set; } = 4;
        public static float power14 { get; set; } = 4;
        public static float power15 { get; set; } = 4;
        public static float power16 { get; set; } = 4;
        public static float power17 { get; set; } = 4;
        public static float power18 { get; set; } = 4;
        public static float power19 { get; set; } = 4;
        public static float power20 { get; set; } = 4;
        public static float power21 { get; set; } = 4;
        public static float power22 { get; set; } = 4;


    }
}
