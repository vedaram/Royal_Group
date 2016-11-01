using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Newtonsoft.Json;
using SecurAX.Logger;
using System.Configuration;
using SecurAX.Export.Excel;

public partial class leave_available : System.Web.UI.Page
{
    const string page = "LEAVE_AVAILABLE";

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

            message = "An error occurred while loading Leave Available page. Please try again. If the error persists, please contact Support.";

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

    private string GetBaseQuery()
    {
        string query = string.Empty;

        query = @"select Emp_code as Emp_Code, Emp_Name, LeaveName, Max_leaves, Leaves_applied, Leave_balance, row FROM (
                select e.Emp_code as Emp_Code, em.Emp_Name, l.LeaveName, e.Max_leaves, e.Leaves_applied, e.Leave_balance, ROW_NUMBER()
                OVER (ORDER BY e.Emp_code) as row from LeaveMaster l, Employee_Leave e, EmployeeMaster em 
                where l.LeaveCode=e.Leave_code and e.Emp_Code = em.emp_code and l.LeaveCode not in('V','OD') and e.Emp_Status='1' ";

        return query;
    }

    [WebMethod]
    public static ReturnObject GetLeavesAvailable(int page_number)
    {
        leave_available page_object = new leave_available();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable employees_working_under_user = new Hashtable();

        DataTable available_leaves = new DataTable();
        DataTable CoManagerID_data = new DataTable();
        DataTable branch_list_table = new DataTable();

        string
           user_name = string.Empty,
           employee_id = string.Empty,
           query = string.Empty,
           CoManagerID = string.Empty,
           BranchList = "'Empty',",
           branchqry = string.Empty;

        int
            user_access_level = 0, IsDelegationManager = 0,
            start_row = 0, number_of_rows = 0;


        try
        {
            start_row = (page_number - 1) * 30;
            number_of_rows = page_number * 30 + 1;

            query = page_object.GetBaseQuery();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            employee_id = HttpContext.Current.Session["employee_id"].ToString();

            //check IsDelegationManager count
            IsDelegationManager = db_connection.GetRecordCount("Select COUNT(DelidationManagerID) from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)");

            if (IsDelegationManager > 0)
            {
                CoManagerID_data = db_connection.ReturnDataTable("Select ManagerId from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)");
                if (CoManagerID_data.Rows.Count > 0)
                {
                    foreach (DataRow dr in CoManagerID_data.Rows)
                    {
                        CoManagerID += "'" + dr["ManagerId"] + "',";
                    }

                    CoManagerID = CoManagerID.TrimEnd(',');
                }
            }

            //To get list of managers under logged in manager for two level approval
            string InnerManagers = "''";
            DataTable dtinnermanager = db_connection.ReturnDataTable("Select Emp_Code From EmployeeMaster Where ManagerID='" + employee_id + "' And Ismanager=1");
            if (dtinnermanager.Rows.Count > 0)
            {
                foreach (DataRow dr in dtinnermanager.Rows)
                {
                    InnerManagers += ",'" + dr["Emp_Code"] + "'";
                }
                InnerManagers = InnerManagers.TrimEnd(',');
            }

            //get list of branches assigned to logged in manager hr
            BranchList = "'Empty',";
            branchqry = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + employee_id + "'";
            branch_list_table = db_connection.ReturnDataTable(branchqry);

            //make list of Branchs
            if (branch_list_table.Rows.Count > 0)
            {
                foreach (DataRow dr in branch_list_table.Rows)
                {
                    BranchList += "'" + dr["BranchCode"] + "',";
                }
                BranchList = BranchList.TrimEnd(',');
            }

            //check CoManagerID 
            if (string.IsNullOrEmpty(CoManagerID))
            {
                CoManagerID = "'Empty'";
            }

            //change query based on user_access_level

            Hashtable hsh1 = new Hashtable();
            hsh1.Add("userid", employee_id);
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("sp_fetchemployee1", hsh1);

            if (user_access_level == 0)
            {
                query = "select *from (select e.Emp_code as Emp_Code,em.Emp_Name Emp_Name,LeaveName,Max_leaves,Leaves_applied,Leave_balance,row_number() over (order by e.emp_code) as row from Employee_Leave e join LeaveMaster l on l.LeaveCode=e.Leave_code join employeemaster em on em.emp_code=e.Emp_code where l.LeaveCode not in('V','OD') ";

            }
            else if (user_access_level == 1 || user_access_level == 3)
            {
                query = "select *from (select e.Emp_code as Emp_Code,em.Emp_Name Emp_Name,LeaveName,Max_leaves,Leaves_applied,Leave_balance,row_number() over (order by e.emp_code) as row from Employee_Leave e join LeaveMaster l on l.LeaveCode=e.Leave_code join EmployeeMaster em on em.Emp_Code=e.emp_code WHERE E.Emp_code in(select Empid from employees) and l.LeaveCode not in('V','OD')";

            }
            else
            {
                query = "select *from (select e.Emp_code as Emp_Code ,em.Emp_Name Emp_Name,LeaveName,Max_leaves,Leaves_applied,Leave_balance,row_number() over (order by e.emp_code) as row from Employee_Leave e join LeaveMaster l on l.LeaveCode=e.Leave_code join EmployeeMaster em on em.Emp_Code=e.Emp_code WHERE E.Emp_code='" + employee_id + "' and l.LeaveCode not in('V','OD')";

            }

            //if (user_access_level == 0)
            //{
            //    query += " and e.Emp_Code !='" + employee_id + "'";
            //}
            //else if (user_access_level == 3)
            //{
            //    query += " And e.Emp_Branch In(" + BranchList + ") ";
            //}
            //else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")
            //{
            //    query += " and (E.ManagerID In('" + employee_id + "'," + CoManagerID + ") and L.Approvallevel in (0,2)) Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + CoManagerID + "," + InnerManagers + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
            //    query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1)";
            //}
            //else if (user_access_level == 1 && CoManagerID == "'Empty'")
            //{
            //    query += " And (E.ManagerID In('" + employee_id + "') and L.Approvallevel in (0,2)) Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + InnerManagers + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
            //    query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1)";
            //}
            //else
            //{
            //    query += " and 1=0 ";
            //}   

            query += " ) a where row > " + start_row + " and row < " + number_of_rows + " order by a.Emp_code";

            available_leaves = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(available_leaves, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_LEAVE_DETAILS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leave Available data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    private string CreateExport(DataTable company_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Employee Code", "Employee Name", "Leave Name", "Max Leaves", "Leaves Applied", "Balance Leave" };

        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "LeaveAvailable-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "LEAVES AVAILABLE", company_data, Context, column_names);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport()
    {
        leave_available page_object = new leave_available();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        DateTime now = DateTime.Now;
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);
        string
            user_id = HttpContext.Current.Session["employee_id"].ToString();

        string[] column_names = new string[] { };

        string
            query = string.Empty, file_name = string.Empty;

        try
        {
            query = "select *from (select e.Emp_code as Emp_Code,em.Emp_Name Emp_Name,LeaveName,Max_leaves,Leaves_applied,Leave_balance from Employee_Leave e join LeaveMaster l on l.LeaveCode=e.Leave_code join EmployeeMaster em on em.Emp_Code=e.emp_code WHERE E.Emp_code in(select Empid from FetchEmployees('" + user_id + "','')) and l.LeaveCode not in('V','OD'))a order by a.Emp_Code";
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
