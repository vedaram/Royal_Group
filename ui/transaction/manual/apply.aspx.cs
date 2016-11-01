using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.Mail;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;
using SecurAX.Import.Excel;
using SecurAX.Export.Excel;

public partial class manual_apply : System.Web.UI.Page
{
    const string page = "MANUAL_PUNCH_APPLICATION";

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

            message = "An error occurred while loading Manual Punch Application page. Please try again. If the error persists, please contact Support.";

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


    private int GetShiftConfiguration(string employee_code, string date)
    {
        DBConnection db_connection = new DBConnection();
        DataTable temp_data = new DataTable();
        string query = string.Empty;

        int
            is_split_shift = 0,
            count_normal_shift = 0,
            normal_shift = 0;

        query = "select count(chkifNormalShift) as count from Shift where Shift_Code = (Select Shift_Code from MASTERPROCESSDAILYDATA where Emp_ID = '" + employee_code + "' and PDate ='" + date + "') ";
        count_normal_shift = db_connection.ExecuteQuery_WithReturnValueInteger(query);

        query = "select chkifNormalShift from Shift where Shift_Code = (Select Shift_Code from MASTERPROCESSDAILYDATA where Emp_ID = '" + employee_code + "' and PDate ='" + date + "') ";
        normal_shift = db_connection.ExecuteQuery_WithReturnValueInteger(query);

        if (normal_shift == 1 || count_normal_shift == 0)
        {
            is_split_shift = 0;
        }
        else if (normal_shift == 0)
        {
            is_split_shift = 1;
        }

        return is_split_shift;
    }

    [WebMethod]
    public static ReturnObject GetPunchDetailsForEmployee(string employee_code, string date)
    {
        manual_apply page_object = new manual_apply();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable shift_data = new DataTable();

        int shift_config = 0;
        string query = string.Empty;

        try
        {
            shift_config = page_object.GetShiftConfiguration(employee_code, date);

            query = "select In_Punch, Out_Punch, BreakOut, BreakIn, shift_code,status from masterprocessdailydata where emp_id = '" + employee_code + "' and PDate = '" + date + "' ";
            // Adding a new column to store the value of is_split_shift
            shift_data = db_connection.ReturnDataTable(query);

            shift_data.Columns.Add("split_shift");

            if (shift_data.Rows.Count == 0)
                shift_data.Rows.Add();

            shift_data.Rows[0]["split_shift"] = shift_config;

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(shift_data, Formatting.Indented);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_SHIFT_CONFIG");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Shift Config. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
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

    private string[] CheckDatesForManualPunch(string current_Empid,string in_date, string in_time, string out_date, string out_time, string break_in_date, string break_in_time, string break_out_date, string break_out_time)
    {
        string[] return_data = new string[2];

        string
            in_date_time = string.Empty, out_date_time = string.Empty,
            today = string.Empty;

        DateTime
            date_in = new DateTime(), date_out = new DateTime(),
            next_day = new DateTime(), date_time_today = new DateTime();

        date_in = DateTime.ParseExact(in_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
        next_day = date_in.AddDays(1);
        in_date_time = date_in.ToString("yyyy/MM/dd");

        date_out = DateTime.ParseExact(out_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
        out_date_time = date_out.ToString("yyyy/MM/dd");
        //  
       // string current_Empid = HttpContext.Current.Session["username"].ToString(),query=string.Empty;
        string query = "Select Count(*) from MasterProcessdailydata where Pdate='" + in_date_time + "' and Emp_id='" + current_Empid + "'";
        DBConnection dbconnection = new DBConnection();
        int count=  dbconnection.ExecuteQuery_WithReturnValueInteger(query);
        if (count == 0)
        {
            return_data[0] = "Record is not Processed";
            return return_data;
        }
          
        //
        if (Convert.ToDateTime(in_time) > Convert.ToDateTime(out_time))
        {
            if (date_out != next_day)
            {
                return_data[0] = "Outpunch date should be next day of inpunch date";
                return return_data;
            }
        }
        if (Convert.ToDateTime(in_time) < Convert.ToDateTime(out_time))
        {
            if (date_in != date_out)
            {
                return_data[0] = "Outpunch date should be same as inpunch date";
                return return_data;
            }
        }

        in_date_time = in_date_time + " " + in_time + ":00";
        out_date_time = out_date_time + " " + out_time + ":00";
 
        
        today = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        date_in = DateTime.ParseExact(in_date_time, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        date_out = DateTime.ParseExact(out_date_time, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        date_time_today = DateTime.ParseExact(today, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

        //date_in = DateTime.ParseExact(in_date_time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        //date_out = DateTime.ParseExact(out_date_time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        //date_time_today = DateTime.ParseExact(today, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);


        if (DateTime.Compare(date_time_today, date_in) == -1)
        {
            return_data[0] = "In Punch Date should be less than Current Date";
            return return_data;
        }

        if (Convert.ToDateTime(out_date_time) > Convert.ToDateTime(today))
        {
            return_data[0] = "Out Punch Time should not be greater than Current Time";
            return return_data;
        }

        if (DateTime.Compare(date_out, date_in) == -1)
        {
            return_data[0] = "Out punch should be greater than In Punch";
            return return_data;
        }

        return return_data;
    }

    private string ConvertUrlsToLinks(string msg)
    {
        string output = msg;
        System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*([a-zA-Z0-9\\?\\#\\=\\/]){1})?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        System.Text.RegularExpressions.MatchCollection mactches = regx.Matches(output);

        foreach (System.Text.RegularExpressions.Match match in mactches)
        {
            output = output.Replace(match.Value, "<a href='" + match.Value + "' target='blank'>" + match.Value + "</a>");
        }
        return output;
    }

    private string InsertData(string mode, string EmpCode, string WorkDate, string InPunch, string OutPunch, string BreakOut, string BreakIn, string Remarks, string outdate, string indate)
    {
        DBConnection objDataTier_InsertData = new DBConnection();
        int intRecordsAffected = 0;
        Hashtable hshParam = new Hashtable();
        Hashtable mail = new Hashtable();
        string employeeEmailID = null;
        string notification = "";

        try
        {
            hshParam.Add("piMode", mode);
            hshParam.Add("piEmpCode", EmpCode);
            hshParam.Add("piWorkDate", WorkDate);
            hshParam.Add("piInPunch", InPunch);
            hshParam.Add("piOutPunch", OutPunch);

            if (BreakOut != "" && BreakIn != "")
            {
                hshParam.Add("piBrkOut", BreakOut);
                hshParam.Add("piBrkIn", BreakIn);
            }

            hshParam.Add("piRemarks", Remarks);
            hshParam.Add("pioutdate", outdate);
            hshParam.Add("piCreatedBy", Session["username"]);
            hshParam.Add("piModifiedBy", "");
            string pdate = null;

            hshParam.Add("pipdate", "");

            objDataTier_InsertData.ExecuteStoredProcedureWithHashtable_WithoutReturn("spSavePunchForApproval", hshParam);

            DBConnection datmtier = new DBConnection();
            DataTable dt = new DataTable();
            mail.Add("employeeId", EmpCode);
            mail.Add("PunchDate", WorkDate);
            dt = datmtier.ExecuteStoredProcedureWithHashtable_WithReturnDatatable("SpCalculateManualpunch", mail);

            int Returcount = 0;
            if (dt.Rows.Count > 0)
            {
                Returcount = Convert.ToInt32(dt.Rows[0]["ManualPunch"].ToString());
            }

            DataTable dt1 = new DataTable();
            DateTime date1 = new DateTime();
            DateTime date2 = new DateTime();
            DateTime date3 = new DateTime();
            DateTime date4 = new DateTime();


            date1 = DateTime.ParseExact(indate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            string inpunchdate = date1.ToString("MM/dd/yyyy");
            date2 = DateTime.ParseExact(outdate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            string outdate1 = date2.ToString("MM/dd/yyyy");
            string Boutdate1 = "";
            string Bindate1 = "";

            if (BreakOut != "" && BreakIn != "")
            {
                date3 = DateTime.ParseExact(BreakOut, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                Boutdate1 = date3.ToString("MM/dd/yyyy");

                date4 = DateTime.ParseExact(BreakIn, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                Bindate1 = date4.ToString("MM/dd/yyyy");
            }

           // string str1 = "select d.DesigName as Emp_Department,dm.DeptName as Emp_Designation,em.managerid as managerid ,em.emp_name as Empname,em.emp_email as Emp_email from  employeemaster em left join DesigMaster d  on em.Emp_Designation=d.DesigCode join deptmaster dm on em.Emp_Department=dm.DeptCode where em.emp_code='" + EmpCode + "'";
            string str1 = "select emp_department , emp_designation , managerid , emp_name , emp_email from employeemaster where emp_code='"+ EmpCode +"'";
            dt1 = objDataTier_InsertData.ReturnDataTable(str1);
            string designame = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("select designame from desigmaster where desigcode = '" + dt1.Rows[0]["emp_designation"].ToString() + "'");
            string deptname = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("select deptname from deptmaster where deptcode = '" + dt1.Rows[0]["emp_department"].ToString() + "'");
            string managerid = dt1.Rows[0]["managerid"].ToString();
            string name = dt1.Rows[0]["emp_name"].ToString();
            employeeEmailID = dt1.Rows[0]["emp_email"].ToString();

            //if 3 days continious manual entry
            if (Returcount >= 3)
            {
                string Subject1 = "Manual entry punch has been submitted three Days continously for Emp name: " + name + ", Emp ID: " + EmpCode + "";
                string str = string.Empty;
                string Location = ConfigurationManager.AppSettings["Location"].ToString();

                if (!string.IsNullOrEmpty(managerid))
                {
                    string encrypt = SSTCryptographer.Encrypt(managerid, "SampleKey");
                    string managerurl = "?MP=" + encrypt + "";
                    Location = Location + managerurl;
                    Location = ConvertUrlsToLinks(Location);
                }

                str = str + "Manual entry punch has been submitted three Days continously By: <br/><br/>Employee Id: " + EmpCode + ".<br/><br/>";
                str = str + "Employee Name : " + name + " <br/><br/>";
                str = str + "Department : " + deptname + " <br/><br/>";
                str = str + "Designation : " + designame + " <br/><br/>";
                str = str + "Manual Entry punch in datetime : " + inpunchdate + " " + InPunch + "<br/><br/>";
                str = str + "Manual Entry punch Out datetime: " + outdate1 + " " + OutPunch + "<br/><br/>";
                if (BreakOut != "" && BreakIn != "")
                {
                    str += "Manual Entry punch BreakOut datetime : " + Boutdate1 + " " + BreakOut + "<br/><br/>";
                    str += "Manual Entry punch BreakIn datetime : " + Bindate1 + " " + BreakIn + "<br/><br/>";
                }
                str = str + "Status : Submitted <br/><br/>";
                str = str + "Remarks : " + Remarks + "<br/><br/>";
                str = str + "Please access " + Location + " to view the transaction.";

                string Body = str;
                bool flag1 = false;


                //ManagerEmailID is Manager EmailID, HrEmailID is Hr manager emial id
                string ManagerEmailID = null;

                //Getting Employee BranchName to check wether any Hr is assigned to particular Branch
                string BranchName = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("Select Emp_Branch From Employeemaster Where Emp_Code='" + EmpCode + "'");
                string HrManagerID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("Select ManagerID from TbManagerHrBranchMapping Where BranchCode='" + BranchName + "'");
                if (!string.IsNullOrEmpty(HrManagerID))
                {
                    ManagerEmailID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("select Emp_Email  from EmployeeMaster where Emp_Code='" + HrManagerID + "'");
                }
                else
                {
                    ManagerEmailID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("select Emp_Email  from EmployeeMaster where Emp_Code=(select Managerid from EmployeeMaster where Emp_Code='" + EmpCode + "')");
                }

            }
            else
            {
                string Subject = "Manual entry punch has been submitted for Emp name: " + name + ", Emp ID: " + EmpCode + "";
                string str = string.Empty;
                string Location = ConfigurationManager.AppSettings["Location"].ToString();

                if (!string.IsNullOrEmpty(managerid))
                {
                    string encrypt = SSTCryptographer.Encrypt(managerid, "SampleKey");
                    string managerurl = "?MP=" + encrypt + "";
                    Location = Location + managerurl;
                    Location = ConvertUrlsToLinks(Location);
                }

                str = str + "The following Manual entry punch has been submitted : <br/><br/>Employee Id: " + EmpCode + ".<br/><br/>";
                str = str + "Employee Name : " + name + " <br/><br/>";
                str = str + "Department : " + deptname + " <br/><br/>";
                str = str + "Designation : " + designame + " <br/><br/>";
                str = str + "Manual Entry punch in datetime : " + inpunchdate + " " + InPunch + "<br/><br/>";
                str = str + "Manual Entry punch Out datetime: " + outdate1 + " " + OutPunch + "<br/><br/>";

                if (BreakOut != "" && BreakIn != "")
                {
                    str += "Manual Entry punch BreakOut datetime : " + Boutdate1 + " " + BreakOut + "<br/><br/>";
                    str += "Manual Entry punch BreakIn datetime : " + Bindate1 + " " + BreakIn + "<br/><br/>";
                }

                str = str + "Status : Submitted <br/><br/>";
                str = str + "Remarks : " + Remarks + "<br/><br/>";
                str = str + "Please access " + Location + " to view the transaction.";

                string Body = str;
                bool flag1 = false;

                employeeEmailID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("select Emp_Email from EmployeeMaster where Emp_Code='" + EmpCode + "'");

                //ManagerEmailID is Manager EmailID
                string ManagerEmailID = null;

                //Getting Delegation managerid/emailid if asigned
                string Manager_ID = null;
                string DelegationMngr_ID = null;

                Manager_ID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("Select Managerid from EmployeeMaster where Emp_Code='" + EmpCode + "'");

                int DelegationCount = objDataTier_InsertData.ExecuteQuery_WithReturnValueInteger("Select Count(ManagerId) from TbAsignDelegation Where ManagerId='" + Manager_ID + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)");

                if (DelegationCount > 0)
                {
                    DelegationMngr_ID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("Select DelidationManagerID from TbAsignDelegation Where ManagerId='" + Manager_ID + "'");
                }

                if (!string.IsNullOrEmpty(DelegationMngr_ID))
                {
                    ManagerEmailID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("Select ((Select Emp_Email from EmployeeMaster Where Emp_Code='" + DelegationMngr_ID + "')+','+(Select Emp_Email from EmployeeMaster Where Emp_Code='" + Manager_ID + "')) As Email");
                }
                else
                {
                    ManagerEmailID = objDataTier_InsertData.ExecuteQuery_WithReturnValueString("select Emp_Email  from EmployeeMaster where Emp_Code=(select Managerid from EmployeeMaster where Emp_Code='" + EmpCode + "')");
                }



            }

        }

        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SAVE_MANUAL_PUNCH");
            throw ex;
        }
        return notification;
    }

    public void autopunchaprv(string employee_id, string workdate, string inpunch, string outpunch, string Reason)
    {
        DBConnection objDataTier_autopunchaprv = new DBConnection();

        if ( Session["username"].ToString() == "admin" || Convert.ToInt32(Session["access_level"]) == 0)
        {
            string order = " order by id desc";
            string leaveid1 = "select top 1 id from PunchForApproval where  EmpCode='" + employee_id + "' ";

            leaveid1 = leaveid1 + order;
            int leaveid = objDataTier_autopunchaprv.ExecuteQuery_WithReturnValueInteger(leaveid1);

            int app = 2;
            ApproveMisingpunch(leaveid, 2);


        }
    }

    private void SendMail(string employee_id, string workdate, string inpunch, string outpunch, string Reason, int status_flag)
    {
        Hashtable leave_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();
        DataTable dt_temp = new DataTable();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty, manual_punch_status = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;

        try
        {

            switch (status_flag)
            {
                case 2:
                    manual_punch_status = "Applied";
                    break;

                case 3:
                    manual_punch_status = "Declined";
                    break;

                case 4:
                    manual_punch_status = "Cancelled";
                    break;
                case 5:
                    manual_punch_status = "Applied";
                    break;
                    
            }
            bool isAdmin=false;
            if (Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 0)
            {
                isAdmin = true;
                manual_punch_status = "Approved";
            }

         


            // Getting the Employee Name
            employee_name = HttpContext.Current.Session["employee_name"].ToString();
            query = "select Emp_name from EmployeeMaster where Emp_Code='" + employee_id + "'";
            employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);
            // Getting the Employee email ID
            query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
            mailCC = db_connection.ExecuteQuery_WithReturnValueString(query);

            // Getting Manager Email ID
            query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
            mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);

            //Getting manager name
            query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
            manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

            /*if it is one level approve  from manager*/
             string currentEmployeeId=HttpContext.Current.Session["username"].ToString();
             if (Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 1 && currentEmployeeId != employee_id)
            {
                query = "select Emp_Email,Emp_Branch from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
                dt_temp = db_connection.ReturnDataTable(query);

                string branch_code = dt_temp.Rows[0]["Emp_Branch"].ToString();
                query = "select BM.ManagerID, EM.Emp_Email, EM.Emp_Name as Emp_name From TbManagerHrBranchMapping BM join EmployeeMaster EM on BM.ManagerID=em.Emp_Code where BM.BranchCode='" + branch_code + "'";
                dt_temp = db_connection.ReturnDataTable(query);

                mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
                manager_name = dt_temp.Rows[0]["Emp_name"].ToString();

                query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                mailCC = db_connection.ExecuteQuery_WithReturnValueString(query);

               
            }
            /*if it is record approved by Hr*/
             if (Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 3 && currentEmployeeId != employee_id)
             {
                 query = "select Emp_Email,Emp_Branch from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
                 dt_temp = db_connection.ReturnDataTable(query);

                 string branch_code = dt_temp.Rows[0]["Emp_Branch"].ToString();
                 query = "select BM.ManagerID, EM.Emp_Email, EM.Emp_Name as Emp_name From TbManagerHrBranchMapping BM join EmployeeMaster EM on BM.ManagerID=em.Emp_Code where BM.BranchCode='" + branch_code + "'";
                 dt_temp = db_connection.ReturnDataTable(query);

                 mailCC = dt_temp.Rows[0]["Emp_Email"].ToString();

                 
                 query = "select Emp_name from EmployeeMaster where Emp_Code='" + employee_id + "'";
                 manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);
                 // Getting the Employee email ID
                 query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
                 mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);
                  


             }


            if (!string.IsNullOrEmpty(mailTo))
            {
                if (isAdmin == true)
                {
                    MailSubject = "Manual Punch Appliction submitted by admin";
                }
                else
                {
                    MailSubject = "Regarding Manual Entry Application";
                }
               
                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following Manual punch has been " + manual_punch_status + " <br/><br/>";
                MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
                MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
                MailBody = MailBody + "Punch Date: " + workdate + "<br/><br/>";
                MailBody = MailBody + "In Punch: " + inpunch + "<br/><br/>";
                MailBody = MailBody + "Out Punch: " + outpunch + "<br/><br/>";
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
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SEND_MANUAL_PUNCH");
        }
    }
    private void SendMailforApproval(string employee_id, string workdate, string inpunch, string outpunch, string Reason, int status_flag,int access,string userid)
    {
        Hashtable leave_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();
        DataTable dt_temp = new DataTable();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, hrEmilId  = string.Empty, query = string.Empty, manual_punch_status = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;

        try
        {

            switch (status_flag)
            {
                case 2:
                    manual_punch_status = "Applied";
                    break;

                case 3:
                    manual_punch_status = "Declined";
                    break;

                case 4:
                    manual_punch_status = "Cancelled";
                    break;
                case 5:
                    manual_punch_status = "Applied";
                    break;

            }
            bool isAdmin = false;
            if (Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 0 )
            {
                isAdmin = true;
                manual_punch_status = "Approved";
            }
           


            // Getting the Employee Name
            employee_name = HttpContext.Current.Session["employee_name"].ToString();
            query = "select Emp_name from EmployeeMaster where Emp_Code='" + employee_id + "'";
            employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);
            // Getting the Employee email ID
            query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
            mailCC = db_connection.ExecuteQuery_WithReturnValueString(query);
            if (employee_id != userid && status_flag == 5 && access==1)
            {
                //getiing Hr details
                //query = "select emp_email from EmployeeMaster where emp_code=( select HrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
                //mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);
                //if (mailTo == "" || String.IsNullOrEmpty(mailTo))
                //{

                //    query = "select emp_email from EmployeeMaster where emp_code=( select AlternativeHrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
                //    mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);
                //    query = "select emp_name from EmployeeMaster where emp_code=( select AlternativeHrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
                //    manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);
                    
                    

                //}
                //else
                //{
                //    query = "select emp_name from EmployeeMaster where emp_code=( select HrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
                //    manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);
                //}

                query = "select Emp_Email,Emp_Branch from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
                dt_temp = db_connection.ReturnDataTable(query);

                string branch_code = dt_temp.Rows[0]["Emp_Branch"].ToString();
                query = "select BM.ManagerID, EM.Emp_Email, EM.Emp_Name as Emp_name From TbManagerHrBranchMapping BM join EmployeeMaster EM on BM.ManagerID=em.Emp_Code where BM.BranchCode='" + branch_code + "'";
                dt_temp = db_connection.ReturnDataTable(query);

                mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
                manager_name = dt_temp.Rows[0]["Emp_name"].ToString();




                // Getting Manager Email ID
                    query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                    mailCC = db_connection.ExecuteQuery_WithReturnValueString(query);

                    MailSubject = "Regarding Manual Entry Applied by Manager And request sent to HR for Further action";
                
                
            }
            else if ((access == 3 || access == 0) && employee_id != userid)
            {
                // Getting the Employee email ID
                query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
                mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);
                // Getting the user email ID
                query = "select Emp_Email from EmployeeMaster where Emp_Code='" + userid + "'";
                mailCC = db_connection.ExecuteQuery_WithReturnValueString(query);
                //Getting employee name
                query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
                manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

                MailSubject = "Regarding Manual Entry Application  ";
            }
            

            

            if (!string.IsNullOrEmpty(mailTo))
            {
                if (isAdmin == true)
                {
                    MailSubject = "Manual Punch Appliction submitted by admin";
                }
                

                MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
                MailBody = MailBody + "The following Manual punch has been " + manual_punch_status + " <br/><br/>";
                MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
                MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
                MailBody = MailBody + "Punch Date: " + workdate + "<br/><br/>";
                MailBody = MailBody + "In Punch: " + inpunch + "<br/><br/>";
                MailBody = MailBody + "Out Punch: " + outpunch + "<br/><br/>";
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
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SEND_MANUAL_PUNCH");
        }
    }
    [WebMethod]
    public static ReturnObject SaveManualPunch(string current)
    {
        manual_apply page_object = new manual_apply();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        JObject current_data = new JObject();
        string EmailMessage = string.Empty, retrun_message = string.Empty;
        string[] validation = null;

        int approval_status = 0, manualpunch_id = 0;

        string
            employee_code = string.Empty,
            in_punch_date = string.Empty, in_punch_time = string.Empty,
            out_punch_date = string.Empty, out_punch_time = string.Empty,
            break_in_date = string.Empty, break_out_date = string.Empty,
            break_in_time = string.Empty, break_out_time = string.Empty,
            remarks = string.Empty, query = string.Empty,currentUser = string.Empty;
        currentUser = HttpContext.Current.Session["username"].ToString();
        try
        {
            current_data = JObject.Parse(current);

            employee_code = current_data["employee_id"].ToString();

            in_punch_date = current_data["punch_in_date"].ToString();
            in_punch_time = current_data["punch_in_time"].ToString();

            out_punch_date = current_data["punch_out_date"].ToString();
            out_punch_time = current_data["punch_out_time"].ToString();

            break_in_date = current_data["break_in_date"].ToString();
            break_in_time = current_data["break_in_time"].ToString();

            break_out_date = current_data["break_out_date"].ToString();
            break_out_time = current_data["break_out_time"].ToString();

            remarks = current_data["remarks"].ToString();

            validation = page_object.CheckDatesForManualPunch(employee_code,in_punch_date, in_punch_time, out_punch_date, out_punch_time, break_in_date, break_in_time, break_out_date, break_out_time);

            if (validation[0] == null)
            {
                query = "select count(*) from punchforapproval where empcode='" + employee_code + "' and workdate = '" + in_punch_date + "' and Approve in (1,3,4) ";
                int count = db_connection.GetRecordCount(query);

                if (count >= 0)
                {
                    query = "select approve from punchforapproval where empcode='" + employee_code + "' and workdate = '" + in_punch_date + "' order by ID desc";
                    approval_status = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                    if (approval_status == 1)
                    {
                        if (Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 1 && employee_code != HttpContext.Current.Session["employee_id"].ToString())
                        {
                            query = "select ID from PunchForApproval where  EmpCode='" + employee_code + "' and WorkDate='" + in_punch_date + "' and Approve=1 ";
                            manualpunch_id = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                            if (manualpunch_id > 0)
                            {
                                // for two level approval
                                page_object.ApproveMisingpunch(manualpunch_id, 5);
                            }
                        }
                        else
                        {
                            EmailMessage = page_object.InsertData("U", employee_code, in_punch_date, in_punch_time, out_punch_time, break_out_date, break_in_date, remarks, DateTime.ParseExact(out_punch_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"), DateTime.ParseExact(in_punch_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"));
                            page_object.SendMail(employee_code, in_punch_date, in_punch_time, out_punch_time, remarks, 2);

                            retrun_message = "Manual Punch entry updated successfully";
                        }
                    }
                    else if (approval_status == 0 || approval_status == 3 || approval_status == 4)
                    {
                        EmailMessage = page_object.InsertData("I", employee_code, in_punch_date, in_punch_time, out_punch_time, break_out_date, break_in_date, remarks, DateTime.ParseExact(out_punch_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"), DateTime.ParseExact(in_punch_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"));


                        if (Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 1 && employee_code != HttpContext.Current.Session["employee_id"].ToString())
                        {
                            query = "select ID from PunchForApproval where  EmpCode='" + employee_code + "' and WorkDate='" + in_punch_date + "' and Approve=1 ";
                            manualpunch_id = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                            if (manualpunch_id > 0)
                            {
                                // for two level approval
                                page_object.ApproveMisingpunch(manualpunch_id, 5);
                            }
                        }
                        else if (currentUser != "admin")
                        {
                            page_object.SendMail(employee_code, in_punch_date, in_punch_time, out_punch_time, remarks, 2);
                        }
                        retrun_message = "Manual Punch entry saved successfully";
                    }
                    else if (approval_status == 2)
                    {
                        retrun_message = "Manual Punch has been approved for the selected dates. Please try with different dates.";
                    }
                }
                else
                {
                    retrun_message = "Manual Punch has been approved for the selected dates. Please try with different dates.";
                }

                //this point need to discuss with FM and Madhu
                //Auto Manual Punch Approval
                if (( Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 0 || Convert.ToInt32(HttpContext.Current.Session["access_level"]) == 3) && employee_code != HttpContext.Current.Session["employee_id"].ToString())
                {
                    query = "select ID from PunchForApproval where  EmpCode='" + employee_code + "' and WorkDate='" + in_punch_date + "' and Approve=1 ";
                    manualpunch_id = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                    if (manualpunch_id > 0)
                    {
                        page_object.ApproveMisingpunch(manualpunch_id, 2);
                    }
                }
                

                return_object.status = "success";
                return_object.return_data = retrun_message;
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = validation[0].ToString();
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SAVE_MANUAL_PUNCH");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving Manual Punch. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    protected void ApproveMisingpunch(long Missingpunchid, int flag)
    {
        manual_apply page_object = new manual_apply();
        DBConnection db_connection = new DBConnection();
        Hashtable hsh = new Hashtable();
        string query = string.Empty, employee_name = string.Empty;
        DataTable manual_punch_data = new DataTable();

        employee_name = HttpContext.Current.Session["employee_name"].ToString();
        if (string.IsNullOrEmpty(employee_name))
        {
            employee_name = "Admin";
        }
        if (flag != 5)
        {
            hsh.Add("piID", Missingpunchid);
            hsh.Add("piApprove", flag);
            hsh.Add("piApprovedBy", employee_name);
            hsh.Add("poRetVal", 0);
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("SpApprovePunch1", hsh);
        }

        //get the manul punch details by ID and save it in MailSender table
        query = "select EmpCode,WorkDate,convert(varchar(5),InPunch,8) as InPunch,convert(varchar(5),OutPunch,108) as OutPunch,convert(varchar(5),BreakOut,108) as BreakOut,convert(varchar(5),BreakIn,108) as BreakIn, Remarks from PunchForApproval where ID=" + Missingpunchid;
        manual_punch_data = db_connection.ReturnDataTable(query);

        page_object.SendMailforApproval(manual_punch_data.Rows[0]["EmpCode"].ToString(), manual_punch_data.Rows[0]["WorkDate"].ToString(), manual_punch_data.Rows[0]["InPunch"].ToString(), manual_punch_data.Rows[0]["OutPunch"].ToString(), manual_punch_data.Rows[0]["Remarks"].ToString(), flag, Convert.ToInt32(HttpContext.Current.Session["access_level"]), HttpContext.Current.Session["username"].ToString());

    }

    [WebMethod]
    public static ReturnObject ValidateEmployeeId(string employee_id)
    {
        manual_apply page_object = new manual_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable Branchlisttable = new DataTable();
        DataTable CoManagerID_data = new DataTable();
        DataTable innermanagertable = new DataTable();

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
            if (employee_id != "")
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

                return_object.status = "success";
                return_object.return_data = employee_id.ToString();
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

    private string[] GetEmployeeCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable branch_list_data = new DataTable();
        DataTable employee_data = new DataTable();

        int IsDelegationManager = 0, user_access_level = 0;
        string[] employee_code_array = null;
        string CoManagerID = string.Empty, query = string.Empty, BranchList = string.Empty, employee_id = string.Empty;

        employee_id = HttpContext.Current.Session["username"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        //check employee is Delegation Manager or not if so get his CoManagerID
        IsDelegationManager = Convert.ToInt32(db_connection.ExecuteQuery_WithReturnValueString("Select COUNT(DelidationManagerID) from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)"));
        if (IsDelegationManager > 0)
            CoManagerID = db_connection.ExecuteQuery_WithReturnValueString("Select ManagerId from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1");

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

        //Validate CoManagerID
        if (string.IsNullOrEmpty(CoManagerID))
        {
            CoManagerID = "'Empty'";
        }

        //modify query as per access level
        if (user_access_level == 0)//Admin
        {
            query += " select Emp_Code from EmployeeMaster where Emp_Status=1";
        }
        else if (user_access_level == 3)//HR
        {
            query += "select Emp_Code from EmployeeMaster where (ManagerID='" + employee_id + "' or  Emp_Code='" + employee_id + "' or Emp_Branch In(" + BranchList + ")) and Emp_Status=1 ";
        }
        else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
        {
            query += " select Emp_Code from EmployeeMaster where  Emp_Code in ( select Emp_Code from EmployeeMaster where ((managerId in ('" + employee_id + "'," + CoManagerID + ") or Emp_Code='" + employee_id + "') and Emp_Status=1 )  or Emp_Branch in (" + BranchList + "))";
        }
        else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager
        {
            query += " select Emp_Code from EmployeeMaster where Emp_Code in ( select Emp_Code from EmployeeMaster where ((managerId='" + employee_id + "' or Emp_Code='" + employee_id + "')  and Emp_Status=1 ) or Emp_Branch in (" + BranchList + "))";
        }

        employee_data = db_connection.ReturnDataTable(query);

        employee_code_array = new string[employee_data.Rows.Count];

        for (int i = 0; i < employee_data.Rows.Count; i++)
        {
            employee_code_array[i] = employee_data.Rows[i]["Emp_Code"].ToString().ToUpper();
        }

        return employee_code_array;
    }

    private void SaveManualPunchData(string emp_code, string work_date, string in_punch, string out_punch, string remarks)
    {
        Hashtable hshParam = new Hashtable();
        DBConnection db_connection = new DBConnection();

        string employee_name = HttpContext.Current.Session["employee_name"].ToString();
        string query = "select * from dbo.PunchForApproval where EmpCode='" + emp_code + "' and WorkDate='" + work_date + "' And Approve In('1','2')";
        int record_count = db_connection.GetRecordCount(query);

        if (record_count > 0)
        {
            hshParam.Add("Mode", "U");
            hshParam.Add("ModifiedBy", employee_name);
        }
        else
        {
            hshParam.Add("piMode", "I");
            hshParam.Add("piEmpCode", emp_code);
            hshParam.Add("piWorkDate", work_date);
            hshParam.Add("piInPunch", in_punch);
            hshParam.Add("piOutPunch", out_punch);
            hshParam.Add("piRemarks", remarks);
            hshParam.Add("pioutdate", work_date);
            hshParam.Add("piCreatedBy", Session["username"]);
            hshParam.Add("piModifiedBy", "");

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("spSavePunchForApproval", hshParam);
        }
    }

    public string ImportManualPunch(DataTable manual_punch_data)
    {
        DBConnection db_connection = new DBConnection();

        int line_no = 0, record_count = 0, manual_punch_id = 0;
        Hashtable hshParam = new Hashtable();
        string[]
             employee_code_array = null;

        string emp_code = string.Empty,
            work_date = string.Empty,
            in_punch = string.Empty,
            out_punch = string.Empty,
            remarks = string.Empty,
            manager_id = string.Empty,
            error_message = string.Empty,
            query = string.Empty,
            branch_id=string.Empty;
            

        bool empcode = true,
            workdate = true,
            inpunch = true,
            outpunch = true,
            branchid = true;

        string employee_id = HttpContext.Current.Session["username"].ToString();
        string employee_name = HttpContext.Current.Session["employee_name"].ToString();
        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());

        try
        {
            employee_code_array = GetEmployeeCodes();
            line_no = 2;

            foreach (DataRow dr in manual_punch_data.Rows)
            {
                line_no++;

                empcode = true; workdate = true; inpunch = true; outpunch = true; branchid = true;

                if (string.IsNullOrEmpty(dr["BRANCH_ID"].ToString()))
                {
                    error_message += Environment.NewLine + "EmpCode is Null or empty on line Number:    " + line_no;
                    branchid = false;
                }
                else
                {
                    branch_id = dr["BRANCH_ID"].ToString();
                }
                 if (string.IsNullOrEmpty(dr["EMP_CODE"].ToString()))
                {
                    error_message += Environment.NewLine + "EmpCode is Null or empty on line Number:    " + line_no;
                    empcode = false;
                }
                else
                {
                    emp_code = dr["EMP_CODE"].ToString();
                }
                //BRANCH_ID
                if (string.IsNullOrEmpty(dr["WORKDATE"].ToString()))
                {
                    workdate = false;
                    error_message += Environment.NewLine + "Work Date is Null or empty on line Number:    " + line_no;
                }
                else
                {
                    work_date = Convert.ToDateTime(dr["WORKDATE"].ToString()).ToString("yyyy-MMM-dd");
                }

                if (string.IsNullOrEmpty(dr["INPUNCH"].ToString()))
                {
                    inpunch = false;
                    error_message += Environment.NewLine + "In Punch is Null or empty on line Number:    " + line_no;
                }
                else
                {
                    in_punch = Convert.ToDateTime(dr["INPUNCH"].ToString()).ToString("HH:mm");
                }

                if (string.IsNullOrEmpty(dr["OUTPUNCH"].ToString()))
                {
                    outpunch = false;
                    error_message += Environment.NewLine + "Out Punch is Null or empty on line Number:    " + line_no;
                }
                else
                {
                    out_punch = Convert.ToDateTime(dr["OUTPUNCH"].ToString()).ToString("HH:mm");
                }


                //checking punch exist in masterprocess dailydata

                string punchTime = string.Empty, punchTimeToHour=string.Empty;
                bool isRecordExist = false;
                if (db_connection.RecordExist("select count(*) from masterprocessdailydata where status='MS' and pdate='" + work_date + "' and emp_id='" + emp_code + "'"))
                {
                    query = "select convert(time(5),in_punch)as punch from masterprocessdailydata where status='MS' and pdate='" + work_date + "' and emp_id='" + emp_code + "'";
                    punchTime = db_connection.ExecuteQuery_WithReturnValueString(query);
                    punchTimeToHour = Convert.ToDateTime(punchTime).ToString("HH:mm");
                    if (punchTimeToHour.Equals(in_punch) || punchTimeToHour.Equals(out_punch))
                    {
                        isRecordExist = true;
                    }
                }
                else if (db_connection.RecordExist("select count(*) from masterprocessdailydata where status='A' and pdate='" + work_date + "' and emp_id='" + emp_code + "'"))
                {
                        isRecordExist = true;
                }


                remarks = dr["remarks"].ToString();

                if (user_access_level == 2)
                {
                    if (employee_id == emp_code)
                    {
                        if (empcode & workdate & inpunch & outpunch)
                        {
                            SaveManualPunchData(emp_code, work_date, in_punch, out_punch, remarks);
                            error_message += Environment.NewLine + "Manual punch has submitted for employee id: " + emp_code;
                        }
                    }
                    else
                    {
                        error_message += Environment.NewLine + "You don`t have permission to upload Manual Punch data for employee id: " + emp_code;
                    }
                }
                else
                {
                    if (Array.IndexOf(employee_code_array, emp_code) > 0 && isRecordExist==true)
                    {

                        if (empcode & workdate & inpunch & outpunch)
                        {
                            SaveManualPunchData(emp_code, work_date, in_punch, out_punch, remarks);
                            error_message += Environment.NewLine + "Manual punch has submitted for employee id: " + emp_code;

                            if (employee_id != emp_code)
                            {
                                manual_punch_id = db_connection.ExecuteQuery_WithReturnValueInteger("select ID from dbo.PunchForApproval where EmpCode='" + emp_code + "' and WorkDate='" + work_date + "'");
                                ApproveMisingpunch(manual_punch_id, 2);
                                error_message += Environment.NewLine + "Manual punch has approved for employee id: " + emp_code;
                            }
                            else
                            {
                                error_message += Environment.NewLine + "You dont have permission to Approve Manual Punch Data for employee id: " + emp_code;
                            }
                        }
                    }
                    else
                    {
                        if (isRecordExist == false)
                        {
                            error_message += Environment.NewLine + "you can't change actual punches through import for employee : " + emp_code+" for date : "+work_date;
                        }
                        else
                        {
                            error_message += Environment.NewLine + "You dont have permission to upload Manual Punch Data for employee id: " + emp_code;
                        }
                        
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_MANUAL_PUNCH");
        }

        return error_message;
    }

    [WebMethod]
    public static ReturnObject DoImport(string file_name)
    {
        manual_apply page_object = new manual_apply();
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

            // Read the excel file and store the data in a DataTable.
            excel_data = ExcelImport.ImportExcelToDataTable(full_upload_path, "");
            // Get the 1st ROW of the EXCEL sheet
            first_row = excel_data.Rows[0];
            // Remove the 1st ROW of the EXCEL sheet. This is essentially the title row.
            excel_data.Rows.Remove(first_row);

            return_message = page_object.ImportManualPunch(excel_data);

            return_object.status = "success";
            return_object.return_data = return_message;

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_MANUAL_PUNCH");

            return_object.status = "error";
            return_object.return_data = "An error occurred while importing manual punch. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }
}