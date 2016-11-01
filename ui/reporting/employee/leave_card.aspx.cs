using System;
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
using System.Data.SqlClient;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Export.Excel;
using SecurAX.Logger;

public partial class employee_leave_card : System.Web.UI.Page
{

    const string page = "EMPLOYEE_LEAVE_CARD";

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

            query = "select distinct designame as designation_name from desigmaster where CompanyCode='" + company_code + "' order by DesigName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "designation";
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
    private void PrepareDataForExport(string filters)
        {
        DBConnection db_connection = new DBConnection();
        Hashtable filter_conditions = new Hashtable();
        JObject filters_data = new JObject();
        JArray branch_data = new JArray();

        string
             user_name,
            employee_name, employee_id, current_user_id,
            where_clause = string.Empty, company_name, employee_status, designation_name,
            branch_name , department_name, category_name = string.Empty;

        int
            user_access_level = 0;

        filters_data = JObject.Parse(filters);
        company_name = filters_data["company_name"].ToString();
        branch_name = filters_data["branch"].ToString();
        department_name = filters_data["department"].ToString();
        designation_name = filters_data["designation"].ToString();
        category_name = filters_data["category"].ToString();
        employee_status = filters_data["status"].ToString();
        employee_id = filters_data["employee_id"].ToString();
        employee_name = filters_data["employee_name"].ToString();

        // Get current logged in user data from the Session variable
        user_name = HttpContext.Current.Session["username"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        current_user_id = HttpContext.Current.Session["employee_id"].ToString();

        if (company_name != "select")
        {

            where_clause += "where";

            if (employee_id == "")
            {
                where_clause += "   EmpID in (select EmpID from [FetchEmployees] ('" + current_user_id + "','" + employee_status + "'))";

                if (branch_name != "select")
                    where_clause = where_clause + " and BranchName = '" + branch_name + "'";

                if (department_name != "select")
                    where_clause = where_clause + " and DeptName = '" + department_name + "'";

                if (category_name != "select")
                    where_clause = where_clause + " and EmployeeCategory='" + category_name + "'";

                if (employee_name != "")
                    where_clause = where_clause + " and EmpName like '%" + employee_name + "%'";
            }

            else
            {

                where_clause += "  EmpID = '" + employee_id + "'";

                if (branch_name != "select")
                    where_clause = where_clause + " and BranchName = '" + branch_name + "'";

                if (department_name != "select")
                    where_clause = where_clause + " and DeptName = '" + department_name + "'";


                if (category_name != "select")
                    where_clause = where_clause + " and EmployeeCategory='" + category_name + "'";

                if (employee_name != "")
                    where_clause = where_clause + " and EmpName like '%" + employee_name + "%'";


            }
            #region Commented for  different branch.
            //where_clause += " where  CompName = '" + company_name + "'";


            //if (branch_name != "select")
            //    where_clause = where_clause + " and BranchName = '" + branch_name + "'";

            //if (department_name != "select")
            //    where_clause = where_clause + " and DeptName = '" + department_name + "'";


            // if (category_name != "select")
            //     where_clause = where_clause + " and EmployeeCategory='" + category_name + "'";

            //// Checking to see if an employee ID has been selected in the filters.
            //if (employee_id != "") // If YES then we pull results only for the selected employee ID.
            //    where_clause = where_clause + " and EmpID = '" + employee_id + "'";
            //else
            //{
            //    where_clause += " and EmpID in (select EmpID from [FetchEmployees] ('" + current_user_id + "','" + employee_status + "'))";
            //}
            //if (employee_name != "")
            //    where_clause = where_clause + " and EmpName like '%" + employee_name + "%'";
            
            #endregion
            filter_conditions.Add("where", where_clause);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("sp_LeaveCard", filter_conditions);
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

        query = "select TOP " + export_limit + " empid ,  empname ,  branchname, deptname,EmployeeCategory ,   leavetype, maxleave, leaveconsumed , leavebl from TempLeaveCard";
        filtered_data = db_connection.ReturnDataTable(query);

        return filtered_data;
    }
    private string CreateExport(DataTable filtered_data, string company_name)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Employee ID", "Employee Name" ,  "Branch", "Department", "Category", "LeaveType", "MaxLeave", "Consumed", "Balance" };

        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "EmploeeLeaveCardReport-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "EMPLOYEE LEAVE CARD", filtered_data, Context, column_names, company_name);

        return file_name;
    }
    [WebMethod]
    public static ReturnObject DoExport(string filters)
    {

        employee_leave_card page_object = new employee_leave_card(); // instance of the page to access non static methods.
        ReturnObject return_object = new ReturnObject();
        DataTable filtered_data = new DataTable();
        JObject filters_data = new JObject();
        string file_name, company_name, exportPath = string.Empty;
        filters_data = JObject.Parse(filters);
        company_name = filters_data["company_name"].ToString();
        try
        {

            page_object.PrepareDataForExport(filters);
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
