
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;
using System.Diagnostics;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Globalization;

public partial class shift_roster : System.Web.UI.Page
{
    const string page = "SHIFT_ROSTER";

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

            message = "An error occurred while loading Sihft Roster page. Please try again. If the error persists, please contact Support.";

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
    public static ReturnObject GetAllCompanies()
    {
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DataTable companyData = new DataTable();
        string query = string.Empty, employee_id = string.Empty, company_code = string.Empty;

        try
        {
            employee_id = HttpContext.Current.Session["username"].ToString();

            //load company list as per employee
            if (employee_id != "admin")
            {
                query = "select emp_company from EmployeeMaster where Emp_Code='" + employee_id + "'";
                company_code = dbConnection.ExecuteQuery_WithReturnValueString(query);
                query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster where CompanyCode='" + company_code + "'";
            }
            else
            {
                query = "select CompanyCode as company_code, CompanyName as company_name from CompanyMaster";
            }
            companyData = dbConnection.ReturnDataTable(query);

            returnObject.status = "success";
            returnObject.return_data = JsonConvert.SerializeObject(companyData, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_COMPANY_DATA");

            returnObject.status = "error";
            returnObject.return_data = "An error occurred while loading Company Data. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return returnObject;
    }

    [WebMethod]
    public static ReturnObject GetCompanyData(string company_code)
    {
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DataSet returnData = new DataSet();
        DataTable tempData = new DataTable();
        string query = string.Empty;

        try
        {
            query = "select DeptName as department_name, DeptCode as department_code from DeptMaster where CompanyCode = '" + company_code + "' ";
            tempData = dbConnection.ReturnDataTable(query);
            tempData.TableName = "department";
            returnData.Tables.Add(tempData);

            query = "select DesigName as designation_name, DesigCode as designation_code from DesigMaster where CompanyCode = '" + company_code + "' ";
            tempData = dbConnection.ReturnDataTable(query);
            tempData.TableName = "designation";
            returnData.Tables.Add(tempData);

            query = "select EmpCategoryName employee_category_name, EmpCategoryCode as employee_category_code from EmployeeCategoryMaster where CompanyCode='" + company_code + "' ";
            tempData = dbConnection.ReturnDataTable(query);
            tempData.TableName = "employee_category";
            returnData.Tables.Add(tempData);

            returnObject.status = "success";
            returnObject.return_data = JsonConvert.SerializeObject(returnData, Formatting.Indented);

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "GET_OTHER_DATA");

            returnObject.status = "error";
            returnObject.return_data = "An error occurred while getting data for the selected Company. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return returnObject;
    }

    private string GetBaseQuery(string company_code)
    {
        string
            query = string.Empty,
            current_user_id = Session["username"].ToString();

        int access_level = Convert.ToInt32(Session["access_level"]);

        // this is the base query for getting employees
        query = "select Emp_Code as employee_code, Emp_Name as employee_name from EmployeeMaster where Emp_Company = '" + company_code + "' and Emp_Status = '1' ";

        if (access_level == 1 || access_level == 3)
        {
            query += " and emp_code in (select EmpID from [FetchEmployees] ('" + current_user_id + "','')) ";
        }


        return query;
    }

    private string GetFilterQuery(string filters, string query)
    {
        JObject filterOption = JObject.Parse(filters);
        string departmentCode = filterOption["filter_department"].ToString();
        string designationCode = filterOption["filter_designation"].ToString();
        string employeeCategoryCode = filterOption["filter_employee_category"].ToString();
        string employeeCode = filterOption["filter_employee_code"].ToString();
        string employeeName = filterOption["filter_employee_name"].ToString();

        if (departmentCode != "select")
        {
            query += " and Emp_Department = '" + departmentCode + "' ";
        }

        if (designationCode != "select")
        {
            query += " and Emp_Designation = '" + designationCode + "' ";
        }

        if (employeeCategoryCode != "select")
        {
            query += " and Emp_Employee_Category = '" + employeeCategoryCode + "' ";
        }

        if (!string.IsNullOrEmpty(employeeCode))
        {
            query += " and Emp_Code = '" + employeeCode + "' ";
        }

        if (!string.IsNullOrEmpty(employeeName))
        {
            query += " and Emp_Name like '%" + employeeName + "%' ";
        }

        return query;
    }

    [WebMethod]
    public static ReturnObject FilterEmployeeData(int page_number, string company_code, string filters)
    {
        shift_roster pageObject = new shift_roster();
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DataTable employeeData = new DataTable();
        string query = string.Empty;
        int offset = 0;

        try
        {
            // get the base query
            query = pageObject.GetBaseQuery(company_code);

            // setup the query to utilize the filters
            query = pageObject.GetFilterQuery(filters, query);

            offset = (page_number - 1) * 30;

            // add the offset and complete the query
            query += " ORDER BY Emp_Code OFFSET " + offset + " ROWS FETCH NEXT 30 ROWS ONLY";

            employeeData = dbConnection.ReturnDataTable(query);

            returnObject.status = "success";
            returnObject.return_data = JsonConvert.SerializeObject(employeeData, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "FILTER_EMPLOYEE_DATA");

            throw ex;
        }

        return returnObject;
    }

    [WebMethod]
    public static ReturnObject GetAllShifts(string company_code)
    {
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DataTable shiftData = new DataTable();
        string query = string.Empty;
        shift_roster pageObject = new shift_roster();
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            returnObject = pageObject.DoLogout();
        }
        else
        {
            try
            {
                query = "select ShiftCode as shift_code, ShiftDesc as shift_name from Shift where CompanyCode = '" + company_code + "' and IsActive=1 ";
                shiftData = dbConnection.ReturnDataTable(query);

                returnObject.status = "success";
                returnObject.return_data = JsonConvert.SerializeObject(shiftData, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "GET_ALL_SHIFTS");

                throw ex;
            }
        }

        return returnObject;
    }

    [WebMethod]
    public static ReturnObject GetShiftsForEmployees(string company_code, string date, string employees)
    {
        shift_roster pageObject = new shift_roster();
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DataSet shiftsData = new DataSet();
        DataTable tempShiftData = new DataTable();
        DateTime selected_date = new DateTime();
        List<string> employeeList = new List<string>();
        string query = string.Empty;

        int
            month = 0, year = 0, i = 0;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            returnObject = pageObject.DoLogout();
        }
        else
        {
            try
            {
                // getting a list of all the shifts in the system for the selected company.
                query = "select Shift_Code as shift_code, Shift_Desc as shift_name from Shift where CompanyCode = '" + company_code + "' and IsActive=1 ";
                tempShiftData = dbConnection.ReturnDataTable(query);
                // storing this shift in a data set
                tempShiftData.TableName = "all_shifts";
                shiftsData.Tables.Add(tempShiftData);

                // obtaining the selected month and year 
                selected_date = DateTime.ParseExact(date, "MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                month = selected_date.Month;
                year = selected_date.Year;

                // parse the array of employees
                employeeList = JsonConvert.DeserializeObject<List<string>>(employees);

                for (i = 0; i < employeeList.Count; i++)
                {
                    query = "select day1, day2, day3, day4, day5, day6, day7, day8, day9, day10, day11, day12, day13, day14, day15, day16, day17, day18, day19, day20, day21, day22, day23, day24, day25, day26, day27, day28, day29, day30, day31 from ShiftEmployee where Empid = '" + employeeList[i].ToString() + "' and month = '" + month + "' and year = '" + year + "' ";
                    tempShiftData = dbConnection.ReturnDataTable(query);

                    // storing shifts assigned for the employees in the data set.
                    // each employees shifts are namespaced by their employee code
                    tempShiftData.TableName = employeeList[i].ToString();
                    shiftsData.Tables.Add(tempShiftData);
                }

                returnObject.status = "success";
                returnObject.return_data = JsonConvert.SerializeObject(shiftsData, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "GET_SHIFTS_FOR_EMPLOYEE");

                throw ex;
            }
        }

        return returnObject;
    }

    private void UpdateDatabase(string mode, string employee_code, JToken shifts, int month, int year)
    {
        DBConnection db_connection = new DBConnection();
        Hashtable shift_roster = new Hashtable();

        int count = 0;

        string
            query = string.Empty, company_code = string.Empty,
                value = string.Empty, key = string.Empty;

        query = "select Emp_Company from EmployeeMaster where Emp_Code='" + employee_code + "'";
        company_code = db_connection.ExecuteQuery_WithReturnValueString(query);

        foreach (var shift in shifts)
        {
            key = "day" + (count + 1).ToString();
            switch (shift.ToString())
            {
                case "select":
                    value = null;
                    break;
                case "woff":
                    value = "woff";
                    break;
                default:
                    value = shift.ToString();
                    break;
            }

            shift_roster.Add(key, value);

            count++;
        }
        
        shift_roster.Add("Mode", mode);
        shift_roster.Add("EmpCompanyCode", employee_code);
        shift_roster.Add("empcode", employee_code);
        shift_roster.Add("month", month);
        shift_roster.Add("year", year);

        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ShiftRoster", shift_roster);
    }

    ReturnObject DoLogout()
    {
        ReturnObject return_object = new ReturnObject();
        return_object.status = "error";
        return_object.return_data = "Session Expired. Please Login to continue...";
        return return_object;

    }
    [WebMethod]
    public static ReturnObject SaveShiftRoster(string employees, string date, int totalShifts, int[] ArrayofWeekends)
    {
        bool isNoWeekOffRecord = false;
        shift_roster pageObject = new shift_roster();
        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DateTime selected_date = new DateTime();
        JArray employeeShifts = new JArray();

        int
            month = 0, year = 0, i = 0,
            count = 0;

        string
            query = string.Empty,
            employee_code = string.Empty;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            returnObject = pageObject.DoLogout();
        }
        else
        {
            try
            {
                // get month and year from the selected date.
                selected_date = DateTime.ParseExact(date, "MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                month = selected_date.Month;
                year = selected_date.Year;

                // parse employees and shifts object
                employeeShifts = JArray.Parse(employees);

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                int numberofdayoff = 0;
                string total_employees = null;
                 string employee_succes=null;
                 bool isWeekOfPresent = true;

                /*cheking week of in every week*/ //2,9,16,23,30
                 for (int j = 0; j < employeeShifts.Count; j++)
                 {


                     for (int m = 0; m < ArrayofWeekends.Length; m++)
                     {
                         int beginIndex = 0, endIndex = 0;
                         if (ArrayofWeekends[m] <= 5)
                         {
                             beginIndex = 0;
                             endIndex = ArrayofWeekends[m] - 1;
                         }
                         else
                         {
                             beginIndex = ArrayofWeekends[m] - 7;
                             endIndex = ArrayofWeekends[m] - 1;
                         }
                         int totalWoffCount = 0;


                         for (int k = beginIndex; k <= endIndex; k++)
                         {

                             if (employeeShifts[j]["shifts"][k].ToString() == "woff")
                             {
                                 totalWoffCount++;
                                 break;
                             }

                         }




                         if (totalWoffCount == 0)
                         {

                             isWeekOfPresent = false;
                             break;
                         }

                        
                     }

                     if (isWeekOfPresent == false)
                     {
                         break;
                     }
                    

                 }

                     if (isWeekOfPresent==true)
                 {
                     //ok
                //for (int j = 0; j < employeeShifts.Count; j++)
                //{
                //    for (int k = 0; k < totalShifts; k++)
                //    {
                //        if (employeeShifts[j]["shifts"][k].ToString() == "woff")
                //        {
                //            numberofdayoff += 1;
                //        }

                //    }


                //    if (numberofdayoff >= 4)
                //    {
                //        numberofdayoff = 0;
                //        if (employeeShifts.Count > 0)
                //        {
                //            for (i = 0; i < employeeShifts.Count; i++)
                //            {
                //                employee_code = employeeShifts[i]["employee_code"].ToString();

                //                query = "select count(*) from ShiftEmployee Where Empid = '" + employee_code + "'and month = '" + month + "'and year = '" + year + "' ";
                //                count = dbConnection.GetRecordCount(query);

                //                if (count > 0)
                //                {
                //                   // pageObject.UpdateDatabase("U", employee_code, employeeShifts[i]["shifts"], month, year);
                //                }
                //                else
                //                {
                //                    //  pageObject.UpdateDatabase("I", employee_code, employeeShifts[i]["shifts"], month, year);
                //                }
                //            }
                //            employee_succes += ","+employeeShifts[j]["employee_code"].ToString();
                //            returnObject.status = "success";
                //           // returnObject.return_data = "Shift Roster changes saved successfully! for Employee ID Please select Day off for other employees " + employee_succes ;

                //        }
                //        else
                //        {
                //            returnObject.status = "error";
                //            returnObject.return_data = "Please select atleast one checkbox!";
                //        }
                //    }
                //    else
                //    {
                //        isNoWeekOffRecord = true;
                //        total_employees += employeeShifts[j]["employee_code"].ToString()+","; 
                //        returnObject.status = "error";
                //        //returnObject.return_data = "Please select day off  for Employee Id " + total_employees+" Only selected employee having Dayoff are saved";

                //    }

                //}
                //if (isNoWeekOffRecord == true)
                //{
                //    returnObject.status = "error";
                //    returnObject.return_data = "Please select day off  for Employee Id  " + total_employees + " and  employees "+ employee_succes + " having Dayoff are saved";
                //}
                //else
                //{
                //    returnObject.status = "success";
                //    returnObject.return_data = "Shift Roster changes saved successfully!";
                //}
               //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  

                     if (employeeShifts.Count > 0)
                     {
                         for (i = 0; i < employeeShifts.Count; i++)
                         {
                             employee_code = employeeShifts[i]["employee_code"].ToString();

                             query = "select count(*) from ShiftEmployee Where Empid = '" + employee_code + "'and month = '" + month + "'and year = '" + year + "' ";
                             count = dbConnection.GetRecordCount(query);

                             if (count > 0)
                             {
                                pageObject.UpdateDatabase("U", employee_code, employeeShifts[i]["shifts"], month, year);
                             }
                             else
                             {
                                 pageObject.UpdateDatabase("I", employee_code, employeeShifts[i]["shifts"], month, year);
                             }
                         }

                         returnObject.status = "success";
                         returnObject.return_data = "Shift Roster changes saved successfully!";

                     }
                     else
                     {
                         returnObject.status = "error";
                         returnObject.return_data = "Please select atleast one checkbox!";
                     }
            }
                else
                 {
                     returnObject.status = "error";
                     returnObject.return_data = "Please select one week off per week";

                 }

            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "SAVE_SHIFTS");

                throw ex;
            }
        }

        return returnObject;
    }

    [WebMethod]
    public static ReturnObject valiadateShiftAssign(string employee_id, string date)
    {
        int returnFlag = 0;

        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        DataTable shiftData = new DataTable();
        string query = string.Empty;
        shift_roster pageObject = new shift_roster();
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            returnObject = pageObject.DoLogout();
        }
        else
        {
            try
            {

                DateTime temp = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string dateInString = temp.ToString("yyyy-MM-dd");
                int holidayRecordCount = 0, leaveRecordCount = 0;

                //checking employee record exist in holiday master

                query = "select count(*)  from holidaymaster  where holgrpcode = (select holidaycode from branchmaster where branchcode = ( select emp_branch from employeemaster where emp_code = '" + employee_id + "')) and '" + dateInString + "' between holfrom and holto";
                holidayRecordCount = dbConnection.GetRecordCount(query);
                if (holidayRecordCount > 0)
                {
                    returnFlag = 1;
                    returnObject.status = "error";
                    returnObject.return_data = returnFlag.ToString();
                }
                else
                {
                    //checking employee record exist in leave master

                    query = "select count(*)  from leave1 where empid='" + employee_id + "' and '" + dateInString + "' between startdate and enddate and flag=2";
                    leaveRecordCount = dbConnection.GetRecordCount(query);

                    if (leaveRecordCount <= 0)
                    {
                        returnObject.status = "success";
                        //returnObject.return_data = returnFlag.ToString();
                    }
                    else
                    {
                        returnFlag = 2;
                        returnObject.status = "error";
                        returnObject.return_data = returnFlag.ToString();

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogException(ex, page, "VALIDATE_SHIFT_ASSIGN");
                returnFlag = 3;
                returnObject.return_data = returnFlag.ToString();
                // throw ex;
            }
        }

        return returnObject;
    }






    [WebMethod]
    public static ReturnObject valiadateShiftHours(string shiftData, string weekFromDate, string weekToDate, string emp_id)
    {

        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        string query = string.Empty;
        shift_roster pageObject = new shift_roster();
        int WorkHourPerWeek = 0, flag = 0; ;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            returnObject = pageObject.DoLogout();
        }
        else
        {
            try
            {
                int finalTotalShiftHours = 0;

                DateTime weekFromDateConvertedToDate = DateTime.ParseExact(weekFromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string weekFromDateToString = weekFromDateConvertedToDate.ToString("yyyy-MM-dd");

                DateTime weekToDateConvertedToDate = DateTime.ParseExact(weekToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string weekToDateToString = weekToDateConvertedToDate.ToString("yyyy-MM-dd");

                string recordValue = "", TotalShiftHours = string.Empty, TotalShiftHoursTominutes = string.Empty;
                //getting all shift data by splitting by comma.
                string[] shiftDattaArray = shiftData.Split(',');
                if (Array.IndexOf(shiftDattaArray, "woff") >= 0)
                {

                    WorkHourPerWeek = 0;
                    for (int i = 0; i < shiftDattaArray.Length; i++)
                    {
                        DataTable shift_Data = new DataTable();
                        string currentShiftCode = shiftDattaArray[i];
                        if (!currentShiftCode.Equals("select") && !currentShiftCode.Equals("woff") && !currentShiftCode.Equals("leave") && !currentShiftCode.Equals("holiday"))
                        {
                            query = "select MaxOverTime_General from Shift where shift_code='" + currentShiftCode + "'and isActive=1";
                            TotalShiftHours = dbConnection.ExecuteQuery_WithReturnValueString(query);
                            TotalShiftHoursTominutes = Convert.ToDateTime(TotalShiftHours).ToString("HH:mm");
                            string[] splitHrsAndMinutes = TotalShiftHoursTominutes.Split(':');
                            int hours = Convert.ToInt32(splitHrsAndMinutes[0].Trim());
                            int minutes = Convert.ToInt32(splitHrsAndMinutes[1].Trim());
                            int finalHours = hours * 60 + minutes;//first converting hour to minutes + then adding minutes
                            finalHours = finalHours / 60;
                            finalTotalShiftHours = finalTotalShiftHours + finalHours;
                        }


                    }
                    query = "select transactiondata from EmployeeTransactionData where empid='" + emp_id + "' and Fromdate='" + weekFromDateToString + "'and Todate='" + weekToDateToString + "'and TransactionType=7 and isactive = 1";
                    WorkHourPerWeek = Convert.ToInt32(dbConnection.ExecuteQuery_WithReturnValueInteger(query));
                }
                else
                {
                    flag = 1;
                    returnObject.status = "error";
                    returnObject.return_data = flag.ToString();
                }

                if (flag == 0)
                {
                    returnObject.status = "success";
                    returnObject.return_data = finalTotalShiftHours.ToString() + "," + WorkHourPerWeek.ToString();
                }


                //returnObject.return_data = JsonConvert.SerializeObject(shiftData, Formatting.Indented);
            }
            catch (Exception ex)
            {
                flag = 3;
                Logger.LogException(ex, page, "VALIDATE_SHIFT_HOURS");
                returnObject.status = "error";
                returnObject.return_data = flag.ToString();
                //returnObject.return_data = "An error occurred while loading validating  shift data. Please try again. If the error persists, please contact Support.";

                //throw ex;
            }
        }

        return returnObject;
    }

    [WebMethod]
    public static ReturnObject valiadateMonthHours(string emp_id,int month)
    {

        DBConnection dbConnection = new DBConnection();
        ReturnObject returnObject = new ReturnObject();
        string query = string.Empty;
        shift_roster pageObject = new shift_roster();
        int  flag = 0;
        if (HttpContext.Current.Session["username"] == null)  // checking session expired or not 
        {
            returnObject = pageObject.DoLogout();
        }
        else
        {
            try
            {
                int totalMonthHours = 0;


                query = string.Format("select sum(cast(transactiondata as int))  from EmployeeTransactionData where EmpID = '{0}' and IsActive = 1 and TransactionType = 8  and datepart(Month, fromdate)={1}", emp_id, month);
                totalMonthHours = dbConnection.ExecuteQuery_WithReturnValueInteger(query);



                if (totalMonthHours>=0)
                {
                    returnObject.status = "success";
                    returnObject.return_data = totalMonthHours.ToString();
                }


                //returnObject.return_data = JsonConvert.SerializeObject(shiftData, Formatting.Indented);
            }
            catch (Exception ex)
            {
                flag = 1;
                Logger.LogException(ex, page, "VALIDATE_SHIFT_HOURS");
                returnObject.status = "error";
                returnObject.return_data = flag.ToString();
                //returnObject.return_data = "An error occurred while loading validating  shift data. Please try again. If the error persists, please contact Support.";

                //throw ex;
            }
        }

        return returnObject;
    }

}




