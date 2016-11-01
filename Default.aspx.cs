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
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Globalization;

public partial class _Default : System.Web.UI.Page
{
    const string page = "LOGIN";

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public void loginButtonClick(object sender, EventArgs e)
    {
        DBConnection db_connection = new DBConnection();
        DataTable user_record = new DataTable();
        string query = string.Empty;
        int invalid_attempts_count = 0;

        try
        {
            query = "select e.emp_code, l.username, e.emp_name, l.access, e.emp_status from employeemaster e right outer join login l on e.emp_code = l.empid where l.username = '" + Request["username"] + "' and l.password = '" + Request["password"] + "' and l.status = 1";

            user_record = db_connection.ReturnDataTable(query);

            if (user_record.Rows.Count > 0)
            {
                    Session["username"] = user_record.Rows[0]["username"].ToString();
                    Session["employee_id"] = user_record.Rows[0]["emp_code"].ToString();
                    Session["employee_name"] = user_record.Rows[0]["emp_name"].ToString();
                    Session["access_level"] = user_record.Rows[0]["access"].ToString();
                    Session["version"] = "2.5.5.0";

                    Response.Redirect("~/authentication/authentication.aspx", false);
            }
            else
            {
                invalid_attempts_count++;
                errorMessage.Text = "Please check your username or password.";

                return;
            }

        }
        catch (Exception ex)
        {
            
        }
        finally
        { 
        }
    }
}