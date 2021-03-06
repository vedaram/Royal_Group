﻿using System;
using System.Collections;
using System.Configuration;
using System.IO;
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
using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Export.Excel;
using SecurAX.Logger;

public partial class monthly_detailed_report : System.Web.UI.Page
{
    const string page = "DETAILED_MONTHLY_REPORT";

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

            message = "An error occurred while loading Detailed Monthly Report. Please try again. If the error persists, please contact Support.";

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

            query = "select DeptName as department_name from DeptMaster where CompanyCode='" + company_code + "' order by DeptName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "department";
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

    private DataTable PrepareDataForExport(string filters)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable filter_conditions = new Hashtable();
        DataTable filtered_data = new DataTable();
        JObject filters_data = new JObject();

        string
            from_date, to_date, user_name, current_user_id,
            where_clause = string.Empty, company_name,
            branch_name, department_name,
            employee_id, employee_name, category_name, employee_status = string.Empty;

        int
            user_access_level;

        filters_data = JObject.Parse(filters);
        company_name = filters_data["company_name"].ToString();
        branch_name = filters_data["branch"].ToString();
        department_name = filters_data["department"].ToString();
        employee_name = filters_data["employee_name"].ToString();
        employee_id = filters_data["employee_id"].ToString();
        category_name = filters_data["category"].ToString();
        employee_status = filters_data["status"].ToString();

        from_date = DateTime.ParseExact(filters_data["from_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        to_date = DateTime.ParseExact(filters_data["to_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

        // Get current logged in user data from the Session variable
        user_name = HttpContext.Current.Session["username"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (user_name != "admin")
            current_user_id = HttpContext.Current.Session["employee_id"].ToString();
        else
            current_user_id = user_name;

        Hashtable employees = new Hashtable();
        employees.Add("piuserid", current_user_id);
        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("sp_fetchemployee1", employees);
        where_clause = " where emp_id in (select EmpID from [FetchEmployees] ('" + current_user_id + "','" + employee_status + "')) ";

        if (company_name != "select")
        {
           // where_clause = where_clause + " and COMP_NAME = '" + company_name + "'";

            if (branch_name != "select")
                where_clause = where_clause + " and CAT_NAME = '" + branch_name + "'";

            if (department_name != "select")
                where_clause = where_clause + " and DEPT_NAME = '" + department_name + "'";

            if (category_name != "select")
                where_clause = where_clause + " and EmployeeCategory='" + category_name + "'";

            if (employee_id != "")
                where_clause = where_clause + " and Emp_ID = '" + employee_id + "' ";
            
            if (employee_name != "")
                where_clause = where_clause + " and Emp_Name like '%" + employee_name + "%' ";
            
            filter_conditions.Add("Fromdate", from_date);
            filter_conditions.Add("Todate", to_date);

            filter_conditions.Add("WhereCondition", where_clause);

            filtered_data = db_connection.ExecuteStoredProcedureWithHashtable_WithReturnDatatable("PrepareMonthlyTimeSheet", filter_conditions);
        }

        return filtered_data;
    }

    private string CreateExport(DataTable filtered_data , string companyname)
    {
        string FileExtension = "EXCEL";
        DateTime now = DateTime.Now;
        string user_id = HttpContext.Current.Session["employee_id"].ToString();
        Warning[] warnings = null;
        string[] streamids = null;
        string mimeType = string.Empty;
        string encoding = string.Empty;
        string extension = string.Empty;
        string file_name = "DetailedMonthlyTimeSheet" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".";

        byte[] bytes = null;
        ReportDataSource rds = new ReportDataSource("DetailedMonthly_DetailedMonthlyReport1", filtered_data);
        ReportViewer viewer = new ReportViewer();
        viewer.ProcessingMode = ProcessingMode.Local;
        viewer.LocalReport.ReportPath = Server.MapPath("~/exports/templates/detailed_monthly_report.rdlc");

        #region  Code for showing logo
        HttpContext context = Context;
        string imageUrl = context.Server.MapPath("~/uploads/CompanyLogo/");
        DBConnection db_connection = new DBConnection();

        CompanyLogoStuff companyLogoStuff = new CompanyLogoStuff();
        string companyCode = companyLogoStuff.getCompanyImageUrl(companyname);
        imageUrl = imageUrl + companyCode;
        viewer.LocalReport.EnableExternalImages = true;

        ReportParameter[] params1 = new ReportParameter[1];
        params1[0] = new ReportParameter("image_path", "file:///" + imageUrl, false);
        viewer.LocalReport.SetParameters(params1); 
        #endregion
        viewer.LocalReport.DataSources.Add(rds);
        bytes = viewer.LocalReport.Render(FileExtension, null, out mimeType, out encoding, out extension, out streamids, out warnings);

        using (FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("~/exports/data/").ToString() + file_name + extension, FileMode.Create))
        {
            fs.Write(bytes, 0, bytes.Length);
        }

        return file_name + extension;
    }

    [WebMethod]
    public static ReturnObject DoExport(string filters)
    {

        monthly_detailed_report page_object = new monthly_detailed_report(); // instance of the page to access non static methods.
        ReturnObject return_object = new ReturnObject();
        DataTable filtered_data = new DataTable();
        string file_name, company_name = string.Empty;
        JObject filters_data = new JObject();
        filters_data = JObject.Parse(filters);
        company_name = filters_data["company_name"].ToString();

        try
        {

            filtered_data = page_object.PrepareDataForExport(filters);

            if (filtered_data.Rows.Count > 0)
            {

                file_name = page_object.CreateExport(filtered_data, company_name);

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
            page_object.Dispose(); //Disposing of the page object to avoid memory leak.
        }

        return return_object;
    }
}