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


public partial class overtime_apply : System.Web.UI.Page
{
    const string page = "OVERTIME_APPLICATION"; 

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

            message = "An error occurred while loading Overtime Application page. Please try again. If the error persists, please contact Support.";

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

    //TODO: Ondate change validation in javascript
    [WebMethod]
    public static ReturnObject CheckAvailableOT(string otdate, string employee_id) 
    {
        overtime_apply page_object= new overtime_apply();
        DBConnection db_connection = new DBConnection();
        DateTime Overtime_datetime = new DateTime();
        ReturnObject return_object = new ReturnObject();
        string othrs = string.Empty;
        string query = string.Empty;
        int count = 0;
        try
        {
            
            query = "select count(OT) from masterprocessdailydata where emp_id='" + employee_id + "' and pdate='" + otdate + "'";
            count = db_connection.ExecuteQuery_WithReturnValueInteger(query);
            if (count < 0)
            {
                return_object.status = "error";
                return_object.return_data = "Overtime doesn't exist for the selected date.";
                
            }
            else
            {
                query = "select OT from masterprocessdailydata where emp_id='" + employee_id + "' and pdate='" + otdate + "'";
                othrs = db_connection.ExecuteQuery_WithReturnValueString(query);
                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(othrs, Formatting.Indented);
            }
            

        }
        catch(Exception ex)
        {
            Logger.LogException(ex, page, "CHECK_AVAILABLE_OT");
            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Overtime Hrs. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
       
    }
   
    private void SendMail(string employee_id, string date, string overtime)
    {
        Hashtable OT_send_mail = new Hashtable();
        DBConnection db_connection = new DBConnection();

        string
            mailTo = string.Empty, mailCC = string.Empty,
            email_id = string.Empty, query = string.Empty,
            employee_name = string.Empty, manager_name = string.Empty,
            MailSubject = string.Empty, MailBody = string.Empty;

        // Getting the Employee ID
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
        query = "select Emp_name from EmployeeMaster where Emp_Code =(select Managerid from EmployeeMaster where Emp_Code='" + employee_id + "')";
        manager_name = db_connection.ExecuteQuery_WithReturnValueString(query);

        if (!string.IsNullOrEmpty(email_id))
            mailTo = email_id;

        if (!string.IsNullOrEmpty(mailTo))
        {

            MailSubject = "Overtime has been submitted by  " + employee_name + " for your approval";

            MailBody = MailBody + "Dear " + manager_name + ", <br/><br/>";
            MailBody = MailBody + "The following Overtime has been Submitted <br/><br/>";
            MailBody = MailBody + "Employee Id : " + employee_id + " <br/><br/>";
            MailBody = MailBody + "Employee Name : " + employee_name + " <br/><br/>";
            MailBody = MailBody + "Overtime Date: " + date + "<br/><br/>";
            MailBody = MailBody + "Overtime Hours : " + overtime + "<br/><br/>";

            OT_send_mail.Add("EmpID", employee_id);
            OT_send_mail.Add("EmpName", employee_name);
            OT_send_mail.Add("ToEmailID", mailTo);
            OT_send_mail.Add("CCEmailID", mailCC);
            OT_send_mail.Add("Subject", MailSubject);
            OT_send_mail.Add("Body", MailBody);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("InsertMailsDetails", OT_send_mail);
        }
    }
   
    private int CheckForOTRecord(string employee_id, string date)
    {
        DBConnection db_connection = new DBConnection();
        int count = 0;
        string query = string.Empty;
     
        try
        {
            query = "SELECT count(*)  FROM Overtime WHERE EmpID='" + employee_id + "' and otdate = '" + date + "' and Flag!=3 ";
            count = db_connection.GetRecordCount(query);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "CHECHK_FOR_OT_DATE");
            throw ex;
        }
        
        return count;
    }

    [WebMethod]
    public static ReturnObject SubmitOT(string jsonData)   
    {
        overtime_apply PageObject = new overtime_apply();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        string employee_id = string.Empty;
        string otdate = string.Empty;
        string othrs = string.Empty;
        string query = string.Empty;
        string employee_name = string.Empty;
        int otcount = 0;
        try
        {

            JObject json_data = JObject.Parse(jsonData);
            employee_id = json_data["employee_id"].ToString();
            otdate = json_data["date"].ToString();
            othrs = json_data["available_ot"].ToString();

            query = "select Emp_Name[Name] from employeeMaster where Emp_Code='" + employee_id + "'";
            employee_name = db_connection.ExecuteQuery_WithReturnValueString(query);
            otcount = PageObject.CheckForOTRecord(employee_id, otdate);
            if (otcount > 0)
            {
                return_object.status = "error";
                return_object.return_data = "OT Application already has been submitted for the selected dates.";
                return return_object;
            }
            else
            {
                query = "insert into Overtime(empid, OTDate, OTHrs, Flag,MFlag, EmpName) values('" + employee_id + "', '" + otdate + "', '" + othrs + "', 1,1,'" + employee_name + "')";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                PageObject.SendMail(employee_id, otdate, othrs);
                return_object.status = "success";
                return_object.return_data = "Overtime Application submitted successfully!";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SUBMIT_OT");
            return_object.status = "error";
            return_object.return_data = "An error occurred while submitting Overtime Application. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            PageObject.Dispose();
        }
        return return_object;
    }
}
