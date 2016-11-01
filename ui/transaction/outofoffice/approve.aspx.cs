using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using SecurAX.Logger;
using System.Data;
using Newtonsoft.Json;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using System.Configuration;
using SecurAX.Export.Excel;

public partial class outofoffice_approve : System.Web.UI.Page
{
    const string page = "OUT_OFF_OFFICE_APPROVAL";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex)
        {
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

    private string GetBaseQuery(int access_level)
    {
        string query = " Select * from ( select OOO_ID,Emp_ID,O.Emp_Name,OOO_type_name as OOO_Type,CONVERT(date,FromDateTime) as FromDate, ";
        query += " substring(CONVERT(VARCHAR, FromDateTime, 114),1,5) as FromTime, CONVERT(date,ToDateTime) as ToDate, ";
        query += " substring(CONVERT(VARCHAR, ToDateTime, 114),1,5) as ToTime,Reason,OOO_Minutes, ";

        if (access_level == 1)
        {
            query += " O.Manager_Status as Status, ";
        }
        if (access_level == 3)
        {
            query += " O.HR_Status as Status, ";
        }
        if (access_level == 0)
        {
            query += " O.Status as Status,";
        }

        query += " O.Manager_Name as Manager_ID, O.Manager_Remark as Manager_Remark,";
        query += " CONVERT(VARCHAR, RIGHT('0' +  RTRIM(OOO_Minutes/60),3) + ':' + RIGHT('0' + RTRIM(OOO_Minutes%60),2)) as Hours, ";
        query += " CONVERT(VARCHAR, RIGHT('0' +  RTRIM(Total_OOO_Minutes/60),3) + ':' + RIGHT('0' + RTRIM(Total_OOO_Minutes%60),2)) as TotalHours, ";
        query += " ROW_NUMBER() OVER (ORDER BY ooo_id desc) as row from outoffoffice O join OOOType OT on O.OOO_type=OT.OOO_type_id  ";
        query += " join EmployeeMaster EM on O.Emp_ID=EM.Emp_Code where  1=1 ";
        return query;
    }

    private string GetFilterQuery(string filters, string query, int access_level)
    {
        JObject filters_data = JObject.Parse(filters);
        string from_date = filters_data["filter_indate"].ToString();
        string to_date = filters_data["filter_outdate"].ToString();
        string outofoffice_status = filters_data["filter_OutOffOffice"].ToString();
        string filter_OutOffOfficetype = filters_data["filter_OutOffOfficetype"].ToString();
        string filter_keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        switch (filter_by)
        {
            case 1:
                query += " and EM.Emp_Code='" + filter_keyword + "'";
                break;
            case 2:
                query += " and EM.Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

        if (filter_OutOffOfficetype != "select")
        {
            query += " and OT.OOO_type_id=" + filter_OutOffOfficetype;
        }

        //==========Manual Pucnh Status==========
        if (outofoffice_status != "select")
        {
            if (access_level == 1)
            {
                query += " and O.Manager_Status='" + outofoffice_status + "'";
            }
            if (access_level == 3)
            {
                query += " and O.HR_Status='" + outofoffice_status + "'";
            }
            if (access_level == 0)
            {
                query += " and O.Status='" + outofoffice_status + "'";
            }
        }
        else
        {
            if (access_level == 1)
            {
                query += " and O.Manager_Status!=0";
            }
            if (access_level == 3)
            {
                query += " and O.HR_Status!=0";
            }

        }

        //========from date and to date============
        //if (from_date != "" && to_date != "")
        if (from_date != "")
        {
            from_date = "01-" + from_date;

            DateTime monthenddate = Convert.ToDateTime(from_date);
            monthenddate = monthenddate.AddMonths(1).AddDays(-1);
            to_date = monthenddate.ToString("dd-MMM-yyyy");
            query += " and FromDateTime>='" + from_date + " 00:00' and ToDateTime<='" + to_date + " 23:59'";
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetOutOffOfficeData(int page_number, bool is_filter, string filters)
    {

        outofoffice_approve page_object = new outofoffice_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable out_off_office_data_table = new DataTable();
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

            //get list of branches assigned to logged in manager hr
            query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + employee_id + "'";
            branch_list_data = db_connection.ReturnDataTable(query);
            query = string.Empty;
            
            if (branch_list_data.Rows.Count > 0)
            {
                foreach (DataRow dr in branch_list_data.Rows)
                {
                    BranchList += "'" + dr["BranchCode"] + "',";
                }

                BranchList = BranchList.TrimEnd(',');
            }
            else
            {
                BranchList = "'Empty'";
            }

            query = page_object.GetBaseQuery(user_access_level); //read main query
            query += " and Emp_ID !='" + employee_id + "'"; //don't show data for logged in employee

            if (user_access_level == 0)
            {
                query += " and Emp_ID in (select Emp_Code from EmployeeMaster where Emp_Status=1)";
            }
            if (user_access_level == 1)
            {
              //  query += " and Emp_ID in (select Emp_Code from EmployeeMaster where ManagerID='" + employee_id + "' )";
               // query += " and Emp_ID in (select empid from fetchemployees ( '"+ employee_id + "' )";
                 query+= string.Format("and Emp_Id in (Select empid from fetchemployees('{0}',''))",employee_id);
                
            }
            if (user_access_level == 3)
            {                
                query += " and Emp_ID in ( select Emp_Code from EmployeeMaster where Emp_Branch In (" + BranchList + "))";
            }

            if (is_filter)
            {
                query = page_object.GetFilterQuery(filters, query, user_access_level);
            }
            else
            {
                if (user_access_level == 3)
                {
                    query += " and O.HR_Status =1 ";
                }
                if (user_access_level == 1)
                {
                    query += " and O.Manager_Status =1 ";
                }

                DateTime date = DateTime.Now.Date;
                string from_date_temp = new DateTime(date.Year, date.Month, 1).ToString("dd-MMM-yyyy");
                string to_date_temp = Convert.ToDateTime(from_date_temp).AddMonths(1).AddDays(-1).ToString("dd-MMM-yyyy");

                query += " and FromDateTime>='" + from_date_temp + " 00:00' and ToDateTime<='" + to_date_temp + " 23:59'";
            }

            query += " ) a where row > " + start_row + " and row < " + number_of_record;
            query += "  order by OOO_ID";

            out_off_office_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(out_off_office_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OUT_OFF_OFFICE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static ReturnObject DoAction(int action, string comments, string selected_rows)
    {
        outofoffice_approve page_object = new outofoffice_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        List<string> selected_out_off_office = JsonConvert.DeserializeObject<List<string>>(selected_rows);

        string
            query = string.Empty, return_message = string.Empty,
            ooo_status = string.Empty;

        long
            ooo_id = 0;
        int update_flag = 0, Existsing_Status = 0;

        try
        {
            switch (action)
            {
                case 2:
                    ooo_status = "Approved";
                    update_flag = 2;
                    break;

                case 3:
                    ooo_status = "Rejected";
                    update_flag = 3;
                    break;
            }

            for (int i = 0; i < selected_out_off_office.Count; i++)
            {
                ooo_id = Convert.ToInt64(selected_out_off_office[i].ToString());

                query = "select Status from outoffoffice where OOO_ID='" + ooo_id + "' ";
                Existsing_Status = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                if (Existsing_Status == 2)
                {
                    return_message += "Out Off Office punch is already approved" + Environment.NewLine;
                }
                else if (Existsing_Status == 3)
                {
                    return_message += "Out Off Office punch is already rejected" + Environment.NewLine;
                }
                else
                {
                    page_object.ApproveOutOffOffice(ooo_id, update_flag, comments);
                    return_message = "Out Off Office " + ooo_status + " successfully!" + Environment.NewLine;
                }
            }

            return_object.status = "success";
            return_object.return_data = return_message;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_OUT_OFF_OFFICE_APPROVAL_STATUS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while updating Out Off Office Approval Status. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    protected void ApproveOutOffOffice(long ooo_id, int status_flag, string comments)
    {
        outofoffice_approve page_object = new outofoffice_approve();
        DBConnection db_connection = new DBConnection();
        Hashtable hsh = new Hashtable();
        DataTable out_off_office_data = new DataTable();
        DataTable employee_data = new DataTable();
        Hashtable approved_list = new Hashtable();

        string query = string.Empty, user_name = string.Empty,
            emp_id = string.Empty, card_no = string.Empty,
            PunchDate1 = string.Empty, Punch_Time1 = string.Empty,
            PunchDate2 = string.Empty, Punch_Time2 = string.Empty,
            employee_id = string.Empty, employee_name = string.Empty;
        int user_access_level = 0, HR_Status = 0, Manager_Status = 0;

        try
        {
            user_name = HttpContext.Current.Session["username"].ToString();
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            employee_name = HttpContext.Current.Session["employee_name"].ToString();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

            if (user_access_level == 0)
            {
                employee_name = "Admin";
                query = "Update outoffoffice set Status=" + status_flag + ",Manager_Name='Admin',HR_Name='Admin'  where OOO_ID=" + ooo_id;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }

            if (user_access_level == 1)
            {
                query = "Update outoffoffice set Manager_Status=" + status_flag + ",Manager_Name='" + employee_name + "',Manager_Remark='" + comments + "' where OOO_ID=" + ooo_id;
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                if (status_flag == 2)
                {
                    query = "Select HR_Status from outoffoffice where OOO_ID=" + ooo_id;
                    HR_Status = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                    if (HR_Status == 3)
                    {
                        query = "Update outoffoffice set Status=" + status_flag + " where OOO_ID=" + ooo_id;
                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                        /*insert approved punch into master trans raw# as well as process table*/
                        approved_list.Add("OOO_ID", ooo_id);
                        db_connection.ExecuteStoredProcedureReturnStringNoPI("SaveApprovedOOO", approved_list);

                    }
                    if (HR_Status == 0)
                    {
                        query = "Update outoffoffice set HR_Status=1 where OOO_ID=" + ooo_id;
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                    }
                }
                if (status_flag == 3)
                {
                    query = "Update outoffoffice set Status=3  where OOO_ID=" + ooo_id;
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
            }

            if (user_access_level == 3)
            {
                if (status_flag == 3)
                {
                    query = "Update outoffoffice set HR_Status=3, HR_Name='" + employee_name + "',HR_Remark='" + comments + "'  where OOO_ID=" + ooo_id;
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    query = "Update outoffoffice set Manager_Status=1 where OOO_ID=" + ooo_id;
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
                if (status_flag == 2)
                {
                    query = "Update outoffoffice set Status=2,HR_Status=2,HR_Name='" + employee_name + "',HR_Remark='" + comments + "'  where OOO_ID=" + ooo_id;
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    /*insert approved punch into master trans raw# as well as process table*/
                    approved_list.Add("OOO_ID", ooo_id);
                    db_connection.ExecuteStoredProcedureReturnStringNoPI("SaveApprovedOOO", approved_list);

                }
            }

            SendMail(ooo_id, status_flag);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "APPROVE_MISSING_PUNCH");
        }
    }

    private void SendMail(long ooo_id, int status_flag)
    {
        Hashtable leave_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();
        DataTable out_off_office_data = new DataTable();
        DataTable dt_temp = new DataTable();
        string
            mailTo = string.Empty, mailCC = string.Empty, HR_ID = string.Empty, branch_code = string.Empty,
            email_id = string.Empty, query = string.Empty, outofoffice_status = string.Empty, Alternate_HR_ID = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty, action_employee_name = string.Empty,
            FromDateTime = string.Empty, ToDateTime = string.Empty, Reason = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty, employee_id = string.Empty;
        int access_level = 0, HR_Status = 0;

        try
        {
            //get the out_off_office details by ID and save it in MailSender table
            query = "select * from outoffoffice where OOO_ID=" + ooo_id;
            out_off_office_data = db_connection.ReturnDataTable(query);

            employee_id = out_off_office_data.Rows[0]["Emp_ID"].ToString();
            employee_name = out_off_office_data.Rows[0]["Emp_Name"].ToString();
            FromDateTime = out_off_office_data.Rows[0]["FromDateTime"].ToString();
            ToDateTime = out_off_office_data.Rows[0]["ToDateTime"].ToString();
            Reason = out_off_office_data.Rows[0]["Reason"].ToString();

            switch (status_flag)
            {
                case 2:
                    outofoffice_status = "Approved";
                    break;

                case 3:
                    outofoffice_status = "Rejected";
                    break;
            }

            access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());
            action_employee_name = HttpContext.Current.Session["employee_name"].ToString();

            //Getting the Employee email ID
            query = "select Emp_Email,Emp_Branch from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
            dt_temp = db_connection.ReturnDataTable(query);

            mailCC = dt_temp.Rows[0]["Emp_Email"].ToString();
            branch_code = dt_temp.Rows[0]["Emp_Branch"].ToString();

            /*if manager doing action means 
             * if manager approves mail will trigger to HR and Emp
             * if already HR rejects then manager approves/rejects means mail will trigger to emp directly its depend on HR_Status
             * if manager rejects mail will trigger to emp
             */
            if (access_level == 1)
            {
                if (status_flag == 2)
                {
                    query = "Select HR_Status from outoffoffice where OOO_ID=" + ooo_id;
                    HR_Status = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                    if (HR_Status == 0 || HR_Status == 1)
                    {
                        // Getting HR Email ID and Name
                        query = "select BM.ManagerID, EM.Emp_Email, EM.Emp_Name as Emp_name From TbManagerHrBranchMapping BM join EmployeeMaster EM on BM.ManagerID=em.Emp_Code where BM.BranchCode='" + branch_code + "'";
                        dt_temp = db_connection.ReturnDataTable(query);

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

                        mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
                        manager_name = dt_temp.Rows[0]["Emp_name"].ToString();

                        leave_send_mail.Add("ToEmailID", mailTo);
                        leave_send_mail.Add("CCEmailID", mailCC);

                        MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                        MailBody = MailBody + "The following Out Off Office punch has been " + outofoffice_status + " by " + action_employee_name + " and submitted for your action.<br/><br/>";
                    }
                    if (HR_Status == 3)
                    {
                        //Getting the Employee email ID
                        query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
                        mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);

                        leave_send_mail.Add("ToEmailID", mailTo);
                        leave_send_mail.Add("CCEmailID", mailTo);

                        MailBody = MailBody + "Dear " + employee_name + ", <br/><br/>";
                        MailBody = MailBody + "The following Out Off Office punch has been " + outofoffice_status + " by " + action_employee_name + "<br/><br/>";
                    }
                }

                if (status_flag == 3)
                {
                    //Getting the Employee email ID
                    query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
                    mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);

                    leave_send_mail.Add("ToEmailID", mailTo);
                    leave_send_mail.Add("CCEmailID", mailTo);

                    MailBody = MailBody + "Dear " + employee_name + ", <br/><br/>";
                    MailBody = MailBody + "The following Out Off Office punch has been " + outofoffice_status + " by " + action_employee_name + "<br/><br/>";
                }
            }

            if (access_level == 3)//HR
            {
                if (status_flag == 2)
                {
                    leave_send_mail.Add("ToEmailID", mailCC);
                    leave_send_mail.Add("CCEmailID", mailCC);

                    mailTo = mailCC;
                }

                if (status_flag == 3)
                {
                    // Getting Manager Email ID and Name
                    query = "select Emp_Email,Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                    dt_temp = db_connection.ReturnDataTable(query);

                    mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
                    manager_name = dt_temp.Rows[0]["Emp_name"].ToString();

                    leave_send_mail.Add("ToEmailID", mailTo);
                    leave_send_mail.Add("CCEmailID", mailCC);
                }

                MailBody = MailBody + "Dear " + employee_name + ", <br/><br/>";
                MailBody = MailBody + "The following Out Off Office punch has been " + outofoffice_status + " by " + action_employee_name + "<br/><br/>";
            }

            if (!string.IsNullOrEmpty(mailTo))
            {
                MailSubject = "Regarding Out Off Office Entry Application";

                MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
                MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
                MailBody = MailBody + "From Date Time: " + FromDateTime + "<br/><br/>";
                MailBody = MailBody + "To Date Time: " + ToDateTime + "<br/><br/>";
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
            Logger.LogException(ex, page, "SEND_MANUAL_PUNCH");
        }
    }

    private string CreateExport(DataTable ooo_data, int access_level)
    {
        DateTime now = DateTime.Now;
        string
          user_id = HttpContext.Current.Session["employee_id"].ToString(),
          file_name = "OutOfOffice-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
        if (access_level == 1)
        {
            // Initializing the column names for the export. 
            string[] column_names =
                new string[] { "Employee ID", "Employee Name", "OutOfOffice Type", "From DateTime", "To DateTime", "Hours", "TotalHours Availed", "Reason", "Manager Status" };


            ExcelExport.ExportDataToExcel(file_name, "OUT OF OFFICE", ooo_data, Context, column_names);
        }
        if (access_level == 3)
        {
            // Initializing the column names for the export. 
            string[] column_names =
                new string[] { "EmployeeID", "EmployeeName", "OutOfOffice Type", "FromDateTime", "ToDateTime", "Hours", "TotalHours Availed", "Reason", "Manager Name", "Manager Remark", "Manager Status", "HR Status" };



            ExcelExport.ExportDataToExcel(file_name, "OUT OF OFFICE", ooo_data, Context, column_names);
        }

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport()
    {
        outofoffice_approve page_object = new outofoffice_approve();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable ooo_data = new DataTable();
        DateTime now = DateTime.Now;
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]),
            user_access_level = 0;

        string[] column_names = new string[] { };

        string employee_id = string.Empty,
            query = string.Empty, file_name = string.Empty;

        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        employee_id = HttpContext.Current.Session["employee_id"].ToString();

        try
        {
            query = " select TOP " + export_limit + " Emp_ID 'EmployeeID',Emp_Name 'EmployeeName',OOO_type_name 'OutOfOffice Type',FromDateTime ,ToDateTime,";

            query += " CONVERT(VARCHAR, RIGHT('0' +  RTRIM(OOO_Minutes/60),3) + ':' + RIGHT('0' + RTRIM(OOO_Minutes%60),2)) as Hours, ";
            query += " CONVERT(VARCHAR, RIGHT('0' +  RTRIM(Total_OOO_Minutes/60),3) + ':' + RIGHT('0' + RTRIM(Total_OOO_Minutes%60),2)) as TotalHours, ";

            query += " Reason, ";

            if (user_access_level == 3)
            {
                query += " Manager_Name 'Manager Name',Manager_Remark 'Manager Remark', ";
                query += " case when Manager_Status=1 then 'Submiited' when Manager_Status=2 then 'Approved' when Manager_Status=3 then 'Rejected' end as Approval,";
                query += " case when HR_Status=1 then 'Submiited' when HR_Status=2 then 'Approved' when HR_Status=3 then 'Rejected' else 'Manager Action Pending' end as HRApproval";
            }
            if (user_access_level == 1)
            {
                query += " case when Manager_Status=1 then 'Submiited' when Manager_Status=2 then 'Approved' when Manager_Status=3 then 'Rejected' end as Approval";
            }

            query += " from outoffoffice O join OOOType OT on O.OOO_type=OT.OOO_type_id ";

            if (user_access_level == 3)
            {
                query += "where Emp_ID in (select Emp_Code from EmployeeMaster where Emp_Branch=(select Emp_Branch from EmployeeMaster where Emp_Code='" + employee_id + "' ))";
            }
            if (user_access_level == 1)
            {
                query += "where Emp_ID in (select Emp_Code from EmployeeMaster where ManagerID='" + employee_id + "' )";
            }

            ooo_data = db_connection.ReturnDataTable(query);

            query += "order by OOO_ID ";

            if (ooo_data.Rows.Count > 0)
            {
                file_name = page_object.CreateExport(ooo_data, user_access_level);

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
