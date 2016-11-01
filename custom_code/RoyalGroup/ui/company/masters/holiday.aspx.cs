using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Globalization;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class masters_holiday : System.Web.UI.Page
{
    const string page = "HOLIDAY_MASTER";

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

            message = "An error occurred while loading Holiday Master page. Please try again. If the error persists, please contact Support.";

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

        JObject filter_data = JObject.Parse(filters);
        string filter_CompanyCode = filter_data["filter_company_code"].ToString();
        string filter_keyword = filter_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filter_data["filter_by"]);

        if (filter_CompanyCode != "select")
        {
            query += " and c.companycode='" + filter_CompanyCode + "'";
        }

        switch (filter_by)
        {

            case 1:
                query += " and hl.holname like '%" + filter_keyword + "%'";
                break;
            case 2:
                query += " and hl.holcode='" + filter_keyword + "'";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetHolidayData(int page_number, bool is_filter, string filters)
    {
        masters_holiday page_object = new masters_holiday();
        DBConnection db_connection = new DBConnection();
        DataTable holiday_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string employee_id, query, company_code = string.Empty;
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;

        try
        {
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            // if employee is logged in then showing only that employee company  data (  done for royal group client first then implemnted in standard as well 
             if (employee_id != "")
             {
                 query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
                 company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                 query = "select holcode as holiday_code, holname as holiday_name, holfrom as holiday_from, holto as holiday_to, holtype as holiday_type, CompanyCode as company_code, CompanyName as company_name FROM (select hl.holcode, hl.holname, convert(varchar,hl.holfrom,103) as holfrom, convert(varchar,hl.holto,103) as holto, hl.holtype, hl.CompanyCode, c.CompanyName, ROW_NUMBER() OVER (ORDER BY hl.holcode) as row from HolidayListDetails hl, CompanyMaster c where c.CompanyCode = hl.companycode and c.CompanyCode='" + company_code + "' ";
             }
             else
             {
                 query = "select holcode as holiday_code, holname as holiday_name, holfrom as holiday_from, holto as holiday_to, holtype as holiday_type, CompanyCode as company_code, CompanyName as company_name FROM (select hl.holcode, hl.holname, convert(varchar,hl.holfrom,103) as holfrom, convert(varchar,hl.holto,103) as holto, hl.holtype, hl.CompanyCode, c.CompanyName, ROW_NUMBER() OVER (ORDER BY hl.holcode) as row from HolidayListDetails hl, CompanyMaster c where c.CompanyCode = hl.companycode ";
             }

            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            query += " ) a where row > " + start_row + " and row < " + number_of_record;

            holiday_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(holiday_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_HOLIDAY_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

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
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GenerateHolidayCode(string company_code)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable holidays = new DataTable();
        string query = string.Empty;
        string last_holiday_added, holiday_value_withcode = string.Empty;
        int temp_holiday = 0;

        try
        {
            query = "select TOP 1 holcode from HolidayListDetails where companycode = '" + company_code + "' ORDER BY holcode DESC";
            holidays = db_connection.ReturnDataTable(query);

            if (holidays.Rows.Count > 0)
            {
                last_holiday_added = holidays.Rows[0]["holcode"].ToString();
                string holiday_values=GetHolidayCode(last_holiday_added);
                string[] holiday_values_array = holiday_values.Split(':');
                holiday_value_withcode = holiday_values_array[0];
                temp_holiday = Convert.ToInt32(holiday_values_array[1]);
                temp_holiday = temp_holiday + 1;
                Debug.WriteLine(temp_holiday);
            }
            else
            {
                last_holiday_added = "H0";
                temp_holiday = 1;
            }

            return_object.status = "success";
            //return_object.return_data = last_holiday_added.Substring(0, (last_holiday_added.Length - Convert.ToString(temp_holiday).Length)) + (temp_holiday);//.ToString("D2");
            if (holiday_value_withcode != "")
            {
                return_object.return_data = holiday_value_withcode + temp_holiday.ToString("D2");
            }
            else
            {
                return_object.return_data = last_holiday_added + temp_holiday.ToString("D2");
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GENERATE_HOLIDAY_CODE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while generating the holiday code. Please try again.";

            throw;
        }


        return return_object;
    }
    private static string GetHolidayCode(string last_holiday_added)
    {
        string finalString = "", stringVal = "" ;
        string HolidayNumber = string.Empty;
        for (int i = last_holiday_added.Length - 1; i >= 0; i--)
        {
            char cuurent = last_holiday_added[i];
            if (Char.IsDigit(cuurent))
            {
                HolidayNumber = cuurent + HolidayNumber;

            }
            else
            {
                stringVal = last_holiday_added.Substring(0,i+1);
                break;
            }

        }
        finalString = stringVal + ":" + HolidayNumber;
        return finalString;

    }

    public void UpdateDatabase(string holiday_code, string holiday_name, string holiday_from, string holiday_to, string company_code, string holiday_group_code, string original_holiday_code, string mode, string holiday_type)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable holiday_data = new Hashtable();

        string
            from_date_time = DateTime.ParseExact(holiday_from, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"),
            to_date_time = DateTime.ParseExact(holiday_to, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

        holiday_data.Clear();
        holiday_data.Add("Mode", mode);
        holiday_data.Add("holcode", holiday_code);
        holiday_data.Add("holname", holiday_name);
        holiday_data.Add("holgrpcode", 0);
        holiday_data.Add("companycode", company_code);
        holiday_data.Add("origholcode", original_holiday_code);
        holiday_data.Add("holfrom", from_date_time);
        holiday_data.Add("holto", to_date_time);
        holiday_data.Add("holtype", holiday_type);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateHolidayMaster", holiday_data);
    }

    private int CheckHolidayName(string holiday_name, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from HolidayListDetails where holname = '" + holiday_name + "' and companycode = '" + company_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckHolidayCode(string holiday_code, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from HolidayListDetails where holcode = '" + holiday_code + "' and CompanyCode = '" + company_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckHolidayDates(string holiday_from, string holiday_to , string company_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        holiday_from = DateTime.ParseExact(holiday_from.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        holiday_to = DateTime.ParseExact(holiday_to.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

        query = "Select count(*) from HolidayListDetails where companycode = '" + company_code + "' and  ";
        query += " ((holfrom>='" + holiday_from + "' and holfrom<='" + holiday_to + "')";
        query += "OR (holfrom<='" + holiday_from + "' and holto>='" + holiday_to + "')";
        query += "OR (holto>='" + holiday_from + "' and holto<='" + holiday_to + "'))";

        count = db_connection.GetRecordCount(query);

        return count;
    }

    [WebMethod]
    public static ReturnObject AddHoliday(string current)
    {

        masters_holiday page_object = new masters_holiday();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        int count = 0;

        string
            query = string.Empty,
            holiday_code = string.Empty,
            holiday_name = string.Empty,
            holiday_from = string.Empty,
            holiday_to = string.Empty,
            company_code = string.Empty,
            holiday_type = string.Empty;
        
        try
        {

            JObject current_data = JObject.Parse(current);
            holiday_code = current_data["holiday_code"].ToString();
            holiday_name = current_data["holiday_name"].ToString();
            holiday_from = current_data["holiday_from"].ToString();
            holiday_to = current_data["holiday_to"].ToString();
            company_code = current_data["company_code"].ToString();
            holiday_type = current_data["holiday_type"].ToString();


            count = page_object.CheckHolidayCode(holiday_code, company_code);
            if (count > 0)
            {

                return_object.status = "error";
                return_object.return_data = "A Holiday with the same Code has been created. Please try again with a different Holiday Code.";

                return return_object;
            }

            count = page_object.CheckHolidayName(holiday_name, company_code);
            if (count > 0)
            {

                return_object.status = "error";
                return_object.return_data = "A Holiday with the same Name has been created. Please try again with a different Holiday Name.";

                return return_object;
            }

            count = page_object.CheckHolidayDates(holiday_from, holiday_to, company_code);
            if (count > 0)
            {

                return_object.status = "error";
                return_object.return_data = "The selected From and To Dates have been mapped to another holiday. Please try again with different dates.";

                return return_object;
            }

            page_object.UpdateDatabase(holiday_code, holiday_name, holiday_from, holiday_to, company_code, "0", "hol", "I", holiday_type);

            return_object.status = "success";
            return_object.return_data = "Holiday added successfully!";
        }
        catch (Exception Ex)
        {

            Logger.LogException(Ex, page, "ADD_HOLIDAY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject EditHoliday(string current, string previous)
    {

        masters_holiday page_object = new masters_holiday();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        string holiday_code = string.Empty;
        string holiday_name = string.Empty;
        string holiday_from = string.Empty;
        string holiday_to = string.Empty;
        string company_code = string.Empty;
        string holiday_type = string.Empty;
        string original_holiday_name = string.Empty;
        string original_holiday_from = string.Empty;
        string original_holiday_to = string.Empty;
        int count = 0;

        try
        {

            JObject current_data = JObject.Parse(current);
            JObject original_data = JObject.Parse(previous);

            holiday_code = current_data["holiday_code"].ToString();
            holiday_name = current_data["holiday_name"].ToString();
            holiday_from = current_data["holiday_from"].ToString();
            holiday_to = current_data["holiday_to"].ToString();
            company_code = current_data["company_code"].ToString();
            holiday_type = current_data["holiday_type"].ToString();

            original_holiday_name = original_data["holiday_name"].ToString();
            original_holiday_from = original_data["holiday_from"].ToString();
            original_holiday_to = original_data["holiday_to"].ToString();

            if (original_holiday_name != holiday_name)
            {
                count = page_object.CheckHolidayName(holiday_name, company_code);
                if (count > 0)
                {

                    return_object.status = "error";
                    return_object.return_data = "A Holiday with the same Name has been created. Please try again with a different Holiday Name.";

                    return return_object;
                }
            }
            Debug.Write(original_holiday_from + " " + holiday_from);
            if (original_holiday_from != holiday_from || original_holiday_to != holiday_to)
            {
                count = page_object.CheckHolidayDates(holiday_from, holiday_to, company_code);
                if (count > 0)
                {

                    return_object.status = "error";
                    return_object.return_data = "The selected From and To Dates have been mapped to another Holiday. Please try again with different Dates";

                    return return_object;
                }
            }

            page_object.UpdateDatabase(holiday_code, holiday_name, holiday_from, holiday_to, company_code, "0", holiday_code, "U", holiday_type);

            return_object.status = "success";
            return_object.return_data = "Holiday edited successfully!";
        }
        catch (Exception Ex)
        {

            Logger.LogException(Ex, page, "EDIT_HOLIDAY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteHoliday(string current)
    {

        masters_holiday page_object = new masters_holiday();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count =  0;

        string
            company_code = string.Empty,
            holiday_group_code = string.Empty,
            holiday_code = string.Empty,
            holiday_from = string.Empty,
            holiday_to = string.Empty,
            holiday_type = string.Empty;

        try
        {

            JObject current_data = JObject.Parse(current);
            holiday_code = current_data["holiday_code"].ToString();
            holiday_from = current_data["holiday_from"].ToString();
            holiday_to = current_data["holiday_to"].ToString();
            company_code = current_data["company_code"].ToString();
            holiday_type = current_data["holiday_type"].ToString();

            query = "select count(*) from HolidayListDetails where holcode in(select distinct holcode from HolidayMaster where holcode = '" + holiday_code + "')";
            count = db_connection.GetRecordCount(query);
            if (count > 0) 
            {
                return_object.status = "error";
                return_object.return_data = "Holidays have been mapped to Holiday Group List. Please unassign the holidays from the Holiday Group List and try again.";
            }
            else 
            {
                page_object.UpdateDatabase(holiday_code, null, holiday_from, holiday_to, company_code, "0", holiday_code, "D", holiday_type);

                return_object.status = "success";
                return_object.return_data = "Holiday deleted successfully!";
            }

        }
        catch (Exception Ex)
        {
            Logger.LogException(Ex, page, "DELETE_HOLIDAY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }
}