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
using System.Text;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class attendance_reprocess : System.Web.UI.Page
{
    const string page = "REPROCESS_DATA";

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

            message = "An error occurred while loading Reprocess Data page. Please try again. If the error persists, please contact Support.";

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

            // load company list as per employee
            if (employee_id != "admin")
            {
                query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                company_code = db_connection.ExecuteQuery_WithReturnValueString(query);
                query = "select CompanyCode , CompanyName  from CompanyMaster where CompanyCode='" + company_code + "' order by CompanyName ";
            }
            else
            {
                query = "select CompanyCode , CompanyName  from CompanyMaster";
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

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable temp_data_table = new DataTable();
        DataTable branch_data = new DataTable();
        DataSet return_data_set = new DataSet();
        string query = string.Empty;
        string branch_list = string.Empty;
        int access_level = 0;

        try
        {

            query = "Select DeptCode, DeptName from DeptMaster where CompanyCode='" + company_code + "' order by DeptCode";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "department";
            return_data_set.Tables.Add(temp_data_table);

            query = "select DesigCode, DesigName from DesigMaster where CompanyCode='" + company_code + "' order by DesigCode";
            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "designation";
            return_data_set.Tables.Add(temp_data_table);

            // Checking for delegation manager and assigned branches
            access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

            if (access_level == 1 || access_level == 3)
            {
                query = "Select DISTINCT tb.BranchCode, b.BranchName From TbManagerHrBranchMapping tb, BranchMaster b Where tb.BranchCode = b.BranchCode and tb.ManagerID = '" + HttpContext.Current.Session["employee_id"].ToString() + "' ";
            }
            else
            {
                query = "select DISTINCT BranchCode, BranchName from BranchMaster where CompanyCode='" + company_code + "' order by BranchCode";
            }


            temp_data_table = db_connection.ReturnDataTable(query);
            temp_data_table.TableName = "branch";
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

    private string GetBaseQuery()
    {
        string query = "select DISTINCT emp_code, emp_name from (select e.emp_code, e.emp_name, ROW_NUMBER() OVER (ORDER BY e.emp_code) as row from employeemaster e left join companymaster c on e.emp_company = c.companycode left join branchmaster b on e.emp_branch = b.branchcode left join deptmaster d on e.emp_department = d.deptcode left join desigmaster de on e.emp_designation = de.desigcode where e.emp_status = 1 ";

        return query;
    }

    private string GetFilterQuery(string filters, string query)
    {
        JObject filters_data = JObject.Parse(filters);

        string
            company_code = filters_data["filter_company"].ToString(),
            branch_code = filters_data["filter_branch"].ToString(),
            department_code = filters_data["filter_department"].ToString(),
            designation_code = filters_data["filter_designation"].ToString(),
            keyword = filters_data["filter_keyword"].ToString();

        int
            filter_by = Convert.ToInt32(filters_data["filter_by"]);

        if (company_code != "select")
            query += " and e.emp_company = '" + company_code + "' ";

        if (branch_code != "select")
            query += " and emp_branch = '" + branch_code + "' ";

        if (department_code != "select")
            query += " and emp_department = '" + department_code + "' ";

        if (designation_code != "select")
            query += " and emp_Designation = '" + designation_code + "' ";

        if (keyword != "")
        {
            switch (filter_by)
            {
                case 1:
                    query += " and emp_code = '" + keyword + "' ";
                    break;

                case 2:
                    query += " and emp_name like '%" + keyword + "%' ";
                    break;

                case 3:
                    query += " and emp_card_no = '" + keyword + "'";
                    break;
            }
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetReprocessData(int page_number, string filters)
    {
        attendance_reprocess page_object = new attendance_reprocess();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable reprocess_data = new DataTable();
        DataTable coManager_data = new DataTable();
        string query = string.Empty,
            comanager_id = string.Empty,
            branch_list = string.Empty,
            employee_code = string.Empty;

        int
            start_row = (page_number - 1) * 30,
            number_of_rows = page_number * 30 + 1,
            access, count = 0;

        try
        {
            query = page_object.GetBaseQuery();
            query = page_object.GetFilterQuery(filters, query);

            access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
            employee_code = HttpContext.Current.Session["username"].ToString();

            if (access == 1 || access == 3)
            {
                //If logged in manager is a delegation manager for any manager, Get the ManagerID to fetch details of related employee
                comanager_id = null;
                count = db_connection.ExecuteQuery_WithReturnValueInteger("select COUNT(DelidationManagerID) from TbAsignDelegation where DelidationManagerID = '" + employee_code + "' and DeliationStatus = 1 and Convert(date,Getdate()) >= Convert(date,Fromdate) and Convert(date,Getdate()) <= Convert(date,Todate)");

                if (count > 0)
                {
                    coManager_data = db_connection.ReturnDataTable("select ManagerId from TbAsignDelegation where DelidationManagerID = '" + employee_code + "' And DeliationStatus = 1");
                    if (coManager_data.Rows.Count > 0)
                    {
                        foreach (DataRow dr in coManager_data.Rows)
                        {
                            comanager_id += "'" + dr["ManagerId"] + "',";
                        }

                        comanager_id = comanager_id.TrimEnd(',');
                    }
                }

                if (string.IsNullOrEmpty(comanager_id))
                {
                    comanager_id = "'Empty'";
                }

                //get list of branches assigned to logged in manager hr
                branch_list = "'Empty',";

                DataTable branch_data = db_connection.ReturnDataTable("select BranchCode from TbManagerHrBranchMapping where ManagerID = '" + employee_code + "' ");
                if (branch_data.Rows.Count > 0)
                {
                    foreach (DataRow branch in branch_data.Rows)
                    {
                        branch_list += "'" + branch["BranchCode"] + "',";
                    }
                }

                branch_list = branch_list.TrimEnd(',');

                query += " and (Emp_Branch In (" + branch_list + ") Or ManagerID In('" + employee_code + "'," + comanager_id + ") ) ";
            }

            query += " ) a where row > " + start_row + " and row < " + number_of_rows;

            reprocess_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(reprocess_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_REPROCESS_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading the data. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }

    [WebMethod]
    public static ReturnObject DoReprocess(string employees, string from_date, string to_date)
    {
        attendance_reprocess page_object = new attendance_reprocess();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        Hashtable reprocessing_data = new Hashtable();
        DataTable process_flags = new DataTable();
        if (HttpContext.Current.Session["username"] == null)
        {
            // HttpContext.Current.Response.Redirect("~/logout.aspx", true);
            return_object = page_object.DoLogout();
        }
        else
        {
            List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);

            StringBuilder employee_id_string = new StringBuilder();

            DateTime
                from_date_time, to_date_time;

            string
                query = string.Empty;

            int
                i = 0, process_flag_1 = 0, process_flag_2 = 0;

            try
            {

                query = "select p.pflag, r.re_flag from ProcessingStatus p, ReprocessFlag r";
                process_flags = db_connection.ReturnDataTable(query);

                if (process_flags.Rows.Count > 0)
                {
                    process_flag_1 = Convert.ToInt32(process_flags.Rows[0]["pflag"]);
                    process_flag_2 = Convert.ToInt32(process_flags.Rows[0]["re_flag"]);
                }

                if (process_flag_1 == 1 || process_flag_2 == 1)
                {
                    return_object.status = "error";
                    return_object.return_data = "Processing is in progress, please wait.";
                }
                else
                {
                    for (i = 0; i < employees_list.Count; i++)
                    {
                        employee_id_string.Append(employees_list[i]);
                        employee_id_string.Append(",");
                    }

                    if (employee_id_string.ToString() == "")
                    {
                        return_object.status = "error";
                        return_object.return_data = "Please select at least one employee.";
                    }
                    else
                    {
                        employee_id_string.Remove(employee_id_string.Length - 1, 1);

                        from_date_time = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        to_date_time = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        reprocessing_data.Add("fromdate", from_date_time.ToString("yyyy-MM-dd"));
                        reprocessing_data.Add("todate", to_date_time.ToString("yyyy-MM-dd"));
                        reprocessing_data.Add("empid", employee_id_string.ToString());

                        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("securtimereprocess_Empid", reprocessing_data);

                        return_object.status = "success";
                        return_object.return_data = "Processing completed successfully!";
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.LogException(ex, page, "REPROCESS_DATA");

                return_object.status = "error";
                return_object.return_data = "An error occurred while reprocessing the data. Please try again. If the error persists, please contact Support.";

                throw;
            }
        }

        return return_object;
    }


}