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
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class lan_enroll : System.Web.UI.Page
{
	const string page = "ENROLL_CARD";

    protected void Page_Load(object sender, EventArgs e)
    {
        try {

            if (Session["username"] == null)
            {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex) {

            string message = "An error occurred while performing this operation. Please try again. If the issue persists, please contact Support.";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("alert('");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    [WebMethod]
    public static ReturnObject getEnrolledCards(int page_number) {

        int start_row              = (page_number - 1) * 30;
        int number_of_record       = (page_number * 30) + 1;
        DBConnection db_connection = new DBConnection();
        DataTable enrollment_data  = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query               = "";

        try {

            query = "select Enrollid, cardid, pin, Empid, Name, row from ( select Enrollid, cardid, pin, Empid, Name, ROW_NUMBER() OVER (ORDER BY Cast(enrollid as int)) as row from enrollmaster) a where row > " + start_row + " and row < " + number_of_record;

            enrollment_data = db_connection.ReturnDataTable(query);

            return_object.status      = "success";
            return_object.return_data = JsonConvert.SerializeObject(enrollment_data, Formatting.Indented);
        }
        catch (Exception ex) {
        	
        	Logger.LogException(ex, page, "GET_ENROLLED_CARDS");

            return_object.status      = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject getEmployeeDetails(string enroll_id) {

        DBConnection db_connection = new DBConnection();
        DataTable employee_details = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string query               = "";

        try {

            query = "select Emp_Code, Emp_Name from EmployeeMaster where Emp_Card_No = '" + enroll_id + "' ";

            employee_details = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employee_details, Formatting.Indented);
        }
        catch (Exception ex) {

        	Logger.LogException(ex, page, "GET_EMPLOYEE_DETAILS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error perists, please contact Support";
        }

        return return_object;
    }


    public static string Reverse(string str)
    {
        int i              = 0;
        string firstdigit  = string.Empty;
        string seconddigit = string.Empty;
        string thirddigit  = string.Empty;
        string fourthdigit = string.Empty;
        string finaldigit  = string.Empty;

        int length = 0;

        length = str.Length;
        
        if (length % 2 == 0)
        {
            for (i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    firstdigit = str.Substring(i, 2);
                    i = i + 1;
                }
                else if (i == 2)
                {
                    seconddigit = str.Substring(i, 2);
                    i = i + 1;
                }
                else if (i == 4)
                {
                    thirddigit = str.Substring(i, 2);
                    i = i + 1;
                }
                else if (i == 6)
                {
                    fourthdigit = str.Substring(i, 2);
                    i = i + 1;
                }   
            }       
        }
        if (i >= length)
        {
            finaldigit = fourthdigit + thirddigit + seconddigit + firstdigit;
        }

        return finaldigit;
    }

    private void updateDatabase(string mode, string enrollment_id, long card_id, string pin_number, string employee_id, string employee_name, int input_mifare)
    {

        string hex, reverse;
        DBConnection db_connection = new DBConnection();

        if (input_mifare == 1)
        {
            hex = card_id.ToString("X");
            reverse = Reverse(hex);
            
            if (!string.IsNullOrEmpty(reverse))
            {
                card_id = int.Parse(reverse, System.Globalization.NumberStyles.HexNumber);
            }
        }

        if (card_id < -1)
        {
            card_id = card_id + 4294967296;
        }
        
        Hashtable enrollment_data = new Hashtable();

        enrollment_data.Add("Mode", mode);
        enrollment_data.Add("enrollid", enrollment_id);
        enrollment_data.Add("cardid", card_id);
        enrollment_data.Add("pin",pin_number);
        enrollment_data.Add("empid", employee_id);
        enrollment_data.Add("empname", employee_name);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateEnrollement", enrollment_data);

    }

    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }

    [WebMethod]
    public static ReturnObject addEnrollment(string current) {

        lan_enroll page_object = new lan_enroll();
        DBConnection db_connection              = new DBConnection();
        ReturnObject return_object              = new ReturnObject();
        string enroll_id                        = string.Empty;
        string pin_number                       = string.Empty;
        string employee_id                      = string.Empty; 
        string employee_name                    = string.Empty;
        string query                            = string.Empty;
        int count                               = 0;
        int input_mifare                        = 0;
        long card_id                            = 0;

        if (HttpContext.Current.Session["username"] == null)
        {
            // HttpContext.Current.Response.Redirect("~/logout.aspx", true);
            return_object = page_object.DoLogout();
        }
        else
        {

            try
            {

                JObject current_data = JObject.Parse(current);
                enroll_id = current_data["Enrollid"].ToString();
                card_id = Convert.ToInt64(current_data["cardid"]);
                pin_number = current_data["pin"].ToString();
                employee_id = current_data["Empid"].ToString();
                employee_name = current_data["Name"].ToString();
                input_mifare = Convert.ToInt32(current_data["input_mifare"]);

                query = "select count(*) from Enrollmaster where Enrollid = '" + enroll_id + "'";
                count = db_connection.GetRecordCount(query);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Enrollment ID already exists. Please try again with a different Enrollment ID";
                }
                else if (db_connection.GetRecordCount("select count(*) from Enrollmaster where Cardid = '" + card_id + "' ") > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Card ID has been taken. Please try again with a different Card ID";
                }
                else
                {

                    page_object.updateDatabase("I", enroll_id, card_id, pin_number, employee_id, employee_name, input_mifare);

                    return_object.status = "success";
                    return_object.return_data = "Enrollment has been completed successfully!";
                }

            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "ADD_ENROLLMENT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support";
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject editEnrollment(string current) {

        lan_enroll page_object = new lan_enroll();
        DBConnection db_connection              = new DBConnection();
        ReturnObject return_object              = new ReturnObject();
        string enroll_id                        = string.Empty;
        string pin_number                       = string.Empty;
        string employee_id                      = string.Empty;
        string employee_name                    = string.Empty;
        long card_id                            = 0;
        int input_mifare                        = 0;

        if (HttpContext.Current.Session["username"] == null)
        {
            // HttpContext.Current.Response.Redirect("~/logout.aspx", true);
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                JObject current_data = JObject.Parse(current);
                enroll_id = current_data["Enrollid"].ToString();
                card_id = Convert.ToInt64(current_data["cardid"]);
                pin_number = current_data["pin"].ToString();
                employee_id = current_data["Empid"].ToString();
                employee_name = current_data["Name"].ToString();
                input_mifare = Convert.ToInt32(current_data["input_mifare"]);

                if (db_connection.GetRecordCount("select count(*) from Enrollmaster where Enrollid = '" + enroll_id + "' and Empid != '" + employee_id + "' ") > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Enrollment ID already exists. Please try again with a different Enrollment ID";
                }
                else if (db_connection.GetRecordCount("select count(*) from Enrollmaster where Cardid = '" + card_id + "' and Empid != '" + employee_id + "' ") > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Card ID has been taken. Please try again with a different Card ID";
                }
                else
                {

                    page_object.updateDatabase("U", enroll_id, card_id, pin_number, employee_id, employee_name, input_mifare);

                    return_object.status = "success";
                    return_object.return_data = "Enrollment details edited successfully!";
                }
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "EDIT_ENROLLMENT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject deleteEnrollment(string current) {

        lan_enroll page_object = new lan_enroll();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable enrollment_data  = new Hashtable();
        string enroll_id           = string.Empty;

        if (HttpContext.Current.Session["username"] == null)
        {
            // HttpContext.Current.Response.Redirect("~/logout.aspx", true);
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                JObject current_data = JObject.Parse(current);
                enroll_id = current_data["Enrollid"].ToString();

                enrollment_data.Add("Mode", "D");
                enrollment_data.Add("enrollid", enroll_id);

                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateEnrollement", enrollment_data);

                return_object.status = "success";
                return_object.return_data = "Enrollment deleted successfully!";
            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "DELETE_ENROLLMENT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
            }
        }

        return return_object;
    }
}