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
using System.IO;
using System.Data.OleDb;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using SecurTime;
using System.Data.SqlClient;
using Newtonsoft.Json;
using SecurAX.Logger;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;


public partial class leave_assign : System.Web.UI.Page
{
    const string page = "LEAVE_ASSIGN";

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

            message = "An error occurred while loading Leave Assign page. Please try again. If the error persists, please contact Support.";

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

    private string GetFilterQuery(string filters, string query)
    {
        JObject filters_data = JObject.Parse(filters);
        string company = filters_data["filter_company"].ToString();
        string branch = filters_data["filter_branch"].ToString();
        string department = filters_data["filter_department"].ToString();
        string designation = filters_data["filter_designation"].ToString();
        string employee_category = filters_data["filter_employee_category"].ToString();
        string shift = filters_data["filter_shift"].ToString();
        string employee_name = filters_data["filter_employee_name"].ToString();
        string employee_id = filters_data["filter_employee_id"].ToString();


        //==========company==========
        if (company != "select")
            query += " and E.Emp_Company='" + company + "'";

        //==========branch==========
        if (branch != "select")
            query += " and E.Emp_Branch='" + branch + "'";

        //==========department==========
        if (department != "select")
            query += " and E.Emp_Department='" + department + "'";

        //==========designation==========
        if (designation != "select")
            query += " and E.Emp_Designation='" + designation + "'";

        //=========employee_category==========
        if (employee_category != "select")
            query += " and E.Emp_Employee_Category='" + employee_category + "'";

        //==========shift==========
        if (shift != "select")
            query += " and E.Emp_Shift_Detail='" + shift + "'";

        //==========employee_name==========
        if (employee_name != "")
            query += " and E.Emp_Name like '%" + employee_name + "%' ";

        //==========employee_id=========
        if (employee_id != "")
            query += " and E.Emp_Code='" + employee_id + "'";

        return query;
    }

    private string GetEmployeeBaseQuery()
    {
        string query = string.Empty;
        string employee_id = HttpContext.Current.Session["employee_id"].ToString();

        if (string.IsNullOrEmpty(employee_id))
        {
            employee_id = "admin";
        }
        
        query = @"Select DISTINCT(Emp_Code) as EmployeeID, Emp_Name as EmployeeName, Emp_Company as Company, Emp_Branch as Branch, Emp_Designation as Designation,Emp_Department as Department,Emp_Employee_Category as EmployeeCategory, Emp_Shift_Detail as Shift from ( select E.Emp_Code, E.Emp_Name, E.Emp_Company, E.Emp_Branch, E.Emp_Designation, E.Emp_Department, E.Emp_Employee_Category, E.Emp_Shift_Detail, ROW_NUMBER() OVER (ORDER BY E.Emp_Code) as row  from EmployeeMaster E left join CompanyMaster C on E.Emp_Company=C.CompanyCode left join BranchMaster B on E.Emp_Branch=B.BranchCode left join DesigMaster D on E.Emp_Designation=D.DesigCode left join DeptMaster D1  on E.Emp_Department=D1.DeptCode    left join EmployeeCategoryMaster EC on E.Emp_Employee_Category=EC.EmpCategoryCode left join Shift S on E.Emp_Shift_Detail =S.Shift_Code where e.emp_code in (select Empid From FetchEmployees('" + employee_id + "','') ) and e.Emp_Status = '1' ";

        return query;
    }

    [WebMethod]
    public static ReturnObject GetEmployeeData(int page_number, bool is_filter, string filters)
    {
        leave_assign page_object = new leave_assign();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable employee_data = new DataTable();

        string query = string.Empty;
        int start_row = 0, number_of_record = 0;

        try
        {
            //set rows count
            start_row = (page_number - 1) * 30;
            number_of_record = page_number * 30 + 1;

            //get base query
            query = page_object.GetEmployeeBaseQuery();

            //append filter query
            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            //append paging
            query += " ) Employees where row > " + start_row + " and row < " + number_of_record;

            employee_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employee_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_EMPLOYEE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Employee Data. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData()
    {
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        string employee_id = HttpContext.Current.Session["employee_id"].ToString();
        string query = string.Empty;

        try
        {
            if (employee_id == "")
                query = "select distinct CompanyName, CompanyCode from companymaster";
            else
                query = "select CompanyCode, CompanyName from companymaster where companycode = (select emp_company from employeemaster where emp_Code = '" + employee_id + "')";

            company_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(company_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_COMPANY_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company Data. Please try again. If the error persists, please contact Support.";
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

            query = "select BranchCode, BranchName from BranchMaster where CompanyCode='" + company_code + "' order by BranchName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "branch";
            return_data_set.Tables.Add(temp_data_table);

            query = "Select DeptCode, DeptName from DeptMaster where CompanyCode='" + company_code + "' order by DeptName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "department";
            return_data_set.Tables.Add(temp_data_table);

            query = "Select distinct(DesigCode), DesigName from DesigMaster where CompanyCode='" + company_code + "' order by DesigName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "designation";
            return_data_set.Tables.Add(temp_data_table);

            query = "Select distinct EmpCategoryCode, EmpCategoryName from employeecategorymaster where CompanyCode='" + company_code + "' order by EmpCategoryName";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "category";
            return_data_set.Tables.Add(temp_data_table);

            query = "select Shift_Code, Shift_Desc from Shift where CompanyCode='" + company_code + "' order by Shift_Desc";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "shift";
            return_data_set.Tables.Add(temp_data_table);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_OTHER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Company data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetLeavesForEmployee(string employee_id, string employee_category)
    {
        leave_assign page_object = new leave_assign();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable leave_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select T1.LeaveCode, T1.LeaveName, L.Max_leaves, L.Leaves_applied, l.Leave_balance from ";
            query += " ( select * from LeaveMaster where (EmployeeCategoryCode = '" + employee_category + "' or EmployeeCategoryCode is null)) ";
            query += " T1 left join Employee_Leave L on L.Leave_code = T1.LeaveCode and Emp_code='" + employee_id + "' where  T1.LeaveCode not in ('CO','V','OD') order by LeaveCode ";

            leave_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leave_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "EMPLOYEE_LEAVES_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leaves for the selected Employee. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    private string LeaveAction(string employee_id, string leave_code, double max_leave, double leave_applied, double leave_balance)
    {
        leave_assign page_object = new leave_assign();
        DBConnection db_connection = new DBConnection();
        string query = string.Empty, status = string.Empty;
        int employee_leave_count = 0;

        try
        {
            query = "select count(*) from Employee_Leave where Emp_Code='" + employee_id + "' and Leave_code='" + leave_code + "'";
            employee_leave_count = db_connection.GetRecordCount(query);

            if (employee_leave_count > 0)
            {
                query = "update Employee_Leave set Max_leaves='" + max_leave + "', Leaves_applied='" + leave_applied + "', Leave_balance='" + leave_balance + "' where Emp_code='" + employee_id + "' and Leave_code='" + leave_code + "'";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                status = "Leaves Updated Successfully\n";
            }
            else
            {
                query = "insert into Employee_Leave (Emp_Code,Leave_Code,Max_leaves,Leaves_Applied ,leave_balance )  values('" + employee_id + "','" + leave_code + "','" + max_leave + "','" + leave_applied + "','" + leave_balance + "')";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
                status = "Leaves Inserted Successfully\n";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OTHER_DATA");
            status = "An error occurred while processing leaves assigned for Leave Code - " + leave_code + "\n";
        }
        return status;
    }

    [WebMethod]
    public static ReturnObject DoAction(string employee_id, string leave_data)
    {
        leave_assign page_object = new leave_assign();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        JArray processed_data = new JArray();

        string leave_code = string.Empty, response_status = string.Empty;
        double leave_balance = 0.0, max_leave = 0.0, leave_applied = 0.0;

        try
        {
            processed_data = JArray.Parse(leave_data);

            for (int i = 0; i < processed_data.Count; i++)
            {
                leave_code = Convert.ToString(processed_data[i]["leave_code"]);
                max_leave = Convert.ToDouble(processed_data[i]["max_leave"]);
                leave_applied = Convert.ToDouble(processed_data[i]["leave_applied"]);
                leave_balance = Convert.ToDouble(processed_data[i]["leave_balance"]);

                if (leave_applied <= max_leave)
                {
                    response_status += leave_code + " " + page_object.LeaveAction(employee_id, leave_code, max_leave, leave_applied, leave_balance);
                }
            }

            if (response_status == "")
                response_status = "Leaves assigned successfully!";

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(response_status, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_OTHER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while save Assigned Leaves data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    private DataTable ReturnExcelasDataTable(string file_name, string sheet_name)
    {
        DataTable leave_data = new DataTable();
        IXLWorksheet workSheet = null;

        try
        {
            //Open the Excel file using ClosedXML.
            using (XLWorkbook workBook = new XLWorkbook(file_name))
            {
                if (!string.IsNullOrEmpty(sheet_name))
                    workSheet = workBook.Worksheet(sheet_name);
                else
                    workSheet = workBook.Worksheet(1);

                //Loop through the Worksheet rows.
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            leave_data.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        leave_data.Rows.Add();
                        int i = 0;
                        foreach (IXLCell cell in row.Cells())
                        {
                            leave_data.Rows[leave_data.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_LEAVES");
        }
        finally
        {
            if (File.Exists(file_name))
                File.Delete(file_name);
        }

        return leave_data;
    }

    private static string[] GetEmployeeCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable employee_data = new DataTable();
        int access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        string current_user_id = HttpContext.Current.Session["employee_id"].ToString();
        string[] employee_code_array = null;
        string query = string.Empty;

        if (access_level == 1 || access_level == 3)
        {
            query = "select distinct (Emp_Code), Emp_Company from employeemaster where Emp_Status='1' and Emp_Code in (select Empid from [FetchEmployees] ('" + current_user_id + "','Active'))";   
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

    private static string[] GetLeaveCodes(string employee_code)
    {
        DBConnection db_connection = new DBConnection();
        string[] Leave_code_array = null;
        DataTable employee_data = new DataTable();
        string query = string.Empty;

        query = " select LeaveCode from LeaveMaster where EmployeeCategoryCode =(select Emp_Employee_Category From employeemaster where emp_code='" + employee_code + "') ";

        employee_data = db_connection.ReturnDataTable(query);

        Leave_code_array = new string[employee_data.Rows.Count];

        for (int i = 0; i < employee_data.Rows.Count; i++)
        {
            Leave_code_array[i] = employee_data.Rows[i]["LeaveCode"].ToString().ToUpper();
        }

        return Leave_code_array;
    }

    [WebMethod]
    public static ReturnObject DoImport(string file_name)
    {
        leave_assign page_object = new leave_assign();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable leave_data = new DataTable();

        string leave_code = string.Empty;
        string effectd_rows = string.Empty;
        string excelConnectionString = string.Empty;
        string upload_path = string.Empty;
        string ExcelFullPath = string.Empty;
        string employee_id = string.Empty;
        string[] rows = new string[2];
        string[] leave_codes_Master = null;
        string[] employee_codes_master = null;
        string Return_message = string.Empty;

        double max_leave = 0.0, leave_applied = 0.0;
        int row_number = 0;
        int total_rows = 0;

        bool
            leave_code_flag = true,
            max_leaves_flag = true;

        string row_inserted = string.Empty, row_rejected = string.Empty;

        try
        {
            upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            ExcelFullPath = HttpContext.Current.Server.MapPath("~/" + upload_path + "/" + file_name);

            leave_data = page_object.ReturnExcelasDataTable(ExcelFullPath, "");

            if (leave_data.Rows.Count > 0)
            {
                total_rows = leave_data.Rows.Count;
                employee_codes_master = GetEmployeeCodes();

                foreach (DataRow dReader in leave_data.Rows)
                {
                    row_number++;
                    if (row_number == 1)
                    {
                        continue;
                    }
                    employee_id = Convert.ToString(dReader["EMP_CODE"]).Trim();

                    if (!string.IsNullOrEmpty(employee_id))
                    {
                        if (Array.IndexOf(employee_codes_master, employee_id) >= 0)
                        {
                            leave_codes_Master = GetLeaveCodes(employee_id);

                            if (!string.IsNullOrEmpty(dReader["LEAVE_CODE"].ToString()))
                            {
                                leave_code = Convert.ToString(dReader["LEAVE_CODE"]);
                            }
                            else
                            {
                                Return_message += " Leave code cannot be empty on row: " + row_number + Environment.NewLine;
                                leave_code_flag = false;
                            }

                            
                            if (!string.IsNullOrEmpty(dReader["MAX_LEAVES"].ToString()))
                            {
                                max_leave = Convert.ToDouble(dReader["MAX_LEAVES"]);
                            }
                            else
                            {
                                Return_message += " Max Leaves cannot be empty on row: " + row_number + Environment.NewLine;
                                max_leaves_flag = false;
                            }

                            if (!string.IsNullOrEmpty(dReader["LEAVES_APPLIED"].ToString()))
                            {
                                leave_applied = Convert.ToDouble(dReader["LEAVES_APPLIED"]);
                            }
                            else
                            {
                                leave_applied = 0.0;
                            }

                            if (leave_code_flag && max_leaves_flag && leave_applied <= max_leave)
                            {
                                if (Array.IndexOf(leave_codes_Master, leave_code) >= 0)
                                {
                                    Hashtable hshParam = new Hashtable();
                                    hshParam.Add("piempcode", employee_id);
                                    hshParam.Add("pileavecode", leave_code);
                                    hshParam.Add("piMaxleave", max_leave);
                                    hshParam.Add("pileaveapplied", leave_applied);

                                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("spimportleave", hshParam);

                                    Return_message += leave_code + " Leave assigned successfully for employee " + employee_id + Environment.NewLine;
                                }
                                else
                                {
                                    Return_message += leave_code + " Leave does not belong for employee " + employee_id + Environment.NewLine;
                                }
                            }
                        }
                        else 
                        {
                            Return_message += " You don't have permission to assign leave for Employee Code " + employee_id + " on row: " + row_number + Environment.NewLine;       
                        }
                    }
                    else
                    {
                        Return_message += " Employee Code cannot be Empty on row: " + row_number + Environment.NewLine;
                    }

                    max_leaves_flag = true;
                    leave_code_flag = true;
                }

                rows[0] = total_rows.ToString();
                rows[1] = effectd_rows;

            }

            return_object.status = "success";
            //return_object.return_data = JsonConvert.SerializeObject(rows, Formatting.Indented);
            return_object.return_data = Return_message;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_FILE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Importing Leaves data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }



}
