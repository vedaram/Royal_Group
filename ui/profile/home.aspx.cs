using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Globalization;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class profile_home : System.Web.UI.Page
{
    const string page = "HOME_PAGE";

    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/Logout.aspx", true);
            }

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "PAGE_LOAD");

            string message = "An error occurred while performing this operation. Please try again. If the issue persists, please contact Support.";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("showAlert('error','");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    [WebMethod]
    public static ReturnObject GetEmployeeStrength()
    {

        // Declaring variables for this function.
        DBConnection db_connection = new DBConnection();
        Dictionary<string, string> return_data = new Dictionary<string, string>();
        ReturnObject return_object = new ReturnObject();

        string
            current_date = "",
            present_query = "",
            employee_count_query = "";

        int
            present_count = 0,
            absent_count = 0,
            employee_count = 0;

        try
        {

            current_date = DateTime.Now.ToString("yyyy-MM-dd");

            //present_query = "select count(distinct(EmpID)) from Trans_raw#_all where PunchDate='" + current_date + "' ";
            present_query = "select count(Status) as Present from masterprocessdailydata where pdate = '" + current_date + "' and status not in ('A', 'L', 'AHL', 'WO', 'H', 'LWP')";
            employee_count_query = "select count(distinct(Emp_Code)) from employeemaster where Emp_Status='1' ";

            present_count = db_connection.GetRecordCount(present_query);
            employee_count = db_connection.GetRecordCount(employee_count_query);

            absent_count = employee_count - present_count;

            // Temporary data structure to hold the data. This will be serialized to JSON as part of the Return Object.
            return_data.Add("present", present_count.ToString());
            return_data.Add("absent", absent_count.ToString());

            // Preparing the return object
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_EMPLOYEE_STRENGTH");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetCalendarData(string today)
    {

        DBConnection db_connection = new DBConnection();
        DataTable calendar_data = new DataTable();
        ReturnObject return_object = new ReturnObject();

        string employee_id = "",
        query = "";

        try
        {

            employee_id = HttpContext.Current.Session["employee_id"].ToString();

            DateTime current_date = DateTime.ParseExact(today, "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
            DateTime first_day = new DateTime(current_date.Year, current_date.Month, 1);
            DateTime last_day = first_day.AddMonths(1).AddDays(-1);

            query = "select emp_name, dept_name, desig_name, status, pdate, Convert(varchar(5),in_punch,108) in_punch, Convert(varchar(5),out_punch,108) out_punch, WorkHrs from masterprocessdailydata where pdate between '" + first_day.ToString("yyyy-MM-dd") + "' and '" + last_day.ToString("yyyy-MM-dd") + "' and pdate is not null and emp_id='" + employee_id + "' order by pdate";

            calendar_data = db_connection.ReturnDataTable(query);

            // Preparing the return object
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(calendar_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_CALENDAR_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetLeaveDetails()
    {

        // Declaring variables for this function.
        DBConnection db_connection = new DBConnection();
        DataSet leave_data_set = new DataSet();
        DataTable temp_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();

        string query = "",
        employee_id = "";

        try
        {

            DateTime current_date = DateTime.Now;

            employee_id = HttpContext.Current.Session["employee_id"].ToString();

            query = "select holname HolidayName, Convert(Date,holfrom,103) HolidayDate from holidaymaster h join companymaster c on c.CompanyCode=h.companycode where holfrom >='" + current_date.ToString("yyyy-MM-dd") + "' and h.holgrpcode=(select holidaycode from branchmaster where branchcode=(select emp_branch from employeemaster where emp_code='" + employee_id + "')) order by Convert(Date,holfrom,103)";

            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "leaves_listing";
            leave_data_set.Tables.Add(temp_data_table);

            //query = "select l.leavename as Leavetype,e.Leaves_applied,e.Leave_balance from Employee_Leave e join LeaveMaster l on e.Leave_code=l.LeaveCode where emp_code='" + employee_id + "'";

            //query = "select  LM.leavename as Leavetype,El.Leaves_applied, EL.Leave_balance from Employee_Leave EL join LeaveMaster LM on";
            query = "select  LM.leavename as Leavetype,El.Leaves_applied, EL.Leave_balance  , coalesce(el.CarryForwardLeave,0) as CarryForwardLeave  from Employee_Leave EL join LeaveMaster LM on";
            query += "  EL.Leave_code=LM.LeaveCode and EL.Emp_code='" + employee_id + "' where (LeaveCode not in ('OD','V') and LM.EmployeeCategoryCode= ";
            query += " (Select Emp_Employee_Category from EmployeeMaster where Emp_code='" + employee_id + "')  ) or LeaveCode = 'CO'";

            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "leaves_balance";
            leave_data_set.Tables.Add(temp_data_table);

            // preparing the return object
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leave_data_set, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_LEAVE_DETAILS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }


    /********************** New function add to count L* ***********************/
    [WebMethod]
    public static string GetLStarCount(string year)
    {
        Hashtable Lstartable = new Hashtable();
        // Declaring variables for this function.
        DBConnection db_connection = new DBConnection();
        Dictionary<string, string> return_data = new Dictionary<string, string>();
        ReturnObject return_object = new ReturnObject();

        string empCode = HttpContext.Current.Session["employee_id"].ToString();

        Lstartable.Add("year", year);
        Lstartable.Add("empid", empCode);
        //Lstartable.Add("LStartCounts", 0);

        //exeStoredProcedure_WithHashtable_ReturnRow
        double lStarCount = 0.0;
        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("getStarLeaveBalance", Lstartable);
        string query = "select LCount from LCount_table where Year=" + year + " and EmpID=" + empCode;
        lStarCount = db_connection.ExecuteQuery_WithReturnValueDouble(query);

        return "" + lStarCount;
    }

}
