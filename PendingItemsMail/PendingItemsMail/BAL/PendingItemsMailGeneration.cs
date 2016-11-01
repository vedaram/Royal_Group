using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;

namespace PendingItemsMail
{
    class PendingItemsMailGeneration
    {
        #region global declarations
        Hashtable hashtable1 = new Hashtable();
        public static SqlConnection con;

        string directoryname = string.Empty;
        IniFile f = new IniFile(string.Format("{0}\\Securtime.ini", Application.StartupPath));
        string Location, encrypt, managerurl = string.Empty;
        #endregion

        public void GeneratePendingManualdata()
        {
            string FileExtension = "EXCEL";

            string server = f.IniReadValue("PendingRecords", "Path");
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
                DataTable manual_entry_pending_data = new DataTable();
                DataTable overtime_pending_data = new DataTable();
                DataTable out_of_office = new DataTable();

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
                            string query, file_name, company_name, exportPath = string.Empty;
                            string messageBody = string.Empty;
                            company_name = db_connection.ExecuteQuery_WithReturnValueString("select companyname from companymaster where companycode in ( select emp_company from employeemaster where emp_code  ='" + managerorhrcode + "')");

                            #region Manual Punch
                            query = "select p.empcode as EmpCode  , p.workdate as WorkDate , p.inpunch as InPunch , p.outpunch as OutPunch , md.status as Status  from punchforapproval p  join MASTERPROCESSDAILYDATA md on  p.WorkDate = md.PDate and p.EmpCode = md.emp_id  where p.approve = 1 and p.empcode in  (select EmpID from [FetchEmployees] ('" + managerorhrcode + "',''))";
                            manual_entry_pending_data = db_connection.ReturnDataTable(query);
                            if (manual_entry_pending_data.Rows.Count > 0)
                            {
                                messageBody += "Dear " + managerorhrname + ",<br/>" + "<br/>";
                                messageBody += "Please find pending approval records for your reportee employee. " + "<br/>" + "<br/>";
                                OpenFileForWriting("Your Pending Manual Entry Approval :    " + DateTime.Now.ToString("dd-MMM-yyyy"));
                                OpenFileForWriting("Employee ID" + "     " + "Date" + "     " + "In Punch" + "    " + "Out Punch" + "     " + "Status" + "    " + "Access Link");
                                messageBody += "Pending Manual Entry Approval List as on : " + DateTime.Now.ToString("dd-MMM-yyyy");
                                messageBody += "<br><br>";
                                messageBody += "<table border=1>";
                                messageBody += "<tr><td>Employee ID</td>";
                                messageBody += "<td>Date</td>";
                                messageBody += "<td>In Punch</td>";
                                messageBody += "<td>Out Punch</td>";
                                messageBody += "<td>Status</td>";
                                // messageBody += "<td>Access Link</td>";
                                messageBody += "</tr>";

                                foreach (DataRow pendingmanualrow in manual_entry_pending_data.Rows)
                                {
                                    Location = f.IniReadValue("Location", "value");
                                    encrypt = SSTCryptographer.Encrypt(managerorhrcode, "SampleKey");
                                    managerurl = "?MP=" + encrypt + "";
                                    Location = Location + managerurl;
                                    Location = ConvertUrlsToLinks(Location);

                                    messageBody += "<tr>";
                                    messageBody += "<td>" + pendingmanualrow["EmpCode"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingmanualrow["WorkDate"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingmanualrow["InPunch"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingmanualrow["OutPunch"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingmanualrow["Status"].ToString() + "</td>";
                                    // messageBody += "<td>" + Location + "</td>";
                                    messageBody += "</tr>";

                                    OpenFileForWriting(pendingmanualrow["EmpCode"].ToString() + "   " + pendingmanualrow["WorkDate"].ToString() + "   " + pendingmanualrow["InPunch"].ToString() + " " + pendingmanualrow["OutPunch"].ToString() + " " + pendingmanualrow["Status"].ToString() + " '" + Location + "'");
                                }

                                messageBody += "</table></br>";
                                messageBody += "<br><br>";

                                messageBody += Environment.NewLine + " " + Environment.NewLine;
                                messageBody += Environment.NewLine + " " + Environment.NewLine;
                            }
                            else
                            {
                                ErrorLog.ErrorLogfile(@"" + directoryname + " ", "No pending manual entry record found for employee under Manager/HR ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                            }
                            #endregion

                            #region Overtime
                            query = "select ot.EMPID as EmpCode  , ot.OTDate as WorkDate , ot.OTHrs as OTHours ,  md.status as Status  from overtime  ot  join MASTERPROCESSDAILYDATA md on  ot.otdate = md.PDate and ot.EMPID = md.emp_id  where ot.Flag = 1 and ot.EMPID in  (select EmpID from [FetchEmployees] ('" + managerorhrcode + "',''))";
                            overtime_pending_data = db_connection.ReturnDataTable(query);
                            if (overtime_pending_data.Rows.Count > 0)
                            {
                                OpenFileForWriting("Your pending OT approval :    " + DateTime.Now.ToString("dd-MMM-yyyy"));
                                OpenFileForWriting("Employee ID" + "     " + "Date" + "     " + "OTHours" + "   " + "Status" + "Access Link");
                                messageBody += "Pending OT Entry Approval List as on : " + DateTime.Now.ToString("dd-MMM-yyyy");
                                messageBody += "<br><br>";
                                messageBody += "<tr>";
                                messageBody += "<table border=1>";
                                messageBody += "<tr><td>Employee ID</td>";
                                messageBody += "<td>Date</td>";
                                messageBody += "<td>OTHours</td>";
                                messageBody += "<td>Status</td>";
                                // messageBody += "<td>Access Link</td>";
                                messageBody += "</tr>";

                                foreach (DataRow pendingotrow in overtime_pending_data.Rows)
                                {
                                    Location = f.IniReadValue("Location", "value");
                                    encrypt = SSTCryptographer.Encrypt(managerorhrcode, "SampleKey");
                                    managerurl = "?MP=" + encrypt + "";
                                    Location = Location + managerurl;
                                    Location = ConvertUrlsToLinks(Location);
                                    messageBody += "<tr>";
                                    messageBody += "<td>" + pendingotrow["EmpCode"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingotrow["WorkDate"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingotrow["OTHours"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingotrow["Status"].ToString() + "</td>";
                                    // messageBody += "<td>" + Location + "</td>";
                                    messageBody += "</tr>";
                                    messageBody += "</tr>";

                                    OpenFileForWriting(pendingotrow["EmpCode"].ToString() + "   " + pendingotrow["WorkDate"].ToString() + "   " + pendingotrow["OTHours"].ToString() + "  " + pendingotrow["Status"].ToString() + " '" + Location + "'");

                                }

                                messageBody += "</table></br>";
                                messageBody += "<br><br>";

                                messageBody += Environment.NewLine + " " + Environment.NewLine;
                                messageBody += Environment.NewLine + " " + Environment.NewLine;
                            }
                            else
                            {
                                ErrorLog.ErrorLogfile(@"" + directoryname + " ", "No pending OT record found for employee under Manager/HR ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                            }

                            #endregion

                            #region OUTOFFOFFICE*/

                            query = string.Empty;
                            if (ismanager == "1")
                            {
                                query = @" select Emp_ID, Emp_Name,OOO_type_name,FromDateTime,ToDateTime from outoffoffice O join OOOType OT on O.OOO_type=OT.OOO_type_id where Manager_Status=1  and 
	                                    Emp_ID in (select EmpID from [FetchEmployees] ('" + managerorhrcode + "',''))";
                            }
                            if (ishr == "1")
                            {
                                query = @" select Emp_ID,Emp_Name,OOO_type_name,FromDateTime,ToDateTime from outoffoffice O join OOOType OT on O.OOO_type=OT.OOO_type_id where HR_Status=1 and
                                        Emp_ID in (select EmpID from [FetchEmployees] ('" + managerorhrcode + "',''))";
                            }

                            query += " and Emp_ID!='" + managerorhrcode + "'";
                            out_of_office = db_connection.ReturnDataTable(query);

                            if (out_of_office.Rows.Count > 0)
                            {
                                messageBody += "Dear " + managerorhrname + ",<br/>" + "<br/>";
                                messageBody += "Please find pending approval records for your reportee employee. " + "<br/>" + "<br/>";
                                OpenFileForWriting("Your Pending OUT OF OFFICE Approval :    " + DateTime.Now.ToString("dd-MMM-yyyy"));
                                OpenFileForWriting("Employee ID" + "     " + "Employee Name" + "     " + "OUT OF OFFICE Type" + "     " + "From DateTime" + "    " + "To DateTime");
                                messageBody += "Pending OUT OF OFFICE Approval List as on : " + DateTime.Now.ToString("dd-MMM-yyyy");
                                messageBody += "<br><br>";
                                messageBody += "<table border=1>";
                                messageBody += "<tr><td>Employee ID</td>";
                                messageBody += "<td>Employee Name</td>";
                                messageBody += "<td>OUT OF OFFICE Type</td>";
                                messageBody += "<td>From DateTime</td>";
                                messageBody += "<td>To DateTime</td>";
                                messageBody += "</tr>";

                                foreach (DataRow pendingooorow in out_of_office.Rows)
                                {
                                    Location = f.IniReadValue("Location", "value");
                                    encrypt = SSTCryptographer.Encrypt(managerorhrcode, "SampleKey");
                                    managerurl = "?MP=" + encrypt + "";
                                    Location = Location + managerurl;
                                    Location = ConvertUrlsToLinks(Location);

                                    messageBody += "<tr>";
                                    messageBody += "<td>" + pendingooorow["Emp_ID"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingooorow["Emp_Name"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingooorow["OOO_type_name"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingooorow["FromDateTime"].ToString() + "</td>";
                                    messageBody += "<td>" + pendingooorow["ToDateTime"].ToString() + "</td>";
                                    messageBody += "</tr>";

                                    OpenFileForWriting(pendingooorow["Emp_ID"].ToString() + "   " + pendingooorow["Emp_Name"].ToString() + "   " + pendingooorow["OOO_type_name"].ToString() + " " + pendingooorow["FromDateTime"].ToString() + " " + pendingooorow["ToDateTime"].ToString());
                                }

                                messageBody += "</table></br>";
                                messageBody += "<br><br>";

                                messageBody += Environment.NewLine + " " + Environment.NewLine;
                                messageBody += Environment.NewLine + " " + Environment.NewLine;
                            }
                            else
                            {
                                ErrorLog.ErrorLogfile(@"" + directoryname + " ", "No pending OUT OF OFFICE record found for employee under Manager/HR ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                            }
                            #endregion

                            if (overtime_pending_data.Rows.Count > 0 || manual_entry_pending_data.Rows.Count > 0 || out_of_office.Rows.Count > 0)
                            {
                                messageBody += "<br /><br /> Thanks and Regards.<br /> <b>";
                                messageBody += "<br /><br /> Please do not reply on this mail, because it is system generated mail!<br /> <b>";
                                mailsendflag = MailSender.SendEmail(managerorhremail, "", messageBody, "", managerorhrname, directoryname);
                                if (mailsendflag)
                                {
                                    ErrorLog.ErrorLogfile(@"" + directoryname + " ", "Mail Sending done successfull for Manager email ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                                }
                                else
                                {
                                    ErrorLog.ErrorLogfile(@"" + directoryname + " ", "Mail Sending failed for Manager email ID :'" + managerorhrcode + "' and name '" + managerorhrname + "'", "");
                                }
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
        private string ConvertUrlsToLinks(string msg)
        {
            string output = msg;
            System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*([a-zA-Z0-9\\?\\#\\=\\/]){1})?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            System.Text.RegularExpressions.MatchCollection mactches = regx.Matches(output);

            foreach (System.Text.RegularExpressions.Match match in mactches)
            {
                output = output.Replace(match.Value, "<a href='" + match.Value + "' target='blank'>" + match.Value + "</a>");
            }
            return output;
        }
        public void OpenFileForWriting(string data)
        {
            string filepath, mailFlag = string.Empty;
            IniFile fi = new IniFile(string.Format("{0}\\Securtime.ini", Application.StartupPath));
            filepath = fi.IniReadValue("PendingRecords", "Path");
            mailFlag = "Y";// fi.IniReadValue("MailFlag", "flag");

            if (mailFlag.ToUpper() == "Y")
            {
                FileStream fs = new FileStream(filepath + "PendingRecords_" + DateTime.Now.ToString("ddMMyyyy") + ".txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(data);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }
    }
}
