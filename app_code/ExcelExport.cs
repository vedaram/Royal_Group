using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ClosedXML.Excel;
using System.Reflection;

namespace SecurAX.Export.Excel
{
    public sealed class ExcelExport
    {
        private ExcelExport() { }

        public static void ExportDataToExcel(string file_name, string report_title, DataTable data, HttpContext context, string[] column_names)
        {
            var work_book = new XLWorkbook();
            var work_sheet = work_book.Worksheets.Add("Sheet 1");
            string export_path = context.Server.MapPath("~/exports/data/");
            int counter = 0;

            var title_styles = work_book.Style;

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
            title_styles.Font.FontSize = 14;

            // Manually adding the column names, as the data adding functions don't display the column names.
            for (counter = 0; counter < column_names.Length; counter++)
            {
                work_sheet.Cell(4, counter + 1).Value = column_names[counter];
                work_sheet.Cell(4, counter + 1).Style.Font.Bold = true;
                work_sheet.Cell(4, counter + 1).Style.Font.FontSize = 16;
            }

            // Adding the datatable to the selected range
            work_sheet.Cell(5, 1).Value = data.AsEnumerable();

            // Save the workbook to the below path
            work_book.SaveAs(export_path + file_name);
        }

        public static void ExportDataToExcel(string file_name, string report_title, DataTable data, HttpContext context, string[] column_names, string company_name)
        {
            string imagePath = context.Server.MapPath("~/uploads/CompanyLogo/");
            DBConnection db_connection = new DBConnection();
            DataTable leave_status = new DataTable();
            string query = string.Empty;
            string status_description = string.Empty;
            var work_book = new XLWorkbook();
            var work_sheet = work_book.Worksheets.Add("Sheet 1");
            var dataForExport = new List<string[]>();
            string export_path = context.Server.MapPath("~/exports/data/");
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

            // description_range = work_sheet.Range(10 + data_count, 1, 10 + data_count, 24);

            work_sheet.Range(9 + data_count, 1, 9 + data_count, 24).Merge();
            work_sheet.Range(9 + data_count, 1, 9 + data_count, 24).Style = description_styles;

            work_sheet.Cell(11 + data_count, 1).Value = leaveStatus;
            work_sheet.Range(11 + data_count, 1, 11 + data_count, 21).Style = description_styles;
            work_sheet.Range(11 + data_count, 1, 11 + data_count, 15).Merge();
            work_book.SaveAs(export_path + file_name);

            CompanyLogoStuff companyStuff = new CompanyLogoStuff();
            imagePath = imagePath + companyStuff.getCompanyImageUrl(company_name);

            string fullPath = export_path + file_name;
           // createLogo(fullPath, imagePath);
        }
        
        public static void ExportDataToExcelWithLogo(string importFile, string report_title, DataTable data, HttpContext context, string[] column_names, string company_name, string exportPath)
        {
            string imagePath = context.Server.MapPath("~/uploads/CompanyLogo/");
            DBConnection db_connection = new DBConnection();
            DataTable leave_status = new DataTable();
            string query = string.Empty;
            string status_description = string.Empty;
            var work_book = new XLWorkbook(importFile);
            var work_sheet = work_book.Worksheet("Sheet 1");
            var dataForExport = new List<string[]>();
            string export_path = context.Server.MapPath("~/exports/data/");
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

          
        }
    }
}