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

public partial class masters_employee_category : System.Web.UI.Page
{
    const string page = "EMPLOYEE_CATEGORY";

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

            message = "An error occurred while loading Employee Category Master. Please try again. If the error persists, please contact Support.";

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
            query = "select EmpCategoryCode as employee_category_code, EmpCategoryName as employee_category_name, CompanyCode as company_code, CompanyName as company_name, Totalhrs as total_hours, process from ( select e.EmpCategoryCode, e.EmpCategoryName, c.CompanyCode, c.CompanyName, e.Totalhrs, e.includeprocess as process, ROW_NUMBER() OVER (ORDER BY e.EmpCategoryCode) as row from EmployeeCategoryMaster e, CompanyMaster c where e.companyCode = c.CompanyCode and  c.CompanyCode='" + company_code + "' ";
        }
        else
        {
            query = "select EmpCategoryCode as employee_category_code, EmpCategoryName as employee_category_name, CompanyCode as company_code, CompanyName as company_name, Totalhrs as total_hours, process from ( select e.EmpCategoryCode, e.EmpCategoryName, c.CompanyCode, c.CompanyName, e.Totalhrs, e.includeprocess as process, ROW_NUMBER() OVER (ORDER BY e.EmpCategoryCode) as row from EmployeeCategoryMaster e, CompanyMaster c where e.companyCode = c.CompanyCode ";
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
                query += " and e.EmpCategoryName like '%" + keyword + "%'";
                break;
            case 2:
                query += " and e.EmpCategoryCode ='" + keyword + "'";
                break;
        }

        return query;
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
    public static ReturnObject GetEmployeeCategoryData(int page_number, bool is_filter, string filters)
    {

        masters_employee_category page_object = new masters_employee_category();
        DBConnection db_connection = new DBConnection();
        DataTable employeeCategory_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;
        string query = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                query = page_object.GetBaseQuery();

                if (is_filter) query = page_object.GetFilterQuery(filters, query);

                query += " ) a where row > " + start_row + " and row < " + number_of_record;
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
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {
        masters_employee_category page_object = new masters_employee_category();
        DataTable company_data = new DataTable();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty, employee_id = string.Empty, company_code = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
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
        }

        return return_object;
    }

    private void UpdateDatabase(string mode, string employee_category_code, string employee_category_name, string company_code, string total_hours, int process)
    {

        DBConnection db_connection = new DBConnection();
        Hashtable employee_category_hash_table = new Hashtable();

        employee_category_hash_table.Add("mode", mode);
        employee_category_hash_table.Add("EmpCategoryCode", employee_category_code);
        employee_category_hash_table.Add("EmpCategoryName", employee_category_name);
        employee_category_hash_table.Add("CompanyCode", company_code);
        employee_category_hash_table.Add("totalhrs", total_hours);
        employee_category_hash_table.Add("process", process);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateGradeMaster1", employee_category_hash_table);

    }

    private int CheckEmployeeCategoryCode(string employee_category_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from EmployeeCategoryMaster where EmpCategoryCode = '" + employee_category_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckEmployeeCategoryName(string employee_category_name, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from EmployeeCategoryMaster where EmpCategoryName='" + employee_category_name + "' and Companycode='" + company_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    [WebMethod]
    public static ReturnObject AddEmployeeCategory(string current)
    {

        masters_employee_category page_object = new masters_employee_category();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string employee_category_code = string.Empty;
        string employee_category_name = string.Empty;
        string company_code = string.Empty;
        string total_hours = string.Empty;
        int process = 0;
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
                employee_category_code = current_data["employee_category_code"].ToString();
                employee_category_name = current_data["employee_category_name"].ToString();
                company_code = current_data["company_code"].ToString();
                total_hours = current_data["total_hours"].ToString();
                process = Convert.ToInt32(current_data["process"]);

                count = page_object.CheckEmployeeCategoryCode(employee_category_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Employee Category Code has been taken. Please try again with a different Code.";

                    return return_object;
                }

                count = page_object.CheckEmployeeCategoryName(employee_category_name, company_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Employee Category Name has been taken. Please try again with a different Name.";

                    return return_object;
                }

                page_object.UpdateDatabase("I", employee_category_code, employee_category_name, company_code, total_hours, process);

                return_object.status = "success";
                return_object.return_data = "Employee Category added successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "ADD_EMPLOYEE_CATEGORY");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving the Employee Category. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject EditEmployeeCategory(string current, string previous)
    {

        masters_employee_category page_object = new masters_employee_category();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string original_employee_category_name = string.Empty;
        string employee_category_code = string.Empty;
        string employee_category_name = string.Empty;
        string company_code = string.Empty;
        string total_hours = string.Empty;
        int process = 0;
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
                employee_category_code = current_data["employee_category_code"].ToString();
                employee_category_name = current_data["employee_category_name"].ToString();
                company_code = current_data["company_code"].ToString();
                total_hours = current_data["total_hours"].ToString();
                process = Convert.ToInt32(current_data["process"]);

                JObject previous_data = JObject.Parse(previous);
                original_employee_category_name = previous_data["employee_category_name"].ToString();

                if (original_employee_category_name != employee_category_name)
                {
                    count = page_object.CheckEmployeeCategoryName(employee_category_name, company_code);
                    if (count > 0)
                    {
                        return_object.status = "error";
                        return_object.return_data = "Employee Category Name has been taken. Please try again with a different Name.";

                        return return_object;
                    }
                }

                page_object.UpdateDatabase("U", employee_category_code, employee_category_name, company_code, total_hours, process);

                return_object.status = "success";
                return_object.return_data = "Employee Category edited successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "EDIT_EMPLOYEE_CATEGORY");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving the Employee Category. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject DeleteEmployeeCategory(string current)
    {
        masters_employee_category page_object = new masters_employee_category();
        DBConnection db_connection = new DBConnection();
        Hashtable employee_category_hash_table = new Hashtable();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        string employee_category_code = string.Empty;
        string company_code = string.Empty;
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
                employee_category_code = current_data["employee_category_code"].ToString();
                company_code = current_data["company_code"].ToString();

                query = "select count(*) from employeemaster where emp_Employee_Category = '" + employee_category_code + "' and Emp_Status = '1' ";
                count = db_connection.GetRecordCount(query);

                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Employees have been assigned to this Employee Category. Please reassign the Employees and try again.";
                }
                else
                {

                    employee_category_hash_table.Add("mode", "D");
                    employee_category_hash_table.Add("EmpCategoryCode", employee_category_code);
                    employee_category_hash_table.Add("CompanyCode", company_code);

                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateGradeMaster1", employee_category_hash_table);

                    return_object.status = "success";
                    return_object.return_data = "Employee Category deleted successfully!";
                }

            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "DELETE_EMPLOYEE_CATEGORY");

                return_object.status = "error";
                return_object.return_data = "An error occurred while deleting this Employee Category. Please try again. If the error persists, please contact Support.";
                throw;
            }
        }

        return return_object;
    }

    private string CreateExport(DataTable company_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Employee Category Code", "Employee Category Name", "Company Code", "Company Name", "Total Hours", "Process" };

        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "EmployeeCategoryMaster-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "EMPLOYEE CATEGORY MASTER", company_data, Context, column_names);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport(string filters, bool is_filter)
    {
        masters_employee_category page_object = new masters_employee_category();
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