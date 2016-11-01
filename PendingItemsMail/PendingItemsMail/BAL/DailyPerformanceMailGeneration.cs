using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Globalization;
using ClosedXML.Excel;

namespace PendingItemsMail
{
    class PendingItemsMail
    {
        #region global declarations
        Hashtable hashtable1 = new Hashtable();
        public static SqlConnection con;
        //string whereclause = "";
        string directoryname = string.Empty;
        IniFile f = new IniFile(string.Format("{0}\\Securtime.ini", Application.StartupPath));

        #endregion
        public void GenerateEmployeeDailyReport()
        {
            string FileExtension = "EXCEL";
            
            string server = f.IniReadValue("LogFilePath", "Path");
            directoryname = server.ToString();
            if (!Directory.Exists(directoryname))
            {
                Directory.CreateDirectory(directoryname);
            }

            try
            {
                bool mailsendflag = false;
                int count = 0;
                int day = 0;
                string monthnumber = string.Empty;
                int year = 0;
                DBConnection db_connection = new DBConnection();
                DataTable filtered_data = new DataTable();
                string
                from_date, to_date, final_date, first_date, initial_date, managerorhrcode, managerorhrname, managerorhremail, ismanager, ishr,
                where_clause, current_user_id, companyname, managernHRfetchquery, dayname, currentdayname = string.Empty;
                DateTime last_date = new DateTime();
                Hashtable filter_conditions = new Hashtable();
                monthnumber = DateTime.Now.ToString("MMM");
                year = DateTime.Now.Year;
                //day = Convert.ToInt32(f.IniReadValue("Daynumber", "number"));
                dayname = f.IniReadValue("Dayname", "name");
                currentdayname = DateTime.Now.DayOfWeek.ToString();
                final_date = DateTime.Now.ToString("dd-MMM-yyyy");
                to_date = DateTime.ParseExact(final_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                string firstDay = string.Empty;
                last_date = Convert.ToDateTime(to_date);
                initial_date = last_date.AddDays(-6).ToString("dd-MMM-yyyy");
                from_date = DateTime.ParseExact(initial_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                managernHRfetchquery = "select emp_code,Emp_Name ,  emp_email , ismanager , ishr from employeemaster where emp_status = 1 and ( ismanager = 1 or ishr = 1 )";
                DataTable managerHRlist = new DataTable();
                managerHRlist = db_connection.ReturnDataTable(managernHRfetchquery);
                if (dayname.ToUpper() == currentdayname.ToUpper())
                {
                    foreach (DataRow row in managerHRlist.Rows)
                    {
                        managerorhrcode = row["emp_code"].ToString();
                        managerorhrname = row["Emp_Name"].ToString();
                        managerorhremail = row["emp_email"].ToString();
                        ismanager = row["ismanager"].ToString();
                        ishr = row["ishr"].ToString();

                        if (!string.IsNullOrEmpty(managerorhremail))
                        {
                            where_clause = "where pdate between '" + from_date + "' and '" + to_date + "' and  emp_id in (select EmpID from [FetchEmployees] ('" + managerorhrcode + "',''))";
                            filter_conditions.Add("where", where_clause);
                            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("DailyPerformanceReporFilter1", filter_conditions);
                            //filtered_data = db_connection.ExecuteStoredProcedureWithHashtable_WithReturnDatatable("PrepareMonthlyTimeSheet", filter_conditions);
                            filter_conditions.Clear();
                            string query, file_name, company_name, exportPath = string.Empty;
                            company_name = db_connection.ExecuteQuery_WithReturnValueString("select companyname from companymaster where companycode in ( select emp_company from employeemaster where emp_code  ='" + managerorhrcode + "')");
                            query = "select PDate, Emp_ID, Emp_Name,cat_name,employee_category ,   Shift_In, In_Punch, Out_Punch, Shift_Out, WorkHRs, Status, OT, LateBy, EarlyBy from DailyPerformancereport1";
                            filtered_data = db_connection.ReturnDataTable(query);
                            if (filtered_data.Rows.Count > 0)
                            {
                                file_name = CreateExport(filtered_data, company_name, to_date, managerorhrcode);
                                exportPath = file_name.Substring(0, file_name.LastIndexOf('.'));
                                exportPath = exportPath + ".xlsx";





                                ErrorLog.ErrorLogfile(@"" + directoryname + " ", "Weekly Report Generated on '" + to_date + "' for Manager/HR ID  '" + managerorhrcode + "' and name '" + managerorhrname + "' ", "");
                                ErrorLog.ErrorLogfile(@"" + directoryname + " ", "Mail Sending Started for Manager/HR ID  '" + managerorhrcode + "' and name '" + managerorhrname + "' ", "");

                               // mailsendflag = MailSender.SendEmail(managerorhremail, "Daily Performance Report", "", "", exportPath, managerorhrname, directoryname);
                                if (mailsendflag)
                                {
                                    ErrorLog.ErrorLogfile(@"" + directoryname + " ", "Mail Sending done successfull for Manager email ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                                }
                                else
                                {
                                    ErrorLog.ErrorLogfile(@"" + directoryname + " ", "Mail Sending failed for Manager email ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                                }
                            }
                            else
                            {
                                ErrorLog.ErrorLogfile(@"" + directoryname + " ", "No record found for employee data under Manager/HR ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                            }
                        }
                        else
                        {
                            ErrorLog.ErrorLogfile(@"" + directoryname + " ", "EmailID is blank for Manager/HR ID " + managerorhrcode + " and name " + managerorhrname + "  ", "");
                        }
                    }
                }
                else
                {

                }






            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogfile(@"" + directoryname + " ", ex.Message, ex.StackTrace);
            }


        }
        private string CreateExport(DataTable filtered_data, string company_name, string to_date, string user_id)
        {
            DateTime now = DateTime.Now;

            // Initializing the column names for the export. 
            string[] column_names =
                new string[] { "Punch Date", "Employee ID", "Employee Name", "Branch", "Category", "Shift In", "Punch In", "Punch Out", "Shift Out", "Hours Worked", "Status", "OT", "Late By", "Early By", "Remarks" };

            string importFile, exportPath, filepath = string.Empty;

            exportPath = "DailyPerformanceReport-" + user_id + "-" + now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            IniFile f1 = new IniFile(string.Format("{0}\\DailyAttendanceReport_Template.xlsx", Application.StartupPath));
            filepath = f1.IniFilePath(f1.path);

            string test = ExportDataToExcelWithLogo(filepath, "DAILY PERFORMANCE REPORT", filtered_data, column_names, company_name, exportPath);

            return test;
        }
        private string ExportDataToExcelWithLogo(string importFile, string report_title, DataTable data, string[] column_names, string company_name, string exportPath)
        {
            DBConnection db_connection = new DBConnection();
            DataTable leave_status = new DataTable();
            string query = string.Empty;
            string status_description, export_path, server1, directoryname1 = string.Empty;
            var work_book = new XLWorkbook(importFile);
            var work_sheet = work_book.Worksheet("Sheet 1");
            var dataForExport = new List<string[]>();

            server1 = f.IniReadValue("Reportpath", "Path");
            directoryname1 = @"" + server1;
            if (!Directory.Exists(directoryname1))
            {
                Directory.CreateDirectory(directoryname1);
            }
            export_path = directoryname1;
            int counter = 0, data_count = 0;
            DataTable totals = new DataTable();

            var title_styles = work_book.Style;
            data_count = data.Rows.Count;
            // Background color for the cell
            title_styles.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

            // Font configuration
            title_styles.Font.Bold = true;
            title_styles.Font.FontColor = XLColor.White;
            title_styles.Font.FontSize = 30;

            // Text Alignment
            title_styles.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Adding the Title in the Cell A1,1
            work_sheet.Cell(1, 1).Value = report_title;

            // Get workbook range for Title
            var title_range = work_sheet.Range(1, 1, 3, 24);

            // Merge the cells in the above range to form the title.
            title_range.Merge();

            // Apply the styles defined before
            title_range.Style = title_styles;

            // For Showing Company Name below report title
            work_sheet.Cell(4, 1).Value = "Company:  " + company_name;
            work_sheet.Range(4, 1, 4, 20).Style.Font.Bold = true;
            work_sheet.Range(4, 1, 4, 20).Style.Font.FontSize = 16;

            // Manually adding the column names, as the data adding functions don't display the column names.
            for (counter = 0; counter < column_names.Length; counter++)
            {
                work_sheet.Cell(5, counter + 1).Value = column_names[counter];
                work_sheet.Cell(5, counter + 1).Style.Font.Bold = true;
                work_sheet.Cell(5, counter + 1).Style.Font.FontSize = 16;
            }

            // Adding the datatable to the selected range
            work_sheet.Cell(6, 1).Value = data.AsEnumerable();

            //  Added for showing various count //

            if (report_title == "DAILY PERFORMANCE REPORT")
            {
                work_sheet.Cell(7 + data_count, 7).Value = "Summary Sheet";
                work_sheet.Cell(7 + data_count, 7).Style.Font.Bold = true;
                work_sheet.Cell(7 + data_count, 7).Style.Font.FontSize = 16;


                // Getting all the calculated data from function 
                query = "select * from [FetchTotalCounts]('1')";
                totals = db_connection.ReturnDataTable(query);


                work_sheet.Cell(8 + data_count, 1).Value = "Total Count";
                work_sheet.Cell(8 + data_count, 1).Style.Font.Bold = true;
                work_sheet.Cell(8 + data_count, 1).Style.Font.FontSize = 10;
                work_sheet.Cell(8 + data_count, 3).Value = "Days";
                work_sheet.Cell(8 + data_count, 3).Style.Font.Bold = true;
                work_sheet.Cell(8 + data_count, 3).Style.Font.FontSize = 10;
                work_sheet.Cell(8 + data_count, 4).Value = totals.Rows[0]["TotalDays"].ToString();
                work_sheet.Cell(8 + data_count, 6).Value = "Employees";
                work_sheet.Cell(8 + data_count, 6).Style.Font.Bold = true;
                work_sheet.Cell(8 + data_count, 6).Style.Font.FontSize = 10;
                work_sheet.Cell(8 + data_count, 7).Value = totals.Rows[0]["TotalEmployee"].ToString();
                work_sheet.Cell(8 + data_count, 9).Value = "Work Hours";
                work_sheet.Cell(8 + data_count, 9).Style.Font.Bold = true;
                work_sheet.Cell(8 + data_count, 9).Style.Font.FontSize = 10;
                work_sheet.Cell(8 + data_count, 10).Value = totals.Rows[0]["Total_Hours"].ToString();
                work_sheet.Cell(8 + data_count, 12).Value = "Early Hours";
                work_sheet.Cell(8 + data_count, 12).Style.Font.Bold = true;
                work_sheet.Cell(8 + data_count, 12).Style.Font.FontSize = 10;
                work_sheet.Cell(8 + data_count, 13).Value = totals.Rows[0]["Total_Earlyby"].ToString();
                work_sheet.Cell(8 + data_count, 15).Value = "Late Hours";
                work_sheet.Cell(8 + data_count, 15).Style.Font.Bold = true;
                work_sheet.Cell(8 + data_count, 15).Style.Font.FontSize = 10;
                work_sheet.Cell(8 + data_count, 16).Value = totals.Rows[0]["Total_Lateby"].ToString();

            }


            status_description = "Description:- P=Present, A=Absent, L=Leave, AHL=Half Absent Half Leave, PHL=Half Day Present Half Day Leave, V=Vacation, OD=On Duty,CO=Comp. Off,MI=Manual in punch,MO=Manual Out Punch,M=Manual Punch,MS=Missing Swipe";
            //getting Leave code description

            leave_status = db_connection.ReturnDataTable("select  leavecode,status from leavemaster where CompanyCode in  ( select companycode from CompanyMaster where CompanyName='" + company_name + "') ");
            //leaveStatus
            string leaveCodeColumn = string.Empty, leaveStatusColumn = string.Empty;
            string leaveStatus = string.Empty;
            int count = 0;
            foreach (DataRow row in leave_status.Rows)
            {
                count++;
                foreach (DataColumn col in leave_status.Columns)
                {
                    leaveCodeColumn = row["leavecode"].ToString();
                    leaveStatusColumn = row["status"].ToString();
                }
                if (count == 1)
                {
                    leaveStatus = leaveCodeColumn + " = " + leaveStatusColumn;
                }
                else
                {
                    leaveStatus = leaveStatus + "," + leaveCodeColumn + " = " + leaveStatusColumn;
                }


            }

            var description_styles = work_book.Style;

            description_styles.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            description_styles.Font.Bold = false;
            description_styles.Font.FontColor = XLColor.AliceBlue;
            description_styles.Font.FontSize = 11;
            description_styles.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            work_sheet.Cell(9 + data_count, 1).Value = status_description;

            work_sheet.Range(9 + data_count, 1, 9 + data_count, 24).Merge();
            work_sheet.Range(9 + data_count, 1, 9 + data_count, 24).Style = description_styles;

            work_sheet.Cell(11 + data_count, 1).Value = leaveStatus;
            work_sheet.Range(11 + data_count, 1, 11 + data_count, 21).Style = description_styles;
            work_sheet.Range(11 + data_count, 1, 11 + data_count, 15).Merge();
            work_book.SaveAs(export_path + exportPath);
            exportPath = export_path + exportPath;
            return exportPath;

        }
    }
}
