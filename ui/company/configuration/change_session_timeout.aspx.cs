﻿using System;
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
using System.Web.Configuration;
using System.Configuration;

public partial class configuration_change_session_timeout : System.Web.UI.Page
{

    const string page = "CHANGE_SESSION_TIMEOUT";

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

            Logger.LogException(ex, page, "PAGE_LOAD");

            message = "An error occurred while loading Shift Settings. Please try again. If the error persists, please contact Support.";

            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }

    }
    [WebMethod]
    public static ReturnObject GetSessionTimeout()
    {

        Configuration configuration;
        SessionStateSection section;
        string timeout = string.Empty;
        ReturnObject return_Object = new ReturnObject();

        try
        {

            configuration = WebConfigurationManager.OpenWebConfiguration("~");
            section = (SessionStateSection)configuration.GetSection("system.web/sessionState");
            timeout = section.Timeout.ToString();
            return_Object.status = "success";
            return_Object.return_data = timeout.ToString();



        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET SESSION TIMEOUT");

            return_Object.status = "error";
            return_Object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_Object;
    }
    [WebMethod]
    public static ReturnObject UpdateSessionTimeout()
    {
        ReturnObject return_Object = new ReturnObject();
        try
        {
            Configuration configuration;
            SessionStateSection section;
            configuration = WebConfigurationManager.OpenWebConfiguration("~");
            section = (SessionStateSection)configuration.GetSection("system.web/sessionState");
            if (section != null)
            {
                section.Timeout = TimeSpan.FromMinutes(40);
                configuration.Save();
                return_Object.status = "success";
                return_Object.return_data = "Session timeout saved successfully!";
            }
        }
        catch (Exception ex )
        {
            Logger.LogException(ex, page, "UPDATE SESSION TIMEOUT");

            return_Object.status = "error";
            return_Object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";

            throw;   
        }
        return return_Object;

    }

}
