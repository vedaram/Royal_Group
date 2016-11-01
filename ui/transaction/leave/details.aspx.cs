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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SecurAX.Logger;
using System.Configuration;
using SecurAX.Import.Excel;

public partial class leave_details : System.Web.UI.Page
{
    const string page = "LEAVE_DETAILS";

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

            message = "An error occurred while loading Leave Approval page. Please try again. If the error persists, please contact Support.";

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
        string leave_type = filters_data["filter_LeaveType"].ToString();
        string leave_status = filters_data["filter_LeaveStatus"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        switch (filter_by)
        {
            case 1:
                filterquery += " and a.emp_id='" + filter_keyword + "'";
                break;
            case 2:
                filterquery += " and a.Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

        //==========Leave Type==========
        if (leave_type != "select")
            filterquery += " and a.Leave_Name like '%" + leave_type + "%'";

        //==========Leave Status=========
        if (leave_status != "select")
            filterquery += " and a.Approval='" + leave_status + "'";

        //========from date and to date============
        if (from_date != "" && to_date != "")
        {
            //from_date = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            //to_date = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            filterquery += " and a.[Fromdate] between '" + from_date + "' and '" + to_date + "'";
        }
        if (filters != "")
        {

            filterquery = filterquery.Remove(1, 3).Insert(1, " where ");
        }

        query = query + filterquery;

        return query;
    }

    private string GetFilterQueryNormalLeave(string filters, string query)
    {
        JObject filters_data = JObject.Parse(filters);
        string filterquery = "";
        string filter_keyword = filters_data["filter_keyword"].ToString();
        string from_date = filters_data["filter_from"].ToString();
        string to_date = filters_data["filter_to"].ToString();
        string company_code = filters_data["filter_CompanyCode"].ToString();
        string leave_type = filters_data["filter_LeaveType"].ToString();
        string leave_status = filters_data["filter_LeaveStatus"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        switch (filter_by)
        {
            case 1:
                filterquery += " and E.Emp_Code='" + filter_keyword + "'";
                break;
            case 2:
                filterquery += " and E.Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

        //==========Leave Type==========
        if (leave_type != "select" && !string.IsNullOrEmpty(leave_type))
        {
            filterquery += " and LM.leavename like '%" + leave_type + "%'";
        }

        //==========Leave Status new=========
        if (leave_status == "select")
        {

        }
        else
        {
            filterquery += " and L1.Flag='" + leave_status + "'";
        }

        //========from date and to date============
        if (from_date != "" && to_date != "")
        {
            filterquery += " and L1.LDate between '" + from_date + "' and '" + to_date + "'";
        }

        query = query + filterquery;

        return query;
    }

    /****************************************************************************************************************************************************************/

    private string GetNormalLeavesBaseQuery()
    {
        string query = string.Empty, employee_id = string.Empty;
        int user_access_level = 0;

        employee_id = HttpContext.Current.Session["employee_id"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        query = @"select * from ( Select LTMT.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                 LTMT.LeaveStatus as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by LTMT.leave_id desc) as rcount  from";

        if (user_access_level == 0)
        {
            query = @"select * from ( Select L1.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                        L1.Flag as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by L1.leave_id desc) as rcount  from 
                         Leave1 L1 JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = L1.Flag where  ";
        }
        else if (user_access_level == 3)
        {
            query += " ( select Distinct(LeaveApplicationID) as leave_id, max(LeaveStatus) as LeaveStatus from LeaveTransactionMaster L1 where L1.ActionEmpCode='" + employee_id + "'  group by L1.LeaveApplicationID ) LTMT";
            query += @" JOIN Leave1 L1 on L1.Leave_id=LTMT.leave_id JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID
                JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = LTMT.LeaveStatus where ";
        }
        else
        {
            query += " ( select Distinct(LeaveApplicationID) as leave_id, max(LeaveStatus) as LeaveStatus from LeaveTransactionMaster L1 group by L1.LeaveApplicationID ) LTMT";
            query += @" JOIN Leave1 L1 on L1.Leave_id=LTMT.leave_id JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID
                JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = LTMT.LeaveStatus where ";
        }
        return query;
    }

    [WebMethod]
    public static ReturnObject GetNormalLeavesData(int page_number, bool is_filter, string filters)
    {
        leave_details page_object = new leave_details();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable normal_leave_data = new DataTable();
        DataTable branch_list_table = new DataTable();
        DataTable CoManagerID_data = new DataTable();
        DataTable self_data_table = new DataTable();

        string
           user_name = string.Empty,
           employee_id = string.Empty,
           query = string.Empty,
           self_query = string.Empty,
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

            //query = page_object.GetNormalLeavesBaseQuery();

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

            if (user_access_level == 0) //admin
            {
                query = @"select * from ( Select L1.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                        L1.Flag as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by L1.leave_id desc) as rcount  from 
                         Leave1 L1 JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = L1.Flag  ";
            }

            if (user_access_level == 3) //HR
            {
                self_query = @"select * from ( Select L1.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                        L1.Flag as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by L1.leave_id desc) as rcount  from 
                         Leave1 L1 JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = L1.Flag and E.Emp_Code='" + employee_id + "' ";

                query = @"select * from ( Select LTMT.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                 LTMT.LeaveStatus as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by LTMT.leave_id desc) as rcount  from";

                query += " ( select Distinct(LeaveApplicationID) as leave_id, max(LeaveStatus) as LeaveStatus from LeaveTransactionMaster L1 where L1.ActionEmpCode='" + employee_id + "'  group by L1.LeaveApplicationID ) LTMT";
                query += @" JOIN Leave1 L1 on L1.Leave_id=LTMT.leave_id JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID
                JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = LTMT.LeaveStatus where ";

                query += "  ( E.Emp_Code in (Select Emp_Code from EmployeeMaster where ManagerID='" + employee_id + "') or e.Emp_Branch In(" + BranchList + ")) ";
            }

            if (user_access_level == 1 && (!string.IsNullOrEmpty(CoManagerID)))//Manager and CoManager
            {
                self_query = @"select * from ( Select L1.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                        L1.Flag as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by L1.leave_id desc) as rcount  from 
                         Leave1 L1 JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = L1.Flag and E.Emp_Code='" + employee_id + "' ";

                query = @"select * from ( Select LTMT.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                 LTMT.LeaveStatus as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by LTMT.leave_id desc) as rcount  from";

                query += " ( select Distinct(LeaveApplicationID) as leave_id, max(LeaveStatus) as LeaveStatus from LeaveTransactionMaster L1 where L1.ActionEmpCode='" + employee_id + "'  group by L1.LeaveApplicationID ) LTMT";
                query += @" JOIN Leave1 L1 on L1.Leave_id=LTMT.leave_id JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID
                JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = LTMT.LeaveStatus where ";

                query += " ( E.Emp_Code in (Select Emp_Code from EmployeeMaster where ManagerID in ('" + employee_id + "'," + CoManagerID + "))) ";
            }

            if (user_access_level == 1 && (string.IsNullOrEmpty(CoManagerID)))//only Manager
            {
                self_query = @"select * from ( Select L1.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                        L1.Flag as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by L1.leave_id desc) as rcount  from 
                         Leave1 L1 JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = L1.Flag and E.Emp_Code='" + employee_id + "'  ";

                query = @"select * from ( Select LTMT.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                 LTMT.LeaveStatus as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by LTMT.leave_id desc) as rcount  from";

                query += " ( select Distinct(LeaveApplicationID) as leave_id, max(LeaveStatus) as LeaveStatus from LeaveTransactionMaster L1 where L1.ActionEmpCode='" + employee_id + "'  group by L1.LeaveApplicationID ) LTMT";
                query += @" JOIN Leave1 L1 on L1.Leave_id=LTMT.leave_id JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID
                JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = LTMT.LeaveStatus where ";

                query += " ( E.Emp_Code in (Select Emp_Code from EmployeeMaster where ManagerID='" + employee_id + "'))";
            }

            if (user_access_level == 2)//only Employee
            {
                self_query = @"select * from ( Select L1.leave_id as leave_id,L1.empid,E.Emp_Name as 'emp_name',LM.LeaveName as 'leavename', L1.StartDate as 'From',L1.EndDate as 'To',LS.Leave_Status_text,
                        L1.Flag as flag,L1.hl_status,L1.Remarks,L1.ReasonForLeave, L1.ApprovedbyName,row_number() over(order by L1.leave_id desc) as rcount  from 
                         Leave1 L1 JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = L1.Flag and E.Emp_Code='" + employee_id + "' ";
            }

            if (!is_filter)
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query += " ) a where  rcount > " + start_row + " and rcount < " + number_of_record;
                }
                if (!string.IsNullOrEmpty(self_query))
                {
                    self_query += " ) a where  rcount > " + start_row + " and rcount < " + number_of_record;
                }
            }

            if (is_filter)
            {
                query = page_object.GetFilterQueryNormalLeave(filters, query);
                query += " ) a ";
                query += "order by a.empid OFFSET " + start_row + " ROWS FETCH NEXT " + number_of_record + " ROWS ONLY ";

                if (!string.IsNullOrEmpty(self_query))
                {
                    self_query = page_object.GetFilterQueryNormalLeave(filters, self_query);
                    self_query += " ) a ";
                    self_query += "order by a.empid OFFSET " + start_row + " ROWS FETCH NEXT " + number_of_record + " ROWS ONLY ";
                }
            }

            if (!string.IsNullOrEmpty(self_query))
            {
                self_data_table = db_connection.ReturnDataTable(self_query);
            }
            if (!string.IsNullOrEmpty(query))
            {
                normal_leave_data = db_connection.ReturnDataTable(query);
                if (self_data_table.Rows.Count > 0)
                {
                    foreach (DataRow dr in self_data_table.Rows)
                    {
                        normal_leave_data.Rows.Add(dr.ItemArray);
                    }
                }
            }
            else
            {
                normal_leave_data = self_data_table;
            }

           

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(normal_leave_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_NORMAL_LEAVES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;

    }

    /****************************************************************************************************************************************************************/

    private string GetLWPLeavesBaseQuery()
    {
        string query = string.Empty;
        query = @"select leave_id, Emp_ID, Emp_Name, Leave_id,Leave_Name, FromDate, ToDate,  Status, Approval, hl_status,remarks,ReasonForLeave,row from 
                (SELECT L.Leave_id, L.EmpID as Emp_ID, E.Emp_Name as Emp_Name, LM.LeaveName as Leave_Name, StartDate as FromDate, EndDate as ToDate,
                ls.Leave_Status_text as Status,Flag as Approval,l.hl_status, LeaveDetails_id, L.noofdays[NoOfDays], L.Remarks,L.ApprovedbyName,L.ReasonForLeave,
                ROW_NUMBER() OVER (ORDER BY l.leave_id desc) as row  FROM LossOnPay L JOIN LeaveMaster LM ON L.LeaveType = LM.LeaveCode join 
                EmployeeMaster e on e.Emp_Code=l.empid join leave_status Ls on Ls.Leave_Status_id = L.Flag  where e.emp_status = 1  ";

        return query;
    }

    [WebMethod]
    public static ReturnObject GetLWPLeavesData(int page_number, bool is_filter, string filters)
    {
        leave_details page_object = new leave_details();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable leaveList = new DataTable();
        DataTable branch_list_data = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string user_name = string.Empty,
                employee_id = string.Empty,
                query = string.Empty,
                CoManagerID = string.Empty,
                BranchList = string.Empty;

        int user_access_level = 0,
            start_row = 0,
            number_of_record = 0,
            IsDelegationManager = 0;

        try
        {
            user_name = HttpContext.Current.Session["username"].ToString();
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

            start_row = (page_number - 1) * 30;
            number_of_record = page_number * 30 + 1;

            //check employee is Delegation Manager or not if so get his CoManagerID
            IsDelegationManager = Convert.ToInt32(db_connection.ExecuteQuery_WithReturnValueString("Select COUNT(DelidationManagerID) from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)"));
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

            //get list of branches assigned to logged in manager hr
            BranchList = "'Empty',";
            query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + employee_id + "'";
            branch_list_data = db_connection.ReturnDataTable(query);
            query = string.Empty;

            query = page_object.GetLWPLeavesBaseQuery(); //read main query

            if (branch_list_data.Rows.Count > 0)
            {
                foreach (DataRow dr in branch_list_data.Rows)
                {
                    BranchList += "'" + dr["BranchCode"] + "',";
                }

            }
            BranchList = BranchList.TrimEnd(',');

            //Validate CoManagerID
            if (string.IsNullOrEmpty(CoManagerID))
            {
                CoManagerID = "'Empty'";
            }

            //modify query as per access level
            if (user_access_level == 0)//Admin
            {
                query += "  ";
            }
            else if (user_access_level == 3)//HR
            {
                query += " and e.Emp_Branch In(" + BranchList + ") ";
            }
            else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
            {
                query += " and l.EmpID In(Select Emp_Code From EmployeeMaster Where Emp_Code In('" + employee_id + "') OR ManagerID In('" + employee_id + "'," + CoManagerID + ")) Or e.Emp_Branch In(" + BranchList + ") ";
            }
            else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager
            {
                query += " and e.Emp_Branch In(" + BranchList + ") or Emp_Code in ( select distinct(Emp_Code) from EmployeeMaster where (managerId='" + employee_id + "' or Emp_Code='" + employee_id + "')  and Emp_Status=1 ) ";
            }
            else
            {
                query += " and e.Emp_Code='" + employee_id + "'";// Only Employee
            }

            if (!is_filter)
            {
                //query += " and l.flag=1 ";
                query += " ) a where row > " + start_row + " and row < " + number_of_record;
            }

            if (is_filter)
            {
                query += " ) a ";
                query = page_object.GetFilterQuery(filters, query);
                query = query + "order by a.emp_id OFFSET " + start_row + " ROWS FETCH NEXT " + number_of_record + " ROWS ONLY ";
            }

            // query += " order by a.Leave_id desc";

            leaveList = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leaveList, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_LWP_LEAVES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }
    
    protected void PrintExcel(string filename, DataTable table)
    {

        string attach = "attachment;filename=" + filename + ".xls";
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

    protected void ExportToExcel(object sender, EventArgs e)
    {

        DBConnection db_connection = new DBConnection();
        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        string employee_id = HttpContext.Current.Session["employee_id"].ToString();
        string selected_tab = Request["selected-tab-export"].ToString();
        string filename = "";
        string query = "";
        string table = "";

        if (selected_tab == "normal")
        {
            filename = "LeaveDetails";
            table = "Leave1";
        }
        else
        {
            filename = "LWPDetails";
            table = "LossOnPay";
        }

        if (user_access_level == 0)
        {
            query = "select l.Leave_id, l.EmpID, e.Emp_Name, lm.LeaveName, l.LDate, l.EndDate, ls.Leave_Status_text, l.Flag, l.hl_status, l.leavedetails_id, l.Remarks from " + table + " l, LeaveMaster lm, EmployeeMaster e, Leave_Status ls where l.LeaveType = lm.LeaveCode and e.Emp_code = l.empid and ls.Leave_Status_id = l.flag";
        }
        else if (user_access_level == 1)
        {
            query = "select l.Leave_id, l.EmpID, e.Emp_Name, lm.LeaveName, l.LDate, l.EndDate, ls.Leave_Status_text, l.Flag, l.hl_status, l.leavedetails_id, l.Remarks from " + table + " l, LeaveMaster lm, EmployeeMaster e, Leave_Status ls where l.LeaveType = lm.LeaveCode and e.Emp_Code = l.EmpID and ls.Leave_Status_id = l.Flag and e.Emp_Code in ( select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1 )";
        }
        else
        {
            query = "select l.Leave_id, l.EmpID, e.Emp_Name, lm.LeaveName, l.LDate, l.EndDate, ls.Leave_Status_text, l.Flag, l.hl_status, l.leavedetails_id, l.Remarks from " + table + " l, LeaveMaster lm, EmployeeMaster e, Leave_Status ls where l.LeaveType = lm.LeaveCode and e.Emp_Code = l.EmpID and ls.Leave_Status_id = l.Flag and l.EmpId = '" + employee_id + "'";
        }

        DataTable leaveList = new DataTable();
        leaveList = db_connection.ReturnDataTable(query);
        PrintExcel(filename, leaveList);
    }

    //Import leaves from HRMS excel file for RoyalGroup 
    /****************************************************************************************************************************************************************/

    private string[] GetEmployeeCodes()
    {
        DBConnection db_connection = new DBConnection();
        string[] employee_code_array = null;
        DataTable employee_data = new DataTable();
        string query = string.Empty;

        query = " select EmpID from [FetchEmployees] ('" + HttpContext.Current.Session["username"].ToString() + "')";

        employee_data = db_connection.ReturnDataTable(query);

        employee_code_array = new string[employee_data.Rows.Count];

        for (int i = 0; i < employee_data.Rows.Count; i++)
        {
            employee_code_array[i] = employee_data.Rows[i]["EmpID"].ToString().ToUpper();
        }

        return employee_code_array;
    }

    private string[] GetLeaveCodes(string employee_id)
    {
        DBConnection db_connection = new DBConnection();
        string[] Leave_code_array = null;
        DataTable employee_data = new DataTable();
        string query = string.Empty;

        query = " select LeaveCode from LeaveMaster where EmployeeCategoryCode =(select Emp_Employee_Category From employeemaster where emp_code='" + employee_id + "') ";

        employee_data = db_connection.ReturnDataTable(query);

        Leave_code_array = new string[employee_data.Rows.Count];

        for (int i = 0; i < employee_data.Rows.Count; i++)
        {
            Leave_code_array[i] = employee_data.Rows[i]["LeaveCode"].ToString().ToUpper();
        }

        return Leave_code_array;
    }

    public string ImportLeaves(DataTable excel_data)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable leave_details = new Hashtable();

        double no_of_days_leave = 0.0;

        int
            row = 1,
            user_access_level = 0;

        bool is_valid = true;

        DateTime
            start_date = new DateTime(),
            end_date = new DateTime();

        string[]
            employee_code_array = null,
            leave_codes_array = null;

        string
            current_employee_id = string.Empty,
            current_employee_name = string.Empty,
            return_message = string.Empty,
            employee_code = string.Empty,
            leave_from = string.Empty,
            leave_to = string.Empty,
            leave_name = string.Empty,
            leave_code = string.Empty,
            half_day = string.Empty,
            existing_leave_id = string.Empty,
            query = string.Empty;

        // getting data about the logged in user.
        current_employee_id = HttpContext.Current.Session["employee_id"].ToString();
        current_employee_name = HttpContext.Current.Session["employee_name"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (string.IsNullOrEmpty(current_employee_name))
        {
            current_employee_name = "admin";
        }

        // getting a list of the all the employee codes in the system.
        // This will be used for comparison purposes to identify valid employee codes.
        employee_code_array = GetEmployeeCodes();

        foreach (DataRow data_row in excel_data.Rows)
        {
            employee_code = data_row["EmpID"].ToString();

            // every row will be processed only if the employee ID has been provided.
            if (!string.IsNullOrEmpty(employee_code))
            {
                /*
                    Checking the employee code from the Excel row with the employee codes in the system. 
                */
                if (Array.IndexOf(employee_code_array, employee_code) < 0)
                {
                    return_message += "Employee Code doesn't exist. Row Number: " + row + System.Environment.NewLine;
                    is_valid = false;
                }
                else
                {
                    // Validating From Date
                    leave_from = data_row["Actual Start Date"].ToString();
                    if (!string.IsNullOrEmpty(leave_from))
                    {
                        leave_from = Convert.ToDateTime(data_row["Actual Start Date"]).ToString("dd-MMM-yyyy");
                        //leave_from = DateTime.ParseExact()
                        start_date = Convert.ToDateTime(leave_from);
                    }
                    else
                    {
                        return_message += "Actual Start Date cannot be empty. Row Number: " + row + System.Environment.NewLine;
                        is_valid = false;
                    }

                    // Validating To Date
                    leave_to = data_row["Actual End Date"].ToString();
                    if (!string.IsNullOrEmpty(leave_to))
                    {
                        leave_to = Convert.ToDateTime(data_row["Actual End Date"]).ToString("dd-MMM-yyyy");
                        end_date = Convert.ToDateTime(leave_to);
                    }
                    else
                    {
                        return_message += "Actual End Date cannot be empty. Row Number: " + row + System.Environment.NewLine;
                        is_valid = false;
                    }

                    // Validating if From Date > To Date
                    if (!string.IsNullOrEmpty(leave_from) && !string.IsNullOrEmpty(leave_to) && start_date > end_date)
                    {
                        return_message += "Actual Start Date cannot be greater than Actual End Date. Row Number: " + row + System.Environment.NewLine;
                        is_valid = false;
                    }

                    // Validating leave code
                    leave_name = data_row["Absence Type"].ToString();
                    if (string.IsNullOrEmpty(leave_name))
                    {
                        return_message += "Absence Type cannot be empty. Row Number: " + row + System.Environment.NewLine;
                        is_valid = false;
                    }
                    else
                    {
                        // Get the leave code based on the leave name and employee code
                        query = "select LeaveCode from Leavemaster where EmployeeCategoryCode=(select Emp_Employee_Category from EmployeeMaster where Emp_Code='" + employee_code + "') and LeaveName='" + leave_name + "'";
                        leave_code = db_connection.ExecuteQuery_WithReturnValueString(query);

                        // Get all leaves assigned for this employee
                        leave_codes_array = GetLeaveCodes(employee_code);

                        if (Array.IndexOf(leave_codes_array, leave_code) < 0)
                        {
                            return_message += "Absence Type not assigned for Employee. Row Number: " + row + System.Environment.NewLine;
                            is_valid = false;
                        }
                    }

                    // Validating No of Days of Leave
                    if (!double.TryParse(data_row["Days"].ToString(), out no_of_days_leave))
                    {
                        return_message += "The value Days is invalid. Row Number: " + row + System.Environment.NewLine;
                        is_valid = false;
                    }
                    else
                    {
                        no_of_days_leave = Convert.ToDouble(data_row["Days"].ToString());

                        if (no_of_days_leave < 0)
                        {
                            return_message += "Days cannot be less than 0. Row Number: " + row + System.Environment.NewLine;
                        }
                        else
                        {
                            if (no_of_days_leave < 1.0)
                            {
                                half_day = "1";
                            }
                            else
                            {
                                half_day = "0";
                            }
                        }
                    }
                }

                if (is_valid)
                {
                    // Checking for leaves around the dates as the current excel row.
                    query = "SELECT leave_id FROM Leave1 WHERE EmpID='" + employee_code + "' AND ";
                    query += "((CONVERT(datetime,startdate,103) >= CONVERT(datetime,'" + leave_from + "',103) And CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + leave_to + "',103)) OR ";
                    query += "(CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + leave_from + "',103) And CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + leave_to + "',103)) OR ";
                    query += "(CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + leave_from + "',103) And CONVERT(datetime,enddate,103) <=CONVERT(datetime,'" + leave_to + "',103))) and flag not in (3,4) ";

                    existing_leave_id = db_connection.ExecuteQuery_WithReturnValueString(query);

                    // If an existing leave is found around the same dates, the leave will be updated to the current values.
                    if (!string.IsNullOrEmpty(existing_leave_id))
                    {
                        query = "update leave1 set Flag=3, MFlag=3 where leave_ID='" + existing_leave_id + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        leave_details.Add("Flag", "4");
                        leave_details.Add("Leaveid", existing_leave_id);
                        leave_details.Add("actioncomment", "Declined");
                        leave_details.Add("Imdtmngrflag", "4");

                        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("SpUpdateLeaveStatus", leave_details);

                        leave_details.Clear();
                    }

                    query = @"insert into leave1(EmpID, LDate, StartDate, EndDate, LeaveType, FLag, MFlag, LeaveDetails_ID, noofdays, ApprovedbyName, hl_status)
                                     values ('" + employee_code + "','" + leave_from + "','" + leave_from + "','" + leave_to + "','" + leave_code + "',2,2,1," + no_of_days_leave + ",'" + current_employee_name + "', '" + half_day + "') ";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    // get the last inserted leave id. We will use this leave ID to update the reports.
                    query = "select TOP 1 leave_id from leave1 order by leave_id DESC";
                    leave_code = db_connection.ExecuteQuery_WithReturnValueString(query);

                    leave_details.Add("Flag", "2");
                    leave_details.Add("Leaveid", leave_code);
                    leave_details.Add("actioncomment", "Approved");
                    leave_details.Add("Imdtmngrflag", "2");

                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("SpUpdateLeaveStatus", leave_details);

                    leave_details.Clear();

                    return_message += Environment.NewLine + "Leaves updated for employee - " + employee_code + System.Environment.NewLine;
                }
            }
            else
            {
                return_message += "Employee ID is empty on row " + row + " " + System.Environment.NewLine;
            }

            // increment the row number
            row++;
            // reset valid flag 
            is_valid = true;
        }

        return return_message;
    }

    [WebMethod]
    public static ReturnObject DoImport(string file_name)
    {
        leave_details PageObject = new leave_details();
        ReturnObject return_object = new ReturnObject();

        DBConnection db_connection = new DBConnection();

        DataTable excel_data = new DataTable();

        string
            default_upload_path = string.Empty, full_upload_path = string.Empty,
            employee_id = string.Empty, return_message = string.Empty,
            query = string.Empty;

        try
        {
            default_upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            full_upload_path = HttpContext.Current.Server.MapPath("~/" + default_upload_path + "/" + file_name);

            // Read the excel file and store the data in a DataTable.
            excel_data = ExcelImport.ImportExcelToDataTable(full_upload_path, "");

            if (excel_data.Rows.Count > 0)
            {
                return_object.status = "success";
                return_object.return_data = PageObject.ImportLeaves(excel_data);
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = "Oops excel sheet is empty";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DO_IMPORT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }
}
