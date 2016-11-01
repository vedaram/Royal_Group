using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Logger;
using SecurAX.Export.Excel;

public partial class masters_leave : System.Web.UI.Page
{
    const string page = "LEAVE_MASTER";

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

            message = "An error occurred while loading Leave Master. Please try again. If the error persists, please contact Support.";

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
        string company_code = filters_data["filter_company_code"].ToString();
        string keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        if (company_code != "select")
        {
            query += " and c.companycode = '" + company_code + "' ";
        }

        switch (filter_by)
        {

            case 1:
                query += " and l.leavename like '%" + keyword + "%'";
                break;
            case 2:
                query += " and l.leavecode ='" + keyword + "'";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetLeaveData(int page_number, bool is_filter, string filters)
    {

        masters_leave page_object = new masters_leave();
        DBConnection db_connection = new DBConnection();
        DataTable leaves_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string employee_id, query, company_code = string.Empty;
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;

        try
        {

            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            // if employee is logged in then showing only that employee company  data (  done for royal group client first then implemnted in standard as well )
            if (employee_id != "")
            {
                query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select leavecode as leave_code, leavename as leave_name, CompanyName as company_name, CompanyCode as company_code, EmpCategoryName as employee_category_name, EmpCategoryCode as employee_category_code, MaxLeave as max_leave, MaxLeaveCarryForward as max_leave_carry_forward, woflag as week_off_flag , status as leave_status, LeaveType as leave_type from ( select l.leavecode, l.leavename, c.CompanyName, c.CompanyCode, ec.EmpCategoryName, ec.EmpCategoryCode, l.MaxLeave, l.MaxLeaveCarryForward, l.woflag,l.status, l.LeaveType, ROW_NUMBER() OVER (ORDER BY l.leavecode) as row from LeaveMaster l, CompanyMaster c, EmployeeCategoryMaster ec where l.EmployeeCategoryCode = ec.EmpCategoryCode and l.CompanyCode = c.CompanyCode and  c.CompanyCode='" + company_code + "' ";
            }
            else
            {
                query = "select leavecode as leave_code, leavename as leave_name, CompanyName as company_name, CompanyCode as company_code, EmpCategoryName as employee_category_name, EmpCategoryCode as employee_category_code, MaxLeave as max_leave, MaxLeaveCarryForward as max_leave_carry_forward, woflag as week_off_flag  , status as leave_status, LeaveType as leave_type  from ( select l.leavecode, l.leavename, c.CompanyName, c.CompanyCode, ec.EmpCategoryName, ec.EmpCategoryCode, l.MaxLeave, l.MaxLeaveCarryForward, l.woflag, l.status, l.LeaveType, ROW_NUMBER() OVER (ORDER BY l.leavecode) as row from LeaveMaster l, CompanyMaster c, EmployeeCategoryMaster ec where l.EmployeeCategoryCode = ec.EmpCategoryCode and l.CompanyCode = c.CompanyCode ";
            }

            if (is_filter) query = page_object.GetFilterQuery(filters, query);

            query += " ) a where row > " + start_row + " and row < " + number_of_record;

            leaves_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leaves_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_LEAVE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leave data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {

        DataTable company_data = new DataTable();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty, employee_id = string.Empty, company_code = string.Empty;

        try
        {

            employee_id = HttpContext.Current.Session["username"].ToString();

            //load company list as per employee
            if (employee_id != "admin")
            {
                query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster where CompanyCode='" + company_code + "' order by CompanyName ";
            }
            else
            {
                query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster";
            }
            company_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(company_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_COMPANY_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company Data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetEmployeeCategoryData(string company_code)
    {

        DBConnection db_connection = new DBConnection();
        DataTable employeeCategory_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;

        try
        {

            query = "select e.EmpCategoryCode as employee_category_code, e.EmpCategoryName as employee_category_name, c.CompanyCode, c.CompanyName, e.Totalhrs, e.includeprocess as process from EmployeeCategoryMaster e, CompanyMaster c where e.companyCode = c.CompanyCode and c.CompanyCode = '" + company_code + "' order by e.EmpCategoryCode";
            employeeCategory_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employeeCategory_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_EMPLOYEE_CATEGORY_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Employee Category data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetLeaveTypeData()
    {

        DBConnection db_connection = new DBConnection();
        DataTable leave_type_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;

        try
        {
            query = "select LeaveTypeName as leave_type from leave_type_table ";
            leave_type_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leave_type_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_LEAVE_TYPE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leave type data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private void UpdateDatabase(string mode, string company_code, string employee_category_code, string leave_code, string leave_name, decimal max_leave, decimal max_leave_carry_forward, int week_off_flag, string leave_status, string leave_type)
    {

        DBConnection db_connection = new DBConnection();
        Hashtable leave_data_hash_table = new Hashtable();
        
        leave_data_hash_table.Add("mode", mode);
        leave_data_hash_table.Add("CompanyCode", company_code);
        leave_data_hash_table.Add("EmployeeCategoryCode", employee_category_code);
        leave_data_hash_table.Add("leavecode", leave_code);
        leave_data_hash_table.Add("leavename", leave_name);
        leave_data_hash_table.Add("Maxleave", max_leave);
        leave_data_hash_table.Add("maxleavecarry", max_leave_carry_forward);
        leave_data_hash_table.Add("woflag", week_off_flag);
        leave_data_hash_table.Add("leavestatus", leave_status);
        leave_data_hash_table.Add("leave_type", leave_type);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("Leave", leave_data_hash_table);
    }

    private int CheckLeaveCode(string leave_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from LeaveMaster where leavecode = '" + leave_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckLeaveName(string leave_name, string employee_category_code, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from LeaveMaster where leavename='" + leave_name + "' and employeecategorycode='" + employee_category_code + "' and companycode='" + company_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    //  Method Created for showing session expired message ...
    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }



    [WebMethod]
    public static ReturnObject AddLeave(string current)
    {

        masters_leave page_object = new masters_leave();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string company_code = string.Empty;
        string employee_category_code = string.Empty;
        string leave_code = string.Empty;
        string leave_name = string.Empty;
        string leave_status = string.Empty, leave_type = string.Empty;
        decimal max_leave = 0;
        decimal max_leave_carry_forward = 0;
        int week_off_flag = 0;
        int count = 0;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                JObject current_data = JObject.Parse(current);
                company_code = current_data["company_code"].ToString();
                employee_category_code = current_data["employee_category_code"].ToString();
                leave_code = current_data["leave_code"].ToString();
                leave_name = current_data["leave_name"].ToString();
                leave_status = current_data["leave_status"].ToString();
                //max_leave = Convert.ToDecimal(current_data["max_leave"]);
                //max_leave_carry_forward = Convert.ToDecimal(current_data["max_leave_carry_forward"]);
                max_leave = 0;
                max_leave_carry_forward = 0;
                week_off_flag = Convert.ToInt32(current_data["week_off_flag"]);
                leave_type = current_data["leave_type"].ToString();

                count = page_object.CheckLeaveCode(leave_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Leave Code has been taken. Please try again with a different Code.";

                    return return_object;
                }

                count = page_object.CheckLeaveName(leave_name, employee_category_code, company_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Leave Name has been taken. Please try again with a different Name.";

                    return return_object;
                }

                page_object.UpdateDatabase("I", company_code, employee_category_code, leave_code, leave_name, max_leave, max_leave_carry_forward, week_off_flag, leave_status, leave_type);

                return_object.status = "success";
                return_object.return_data = "Leave added successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "ADD_LEAVE");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving Leave details. Please try again. If the error persists, please contact Support.";

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject EditLeave(string current, string previous)
    {

        masters_leave page_object = new masters_leave();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string original_leave_name = string.Empty;
        string company_code = string.Empty;
        string employee_category_code = string.Empty;
        string leave_code = string.Empty;
        string leave_name = string.Empty;
        string leave_status = string.Empty, leave_type = string.Empty;
        decimal max_leave = 0;
        decimal max_leave_carry_forward = 0;
        int week_off_flag = 0;
        int count = 0;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                JObject current_data = JObject.Parse(current);
                company_code = current_data["company_code"].ToString();
                employee_category_code = current_data["employee_category_code"].ToString();
                leave_code = current_data["leave_code"].ToString();
                leave_name = current_data["leave_name"].ToString();
                leave_status = current_data["leave_status"].ToString();
                max_leave = Convert.ToDecimal(current_data["max_leave"]);
                max_leave_carry_forward = Convert.ToDecimal(current_data["max_leave_carry_forward"]);
                week_off_flag = Convert.ToInt32(current_data["week_off_flag"]);
                leave_type = current_data["leave_type"].ToString();

                JObject previous_data = JObject.Parse(previous);
                original_leave_name = previous_data["leave_name"].ToString();

                if (original_leave_name != leave_name)
                {
                    count = page_object.CheckLeaveName(leave_name, employee_category_code, company_code);
                    if (count > 0)
                    {
                        return_object.status = "error";
                        return_object.return_data = "Leave Name has been taken. Please try again with a different Name.";

                        return return_object;
                    }
                }

                page_object.UpdateDatabase("U", company_code, employee_category_code, leave_code, leave_name, max_leave, max_leave_carry_forward, week_off_flag, leave_status, leave_type);

                return_object.status = "success";
                return_object.return_data = "Leave edited successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "EDIT_LEAVE");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving Leave details. Please try again. If the error persists, please contact Support.";

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteLeave(string current)
    {
        masters_leave page_object = new masters_leave();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable leave_data_hash_table = new Hashtable();
        string company_code = string.Empty;
        string employee_category_code = string.Empty;
        string leave_code = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                JObject current_data = JObject.Parse(current);
                company_code = current_data["company_code"].ToString();
                employee_category_code = current_data["employee_category_code"].ToString();
                leave_code = current_data["leave_code"].ToString();

                leave_data_hash_table.Add("mode", "D");
                leave_data_hash_table.Add("CompanyCode", company_code);
                leave_data_hash_table.Add("EmployeeCategoryCode", employee_category_code);
                leave_data_hash_table.Add("leavecode", leave_code);
                

                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("Leave", leave_data_hash_table);

                return_object.status = "success";
                return_object.return_data = "Leave deleted successfully!";
            }
            catch (Exception Ex)
            {

                Logger.LogException(Ex, page, "DELETE_LEAVE");

                return_object.status = "error";
                return_object.return_data = "An error occurred while deleting this Leave. Please try again. If the error persists, please contact Support.";

                throw;
            }
        }

        return return_object;
    }
}