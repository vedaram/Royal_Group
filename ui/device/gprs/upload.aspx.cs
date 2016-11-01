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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class template_upload : System.Web.UI.Page
{
    const string page = "TEMPLATE_UPLOAD";

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

            message = "An error occurred while loading Template Upload page. Please try again. If the error persists, please contact Support.";

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

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable company_data = new DataTable();
        string query = string.Empty, employee_id = string.Empty, company_code = string.Empty;

        try
        {
            employee_id = HttpContext.Current.Session["username"].ToString();

            //load company list as per employee
            if (employee_id != "admin")
            {
                query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster where CompanyCode='" + company_code + "' order by CompanyName ";
            }
            else
            {
                query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster";
            }
            company_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(company_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_COMPANY_DATA");

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataSet return_data = new DataSet();
        DataTable temp_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DISTINCT BranchCode as branch_code, BranchName as branch_name from BranchMaster where CompanyCode = '" + company_code + "' order by BranchName";
            temp_data = db_connection.ReturnDataTable(query);
            temp_data.TableName = "branch";
            return_data.Tables.Add(temp_data);

            query = "Select DeptCode as department_code, DeptName as department_name from DeptMaster where CompanyCode = '" + company_code + "' order by DeptName";
            temp_data = db_connection.ReturnDataTable(query);
            temp_data.TableName = "department";
            return_data.Tables.Add(temp_data);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OTHER_DATA");

            throw;
        }

        return return_object;
    }

    private string GetBaseQuery()
    {
        string query = "select distinct(s.EnrollID) as enroll_id, e.emp_name as employee_name from SAXPushTemplate s, EmployeeMaster e where s.EnrollId = e.Emp_Card_No";

        return query;
    }

    private string GetFilterQuery(string query, string filters)
    {
        JObject filter_data = JObject.Parse(filters);
        string company_code = filter_data["filter_company"].ToString();
        string branch_code = filter_data["filter_branch"].ToString();
        string department_code = filter_data["filter_department"].ToString();
        int filter_by = Convert.ToInt32(filter_data["filter_by"]);
        string filter_keyword = filter_data["filter_keyword"].ToString();

        if (company_code != "select")
            query += " and e.Emp_Company = '" + company_code + "' ";

        if (branch_code != "select")
            query += " and e.Emp_Branch = '" + branch_code + "' ";

        if (department_code != "select")
            query += " and e.Emp_Department = '" + department_code + "' ";

        if (filter_by > 0)
        {
            switch (filter_by)
            {
                case 1:
                    query += " and s.EnrollID = '" + filter_keyword + "' ";
                    break;
                case 2:
                    query += " and e.Emp_Code = '" + filter_keyword + "' ";
                    break;
                case 3:
                    query += " and e.Emp_Name like '%" + filter_keyword + "%' ";
                    break;
            }
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetEnrollmentData(int page_number, bool is_filter, string filters)
    {
        template_upload page_object = new template_upload();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable enrollment_data = new DataTable();
        string query = string.Empty;
        int start_row = (page_number - 1) * 30;

        try
        {
            query = page_object.GetBaseQuery();

            if (is_filter)
                query = page_object.GetFilterQuery(query, filters);

            query += " ORDER BY s.EnrollID OFFSET " + start_row + " ROWS FETCH NEXT 30 ROWS ONLY";

            enrollment_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(enrollment_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_ENROLLMENT_DATA");

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetDeviceData()
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable device_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DeviceLocation as device_location, DeviceId as device_id from LastDateSAXPush order by DeviceLocation";

            device_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(device_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_DEVICE_DATA");

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject SaveEnrollments(string enrollments, string devices)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        List<string> enrollment_list = JsonConvert.DeserializeObject<List<string>>(enrollments);
        List<string> device_list = JsonConvert.DeserializeObject<List<string>>(devices);
        string
            query = string.Empty,
            current_employee_id = string.Empty,
            enrollment_string = string.Empty;

        int
            i = 0, j = 0;

        try
        {
            current_employee_id = HttpContext.Current.Session["username"].ToString();

            // generate a comma seperated list of enrollment IDs
            for (j = 0; j < enrollment_list.Count; j++)
            {
                enrollment_string += "'" + enrollment_list[j] + "',";
            }

            // remove the last comma
            enrollment_string = enrollment_string.TrimEnd(',');

            for (i = 0; i < device_list.Count; i++)
            {

                query = " Insert into EnrollmentIDlist(DeviceID, EnrollID, Privilege, FPID,FPtemplate, Size, Name, Password, Card, Action, UserID, Status)";
                query += " select '" + device_list[i] + "', EnrollID, Privilege, FPID, FPTemplate, Size, Name, Password, Card, 'Upload','" + current_employee_id + "', NULL from SAXPushTemplate";
                query += "  where EnrollID in (" + enrollment_string + ")";

                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }

            return_object.status = "success";
            return_object.return_data = "Enrollments saved successfully!";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SAVE_ENROLLMENT");

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject RebootDevice(string devices)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        List<string> device_list = JsonConvert.DeserializeObject<List<string>>(devices);
        string query = string.Empty;

        int i = 0;

        try
        {
            for (i = 0; i < device_list.Count; i++)
            {
                query += "  Insert into DeviceIDList(DeviceID,Flag) values ('" + device_list[i] + "',0)   ";
                query += " Insert into EnrollmentIDList(DeviceID,Action,Status) values ('" + device_list[i] + "','Reboot',NULL)";
            }

            db_connection.ExecuteQuery_WithOutReturnValue(query);

            return_object.status = "success";
            return_object.return_data = "Devices have been queued for reboot. Please wait for a while and check";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "REBOOT_DEVICE");

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteEnrollments(string enrollments, string devices)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        List<string> enrollment_list = JsonConvert.DeserializeObject<List<string>>(enrollments);
        List<string> device_list = JsonConvert.DeserializeObject<List<string>>(devices);
        string query = string.Empty;
        string enrollment_string = string.Empty;

        int
            i = 0, j = 0;

        try
        {
            // generate a comma seperated list of enrollment IDs
            for (j = 0; j < enrollment_list.Count; j++)
            {
                enrollment_string += "'" + enrollment_list[j] + "',";
            }

            // remove the last comma
            enrollment_string = enrollment_string.TrimEnd(',');

            for (i = 0; i < device_list.Count; i++)
            {

                query = "   Insert into EnrollmentIDlist(EnrollID, DeviceID, Action, Status)";
                query += "  select distinct(EnrollID), '" + device_list[i] + "', 'Delete', NULL from SAXPushTemplate";
                query += "  where  EnrollID in (" + enrollment_string + ")";

                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }

            query = "insert into DeviceIdList(DeviceID) select Distinct(DeviceID) from EnrollmentIDList update DeviceIdList set Flag=0";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            return_object.status = "success";
            return_object.return_data = "Enrollments saved successfully!";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DELETE_ENROLLMENTS");

            throw;
        }

        return return_object;
    }
}