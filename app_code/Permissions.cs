using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for Permissions
/// </summary>
public class Permissions
{
    public void setManager(string mode, string employee_code) {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        string permissions = "000000000000000000000111111101111111111100111111111111111111111111111";

        if (mode == "I")
        {
            query = "insert into EMPLOYEE_PERMISSIONS(EMPLOYEE_CODE, PERMISSIONS) values ('" + employee_code + "', '" + permissions + "')";
        }
        else
        {
            query = "update EMPLOYEE_PERMISSIONS set PERMISSIONS ='" + permissions + "' where EMPLOYEE_CODE ='" + employee_code + "' ";
        }

        db_connection.ExecuteQuery_WithOutReturnValue(query);
    }

    public void setHRManager(string mode, string employee_code) {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        string permissions = "011000000000111100100111111101111111111100111111111111111111111111111";

        if (mode == "I")
        {
            query = "insert into EMPLOYEE_PERMISSIONS(EMPLOYEE_CODE, PERMISSIONS) values ('" + employee_code + "', '" + permissions + "')";
        }
        else
        {
            query = "update EMPLOYEE_PERMISSIONS set PERMISSIONS ='" + permissions + "' where EMPLOYEE_CODE ='" + employee_code + "' ";
        }

        db_connection.ExecuteQuery_WithOutReturnValue(query);
    }

    public void setEmployee(string mode, string employee_code) {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        string permissions = "000000100000000000000110110101101101010000111111100000000000000000101";

        if (mode == "I")
        {
            query = "insert into EMPLOYEE_PERMISSIONS(EMPLOYEE_CODE, PERMISSIONS) values ('" + employee_code + "', '" + permissions + "')"; 
        }
        else
        {
            query = "update EMPLOYEE_PERMISSIONS set PERMISSIONS ='" + permissions + "' where EMPLOYEE_CODE ='" + employee_code + "' ";
        }

        db_connection.ExecuteQuery_WithOutReturnValue(query);
    }

    public void setAdmin(string mode, string employee_code) {

        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        string permissions = "011111111111111111100111111111111111111100111111111111111111111111111";

        if (mode == "I")
        {
            query = "insert into EMPLOYEE_PERMISSIONS(EMPLOYEE_CODE, PERMISSIONS) values ('" + employee_code + "', '" + permissions + "')";
        }
        else
        {
            query = "update EMPLOYEE_PERMISSIONS set PERMISSIONS ='" + permissions + "' where EMPLOYEE_CODE ='" + employee_code + "' ";
        }

        db_connection.ExecuteQuery_WithOutReturnValue(query);
    }
}
