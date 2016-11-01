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
using System.Data.OleDb;
using System.IO;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class manual_details : System.Web.UI.Page
{
    const string page = "MANUAL_PUNCH_DETAILS";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex)
        {
            string message = "An error occurred while performing this operation. Please try again. If the issue persists, please contact Support.";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("showAlert('error','");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    private string GetBaseQuery()
    {
        // string query = "select id, Empcode, Emp_Name, WorkDate, outdate, InPunch, OutPunch, approve, Leave_Status_text, ReasonForManualPunch,Modifiedby from ( select pa.id, pa.Empcode, e.Emp_Name, pa.WorkDate, pa.outdate, pa.InPunch, pa.OutPunch, pa.approve, ls.Leave_Status_text, pa.ReasonForManualPunch,pa.modifiedby, ROW_NUMBER() OVER (ORDER BY pa.id desc) as row from PunchForApproval pa, EmployeeMaster e, Leave_status ls  where e.Emp_code = pa.Empcode and ls.Leave_Status_id = pa.approve  ";

        string query = "select id, Empcode, Emp_Name, WorkDate, outdate, InPunch, OutPunch, approve, Leave_Status_text, ReasonForManualPunch,Manager_remark,Hr_remark,Modifiedby from ( select pa.id, pa.Empcode, e.Emp_Name, pa.WorkDate, pa.outdate, pa.InPunch, pa.OutPunch, pa.approve, ls.Leave_Status_text, pa.ReasonForManualPunch,pa.Manager_remark,pa.Hr_remark,pa.Modifiedby, ROW_NUMBER() OVER (ORDER BY pa.id desc) as row from PunchForApproval pa, EmployeeMaster e, Leave_status ls  where e.Emp_code = pa.Empcode";
        int user_access_level = 0;
        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        // and ls.Leave_Status_id = pa.approve  "
        if (user_access_level == 3)//HR
        {
            query += " and ls.Leave_Status_id=pa.Hr_approval";
        }
        else if (user_access_level == 1)//manager
        {
            query += " and ls.Leave_Status_id=pa.manager_approval";
        }
        else//if employee
        {
            query += " and ls.Leave_Status_id = pa.approve";
        }


        return query;
    }

    private string GetFilterQuery(string filters, string query)
    {
        JObject filters_data = JObject.Parse(filters);
        string from_date = filters_data["filter_indate"].ToString();
        string to_date = filters_data["filter_outdate"].ToString();
        string manual_pucnh_status = filters_data["filter_ManualStatus"].ToString();
        string filter_keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);

        int user_access_level = 0;


        user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        if (user_access_level == 0)//Admin
        {
            query += " ";
        }
        else if (user_access_level == 3)//HR
        {
            if (manual_pucnh_status == "select")
            {
                query += " and pa.Hr_approval in(0,1,2,3)";
            }
            else
            {
                query += " and pa.Hr_approval='" + manual_pucnh_status + "'";
            }

        }
        else if (user_access_level == 1)//manager
        {

            if (manual_pucnh_status == "select")
            {
                query += " and pa.manager_approval in(0,1,2,3)";
            }
            else
            {
                query += " and pa.manager_approval='" + manual_pucnh_status + "'";
            }
        }
        else//for employee
        {


            if (manual_pucnh_status == "select")
            {
                query += " and pa.approve in(0,1,2,3)";
            }
            else
            {
                query += " and pa.approve='" + manual_pucnh_status + "'";
            }
        }



        switch (filter_by)
        {
            case 1:
                query += " and e.Emp_Code='" + filter_keyword + "'";
                break;
            case 2:
                query += " and e.Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

        //==========Manual Pucnh Status==========
        //if (manual_pucnh_status != "select")
        //    query += " and pa.approve='" + manual_pucnh_status + "'";

        //========from date and to date============
        if (from_date != "" && to_date != "")
        {
            query += " and pa.WorkDate between '" + from_date + "' and '" + to_date + "'";
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetManualPunchData(int page_number, bool is_filter, string filters)
    {
        manual_details page_object = new manual_details();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable manual_punch_data_table = new DataTable();
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
            IsDelegationManager = 0, flag = 0;

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

            query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + employee_id + "'";
            branch_list_data = db_connection.ReturnDataTable(query);
            query = string.Empty;

            query = page_object.GetBaseQuery(); //read main query

            if (branch_list_data.Rows.Count > 0)
            {
                foreach (DataRow dr in branch_list_data.Rows)
                {
                    BranchList += "'" + dr["BranchCode"] + "',";
                }

                BranchList = BranchList.TrimEnd(',');
            }
            else
            {
                BranchList = "'Empty'";
            }

            //Validate CoManagerID
            if (string.IsNullOrEmpty(CoManagerID))
            {
                CoManagerID = "'Empty'";
            }

            //modify query as per access level
            if (user_access_level == 0)//Admin
            {
                query += " ";
            }
            else if (user_access_level == 3)//HR
            {
                flag = 1;
                query += " and ((e.Emp_Code='" + employee_id + "') or (e.Emp_Code in ( Select Emp_code from Employeemaster where Emp_Branch In (" + BranchList + ")))) ";
                //query += " and (e.Emp_Code='" + employee_id + "' or e.Emp_Code in(Select Emp_code from Employeemaster where Emp_Branch =(Select Emp_branch from TbManagerHrBranchMapping where ManagerId='" + employee_id + "')))";

                // Select Emp_code from Employeemaster where Emp_Branch =(Select Emp_branch from Employeemaster where Emp_Code='23760')
            }
            else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "'Empty'")//Manager and CoManager
            {
                flag = 2;
                query += " and e.Emp_Code in ( select Emp_Code from EmployeeMaster where ((managerId in ('" + employee_id + "'," + CoManagerID + ") or Emp_Code='" + employee_id + "') and Emp_Status=1 )  or Emp_Branch in (" + BranchList + "))";
            }
            else if (user_access_level == 1 && CoManagerID == "'Empty'")//Only Manager
            {
                flag = 2;
                query += " and e.Emp_Code in ( select Emp_Code from EmployeeMaster where ((managerId='" + employee_id + "' or Emp_Code='" + employee_id + "')  and Emp_Status=1 ) or Emp_Branch in (" + BranchList + "))";
            }
            else
            {
                flag = 2;
                query += " and e.Emp_Code='" + employee_id + "'";// Only Employee
            }

            if (is_filter)
            {

                JObject current_data = new JObject();
                //{"filter_indate":"","filter_outdate":"","filter_ManualStatus":"2","filter_by":"0","filter_keyword":""}
                current_data = JObject.Parse(filters);
                string status = current_data["filter_ManualStatus"].ToString();

                query = page_object.GetFilterQuery(filters, query);
                //if (flag == 1)
                //{
                //    //query += "  and pa.Hr_approval=1 ";
                //     if (status == "3")
                //    {
                //        query += " and pa.Hr_approval=2";
                //    }
                //     else if (status == "2")
                //     {
                //         query += " and pa.manager_approval=0 ";
                //     }
                //     else if (status == "1")
                //     {
                //         query += " and pa.Hr_approval=1";
                //     }
                //}
                //else if (flag == 2)
                //{
                //    if (status == "select")
                //    {
                //        query += " and pa.manager_approval in(0,1) ";
                //    }
                //    else if (status == "2")
                //    {
                //        query += " and pa.manager_approval=0 ";
                //    }
                //    else if (status == "3")
                //    {
                //        query += " and pa.manager_approval=1 ";
                //    }
                //    else if (status == "1")
                //    {
                //        query += " and pa.manager_approval=1 ";
                //    }
                //}

                //else
                //{
                //    query += " ";
                //}
            }
            else
            {
                if (flag == 1)
                {
                    query += "  and pa.Hr_approval in(1,2,3)";

                }
                else if (flag == 2)
                {
                    // query += "  and pa.manager_approval=1 ";
                    query += "  and pa.manager_approval in(1,2,3)";
                }
                else
                {
                    query += "  and pa.approve in(1,2,3)";
                }

            }

            query += " ) a where row > " + start_row + " and row < " + number_of_record;
            query += "  order by a.workdate desc";

            manual_punch_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(manual_punch_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_MANUAL_PUNCHS");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }

    [WebMethod]
    public static string changeManualPunchStatus(string jsonData, int flag)
    {

        DBConnection db_connection = new DBConnection();
        Dictionary<string, string> return_data = new Dictionary<string, string>();

        return JsonConvert.SerializeObject(return_data, Formatting.Indented);
    }

    protected void PrintExcel(DataTable table)
    {

        string attach = "attachment;filename=ManualPunchDetails.xls";
        Response.ClearContent();
        Response.AddHeader("content-disposition", attach);
        Response.ContentType = "application/ms-excel";
        if (table != null)
        {
            foreach (DataColumn dc in table.Columns)
            {
                Response.Write(dc.ColumnName + "\t");

            }
            Response.Write(System.Environment.NewLine);
            foreach (DataRow dr in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Response.Write(dr[i].ToString() + "\t");
                }
                Response.Write("\n");
            }
            Response.End();
        }
    }

    protected void ExportToExcel(object sender, EventArgs e)
    {

        DBConnection db_connection = new DBConnection();
        DataTable manual_punch_data_table = new DataTable();
        string query = "";
        string employee_id = HttpContext.Current.Session["employee_id"].ToString();
        int user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

        if (user_access_level == 0)
        {
            query = "";
        }
        else
        {
            query = "";
        }

        manual_punch_data_table = db_connection.ReturnDataTable(query);
        PrintExcel(manual_punch_data_table);
    }
}