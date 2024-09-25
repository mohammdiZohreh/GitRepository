using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.IO;


namespace TesterIKBCM
{
    public class SerialManager
    {
        public event EventHandler SerialManagerDataReceive;

        private SerialPort _serialPort { get; set; }
        private delegate void SetTextDeleg(byte[] data);
        public byte[] IncomingBytes { get; set; }
        private int State { get; set; } = 0;
        private int DataLen { get; set; }

        private int sum = 0;
        private byte checksum;
        private List<byte> Data { get; set; }
        public int BadCheckSum { get; private set; } = 0;
        public byte id { get; set; }
        public byte messageType { get; set; }
        public int packtExtCounter = 0;
        public SerialManager()
        {
            _serialPort = new SerialPort();
        }
        public SerialManager(string comPort, int baudRate, Parity parity = Parity.None, int databits = 8, StopBits stopBits = StopBits.None, bool readFromBuffer = true)
        {
            _serialPort = new SerialPort(comPort, baudRate, parity: parity, dataBits: databits, stopBits: stopBits);
            _serialPort.ReadBufferSize = 1024 * 1024 * 10;
            if (!readFromBuffer)
                _serialPort.DataReceived += SerialPort_DataReceived;

        }
        //PacketView.packet=new li
        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //    Thread.Sleep(500);
            if (!_serialPort.IsOpen)
                return;
            var bytes = _serialPort.BytesToRead;

            var buffer = new byte[bytes];
            _serialPort.Read(buffer, 0, bytes);
            IncomingBytes = buffer;

            PacketView.data = BitConverter.ToString(IncomingBytes);   //for test
            SerialManagerDataReceive?.Invoke(this, new EventArgs());
        }

        public bool IsOpen()
        {
            return _serialPort.IsOpen;
        }

        public void ExctractDataFromSerialByte(List<List<byte>> dataList, byte[] incomingBytes)
        {
            try
            {
                foreach (var item in incomingBytes)
                {
                    switch (State)
                    {
                        case 0:
                            if ((item == 0x11) || (item == 0x22) || (item == 0x33) || (item == 0x44) || (item == 0x55) || (item == 0x66) || (item == 0x77) || (item == 0x88) || (item == 0x99))
                            {
                                id = item;
                                State = 1;
                                // sum = item;  //ID Does not calc in checksum

                            }
                            break;
                        case 1:
                            
                                messageType = item;
                                State = 2;
                                sum = item;
                           
                                //System.Diagnostics.Trace.WriteLine("header error");
                          
                            break;

                        case 2:
                            DataLen = item;
                            Data = new List<byte>();
                            sum += item;
                            packtExtCounter = 0;
                            State = 3;       //one byte Datalen

                            break;

                        case 3:
                            if (DataLen > Data.Count)
                            {
                                Data.Add(item);
                                sum += item;
                                packtExtCounter++;
                                if (packtExtCounter > 20)
                                    State = 0;
                            }
                            else
                            {
                                if (sum >= 0xff)
                                {
                                    while (sum >= 0xff)
                                        sum -= 0xff;
                                }
                                checksum = (byte)(0xff - sum);
                                if ((checksum & 0xff) == item)
                                {
                                    dataList.Add(Data);

                                }
                                else
                                {
                                    //dataList.Add(Data);
                                    BadCheckSum++;
                                }
                                State = 0;
                            }
                            break;
                    }
                }
            }
            catch (Exception e) { }

        }

        public void ClosePort()
        {
            if (_serialPort.IsOpen)
                try
                {

                    _serialPort.Close();
                }
                catch { }
        }
        public void OpenPort()
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public bool Write(byte[] input)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Write(input, 0, input.Length);

                    return true;
                }
                else
                {

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //for test
        string path = Directory.GetCurrentDirectory() + @"\AllUsers";
        //public void WriteToJson2(byte[] data)
        //{
        //    var jsonData = JsonConvert.SerializeObject(data.ToArray());

        //    System.IO.File.WriteAllText(path + @"\UserInfo.txt", jsonData);
        //}
        //public void WriteToJson(List<string> data)
        //{
        //    string fileName = path + @"\UserInfo.txt";

        //    {
        //        if (File.Exists(fileName))
        //        {
        //            var jsonData = System.IO.File.ReadAllText(path + @"\UserInfo.txt");
        //            // De-serialize to object or create new list
        //            //var employeeList = JsonConvert.DeserializeObject<List<string>>(jsonData) ?? new List<string>();
        //            string employeeList = JsonConvert.DeserializeObject<string>(jsonData);
        //            if (employeeList != null)
        //                data.Add(employeeList);
        //            //foreach (var item in employeeList)
        //            //{

        //            //    data.Add(item);
        //            //}

        //            jsonData = JsonConvert.SerializeObject(data.ToArray());
        //            System.IO.File.WriteAllText(path + @"\UserInfo.txt", jsonData);
        //        }
        //        else
        //        {

        //            var jsonData = JsonConvert.SerializeObject(data.ToArray());
        //            System.IO.File.WriteAllText(path + @"\UserInfo.txt", jsonData);
        //        }

        //    }
        //    //else
        //    //{

        //    //    var jsonData = JsonConvert.SerializeObject(data.ToArray());
        //    //    System.IO.File.WriteAllText(path + @"\UserInfo.txt", jsonData);
        //    //}

        //}
        //private void Loadfile()
        //{
        //    //UsersList.Add(operatorNameCombo.EditValue?.ToString());
        //    var read = LoadFromFile();
        //    if (read != null)
        //        foreach (var item in read)
        //        {


        //        }
        //}

        //private List<string> LoadFromFile()
        //{
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //        return null;
        //    }
        //    // File.SetAttributes(path, FileAttributes.Normal);
        //    string fileName = path + @"\UserInfo.txt";
        //    if (File.Exists(fileName))
        //    {
        //        using (var file = File.OpenText(Directory.GetCurrentDirectory() + @"\AllUsers\UserInfo.txt"))
        //        {
        //            var serializer = new JsonSerializer();
        //            return (List<string>)serializer.Deserialize(file, typeof(List<string>));
        //        }
        //    }
        //    else return null;
        //}

    }
}
