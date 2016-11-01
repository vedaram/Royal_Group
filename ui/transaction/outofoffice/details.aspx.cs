using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;
using System.Web.Services;

public partial class outofoffice_details : System.Web.UI.Page
{
    const string page = "OUT_OFF_OFFICE_DETAILS";

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

    private string GetBaseQuery(int access_level)
    {
        string query = " Select * from ( select OOO_ID,Emp_ID,O.Emp_Name,OOO_type_name as OOO_Type,CONVERT(date,FromDateTime) as FromDate, ";
        query += " substring(CONVERT(VARCHAR, FromDateTime, 114),1,5) as FromTime, CONVERT(date,ToDateTime) as ToDate, ";
        query += " substring(CONVERT(VARCHAR, ToDateTime, 114),1,5) as ToTime,Reason,OOO_Minutes, ";

        if (access_level == 1)
        {
            query += " O.Manager_Status as Status,";
        }
        if (access_level == 3)
        {
            query += " O.HR_Status as Status,";
            query += " O.Manager_Name as Manager_ID, O.Manager_Remark as Manager_Remark,";
        }
        if (access_level == 0)
        {
            query += " O.Status as Status,";
            query += " O.Manager_Name as Manager_ID, O.Manager_Remark as Manager_Remark,";
        }
        if (access_level==2)
        {
            query += " O.Status as Status,";
        }
                
        query += " CONVERT(VARCHAR, RIGHT('0' +  RTRIM(OOO_Minutes/60),3) + ':' + RIGHT('0' + RTRIM(OOO_Minutes%60),2)) as Hours, ";
        query += " CONVERT(VARCHAR, RIGHT('0' +  RTRIM(Total_OOO_Minutes/60),3) + ':' + RIGHT('0' + RTRIM(Total_OOO_Minutes%60),2)) as TotalHours, ";
        query += " ROW_NUMBER() OVER (ORDER BY ooo_id desc) as row from outoffoffice O join OOOType OT on O.OOO_type=OT.OOO_type_id  ";
        query += " join EmployeeMaster EM on O.Emp_ID=EM.Emp_Code where  1=1 ";
        return query;
    }

    private string GetFilterQuery(string filters, string query, int access_level)
    {
        JObject filters_data = JObject.Parse(filters);
        string from_date = filters_data["filter_indate"].ToString();
        string to_date = filters_data["filter_outdate"].ToString();
        string outofoffice_status = filters_data["filter_OutOffOffice"].ToString();
        string filter_OutOffOfficetype = filters_data["filter_OutOffOfficetype"].ToString();
        string filter_keyword = filters_data["filter_keyword"].ToString();
        int filter_by = Convert.ToInt32(filters_data["filter_by"]);
        

        switch (filter_by)
        {
            case 1:
                query += " and EM.Emp_Code='" + filter_keyword + "'";
                break;
            case 2:
                query += " and EM.Emp_Name like '%" + filter_keyword + "%'";
                break;
        }

        if (filter_OutOffOfficetype != "select")
        {
            query += " and OT.OOO_type_id=" + filter_OutOffOfficetype;
        }

        //==========Manual Pucnh Status==========
        if (outofoffice_status != "select")
        {
            if (access_level == 1)
            {
                query += " and O.Manager_Status='" + outofoffice_status + "'";
            }
            if (access_level == 3)
            {
                query += " and O.HR_Status='" + outofoffice_status + "'";
            }
            if (access_level == 0 || access_level == 2)
            {
                query += " and O.Status='" + outofoffice_status + "'";
            }
        }

        //========from date and to date============
        if (from_date != "")
        {
            from_date = "01-" + from_date;

            DateTime monthenddate = Convert.ToDateTime(from_date);
            monthenddate = monthenddate.AddMonths(1).AddDays(-1);
            to_date = monthenddate.ToString("dd-MMM-yyyy");
            query += " and FromDateTime>='" + from_date + " 00:00' and ToDateTime<='" + to_date + " 23:59'";
        }
        return query;
    }

    [WebMethod]
    public static ReturnObject GetOutOffOfficeData(int page_number, bool is_filter, string filters)
    {

        outofoffice_details page_object = new outofoffice_details();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable out_off_office_data_table = new DataTable();
        DataTable branch_list_data = new DataTable();
        DataTable CoManagerID_data = new DataTable();

        string user_name = string.Empty,
                employee_id = string.Empty,
                query = string.Empty,
                CoManagerID = string.Empty,
                BranchList = string.Empty;

        int user_access_level = 0,
            start_row = 0,
            number_of_record = 0;

        try
        {
            user_name = HttpContext.Current.Session["username"].ToString();
            employee_id = HttpContext.Current.Session["employee_id"].ToString();
            user_access_level = Convert.ToInt32(HttpContext.Current.Session["access_level"]);

            start_row = (page_number - 1) * 30;
            number_of_record = page_number * 30 + 1;

            //get list of branches assigned to logged in manager hr
            query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + employee_id + "'";
            branch_list_data = db_connection.ReturnDataTable(query);
            query = string.Empty;

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

            query = page_object.GetBaseQuery(user_access_level); //read base query
            
            if (user_access_level == 0)//admin login
            {
                query += " and  Emp_ID in (select Emp_Code from EmployeeMaster where Emp_Status=1)";
            }
            if (user_access_level == 1)//manager login 
            {
                query += " and (Emp_ID ='" + employee_id + "' or Emp_ID in (select Emp_Code from EmployeeMaster where ManagerID='" + employee_id + "' ))";
            }
            if (user_access_level == 2)//employee login
            {
                query += " and Emp_ID ='" + employee_id + "'";
            }
            if (user_access_level == 3)//HR login
            {
                query += " and Emp_ID in ( select Emp_Code from EmployeeMaster where Emp_Branch In (" + BranchList + "))";
            }
            
            if (is_filter)
            {
                query = page_object.GetFilterQuery(filters, query, user_access_level);
            }
            else
            {
                if (user_access_level == 3)
                {
                    query += " and O.HR_Status!=0";
                }
                if (user_access_level == 1)
                {
                    query += " and O.Manager_Status!=0";
                }
                
                DateTime date = DateTime.Now.Date;
                string from_date_temp = new DateTime(date.Year, date.Month, 1).ToString("dd-MMM-yyyy");
                string to_date_temp = Convert.ToDateTime(from_date_temp).AddMonths(1).AddDays(-1).ToString("dd-MMM-yyyy");

                query += " and FromDateTime>='" + from_date_temp + " 00:00' and ToDateTime<='" + to_date_temp + " 23:59'";
            }

            query += " ) a where row > " + start_row + " and row < " + number_of_record;
            query += "  order by OOO_ID";

            out_off_office_data_table = db_connection.ReturnDataTable(query);

            //out_off_office_data_table = page_object.CalculateTotalHours(out_off_office_data_table);
            
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(out_off_office_data_table, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OUT_OFF_OFFICE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
        }
        return return_object;
    }
}
