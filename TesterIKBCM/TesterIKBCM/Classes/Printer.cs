using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.BarCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterIKBCM
{
    public class Printer
    {
        private DevExpress.XtraEditors.BarCodeControl barCodeControl1;
        public Printer()
        {
            this.barCodeControl1 = new DevExpress.XtraEditors.BarCodeControl();
            //this.barCodeControl1.Text = "NU8D4ES2730001";
            barCodeControl1.Width = 400;
            barCodeControl1.Height = 150;//80
            barCodeControl1.Font = null;


        }
        //*********************
        PrintQueueCollection _Printers;

        public Printer(string n)
        {
            _Printers = new PrintServer().GetPrintQueues(new[] {
          EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections});

        }

        public PrintQueueCollection Printers
        { get { return _Printers; } }

        public void TSCPrint(string barcode)   // string barcodeText, string labelText,string name,string[] param,string[] barcodeParam, int width, int height)
        {
            try
            {
                barCodeControl1.Width = 400;
                barCodeControl1.Height = 150;//80

                TSCLIB_DLL.openport(lbl.Printer_Name);  //"TSC TE200"       //Open specified printer driver
                TSCLIB_DLL.setup(lbl.label_Width, lbl.label_Height, lbl.label_Speed, lbl.label_Density, lbl.label_Sensor, lbl.label_Vertical, lbl.label_Offset);
                TSCLIB_DLL.clearbuffer();     //Clear image buffer
                TSCLIB_DLL.windowsfont(lbl.wfp_x, lbl.wfp_y, lbl.wfp_fontHeight, lbl.wfp_rotation, lbl.wfp_fontstyle, lbl.wfp_underline, lbl.wfp_font, lbl.wfp_content);// 30, 20, 25, 0, 2, 0, lbl.wfp_font, lbl.wfp_content);//(30, 20, 25, 0, 2, 0, "Century Gothic", labelText);

                //if(barcode)
                TSCLIB_DLL.barcode(lbl.barcd_x, lbl.barcd_y, lbl.barcd_type, lbl.barcd_Height, lbl.barcd_readble, lbl.barcd_rotation, lbl.barcd_narrow, lbl.barcd_wide, barcode);

                TSCLIB_DLL.sendcommand("PUTPCX 100,400,\"UL.PCX\"");  //Drawing PCX graphic

                TSCLIB_DLL.printlabel("1", "1"); //Print labels
                //TSCLIB_DLL.printerfont("30", "20", "Arial", "0", "1", "1", "BNazanin");//added/test
                TSCLIB_DLL.closeport();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public void TSCPrintFail(string labelText)
        {
            try
            {
                barCodeControl1.Width = 400;
                barCodeControl1.Height = 150;//80

                TSCLIB_DLL.openport(lbl.Printer_Name);  //"TSC TE200"       //Open specified printer driver
                TSCLIB_DLL.setup(lbl.label_Width, lbl.label_Height, lbl.label_Speed, lbl.label_Density, lbl.label_Sensor, lbl.label_Vertical, lbl.label_Offset);
                TSCLIB_DLL.clearbuffer();     //Clear image buffer
                TSCLIB_DLL.windowsfont(lbl.wff_x, lbl.wff_y, lbl.wff_fontHeight, lbl.wff_rotation, lbl.wff_fontstyle, lbl.wff_underline, lbl.wff_font, labelText);//(50, 25, 35, 0, 2, 0, "Arial", labelText)//(30, 20, 25, 0, 2, 0, "Arial", labelText);//

                TSCLIB_DLL.sendcommand("PUTPCX 100,400,\"UL.PCX\"");  //Drawing PCX graphic

                TSCLIB_DLL.printlabel("1", "1"); //Print labels
                                                 //  TSCLIB_DLL.printerfont("60", "30", "Arial", "0", "1", "1", "BNazanin");//added/test
                TSCLIB_DLL.closeport();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
        string printText = "IKBCM Test Pass";

        public void TSCPrint(string barcodeText, string labelText)
        {
            try
            {
                TSCLIB_DLL.openport("TSC TE200");         //Open specified printer driver
                TSCLIB_DLL.setup("55", "16", "4", "8", "0", "3", "0");     
                TSCLIB_DLL.clearbuffer();     //Clear image buffer
                TSCLIB_DLL.windowsfont(30, 20, 25, 0, 2, 0, "Century Gothic", labelText);  


                TSCLIB_DLL.barcode("30", "45", "128", "40", "1", "0", "2", "2", barcodeText); 
          
                TSCLIB_DLL.sendcommand("PUTPCX 100,400,\"UL.PCX\"");  //Drawing PCX graphic
               
                TSCLIB_DLL.printlabel("1", "1"); //Print labels
                TSCLIB_DLL.closeport();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
        public void print_Click(string text)
        {
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();
            // pd.PrinterSettings.PrinterName= "Microsoft Print to PDF";// "ZDesigner GC420t (EPL)"; // printerCb.SelectedItem.ToString();// "ZDesigner GC420t (EPL)";
            printText = text;                                                                  //Add PrintPage event handler  
            pd.PrintPage += new PrintPageEventHandler(this.PrintTextFileHandler);
            try
            {
                pd.Print();
            }
            catch (Exception e  ) { }
        }

        private void PrintTextFileHandler(object sender, PrintPageEventArgs eventArgs)
        {
            Graphics g = eventArgs.Graphics;
            Font font = new Font("Arial", 16);
            SolidBrush brush = new SolidBrush(Color.Black);
            g.DrawString(printText, font, brush, new Rectangle(20, 20, 303, 150));
            //
            int size = eventArgs.MarginBounds.Width;
            //Create a graphics with the size of the printer page's width
            Bitmap imageToPr = new Bitmap(size - 30, size - 30);
            Graphics graphics = Graphics.FromImage(imageToPr);
            graphics.DrawImage(barCodeControl1.ExportToImage(), new Rectangle(0, 0, size, size ));
           
           
            eventArgs.Graphics.DrawImage(imageToPr, new Rectangle((int)eventArgs.MarginBounds.Left + 50, (int)eventArgs.MarginBounds.Top - 40, imageToPr.Width, imageToPr.Height), 0, 0, size - 30, size - 30, GraphicsUnit.Pixel);
        }

        public void Print(string barcodeText,string labelText)
        {
           try
            {
                PrintDocument pd = new PrintDocument();

                
                //Disable the printing document pop-up dialog shown during printing.
                PrintController printController = new StandardPrintController();
                pd.PrintController = printController;

                //For testing only: Hardcoded set paper size to particular paper.
                //pd.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("Custom 6x4", 720, 478);
                //pd.DefaultPageSettings.PaperSize = new PaperSize("Custom 6x4", 720, 478);
                this.barCodeControl1.Text = barcodeText;
                pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                pd.PrinterSettings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                //pd.PrinterSettings.PrinterName=  "Microsoft Print to PDF";//Foxit PDF Editor Printer// "ZDesigner GC420t (EPL)";
                // pd.PrinterSettings.PrinterName = "ZDesigner GC420t (EPL)";
                var p = pd.PrinterSettings.PrinterName;
                pd.PrintPage += (sndr, args) =>
                {
                    System.Drawing.Image i = barCodeControl1.ExportToImage(); //System.Drawing.Image.FromFile(FileName);


                    //System.Drawing.Rectangle m = args.MarginBounds;


                    //if ((double)i.Width / (double)i.Height > (double)m.Width / (double)m.Height) // image is wider
                    //{
                    //    m.Height = (int)((double)i.Height / (double)i.Width * (double)m.Width);
                    //}
                    //else
                    //{
                    //    m.Width = (int)((double)i.Width / (double)i.Height * (double)m.Height);
                    //}
                    //m.Height =25;
                    //m.Width = 250;

                    //// pd.DefaultPageSettings.Landscape = m.Width > m.Height;

                    //m.Y = (int)((((System.Drawing.Printing.PrintDocument)(sndr)).DefaultPageSettings.PaperSize.Height - m.Height) / 2);
                    //m.X = (int)((((System.Drawing.Printing.PrintDocument)(sndr)).DefaultPageSettings.PaperSize.Width - m.Width) / 2);
                    //m.X = 15;
                    //m.Y = 25;
                    //Font font = new Font("Arial", 6);
                    //Font font2 = new Font("Arial", 8);
                    //SolidBrush brush = new SolidBrush(Color.Black);
                    //args. Graphics.DrawString(labelText, font, brush, new Rectangle(15, 5, 100, 50)); //(20, 5, 303, 150));
                    //args.Graphics.DrawString(barcodeText, font2, brush, new Rectangle(-10, 9, 180, 100));// 303, 150));//(15, m.Height+m.Y +3, 200, 50));

                    //args.Graphics.DrawImage(i, m);

                    Graphics g = args.Graphics;
                    Font font = new Font("Arial", 16);
                    SolidBrush brush = new SolidBrush(Color.Black);
                    g.DrawString(printText, font, brush, new Rectangle(20, 20, 303, 150));
                    //
                    int size = args.MarginBounds.Width;
                    //Create a graphics with the size of the printer page's width
                    Bitmap imageToPr = new Bitmap(size - 30, size - 30);
                    Graphics graphics = Graphics.FromImage(imageToPr);
                    graphics.DrawImage(barCodeControl1.ExportToImage(), new Rectangle(0, 0, size, size));


                    args.Graphics.DrawImage(imageToPr, new Rectangle((int)args.MarginBounds.Left + 50, (int)args.MarginBounds.Top - 40, imageToPr.Width, imageToPr.Height), 0, 0, size - 30, size - 30, GraphicsUnit.Pixel);
                };
                pd.Print();
            }
            catch (Exception ex)
            {
                //log.ErrorFormat("Error : {0}\n By : {1}-{2}", ex.ToString(), this.GetType(), MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
               
            }
        }

    }
}
