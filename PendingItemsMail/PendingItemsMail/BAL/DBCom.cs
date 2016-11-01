using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
//using System.Windows.Forms;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;


//using Microsoft.SqlServer.Management.Common;
//using Microsoft.SqlServer.Management.Smo;


    public class DBCom
    {
        public static string server2;
        //public static string server3;
        public static string port1;
        //public static string port2;
        public static string userid1;
        public static string userid3;
        public static string userid8;
        //public static string userid2;
        public static string password1;
        public static SqlConnection con;
        public static SqlCommand cmd;
        public static SqlDataAdapter adp;
        public static string DateTime1;
        public static int imghgt;
        public static int imgwdt;
        //public static Bitmap image;
        public static SqlDataReader rdr;
        public static SqlDataReader rdr1;
        public static SqlDataReader rdr2;
        public static SqlDataReader rdr3;
        public static DataSet dst;
        public static DataTable dtbl;
        public static string id = "";
        public static string imd = "";
        public static int ret;
        public static int empmax;
        public static int devmax;
        public static string devserialnos;
        public static string lstatus;
      
        public static string str;
        public static string gempid="";
        public static string value;
        public static string shift;
         public static string print;
         public static string valid;
         public static string emplist="";
         public static string codec = "";
         public static string cname = "";
         public static string DeviceType = "";
         public static string ModelType = "";
        // public static string dupenroll;
        public static int count;
        public static void getConnection()
        {
            //ret = 0;
            //string server = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            IniFile f = new IniFile(string.Format("{0}\\Securtime.ini", Application.StartupPath));
            string server = f.IniReadValue("server", "server");

            con = new SqlConnection(server);
            if (con.State == ConnectionState.Open)
                con.Close();

            con.Open();

        }
        public static void getData(string qry)
        {
            try
            {
                getConnection();

                cmd = new SqlCommand(qry, con);
                rdr = cmd.ExecuteReader();

                //con.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("ERROR : " + exp1.Message, "Ipo-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // MessageBox.Show(ex.Message);
            }
        }

        public static int getfirstdata(string qry)
        {

            getConnection();
            cmd = new SqlCommand(qry, con);
            ret = Convert.ToInt32(cmd.ExecuteScalar());
            return ret;
        }


        public  static string DateTimeToString(string Date)
        {
            return Date == string.Empty ? string.Empty : DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
        }

        public static int CompareDate(string Date1, string Date2)
        {
            if (Date1 == string.Empty && Date2 == string.Empty)
            {
                return DateTime.ParseExact(Date1, "dd/MM/yyyy", CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(Date2, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            }
            else
            {
                return 2;
            }
        }

        public static int CompareDateTime(string Date1, string Date2)
        {
            if (Date1 == string.Empty && Date2 == string.Empty)
            {
                return DateTime.ParseExact(Date1, "HH:mm", CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(Date2, "HH:mm", CultureInfo.InvariantCulture));
            }
            else {
                return 2;
            }
        }
        public static string DateTimeToString_DB(string Date)
        {
            return Date == string.Empty ? string.Empty : DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
        }

        public static string firstdata(string sr)
        {
            try
            {
                getConnection();

                cmd = new SqlCommand(sr, con);
                string fdata = Convert.ToString(cmd.ExecuteScalar());
                return fdata;


            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
                return "";

            }
        }
        //public static void runscript(string qry)
        //{
        //    try
        //    {
        //        getConnection();

        //        Server server = new Server(new ServerConnection(con));
        //        server.ConnectionContext.ExecuteNonQuery(qry);
        //        //cmd = new SqlCommand(qry, con);
        //        //cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //    }

        //    //con.Close();

        //}

        public static void getData1(string qry)
        {
            try
            {
                getConnection();

                cmd = new SqlCommand(qry, con);
                rdr1 = cmd.ExecuteReader();

                //con.Close();
            }
            catch (Exception exe1)
            {
                //MessageBox.Show("ERROR : " + exp1.Message, "Ipo-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // MessageBox.Show(exe1.Message);
            }
        }
        public static void getData2(string qry)
        {
            try
            {
                getConnection();

                cmd = new SqlCommand(qry, con);
                rdr2 = cmd.ExecuteReader();

                //con.Close();
            }
            catch (Exception exp1)
            {
                //MessageBox.Show("ERROR : " + exp1.Message, "Ipo-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // MessageBox.Show(exp1.Message);
            }
        }


        public static SqlDataAdapter getDataAdapter(string SQL)
        {
            getConnection();
            SqlDataAdapter da = new SqlDataAdapter(SQL, con);
            return da;
        }

        public static void getData3(string qry)
        {
            try
            {
                getConnection();

                cmd = new SqlCommand(qry, con);
                rdr3 = cmd.ExecuteReader();

                //con.Close();
            }
            catch (Exception exp1)
            {
                //MessageBox.Show("ERROR : " + exp1.Message, "Ipo-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show(exp1.Message);
            }
        }

        public static void putData(string qry)
        {

            getConnection();

            cmd = new SqlCommand(qry, con);
            cmd.ExecuteNonQuery();

            //con.Close();

        }


        public static SqlDataAdapter GetDA(string SQL)
        {
            SqlDataAdapter da = new SqlDataAdapter(SQL, con);
            return da;
        }


        public static void countData(string qry)
        {
            if(qry.Contains("Communication"))
            { 
            
            }
            getConnection();

            cmd = new SqlCommand(qry, con);
            count = Convert.ToInt32(DBCom.cmd.ExecuteScalar());
            con.Close();
        }

        public static void bulkCopy(DataTable dtb, string dstTbl)
        {
            getConnection();

            SqlBulkCopy sbc = new SqlBulkCopy(con);
            sbc.DestinationTableName = dstTbl;
            sbc.WriteToServer(dtb);
        }
        public static void GetMailCredentials(out string server3, out string port2, out string userid5, out string userid4, out string password2 , out string userid9)
        {
            GetMailConnection();
            server3 = server2;
            port2 = port1;
            userid5 =userid1;
            userid4 = userid3;
            password2 = password1;
            userid9 = userid8;

        }
        public static void GetMailConnection()
        {
            IniFile f = new IniFile(string.Format("{0}\\Securtime.ini", Application.StartupPath));
            string server1 = f.IniReadValue("MailServer", "server1");
            string port = f.IniReadValue("MailPort", "port");
            string userid = f.IniReadValue("MailUserId", "UserId");
            string useridto = "";// f.IniReadValue("MailTO", "MailTo");
            string password = f.IniReadValue("MailPassword", "Password");
            string usercc = "";// f.IniReadValue("MailCC", "MailCC");
            server2 = server1;
            port1 = port;
            userid1 = userid;
            userid3 = useridto;
            password1 = password;
            userid8 = usercc;
        }

        public static DataTable fillDataTable(string qry)
        {
            getConnection();
            
            dtbl = new DataTable();
            dtbl.Rows.Clear();
            adp = new SqlDataAdapter(qry, con);
            adp.Fill(dtbl);
            return dtbl;
        }
        public static bool RecordExist(string strSql)
        {
            bool functionReturnValue = false;
            try
            {
                getConnection();
                SqlCommand cmd = new SqlCommand(strSql, con);
                int n = 0;
                n = Convert.ToInt32(cmd.ExecuteScalar());
                if (n != 0)
                {
                    functionReturnValue = true;
                }
                else
                {
                    functionReturnValue = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return functionReturnValue;

        }
        public static string getscalar(string qry)
        {
            string ret;
            getConnection();
            cmd = new SqlCommand(qry, con);
            ret = Convert.ToString(cmd.ExecuteScalar());
            return ret;

        }

        public static DataSet fillDataSet1(string qry)
        {
            getConnection();

            dst = new DataSet();
            adp = new SqlDataAdapter(qry, con);
            adp.Fill(dst);

            return dst;
        }

        public static DataSet fillDataSet(string qry, string name)
        {
            getConnection();

            dst = new DataSet();
            adp = new SqlDataAdapter(qry, con);
            adp.Fill(dst, name);

            return dst;
        }

        public static void exeStoredProc(string pnm)
        {
            try
            {
                getConnection();
                cmd = new SqlCommand(pnm, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                ret = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
        }

        public static void exeStoredProc_P1(string prcName, string param1)
        {
            getConnection();

            cmd = new SqlCommand(prcName, con);
            cmd.Parameters.Add("@QRY", SqlDbType.VarChar).Value = param1;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }


        //public static void exeStoredProc_P2(string prcName, string enrollid, string punchtime, string devid)
        //{
        //    getConnection();

        //    cmd = new SqlCommand("PunchInfo", con);
        //    cmd.Parameters.Add("@EnrollNo", SqlDbType.VarChar).Value = enrollid.ToString();
        //    cmd.Parameters.Add("@PunchTime", SqlDbType.DateTime).Value = Convert.ToDateTime(punchtime);
        //    cmd.Parameters.Add("@MachineId", SqlDbType.VarChar).Value = devid.ToString();
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.ExecuteNonQuery();
        //}

        public static SqlDataAdapter DataAdapter(string SQL)
        {
            getConnection();
            SqlDataAdapter DA = new SqlDataAdapter(SQL, con);
            return DA;
        }


        public static void exeStoredProc_P2(string prcName, string enrollid, string punchtime, string pdate, string vCode, string devid, string cardno)
        {
            getConnection();

            cmd = new SqlCommand(prcName, con);
            cmd.Parameters.Add("@EnrollNo", SqlDbType.VarChar).Value = enrollid.ToString();
            cmd.Parameters.Add("@PunchTime", SqlDbType.DateTime).Value = Convert.ToDateTime(punchtime);
            cmd.Parameters.Add("@PunchDate", SqlDbType.DateTime).Value = Convert.ToDateTime(pdate);
            cmd.Parameters.Add("@vCode", SqlDbType.VarChar).Value = vCode.ToString();
            cmd.Parameters.Add("@MachineId", SqlDbType.VarChar).Value = devid.ToString();
           // cmd.Parameters.Add("@CardNO", SqlDbType.VarChar).Value = cardno.ToString();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }
        public static void exeStoredProcedure(string pnm, Hashtable hashTable)
        {
            try
            {
                getConnection();

                IDictionaryEnumerator ien;
                cmd = new SqlCommand(pnm, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                if (hashTable != null)
                {
                    ien = hashTable.GetEnumerator();

                    while (ien.MoveNext())
                    {
                        string skey = "@" + Convert.ToString(ien.Key);                        
                        string sValue = Convert.ToString(ien.Value);
                       // Console.WriteLine("KEy - " + skey.Replace('@',' ') + "Value - "+sValue);
                        cmd.Parameters.Add(new SqlParameter(skey, ien.Value));
                    }

                }
                // cmd.CommandType = CommandType.Text;
                ret = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                str = ex.Message;

                if (str == "String or binary data would be truncated.\r\nThe statement has been terminated.")
                {

                }
                else
                {
                   // MessageBox.Show(ex.Message);
                
                }
            }
        }
    }

