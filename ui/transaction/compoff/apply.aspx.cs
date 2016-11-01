using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class compoff_apply : System.Web.UI.Page
{
    const string page = "COMPOFF_APPLICATION";

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

    private int CheckCompoff(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        try
        {
            query = "Select count(*) FROM compoffdetails WHERE EmpID='" + employee_id + "' AND ";
            query += "((CONVERT(datetime,fromdate,103) >= CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,fromdate,103) <=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,fromdate,103) <=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,todate,103) >=CONVERT(datetime,'" + Leave_to + "',103)) OR ";
            query += "(CONVERT(datetime,todate,103) >=CONVERT(datetime,'" + Leave_from + "',103) And CONVERT(datetime,todate,103) <=CONVERT(datetime,'" + Leave_to + "',103))) and flag not in (3,4) ";
            count = db_connection.GetRecordCount(query);
           

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_COMPOFF");
            throw ex;
        }
        return count;
    }

    private int CheckForValidCompoff(string employee_id, string Leave_from, string Leave_to)
    {

        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
        try
        {
            query = "select count(*) from masterprocessdailydata where EMP_ID='" + employee_id + "' AND workhrs is not null and SOT is not null ";
            query += "AND PDATE BETWEEN CONVERT(datetime,'" + Leave_from + "',103) AND CONVERT(datetime,'" + Leave_to + "',103) and";
            query += "(status='HP' or status='WOP' or status='M' or status='MI' or status='MO' or status='M' or status='WO') ";
            count = db_connection.GetRecordCount(query);
            
            //getting WO Dates between leave_from and leave_to
           
            if (count == 0)
            {
                //[sp_ChkOdOnWeekOff] @fromDate,@toDate,@empId,@OutputCount
                Hashtable HT_CompOff=new Hashtable();
                HT_CompOff.Add("fromDate",Leave_from);
                HT_CompOff.Add("toDate",Leave_to);
                HT_CompOff.Add("empId",employee_id);
                
                count = db_connection.ExecuteStoredProcedureReturnInteger_OneOutput("sp_ChkOdOnWeekOff", "@OutputCount", HT_CompOff);
            }           

             
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_COMPOFF");
            throw ex;
        }
        return count;
    }

    


    
    private int InsertData_Compoff(string employee_id, string FromDate, string ToDate, string Reason)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable Compoff_hshParam = new Hashtable();
        int recordAffected = 0;
        Compoff_hshParam.Add("piMode", "I");
        Compoff_hshParam.Add("piEmpCode", employee_id);
        Compoff_hshParam.Add("piFromDate", FromDate);
        Compoff_hshParam.Add("piToDate", ToDate);
        Compoff_hshParam.Add("piReason", Reason);
        Compoff_hshParam.Add("piflag", 1);
        recordAffected = db_connection.exeStoredProcedure_WithHashtable_ReturnRow("spUpsertcompoff", Compoff_hshParam);
        return recordAffected;
    }
    [WebMethod]
    public static ReturnObject SubmitCompff(string current)
    {
        compoff_apply PageObject = new compoff_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        string Employee_id = string.Empty;
        string FromDate = string.Empty;
        string ToDate = string.Empty;
        string Reason = string.Empty;
        int count = 0;
        int Compoffsubmitflag = 0;

        try
        {

            JObject json_data = JObject.Parse(current);
            Employee_id = json_data["employee_id"].ToString();
            FromDate = json_data["from_date"].ToString();
            ToDate = json_data["to_date"].ToString();
            Reason = json_data["reason"].ToString();
            count = PageObject.CheckCompoff(Employee_id, FromDate, ToDate);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Compoff already has been submitted for the selected dates.";
                return return_object;
            }
            count = PageObject.CheckForValidCompoff(Employee_id, FromDate, ToDate);
            if (count == 0)
            {
                return_object.status = "error";
                return_object.return_data = "Sorry! You are not eligible for CompOff on Selected Date Range.";
                return return_object;
            }
            Compoffsubmitflag = PageObject.InsertData_Compoff(Employee_id, FromDate, ToDate, Reason);
            if (Compoffsubmitflag > 0)
            {
                PageObject.SendMail(Employee_id, FromDate, ToDate, Reason);
                return_object.status = "success";
                return_object.return_data = "Compoff submitted successfully and email will sent to your manager.";
            }
            
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SUBMIT_COMPOFF");
            return_object.status = "error";
            return_object.return_data = "An error occurred while Submitting OD Leave Application. Please try again. If the error persists, please contact Support.";
        }

        finally
        {
            PageObject.Dispose();
        }
        return return_object;
    }

    private void SendMail(string employee_id, string FromDate, string ToDate, string Reason)
    {
        Hashtable Compoff_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;

        // Getting the Employee ID
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

        //Getting manager name
        query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailTo = email_id;

        if (!string.IsNullOrEmpty(mailTo))
        {
            MailSubject = "Compoff has been submitted by  " + employee_name + " for your approval";

            MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
            MailBody = MailBody + "The following Compoff has been Submitted <br/><br/>";
            MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
            MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
            MailBody = MailBody + "From Date: " + FromDate + "<br/><br/>";
            MailBody = MailBody + "To Date : " + ToDate + "<br/><br/>";
            MailBody = MailBody + "Reason:  " + Reason + "<br/><br/>";

            Compoff_send_mail.Add("EmpID", employee_id);
            Compoff_send_mail.Add("EmpName", employee_name);
            Compoff_send_mail.Add("ToEmailID", mailTo);
            Compoff_send_mail.Add("CCEmailID", mailCC);
            Compoff_send_mail.Add("Subject", MailSubject);
            Compoff_send_mail.Add("Body", MailBody);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", Compoff_send_mail);
        }
        else
        {
            //manager email doestn't exist
        }
    }   
}