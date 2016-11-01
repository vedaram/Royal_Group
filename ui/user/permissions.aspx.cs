using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
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

public partial class user_permissions : System.Web.UI.Page
{
    const string page = "ACCESS_PERMISSIONS";

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

            message = "An error occurred while loading Access Permissions page. Please try again. If the error persists, please contact Support.";

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

	private string GetBaseQuery(string user_id)
    {
        string query = "select DISTINCT l.EmpId as employee_code, Emp_Name as employee_name from EmployeeMaster e, Login l where l.Empid = e.Emp_Code and l.Status = 1";

        return query;
    }

    private string GetFilterQuery(string query, string filters)
    {
        JObject filters_data = JObject.Parse(filters);
        int user_category = Convert.ToInt32(filters_data["filter_user_category"]);
        string filter_keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        if (user_category >= 0) {
            switch (user_category) {
            case 0: 
                query += " and l.access = 0 ";
                break;
            case 1:
                query += " and l.access = 1 ";
                break;
            case 2:
                query += " and l.access = 2 ";
                break;
            case 3:
                query += " and l.access = 3 ";
                break;
            }
        }

        switch (filter_by)
        { 
            case 1:
                query += " and e.Emp_Code = '" + filter_keyword + "' ";
                break;
            case 2:
                query += " and e.Emp_Name like '%" + filter_keyword + "%' ";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetEmployeeData(int page_number, bool is_filter, string filters)
    {
        user_permissions page_object = new user_permissions();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable employee_data = new DataTable();

        int 
            start_row = (page_number - 1) * 30,
            number_of_rows = page_number * 30 + 1;

        string
            query = string.Empty,
            user_id = string.Empty;

        try
        {
            user_id = HttpContext.Current.Session["username"].ToString();
            if (user_id != "admin")
                user_id = HttpContext.Current.Session["employee_id"].ToString();

            query = page_object.GetBaseQuery(user_id);

            if (is_filter)
                query = page_object.GetFilterQuery(query, filters);

            query += " ORDER BY l.EmpId OFFSET " + start_row + " ROWS FETCH NEXT 30 ROWS ONLY";

            employee_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employee_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_EMPLOYEE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Employee data. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }
        
        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetPermissions(string employee_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable permissions_data = new DataTable();
        string query = string.Empty;

        try
        {   
            query = "select * from EMPLOYEE_PERMISSIONS where EMPLOYEE_CODE = '" + employee_code + "' ";
            permissions_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(permissions_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_PERMISSIONS_FOR_EMPLOYEE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading permissions for the selected Employee. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject SavePermissions(string employees, string permissions)
    {
    	DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
    	List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);
    	string query                = string.Empty;
        int i                       = 0;

        try
        {
        	for (i = 0; i < employees_list.Count; i++) {

        		query = "select count(*) from EMPLOYEE_PERMISSIONS where EMPLOYEE_CODE = '" + employees_list[i] + "' ";
                if (db_connection.GetRecordCount(query) > 0)
                {
                    query = "update EMPLOYEE_PERMISSIONS set PERMISSIONS ='" + permissions + "' where EMPLOYEE_CODE ='" + employees_list[i] + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
                else
                {
                    query = "insert into EMPLOYEE_PERMISSIONS(EMPLOYEE_CODE, PERMISSIONS) values ('" + employees_list[i] + "', '" + permissions + "')";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
                
            }
            
            return_object.status = "success";
            return_object.return_data = "Permissions saved successfully!";
        }
        catch (Exception ex)
        {
        	Logger.LogException(ex, page, "SAVE_PERMISSIONS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeletePermissions(string employee_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;

        try
        {
            query = "delete from EMPLOYEE_PERMISSIONS where EMPLOYEE_CODE = '" + employee_code + "' ";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            return_object.status = "success";
            return_object.return_data = "Permissions deleted successfully!";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DELETE_PERMISSIONS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while deleting permissions for the selected employee. Please try again.";

            throw;
        }

        return return_object;
    }
}
