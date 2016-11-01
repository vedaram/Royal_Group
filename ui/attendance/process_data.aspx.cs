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

public partial class attendance_process_data : System.Web.UI.Page
{
	const string page = "PROCESS_DATA";

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
            // log the exception
            Logger.LogException(ex, page, "PAGE_LOAD");

            message = "An error occurred while loading Process Data page. Please try again. If the error persists, please contact Support.";

            // display a generic error in the UI
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
    public static ReturnObject GetUnprocessedData(int pageNumber)
    {
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DataTable unprocessedData = new DataTable();
        int startRow = (pageNumber - 1) * 30;
        string query = string.Empty;

        try
        {
            query = "select EmpID as employee_code, CardNo as card_number, convert(varchar(30),Punch_Time,120) as punch_time, DeviceID as device_id from Trans_raw#_Temp order by Punch_Time desc OFFSET " + startRow + " ROWS FETCH NEXT 30 ROWS ONLY";

            unprocessedData = dbConnection.ReturnDataTable(query);

            returnObject.status = "success";
            returnObject.return_data = JsonConvert.SerializeObject(unprocessedData, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_UNPROCESSED_DATA");

            returnObject.status = "error";
            returnObject.return_data = "An error occurred while loading data for the page. Please try again";

            throw ex;
        }

        return returnObject;
    }

    [WebMethod]
    public static ReturnObject ProcessData()
    {
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        Hashtable processData = new Hashtable();
        string returnMessage = string.Empty;
        string CheckReprocessQuery = string.Empty;
        string CheckProcessQuery = string.Empty;
        int ReprocessFlag = 0;
        int ProcessFlag = 0;

        try
        {
            string username = HttpContext.Current.Session["username"].ToString();

            //checking whether reprocess is happening
            CheckReprocessQuery = "select Count(re_flag) from ReprocessFlag where re_flag=1";
            ReprocessFlag = dbConnection.GetRecordCount(CheckReprocessQuery);

            CheckProcessQuery = "select Count(pflag) from ProcessingStatus where pflag=1";
            ProcessFlag = dbConnection.GetRecordCount(CheckProcessQuery);

            if (ReprocessFlag != 1 && ProcessFlag != 1)//checking whether reprocess is happening
            {

                returnMessage = "Data processing started at " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

                processData.Clear();
                processData.Add("Flag", "I"); //Insert latest records from Trans_raw#_Temp to Trans_raw#
                dbConnection.ExecuteStoredProcedureWithHashtable_WithoutReturn("preProcessing", processData);

                processData.Clear();
                processData.Add("Flag", "P"); // Start Processing  the records of Trans_raw#
                dbConnection.ExecuteStoredProcedureWithHashtable_WithoutReturn("preProcessing", processData);

                returnMessage += "Data processing completed at " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
            }
            else
            {
                returnMessage = "Data is currently being Reprocessed or Processed. Please try again later." + System.Environment.NewLine;
                returnMessage += "Reprocess Flag: " + ReprocessFlag + System.Environment.NewLine;
                returnMessage += "Process Falg: " + ProcessFlag + System.Environment.NewLine;
            }

            returnObject.status = "success";
            returnObject.return_data = returnMessage;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "PROCESS_DATA");

            returnObject.status = "error";
            returnObject.return_data = "An error occurred while performing this operation. Please try again";

            throw ex;
        }


        return returnObject;
    }
}
