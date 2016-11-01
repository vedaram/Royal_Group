﻿using System;
using System.Collections;
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
using System.Data.OleDb;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Logger;
using SecurAX.Import.Excel;
using SecurAX.Export.Excel;

public partial class masters_employee : System.Web.UI.Page
{
    const string page = "EMPLOYEE_MASTER";

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

            message = "An error occurred while loading Employee Master. Please try again. If the error persists, please contact Support.";

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

        string query = "select e.emp_code as employee_code, e.emp_name as employee_name, c.CompanyCode as company_code, c.CompanyName as company_name, b.branchcode as branch_code, b.branchname as branch_name, d.deptcode as department_code, d.deptname as department_name, de.desigcode as designation_code, de.designame as designation_name, e.emp_card_no as enroll_id, e.emp_status as employee_status , e.ot_eligibility as ot_eligibility ,  e.Ramadan_Eligibility as Ramadan_Eligibility  from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode ";

        return query;
    }

    private string GetFilterQuery(string filters, string query)
    {

        JObject filter_data = JObject.Parse(filters);
        string company_code = filter_data["filter_company"].ToString();
        string branch_code = filter_data["filter_branch"].ToString();
        string department_code = filter_data["filter_department"].ToString();
        string designation_code = filter_data["filter_designation"].ToString();
        string keyword = filter_data["filter_keyword"].ToString();
        int status = Convert.ToInt32(filter_data["filter_status"]);
        int filter_by = Convert.ToInt32(filter_data["filter_by"]);

        switch (status)
        {
            case 1:
                query += " where e.emp_status = 1 ";
                break;
            case 2:
                query += " where e.emp_status = 2 ";
                break;
            case 3:
                query += " where e.emp_status = 3 ";
                break;
            case 4:
                query += " where e.emp_status = 4 ";
                break;
        }

        if (company_code != "select") query += " and c.companycode ='" + company_code + "' ";

        if (branch_code != "select") query += " and e.emp_branch ='" + branch_code + "' ";

        if (department_code != "select") query += " and e.emp_department ='" + department_code + "' ";

        if (designation_code != "select") query += " and e.emp_designation ='" + designation_code + "'";

        switch (filter_by)
        {
            case 1:
                query += " and e.emp_code = '" + keyword + "' ";
                break;
            case 2:
                query += " and e.emp_name like '%" + keyword + "%'";
                break;
            case 3:
                query += " and e.emp_card_no = '" + keyword + "' ";
                break;
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject FilterEmployeeData(int page_number, string filters)
    {

        masters_employee page_object = new masters_employee();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable employee_data = new DataTable();

        string
            query = string.Empty,
            comanager_id = string.Empty,
            username = string.Empty,
            branch_list = string.Empty;

        int
            start_row = (page_number - 1) * 30,
            access = 0;

        try
        {

            access = Convert.ToInt32(HttpContext.Current.Session["access"]);
            username = HttpContext.Current.Session["username"].ToString();

            query = page_object.GetBaseQuery();
            query = page_object.GetFilterQuery(filters, query);

            if (access == 1 || access == 3)
            {
                query += " and e.emp_code in (select EmpID from [FetchEmployees] ('" + username + "')) ";
            }

            query += " group by e.emp_code, e.emp_name, c.CompanyCode, c.CompanyName, b.branchcode, b.branchname, d.deptcode, d.deptname, de.desigcode, de.designame, e.emp_card_no, e.emp_status , e.ot_eligibility , e.Ramadan_Eligibility ORDER BY e.emp_code, e.emp_name, c.CompanyName OFFSET " + start_row + " ROWS FETCH NEXT 30 ROWS ONLY";

            employee_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employee_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "FILTER_EMPLOYEE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Employee Data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetEmployeeData(int page_number)
    {

        masters_employee page_object = new masters_employee();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable employee_data = new DataTable();

        string
            query = string.Empty,
            comanager_id = string.Empty,
            username = string.Empty,
            branch_list = string.Empty;

        int
            start_row = (page_number - 1) * 30,
            count = 0,
            access = 0;

        try
        {

            access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            username = HttpContext.Current.Session["username"].ToString();

            query = page_object.GetBaseQuery();

            query += " where Emp_Status = '1' ";

            if (access == 1 || access == 3 || access == 2)
            {
                query += " and e.emp_code in (select EmpID from [FetchEmployees] ('" + username + "','')) ";
            }

            query += " group by e.emp_code, e.emp_name, c.CompanyCode, c.CompanyName, b.branchcode, b.branchname, d.deptcode, d.deptname, de.desigcode, de.designame, e.emp_card_no, e.emp_status  , e.ot_eligibility , e.Ramadan_Eligibility ORDER BY e.emp_code, e.emp_name, c.CompanyName OFFSET " + start_row + " ROWS FETCH NEXT 30 ROWS ONLY";

            employee_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employee_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_EMPLOYEE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Employee Data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
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

    protected DataTable GetBranchData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        DataTable branch_data = new DataTable();
        string query = string.Empty;
        int access_level = Convert.ToInt32(Session["access"]);

        if (access_level == 3 || access_level == 1)
        {
            query = "Select tb.BranchCode as branch_code, b.BranchName as branch_name From TbManagerHrBranchMapping tb, BranchMaster b Where b.BranchCode = tb.BranchCode and tb.ManagerID = '" + Session["employee_id"].ToString() + "' and b.CompanyCode = '" + company_code + "' order by b.BranchName";
        }
        else
        {
            query = "select distinct branchcode as branch_code, branchname as branch_name from branchmaster  where companycode = '" + company_code + "' order by branchname";
        }

        branch_data = db_connection.ReturnDataTable(query);

        return branch_data;
    }

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {

        masters_employee page_object = new masters_employee();
        DBConnection db_connection = new DBConnection();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        ReturnObject return_object = new ReturnObject();
        string department_query = string.Empty;
        string designation_query = string.Empty;
        string branch_query = string.Empty;

        try
        {

            department_query = "Select DeptName as department_name, DeptCode as department_code from DeptMaster where CompanyCode='" + company_code + "' order by DeptCode";
            temp_data_table = db_connection.ReturnDataTable(department_query);
            temp_data_table.TableName = "department";
            return_data_set.Tables.Add(temp_data_table);

            designation_query = "Select desigcode as designation_code, designame as designation_name from DesigMaster where CompanyCode='" + company_code + "' order by desigcode";
            temp_data_table = db_connection.ReturnDataTable(designation_query);
            temp_data_table.TableName = "designation";
            return_data_set.Tables.Add(temp_data_table);

            temp_data_table = page_object.GetBranchData(company_code);
            temp_data_table.TableName = "branch";
            return_data_set.Tables.Add(temp_data_table);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_OTHER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading data for OT Eligibility. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteEmployee(string employee_id)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        string today = DateTime.Now.Date.ToString("dd/MM/yyyy");
        DateTime date_of_leaving = new DateTime();

        try
        {
            date_of_leaving = DateTime.ParseExact(today, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            query = "update EmployeeMaster set Emp_Status = 2, Emp_Dol = '" + date_of_leaving + "' where Emp_Code = '" + employee_id + "' ";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            query = "update login set status=0 where empid = '" + employee_id + "' ";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            return_object.status = "success";
            return_object.return_data = "Employee Terminated";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "DELETE_EMPLOYEE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Terminating the Employee. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject ReinstateEmployee(string employee_id, int action)
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;

        try
        {
            query = "update employeemaster set isreinstate = " + action;

            if (action == 1)
                query += ", Emp_Status = 1, Emp_Dol = '1900-01-01' ";

            query += " where Emp_Code = '" + employee_id + "'";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            query = "update login set status = 1 where empid = '" + employee_id + "' ";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            return_object.status = "success";
            return_object.return_data = "Employee status updated!";
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "REINSTATE_EMPLOYEE");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Reinstating the Employee. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private string CreateExport(DataTable company_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        //string[] column_names =
        //    new string[] { "Employee Code", "Employee Card Number", "Employee Name", "Company", "Branch", "Department", "Designation", "Shift", "Date of Joining", "Date of Birth", "Gender", "Address", "Phone Number", "Email Address" };
        
        string[] column_names =
           new string[] { "Employee Code", "Employee Name", "Dob", "Gender", "Address", "Phone", "Email", "Date Of Join", "Date of Leave", "Company", "Branch", "Department", "Designation", "Employee Category", "Card No", "Shift", "Status", "OT_Eligibility", "Passport_No", "Passport_Exp_Date", "Emirates_No", "Nationality", "Emergency_Contact_No", "Visa_Exp_Date", "IsManager", "ManagerId", "otflag", "isreinstate", "IsHR", "AbscondingDate", "isAutoShiftEligible", "Employee Relegion" };


        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "EmployeeMaster-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "EMPLOYEE MASTER", company_data, Context, column_names);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport()
    {
        masters_employee page_object = new masters_employee();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        DateTime now = DateTime.Now;
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);

        string[] column_names = new string[] { };

        string employee_id=string.Empty,company_code = string.Empty,
            query = string.Empty, file_name = string.Empty;

        try
        {
            employee_id = HttpContext.Current.Session["username"].ToString();

            if (employee_id != "admin")
            {
                //Exporing perticular company employee data if user is not admin
                query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select TOP " + export_limit + " Emp_Code,Emp_Name,Emp_Dob,Emp_Gender ,Emp_Address ,Emp_Phone,Emp_Email,Emp_Doj,Emp_Dol ,Emp_Company,Emp_Branch,Emp_Department,Emp_Designation,Emp_Employee_Category ,Emp_Card_No ,Emp_Shift_Detail ,Emp_Status ,OT_Eligibility ,Passport_No ,Passport_Exp_Date,Emirates_No ,Nationality ,Emergency_Contact_No,Visa_Exp_Date,IsManager,ManagerId,otflag,isreinstate ,IsHR,AbscondingDate,isAutoShiftEligible,employee_religion from employeemaster where Emp_Company='" + company_code + "'";
            }
            else
            {
                //Exporing All company employee data
                query = "select TOP " + export_limit + " Emp_Code,Emp_Name,Emp_Dob,Emp_Gender ,Emp_Address ,Emp_Phone,Emp_Email,Emp_Doj,Emp_Dol ,Emp_Company,Emp_Branch,Emp_Department,Emp_Designation,Emp_Employee_Category ,Emp_Card_No ,Emp_Shift_Detail ,Emp_Status ,OT_Eligibility ,Passport_No ,Passport_Exp_Date,Emirates_No ,Nationality ,Emergency_Contact_No,Visa_Exp_Date,IsManager,ManagerId,otflag,isreinstate ,IsHR,AbscondingDate,isAutoShiftEligible,employee_religion from employeemaster";
            }

            //query = "select TOP " + export_limit + " Emp_Code, Emp_Card_No, Emp_Name, Emp_Company, Emp_Branch, Emp_Department, Emp_Designation, Emp_Shift_Detail, Emp_Doj, Emp_Dob, Emp_Gender, Emp_Address, Emp_Phone, Emp_Email from EmployeeMaster";
            company_data = db_connection.ReturnDataTable(query);

            if (company_data.Rows.Count > 0)
            {

                file_name = page_object.CreateExport(company_data);

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
    public static ReturnObject DoExportTransaction()
    {
        masters_employee page_object = new masters_employee();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable employee_transaction_data = new DataTable();
        DateTime now = DateTime.Now;
        int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);

        string[] column_names = new string[] { };

        string employee_id = string.Empty, company_code = string.Empty,
            query = string.Empty, file_name = string.Empty;

        try
        {
            employee_id = HttpContext.Current.Session["username"].ToString();

            if (employee_id != "admin")
            {
                //Exporing perticular company employee data if user is not admin
                query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select TOP " + export_limit + "  empid  ,tt.transactionname ,fromdate , todate , transactiondata ,  case when isactive = 1 then 'Active' else 'InActive' end As Status from EmployeeTransactionData  etd left join transactiontype tt on tt.TransactionType = etd.TransactionType  where empid in  (select empid from [fetchemployees]('" + employee_id + "',''))  order by tt.TransactionName";

            }
            else
            {
                //Exporing All company employee data
                query = "select TOP " + export_limit + "  empid  ,tt.transactionname ,fromdate , todate , transactiondata ,  case when isactive = 1 then 'Active' else 'InActive' end As Status from EmployeeTransactionData  etd left join transactiontype tt on tt.TransactionType = etd.TransactionType  where empid in  (select empid from [fetchemployees]('admin',''))  order by tt.TransactionName";
            }

            //query = "select TOP " + export_limit + " Emp_Code, Emp_Card_No, Emp_Name, Emp_Company, Emp_Branch, Emp_Department, Emp_Designation, Emp_Shift_Detail, Emp_Doj, Emp_Dob, Emp_Gender, Emp_Address, Emp_Phone, Emp_Email from EmployeeMaster";
            employee_transaction_data = db_connection.ReturnDataTable(query);

            if (employee_transaction_data.Rows.Count > 0)
            {

                file_name = page_object.CreateExportTransaction(employee_transaction_data);

                return_object.status = "success";
                return_object.return_data = file_name;
            }
            else
            {
                return_object.status = "info";
                return_object.return_data = "No data available for export";
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
    private string CreateExportTransaction(DataTable employee_transaction_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        //string[] column_names =
        //    new string[] { "Employee Code", "Employee Card Number", "Employee Name", "Company", "Branch", "Department", "Designation", "Shift", "Date of Joining", "Date of Birth", "Gender", "Address", "Phone Number", "Email Address" };

        string[] column_names =
           new string[] { "Employee Code", "Transaction Name", "From Date", "To Date", "Transaction Value", "Status" };


        string
            user_id = HttpContext.Current.Session["employee_id"].ToString(),
            file_name = "EmployeeDataTrnsaction-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "EMPLOYEE DATA TRANSACTION", employee_transaction_data, Context, column_names);

        return file_name;
    }
}