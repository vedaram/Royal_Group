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

public partial class masters_change_manager : System.Web.UI.Page
{
    const string page = "CHANGE_MANAGER";

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

            message = "An error occurred while loading Change Manager. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetBranchData(string company_code)
    {

        DataTable branch_data = new DataTable();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;

        try
        {
            query = "select DISTINCT BranchCode as branch_code, BranchName as branch_name from BranchMaster where CompanyCode = '" + company_code + "' order by BranchName ASC";
            branch_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(branch_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_BRANCH_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Branch data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    protected string GetFilterQuery(string query, string filters)
    {
        JObject filter_data = JObject.Parse(filters);
        string company_code = filter_data["filter_company"].ToString();
        string branch_code = filter_data["filter_branch"].ToString();
        int filter_by = Convert.ToInt32(filter_data["filter_by"]);
        string keyword = filter_data["filter_keyword"].ToString();

        if (company_code != "select")
            query += " and e.emp_company='" + company_code + "' ";

        if (branch_code != "select")
            query += "and e.emp_branch='" + branch_code + "' ";

        switch (filter_by)
        {
            case 1:
                query += " and e.emp_code = '" + keyword + "' ";
                break;
            case 2:
                query += " and e.emp_name like '%" + keyword + "%' ";
                break;
            case 3:
                query += " and e.emp_card_no='" + keyword + "' ";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetManagerData(string filters)
    {
        masters_change_manager page_object = new masters_change_manager();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable manager_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DISTINCT emp_code as employee_code, (emp_name +' [' + emp_code + ']') as employee_name from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode where e.emp_status = 1 and e.IsManager = 1 ";

            query = page_object.GetFilterQuery(query, filters);

            manager_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(manager_data, Formatting.Indented);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_MANAGER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Manager data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject getSourceManagerData(string filters)
    {
        masters_change_manager page_object = new masters_change_manager();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable source_manager_data = new DataTable();
        string employee_id, query, company_code = string.Empty;
        int access = 0;

        try
        {
            access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            // If manager is logged in then in source manager only that manager ID should be there .. 
            if (access == 1)
            {
                query = "select DISTINCT emp_code as employee_code, (emp_name +' [' + emp_code + ']') as employee_name from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode where e.emp_status = 1 and e.IsManager = 1 and e.emp_code ='" + employee_id + "'";
            }
            else
            {
                query = "select DISTINCT emp_code as employee_code, (emp_name +' [' + emp_code + ']') as employee_name from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode where e.emp_status = 1 and e.IsManager = 1";
            }

            query = page_object.GetFilterQuery(query, filters);

            source_manager_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(source_manager_data, Formatting.Indented);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_SOURCE_MANAGER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Manager data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetEmployeeData(string manager_id)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable employee_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DISTINCT emp_code as employee_code, emp_name as employee_name from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode where e.emp_status = 1 and e.ManagerId = '" + manager_id + "' and e.Emp_Code != '" + manager_id + "' ";

            employee_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employee_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_EMPLOYEE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Employee data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject UpdateManager(string employees, string new_manager_id)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int i = 0, affected_count = 0;

        try
        {
            List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);

            for (i = 0; i < employees_list.Count; i++)
            {
                query = "update EmployeeMaster set ManagerId = '" + new_manager_id + "' where Emp_Code = '" + employees_list[i] + "' ";
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                affected_count++;
            }

            return_object.status = "success";
            return_object.return_data = affected_count.ToString();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_MANAGER");

            return_object.status = "error";
            return_object.return_data = "An error occurred while updating Manager for selected employees. Please try again.";

            throw;
        }

        return return_object;
    }
}