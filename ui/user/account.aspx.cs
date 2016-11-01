using System;
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
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class user_account : System.Web.UI.Page
{
    const string page = "ACCOUNTS";

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

            message = "An error occurred while loading Account page. Please try again. If the error persists, please contact Support.";

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
        string query = "select Empid as employee_code, UserName as username, Password as password, Access as access_level, Emp_Name as employee_name from ( select l.Empid, l.UserName, l.Password, l.Access, e.Emp_Name, ROW_NUMBER() OVER (ORDER BY l.Empid) as row from login l, EmployeeMaster e where l.Empid = e.Emp_code and l.status = 1 ";
        
        return query;
    }

    private string GetFilterQuery(string query, string filters)
    {
        JObject filters_data = JObject.Parse(filters);
        int access_level = Convert.ToInt32(filters_data["filter_access_level"]);
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);
        string filter_keyword = filters_data["filter_keyword"].ToString();

        if (access_level >= 0) {
            query += " and l.Access = '" + access_level + "' ";
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
    public static ReturnObject GetUserData(int page_number, bool is_filter, string filters)
    {
        user_account page_object = new user_account();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable user_data = new DataTable();

        string
            query = string.Empty,
            user_id = string.Empty;

        int
            access_level = 0,
            start_row = (page_number - 1) * 30,
            number_of_rows = page_number * 30 + 1;

        try
        {
            user_id = HttpContext.Current.Session["username"].ToString();
            if (user_id != "admin")
                user_id = HttpContext.Current.Session["employee_id"].ToString();

            access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

            query = page_object.GetBaseQuery();

            switch (access_level)
            {
                case 0:
                    query += " and L.UserName!='admin' and L.Password!='admin' ";
                    break;

                default:
                    query += " and E.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + user_id + "' and Emp_Status=1) ";
                    break;
            }

            query = page_object.GetFilterQuery(query, filters);

            query += " ) a where row > " + start_row + " and row < " + number_of_rows;

            user_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(user_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_USER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading User data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    private void InsertUpdate(string mode, string employee_id, string user_name, string password, int user_type, int status)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable user_details = new Hashtable();

        user_details.Clear();

        user_details.Add("mode", mode);
        user_details.Add("empid", employee_id);
        user_details.Add("username", user_name);
        user_details.Add("password", password);
        user_details.Add("access", user_type);
        user_details.Add("status", status);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("Sp_UserAccount", user_details);
    }

    [WebMethod]
    public static ReturnObject UpdateUser(string current, string previous_user_id)
    {
        user_account page_object = new user_account();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        JObject current_data = new JObject();

        string
            employee_id = string.Empty,
            user_name = string.Empty,
            password = string.Empty,
            confirm_password = string.Empty,
            query = string.Empty;

        int
            user_access_level = 0,
            count = 0;

        try
        {
            // Parsing data from JSON
            current_data = JObject.Parse(current);
            // Storing data for later use.
            employee_id = current_data["employee_code"].ToString();
            user_name = current_data["username"].ToString();
            password = current_data["password"].ToString();
            confirm_password = current_data["confirm_password"].ToString();
            user_access_level = Convert.ToInt32(current_data["access_level"]);

            // Query for setting user access level
            query = "update EmployeeMaster set ";

            switch (user_access_level)
            {
                case 0:
                    query += " ismanager = 0, IsHr = 0 ";
                    break;
                case 1:
                    query += " ismanager = 1, IsHr = 0 ";
                    break;
                case 2:
                    query += " ismanager = 0, IsHr = 0 ";
                    break;
                case 3:
                    query += " ismanager = 0, IsHr = 1 ";
                    break;
            }

            query += " where emp_code='" + employee_id + "' ";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            if (previous_user_id != user_name)
            {
                query = "select count(*) from login where UserName = '" + user_name + "'  and status = 1";
                count = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "User name has been taken. Please try again with a different user name.";
                }
                else
                {
                    page_object.InsertUpdate("U", employee_id, user_name, password, user_access_level, 1);

                    return_object.status = "success";
                    return_object.return_data = "Changes saved successfully!";
                }
            }
            else 
            {
                page_object.InsertUpdate("U", employee_id, user_name, password, user_access_level, 1);

                return_object.status = "success";
                return_object.return_data = "Changes saved successfully!";
            }
            
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_USER");

            return_object.status = "error";
            return_object.return_data = "An error occurred while updating user details. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteUser(string employee_id)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable user_details = new Hashtable();

        try
        {
            user_details.Add("mode", "D");
            user_details.Add("empid", employee_id);
            user_details.Add("status", 0);
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("Sp_UserAccount", user_details);

            return_object.status = "success";
            return_object.return_data = "User deleted successfully!";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DELETE_USER");

            return_object.status = "error";
            return_object.return_data = "An error occurred while deleting the user. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }
}