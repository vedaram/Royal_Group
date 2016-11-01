﻿using System;
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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class configuration_shift_setting : System.Web.UI.Page
{
    const string page = "SHIFT_SETTINGS";

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

            message = "An error occurred while loading Shift Settings. Please try again. If the error persists, please contact Support.";

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
            return_object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetShiftSetting(string company_code)
    {

        DBConnection db_connection = new DBConnection();
        DataTable shift_setting_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;

        try
        {

            query = "select actual as break_type, isaso as is_auto_shift, isramadan as is_ramadan, convert (varchar, ramadanstdt, 103) as ramadan_from_date, convert (varchar, ramadantodt, 103) as ramadan_to_date, convert(varchar(5),breakhrs,108) as break_hours, BreakDeductionRequired as break_deduction_required, Convert(varchar(5),MinWorkHrsforDeduction,108) as min_work_hours_for_deduction, TotalDeductionTime as break_deduction, Convert(varchar(5), MINWORKHRS, 108) as min_work_hours, Convert(varchar(5),MAXWORKHRS,108) as max_work_hours, FromDay as from_day, TOday as to_day, Blokday as block_day, MaxHolidayCount as max_holiday_count , convert(varchar(5),WeeklyOffCutoffTime , 108) as cutoff_time from shiftsetting where companycode = '" + company_code + "' ";
            shift_setting_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(shift_setting_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_SHIFT_SETTINGS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Shift Settings. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject SaveShiftSetting(string current)
    {

        DBConnection db_connection = new DBConnection();
        Hashtable shift_setting_data = new Hashtable();
        ReturnObject return_object = new ReturnObject();
        string from_day, to_day, block_day;

        try
        {

            JObject current_data = JObject.Parse(current);

            shift_setting_data.Add("mode", current_data["mode"].ToString());
            shift_setting_data.Add("Companycode", current_data["company"].ToString());

            // BREAK DETAILS
            shift_setting_data.Add("actual", current_data["break_type"].ToString());
            if (current_data["break_hours"].ToString() != "")
                shift_setting_data.Add("break", Convert.ToDateTime(current_data["break_hours"]));
            else
                shift_setting_data.Add("break", Convert.ToDateTime("00:00"));

            // AUTO SHIFT
            shift_setting_data.Add("isaso", current_data["is_auto_shift"].ToString());

            // RAMADAN
            shift_setting_data.Add("isramadan", current_data["is_ramadan"].ToString());
            if (current_data["is_ramadan"].ToString() == "1")
            {
                shift_setting_data.Add("ramadanstdt", DateTime.ParseExact(current_data["ramadan_from_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture));
                shift_setting_data.Add("ramadantodt", DateTime.ParseExact(current_data["ramadan_to_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture));
            }

            // WORK HOURS FOR PAYROLL
            if (current_data["min_work_hours"].ToString() != "")
                shift_setting_data.Add("MINWORKHRS1", Convert.ToDateTime(current_data["min_work_hours"]));
            else
                shift_setting_data.Add("MINWORKHRS1", Convert.ToDateTime("00:00"));

            if (current_data["max_work_hours"].ToString() != "")
                shift_setting_data.Add("MAXWORKHRS1", Convert.ToDateTime(current_data["max_work_hours"]));
            else
                shift_setting_data.Add("MAXWORKHRS1", Convert.ToDateTime("00:00"));

            // HOLIDAY COUNT
            shift_setting_data.Add("MaxHolidayCount", Convert.ToInt32(current_data["max_holiday_count"]));

            // BREAK DEDUCTION
            shift_setting_data.Add("BrkDedRqd", Convert.ToInt32(current_data["break_deduction_required"]));

            if (!string.IsNullOrEmpty(current_data["min_work_hours_for_deduction"].ToString()))
                shift_setting_data.Add("MinWrkHrs", current_data["min_work_hours_for_deduction"].ToString());
            else
                shift_setting_data.Add("MinWrkHrs", "00:00");

            if (!string.IsNullOrEmpty(current_data["break_deduction"].ToString()))
                shift_setting_data.Add("TotDedTime", Convert.ToInt32(current_data["break_deduction"]));
            else
                shift_setting_data.Add("TotDedTime", 0);

            // PAYROLL
            from_day = current_data["from_day"].ToString();
            to_day = current_data["to_day"].ToString();
            block_day = current_data["block_day"].ToString();

            if (!string.IsNullOrEmpty(from_day))
                shift_setting_data.Add("FromDay", Convert.ToInt32(from_day));
            else
                shift_setting_data.Add("FromDay", 0);

            if (!string.IsNullOrEmpty(to_day))
                shift_setting_data.Add("Today", Convert.ToInt32(to_day));
            else
                shift_setting_data.Add("Today", 0);

            if (!string.IsNullOrEmpty(block_day))
                shift_setting_data.Add("Blokday", Convert.ToInt32(block_day));
            else
                shift_setting_data.Add("Blokday", 0);

            #region Created for weeklyoff cutoff time  
            if (!string.IsNullOrEmpty(current_data["cutoff_time"].ToString()))
                shift_setting_data.Add("WeeklyOffCutoffTime", current_data["cutoff_time"]); 
            #endregion

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateShiftSetting", shift_setting_data);

            return_object.status = "success";
            return_object.return_data = "Shift Settings saved successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "SAVE_SHIFT_SETTINGS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving Shift Settings. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetRamadanHistoryData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataSet return_data = new DataSet();
        DataTable ramadan_history_data = new DataTable();
        string query = string.Empty;

        try
        {
            // get all the RamadanHistory 
            query = "select companycode as company_code, Year as 'year', FromDate as from_date, ToDate as 'to_date'  from RamadanHistory where CompanyCode='" + company_code + "'";
            ramadan_history_data = db_connection.ReturnDataTable(query);
            //ramadan_history_data.TableName = "selected_holidays";
            return_data.Tables.Add(ramadan_history_data);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_RAMADAN_HISTORY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Ramadan History data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }
}