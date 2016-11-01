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
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Export.Excel;
using SecurAX.Logger;

public partial class masters_department : System.Web.UI.Page
{
    const string page = "DEPARTMENT MASTER";

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

            message = "An error occurred while loading Department Master. Please try again. If the error persists, please contact Support.";

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
        DBConnection db_connection = new DBConnection();
        string employee_id, query, company_code = string.Empty;
        employee_id = HttpContext.Current.Session["employee_id"].ToString();
        // if employee is logged in then showing only that employee company  data (  done for royal group client first then implemnted in standard as well )
        if (employee_id != "")
        {
            query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
            company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
            query = "select DeptCode as department_code, DeptName as department_name, CompanyName as company_name, CompanyCode as company_code from ( select d.DeptCode, d.DeptName, c.CompanyName, c.CompanyCode, ROW_NUMBER() OVER (ORDER BY d.DeptCode) as row from DeptMaster d, CompanyMaster c where c.Companycode = d.companycode and  c.CompanyCode='" + company_code + "' ";
        }
        else
        {
            query = "select DeptCode as department_code, DeptName as department_name, CompanyName as company_name, CompanyCode as company_code from ( select d.DeptCode, d.DeptName, c.CompanyName, c.CompanyCode, ROW_NUMBER() OVER (ORDER BY d.DeptCode) as row from DeptMaster d, CompanyMaster c where c.Companycode = d.companycode ";
        }

        return query;
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
                query += " and d.DeptCode='" + keyword + "'";
                break;
            case 2:
                query += " and d.Deptname like '%" + keyword + "%'";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetDepartmentData(int page_number, bool is_filter, string filters)
    {
        masters_department page_object = new masters_department();
        DBConnection db_connection = new DBConnection();
        DataTable department_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;

        try
        {

            query = page_object.GetBaseQuery();

            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            query += " ) a where row > " + start_row + " and row < " + number_of_record;

            department_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(department_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_DEPARTMENT_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Department data. Please try again. If the error persists, please contact Support.";

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

    private void UpdateDatabase(string mode, string department_code, string department_name, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        Hashtable department_data = new Hashtable();

        department_data.Add("Mode", mode);
        department_data.Add("DepartmentCode", department_code);
        department_data.Add("DepartmentName", department_name);
        department_data.Add("CompanyCode", company_code);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateDepartment", department_data);

    }

    private int CheckDepartmentName(string department_name, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from DeptMaster where DeptName = '" + department_name + "' and CompanyCode = '" + company_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckDepartmentCode(string department_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from DeptMaster where DeptCode = '" + department_code + "' ";
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
    public static ReturnObject AddDepartment(string current)
    {

        masters_department page_object = new masters_department();
        ReturnObject return_object = new ReturnObject();
        string company_code = string.Empty;
        string department_code = string.Empty;
        string department_name = string.Empty;
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
                department_code = current_data["department_code"].ToString();
                department_name = current_data["department_name"].ToString();

                count = page_object.CheckDepartmentCode(department_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Department Code has been taken. Please try again with a different Code.";

                    return return_object;
                }

                count = page_object.CheckDepartmentName(department_name, company_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Department Name has been taken. Please try again with a different Name.";

                    return return_object;
                }

                page_object.UpdateDatabase("I", department_code, department_name, company_code);

                return_object.status = "success";
                return_object.return_data = "Department added successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "ADD_DEPARTMENT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving the Department. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject EditDepartment(string current, string previous)
    {

        masters_department page_object = new masters_department();
        ReturnObject return_object = new ReturnObject();
        string original_department_name = string.Empty;
        string company_code = string.Empty;
        string department_code = string.Empty;
        string department_name = string.Empty;
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
                department_code = current_data["department_code"].ToString();
                department_name = current_data["department_name"].ToString();

                JObject previous_data = JObject.Parse(previous);
                original_department_name = previous_data["department_name"].ToString();

                if (original_department_name != department_name)
                {
                    count = page_object.CheckDepartmentName(department_name, company_code);
                    if (count > 0)
                    {
                        return_object.status = "error";
                        return_object.return_data = "Department Name has been taken. Please try again with a different Name";

                        return return_object;
                    }
                }

                page_object.UpdateDatabase("U", department_code, department_name, company_code);

                return_object.status = "success";
                return_object.return_data = "Department edited successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "EDIT_DEPARTMENT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving the Department. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject DeleteDepartment(string current)
    {

        masters_department page_object = new masters_department();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        string company_code = string.Empty;
        string department_code = string.Empty;
        string department_name = string.Empty;
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
                department_code = current_data["department_code"].ToString();
                department_name = current_data["department_name"].ToString();
                company_code = current_data["company_code"].ToString();

                query = "select count(*) from EmployeeMaster where Emp_Department = '" + department_code + "' and Emp_Status = 1";
                count = db_connection.GetRecordCount(query);

                if (count > 0)
                {

                    return_object.status = "error";
                    return_object.return_data = "Please unassign or delete the employees mapped to this Department and try again.";

                }
                else
                {

                    page_object.UpdateDatabase("D", department_code, department_name, company_code);

                    return_object.status = "success";
                    return_object.return_data = "Department deleted successfully!";
                }
            }
            catch (Exception Ex)
            {

                Logger.LogException(Ex, page, "DELETE_DEPARTMENT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while deleteing the Department. Please try again. If the error persists, please contact Support.";

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }

    private string CreateExport(DataTable company_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Department Code", "Department Name", "Company Name", "CompanyCode" };

        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "DepartmentMaster-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "DEPARTMENT MASTER", company_data, Context, column_names);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport(string filters, bool is_filter)
    {
        masters_department page_object = new masters_department();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable branch_data = new DataTable();
        DateTime now = DateTime.Now;
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);

        string[] column_names = new string[] { };

        string
            query = string.Empty, file_name = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {
                query = page_object.GetBaseQuery();

                if (is_filter)
                    query += page_object.GetFilterQuery(filters, query);

                query += " ) a where row > 0 and row < " + export_limit;

                branch_data = db_connection.ReturnDataTable(query);

                if (branch_data.Rows.Count > 0)
                {

                    file_name = page_object.CreateExport(branch_data);

                    return_object.status = "success";
                    return_object.return_data = file_name;
                }
                else
                {
                    return_object.status = "info";
                    return_object.return_data = "No data found with the selected filters. Please try again with different filters.";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "GET_DATA_FOR_EXPORT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while generating your report. Please try again. If the error persists, please contact Support.";

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