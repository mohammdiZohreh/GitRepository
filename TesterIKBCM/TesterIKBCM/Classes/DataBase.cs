using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesterIKBCM
{
   public static class DataBase
    {
        public static SQLiteConnection con = new SQLiteConnection(@" Data Source = .\DENAIKBCMTester.db;Version=3;"); //(@" Data Source = .\K125IKBCMTester.db;Version=3;");// (@"Data Source=.\K125IKBCMTester.db;Initial Catalog=EKS;Integrated Security=True;Pooling=False");//);
       // public static string con = @" Data Source = .\DENAIKBCMTester.db;Version=3;";// @"Data Source=.\K125IKBCMTester.db;Version=3;";
        public static SQLiteCommand sql_command;

        public static List<string> Users = new List<string>();
        public static bool readFlag = false;
        public static float[] outputThr = new float[84];
        public static List<string> savedReports = new List<string>();
        //private static string LoadConnectionString(string id = "Default")
        //{
        //    var a = ConfigurationManager.ConnectionStrings[id].ConnectionString;
        //    return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        //}
        //
        public static void SaveReportPath()
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con)) //@"Data Source=.\K125IKBCMTester.db;Initial Catalog=EKS;Integrated Security=True;Pooling=False"
                {
                    sqlconnection.Open();



                    string insertXmlQuery = @"Insert Into [reportPath] (path)
                                          Values(@path)";

                    string updateXmlQuery = @"UPDATE [reportPath] SET path=@path ";
                    string s = "SELECT COUNT(*) from reportPath";
                    var r = checkEmptyTable(s);
                    //con.Close();
                    //con.Open();

                    string sql = null;
                    if (r)
                    {
                        sql = insertXmlQuery;
                    }
                    else
                    {
                        sql = updateXmlQuery;
                    }

                    SQLiteCommand insertCommand = new SQLiteCommand(sql, sqlconnection);
                    sqlconnection.Close();
                    sqlconnection.Open();
                    insertCommand.Parameters.AddWithValue("@path", AutoTest.SaveReportPath);




                    insertCommand.ExecuteNonQuery();
                    sqlconnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public static bool LoadReportPath()
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Open();

                    string selectQuery = string.Format(@"Select path From [reportPath] ");

                    int ind = 0;
                    SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlconnection);
                    SQLiteDataReader reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        AutoTest.SaveReportPath = reader[ind++].ToString();
                        reader.Close();
                        sqlconnection.Close();
                        
                        return true;
                    }
                    sqlconnection.Close();
                    return false;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static void SavePrintrSetting()
        {

            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Open();



                    string insertXmlQuery = @"Insert Into [printrSeting] (lbl_w,lbl_h,lbl_d,lbl_vg,lbl_of,lbl_sp,lbl_se,print_dpi,br_x,br_y,br_he,br_con,br_re,br_t,br_ro,br_n,br_w,wf_x,wf_y,wf_h,wf_con,wf_ro,wf_un,wf_fs,wf_f,wff_x,wff_y,wff_h,wff_con,wff_ro,wff_un,wff_fs,wff_f,printr_name)
                                          Values(@lbl_w,@lbl_h,@lbl_d,@lbl_vg,@lbl_of,@lbl_sp,@lbl_se,@print_dpi,@br_x,@br_y,@br_he,@br_con,@br_re,@br_t,@br_ro,@br_n,@br_w,@wf_x,@wf_y,@wf_h,@wf_con,@wf_ro,@wf_un,@wf_fs,@wf_f,@wff_x,@wff_y,@wff_h,@wff_con,@wff_ro,@wff_un,@wff_fs,@wff_f,@printr_name)";

                    string updateXmlQuery = @"UPDATE [printrSeting] SET lbl_w=@lbl_w,lbl_h=@lbl_h,lbl_d=@lbl_d,lbl_vg=@lbl_vg,lbl_of=@lbl_of,lbl_sp=@lbl_sp,lbl_se=@lbl_se,print_dpi=@print_dpi,br_x=@br_x,br_y=@br_y,br_he=@br_he,br_con=@br_con,br_re=@br_re,br_t=@br_t,br_ro=@br_ro,br_n=@br_n,br_w=@br_w,wf_x=@wf_x,wf_y=@wf_y,wf_h=@wf_h,wf_con=@wf_con,wf_ro=@wf_ro,wf_un=@wf_un,wf_fs=@wf_fs,wf_f=@wf_f,wff_x=@wff_x,wff_y=@wff_y,wff_h=@wff_h,wff_con=@wff_con,wff_ro=@wff_ro,wff_un=@wff_un,wff_fs=@wff_fs,wff_f=@wff_f,printr_name=@printr_name ";
                    string s = "SELECT COUNT(*) from printrSeting";
                    var r = checkEmptyTable(s);
                    //con.Close();
                    //con.Open();

                    string sql = null;
                    if (r)
                    {
                        sql = insertXmlQuery;
                    }
                    else
                    {
                        sql = updateXmlQuery;
                    }

                    SQLiteCommand insertCommand = new SQLiteCommand(sql, sqlconnection);
                    sqlconnection.Close();
                    sqlconnection.Open();
                    insertCommand.Parameters.AddWithValue("@lbl_w", lbl.label_Width);
                    insertCommand.Parameters.AddWithValue("@lbl_h", lbl.label_Height);
                    insertCommand.Parameters.AddWithValue("@lbl_d", lbl.label_Density);
                    insertCommand.Parameters.AddWithValue("@lbl_vg", lbl.label_Vertical);
                    insertCommand.Parameters.AddWithValue("@lbl_of", lbl.label_Offset);
                    insertCommand.Parameters.AddWithValue("@lbl_sp", lbl.label_Speed);
                    insertCommand.Parameters.AddWithValue("@lbl_se", lbl.label_Sensor);
                    insertCommand.Parameters.AddWithValue("@print_dpi", lbl.printr_dpi);
                    insertCommand.Parameters.AddWithValue("@br_x", lbl.barcd_x);
                    insertCommand.Parameters.AddWithValue("@br_y", lbl.barcd_y);
                    insertCommand.Parameters.AddWithValue("@br_he", lbl.barcd_Height);
                    insertCommand.Parameters.AddWithValue("@br_con", lbl.barcd_content);
                    insertCommand.Parameters.AddWithValue("@br_re", lbl.barcd_readble);
                    insertCommand.Parameters.AddWithValue("@br_t", lbl.barcd_type);
                    insertCommand.Parameters.AddWithValue("@br_ro", lbl.barcd_rotation);
                    insertCommand.Parameters.AddWithValue("@br_n", lbl.barcd_narrow);
                    insertCommand.Parameters.AddWithValue("@br_w", lbl.barcd_wide);
                    insertCommand.Parameters.AddWithValue("@wf_x", lbl.wfp_x);
                    insertCommand.Parameters.AddWithValue("@wf_y", lbl.wfp_y);
                    insertCommand.Parameters.AddWithValue("@wf_h", lbl.wfp_fontHeight);
                    insertCommand.Parameters.AddWithValue("@wf_con", lbl.wfp_content);
                    insertCommand.Parameters.AddWithValue("@wf_ro", lbl.wfp_rotation);
                    insertCommand.Parameters.AddWithValue("@wf_un", lbl.wfp_underline);
                    insertCommand.Parameters.AddWithValue("@wf_fs", lbl.wfp_fontstyle);
                    insertCommand.Parameters.AddWithValue("@wf_f", lbl.wfp_font);
                    insertCommand.Parameters.AddWithValue("@wff_x", lbl.wff_x);
                    insertCommand.Parameters.AddWithValue("@wff_y", lbl.wff_y);
                    insertCommand.Parameters.AddWithValue("@wff_h", lbl.wff_fontHeight);
                    insertCommand.Parameters.AddWithValue("@wff_con", lbl.wff_content);
                    insertCommand.Parameters.AddWithValue("@wff_ro", lbl.wff_rotation);
                    insertCommand.Parameters.AddWithValue("@wff_un", lbl.wff_underline);
                    insertCommand.Parameters.AddWithValue("@wff_fs", lbl.wff_fontstyle);
                    insertCommand.Parameters.AddWithValue("@wff_f", lbl.wff_font);
                    insertCommand.Parameters.AddWithValue("@printr_name", lbl.Printer_Name);



                    insertCommand.ExecuteNonQuery();
                    sqlconnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public static bool LoadPrintrSetting()
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Open();

                    string selectQuery = string.Format(@"Select lbl_w,lbl_h,lbl_d,lbl_vg,lbl_of,lbl_sp,lbl_se,print_dpi,br_x,br_y,br_he,br_con,br_re,br_t,br_ro,br_n,br_w,wf_x,wf_y,
                     wf_h,wf_con,wf_ro,wf_un,wf_fs,wf_f,wff_x,wff_y,wff_h,wff_con,wff_ro,wff_un,wff_fs,wff_f,printr_name
                      From [printrSeting] ");


                    int ind = 0;

                    SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlconnection);
                    SQLiteDataReader reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        //TestInfoClass.DayNum = int.Parse(reader["DayNum"].ToString());
                        //TestInfoClass.TrackingNumSt = reader["TrackingNumSt"].ToString();
                        lbl.label_Width = reader[ind++].ToString();
                        lbl.label_Height = reader[ind++].ToString();
                        lbl.label_Density = reader[ind++].ToString();
                        lbl.label_Vertical = reader[ind++].ToString();
                        lbl.label_Offset = reader[ind++].ToString();
                        lbl.label_Speed = reader[ind++].ToString();
                        lbl.label_Sensor = reader[ind++].ToString();
                        lbl.printr_dpi = int.Parse(reader[ind++].ToString());
                        lbl.barcd_x = reader[ind++].ToString();
                        lbl.barcd_y = reader[ind++].ToString();
                        lbl.barcd_Height = reader[ind++].ToString();
                        lbl.barcd_content = reader[ind++].ToString();
                        lbl.barcd_readble = reader[ind++].ToString();
                        lbl.barcd_type = reader[ind++].ToString();
                        lbl.barcd_rotation = reader[ind++].ToString();
                        lbl.barcd_narrow = reader[ind++].ToString();
                        lbl.barcd_wide = reader[ind++].ToString();
                        lbl.wfp_x = int.Parse(reader[ind++].ToString());
                        lbl.wfp_y = int.Parse(reader[ind++].ToString());
                        lbl.wfp_fontHeight = int.Parse(reader[ind++].ToString());
                        lbl.wfp_content = reader[ind++].ToString();
                        lbl.wfp_rotation = int.Parse(reader[ind++].ToString());
                        lbl.wfp_underline = int.Parse(reader[ind++].ToString());
                        lbl.wfp_fontstyle = int.Parse(reader[ind++].ToString());
                        lbl.wfp_font = reader[ind++].ToString();
                        lbl.wff_x = int.Parse(reader[ind++].ToString());
                        lbl.wff_y = int.Parse(reader[ind++].ToString());
                        lbl.wff_fontHeight = int.Parse(reader[ind++].ToString());
                        lbl.wff_content = reader[ind++].ToString();
                        lbl.wff_rotation = int.Parse(reader[ind++].ToString());
                        lbl.wff_underline = int.Parse(reader[ind++].ToString());
                        lbl.wff_fontstyle = int.Parse(reader[ind++].ToString());
                        lbl.wff_font = reader[ind++].ToString();
                        lbl.Printer_Name = reader[ind++].ToString();
                        reader.Close();

                        // UpdateThresholds();
                        sqlconnection.Close();
                        return true;
                    }
                    else
                    {
                        sqlconnection.Close();
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static bool AddUser(string username,int uid,int acslvl)
        {
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(con))
                {
                    string sql = "Select UserName,UserID,Access from UserTb where UserName=@UserName or UserID=@UserID ";
                    c.Close();
                    c.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, c))
                    {
                        cmd.Parameters.AddWithValue("UserName", username );//SettingHouse.UserName
                        cmd.Parameters.AddWithValue("UserID", uid ); //SettingHouse.UserID
                        cmd.Parameters.AddWithValue("Access", acslvl); // SettingHouse.accessLevel

                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {

                            while (rdr.Read())
                            {
                                if ((rdr["UserName"].ToString() == username) || (int.Parse(rdr["UserID"].ToString()) == uid))
                                {
                                    rdr.Close();
                                    c.Close();
                                    return false;

                                }
                               // rdr.Close(); //?
                            }
                            rdr.Close();

                        }
                        c.Close();
                        sql = "INSERT INTO UserTb (UserName,UserID,Access) VALUES('" + username + "','" + uid + "','" + acslvl + "')";
                        c.Open();
                        using (SQLiteCommand cmd2 = new SQLiteCommand(sql, c))
                        {

                            int result = cmd2.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static bool LoadUser()
        {
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(con))
                {

                    string sql = "Select UserName,UserID,Access from UserTb where UserName=@UserName or UserID=@UserID ";
                    c.Close();
                    c.Open();

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, c))
                    {
                        cmd.Parameters.AddWithValue("UserName", SettingHouse.UserName);
                        cmd.Parameters.AddWithValue("UserID", SettingHouse.UserID);
                        cmd.Parameters.AddWithValue("Access", SettingHouse.accessLevel);


                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                SettingHouse.UserName = rdr["UserName"].ToString();
                                SettingHouse.UserID = int.Parse(rdr["UserID"].ToString());
                                SettingHouse.accessLevel = int.Parse(rdr["Access"].ToString());
                                rdr.Close();
                                return true;

                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool LoadAllUsers()
        {
            readFlag = false;
            Users.Clear();
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(con))
                {

                    string sql = "Select UserName,UserID,Access from UserTb ";
                    c.Open();

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, c))
                    {

                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Users.Add(rdr["UserName"].ToString());
                                readFlag = true;
                            }
                            rdr.Close();
                            if (readFlag)
                                return true;
                            else return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool RemoveUser()
        {
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(con))
                {
                    c.Open();
                    string sql = "delete from UserTb where UserID= " + SettingHouse.UserID + " ";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, c))
                    {
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                            return true;
                    }
                    c.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EditUser()
        {
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(con))
                {
                    c.Open();
                    //string sql = "update UserTb set UserName='" + SettingHouse.UserName + " ', UserID ='" + SettingHouse.NewUserID + "',Access=' " + SettingHouse.accessLevel + "' where  UserID= " + SettingHouse.UserID + " ";
                    using (SQLiteCommand command = new SQLiteCommand(c))
                    {
                        command.CommandText = "update UserTb set UserName = :UserName,UserID = :UserID, Access = :Access where UserID=:UserID";

                        command.Parameters.Add("UserName", DbType.String).Value = SettingHouse.UserName;
                        command.Parameters.Add("UserID", DbType.Int32).Value = SettingHouse.NewUserID;
                        command.Parameters.Add("Access", DbType.Int32).Value = SettingHouse.accessLevel;
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            c.Close();
                            return true;
                        }
                        bool re = RemoveUser();
                        if (re)
                            re = AddUser(SettingHouse.UserName, SettingHouse.NewUserID, SettingHouse.accessLevel);
                        if (re)
                            return true;
                    }
                    c.Close();
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            finally
            {
                
            }
        }
        //
        //public static bool checkEmptyTable(string sql)
        //{
        //    try
        //    {
        //        con2.Close();
        //        con2.Open();

        //        sql_command = new SQLiteCommand();
        //        sql_command.Connection = con2;
        //        sql_command.CommandText = sql;


        //        int result = int.Parse(sql_command.ExecuteScalar().ToString());

        //        return result == 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        con2.Close();
        //    }
        //}
        public static bool checkEmptyTable(string sql)
        {
            try
            {
                con.Close();
                con.Open();

                sql_command = new SQLiteCommand();
                sql_command.Connection = con;
                sql_command.CommandText = sql;


                int result = int.Parse(sql_command.ExecuteScalar().ToString());

                return result == 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }
        private static void ExecuteNonQuery(string queryString)    //for instance
        {
            using (var connection = new SQLiteConnection(con))
                     
            {
                using (var command = new SQLiteCommand(queryString, connection))
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void SaveTestSpecTblInDb()
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Close();
                    sqlconnection.Open();



                    string insertXmlQuery = @"Insert Into [TestSpectbl] (TrackingNum,ProductSerial,DayNum,TestStrategy,BcmSoftwareVr,BcmHardwareVr,Year,TotalTestPass,TotalTestFail,TrackingNumSt,inpTestStrategy,BcmBootVr,CasSoftwareVr,CasHardwareVr,CasBootVr)
                                          Values(@TrackingNum,@ProductSerial,@DayNum,@TestStrategy,@BcmSoftwareVr,@BcmHardwareVr,@Year,@TotalTestPass,@TotalTestFail,@TrackingNumSt,@inpTestStrategy,@BcmBootVr,@CasSoftwareVr,@CasHardwareVr,@CasBootVr)";//,@BcmBootVr,@CasSoftwareVr,@CasHardwareVr,@CasBootVr

                    string updateXmlQuery = @"UPDATE [TestSpectbl] SET TrackingNum=@TrackingNum,ProductSerial=@ProductSerial,DayNum=@DayNum,TestStrategy=@TestStrategy,BcmSoftwareVr=@BcmSoftwareVr,BcmHardwareVr=@BcmHardwareVr,Year=@Year,TotalTestPass=@TotalTestPass,TotalTestFail=@TotalTestFail,TrackingNumSt=@TrackingNumSt,inpTestStrategy=@inpTestStrategy
                    ,BcmBootVr=@BcmBootVr,CasSoftwareVr=@CasSoftwareVr,CasHardwareVr=@CasHardwareVr,CasBootVr=@CasBootVr";
                    string s = "SELECT COUNT(*) from TestSpectbl";
                    var r = checkEmptyTable(s);
                    //con.Close();
                    //con.Open();

                    string sql = null;
                    if (r)
                    {
                        sql = insertXmlQuery;
                    }
                    else
                    {
                        sql = updateXmlQuery;
                    }

                    SQLiteCommand insertCommand = new SQLiteCommand(sql, sqlconnection);
                    sqlconnection.Close();
                    sqlconnection.Open();
                    insertCommand.Parameters.AddWithValue("@TrackingNum", TestInfoClass.TrackingNum);
                    insertCommand.Parameters.AddWithValue("@ProductSerial", TestInfoClass.ProductSerial);
                    insertCommand.Parameters.AddWithValue("@DayNum", TestInfoClass.DayNum);
                    insertCommand.Parameters.AddWithValue("@TestStrategy", TestInfoClass.TestStrategy);
                    insertCommand.Parameters.AddWithValue("@BcmSoftwareVr", TestInfoClass.BCM_SoftwareVr_Exp);
                    insertCommand.Parameters.AddWithValue("@BcmHardwareVr", TestInfoClass.BCM_HardWareVr_Exp);
                    insertCommand.Parameters.AddWithValue("@Year", TestInfoClass.Year);
                    insertCommand.Parameters.AddWithValue("@TotalTestPass", TestInfoClass.TotalTestPass);
                    insertCommand.Parameters.AddWithValue("@TotalTestFail", TestInfoClass.TotalTestFail);
                    insertCommand.Parameters.AddWithValue("@TrackingNumSt", TestInfoClass.TrackingNumSt);
                    insertCommand.Parameters.AddWithValue("@inpTestStrategy", TestInfoClass.InputTestStrategy);
                    insertCommand.Parameters.AddWithValue("@BcmBootVr", TestInfoClass.BCM_BootloaderVr_Exp);
                    insertCommand.Parameters.AddWithValue("@CasSoftwareVr", TestInfoClass.CAS_SoftwareVr_Exp);
                    insertCommand.Parameters.AddWithValue("@CasHardwareVr", TestInfoClass.CAS_HardWareVr_Exp);
                    insertCommand.Parameters.AddWithValue("@CasBootVr", TestInfoClass.CAS_BootloaderVr_Exp);

                    insertCommand.ExecuteNonQuery();
                    sqlconnection.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
       
        public static bool LoadTestSpecTblInDb()
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Open();

                    string selectQuery = string.Format(@"Select TrackingNum,ProductSerial,DayNum,TestStrategy,BcmSoftwareVr,BcmHardwareVr,Year,TotalTestPass,TotalTestFail,TrackingNumSt,inpTestStrategy,BcmBootVr,CasSoftwareVr,CasHardwareVr,CasBootVr From [TestSpectbl] ");//,BcmBootVr,CasSoftwareVr,CasHardwareVr,CasBootVr

                    int ind = 0;
                    SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlconnection);
                    SQLiteDataReader reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        TestInfoClass.TrackingNum = int.Parse(reader[ind++].ToString());
                        TestInfoClass.ProductSerial = int.Parse(reader[ind++].ToString());
                        TestInfoClass.DayNum = int.Parse(reader[ind++].ToString());
                        TestInfoClass.TestStrategy = byte.Parse(reader[ind++].ToString());
                        TestInfoClass.BCM_SoftwareVr_Exp = (reader[ind++].ToString());
                        TestInfoClass.BCM_HardWareVr_Exp = (reader[ind++].ToString());
                        TestInfoClass.Year = (reader[ind++].ToString());
                        TestInfoClass.TotalTestPass = int.Parse(reader[ind++].ToString());
                        TestInfoClass.TotalTestFail = int.Parse(reader[ind++].ToString());
                        TestInfoClass.TrackingNumSt = (reader[ind++].ToString());
                        TestInfoClass.InputTestStrategy = byte.Parse(reader[ind++].ToString());
                        TestInfoClass.BCM_BootloaderVr_Exp = reader[ind++].ToString();
                        TestInfoClass.CAS_SoftwareVr_Exp = reader[ind++].ToString();
                        TestInfoClass.CAS_HardWareVr_Exp = reader[ind++].ToString();
                        TestInfoClass.CAS_BootloaderVr_Exp = reader[ind++].ToString();

                        reader.Close();
                        sqlconnection.Close();
                        return true;
                    }
                    sqlconnection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
      
        public static float[] LoadDefaultOutputsThresholdsFromSqlDatabase()
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Open();

                    string selectQuery = string.Format(@"Select p1,p2 From [ThrTable0] ");

                    // Read Byte [] Value from Sql Table 
                    SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlconnection);
                    SQLiteDataReader reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        float p1 = float.Parse(reader[0].ToString());
                        float p2 = float.Parse(reader[1].ToString());
                        float[] byteData = new float[5];
                        byteData[0] = p1;
                        byteData[1] = p2;
                        return byteData;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void SaveOutputsThresholdsInDb(float[] outputThr, float[] outputThrMx)
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Open();
                    
                    string insertXmlQuery = @"Insert Into [ThrTable2] (loadC1,loadC2,loadC3,loadC4,loadC5,loadC6,loadC7,loadC8,loadC9,loadC10,loadC11,loadC12,loadC13,loadC14,
                                           loadC15,loadC16,loadC17,loadC18,loadC19,loadC20,loadC21,loadC22,loadC23,loadC24,loadC25,loadC26,loadC27,loadC28,
                                           loadC29,loadC30,loadC31,loadC32,loadC33,loadC34,loadC35,loadC36,loadC37,loadC38,loadC39,loadC40,loadC41,loadC42,
                                          loadT1,loadT2,loadT3,loadT4,loadT5,loadT6,loadT7,loadT8,loadT9,loadT10,loadT11,loadT12,loadT13,loadT14,
                                          loadT15,loadT16,loadT17,loadT18,loadT19,loadT20,loadT21,loadT22,loadT23,loadT24,loadT25,loadT26,loadT27,loadT28, 
                                           loadT29,loadT30,loadT31,loadT32,loadT33,loadT34,loadT35,loadT36,loadT37,loadT38,loadT39,loadT40, loadT41,loadT42)
                                          Values(@loadC1,@loadC2,@loadC3,@loadC4,@loadC5,@loadC6,@loadC7,@loadC8,@loadC9,@loadC10,@loadC11 ,@loadC12 ,@loadC13,@loadC14,
                                           @loadC15,@loadC16,@loadC17,@loadC18,@loadC19,@loadC20,@loadC21,@loadC22,@loadC23,@loadC24 ,@loadC25,@loadC26 ,@loadC27,@loadC28,
                                           @loadC29,@loadC30,@loadC31,@loadC32,@loadC33,@loadC34,@loadC35,@loadC36,@loadC37,@loadC38,@loadC39,@loadC40,@loadC41,@loadC42,
                                              @loadT1,@loadT2,@loadT3,@loadT4,@loadT5,@loadT6,@loadT7,@loadT8,@loadT9,@loadT10,@loadT11,@loadT12,@loadT13,@loadT14,
                                            @loadT15,@loadT16,@loadT17,@loadT18,@loadT19,@loadT20,@loadT21,@loadT22,@loadT23,@loadT24,@loadT25,@loadT26, @loadT27,@loadT28 ,@loadT29,
                                            @loadT30,@loadT31,@loadT32,@loadT33,@loadT34,@loadT35,@loadT36,@loadT37,@loadT38,@loadT39,@loadT40,@loadT41,@loadT42)";

                    string updateXmlQuery = @"UPDATE [ThrTable2] SET loadC1=@loadC1,loadC2=@loadC2,loadC3=@loadC3,loadC4=@loadC4,loadC5=@loadC5,loadC6=@loadC6,loadC7=@loadC7,loadC8=@loadC8,loadC9=@loadC9,loadC10=@loadC10,loadC11=@loadC11,loadC12=@loadC12,loadC13=@loadC13,loadC14=@loadC14,
                                           loadC15=@loadC15,loadC16=@loadC16,loadC17=@loadC17,loadC18=@loadC18,loadC19=@loadC19,loadC20=@loadC20,loadC21=@loadC21,loadC22=@loadC22,loadC23=@loadC23,loadC24=@loadC24,loadC25=@loadC25,loadC26=@loadC26,loadC27=@loadC27,loadC28=@loadC28,
                                           loadC29=@loadC29,loadC30=@loadC30,loadC31=@loadC31,loadC32=@loadC32,loadC33=@loadC33,loadC34=@loadC34,loadC35=@loadC35,loadC36=@loadC36,loadC37=@loadC37,loadC38=@loadC38,loadC39=@loadC39,loadC40=@loadC40,loadC41=@loadC41,loadC42=@loadC42,
                                          loadT1=@loadT1,loadT2=@loadT2,loadT3=@loadT3,loadT4=@loadT4,loadT5=@loadT5,loadT6=@loadT6,loadT7=@loadT7,loadT8=@loadT8,loadT9=@loadT9,loadT10=@loadT10,loadT11=@loadT11,loadT12=@loadT12,loadT13=@loadT13,loadT14=@loadT14,
                                          loadT15=@loadT15,loadT16=@loadT16,loadT17=@loadT17,loadT18=@loadT18,loadT19=@loadT19,loadT20=@loadT20,loadT21=@loadT21,loadT22=@loadT22,loadT23=@loadT23,loadT24=@loadT24,loadT25=@loadT25,loadT26=@loadT26,loadT27=@loadT27,loadT28=@loadT28, 
                                           loadT29=@loadT29,loadT30=@loadT30,loadT31=@loadT31,loadT32=@loadT32,loadT33=@loadT33,loadT34=@loadT34,loadT35=@loadT35,loadT36=@loadT36,loadT37=@loadT37,loadT38=@loadT38,loadT39=@loadT39,loadT40=@loadT40, loadT41=@loadT41,loadT42=@loadT42 ";// WHERE loadC1=@loadC1 ";
                    string s = "SELECT COUNT(*) from ThrTable2";
                    var r = checkEmptyTable(s);
                    //con.Close();
                    //con.Open();

                    string sql = null;
                    if (r)
                    {
                        sql = insertXmlQuery;
                    }
                    else
                    {
                        sql = updateXmlQuery;
                    }
                    int indx = 0;
                    SQLiteCommand insertCommand = new SQLiteCommand(sql, sqlconnection);
                    sqlconnection.Close();
                    sqlconnection.Open();
                    insertCommand.Parameters.AddWithValue("@loadC1", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC2", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC3", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC4", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC5", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC6", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC7", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC8", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC9", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC10", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC11", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC12", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC13", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC14", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC15", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC16", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC17", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC18", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC19", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC20", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC21", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC22", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC23", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC24", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC25", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC26", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC27", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC28", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC29", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC30", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC31", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC32", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC33", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC34", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC35", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC36", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC37", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC38", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC39", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC40", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC41", outputThr[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadC42", outputThr[indx++]);
                    //
                    indx = 0;
                    insertCommand.Parameters.AddWithValue("@loadT1", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT2", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT3", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT4", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT5", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT6", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT7", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT8", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT9", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT10", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT11", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT12", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT13", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT14", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT15", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT16", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT17", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT18", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT19", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT20", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT21", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT22", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT23", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT24", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT25", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT26", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT27", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT28", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT29", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT30", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT31", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT32", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT33", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT34", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT35", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT36", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT37", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT38", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT39", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT40", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT41", outputThrMx[indx++]);
                    insertCommand.Parameters.AddWithValue("@loadT42", outputThrMx[indx++]);
                    //
                    insertCommand.ExecuteNonQuery();
                    sqlconnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public static float[] LoadOutputsThresholdsFromSqlDatabase()
        {
            try
            {
                using (SQLiteConnection sqlconnection = new SQLiteConnection(con))
                {
                    sqlconnection.Open();
                    string selectQuery = string.Format(@"Select loadC1,loadC2,loadC3,loadC4,loadC5,loadC6,loadC7,loadC8,loadC9,loadC10,loadC11,loadC12,loadC13,loadC14,
                                           loadC15,loadC16,loadC17,loadC18,loadC19,loadC20,loadC21,loadC22,loadC23,loadC24,loadC25,loadC26,loadC27,loadC28,
                                           loadC29,loadC30,loadC31,loadC32,loadC33,loadC34,loadC35,loadC36,loadC37,loadC38,loadC39,loadC40,loadC41,loadC42,
                                          loadT1,loadT2,loadT3,loadT4,loadT5,loadT6,loadT7,loadT8,loadT9,loadT10,loadT11,loadT12,loadT13,loadT14,
                                          loadT15,loadT16,loadT17,loadT18,loadT19,loadT20,loadT21,loadT22,loadT23,loadT24,loadT25,loadT26,loadT27,loadT28, 
                                           loadT29,loadT30,loadT31,loadT32,loadT33,loadT34,loadT35,loadT36,loadT37,loadT38,loadT39,loadT40, loadT41,loadT42
                      From [ThrTable2] ");


                    // float[] outputThr = new float[84];
                    int ind = 0;
                    //int[] waitTimes;
                    SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, sqlconnection);
                    SQLiteDataReader reader = selectCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        for (int i = 0; i < 42; i++)        //for Min thr
                        {
                            outputThr[ind] = float.Parse(reader[ind++].ToString());   
                        }
                        for (int i = 0; i < 42; i++)     //for Max thr
                        {
                            outputThr[ind] = float.Parse(reader[ind++].ToString());
                        }

                        UpdateThresholds();
                        reader.Close();
                        sqlconnection.Close();
                        return outputThr;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private static void UpdateThresholds()
        {
            int indx = 0;
            thr.outOne1 = outputThr[indx++];
            thr.outOne2 = outputThr[indx++];
            thr.outOne3 = outputThr[indx++];
            thr.outOne4 = outputThr[indx++];
            thr.outOne5 = outputThr[indx++];
            thr.outOne6 = outputThr[indx++];
            thr.outOne7 = outputThr[indx++];
            thr.outOne8 = outputThr[indx++];
            thr.outOne9 = outputThr[indx++];
            thr.outOne10 = outputThr[indx++];
            thr.outOne11 = outputThr[indx++];
            thr.outOne12 = outputThr[indx++];
            thr.outOne13 = outputThr[indx++];
            thr.outOne14 = outputThr[indx++];

            thr.outTwo1 = outputThr[indx++];
            thr.outTwo2 = outputThr[indx++];
            thr.outTwo3 = outputThr[indx++];
            thr.outTwo4 = outputThr[indx++];
            thr.outTwo5 = outputThr[indx++];
            thr.outTwo6 = outputThr[indx++];
            thr.outTwo7 = outputThr[indx++];
            thr.outTwo8 = outputThr[indx++];
            thr.outTwo9 = outputThr[indx++];
            thr.outTwo10 = outputThr[indx++];
            thr.outTwo11 = outputThr[indx++];
            thr.outTwo12 = outputThr[indx++];
            thr.outTwo13 = outputThr[indx++];
            thr.outTwo14 = outputThr[indx++];

            thr.outThree1 = outputThr[indx++];
            thr.outThree2 = outputThr[indx++];
            thr.outThree3 = outputThr[indx++];
            thr.outThree4 = outputThr[indx++];
            thr.outThree5 = outputThr[indx++];
            indx++;    //thr.outThree6 = outputThr[indx++];
            indx++;    //thr.outThree7 = outputThr[indx++];
            indx++;    //thr.outThree8 = outputThr[indx++];
            indx++;   //thr.outThree9 = outputThr[indx++];
            indx++;   //thr.outThree10 = outputThr[indx++];
            indx++;   //thr.outThree11 = outputThr[indx++];
            indx++;   //thr.outThree12 = outputThr[indx++];
            indx++;   //thr.outThree13 = outputThr[indx++];
            indx++;   //thr.outThree14 = outputThr[indx++];
            //Max

            thr.outOne1Mx = outputThr[indx++];
            thr.outOne2Mx = outputThr[indx++];
            thr.outOne3Mx = outputThr[indx++];
            thr.outOne4Mx = outputThr[indx++];
            thr.outOne5Mx = outputThr[indx++];
            thr.outOne6Mx = outputThr[indx++];
            thr.outOne7Mx = outputThr[indx++];
            thr.outOne8Mx = outputThr[indx++];
            thr.outOne9Mx = outputThr[indx++];
            thr.outOne10Mx = outputThr[indx++];
            thr.outOne11Mx = outputThr[indx++];
            thr.outOne12Mx = outputThr[indx++];
            thr.outOne13Mx = outputThr[indx++];
            thr.outOne14Mx = outputThr[indx++];

            thr.outTwo1Mx = outputThr[indx++];
            thr.outTwo2Mx = outputThr[indx++];
            thr.outTwo3Mx = outputThr[indx++];
            thr.outTwo4Mx = outputThr[indx++];
            thr.outTwo5Mx = outputThr[indx++];
            thr.outTwo6Mx = outputThr[indx++];
            thr.outTwo7Mx = outputThr[indx++];
            thr.outTwo8Mx = outputThr[indx++];
            thr.outTwo9Mx = outputThr[indx++];
            thr.outTwo10Mx = outputThr[indx++];
            thr.outTwo11Mx = outputThr[indx++];
            thr.outTwo12Mx = outputThr[indx++];
            thr.outTwo13Mx = outputThr[indx++];
            thr.outTwo14Mx = outputThr[indx++];

            thr.outThree1Mx = outputThr[indx++];
            thr.outThree2Mx = outputThr[indx++];
            thr.outThree3Mx = outputThr[indx++];
            thr.outThree4Mx = outputThr[indx++];
            thr.outThree5Mx = outputThr[indx++];
            indx++;     //thr.outThree6Mx = outputThr[indx++];
            indx++;     //thr.outThree7Mx = outputThr[indx++];
            indx++;     //thr.outThree8Mx = outputThr[indx++];
            indx++;     //thr.outThree9Mx = outputThr[indx++];
            indx++;     //thr.outThree10Mx = outputThr[indx++];
            indx++;     //thr.outThree11Mx = outputThr[indx++];
            indx++;     //thr.outThree12Mx = outputThr[indx++];
            indx++;     //thr.outThree13Mx = outputThr[indx++];
            indx++;     //thr.outThree14Mx = outputThr[indx++];
        }
      
    }
}
