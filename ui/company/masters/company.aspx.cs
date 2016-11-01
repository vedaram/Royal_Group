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
using System.Text.RegularExpressions;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Export.Excel;
using SecurAX.Logger;


public partial class masters_company : System.Web.UI.Page
{
    const string page = "COMPANY_MANAGEMENT";

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
            // log the exception
            Logger.LogException(ex, page, "PAGE_LOAD");

            message = "An error occurred while loading Company Master page. Please try again. If the error persists, please contact Support.";

            // display a generic error in the UI
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    private DataTable SetColumnNames(DataTable data_table)
    {
        data_table.Columns["CompanyCode"].ColumnName = "company_code";
        data_table.Columns["CompanyName"].ColumnName = "company_name";
        data_table.Columns["Address"].ColumnName = "company_address";
        data_table.Columns["Phone"].ColumnName = "phone_number";
        data_table.Columns["Fax"].ColumnName = "fax_number";
        data_table.Columns["Email"].ColumnName = "email_address";
        data_table.Columns["PIN"].ColumnName = "pin_code";
        data_table.Columns["URL"].ColumnName = "website";

        return data_table;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData(int page_number)
    {
        masters_company page_object = new masters_company();
        string employee_id, query, company_code = string.Empty;
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        ReturnObject return_object = new ReturnObject();
        int start_row = (page_number - 1) * 30;
        int number_of_rows = page_number * 30 + 1;

        try
        {
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            // if employee is logged in then showing only that employee company  data (  done for royal group client first then implemnted in standard as well )
            if (employee_id != "")
            {
                query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select * FROM (select *, ROW_NUMBER() OVER (ORDER BY CompanyCode) as row FROM CompanyMaster where companycode = '" + company_code + "' "; // a where row > " + start_row + " and row < " + number_of_rows;
            }
            else
            {
                query = "select * FROM (select *, ROW_NUMBER() OVER (ORDER BY CompanyCode) as row FROM CompanyMaster";
            }
            query += " ) a where row > " + start_row + " and row < " + number_of_rows;

            company_data = db_connection.ReturnDataTable(query);

            company_data = page_object.SetColumnNames(company_data);

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

    private void UpdateDatabase(string companycode, string companyname, string address, string phone, string fax, string email, string pin, string url, string mode)
    {
        Hashtable company_data = new Hashtable();
        DBConnection db_connection = new DBConnection();

        company_data.Add("Mode", mode);
        company_data.Add("CompanyCode", companycode);
        company_data.Add("CompanyName", companyname);
        company_data.Add("Address", address);
        company_data.Add("Phone", phone);
        company_data.Add("Fax", fax);
        company_data.Add("Email", email);
        company_data.Add("Pin", pin);
        company_data.Add("Url", url);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateCompany", company_data);
    }

    protected void CreateShiftSetting(string company_code)
    {

        Hashtable shift_settings = new Hashtable();
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;

        shift_settings.Add("Mode", "I");
        shift_settings.Add("CompanyCode", company_code);
        shift_settings.Add("actual", Convert.ToString(0));
        shift_settings.Add("break", "");
        shift_settings.Add("isaso", "");
        shift_settings.Add("isramadan", "");
        shift_settings.Add("ramadanstdt", "");
        shift_settings.Add("ramadantodt", "");

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateShiftSetting", shift_settings);

        query = "update ShiftSetting Set isfkey = 0 where CompanyCode ='" + company_code + "'";
        db_connection.ExecuteQuery_WithOutReturnValue(query);
    }

    private int CheckCompanyCode(string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from CompanyMaster where CompanyCode = '" + company_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckCompanyName(string company_name)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from CompanyMaster where CompanyName = '" + company_name + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    [WebMethod]
    public static ReturnObject AddCompany(string current)
    {

        masters_company page_object = new masters_company();
        ReturnObject return_object = new ReturnObject();
        string company_name = string.Empty;
        string company_code = string.Empty;
        string address = string.Empty;
        string phone = string.Empty;
        string fax = string.Empty;
        string email = string.Empty;
        string pin = string.Empty;
        string url = string.Empty;
        string message = string.Empty;
        int count = 0;

        try
        {
            JObject current_data = JObject.Parse(current);
            company_name = current_data["company_name"].ToString();
            company_code = current_data["company_code"].ToString();
            address = current_data["company_address"].ToString();
            phone = current_data["phone_number"].ToString();
            fax = current_data["fax_number"].ToString();
            email = current_data["email_address"].ToString();
            pin = current_data["pin_code"].ToString();
            url = current_data["website"].ToString();

            count = page_object.CheckCompanyCode(company_code);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Company Code has been taken. Please try again with a different Code.";

                return return_object;
            }

            count = page_object.CheckCompanyName(company_name);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Company Name has been taken. Please try again with a different Name.";

                return return_object;
            }

            page_object.UpdateDatabase(company_code, company_name, address, phone, fax, email, pin, url, "I");

            // creating default row for Shift Setting
            page_object.CreateShiftSetting(company_code);

            return_object.status = "success";
            return_object.return_data = "Company added successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "ADD_COMPANY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while adding a new Company. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject EditCompany(string current, string previous)
    {

        masters_company page_object = new masters_company();
        ReturnObject return_object = new ReturnObject();
        string
            previous_company_name = string.Empty,
            company_name = string.Empty,
            company_code = string.Empty,
            address = string.Empty,
            phone = string.Empty,
            fax = string.Empty,
            email = string.Empty,
            pin = string.Empty,
            url = string.Empty,
            message = string.Empty;

        int count = 0;

        try
        {

            JObject current_data = JObject.Parse(current);
            company_name = current_data["company_name"].ToString();
            company_code = current_data["company_code"].ToString();
            address = current_data["company_address"].ToString();
            phone = current_data["phone_number"].ToString();
            fax = current_data["fax_number"].ToString();
            email = current_data["email_address"].ToString();
            pin = current_data["pin_code"].ToString();
            url = current_data["website"].ToString();

            JObject previous_data = JObject.Parse(previous);
            previous_company_name = previous_data["company_name"].ToString();

            if (previous_company_name != company_name)
            {
                count = page_object.CheckCompanyName(company_name);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Company Name has been taken. Please try again with a different Name.";

                    return return_object;
                }
            }

            page_object.UpdateDatabase(company_code, company_name, address, phone, fax, email, pin, url, "U");

            return_object.status = "success";
            return_object.return_data = "Company edited successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "EDIT_COMPANY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while editing Company details. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteCompany(string current)
    {

        masters_company page_object = new masters_company();

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        int employee_count = 0;
        int branch_count = 0;
        int department_count = 0;
        int designation_count = 0;
        int employee_category_count = 0;
        int holiday_list_count = 0;
        int holiday_group_count = 0;

        string query = string.Empty;
        string company_code = string.Empty;
        string company_name = string.Empty;
        string address = string.Empty;
        string phone = string.Empty;
        string fax = string.Empty;
        string email = string.Empty;
        string pin = string.Empty;
        string url = string.Empty;

        try
        {

            JObject current_data = JObject.Parse(current);
            company_name = current_data["company_name"].ToString();
            company_code = current_data["company_code"].ToString();
            address = current_data["company_address"].ToString();
            phone = current_data["phone_number"].ToString();
            fax = current_data["fax_number"].ToString();
            email = current_data["email_address"].ToString();
            pin = current_data["pin_code"].ToString();
            url = current_data["website"].ToString();

            query = "select count (*) from EmployeeMaster where Emp_Company='" + company_code + "'";
            employee_count = db_connection.GetRecordCount(query);
            if (employee_count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Employees have been mapped to this Company. Please delete or reassign the employees.";

                return return_object;
            }

            query = "select count (*) from DeptMaster where CompanyCode='" + company_code + "'";
            department_count = db_connection.GetRecordCount(query);
            if (department_count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Departments have been mapped to this Company. Please delete or reassign the departments.";

                return return_object;
            }

            query = "select count (*) from BranchMaster where CompanyCode='" + company_code + "'";
            branch_count = db_connection.GetRecordCount(query);
            if (branch_count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Branches have been mapped to this Company. Please delete or reassign the branches.";

                return return_object;
            }

            query = "select count (*) from DesigMaster where CompanyCode='" + company_code + "'";
            designation_count = db_connection.GetRecordCount(query);
            if (designation_count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Designations have been mapped to this Company. Please delete or reassign the designations.";

                return return_object;
            }

            query = "select count (*) from EmployeeCategoryMaster where CompanyCode='" + company_code + "'";
            employee_category_count = db_connection.GetRecordCount(query);
            if (employee_category_count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Employee Categories have been mapped to this Company. Please delete or reassign the categories.";

                return return_object;
            }

            query = "select count (*) from HolidayMaster where CompanyCode = '" + company_code + "' ";
            holiday_list_count = db_connection.GetRecordCount(query);
            if (holiday_list_count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Holidays have been mapped to this Company. Please delete or reassign the categories.";

                return return_object;
            }

            query = "select count (*) from HolidayGroup where CompanyCode = '" + company_code + "' ";
            holiday_group_count = db_connection.GetRecordCount(query);
            if (holiday_group_count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Holiday Groups have been mapped to this Company. Please delete or reassign the categories.";

                return return_object;
            }

            //TODO: Add query to delete Shift Settings record

            page_object.UpdateDatabase(company_code, company_name, address, phone, fax, email, pin, url, "D");

            query = "delete from Shift where CompanyCode='" + company_code + "'";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            query = "delete from ShiftEmployee where EmpCompanyCode='" + company_code + "'";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            query = "delete from LeaveMaster where CompanyCode='" + company_code + "'";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            return_object.status = "success";
            return_object.return_data = "Company deleted successfully!";

        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "DELETE_COMPANY");

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

    private string CreateExport(DataTable company_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Company Code", "Company Name", "Address", "Phone", "Fax", "Email", "PIN", "URL" };

        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "CompanyMaster-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "COMPANY MASTER", company_data, Context, column_names);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport()
    {
        masters_company page_object = new masters_company();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        DateTime now = DateTime.Now;
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);

        string[] column_names = new string[] { };

        string
            query = string.Empty, file_name = string.Empty;

        try
        {
            query = "select TOP " + export_limit + " * from CompanyMaster";
            company_data = db_connection.ReturnDataTable(query);

            if (company_data.Rows.Count > 0)
            {

                file_name = page_object.CreateExport(company_data);

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

        return return_object;
    }
}
