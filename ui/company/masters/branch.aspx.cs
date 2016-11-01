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
using System.Globalization;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Export.Excel;
using SecurAX.Logger;
using System.Diagnostics;

public partial class masters_branch : System.Web.UI.Page
{
    const string page = "BRANCH_MASTER";

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

            message = "An error occurred while loading Branch Master page. Please try again. If the error persists, please contact Support.";

            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    private string GetBaseQuery()
    {
        DBConnection db_connection = new DBConnection();
        string employee_id, query, company_code = string.Empty;
        employee_id = HttpContext.Current.Session["employee_id"].ToString();
        // if employee is logged in then showing only that employee company  data (  done for royal group client first then implemnted in standard as well )
        if (employee_id != "")
        {
            query = "select emp_company from employeemaster where emp_code='" + employee_id + "'";
            company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
            //  query = "select BranchCode as branch_code, BranchName as branch_name, CompanyCode as company_code, CompanyName as company_name, BranchAddress as branch_address, PhoneNo as phone_number, FaxNo as fax_number, Email as email_address FROM ( select b.BranchCode, b.BranchName, c.CompanyCode, c.CompanyName, b.BranchAddress, b.PhoneNo, b.FaxNo, b.Email, ROW_NUMBER() OVER (ORDER BY b.BranchName) as row from branchmaster b, companymaster c, holidaygroup h where b.CompanyCode = c.CompanyCode and c.CompanyCode = h.CompanyCode and b.HolidayCode = h.holgrpcode  and  c.CompanyCode='" + company_code + "' ";
            query = "select BranchCode as branch_code, BranchName as branch_name, CompanyCode as company_code, CompanyName as company_name, BranchAddress as branch_address, PhoneNo as phone_number, FaxNo as fax_number, Email as email_address,HrIncharge,AlternativeHrIncharge FROM ( select b.BranchCode, b.BranchName, c.CompanyCode, c.CompanyName, b.BranchAddress, b.PhoneNo, b.FaxNo, b.Email,b.HrIncharge,b.AlternativeHrIncharge, ROW_NUMBER() OVER (ORDER BY b.BranchName) as row from branchmaster b, companymaster c, holidaygroup h where b.CompanyCode = c.CompanyCode and c.CompanyCode = h.CompanyCode and b.HolidayCode = h.holgrpcode  and  c.CompanyCode='" + company_code + "' ";
        }
        else
        {
            // query = "select BranchCode as branch_code, BranchName as branch_name, CompanyCode as company_code, CompanyName as company_name, BranchAddress as branch_address, PhoneNo as phone_number, FaxNo as fax_number, Email as email_address FROM ( select b.BranchCode, b.BranchName, c.CompanyCode, c.CompanyName, b.BranchAddress, b.PhoneNo, b.FaxNo, b.Email, ROW_NUMBER() OVER (ORDER BY b.BranchName) as row from branchmaster b, companymaster c, holidaygroup h where b.CompanyCode = c.CompanyCode and c.CompanyCode = h.CompanyCode and b.HolidayCode = h.holgrpcode ";
            query = "select BranchCode as branch_code, BranchName as branch_name, CompanyCode as company_code, CompanyName as company_name, BranchAddress as branch_address, PhoneNo as phone_number, FaxNo as fax_number, Email as email_address,HrIncharge,AlternativeHrIncharge FROM ( select b.BranchCode, b.BranchName, c.CompanyCode, c.CompanyName, b.BranchAddress, b.PhoneNo, b.FaxNo, b.Email,b.HrIncharge,b.AlternativeHrIncharge, ROW_NUMBER() OVER (ORDER BY b.BranchName) as row from branchmaster b, companymaster c, holidaygroup h where b.CompanyCode = c.CompanyCode and c.CompanyCode = h.CompanyCode and b.HolidayCode = h.holgrpcode ";
        }

        return query;
    }

    protected string GetFilterQuery(string filters, string query)
    {

        JObject filters_data = JObject.Parse(filters);
        string company_code = filters_data["filter_company_code"].ToString();
        string keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        if (company_code != "select")
        {
            query += " and c.companycode='" + company_code + "'";
        }

        switch (filter_by)
        {

            case 1:
                query += " and h.holgrpname like '%" + keyword + "%'";
                break;

            case 2:
                query += " and b.branchcode='" + keyword + "'";
                break;

            case 3:
                query += " and b.branchname like '%" + keyword + "%'";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetBranchData(int page_number, bool is_filter, string filters)
    {
        masters_branch page_object = new masters_branch();
        DBConnection db_connection = new DBConnection();
        DataTable branch_data_table = new DataTable();
        ReturnObject return_object = new ReturnObject();
        int start_row = (page_number - 1) * 30;
        int number_of_record = page_number * 30 + 1;
        string query = string.Empty;

        try
        {

            query = page_object.GetBaseQuery();

            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            query += " Group by b.BranchCode, b.BranchName, c.CompanyCode, c.CompanyName, b.BranchAddress, b.PhoneNo, b.FaxNo, b.Email,b.HrIncharge,b.AlternativeHrIncharge) a where row > " + start_row + " and row < " + number_of_record;

            branch_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(branch_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_BRANCH_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Branch Data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetHolidayGroupData(string company_code, string branch_code)
    {
        DataTable holiday_groups = new DataTable();
        DataSet return_data = new DataSet();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;

        try
        {
            query = "select holgrpcode as holiday_group_code, holgrpname as holiday_group_name  from HolidayGroup where companycode='" + company_code + "' order by holgrpname";
            holiday_groups = db_connection.ReturnDataTable(query);
            holiday_groups.TableName = "all_holiday_groups";
            return_data.Tables.Add(holiday_groups);

            if (branch_code != "")
            {
                query = "select holidaycode from BranchMaster where BranchCode = '" + branch_code + "' ";
                holiday_groups = db_connection.ReturnDataTable(query);
                holiday_groups.TableName = "selected_holiday_groups";
                return_data.Tables.Add(holiday_groups);
            }

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_HOLIDAY_GROUPS_FOR_COMPANY");

            return_object.status = " error";
            return_object.return_data = "An error occurred while loading Holiday Groups. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {

        DataTable company_data = new DataTable();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
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

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company Data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    public void UpdateDatabase(string branch_code, string branch_name, string company_code, string holiday_group_code, string address, string phone, string fax, string email, string hr_incharge, string alternative_hr_incharge, string mode)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable branch_data = new Hashtable();

        branch_data.Add("Mode", mode);
        branch_data.Add("BranchCode", branch_code);
        branch_data.Add("BranchName", branch_name);
        branch_data.Add("CompanyCode", company_code);
        branch_data.Add("HolidayCode", holiday_group_code);
        branch_data.Add("Address", address);
        branch_data.Add("Phone", phone);
        branch_data.Add("Fax", fax);
        branch_data.Add("Email", email);
        branch_data.Add("HrIncharge", hr_incharge);
        branch_data.Add("AlternativeHrincharge", alternative_hr_incharge);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateBranchMaster", branch_data);
    }

    private int CheckBranchName(string branch_name, string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from branchmaster where branchname = '" + branch_name + "' and companycode = '" + company_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    private int CheckBranchCode(string branch_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from branchmaster where branchcode = '" + branch_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }

    [WebMethod]
    public static ReturnObject AddBranch(string current, string holiday_groups)
    {

        masters_branch page_object = new masters_branch();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        JArray holiday_groups_data = new JArray();

        string
                branch_code = string.Empty, branch_name = string.Empty,
                company_code = string.Empty, holiday_group_code = string.Empty,
                address = string.Empty, phone = string.Empty, fax = string.Empty, email = string.Empty, hr_incharge = string.Empty, alternative_hr_incharge = string.Empty;

        int count = 0, i = 0;

        try
        {

            JObject current_data = JObject.Parse(current);
            branch_code = current_data["branch_code"].ToString();
            branch_name = current_data["branch_name"].ToString();
            company_code = current_data["company_code"].ToString();
            address = current_data["branch_address"].ToString();
            phone = current_data["phone_number"].ToString();
            fax = current_data["fax_number"].ToString();
            email = current_data["email_address"].ToString();
            hr_incharge = current_data["Hr_Incharge"].ToString();
            alternative_hr_incharge = current_data["Alternative_Hr_Incharge"].ToString();
            count = page_object.CheckBranchCode(branch_code);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Branch Code has been taken. Please try again with a different Code.";

                return return_object;
            }

            count = page_object.CheckBranchName(branch_name, company_code);
            if (count > 0)
            {
                return_object.status = "error";
                return_object.return_data = "Branch Name has been taken. Please try again with a different Name";

                return return_object;
            }

            holiday_groups_data = JArray.Parse(holiday_groups);
            for (i = 0; i < holiday_groups_data.Count; i++)
            {
                page_object.UpdateDatabase(branch_code, branch_name, company_code, holiday_groups_data[i].ToString(), address, phone, fax, email, hr_incharge, alternative_hr_incharge, "I");
            }

            return_object.status = "success";
            return_object.return_data = "Branch added successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "ADD_BRANCH");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving Branch details. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;

    }

    [WebMethod]
    public static ReturnObject EditBranch(string current, string previous, string holiday_groups)
    {

        masters_branch page_object = new masters_branch();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        JArray holiday_groups_data = new JArray();

        string
                original_branch_name = string.Empty, branch_code = string.Empty, branch_name = string.Empty,
                company_code = string.Empty, holiday_group_code = string.Empty,
                address = string.Empty, phone = string.Empty, fax = string.Empty, email = string.Empty, hr_incharge = string.Empty, alternative_hr_incharge = string.Empty;

        int count = 0, i = 0;

        try
        {

            JObject current_data = JObject.Parse(current);
            branch_code = current_data["branch_code"].ToString();
            branch_name = current_data["branch_name"].ToString();
            company_code = current_data["company_code"].ToString();
            address = current_data["branch_address"].ToString();
            phone = current_data["phone_number"].ToString();
            fax = current_data["fax_number"].ToString();
            email = current_data["email_address"].ToString();
            hr_incharge = current_data["Hr_Incharge"].ToString();
            alternative_hr_incharge = current_data["Alternative_Hr_Incharge"].ToString();


            JObject previous_data = JObject.Parse(previous);
            original_branch_name = previous_data["branch_name"].ToString();
            // Parse the array of holiday codes.
            holiday_groups_data = JArray.Parse(holiday_groups);

            if (original_branch_name != branch_name)
            {
                count = page_object.CheckBranchName(branch_name, company_code);
                if (count > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Branch Name has been taken. Please try again with a different Name.";

                    return return_object;
                }
                else
                {
                    for (i = 0; i < holiday_groups_data.Count; i++)
                    {
                        page_object.UpdateDatabase(branch_code, branch_name, company_code, holiday_groups_data[i].ToString(), address, phone, fax, email, hr_incharge, alternative_hr_incharge, "U");
                    }
                }
            }
            else
            {
                // delete the existing branch data from BranchMaster table.
                page_object.UpdateDatabase(branch_code, branch_name, company_code, "", address, phone, fax, email, hr_incharge, alternative_hr_incharge, "D");

                for (i = 0; i < holiday_groups_data.Count; i++)
                {
                    page_object.UpdateDatabase(branch_code, branch_name, company_code, holiday_groups_data[i].ToString(), address, phone, fax, email, hr_incharge, alternative_hr_incharge, "I");
                }
            }

            return_object.status = "success";
            return_object.return_data = "Branch edited successfully!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "EDIT_BRANCH");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving Branch details. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }


    [WebMethod]
    public static ReturnObject DeleteBranch(string current)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable branch_data = new Hashtable();
        string branch_code = string.Empty;
        string query = string.Empty;
        int employee_count = 0;

        try
        {

            JObject current_data = JObject.Parse(current);
            branch_code = current_data["branch_code"].ToString();

            query = "select count(emp_branch) from employeemaster where emp_branch = '" + branch_code + "' and emp_status = 1";
            employee_count = db_connection.GetRecordCount(query);

            if (employee_count == 0)
            {

                branch_data.Add("Mode", "D");
                branch_data.Add("branchcode", branch_code);

                db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("manipulatebranchmaster", branch_data);

                return_object.status = "success";
                return_object.return_data = "Branch deleted successfully!";
            }
            else
            {

                return_object.status = "error";
                return_object.return_data = "Employees have been mapped to this Branch. Please delete or reassign the Employees.";
            }
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "DELETE_BRANCH");

            return_object.status = "error";
            return_object.return_data = " An error occurred while deleting the Branch. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private string CreateExport(DataTable company_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Branch Code", "Branch Name", "Company Code", "Company Name", "Branch Address", "Phone No", "Fax No", "Email" };

        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "BranchMaster-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "BRANCH MASTER", company_data, Context, column_names);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport(string filters, bool is_filter)
    {
        masters_branch page_object = new masters_branch();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable branch_data = new DataTable();
        DateTime now = DateTime.Now;
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);

        string[] column_names = new string[] { };

        string
            query = string.Empty, file_name = string.Empty;

        try
        {
            query = page_object.GetBaseQuery();

            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            query += " Group by b.BranchCode, b.BranchName, c.CompanyCode, c.CompanyName, b.BranchAddress, b.PhoneNo, b.FaxNo, b.Email) a where row > 0 and row < " + export_limit;

            branch_data = db_connection.ReturnDataTable(query);

            if (branch_data.Rows.Count > 0)
            {

                file_name = page_object.CreateExport(branch_data);

                return_object.status = "success";
                return_object.return_data = file_name;
            }
            else
            {
                return_object.status = "info";
                return_object.return_data = "No data found with the selected filters. Please try again with different filters.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_DATA_FOR_EXPORT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while generating your report. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }


    [WebMethod]
    public static ReturnObject GenerateBranchCode(string company_code)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        
        string query = string.Empty;
        string old_branch_code = string.Empty;
        int new_branch_code = 0;
        //string company_code = string.Empty;

        try
        {
            //query = "select TOP 1 branchcode from  BranchMaster  ORDER BY  branchcode DESC";
            //old_branch_code = db_connection.ExecuteQuery_WithReturnValueString(query);

         


            //JObject current_data = JObject.Parse(current);
            //company_code = current_data["company_code"].ToString();

            //query = "select max(dbo.getFirstNumeric(branchcode)) from BranchMaster";
            //old_branch_code = db_connection.ExecuteQuery_WithReturnValueString(query);

            query = "select dbo.getbranchcode('"+company_code+"') ";
            old_branch_code = db_connection.ExecuteQuery_WithReturnValueString(query);
            

            //if (!string.IsNullOrEmpty(old_branch_code))
            //{
            //    new_branch_code = Convert.ToInt32(old_branch_code);
            //    new_branch_code++;                              
            //}
            //else
            //{
            //    new_branch_code = 1;
            //}

            return_object.status = "success";
            return_object.return_data = old_branch_code.ToString();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GENERATE_BRANCH_CODE");
            
            return_object.status = "error";
            return_object.return_data = "An error occurred while generating the branch code. Please try again.";

            throw;
        }


        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetHrInfo(string company_code, string branch_code)
    {

        DataSet return_data = new DataSet();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        DataTable hrinfo = new DataTable();

        try
        {

            query = "select Emp_name,Emp_Code from employeemaster where IsHr='1' and Emp_Company='" + company_code + "'";


            hrinfo = db_connection.ReturnDataTable(query);
            hrinfo.TableName = "HrIncharge";           
            return_data.Tables.Add(hrinfo);
           
            if (branch_code != "")
            {
                query = "select HrIncharge from BranchMaster where BranchCode = '" + branch_code + "' ";
                hrinfo = db_connection.ReturnDataTable(query);
                hrinfo.TableName = "selectedhr";
                return_data.Tables.Add(hrinfo);
            }
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_HRINFO");
            return_object.status = " error";
            return_object.return_data = "An error occurred while Loading HrInfo. Please try again. If the error persists, please contact Support.";
            throw;
        }
        return return_object;

    }



    [WebMethod]
    public static ReturnObject GetAlternativeHrInfo(string company_code, string branch_code)
    {

        DataSet return_data = new DataSet();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        DataTable alternativehrinfo = new DataTable();

        try
        {

            query = "select Emp_Code, Emp_name from employeemaster where IsHr='1' and Emp_Company='" + company_code + "'";


            alternativehrinfo = db_connection.ReturnDataTable(query);
            alternativehrinfo.TableName = "AlternativeHrIncharge";
            return_data.Tables.Add(alternativehrinfo);
            if (branch_code != "")
            {
                query = "select AlternativeHrIncharge from BranchMaster where BranchCode = '" + branch_code + "' ";
                alternativehrinfo = db_connection.ReturnDataTable(query);
                alternativehrinfo.TableName = "selectedAlthr";
                return_data.Tables.Add(alternativehrinfo);


            }

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_ALTERNATIVEHRINFO");
            return_object.status = " error";
            return_object.return_data = "An error occurred while Loading AlternativeHrInfo. Please try again. If the error persists, please contact Support.";
            throw;
        }
        return return_object;
    }
}



