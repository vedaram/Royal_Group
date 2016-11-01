using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;
using System.Web.Services;
using SecurAX.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class attendance_usb_download : System.Web.UI.Page
{
    const string page = "USB_DOWNLOAD_AND_PROCESS";

    protected void Page_Load(object sender, EventArgs e)
    {

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string message = string.Empty;

        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "PAGE_LOAD");

            message = "An error occurred while loading USB Download & Process page. Please try again. If the error persists, please contact Support.";

            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    [WebMethod]
    public static ReturnObject GetDeviceLocationData()
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable device_location_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DeviceLocation as devicelocation, deviceid from Device_Location order by DeviceLocation";
            device_location_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(device_location_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_DEVICE_LOCATION_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Device Location data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DoImport(string file_name, string device_id) 
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable device_details = new Hashtable();
        DateTime last_date_time = new DateTime();

        int count = 0;

        string
            query = string.Empty,
            temp_donwload_date = string.Empty,
            file_path = string.Empty,
            upload_path = string.Empty;

        try
        {
            upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            file_path = HttpContext.Current.Server.MapPath("~/" + upload_path + "/" + file_name);

            query = "bulk insert temptrans from '" + file_path + "' with(fieldterminator ='\t',rowterminator='\n')";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            last_date_time = Convert.ToDateTime("2015-01-01 00:00:00");

            query = "select DownloadPunchTime from LastDate_BioPush where DeviceId='" + device_id + "'";
            temp_donwload_date = db_connection.ExecuteQuery_WithReturnValueString(query);
                
            if (!string.IsNullOrEmpty(temp_donwload_date))
            {
                last_date_time = Convert.ToDateTime(temp_donwload_date);
            }
            else
            {
                last_date_time = DateTime.Now.AddMonths(-1);
                last_date_time = last_date_time.AddDays(-DateTime.Now.Day);
                last_date_time = last_date_time.AddDays(15);
            }

            device_details.Add("deviceid", device_id);
            device_details.Add("devicelastdate", last_date_time);
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("INsertTrans_raw#_USBImport", device_details);

            query = "select COUNT(*) from Trans_Raw# where deviceid='" + device_id + "'";
            count = db_connection.ExecuteQuery_WithReturnValueInteger(query);

            if (File.Exists(file_path))
                File.Delete(file_path);

            return_object.status = "success";
            return_object.return_data = count.ToString() + " records download.";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DO_IMPORT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Processing the file. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }
}