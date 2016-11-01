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
using System.Xml.Linq;
using System.Text;
using System.Configuration;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Logger;

public partial class manual_approve : System.Web.UI.Page
{
    const string page = "MANUAL_PUNCH_APPROVAL";

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

    private string GetBaseQuery()
    {
        string query = "select id, Empcode, Emp_Name, WorkDate, outdate, InPunch, OutPunch, approve, Leave_Status_text, ReasonForManualPunch,Manager_remark,Hr_remark,Modifiedby from ( select pa.id, pa.Empcode, e.Emp_Name, pa.WorkDate, pa.outdate, pa.InPunch, pa.OutPunch, pa.approve, ls.Leave_Status_text, pa.ReasonForManualPunch,pa.Manager_remark,pa.Hr_remark,pa.Modifiedby, ROW_NUMBER() OVER (ORDER BY pa.id desc) as row from PunchForApproval pa, EmployeeMaster e, Leave_status ls  where e.Emp_code = pa.Empcode";
        int user_access_level = 0;
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        // and ls.Leave_Status_id = pa.approve  "
          if (user_access_level == 3)//HR
        {
            query += " and ls.Leave_Status_id=pa.Hr_approval";
        }
        else if (user_access_level == 1)//manager
        {
            query += " and ls.Leave_Status_id=pa.manager_approval";
        }
          

        return query;
    }

    private string GetFilterQuery(string filters, string query)
    {
        JObject filters_data = JObject.Parse(filters);
        string from_date = filters_data["filter_indate"].ToString();
        string to_date = filters_data["filter_outdate"].ToString();
        string manual_pucnh_status = filters_data["filter_ManualStatus"].ToString();
        string filter_keyword = filters_data["filter_keyword"].ToString();
        int user_access_level =0;
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        if (user_access_level == 0)//Admin
        {
            query += " ";

        }
        else if (user_access_level == 3)//HR
        {
           
            if (manual_pucnh_status == "select")
            {
                query += " and pa.Hr_approval in(0,1,2,3)";
            }
            else
            {
                query += " and pa.Hr_approval='" + manual_pucnh_status + "'";
            }
        }
        else if (user_access_level == 1)//manager
        {
            if (manual_pucnh_status == "select")
            {
                query += " and pa.manager_approval in(0,1,2,3)";
            }
            else
            {
                query += " and pa.manager_approval='" + manual_pucnh_status + "'";
            }
           
        }

        switch (filter_by)
        {
            case 1:
                query += " and e.Emp_Code='" + filter_keyword + "'";
                break;
            case 2:
                query += " and e.Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

        //cheking manager or hr 

       
 



        //==========Manual Pucnh Status==========
        //if (manual_pucnh_status != "select")
        //    query += " and pa.approve='" + manual_pucnh_status + "'";

        //========from date and to date============
        if (from_date != "" && to_date != "")
        {
            query += " and pa.WorkDate between '" + from_date + "' and '" + to_date + "'";
        }
        
        return query;
    }

    [WebMethod]
    public static ReturnObject GetManualPunchData(int page_number, bool is_filter, string filters)
    {
        manual_approve page_object = new manual_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable manual_punch_data_table = new DataTable();
        DataTable branch_list_data = new DataTable();
        DataTable CoManagerID_data = new DataTable();
        int flag = 0;
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
                flag = 1;
                query += " and e.Emp_Code in (select Emp_Code from EmployeeMaster where Emp_Branch In (" + BranchList + ")) ";
            }
            else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
            {
                flag = 2;
                query += " and e.Emp_Code in ( select Emp_Code from EmployeeMaster where ((managerId in ('" + employee_id + "'," + CoManagerID + ")) and Emp_Status=1 )  or Emp_Branch in (" + BranchList + "))";
            }
            else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager
            {
                flag = 2;
                query += " and e.Emp_Code in ( select Emp_Code from EmployeeMaster where (managerId='" + employee_id + "'  and Emp_Status=1 ) or Emp_Branch in (" + BranchList + "))";
            }
            else
            {
                query += " and 1=0 '" + employee_id + "'";// Only Employee but for approval employee don't have access to page
            }

            if (is_filter)
            {
                query = page_object.GetFilterQuery(filters, query);
                //if (flag == 1)
                //{
                //    query += "  and pa.Hr_approval=1 ";
                //}
                //else if (flag == 2)
                //{
                //    query += "  and pa.manager_approval=1 ";
                //}
                //else
                //{
                //    query += " ";
                //}
            }
            else
            {
                if (flag==1)
                {
                    query += " and pa.approve=1 and pa.Hr_approval=1 ";
                }
                else if (flag == 2)
                {
                    query += " and pa.approve=1 and pa.manager_approval=1 ";
                }
                else
                {
                    query += " and pa.approve=1";
                }
               
            }
            //manager_approval=0,Hr_approval=1
            query += " ) a where row > " + start_row + " and row < " + number_of_record;
            query += "  order by a.workdate desc";

            manual_punch_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(manual_punch_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_MANUAL_PUNCHES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static string changeManualPunchStatus(string jsonData, int flag)
    {

        DBConnection db_connection = new DBConnection();

        Dictionary<string, string> return_data = new Dictionary<string, string>();

        return JsonConvert.SerializeObject(return_data, Formatting.Indented);
    }

    protected void PrintExcel(DataTable table)
    {

        string attach = "attachment;filename=ManualPunchApproval.xls";
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

        DataTable manual_punch_data_table = new DataTable();

        string query = "";

        string employee_id = HttpContext.Current.Session["employee_id"].ToString();

        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (user_access_level == 0)
        {

            query = "";
        }
        else
        {

            query = "";
        }

        manual_punch_data_table = db_connection.ReturnDataTable(query);

        PrintExcel(manual_punch_data_table);
    }

    [WebMethod]
    public static ReturnObject DoAction(int action, string comments, string selected_rows)
    {
        manual_approve page_object = new manual_approve();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        bool isRecordApproved = false,allreadyApproved=false,alreadyDeclined=false;
        List<string> selected_manual_punch = JsonConvert.DeserializeObject<List<string>>(selected_rows);

        string
            query = string.Empty, return_message = string.Empty,
            manual_punch_status = string.Empty, alternativehrincharge = string.Empty, hrincharge = string.Empty;

        long
            manual_punch_id = 0;
        int update_flag = 0, Existsing_Status = 0, count = 0, user_access_level=0;
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        try
        {
            string employee_name = HttpContext.Current.Session["employee_name"].ToString();
            if (string.IsNullOrEmpty(employee_name))
            {
                employee_name = "Admin";
            }

            switch (action)
            {
                case 2:
                    manual_punch_status = "Approved";
                  
                    update_flag = 2;


                    break;

                case 3:
                    manual_punch_status = "Declined";
                    update_flag = 3;
                    break;

                case 4:
                    manual_punch_status = "Cancelled";
                    update_flag = 4;
                    break;
            }
            //cheking record already approved or not 

            for (int i = 0; i < selected_manual_punch.Count; i++)
            {
                switch (action)
                {
                    case 2:
                        manual_punch_status = "Approved";

                        update_flag = 2;


                        break;

                    case 3:
                        manual_punch_status = "Declined";
                        update_flag = 3;
                        break;

                    case 4:
                        manual_punch_status = "Cancelled";
                        update_flag = 4;
                        break;
                }

                manual_punch_id = Convert.ToInt64(selected_manual_punch[i].ToString());

                query = "select approve from PunchForApproval where ID='" + manual_punch_id + "' ";
                Existsing_Status = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                int access_level = 0;
                access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
                // and ls.Leave_Status_id = pa.approve  "
                if (access_level == 3)//HR
                {
                    query = "select Hr_approval from PunchForApproval where ID='" + manual_punch_id + "' ";
                    Existsing_Status = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                }
                else if (access_level == 1)//manager
                {

                    query = "select manager_approval from PunchForApproval where ID='" + manual_punch_id + "' ";
                    Existsing_Status = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                }
                else
                {
                    query = "select approve from PunchForApproval where ID='" + manual_punch_id + "' ";
                    Existsing_Status = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                }


                if (Existsing_Status == 2)
                {
                   // return_message += "Manual punch is already approved" + Environment.NewLine;
                    isRecordApproved = true;
                    allreadyApproved = true;
                  // break;
                }
                else if (Existsing_Status == 3)
                {
                   // return_message += "Manual punch is already declined" + Environment.NewLine;
                    isRecordApproved = true;
                    alreadyDeclined = true;
                   // break;
                }
                 
            }
            //end of record already approved or not 
            if (isRecordApproved == false)
            {
                for (int i = 0; i < selected_manual_punch.Count; i++)
                {
                    switch (action)
                    {
                        case 2:
                            manual_punch_status = "Approved";

                            update_flag = 2;


                            break;

                        case 3:
                            manual_punch_status = "Declined";
                            update_flag = 3;
                            break;

                        case 4:
                            manual_punch_status = "Cancelled";
                            update_flag = 4;
                            break;
                    }

                    manual_punch_id = Convert.ToInt64(selected_manual_punch[i].ToString());



                    //when hr approve record
                    if (user_access_level == 3 && update_flag == 2)
                    {
                        query = "update punchforapproval set Hr_approval=2,ModifiedBy='" + employee_name + "',Hr_remark='"+comments+"' where id='" + manual_punch_id + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        update_flag = 4;
                    }
                    //when hr decline record
                    else if (user_access_level == 3 && update_flag == 3)
                    {
                        query = "update punchforapproval set Hr_approval=3,manager_approval=1 ,ModifiedBy='" + employee_name + "' ,Hr_remark='" + comments + "' where id='" + manual_punch_id + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        update_flag = 0;
                    }
                    //when Manager decline record
                    else if (user_access_level == 1 && update_flag == 3)
                    {
                        query = "update punchforapproval set manager_approval=3,ModifiedBy='" + employee_name + "' ,Manager_remark='" + comments + "' where id='" + manual_punch_id + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        // update_flag = 0;
                    }
                    //when Manager approve record
                    else if (user_access_level == 1 && update_flag == 2)
                    {
                        //if record already declined by Hr
                        query = "select count(*) from punchforapproval where Hr_approval=3 and  id='" + manual_punch_id + "'";

                        count = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                        if (count > 0)
                        {
                            query = "update punchforapproval set manager_approval=2,ModifiedBy='" + employee_name + "', Manager_remark='" + comments + "' where id='" + manual_punch_id + "'";
                            db_connection.ExecuteQuery_WithOutReturnValue(query);
                            update_flag = 2;
                        }
                        //if record approved by manager & sending for Hr
                        else
                        {

                            query = "update punchforapproval set manager_approval=2,Hr_approval=1,ModifiedBy='" + employee_name + "',Manager_remark='" + comments + "' where id='" + manual_punch_id + "'";
                            db_connection.ExecuteQuery_WithOutReturnValue(query);
                            update_flag = 1;
                        }
                    }

                    page_object.ApproveMisingpunch(manual_punch_id, update_flag);


                }//end of for
                return_object.status = "success";
                return_message = "ManualPunch " + manual_punch_status + " successfully!" + Environment.NewLine;

            }//end of main if
            else
            {
                return_object.status = "error";
                 
                    
                    if (allreadyApproved == true && alreadyDeclined==false)
                {
                    return_message = "Manual punch is already approved" + Environment.NewLine;
                }
                    else if (allreadyApproved == false && alreadyDeclined == true)
                    {
                        return_message = "Manual punch is already Declined" + Environment.NewLine;
                    }
                    else
                    {
                        return_message = "There is an approved or declined record in selection please check";  
                    }
                //return_message = "Manual punch is already approved" + Environment.NewLine;
                //return_message += "Manual punch is already declined" + Environment.NewLine;
              
            }
            
            
            return_object.return_data = return_message;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "UPDATE_MANUAL_PUNCH_APPROVAL_STATUS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while updating Manual Punch Approval Status. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    protected void ApproveMisingpunch(long Missingpunchid, int status_flag)
    {
        manual_approve page_object = new manual_approve();
        DBConnection db_connection = new DBConnection();
        Hashtable hsh = new Hashtable();
        DataTable manual_punch_data = new DataTable();
        string query = string.Empty;
        bool isHrflagApproved = false,isHrflagDeclined = false;
        if (status_flag==0)
        {
            isHrflagDeclined = true;
            status_flag = 1;
        }
        if (status_flag == 4)
        {
            isHrflagApproved = true;
            status_flag = 2;
        }
        try
        {
            string employee_name = HttpContext.Current.Session["employee_name"].ToString();
            if (string.IsNullOrEmpty(employee_name))
            {
                employee_name = "Admin";
            }

            hsh.Add("piID", Missingpunchid);
            hsh.Add("piApprove", status_flag);
            hsh.Add("piApprovedBy", employee_name);
            hsh.Add("poRetVal", 0);
            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("SpApprovePunch1", hsh);

            //get the manul punch details by ID and save it in MailSender table
            query = "select EmpCode,WorkDate,convert(varchar(5),InPunch,8) as InPunch,convert(varchar(5),OutPunch,108) as OutPunch,convert(varchar(5),BreakOut,108) as BreakOut,convert(varchar(5),BreakIn,108) as BreakIn, reasonformanualpunch from PunchForApproval where ID=" + Missingpunchid;
            manual_punch_data = db_connection.ReturnDataTable(query);
            if (status_flag == 1 && isHrflagDeclined==true)
            {
                
                status_flag = 0;
            }
            if (status_flag == 2 && isHrflagApproved==true)
            {
                status_flag = 5;
            }
            SendMail(manual_punch_data.Rows[0]["EmpCode"].ToString(), manual_punch_data.Rows[0]["WorkDate"].ToString(), manual_punch_data.Rows[0]["InPunch"].ToString(), manual_punch_data.Rows[0]["OutPunch"].ToString(), manual_punch_data.Rows[0]["reasonformanualpunch"].ToString(), status_flag);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "APPROVE_MISSING_PUNCH");
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
            employee_name = string.Empty, manager_name = string.Empty,hrName=string.Empty,hrEmilId=string.Empty,toEmployeeName=string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;

        try
        {

            switch (status_flag)
            {
                case 0:
                    manual_punch_status = "Declined By Hr and request forwarded to Manager ";
                    break;
                case 1:
                    manual_punch_status = "Approved And request sent to HR ";
                    break;
                case 2:
                    manual_punch_status = "Approved";
                    break;

                case 3:
                    manual_punch_status = "Declined";
                    break;

                case 4:
                    manual_punch_status = "Cancelled";
                    break;
                case 5:
                    manual_punch_status = "Approved By HR";
                    break;
            }




            //query = "select BM.ManagerID, EM.Emp_Email, EM.Emp_Name as Emp_name From TbManagerHrBranchMapping BM join EmployeeMaster EM on BM.ManagerID=em.Emp_Code where BM.BranchCode='" + branch_code + "'";
            //dt_temp = db_connection.ReturnDataTable(query);

            //mailTo = dt_temp.Rows[0]["Emp_Email"].ToString();
            //manager_name = dt_temp.Rows[0]["Emp_name"].ToString();

            //leave_send_mail.Add("ToEmailID", mailTo);
            //leave_send_mail.Add("CCEmailID", mailCC);


           
            //
            //
         //   DataTable employeeData = new DataTable();
            query = "select Emp_name,Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
            employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);

            // Getting the Employee email ID
            query = "select Emp_Email from EmployeeMaster where Emp_Code='" + employee_id + "'";
            mailTo = db_connection.ExecuteQuery_WithReturnValueString(query);

            // Getting Manager Email ID
            query = "select Emp_Email from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
            mailCC = db_connection.ExecuteQuery_WithReturnValueString(query);

            //Getting manager name
            query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
            manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

            //getiing Hr details
               //query="select emp_email from EmployeeMaster where emp_code=( select AlternativeHrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
               //hrEmilId = db_connection.ExecuteQuery_WithReturnValueString(query);
               //if (hrEmilId == "" || String.IsNullOrEmpty(hrEmilId))
               // {
                    
               //     query = "select emp_email from EmployeeMaster where emp_code=( select HrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
               //     hrEmilId = db_connection.ExecuteQuery_WithReturnValueString(query);
               //     query = "select emp_name from EmployeeMaster where emp_code=( select HrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
               //     hrName = db_connection.ExecuteQuery_WithReturnValueString(query);

               // }else
               // {
               //      query = "select emp_name from EmployeeMaster where emp_code=( select AlternativeHrIncharge from branchmaster where branchcode=(select emp_branch from EmployeeMaster where Emp_Code='" + employee_id + "'))";
               //      hrName = db_connection.ExecuteQuery_WithReturnValueString(query);
               // }

            //gettng Hr details from TbManagerHrBranchMapping

            query = "select Emp_Email,Emp_Branch from EmployeeMaster where Emp_Code='" + employee_id + "' and Emp_Status=1";
            dt_temp = db_connection.ReturnDataTable(query);

            
             string branch_code = dt_temp.Rows[0]["Emp_Branch"].ToString();
               query = "select BM.ManagerID, EM.Emp_Email, EM.Emp_Name as Emp_name From TbManagerHrBranchMapping BM join EmployeeMaster EM on BM.ManagerID=em.Emp_Code where BM.BranchCode='" + branch_code + "'";
               dt_temp = db_connection.ReturnDataTable(query);

               hrEmilId = dt_temp.Rows[0]["Emp_Email"].ToString();
               hrName = dt_temp.Rows[0]["Emp_name"].ToString();






                toEmployeeName = employee_name;

            //sending mail to hr and cc as manager  
            if (status_flag==1)
            {
                mailTo = hrEmilId;
                toEmployeeName = hrName;
                
            }
            //sending mail to Manager and cc as Hr  
            if (status_flag == 0)
            {
                mailTo = mailCC;
                toEmployeeName = manager_name;
                mailCC = hrEmilId;
               // manager_name = hrName;
            }
            if (status_flag==5)
            {
                mailCC = hrEmilId;
            }

            if (!string.IsNullOrEmpty(mailTo))
            {
                MailSubject = "Regarding Manual Entry Application";
                MailBody = MailBody + "Dear " + toEmployeeName + ", <br/><br/>";
                //if request Declined By Hr and request forwarded
                if (status_flag == 0)
                {
                    MailBody = MailBody + "The following Manual punch has been " + manual_punch_status + " <br/><br/>";
                }
                //if request Approved  By Hr and Manager 
                else if (status_flag == 5)
                {
                    MailBody = MailBody + "The following Manual punch has been " + manual_punch_status + " <br/><br/>";
                }
                //if request Approved  By  Manager or Declined By Manager 
                else
                {
                    MailBody = MailBody + "The following Manual punch has been " + manual_punch_status + " by " + manager_name + ". <br/><br/>";
                }
                
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


}
