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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;
using System.Data.SqlClient;

public partial class masters_holiday_group : System.Web.UI.Page
{
    const string page = "HOLIDAY_GROUP_MASTER";

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

            message = "An error occurred while loading Holiday Group Master page. Please try again. If the error persists, please contact Support.";

            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    protected string GetFilterQuery(string filters, string query)
    {

        JObject filters_data = JObject.Parse(filters);
        string company_code = filters_data["filter_company_code"].ToString();
        string keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        if (company_code != "select")
            query += " and c.companycode='" + company_code + "'";

        switch (filter_by)
        {

            case 1:
                query += " and h.holgrpname like '%" + keyword + "%'";
                break;

            case 2:
                query += " and h.holgrpcode='" + keyword + "'";
                break;
        };

        return query;
    }

    private DataTable SetColumnNames(DataTable data_table)
    {
        data_table.Columns["CompanyCode"].ColumnName = "company_code";
        data_table.Columns["CompanyName"].ColumnName = "company_name";
        data_table.Columns["holgrpcode"].ColumnName = "holiday_group_code";
        data_table.Columns["holgrpname"].ColumnName = "holiday_group_name";
        data_table.Columns["maxdays"].ColumnName = "max_days";

        return data_table;
    }

    [WebMethod]
    public static ReturnObject GetHolidayGroupData(int page_number, bool is_filter, string filters)
    {
        masters_holiday_group page_object = new masters_holiday_group();
        DBConnection db_connection = new DBConnection();
        DataTable holidayGroup_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;
        string query = string.Empty;
        string employee_id, company_code = string.Empty;

        try
        {

            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            // if employee is logged in then showing only that employee company  data (  done for royal group client first then implemnted in standard as well )
            if (employee_id != "")
            {
                query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select CompanyName, CompanyCode, holgrpcode, holgrpname, maxdays FROM (select c.CompanyName, h.CompanyCode, h.holgrpcode, h.holgrpname, h.maxdays, ROW_NUMBER() OVER (ORDER BY h.holgrpcode) as row from HolidayGroup h, CompanyMaster c where c.CompanyCode = h.companycode and  c.CompanyCode='" + company_code + "' ";
            }
            else
            {
                query = "select CompanyName, CompanyCode, holgrpcode, holgrpname, maxdays FROM (select c.CompanyName, h.CompanyCode, h.holgrpcode, h.holgrpname, h.maxdays, ROW_NUMBER() OVER (ORDER BY h.holgrpcode) as row from HolidayGroup h, CompanyMaster c where c.CompanyCode = h.companycode";
            }

            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            query += " ) a where row > " + start_row + " and row < " + number_of_record;

            holidayGroup_data_table = db_connection.ReturnDataTable(query);

            holidayGroup_data_table = page_object.SetColumnNames(holidayGroup_data_table);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(holidayGroup_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_HOLIDAY_GROUPS_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Holiday Groups. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
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

    private void UpdateDatabase(string holiday_group_code, string holiday_group_name, string company_code, string original_holiday_group_code, string mode, string max_days)
    {
        Hashtable holiday_group_data = new Hashtable();
        DBConnection db_connection = new DBConnection();

        holiday_group_data.Add("Mode", mode);
        holiday_group_data.Add("holgrpcode", holiday_group_code);
        holiday_group_data.Add("holgrpname", holiday_group_name);
        holiday_group_data.Add("companycode", company_code);
        holiday_group_data.Add("origholgrpcode", original_holiday_group_code);
        holiday_group_data.Add("maxdays", max_days);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateHolidayGroup", holiday_group_data);
    }

    private int CheckHolidayGroupCode(string holiday_group_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from HolidayGroup where holgrpcode ='" + holiday_group_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckHolidayGroupName(string holiday_group_name, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from HolidayGroup where holgrpname ='" + holiday_group_name + "' and CompanyCode ='" + company_code + "'";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    [WebMethod]
    public static ReturnObject AddHolidayGroup(string current)
    {

        masters_holiday_group page_object = new masters_holiday_group();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string original_holiday_group_code = string.Empty;
        string holiday_group_code = string.Empty;
        string holiday_group_name = string.Empty;
        string company_code = string.Empty;
        string message = string.Empty;
        string max_days = string.Empty;
        int count = 0;

        try
        {

            JObject current_data = JObject.Parse(current);
            original_holiday_group_code = "hol";
            holiday_group_code = current_data["holiday_group_code"].ToString();
            holiday_group_name = current_data["holiday_group_name"].ToString();
            company_code = current_data["company_code"].ToString();
            max_days = current_data["max_days"].ToString();

            count = page_object.CheckHolidayGroupCode(holiday_group_code);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Holiday Group Code has been taken. Please try again with a different Code.";

                return return_object;
            }

            count = page_object.CheckHolidayGroupName(holiday_group_name, company_code);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Holiday Group Name has been taken. Please try again with a different Name.";

                return return_object;
            }

            page_object.UpdateDatabase(holiday_group_code, holiday_group_name, company_code, "hol", "I", max_days);

            return_object.status = "success";
            return_object.return_data = "Holiday Group added successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "ADD_HOLIDAY_GROUP");

            return_object.status = "error";
            return_object.return_data = "An error occurred while adding a new Holiday Group. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject EditHolidayGroup(string current, string previous)
    {

        masters_holiday_group page_object = new masters_holiday_group();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        string original_holiday_group_code = string.Empty;
        string original_holiday_group_name = string.Empty;
        string holiday_group_code = string.Empty;
        string holiday_group_name = string.Empty;
        string company_code = string.Empty;
        string max_days = string.Empty;
        int count = 0;

        try
        {

            JObject current_data = JObject.Parse(current);
            holiday_group_code = current_data["holiday_group_code"].ToString();
            holiday_group_name = current_data["holiday_group_name"].ToString();
            company_code = current_data["company_code"].ToString();
            max_days = current_data["max_days"].ToString();
            original_holiday_group_code = holiday_group_code;

            query = "select count(holname) As Count  from HolidayMaster Where holgrpcode = '" + holiday_group_code + "' and  holtype = 'Restricted' and holyear = year(getdate())";
            count = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            if (Convert.ToInt32(max_days) < count)
            {
                return_object.status = "error";
                return_object.return_data = "Please check restricted Holiday count in the Holiday List";
            }
            else
            {
                JObject previous_data = JObject.Parse(previous);
                original_holiday_group_name = previous_data["holiday_group_name"].ToString();

                if (original_holiday_group_name != holiday_group_name)
                {

                    count = page_object.CheckHolidayGroupName(holiday_group_name, company_code);
                    if (count > 0)
                    {
                        return_object.status = "error";
                        return_object.return_data = "Holiday Group Name has been taken. Please try again with a different Name.";

                        return return_object;
                    }
                }

                page_object.UpdateDatabase(holiday_group_code, holiday_group_name, company_code, original_holiday_group_code, "U", max_days);

                return_object.status = "success";
                return_object.return_data = "Holiday Group edited successfully!";
            }
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "EDIT_HOLIDAY_GROUP");

            return_object.status = "error";
            return_object.return_data = "An error occurred while editing Holiday Group details. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteHolidayGroup(string current)
    {

        masters_holiday_group page_object = new masters_holiday_group();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string company_code = string.Empty;
        string holiday_group_code = string.Empty;
        string holiday_group_name = string.Empty;
        string query = string.Empty;

        try
        {
            JObject current_data = JObject.Parse(current);
            holiday_group_code = current_data["holiday_group_code"].ToString();
            company_code = current_data["company_code"].ToString();

            query = "select count(*) from BranchMaster where HolidayCode='" + holiday_group_code + "'";
            if (db_connection.RecordExist(query))
            {
                return_object.status = "error";
                return_object.return_data = "This Holiday Group has been mapped to a Branch. Please delete or reassign the Branch.";
            }
            else if (db_connection.GetRecordCount("select count(*) from HolidayMaster where holgrpcode = '" + holiday_group_code + "' ") > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Holidays have been assigned to this Holiday Group. Please delete or unassign the holidays and try again.";
            }
            else
            {
                page_object.UpdateDatabase("", "", company_code, holiday_group_code, "D", "");

                return_object.status = "success";
                return_object.return_data = "Holiday Group deleted successfully!";
            }

        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "DELETE_HOLIDAY_GROUP");

            return_object.status = "error";
            return_object.return_data = "An error occurred while deleting the Holiday Group. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

}