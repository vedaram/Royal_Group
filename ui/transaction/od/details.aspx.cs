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
using System.Globalization;
using SecurTime;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class od_details : System.Web.UI.Page
{
    const string page = "OD_DETAILS";

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

            message = "An error occurred while loading OD Leave Approval page. Please try again. If the error persists, please contact Support.";

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

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        string employee_id = HttpContext.Current.Session["employee_id"].ToString();
        string company_query = string.Empty;

        try
        {
            if (employee_id == "")
                company_query = "select distinct CompanyName, CompanyCode from companymaster";
            else
                company_query = "select CompanyCode, CompanyName from companymaster where companycode = (select emp_company from employeemaster where emp_Code = '" + employee_id + "')";

            company_data = db_connection.ReturnDataTable(company_query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(company_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_COMPANY_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company Data. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetLeaveTypeData(string company_code)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable leavetype_data_table = new DataTable();
        string leavetype_query = string.Empty;

        try
        {
            leavetype_query = " select LeaveCode, LeaveName FROM LeaveMaster where LeaveCode not in ('CO','OD','V') and companycode='" + company_code + "'";

            leavetype_data_table = db_connection.ReturnDataTable(leavetype_query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leavetype_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_LEAVE_TYPE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leave Type data. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    private string GetFilterQuery(string filters, string query)
    {
        JObject filters_data = JObject.Parse(filters);
        string filterquery = "";
        string filter_keyword = filters_data["filter_keyword"].ToString();
        string from_date = filters_data["filter_from"].ToString();
        string to_date = filters_data["filter_to"].ToString();
        string company_code = filters_data["filter_CompanyCode"].ToString();
        
        string leave_status = filters_data["filter_LeaveStatus"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        switch (filter_by)
        {
            case 1:
                filterquery += " and a.Emp_Code='" + filter_keyword + "'";
                break;
            case 2:
                filterquery += " and a.Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

       
        //==========Leave Status=========
        if (leave_status != "select")
            filterquery += " and a.Approval='" + leave_status + "'";

        //========from date and to date============
        if (from_date != "" && to_date != "")
        {
            //from_date = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            //to_date = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            filterquery += " and a.Fromdate between '" + from_date + "' and '" + to_date + "'";
        }

        if (filters != "")
        {

            filterquery = filterquery.Remove(1, 3).Insert(1, " where ");
        }

        query = query + filterquery;

        return query;
    }

    /****************************************************************************************************************************************************************/

    private string GetODLeavesBaseQuery()
    {
        string query = @"select Leave_id, Emp_Code,Emp_Name,LeaveName,FromDate,ToDate,Status,Approval,hl_status,Remarks,ApprovedbyName  FROM (
                        SELECT Leave_id,L.EmpID as Emp_Code, E.Emp_Name as Emp_Name, LM.LeaveName as LeaveName, L.LDate as FromDate, L.EndDate as ToDate,
                        ls.Leave_Status_text as Status,L.Flag as Approval,l.hl_status,L.Leave_id LeaveDetails_id,L.Remarks,L.ApprovedbyName 
                        , ROW_NUMBER() OVER (ORDER BY l.leave_id desc) as row FROM ODLeave L JOIN LeaveMaster LM ON L.LeaveType = LM.LeaveCode join EmployeeMaster e 
                        on e.Emp_Code=l.empid join leave_status Ls on Ls.Leave_Status_id = L.Flag where e.emp_status = 1 ";
        return query;
    }

    [WebMethod]
    public static ReturnObject GetODLeavesData(int page_number, bool is_filter, string filters)
    {
        od_details page_object = new od_details();

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable od_leave_data = new DataTable();
        DataTable branch_list_table = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string
          user_name = string.Empty,
          employee_id = string.Empty,
          query = string.Empty,
          CoManagerID = string.Empty,
          BranchList = "'Empty',",
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

            query = page_object.GetODLeavesBaseQuery();

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
            if (user_access_level == 0)
            {
                if (employee_id == "")
                {
                    query += " and e.Emp_Code !='" + employee_id + "'";
                }
              
            }
            else if (user_access_level == 3)
            {
                query += " And (e.Emp_Branch In(" + BranchList + ") or e.emp_code='" + employee_id + "') ";
            }
            else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")
            {
                query += " and (E.ManagerID In('" + employee_id + "'," + CoManagerID + ") and L.Approvallevel in (0,2)) or e.emp_code='" + employee_id + "' Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + CoManagerID + "," + InnerManagers + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
                query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1)";
            }
            else if (user_access_level == 1 && CoManagerID == "'Empty'")
            {
                query += " And (E.ManagerID In('" + employee_id + "') and L.Approvallevel in (0,2)) or e.emp_code='" + employee_id + "' Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + InnerManagers + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
                query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1)";
            }
            else
            {
                    query += " and e.Emp_Code='" + employee_id + "'";// Only Employee
            }

            
            if (!is_filter)
            {
                //query += " and 1=1";
                query += " ) a where row > " + start_row + " and row < " + number_of_record;
            }
                       

            if (is_filter)
            {
                query += " ) a ";
                query = page_object.GetFilterQuery(filters, query);
                query=query+"order by a.Emp_Code OFFSET " + start_row + " ROWS FETCH NEXT " + number_of_record + " ROWS ONLY " ;
            }

            od_leave_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(od_leave_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OD_LEAVES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;

    }

    protected void PrintExcel(DataTable table) {

        string attach = "attachment;filename=ODLeaveDetails.xls";
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

    protected void ExportToExcel(object sender, EventArgs e) {

        DBConnection db_connection = new DBConnection();
        DataTable od_leave_data_table = new DataTable();
        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        string employee_id = HttpContext.Current.Session["employee_id"].ToString();
        string query = string.Empty;

        if(user_access_level == 0) {

                query = "select l.Leave_id, l.EmpID, e.Emp_Name, lm.LeaveName, l.LDate, l.EndDate, ls.Leave_Status_text, l.Flag, l.hl_status, l.leavedetails_id, l.Remarks from ODLeave l, LeaveMaster lm, EmployeeMaster e, Leave_Status ls where l.LeaveType = lm.LeaveCode and e.Emp_code = l.empid and ls.Leave_Status_id = l.flag order by l.Leave_id";
            }
            else if(user_access_level == 1) {

                query = "select l.Leave_id, l.EmpID, e.Emp_Name, lm.LeaveName, l.LDate, l.EndDate, ls.Leave_Status_text, l.Flag, l.hl_status, l.leavedetails_id, l.Remarks from ODLeave l, LeaveMaster lm, EmployeeMaster e, Leave_Status ls where l.LeaveType = lm.LeaveCode and e.Emp_Code = l.EmpID and ls.Leave_Status_id = l.Flag and e.Emp_Code in ( select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1 ) order by l.Leave_id";
            }
            else {

                query = "select l.Leave_id, l.EmpID, e.Emp_Name, lm.LeaveName, l.LDate, l.EndDate, ls.Leave_Status_text, l.Flag, l.hl_status, l.leavedetails_id, l.Remarks, ROW_NUMBER() OVER (ORDER BY l.Leave_id) as row from ODLeave l, LeaveMaster lm, EmployeeMaster e, Leave_Status ls where l.LeaveType = lm.LeaveCode and e.Emp_Code = l.EmpID and ls.Leave_Status_id = l.Flag and l.EmpId = '" + employee_id + "' order by l.Leave_id";

            }
        
        od_leave_data_table = db_connection.ReturnDataTable(query);
        PrintExcel(od_leave_data_table);
    }
}
