using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class device_information : System.Web.UI.Page
{
    const string page = "DEVICE_INFORMATION";
    
    protected void Page_Load(object sender, EventArgs e) {

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string message = string.Empty;
        
        try {

            if (Session["username"] == null) {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex) {

            Logger.LogException(ex, page, "PAGE_LOAD");

            message = "An error occurred while loading Device Information page. Please try again. If the error persists, please contact Support.";
            
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    private string GetFilterQuery(string filters, string query)
    {

        JObject filters_data = JObject.Parse(filters);
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);
        string keyword = filters_data["filter_keyword"].ToString();

        switch (filter_by)
        {

            case 1:
                query += " and dl.devicelocation like '%" + keyword + "%' ";
                break;
            case 2:
                query += " and dl.deviceid = '" + keyword + "' ";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetDeviceData(int page_number, bool is_filter, string filters)
    {

        device_information page_object = new device_information();
        DBConnection db_connection = new DBConnection();
        DataTable device_location_data = new DataTable();
        ReturnObject return_object = new ReturnObject();
        int start_row = (page_number - 1) * 30;
        string query = string.Empty;

        try
        {
            query = "select dl.Deviceid as device_id, dl.DeviceLocation as device_location, lp.LastPunchDateTime as download_punch_time from Device_Location dl, LastDateSAXPush lp where dl.DeviceId = lp.Deviceid";

            if (is_filter) query = page_object.GetFilterQuery(filters, query);

            query += " ORDER BY dl.DeviceLocation OFFSET " + start_row + " ROWS FETCH NEXT 30 ROWS ONLY";

            device_location_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(device_location_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_DEVICE_LOCATION_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetBranchData()
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable branch_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select BranchCode, BranchName from branchmaster";
            branch_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(branch_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_BRANCH_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Branch data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    public void UpdateDeviceLocation(string mode, string device_location, string device_id)
    {

        DBConnection db_connection = new DBConnection();
        Hashtable device_data = new Hashtable();

        device_data.Add("Mode", mode);
        device_data.Add("Location", device_location);
        device_data.Add("deviceserialno", device_id);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("SpUpdateDeviceLocation", device_data);
    }

    [WebMethod]
    public static ReturnObject UpdateDevice(string current)
    {

        device_information page_object = new device_information();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string branch_code = string.Empty;
        string device_location = string.Empty;
        string device_id = string.Empty;

        try
        {

            JObject current_data = JObject.Parse(current);
            device_location = current_data["device_location"].ToString();
            device_id = current_data["device_id"].ToString();

            page_object.UpdateDeviceLocation("U", device_location, device_id);

            return_object.status = "success";
            return_object.return_data = "Device details updated successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "UPDATE_DEVICE_LOCATION");

            return_object.status = "error";
            return_object.return_data = "An error occurred while updating Device details. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteDevice(string current)
    {

        device_information page_object = new device_information();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string branch_code = string.Empty;
        string device_location = string.Empty;
        string device_id = string.Empty;

        try
        {

            JObject current_data = JObject.Parse(current);
            device_location = current_data["device_location"].ToString();
            device_id = current_data["device_id"].ToString();

            page_object.UpdateDeviceLocation("D", device_location, device_id);

            return_object.status = "success";
            return_object.return_data = "Device Location deleted successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "DELETE_DEVICE_LOCATION");

            return_object.status = "error";
            return_object.return_data = "An error occurred while deleting the Device Location. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }
}
