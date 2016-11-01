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

public partial class profile_update : System.Web.UI.Page
{
	const string page = "PROFILE";

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

            message = "An error occurred while loading Company Master page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetEmployeeData()
    {
    	DBConnection db_connection = new DBConnection();
    	ReturnObject return_object = new ReturnObject();
    	DataTable employee_data = new DataTable();
        string employee_code = string.Empty;
    	string query = string.Empty;

    	try
    	{
            employee_code = HttpContext.Current.Session["username"].ToString();
            if (employee_code != "admin")
                employee_code = HttpContext.Current.Session["employee_id"].ToString();

            query = "select e.Emp_Code as employee_code, e.Emp_Name as employee_name, ds.DesigName as designation_name, d.DeptName as department_name from EmployeeMaster e left join DeptMaster d on e.Emp_Department = d.DeptCode left join DesigMaster ds on e.Emp_Designation = ds.DesigCode where Emp_Code='" + employee_code + "'";

    		employee_data = db_connection.ReturnDataTable(query);

    		return_object.status = "success";
    		return_object.return_data = JsonConvert.SerializeObject(employee_data);
    	}
    	catch (Exception ex)
    	{
    		Logger.LogException(ex, page, "GET_EMPLOYEE_DATA");

    		throw;
    	}

    	return return_object;
    }

    [WebMethod]
    public static ReturnObject SavePassword(string current)
    {
    	DBConnection db_connection = new DBConnection();
    	ReturnObject return_object = new ReturnObject();
    	JObject current_data = new JObject();
        int count = 0;
        string
            current_password = string.Empty, query = string.Empty,
            new_password = string.Empty, username = string.Empty;

    	try
    	{
            username = HttpContext.Current.Session["username"].ToString();

    		current_data = JObject.Parse(current);
    		current_password = current_data["current_password"].ToString();
    		new_password = current_data["new_password"].ToString();

    		query = "select count(*) from login where UserName = '" + username + "' and Password = '" + current_password + "' ";
            count = db_connection.GetRecordCount(query);
            if (count > 0)
            {
                query = "update login set password = '" + new_password + "' where UserName = '" + username + "' ";
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                return_object.status = "success";
                return_object.return_data = "Password changed successfully!";
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = "Please check your current password";
            }
    	}
    	catch (Exception ex)
    	{
    		Logger.LogException(ex, page, "SAVE_PASSWORD");

    		throw;
    	}

    	return return_object;
    }
}
