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
using SecurAX.Export.Excel;

public partial class daily_punch_report : System.Web.UI.Page
{
    const string page = "DAILY_PUNCH_REPORT";

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

            message = "An error occurred while loading Daily Performance Report. Please try again. If the error persists, please contact Support.";

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
                //query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                //company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                //query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster where CompanyCode='" + company_code + "'";
                query = " Select distinct( CompanyCode) as  company_code,CompanyName as company_name  from CompanyMaster   where  CompanyCode in ( Select  CompanyCode from  TbManagerHrBranchMapping where ManagerID='" + employee_id + "')  or CompanyCode in (Select Emp_company  from Employeemaster where Emp_code='" + employee_id + "')";
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
            return_object.return_data = "An error occurred while loading Company Data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }
    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        string query = string.Empty;

        try
        {

            query = "Select DeptName as department_name from DeptMaster where CompanyCode='" + company_code + "' order by DeptName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "department";
            return_data_set.Tables.Add(temp_data_table);

            query = "select distinct Shift_Desc as shift_name from Shift where CompanyCode='" + company_code + "' and IsActive=1 order by Shift_Desc";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "shift";
            return_data_set.Tables.Add(temp_data_table);

            query = "select DISTINCT BranchName as branch_name from BranchMaster where CompanyCode='" + company_code + "' order by BranchName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "branch";
            return_data_set.Tables.Add(temp_data_table);

            query = "select  DISTINCT EmpCategoryName as category_name from EmployeeCategoryMaster where CompanyCode='" + company_code + "' order by EmpCategoryName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "category";
            return_data_set.Tables.Add(temp_data_table);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_OTHER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }
    private void PrepareDataForExport(string filters, string branches)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable filter_conditions = new Hashtable();
        JObject filters_data = new JObject();
        JArray branch_data = new JArray();

        string
            from_date, to_date, user_name,
            employee_name, employee_id, current_user_id,
            where_clause = string.Empty, company_name,category_name ,  employee_status ,
            department_name, shift_name, branch_list = string.Empty;

        int
            user_access_level, i = 0;

        filters_data = JObject.Parse(filters);
        company_name = filters_data["company_name"].ToString();
        department_name = filters_data["department"].ToString();
        shift_name = filters_data["shift"].ToString();
        category_name = filters_data["category"].ToString();
        employee_status = filters_data["status"].ToString();
        employee_id = filters_data["employee_id"].ToString();
        employee_name = filters_data["employee_name"].ToString();

        from_date = DateTime.ParseExact(filters_data["from_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        to_date = DateTime.ParseExact(filters_data["to_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

        // Get current logged in user data from the Session variable
        user_name = HttpContext.Current.Session["username"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        current_user_id = HttpContext.Current.Session["employee_id"].ToString();

        // process branch IDs
        branch_data = JArray.Parse(branches);
        for (i = 0; i < branch_data.Count; i++)
        {
            branch_list += "'" + branch_data[i].ToString() + "',";
        }

        where_clause += " where m.punchdate between '" + from_date + "' and '" + to_date + "' ";

        if (company_name != "select")
        {
           // where_clause = where_clause + " and c.Companyname = '" + company_name + "'";

            if (!string.IsNullOrEmpty(branch_list))
            {
                branch_list = branch_list.Remove(branch_list.Length - 1, 1);
                where_clause = where_clause + " and b.branchname in (" + branch_list + ")";
            }

            if (department_name != "select")
                where_clause = where_clause + " and d.DeptName = '" + department_name + "'";

            if (shift_name != "select")
                where_clause = where_clause + " and s.Shift_Desc = '" + shift_name + "'";


            if (category_name != "select")
                where_clause = where_clause + " and ecm.EmpCategoryName='" + category_name + "'";

            // Checking to see if an employee ID has been selected in the filters.
            if (employee_id != "") // If YES then we pull results only for the selected employee ID.
                where_clause = where_clause + " and m.EmpId = '" + employee_id + "'";
            else
            {
                where_clause += " and m.EmpId in (select EmpID from [FetchEmployees] ('" + current_user_id + "','" + employee_status + "'))";
            }
            if (employee_name != "")
                where_clause = where_clause + " and e.Emp_Name like '%" + employee_name + "%'";

            filter_conditions.Add("where", where_clause);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("DailyPunchReport1", filter_conditions);
        }

    }
    private DataTable GetFilteredData()
    {

        // NOTE: 30,000 is the upper limit on the number of rows that can be exported.
        // This value can be adjusted in the Web.config
        // Please consult a Senior Developer before changing this UPPER LIMIT.

        DBConnection db_connection = new DBConnection();
        DataTable filtered_data = new DataTable();
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);
        string query = string.Empty;

        query = "select TOP " + export_limit + " PDate, Emp_ID, Emp_Name, CONVERT(varchar,punch_time,108) ,Branch_Name,  Dept_Name,EmployeeCategory Shift_Name from DailyEmployeePunchReport";
        filtered_data = db_connection.ReturnDataTable(query);

        return filtered_data;
    }
    private string CreateExport(DataTable filtered_data, string company_name)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Punch Date", "Employee ID", "Employee Name","Punch Time" , "Branch", "Department","Category", "Shift"};

     

        string importFile,
      user_id = HttpContext.Current.Session["employee_id"].ToString(),
      exportPath = "DailyPunchReport-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
        importFile = "DailyAttendanceReport_Template" + ".xlsx";

        string importFilePath = HttpContext.Current.Server.MapPath("~/exports/templates/" + importFile);
        ExcelExport.ExportDataToExcelWithLogo(importFilePath, "DAILY PUNCH REPORT", filtered_data, Context, column_names, company_name, exportPath);

        return exportPath;
    }
    [WebMethod]
    public static ReturnObject DoExport(string filters, string branches)
    {

        daily_punch_report page_object = new daily_punch_report(); // instance of the page to access non static methods.
        ReturnObject return_object = new ReturnObject();
        DataTable filtered_data = new DataTable();
        JObject filters_data = new JObject();
        string file_name, company_name, exportPath = string.Empty;
        filters_data = JObject.Parse(filters);
        company_name = filters_data["company_name"].ToString();
        try
        {

            page_object.PrepareDataForExport(filters, branches);
            filtered_data = page_object.GetFilteredData();

            if (filtered_data.Rows.Count > 0)
            {

                file_name = page_object.CreateExport(filtered_data, company_name);
                exportPath = file_name.Substring(0, file_name.LastIndexOf('.'));
                exportPath = exportPath + ".xlsx";
                return_object.status = "success";
                return_object.return_data = exportPath;
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
            page_object.Dispose(); //Disposing of the page object to avoid memory leak.
        }

        return return_object;
    }
}
