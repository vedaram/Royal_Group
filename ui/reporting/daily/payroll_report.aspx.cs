using System;
using System.Data;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;
using System.Collections;
using System.Globalization;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Diagnostics;

public partial class daily_payroll_report : System.Web.UI.Page
{
    const string page = "DAILY_PAYROLL_REPORT";

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

            message = "An error occurred while loading Detailed Payroll Report. Please try again. If the error persists, please contact Support.";

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
                //query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                //company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                //query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster where CompanyCode='" + company_code + "'";
                query = " Select distinct( CompanyCode) as  company_code,CompanyName as company_name  from CompanyMaster   where  CompanyCode in ( Select  CompanyCode from  TbManagerHrBranchMapping where ManagerID='" + employee_id + "')  or CompanyCode in (Select Emp_company  from Employeemaster where Emp_code='" + employee_id + "')";
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
            return_object.return_data = "An error occurred while loading Company Data. Please refresh the page and try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        string query = string.Empty;

        try
        {

            query = "select DeptName as department_name from DeptMaster where CompanyCode='" + company_code + "' order by DeptName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "department";
            return_data_set.Tables.Add(temp_data_table);

            query = "select DISTINCT BranchName as branch_name from BranchMaster where CompanyCode='" + company_code + "' order by BranchName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "branch";
            return_data_set.Tables.Add(temp_data_table);

            query = "select DesigName as designation_name from DesigMaster where CompanyCode='" + company_code + "' order by DesigName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "designation";
            return_data_set.Tables.Add(temp_data_table);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OTHER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private DataTable PrepareDataForExport(string filters)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable where_conditions = new Hashtable();
        DataTable filtered_data = new DataTable();
        JObject filter_options = new JObject();

        string from_date, to_date, company_name,
            branch_name, department_name, designation_name,
            employee_id, employee_name,
            where_clause, user_name,
            current_user_id;

        int user_access_level = 0;

        filter_options = JObject.Parse(filters);
        company_name = filter_options["company_name"].ToString();
        branch_name = filter_options["branch"].ToString();
        department_name = filter_options["department"].ToString();
        designation_name = filter_options["designation"].ToString();
        employee_name = filter_options["employee_name"].ToString();
        employee_id = filter_options["employee_id"].ToString();

        from_date = DateTime.ParseExact(filter_options["from_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        to_date = DateTime.ParseExact(filter_options["to_date"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

        // Get current logged in user data from the Session variable
        user_name = HttpContext.Current.Session["username"].ToString();
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (user_name != "admin")
            current_user_id = HttpContext.Current.Session["employee_id"].ToString();
        else
            current_user_id = user_name;

        where_clause = " and emp_id in (select EmpID from [FetchEmployees] ('" + current_user_id + "','')) ";

        if (company_name != "select")
        {
            #region Commented  bcoz employee of other company should come in this Report  also changed in the last line of Procedure PayrollAgrregration
            // where_clause = where_clause + " and COMP_NAME = '" + company_name + "'";  
            #endregion

            if (branch_name != "select")
                where_clause = where_clause + " and CAT_NAME = '" + branch_name + "'";

            if (department_name != "select")
                where_clause = where_clause + " and DEPT_NAME = '" + department_name + "'";

            if (designation_name != "select")
                where_clause = where_clause + " and DESIG_NAME = '" + designation_name + "' ";

            if (employee_id != "")
                where_clause = where_clause + " and Emp_ID = '" + employee_id + "' ";

            if (employee_name != "")
                where_clause = where_clause + " and Emp_Name like '%" + employee_name + "%' ";

            where_conditions.Add("FromDate", from_date);
            where_conditions.Add("ToDate", to_date);
            where_conditions.Add("whereCondition", where_clause);
            // companyname parameter added to get breakhours for particular company 
            where_conditions.Add("CompanyName", company_name);

            filtered_data = db_connection.ExecuteStoredProcedureWithHashtable_WithReturnDatatable("PayrollAggregation", where_conditions);
        }

        return filtered_data;
    }

    private string CreateExport(DataTable filtered_data, string from_date, string to_date)
    {
        DBConnection db_connection = new DBConnection();
        DataTable totals = new DataTable();
        DateTime now = DateTime.Now;

        // setup the workbook details
        var work_book = new XLWorkbook();
        var work_sheet = work_book.Worksheets.Add("Sheet 1");
        var dataForExport = new List<string[]>();

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Employee ID", "Employee Name", "Branch", "Department", "Designation", "Month", "Week Day", "Work Date", "First Punch", "Last Punch", "Actual Hours", "Rounded Hours", "Mandatory Hours", "Discrepancey", "Status", "Late By", "Early By", "1MP", "2MP", "Overtime" };

        string
            user_id = HttpContext.Current.Session["username"].ToString(),
            file_name = "DailyPayrollReport-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx",
            current_employee_id = string.Empty,
            export_path = HttpContext.Current.Server.MapPath("~/exports/data/"),
            employee_id = string.Empty, employee_name = string.Empty, department = string.Empty, branch = string.Empty,
            designation = string.Empty, month = string.Empty, week_day = string.Empty,
            work_date = string.Empty, first_punch = string.Empty, last_punch = string.Empty,
            actual_hours = string.Empty, rounded_hours = string.Empty, mandatory_hours = string.Empty, discrepancy = string.Empty,
            status = string.Empty, late_by = string.Empty, early_by = string.Empty,
            mp1 = string.Empty, mp2 = string.Empty, overtime = string.Empty, query = string.Empty;

        int counter = 3;

        // storing the employee ID on the first row in this variable.
        current_employee_id = filtered_data.Rows[0]["EmployeeID"].ToString();

        dataForExport.Add(new string[] { filtered_data.Rows[0]["CompanyName"].ToString(), "Report From " + from_date + " to " + to_date });
        work_sheet.Range(1, 1, 1, 19).Style.Font.Bold = true;

        // adding the header row for the 1st employee
        dataForExport.Add(column_names);
        // after adding the header, lets make the font style bold
        work_sheet.Range(2, 1, 2, 20).Style.Font.Bold = true;

        foreach (DataRow data_row in filtered_data.Rows)
        {
            if (counter != 3 && current_employee_id != data_row["EmployeeID"].ToString())
            {
                // call function to generate the sum
                query = "select * from [FetchDiscrepancyHours]('" + current_employee_id + "')";
                totals = db_connection.ReturnDataTable(query);

                // add a row befor the next employee data is added.
                dataForExport.Add(new string[] { "Total", "", "", "", "", "", "", "", "", "", totals.Rows[0]["Actual_Hours"].ToString(), totals.Rows[0]["Rounded_Hours"].ToString(), totals.Rows[0]["Mandatory_Hours"].ToString(), totals.Rows[0]["Discrepancy"].ToString(), totals.Rows[0]["Status"].ToString(), totals.Rows[0]["LateBy"].ToString(), totals.Rows[0]["EarlBy"].ToString(), totals.Rows[0]["1MP"].ToString(), totals.Rows[0]["2MP"].ToString(), totals.Rows[0]["OverTime"].ToString() });

                // Highlighting total row 
                work_sheet.Range(counter, 1, counter, 20).Style.Font.Bold = true;
                work_sheet.Range(counter, 1, counter, 20).Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                work_sheet.Range(counter, 1, counter, 20).Style.Font.FontColor = XLColor.White;

                // capture the new employee ID
                current_employee_id = data_row["EmployeeID"].ToString();

                counter += 1;
            }

            employee_id = data_row["EmployeeID"].ToString();
            employee_name = data_row["EmployeeName"].ToString();
            branch = data_row["BranchName"].ToString();
            department = data_row["Department"].ToString();
            designation = data_row["Designation"].ToString();
            month = data_row["Month"].ToString();
            week_day = data_row["WeekDay"].ToString();
            work_date = Convert.ToDateTime(data_row["WorkDate"].ToString()).ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(data_row["FirstIn"].ToString()))
                first_punch = Convert.ToDateTime(data_row["FirstIn"].ToString()).ToString("HH:mm");
            else
                first_punch = "";

            Debug.WriteLine(data_row["LastOut"].ToString());
            if (!string.IsNullOrEmpty(data_row["LastOut"].ToString()))
                last_punch = Convert.ToDateTime(data_row["LastOut"].ToString()).ToString("HH:mm");
            else
                last_punch = "";

            actual_hours = Convert.ToDateTime(data_row["Actual_Hours"].ToString()).ToString("HH:mm");
            rounded_hours = data_row["Rounded_Hours"].ToString();
            mandatory_hours = data_row["Mandatory_Hours"].ToString();
            discrepancy = data_row["Discrepancy"].ToString();
            status = data_row["Status"].ToString();
            late_by = data_row["LateBy"].ToString();
            early_by = data_row["EarlBy"].ToString();
            mp1 = data_row["OneMissingPunch"].ToString();
            mp2 = data_row["TwoMissingPunch"].ToString();
            overtime = data_row["OverTime"].ToString();

            dataForExport.Add(new string[] { employee_id, employee_name, branch, department, designation, month, week_day, work_date, first_punch, last_punch, actual_hours, rounded_hours, mandatory_hours, discrepancy, status, late_by, early_by, mp1, mp2, overtime });

            // checking for WO , WOP & H and given those row differenct background color ..
            if ((mandatory_hours == "00:00") && (status == "H" || status == "WO" || status == "WOP" || status == "HP"))
            {
                work_sheet.Range(counter, 1, counter, 20).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }
            // checking for WO Day contains Missing punch & making row color as blue
            else if (status.Contains("MP"))
            {
                int isWfCountToInt = 0;

                isWfCountToInt = db_connection.ExecuteProcedureInOutParametersToChkWO("checkWOForPassedDate", work_date, employee_id, "OutputCount");

                if (isWfCountToInt > 0)
                {
                    work_sheet.Range(counter, 1, counter, 20).Style.Fill.BackgroundColor = XLColor.LightBlue;
                }

            }

            counter++;
        }

        query = "select * from [FetchDiscrepancyHours]('" + current_employee_id + "')";
        totals = db_connection.ReturnDataTable(query);

        // add total row for  the final employee data ...
        dataForExport.Add(new string[] { "Total", "", "", "", "", "", "", "", "", "", totals.Rows[0]["Actual_Hours"].ToString(), totals.Rows[0]["Rounded_Hours"].ToString(), totals.Rows[0]["Mandatory_Hours"].ToString(), totals.Rows[0]["Discrepancy"].ToString(), totals.Rows[0]["Status"].ToString(), totals.Rows[0]["LateBy"].ToString(), totals.Rows[0]["EarlBy"].ToString(), totals.Rows[0]["1MP"].ToString(), totals.Rows[0]["2MP"].ToString(), totals.Rows[0]["OverTime"].ToString() });
        work_sheet.Range(counter, 1, counter, 20).Style.Font.Bold = true;
        work_sheet.Range(counter, 1, counter, 20).Style.Fill.BackgroundColor = XLColor.AirForceBlue;
        work_sheet.Range(counter, 1, counter, 20).Style.Font.FontColor = XLColor.White;
        // Adding the final data to the sheet
        work_sheet.Cell(1, 1).InsertData(dataForExport);
        // Saving the file to disk.
        work_book.SaveAs(export_path + file_name);

        return file_name;
    }

    [WebMethod]
    public static ReturnObject DoExport(string filters)
    {

        daily_payroll_report page_object = new daily_payroll_report(); // instance of the page to access non static methods.
        ReturnObject return_object = new ReturnObject();
        DataTable filtered_data = new DataTable();
        JObject filter_options = new JObject();
        string
            file_name = string.Empty,
            from_date = string.Empty,
            to_date = string.Empty;

        try
        {

            filtered_data = page_object.PrepareDataForExport(filters);

            if (filtered_data.Rows.Count > 0)
            {
                filter_options = JObject.Parse(filters);
                from_date = filter_options["from_date"].ToString();
                to_date = filter_options["to_date"].ToString();
                file_name = page_object.CreateExport(filtered_data, from_date, to_date);

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
            page_object.Dispose(); //Disposing of the page object to avoid memory leak.
        }

        return return_object;
    }
}