using System;
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
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Collections.Generic;
using SecurAX.Logger;
using SecurAX.Import.Excel;
using SecurAX.Export.Excel;
using Newtonsoft.Json;
using System.Web.Services;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ClosedXML.Excel;


public partial class shift_import : System.Web.UI.Page
{
    const string page = "SHIFT_ROSTER_IMPORT";

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

            message = "An error occurred while loading Sihft Roster Import page. Please try again. If the error persists, please contact Support.";

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

    private string[] GetShiftCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable shift_data = new DataTable();
        string[] shift_code_array = null;
        string query = string.Empty;

        query = "select distinct(shift_code) from shift ";
        shift_data = db_connection.ReturnDataTable(query);

        shift_code_array = new string[shift_data.Rows.Count];

        for (int i = 0; i < shift_data.Rows.Count; i++)
        {
            shift_code_array[i] = shift_data.Rows[i]["shift_code"].ToString().ToUpper();
        }

        return shift_code_array;
    }

    private string[] GetEmployeeCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable employee_data = new DataTable();
        int access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        string current_user_id = HttpContext.Current.Session["employee_id"].ToString();
        string[] employee_code_array = null;
        string query = string.Empty;

        if (access_level == 1 || access_level == 3)
        {
            query = "select distinct (Emp_Code), Emp_Company from employeemaster where Emp_Status='1' and Emp_Code in (select Empid from [FetchEmployees] ('" + current_user_id + "',''))";
        }
        else
        {
            query = "select distinct (Emp_Code), Emp_Company from employeemaster where Emp_Status='1'";
        }

        employee_data = db_connection.ReturnDataTable(query);

        employee_code_array = new string[employee_data.Rows.Count];

        for (int i = 0; i < employee_data.Rows.Count; i++)
        {
            employee_code_array[i] = employee_data.Rows[i]["Emp_Code"].ToString().ToUpper();
        }

        return employee_code_array;
    }

    [WebMethod]
    public static ReturnObject DoImport(string file_name)
    {
        shift_import page_object = new shift_import();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        string[]
            employee_code_array = null, shift_code_array = null,
            day = new string[31];

        DataRow[] result = null;
        DataRow first_row = null;

        DataTable excel_shift_data = new DataTable();

        bool
            update_status = false, shift_code_exists = true, employee_code_exists = true;
        int IsActiveShift = 0;
        string
            default_upload_path = string.Empty, full_upload_path = string.Empty,
            employee_id = string.Empty, return_message = string.Empty,
            shift_month = string.Empty, shift_year = string.Empty,
            query = string.Empty, update_query = string.Empty;

        try
        {
            default_upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            full_upload_path = HttpContext.Current.Server.MapPath("~/" + default_upload_path + "/" + file_name);

            // Read the excel file and store the data in a DataTable.
            excel_shift_data = ExcelImport.ImportExcelToDataTable(full_upload_path, "");
            // Get the 1st ROW of the EXCEL sheet
            first_row = excel_shift_data.Rows[0];
            // Remove the 1st ROW of the EXCEL sheet. This is essentially the title row.
            excel_shift_data.Rows.Remove(first_row);

            employee_code_array = page_object.GetEmployeeCodes();
            shift_code_array = page_object.GetShiftCodes();

            if (excel_shift_data.Rows.Count > 0)
            {
                foreach (DataRow row in excel_shift_data.Rows)
                {
                    employee_id = row[0].ToString().Trim();
                    shift_code_exists = true;
                    employee_code_exists = true;
                    update_status = false;
                    update_query = string.Empty;

                    if (string.IsNullOrEmpty(employee_id))
                    {
                        employee_code_exists = false;
                    }
                    else
                    {
                        if (Array.IndexOf(employee_code_array, employee_id) < 0)
                        {
                            return_message += Environment.NewLine + "You don't have permission to assign shift for Employee Code '" + employee_id + "' or this Employee Code Invalid.";
                            employee_code_exists = false;
                        }
                        else
                        {
                            shift_month = row[1].ToString().Trim();
                            shift_year = row[2].ToString().Trim();

                            // Getting shift codes for all the days of the month
                            for (int i = 3; i < 34; i++)
                                day[i - 3] = row[i].ToString().ToUpper().Trim();

                            query = "select count(*) from shiftemployee where month = '" + shift_month + "' and year = '" + shift_year + "' and Empid = '" + employee_id + "'";

                            // Read employee data from shiftemployee Table
                            if (db_connection.GetRecordCount(query) > 0)
                                update_status = true;

                            for (int k = 0; k < day.Length; k++)
                            {
                                if (!string.IsNullOrEmpty(day[k]))
                                {
                                    if (day[k].ToString().ToLower() != "woff")
                                    {
                                        if (Array.IndexOf(shift_code_array, day[k].ToString().ToUpper()) < 0)
                                        {
                                            return_message += Environment.NewLine + "Shift Code '" + day[k].ToString() + "' is Invalid for EmpID: " + employee_id;
                                            shift_code_exists = false;
                                        }
                                        else
                                        {
                                            query = "select IsActive from shift where shift_code='" + day[k].ToString().ToUpper() + "'";
                                            IsActiveShift = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                            if (IsActiveShift == 0)
                                            {
                                                return_message += Environment.NewLine + "Shift Code '" + day[k].ToString() + "' is deactive for EmpID: " + employee_id;
                                                shift_code_exists = false;
                                            }
                                        }
                                        // END OF IF
                                    }
                                    //END OF IF
                                }
                                // END OF IF
                            }
                            // END OF FOR
                        }

                        // If shift and empid exists then go for main operation
                        if ((shift_code_exists == true) && (employee_code_exists == true))
                        {
                            if (update_status == true)
                            {
                                for (int i = 0; i < 31; i++)
                                {
                                    string shift_code = day[i].ToString();

                                    if (shift_code == "WOFF") shift_code = "woff";

                                    if (!string.IsNullOrEmpty(shift_code))
                                    {
                                        update_query += " day" + (i + 1) + "='" + shift_code + "',";
                                    }
                                    else
                                    {
                                        update_query += " day" + (i + 1) + "='',";
                                    }
                                }

                                if (!string.IsNullOrEmpty(update_query))
                                {
                                    update_query = update_query.Remove(update_query.Length - 1, 1);
                                    update_query = "Update shiftemployee set " + update_query + " where empid='" + employee_id + "' and month='" + shift_month + "' and year='" + shift_year + "'";
                                    db_connection.ExecuteQuery_WithOutReturnValue(update_query);
                                    return_message += Environment.NewLine + "Roster Updated for EmpID: " + employee_id + ", Month: " + shift_month + ", Year: " + shift_year;

                                }
                            }
                            else
                            {
                                query = "select Emp_Company from EmployeeMaster where Emp_Code = '" + employee_id + "' ";
                                string company_code = db_connection.ExecuteQuery_WithReturnValueString(query);

                                update_query = string.Empty;
                                update_query = @"insert into ShiftEmployee(EmpCompanyCode, Empid,Month,Year,day1,day2,day3,day4,day5,day6,day7,day8,day9,day10,day11,day12,day13,day14,day15,day16,day17,day18,day19,day20,day21,day22,day23,day24,day25,day26,day27,day28,day29,day30,day31) values ( '" + company_code + "', '" + employee_id + " ', '" + shift_month + "', '" + shift_year + "', ";

                                for (int i = 0; i < day.Length; i++)
                                {
                                    update_query += " '" + day[i].ToString() + "',";
                                }

                                update_query = update_query.Remove(update_query.Length - 1, 1);
                                update_query += " )";
                                db_connection.ExecuteQuery_WithOutReturnValue(update_query);

                                return_message += Environment.NewLine + "Roster Inserted for EmpID: " + employee_id + ", Month: " + shift_month + ", Year: " + shift_year;
                            }

                        }

                    }
                    // END OF FOREACH LOOP
                }
                // END OF IF

                return_object.status = "success";
                return_object.return_data = return_message;
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = "No data found in the Excel Sheet. Please check your file and try again.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_SHIFT_ROSTER");

            return_object.status = "error";
            return_object.return_data = "An error occurred while importing shift roster. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    private string CreateExport(DataTable company_data)
    {
        DateTime now = DateTime.Now;

        // Initializing the column names for the export. 
        string[] column_names =
            new string[] { "Shift Code", "Shift Name", "In Time", "Out Time", "Weekly Off 1", "Weekly Off 2" };

        string
            user_id = HttpContext.Current.Session["username"].ToString(),
            file_name = "ShiftDetails-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

        ExcelExport.ExportDataToExcel(file_name, "SHIFT DETAILS", company_data, Context, column_names);

        return file_name;
    }

    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }

    [WebMethod]
    public static ReturnObject DoExport()
    {
        shift_import page_object = new shift_import();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        DateTime now = DateTime.Now;
        if (HttpContext.Current.Session["username"] == null)
        {
            // HttpContext.Current.Response.Redirect("~/logout.aspx", true);
            return_object = page_object.DoLogout();
        }
        else
        {
            int export_limit = Convert.ToInt32(ConfigurationManager.AppSettings["EXPORT_LIMIT"]);

            string[] column_names = new string[] { };

            string
                query = string.Empty, file_name = string.Empty;

            try
            {
                query = "select TOP " + export_limit + " Shift_Code ,Shift_Desc, In_Time, Out_Time,  WeeklyOff1, WeeklyOff2 from Shift where IsActive=1 order by Shift_Code ASC";
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
                    return_object.return_data = "No data found.";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "GET_DATA_FOR_EXPORT");

                return_object.status = "error";
                return_object.return_data = "An error occurred while generating your report. Please try again. If the error persists, please contact Support.";
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }
}