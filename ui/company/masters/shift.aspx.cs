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

public partial class masters_shift : System.Web.UI.Page
{
    const string page = "SHIFT_MASTER";

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

            message = "An error occurred while loading Shift Master page. Please try again. If the error persists, please contact Support.";

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

        if (company_code != "select") query += " and c.companycode = '" + company_code + "' ";

        switch (filter_by)
        {

            case 1:
                query += " and s.Shift_Desc like '%" + keyword + "%'";
                break;
            case 2:
                query += " and s.Shift_Code ='" + keyword + "'";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetShiftData(int page_number, bool is_filter, string filters)
    {

        masters_shift page_object = new masters_shift();
        DBConnection db_connection = new DBConnection();
        DataTable shifts_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string employee_id, query, company_code = string.Empty;
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;

        try
        {

            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            // if employee is logged in then showing only that employee company shift  data (  done for royal group client first then implemnted in standard as well )
            if (employee_id != "")
            {
                query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select CompanyCode as company_code, CompanyName as company_name, Shift_Code as shift_code, Shift_Desc as shift_desc, Shift_Hours as shift_hours, IsActive from ( select c.CompanyCode, c.CompanyName, s.Shift_Code, s.Shift_Desc, convert(varchar(5), s.MaxOverTime_General) as 'Shift_Hours', IsActive, ROW_NUMBER() OVER (ORDER BY s.Shift_Code) as row from Shift s, CompanyMaster c where c.CompanyCode = s.CompanyCode and  c.CompanyCode='" + company_code + "' ";
            }
            else
            {
                query = "select CompanyCode as company_code, CompanyName as company_name, Shift_Code as shift_code, Shift_Desc as shift_desc, Shift_Hours as shift_hours, IsActive from ( select c.CompanyCode, c.CompanyName, s.Shift_Code, s.Shift_Desc, convert(varchar(5), s.MaxOverTime_General) as 'Shift_Hours', IsActive, ROW_NUMBER() OVER (ORDER BY s.Shift_Code) as row from Shift s, CompanyMaster c where c.CompanyCode = s.CompanyCode";
            }


            if (is_filter) query = page_object.GetFilterQuery(filters, query);

            //query += " ) a where IsActive=1 and row > " + start_row + " and row < " + number_of_record;
            query += " ) a where row > " + start_row + " and row < " + number_of_record;
            shifts_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(shifts_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_SHIFT_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Shift data. Please try again. If the error persists, please contact Support.";
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
            return_object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
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
    public static ReturnObject DeleteShift(string current)
    {
        masters_shift page_object = new masters_shift();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        JObject current_data = new JObject();
        string company_code = string.Empty;
        string shift_code = string.Empty;
        string query = string.Empty;
        int count = 0;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                current_data = JObject.Parse(current);
                company_code = current_data["company_code"].ToString();
                shift_code = current_data["shift_code"].ToString();

                query = "select count(*) from employeeMaster where Emp_Shift_Detail = '" + shift_code + "' ";
                count = db_connection.GetRecordCount(query);

                if (count > 0)
                {

                    return_object.status = "error";
                    return_object.return_data = "Employees have been mapped to this shift. Please reassign or delete the employees.";
                }
                else
                {
                    query = "delete from Shift where Shift_Code = '" + shift_code + "' and CompanyCode = '" + company_code + "' ";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    return_object.status = "success";
                    return_object.return_data = "Shift deleted successfully!";
                }
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "DELETE_SHIFT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while deleting this Shift. Please try again. If the error persists, please contact Support.";

                throw;
            }
        }

        return return_object;
    }

}