using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Data.SqlClient;


namespace TesterIKBCM
{
    public partial class XtraReport1 : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraReport1()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection();
        SqlCommand sql_command;//= new SqlCommand();
        public static class SetParam
        {
            public static string input1 { get; set; }
            public static string input2 { get; set; }
        }
        public void ReportGetData(string date, string time, string name)
        {
            xrDate.Text = date;   // + "\r\n";
            xrTime.Text = time;
            xrUser.Text = name;

            //xrInp1.Text = SetParam.input1;

        }
        public void reportGetData3(string result1, string result2, string result3)
        {
            try
            {
                autoResult1.Text = result1;
                autoResult2.Text = result2;
                autoResult3.Text = result3;


            }
            catch (Exception ex)
            {

            }
        }
        public void reportGetData2(byte[] network,  byte[] input, byte[] output, string[] outputCur, string[] inputVolt)
        {
            try
            {
                //foreach (var item in input)
                //{

                //    xrInput1.Text += " _ " + item + "\r\n" + "\r\n";
                //}
                //foreach (var item in output)
                //{
                //    xrOutput.Text += " _ " + item + "\r\n" + "\r\n";
                //}

                //Network Params
                xrNC1.Checked = network[0] == 1 ? true : false;        
                xrNC2.Checked = network[1] == 1 ? true : false;
                xrNC3.Checked = network[2] == 1 ? true : false;
                xrNC4.Checked = network[3] == 1 ? true : false;
                xrNC5.Checked = network[4] == 1 ? true : false;
                xrNC6.Checked = network[5] == 1 ? true : false;
                xrNC7.Checked = network[6] == 1 ? true : false;
                xrNC8.Checked = network[7] == 1 ? true : false;
                xrNC9.Checked = network[8] == 1 ? true : false;
                xrNC10.Checked = network[9] == 1 ? true : false;
                xrNC11.Checked = network[10] == 1 ? true : false;
                xrNC12.Checked = network[11] == 1 ? true : false;
                xrNC13.Checked = network[12] == 1 ? true : false;
                xrNC14.Checked = network[13] == 1 ? true : false;


                xrNC1.BackColor = network[0] == 1 ? Color.Green : Color.Red;
                xrNC2.BackColor = network[1] == 1 ? Color.Green : Color.Red;
                xrNC3.BackColor = network[2] == 1 ? Color.Green : Color.Red;
                xrNC4.BackColor = network[3] == 1 ? Color.Green : Color.Red;
                xrNC5.BackColor = network[4] == 1 ? Color.Green : Color.Red;
                xrNC6.BackColor = network[5] == 1 ? Color.Green : Color.Red;
                xrNC7.BackColor = network[6] == 1 ? Color.Green : Color.Red;
                xrNC8.BackColor = network[7] == 1 ? Color.Green : Color.Red;
                xrNC9.BackColor = network[8] == 1 ? Color.Green : Color.Red;
                xrNC10.BackColor = network[9] == 1 ? Color.Green : Color.Red;
                xrNC11.BackColor = network[10] == 1 ? Color.Green : Color.Red;
                xrNC12.BackColor = network[11] == 1 ? Color.Green : Color.Red;
                xrNC13.BackColor = network[12] == 1 ? Color.Green : Color.Red;
                xrNC14.BackColor = network[13] == 1 ? Color.Green : Color.Red;

                //Input
                xrInC1.Checked = input[0] == 1 ? true : false;
                xrInC2.Checked = input[1] == 1 ? true : false;
                xrInC3.Checked = input[2] == 1 ? true : false;
                xrInC4.Checked = input[3] == 1 ? true : false;
                xrInC5.Checked = input[4] == 1 ? true : false;
                xrInC6.Checked = input[5] == 1 ? true : false;
                xrInC7.Checked = input[6] == 1 ? true : false;
                xrInC8.Checked = input[7] == 1 ? true : false;
                xrInC9.Checked = input[8] == 1 ? true : false;
                xrInC10.Checked = input[9] == 1 ? true : false;
                xrInC11.Checked = input[10] == 1 ? true : false;
                xrInC12.Checked = input[11] == 1 ? true : false;
                xrInC13.Checked = input[12] == 1 ? true : false;
                xrInC14.Checked = input[13] == 1 ? true : false;
                xrInC15.Checked = input[14] == 1 ? true : false;
                xrInC16.Checked = input[15] == 1 ? true : false;
                xrInC17.Checked = input[16] == 1 ? true : false;
                xrInC18.Checked = input[17] == 1 ? true : false;
                xrInC19.Checked = input[18] == 1 ? true : false;
                xrInC20.Checked = input[19] == 1 ? true : false;
                xrInC21.Checked = input[20] == 1 ? true : false;
                xrInC22.Checked = input[21] == 1 ? true : false;
                xrInC23.Checked = input[22] == 1 ? true : false;
                xrInC24.Checked = input[23] == 1 ? true : false;
                xrInC25.Checked = input[24] == 1 ? true : false;
                xrInC26.Checked = input[25] == 1 ? true : false;
                xrInC27.Checked = input[26] == 1 ? true : false;
                xrInC28.Checked = input[27] == 1 ? true : false;
                xrInC29.Checked = input[28] == 1 ? true : false;
                xrInC30.Checked = input[29] == 1 ? true : false;
                xrInC31.Checked = input[30] == 1 ? true : false;
                xrInC32.Checked = input[31] == 1 ? true : false;
                xrInC33.Checked = input[32] == 1 ? true : false;
                xrInC34.Checked = input[33] == 1 ? true : false;
                xrInC35.Checked = input[34] == 1 ? true : false;
                xrInC36.Checked = input[35] == 1 ? true : false;
                xrInC37.Checked = input[36] == 1 ? true : false;
                xrInC38.Checked = input[37] == 1 ? true : false;
                xrInC39.Checked = input[38] == 1 ? true : false;
                xrInC40.Checked = input[39] == 1 ? true : false;
                xrInC41.Checked = input[40] == 1 ? true : false;
                //
                xrCI1C.Checked = input[41] == 1 ? true : false;
                xrCI2C.Checked = input[42] == 1 ? true : false;
                xrCI3C.Checked = input[43] == 1 ? true : false;
                xrCI4C.Checked = input[44] == 1 ? true : false;
                xrCI5C.Checked = input[45] == 1 ? true : false;
                xrCI6C.Checked = input[46] == 1 ? true : false;
                xrCI7C.Checked = input[47] == 1 ? true : false;

                xrAI1C.Checked = input[48] == 1 ? true : false;
                

                xrInC1.BackColor = input[0] == 1 ? Color.Green : Color.Red;
                xrInC2.BackColor = input[1] == 1 ? Color.Green : Color.Red;
                xrInC3.BackColor = input[2] == 1 ? Color.Green : Color.Red;
                xrInC4.BackColor = input[3] == 1 ? Color.Green : Color.Red;
                xrInC5.BackColor = input[4] == 1 ? Color.Green : Color.Red;
                xrInC6.BackColor = input[5] == 1 ? Color.Green : Color.Red;
                xrInC7.BackColor = input[6] == 1 ? Color.Green : Color.Red;
                xrInC8.BackColor = input[7] == 1 ? Color.Green : Color.Red;
                xrInC9.BackColor = input[8] == 1 ? Color.Green : Color.Red;
                xrInC10.BackColor = input[9] == 1 ? Color.Green : Color.Red;
                xrInC11.BackColor = input[10] == 1 ? Color.Green : Color.Red;
                xrInC12.BackColor = input[11] == 1 ? Color.Green : Color.Red;
                xrInC13.BackColor = input[12] == 1 ? Color.Green : Color.Red;
                xrInC14.BackColor = input[13] == 1 ? Color.Green : Color.Red;
                xrInC15.BackColor = input[14] == 1 ? Color.Green : Color.Red;
                xrInC16.BackColor = input[15] == 1 ? Color.Green : Color.Red;
                xrInC17.BackColor = input[16] == 1 ? Color.Green : Color.Red;
                xrInC18.BackColor = input[17] == 1 ? Color.Green : Color.Red;
                xrInC19.BackColor = input[18] == 1 ? Color.Green : Color.Red;
                xrInC20.BackColor = input[19] == 1 ? Color.Green : Color.Red;
                xrInC21.BackColor = input[20] == 1 ? Color.Green : Color.Red;
                xrInC22.BackColor = input[21] == 1 ? Color.Green : Color.Red;
                xrInC23.BackColor = input[22] == 1 ? Color.Green : Color.Red;
                xrInC24.BackColor = input[23] == 1 ? Color.Green : Color.Red;
                xrInC25.BackColor = input[24] == 1 ? Color.Green : Color.Red;
                xrInC26.BackColor = input[25] == 1 ? Color.Green : Color.Red;
                xrInC27.BackColor = input[26] == 1 ? Color.Green : Color.Red;
                xrInC28.BackColor = input[27] == 1 ? Color.Green : Color.Red;
                xrInC29.BackColor = input[28] == 1 ? Color.Green : Color.Red;
                xrInC30.BackColor = input[29] == 1 ? Color.Green : Color.Red;
                xrInC31.BackColor = input[30] == 1 ? Color.Green : Color.Red;
                xrInC32.BackColor = input[31] == 1 ? Color.Green : Color.Red;
                xrInC33.BackColor = input[32] == 1 ? Color.Green : Color.Red;
                xrInC34.BackColor = input[33] == 1 ? Color.Green : Color.Red;
                xrInC35.BackColor = input[34] == 1 ? Color.Green : Color.Red;
                xrInC36.BackColor = input[35] == 1 ? Color.Green : Color.Red;
                xrInC37.BackColor = input[36] == 1 ? Color.Green : Color.Red;
                xrInC38.BackColor = input[37] == 1 ? Color.Green : Color.Red;
                xrInC39.BackColor = input[38] == 1 ? Color.Green : Color.Red;
                xrInC40.BackColor = input[39] == 1 ? Color.Green : Color.Red;
                xrInC41.BackColor = input[40] == 1 ? Color.Green : Color.Red;

                xrCI1C.BackColor = input[41] == 1 ? Color.Green : Color.Red;
                xrCI2C.BackColor = input[42] == 1 ? Color.Green : Color.Red;
                xrCI3C.BackColor = input[43] == 1 ? Color.Green : Color.Red;
                xrCI4C.BackColor = input[44] == 1 ? Color.Green : Color.Red;
                xrCI5C.BackColor = input[45] == 1 ? Color.Green : Color.Red;
                xrCI6C.BackColor = input[46] == 1 ? Color.Green : Color.Red;
                xrCI7C.BackColor = input[47] == 1 ? Color.Green : Color.Red;

                xrAI1C.BackColor = input[48] == 1 ? Color.Green : Color.Red;

                //Output                
                xrOC1.Text = outputCur[0];
                xrOC1.BackColor = output[0] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC2.Text = outputCur[1];
                xrOC2.BackColor = output[1] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC3.Text = outputCur[2];
                xrOC3.BackColor = output[2] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC4.Text = outputCur[3];
                xrOC4.BackColor = output[3] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC5.Text = outputCur[4];
                xrOC5.BackColor = output[4] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC6.Text = outputCur[5];
                xrOC6.BackColor = output[5] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC7.Text = outputCur[6];
                xrOC7.BackColor = output[6] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC8.Text = outputCur[7];
                xrOC8.BackColor = output[7] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC9.Text = outputCur[8];
                xrOC9.BackColor = output[8] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC10.Text = outputCur[9];
                xrOC10.BackColor = output[9] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC11.Text = outputCur[10];
                xrOC11.BackColor = output[10] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC12.Text = outputCur[11];
                xrOC12.BackColor = output[11] == 1 ? Color.YellowGreen : Color.Pink;
                
                xrOC13.Text = outputCur[12];
                xrOC13.BackColor = output[12] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC14.Text = outputCur[13];
                xrOC14.BackColor = output[13] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC15.Text = outputCur[14];
                xrOC15.BackColor = output[14] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC16.Text = outputCur[15];
                xrOC16.BackColor = output[15] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC17.Text = outputCur[16];
                xrOC17.BackColor = output[16] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC18.Text = outputCur[17];
                xrOC18.BackColor = output[17] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC19.Text = outputCur[18];
                xrOC19.BackColor = output[18] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC20.Text = outputCur[19];
                xrOC20.BackColor = output[19] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC21.Text = outputCur[20];
                xrOC21.BackColor = output[20] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC22.Text = outputCur[21];
                xrOC22.BackColor = output[21] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC23.Text = outputCur[22];
                xrOC23.BackColor = output[22] == 1 ? Color.YellowGreen : Color.Pink;
               
                xrOC24.Text = outputCur[23];
                xrOC24.BackColor = output[23] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC25.Text = outputCur[24];
                xrOC25.BackColor = output[24] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC26.Text = outputCur[25];
                xrOC26.BackColor = output[25] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC27.Text = outputCur[26];
                xrOC27.BackColor = output[26] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC28.Text = outputCur[27];
                xrOC28.BackColor = output[27] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC29.Text = outputCur[28];
                xrOC29.BackColor = output[28] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC30.Text = outputCur[29];
                xrOC30.BackColor = output[29] == 1 ? Color.YellowGreen : Color.Pink;

                xrOC31.Text = outputCur[30];
                xrOC31.BackColor = output[30] == 1 ? Color.YellowGreen : Color.Pink;
                //CAS Output
                xrCO1C.Text = outputCur[31];
                xrCO1C.BackColor = output[31] == 1 ? Color.YellowGreen : Color.Pink;

                xrCO2C.Text = outputCur[32];
                xrCO2C.BackColor = output[32] == 1 ? Color.YellowGreen : Color.Pink;

                //Digital Output
                xrDO1C.Checked = output[33] == 1 ? true : false;
                xrDO1C.BackColor = output[33] == 1 ? Color.Green : Color.Red;

                xrDO2C.Checked = output[34] == 1 ? true : false;
                xrDO2C.BackColor = output[34] == 1 ? Color.Green : Color.Red;

                xrDO3C.Checked = output[35] == 1 ? true : false;
                xrDO3C.BackColor = output[35] == 1 ? Color.Green : Color.Red;

                xrDO4C.Checked = output[36] == 1 ? true : false;
                xrDO4C.BackColor = output[36] == 1 ? Color.Green : Color.Red;

                xrDO5C.Checked = output[37] == 1 ? true : false;
                xrDO5C.BackColor = output[37] == 1 ? Color.Green : Color.Red;

                xrDO6C.Checked = output[38] == 1 ? true : false;
                xrDO6C.BackColor = output[38] == 1 ? Color.Green : Color.Red;

                xrDO7C.Checked = output[39] == 1 ? true : false;
                xrDO7C.BackColor = output[39] == 1 ? Color.Green : Color.Red;

                xrDO8C.Checked = output[40] == 1 ? true : false;
                xrDO8C.BackColor = output[40] == 1 ? Color.Green : Color.Red;

                xrDO9C.Checked = output[41] == 1 ? true : false;
                xrDO9C.BackColor = output[41] == 1 ? Color.Green : Color.Red;

                xrDO10C.Checked = output[42] == 1 ? true : false;
                xrDO10C.BackColor = output[42] == 1 ? Color.Green : Color.Red;

                //CAS Digital Output
                xrCDO1C.Checked = output[43] == 1 ? true : false;
                xrCDO1C.BackColor = output[43] == 1 ? Color.Green : Color.Red;

                xrCDO2C.Checked = output[44] == 1 ? true : false;
                xrCDO2C.BackColor = output[44] == 1 ? Color.Green : Color.Red;
                
                xrCDO3C.Checked = output[45] == 1 ? true : false;
                xrCDO3C.BackColor = output[45] == 1 ? Color.Green : Color.Red;

                xrCDO4C.Checked = output[46] == 1 ? true : false;
                xrCDO4C.BackColor = output[46] == 1 ? Color.Green : Color.Red;

            }
            catch (Exception e)
            {

            }

        }
        public void SetReportNames(string[] Net, string[] input, string[] output)
        {
            xrN1.Text = Net[0];
            xrN2.Text = Net[1];
            xrN3.Text = Net[2];
            xrN4.Text = Net[3];
            xrN5.Text = Net[4];
            xrN6.Text = Net[5];
            xrN7.Text = Net[6];
            xrN8.Text = Net[7];
            xrN9.Text = Net[8];
            xrN10.Text = Net[9];
            xrN11.Text = Net[10];
            xrN12.Text = Net[11];
            xrN13.Text = Net[12];
            xrN14.Text = Net[13];

            //
            xrIn1.Text = input[0];
            xrIn2.Text = input[1];
            xrIn3.Text = input[2];
            xrIn4.Text = input[3];
            xrIn5.Text = input[4];
            xrIn6.Text = input[5];
            xrIn7.Text = input[6];
            xrIn8.Text = input[7];
            xrIn9.Text = input[8];
            xrIn10.Text = input[9];
            xrIn11.Text = input[10];
            xrIn12.Text = input[11];
            xrIn13.Text = input[12];
            xrIn14.Text = input[13];
            xrIn15.Text = input[14];
            xrIn16.Text = input[15];
            xrIn17.Text = input[16];
            xrIn18.Text = input[17];
            xrIn19.Text = input[18];
            xrIn20.Text = input[19];
            xrIn21.Text = input[20];
            xrIn22.Text = input[21];
            xrIn23.Text = input[22];
            xrIn24.Text = input[23];
            xrIn25.Text = input[24];
            xrIn26.Text = input[25];
            xrIn27.Text = input[26];
            xrIn28.Text = input[27];
            xrIn29.Text = input[28];    //In1_L29
            xrIn30.Text = input[29];    //In2_L1
            xrIn31.Text = input[30];    //In2_L2
            xrIn32.Text = input[31];
            xrIn33.Text = input[32];
            xrIn34.Text = input[33];

            xrIn35.Text = input[34];
            xrIn36.Text = input[35];
            xrIn37.Text = input[36];
            xrIn38.Text = input[37];
            xrIn39.Text = input[38];
            xrIn40.Text = input[39];   //In2_L11
            xrIn41.Text = input[40];   //In2_H1


            xrCI1.Text = input[41];     //In2_H5
            xrCI2.Text = input[42];     //In2_H2
            xrCI3.Text = input[43];     //In2_H3
            xrCI4.Text = input[44];     //In2_H4
            xrCI5.Text = input[45];     //In2_L12
            xrCI6.Text = input[46];     //In2_L13
            xrCI7.Text = input[47];     //In2_L14

            xrAI1.Text = input[48];   //Analog


            xrO1.Text = output[0];
            xrO2.Text = output[1];
            xrO3.Text = output[2];
            xrO4.Text = output[3];
            xrO5.Text = output[4];
            xrO6.Text = output[5];
            xrO7.Text = output[6];
            xrO8.Text = output[7];
            xrO9.Text = output[8];
            xrO10.Text = output[9];
            xrO11.Text = output[10];
            xrO12.Text = output[11];
            xrO13.Text = output[12];
            xrO14.Text = output[13];   //out1_14
            xrO15.Text = output[14];
            xrO16.Text = output[15];
            xrO17.Text = output[16];
            xrO18.Text = output[17];
            xrO19.Text = output[18];
            xrO20.Text = output[19];
            xrO21.Text = output[20];
            xrO22.Text = output[21];
            xrO23.Text = output[22];
            xrO24.Text = output[23];
            xrO25.Text = output[24];
            xrO26.Text = output[25];
            xrO27.Text = output[26];
            xrO28.Text = output[27];      //out2_14

            xrO29.Text = output[28];     //out3_1
            xrO30.Text = output[29];
            xrO31.Text = output[30];

            xrCO1.Text = output[31];      //outCas
            xrCO2.Text = output[32];     //

            xrDO1.Text = output[33];     //Digital Out
            xrDO2.Text = output[34];
            xrDO3.Text = output[35];
            xrDO4.Text = output[36];
            xrDO5.Text = output[37];
            xrDO6.Text = output[38];
            xrDO7.Text = output[39];
            xrDO8.Text = output[40];
            xrDO9.Text = output[41];
            xrDO10.Text = output[42];

            xrCDO1.Text = output[43];      //Cas Digital Out
            xrCDO2.Text = output[44];
            xrCDO3.Text = output[45];
            xrCDO4.Text = output[46];

        }


        public void loadTestSpecification(string partNum, string trackingNumSt, string testRes, string[] vrsions, string description)
        {
            xrTracking.Text = trackingNumSt;
            testResult.Text = testRes;
            xrBcmSoftwrVersion.Text = vrsions[0];
            // xrHrdwrVersion.Text = hrdwVersion;
            xrBcmHardwareVersion.Text = vrsions[2];// bcmSftwrVersion;
            xrCasSoftwrVersion.Text = vrsions[1];//  casSftwrVersion;

            //xrDescription.Text = "    " + description;
        }

        public XRTable CreateXRTable()
        {
            int cellsInRow = 3;
            int rowsCount = 31;
            float rowHeight = 100f;

            XRTable table = new XRTable();
            table.Borders = DevExpress.XtraPrinting.BorderSide.All;
            table.BeginInit();

            for (int i = 0; i < rowsCount; i++)
            {
                XRTableRow row = new XRTableRow();
                row.HeightF = rowHeight;
                for (int j = 0; j < cellsInRow; j++)
                {
                    XRTableCell cell = new XRTableCell();
                    row.Cells.Add(cell);
                }
                table.Rows.Add(row);
            }

            table.BeforePrint += new PrintEventHandler(table_BeforePrint);
            table.EndInit();
            return table;
        }

        private void table_BeforePrint(object sender, PrintEventArgs e)
        {

            throw new NotImplementedException();
        }

        private void XtraReport1_BeforePrint(object sender, PrintEventArgs e)
        {
            //loadTestSpec();
        }
    }
}
