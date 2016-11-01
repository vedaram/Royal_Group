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
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.Services;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class leave_approve : System.Web.UI.Page
{
    const string page = "LEAVE_APPROVAL";

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
                filterquery += " and l.empid='" + filter_keyword + "'";
                break;
            case 2:
                filterquery += " and e.emp_name like '%" + filter_keyword + "%'";
                break;
        }

        //==========Leave Type==========
        if (leave_type != "select")
            filterquery += " and lm.leavename like '%" + leave_type + "%'";

        //==========Leave Status=========
        if (leave_status == "1" && leave_status != "select")
        {
            leave_status = " ('1','5')";
            filterquery += " and l.Flag in " + leave_status + "";
        }
        else if (leave_status != "select" && leave_status != "('1','5')")
            filterquery += " and l.Flag='" + leave_status + "'";

        //========from date and to date============
        if (from_date != "" && to_date != "")
        {
            //from_date = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            //to_date = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            filterquery += " and l.[ldate] between '" + from_date + "' and '" + to_date + "'";
        }

        if (filters != "")
        {

            //filterquery = filterquery.Remove(1, 3).Insert(1, " where ");
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
            query += " ( select Distinct(LeaveApplicationID) as leave_id, max(LeaveStatus) as LeaveStatus from LeaveTransactionMaster L1 where L1.ActionEmpCode='" + employee_id + "' or L1.CoActionEmpCode='" + employee_id + "' group by L1.LeaveApplicationID ) LTMT";
            query += @" JOIN Leave1 L1 on L1.Leave_id=LTMT.leave_id JOIN EmployeeMaster E on E.Emp_Code=L1.EMPID
                JOIN LeaveMaster LM ON L1.LeaveType = LM.LeaveCode  JOIN Leave_Status LS on LS.Leave_Status_id = LTMT.LeaveStatus where ";
        }



        return query;
    }

    [WebMethod]
    public static ReturnObject GetNormalLeavesData(int page_number, bool is_filter, string filters)
    {
        leave_approve page_object = new leave_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable normal_leave_data = new DataTable();
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

            query = page_object.GetNormalLeavesBaseQuery();

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
                query += "  1=1 ";
            }
            if (user_access_level == 3) //HR
            {
                query += "  (1=1 or E.Emp_Code in (Select Emp_Code from EmployeeMaster where ManagerID='" + employee_id + "') or e.Emp_Branch In(" + BranchList + ")) ";
            }
            if (user_access_level == 1 && (!string.IsNullOrEmpty(CoManagerID)))//Manager and CoManager
            {
                query += " (1=1 or E.Emp_Code in (Select Emp_Code from EmployeeMaster where ManagerID in ('" + employee_id + "'," + CoManagerID + "))) ";
            }
            if (user_access_level == 1 && (string.IsNullOrEmpty(CoManagerID)))//only Manager
            {
                query += " (1=1 or E.Emp_Code in (Select Emp_Code from EmployeeMaster where ManagerID='" + employee_id + "'))";
            }


            if (!is_filter)
            {
                query += " ) a where a.flag = 1 and rcount > " + start_row + " and rcount < " + number_of_record;
            }

            if (is_filter)
            {
                query = page_object.GetFilterQueryNormalLeave(filters, query);
                query += " ) a ";
                query += "order by a.empid OFFSET " + start_row + " ROWS FETCH NEXT " + number_of_record + " ROWS ONLY ";
            }

            normal_leave_data = db_connection.ReturnDataTable(query);

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

        //        query = @"select leave_id, empid, emp_name, leavename, StartDate as 'From', enddate as 'To', Leave_Status_text, flag, hl_status, 
        //                leavedetails_id, remarks from (select l.leave_id, l.empid, e.emp_name, lm.leavename, l.StartDate, l.enddate, ls.Leave_Status_text, l.flag, hl_status, 
        //                l.leavedetails_id, l.remarks, ROW_NUMBER() OVER (ORDER BY l.leave_id) as row FROM LossOnpay l, LeaveMaster lm, EmployeeMaster e, leave_status ls where 
        //                l.leaveType = lm.leavecode and e.emp_code = l.empid and ls.leave_status_id = l.flag and l.flag = 1  and e.emp_status = 1 ";

        query = @"select *from (SELECT leave_id, L.EmpID [empid], E.Emp_Name [emp_name], LM.LeaveName [leavename], L.LDate [From], L.EndDate [To],
            ls.Leave_Status_text as Leave_Status_text,Flag as flag,l.hl_status,L.Leave_id LeaveDetails_id, L.Remarks,L.ApprovedbyName,L.ReasonForLeave,
            row_number() over(order by leave_id desc) as rcount FROM Lossonpay L JOIN LeaveMaster LM ON L.LeaveType = LM.LeaveCode join EmployeeMaster e 
            on e.Emp_Code=l.empid join leave_status Ls on Ls.Leave_Status_id = L.Flag Where L.Flag=1";
        return query;
    }

    [WebMethod]
    public static ReturnObject GetLWPLeavesData(int page_number, bool is_filter, string filters)
    {
        leave_approve page_object = new leave_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable lwp_leave_data = new DataTable();
        DataTable branch_list_table = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string
            user_name = string.Empty, employee_id = string.Empty,
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
            start_row = (page_number - 1) * 30;
            number_of_record = page_number * 30 + 1;

            // getting session data for later use in the function.
            user_name = HttpContext.Current.Session["username"].ToString();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            employee_id = HttpContext.Current.Session["employee_id"].ToString();

            query = page_object.GetLWPLeavesBaseQuery();

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
                query += " and e.Emp_Code !='" + employee_id + "'";
            }
            else if (user_access_level == 3)
            {
                query += " And e.Emp_Branch In(" + BranchList + ") ";
            }
            else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")
            {
                query += " and (E.ManagerID In('" + employee_id + "'," + CoManagerID + ") and L.Approvallevel in (0,2)) Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + CoManagerID + "," + InnerManagers + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
                query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1)";
            }
            else if (user_access_level == 1 && CoManagerID == "'Empty'")
            {
                query += " And (E.ManagerID In('" + employee_id + "') and L.Approvallevel in (0,2)) Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + InnerManagers + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
                query += " or e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1)";
            }
            else
            {
                query += " and 1=0 ";
            }

            if (!is_filter)
            {

                query += " and l.flag=1 ";
            }

            query += " ) a where rcount > " + start_row + " and rcount < " + number_of_record;

            if (is_filter)
            {
                query = page_object.GetFilterQuery(filters, query);
            }



            lwp_leave_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(lwp_leave_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_LOP_LEAVES");

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

    public string UpdateLeave(int leave_id, int leave_flag, string leave_comment, string leave_category, int imediatemnrflag)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable leave_delete = new Hashtable();
        Hashtable leave_update = new Hashtable();
        string LoggedinuserName = HttpContext.Current.Session["username"].ToString();
        int managerflag;
        string Message = "";

        leave_delete.Add("Leaveid", leave_id);

        if (leave_category == "leave")
        {
            int LeaveAprvLevel = db_connection.ExecuteQuery_WithReturnValueInteger("Select ApprovalLevel from Leave1 Where Leave_ID='" + leave_id + "'");
            if (LeaveAprvLevel == 2 && LoggedinuserName != "admin" && leave_flag != 4)
            {

                managerflag = 5;
                imediatemnrflag = 1;

                leave_update.Add("Flag", managerflag);
                leave_update.Add("Leaveid", leave_id);
                leave_update.Add("actioncomment", leave_comment);
                leave_update.Add("Imdtmngrflag", imediatemnrflag);
                Message = "Leave approved and submited for second level approval";
            }
            else
            {

                leave_update.Add("Flag", leave_flag);
                leave_update.Add("Leaveid", leave_id);
                leave_update.Add("actioncomment", leave_comment);
                leave_update.Add("Imdtmngrflag", imediatemnrflag);
            }

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("SpUpdateLeaveStatus", leave_update);
            if (leave_flag == 3 || leave_flag == 4)
                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("spDeleteLeave", leave_delete);
        }
        else
        {
            int LeaveAprvLevel = db_connection.ExecuteQuery_WithReturnValueInteger("Select ApprovalLevel from lossonpay Where Leave_ID='" + leave_id + "'");
            if (LeaveAprvLevel == 2 && LoggedinuserName != "admin" && leave_flag != 4)
            {

                managerflag = 5;
                imediatemnrflag = 1;

                leave_update.Add("Flag", managerflag);
                leave_update.Add("Leaveid", leave_id);
                leave_update.Add("actioncomment", leave_comment);
                leave_update.Add("Imdtmngrflag", imediatemnrflag);


            }
            else
            {

                leave_update.Add("Flag", leave_flag);
                leave_update.Add("Leaveid", leave_id);
                leave_update.Add("actioncomment", leave_comment);
            }
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("SpUpdateLeaveStatus1", leave_update);

            if (leave_flag == 3)
                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("spDeleteLeave1", leave_delete);
        }
        return Message;
    }

    private void SendMail(int leave_id, string leave_category, string leave_status)
    {
        Hashtable leave_send_mail = new Hashtable();
        DataTable leave_data = new DataTable();
        DataTable manager_data = new DataTable();
        DBConnection db_connection = new DBConnection();
        string overtimestatus = string.Empty;

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty,
            employee_id = string.Empty, employee_name = string.Empty,
            leave_type = string.Empty, Leave_date = string.Empty,
            from_date = string.Empty, to_date = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty,
            ActionEmpCode = string.Empty, manager_name = string.Empty,
            leave_name = string.Empty, LastActionEmpID = string.Empty,
            LastActionEmpName=string.Empty;

        double noOfLeaveApplied = 0, closingLeaveBalance = 0, Openingbalance = 0;
        int leave_flag = 0, user_access_level = 0;
                
        LastActionEmpID = HttpContext.Current.Session["username"].ToString();
        LastActionEmpName = db_connection.ExecuteQuery_WithReturnValueString("Select Emp_Name from EMployeeMaster where Emp_Code='"+LastActionEmpID+"'");
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (leave_category == "leave")
            query = "select L.empid [Emp ID], e.emp_name [Emp Name], L.LDate [Leave Date], L.StartDate [From Date], L.EndDate [To Date], L.LeaveType,LM.LeaveName, L.Flag from leave1 L join EmployeeMaster e on L.EMPID = e.Emp_Code join LeaveMaster LM on LM.LeaveCode=L.LeaveType where Leave_id='" + leave_id + "'";
        else
            query = "select L.empid [Emp ID], e.emp_name [Emp Name], L.LDate [Leave Date], L.StartDate [From Date], L.EndDate [To Date], L.LeaveType, LM.LeaveName,  L.Flag from lossonpay L join EmployeeMaster e on L.EMPID = e.Emp_Code join LeaveMaster LM on LM.LeaveCode=L.LeaveType where Leave_id='" + leave_id + "'";

        leave_data = db_connection.ReturnDataTable(query);

        employee_id = leave_data.Rows[0]["Emp ID"].ToString();
        employee_name = leave_data.Rows[0]["Emp Name"].ToString();
        leave_type = leave_data.Rows[0]["LeaveType"].ToString();
        leave_name = leave_data.Rows[0]["LeaveName"].ToString();
        leave_flag = Convert.ToInt32(leave_data.Rows[0]["Flag"].ToString());

        Leave_date = Convert.ToDateTime(leave_data.Rows[0]["Leave Date"]).ToString("yyyy-MM-dd");
        from_date = Convert.ToDateTime(leave_data.Rows[0]["From Date"]).ToString("yyyy-MM-dd");
        to_date = Convert.ToDateTime(leave_data.Rows[0]["To Date"]).ToString("yyyy-MM-dd");

        // Getting the Employee ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        //Getting Leave applied from DB
        query = "select dbo.[BalanceLeave]('" + employee_id + "',CONVERT(date,'" + from_date + "'),CONVERT(date,'" + to_date + "'),'" + leave_type + "')";
        noOfLeaveApplied = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));

        //Getting Leave Balance from DB
        query = "select leave_balance from employee_leave where Emp_Code='" + employee_id + "' and Leave_code='" + leave_type + "'";
        closingLeaveBalance = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));

        Openingbalance = closingLeaveBalance + noOfLeaveApplied;

        if (!string.IsNullOrEmpty(email_id))
        {
            mailCC = email_id;
            email_id = "";
        }
        

        /*if still work flow is pending then only read next action employee id,name and email*/
        if (leave_flag == 1)
        {
            //getting email id for next action emp id
            query = "select ActionEmpCode from LeaveTransactionMaster where LeaveApplicationID=" + leave_id + " and TransActionID=(select max(TransactionID) from LeaveTransactionMaster where LeaveApplicationID=" + leave_id + ")";
            ActionEmpCode = db_connection.ExecuteQuery_WithReturnValueString(query);

            query = "select Emp_Email,Emp_Name from EmployeeMaster where Emp_Code ='" + ActionEmpCode + "'";
            manager_data = db_connection.ReturnDataTable(query);

            if (manager_data.Rows.Count>0)
            {
                email_id = manager_data.Rows[0]["Emp_Email"].ToString();
                manager_name = manager_data.Rows[0]["Emp_Name"].ToString();
            }         
        }
        if (email_id == "")
        {
            query = "select Emp_Email from EmployeeMaster where Emp_Code='" + LastActionEmpID + "'";
            email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

            if (!string.IsNullOrEmpty(email_id))
                mailTo = email_id;
        }

        if (!string.IsNullOrEmpty(mailTo))
        {
            if (leave_flag == 1)
            {
                MailSubject = "Leave has been approved by " + LastActionEmpName + "-(" + LastActionEmpID + ") waiting for your further action <br/><br/>";
                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following Leave has been Submitted <br/><br/>";
                MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
                MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
                MailBody = MailBody + "LeaveType : " + leave_name + " <br/><br/>";

                //Leave Details stffs
                MailBody = MailBody + "Applied Leave Details <br/><br/>";
                MailBody = MailBody + "Opening balance: " + Openingbalance + " <br/><br/>";
                MailBody = MailBody + "Consumed Leaves: " + noOfLeaveApplied + " <br/><br/>";
                MailBody = MailBody + "Closing Balance : " + closingLeaveBalance + " <br/><br/>";

                leave_send_mail.Add("ToEmailID", mailTo);
                leave_send_mail.Add("CCEmailID", mailCC);
            }
            else
            {
                MailSubject = "Regarding Leave " + leave_status;
                MailBody = "Dear " + employee_name + ", <br/><br/> Your leave is " + leave_status + " for the date which is as per the following :<br/><br/> Leave Date: " + Leave_date + "<br/><br/>Leave Type: " + leave_name + " <br/><br/> Leave From: " + from_date + "<br/><br/> Leave To: " + to_date + "<br/><br/>Thanks";

                leave_send_mail.Add("ToEmailID", mailCC);
                leave_send_mail.Add("CCEmailID", mailTo);
            }

            leave_send_mail.Add("EmpID", employee_id);
            leave_send_mail.Add("EmpName", employee_name);
            leave_send_mail.Add("Subject", MailSubject);
            leave_send_mail.Add("Body", MailBody);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", leave_send_mail);
        }
    }

    private string LeaveAction(int leave_id, int action, string leave_status, string selected_tab, int imediatemnrflag, string comments)
    {
        leave_approve page_object = new leave_approve();
        DBConnection db_connection = new DBConnection();
        DataTable leave_details = new DataTable();

        Hashtable leave_details_data = new Hashtable();

        int user_access_level = 0,WFStatus=0;
        string Message = "", LeaveType = "", Employee_id = "", Approvedby = "";
        string query = string.Empty, wf_code = string.Empty, ActionEmpID = string.Empty, CoActionEmpID = string.Empty;
        int PreviousFlag = 0, CurrentFlag = 0;

        CurrentFlag = action;
        ActionEmpID = HttpContext.Current.Session["username"].ToString();
        Approvedby = HttpContext.Current.Session["employee_name"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (string.IsNullOrEmpty(Approvedby))
        {
            Approvedby = "Admin";
        }

        if (user_access_level!=0)
        {
            query = "select max(WFStatus) from LeaveTransactionMaster where ActionEmpCode='" + ActionEmpID + "' and and leaveapplicationid=" + leave_id;
            WFStatus = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            if (WFStatus == 1)
            {
                Message = "Already you have taken action.";
                return Message;
            }
 
        }
        

        /*
        leave_details_data.Add("pialevel", user_access_level);
        leave_details_data.Add("pileave_id", leave_id);
        leave_details_data.Add("pistat", action);
        leave_details_data.Add("pitxt", selected_tab);
        leave_details_data.Add("piapprovedby", Approvedby);
        leave_details_data.Add("picomments", comments);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("spapproveall", leave_details_data);

        Message=page_object.UpdateLeave(leave_id, action, leave_status, selected_tab, imediatemnrflag);
         * */

        query = "Select EmpID,LeaveType,Flag  from Leave1 where leave_id=" + leave_id;
        leave_details = db_connection.ReturnDataTable(query);

        Employee_id = leave_details.Rows[0]["EmpID"].ToString();
        LeaveType = leave_details.Rows[0]["LeaveType"].ToString();
        PreviousFlag = Convert.ToInt32(leave_details.Rows[0]["Flag"].ToString());

        /*what if after final approval if anybody wants to cancel/declien leave*/
        if (PreviousFlag == 2 && (CurrentFlag == 3 || CurrentFlag == 4))
        {
            /*write code to update flag and revert leave balance*/

            /*update status*/
            Hashtable leave_update = new Hashtable();

            /*revert leave balance*/
            leave_update.Add("Flag", CurrentFlag);
            leave_update.Add("Leaveid", leave_id);
            leave_update.Add("actioncomment", comments);
            leave_update.Add("Imdtmngrflag", imediatemnrflag);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("SpUpdateLeaveStatus", leave_update);

            Hashtable leave_delete = new Hashtable();
            leave_delete.Add("Leaveid", leave_id);
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("spDeleteLeave", leave_delete);

            /*this is to get WFCode from procedure by passing empid and leavetype*/
            wf_code = db_connection.ExecuteProcedureInOutParameters("ReadApprovalWFCode", Employee_id, LeaveType, "WorkFlowCode");

            /*add record into leave transaction*/
            query = "insert into LeaveTransactionMaster values (" + leave_id + ",'" + wf_code + "',1,1,'" + ActionEmpID + "',NULL,CONVERT(DATE,GETDATE())," + CurrentFlag + ") ";
            db_connection.ExecuteQuery_WithOutReturnValue(query);
        }

        if (PreviousFlag == 1)
        {
            /*this is to get WFCode from procedure by passing empid and leavetype*/
            wf_code = db_connection.ExecuteProcedureInOutParameters("ReadApprovalWFCode", Employee_id, LeaveType, "WorkFlowCode");

            if (!string.IsNullOrEmpty(wf_code))
            {
                leave_details_data.Add("Action_EmpCode", ActionEmpID);
                leave_details_data.Add("LeaveApplication_ID", leave_id);
                leave_details_data.Add("WorkFlow_Code", wf_code);
                leave_details_data.Add("LeaveStatus", CurrentFlag);
                leave_details_data.Add("AccessLevel", user_access_level);

                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ProcessLeaveApplication", leave_details_data);
            }
            else
            {
                /*what if emp doesn't belong to any group*/
            }
        }

        page_object.SendMail(leave_id, selected_tab, leave_status);
        return Message;
    }

    [WebMethod]
    public static ReturnObject DoAction(string selected_tab, int action, string comments, string selected_rows)
    {
        leave_approve page_object = new leave_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        List<string> selected_leaves = JsonConvert.DeserializeObject<List<string>>(selected_rows);
        String Message = string.Empty;

        string
            query = string.Empty,
            leave_status = string.Empty;

        int
            leave_id = 0, current_leave_flag = 0, imediatemnrflag = 0;

        try
        {
            switch (action)
            {
                case 2:
                    leave_status = "Approved";
                    break;

                case 3:
                    leave_status = "Declined";
                    break;

                case 4:
                    leave_status = "Cancelled";
                    break;
            }

            for (int i = 0; i < selected_leaves.Count; i++)
            {
                leave_id = Convert.ToInt32(selected_leaves[i].ToString());

                if (selected_tab == "leave")
                    query = "select flag from leave1 where leave_id='" + leave_id + "'";
                else
                    query = "select flag from lossonpay where leave_id='" + leave_id + "'";

                current_leave_flag = Convert.ToInt32(db_connection.ExecuteQuery_WithReturnValueString(query));
                if (current_leave_flag == 2 && action == 3)
                {
                    return_object.status = "error";
                    return_object.return_data = "Leave is approved and cannot be declined. Please cancel the leave";
                    return return_object;
                }
                if (current_leave_flag == 2 && action == 2)
                {
                    return_object.status = "error";
                    return_object.return_data = "Leave is approved already . ";
                    return return_object;
                }
                if (current_leave_flag == 4)
                {
                    return_object.status = "error";
                    return_object.return_data = "Leave is already canceled";
                    return return_object;
                }
                if (current_leave_flag == 3)
                {
                    return_object.status = "error";
                    return_object.return_data = "Leave is already Declined";
                    return return_object;
                }

                /* STATUS MAP
                 * Submitted = 1 
                 * Approved  = 2 
                 * Declined  = 3 
                 * Canceled  = 4
                */


                // As the leave has been submitted further actions possible are Approving, Declining or Cancelling.
                if ((current_leave_flag == 1) && (action == 2 || action == 3 || action == 4))
                    Message = page_object.LeaveAction(leave_id, action, leave_status, selected_tab, imediatemnrflag, comments);

                // As the leave has been APPROVED, only CANCELLING the leave is possible.
                if ((current_leave_flag == 2) && (action == 4))
                    Message = page_object.LeaveAction(leave_id, action, leave_status, selected_tab, imediatemnrflag, comments);

                if (current_leave_flag == 3) { } // As the leave has been DECLINED, no further action is possible.

                if (current_leave_flag == 4) { } // As the leave has been CANCELLED, no further action is possible.

                if (current_leave_flag == 5)
                {
                    action = 2;
                    imediatemnrflag = 2;
                    Message = page_object.LeaveAction(leave_id, action, leave_status, selected_tab, imediatemnrflag, comments);
                }

            }

            return_object.status = "success";
            if (Message != "")
            {
                return_object.return_data = Message;
            }
            else
            {
                return_object.return_data = "Leave " + leave_status + " successfully!";
            }

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_LEAVE_APPROVAL_STATUS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while updating Leave Approval Status. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }
}
