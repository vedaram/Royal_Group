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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;
using SecurAX.Export.Excel;
using System.IO;
using ClosedXML.Excel;
using SecurAX.Import.Excel;
using System.Windows.Forms;

public partial class leave_apply : System.Web.UI.Page
{
    const string page = "LEAVE_APPLICATION";

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

            message = "An error occurred while loading Leave Application page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject ValidateEmployeeId(string employee_id)
    {
        leave_apply page_object = new leave_apply();
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



                /* if (txtempWorkDate.Text != string.Empty)
                 {
                     DBCom.getscalar("select chkifNormalShift from Shift where Shift_Code = (Select Shift_Code from MASTERPROCESSDAILYDATA where Emp_ID = '" + EmpCode + "' and PDate ='" + DBCom.DateTimeToString_DB(txtempWorkDate.Text) + "'");
                 }*/

                if (employee_id.Trim() != HttpContext.Current.Session["employee_id"].ToString())
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString()) == 1)
                    {
                        if (!db_connection.RecordExist("select count(*) from employeeMaster where emp_Code='" + employee_id.Trim() + "' and emp_status=1 and (managerid In('" + HttpContext.Current.Session["employee_id"].ToString() + "'," + CoManagerID + ") Or Emp_Branch In(" + BranchList + "))"))
                        {
                            return_object.status = "error";
                            return_object.return_data = "Entered Employee id does not belongs to this manager.";
                            return return_object;

                        }
                    }
                    else if (Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString()) == 0)
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
    public static ReturnObject GetLeaveType(string employee_id)
    {
        leave_apply PageObject = new leave_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable Leave_Types = new DataTable();
        string query = string.Empty; //to fetch employeecategory
        string leavequery = string.Empty; //to fetch leavenames
        string employee_category = string.Empty, Emp_Gender = string.Empty;

        try
        {
            if (db_connection.RecordExist("select count(*) from employeeMaster where emp_Code='" + employee_id + "'"))
            {
                query = "select emp_employee_category from employeemaster where emp_Code='" + employee_id + "' ";
                employee_category = db_connection.ExecuteQuery_WithReturnValueString(query);

                if (!string.IsNullOrEmpty(employee_category))
                {
                    query = "select Emp_Gender from employeemaster where emp_Code='" + employee_id + "' ";
                    Emp_Gender = db_connection.ExecuteQuery_WithReturnValueString(query);

                    if (Emp_Gender.ToUpper() == "MALE")
                    {
                        leavequery = "SELECT LeaveCode, LeaveName FROM LeaveMaster where (LeaveCode not in ('OD','V') and employeecategorycode='" + employee_category + "' and LeaveName not like '%Maternity%' ) or LeaveCode = 'CO' ";
                    }
                    else
                    {
                        leavequery = "SELECT LeaveCode, LeaveName FROM LeaveMaster where (LeaveCode not in ('OD','V') and employeecategorycode='" + employee_category + "' and LeaveName not like '%Paternity%' ) or LeaveCode = 'CO' ";
                    }
                    Leave_Types = db_connection.ReturnDataTable(leavequery);
                }
                else
                {
                    leavequery = "SELECT LeaveCode, LeaveName FROM LeaveMaster where LeaveCode = 'CO' ";
                    Leave_Types = db_connection.ReturnDataTable(leavequery);
                }
            }
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(Leave_Types, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_LEAVE_TYPE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leave Types. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetAvailableLeaves(string employee_id)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable leave_data = new DataTable();
        string query = string.Empty;

        try
        {
            //query = "select l.leavename as Leavetype, e.Leave_balance from Employee_Leave e join LeaveMaster l on e.Leave_code=l.LeaveCode where emp_code='" + employee_id + "'";

            query = "select  LM.leavename as Leavetype, EL.Leave_balance from Employee_Leave EL join LeaveMaster LM on";
            query += "  EL.Leave_code=LM.LeaveCode and EL.Emp_code='" + employee_id + "' where (LeaveCode not in ('OD','V') and LM.EmployeeCategoryCode= ";
            query += " (Select Emp_Employee_Category from EmployeeMaster where Emp_code='" + employee_id + "')  ) or LeaveCode = 'CO'";
            leave_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leave_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_AVAILABLE_LEAVES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Available Leaves. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    private int CheckForHoliday(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string brnchquery = string.Empty;
        string holgrpquery = string.Empty;
        string holidaysquery = string.Empty;
        string getempbranch = string.Empty;
        string holiday_group = string.Empty;

        //Leave_from = DateTime.ParseExact(Leave_from, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
        //Leave_to = DateTime.ParseExact(Leave_to, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

        brnchquery = "select emp_branch from employeemaster where emp_Code='" + employee_id + "' ";
        getempbranch = db_connection.ExecuteQuery_WithReturnValueString(brnchquery);
        try
        {
            if (!string.IsNullOrEmpty(getempbranch))
            {
                holgrpquery = "select holidaycode from branchmaster where branchcode='" + getempbranch + "'";
                holiday_group = db_connection.ExecuteQuery_WithReturnValueString(holgrpquery);

                holidaysquery = "select count(*) from holidaylist where (hdate='" + Leave_from + "' or hdate='" + Leave_to + "') and hgroup='" + holiday_group + "'";
                count = db_connection.GetRecordCount(holidaysquery);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_HOLIDAY");
            throw ex;
        }

        return count;
    }

    private int CheckForWeekoff(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int ret_procedurevalue = 0;
        //Leave_from = DateTime.ParseExact(Leave_from, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
        //Leave_to = DateTime.ParseExact(Leave_to, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

        Hashtable hshParam = new Hashtable();
        hshParam.Add("Empid", employee_id);
        hshParam.Add("date1", Leave_from);
        hshParam.Add("date2", Leave_to);
        try
        {
            ret_procedurevalue = db_connection.ExecuteStoredProcedureReturnInteger("Spvalidateholiday1", hshParam);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECK_FOR_WEEKOFF");
            throw ex;
        }
        return ret_procedurevalue;

    }

    private int CheckForLeave(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        //Leave_from = DateTime.ParseExact(Leave_from, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
        //Leave_to = DateTime.ParseExact(Leave_to, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
        try
        {
            query = "SELECT COUNT(*) FROM Leave1 WHERE EmpID='" + employee_id + "' AND ";
            query += "((CONVERT(datetime,startdate,103) >= CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,startdate,103) <=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,enddate,103) >=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,enddate,103) <=CONVERT(datetime,'" + Leave_to + "',103))) and flag not in (3,4) and leavetype not in (select leavecode from leavemaster where leavetype='WeeklyOff') ";
            count = db_connection.GetRecordCount(query);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_LEAVE");
            throw ex;
        }
        return count;
    }

    private int CheckForLossonpay(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        //Leave_from = DateTime.ParseExact(Leave_from, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
        //Leave_to = DateTime.ParseExact(Leave_to, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
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

    private int CheckForOD(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        //Leave_from = DateTime.ParseExact(Leave_from, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
        //Leave_to = DateTime.ParseExact(Leave_to, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
        try
        {
            query = "SELECT count(*) FROM ODLEAVE WHERE EmpID='" + employee_id + "' AND ";
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

    /*This function is for validating sandwitch rule*/
    public string ValidateLeavedate(string Empid, DateTime FromDate, DateTime Todate, string leavecode)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable ht = new Hashtable();

        int return_count = 0;
        string return_value = "";
        DateTime max_date = new DateTime();
        string query = "";

        try
        {
            query = "select max(PDate) from MASTERPROCESSDAILYDATA where Emp_ID='" + Empid + "'";
            max_date = Convert.ToDateTime(db_connection.ExecuteQuery_WithReturnValueString(query)).Date;

            if (FromDate.Date <= max_date.Date && Todate.Date <= max_date.Date)
            {
                ht.Add("Mode", "P");
            }
            else
            {
                ht.Add("Mode", "F");
            }

            ht.Add("EmpID", Empid);
            ht.Add("from_date", Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd"));
            ht.Add("to_date", Convert.ToDateTime(Todate).ToString("yyyy-MM-dd"));
            ht.Add("LeaveType", leavecode);
            ht.Add("ReturnValue", "");
            return_value = db_connection.ExecuteStoredProcedureReturnStringNoPI("SandwichLeave", ht);

        }
        catch (Exception ex)
        {

        }

        return return_value;

    }

    private void WLLeavesInsert(string Employee_id, string FromDate, string ToDate)
    {
        DBConnection db_connection = new DBConnection();

        DateTime wl_from_date = new DateTime();
        DateTime wl_to_date = new DateTime();
        int date_diff = 0;
        string query = "", wl_date = "";
        wl_from_date = Convert.ToDateTime(FromDate);
        wl_to_date = Convert.ToDateTime(ToDate);
        date_diff = Convert.ToInt32((wl_to_date - wl_from_date).TotalDays);

        for (int i = 0; i <= date_diff; i++)
        {
            wl_date = wl_from_date.ToString("yyyy-MM-dd");
            query = "insert into WLStatusRecord values('" + Employee_id + "','" + wl_date + "','WL',1)";
            db_connection.ExecuteQuery_WithOutReturnValue(query);
            wl_from_date = wl_from_date.AddDays(1);
        }

    }

    private int InsertData(string Employee_id, string LeaveType, string FromDate, string ToDate, string Reason, int approvallevel, int half_day)
    {
        DBConnection db_connection = new DBConnection();
        int recordaffected = 0, leave_id = 0, Auto_Approval;
        string username = Session["username"].ToString();
        string query = string.Empty, wf_code = string.Empty, ManagerID = string.Empty, CoManagerID = string.Empty;
        double total_days = 0.0;
        double comffDayapplied = 0.0;
        ArrayList compoffhas = new ArrayList();

        Hashtable hshParam = new Hashtable();

        hshParam.Add("piMode", "I");
        hshParam.Add("piEmpCode", Employee_id);
        hshParam.Add("piLeaveType", LeaveType);
        hshParam.Add("piFromDate", FromDate);
        hshParam.Add("piToDate", ToDate);
        hshParam.Add("piCreatedBy", username);
        hshParam.Add("piModifiedBy", "");
        hshParam.Add("piReason", Reason);
        hshParam.Add("piapprovallevel", approvallevel);
        hshParam.Add("pihalfday", half_day);

        recordaffected = db_connection.exeStoredProcedure_WithHashtable_ReturnRow("spUpsertLeaveDetails", hshParam);

        query = "Select Leavetype from LeaveMaster where LeaveCode='" + LeaveType + "'";
        string WLType = db_connection.ExecuteQuery_WithReturnValueString(query);
        if (WLType.ToUpper() == "WEEKLYOFF")
        {
            WLLeavesInsert(Employee_id, FromDate, ToDate);
        }


        /*this is to get leave_id to insert into leave transaction table for approval purpose*/
        query = "select Leave_ID from Leave1 where LDate='" + FromDate + "' and LeaveType='" + LeaveType + "' and EMPID='" + Employee_id + "' and Flag=1";
        leave_id = db_connection.ExecuteQuery_WithReturnValueInteger(query);

        /*this is to get WFCode from procedure by passing empid and leavetype*/
        wf_code = db_connection.ExecuteProcedureInOutParameters("ReadApprovalWFCode", Employee_id, LeaveType, "WorkFlowCode");

        if (!string.IsNullOrEmpty(wf_code))
        {
            //check for auto approval
            query = "Select top 1 ApproveLevel from ApprovalLevelMaster where WorkFlowCode ='" + wf_code + "'";
            Auto_Approval = db_connection.ExecuteQuery_WithReturnValueInteger(query);

            if (Auto_Approval == 9)
            {
                /*add record into leave transaction*/
                query = "insert into LeaveTransactionMaster values (" + leave_id + ",'" + wf_code + "',1,9,'System',NULL,CONVERT(DATE,GETDATE()),2) ";
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                /*Approve it from leave1 bcz its auto approve*/
                query = "update Leave1 set Flag=2 where Leave_ID=" + leave_id;
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                Hashtable leave_update = new Hashtable();
                leave_update.Add("Flag", 2);
                leave_update.Add("Leaveid", leave_id);
                leave_update.Add("actioncomment", "Auto Approved");
                leave_update.Add("Imdtmngrflag", 0);
                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn_pi("SpUpdateLeaveStatus", leave_update);

            }
            else
            {
                /*insert record into leave transaction table for approval*/
                query = "select ManagerID from employeemaster where emp_code='" + Employee_id + "'";
                ManagerID = db_connection.ExecuteQuery_WithReturnValueString(query);

                if (!string.IsNullOrEmpty(ManagerID))//if manager id exists
                {
                    query = "Select DelidationManagerID from TbAsignDelegation Where ManagerId='" + ManagerID + "' and CONVERT(DATE,GETDATE()) between Fromdate and Todate";
                    CoManagerID = db_connection.ExecuteQuery_WithReturnValueString(query);

                    if (!string.IsNullOrEmpty(CoManagerID))//if manager is deligated
                    {
                        query = "insert into LeaveTransactionMaster values('" + leave_id + "','" + wf_code + "',0,1,'" + CoManagerID + "','" + ManagerID + "',CONVERT(DATE,GETDATE()),1)";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                    }
                    else    //if manager is not deligated
                    {
                        query = "insert into LeaveTransactionMaster values('" + leave_id + "','" + wf_code + "',0,1,'" + ManagerID + "',NULL,CONVERT(DATE,GETDATE()),1)";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                    }
                }
                else
                {
                    query = "insert into LeaveTransactionMaster values('" + leave_id + "','Auto',0,1,'admin',NULL,CONVERT(DATE,GETDATE()),1)";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
            }
        }

        if (recordaffected > 0 && LeaveType == "CO")
        {

            int affectedrecord = 0;
            total_days = (Convert.ToDateTime(ToDate) - Convert.ToDateTime(FromDate)).TotalDays + 1;
            query = "select dbo.[BalanceLeave]('" + Employee_id + "',CONVERT(datetime,'" + FromDate + "',103),CONVERT(datetime,'" + ToDate + "',103),'" + LeaveType + "')";
            comffDayapplied = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));

            if (total_days == comffDayapplied)
            {
                query = "with CompoffDetailsSplitCTE as(select top " + Convert.ToString(comffDayapplied) + " LFlag,LdateFrom,LdateTo from CompoffDetailsSplit where empid='" + Employee_id + "' and LFlag=0 and (CompoffDate between convert(datetime,dateadd(day,-90,convert(datetime,'" + FromDate + "',103)),103) and convert(datetime,'" + FromDate + "',103) or CompoffDate between convert(datetime,dateadd(day,-90,getdate()),103 ) and getdate()) order by CompoffDate)update CompoffDetailsSplitCTE set LFlag=1,LdateFrom=convert(datetime,'" + FromDate + "',103),LdateTo=convert(datetime,'" + ToDate + "',103)";
                affectedrecord = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            }
            else
            {

                if (compoffhas.Count > 0)
                {
                    for (int i = 0; i < compoffhas.Count; i++)
                    {
                        query = "with CompoffDetailsSplitCTE as(select top  1  LFlag,LdateFrom,LdateTo from CompoffDetailsSplit where empid='" + Employee_id + "' and LFlag=0 and (CompoffDate between convert(datetime,dateadd(day,-90,convert(datetime,'" + Convert.ToDateTime(compoffhas[i]).ToString("yyyy-MM-dd") + "',120)),120) and convert(datetime,'" + Convert.ToDateTime(compoffhas[i]).ToString("yyyy-MM-dd") + "',120) or CompoffDate between convert(datetime,dateadd(day,-90,getdate()),103 ) and getdate()) order by CompoffDate)update CompoffDetailsSplitCTE set LFlag=1,LdateFrom=convert(datetime,'" + FromDate + "',103),LdateTo=convert(datetime,'" + ToDate + "',103)";
                        affectedrecord = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                    }
                }

            }
            
        }

        return recordaffected;
    }

    private void SendMail(string employee_id, string Leavetype, string FromDate, string ToDate, string Reason, int halfday, double[] leaveHistory)
    {
        Hashtable leave_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty,
            Leave_name = string.Empty;
        double noOfLeaveApplied = 0.0, closingLeaveBalance = 0.0, Openingbalance = 0.0;

        // Getting the Employee email ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailCC = email_id;

        // Getting the Employee name
        query = "select Emp_Name from EmployeeMaster where Emp_Code='" + employee_id + "'";
        employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        // Getting Manager Email ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        //Getting manager name
        query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + HttpContext.Current.Session["employee_id"].ToString() + "')";
        manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        // Getting Leave Name
        query = "select LeaveName  from Leavemaster where Leavecode='" + Leavetype + "'";
        Leave_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        //Getting Leave applied from DB
        query = "select dbo.[BalanceLeave]('" + employee_id + "',CONVERT(date,'" + FromDate + "'),CONVERT(date,'" + ToDate + "'),'" + Leavetype + "')";
        noOfLeaveApplied = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));

        //Getting Leave Balance from DB
        query = "select leave_balance from employee_leave where Emp_Code='" + employee_id + "' and Leave_code='" + Leavetype + "'";
        closingLeaveBalance = Convert.ToDouble(Convert.ToString(db_connection.ExecuteQuery_WithReturnValueString(query)));

        Openingbalance = leaveHistory[1];

        if (!string.IsNullOrEmpty(email_id))
            mailTo = email_id;

        if (!string.IsNullOrEmpty(mailTo))
        {
            if (halfday == 1)
            {
                MailSubject = "HalfDay Leave Application has been submitted by  " + employee_name + " for your approval";
                noOfLeaveApplied = 0.5;
            }
            else
            {
                MailSubject = "Leave Application has been submitted by  " + employee_name + " for your approval";
            }
            MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
            MailBody = MailBody + "The following Leave has been Submitted <br/><br/>";
            MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
            MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
            MailBody = MailBody + "LeaveType : " + Leave_name + " <br/><br/>";
            //Leave Details stffs
            MailBody = MailBody + "Applied Leave Details <br/><br/>";
            MailBody = MailBody + "Opening balance: " + Openingbalance + " <br/><br/>";
            MailBody = MailBody + "Consumed Leaves: " + noOfLeaveApplied + " <br/><br/>";
            MailBody = MailBody + "Closing Balance : " + closingLeaveBalance + " <br/><br/>";

            if (halfday == 1)
            {
                MailBody = MailBody + "Date: " + FromDate + "<br/><br/>";
            }
            else
            {
                MailBody = MailBody + "From Date: " + FromDate + "<br/><br/>";
                MailBody = MailBody + "To Date : " + ToDate + "<br/><br/>";
            }
            MailBody = MailBody + "Reason:  " + Reason + "<br/><br/>";

            leave_send_mail.Add("EmpID", employee_id);
            leave_send_mail.Add("EmpName", employee_name);
            leave_send_mail.Add("ToEmailID", mailTo);
            leave_send_mail.Add("CCEmailID", mailCC);
            leave_send_mail.Add("Subject", MailSubject);
            leave_send_mail.Add("Body", MailBody);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", leave_send_mail);
        }
    }

    //function: calculating the no of compoff balance in last 90 days
    private int GetCompoffBalance(string employee_id, string leavetype, string fromdate, string todate)
    {
        //leave_apply PageObject = new leave_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable Leave_Types = new DataTable();
        string query = string.Empty; //to fetch employeecategory
        string leavequery = string.Empty; //to fetch leavenames
        string employee_category = string.Empty;
        int checkcomffcount = 0;


        if (leavetype == "CO")
        {
            query = "select count(*) from CompoffDetailsSplit where empid='" + employee_id + "' and LFlag=0 "
                              + "and CompoffDate between convert(datetime,dateadd(day,-90,convert(datetime,'" + fromdate + "',103)),103)"
                              + "and convert(datetime,'" + todate + "',103) or CompoffDate between"
                              + " convert(datetime,dateadd(day,-90,getdate()),103 ) and getdate()";
            checkcomffcount = db_connection.ExecuteQuery_WithReturnValueInteger(query);

        }
        return checkcomffcount;
    }

    private string[] ValidatingCompoff(string employee_id, string fromdate, double totalnoofdays)
    {
        ArrayList compoffhas = new ArrayList();
        ArrayList compoffNOT = new ArrayList();
        ArrayList WorHolD = new ArrayList();
        DBConnection db_connection = new DBConnection();
        string[] data = new string[2];
        int count = 0;
        string
            query = string.Empty, status = string.Empty, message = string.Empty;
        DateTime startday = new DateTime();


        for (int i = 1; i <= totalnoofdays; i++)
        {
            query = "select dbo.[CheckCompoffDay]('" + employee_id + "',CONVERT(datetime,'" + fromdate + "',120))";
            count = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            if (count > 0)
            {
                compoffhas.Add(startday.ToString());
            }
            else if (count == -1)
            {
                compoffNOT.Add(startday.ToString());
            }
            else if (count == 0)
            {
                WorHolD.Add(startday.ToString());
            }
            startday = startday.AddDays(1);
        }

        if (compoffNOT.Count > 0)
        {
            string datesncompoff = string.Empty;
            for (int k = 0; k < compoffNOT.Count; k++)
            {
                datesncompoff = datesncompoff + Convert.ToDateTime(compoffNOT[k]).ToString("yyyy-MM-dd") + "  ";
            }
            data[0] = "error";
            data[1] = "You can not apply compoff leaves for selected dates because you do not have compoff balance in last 90 days.";
            return data;

        }
        else if (compoffhas.Count == 0)
        {
            data[0] = "error";
            data[1] = "You can not apply compoff for the selected dates because these are either holiday or weekoff.";
            return data;
        }
        else if (compoffhas.Count > 0)
        {
            data[0] = "success";
            data[1] = "";
        }
        return data;

    }

    [WebMethod]
    public static ReturnObject CheckForSandwich(string current)
    {
        leave_apply PageObject = new leave_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        string
           Employee_id = string.Empty, LeaveType = string.Empty,
           FromDate = string.Empty, ToDate = string.Empty,
           display_message = string.Empty, return_value,
           query = string.Empty;

        int half_day = 0;

        double totalnoofdays = 0.0;

        try
        {
            JObject current_data = JObject.Parse(current);
            Employee_id = current_data["employee_id"].ToString();
            LeaveType = current_data["leave_type"].ToString();
            FromDate = current_data["from_date"].ToString();
            ToDate = current_data["to_date"].ToString();
            half_day = Convert.ToInt32(current_data["half_day"]);

            /*if half day don't check sandwich*/
            if (half_day == 0)
            {
                /*Sandwich rule code from here*/
                query = "select dbo.[BalanceLeave]('" + Employee_id + "',CONVERT(date,'" + FromDate + "'),CONVERT(date,'" + ToDate + "'),'" + LeaveType + "')";
                totalnoofdays = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));

                return_value = PageObject.ValidateLeavedate(Employee_id, Convert.ToDateTime(FromDate).Date, Convert.ToDateTime(FromDate).Date, LeaveType);

                string[] return_value_list = return_value.Split(':');

                double return_count = Convert.ToInt32(return_value_list[0].ToString());
                string from_date = return_value_list[1].ToString();
                string to_date = return_value_list[2].ToString();

                if (totalnoofdays != return_count)
                {
                    if (return_count == 0.5)
                    {
                        display_message = return_count + "  Leave will be  deducted for " + from_date;
                    }
                    else
                    {
                        display_message = return_count + "  Leave(s) will be  deducted from " + from_date + " to " + to_date + "";
                    }

                    return_object.status = "confirm-leave-deduction";
                    return_object.return_data = display_message;
                }
                else
                {
                    return_object = SubmitLeave(current);
                }
            }
            else
            {
                return_object = SubmitLeave(current);
            }

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_SANDWICH");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Submitting Leave Application. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static ReturnObject SubmitLeave(string current)
    {
        double[] leaveHistory = new double[3];
        leave_apply PageObject = new leave_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        string
            Employee_id = string.Empty, LeaveType = string.Empty,
            FromDate = string.Empty, ToDate = string.Empty,
            Reason = string.Empty, returnstatus = string.Empty,
            returnmessage = string.Empty, query = string.Empty,
            DisplayMesage = string.Empty, return_value = string.Empty;

        bool continue_flag = true;

        int
            half_day = 0, approvallevel = 1,
            count = 0, leavesubmitflag = 0;

        double
            LeaveBalance = 0.0, totalnoofdays = 0.0, actualapplieddays = 0.0,
            compoffvirtualbalance = 0.0;
        DateTime LeaveEnd_Date = new DateTime();

        try
        {

            JObject current_data = JObject.Parse(current);
            Employee_id = current_data["employee_id"].ToString();
            LeaveType = current_data["leave_type"].ToString();
            FromDate = current_data["from_date"].ToString();
            ToDate = current_data["to_date"].ToString();
            Reason = current_data["reason"].ToString();
            half_day = Convert.ToInt32(current_data["half_day"]);

            /*if half day don't check sandwich*/
            if (half_day == 0)
            {
                /*Sand wich dates*/
                return_value = PageObject.ValidateLeavedate(Employee_id, Convert.ToDateTime(FromDate).Date, Convert.ToDateTime(ToDate).Date, LeaveType);

                string[] return_value_list = return_value.Split(':');

                double return_count = Convert.ToInt32(return_value_list[0].ToString());
                string from_date = return_value_list[1].ToString();
                string to_date = return_value_list[2].ToString();

                FromDate = Convert.ToDateTime(from_date).ToString("dd-MMM-yyyy");
                ToDate = Convert.ToDateTime(to_date).ToString("dd-MMM-yyyy");
                /*Sandwich end*/

                totalnoofdays = (Convert.ToDateTime(ToDate) - Convert.ToDateTime(FromDate)).TotalDays + 1;
            }

            if (half_day == 1)
            {
                /*Check for employee is hald day eligible is there or not*/

                int half_day_eligible = 0;

                half_day_eligible = Convert.ToInt32(db_connection.ExecuteProcedureOneInOutParameters("HalfDayEligibleGroupPolicy", Employee_id, "HalfDayEligible"));

                if (half_day_eligible == 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Sorry, You are not eligible for half day leave.";
                    return return_object;
                }

                ToDate = FromDate;
                totalnoofdays = 0.5;
            }
            else
            {
                totalnoofdays = (Convert.ToDateTime(ToDate) - Convert.ToDateTime(FromDate)).TotalDays + 1;
                //storing no of leaves employee has taken
                leaveHistory[0] = totalnoofdays;
            }

            if (!db_connection.RecordExist("select * from Employee_leave where emp_Code='" + Employee_id + "' and Leave_code='" + LeaveType + "'"))
            {
                return_object.status = "error";
                return_object.return_data = "Leaves are not Assigned for the Employee.";
                return return_object;
            }

            string[] returnarray = PageObject.ValidatingCompoff(Employee_id, FromDate, totalnoofdays);
            //calculating the actual no of leaves 
            //query = "select dbo.[BalanceLeave]('" + Employee_id + "',CONVERT(datetime,'" + FromDate + "',103),CONVERT(datetime,'" + ToDate + "',103),'" + LeaveType + "')";
            query = "select dbo.[BalanceLeave]('" + Employee_id + "',CONVERT(date,'" + FromDate + "'),CONVERT(date,'" + ToDate + "'),'" + LeaveType + "')";
            actualapplieddays = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));

            if (db_connection.RecordExist("select Leave_balance from employee_leave where leave_code = '" + LeaveType + "' and emp_code = '" + Employee_id + "' "))
            {
                query = "select Leave_balance from employee_leave where leave_code = '" + LeaveType + "' and emp_code = '" + Employee_id + "' ";
                LeaveBalance = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));
                //
                leaveHistory[1] = LeaveBalance;
                leaveHistory[2] = LeaveBalance - totalnoofdays;
            }


            count = PageObject.CheckForHoliday(Employee_id, FromDate, ToDate);
            if (count > 0)
            {

                return_object.status = "error";
                return_object.return_data = "OOPs one of the selected date is on Holiday.";

                return return_object;
            }
            count = PageObject.CheckForWeekoff(Employee_id, FromDate, ToDate);
            if (count > 0)
            {

                return_object.status = "error";
                return_object.return_data = "OOPs one of the selected date is on WeekOff.";

                return return_object;
            }
            count = PageObject.CheckForLeave(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Leave already has been submitted for the selected dates.";
                return return_object;
            }
            count = PageObject.CheckForLossonpay(Employee_id, FromDate, ToDate);
            if (count > 0)
            {

                return_object.status = "error";
                return_object.return_data = "Leave already has been submitted for the selected dates.Please check in LWP Details page.";

                return return_object;
            }
            count = PageObject.CheckForOD(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "OD has been submitted for the selected dates.";
                return return_object;
            }

            //if (LeaveBalance < actualapplieddays)
            //{

            //    DialogResult Result = MessageBox.Show("You dont have sufficient leave balance applied leaves will be considered as LOP", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //    if (Result.ToString() == "No")
            //    {
            //        return_object.status = "error";
            //        return_object.return_data = "Leaves are not applied.";
            //        return return_object;
            //    }
            //}

            //below section of code if 2 level leave approval
            LeaveEnd_Date = Convert.ToDateTime(ToDate);
            if (LeaveEnd_Date.AddDays(7) <= DateTime.Today)
            {
                approvallevel = 2;
            }

            int pos = -1;
            //below section of code is for validating Compoff  
            if (LeaveBalance >= 0.5 && LeaveType == "CO")
            {
                compoffvirtualbalance = PageObject.GetCompoffBalance(Employee_id, "CO", FromDate, ToDate);
                if (compoffvirtualbalance >= 0.5)
                {
                    if (totalnoofdays == actualapplieddays)
                    {

                        pos = Array.IndexOf(returnarray, "error");
                        returnstatus = Convert.ToString(returnarray[0]);
                        returnmessage = returnarray[1];
                        if (pos > -1)
                        {
                            return_object.status = "error";
                            return_object.return_data = returnarray[1];
                            return return_object;
                        }
                        pos = Array.IndexOf(returnarray, "success");
                        if (pos > -1)
                        {
                            if (actualapplieddays <= compoffvirtualbalance)
                            {
                                leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);
                            }
                            else
                            {
                                return_object.status = "error";
                                return_object.return_data = "Sorry! Your actual comp-off balance in last 90 days is " + Convert.ToString(compoffvirtualbalance) + " days.";
                                return return_object;
                            }
                        }
                    }
                    else
                    {
                        pos = Array.IndexOf(returnarray, "error");
                        returnstatus = Convert.ToString(returnarray[0]);
                        returnmessage = returnarray[1];
                        if (pos > -1)
                        {
                            return_object.status = "error";
                            return_object.return_data = returnarray[1];
                            return return_object;
                        }
                        pos = Array.IndexOf(returnarray, "success");
                        if (pos > -1)
                        {
                            if (actualapplieddays <= compoffvirtualbalance)
                            {
                                leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);
                            }
                            else
                            {
                                return_object.status = "error";
                                return_object.return_data = "Sorry! Your actual comp-off balance in last 90 days is " + Convert.ToString(compoffvirtualbalance) + " days.";
                                return return_object;
                            }
                        }

                    }
                }
                else
                {
                    return_object.status = "error";
                    return_object.return_data = "Sorry! Compoff Balance get lapsed for the selected date .";
                    return return_object;
                }
            }
            if (LeaveBalance >= 0.5 && LeaveType != "CO")
            {
                leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);

            }
            else
            {
                if (LeaveType == "CO" && LeaveBalance == 0.0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Sorry! Compoff Balance is 0.0 .";
                    return return_object;
                }
                else if (LeaveType != "CO")
                {
                    leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);
                }

            }

            //if manager or hr or admin submits the leave for their subordinates then autoapproval of leave
            if (Employee_id != HttpContext.Current.Session["employee_id"].ToString())
            {
                Submitleaveforsub(leavesubmitflag, Employee_id, FromDate, approvallevel, LeaveType);
                return_object.status = "success";
                return_object.return_data = "Leave submitted successfully.Email will be sent shortly";
                // send mail for auto approved leave
                AutoApprovalmail(Employee_id, LeaveType, FromDate, ToDate, Reason, half_day, approvallevel);
            }
            else
            {
                if (leavesubmitflag > 0)
                {
                    PageObject.SendMail(Employee_id, LeaveType, FromDate, ToDate, Reason, half_day, leaveHistory);
                    return_object.status = "success";
                    return_object.return_data = "Leave submitted successfully.Email will be sent to your manager";
                }

            }


        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SUBMIT_LEAVE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Submitting Leave Application. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            PageObject.Dispose();
        }

        return return_object;
    }

    private static void Submitleaveforsub(int leavesubmitflag, string Employee_id, string FromDate, int approvallevel, string LeaveType)
    {
        DBConnection db_connection = new DBConnection();
        string Session_emp_name = HttpContext.Current.Session["employee_name"].ToString(), leaveid;
        string query = string.Empty;

        int CurrentFlag = 2;
        string ActionEmpID = HttpContext.Current.Session["username"].ToString();
        string Approvedby = HttpContext.Current.Session["employee_name"].ToString();
        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (string.IsNullOrEmpty(Session_emp_name))
        {
            Session_emp_name = "Admin";
        }
        if (leavesubmitflag > 0)
        {
            if (db_connection.RecordExist("select leave_id from leave1 where empid='" + Employee_id + "' and Startdate = '" + FromDate + "' and flag = 1 "))
            {
                leaveid = db_connection.ExecuteQuery_WithReturnValueString("select leave_id from leave1 where empid='" + Employee_id + "' and Startdate = '" + FromDate + "' and flag = 1 ");

                /*this is to get WFCode from procedure by passing empid and leavetype*/
                string wf_code = db_connection.ExecuteProcedureInOutParameters("ReadApprovalWFCode", Employee_id, LeaveType, "WorkFlowCode");
                Hashtable leave_details_data = new Hashtable();
                if (!string.IsNullOrEmpty(wf_code))
                {
                    leave_details_data.Add("Action_EmpCode", ActionEmpID);
                    leave_details_data.Add("LeaveApplication_ID", leaveid);
                    leave_details_data.Add("WorkFlow_Code", wf_code);
                    leave_details_data.Add("LeaveStatus", CurrentFlag);
                    leave_details_data.Add("AccessLevel", user_access_level);

                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ProcessLeaveApplication", leave_details_data);
                }


            }

            if (db_connection.RecordExist("select leave_id from lossonpay where empid='" + Employee_id + "' and Startdate = '" + FromDate + "' and flag = 1 "))
            {
                leaveid = db_connection.ExecuteQuery_WithReturnValueString("select leave_id from lossonpay where empid='" + Employee_id + "' and Startdate = '" + FromDate + "' and flag = 1");
                if (approvallevel == 1)
                {
                    query = "update lossonpay set flag = 2 where empid='" + Employee_id + "',ApprovedbyName='" + Session_emp_name + "' and Startdate = '" + FromDate + "' and flag = 1 and leave_id = '" + leaveid + "' ";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                    Autoapprovelossonpay(leaveid, 2, "Auto approved");//To approve the leave
                }


            }
        }

    }

    private static void Autoapproveleave(string Leaveid, int Flag, string comments, string Leavetype)
    {
        DBConnection objDataTier_UpdateLeaveStaus = new DBConnection();
        Hashtable hsh = new Hashtable();
        hsh.Add("piFlag", Flag);
        hsh.Add("piLeaveid", Leaveid);
        hsh.Add("piactioncomment", comments);
        if (Flag == 5)
            hsh.Add("piImdtmngrflag", 1);
        objDataTier_UpdateLeaveStaus.ExecuteStoredProcedureWithHashtable_WithoutReturn("SpUpdateLeaveStatus", hsh);
        if (Leavetype == "CO")
        {
            string Empid = null;
            string leaveT = null;
            string StartDate = null;
            string EndDate = null;
            int compoffapprovalflag;
            DataSet rdr;

            string qry3 = "select EMPID,StartDate,EndDate,LeaveType,flag from leave1 where leave_id='" + Leaveid + "'";
            rdr = objDataTier_UpdateLeaveStaus.ReturnDataSet(qry3);

            compoffapprovalflag = Convert.ToInt32(rdr.Tables[0].Rows[0][4].ToString());
            if (compoffapprovalflag == 2)
            {
                if (rdr != null && rdr.Tables.Count > 0 && rdr.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < rdr.Tables[0].Rows.Count; i++)
                    {
                        Empid = rdr.Tables[0].Rows[i][0].ToString();
                        StartDate = rdr.Tables[0].Rows[i][1].ToString();
                        EndDate = rdr.Tables[0].Rows[i][2].ToString();
                        leaveT = rdr.Tables[0].Rows[i][3].ToString();
                    }

                    if (leaveT == "CO")
                    {
                        string updateCompoffDetailsSplit = "with CompoffDetailsSplitCTE as(select top (datediff(Day,convert(datetime,'" + StartDate + "',120),convert(datetime,'" + EndDate + "',120))+1) LFlag,LdateFrom,LdateTo from CompoffDetailsSplit where empid='" + Empid + "' and LFlag=1 and LdateFrom = convert(datetime,'" + StartDate + "',120) and LdateTo=convert(datetime,'" + EndDate + "',120) order by CompoffDate)update CompoffDetailsSplitCTE set LFlag=2";
                        int affectedrecord = objDataTier_UpdateLeaveStaus.ExecuteQuery_WithReturnValueInteger(updateCompoffDetailsSplit);
                    }
                }
            }
        }

    }

    private static void Autoapprovelossonpay(string Leaveid, int Flag, string comments)
    {
        DBConnection objDataTier_UpdateLeaveStaus = new DBConnection();
        Hashtable hsh = new Hashtable();
        hsh.Add("Flag", Flag);
        hsh.Add("Leaveid", Leaveid);
        hsh.Add("actioncomment", comments);
        objDataTier_UpdateLeaveStaus.ExecuteStoredProcedureWithHashtable_WithoutReturn("SpUpdateLeaveStatus1", hsh);

    }

    private static void AutoApprovalmail(string employee_id, string Leavetype, string FromDate, string ToDate, string Reason, int halfday, int approvallevel)
    {
        Hashtable leave_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty,
            Leave_name = string.Empty, immediate_mgr_email = string.Empty,
            manager_id = string.Empty, immediate_mgr_name = string.Empty,
            user_id = string.Empty, user_name = string.Empty;
        int login_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());

        user_id = HttpContext.Current.Session["employee_id"].ToString();
        if (!string.IsNullOrEmpty(user_id))
        {
            query = "select Emp_Name from EmployeeMaster where Emp_Code='" + user_id + "'";
            user_name = db_connection.ExecuteQuery_WithReturnValueString(query);
        }
        else
        {
            user_name = "admin";
        }
        // Getting the Employee email ID 
        query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        // Getting the Employee name
        query = "select Emp_Name from EmployeeMaster where Emp_Code='" + employee_id + "'";
        employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailCC = email_id;

        // Getting Manager Email ID
        query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        email_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailTo = email_id;

        //Getting manager id
        query = "select Emp_code from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        manager_id = db_connection.ExecuteQuery_WithReturnValueString(query);

        // Getting Leave Name
        query = "select LeaveName  from Leavemaster where Leavecode='" + Leavetype + "'";
        Leave_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        //Getting immediate manager EMAILID

        if (db_connection.RecordExist("select Emp_email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + manager_id + "')"))
        {
            immediate_mgr_email = db_connection.ExecuteQuery_WithReturnValueString(query);
        }


        if (!string.IsNullOrEmpty(mailTo))
        {
            if (halfday == 1)
            {
                if (approvallevel == 2)
                {
                    MailSubject = "HalfDay Leave Application has been submitted by the " + user_name + "[" + user_id + "] for Employee:[" + employee_id + "] ";
                    if (!string.IsNullOrEmpty(immediate_mgr_email))
                        mailCC = immediate_mgr_email;
                }
                if (approvallevel == 1)
                    MailSubject = "HalfDay Leave Application has been approved by  " + user_name + "";
            }
            else
            {
                if (approvallevel == 2)
                {
                    MailSubject = "Leave Application has been submitted by the " + user_name + "[" + user_id + "] for Employee:[" + employee_id + "]";
                    mailCC = immediate_mgr_email;
                }
                if (approvallevel == 1)
                    MailSubject = "Leave Application has been approved by  " + user_name + " ";
            }
            MailBody = MailBody + "Dear Sir/Madam, <br/><br/>";
            if (approvallevel == 2)
            {
                MailBody = MailBody + "The following Leave has been approved by " + user_name + " and waiting for the 2nd level approval <br/><br/>";
            }
            if (approvallevel == 1)
            {
                MailBody = MailBody + "The following Leave has been Approved <br/><br/>";
            }
            MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
            MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
            MailBody = MailBody + "LeaveType : " + Leave_name + " <br/><br/>";
            if (halfday == 1)
            {
                MailBody = MailBody + "Date: " + FromDate + "<br/><br/>";
            }
            else
            {
                MailBody = MailBody + "From Date: " + FromDate + "<br/><br/>";
                MailBody = MailBody + "To Date : " + ToDate + "<br/><br/>";
            }
            MailBody = MailBody + "Reason:  " + Reason + "<br/><br/>";

            leave_send_mail.Add("EmpID", employee_id);
            leave_send_mail.Add("EmpName", employee_name);
            leave_send_mail.Add("ToEmailID", mailTo);
            leave_send_mail.Add("CCEmailID", mailCC);
            leave_send_mail.Add("Subject", MailSubject);
            leave_send_mail.Add("Body", MailBody);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("insertmaildetails", leave_send_mail);
        }
    }

    private string CreateExport()
    {
        string
           Emp_id = HttpContext.Current.Session["employee_id"].ToString(),
           file_name = "LeaveTemplate-" + Emp_id + "-" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        string export_path = HttpContext.Current.Server.MapPath("~/exports/data/");

        DateTime now = DateTime.Now;
        DBConnection db_connection = new DBConnection();
        DataTable Leave_Details = new DataTable();
        DataTable Leave_Template = new DataTable();
        DataTable Leave_Types = new DataTable();

        DataSet ds = new DataSet();
        string query = "";

        Leave_Template.Columns.Add("EMP_CODE");
        Leave_Template.Columns.Add("FROMDATE");
        Leave_Template.Columns.Add("TODATE");
        Leave_Template.Columns.Add("LEAVETYPE");
        Leave_Template.Columns.Add("REMARKS");

        DataRow newRow = Leave_Template.NewRow();

        newRow["EMP_CODE"] = "Emp_code(Upto 8 Alphanumeric ex:ABC00012)";
        newRow["FROMDATE"] = "FROMDATE(Date Format ex:yyyy-MM-dd)";
        newRow["TODATE"] = "TODATE(Date Format  ex:yyyy-MM-dd)";
        newRow["LEAVETYPE"] = "LEAVETYPE(ENTER LEAVE CODE)	";
        newRow["REMARKS"] = "REMARKS(max 200 characters ex:remarks)";

        Leave_Template.Rows.Add(newRow);

        query = "select Emp_code ,Leave_code, convert(varchar(50),Max_leaves) as Max_leaves, convert(varchar(50),Leaves_applied) as Leaves_applied , convert(varchar(50),Leave_balance)as Leave_balance  from Employee_Leave where leave_code not in ('CO') and Emp_code in ( select EmpID from [FetchEmployees] ('" + HttpContext.Current.Session["employee_id"].ToString() + "','')) order by Emp_code";
        Leave_Details = db_connection.ReturnDataTable(query);

        query = "select EmployeeCategoryCode , LeaveCode , LeaveName  from leavemaster";
        Leave_Types = db_connection.ReturnDataTable(query);

        ds.Tables.Add(Leave_Template);
        ds.Tables.Add(Leave_Details);
        ds.Tables.Add(Leave_Types);

        ds.Tables[0].TableName = "Leave Template";
        ds.Tables[1].TableName = "Leave Details";
        ds.Tables[2].TableName = "Leave Types";

        using (XLWorkbook wb = new XLWorkbook())
        {
            foreach (DataTable dt in ds.Tables)
            {
                wb.Worksheets.Add(dt, dt.TableName);
            }

            wb.SaveAs(export_path + file_name);
        }

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport()
    {
        leave_apply page_object = new leave_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable Leave_Balance = new DataTable();
        DateTime now = DateTime.Now;
        string Employee_id = HttpContext.Current.Session["employee_id"].ToString();
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);

        string[] column_names = new string[] { };

        string
            query = string.Empty, file_name = string.Empty;

        try
        {
            file_name = page_object.CreateExport();

            return_object.status = "success";
            return_object.return_data = file_name;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_DATA_FOR_EXPORT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while generating your report. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    private string[] GetEmployeeCodes()
    {
        DBConnection db_connection = new DBConnection();
        string[] employee_code_array = null;
        DataTable employee_data = new DataTable();
        string query = string.Empty;

        query = " select EmpID from [FetchEmployees] ('" + HttpContext.Current.Session["employee_id"].ToString() + "','')";

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

    public static ReturnObject SubmitLeaveFromExcel(string Employee_id, string LeaveType, string FromDate, string ToDate, string Reason)
    {
        leave_apply PageObject = new leave_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();

        string returnstatus = string.Empty,
            returnmessage = string.Empty, query = string.Empty, leaveid = string.Empty;

        int
            half_day = 0, approvallevel = 1,
            count = 0, leavesubmitflag = 0;

        double
            LeaveBalance = 0.0, totalnoofdays = 0.0, actualapplieddays = 0.0,
            compoffvirtualbalance = 0.0;
        DateTime LeaveEnd_Date = new DateTime();

        try
        {
            totalnoofdays = (Convert.ToDateTime(ToDate) - Convert.ToDateTime(FromDate)).TotalDays + 1;

            if (!db_connection.RecordExist("select count(*) from Employee_leave where emp_Code='" + Employee_id + "'"))
            {
                return_object.status = "error";
                return_object.return_data = "Leaves are not Assigned for the Employee.";
                return return_object;
            }

            string[] returnarray = PageObject.ValidatingCompoff(Employee_id, FromDate, totalnoofdays);

            //calculating the actual no of leaves 
            query = "select dbo.[BalanceLeave]('" + Employee_id + "',CONVERT(datetime,'" + FromDate + "',103),CONVERT(datetime,'" + ToDate + "',103),'" + LeaveType + "')";
            actualapplieddays = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));

            if (db_connection.RecordExist("select Leave_balance from employee_leave where leave_code = '" + LeaveType + "' and emp_code = '" + Employee_id + "' "))
            {
                query = "select Leave_balance from employee_leave where leave_code = '" + LeaveType + "' and emp_code = '" + Employee_id + "' ";
                LeaveBalance = Convert.ToDouble(db_connection.ExecuteQuery_WithReturnValueString(query));
            }

            count = PageObject.CheckForHoliday(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Oops one of the selected date is on Holiday.";

                return return_object;
            }

            count = PageObject.CheckForWeekoff(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "OOPs one of the selected date is on WeekOff.";

                return return_object;
            }

            count = PageObject.CheckForLeave(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Leave already has been submitted for the selected dates.";
                return return_object;
            }

            count = PageObject.CheckForLossonpay(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Leave already has been submitted for the selected dates.Please check in LWP Details page.";
                return return_object;
            }

            count = PageObject.CheckForOD(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "OD has been submitted for the selected dates.";
                return return_object;
            }



            int pos = -1;
            //below section of code is for validating Compoff  
            if (LeaveBalance >= 0.5 && LeaveType == "CO")
            {
                compoffvirtualbalance = PageObject.GetCompoffBalance(Employee_id, "CO", FromDate, ToDate);
                if (compoffvirtualbalance >= 0.5)
                {
                    if (totalnoofdays == actualapplieddays)
                    {
                        pos = Array.IndexOf(returnarray, "error");
                        returnstatus = Convert.ToString(returnarray[0]);
                        returnmessage = returnarray[1];
                        if (pos > -1)
                        {
                            return_object.status = "error";
                            return_object.return_data = returnarray[1];
                            return return_object;
                        }

                        pos = Array.IndexOf(returnarray, "success");
                        if (pos > -1)
                        {
                            if (actualapplieddays <= compoffvirtualbalance)
                            {
                                leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);
                            }
                            else
                            {
                                return_object.status = "error";
                                return_object.return_data = "Sorry! Your actual comp-off balance in last 90 days is " + Convert.ToString(compoffvirtualbalance) + " days.";
                                return return_object;
                            }
                        }

                    }
                    else
                    {
                        pos = Array.IndexOf(returnarray, "error");
                        returnstatus = Convert.ToString(returnarray[0]);
                        returnmessage = returnarray[1];
                        if (pos > -1)
                        {
                            return_object.status = "error";
                            return_object.return_data = returnarray[1];
                            return return_object;
                        }
                        pos = Array.IndexOf(returnarray, "success");
                        if (pos > -1)
                        {
                            if (actualapplieddays <= compoffvirtualbalance)
                            {
                                leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);
                            }
                            else
                            {
                                return_object.status = "error";
                                return_object.return_data = "Sorry! Your actual comp-off balance in last 90 days is " + Convert.ToString(compoffvirtualbalance) + " days.";
                                return return_object;
                            }
                        }

                    }
                }
                else
                {
                    return_object.status = "error";
                    return_object.return_data = "Sorry! Compoff Balance get lapsed for the selected date .";
                    return return_object;
                }
            }
            if (LeaveBalance >= 0.5)
            {
                leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);

            }
            else
            {
                if (LeaveType == "CO" && LeaveBalance == 0.0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Sorry! Compoff Balance is 0.0 .";
                    return return_object;
                }
                else
                {
                    leavesubmitflag = PageObject.InsertData(Employee_id, LeaveType, FromDate, ToDate, Reason, approvallevel, half_day);
                }

            }

            //if manager or hr or admin submits the leave for their subordinates then autoapproval of leave
            if (Employee_id != HttpContext.Current.Session["employee_id"].ToString())
            {
                leaveid = db_connection.ExecuteQuery_WithReturnValueString("select leave_id from leave1 where empid='" + Employee_id + "' and Startdate = '" + FromDate + "' and flag = 1 ");
                Submitleaveforsub(leavesubmitflag, Employee_id, FromDate, approvallevel, LeaveType);
                //Autoapproveleave(leaveid, 2, "Auto approved", LeaveType);//To approve the leave
                return_object.status = "success";
                return_object.return_data = "Leave submitted successfully.Email will be sent shortly";
                // send mail for auto approved leave
                AutoApprovalmail(Employee_id, LeaveType, FromDate, ToDate, Reason, half_day, approvallevel);
            }
            else
            {
                if (leavesubmitflag > 0)
                {
                    double[] leaveHistory = new double[3];
                    PageObject.SendMail(Employee_id, LeaveType, FromDate, ToDate, Reason, half_day, leaveHistory);
                    return_object.status = "success";
                    return_object.return_data = "Leave submitted successfully.Email will be sent to your manager";
                }
            }
        }

        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SUBMIT_LEAVE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Submitting Leave Application. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            PageObject.Dispose();
        }

        return return_object;
    }

    public string ImportLeave(DataTable leave_data)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable hshParam = new Hashtable();

        int line_no = 0;

        string[]
             employee_code_array = null, leave_codes = null;

        string empcode = string.Empty, error_message = string.Empty,
            fromdate = string.Empty,
            todate = string.Empty,
            leavetype = string.Empty,
            remarks = string.Empty;

        bool emp_code = true,
            from_date = true,
            to_date = true,
            leave_type = true;

        string employee_id = HttpContext.Current.Session["username"].ToString();
        string employee_name = HttpContext.Current.Session["employee_name"].ToString();
        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());

        try
        {
            employee_code_array = GetEmployeeCodes();
            line_no = 2;

            foreach (DataRow dr in leave_data.Rows)
            {
                line_no++;

                emp_code = true; from_date = true; to_date = true; leave_type = true;

                if (string.IsNullOrEmpty(dr["EMP_CODE"].ToString()))
                {
                    error_message += Environment.NewLine + "EmpCode is Null or empty on line Number:    " + line_no;
                    emp_code = false;
                }
                else
                {
                    empcode = dr["EMP_CODE"].ToString();
                    leave_codes = GetLeaveCodes(empcode);

                    if (string.IsNullOrEmpty(dr["FROMDATE"].ToString()))
                    {
                        from_date = false;
                        error_message += Environment.NewLine + "FROMDATE is Null or empty on line Number:    " + line_no;
                    }
                    else
                    {
                        fromdate = Convert.ToDateTime(dr["FROMDATE"].ToString()).ToString("yyyy-MMM-dd");
                    }

                    if (string.IsNullOrEmpty(dr["TODATE"].ToString()))
                    {
                        to_date = false;
                        error_message += Environment.NewLine + "TODATE is Null or empty on line Number:    " + line_no;
                    }
                    else
                    {
                        todate = Convert.ToDateTime(dr["TODATE"].ToString()).ToString("yyyy-MMM-dd");
                    }

                    if (string.IsNullOrEmpty(dr["LEAVETYPE"].ToString()))
                    {
                        leave_type = false;
                        error_message += Environment.NewLine + "Leave Type is Null or empty on line Number:    " + line_no;
                    }
                    else
                    {
                        leavetype = dr["LEAVETYPE"].ToString();
                    }

                    remarks = dr["REMARKS"].ToString();

                    if (Array.IndexOf(employee_code_array, empcode) >= 0)
                    {
                        if (Array.IndexOf(leave_codes, leavetype) >= 0)
                        {
                            if (emp_code & from_date & to_date & leave_type)
                            {
                                return_object = SubmitLeaveFromExcel(empcode, leavetype, fromdate, todate, remarks);
                                error_message += Environment.NewLine + " " + return_object.return_data + " - " + empcode;
                            }
                        }
                        else
                        {
                            error_message += Environment.NewLine + "Leave code does not belong for employee id: " + empcode;
                        }

                    }
                    else
                    {
                        error_message += Environment.NewLine + "You don't have permission to submit leave for employee id: " + empcode;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_LEAVE");
        }

        return error_message;
    }

    [WebMethod]
    public static ReturnObject DoImport(string file_name)
    {
        leave_apply page_object = new leave_apply();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        DataTable excel_data = new DataTable();
        DataRow first_row = null;

        string
            default_upload_path = string.Empty, full_upload_path = string.Empty,
            employee_id = string.Empty, return_message = string.Empty,
            query = string.Empty;

        try
        {
            default_upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            full_upload_path = HttpContext.Current.Server.MapPath("~/" + default_upload_path + "/" + file_name);

            //full_upload_path = file_name;
            // Read the excel file and store the data in a DataTable.
            excel_data = ExcelImport.ImportExcelToDataTable(full_upload_path, "");
            // Get the 1st ROW of the EXCEL sheet
            first_row = excel_data.Rows[0];
            // Remove the 1st ROW of the EXCEL sheet. This is essentially the title row.
            excel_data.Rows.Remove(first_row);

            return_message = page_object.ImportLeave(excel_data);

            return_object.status = "success";
            return_object.return_data = return_message;


        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_LEAVE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while importing leave. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }
}