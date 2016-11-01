using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Services;
using System.Data.SqlClient;
using System.Reflection;
using System.DirectoryServices;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.Generic;
using SecurAX.Logger;

public partial class application_authentication : System.Web.UI.Page
{
    private static readonly byte[] IV =
     new byte[8] { 241, 3, 45, 29, 0, 76, 173, 59 };

    protected void Page_Load(object sender, EventArgs e)
    {
        // Nothing to load or check in this function.
        // Function is there to maintain consistency with the rest of the Code Behind files.
    }

    private static string Encrypt(string s)
    {
        string cryptoKey = "$3Cu8^X";
        if (s == null || s.Length == 0) return string.Empty;

        string result = string.Empty;

        try
        {
            byte[] buffer = Encoding.ASCII.GetBytes(s);

            TripleDESCryptoServiceProvider des =
                new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider MD5 =
                new MD5CryptoServiceProvider();

            des.Key =
                MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

            des.IV = IV;
            result = Convert.ToBase64String(
                des.CreateEncryptor().TransformFinalBlock(
                    buffer, 0, buffer.Length));
        }
        catch
        {
            throw;
        }

        return result;
    }

    private static string Decrypt(string s)
    {

        string cryptoKey = "$3Cu8^X";
        if (s == null || s.Length == 0) return string.Empty;

        string result = string.Empty;

        try
        {
            byte[] buffer = Convert.FromBase64String(s);

            TripleDESCryptoServiceProvider des =
                new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider MD5 =
                new MD5CryptoServiceProvider();

            des.Key =
                MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

            des.IV = IV;

            result = Encoding.ASCII.GetString(
                des.CreateDecryptor().TransformFinalBlock(
                buffer, 0, buffer.Length));
        }
        catch
        {
            throw;
        }
        return result;
    }

    [WebMethod]
    public static ReturnObject checkLicenseValidity()
    {
        // Declaring and initializing variables to be used in this function.
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable license_info = new DataTable();
        string[] customer_details = new string[10];
        string encrypted_data = string.Empty;
        string decrypted_data = string.Empty;
        string user_name = string.Empty;
        string query = string.Empty;
        int user_access_level = 0;
        int license_status = 0;

        try
        {
            // storing session details in local variables.
            user_name = HttpContext.Current.Session["username"].ToString();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

            // checking if an entry for the license is present in the DB.
            query = "select count(details) from licenseinfo";
            license_status = db_connection.GetRecordCount(query);

            if (license_status == 0)
            {

                return_object.status = "error";

                if (user_access_level == 0)
                {
                    return_object.return_data = "Please purchase a license to use SecurTime";
                }
                else
                {
                    return_object.return_data = "Please request your administrator to purchase a license";
                }
            }
            else
            {

                query = "select details,lstatus from licenseinfo";
                license_info = db_connection.ReturnDataTable(query);

                if (license_info.Rows.Count > 0)
                {

                    foreach (DataRow dr in license_info.Rows)
                    {
                        encrypted_data = dr["details"].ToString();
                        license_status = Convert.ToInt32(dr["lstatus"].ToString());
                    }

                    decrypted_data = Decrypt(encrypted_data);
                    customer_details = decrypted_data.Split(';');
                }

                DateTime license_expiry_date = new DateTime();
                if (license_status == 1)
                {
                    //license_expiry_date = Convert.ToDateTime(customer_details[1].ToString()).Date;
                    license_expiry_date = DateTime.ParseExact(customer_details[1].ToString(), "MM-dd-yyyy HH:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).Date;
                }
                else if (license_status == 2)
                {
                    //license_expiry_date = Convert.ToDateTime(customer_details[2].ToString()).Date;
                    license_expiry_date = DateTime.ParseExact(customer_details[2].ToString(), "MM-dd-yyyy HH:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).Date;
                }

                if (license_expiry_date < DateTime.Now.Date)
                {
                    return_object.status = "error";
                    return_object.return_data = "Trial period has elapsed. Please register your license for continued usage.";
                }
                else
                {

                    if (user_access_level == 0)
                    {
                        query = "update login set lastlogin=getdate() where username='admin'";
                    }
                    else
                    {
                        query = "update login set lastlogin=getdate() where username='" + user_name + "'";
                    }

                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    return_object.status = "success";
                    return_object.return_data = "Success!";
                }
            }

        }
        catch (Exception ex)
        {

            Logger.LogException(ex, "AUTHENTICATION", "CHECK_LICENSE_VALIDITY");

            return_object.status = "error";
            return_object.return_data = "An error occurred while checking License Validity. Please refresh the page and try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject loadSessionData()
    {

        DBConnection db_connection = new DBConnection();
        DataTable user_details = new DataTable();
        Dictionary<string, string> admin_details = new Dictionary<string, string>();
        ReturnObject return_object = new ReturnObject();
        string file_path = string.Empty;
        string user_name = string.Empty;
        string employee_id = string.Empty;
        string employee_photo = string.Empty;
        string company_logo = string.Empty;
        string query = string.Empty;
        string company_code = string.Empty;
         
        byte[] image;

        try
        {
            user_name = HttpContext.Current.Session["username"].ToString();
           
            if (user_name == "admin")
            {
                // Setting the session values for Display Picture and Display Name. This will be used in the top menu.
                HttpContext.Current.Session["display_picture"] = "../../resources/images/default-user-icon.png";
                HttpContext.Current.Session["display_name"] = "admin";
                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                if (File.Exists(HttpContext.Current.Server.MapPath("~/uploads/CompanyLogo/" + "file_name" + ".png")))
                {

                    company_logo = baseUrl + "uploads/CompanyLogo/" + "file_name" + ".png";

                }
                else
                {

                    company_logo = baseUrl + "resources/images/logo100.png";
                }
                HttpContext.Current.Session["company_logo"] = company_logo;
                
                // Dictionary with admin details to be returned to the UI. This will be stored as session data in JS.
                admin_details.Add("username", "admin");
                admin_details.Add("employee_id", "admin");
                admin_details.Add("access_level", "0");
                admin_details.Add("emp_photo", "../../resources/images/default-user-icon.png");

                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(admin_details, Formatting.Indented);
            }
            else
            {
                employee_id = HttpContext.Current.Session["employee_id"].ToString();


                query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);


                query = "select l.username, l.empid, l.access, e.emp_name, e.emp_email, e.emp_employee_category, e.emp_department, de.deptname, e.emp_designation, des.designame from employeemaster e left join login l on e.emp_code = l.empid left join desigmaster des on e.emp_designation = des.desigcode left join deptmaster de on e.emp_department = de.deptcode where  e.emp_code = '" + employee_id + "' ";

                user_details = db_connection.ReturnDataTable(query);

                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";

                if (File.Exists(HttpContext.Current.Server.MapPath("~/uploads/profiles/" + employee_id + ".png")))
                {

                    employee_photo = baseUrl + "uploads/profiles/" + employee_id + ".png";

                }
                else {

                    employee_photo = baseUrl + "resources/images/default-user-icon.png";
                }

                // checking company logo

                if (File.Exists(HttpContext.Current.Server.MapPath("~/uploads/CompanyLogo/" + company_code + ".png")))
                {

                    company_logo = baseUrl + "uploads/CompanyLogo/" + company_code + ".png";

                }
                else
                {

                    company_logo = baseUrl + "resources/images/logo100.png";
                }

                user_details.Columns.Add("emp_photo");
                user_details.Rows[0]["emp_photo"] = employee_photo;

                // Setting the session values for Display Picture and Display Name. This will be used in the top menu.
                HttpContext.Current.Session["display_picture"] = employee_photo;
                //stroring company logo in session
                HttpContext.Current.Session["company_logo"] = company_logo;
                HttpContext.Current.Session["display_name"] = user_details.Rows[0]["emp_name"].ToString();

                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(user_details, Formatting.Indented);
            }
        }
        catch (Exception ex)
        {
            return_object.status = "error";
            return_object.return_data = "An error occurred while loading User details. Please refresh the page and try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject loadMenuStructure()
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable menu_permissions = new DataTable();
        string query = string.Empty;
        string user = string.Empty;

        try
        {
            user = HttpContext.Current.Session["employee_id"].ToString();

            query = "select PERMISSIONS from EMPLOYEE_PERMISSIONS where EMPLOYEE_CODE ='" + user + "'";

            menu_permissions = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(menu_permissions, Formatting.Indented);
        }
        catch (Exception ex)
        {
            return_object.status = "error";
            return_object.return_data = "An error occurred while loading the Menu. Please refresh the page and try again. If the error persists, please contact Support.";
        }

        return return_object;
    }
}
