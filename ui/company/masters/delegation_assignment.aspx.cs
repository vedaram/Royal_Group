﻿using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using SecurAX.Logger;
using System.Configuration;
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
using SecurAX.Export.Excel;

public partial class masters_delegation_assignment : System.Web.UI.Page
{
    const string page = "DELEGATION_ASSIGNMENT";

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

            message = "An error occurred while loading Delegation Assignment page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetDelegationData(int page_number)
    {

        masters_delegation_assignment page_object = new masters_delegation_assignment();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable delegation_data = new DataTable();
        int start_row = (page_number - 1) * 30;
        int number_of_rows = page_number * 30 + 1;
        string query = string.Empty;

        try
        {
            query = "select Fromdate as from_date, Todate as to_date, ManagerId as manager_id, DelidationManagerID as delegation_manager_id, row from (select Fromdate, Todate, ManagerId, DelidationManagerID, ROW_NUMBER() OVER (ORDER BY Fromdate) as row from TbAsignDelegation where DeliationStatus = 1) a where row > " + start_row + " and row < " + number_of_rows;

            delegation_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(delegation_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_DELEGATION_LIST");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Delegation data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable company_data = new DataTable();
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

    [WebMethod]
    public static ReturnObject GetBranchData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable branch_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DISTINCT BranchCode as branch_code, BranchName as branch_name from BranchMaster where CompanyCode = '" + company_code + "' ";
            branch_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(branch_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_BRANCH_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Branch Data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetManagerData(string company_code, string branch_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable manager_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select Distinct Emp_Code as employee_code, (Emp_Name+' ['+Emp_Code+']') As employee_name from employeemaster Where IsManager=1 And Emp_Company='" + company_code + "' And Emp_Branch='" + branch_code + "' ";
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
    public static ReturnObject getSourceManagerData(string company_code, string branch_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable source_manager_data = new DataTable();
        string employee_id, query = string.Empty;
        int access = 0;

        try
        {
            access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            // If manager is logged in then in source manager only that manager ID should be there .. 
            if (access == 1)
            {
                query = "select Distinct Emp_Code as employee_code, (Emp_Name+' ['+Emp_Code+']') As employee_name from employeemaster Where IsManager=1 And Emp_Company='" + company_code + "' And Emp_Branch='" + branch_code + "' and emp_code = '" + employee_id + "' ";
            }
            else
            {
                query = "select Distinct Emp_Code as employee_code, (Emp_Name+' ['+Emp_Code+']') As employee_name from employeemaster Where IsManager=1 And Emp_Company='" + company_code + "' And Emp_Branch='" + branch_code + "' ";
            }

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
    public static ReturnObject SaveDelegationAssignment(string current)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        JObject current_data = new JObject();

        int
            count = 0;

        string
            query = string.Empty,
            start_date, end_date,
            current_manager_id,
            delegation_manager_id;

        try
        {
            current_data = JObject.Parse(current);

            start_date = DateTime.ParseExact(current_data["from_date"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 00:00:00";
            end_date = DateTime.ParseExact(current_data["to_date"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 00:00:00";

            current_manager_id = current_data["manager_id"].ToString();
            delegation_manager_id = current_data["delegation_manager_id"].ToString();

            query = "select count(ManagerId) from TbAsignDelegation Where ManagerId = '" + current_manager_id + "' ";
            count = db_connection.GetRecordCount(query);

            if (count <= 0)
            {
                query = "insert into TbAsignDelegation(Fromdate,Todate,ManagerId,DelidationManagerID) values('" + start_date + "','" + end_date + "','" + current_manager_id + "','" + delegation_manager_id + "')";
                count = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            }
            else
            {
                query = "update TbAsignDelegation set Fromdate = '" + start_date + "',Todate = '" + end_date + "',DelidationManagerID = '" + delegation_manager_id + "',DeliationStatus=1 Where ManagerId='" + current_manager_id + "' ";
                count = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            }

            // Checking count of rows affected from either of the above queries.
            if (count > 0)
            {
                return_object.status = "success";
                return_object.return_data = "Delegation Assignment details saved successfully!";
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = "An error occurred while saving Delegation Assignment. Please try again. If the error persists, please contact Support.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SAVE_DELEGATION_ASSIGNMENT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving Delegation Assignment. Please try again. If the error persists, please contact Support.";
            throw;
        }

        return return_object;
    }
}