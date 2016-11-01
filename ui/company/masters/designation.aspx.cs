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
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Export.Excel;
using SecurAX.Logger;

public partial class masters_designation : System.Web.UI.Page
{
    const string page = "DESIGNATION MASTER";

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

            message = "An error occurred while loading Designation Master. Please try again. If the error persists, please contact Support.";

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
            query = "select desigcode as designation_code, designame as designation_name, CompanyName as company_name, CompanyCode as company_code FROM ( select d.desigcode, d.designame, c.CompanyName, c.CompanyCode, ROW_NUMBER() OVER (ORDER BY d.desigcode) as row from desigmaster d, companymaster c where d.companycode = c.companycode and c.CompanyCode='" + company_code + "'  ";
        }
        else
        {
            query = "select desigcode as designation_code, designame as designation_name, CompanyName as company_name, CompanyCode as company_code FROM ( select d.desigcode, d.designame, c.CompanyName, c.CompanyCode, ROW_NUMBER() OVER (ORDER BY d.desigcode) as row from desigmaster d, companymaster c where d.companycode = c.companycode ";
        }

        return query;
    }

    protected string GetFilterQuery(string filters, string query)
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
                query += " and d.desigcode ='" + keyword + "'";
                break;
            case 2:
                query += " and d.designame like '%" + keyword + "%'";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetDesignationData(int page_number, bool is_filter, string filters)
    {
        masters_designation page_object = new masters_designation();
        DBConnection db_connection = new DBConnection();
        DataTable designation_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;

        try
        {
            query = page_object.GetBaseQuery();

            if (is_filter) query = page_object.GetFilterQuery(filters, query);

            query += " ) a where row > " + start_row + " and row < " + number_of_record;

            designation_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(designation_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_DESIGNATION_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Designation data. Please try again. If the error persists, please contact Support.";

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
            return_object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private int CheckDesignationName(string designation_name, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from desigmaster where companycode = '" + company_code + "' and designame = '" + designation_name + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckDesignationCode(string designation_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from desigmaster where desigcode = '" + designation_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    public void UpdateDatabase(string designation_code, string designation_name, string company_code, string mode)
    {

        DBConnection db_connection = new DBConnection();
        Hashtable designation_hash_table = new Hashtable();

        designation_hash_table.Add("Mode", mode);
        designation_hash_table.Add("DesigCode", designation_code);
        designation_hash_table.Add("DesigName", designation_name);
        designation_hash_table.Add("CompanyCode", company_code);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateDesigMaster", designation_hash_table);
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
    public static ReturnObject AddDesignation(string current)
    {

        masters_designation page_object = new masters_designation();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string company_code = string.Empty;
        string designation_code = string.Empty;
        string designation_name = string.Empty;
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
                designation_name = current_data["designation_name"].ToString();
                designation_code = current_data["designation_code"].ToString();

                count = page_object.CheckDesignationCode(designation_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Designation Code has been taken. Please try again with a different Code.";

                    return return_object;
                }

                count = page_object.CheckDesignationName(designation_name, company_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Designation Name has been taken. Please try again with a different Name.";

                    return return_object;
                }

                page_object.UpdateDatabase(designation_code, designation_name, company_code, "I");

                return_object.status = "success";
                return_object.return_data = "Designation added successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "ADD_DESIGNATION");

                return_object.status = "error";
                return_object.return_data = "An error occurred while adding a new Designation. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject EditDesignation(string current, string previous)
    {

        masters_designation page_object = new masters_designation();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string original_designation_name = string.Empty;
        string company_code = string.Empty;
        string designation_code = string.Empty;
        string designation_name = string.Empty;
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
                designation_name = current_data["designation_name"].ToString();
                designation_code = current_data["designation_code"].ToString();

                JObject previous_data = JObject.Parse(previous);
                original_designation_name = previous_data["designation_name"].ToString();

                if (original_designation_name != designation_name)
                {
                    count = page_object.CheckDesignationName(designation_name, company_code);
                    if (count > 0)
                    {
                        return_object.status = "error";
                        return_object.return_data = "Designation Name has been taken. Please try again with a different Name.";

                        return return_object;
                    }
                }

                page_object.UpdateDatabase(designation_code, designation_name, company_code, "U");

                return_object.status = "success";
                return_object.return_data = "Designation edited successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "EDIT_DESIGNATION");

                return_object.status = "error";
                return_object.return_data = "An error occurred while saving Designation details. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject DeleteDesignation(string current)
    {
        masters_designation page_object = new masters_designation();
        Hashtable designation_hash_table = new Hashtable();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        string designation_code = string.Empty;
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
                designation_code = current_data["designation_code"].ToString();

                query = "select count(emp_designation) from employeemaster where emp_designation='" + designation_code + "' and emp_status = 1";
                count = db_connection.GetRecordCount(query);

                if (count > 0)
                {

                    return_object.status = "error";
                    return_object.return_data = "Employees have been assigned this Designation. Please reassign or delete the Employees.";

                }
                else
                {

                    designation_hash_table.Add("Mode", "D");
                    designation_hash_table.Add("DesigCode", designation_code);

                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateDesigMaster", designation_hash_table);

                    return_object.status = "success";
                    return_object.return_data = "Designation deleted successfully!";
                }
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "DELETE_DESIGNATION");

                return_object.status = "error";
                return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

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
            new string[] { "Designation Code", "Designation Name", "Company Name", "Company Code" };

        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "DesignationMaster-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "DESIGNATION MASTER", company_data, Context, column_names);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport(string filters, bool is_filter)
    {
        masters_designation page_object = new masters_designation();
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