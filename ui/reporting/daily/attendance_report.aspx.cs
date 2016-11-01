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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;
using SecurAX.Export.Excel;

public partial class daily_attendance_report : System.Web.UI.Page
{

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
            Logger.LogException(ex, "DAILY_ATTENDANCE_STATUS_REPORT", "PAGE_LOAD");

            message = "An error occurred while loading Daily Attendance Status Report. Please try again. If the error persists, please contact Support.";

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
            Logger.LogException(ex, "DAILY_ATTENDANCE_STATUS_REPORT", "GET_COMPANY_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occured while loading Company Data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataSet return_data_set = new DataSet();
        DataTable temp_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DISTINCT BranchCode as branch_code, BranchName as branch_name from BranchMaster where CompanyCode = '" + company_code + "' order by BranchName";
            temp_data = db_connection.ReturnDataTable(query);
            temp_data.TableName = "branch";
            return_data_set.Tables.Add(temp_data);

            query = "select DeptName as department_name from DeptMaster where CompanyCode = '" + company_code + "' order by DeptName";
            temp_data = db_connection.ReturnDataTable(query);
            temp_data.TableName = "department";
            return_data_set.Tables.Add(temp_data);

            query = "select distinct Shift_Desc as shift_name from Shift where CompanyCode = '" + company_code + "' and IsActive=1 order by Shift_Desc";
            temp_data = db_connection.ReturnDataTable(query);
            temp_data.TableName = "shift";
            return_data_set.Tables.Add(temp_data);

            query = "select  DISTINCT EmpCategoryName as category_name from EmployeeCategoryMaster where CompanyCode='" + company_code + "' order by EmpCategoryName";
            temp_data = db_connection.ReturnDataTable(query);
            temp_data.TableName = "category";
            return_data_set.Tables.Add(temp_data);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, "DAILY_ATTENDANCE_STATUS_REPORT", "GET_COMPANY_RELATED_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occured while loading data for the selected company. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private string GetAttendanceStatus(string status)
    {
        string attendance_status = string.Empty;

        switch (status)
        {
            case "Absent":
                attendance_status = "A";
                break;
            case "Present":
                attendance_status = "P";
                break;
            case "OnLeave":
                attendance_status = "L";
                break;
            default:
                attendance_status = "";
                break;
        }

        return attendance_status;
    }

   

    private string GetQueryBasedOnAccessLevel(int access_level, string employee_id)
    {
        string query = string.Empty;

        switch (access_level)
        {
            case 1:
                query = " and emp_id in (select distinct(Emp_Code) from EmployeeMaster where (managerId = '" + employee_id + "' or Emp_Code = '" + employee_id + "') and Emp_Status = 1 )";
                break;
            case 0:
                query = " and emp_id in (select distinct(Emp_Code) from EmployeeMaster where Emp_Status = 1)";
                break;
            default:
                query = " and emp_id ='" + employee_id + "'";
                break;
        }

        return query;
    }

    private void PrepareDataForExport(string filters, string branches)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable filter_conditions = new Hashtable();
        JObject filters_data = new JObject();
        JArray branch_data = new JArray();

        int user_access_level = 0, i = 0;

        string
            from_date, to_date, status, user_name,
            company_name, shift_name, department_name,category_name , 
            employee_status , current_user_id, where_clause = string.Empty,
            branch_list = string.Empty;

        try
        {
            // Storing Session data for later use
            user_name = Session["username"].ToString();
            user_access_level = Convert.ToInt32(Session["access_level"]);
            current_user_id = Session["employee_id"].ToString();

            // Checking for admin user
            if (user_name != "admin")
                user_name = Session["employee_id"].ToString();

            // Parsing filters JSON object
            filters_data = JObject.Parse(filters);
            company_name = filters_data["company_name"].ToString();
            department_name = filters_data["department"].ToString();
            shift_name = filters_data["shift"].ToString();
            category_name = filters_data["category"].ToString();
            employee_status = filters_data["employeestatus"].ToString();

            from_date = DateTime.ParseExact(filters_data["from_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            to_date = DateTime.ParseExact(filters_data["to_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            // process branch IDs
            branch_data = JArray.Parse(branches);
            for (i = 0; i < branch_data.Count; i++)
            {
                branch_list += "'" + branch_data[i].ToString() + "',";
            }

            where_clause += " where pdate between '" + from_date + "' and '" + to_date + "' ";

            status = GetAttendanceStatus(filters_data["status"].ToString());
            if (status != "")
            {
                where_clause = where_clause + " and Status = '" + status + "'";
            }
            // Modifying the query based on the user access level
            where_clause += " and emp_id in (select EmpID from [FetchEmployees] ('" + current_user_id + "','" + employee_status  + "'))";

            if (company_name != "select")
            {
                //  where_clause = where_clause + " and Comp_Name = '" + company_name + "'";    // comeneted because  employee of diferent company should come in this report

                if (!string.IsNullOrEmpty(branch_list))
                {
                    branch_list = branch_list.Remove(branch_list.Length - 1, 1);
                    where_clause = where_clause + " and Cat_Name in (" + branch_list + ")";
                }

                if (department_name != "select")
                    where_clause = where_clause + " and Dept_Name='" + department_name + "'";

                if (shift_name != "select")
                    where_clause = where_clause + " and Shift_Name='" + shift_name + "'";
                if (category_name != "select")
                    where_clause = where_clause + " and EmployeeCategory='" + category_name + "'";
            }

            filter_conditions.Add("where", where_clause);
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("DailyAttendanceReport1", filter_conditions);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DAILY_ATTENDANCE_STATUS_REPORT", "PREPARE_DATA_FOR_EXPORT");
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

        query = "select TOP " + export_limit + " * from presentabsent";
        filtered_data = db_connection.ReturnDataTable(query);

        return filtered_data;
    }

    private string CreateExport(DataTable filtered_data, string company_name)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Employee ID", "Employee Name", "Punch Date", "In Time", "Out Time", "Status", "Company Name", "Branch Name", "Department Name", "Category Name", "Shift Name" };

        //string
        //    user_id = HttpContext.Current.Session["employee_id"].ToString(),
        //    file_name = "DailyAttendanceReport-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        string importFile,
           user_id = HttpContext.Current.Session["employee_id"].ToString(),
           exportPath = "DailyAttendanceReport-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
        importFile = "DailyAttendanceReport_Template" + ".xlsx";

        string importFilePath = HttpContext.Current.Server.MapPath("~/exports/templates/" + importFile);
        ExcelExport.ExportDataToExcelWithLogo(importFilePath, "DAILY ATTENDANCE REPORT", filtered_data, Context, column_names, company_name, exportPath);


        //ExcelExport.ExportDataToExcel(exportPath, "DAILY ATTENDANCE REPORT", filtered_data, Context, column_names, company_name);

        return exportPath ;
    }

    [WebMethod]
    public static ReturnObject DoExport(string filters, string branches)
    {

        daily_attendance_report page_object = new daily_attendance_report();
        ReturnObject return_object = new ReturnObject();
        DataTable filtered_data = new DataTable();
        string file_name, company_name, exportPath = string.Empty;
        JObject filters_data = new JObject();
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
            Logger.LogException(ex, "DAILY_ATTENDANCE_STATUS_REPORT", "GET_DATA_FOR_EXPORT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while exporting the data. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }
}