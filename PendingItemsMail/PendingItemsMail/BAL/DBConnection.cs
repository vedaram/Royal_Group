using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

/// <summary>
/// Summary description for DBConnection
/// </summary>
public class DBConnection
{

    public SqlConnection getConnection()
    {
        SqlConnection con = new SqlConnection();
        try
        {
            IniFile f = new IniFile(string.Format("{0}\\Securtime.ini", Application.StartupPath));
            string server = f.IniReadValue("server", "server");
            con = new SqlConnection(server);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        catch (Exception ex)
        {
            createfile(ex.Message.ToString() + DateTime.Now);
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        return con;
    }

    public int GetRecordCount(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        int recordCount = 0;
        try
        {
            con = getConnection();
            con.Open();
            SqlCommand cmd = new SqlCommand(sql_query, con);
            recordCount = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return recordCount;
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString() + DateTime.Now + " GetRecordCount; Query=" + sql_query);
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
            return recordCount;

        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public void ExecuteQuery_WithOutReturnValue(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        try
        {
            con = getConnection();
            con.Open();
            SqlCommand cmd = new SqlCommand(sql_query, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

    }

    public string ExecuteQuery_WithReturnValueString(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        string ReturnValue = string.Empty;
        SqlCommand cmdCommand = new SqlCommand();
        try
        {
            con = getConnection();
            con.Open();
            SqlCommand cmd = new SqlCommand(sql_query, con);
            ReturnValue = Convert.ToString(cmd.ExecuteScalar());
            con.Close();
            return ReturnValue;
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
            return ReturnValue = "error";
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

    }

    public string ExecuteSQLQuery_ScalarStringMultiColumns(string sSQL)
    {
        string weekOf1 = string.Empty, weekOf2 = string.Empty;
        SqlConnection con = new SqlConnection();
        string ReturnValue = string.Empty;
        SqlCommand cmdCommand = new SqlCommand();
        try
        {
            con = getConnection();
            con.Open();


            SqlCommand cmd = new SqlCommand(sSQL, con);
            string str = "";
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {

                weekOf1 = dr["WeeklyOff1"].ToString();
                weekOf2 = dr["WeeklyOff2"].ToString();

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return weekOf1 + ":" + weekOf2;
    }

    public int ExecuteQuery_WithReturnValueInteger(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        int ReturnValue = 0;
        SqlCommand cmdCommand = new SqlCommand();
        try
        {
            con = getConnection();
            con.Open();
            cmdCommand = new SqlCommand(sql_query, con);
            cmdCommand.CommandTimeout = 0;
            ReturnValue = Convert.ToInt32(cmdCommand.ExecuteScalar());
            return ReturnValue;
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
            return ReturnValue;
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public double ExecuteQuery_WithReturnValueDouble(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        double ReturnValue = 0.0;
        SqlCommand cmdCommand = new SqlCommand();
        try
        {
            con = getConnection();
            con.Open();
            cmdCommand = new SqlCommand(sql_query, con);
            cmdCommand.CommandTimeout = 0;
            ReturnValue = Convert.ToDouble(cmdCommand.ExecuteScalar());
            return ReturnValue;
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
            return ReturnValue;
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }


    public DataTable ReturnDataTable(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        DataTable dt_obejct = new DataTable();
        try
        {
            con = getConnection();

            SqlDataAdapter da = new SqlDataAdapter(sql_query, con);
            da.Fill(dt_obejct);
            return dt_obejct;
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
            return dt_obejct;
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public DataSet ReturnDataSet(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        DataSet ds = new DataSet();
        try
        {
            con = getConnection();

            SqlDataAdapter da = new SqlDataAdapter(sql_query, con);
            da.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
            return ds;
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public void ExecuteStoredProcedure_WithoutReturn(string procedureName)
    {

        SqlConnection con = getConnection();
        try
        {
            con.Open();
            SqlCommand comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;
            comObject.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public void ExecuteStoredProcedureWithHashtable_WithoutReturn(string procedureName, Hashtable hashTable)
    {
        SqlConnection con = getConnection();
        try
        {
            IDictionaryEnumerator ien;
            con.Open();
            SqlCommand comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;

            if (hashTable != null)
            {
                ien = hashTable.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, sValue));
                }
            }
            comObject.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public void ExecuteStoredProcedureWithHashtable_WithoutReturn_pi(string procedureName, Hashtable hashTable)
    {
        SqlConnection con = getConnection();
        try
        {
            IDictionaryEnumerator ien;
            con.Open();
            SqlCommand comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;

            if (hashTable != null)
            {
                ien = hashTable.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@pi" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, sValue));
                }
            }
            comObject.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public DataTable ExecuteStoredProcedureWithHashtable_WithReturnDatatable(string procedureName, Hashtable hashTable)
    {
        SqlConnection con = getConnection();
        SqlCommand comObject = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt_object = new DataTable();
        try
        {
            IDictionaryEnumerator ien;
            con.Open();
            comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;

            if (hashTable != null)
            {
                ien = hashTable.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, sValue));
                }
            }

            da = new SqlDataAdapter(comObject);
            da.Fill(dt_object);
            con.Close();
            return dt_object;
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
            return dt_object;
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public string ExecuteStoredProcedureReturnString(string procedureName)
    {
        SqlConnection con = getConnection();
        SqlCommand comObject = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt_object = new DataTable();
        string ReturnValue = string.Empty;
        try
        {
            con = getConnection();
            comObject = new SqlCommand(procedureName, con);
            comObject.CommandTimeout = 0;
            comObject.CommandType = CommandType.StoredProcedure;
            ReturnValue = Convert.ToString(comObject.ExecuteNonQuery().ToString());

        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);

        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return ReturnValue;

    }

    public int ExecuteStoredProcedureReturnInteger(string procedureName, Hashtable hashtable_data)
    {
        SqlConnection con = getConnection();
        SqlCommand comObject = new SqlCommand();

        int ReturnValue = 0;

        try
        {
            IDictionaryEnumerator ien;
            con.Open();
            comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;

            if (hashtable_data != null)
            {
                ien = hashtable_data.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@pi" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, sValue));
                }
            }

            ReturnValue = comObject.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);

        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return ReturnValue;
    }

    public int ExecuteStoredProcedureReturnInteger_OneOutput(string procedureName, string OutputParameter, Hashtable hashtable_data)
    {
        SqlConnection con = getConnection();
        SqlCommand comObject = new SqlCommand();

        int ReturnValue = 0;

        try
        {
            IDictionaryEnumerator ien;
            con.Open();
            comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;

            if (hashtable_data != null)
            {
                ien = hashtable_data.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, sValue));
                }
            }

            comObject.Parameters.Add(OutputParameter, SqlDbType.Int, 100000);
            comObject.Parameters[OutputParameter].Direction = ParameterDirection.Output;

            comObject.ExecuteNonQuery();
            ReturnValue = Convert.ToInt32(comObject.Parameters[OutputParameter].Value.ToString());

        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);

        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return ReturnValue;
    }

    public string ExecuteStoredProcedureReturnString(string procedureName, Hashtable hashtable_data)
    {
        SqlConnection con = getConnection();
        SqlCommand comObject = new SqlCommand();

        string ReturnValue = string.Empty;

        try
        {
            IDictionaryEnumerator ien;
            con.Open();
            comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;

            if (hashtable_data != null)
            {
                ien = hashtable_data.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@pi" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, sValue));
                }
            }

            ReturnValue = Convert.ToString(comObject.ExecuteNonQuery());

        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);

        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return ReturnValue;
    }

    public string ExecuteStoredProcedureReturnStringNoPI(string procedureName, Hashtable hashtable_data)
    {
        SqlConnection con = getConnection();
        SqlCommand comObject = new SqlCommand();
        string sKey = null;
        string sValue = null;
        string ReturnValue = string.Empty;

        try
        {
            string functionReturnValue = string.Empty;
            SqlCommand cmdCommand = new SqlCommand(); ;

            IDictionaryEnumerator pParam = hashtable_data.GetEnumerator();

            con.Open();
            cmdCommand = new SqlCommand(procedureName, con);
            cmdCommand.CommandType = CommandType.StoredProcedure;
            cmdCommand.CommandTimeout = 0;
            while (pParam.MoveNext())
            {
                sKey = "@" + pParam.Key;
                sValue = pParam.Value.ToString();
                cmdCommand.Parameters.Add(new SqlParameter(sKey, sValue));
            }

            ReturnValue = Convert.ToString(cmdCommand.ExecuteScalar());

        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);

        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return ReturnValue;
    }

    public bool RecordExist(string sql_query)
    {
        SqlConnection con = new SqlConnection();
        bool ReturnValue = false;

        try
        {
            con = getConnection();
            con.Open();
            SqlCommand cmd = new SqlCommand(sql_query, con);
            int n = Convert.ToInt32(cmd.ExecuteScalar());
            if (n != 0)
            {
                ReturnValue = true;
            }
            con.Close();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return ReturnValue;

    }

    public int exeStoredProcedure_WithHashtable_ReturnRow(string pnm, Hashtable hashTable)
    {
        SqlConnection con = getConnection();
        SqlCommand comObject = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dt_object = new DataTable();
        int ReturnValue = 0;
        try
        {
            con.Open();
            IDictionaryEnumerator ien;
            comObject = new SqlCommand(pnm, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;
            if (hashTable != null)
            {
                ien = hashTable.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, ien.Value));
                }

            }
            ReturnValue = comObject.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);

        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        return ReturnValue;
    }

    public void ExecuteStoredProcedure_EmployeeMaster(string procedureName, Hashtable hashTable)
    {
        SqlConnection con = getConnection();
        try
        {
            IDictionaryEnumerator ien;
            con.Open();
            SqlCommand comObject = new SqlCommand(procedureName, con);
            comObject.CommandType = CommandType.StoredProcedure;
            comObject.CommandTimeout = 0;

            if (hashTable != null)
            {
                ien = hashTable.GetEnumerator();

                while (ien.MoveNext())
                {
                    string skey = "@" + Convert.ToString(ien.Key);
                    string sValue = Convert.ToString(ien.Value);
                    comObject.Parameters.Add(new SqlParameter(skey, ien.Value));
                }
            }
            comObject.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }

    public void createfile(string errorMessage)
    {
        /*        string path = ConfigurationManager.AppSettings["manualimperror"];
                string fileName = "DataTierError_" + DateTime.Now.ToString("ddMMyyyy");
                FileStream fs = new FileStream(@path + fileName + ".txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Environment.NewLine + errorMessage + " at " + DateTime.Now);
                sw.Flush();
                sw.Close();
                fs.Close();*/
    }

    public string ExecuteProcedureInOutParameters(string procedureName, string InParameter1, string InParameter2, string OutParameter)
    {
        SqlConnection con = getConnection();
        string returnValue = string.Empty;

        string inParam1 = string.Empty;
        string inParam2 = string.Empty;
        string outParam = string.Empty;

        try
        {
            inParam1 = "@" + InParameter1;
            inParam2 = "@" + InParameter2;
            outParam = "@" + OutParameter;

            con.Open();
            SqlCommand cmd = new SqlCommand(procedureName, con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Emp_Code", InParameter1);
            cmd.Parameters.AddWithValue("@LeaveCode", InParameter2);
            cmd.Parameters.Add(outParam, SqlDbType.VarChar, 100000);

            cmd.Parameters[outParam].Direction = ParameterDirection.Output;

            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();

            con.Close();

            returnValue = cmd.Parameters[outParam].Value.ToString();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        return returnValue;
    }

    public int ExecuteProcedureInOutParametersToChkWO(string procedureName, string InParameter1, string InParameter2, string OutParameter)
    {
        SqlConnection con = getConnection();
        int returnValue = 0;

        string inParam1 = string.Empty;
        string inParam2 = string.Empty;
        string outParam = string.Empty;

        try
        {
            inParam1 = "@" + InParameter1;
            inParam2 = "@" + InParameter2;
            outParam = "@" + OutParameter;

            con.Open();
            SqlCommand cmd = new SqlCommand(procedureName, con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@WODate", InParameter1);
            cmd.Parameters.AddWithValue("@EmpID", InParameter2);
            cmd.Parameters.Add(outParam, SqlDbType.VarChar, 100000);

            cmd.Parameters[outParam].Direction = ParameterDirection.Output;

            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();

            con.Close();

            returnValue = Convert.ToInt32(cmd.Parameters[outParam].Value.ToString());
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        return returnValue;
    }


    public string ExecuteProcedureOneInOutParameters(string procedureName, string InParameter, string OutParameter)
    {
        SqlConnection con = getConnection();
        string returnValue = string.Empty;

        string inParam1 = string.Empty;
        string inParam2 = string.Empty;
        string outParam = string.Empty;

        try
        {
            inParam1 = "@" + InParameter;
            outParam = "@" + OutParameter;

            con.Open();
            SqlCommand cmd = new SqlCommand(procedureName, con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Emp_Code", InParameter);
            cmd.Parameters.Add(outParam, SqlDbType.VarChar, 100000);

            cmd.Parameters[outParam].Direction = ParameterDirection.Output;

            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();

            con.Close();

            returnValue = cmd.Parameters[outParam].Value.ToString();
        }
        catch (Exception ex)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            createfile(ex.Message.ToString());
            HttpContext ctx = HttpContext.Current;
            string pagename = ctx.Request.Url.Segments[ctx.Request.Url.Segments.Length - 1];
            createfile("Event logged by " + ((new StackTrace()).GetFrame(1).GetMethod().Name) + " in " + pagename);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        return returnValue;
    }

}
