using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SecurAX.Logger;
using System.Data;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using System.Collections;

public partial class outofoffice_apply : System.Web.UI.Page
{
    const string page = "OUT_OF_OFFICE_APPLICATION";

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

            message = "An error occurred while loading Out of office Application page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject CheckEmployee(string employee_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        DataTable branch_data = new DataTable();

        string
            query = string.Empty,
            current_user_id = string.Empty,
            branch_list = string.Empty,
            delegation_manager_id = string.Empty;

        int
            user_access_level = 0;

        try
        {
            query = "select count(*) from EmployeeMaster where Emp_Code = '" + employee_code + "' and Emp_Status = 1";
            if (db_connection.RecordExist(query))
            {
                return_object.status = "error";
                return_object.return_data = "Employee ID doesn't exist";
            }
            else
            {
                current_user_id = HttpContext.Current.Session["username"].ToString();

                if (current_user_id != employee_code)
                {
                    user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

                    if (user_access_level == 1 || user_access_level == 3)
                    {
                        // Get a list of branches for the current user.
                        query = "select BranchCode from TbManagerHrBranchMapping where ManagerID='" + employee_code + "' ";
                        branch_data = db_connection.ReturnDataTable(query);

                        if (branch_data.Rows.Count > 0)
                        {
                            foreach (DataRow row in branch_data.Rows)
                            {
                                branch_list += "'" + row["BranchCode"] + "',";
                            }
                        }
                        branch_list = branch_list.TrimEnd(',');

                        // Get the delegation manager for the user.
                        query = "Select ManagerID from TbAsignDelegation Where DelidationManagerID='" + current_user_id + "'  And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)";
                        delegation_manager_id = db_connection.ExecuteQuery_WithReturnValueString(query);

                        query = "select count(*) from employeeMaster where emp_Code='" + employee_code + "' and emp_status=1 and managerid In('" + current_user_id + "','" + delegation_manager_id + "') Or Emp_Branch In(" + branch_list + ")";
                        if (db_connection.RecordExist(query))
                        {
                            return_object.status = "error";
                            return_object.return_data = "The selected employee reports to a different manager.";
                        }
                    }
                    // END OF IF
                }
                // END OF IF
            }
            // END OF ELSE

            return_object.status = "success";
            return_object.return_data = "";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECK_EMPLOYEE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while validating Employee Details. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject ValidateEmployeeId(string employee_id)
    {
        outofoffice_apply page_object = new outofoffice_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable Branchlisttable = new DataTable();
        DataTable CoManagerID_data = new DataTable();
        DataTable innermanagertable = new DataTable();

        string
               employee_name = string.Empty,
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
            query = "Select Emp_Name from EmployeeMaster where Emp_Code='" + employee_id + "'";
            employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);

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
            if (!string.IsNullOrEmpty(employee_id))
            {
                if (!db_connection.RecordExist("select count(*) from employeeMaster where emp_Code='" + employee_id + "' and emp_status=1"))
                {
                    return_object.status = "error";
                    return_object.return_data = "Employee doesn't Exist.";
                    return return_object;
                }

                if (employee_id.Trim() != HttpContext.Current.Session["employee_id"].ToString())
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString()) == 1 || Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString()) == 3)
                    {
                        if (!db_connection.RecordExist("select count(*) from employeeMaster where emp_Code='" + employee_id.Trim() + "' and emp_status=1 and (managerid In('" + HttpContext.Current.Session["employee_id"].ToString() + "'," + CoManagerID + ") Or Emp_Branch In (" + BranchList + "))"))
                        {
                            return_object.status = "error";
                            return_object.return_data = "Entered Employee id does not belongs to this manager.";
                            return return_object;
                        }
                    }
                }

                if (string.IsNullOrEmpty(employee_name))
                {
                    employee_name = "";
                }

                return_object.status = "success";
                return_object.return_data = employee_id.ToString() + "," + employee_name.ToString();

            }

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
    public static ReturnObject CheckPOOOHours(string current)
    {
        outofoffice_apply page_object = new outofoffice_apply();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        JObject current_data = new JObject();
        string EmailMessage = string.Empty, retrun_message = string.Empty;

        int ooo_type = 0, ooo_hours = 0, ooo_hours_current = 0;

        string
            employee_code = string.Empty, employee_name = string.Empty,
            from_date = string.Empty, from_time = string.Empty,
            to_date = string.Empty, to_time = string.Empty,
            from_date_time = string.Empty, to_date_time = string.Empty,
            display_message = string.Empty,
            username = string.Empty, query = string.Empty;

        try
        {
            current_data = JObject.Parse(current);

            employee_code = current_data["employee_id"].ToString();
            employee_name = current_data["employee_name"].ToString();
            from_date_time = current_data["in_date"].ToString();
            to_date_time = current_data["out_date"].ToString();
            ooo_hours_current = Convert.ToInt32(current_data["total_hour"].ToString());

            DateTime date = Convert.ToDateTime(from_date_time).Date;
            from_date_time = new DateTime(date.Year, date.Month, 1).ToString("dd-MMM-yyyy");
            to_date_time = Convert.ToDateTime(from_date_time).AddMonths(1).AddDays(-1).ToString("dd-MMM-yyyy");

            from_date_time = from_date_time + " 00:00";
            to_date_time = to_date_time + " 23:59";

            ooo_type = Convert.ToInt32(current_data["outofoffice"].ToString());

            if (ooo_type == 1)
            {
                query = "select sum(DATEdiff(MINUTE,fromdatetime,todatetime)) from outoffoffice where Emp_ID='" + employee_code + "' and  FromDateTime >='" + from_date_time + "' and  ToDateTime<='" + to_date_time + "' and OOO_type=1";
                ooo_hours = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                ooo_hours_current = ooo_hours_current + ooo_hours;

                if (ooo_hours_current > 180)
                {
                    display_message = "Used hours of personal out of office time off is " + ooo_hours + " Minutes, you have left only " + (180 - ooo_hours) + " Minutes.";

                    return_object.status = "confirm-personal-ooo_hours";
                    return_object.return_data = display_message;
                }
                else
                {
                    return_object = SaveOutOfOffice(current);
                }
            }
            else
            {
                return_object = SaveOutOfOffice(current);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_PERSONAL_TYPE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while checking personal ooo hours. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject SaveOutOfOffice(string current)
    {
        outofoffice_apply page_object = new outofoffice_apply();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        JObject current_data = new JObject();
        string EmailMessage = string.Empty, retrun_message = string.Empty;

        int ooo_id = 0, access_level = 0, record_count = 0, ooo_type = 0, ooo_hours = 0;

        string
            employee_code = string.Empty, employee_name = string.Empty,
            from_date = string.Empty, from_time = string.Empty,
            to_date = string.Empty, to_time = string.Empty,
            from_date_time = string.Empty, to_date_time = string.Empty,
            reason = string.Empty, action_employee_code = string.Empty,
            username = string.Empty, query = string.Empty;

        try
        {
            access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());
            username = HttpContext.Current.Session["employee_id"].ToString();
            action_employee_code = HttpContext.Current.Session["employee_name"].ToString();

            current_data = JObject.Parse(current);

            employee_code = current_data["employee_id"].ToString();
            employee_name = current_data["employee_name"].ToString();

            from_date = current_data["in_date"].ToString();
            from_time = current_data["in_time"].ToString();
            from_date_time = from_date + " " + from_time;

            to_date = current_data["out_date"].ToString();
            to_time = current_data["out_time"].ToString();
            to_date_time = to_date + " " + to_time;

            ooo_hours = Convert.ToInt32(current_data["total_hour"].ToString());

            ooo_type = Convert.ToInt32(current_data["outofoffice"].ToString());
            reason = current_data["reason"].ToString();

            query = "select count(*) from outoffoffice where Emp_ID='" + employee_code + "' and (('" + from_date_time + "' between FromDateTime and ToDateTime) or  ('" + to_date_time + "' between FromDateTime and ToDateTime)) ";
            record_count = db_connection.GetRecordCount(query);

            if (record_count == 0)
            {
                query = "select count(*) from outoffoffice where Emp_ID='" + employee_code + "' and ( FromDateTime >='" + from_date_time + "' and  ToDateTime<='" + to_date_time + "') ";
                record_count = db_connection.GetRecordCount(query);
            }

            if (record_count == 0)
            {
                query = "insert into outoffoffice (Emp_ID,Emp_Name,OOO_type,FromDateTime,ToDateTime,Status,OOO_Minutes,Reason,Manager_Status,HR_Status,Manager_Name,Manager_Remark)values (";
                query += "'" + employee_code + "','" + employee_name + "'," + ooo_type + ",'" + from_date_time + "','" + to_date_time + "',1," + ooo_hours + ",";

                if (access_level == 1 && employee_code != HttpContext.Current.Session["employee_id"].ToString())//Manager and other employee
                {
                    query += "'" + reason + "',2,1,'" + action_employee_code + "','' )";
                }
                else if (access_level == 1 && employee_code == HttpContext.Current.Session["employee_id"].ToString())//Manager and himself
                {
                    query += "'" + reason + "',1,0,'','' )";
                }
                else if (access_level == 2)//employee
                {
                    query += "'" + reason + "',1,0,'','' )";
                }
                else if (access_level == 3)//HR
                {
                    query += "'" + reason + "',2,2,'','' )";
                }

                db_connection.ExecuteQuery_WithOutReturnValue(query);


                return_object.status = "success";
                retrun_message = "Out off Office Punch submitted successfully";

            }
            else
            {
                return_object.status = "error";
                retrun_message = "Out off Office Punch Already been submitted for the selected dates. Please try with different dates.";
            }

            //Auto Approval when HR submits ooo punch for employee
            if ((access_level == 0 || access_level == 3) && employee_code != HttpContext.Current.Session["employee_id"].ToString())
            {
                query = "select OOO_ID from outoffoffice where Emp_ID='" + employee_code + "' and FromDateTime='" + from_date_time + "' and ToDateTime='" + to_date_time + "' ";
                ooo_id = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                query = "update outoffoffice set Status=2 where OOO_ID=" + ooo_id;
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                return_object.status = "success";
                retrun_message += " and approved by HR";
            }

            /*Add previous ooo hours to applying hours*/
            int Total_OOO_Minutes = 0;
            string fromdatetime = from_date_time, todatetime = string.Empty;
            DateTime date = Convert.ToDateTime(fromdatetime).Date;
            fromdatetime = new DateTime(date.Year, date.Month, 1).ToString("dd-MMM-yyyy");
            todatetime = Convert.ToDateTime(fromdatetime).AddMonths(1).AddDays(-1).ToString("dd-MMM-yyyy");

            fromdatetime = fromdatetime + " 00:00";
            todatetime = todatetime + " 23:59";

            query = "select OOO_ID from outoffoffice where OOO_type=" + ooo_type + " and Emp_ID='" + employee_code + "' and FromDateTime='" + from_date_time + "' and ToDateTime='" + to_date_time + "' ";
            ooo_id = db_connection.ExecuteQuery_WithReturnValueInteger(query);

            query = "Select top 1 Total_OOO_Minutes from outoffoffice where OOO_type=" + ooo_type + " and OOO_ID!= " + ooo_id + "and Emp_ID='" + employee_code + "' and FromDateTime>='" + fromdatetime + "' and ToDateTime<='" + todatetime + "' order by OOO_ID desc";
            Total_OOO_Minutes = db_connection.ExecuteQuery_WithReturnValueInteger(query);

            Total_OOO_Minutes = Total_OOO_Minutes + ooo_hours;

            query = "update outoffoffice set Total_OOO_Minutes=" + Total_OOO_Minutes + " where OOO_ID=" + ooo_id;
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            SendMail(employee_code, employee_name, from_date_time, to_date_time, reason, 0);
            return_object.return_data = retrun_message;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SAVE_OUTOFFOFFICE_PUNCH");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving Out Off Office Punch. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }


    private static void SendMail(string employee_id, string employee_name, string fromdatetime, string todatetime, string Reason, int status_flag)
    {
        Hashtable leave_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();
        DataTable dt_temp = new DataTable();

        int access_level = 0;
        string
            mailTo = string.Empty, mailCC = string.Empty, HR_ID = string.Empty, Alternate_HR_ID = string.Empty,
            email_id = string.Empty, query = string.Empty, manual_punch_status = string.Empty,
            manager_name = string.Empty, branch_code = string.Empty, action_employee_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;

        try
        {
            access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());
            action_employee_name = HttpContext.Current.Session["employee_name"].ToString();

            //Getting the Employee email ID
            query = "select Emp_Email,Emp_Branch from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
            dt_temp = db_connection.ReturnDataTable(query);

            mailCC = dt_temp.Rows[0]["Emp_Email"].ToString();
            branch_code = dt_temp.Rows[0]["Emp_Branch"].ToString();

            //if manager submits ooo behalf of employee then mail should trigger to HR
            if (access_level == 1 && employee_id != HttpContext.Current.Session["employee_id"].ToString())
            {
                //// Getting HR Email ID and Name
                //query = "select HrIncharge,AlternativeHrIncharge from BranchMaster where BranchCode ='" + branch_code + "'";
                //dt_temp = db_connection.ReturnDataTable(query);

                //HR_ID = dt_temp.Rows[0]["HrIncharge"].ToString();
                //Alternate_HR_ID = dt_temp.Rows[0]["AlternativeHrIncharge"].ToString();

                //if (!string.IsNullOrEmpty(HR_ID))
                //{
                //    query = "select Emp_Email,Emp_name from EmployeeMaster where Emp_Code ='" + HR_ID + "'";
                //}
                //else
                //{
                //    query = "select Emp_Email,Emp_name from EmployeeMaster where Emp_Code ='" + Alternate_HR_ID + "'";
                //}

                //dt_temp = db_connection.ReturnDataTable(query);
                query = "select BM.ManagerID, EM.Emp_Email, EM.Emp_Name as Emp_name From TbManagerHrBranchMapping BM join EmployeeMaster EM on BM.ManagerID=em.Emp_Code where BM.BranchCode='" + branch_code + "'";
                dt_temp = db_connection.ReturnDataTable(query);

                mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
                manager_name = dt_temp.Rows[0]["Emp_name"].ToString();

                leave_send_mail.Add("ToEmailID", mailTo);
                leave_send_mail.Add("CCEmailID", mailCC);

                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following Out Off Office punch has been submitted  and Approved by " + action_employee_name + " and submitted for your action.<br/><br/>";
            }

            //if employee submits ooo then mail should trigger to manager
            if (access_level == 1 && employee_id == HttpContext.Current.Session["employee_id"].ToString())
            {
                // Getting Manager/HR Email ID and Name
                query = "select Emp_Email,Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                dt_temp = db_connection.ReturnDataTable(query);

                mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
                manager_name = dt_temp.Rows[0]["Emp_name"].ToString();

                leave_send_mail.Add("ToEmailID", mailTo);
                leave_send_mail.Add("CCEmailID", mailCC);

                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following Out Off Office punch has been submitted by " + action_employee_name + "<br/><br/>";
            }

            //if employee submits ooo then mail should trigger to manager
            if (access_level == 2)//employee
            {
                // Getting Manager/HR Email ID and Name
                query = "select Emp_Email,Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                dt_temp = db_connection.ReturnDataTable(query);

                mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
                manager_name = dt_temp.Rows[0]["Emp_name"].ToString();

                leave_send_mail.Add("ToEmailID", mailTo);
                leave_send_mail.Add("CCEmailID", mailCC);

                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following Out Off Office punch has been submitted by " + action_employee_name + "<br/><br/>";
            }
            if (access_level == 3)//HR
            {
                leave_send_mail.Add("ToEmailID", mailCC);
                leave_send_mail.Add("CCEmailID", mailCC);

                mailTo = mailCC;

                MailBody = MailBody + "Dear " + employee_name + ", <br/><br/>";
                MailBody = MailBody + "The following Out Off Office punch has been submitted by " + action_employee_name + " and Approved. <br/><br/>";
            }

            if (!string.IsNullOrEmpty(mailTo))
            {
                MailSubject = "Regarding Out Off Office Entry Application";

                MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
                MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
                MailBody = MailBody + "From Date Time: " + fromdatetime + "<br/><br/>";
                MailBody = MailBody + "To Date Time: " + todatetime + "<br/><br/>";
                MailBody = MailBody + "Reason:  " + Reason + "<br/><br/>";

                leave_send_mail.Add("EmpID", employee_id);
                leave_send_mail.Add("EmpName", employee_name);
                leave_send_mail.Add("Subject", MailSubject);
                leave_send_mail.Add("Body", MailBody);

                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", leave_send_mail);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SEND_OUTOFFOFFICE_PUNCH");
        }
    }
}
