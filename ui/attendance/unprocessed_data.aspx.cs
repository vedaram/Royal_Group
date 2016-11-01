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
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class attendance_unprocessed_data : System.Web.UI.Page
{
    const string page = "UNPROCESSED_DATA";

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

            message = "An error occurred while loading Unprocessed Data page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetUnprocessedData(int page_number)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable unprocessed_data = new DataTable();
        int start_row = (page_number - 1) * 30;

        string
            query = string.Empty;

        try
        {
            // JOIN with employee master to get employee code.
            query = "select cardno, empid, Punch_time from unprocessed_punches";
            query = "select up.cardno as card_number, e.Emp_code as employee_code, up.Punch_time as punch_time from Unprocessed_punches up left outer join EmployeeMaster e on up.cardno = e.Emp_Card_No ORDER BY up.Punch_Time DESC OFFSET " + start_row + " ROWS FETCH NEXT 30 ROWS ONLY";

            unprocessed_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(unprocessed_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_UNPROCESSED_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Unprocessed data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DoReprocess()
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable where_conditions = new Hashtable();
        string 
            query = string.Empty,
            where_condition = string.Empty;
        int count = 0;

        try
        {
            query = "select count(*) from unprocessed_punches";
            count = db_connection.ExecuteQuery_WithReturnValueInteger(query);

            if (count > 0)
            {
                db_connection.ExecuteStoredProcedure_WithoutReturn("securtime_unprocessed");

                return_object.status = "success";
                return_object.return_data = "Reprocessing completed successfully!";
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = "No data found for reprocessing.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "REPROCESS_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while reprocessing the data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }
}