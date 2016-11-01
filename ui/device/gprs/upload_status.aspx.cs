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
using SecurAX.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class template_upload_status : System.Web.UI.Page
{
	const string page = "TEMPLATE_UPLOAD_STATUS";

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

            message = "An error occurred while loading Template Upload page. Please try again. If the error persists, please contact Support.";

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

    private string GetBaseQuery()
    {
    	string query = "select distinct(EnrollID) as enroll_id, Name as employee_name, DeviceID as device_id, case UploadFlag when 2 then 'uploaded' else 'Inprogress' end as upload_flag from EnrollmentIDList ";

    	return query;
    }

    private string GetFilterQuery(string query, string filters) 
    {
    	JObject filter_data = JObject.Parse(filters);
    	int filter_by = Convert.ToInt32(filter_data["filter_by"]);
    	string filter_keyword = filter_data["filter_keyword"].ToString();
    	int upload_status = Convert.ToInt32(filter_data["filter_upload_status"]);

    	if (upload_status != -99 || filter_by != 0 || filter_keyword != "")
    	{
    		query += " where ";

    		if (upload_status > -99) 
    		{
    			query += " UploadFlag = '" + upload_status + "' ";
    		}

    		if (filter_by > 0) 
    		{
    			if (upload_status > -99) 
    				query += " and ";

				switch (filter_by)
				{
					case 1: 
						query += " EnrollID = '" + filter_keyword + "' ";
						break;
					case 2:
						query += " Name like '%" + filter_keyword + "%' ";
						break;
					case 3:
						query += " DeviceID = '" + filter_keyword + "' ";
						break;
				}
    		}
    	}

    	return query;
    }

    [WebMethod]
    public static ReturnObject GetEnrollmentData(int page_number, bool is_filter, string filters)
    {
    	template_upload_status page_object = new template_upload_status();
    	DBConnection db_connection = new DBConnection();
    	ReturnObject return_object = new ReturnObject();
    	DataTable enrollment_data = new DataTable();
    	string query = string.Empty;
    	int start_row = (page_number - 1) * 30;

        try
        {
        	query = page_object.GetBaseQuery();
        	if (is_filter)
        		query = page_object.GetFilterQuery(query, filters);

    		query += " ORDER BY EnrollID OFFSET " + start_row + " ROWS FETCH NEXT 30 ROWS ONLY";

    		enrollment_data = db_connection.ReturnDataTable(query);

    		return_object.status = "success";
    		return_object.return_data = JsonConvert.SerializeObject(enrollment_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
        	Logger.LogException(ex, page, "GET_ENROLLMENT_DATA");
        }

        return return_object;
    }
}
