﻿using System;
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

public partial class masters_holiday_list : System.Web.UI.Page
{
    const string page = "HOLIDAY_LIST_MASTER";

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

            message = "An error occurred while loading Holiday List Master page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetCompanyData()
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable company_data = new DataTable();
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
            return_object.return_data = "An error occurred while loading Company Data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private string GetBaseQuery()
    {
        string query = "select holgrpcode as holiday_group_code, holgrpname as holiday_group_name, companycode as company_code from (select DISTINCT hg.holgrpcode, hg.holgrpname, hg.companycode, ROW_NUMBER() OVER (ORDER BY hg.holgrpcode) as row from HolidayGroup hg, Holidaymaster Hm where Hg.holgrpcode=Hm.holgrpcode and hg.holgrpcode in (select holgrpcode from HolidayMaster) ";

        return query;
    }

    private string GetFilterQuery(string filters, string query)
    {
        JObject filters_data = JObject.Parse(filters);
        string company_code = filters_data["filter_company_code"].ToString();
        string filter_keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        if (company_code != "select")
            query += " and hg.companycode = '" + company_code + "' ";

        switch (filter_by)
        {
            case 1:
                query += " and hg.holgrpname like '%" + filter_keyword + "%' ";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetHolidayListData(int page_number, bool is_filter, string filters)
    {
        masters_holiday_list page_object = new masters_holiday_list();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable holiday_list_data = new DataTable();
        string query = string.Empty;

        int
            start_row = (page_number - 1) * 30,
            number_of_rows = page_number * 30 + 1;

        try
        {
            query = page_object.GetBaseQuery();

            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            query += " group by Hg.holgrpcode, hg.holgrpname, hg.companycode) a where row > " + start_row + " and row < " + number_of_rows;

            holiday_list_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(holiday_list_data, Formatting.Indented);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_HOLIDAY_LIST_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Holiday List data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetHolidayGroupData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable holiday_groups_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select holgrpcode as holiday_group_code, holgrpname as holiday_group_name from HolidayGroup where companycode = '" + company_code + "' order by holgrpname";
            holiday_groups_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(holiday_groups_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_HOLIDAY_GROUPS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Holiday Groups data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetHolidayData(string year, string holiday_group_code, bool get_selected, string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataSet return_data = new DataSet();
        DataTable holiday_data = new DataTable();
        string query = string.Empty;

        try
        {
            if (year == "")
                year = DateTime.Now.Year.ToString();

            // Get all holidays for the selected year
            if (get_selected)
            {
                query = "select holname as holiday_name, holcode as holiday_code from HolidayListDetails Where Year(holfrom)='" + year + "' and companycode = '" + company_code + "' ";
                holiday_data = db_connection.ReturnDataTable(query);
                holiday_data.TableName = "all_holidays";
                return_data.Tables.Add(holiday_data);
            }

            // get all the holidays assigned to this list
            query = "select * from HolidayMaster where holgrpcode = '" + holiday_group_code + "' and year(holfrom) = '" + year + "' ";
            holiday_data = db_connection.ReturnDataTable(query);
            holiday_data.TableName = "selected_holidays";
            return_data.Tables.Add(holiday_data);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_HOLIDAY_LIST");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Holiday List data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private int GetRestrictedHolidayCount(string holidays)
    {
        DBConnection db_connection = new DBConnection();
        JArray holiday_data = JArray.Parse(holidays);

        string
            query = string.Empty,
            holiday_code = string.Empty,
            holiday_code_string = string.Empty;

        int
            count_restricted_holidays = 0,
            counter = 0;

        for (counter = 0; counter < holiday_data.Count; counter++)
        {
            holiday_code_string += "'" + holiday_data[counter]["holiday_code"].ToString() + "',";
        }

        holiday_code_string = holiday_code_string.Remove(holiday_code_string.Length - 1, 1);

        query = "select COUNT(*) from HolidayListDetails where holtype = 'Restricted'  and holcode in (" + holiday_code_string + ") ";
        count_restricted_holidays = db_connection.ExecuteQuery_WithReturnValueInteger(query);

        return count_restricted_holidays;
    }

    private void UpdateDatabase(string holiday_code, string holiday_name, string company_code, string holiday_group_code, string original_holiday_code, string mode, int select_holiday_count)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable holiday_data = new Hashtable();

        holiday_data.Add("Mode", mode);
        holiday_data.Add("holcode", holiday_code);
        holiday_data.Add("holname", holiday_name);
        holiday_data.Add("holgrpcode", holiday_group_code);
        holiday_data.Add("companycode", company_code);
        holiday_data.Add("origholcode", original_holiday_code);
        holiday_data.Add("holfrom", null);
        holiday_data.Add("holto", null);
        holiday_data.Add("SelectedHolidayCount", select_holiday_count);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateHolidayGroupMaster", holiday_data);
    }

    private int GetMaximumRestrictedDays(string holiday_group_code)
    {
        DBConnection db_connection = new DBConnection();
        int max_days = 0;
        string query = string.Empty;

        query = "select maxdays from HolidayGroup where holgrpcode='" + holiday_group_code + "' ";
        max_days = db_connection.ExecuteQuery_WithReturnValueInteger(query);

        return max_days;
    }

    [WebMethod]
    public static ReturnObject SaveHolidayList(string current, string holidays, string mode)
    {
        masters_holiday_list page_object = new masters_holiday_list();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        JArray holiday_data = new JArray();
        JObject current_data = new JObject();

        int
            selected_holiday_count = 0,
            count_restricted_holidays = 0,
            max_days = 0,
            counter = 0;

        string
            company_code = string.Empty,
            holiday_group_code = string.Empty,
            holiday_name = string.Empty,
            holiday_code = string.Empty,
            year = string.Empty,
            query = string.Empty;

        try
        {
            current_data = JObject.Parse(current);
            company_code = current_data["company_code"].ToString();
            holiday_group_code = current_data["holiday_group_code"].ToString();
            year = current_data["year"].ToString();

            if (mode == "U")
            {
                query = "delete from Holidaymaster where holgrpcode = '" + holiday_group_code + "' and holyear = '" + year + "' ";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }

            count_restricted_holidays = page_object.GetRestrictedHolidayCount(holidays);
            max_days = page_object.GetMaximumRestrictedDays(holiday_group_code);

            if (count_restricted_holidays <= max_days)
            {
                holiday_data = JArray.Parse(holidays);

                for (counter = 0; counter < holiday_data.Count; counter++)
                {
                    holiday_code = holiday_data[counter]["holiday_code"].ToString();
                    holiday_name = holiday_data[counter]["holiday_name"].ToString();

                    selected_holiday_count += 1;

                    page_object.UpdateDatabase(holiday_code, holiday_name, company_code, holiday_group_code, "hol", mode, selected_holiday_count);
                }

                return_object.status = "success";
                return_object.return_data = "Holiday List saved successfully!";
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = "Additional Restricted days can't be added to this Holiday List";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SAVE_HOLIDAY_LIST");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving new Holiday List. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteHolidayList(string current, string holidays)
    {
        masters_holiday_list page_object = new masters_holiday_list();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        JArray holiday_data = new JArray();
        JObject current_data = new JObject();

        string
            company_code = string.Empty,
            holiday_group_code = string.Empty,
            holiday_name = string.Empty,
            holiday_code = string.Empty,
            year = string.Empty,
            query = string.Empty;

        int
                counter = 0,
                selected_holiday_count = 0,
                count = 0;

        try
        {
            current_data = JObject.Parse(current);
            company_code = current_data["company_code"].ToString();
            holiday_group_code = current_data["holiday_group_code"].ToString();
            year = current_data["year"].ToString();


            query = "select count(*) from BranchMaster where HolidayCode='" + holiday_group_code + "'";
            if (db_connection.RecordExist(query))
            {
                return_object.status = "error";
                return_object.return_data = "This Holiday Group has been mapped to a Branch. Please delete or reassign the Branch.";
            }
            else
            {
                holiday_data = JArray.Parse(holidays);

                for (counter = 0; counter < holiday_data.Count; counter++)
                {
                    holiday_code = holiday_data[counter]["holiday_code"].ToString();
                    holiday_name = holiday_data[counter]["holiday_name"].ToString();

                    selected_holiday_count += 1;

                    page_object.UpdateDatabase(holiday_code, holiday_name, company_code, holiday_group_code, "hal", "D", selected_holiday_count);
                }

                query = "select count(*) from HolidayMaster where holgrpcode = '" + holiday_group_code + "' ";
                count = db_connection.GetRecordCount(query);

                return_object.status = "success";
                return_object.return_data = count.ToString();
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DELETE_HOLIDAY_LIST");

            throw;
        }

        return return_object;
    }
}