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
using System.Globalization;
using System.Data.OleDb;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using SecurAX.Logger;
using SecurAX.Import.Excel;

public partial class masters_manage_employee : System.Web.UI.Page
{
    const string page = "MANAGE_EMPLOYEE";

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

            message = "An error occurred while loading Manage Employee page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetEmployeeDetails(string employee_code)
    {
        DataTable employee_data = new DataTable();
        ReturnObject return_object = new ReturnObject();
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;

        try
        {
            query = "select e.Emp_Code as employee_code, e.Emp_Name as employee_name, e.Emp_Card_No as enroll_id, CONVERT(varchar,e.Emp_Dob,103) as date_of_birth, CONVERT(varchar,e.Emp_Dol,103) as date_of_leaving, convert (varchar,e.Emp_Doj,103) as date_of_joining, e.Emp_Gender as gender, e.Emp_Address as address, e.Emp_Phone as phone_number, e.Emp_Email as email_address, e.Emp_Company as company_code, e.Emp_Branch as branch_code, e.Emp_Department as department_code, e.Emp_Designation as designation_code, e.Emp_Employee_Category as employee_category_code, e.Emp_Shift_Detail as shift_code, e.Emp_Status as employee_status, e.Emp_Photo, e.Passport_No as passport_number, CONVERT(varchar,e.Passport_Exp_Date,103) as passport_expiry, e.Emirates_No as emirates_number, e.Nationality as nationality, e.Emergency_Contact_No as emergency_contact_number, CONVERT(varchar,e.Visa_Exp_Date,103) as visa_expiry_date, e.IsManager, e.ManagerId as manager_id, e.IsHR, access as IsAdmin,  e.IsAutoShiftEligible as IsAutoShiftEligible , e.Employee_Religion as religion , e.ot_eligibility as ot_eligibility , e.Ramadan_Eligibility as Ramadan_Eligibility , e.PunchException_Eligibility as PunchException_Eligibility , e.WorkHourPerday_Eligibility as work_hours_day , e.WorkHourPerWeek_Eligibility as work_hours_week , e.WorkHourPerMonth_Eligibility as work_hours_month   from EmployeeMaster e, login l where l.Empid = e.Emp_Code and Emp_Code='" + employee_code + "'";

            employee_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(employee_data, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_EMPLOYEE_DATA");

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject saveEmployeeTransactionData(string returndataset)
    {

        masters_manage_employee page_object = new masters_manage_employee();
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
                if (employeedata.Rows.Count > 0)
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
    public static ReturnObject addEmployeeTransaction(string current)
    {
        // current = "
        masters_manage_employee page_object = new masters_manage_employee();
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
                return_object = masters_manage_employee.saveEmployeeTransactionData(current);
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

    [WebMethod]
    public static ReturnObject GetEmployeeTransactionData(string employee_code, string company_code, string branch_code)
    {
        masters_manage_employee page_object = new masters_manage_employee();
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

    //  Method Created for showing session expired message ...
    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }

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

    [WebMethod]
    public static ReturnObject getmanagerlist(string branch_code)
    {

        masters_manage_employee page_object = new masters_manage_employee();
        DBConnection db_connection = new DBConnection();
        DataTable temp_data_table = new DataTable();
        DataSet return_data_set = new DataSet();
        ReturnObject return_object = new ReturnObject();
        try
        {
            temp_data_table = page_object.GetManagerData(branch_code);
            temp_data_table.TableName = "manager";
            return_data_set.Tables.Add(temp_data_table);
            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(return_data_set, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "getbranchdata");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading data for  Branch. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally
        {
            page_object.Dispose();
        }
        return return_object;

    }
  
    protected DataTable GetManagerData(string company_code)
    {
        string branch_code = company_code;
        DBConnection db_connection = new DBConnection();
        DataTable manager_data = new DataTable();
        string employee_id, query = string.Empty;
        int access = 0;
        access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        employee_id = HttpContext.Current.Session["employee_id"].ToString();
        // if manager  is logged in then showing only that manager while addin new employee 
        if (access == 1)
        {
            // query = "select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name from employeemaster where IsManager = 1 and Emp_Company = '" + company_code + "' and emp_code = '" + employee_id + "'  order by Emp_Name";
            query = " Select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name  From EmployeeMaster E  where   IsManager=1 and E.Emp_code in (Select T.ManagerId from TbManagerHrBranchMapping T where T.Branchcode ='" + branch_code + "')";
        }
        else
        {
            // query = "select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name from employeemaster where IsManager = 1 and Emp_Company = '" + company_code + "' order by Emp_Name";
            query = " Select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name  From EmployeeMaster E  where  IsManager=1 and   E.Emp_code in (Select   T.ManagerId from TbManagerHrBranchMapping T where T.Branchcode ='" + branch_code + "')";

        }
        manager_data = db_connection.ReturnDataTable(query);
        return manager_data;
        //DBConnection db_connection = new DBConnection();
        //DataTable manager_data = new DataTable();
        //string employee_id, query = string.Empty;
        //int access = 0;
        //access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        //employee_id = HttpContext.Current.Session["employee_id"].ToString();
        //// if manager  is logged in then showing only that manager while addin new employee 
        //if (access == 1)
        //{
        //    query = "select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name from employeemaster where IsManager = 1 and Emp_Company = '" + company_code + "' and emp_code = '" + employee_id + "'  order by Emp_Name";
        //}
        //else
        //{
        //    query = "select Emp_Code as employee_code, (Emp_Name+' ('+Emp_Code+')') As employee_name from employeemaster where IsManager = 1 and Emp_Company = '" + company_code + "' order by Emp_Name";
        //}
        //manager_data = db_connection.ReturnDataTable(query);

        //return manager_data;
    }

    [WebMethod]
    public static ReturnObject GetOtherData(string company_code)
    {

        masters_manage_employee page_object = new masters_manage_employee();
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
                /* getting department data and adding it to Data set to be returned */
                query = "Select DeptName as department_name, DeptCode as department_code from DeptMaster where CompanyCode='" + company_code + "' order by DeptCode";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "department";
                return_data_set.Tables.Add(temp_data_table);

                /* getting designation data and adding it to Data set to be returned */
                query = "Select desigcode as designation_code, designame as designation_name from DesigMaster where CompanyCode='" + company_code + "' order by desigcode";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "designation";
                return_data_set.Tables.Add(temp_data_table);

                /* getting shift data and adding it to Data set to be returned */
                query = "select shift_code, shift_desc from shift where companycode = '" + company_code + "' and IsActive=1 order by shift_desc";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "shift";
                return_data_set.Tables.Add(temp_data_table);

                /* getting employee category data and adding it to Data set to be returned */
                query = "select empcategorycode as employee_category_code, empcategoryname as employee_category_name from employeecategorymaster where companycode = '" + company_code + "'  order by empcategoryname";
                temp_data_table = db_connection.ReturnDataTable(query);
                temp_data_table.TableName = "employee_category";
                return_data_set.Tables.Add(temp_data_table);

                /* get branch data and add it to the data set to be returned. */
                temp_data_table = page_object.GetBranchData(company_code);
                temp_data_table.TableName = "branch";
                return_data_set.Tables.Add(temp_data_table);

                #region  commeneted  if needed un comment and dispaly all manager according to comapny Selection . 
                /* get manager data and add it to the data set */
                temp_data_table = page_object.GetManagerDataCompany(company_code);
                temp_data_table.TableName = "manager";
                return_data_set.Tables.Add(temp_data_table);
                
                #endregion
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

    protected Hashtable prepareData(string current, string mode)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable employee_data = new Hashtable();
        JObject current_data = JObject.Parse(current);
        byte[] image = new byte[0];
        int
            is_manager = 0, is_HR_manager = 0, access = 0, user_type = 2;
        string
            branch_code = current_data["branch_code"].ToString(),
            department_code = current_data["department_code"].ToString(),
            designation_code = current_data["designation_code"].ToString(),
            shift_code = current_data["shift_code"].ToString(),
            employee_category_code = current_data["employee_category_code"].ToString(),
            manager_id = current_data["manager_id"].ToString(),
            religion = current_data["employee_religion"].ToString(),
            employee_status = string.Empty;

        DateTime
                date_of_leaving = Convert.ToDateTime("1900-01-01 00:00:00.000"),
                date_of_birth = Convert.ToDateTime("1900-01-01 00:00:00.000"),
                passport_expiry_date = Convert.ToDateTime("1900-01-01 00:00:00.000"),
                visa_expiry_date = Convert.ToDateTime("1900-01-01 00:00:00.000");

        
        

        if (current_data["user_type"].ToString() != "")
        {
            user_type = Convert.ToInt32(current_data["user_type"]);
        }

        // checking current role of the user to created
        if (user_type == 0)
        {
            access = 0;
        }
        else if (user_type == 1)
        {
            is_manager = 1;
            access = 1;
        }
        else if (user_type == 3)
        {
            is_HR_manager = 1;
            access = 3;
        }
        else
        {
            access = 2;
        }
        // setting default values for dropdowns 
        if (branch_code == "select")
            branch_code = "";

        if (department_code == "select")
            department_code = "";

        if (designation_code == "select")
            designation_code = "";

        if (employee_category_code == "select")
            employee_category_code = "";

        if (shift_code == "select")
            shift_code = "";

        if (manager_id == "select")
            manager_id = "";

        if (religion == "select")
            religion = "";

        if (current_data["passport_expiry"].ToString() != "" && current_data["visa_expiry"].ToString() != "")
        {
            passport_expiry_date = DateTime.ParseExact(current_data["passport_expiry"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            visa_expiry_date = DateTime.ParseExact(current_data["visa_expiry"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
        }

        if (current_data["date_of_leaving"].ToString() != "")
        {
            date_of_leaving = DateTime.ParseExact(current_data["date_of_leaving"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
        }

        if (current_data["date_of_birth"].ToString() != "")
        {
            date_of_birth = DateTime.ParseExact(current_data["date_of_birth"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
        }

        if (mode == "I")
        {
            if (DateTime.Compare(new DateTime(), date_of_leaving) > 0)
                employee_status = "0";
            else
                employee_status = current_data["employee_status"].ToString();
        }
        else if (mode == "U")
        {
            if (DateTime.Compare(DateTime.Now.Date, date_of_leaving) < 0)
            {
                string query = "Select COUNT(*) from terminatedemployee where empid='" + current_data["employee_code"].ToString() + "' ";
                int count = db_connection.ExecuteQuery_WithReturnValueInteger(query);
                if (count == 0)
                {
                    query = "insert into terminatedemployee (empid,date,status) values ('" + current_data["employee_code"].ToString() + "','" + date_of_leaving + "','" + current_data["employee_status"].ToString() + "')";

                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
                else
                {
                    query = "update terminatedemployee set date='" + date_of_leaving + "',status='" + current_data["employee_status"].ToString() + "' where empid='" + current_data["employee_code"].ToString() + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
                employee_status = "1";
            }
            else
            {
                employee_status = current_data["employee_status"].ToString();
            }
        }

        employee_data.Add("Mode", mode);
        employee_data.Add("Emp_Code", current_data["employee_code"].ToString());
        employee_data.Add("Emp_Name", current_data["employee_name"].ToString());
        employee_data.Add("Emp_Address", current_data["address"].ToString());
        employee_data.Add("Emp_Phone", current_data["phone_number"].ToString());
        employee_data.Add("Emp_Email", current_data["email_address"].ToString());
        employee_data.Add("Emp_Card_No", current_data["enroll_id"].ToString());
        employee_data.Add("Emp_Dob", date_of_birth);
        employee_data.Add("Emp_Doj", DateTime.ParseExact(current_data["date_of_joining"].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture));
        employee_data.Add("Emp_Dol", date_of_leaving);
        employee_data.Add("Emp_Company", current_data["company_code"].ToString());
        employee_data.Add("Emp_Branch", branch_code);
        employee_data.Add("Emp_Department", department_code);
        employee_data.Add("Emp_Designation", designation_code);
        employee_data.Add("Emp_Employee_Category", employee_category_code);
        employee_data.Add("Emp_Shift_Detail", shift_code);
        employee_data.Add("Emp_Gender", current_data["gender"].ToString());
        employee_data.Add("Emp_Status", employee_status);
        employee_data.Add("Emp_Photo", new byte[0]);
        employee_data.Add("OT_Eligibility", "0");
        employee_data.Add("Passport_No", current_data["passport_number"].ToString());
        employee_data.Add("Passprt_Exp_Date", passport_expiry_date);
        employee_data.Add("Emirates_No", current_data["emirates_number"].ToString());
        employee_data.Add("_Nationality", current_data["nationality"].ToString());
        employee_data.Add("Emergency_Contact_No", current_data["emergency_contact_number"].ToString());
        employee_data.Add("Visa_Exp_Date", visa_expiry_date);
        employee_data.Add("IsManager", is_manager);
        employee_data.Add("ManagerId", manager_id);
        employee_data.Add("IsHR", is_HR_manager);
        employee_data.Add("access", access);
        employee_data.Add("IsAutoShiftEligible", Convert.ToInt32(current_data["auto_checked"].ToString()));
        employee_data.Add("religion", religion);

        return employee_data;
    }

    private void UpdateDatabase(Hashtable data)
    {
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int access = Convert.ToInt32(data["access"]);
        int employee_status = Convert.ToInt32(data["Emp_Status"]);
        string employee_code = data["Emp_Code"].ToString();
        string mode = data["Mode"].ToString();
        string today, termination_date;
        DateTime date_of_leaving = new DateTime();

        data.Remove("access");

        db_connection.ExecuteStoredProcedure_EmployeeMaster("ManipulateEmployee", data);

        if (mode == "I")
        {
            query = "insert into login(Empid,UserName,Password,Access,status)values('" + employee_code + "','" + employee_code + "','password','" + access + "',1)";
            db_connection.ExecuteQuery_WithOutReturnValue(query);
            SetPrivilges(mode, employee_code, access);
        }
        else
        {
            query = "update login set access='" + access + "' where Empid='" + employee_code + "'";
            db_connection.ExecuteQuery_WithOutReturnValue(query);

            if (employee_status == 2 || employee_status == 3 || employee_status == 4)
            {
                DateTime systemDate = DateTime.Now;
                date_of_leaving = Convert.ToDateTime(data["Emp_Dol"]);

                if (systemDate.Date > date_of_leaving.Date || systemDate.Date == date_of_leaving.Date)
                {
                    query = "update login set status=0 where empid='" + employee_code + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
            }
            else
            {
                query = "update login set status=1 where empid='" + employee_code + "'";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }

            if (db_connection.RecordExist("select count(*) from EMPLOYEE_PERMISSIONS where EMPLOYEE_CODE='" + employee_code + "'"))
            {
                SetPrivilges(mode, employee_code, access);
            }
            else
            {
                mode = "I";
                SetPrivilges(mode, employee_code, access);
            }

            if (data["Emp_Status"].ToString() == "3")
            {
                termination_date = date_of_leaving.ToString("yyyy-MM-dd") + " 00:00:00";
                today = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                query = "update masterprocessdailydata set status = null where status = 'A' and emp_id = '" + employee_code + "' and Pdate between '" + termination_date + "' AND  '" + today + "'";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }
            if (data["Emp_Status"].ToString() == "2")
            {
                termination_date = date_of_leaving.ToString("yyyy-MM-dd") + " 00:00:00";
                today = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                query = "update masterprocessdailydata set status = null where status = 'A' and emp_id = '" + employee_code + "' and Pdate between '" + termination_date + "' AND  '" + today + "'";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }
        }
    }

    private int CheckEnrollID(string enroll_id)
    {
        DBConnection db_connection = new DBConnection();
        string query = string.Empty;
        int count = 0;

        query = "select count(*) from EmployeeMaster where Emp_Card_No = '" + enroll_id + "' and Emp_Status = 1 ";
        count = db_connection.GetRecordCount(query);

        return count;
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
    public static ReturnObject addEmployee(string current)
    {
        masters_manage_employee page_object = new masters_manage_employee();
        ReturnObject return_object = new ReturnObject();
        Hashtable prepared_data = new Hashtable();
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
                current_data = JObject.Parse(current);
                enroll_id = current_data["enroll_id"].ToString();
                employee_code = current_data["employee_code"].ToString();

                if (page_object.CheckEmployeeCode(employee_code) > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Employee Code has been taken. Please try again with a different Employee Code";
                }
                else if (page_object.CheckEnrollID(enroll_id) > 0)
                {
                    return_object.status = "error";
                    return_object.return_data = "Enrollment ID has been taken. Please try again with a different Enrollment ID";
                }
                else
                {
                    prepared_data = page_object.prepareData(current, "I");
                    page_object.UpdateDatabase(prepared_data);

                    return_object.status = "success";
                    return_object.return_data = "Employee added successfully!";
                }
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
    public static ReturnObject editEmployee(string current, string previous_enroll_id)
    {
        masters_manage_employee page_object = new masters_manage_employee();
        ReturnObject return_object = new ReturnObject();
        Hashtable prepared_data = new Hashtable();
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
                current_data = JObject.Parse(current);
                enroll_id = current_data["enroll_id"].ToString();
                employee_code = current_data["employee_code"].ToString();

                if (previous_enroll_id != enroll_id)
                {
                    if (page_object.CheckEnrollID(enroll_id) > 0)
                    {
                        return_object.status = "error";
                        return_object.return_data = "Enrollment ID has been taken. Please try again with a different Enrollment ID";

                        return return_object;
                    }

                }

                prepared_data = page_object.prepareData(current, "U");
                page_object.UpdateDatabase(prepared_data);

                return_object.status = "success";
                return_object.return_data = "Employee edited successfully!";

            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "EDIT_EMPLOYEE");

                throw;
            }
            finally
            {
                page_object.Dispose();
            }
        }

        return return_object;
    }

    private void SetPrivilges(string mode, string employee_code, int access)
    {
        //DataTable dt = new DataTable();
        Permissions permissions = new Permissions();
        Hashtable employee_data = new Hashtable();

        try
        {

            if (mode == "I")
            {
                if (access == 0)
                {
                    /*employee_data = setAdmin(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setAdmin(mode, employee_code);
                }
                if (access == 1)
                {
                    /*employee_data = setManager(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setManager(mode, employee_code);
                }
                if (access == 2)
                {
                    /*employee_data = setEmployee(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setEmployee(mode, employee_code);
                }
                if (access == 3)
                {
                    /*employee_data = setHRPrevilege(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setHRManager(mode, employee_code);
                }

            }
            if (mode == "U")
            {
                if (access == 0)
                {
                    /*employee_data = setAdmin(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setAdmin(mode, employee_code);
                }
                if (access == 1)
                {
                    /*employee_data = setManager(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setManager(mode, employee_code);
                }
                if (access == 2)
                {
                    /*employee_data = setEmployee(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setEmployee(mode, employee_code);
                }
                if (access == 3)
                {
                    /*employee_data = setHRPrevilege(mode, employee_code);
                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_data);*/
                    permissions.setHRManager(mode, employee_code);
                }

            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SET_PRIVILEGES");
        }
    }
    protected DataTable GetManagerDataCompany(string company_code)
    {
        DBConnection db_connection = new DBConnection();
        DataTable manager_data = new DataTable();
        string employee_id, query = string.Empty;
        int access = 0;
        access = Convert.ToInt32(HttpContext.Current.Session["access_level"]);
        employee_id = HttpContext.Current.Session["employee_id"].ToString();
        // if manager  is logged in then showing only that manager while addin new employee 
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
}