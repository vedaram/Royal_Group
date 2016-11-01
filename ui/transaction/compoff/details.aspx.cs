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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SecurAX.Logger;
using System.Web.Services;

public partial class compoff_details : System.Web.UI.Page
{
    const string page = "COMPOFF_DETAILS";

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

    protected string GetBaseQuery()
    {
        string query = @"SELECT compoff_id, EmpID, Emp_Name, fromdate, todate, Leave_Status_text,ApprovedByName, Flag FROM 
                    ( SELECT l.compoff_id, l.EmpID, e.Emp_Name, l.fromdate, l.todate, ls.Leave_Status_text, l.Flag,ApprovedByName, ROW_NUMBER() OVER (ORDER BY compoff_id desc)
                    as row FROM compoffdetails l, EmployeeMaster e, leave_status ls where e.Emp_Code = l.empid and ls.Leave_Status_id = l.Flag ";

        return query;
    }

    protected string GetFilterQuery(string filtersData, string query)
    {
        JObject filter_data = JObject.Parse(filtersData);

        string filter_keyword = filter_data["filter_keyword"].ToString();
        string filter_CompoffStatus = filter_data["filter_CompoffStatus"].ToString();
        int filter_by = Convert.ToInt32(filter_data["filter_by"]);
        string filter_indate = filter_data["filter_indate"].ToString();
        string filter_outdate = filter_data["filter_outdate"].ToString();
                
        if (filter_by == 1)
        {
            query += " and e.Emp_Code = '" + filter_keyword + "' ";
        }
        if (filter_by == 2)
        {
            query += " and e.Emp_Name like '%" + filter_keyword + "%' ";
        }

        if (filter_indate != "")
        {
            query += " and l.fromdate >= '" + filter_indate + "' ";
        }

        if (filter_outdate != "")
        {
            query += " and l.todate <= '" + filter_outdate + "' ";
        }

        if (filter_CompoffStatus !="select")
        {
            query += " and l.Flag = '" + filter_CompoffStatus + "' ";
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject GetCompOffData(int page_number, bool is_filter, string filters)
    {
        compoff_details page_object = new compoff_details();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable leave_listing = new DataTable();
        DataTable branch_list_data = new DataTable();

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
                CoManagerID = db_connection.ExecuteQuery_WithReturnValueString("Select ManagerId from TbAsignDelegation Where DelidationManagerID='" + employee_id + "' And DeliationStatus=1");

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
                CoManagerID = "Empty";
            }

            //modify query as per access level
            if (user_access_level == 0)//Admin
            {
                query += " ";
            }
            else if (user_access_level == 3)//HR
            {
                query += " and ( l.EmpID='" + employee_id + "' or e.Emp_Branch In(" + BranchList + ")) ";
            }
            else if (user_access_level == 1 && !string.IsNullOrEmpty(CoManagerID) && CoManagerID != "Empty")//Manager and CoManager
            {
                query += " and l.EmpID in ( select Emp_Code from EmployeeMaster where ((managerId in ('" + employee_id + "','" + CoManagerID + "') or Emp_Code='" + employee_id + "') and Emp_Status=1 )  or Emp_Branch in (" + BranchList + "))";
            }
            else if (user_access_level == 1 && CoManagerID == "Empty")//Only Manager
            {
                query += " and l.EmpID in ( select Emp_Code from EmployeeMaster where ((managerId='" + employee_id + "' or Emp_Code='" + employee_id + "')  and Emp_Status=1 ) or Emp_Branch in (" + BranchList + "))";
            }
            else
            {
                query += " and l.EmpID='" + employee_id + "'";// Only Employee
            }

            if (is_filter)
                query = page_object.GetFilterQuery(filters, query);

            query += " ) a where row > " + start_row + " and row < " + number_of_record;
            query += "  order by a.fromdate desc";

            leave_listing = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(leave_listing, Formatting.Indented);
        }
        catch (Exception Ex)
        {
            Logger.LogException(Ex, page, "GET_COMP_OFF_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Leave Type data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }
}
