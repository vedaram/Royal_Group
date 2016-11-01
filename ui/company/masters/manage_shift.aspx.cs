﻿using System;
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
using System.Globalization;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Export.Excel;
using SecurAX.Logger;

public partial class masters_manage_shift : System.Web.UI.Page
{
    const string page = "MANAGE_SHIFT";

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

            message = "An error occurred while loading Manage Shift page. Please try again. If the error persists, please contact Support.";

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
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetShiftDetails(string shift_code)
    {

        masters_manage_shift page_object = new masters_manage_shift();
        DBConnection db_connection = new DBConnection();
        DataTable shifts_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {
                query = "select c.CompanyCode as company_code, c.CompanyName as company_name, s.Shift_Code as shift_code, s.Shift_Desc as shift_desc, s.In_Time as in_time, s.Out_Time as out_time, s.MaxWorkTime as max_overtime, s.ingrace as in_grace, s.brkingrace as break_in_grace, s.brkoutgrace as break_out_grace, s.breakout as break_out, s.breakin as break_in, s.outgrace as out_grace, s.Minovertime as min_overtime, s.StatusNightShift as status_night_shift, s.StatusHalfDay as status_half_day, s.HalfDay as half_day, s.max_overtime as max_shift_end_cut_off_time, s.WeeklyOff1 as weekly_off1, s.WeeklyOff2 as weekly_off2, s.endtime_halfday as end_time_half_day, s.GraceIn as grace_in, s.GraceOut as grace_out, s.Ramadan_InTime as ramadan_in_time, s.Ramadan_OutTime as ramadan_out_time, s.chkifNormalShift as check_if_normal_shift, s.Grace_Bout as grace_break_out, s.Grace_BIn as grace_break_in, s.starttime_halfday as start_time_half_day,branch_code as branch,Employee_Category_Code as category, IsAutoShiftEligible as auto_checked, IsActive as is_active from Shift s, CompanyMaster c where c.CompanyCode = s.CompanyCode and s.Shift_Code = '" + shift_code + "' ";

                shifts_data_table = db_connection.ReturnDataTable(query);

                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(shifts_data_table, Formatting.Indented);
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "GET_SHIFT_DETAILS");

                return_object.status = "error";
                return_object.return_data = "An error occurred while loading Shift data. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject CheckShiftCodeAssigned(string shift_code)
    {
        masters_manage_shift page_object = new masters_manage_shift();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int is_active = 0;

        try
        {
            /*check for Employee is anybody has this shift */
            query = "SELECT count(Emp_Shift_Detail) from EmployeeMaster where Emp_Shift_Detail='" + shift_code + "' and Emp_Status=1 ";
            is_active = db_connection.GetRecordCount(query);
            
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(is_active, Formatting.Indented);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_COMPANY_CATEGORY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading data for OT Eligibility. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    private void UpdateDatabase(JObject data, string mode)
    {

        DBConnection db_connection = new DBConnection();
        Hashtable shift_data = new Hashtable();
        string company_code = data["company_code"].ToString();
        string shift_code = data["shift_code"].ToString();
        string shift_name = data["shift_desc"].ToString();
        int check_normal_shift = Convert.ToInt32(data["check_if_normal_shift"]);
        DateTime start_time = new DateTime();
        string break_out = data["break_out"].ToString();
        string break_in = data["break_in"].ToString();
        DateTime end_time = new DateTime();
        string normal_shift_grace_in = data["in_grace"].ToString();
        string normal_shift_grace_break_out = data["break_out_grace"].ToString();
        string normal_shift_grace_break_in = data["break_in_grace"].ToString();
        string normal_shift_grace_out = data["out_grace"].ToString();
        string ramdan_start_time = data["ramadan_in_time"].ToString();
        string ramdan_end_time = data["ramadan_out_time"].ToString();
        int check_night_shift = Convert.ToInt32(data["status_night_shift"]);
        string night_shift = data["max_shift_end_cut_off_time"].ToString();
        string night_shift_grace_in = data["grace_in"].ToString();
        string night_shift_grace_out = data["grace_out"].ToString();
        string night_shift_grace_break_in = data["grace_break_in"].ToString();
        string night_shift_grace_break_out = data["grace_break_out"].ToString();
        int check_overtime = Convert.ToInt32(data["overtime"]);
        string min_overtime = data["min_overtime"].ToString();
        string max_overtime = data["max_overtime"].ToString();
        int check_half_day = Convert.ToInt32(data["status_half_day"]);
        string half_day = data["half_day"].ToString();
        string half_day_start = data["start_time_half_day"].ToString();
        string half_day_end = data["end_time_half_day"].ToString();
        string week_off_1 = data["weekly_off1"].ToString();
        string week_off_2 = data["weekly_off2"].ToString();
        string Branch_Code = data["branch"].ToString();
        string Employee_Category_Code = data["category"].ToString();
        int IsAutoShiftEligible = Convert.ToInt32(data["auto_checked"].ToString());
        int IsActive = Convert.ToInt32(data["is_active"].ToString());


        // The mode of the DB operation to be performed.
        // I = Insert
        // U = Update
        shift_data.Add("Mode", mode);
        // Basic data regarding the shift and settings.
        shift_data.Add("CompanyCode", company_code);
        shift_data.Add("ShiftCode ", shift_code.ToUpper());
        shift_data.Add("ShiftName", shift_name);

        DateTime.TryParse(data["in_time"].ToString(), out start_time);
        shift_data.Add("StartTime", start_time);
        DateTime.TryParse(data["out_time"].ToString(), out end_time);
        shift_data.Add("EndTime", end_time);

        shift_data.Add("HalfDayendtime", half_day_end == string.Empty ? Convert.ToDateTime(Convert.ToDateTime(DateTime.Today.ToShortDateString())) : Convert.ToDateTime(half_day_end));

        if (Convert.ToInt32(data["status_night_shift"]) == 0)
            shift_data.Add("MaxOverTime", "");
        else
            shift_data.Add("MaxOverTime", night_shift);

        if (half_day == "select")
            half_day = null;
        shift_data.Add("HalfDay", half_day);

        if (Convert.ToInt32(data["overtime"]) == 0)
        {
            shift_data.Add("MaxWorkTime", "");
            shift_data.Add("minovertime", "");
        }
        else
        {
            shift_data.Add("MaxWorkTime", max_overtime);
            shift_data.Add("minovertime", min_overtime);
        }

        // Processing Weekly Off data and settings.
        if (week_off_1 == "select")
            week_off_1 = null;
        shift_data.Add("WeakOff1", week_off_1);

        if (week_off_2 == "select")
            week_off_2 = null;
        shift_data.Add("WeakOff2", week_off_2);

        shift_data.Add("breakout", break_out);
        shift_data.Add("breakin", break_in);

        shift_data.Add("sgrace", normal_shift_grace_in);
        shift_data.Add("boutgrace", normal_shift_grace_break_out);
        shift_data.Add("bingrace", normal_shift_grace_break_in);
        shift_data.Add("egrace", normal_shift_grace_out);

        shift_data.Add("InGrace", night_shift_grace_in == string.Empty ? "0" : night_shift_grace_in);
        shift_data.Add("OutGrace", night_shift_grace_out == string.Empty ? "0" : night_shift_grace_out);
        shift_data.Add("Bin_Grace", night_shift_grace_break_in == string.Empty ? "0" : night_shift_grace_break_in);
        shift_data.Add("Bout_Grace", night_shift_grace_break_out == string.Empty ? "0" : night_shift_grace_break_out);

        DateTime ramdan_start = Convert.ToDateTime(ramdan_start_time == string.Empty ? Convert.ToDateTime(Convert.ToDateTime(DateTime.Today.ToShortDateString())) : Convert.ToDateTime(ramdan_start_time));
        shift_data.Add("Ramadan_StartTime", ramdan_start);
        DateTime ramdan_end = Convert.ToDateTime(ramdan_end_time == string.Empty ? Convert.ToDateTime(Convert.ToDateTime(DateTime.Today.ToShortDateString())) : Convert.ToDateTime(ramdan_end_time));
        shift_data.Add("Ramadan_EndTime", ramdan_end);
        shift_data.Add("ifNormalShift", check_normal_shift.ToString());

        //for autoshift
        shift_data.Add("Branch_Code", Branch_Code);
        shift_data.Add("Employee_Category_Code", Employee_Category_Code);
        shift_data.Add("IsAutoShiftEligible", IsAutoShiftEligible);
        shift_data.Add("IsActive", IsActive);


        shift_data.Add("HalfDaystarttime", half_day_start == string.Empty ? Convert.ToDateTime(Convert.ToDateTime(DateTime.Today.ToShortDateString())) : Convert.ToDateTime(half_day_start));

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("General", shift_data);
    }

    private int CheckShiftName(string shift_name)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "Select Count(*) from Shift where Shift_Desc ='" + shift_name + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckShiftCode(string shift_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "Select Count(*) from Shift where Shift_Code ='" + shift_code + "' ";
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
    public static ReturnObject AddShift(string current)
    {

        masters_manage_shift page_object = new masters_manage_shift();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string shift_code = string.Empty;
        string shift_name = string.Empty;
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
                shift_code = current_data["shift_code"].ToString();
                shift_name = current_data["shift_desc"].ToString();

                count = page_object.CheckShiftCode(shift_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Shift Code has been taken. Please try again with a different Code.";

                    return return_object;
                }

                count = page_object.CheckShiftName(shift_name);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Shift Name has been taken. Please try again with a different Name.";

                    return return_object;
                }

                page_object.UpdateDatabase(current_data, "I");

                return_object.status = "success";
                return_object.return_data = "Shift added successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "ADD_SHIFT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving Shift details. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject EditShift(string current)
    {

        masters_manage_shift page_object = new masters_manage_shift();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string shift_name = string.Empty;
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
                shift_name = current_data["shift_desc"].ToString();

                //JObject previous_data = JObject.Parse(previous);
                //original_shift_name   = previous_data["shift_desc"].ToString();

                // if (original_shift_name != shift_name) {
                //     count = page_object.CheckShiftName(shift_name);
                //     if (count > 0) {
                //         return_object.status      = "error";
                //         return_object.return_data = "Shift Name has been taken. Please try again with a different Name.";

                //         return return_object;
                //     }
                // }

                page_object.UpdateDatabase(current_data, "U");

                return_object.status = "success";
                return_object.return_data = "Shift edited successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "EDIT_SHIFT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving Shift details. Please try again. If the error persists, please contact Support.";

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }

    protected DataTable GetBranchData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        DataTable branch_data = new DataTable();
        string query = string.Empty;
        int access_level = Convert.ToInt32(Session["access"]);

        if (access_level == 3 || access_level == 1)
        {
            query = "Select tb.BranchCode as branch_code, b.BranchName as branch_name From TbManagerHrBranchMapping tb, BranchMaster b Where b.BranchCode = tb.BranchCode and tb.ManagerID = '" + Session["employee_id"].ToString() + "' and b.CompanyCode = '" + company_code + "' order by b.BranchName";
        }
        else
        {
            query = "select distinct branchcode as branch_code, branchname as branch_name from branchmaster  where companycode = '" + company_code + "' order by branchname";
        }

        branch_data = db_connection.ReturnDataTable(query);

        return branch_data;
    }

    [WebMethod]
    public static ReturnObject GetCompanyCategory(string company_code)
    {

        masters_manage_shift page_object = new masters_manage_shift();
        DBConnection db_connection = new DBConnection();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                /* getting employee category data and adding it to Data set to be returned */
                query = "select empcategorycode as employee_category_code, empcategoryname as employee_category_name from employeecategorymaster where companycode = '" + company_code + "'  order by empcategoryname";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "employee_category";
                return_data_set.Tables.Add(temp_data_table);

                /* get branch data and add it to the data set to be returned. */
                temp_data_table = page_object.GetBranchData(company_code);
                temp_data_table.TableName = "branch";
                return_data_set.Tables.Add(temp_data_table);


                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "GET_COMPANY_CATEGORY");

                return_object.status = "error";
                return_object.return_data = "An error occurred while loading data for OT Eligibility. Please try again. If the error persists, please contact Support.";

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }



}