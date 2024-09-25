using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterIKBCM
{
    public static class ReceiveParser
    {
        public static void ExctractInputDataInputBoard(byte[] data, InputParams outdata)
        {
            try
            {
                outdata.b1 = (byte)(data[6] >> 0 & 0x1);       //Front Wash Pump Switch
                outdata.b2 = (byte)(data[6] >> 1 & 0x1);       //Front High Speed Wiper Switch
                outdata.b3 = (byte)(data[6] >> 2 & 0x1);       //Front Low Speed Wiper Switch
                outdata.b4 = (byte)(data[6] >> 3 & 0x1);        //Front Intermittent Wiper Switch
                outdata.b5 = (byte)(data[6] >> 4 & 0x1);       //Front Wiper Zero Position Switch
                outdata.b6 = (byte)(data[6] >> 5 & 0x1);        //Rear Wash-Wipe Switch
                outdata.b7 = (byte)(data[6] >> 6 & 0x1);       //Front Auto Wiper Switch
                outdata.b8 = (byte)(data[6] >> 7 & 0x1);        //Front Wiper MIST SW

                outdata.b9 = (byte)(data[5] >> 0 & 0x1);       //Rear Intermittent Wiper Switch
                outdata.b10 = (byte)(data[5] >> 1 & 0x1);       //Auto Light Switch
                outdata.b11 = (byte)(data[5] >> 2 & 0x1);            //Hazard Switch
                outdata.b12 = (byte)(data[5] >> 3 & 0x1);         //Side Lamp Switch
                outdata.b13 = (byte)(data[5] >> 4 & 0x1);         //Dipped Lamp Switch
                outdata.b14 = (byte)(data[5] >> 5 & 0x1);         //Main Lamp Switch 
                outdata.b15 = (byte)(data[5] >> 6 & 0x1);         //Front Fog Lamp Switch
                outdata.b16 = (byte)(data[5] >> 7 & 0x1);         //Rear Fog Lamp Switch 

                outdata.b17 = (byte)(data[4] >> 0 & 0x1);          //Head Lamp Passing Light Switch
                outdata.b18 = (byte)(data[4] >> 1 & 0x1);          //RH Indicator Switch
                outdata.b19 = (byte)(data[4] >> 2 & 0x1);          //LH Indicator Switch
                outdata.b20 = (byte)(data[4] >> 3 & 0x1);          //Trip Key Switch
                outdata.b21 = (byte)(data[4] >> 4 & 0x1);          //Horn Switch 
                outdata.b22 = (byte)(data[4] >> 5 & 0x1);          //Master Lock Switch
                outdata.b23 = (byte)(data[4] >> 6 & 0x1);          //Bonnet Status Switch
                outdata.b24 = (byte)(data[4] >> 7 & 0x1);          //Trunk lid open Switch

                outdata.b25 = (byte)(data[3] >> 0 & 0x1);          //BSD Activation Switch
                outdata.b26 = (byte)(data[3] >> 1 & 0x1);          //Camera 360 Switch
                outdata.b27 = (byte)(data[3] >> 2 & 0x1);          //Engine Oil Pressure Switch
                outdata.b28 = (byte)(data[3] >> 3 & 0x1);          //Brake Oil Fluid Level Switch
                outdata.b29 = (byte)(data[3] >> 4 & 0x1);          //Driver Seat Belt Switch
                outdata.b30 = (byte)(data[3] >> 5 & 0x1);         //reserve
                outdata.b31 = (byte)(data[3] >> 6 & 0x1);         //reserve
                outdata.b32 = (byte)(data[3] >> 7 & 0x1);         //reserve

                outdata.b33 = (byte)(data[2] >> 0 & 0x1);         //Analog in    //??
                outdata.b34 = (byte)(data[2] >> 1 & 0x1);         //Analog in
                outdata.b35 = (byte)(data[2] >> 2 & 0x1);          //Analog in
                outdata.b36 = (byte)(data[2] >> 3 & 0x1);           //Analog in
                outdata.b2_1 = (byte)(data[2] >> 4 & 0x1);         //FLH Door Status Switch
                outdata.b2_2 = (byte)(data[2] >> 5 & 0x1);         //FRH Door Status Switch
                outdata.b2_3 = (byte)(data[2] >> 6 & 0x1);         //PAS Activation Switch
                outdata.b2_4 = (byte)(data[2] >> 7 & 0x1);         //Roof Lamp Switch

                outdata.b2_5 = (byte)(data[1] >> 0 & 0x1);          //RLH Door Status Switch
                outdata.b2_6 = (byte)(data[1] >> 1 & 0x1);          //RRH Door Status Switch
                outdata.b2_7 = (byte)(data[1] >> 2 & 0x1);          //FLH Door Lock Status Switch
                outdata.b2_8 = (byte)(data[1] >> 3 & 0x1);          //FRH Door Lock Status Switch
                outdata.b2_9 = (byte)(data[1] >> 4 & 0x1);          //Crash Unlock
                outdata.b2_10 = (byte)(data[1] >> 5 & 0x1);         //Rear Wiper Zero Position
                outdata.b2_11 = (byte)(data[1] >> 6 & 0x1);         //Driver Seat Belt Switch
                outdata.b2_12 = (byte)(data[1] >> 7 & 0x1);         //Trunk Status Switch

                outdata.b2_13 = (byte)(data[0] >> 0 & 0x1);          //SSB SW2 IN
                outdata.b2_14 = (byte)(data[0] >> 1 & 0x1);          //SSB SW1 IN
                outdata.b2_31 = (byte)(data[0] >> 2 & 0x1);         //In2_H1 : Brake Pedal Switch
                outdata.b2_32 = (byte)(data[0] >> 3 & 0x1);         //In2_H2  :Start Relay

                outdata.b2_33 = (byte)(data[0] >> 4 & 0x1);         //In2_H3   :ACC Relay
                outdata.b2_34 = (byte)(data[0] >> 5 & 0x1);        //In2_H4    :ON Relay
                outdata.b2_35 = (byte)(data[0] >> 6 & 0x1);        //In2_H5    :Brake Pedal Switch
                outdata.b2_36 = (byte)(data[0] >> 7 & 0x1);         //res

            }
            catch (Exception e)
            {
            }
        }

        public static void ExctractAnalogInputData(byte[] data)
        {
            try
            {
                //AnalogInput.RelayNum = data[0];
                // AnalogInput.Value = (short)(data[2] + (data[1] << 8));// BitConverter.ToInt16(data, 1);
                AnalogInput.Value = data[0];
            }
            catch (Exception e)
            {
            }
        }
        //public static void ExctractOutputDataB3(byte[] data, OutPutParams outdata,int division)
        //{
        //    try
        //    {
        //        outdata.outputNumber = data[0];
        //        outdata.current = (float)(data[2] + (data[1] << 8)) / 2000;// BitConverter.ToInt16(data, 1);

        //    }
        //    catch (Exception e)
        //    {
        //    }
        //}

        public static void ExctractOutputData(byte[] data, OutPutParams outdata, int division)
        {
            try
            {
                outdata.outputNumber = data[0];
                outdata.current = (float)(data[2] + (data[1] << 8)) / division;// BitConverter.ToInt16(data, 1);

            }
            catch (Exception e)
            {
            }
        }

        public static void ExctractOutputDataDigitalOutputBoard(byte[] data, OutPutParams outdata)
        {
            try
            {
                outdata.db0 = (byte)(data[0] >> 0 & 0x1);
                outdata.db1 = (byte)(data[0] >> 1 & 0x1);
                outdata.db2 = (byte)(data[0] >> 2 & 0x1);
                outdata.db3 = (byte)(data[0] >> 3 & 0x1);
                outdata.db4 = (byte)(data[0] >> 4 & 0x1);
                outdata.db5 = (byte)(data[0] >> 5 & 0x1);
                outdata.db6 = (byte)(data[0] >> 6 & 0x1);
                outdata.db7 = (byte)(data[0] >> 7 & 0x1);

                outdata.db8 = (byte)(data[1] >> 0 & 0x1);
                outdata.db9 = (byte)(data[1] >> 1 & 0x1);
                outdata.db10 = (byte)(data[1] >> 2 & 0x1);
                outdata.db11 = (byte)(data[1] >> 3 & 0x1);
                outdata.db12 = (byte)(data[1] >> 4 & 0x1);
                outdata.db13 = (byte)(data[1] >> 5 & 0x1);
                outdata.db14 = (byte)(data[1] >> 6 & 0x1);
                outdata.db15 = (byte)(data[1] >> 7 & 0x1);

            }
            catch (Exception e)
            {
            }
        }

        public static void ExctractPowerData(byte[] data, PowerParams power)
        {
            try
            {
                power.Number = data[0];
                power.current = (float)(data[2] + (data[1] << 8)) / 1000f;   // BitConverter.ToInt16(data, 1);

                //power.FogLampsInputPower = BitConverter.ToInt16(data, 0);
                //power.HeaterInputPower = BitConverter.ToInt16(data, 2);
            }
            catch (Exception e)
            {
            }
        }

        public static void ExctractNetDataCan(byte[] data, NetParams net)
        {
            try
            {
                net.DataCANH = new byte[4];
                net.DataCANL = new byte[4];
                net.DataCASCANL = new byte[4];

                switch (data[0])
                {
                    case 1:
                        //net.CANHS = 1;
                        net.Mode = 1;
                        for (int i = 0; i < 4; i++)
                            net.DataCANH[i] = data[1 + i];




                        break;
                    case 2:
                        //net.CANLS = 1;
                        net.Mode = 2;

                        for (int i = 0; i < 4; i++)
                            net.DataCANL[i] = data[1 + i];

                        break;

                    case 3:
                        net.Mode = 3;

                        for (int i = 0; i < 4; i++)
                            net.DataCASCANL[i] = data[1 + i];
                        break;

                }

            }
            catch (Exception e)
            {
            }
        }
        public static void ExctractNetDataLin(byte[] data, NetParams net)
        {
            try
            {
                net.DataLinF = new byte[4];
                net.DataLinR = new byte[4];

                switch (data[0])
                {
                    case 1:
                        // net.LinFront = 1;
                        net.Mode = 1;

                        for (int i = 0; i < 4; i++)
                            net.DataLinF[i] = data[1 + i];



                        break;
                    case 2:
                        //net.LinRear = 1;
                        net.Mode = 2;

                        for (int i = 0; i < 4; i++)
                            net.DataLinR[i] = data[1 + i];

                        break;

                }

            }
            catch (Exception e)
            {
            }
        }
        public static void ExtractInfoData(byte[] data, InfoParams info)
        {
            try
            {
                info.ECUType = (EcuType)data[1];
                info.EnvironmentTemp = data[2];
                info.OverTemp = data[3];
                info.HardwareVersion = Encoding.ASCII.GetString(data, 4, 2);
                info.SoftwareVersion = Encoding.ASCII.GetString(data, 6, 2);
                info.ProductionDate = Encoding.ASCII.GetString(data, 8, 2);//System.BitConverter.ToString(data, 15, 10);
            }
            catch (Exception e)
            {

            }
        }


    }
}
