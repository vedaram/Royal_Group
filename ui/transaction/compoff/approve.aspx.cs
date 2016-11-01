using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class compoff_approve : System.Web.UI.Page
{
    const string page = "COMPOFF_APPROVAL";

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


    protected string GetFilterQuery(string filtersData, string query)
    {
        JObject filter_data = JObject.Parse(filtersData);

        string filter_keyword = filter_data["filter_keyword"].ToString();
        string filter_CompoffStatus = filter_data["filter_CompoffStatus"].ToString();
        int filter_by = Convert.ToInt32(filter_data["filter_by"]);
        string filter_indate = filter_data["filter_indate"].ToString();
        string filter_outdate = filter_data["filter_outdate"].ToString();

        if (filter_by == 1)
        {
            query += " and e.Emp_Code = '" + filter_keyword + "' ";
        }
        if (filter_by == 2)
        {
            query += " and e.Emp_Name like '%" + filter_keyword + "%' ";
        }

        if (filter_indate != "")
        {
            query += " and l.fromdate >= '" + filter_indate + "' ";
        }

        if (filter_outdate != "")
        {
            query += " and l.todate <= '" + filter_outdate + "' ";
        }

        if (filter_CompoffStatus != "select")
        {
            query += " and l.Flag = '" + filter_CompoffStatus + "' ";
        }

        return query;
    }

    protected string GetBaseQuery()
    {
        string query = @"SELECT compoff_id, EmpID, Emp_Name, fromdate, todate, Leave_Status_text, Flag,ApprovedbyName FROM (
                    SELECT l.compoff_id, l.EmpID, e.Emp_Name, l.fromdate, l.todate, ls.Leave_Status_text, l.Flag,ApprovedbyName, ROW_NUMBER()
                    OVER (ORDER BY compoff_id) as row FROM compoffdetails l, EmployeeMaster e, leave_status ls where e.Emp_Code = l.empid 
                    and ls.Leave_Status_id = l.Flag ";

        return query;
    }

    [WebMethod]
    public static ReturnObject GetCompOffData(int page_number, bool is_filter, string filters)
    {
        compoff_approve page_object = new compoff_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable leave_listing = new DataTable();
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

            query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + employee_id + "'";
            branch_list_data = db_connection.ReturnDataTable(query);
            query = string.Empty;

            query = page_object.GetBaseQuery(); //read main query

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
                query += " ";
            }
            else if (user_access_level == 3)//HR
            {
                query += " and ( l.EmpID='" + employee_id + "' or e.Emp_Branch In(" + BranchList + ")) ";
            }
            else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
            {
                query += " and l.EmpID in ( select Emp_Code from EmployeeMaster where ((managerId in ('" + employee_id + "'," + CoManagerID + ") or Emp_Code='" + employee_id + "') and Emp_Status=1 )  or Emp_Branch in (" + BranchList + "))";
            }
            else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager
            {
                query += " and l.EmpID in ( select Emp_Code from EmployeeMaster where ((managerId='" + employee_id + "' or Emp_Code='" + employee_id + "')  and Emp_Status=1 ) or Emp_Branch in (" + BranchList + "))";
            }
            else
            {
                query += " and 1=0";// Only Employee
            }

            if (is_filter)
            {
                query = page_object.GetFilterQuery(filters, query);
            }
            else
            {
                query += " and l.Flag=1 ";

            }

            query += " ) a where row > " + start_row + " and row < " + number_of_record;
            query += "  order by a.fromdate desc";

            leave_listing = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leave_listing, Formatting.Indented);
        }
        catch (Exception Ex)
        {
            Logger.LogException(Ex, page, "GET_COMPOFF_DATA");
            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leave Type data. Please try again. If the error persists, please contact Support.";
        }


        return return_object;
    }

    private void updatecompoff(string employee_id, int flag, long compoff_id, int number_of_days, string comments)
    {
        try
        {
            DBConnection db_connection = new DBConnection();
            string Approvedby = HttpContext.Current.Session["employee_name"].ToString();
            if (string.IsNullOrEmpty(Approvedby))
            {
                Approvedby = "Admin";
            }
            Hashtable comp_off_details = new Hashtable();

            comp_off_details.Add("empid", employee_id);
            comp_off_details.Add("flag", flag);
            comp_off_details.Add("compffid", compoff_id);
            comp_off_details.Add("count", number_of_days);
            comp_off_details.Add("comment", comments);
            comp_off_details.Add("approvedby", Approvedby);

            db_connection.ExecuteStoredProcedureReturnInteger("spUpdatecompoff", comp_off_details);

            Hashtable hsh = new Hashtable();
            hsh.Add("empid", employee_id);
            hsh.Add("flag", flag);
            hsh.Add("compffid", compoff_id);
            hsh.Add("count", number_of_days);
            hsh.Add("comment", comments);
            db_connection.ExecuteStoredProcedureReturnInteger("spInsertcompoffSplit", hsh);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_COMPOFF");

        }
    }

    private void sendMail(string employee_id, string from_date, string to_date, string remarks, string Comp_Off_status)
    {
        Hashtable leave_send_mail = new Hashtable();
        DataTable employee_record = new DataTable();
        DBConnection db_connection = new DBConnection();

        string
            email_id = string.Empty, query = string.Empty,
            employee_name = string.Empty, employee_email = string.Empty,
            leave_type = string.Empty, Leave_date = string.Empty, manager_email = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;
        try
        {
            employee_record = db_connection.ReturnDataTable("select Emp_Name,Emp_Email from EmployeeMaster where Emp_code='" + employee_id + "'");

            employee_name = employee_record.Rows[0]["Emp_Name"].ToString();
            employee_email = employee_record.Rows[0]["Emp_Email"].ToString();

            manager_email = db_connection.ExecuteQuery_WithReturnValueString("select Emp_Email  from EmployeeMaster where Emp_Code = (select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')");

            if (!string.IsNullOrEmpty(manager_email))
            {
                manager_email = "";
            }

            if (!string.IsNullOrEmpty(employee_email))
            {
                MailSubject = "Regarding Comp Off Application";
                MailBody = "Dear " + employee_name + ", <br/><br/> Your Comp Off is " + Comp_Off_status + " for the date which is as per the following :<br/><br/> From Date: " + from_date + "<br/><br/> To Date: " + to_date + "<br/><br/>Thanks.";

                leave_send_mail.Add("EmpID", employee_id);
                leave_send_mail.Add("EmpName", employee_name);
                leave_send_mail.Add("ToEmailID", employee_email);
                leave_send_mail.Add("CCEmailID", manager_email);
                leave_send_mail.Add("Subject", MailSubject);
                leave_send_mail.Add("Body", MailBody);

                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", leave_send_mail);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SEND_MAIL");
        }
    }

    [WebMethod]
    public static string changeCompOffStatus(string jsonData)
    {
        DBConnection db_connection = new DBConnection();
        Dictionary<string, string> return_data = new Dictionary<string, string>();

        try
        {
            JObject json_data = JObject.Parse(jsonData);
            string employee_id = json_data["EmpID"].ToString();
            string from_date = json_data["fromdate"].ToString();
            string to_date = json_data["todate"].ToString();
            string comments = json_data["remarks"].ToString();
            int compoff_id = Convert.ToInt32(json_data["compoff_id"]);

            DateTime formatted_from_date = new DateTime();
            DateTime formatted_to_date = new DateTime();

            formatted_from_date = DateTime.ParseExact(from_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            formatted_to_date = DateTime.ParseExact(to_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            int number_of_days = (formatted_to_date - formatted_from_date).Days + 1;

            int flag = Convert.ToInt32(json_data["flag"]);

            compoff_approve page_object = new compoff_approve();

            page_object.updatecompoff(employee_id, flag, compoff_id, number_of_days, comments);

            // string message = page_object.sendMail(jsonData);
            string message = "";
            return_data.Add("status", "success");

            return_data.Add("message", "Comp Off requested successfully" + message);

            page_object.Dispose();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHANGE_COMPOFF_STATUS");

            return_data.Add("status", "error");
            return_data.Add("message", "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.");
        }

        return JsonConvert.SerializeObject(return_data, Formatting.Indented);
    }

    protected void PrintExcel(DataTable table)
    {

        string attach = "attachment;filename=ApproveCompOff.xls";
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

        DataTable leave_data_table = new DataTable();
        DBConnection db_connection = new DBConnection();
        string query;

        try
        {

            int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            string employee_id = HttpContext.Current.Session["username"].ToString();

            if (user_access_level == 0)
            {
                query = "SELECT l.compoff_id, l.EmpID, e.Emp_Name, l.fromdate, l.todate, ls.Leave_Status_text, l.Flag FROM compoffdetails l, EmployeeMaster e, leave_status ls where e.Emp_Code = l.empid and ls.Leave_Status_id = l.Flag order by l.compoff_id";
            }
            else if (user_access_level == 1)
            {
                query = "SELECT l.compoff_id, l.EmpID, e.Emp_Name, l.fromdate, l.todate, ls.Leave_Status_text, l.Flag FROM compoffdetails l, EmployeeMaster e, leave_status ls where e.Emp_Code = l.empid and ls.Leave_Status_id = l.Flag and l.EmpID in (select emp_code from EmployeeMaster where managerId = '" + employee_id + "' ) order by l.compoff_id";
            }
            else
            {
                query = "SELECT l.compoff_id, l.EmpID, e.Emp_Name, l.fromdate, l.todate, ls.Leave_Status_text, l.Flag FROM compoffdetails l, EmployeeMaster e, leave_status ls where e.Emp_Code = l.empid and ls.Leave_Status_id = l.Flag and l.EmpID = '" + employee_id + "') order by l.compoff_id";
            }

            leave_data_table = db_connection.ReturnDataTable(query);

            PrintExcel(leave_data_table);
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

    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }

    [WebMethod]
    public static ReturnObject DoAction(int action, string comments, string selected_rows)
    {
        compoff_approve page_object = new compoff_approve();
        DBConnection db_connection = new DBConnection();
        DataTable Compoff_data = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string empid = string.Empty, Query = string.Empty, From = string.Empty, Todate = string.Empty, Remarks = string.Empty;
        int No_of_days = 0, last_flag = 0;

        List<string> selected_manual_punch = JsonConvert.DeserializeObject<List<string>>(selected_rows);

        string
            query = string.Empty, retrn_message = string.Empty,
            Comp_Off_status = string.Empty;

        long
            Comp_Off_id = 0;

        if (HttpContext.Current.Session["username"] == null)
        {
           return_object = page_object.DoLogout();
        }
        else
        {

            try
            {
                switch (action)
                {
                    case 2:
                        Comp_Off_status = "Approved";
                        break;

                    case 3:
                        Comp_Off_status = "Declined";
                        break;

                    case 4:
                        Comp_Off_status = "Cancelled";
                        break;
                }

                for (int i = 0; i < selected_manual_punch.Count; i++)
                {
                    Comp_Off_id = Convert.ToInt64(selected_manual_punch[i].ToString());
                    Query = "select * From compoffdetails where compoff_id='" + Comp_Off_id + "'";
                    Compoff_data = db_connection.ReturnDataTable(Query);
                    empid = Compoff_data.Rows[0]["EMPID"].ToString();
                    From = Compoff_data.Rows[0]["FromDate"].ToString();
                    Todate = Compoff_data.Rows[0]["ToDate"].ToString();
                    Remarks = Compoff_data.Rows[0]["Remarks"].ToString();
                    last_flag = Convert.ToInt32(Compoff_data.Rows[0]["Flag"].ToString());

                    if (last_flag == 2 && action == 3)
                    {
                        retrn_message += "Comp Off already Approved, you can't decline it.";
                    }
                    else if (last_flag == 3 && action == 2)
                    {
                        retrn_message += "Comp Off already Declined, you can't approve it.";
                    }
                    else if (last_flag == action)
                    {
                        retrn_message += "Comp Off already " + Comp_Off_status;
                    }
                    else
                    {
                        page_object.updatecompoff(empid, action, Comp_Off_id, No_of_days, comments);
                        page_object.sendMail(empid, From, Todate, Remarks, Comp_Off_status);
                        retrn_message = "Comp off status updated successfully!";
                    }
                }

                return_object.status = "success";

                return_object.return_data = retrn_message;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "UPDATE_COMPOFF_APPROVAL_STATUS");

                return_object.status = "error";
                return_object.return_data = "An error occurred while updating Leave Approval Status. Please try again. If the error persists, please contact Support.";
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }
}
