using System;
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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Logger;


public partial class overtime_details : System.Web.UI.Page
{
    const string page = "OVERTIME_DETAILS";

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

            message = "An error occurred while loading Overtime Details page. Please try again. If the error persists, please contact Support.";

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

    protected string GetFilterQuery(string filtersData, string query)
    {
        JObject filter_data = new JObject();
        int filter_by = 0;
        string filter_keyword = string.Empty;
        int filter_LeaveStatus = 0;
        string filter_hours = string.Empty;
        string filter_date = string.Empty;

        try
        {
            filter_data = JObject.Parse(filtersData);
            filter_by = Convert.ToInt32(filter_data["filter_by"]);
            filter_keyword = filter_data["filter_keyword"].ToString();
            filter_LeaveStatus = Convert.ToInt32(filter_data["filter_LeaveStatus"]);
            filter_date = filter_data["filter_date"].ToString();
            filter_hours = filter_data["filter_hours"].ToString();

            if (filter_by == 1)
            {
                query += " and e.Emp_Code = '" + filter_keyword + "' ";
            }
            else if (filter_by == 2)
            {
                query += " and e.Emp_Name like '%" + filter_keyword + "%' ";
            }

            if (filter_LeaveStatus != 0)
            {
                query += " and O.Flag = '" + filter_LeaveStatus + "' ";
            }

            if (filter_date != "")
            {
                query += " and O.OTDate = '" + filter_date + "' ";
            }

            if (filter_hours != "")
            {
                query += " and O.OtHrs >= '" + filter_hours + "' ";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_FILTER_QUERY");
        }

        return query;
    }

    private string GetBaseQuery()
    {
        string query = @"select Overtimeid, OTDate, EmpID, Emp_Name, OtHrs, Status, Approval, modifiedby,row from
                        ( SELECT O.Overtimeid, O.OTDate, O.OTDate[OT Date], O.EMPID as 'EmpID', E.Emp_Name  as 'Emp_Name', O.OtHrs, case when o.Flag=1
                        and o.MFlag =1 then 'Submitted' when o.Flag=1 and o.MFlag =2 then 'Approved by fist manager' when o.Flag=3 and o.MFlag =3 then
                        'Declined' when o.Flag=2 and o.MFlag =2 then 'Approved' end  as Status,O.Flag as Approval, O.modifiedby,
                       ROW_NUMBER() OVER (ORDER BY O.Overtimeid desc) as row From Overtime O JOIN EmployeeMaster E on E.Emp_Code=O.Empid join leave_status Ls on
						  Ls.Leave_Status_id = o.MFlag where 1=1 ";
        return query;
    }

    [WebMethod]
    public static ReturnObject getOvertimeData(int page_number, bool is_filter, string filters)
    {
        overtime_details page_object = new overtime_details();

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable overtime_data = new DataTable();
        DataTable branch_list_table = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string
           user_name = string.Empty,
           employee_id = string.Empty,
           query = string.Empty,
           status_line = string.Empty,
           CoManagerID = string.Empty,
           BranchList = string.Empty,
           branchqry = string.Empty;

        int
            start_row = 0, number_of_record = 0,
            user_access_level = 0,
            IsDelegationManager = 0;

        try
        {
            // getting session data for later use in the function.
            user_name = HttpContext.Current.Session["username"].ToString();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            employee_id = HttpContext.Current.Session["employee_id"].ToString();

            // Setting the values for pagination
            start_row = (page_number - 1) * 30;
            number_of_record = page_number * 30 + 1;

            query = page_object.GetBaseQuery();

            //check IsDelegationManager count
            IsDelegationManager = db_connection.GetRecordCount("Select COUNT(DelidationManagerID) from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)");

            if (IsDelegationManager > 0)
            {
                query = "Select ManagerId from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)";
                CoManagerID_data = db_connection.ReturnDataTable(query);
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
            else
            {
                BranchList = "'Empty'";
            }

            //check CoManagerID 
            if (string.IsNullOrEmpty(CoManagerID))
            {
                CoManagerID = "'Empty'";
            }

            if (is_filter)
            {
                JObject filter_data = new JObject();

                filter_data = JObject.Parse(filters);
                int filter_by = Convert.ToInt32(filter_data["filter_by"]);
                string filter_keyword = filter_data["filter_keyword"].ToString();
                int filter_LeaveStatus = Convert.ToInt32(filter_data["filter_LeaveStatus"]);
                string filter_date = filter_data["filter_date"].ToString();
                string filter_hours = filter_data["filter_hours"].ToString();

                query = page_object.GetBaseQuery();

                if (filter_LeaveStatus != 0)
                {                    
                    status_line = "(o.Flag=" + filter_LeaveStatus + " and o.mFlag=" + filter_LeaveStatus + ")";                   

                    if (user_access_level == 0)//Admin
                    {
                        query += " and (o.Flag=" + filter_LeaveStatus + " and o.mFlag=" + filter_LeaveStatus + ") ";
                    }
                    else if (user_access_level == 3)//HR
                    {
                        query += " and " + status_line + " and O.EmpID in ( select Emp_Code from EmployeeMaster where Emp_Branch in (" + BranchList + ")  or O.EmpID='" + employee_id + "' or ManagerID='" + employee_id + "' or ManagerID is null or ManagerID ='' ) ";
                    }
                    else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
                    {
                        query += "and O.EmpID in (select emp_code from employeemaster where ManagerId In ('" + employee_id + "'," + CoManagerID + ") or Emp_Code='" + employee_id + "' and  " + status_line + " ) or (O.EMPID In(Select Emp_Code From EmployeeMaster Where Emp_Code Not In ('" + employee_id + "') And ManagerID In(" + InnerManagers + ") and ManagerID!='' and (o.Flag=" + filter_LeaveStatus + " and o.MFlag =" + filter_LeaveStatus + "))  Or e.Emp_Branch In (" + BranchList + ")) ";
                    }
                    else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager 
                    {
                        //query += " and O.EmpID in (select Emp_Code from EmployeeMaster where ManagerId In ('" + employee_id + "') and (o.Flag='1' and o.MFlag =" + filter_LeaveStatus + ")) or (O.EmpID In(Select Emp_Code From EmployeeMaster Where Emp_Code Not In('" + employee_id + "') And ManagerID In(" + InnerManagers + ") and (o.Flag=" + filter_LeaveStatus + " or o.MFlag =" + filter_LeaveStatus + "))   Or (e.Emp_Branch In(" + BranchList + ") and o.MFlag=" + filter_LeaveStatus + ")) ";
                        query += " and (O.EmpID in (select Emp_Code from EmployeeMaster where ManagerID In ('" + employee_id + "') or Emp_Code='" + employee_id + "') and " + status_line + ")  OR 	(O.EmpID In(Select Emp_Code From EmployeeMaster Where ManagerID In (" + InnerManagers + ")  and ManagerID!='' and Emp_Code!='" + employee_id + "') and  (o.Flag=" + filter_LeaveStatus + " and o.mFlag=" + filter_LeaveStatus + ")) OR e.Emp_Branch In('Empty')";
                    }
                    else
                    {
                        query += " O.EmpID='" + HttpContext.Current.Session["employee_id"] + "' ";// Only Employee
                    }
                }
                else
                {
                    if (user_access_level == 0)//Admin
                    {
                        query += " ";
                    }
                    else if (user_access_level == 3)//HR
                    {
                        query += " and  and O.EmpID in ( select Emp_Code from EmployeeMaster where Emp_Branch in (" + BranchList + ")  or O.EmpID='" + employee_id + "' or ManagerID='" + employee_id + "' or ManagerID is null or ManagerID ='' ) ";
                    }
                    else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
                    {
                        query += "and O.EmpID in (select emp_code from employeemaster where ManagerId In ('" + employee_id + "'," + CoManagerID + ") or Emp_Code='" + employee_id + "' ) or (O.EMPID In(Select Emp_Code From EmployeeMaster Where Emp_Code Not In ('" + employee_id + "') And ManagerID In(" + InnerManagers + "))  Or e.Emp_Branch In(" + BranchList + ")) ";
                    }
                    else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager 
                    {
                        query += " and O.EmpID in (select Emp_Code from EmployeeMaster where ManagerId In ('" + employee_id + "') or Emp_Code='" + employee_id + "' ) or (O.EmpID In(Select Emp_Code From EmployeeMaster Where Emp_Code Not In('" + employee_id + "') And ManagerID In(" + InnerManagers + ") )   Or (e.Emp_Branch In(" + BranchList + " )and o.MFlag =1)) ";

                    }
                    else
                    {
                        query += " O.EmpID='" + HttpContext.Current.Session["employee_id"] + "' ";// Only Employee
                    }
                }

                query += " ) a where row > " + start_row + " and row < " + number_of_record;

                if (filter_by == 1)
                {
                    query += " and a.EmpID= '" + filter_keyword + "' ";
                }
                else if (filter_by == 2)
                {
                    query += " and a.Emp_Name like '%" + filter_keyword + "%' ";
                }

                if (filter_date != "")
                {
                    query += " and a.OTDate = '" + filter_date + "' ";
                }

                if (filter_hours != "")
                {
                    query += " and a.OtHrs >= '" + filter_hours + "' ";
                }
            }
            else
            {
                query = page_object.GetBaseQuery();

                //modify query as per access level
                if (user_access_level == 0)//Admin
                {
                    query += " ";
                }
                else if (user_access_level == 3)//HR
                {
                    query += " and O.EmpID in ( select Emp_Code from EmployeeMaster where Emp_Branch in (" + BranchList + ")  or O.EmpID='" + employee_id + "' or ManagerID='" + employee_id + "' or ManagerID is null or ManagerID ='' ) ";
                }
                else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
                {
                    query += "and O.EmpID in (select emp_code from employeemaster where ManagerId In ('" + employee_id + "'," + CoManagerID + ") or Emp_Code='" + employee_id + "') or (O.EMPID In (Select Emp_Code From EmployeeMaster Where Emp_Code Not In ('" + employee_id + "') And ManagerID In(" + InnerManagers + "))  Or e.Emp_Branch In(" + BranchList + ")) ";
                }
                else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager 
                {
                    query += " and O.EmpID in (select Emp_Code from EmployeeMaster where ManagerId In ('" + employee_id + "') or Emp_Code='" + employee_id + "') or (O.EmpID In (Select Emp_Code From EmployeeMaster Where Emp_Code Not In('" + employee_id + "') And ManagerID In (" + InnerManagers + "))   Or e.Emp_Branch In(" + BranchList + ")) ";
                }
                else
                {
                    query += " O.EmpID='" + HttpContext.Current.Session["employee_id"] + "' ";// Only Employee
                }

                query += " ) a where row > " + start_row + " and row < " + number_of_record;
            }

            query += " order by a.OTDate desc";

            overtime_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(overtime_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OVERTIME_DETAILS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    protected void PrintExcel(DataTable table)
    {
        try
        {
            string attach = "attachment;filename=OvertimeApproval.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attach);
            Response.ContentType = "application/ms-excel";
            if (table != null)
            {
                foreach (DataColumn dc in table.Columns)
                {
                    Response.Write(dc.ColumnName + "\t");

                }
                Response.Write(System.Environment.NewLine);
                foreach (DataRow dr in table.Rows)
                {
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Response.Write(dr[i].ToString() + "\t");
                    }
                    Response.Write("\n");
                }
                Response.End();
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "PRINT_EXCEL");
        }
    }

    protected void ExportToExcel(object sender, EventArgs e)
    {
        DBConnection db_connection = new DBConnection();
        DataTable overtime_data_table = new DataTable();
        int user_access_level = 0;
        string employee_id = string.Empty;
        string query = string.Empty;

        try
        {
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            employee_id = HttpContext.Current.Session["Employee_id"].ToString();

            if (user_access_level == 0)
            {
                query = "SELECT o.Overtimeid, o.OTDate, o.EMPID, e.Emp_Name, o.OtHrs, ls.Leave_Status_text, o.Flag, o.modifiedby from Overtime o, EmployeeMaster e, leave_status ls where e.Emp_Code = o.Empid and ls.Leave_Status_id = o.Flag and ls.Leave_Status_text = 'Submitted' order by o.Overtimeid";
            }
            else
            {
                query = "SELECT o.Overtimeid, o.OTDate, o.EMPID, e.Emp_Name, o.OtHrs, ls.Leave_Status_text, o.Flag, o.modifiedby from Overtime o, EmployeeMaster e, leave_status ls where e.Emp_Code = o.Empid and ls.Leave_Status_id = o.Flag and ls.Leave_Status_text = 'Submitted' and e.Emp_ID in(select empid in employees where empid!='" + employee_id + "') order by o.Overtimeid";
            }

            overtime_data_table = db_connection.ReturnDataTable(query);
            PrintExcel(overtime_data_table);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "EXPORT_TO_EXCEL");
        }
    }

}
