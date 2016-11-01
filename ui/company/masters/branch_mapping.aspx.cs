using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class masters_branch_mapping : System.Web.UI.Page
{
    const string page = "BRANCH_MAPPING_TO_HR";

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

            message = "An error occurred while loading Branch Mapping to HR page. Please try again. If the error persists, please contact Support.";

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
        string query = string.Empty;

        try
        {
            query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster";
            company_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(company_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_COMPANY_DATA");

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetBranchData(string manager_id, string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataSet return_data = new DataSet();
        DataTable branch_data = new DataTable();
        string query = string.Empty;
        int i = 0;

        try
        {
            #region Dispaly All Branches  to HR and Managers
            //query = "select DISTINCT BranchCode as branch_code, BranchName as branch_name from BranchMaster where CompanyCode = '" + company_code + "' "; 
            query = "select DISTINCT BranchCode as branch_code, BranchName as branch_name from BranchMaster";
            #endregion
            branch_data = db_connection.ReturnDataTable(query);
            branch_data.TableName = "all_branches";
            return_data.Tables.Add(branch_data);

            query = "select BranchCode from TbManagerHrBranchMapping where ManagerID = '" + manager_id + "' ";
            branch_data = db_connection.ReturnDataTable(query);
            branch_data.TableName = "selected_branches";
            return_data.Tables.Add(branch_data);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data, Formatting.Indented);
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
    public static ReturnObject GetManagerData(int filter, string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable manager_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select Emp_Code as employee_code, (Emp_Name+'('+Emp_Code+')') as employee_name from EmployeeMaster where Emp_Company = '" + company_code + "' ";

            switch (filter)
            { 
                case 0:
                    query += " and (Ismanager = 1 or IsHR = 1) ";
                    break;
                case 1:
                    query += " and IsHR = 1 ";
                    break;
                case 2:
                    query += " and Ismanager = 1 ";
                    break;
            }

            query += " order by Emp_Name ASC"; 

            manager_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(manager_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_MANAGER_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Manager Data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject SaveBranchMapping(string branches, string managers, string company_code)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        List<string> managers_list = new List<string>();
        List<string> branch_list = new List<string>();

        string query = string.Empty;

        int
            i = 0, j = 0, count = 0,
            total_rows_affected = 0;

        try
        {
            managers_list = JsonConvert.DeserializeObject<List<string>>(managers);
            branch_list = JsonConvert.DeserializeObject<List<string>>(branches);

            for (i = 0; i < managers_list.Count; i++)
            {
                query = "delete from TbManagerHrBranchMapping where ManagerID = '" + managers_list[i] + "' ";
                db_connection.ExecuteQuery_WithOutReturnValue(query);

                for (j = 0; j < branch_list.Count; j++)
                {
                    string newquery = "Select CompanyCode from BranchMaster where Branchcode='" + branch_list[j] + "'";
                    string branch_Company_code = db_connection.ExecuteQuery_WithReturnValueString(newquery);
                    query = "Select Count(*) from TbManagerHrBranchMapping Where ManagerID='" + managers_list[i] + "' And BranchCode='" + branch_list[j] + "'";
                    count = db_connection.ExecuteQuery_WithReturnValueInteger(query);

                    if (count > 0)
                    {
                        query = "update TbManagerHrBranchMapping set ManagerID = '" + managers_list[i] + "', BranchCode='" + branch_list[j] + "', CompanyCode = '" + branch_Company_code + "' where ManagerID = '" + managers_list[i] + "' And BranchCode = '" + branch_list[j] + "' and CompanyCode = '" + branch_Company_code + "' ";
                    }
                    else
                    {
                        query = "insert into TbManagerHrBranchMapping(ManagerID, BranchCode, CompanyCode) values('" + managers_list[i] + "','" + branch_list[j] + "', '" + branch_Company_code + "')";
                    }

                    total_rows_affected += db_connection.ExecuteQuery_WithReturnValueInteger(query);
                }
            }

            return_object.status = "success";
            return_object.return_data = "Branch Mapping to Manager/HR updated successfully!";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SAVE_BRANCH_MAPPING");

            return_object.status = "error";
            return_object.return_data = "An error occurred while saving Branch Mapping. Please try again. If the error persists, please contact Support.";

            throw;
        }
        
        return return_object;
    }

    [WebMethod]
    public static ReturnObject GetMappedBranches(string manager_id)
    {
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable branch_data = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select BranchCode from TbManagerHrBranchMapping where ManagerID = '" + manager_id + "' ";
            branch_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(branch_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_MAPPED_BRANCHES");

            throw;
        }

        return return_object;
    }
}
