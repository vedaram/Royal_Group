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
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Net;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;


public partial class od_apply : System.Web.UI.Page
{
    const string page = "OD_LEAVE_APPLICATION";

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

            message = "An error occurred while loading OD Leave Application page. Please try again. If the error persists, please contact Support.";

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

    private int CheckForLeave_OD(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        try
        {
            query = "SELECT COUNT(*) FROM Leave1 WHERE EmpID='" + employee_id + "' AND ";
            query += "((CONVERT(datetime,startdate,103) >= CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) <=CONVERT(datetime,'" + Leave_to + "',103))) and flag not in (3,4) ";
            count = db_connection.GetRecordCount(query);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_LEAVE");
            throw ex;
        }
        return count;
    }

    private int CheckForLossonpay_OD(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        try
        {
            query = "SELECT count(*) FROM LossonPay WHERE EmpID='" + employee_id + "' AND ";
            query += "((CONVERT(datetime,startdate,103) >= CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) <=CONVERT(datetime,'" + Leave_to + "',103))) and flag not in (3,4) ";
            count = db_connection.GetRecordCount(query);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_LOSSONPAY");
            throw ex;
        }

        return count;
    }

    private int CheckForOD_OD(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        try
        {
            query = "SELECT count(*)  FROM ODLEAVE WHERE EmpID='" + employee_id + "' AND ";
            query += "((CONVERT(datetime,startdate,103) >= CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) <=CONVERT(datetime,'" + Leave_to + "',103))) and flag not in (3,4) ";
            count = db_connection.GetRecordCount(query);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_OD");
            throw ex;
        }
        return count;
    }

    private int InsertData_OD(string EmpCode, string FromDate, string ToDate, string Reason, string OD_Type)
    {
        int ApprovalLevel = 0;
        DBConnection db_connection = new DBConnection();
        Hashtable hshParam = new Hashtable();
        int recordAffected = 0, user_access_level = 0;
        string query = "";
        bool IsAutoApproved = false;

        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        hshParam.Add("PiMode", "I");
        hshParam.Add("PiEmpCode", EmpCode);
        hshParam.Add("PiLeaveType", OD_Type);
        hshParam.Add("piFromDate", FromDate);
        hshParam.Add("piToDate", ToDate);
        hshParam.Add("piCreatedBy", Session["username"]);
        hshParam.Add("piModifiedBy", "");
        hshParam.Add("piReason", Reason);
        hshParam.Add("piTimestamp", 0);
        hshParam.Add("piAddress", "");
        hshParam.Add("piLatitude", "");
        hshParam.Add("piLongitude", "");
        hshParam.Add("piSource", "");
        hshParam.Add("piapprovallevel", ApprovalLevel);
        recordAffected = db_connection.exeStoredProcedure_WithHashtable_ReturnRow("spUpsertLeaveotherDetails", hshParam);


        /*this is to get leave_id to insert into leave transaction table for approval purpose*/
        query = "select Leave_ID from ODLeave where LDate='" + FromDate + "' and LeaveType='" + OD_Type + "' and EMPID='" + EmpCode + "' and Flag=1";
        int leave_id = db_connection.ExecuteQuery_WithReturnValueInteger(query);

        /*this is to get WFCode from procedure by passing empid and leavetype*/
        string wf_code = db_connection.ExecuteProcedureInOutParameters("ReadApprovalWFCode", EmpCode, OD_Type, "WorkFlowCode");
        
        if (!string.IsNullOrEmpty(wf_code))
        {
            //check for auto approval
            query = "Select top 1 ApproveLevel from ApprovalLevelMaster where WorkFlowCode ='" + wf_code + "'";
            int Auto_Approval = db_connection.ExecuteQuery_WithReturnValueInteger(query);

            if (Auto_Approval == 9)
            {
                /*Approve it from leave1 bcz its auto approve*/
                query = "update ODLeave set Flag=2 where Leave_ID=" + leave_id;
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                Hashtable leave_update = new Hashtable();
                leave_update.Add("piFlag", 2);
                leave_update.Add("piLeaveid", leave_id);
                leave_update.Add("piactioncomment", "Auto Approved");

                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("SpUpdateODLeaveStatus", leave_update);

                IsAutoApproved = true;

            }
        }

        SendMail(EmpCode, FromDate, ToDate, Reason, OD_Type, IsAutoApproved);

        return recordAffected;
    }

    private void SendMail(string employee_id, string FromDate, string ToDate, string Reason, string od_type, bool IsAutoApproved)
    {
        Hashtable ODleave_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;

        //Getting the Employee ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailCC = email_id;

        // Getting the Employee name
        query = "select Emp_Name from EmployeeMaster where Emp_Code='" + employee_id + "'";
        employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        //Getting Manager Email ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        //Getting manager name
        query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailTo = email_id;

        if (!string.IsNullOrEmpty(mailTo))
        {
            if (IsAutoApproved == true)
            {
                MailSubject = od_type + " has been submitted by  " + employee_name + " and Auto approved";

                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following " + od_type + " has been Submitted and Auto approved <br/><br/>";
            }
            else
            {
                MailSubject = od_type + " has been submitted by  " + employee_name + " for your approval";

                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following " + od_type + " has been Submitted <br/><br/>";
            }

            MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
            MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
            MailBody = MailBody + "From Date: " + FromDate + "<br/><br/>";
            MailBody = MailBody + "To Date : " + ToDate + "<br/><br/>";
            MailBody = MailBody + "Reason:  " + Reason + "<br/><br/>";

            ODleave_send_mail.Add("EmpID", employee_id);
            ODleave_send_mail.Add("EmpName", employee_name);
            ODleave_send_mail.Add("ToEmailID", mailTo);
            ODleave_send_mail.Add("CCEmailID", mailCC);
            ODleave_send_mail.Add("Subject", MailSubject);
            ODleave_send_mail.Add("Body", MailBody);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", ODleave_send_mail);
        }
    }

    [WebMethod]
    public static ReturnObject ValidateEmployeeId(string employee_id)
    {
        od_apply page_object = new od_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable Branchlisttable = new DataTable();
        DataTable innermanagertable = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string
            BranchList = string.Empty,
            ismanager = string.Empty,
            ishr = string.Empty,
            query = string.Empty,
            deligationmanager = string.Empty,
            CoManagerID = string.Empty,
            InnerManagers = string.Empty;
        int IsDelegationMngr = 0;

        try
        {
            if (employee_id == "")
            {
                return_object.status = "error";
                return_object.return_data = "Please enter Employee ID";
                return return_object;
            }

            IsDelegationMngr = db_connection.ExecuteQuery_WithReturnValueInteger("Select COUNT(DelidationManagerID) from TbAsignDelegation Where DelidationManagerID='" + HttpContext.Current.Session["employee_id"].ToString() + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)");
            if (IsDelegationMngr > 0)
            {
                query = "Select ManagerId from TbAsignDelegation Where DelidationManagerID='" + HttpContext.Current.Session["employee_id"].ToString() + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)";
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
            if (string.IsNullOrEmpty(CoManagerID))
            {
                CoManagerID = "'Empty'";
            }
            query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + HttpContext.Current.Session["employee_id"].ToString() + "'";
            Branchlisttable = db_connection.ReturnDataTable(query);

            if (Branchlisttable.Rows.Count > 0)
            {
                foreach (DataRow dr in Branchlisttable.Rows)
                {
                    BranchList += "'" + dr["BranchCode"] + "',";
                }
            }
            BranchList = BranchList.TrimEnd(',');

            if (string.IsNullOrEmpty(BranchList))
            {
                BranchList = "'Empty'";
            }
            if (!string.IsNullOrEmpty(employee_id) && employee_id != "admin")
            {
                if (!db_connection.RecordExist("select count(*) from employeeMaster where emp_Code='" + employee_id + "' and emp_status=1"))
                {
                    return_object.status = "error";
                    return_object.return_data = "Employee doesn't Exist.";
                    return return_object;
                }

                if (employee_id.Trim() != HttpContext.Current.Session["employee_id"].ToString())
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString()) == 0)
                    {
                        if (!db_connection.RecordExist("select count(*) from employeeMaster where emp_Code='" + employee_id.Trim() + "' and emp_status=1 "))
                        {
                            return_object.status = "error";
                            return_object.return_data = "Entered Employee id does not belongs to this manager.";
                            return return_object;

                        }
                    }
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "You don't have permission to apply leave for this employee.";
                        return return_object;
                    }

                }

            }
            return_object.status = "success";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "VALIDATE_EMPLOYEE_ID");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading EmployeeId. Please try again. If the error persists, please contact Support.";
        }

        return return_object;

    }


    [WebMethod]
    public static ReturnObject SubmitLeave_OD(string current)
    {
        od_apply PageObject = new od_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        string Employee_id = string.Empty;
        string FromDate = string.Empty;
        string ToDate = string.Empty;
        string Reason = string.Empty;
        string OD_Type = string.Empty;
        int count = 0;
        int ODsubmitflag = 0;

        try
        {

            JObject json_data = JObject.Parse(current);
            Employee_id = json_data["employee_id"].ToString();
            FromDate = json_data["from_date"].ToString();
            ToDate = json_data["to_date"].ToString();
            Reason = json_data["reason"].ToString();
            OD_Type = json_data["od_types"].ToString();

            count = PageObject.CheckForLeave_OD(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Leave already has been submitted for the selected dates.";
                return return_object;
            }
            count = PageObject.CheckForLossonpay_OD(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Leave already has been submitted for the selected dates.Please check in LWP Details page.";
                return return_object;
            }
            count = PageObject.CheckForOD_OD(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "OD has already been submitted for the selected dates.";
                return return_object;
            }

            ODsubmitflag = PageObject.InsertData_OD(Employee_id, FromDate, ToDate, Reason, OD_Type);
            if (ODsubmitflag > 0)
            {
                //PageObject.SendMail(Employee_id, FromDate, ToDate, Reason);
                return_object.status = "success";
                return_object.return_data = OD_Type + " Leave submitted successfully.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SUBMIT_OD_LEAVE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Submitting OD Leave Application. Please try again. If the error persists, please contact Support.";
        }

        finally
        {
            PageObject.Dispose();
        }
        return return_object;

    }
}