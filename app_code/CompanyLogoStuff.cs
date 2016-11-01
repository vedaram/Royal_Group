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
using System.IO;

/// <summary>
/// Summary description for CompanyLogoStuff
/// </summary>
public class CompanyLogoStuff
{
    public CompanyLogoStuff()
    {
    }

    public string getCompanyImageUrl(string companyName)
    {
        string companyCode = string.Empty;
        string company_logo = string.Empty;
        DBConnection db_connection = new DBConnection();
        string query = "select companycode from companymaster where companyname='" + companyName + "'";
        companyCode = db_connection.ExecuteQuery_WithReturnValueString(query);
        string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
        if (File.Exists(HttpContext.Current.Server.MapPath("~/uploads/CompanyLogo/" + companyCode + ".png")))
        {

            company_logo =companyCode + ".png";

        }
        else
        {

            company_logo ="logo100.png";
        }

        return company_logo;
    }
}
