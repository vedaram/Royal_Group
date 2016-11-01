using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Globalization;
using SecurTime;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SecurAX.Logger;
using System.Configuration;
using SecurAX.Import.Excel;
using SecurAX.Export.Excel;

public partial class employee_data_transaction : System.Web.UI.Page
{

    const string page = "EMPLOYEE_DATA_TRANSACTION";
    static string errormessage = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string message = string.Empty;

        try
        {
            //GetEmployeeTransactionData("10", "", "");
            if (Session["username"] == null)
            {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex)
        {
            // log the exception
            Logger.LogException(ex, page, "PAGE_LOAD");

            message = "An error occurred while loading Leave Approval page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetShiftData(int page_number, bool is_filter, string filters)
    {
        employee_data_transaction page_object = new employee_data_transaction();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable Shift_data = new DataTable();
        DataTable branch_list_data = new DataTable();
        DataTable CoManagerID_data = new DataTable();


        string user_name = string.Empty,
                employee_id = string.Empty,
                query = string.Empty,
                CoManagerID = string.Empty,
                BranchList = string.Empty;

        int user_access_level = 0,
            start_row = 0,
            number_of_record = 0,
            IsDelegationManager = 0;

        try
        {
            user_name = HttpContext.Current.Session["username"].ToString();
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

            start_row = (page_number - 1) * 30;
            number_of_record = page_number * 30 + 1;

            //check employee is Delegation Manager or not if so get his CoManagerID
            IsDelegationManager = Convert.ToInt32(db_connection.ExecuteQuery_WithReturnValueString("Select COUNT(DelidationManagerID) from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)"));
            if (IsDelegationManager > 0)
            {
                query = "Select ManagerId from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate)";
                CoManagerID_data = db_connection.ReturnDataTable(query);
                if (CoManagerID_data.Rows.Count > 0)
                {
                    foreach (DataRow dr in CoManagerID_data.Rows)
                    {
                        CoManagerID += "'" + dr["ManagerId"] + "',";
                    }

                    CoManagerID = CoManagerID.TrimEnd(',');
                }
            }

            //get list of branches assigned to logged in manager hr
            BranchList = "'Empty',";
            query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + employee_id + "'";
            branch_list_data = db_connection.ReturnDataTable(query);
            query = string.Empty;

            query = page_object.GetShiftDataBaseQuery(); //read main query

            //if (branch_list_data.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in branch_list_data.Rows)
            //    {
            //        BranchList += "'" + dr["BranchCode"] + "',";
            //    }

            //}
            //BranchList = BranchList.TrimEnd(',');

            ////Validate CoManagerID
            //if (string.IsNullOrEmpty(CoManagerID))
            //{
            //    CoManagerID = "'Empty'";
            //}

            ////modify query as per access level
            //if (user_access_level == 0)//Admin
            //{
            //    query += "  ";
            //}
            //else if (user_access_level == 3)//HR
            //{
            //    query += " and et.EmployeeBranch In(" + BranchList + ") ";
            //}
            //else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
            //{
            //    query += " and et.EmployeeCode In(Select Emp_Code From Employeemaster  Where Emp_Code In('" + employee_id + "') OR ManagerID In('" + employee_id + "'," + CoManagerID + ")) Or et.EmployeeBranch In(" + BranchList + ") ";
            //}
            //else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager
            //{
            //    query += " and et.EmployeeBranch In(" + BranchList + ") or et.EmployeeCode in ( select distinct(Emp_Code) from EmployeeMaster where (managerId='" + employee_id + "' or Emp_Code='" + employee_id + "')  and Emp_Status=1 ) ";
            //}
            //else
            //{
            //    query += " and et.EmployeeCode='" + employee_id + "'";// Only Employee
            //}

            //if (!is_filter)
            //{
            //    //query += " and l.flag=1 ";
            //    query += " ) a where row > " + start_row + " and row < " + number_of_record;
            //}

            //if (is_filter)
            //{
            //    query += " ) a ";
            //    // query = page_object.GetShiftDataBaseQuery(filters, query);
            //    query = query + "order by a.emp_id OFFSET " + start_row + " ROWS FETCH NEXT " + number_of_record + " ROWS ONLY ";
            //}

            // query += " order by a.Leave_id desc";

            Shift_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(Shift_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_SHIFT_DATES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }
    [WebMethod]
    public static ReturnObject DoExport()
    {
        employee_data_transaction page_object = new employee_data_transaction();
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

                file_name = page_object.CreateExport(employee_transaction_data);

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
    private string CreateExport(DataTable employee_transaction_data)
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
    private string GetShiftDataBaseQuery()
    {
        string query = string.Empty;
        query = @"select et.TransactionID , et.EmployeeCode , et.employeename ,   c.CompanyName , b.branchname  , et.shiftfromdate 
                    , et.shifttodate , s.Shift_desc , et.Shift_Eligibility , e.managerid   from Employee_TransactionData et left join CompanyMaster  c
                on  et.employeecompany = c.companycode left join branchmaster b on et.employeebranch = b.branchcode 
                left join shift s on et.shift_name =  s.shift_code left join employeemaster e on et.employeecode = e.emp_code   ";

        return query;
    }
    private int CheckEmployeeCode(string employee_code)
    {
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from EmployeeMaster where Emp_Code = '" + employee_code + "' ";
        count = db_connection.GetRecordCount(query);

        return count;
    }
    [WebMethod]
    public static ReturnObject saveEmployeeTransactionData(string returndataset)
    {

        employee_data_transaction page_object = new employee_data_transaction();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        Hashtable prepared_data = new Hashtable();
        string result = string.Empty;
        bool recordsaveresult = false;
        JObject current_data = new JObject();
        string enroll_id = string.Empty;
        string employee_code = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {
                int transactionid = 0;
                int transactiontype = 0;
                string from_date, to_date, transactiondata, query, statusflag = string.Empty;
                DataSet return_data_set = (DataSet)JsonConvert.DeserializeObject(returndataset, (typeof(DataSet)));
                DataTable employeedata = return_data_set.Tables["employeedata"];
                


                //DataTable shifttable = return_data_set.Tables["shift"];
                //DataTable ottable = return_data_set.Tables["ot"];
                //DataTable ramadantable = return_data_set.Tables["ramadan"];
                //DataTable punchexceptiontable = return_data_set.Tables["punchexception"];
                //DataTable workhoursperday = return_data_set.Tables["workhourperday"];
                //DataTable workhousperweek = return_data_set.Tables["workhourperweek"];
                //DataTable workhourspermonth = return_data_set.Tables["workhourpermonth"];
                //DataTable maternity = return_data_set.Tables["maternity"];
                //DataTable termination = return_data_set.Tables["termination"];
                //DataTable manager = return_data_set.Tables["manager"];


             //   if (page_object.CheckEmployeeCode(employee_code) > 0)
                if ( employeedata.Rows.Count > 0)
                {
                    employee_code = employeedata.Rows[0][0].ToString();
                    DataTable ResultDataset = new DataTable();
                    for (int i = 1; i < return_data_set.Tables.Count; i++)
                    {
                        ResultDataset = return_data_set.Tables[i];
                        if (ResultDataset.TableName == "shiftData")
                        {
                            //ResultDataset.Columns.Add("shift_desc", typeof(System.Char));
                            if (ResultDataset.Columns.Contains("shift_desc"))
                            {
                                ResultDataset.Columns.Remove("shift_desc");
                            }
                            // if ( ResultDataset["StatusFlag"] == "I")

                            //ResultDataset.Columns.Remove("shift_desc");
                            //foreach (DataRow row in ResultDataset.Rows)
                            //{
                            //    string status = row["StatusFlag"].ToString();
                            //    if (status != "I")
                            //    {
                            //        ResultDataset.Columns.Remove("shift_desc");

                            //    }
                            //}
                           
                        }
                        if (ResultDataset.Rows.Count > 0)
                        {
                            db_connection.ExecuteSPWithDatatable(ResultDataset, employee_code, "test_employeetransaction");
                        }
                    }
                }
                else
                {
                    return_object.status = "error";
                    return_object.return_data = "This employee code does not exists in Employee Master. Please try again with a different Employee Code";

                }

                return_object.status = "success";
                return_object.return_data = "Employee Transaction data saved.";

                //db_connection.ExecuteSPWithDatatable(shifttable, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(ottable, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(ramadantable, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(punchexceptiontable, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(workhoursperday, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(workhousperweek, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(workhourspermonth, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(maternity, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(termination, employee_code, "test_employeetransaction");
                //db_connection.ExecuteSPWithDatatable(manager, employee_code, "test_employeetransaction");

                /*  foreach (DataRow row in shifttable.Rows)
                  {
                      transactionid = Convert.ToInt32(row["id"]);
                      transactiontype = Convert.ToInt32(row["transactiontype"]);

                      from_date = row["fromdate"].ToString();
                      to_date = row["todate"].ToString();
                      transactiondata = row["transactiondata"].ToString();
                      statusflag = row["StatusFlag"].ToString();

                      if (statusflag == "U")
                      {
                          query = "update EmployeeTransactionData set fromdate='" + from_date + "',todate='" + to_date + "', transactiondata='" + transactiondata + "' where id=" + transactionid;
                          db_connection.ExecuteQuery_WithOutReturnValue(query);
                      }
                      if (statusflag == "N")
                      {
                          DataTable temp_data = new DataTable();
                          query = "select id from EmployeeTransactionData where  id = '" + transactionid + "'";
                          temp_data = db_connection.ReturnDataTable(query);
                          if (temp_data.Rows.Count > 0)
                          {
                              query = "update EmployeeTransactionData set fromdate='" + from_date + "',todate='" + to_date + "', transactiondata='" + transactiondata + "' where id=" + transactionid;
                              db_connection.ExecuteQuery_WithOutReturnValue(query);
                          }
                          else
                          {
                              query = @"insert into EmployeeTransactionData(Empid,transactiontype,fromdate , todate , transactiondata) 
                                      values ('','" + transactiontype + "','" + from_date + "','" + to_date + "','" + transactiondata + "')";
                              db_connection.ExecuteQuery_WithOutReturnValue(query);
                          }
                      }

                  } */



            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "ADD_EMPLOYEE");

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;

    }

    [WebMethod]
    public static ReturnObject addEmployee(string current)
    {
        // current = "
        employee_data_transaction page_object = new employee_data_transaction();
        ReturnObject return_object = new ReturnObject();
        Hashtable prepared_data = new Hashtable();
        string result = string.Empty;
        bool recordsaveresult = false;
        JObject current_data = new JObject();
        string enroll_id = string.Empty;
        string employee_code = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {
                //current_data = JObject.Parse(current);
                //employee_code = current_data["employee_data"].ToString();

                //if (page_object.CheckEmployeeCode(employee_code) > 0)
               return_object =  employee_data_transaction.saveEmployeeTransactionData(current);
                /*recordsaveresult = page_object.prepareData(current, "I");
                page_object.UpdateDatabase(prepared_data);
                if (recordsaveresult)
                {
                    return_object.status = "success";
                    return_object.return_data = "Employee transaction Record added successfully!";
                }
                else
                {
                    return_object.status = "error";
                    return_object.return_data = errormessage;
                    errormessage = "";
                }*/

                //}
                //else
                //{
                //    return_object.status = "error";
                //    return_object.return_data = "This employee code does not exists in Employee Master. Please try again with a different Employee Code";

                //}

            }

            catch (Exception ex)
            {
                Logger.LogException(ex, page, "ADD_EMPLOYEE");

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;


    }
    protected bool prepareData(string current, string mode)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable employee_data = new Hashtable();
        ReturnObject return_object = new ReturnObject();

        DataTable employee_transaction_data = new DataTable();
        DataTable dt_temp = new DataTable();
        DateTime maternity_to_date, child_date_of_birth = Convert.ToDateTime("1900-01-01 00:00:00.000");
        string[] employee_code_array = null, shift_code_array = null;

        bool update_flag = false;
        int employee_count = 0, row_number = 2, AccessLevel = 0, is_manager = 0, LoginAccessLevel = 0, TransactionID = 0;



        string upload_path = string.Empty, ExcelFullPath = string.Empty, query = string.Empty, update_query = string.Empty, employee_gender = string.Empty,
                current_user = string.Empty, EmployeeCode = string.Empty, EmployeeName = string.Empty,
                EmployeeCompany = string.Empty, EmployeeBranch = string.Empty, EmployeeDepartment = string.Empty, EmployeeCategory = string.Empty,
                ShiftFromDate = string.Empty, ShiftToDate = string.Empty, Shift_Code = string.Empty, return_message = string.Empty,
                OTFromdate = string.Empty, OTTodate = string.Empty, RamadanFromdate = string.Empty, RamadanTodate = string.Empty,
                PunchExceptionFromdate = string.Empty, PunchExceptionTodate = string.Empty,
                ChildDateofBirth = string.Empty, MaternityFromDate = string.Empty, MaternityToDate = string.Empty,
                WorkHourPerdayFromdate = string.Empty, WorkHourPerdayTodate = string.Empty, WorkHourPerday = string.Empty,
                WorkHourPerWeekFromdate = string.Empty, WorkHourPerWeekTodate = string.Empty, WorkHourPerWeek = string.Empty,
                WorkHourPerMonthFromdate = string.Empty, WorkHourPerMonthTodate = string.Empty, WorkHourPerMonth = string.Empty,
                LineManagerFromdate = string.Empty, LineManagerTodate = string.Empty, LineManagerID = string.Empty, Terminationdate = string.Empty;

        JObject current_data = JObject.Parse(current);

        byte[] image = new byte[0];
        int

            is_shift_eligible = 0, is_ot_eligible = 0, is_ramadan_eligible = 0, is_punch_exception = 0, is_maternity_eligible = 0,
            is_workhour_perday_eligible = 0, is_workhour_perweek_eligible = 0, is_workhour_permonth_eligible = 0, is_termination_eligible = 0,
            is_line_manager_eligible = 0;
        string shift_name = string.Empty;
        EmployeeCode = current_data["employee_data"].ToString();
        // shift_name = current_data["shift_select_data"].ToString();

        query = "select Emp_Code,Emp_Company,Emp_Name,Emp_Company,Emp_Branch,Emp_Department,Emp_Employee_Category from EmployeeMaster where Emp_Code='" + EmployeeCode + "'";
        dt_temp = db_connection.ReturnDataTable(query);

        EmployeeCompany = dt_temp.Rows[0]["Emp_Company"].ToString();
        EmployeeName = dt_temp.Rows[0]["Emp_Name"].ToString();
        EmployeeBranch = dt_temp.Rows[0]["Emp_Branch"].ToString();
        EmployeeCategory = dt_temp.Rows[0]["Emp_Employee_Category"].ToString();
        EmployeeDepartment = dt_temp.Rows[0]["Emp_Department"].ToString();

        query = @"insert into employee_transactiondata(EmployeeCode,EmployeeCompany,EmployeeName,EmployeeBranch,EmployeeDepartment,EmployeeCategory ) 
                                    values ('" + EmployeeCode + "','" + EmployeeCompany + "','" + EmployeeName + "','" + EmployeeBranch + "','" + EmployeeDepartment + "','" + EmployeeCategory + "')";
        db_connection.ExecuteQuery_WithOutReturnValue(query);

        query = "Select max(TransactionID) from employee_transactiondata where EmployeeCode='" + EmployeeCode + "' and EmployeeCompany='" + EmployeeCompany + "'";
        TransactionID = db_connection.ExecuteQuery_WithReturnValueInteger(query);
        //  Changes for employee transaction 
        // SHift validation
        if (Convert.ToBoolean(current_data["shift_date_chkbox"]))
        {
            ShiftFromDate = current_data["shift_From_date"].ToString();
            ShiftToDate = current_data["shift_To_date"].ToString();

            query = "select convert(date,Shiftfromdate) as Shiftfromdate, convert(date,ShiftTodate) as ShiftTodate from employee_transactiondata where ((Shiftfromdate  between '" + ShiftFromDate + "' and '" + ShiftToDate + "') or (ShiftTodate between '" + ShiftFromDate + "' and '" + ShiftToDate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                ShiftFromDate = Convert.ToDateTime(dt_temp.Rows[0]["ShiftFromDate"].ToString()).ToString("dd-MMM-yyyy");
                ShiftToDate = Convert.ToDateTime(dt_temp.Rows[0]["ShiftToDate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Shift already exists for selected dates";
                }

            }
            else
            {
                is_shift_eligible = 1;
                query = "update employee_transactiondata set Shift_Eligibility=" + is_shift_eligible + ",Shift_code='" + shift_name + "', ShiftFromDate='" + ShiftFromDate + "', ShiftToDate='" + ShiftToDate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;

            }
        }
        // OT validation
        if (Convert.ToBoolean(current_data["OT_date"]))
        {
            OTFromdate = current_data["OT_from_date"].ToString();
            OTTodate = current_data["OT_To_date"].ToString();
            query = "select convert(date,OTFromdate) as OTFromdate, convert(date,OTTodate) as OTTodate from employee_transactiondata where ((OTFromdate  between '" + OTFromdate + "' and '" + OTTodate + "') or (OTTodate between '" + OTFromdate + "' and '" + OTTodate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                OTFromdate = Convert.ToDateTime(dt_temp.Rows[0]["OTFromdate"].ToString()).ToString("dd-MMM-yyyy");
                OTTodate = Convert.ToDateTime(dt_temp.Rows[0]["OTTodate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "OT already exists for selected dates";
                }
            }
            else
            {
                is_ot_eligible = 1;
                query = "update employee_transactiondata set OT_Eligibility=" + is_ot_eligible + ", OTFromdate='" + OTFromdate + "', OTTodate='" + OTTodate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;

            }
        }
        // Ramadan validation
        if (Convert.ToBoolean(current_data["ramadan_date"]))
        {
            RamadanFromdate = current_data["ramadan_from_date"].ToString();
            RamadanTodate = current_data["ramadan_to_date"].ToString();
            query = "select convert(date,RamadanFromdate) as RamadanFromdate, convert(date,RamadanTodate) as RamadanTodate from employee_transactiondata where ((RamadanFromdate  between '" + RamadanFromdate + "' and '" + RamadanTodate + "') or (RamadanTodate between '" + RamadanFromdate + "' and '" + RamadanTodate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                RamadanFromdate = Convert.ToDateTime(dt_temp.Rows[0]["RamadanFromdate"].ToString()).ToString("dd-MMM-yyyy");
                RamadanTodate = Convert.ToDateTime(dt_temp.Rows[0]["RamadanTodate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                return_message += Environment.NewLine + "Ramadan data exists from " + RamadanFromdate + " to " + RamadanTodate + " for row " + row_number;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Ramadan already exists for selected dates";
                }
            }
            else
            {
                is_ramadan_eligible = 1;
                query = "update employee_transactiondata set Ramadan_Eligibility=" + is_ramadan_eligible + ", RamadanFromdate='" + RamadanFromdate + "', RamadanTodate='" + RamadanTodate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;
            }
        }
        // Punch validation
        if (Convert.ToBoolean(current_data["punch_date"]))
        {
            PunchExceptionFromdate = current_data["punch_from_date"].ToString();
            PunchExceptionTodate = current_data["punch_to_date"].ToString();
            query = "select convert(date,PunchExceptionFromdate) as PunchExceptionFromdate, convert(date,PunchExceptionTodate) as PunchExceptionTodate from employee_transactiondata where ((PunchExceptionFromdate  between '" + PunchExceptionFromdate + "' and '" + PunchExceptionTodate + "') or (PunchExceptionTodate between '" + PunchExceptionFromdate + "' and '" + PunchExceptionTodate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                PunchExceptionFromdate = Convert.ToDateTime(dt_temp.Rows[0]["PunchExceptionFromdate"].ToString()).ToString("dd-MMM-yyyy");
                PunchExceptionTodate = Convert.ToDateTime(dt_temp.Rows[0]["PunchExceptionTodate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                return_message += Environment.NewLine + "PunchException data exists from " + PunchExceptionFromdate + " to " + PunchExceptionTodate + " for row " + row_number;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Punch Exception already exists for selected dates";
                }
            }
            else
            {
                is_punch_exception = 1;
                query = "update employee_transactiondata set PunchException_Eligibility=" + is_punch_exception + ", PunchExceptionFromdate='" + PunchExceptionFromdate + "', PunchExceptionTodate='" + PunchExceptionTodate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;
            }
        }

        /*Maternity*/
        if (Convert.ToBoolean(current_data["maternity_date"]))
        {
            ChildDateofBirth = current_data["child_date_of_birth"].ToString();
            query = "select convert(date,MaternityFromDate) as MaternityFromDate, convert(date,MaternityToDate) as MaternityToDate from employee_transactiondata where ((MaternityFromDate  between '" + MaternityFromDate + "' and '" + MaternityToDate + "') or (MaternityToDate between '" + MaternityFromDate + "' and '" + MaternityToDate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                MaternityFromDate = Convert.ToDateTime(dt_temp.Rows[0]["MaternityFromDate"].ToString()).ToString("dd-MMM-yyyy");
                MaternityToDate = Convert.ToDateTime(dt_temp.Rows[0]["MaternityToDate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                return_message += Environment.NewLine + "Maternity data exists from " + MaternityFromDate + " to " + MaternityToDate + " for row " + row_number;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Maternity already exists for selected dates";
                }
            }
            else
            {
                is_maternity_eligible = 1;
                child_date_of_birth = DateTime.ParseExact(current_data["child_date_of_birth"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                maternity_to_date = child_date_of_birth.AddMonths(18);
                query = "update employee_transactiondata set ChildDateofBirth='" + ChildDateofBirth + "',MaternityBreakHours= '01:00:00.0000000' ,  Maternity_Eligibility=" + is_maternity_eligible + ", MaternityFromDate='" + ChildDateofBirth + "', MaternityToDate='" + maternity_to_date + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;

            }
        }

        /*WorkHourPerday*/
        if (Convert.ToBoolean(current_data["WH_day_date"]))
        {
            WorkHourPerdayFromdate = current_data["WH_day_from_date"].ToString();
            WorkHourPerdayTodate = current_data["WH_day_to_date"].ToString();
            WorkHourPerday = current_data["WH_day_drop_drown"].ToString();

            query = "select convert(date,WorkHourPerdayFromdate) as WorkHourPerdayFromdate, convert(date,WorkHourPerdayTodate) as WorkHourPerdayTodate from employee_transactiondata where ((WorkHourPerdayFromdate  between '" + WorkHourPerdayFromdate + "' and '" + WorkHourPerdayTodate + "') or (WorkHourPerdayTodate between '" + WorkHourPerdayFromdate + "' and '" + WorkHourPerdayTodate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                WorkHourPerdayFromdate = Convert.ToDateTime(dt_temp.Rows[0]["WorkHourPerdayFromdate"].ToString()).ToString("dd-MMM-yyyy");
                WorkHourPerdayTodate = Convert.ToDateTime(dt_temp.Rows[0]["WorkHourPerdayTodate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                return_message += Environment.NewLine + "WorkHourPerday data exists from " + WorkHourPerdayFromdate + " to " + WorkHourPerdayTodate + " for row " + row_number;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Work hoours per day already exists for selected dates";
                }
            }
            else
            {
                is_workhour_perday_eligible = 1;
                query = "update employee_transactiondata set WorkHourPerday='" + WorkHourPerday + "' ,  WorkHourPerday_Eligibility=" + is_workhour_perday_eligible + ", WorkHourPerdayFromdate='" + WorkHourPerdayFromdate + "', WorkHourPerdayTodate='" + WorkHourPerdayTodate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;

            }
        }

        /*WorkHourPerWeek*/
        if (Convert.ToBoolean(current_data["WH_week_date"]))
        {
            WorkHourPerWeekFromdate = current_data["WH_week_from_date"].ToString();
            WorkHourPerWeekTodate = current_data["WH_week_to_date"].ToString();
            WorkHourPerWeek = current_data["WH_week_drop_down"].ToString();

            query = "select convert(date,WorkHourPerWeekFromdate) as WorkHourPerWeekFromdate, convert(date,WorkHourPerWeekTodate) as WorkHourPerWeekTodate from employee_transactiondata where ((WorkHourPerWeekFromdate  between '" + WorkHourPerWeekFromdate + "' and '" + WorkHourPerWeekTodate + "') or (WorkHourPerWeekTodate between '" + WorkHourPerWeekFromdate + "' and '" + WorkHourPerWeekTodate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                WorkHourPerWeekFromdate = Convert.ToDateTime(dt_temp.Rows[0]["WorkHourPerWeekFromdate"].ToString()).ToString("dd-MMM-yyyy");
                WorkHourPerWeekTodate = Convert.ToDateTime(dt_temp.Rows[0]["WorkHourPerWeekTodate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                return_message += Environment.NewLine + "WorkHourPerWeek data exists from " + WorkHourPerWeekFromdate + " to " + WorkHourPerWeekTodate + " for row " + row_number;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Work hoours per week already exists for selected dates";
                }
            }
            else
            {
                is_workhour_perweek_eligible = 1;
                query = "update employee_transactiondata set WorkHourPerWeek='" + WorkHourPerWeek + "' ,  WorkHourPerWeek_Eligibility=" + is_workhour_perweek_eligible + ", WorkHourPerWeekFromdate='" + WorkHourPerWeekFromdate + "', WorkHourPerWeekTodate='" + WorkHourPerWeekTodate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;
                return_message += Environment.NewLine + "WorkHourPerWeek data saved from " + WorkHourPerWeekFromdate + " to " + WorkHourPerWeekTodate + " for row " + row_number;
            }
        }

        /*WorkHourPerMonth*/
        if (Convert.ToBoolean(current_data["WH_month_date"]))
        {
            WorkHourPerMonthFromdate = current_data["WH_month_from_date"].ToString();
            WorkHourPerMonthTodate = current_data["WH_month_to_date"].ToString();
            WorkHourPerMonth = current_data["WH_month_drop_down"].ToString();
            query = "select convert(date,WorkHourPerMonthFromdate) as WorkHourPerMonthFromdate, convert(date,WorkHourPerMonthTodate) as WorkHourPerMonthTodate from employee_transactiondata where ((WorkHourPerMonthFromdate  between '" + WorkHourPerMonthFromdate + "' and '" + WorkHourPerMonthTodate + "') or (WorkHourPerMonthTodate between '" + WorkHourPerMonthFromdate + "' and '" + WorkHourPerMonthTodate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                WorkHourPerMonthFromdate = Convert.ToDateTime(dt_temp.Rows[0]["WorkHourPerMonthFromdate"].ToString()).ToString("dd-MMM-yyyy");
                WorkHourPerMonthTodate = Convert.ToDateTime(dt_temp.Rows[0]["WorkHourPerMonthTodate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                return_message += Environment.NewLine + "WorkHourPerMonth data exists from " + WorkHourPerMonthFromdate + " to " + WorkHourPerMonthTodate + " for row " + row_number;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Work hoours per month already exists for selected dates";
                }
            }
            else
            {
                is_workhour_permonth_eligible = 1;
                query = "update employee_transactiondata set WorkHourPerMonth='" + WorkHourPerMonth + "' ,  WorkHourPerMonth_Eligibility=" + is_workhour_permonth_eligible + ", WorkHourPerMonthFromdate='" + WorkHourPerMonthFromdate + "', WorkHourPerMonthTodate='" + WorkHourPerMonthTodate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;

            }
        }

        /*LineManager*/
        if (Convert.ToBoolean(current_data["line_manager"]))
        {
            LineManagerFromdate = current_data["line_from_date"].ToString();
            LineManagerTodate = current_data["line_to_date"].ToString();
            LineManagerID = current_data["line_manager_drop_down"].ToString();
            query = "select convert(date,LineManagerFromdate) as LineManagerFromdate, convert(date,LineManagerTodate) as LineManagerTodate from employee_transactiondata where ((LineManagerFromdate  between '" + LineManagerFromdate + "' and '" + LineManagerTodate + "') or (LineManagerTodate between '" + LineManagerFromdate + "' and '" + LineManagerTodate + "')) and employeecode = '" + EmployeeCode + "'";
            dt_temp = db_connection.ReturnDataTable(query);

            if (dt_temp.Rows.Count > 0)
            {
                LineManagerFromdate = Convert.ToDateTime(dt_temp.Rows[0]["LineManagerFromdate"].ToString()).ToString("dd-MMM-yyyy");
                LineManagerTodate = Convert.ToDateTime(dt_temp.Rows[0]["LineManagerTodate"].ToString()).ToString("dd-MMM-yyyy");
                update_flag = false;
                return_message += Environment.NewLine + "LineManager data exists from " + LineManagerFromdate + " to " + LineManagerTodate + " for row " + row_number;
                if (errormessage != "")
                {
                    errormessage = "," + errormessage;
                }
                else
                {
                    errormessage = errormessage + "Line Manager already exists for selected dates";
                }
            }
            else
            {
                query = "update employee_transactiondata set LineManagerID=" + LineManagerID + ", LineManagerFromdate='" + LineManagerFromdate + "', LineManagerTodate='" + LineManagerTodate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;

            }
        }

        /*Termination*/
        if (Convert.ToBoolean(current_data["termination_date"]))
        {
            Terminationdate = current_data["termination_from_date"].ToString();
            if (!string.IsNullOrEmpty(Terminationdate))
            {
                query = "update employee_transactiondata set Terminationdate='" + Terminationdate + "' where EmployeeCode='" + EmployeeCode + "' and TransactionID=" + TransactionID;
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                update_flag = true;

            }
        }

        /*if non of values updated means delete the row which has been inserted at be begining*/
        if (update_flag == false)
        {
            query = "Delete from employee_transactiondata where TransactionID=" + TransactionID;
            db_connection.ExecuteQuery_WithOutReturnValue(query);
        }






        return update_flag;
    }
    private void UpdateDatabase(Hashtable data)
    {
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;

        string employee_code = data["EmployeeCode"].ToString();
        string mode = data["Mode"].ToString();


        db_connection.ExecuteStoredProcedure_EmployeeMaster("ManipulateEmployeeTransationData", data);


    }

    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }
    protected DataTable GetManagerData(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        DataTable manager_data = new DataTable();
        string employee_id, query = string.Empty;
        int access = 0;
        access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        employee_id = HttpContext.Current.Session["employee_id"].ToString();
        // if manager  is logged in then showing only that manager while adding new employee 
        if (access == 1)
        {
            query = "select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name from employeemaster where IsManager = 1 and Emp_Company = '" + company_code + "' and emp_code = '" + employee_id + "'  order by Emp_Name";
        }
        else
        {
            query = "select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name from employeemaster where IsManager = 1 and Emp_Company = '" + company_code + "' order by Emp_Name";
        }
        manager_data = db_connection.ReturnDataTable(query);

        return manager_data;
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
            return_object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
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



    private string[] GetShiftCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable shift_data = new DataTable();
        string[] shift_code_array = null;
        string query = string.Empty;

        query = "select distinct(shift_code) from shift where IsActive=1";
        shift_data = db_connection.ReturnDataTable(query);

        shift_code_array = new string[shift_data.Rows.Count];

        for (int i = 0; i < shift_data.Rows.Count; i++)
        {
            shift_code_array[i] = shift_data.Rows[i]["shift_code"].ToString().ToUpper();
        }

        return shift_code_array;
    }

    private string[] GetManagerCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable manager_data = new DataTable();
        string[] manager_code_array = null;
        string query = string.Empty;

        query = "select emp_code  from EmployeeMaster where IsManager = 1  and emp_status = 1 ";
        manager_data = db_connection.ReturnDataTable(query);

        manager_code_array = new string[manager_data.Rows.Count];

        for (int i = 0; i < manager_data.Rows.Count; i++)
        {
            manager_code_array[i] = manager_data.Rows[i]["emp_code"].ToString().ToUpper();
        }

        return manager_code_array;
    }
    [WebMethod]
    public static ReturnObject DoImport(string file_name)
    {
        employee_data_transaction page_object = new employee_data_transaction();

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        DataTable employee_transaction_data = new DataTable();
        DataTable dt_temp = new DataTable();

        string[] employee_code_array = null, shift_code_array = null, manager_code_array = null;

        bool valid_flag = true, HrBranchFlag = false, employee_code_exists = true, update_flag = false;
        int employee_count = 0, row_number = 2, AccessLevel = 0, is_manager = 0, LoginAccessLevel = 0, TransactionID = 0;

        List<string> companyList = null, branchList = null, departmentList = null, designationList = null, employee_categoryList = null, shiftList = null
            , employee_cardList = null;

        string upload_path = string.Empty, ExcelFullPath = string.Empty, query = string.Empty, update_query = string.Empty, employee_gender = string.Empty,
                current_user = string.Empty, EmployeeCode = string.Empty, EmployeeName = string.Empty,
                EmployeeCompany = string.Empty, EmployeeBranch = string.Empty, EmployeeDepartment = string.Empty, EmployeeCategory = string.Empty,
                ShiftFromDate = string.Empty, ShiftToDate = string.Empty, Shift_Code = string.Empty, return_message = string.Empty,
                OTFromdate = string.Empty, OTTodate = string.Empty, RamadanFromdate = string.Empty, RamadanTodate = string.Empty,
                PunchExceptionFromdate = string.Empty, PunchExceptionTodate = string.Empty,
                ChildDateofBirth = string.Empty, MaternityFromDate = string.Empty, MaternityToDate = string.Empty,
                WorkHourPerdayFromdate = string.Empty, WorkHourPerdayTodate = string.Empty, WorkHourPerday = string.Empty,
                WorkHourPerWeekFromdate = string.Empty, WorkHourPerWeekTodate = string.Empty, WorkHourPerWeek = string.Empty,
                WorkHourPerMonthFromdate = string.Empty, WorkHourPerMonthTodate = string.Empty, WorkHourPerMonth = string.Empty,
                LineManagerFromdate = string.Empty, LineManagerTodate = string.Empty, LineManagerID = string.Empty, Terminationdate = string.Empty;

        int WorkHourPerday_Eligibility = 0, WorkHourPerWeek_Eligibility = 0, WorkHourPerMonth_Eligibility = 0, ID = 0,
            Shift_Eligibility = 0, OT_Eligibility = 0, Ramadan_Eligibility = 0, PunchException_Eligibility = 0, Maternity_Eligibility = 0;

        bool ShiftFD = false, ShiftTD = false, OTFD = false, OTTD = false, CD = false, PEFD = false, PETD = false, TD = false, LMFD = false, LMTD = false,
         LMMID = false, WHPDFD = false, WHPDTD = false, WHPWFD = false, WHPWTD = false, WHPMFD = false, WHPMTD = false, RFD = false, RTD = false, IsShift = false;

        DataRow first_row = null;
        DateTime FromDate, ToDate;
        try
        {
            current_user = HttpContext.Current.Session["username"].ToString();
            LoginAccessLevel = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());

            upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            ExcelFullPath = HttpContext.Current.Server.MapPath("~/" + upload_path + "/" + file_name);
           // ExcelFullPath = "D:\\SecurAX Projects\\RG\\employee_transaction.xlsx";

            //read data from  importexcel file 
            employee_transaction_data = ExcelImport.ImportExcelToDataTable(ExcelFullPath, "");

            //remove 2nd row from excel sheet bcz it has no data
            first_row = employee_transaction_data.Rows[0];
            employee_transaction_data.Rows.Remove(first_row);

            employee_code_array = page_object.GetEmployeeCodes();
            shift_code_array = page_object.GetShiftCodes();
            manager_code_array = page_object.GetManagerCodes();

            //check databatable rows count
            if (employee_transaction_data.Rows.Count > 0)
            {
                foreach (DataRow dr in employee_transaction_data.Rows)
                {
                    row_number++; /*keep excel row number in this variable*/

                    EmployeeCode = dr["EmployeeCode"].ToString();

                    /*checking for employee code value*/
                    if (string.IsNullOrEmpty(EmployeeCode))
                    {
                        return_message += Environment.NewLine + "Employee Code is Empty row number " + row_number;
                    }
                    else
                    {
                        /*checking employee existance*/
                        if (Array.IndexOf(employee_code_array, EmployeeCode.ToUpper()) < 0)
                        {
                            return_message += Environment.NewLine + "You don't have permission to add Employee Transaction data for Employee Code '" + EmployeeCode + "' row number " + row_number;
                        }
                        else
                        {

                            #region Shift

                            if (!string.IsNullOrEmpty(dr["ShiftFromDate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["ShiftFromDate"].ToString()))
                                {
                                    ShiftFromDate = Convert.ToDateTime(dr["ShiftFromDate"].ToString()).ToString("yyyy-MM-dd");
                                    ShiftFD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "ShiftFromDate is invalid or date is past date for row number  " + row_number;
                                    ShiftFD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["ShiftToDate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["ShiftToDate"].ToString()))
                                {
                                    ShiftToDate = Convert.ToDateTime(dr["ShiftToDate"].ToString()).ToString("yyyy-MM-dd");
                                    ShiftTD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "ShiftToDate is invalid or date is past date for row number  " + row_number;
                                    ShiftTD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["Shift_Code"].ToString()))
                            {
                                Shift_Code = dr["Shift_Code"].ToString();

                                IsShift = true;

                                if (Array.IndexOf(shift_code_array, Shift_Code.ToUpper()) < 0)
                                {
                                    return_message += Environment.NewLine + "Shift Code '" + Shift_Code + "' is Invalid for row number  " + row_number;
                                    IsShift = false;
                                }
                            }

                            /*Shift*/
                            if (ShiftFD && ShiftTD && IsShift)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate ='" + ShiftFromDate + "' and ToDate = '" + ShiftToDate + "' and empid = '" + EmployeeCode + "' and  TransactionType=1 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                    query = "update EmployeeTransactionData set TransactionData='" + Shift_Code + "' where ID=" + ID;
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "Shift data updated from " + ShiftFromDate + " to " + ShiftToDate + " for row " + row_number;
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',1,'" + ShiftFromDate + "','" + ShiftToDate + "','" + Shift_Code + "',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "Shift data saved from " + ShiftFromDate + " to " + ShiftToDate + " for row " + row_number;
                                }
                            }
                            #endregion

                            #region OT

                            if (!string.IsNullOrEmpty(dr["OTFromdate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["OTFromdate"].ToString()))
                                {
                                    OTFromdate = Convert.ToDateTime(dr["OTFromdate"].ToString()).ToString("yyyy-MM-dd");
                                    OTFD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "OTFromdate is invalid or date is past date for row number  " + row_number;
                                    OTFD = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(dr["OTTodate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["OTTodate"].ToString()))
                                {
                                    OTTodate = Convert.ToDateTime(dr["OTTodate"].ToString()).ToString("yyyy-MM-dd");
                                    OTTD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "OTTodate is invalid or date is past date for row number  " + row_number;
                                    OTTD = false;
                                }
                            }

                            /*OverTime*/
                            if (OTFD && OTTD)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate ='" + OTFromdate + "' and ToDate= '" + OTTodate + "' and empid = '" + EmployeeCode + "' and  TransactionType=2 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',2,'" + OTFromdate + "','" + OTTodate + "','1',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "OverTime data saved from " + OTFromdate + " to " + OTTodate + " for row " + row_number;
                                }
                            }
                            #endregion

                            #region Ramadan
                            if (!string.IsNullOrEmpty(dr["RamadanFromdate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["RamadanFromdate"].ToString()))
                                {
                                    RamadanFromdate = Convert.ToDateTime(dr["RamadanFromdate"].ToString()).ToString("yyyy-MM-dd");
                                    RFD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "RamadanFromdate is invalid or date is past date for row number  " + row_number;
                                    RFD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["RamadanTodate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["RamadanTodate"].ToString()))
                                {
                                    RamadanTodate = Convert.ToDateTime(dr["RamadanTodate"].ToString()).ToString("yyyy-MM-dd");
                                    RTD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "RamadanTodate is invalid or date is past date for row number  " + row_number;
                                    RTD = false;
                                }
                            }

                            /*Ramadan*/
                            if (RFD && RTD)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate ='" + RamadanFromdate + "' and ToDate= '" + RamadanTodate + "' and empid = '" + EmployeeCode + "' and  TransactionType=3 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',3,'" + RamadanFromdate + "','" + RamadanTodate + "','1',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "Ramadan data saved from " + RamadanFromdate + " to " + RamadanTodate + " for row " + row_number;
                                }
                            }
                            #endregion

                            #region Punch
                            if (!string.IsNullOrEmpty(dr["PunchExceptionFromdate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["PunchExceptionFromdate"].ToString()))
                                {
                                    PunchExceptionFromdate = Convert.ToDateTime(dr["PunchExceptionFromdate"].ToString()).ToString("yyyy-MM-dd");
                                    PEFD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "PunchExceptionFromdate is invalid or date is past date for row number  " + row_number;
                                    PEFD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["PunchExceptionTodate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["PunchExceptionTodate"].ToString()))
                                {
                                    PunchExceptionTodate = Convert.ToDateTime(dr["PunchExceptionTodate"].ToString()).ToString("yyyy-MM-dd");
                                    PETD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "PunchExceptionTodate is invalid or date is past date for row number  " + row_number;
                                    PETD = false;
                                }
                            }

                            /*PunchException*/
                            if (PEFD && PETD)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate='" + PunchExceptionFromdate + "' and ToDate= '" + PunchExceptionTodate + "' and empid = '" + EmployeeCode + "' and  TransactionType=4 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',4,'" + PunchExceptionFromdate + "','" + PunchExceptionTodate + "','1',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "PunchException data saved from " + PunchExceptionFromdate + " to " + PunchExceptionTodate + " for row " + row_number;
                                }
                            }
                            #endregion

                            #region Maternity

                            if (!string.IsNullOrEmpty(dr["ChildDateofBirth"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["ChildDateofBirth"].ToString()))
                                {
                                    ChildDateofBirth = Convert.ToDateTime(dr["ChildDateofBirth"].ToString()).ToString("yyyy-MM-dd");

                                    MaternityFromDate = ChildDateofBirth;
                                    MaternityToDate = Convert.ToDateTime(ChildDateofBirth).AddMonths(18).ToString("yyyy-MM-dd");

                                    CD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "ChildDateofBirth is invalid or date is past date for row number  " + row_number;
                                    CD = false;
                                }
                            }

                            /*Maternity*/
                            if (CD)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate ='" + MaternityFromDate + "' and ToDate= '" + MaternityToDate + "' and empid = '" + EmployeeCode + "' and  TransactionType=5 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                    query = "update EmployeeTransactionData set TransactionData='" + ChildDateofBirth + "' where ID=" + ID;
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                                    ID = 0;

                                    return_message += Environment.NewLine + "Maternity data updated from " + MaternityFromDate + " to " + MaternityToDate + " for row " + row_number;
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',5,'" + MaternityFromDate + "','" + MaternityToDate + "','" + ChildDateofBirth + "',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "Maternity data saved from " + MaternityFromDate + " to " + MaternityToDate + " for row " + row_number;
                                }
                            }
                            #endregion

                            #region WorkHourPerDay

                            if (!string.IsNullOrEmpty(dr["WorkHourPerdayFromdate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["WorkHourPerdayTodate"].ToString()))
                                {
                                    WorkHourPerdayFromdate = Convert.ToDateTime(dr["WorkHourPerdayFromdate"].ToString()).ToString("yyyy-MM-dd");
                                    WHPDFD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "WorkHourPerdayFromdate is invalid or date is past date for row number  " + row_number;
                                    WHPDFD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["WorkHourPerdayTodate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["WorkHourPerdayTodate"].ToString()))
                                {
                                    WorkHourPerdayTodate = Convert.ToDateTime(dr["WorkHourPerdayTodate"].ToString()).ToString("yyyy-MM-dd");
                                    WHPDTD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "WorkHourPerdayTodate is invalid or date is past date for row number  " + row_number;
                                    WHPDTD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["WorkHourPerday"].ToString()))
                            {
                                WorkHourPerday = dr["WorkHourPerday"].ToString();
                            }

                            /*WorkHourPerday*/
                            if (WHPDFD && WHPDTD)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate ='" + WorkHourPerdayFromdate + "' and ToDate= '" + WorkHourPerdayTodate + "' and empid = '" + EmployeeCode + "' and  TransactionType=6 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                    query = "update EmployeeTransactionData set TransactionData='" + WorkHourPerday + "' where ID=" + ID;
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                                    ID = 0;
                                    return_message += Environment.NewLine + "WorkHourPerday data exists from " + WorkHourPerdayFromdate + " to " + WorkHourPerdayTodate + " for row " + row_number;
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',6,'" + WorkHourPerdayFromdate + "','" + WorkHourPerdayTodate + "','" + WorkHourPerday + "',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "WorkHourPerday data saved from " + WorkHourPerdayFromdate + " to " + WorkHourPerdayTodate + " for row " + row_number;
                                }
                            }

                            #endregion

                            #region WorkHOurPerWeek

                            if (!string.IsNullOrEmpty(dr["WorkHourPerWeekFromdate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["WorkHourPerWeekFromdate"].ToString()))
                                {
                                    WorkHourPerWeekFromdate = Convert.ToDateTime(dr["WorkHourPerWeekFromdate"].ToString()).ToString("yyyy-MM-dd");
                                    WHPWFD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "WorkHourPerWeekFromdate is invalid or date is past date for row number  " + row_number;
                                    WHPWFD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["WorkHourPerWeekTodate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["WorkHourPerWeekTodate"].ToString()))
                                {
                                    WorkHourPerWeekTodate = Convert.ToDateTime(dr["WorkHourPerWeekTodate"].ToString()).ToString("yyyy-MM-dd");
                                    WHPWTD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "WorkHourPerWeekTodate is invalid or date is past date for row number  " + row_number;
                                    WHPWTD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["WorkHourPerWeek"].ToString()))
                            {
                                WorkHourPerWeek = dr["WorkHourPerWeek"].ToString();
                            }

                            /*WorkHourPerWeek*/
                            if (WHPWFD && WHPWTD)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate ='" + WorkHourPerWeekFromdate + "' and ToDate= '" + WorkHourPerWeekTodate + "')) and empid = '" + EmployeeCode + "' and  TransactionType=7 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                    query = "update EmployeeTransactionData set TransactionData='" + WorkHourPerWeek + "' where ID=" + ID;
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                                    ID = 0;
                                    return_message += Environment.NewLine + "WorkHourPerWeek data updated from " + WorkHourPerWeekFromdate + " to " + WorkHourPerWeekTodate + " for row " + row_number;
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',7,'" + WorkHourPerWeekFromdate + "','" + WorkHourPerWeekTodate + "','" + WorkHourPerWeek + "',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "WorkHourPerWeek data saved from " + WorkHourPerWeekFromdate + " to " + WorkHourPerWeekTodate + " for row " + row_number;
                                }
                            }
                            #endregion

                            #region WorkHourPerMonth

                            if (!string.IsNullOrEmpty(dr["WorkHourPerMonthFromdate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["WorkHourPerMonthFromdate"].ToString()))
                                {
                                    WorkHourPerMonthFromdate = Convert.ToDateTime(dr["WorkHourPerMonthFromdate"].ToString()).ToString("yyyy-MM-dd");
                                    WHPMFD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "WorkHourPerMonthFromdate is invalid or date is past date for row number  " + row_number;
                                    WHPMFD = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["WorkHourPerMonthTodate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["WorkHourPerMonthTodate"].ToString()))
                                {
                                    WorkHourPerMonthTodate = Convert.ToDateTime(dr["WorkHourPerMonthTodate"].ToString()).ToString("yyyy-MM-dd");
                                    WHPMTD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "WorkHourPerMonthTodate is invalid or date is past date for row number  " + row_number;
                                    WHPMTD = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(dr["WorkHourPerMonth"].ToString()))
                            {
                                WorkHourPerMonth = dr["WorkHourPerMonth"].ToString();
                            }

                            /*WorkHourPerMonth*/
                            if (WHPMFD && WHPMTD)
                            {
                                query = "select ID from EmployeeTransactionData where FromDate ='" + WorkHourPerMonthFromdate + "' and ToDate= '" + WorkHourPerMonthTodate + "' and empid = '" + EmployeeCode + "' and  TransactionType=8 ";
                                ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                if (ID > 0)
                                {
                                    query = "update EmployeeTransactionData set TransactionData='" + WorkHourPerMonth + "' where ID=" + ID;
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                                    ID = 0;
                                    return_message += Environment.NewLine + "WorkHourPerMonth data updated from " + WorkHourPerMonthFromdate + " to " + WorkHourPerMonthTodate + " for row " + row_number;
                                }
                                else
                                {
                                    query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                    query += " values ('" + EmployeeCode + "',8,'" + WorkHourPerMonthFromdate + "','" + WorkHourPerMonthTodate + "','" + WorkHourPerMonth + "',1)";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += Environment.NewLine + "WorkHourPerMonth data saved from " + WorkHourPerMonthFromdate + " to " + WorkHourPerMonthTodate + " for row " + row_number;
                                }
                            }
                            #endregion

                            #region Terminate
                            if (!string.IsNullOrEmpty(dr["Terminationdate"].ToString()))
                            {
                                if (page_object.ValidateDate(dr["Terminationdate"].ToString()))
                                {
                                    Terminationdate = Convert.ToDateTime(dr["Terminationdate"].ToString()).ToString("yyyy-MM-dd");
                                    TD = true;
                                }
                                else
                                {
                                    return_message += Environment.NewLine + "Terminationdate is invalid or date is past date for row number  " + row_number;
                                    TD = false;
                                }
                            }

                            /*Termination*/
                            if (TD)
                            {
                                if (!string.IsNullOrEmpty(Terminationdate))
                                {
                                    query = "Select ID from EmployeeTransactionData where FromDate='" + Terminationdate + "' empid = '" + EmployeeCode + "' and  TransactionType=9 ";
                                    ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                    if (ID > 0)
                                    {
                                        query = "update EmployeeTransactionData set TransactionData='" + Terminationdate + "' where ID=" + ID;
                                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                                        ID = 0;
                                        return_message += Environment.NewLine + "Termination data updated as " + Terminationdate + " for row " + row_number;
                                    }
                                    else
                                    {
                                        query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                        query += " values ('" + EmployeeCode + "',9,'" + Terminationdate + "','" + Terminationdate + "','" + Terminationdate + "',1)";
                                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                                        return_message += Environment.NewLine + "Termination data saved for date: " + Terminationdate + " for row " + row_number;
                                    }
                                }

                            #endregion

                                #region LineManager
                                if (!string.IsNullOrEmpty(dr["LineManagerFromdate"].ToString()))
                                {
                                    if (page_object.ValidateDate(dr["LineManagerFromdate"].ToString()))
                                    {
                                        LineManagerFromdate = Convert.ToDateTime(dr["LineManagerFromdate"].ToString()).ToString("yyyy-MM-dd");
                                        LMFD = true;
                                    }
                                    else
                                    {
                                        return_message += Environment.NewLine + "LineManagerFromdate is invalid or date is past date for row number  " + row_number;
                                        LMFD = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(dr["LineManagerTodate"].ToString()))
                                {
                                    if (page_object.ValidateDate(dr["LineManagerTodate"].ToString()))
                                    {
                                        LineManagerTodate = Convert.ToDateTime(dr["LineManagerTodate"].ToString()).ToString("yyyy-MM-dd");
                                        LMTD = true;
                                    }
                                    else
                                    {
                                        return_message += Environment.NewLine + "LineManagerTodate is invalid or date is past date for row number  " + row_number;
                                        LMTD = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(dr["LineManagerID"].ToString()))
                                {
                                    LineManagerID = dr["LineManagerID"].ToString();

                                    LMMID = true;

                                    if (Array.IndexOf(manager_code_array, LineManagerID.ToUpper()) < 0)
                                    {
                                        return_message += Environment.NewLine + "Manager Code '" + LineManagerID + "' is Invalid for row number  " + row_number;
                                        LMMID = false;
                                    }
                                }


                                /*LineManager*/
                                if (LMFD && LMTD && LMMID)
                                {
                                    query = @"select convert(date,FromDate) as LineManagerFromdate, convert(date,ToDate) as LineManagerTodate from EmployeeTransactionData where
                                    ((FromDate >='" + WorkHourPerMonthFromdate + "' or ToDate<= '" + WorkHourPerMonthTodate + "')) and empid = '" + EmployeeCode + "' and  TransactionType=10 ";
                                    ID = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                                    if (ID > 0)
                                    {
                                        query = "update EmployeeTransactionData set TransactionData='" + LineManagerID + "' where ID=" + ID;
                                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                                        ID = 0;
                                        return_message += Environment.NewLine + "LineManager data updated from " + LineManagerFromdate + " to " + LineManagerTodate + " for row " + row_number;
                                    }
                                    else
                                    {
                                        query = "Insert into EmployeeTransactionData (EmpID, TransactionType, FromDate, ToDate, TransactionData, IsActive)";
                                        query += " values ('" + EmployeeCode + "',10,'" + LineManagerFromdate + "','" + LineManagerTodate + "','" + LineManagerID + "',1)";
                                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                                        return_message += Environment.NewLine + "LineManager data saved from " + LineManagerFromdate + " to " + LineManagerTodate + " for row " + row_number;
                                    }
                                }
                                #endregion

                            }
                        }

                        /*reset values for next employee*/
                        EmployeeCompany = string.Empty; EmployeeCode = string.Empty; EmployeeName = string.Empty; EmployeeBranch = string.Empty;
                        EmployeeCategory = string.Empty; EmployeeDepartment = string.Empty; Terminationdate = string.Empty;
                        ShiftFromDate = string.Empty; ShiftToDate = string.Empty; Shift_Code = string.Empty;
                        OTFromdate = string.Empty; OTTodate = string.Empty; RamadanFromdate = string.Empty; RamadanTodate = string.Empty;
                        PunchExceptionFromdate = string.Empty; PunchExceptionTodate = string.Empty;
                        ChildDateofBirth = string.Empty; MaternityFromDate = string.Empty; MaternityToDate = string.Empty;
                        WorkHourPerdayFromdate = string.Empty; WorkHourPerdayTodate = string.Empty; WorkHourPerday = string.Empty;
                        WorkHourPerWeekFromdate = string.Empty; WorkHourPerWeekTodate = string.Empty; WorkHourPerWeek = string.Empty;
                        WorkHourPerMonthFromdate = string.Empty; WorkHourPerMonthTodate = string.Empty; WorkHourPerMonth = string.Empty;
                        LineManagerFromdate = string.Empty; LineManagerTodate = string.Empty; LineManagerID = string.Empty;

                        WorkHourPerday_Eligibility = 0; WorkHourPerWeek_Eligibility = 0; WorkHourPerMonth_Eligibility = 0;
                        Shift_Eligibility = 0; OT_Eligibility = 0; Ramadan_Eligibility = 0; PunchException_Eligibility = 0; Maternity_Eligibility = 0;

                        ShiftFD = false; ShiftTD = false; OTFD = false; OTTD = false; CD = false; PEFD = false; PETD = false; TD = false; LMFD = false; LMTD = false;
                        WHPDFD = false; WHPDTD = false; WHPWFD = false; WHPWTD = false; WHPMFD = false; WHPMTD = false; RFD = false; RTD = false; IsShift = false;

                    }
                }

                return_object.status = "success";
                return_object.return_data = return_message;

            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "DO_IMPORT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading data for DO IMPORT. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
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

    private bool ValidateDate(string date_value)
    {
        bool return_date = false;
        DateTime dt_value = new DateTime();

        try
        {
            if (DateTime.TryParse(date_value, out dt_value))
            {
                if (dt_value.Date >= DateTime.Now.Date)
                {
                    return_date = true;
                }
            }
        }
        catch (Exception ex)
        {
            return_date = false;
        }

        return return_date;
    }



    [WebMethod]
    public static ReturnObject GetEmployeeData(string employee_code)
    {

        employee_data_transaction page_object = new employee_data_transaction();
        DBConnection db_connection = new DBConnection();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty, company_code = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {

                /* getting shift data and adding it to Data set to be returned */

                //getting employee company code by help of employee_code
                query = "select emp_company from employeemaster where emp_code = '" + employee_code + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);

                query = "select shift_code, shift_desc from shift where companycode = '" + company_code + "' and IsActive=1 order by shift_desc";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "shift";
                return_data_set.Tables.Add(temp_data_table);

                /* get manager data and add it to the data set */
                temp_data_table = page_object.GetManagerData(company_code);
                temp_data_table.TableName = "manager";
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
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetEmployeeTransactionData(string employee_code, string company_code, string branch_code)
    {
        employee_data_transaction page_object = new employee_data_transaction();
        DBConnection db_connection = new DBConnection();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        ReturnObject return_object = new ReturnObject();
        string query = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            return_object = page_object.DoLogout();
        }
        else
        {
            try
            {
                temp_data_table = null;
                query = "Select emp_code from Employeemaster where Emp_Code='" + employee_code + "'";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "employeedata";

                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting shift data and adding it to Data set to be returned */
                temp_data_table = null;
                query = "Select   id , transactiontype  ,  fromdate , todate , s.shift_desc, transactiondata from EmployeeTransactionData etd   join shift s on  etd.TransactionData = s.Shift_Code and etd.empid='" + employee_code + "' and etd.TransactionType = 1 and etd.IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "shiftData";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting OT data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select OTFromdate , OTTodate  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by OTFromdate";
                query = "Select   id , transactiontype  ,  fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 2 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "ot";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting Ramadan data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select RamadanFromdate , RamadanTodate  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by RamadanFromdate";
                query = "Select  id , transactiontype  , fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 3 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "ramadan";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting punch exceeption data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select PunchExceptionFromdate , PunchExceptiontodate  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by PunchExceptionFromdate";
                query = "Select  id , transactiontype  , fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 4 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "punchexception";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting work hour per day  data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select WorkHourPerdayFromdate , WorkHourPerdayTodate , WorkHourPerday  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerdayFromdate";
                query = "Select  id , transactiontype  , fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 6 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "workhourperday";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting work hour per week  data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select WorkHourPerWeekFromdate , WorkHourPerWeekTodate , WorkHourPerWeek  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerWeekFromdate";
                query = "Select  id , transactiontype  , fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 7 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "workhourperweek";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting work hour per month  data and adding it to Data set to be returned */
                temp_data_table = null;
                // query = "Select WorkHourPerMonthFromdate , WorkHourPerMonthTodate , WorkHourPerMonth  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerMonthFromdate";
                query = "Select   id , transactiontype  ,  fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 8 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "workhourpermonth";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting maternity   data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select ChildDateofBirth    from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by ChildDateofBirth";
                query = "Select  id , transactiontype  ,  fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 5 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "maternity";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting termination data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select Terminationdate    from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by Terminationdate";
                query = "Select   id , transactiontype  ,  fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 9 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "termination";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                /* getting work hour per month  data and adding it to Data set to be returned */
                temp_data_table = null;
                //query = "Select LineManagerFromdate , LineManagerTodate , LineManagerID  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by LineManagerFromdate";
                query = "Select  id , transactiontype  ,  fromdate , todate , transactiondata from EmployeeTransactionData where empid='" + employee_code + "' and TransactionType = 10 and IsActive=1";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "manager";
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));
                foreach (DataRow row in temp_data_table.Rows)
                {
                    row["StatusFlag"] = "N";
                    //N == No Change
                    //I == Inserted row
                    //D == Deleted row
                    //E == Changed row
                }
                //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time
                return_data_set.Tables.Add(temp_data_table);
                //=================================================================================================================
                return_object.status = "success";
                return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);

                //string jsonvalue = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
                //saveEmployeeTransactionData(return_object.return_data);
                //DataSet returndataset = (DataSet)JsonConvert.DeserializeObject(jsonvalue, (typeof(DataSet)));
                //DataTable shifttable = new DataTable();
                //shifttable = returndataset.Tables["shift"];
                //  for ( int i = 0; i <= shifttable.Rows.Count)




            }
            catch (Exception ex)
            {

                Logger.LogException(ex, page, "GET_TRANSACTION_DATA");

                return_object.status = "error";
                return_object.return_data = "An error occurred while loading data for OT Eligibility. Please try again. If the error persists, please contact Support.";

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }



    //[WebMethod]
    //public static ReturnObject GetEmployeeTransactionData(string employee_code, string company_code, string branch_code)

    //{

    //    employee_data_transaction page_object = new employee_data_transaction();

    //    DBConnection db_connection = new DBConnection();

    //    DataTable temp_data_table = new DataTable();

    //    DataSet return_data_set = new DataSet();

    //    ReturnObject return_object = new ReturnObject();

    //    string query = string.Empty;

    //    if (HttpContext.Current.Session["username"] == null) // checking session expired or not

    //    {

    //        return_object = page_object.DoLogout();

    //    }

    //    else

    //    {

    //        try

    //        {

    //            temp_data_table = null;

    //            query = "Select emp_company , emp_branch from Employeemaster where Emp_Code='" + employee_code + "'and emp_company='" + company_code + "' and emp_branch='" + branch_code + "'";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "employeedata";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting shift data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select ShiftFromdate , ShiftTodate , Shift_name, Shift_code from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by ShiftFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "shift";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting OT data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select OTFromdate , OTTodate from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by OTFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "ot";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting Ramadan data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select RamadanFromdate , RamadanTodate from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by RamadanFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "ramadan";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting punch exceeption data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select PunchExceptionFromdate , PunchExceptiontodate from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by PunchExceptionFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "punchexception";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting work hour per day data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select WorkHourPerdayFromdate , WorkHourPerdayTodate , WorkHourPerday from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerdayFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "workhourperday";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting work hour per week data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select WorkHourPerWeekFromdate , WorkHourPerWeekTodate , WorkHourPerWeek from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerWeekFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "workhourperweek";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting work hour per month data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select WorkHourPerMonthFromdate , WorkHourPerMonthTodate , WorkHourPerMonth from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerMonthFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "workhourpermonth";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting maternity data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select ChildDateofBirth from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by ChildDateofBirth";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "maternity";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting termination data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select Terminationdate from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by Terminationdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "termination";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            /* getting work hour per month data and adding it to Data set to be returned */

    //            temp_data_table = null;

    //            query = "Select LineManagerFromdate , LineManagerTodate , LineManagerID from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by LineManagerFromdate";

    //            temp_data_table = db_connection.ReturnDataTable(query);

    //            temp_data_table.TableName = "manager";

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            temp_data_table.Columns.Add("StatusFlag", typeof(System.Char));

    //            foreach (DataRow row in temp_data_table.Rows)

    //            {

    //                row["StatusFlag"] = "N";

    //                //N == No Change

    //                //I == Inserted row

    //                //D == Deleted row

    //                //E == Changed row

    //            }

    //            //Start: DNB == 03-Sep-2016 == Adding default status of "no change" for all records being sent for te 1st time

    //            return_data_set.Tables.Add(temp_data_table);

    //            //=================================================================================================================

    //            return_object.status = "success";

    //            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);

    //        }

    //        catch (Exception ex)

    //        {

    //            Logger.LogException(ex, page, "GET_TRANSACTION_DATA");

    //            return_object.status = "error";

    //            return_object.return_data = "An error occurred while loading data for OT Eligibility. Please try again. If the error persists, please contact Support.";

    //            throw;

    //        }

    //        finally

    //        {

    //            page_object.Dispose();

    //        }

    //    }

    //    return return_object;

    //}


    //public static ReturnObject GetEmployeeTransactionData(string employee_code,string company_code,string branch_code)
    //{
    //    employee_data_transaction page_object = new employee_data_transaction();
    //    DBConnection db_connection = new DBConnection();
    //    DataTable temp_data_table = new DataTable();
    //    DataSet return_data_set = new DataSet();
    //    ReturnObject return_object = new ReturnObject();
    //    string query = string.Empty;
    //    if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
    //    {
    //        return_object = page_object.DoLogout();
    //    }
    //    else
    //    {
    //        try
    //        {
    //            query = "Select emp_company  , emp_branch  from Employeemaster where Emp_Code='" + employee_code + "'and emp_company='"+ company_code + "' and emp_branch='"+ branch_code + "'";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "employeedata";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting shift data and adding it to Data set to be returned */
    //            query = "Select ShiftFromdate , ShiftTodate , Shift_name from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by ShiftFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "shift";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting OT data and adding it to Data set to be returned */
    //            query = "Select OTFromdate , OTTodate  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by OTFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "ot";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting Ramadan data and adding it to Data set to be returned */
    //            query = "Select RamadanFromdate , RamadanTodate  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by RamadanFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "ramadan";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting punch exceeption data and adding it to Data set to be returned */
    //            query = "Select PunchExceptionFromdate , PunchExceptiontodate  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by PunchExceptionFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "punchexception";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting work hour per day  data and adding it to Data set to be returned */
    //            query = "Select WorkHourPerdayFromdate , WorkHourPerdayTodate , WorkHourPerday  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerdayFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "workhourperday";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting work hour per week  data and adding it to Data set to be returned */
    //            query = "Select WorkHourPerWeekFromdate , WorkHourPerWeekTodate , WorkHourPerWeek  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerWeekFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "workhourperweek";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting work hour per month  data and adding it to Data set to be returned */
    //            query = "Select WorkHourPerMonthFromdate , WorkHourPerMonthTodate , WorkHourPerMonth  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by WorkHourPerMonthFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "workhourpermonth";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting maternity   data and adding it to Data set to be returned */
    //            query = "Select ChildDateofBirth    from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by ChildDateofBirth";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "maternity";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting termination data and adding it to Data set to be returned */
    //            query = "Select Terminationdate    from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by Terminationdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "termination";
    //            return_data_set.Tables.Add(temp_data_table);

    //            /* getting work hour per month  data and adding it to Data set to be returned */
    //            query = "Select LineManagerFromdate , LineManagerTodate , LineManagerID  from Employee_TransactionData where EmployeeCode='" + employee_code + "' order by LineManagerFromdate";
    //            temp_data_table = db_connection.ReturnDataTable(query);
    //            temp_data_table.TableName = "manager";
    //            return_data_set.Tables.Add(temp_data_table);


    //            return_object.status = "success";
    //            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
    //        }
    //        catch (Exception ex)
    //        {

    //            Logger.LogException(ex, page, "GET_TRANSACTION_DATA");

    //            return_object.status = "error";
    //            return_object.return_data = "An error occurred while loading data for OT Eligibility. Please try again. If the error persists, please contact Support.";

    //            throw;
    //        }
    //        finally
    //        {
    //            page_object.Dispose();
    //        }
    //    }

    //    return return_object;
    //}

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {

        employee_data_transaction page_object = new employee_data_transaction();
        DBConnection db_connection = new DBConnection();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        ReturnObject return_object = new ReturnObject();
        string department_query = string.Empty;
        string designation_query = string.Empty;
        string branch_query = string.Empty;

        try
        {

            //department_query = "Select DeptName as department_name, DeptCode as department_code from DeptMaster where CompanyCode='" + company_code + "' order by DeptCode";
            //temp_data_table = db_connection.ReturnDataTable(department_query);
            //temp_data_table.TableName = "department";
            //return_data_set.Tables.Add(temp_data_table);

            //designation_query = "Select desigcode as designation_code, designame as designation_name from DesigMaster where CompanyCode='" + company_code + "' order by desigcode";
            //temp_data_table = db_connection.ReturnDataTable(designation_query);
            //temp_data_table.TableName = "designation";
            //return_data_set.Tables.Add(temp_data_table);

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
}
