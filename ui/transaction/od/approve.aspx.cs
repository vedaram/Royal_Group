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
using Newtonsoft.Json;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class od_approve : System.Web.UI.Page
{
    const string page = "OD_APPROVAL";

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
        string filter_keyword = filters_data["filter_keyword"].ToString();
        string from_date = filters_data["filter_from"].ToString();
        string to_date = filters_data["filter_to"].ToString();
        string company_code = filters_data["filter_CompanyCode"].ToString();
        string leave_status = filters_data["filter_LeaveStatus"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        switch (filter_by)
        {
            case 1:
                query += " and Emp_Code='" + filter_keyword + "'";
                break;
            case 2:
                query += " and Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

        //==========Leave Status=========
        if (leave_status != "0")
            query += " and l.Flag='" + leave_status + "'";

        //========from date and to date============
        if (from_date != "" && to_date != "")
        {
            //from_date = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            //to_date = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            query += " and l.startdate between '" + from_date + "' and '" + to_date + "'";
        }

        return query;
    }

    /****************************************************************************************************************************************************************/

    private string GetODLeavesBaseQuery()
    {
        string query = @"select Leave_id, Emp_Code,Emp_Name,LeaveName,FromDate,ToDate,Status,Approval,hl_status,Remarks,ApprovedbyName  FROM (SELECT Leave_id,L.EmpID as Emp_Code, E.Emp_Name as Emp_Name, LM.LeaveName as LeaveName, L.LDate as FromDate, L.EndDate as ToDate, ls.Leave_Status_text as Status,L.Flag as Approval,l.hl_status,L.Leave_id LeaveDetails_id,L.Remarks,L.ApprovedbyName , ROW_NUMBER() OVER (ORDER BY l.Leave_id DESC) as row FROM ODLeave L JOIN LeaveMaster LM ON L.LeaveType = LM.LeaveCode join EmployeeMaster e on e.Emp_Code=l.empid join leave_status Ls on Ls.Leave_Status_id = L.Flag where  e.emp_status = 1 ";
        return query;
    }

    [WebMethod]
    public static ReturnObject GetODLeavesData(int page_number, bool is_filter, string filters)
    {
        od_approve page_object = new od_approve();

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
                
            }

            BranchList = BranchList.TrimEnd(',');

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
                //query += " and (E.ManagerID In('" + employee_id + "'," + CoManagerID + ") ) Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + CoManagerID + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
                query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId In ('" + employee_id + "'," + CoManagerID + ") and Emp_Status=1)";
            }
            else if (user_access_level == 1 && CoManagerID == "'Empty'")
            {
                //query += " And (E.ManagerID In('" + employee_id + "')) Or (L.EmpID In(Select Emp_Code from employeemaster Where ManagerID In(" + InnerManagers + ") And L.Approvallevel=1 And L.Flag=5 And L.MFlag=1)) ";
                //query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId='" + employee_id + "' and Emp_Status=1)";
                query += " and e.Emp_Code in (select distinct(Emp_Code) from EmployeeMaster where managerId In ('" + employee_id + "') and Emp_Status=1)";
            }
            else
            {
                query += " and 1=0 ";
            }

            
            if (!is_filter)
            {
                query += " and L.flag=1 ";
                query += " ) a where a.Approval=1 and row > " + start_row + " and row < " + number_of_record;
            }

            

            if (is_filter)
            {
                query = page_object.GetFilterQuery(filters, query);
                query += " ) a where row > " + start_row + " and row < " + number_of_record;
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

    public void UpdateLeave(int leave_id, int leave_flag, string leave_comment)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable leave_delete = new Hashtable();
        Hashtable leave_update = new Hashtable();

        leave_update.Add("piFlag", leave_flag);
        leave_update.Add("piLeaveid", leave_id);
        leave_update.Add("piactioncomment", leave_comment);

        leave_delete.Add("Leaveid", leave_id);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("SpUpdateODLeaveStatus", leave_update);

    }

    private void SendMail(int leave_id, string leave_status)
    {
        Hashtable leave_send_mail = new Hashtable();
        DataTable leave_data = new DataTable();
        DBConnection db_connection = new DBConnection();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty,
            employee_id = string.Empty, employee_name = string.Empty,
            leave_type = string.Empty, Leave_date = string.Empty,
            from_date = string.Empty, to_date = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;


        query = "select L.empid [Emp ID], e.emp_name [Emp Name], L.LDate [Leave Date], L.StartDate [From Date], L.EndDate [To Date], L.LeaveType from ODleave L join EmployeeMaster e on L.EMPID = e.Emp_Code where Leave_id='" + leave_id + "'";

        leave_data = db_connection.ReturnDataTable(query);

        employee_id = leave_data.Rows[0]["Emp ID"].ToString();
        employee_name = leave_data.Rows[0]["Emp Name"].ToString();
        leave_type = leave_data.Rows[0]["LeaveType"].ToString();

        Leave_date = Convert.ToDateTime(leave_data.Rows[0]["Leave Date"]).ToString("dd/MM/yyyy");
        from_date = Convert.ToDateTime(leave_data.Rows[0]["From Date"]).ToString("dd/MM/yyyy");
        to_date = Convert.ToDateTime(leave_data.Rows[0]["To Date"]).ToString("dd/MM/yyyy");

        // Getting the Employee ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailTo = email_id;

        // Getting Manager Email ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailCC = email_id;

        if (!string.IsNullOrEmpty(mailTo))
        {
            MailSubject = "Regarding OD Leave " + leave_status;
            MailBody = "Dear " + employee_name + ", <br/><br/> Your leave is " + leave_status + " for the date which is as per the following :<br/><br/> Leave Date: " + Leave_date + "<br/><br/>Leave Type: " + leave_type + " <br/><br/> Leave From: " + from_date + "<br/><br/> Leave To: " + to_date + "<br/><br/>Thanks";

            leave_send_mail.Add("EmpID", employee_id);
            leave_send_mail.Add("EmpName", employee_name);
            leave_send_mail.Add("ToEmailID", mailTo);
            leave_send_mail.Add("CCEmailID", mailCC);
            leave_send_mail.Add("Subject", MailSubject);
            leave_send_mail.Add("Body", MailBody);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", leave_send_mail);
        }
    }

    private void LeaveAction(int leave_id, int action, string leave_status, string comments)
    {
        od_approve page_object = new od_approve();
        DBConnection db_connection = new DBConnection();
        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        Hashtable leave_details_data = new Hashtable();
        string employee_name = string.Empty;


        if (!string.IsNullOrEmpty(HttpContext.Current.Session["employee_name"].ToString()))
            employee_name = HttpContext.Current.Session["employee_name"].ToString();

        leave_details_data.Add("pialevel", user_access_level);
        leave_details_data.Add("pileave_id", leave_id);
        leave_details_data.Add("pistat", action);
        leave_details_data.Add("pitxt", comments);
        leave_details_data.Add("piapprovedby", employee_name);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("SPODApproveall", leave_details_data);

        page_object.UpdateLeave(leave_id, action, leave_status);
        page_object.SendMail(leave_id, leave_status);
    }

    [WebMethod]
    public static ReturnObject DoAction(int action, string comments, string selected_rows)
    {
        od_approve page_object = new od_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        List<string> selected_leaves = JsonConvert.DeserializeObject<List<string>>(selected_rows);

        string
            query = string.Empty,
            leave_status = string.Empty;

        int
            leave_id = 0, current_leave_flag = 0;

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

                query = "select flag from ODleave where leave_id='" + leave_id + "'";

                current_leave_flag = Convert.ToInt32(db_connection.ExecuteQuery_WithReturnValueString(query));

                if (current_leave_flag == 2 && action == 3)
                {
                    return_object.status = "error";
                    return_object.return_data = "OD is approved and cannot be declined. Please cancel the OD";
                    return return_object;
                }
                if (current_leave_flag == 2 && action == 2)
                {
                    return_object.status = "error";
                    return_object.return_data = "OD is approved already . ";
                    return return_object;
                }
                if (current_leave_flag == 4)
                {
                    return_object.status = "error";
                    return_object.return_data = "OD is already canceled";
                    return return_object;
                }
                if (current_leave_flag == 3)
                {
                    return_object.status = "error";
                    return_object.return_data = "OD is already Declined";
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
                    page_object.LeaveAction(leave_id, action, leave_status, comments);

                // As the leave has been APPROVED, only CANCELLING the leave is possible.
                if ((current_leave_flag == 2) && (action == 4))
                    page_object.LeaveAction(leave_id, action, leave_status, comments);

                if (current_leave_flag == 3) { } // As the leave has been DECLINED, no further action is possible.

                if (current_leave_flag == 4) { } // As the leave has been CANCELLED, no further action is possible.

            }

            return_object.status = "success";
            return_object.return_data = "Leave " + leave_status + " successfully!";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_OD_APPROVAL_STATUS");

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
