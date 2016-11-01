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

public partial class overtime_approve : System.Web.UI.Page
{
    const string page = "OVERTIME_APPROVAL";
    bool is_filter = true;

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

            message = "An error occurred while loading Overtime approval page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetDepartmentData(string company_code)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable department_data_table = new DataTable();
        string department_query = string.Empty;

        try
        {
            department_query = " select DeptCode, deptName FROM deptmaster where companycode='" + company_code + "'";

            department_data_table = db_connection.ReturnDataTable(department_query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(department_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_DEPARTMENT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Department data. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetDesignationData(string company_code)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable designation_data_table = new DataTable();
        string designation_query = string.Empty;

        try
        {
            designation_query = " select desigcode, designame FROM desigmaster where companycode='" + company_code + "'";

            designation_data_table = db_connection.ReturnDataTable(designation_query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(designation_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_DESIGNATION");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Designation data. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {

        DataTable company_data = new DataTable();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty, employee_id = string.Empty, company_code = string.Empty;

        try
        {

            employee_id = HttpContext.Current.Session["username"].ToString();

            //load company list as per employee
            if (employee_id != "admin")
            {
                query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select CompanyCode as CompanyCode, CompanyName as CompanyName from CompanyMaster where CompanyCode='" + company_code + "' order by CompanyName ";
            }
            else
            {
                query = "select CompanyCode as CompanyCode, CompanyName as CompanyName from CompanyMaster";
            }
            company_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(company_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_COMPANY_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company Data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
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
                query += " and a.EmpID= '" + filter_keyword + "' ";
            }
            else if (filter_by == 2)
            {
                query += " and a.EmpName like '%" + filter_keyword + "%' ";
            }

            if (filter_LeaveStatus != 0)
            {
                query += " and a.Approval= '" + filter_LeaveStatus + "'  and a.mFlag='" + filter_LeaveStatus + "' ";
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
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_FILTER_QUERY");
        }

        return query;
    }

    private string GetBaseQuery()
    {
        string query = @"select Overtimeid, OTDate, EmpID, EmpName, OtHrs, Status, Approval, modifiedby,mFlag,row from
                        ( SELECT O.Overtimeid, O.OTDate, O.OTDate[OT Date], O.EMPID as 'EmpID', E.Emp_Name  as 'EmpName', O.OtHrs,O.mFlag, case when o.Flag=1
                        and o.MFlag =1 then 'Submitted' when o.Flag=1 and o.MFlag =2 then 'Approved by fist manager' when o.Flag=3 and o.MFlag =3 then
                        'Declined' when o.Flag=2 and o.MFlag =2 then 'Approved' when o.Flag=4 and o.MFlag =4 then 'Cancelled' when o.Flag=2 and o.MFlag =0 then 'Approved'  
                         when o.Flag=2 and o.MFlag =1 then 'Approved' end  as Status,O.Flag as Approval, O.modifiedby,
                       ROW_NUMBER() OVER (ORDER BY O.Overtimeid desc) as row From Overtime O JOIN EmployeeMaster E on E.Emp_Code=O.Empid join leave_status Ls on
						  Ls.Leave_Status_id = o.MFlag where 1=1 ";
        return query;
    }

    [WebMethod]
    public static ReturnObject getOvertimeData(int page_number, bool is_filter, string filters)
    {
        overtime_approve page_object = new overtime_approve();
        is_filter = true;
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable overtime_data = new DataTable();
        DataTable branch_list_table = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string
           user_name = string.Empty,
           company_code = string.Empty,
           department_code = string.Empty,
            designation_code = string.Empty,
            from_date = string.Empty,
            employeecode = string.Empty,
            employeename = string.Empty,
            to_date = string.Empty,
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

            if (is_filter && filters.Length > 2)
            {
                JObject filter_data = new JObject();

                filter_data = JObject.Parse(filters);
                company_code = filter_data["filter_CompanyCode"].ToString();
                department_code = filter_data["filter_DepartmentCode"].ToString();
                designation_code = filter_data["filter_DesignationCode"].ToString();
                //int filter_by = Convert.ToInt32(filter_data["filter_by"]);
                //string filter_keyword = filter_data["filter_keyword"].ToString();
                employeecode = filter_data["employee_id"].ToString();
                employeename = filter_data["employee_name"].ToString();
                int filter_View = Convert.ToInt32(filter_data["filter_View"]);
                int filter_Requirement = Convert.ToInt32(filter_data["filter_Requirement"]);

                from_date = DateTime.ParseExact(filter_data["filter_from"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                to_date = DateTime.ParseExact(filter_data["filter_to"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                // query = page_object.GetBaseQuery();
                Hashtable filter_conditions = new Hashtable();
                filter_conditions.Add("employeeid", employeecode);
                filter_conditions.Add("FromDate", from_date);
                filter_conditions.Add("ToDate", to_date);

               // db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("OvertimeCalculation", filter_conditions);
                query = "select empid as employeeId,Weekdays ,OTDate as WeekDate, TimingFrom , TimingTo ,   RoundedHours , WithinLegal as WithinLegalRequirementTime   ,AboveLegal as AboveLegalRequirementTime  , ShortageWorkingHours , WLR_Confirmed , ALR_Confirmed from Overtime where empid='" + employeecode + "' and  otdate between '" + from_date + "'  and '" + to_date + "'order by OTDate";
                if (filter_View == 1)
                {
                    //query = query + "  where  IsOvertimeDay= 1 ";


                }
                //else
                //{
                //    if (user_access_level == 0)//Admin
                //    {
                //        query += " ";
                //    }
                //    else if (user_access_level == 3)//HR
                //    {
                //        query += " and  O.EmpID in ( select Emp_Code from EmployeeMaster where Emp_Branch in (" + BranchList + ")  or O.EmpID='" + employee_id + "' or ManagerID='" + employee_id + "' or ManagerID is null or ManagerID ='' ) ";
                //    }
                //    else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
                //    {
                //        query += "and O.EmpID in (select emp_code from employeemaster where ManagerId In ('" + employee_id + "'," + CoManagerID + ") ) or (O.EMPID In(Select Emp_Code From EmployeeMaster Where Emp_Code Not In ('" + employee_id + "') And ManagerID In(" + InnerManagers + "))  Or e.Emp_Branch In(" + BranchList + ")) ";
                //    }
                //    else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager 
                //    {
                //        query += " and O.EmpID in (select Emp_Code from EmployeeMaster where ManagerId In ('" + employee_id + "')  ) or (O.EmpID In(Select Emp_Code From EmployeeMaster Where Emp_Code Not In('" + employee_id + "') And ManagerID In(" + InnerManagers + ") )   Or (e.Emp_Branch In(" + BranchList + " )and o.MFlag =1)) ";

                //    }
                //    else
                //    {
                //        query += " 1=0 ";// Only Employee
                //    }
                //}

                //query += " ) a where row > " + start_row + " and row < " + number_of_record;

                //if (filter_by == 1)
                //{
                //    query += " and a.EmpID= '" + filter_keyword + "' ";
                //}
                //else if (filter_by == 2)
                //{
                //    query += " and a.Emp_Name like '%" + filter_keyword + "%' ";
                //}


            }
            //else
            //{
            //    query = page_object.GetBaseQuery();

            //    //modify query as per access level
            //    if (user_access_level == 0)//Admin
            //    {
            //        query += " and (o.Flag=1 and o.MFlag =1)";
            //    }
            //    else if (user_access_level == 3)//HR
            //    {
            //        query += " and (o.Flag=1 or o.MFlag =1) and O.EmpID in ( select Emp_Code from EmployeeMaster where Emp_Branch in (" + BranchList + ")  or O.EmpID='" + employee_id + "' or ManagerID='" + employee_id + "' or ManagerID is null or ManagerID ='' ) ";
            //    }
            //    else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
            //    {
            //        query += "and O.EmpID in (select emp_code from employeemaster where ManagerId In ('" + employee_id + "'," + CoManagerID + ") and (o.Flag=1 or o.MFlag =1)) or (O.EMPID In (Select Emp_Code From EmployeeMaster Where Emp_Code Not In ('" + employee_id + "') And ManagerID In(" + InnerManagers + ") and (o.Flag=1 and o.MFlag =2))  Or e.Emp_Branch In(" + BranchList + ")) ";
            //    }
            //    else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager 
            //    {
            //        query += " and O.EmpID in (select Emp_Code from EmployeeMaster where ManagerId In ('" + employee_id + "') and (o.Flag=1 and o.MFlag =1)) or (O.EmpID In (Select Emp_Code From EmployeeMaster Where Emp_Code Not In('" + employee_id + "') And ManagerID In (" + InnerManagers + ") and (o.Flag=1 and o.MFlag =2))   Or e.Emp_Branch In(" + BranchList + ")) ";
            //    }
            //    else
            //    {
            //        query += " 1=0 ";// Only Employee
            //    }

            //    query += " ) a where row > " + start_row + " and row < " + number_of_record;
            //}

            //query += " order by a.OTDate desc";
            //query = "select empid as employeeId,Weekdays ,OTDate as WeekDate, TimingFrom , TimingTo ,   RoundedHours , WithinLegal as WithinLegalRequirementTime   ,AboveLegal as AboveLegalRequirementTime  , ShortageWorkingHours , WLR_Confirmed , ALR_Confirmed from Overtime where empid='10' and  otdate between '2016-07-01'  and '2016-07-31'order by OTDate";
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

    public void updateOvertime(int overtime_id, int flag, string overtime_status)
    {
        DBConnection db_connection = new DBConnection();
        string employee_name = string.Empty;
        string query = string.Empty;
        int oflag, mflag, access_level = 0;

        try
        {
            access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());

            query = "select Flag from OverTime where Overtimeid=" + overtime_id;
            oflag = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            query = "select mFlag from OverTime where Overtimeid=" + overtime_id;
            mflag = db_connection.ExecuteQuery_WithReturnValueInteger(query);

            employee_name = HttpContext.Current.Session["employee_name"].ToString();

            if (access_level == 0 || access_level == 3)//HR or Admin
            {
                query = "Update Overtime set flag = " + flag + ",mflag = " + flag + ", modifiedby = '" + employee_name + "' where Overtimeid = " + overtime_id + "";
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                sendMailForwarded(overtime_id, overtime_status);
            }
            else if (oflag == 1 && mflag == 1 && flag == 2)
            {
                query = "Update Overtime set mflag = " + flag + ", modifiedby = '" + employee_name + "' where Overtimeid = " + overtime_id + "";
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                sendMailForwarded(overtime_id, overtime_status);
            }
            else if (oflag == 1 && mflag == 2 && flag == 2)
            {
                query = "Update Overtime set flag = " + flag + ", modifiedby = '" + employee_name + "' where Overtimeid = " + overtime_id + "";
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                sendMail(overtime_id, overtime_status);
            }
            if (flag == 3 || flag == 4)
            {
                if (mflag == 1)
                {
                    query = "Update Overtime set mflag = " + flag + ",flag = " + flag + ", modifiedby = '" + employee_name + "' where Overtimeid = " + overtime_id + "";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                    sendMail(overtime_id, overtime_status);
                }
                else
                {
                    query = "Update Overtime set flag = " + flag + ", modifiedby = '" + employee_name + "' where Overtimeid = " + overtime_id + "";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    sendMail(overtime_id, overtime_status);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_OVERTIME");
        }
    }

    protected void sendMail(int OverTimeID, string overtime_status)
    {
        DBConnection db_connection = new DBConnection();
        JObject json_data = new JObject();
        Hashtable leave_send_mail = new Hashtable();
        DataTable overtime_data_table = new DataTable();
        DateTime OTDate = new DateTime();

        string employee_id = string.Empty,
         employee_name = string.Empty,
         overtime_date = string.Empty,
         OTHours = string.Empty,
         MailSubject = string.Empty,
         MailBody = string.Empty,
         employee_email = string.Empty,
         query = string.Empty,
         manager_email = string.Empty;

        int Flag = 0;

        try
        {
            query = "select EMPID,OTDate,OTHrs,EmpName from Overtime where Overtimeid='" + OverTimeID + "'";
            overtime_data_table = db_connection.ReturnDataTable(query);

            if (overtime_data_table.Rows.Count > 0)
            {
                employee_id = overtime_data_table.Rows[0]["EMPID"].ToString();
                employee_name = overtime_data_table.Rows[0]["EmpName"].ToString();
                overtime_date = Convert.ToDateTime(overtime_data_table.Rows[0]["OTDate"]).ToString("dd/MM/yyyy");
                OTHours = Convert.ToDateTime(overtime_data_table.Rows[0]["OTHrs"]).ToString("HH:mm");

                employee_email = db_connection.ExecuteQuery_WithReturnValueString("select Emp_Email from EmployeeMaster where Emp_Code = '" + employee_id + "' ");

                manager_email = db_connection.ExecuteQuery_WithReturnValueString("select Emp_Email  from EmployeeMaster where Emp_Code = (select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')");

                if (!string.IsNullOrEmpty(manager_email))
                {
                    MailSubject = "Regarding Overtime " + overtime_status;
                    MailBody = "Dear " + employee_name;
                    MailBody += "<br/><br/> Overtime is " + overtime_status + " for the date which is as per the following :";
                    MailBody += "<br/><br/>Employee ID" + employee_id;
                    MailBody += "<br/><br/>Overtime Date: " + OTDate;
                    MailBody += "<br/><br/>OT Hrs: " + OTHours + " <br/><br/>Thanks";

                    leave_send_mail.Add("EmpID", employee_id);
                    leave_send_mail.Add("EmpName", employee_name);
                    leave_send_mail.Add("ToEmailID", manager_email);
                    leave_send_mail.Add("CCEmailID", employee_email);
                    leave_send_mail.Add("Subject", MailSubject);
                    leave_send_mail.Add("Body", MailBody);

                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", leave_send_mail);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SEND_MAIL");
        }

    }

    protected void sendMailForwarded(int OverTimeID, string overtime_status)
    {
        DBConnection db_connection = new DBConnection();
        JObject json_data = new JObject();
        Hashtable leave_send_mail = new Hashtable();
        DataTable overtime_data_table = new DataTable();
        DateTime OTDate = new DateTime();

        string employee_id = string.Empty,
         employee_name = string.Empty,
         overtime_date = string.Empty,
         OTHours = string.Empty,
         MailSubject = string.Empty,
         MailBody = string.Empty,
         employee_email = string.Empty,
         query = string.Empty,
         manager_email = string.Empty,
         immediate_mgr_email = string.Empty, manager_name = string.Empty,
         manager_id = string.Empty, Mailcc = string.Empty;

        int Flag = 0;

        try
        {
            query = "select EMPID,OTDate,OTHrs,EmpName from Overtime where Overtimeid='" + OverTimeID + "'";
            overtime_data_table = db_connection.ReturnDataTable(query);

            if (overtime_data_table.Rows.Count > 0)
            {
                employee_id = overtime_data_table.Rows[0]["EMPID"].ToString();
                employee_name = overtime_data_table.Rows[0]["EmpName"].ToString();
                overtime_date = Convert.ToDateTime(overtime_data_table.Rows[0]["OTDate"]).ToString("dd/MM/yyyy");
                OTHours = Convert.ToDateTime(overtime_data_table.Rows[0]["OTHrs"]).ToString("HH:mm");

                employee_email = db_connection.ExecuteQuery_WithReturnValueString("select Emp_Email from EmployeeMaster where Emp_Code = '" + employee_id + "' ");
                if (!string.IsNullOrEmpty(employee_email))
                {
                    Mailcc = employee_email;
                }
                manager_email = db_connection.ExecuteQuery_WithReturnValueString("select Emp_Email  from EmployeeMaster where Emp_Code = (select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')");


                //Getting manager id
                query = "select Emp_code from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                manager_id = db_connection.ExecuteQuery_WithReturnValueString(query);

                //Getting manager name
                query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

                //Getting immediate manager EMAILID
                query = "select Emp_email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + manager_id + "')";
                immediate_mgr_email = db_connection.ExecuteQuery_WithReturnValueString(query);

                if (!string.IsNullOrEmpty(immediate_mgr_email))
                {
                    Mailcc = immediate_mgr_email;
                }

                if (!string.IsNullOrEmpty(manager_email))
                {
                    MailSubject = "Regarding  Overtime ";
                    MailBody = "Dear  Sir/Madam";
                    MailBody += "<br/><br/> Overtime has been " + overtime_status + " by the Manager " + manager_name + " and forwarded for your Approval :";
                    MailBody += "<br/><br/>Employee ID" + employee_id;
                    MailBody += "<br/><br/>Overtime Date: " + OTDate;
                    MailBody += "<br/><br/>OT Hrs: " + OTHours + " <br/><br/>Thanks";

                    leave_send_mail.Add("EmpID", employee_id);
                    leave_send_mail.Add("EmpName", employee_name);
                    leave_send_mail.Add("ToEmailID", manager_email);
                    leave_send_mail.Add("CCEmailID", Mailcc);
                    leave_send_mail.Add("Subject", MailSubject);
                    leave_send_mail.Add("Body", MailBody);

                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", leave_send_mail);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SEND_MAIL");
        }

    }


    [WebMethod]
    public static ReturnObject DoContinue(string current,string filterFromDate,string filterToDate,string empid)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();

        try
        {
             
            DataSet return_data_set = (DataSet)JsonConvert.DeserializeObject(current, (typeof(DataSet)));
            DataTable OT = return_data_set.Tables[0];
            
            if (OT.Rows.Count>0)
            {
                db_connection.ExecuteSPWithDatatable(OT, "", "UpdateFirstOT");
                return_object.status = "success";
                
            }
        }
       
        catch (Exception ex)
        {
            return_object.status = "error";
        }
        
        return return_object;
    }

    [WebMethod]
    public static ReturnObject DoContinueForOt2()
    {
        overtime_approve page_object = new overtime_approve();
       
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable overtime_data = new DataTable();
        DataTable branch_list_table = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string
           user_name = string.Empty,
           company_code = string.Empty,
           department_code = string.Empty,
            designation_code = string.Empty,
            from_date = string.Empty,
            employeecode = string.Empty,
            employeename = string.Empty,
            to_date = string.Empty,
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

            
             



            query = "SELECT  EmployeeID,EmployeeName,WeekDates,WeeklyMandatoryHours,ActualWeeklyRoundedHours ,RejectedOT,WithinLegalLimits,AboveLegalLimits,ShortageofCompletedWorkingHours,TotalRegularOT,TotalNightOT,WithinLegal,AboveLegal,PublicHolidayOvertime,WeekendcompensatewithCompOffDays,PublicHolidayscompensatewithCompOffDays FROM OverTime_MonthCalculation";


            overtime_data = db_connection.ReturnDataTable(query);
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(overtime_data, Formatting.Indented);

        }
        catch(Exception e)
        {
            return_object.status = "error";
            //return_object.return_data = JsonConvert.SerializeObject(overtime_data, Formatting.Indented);
        }


      
        return return_object;

    }

    [WebMethod]
    public static ReturnObject DoFinalContinue(string current)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();

        try
        {

            DataSet return_data_set = (DataSet)JsonConvert.DeserializeObject(current, (typeof(DataSet)));
            DataTable OT = return_data_set.Tables[0];

            if (OT.Rows.Count > 0)
            {
                db_connection.ExecuteSPWithDatatable(OT, "", "UpdateSecondOT");
                return_object.status = "success";

            }
        }

        catch (Exception ex)
        {
            return_object.status = "error";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject getFromDate(string EmployeeId, string companyCode)
    {
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty,resultdate=string.Empty;
        DBConnection db_connection = new DBConnection();
        int record_count = 0;

        try
        {
            query = "Select count(*) from EmployeeMaster where Emp_Company='" + companyCode + "' and Emp_Code='" + EmployeeId + "'";
            record_count = db_connection.GetRecordCount(query);

            if (record_count > 0)
            {
                query = "select min(otdate)from Overtime where empid='" + EmployeeId + "' and WLR_Confirmed=1 and ALR_Confirmed=1";
                resultdate = db_connection.ExecuteQuery_WithReturnValueString(query);
                return_object.return_data = resultdate;
                return_object.status = "success";
            }
            else
            {
                return_object.status = "failure";
                return_object.return_data = "Employee doesn't belong selected company";
            }
        }

        catch (Exception ex)
        {
            return_object.status = "error";
        }
       
        return return_object;
    }



    [WebMethod]
    public static ReturnObject DoAction(int action, string comments, string selected_rows)
    {
       overtime_approve page_object = new overtime_approve();
        List<string> selected_overtime = JsonConvert.DeserializeObject<List<string>>(selected_rows);
        ReturnObject return_object = new ReturnObject();
        DateTime OTDate = new DateTime();
        DBConnection db_connection = new DBConnection();

        string
            query = string.Empty, return_message = string.Empty,
            overtime_status = string.Empty;

        int overtime_id = 0, ot_flag = 0, mot_flag = 0;

        try
        {
            switch (action)
            {
                case 2:
                    overtime_status = "Approved";
                    break;

                case 3:
                    overtime_status = "Declined";
                    break;

                case 4:
                    overtime_status = "Cancelled";
                    break;
            }

            return_message = string.Empty;

            for (int i = 0; i < selected_overtime.Count; i++)
            {
                overtime_id = Convert.ToInt32(selected_overtime[i].ToString());

                query = "select Flag from OverTime where overtimeid='" + overtime_id + "'";
                ot_flag = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                query = "select mFlag from OverTime where overtimeid='" + overtime_id + "'";
                mot_flag = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                if (ot_flag == action)
                {
                    return_message += "OverTime is alread " + overtime_status + Environment.NewLine;
                }
                else
                {
                    page_object.updateOvertime(overtime_id, action, overtime_status);
                    if (mot_flag == 1 && ot_flag == 1)
                    {
                        return_message += "Overtime " + overtime_status + " Forwarded successfully for Manager." + Environment.NewLine;
                    }
                    else
                    {
                        return_message += "Overtime " + overtime_status + " successfully for selected Employee's." + Environment.NewLine;
                    }

                }
            }

            page_object.Dispose();

            return_object.status = "success";
            return_object.return_data = return_message;//JsonConvert.SerializeObject("Overtime " + overtime_status + " successfully for selected Employee's.", Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DoAction");

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
            employee_id = HttpContext.Current.Session["employee_id"].ToString();

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
