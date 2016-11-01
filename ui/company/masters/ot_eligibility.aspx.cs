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

public partial class masters_ot_eligibility : System.Web.UI.Page
{
    const string page = "OT_ELIGIBILITY";

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

            message = "An error occurred while loading OT Eligibility Master. Please try again. If the error persists, please contact Support.";

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

        JObject filter_data = JObject.Parse(filters);
        string company_code = filter_data["filter_company"].ToString();
        string branch_code = filter_data["filter_branch"].ToString();
        string department_code = filter_data["filter_department"].ToString();
        string designation_code = filter_data["filter_designation"].ToString();
        string keyword = filter_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filter_data["filter_by"]);

        if (company_code != "select") query += " and c.companycode ='" + company_code + "' ";

        if (branch_code != "select") query += " and e.emp_branch ='" + branch_code + "' ";

        if (department_code != "select") query += " and e.emp_department ='" + department_code + "' ";

        if (designation_code != "select") query += " and e.emp_designation ='" + designation_code + "'";

        switch (filter_by)
        {

            case 1:
                query += " and e.emp_code like '%" + keyword + "%' ";
                break;
            case 2:
                query += " and e.emp_name like '%" + keyword + "%'";
                break;
            case 3:
                query += " and e.emp_card_no = '" + keyword + "' ";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetEmployeeData(int page_number, string filters)
    {

        masters_ot_eligibility page_object = new masters_ot_eligibility();
        DBConnection db_connection = new DBConnection();
        DataTable shifts_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string employee_id, query = string.Empty;
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                employee_id = HttpContext.Current.Session["employee_id"].ToString();
                // if employee is logged in then showing only that employee   data  who belongs to those (  done for royal group client first then implemnted in standard as well )
                if (employee_id != "")
                {
                    query = "select DISTINCT EmpCode as employee_code, EmpName as employee_name, CompanyName as company_name, CompanyCode as company_code, branchname as branch_name, deptname as department_name, designame as designation_name, emp_card_no as employee_card_number, OT_Eligibility as ot_eligibility from ( select e.emp_code as EmpCode, e.emp_name as EmpName, c.CompanyName, c.CompanyCode, b.branchname, d.deptname, de.designame, e.emp_card_no, e.OT_Eligibility, ROW_NUMBER() OVER (ORDER BY e.emp_code) as row from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode where 1=1 and  e.emp_code in ( select empid from FetchEmployees('" + employee_id + "','')) ";
                }
                else
                {
                    query = "select DISTINCT EmpCode as employee_code, EmpName as employee_name, CompanyName as company_name, CompanyCode as company_code, branchname as branch_name, deptname as department_name, designame as designation_name, emp_card_no as employee_card_number, OT_Eligibility as ot_eligibility from ( select e.emp_code as EmpCode, e.emp_name as EmpName, c.CompanyName, c.CompanyCode, b.branchname, d.deptname, de.designame, e.emp_card_no, e.OT_Eligibility, ROW_NUMBER() OVER (ORDER BY e.emp_code) as row from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode where 1=1 ";
                }

                query += " and e.Emp_Status='1'";
                
                query = page_object.GetFilterQuery(filters, query);

                query += " group by e.emp_code, e.emp_name, c.CompanyCode, c.CompanyName, b.branchcode, b.branchname, d.deptname, de.designame, e.emp_card_no, e.OT_Eligibility) a where row > " + start_row + " and row < " + number_of_record;

                shifts_data_table = db_connection.ReturnDataTable(query);

                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(shifts_data_table, Formatting.Indented);
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "GET_EMPLOYEE_DATA");

                return_object.status = "error";
                return_object.return_data = "An error occurred while loading Employee data. Please try again. If the error persists, please contact Support.";

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

    //  Method Created for showing session expired message ...
    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {
        masters_ot_eligibility page_object = new masters_ot_eligibility();
        DBConnection db_connection = new DBConnection();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        ReturnObject return_object = new ReturnObject();
        string department_query = string.Empty;
        string designation_query = string.Empty;
        string branch_query = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                department_query = "Select DeptName as department_name, DeptCode as department_code from DeptMaster where CompanyCode='" + company_code + "' order by DeptCode";
                temp_data_table = db_connection.ReturnDataTable(department_query);
                temp_data_table.TableName = "department";
                return_data_set.Tables.Add(temp_data_table);

                designation_query = "Select desigcode as designation_code, designame as designation_name from DesigMaster where CompanyCode='" + company_code + "' order by desigcode";
                temp_data_table = db_connection.ReturnDataTable(designation_query);
                temp_data_table.TableName = "designation";
                return_data_set.Tables.Add(temp_data_table);

                branch_query = "Select DISTINCT BranchCode as branch_code, BranchName as branch_name from BranchMaster where CompanyCode='" + company_code + "' order by BranchName";
                temp_data_table = db_connection.ReturnDataTable(branch_query);
                temp_data_table.TableName = "branch";
                return_data_set.Tables.Add(temp_data_table);

                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "GET_OTHER_DATA");

                return_object.status = "error";
                return_object.return_data = "An error occurred while loading data for OT Eligibility. Please try again. If the error persists, please contact Support.";

                throw;
            }
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject SaveOTEligibility(string employees, string action)
    {
        masters_ot_eligibility page_object = new masters_ot_eligibility();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);
        string query = string.Empty;
        int i = 0;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                for (i = 0; i < employees_list.Count; i++)
                {
                    query = "update EmployeeMaster set OT_Eligibility='" + action + "' where Emp_Code='" + employees_list[i] + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }

                return_object.status = "success";
                return_object.return_data = "OT Eligibility details saved successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "SAVE_OT_ELIGIBILITY");

                return_object.status = "error";
                return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

                throw;
            }
        }

        return return_object;
    }
}