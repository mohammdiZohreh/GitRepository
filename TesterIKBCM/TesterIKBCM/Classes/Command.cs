using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterIKBCM
{
    public class Command
    {
        public ID id { get; set; }
    }
    public enum ID
    {
        InputB1 = 0x11,
        InputB2 = 0x22,
        OutputB1 = 0x33,
        OutputB2 = 0x44,
        OutputB3 = 0x55,
        DigitalOutput = 0x66,
        Network_Board = 0x77,
        Power_Board = 0x88,
        CAS_tBoard=0x99

    }
    public enum MsType
    {
        SwitchONOff = 1,
        Current = 2,
        CANMessage = 3,
        LINMessage = 4,
        BCMInputStatus = 5,
        CASInputStatus = 0x10,
        BCMAnalogInputStatus = 6,
        DigitalOutputBStatus = 7,
        Temp = 8,
        BCMCurrent = 9,

        StartStopCmd = 0xa,
        PowerShort =0xB,
        PowerVoltSw=0xC,
        BoardPowerSw=0xD,
        //0xE
        NoMessage = 0xf,
        SupplierCode=0x11,
        RKE=0x31,
        LF=0x32,
        Immo=0x33,
        KickSensor=0x34,
        TPMS=0x35,
        DHS=0x36,

        BCM_SoftwarVersion = 0x3E,
        BCM_HardwarVersion = 0x3D,
        BCM_BootloadrVersion = 0x3F,

        CAS_SoftwarVersion = 0x3A,
        CAS_HardwarVersion = 0x3C,
        CAS_BootloadrVersion = 0x3B,

        Welcome = 0xAA,
        OutputTest1 = 0xBB,
        OutputTest2 = 0xCC,
        OutputStatus1 = 0xEE,
        OutputStatus2 = 0xDD,
        CheckConnection=0xAA,
        CrashUnlock_Cancling = 0x2C
    }
    public enum version
    {
        BCM_Software= 0x1,
        BCM_Hardware= 0x2,
        BCM_BootLoader= 0x3,
        CAS_Software = 0x4,
        CAS_Hardware = 0x5,
        CAS_BootLoader = 0x6,

    }
    public enum EcuType
    {
        IKBCM = 'I',
        Dena = 'D'
    }

}
