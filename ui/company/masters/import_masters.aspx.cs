using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Web.Services;
using SecurAX.Logger;
using SecurAX.Import.Excel;
using System.Text.RegularExpressions;

public partial class masters_import_masters : System.Web.UI.Page
{
    const string page = "IMPORT_MASTERS";

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

            message = "An error occurred while loading Import Masters page. Please try again. If the error persists, please contact Support.";

            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    private List<string> GetLeaveCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable leave_data = new DataTable();
        List<string> leave_codes = new List<string>();
        string query = string.Empty;

        query = "select LeaveCode from LeaveMaster ";
        leave_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < leave_data.Rows.Count; i++)
        {
            leave_codes.Add(leave_data.Rows[i]["LeaveCode"].ToString());
        }

        return leave_codes;
    }

    private List<string> GetCompanyCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable company_data = new DataTable();
        List<string> company_codes = new List<string>();
        string query = string.Empty;

        query = "select CompanyCode from CompanyMaster ";
        company_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < company_data.Rows.Count; i++)
        {
            company_codes.Add(company_data.Rows[i]["CompanyCode"].ToString());
        }

        return company_codes;
    }

    private List<string> GetBranchCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable branch_data = new DataTable();
        List<string> branch_codes = new List<string>();
        string query = string.Empty;

        query = "select branchcode from BranchMaster";
        branch_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < branch_data.Rows.Count; i++)
        {
            branch_codes.Add(branch_data.Rows[i]["branchcode"].ToString());
        }

        return branch_codes;
    }

    private List<string> GetDepartmentCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable department_data = new DataTable();
        List<string> department_codes = new List<string>();
        string query = string.Empty;

        query = "select DeptCode from DeptMaster ";
        department_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < department_data.Rows.Count; i++)
        {
            department_codes.Add(department_data.Rows[i]["DeptCode"].ToString());
        }

        return department_codes;
    }

    private List<string> GetDesignationCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable designation_data = new DataTable();
        List<string> designation_codes = new List<string>();
        string query = string.Empty;

        query = "select DesigCode from DesigMaster ";
        designation_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < designation_data.Rows.Count; i++)
        {
            designation_codes.Add(designation_data.Rows[i]["DesigCode"].ToString());
        }

        return designation_codes;
    }

    private List<string> GetCategoryCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable category_data = new DataTable();
        List<string> category_codes = new List<string>();
        string query = string.Empty;

        query = "select empcategorycode from employeecategorymaster ";
        category_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < category_data.Rows.Count; i++)
        {
            category_codes.Add(category_data.Rows[i]["empcategorycode"].ToString());
        }

        return category_codes;
    }

    private List<string> GetShiftCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable shift_data = new DataTable();
        List<string> shift_codes = new List<string>();
        string query = string.Empty;

        query = "select Shift_Code from shift";
        shift_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < shift_data.Rows.Count; i++)
        {
            shift_codes.Add(shift_data.Rows[i]["Shift_Code"].ToString());
        }

        return shift_codes;
    }

    private List<string> GetCardNumbers()
    {
        DBConnection db_connection = new DBConnection();
        DataTable card_data = new DataTable();
        List<string> card_numbers = new List<string>();
        string query = string.Empty;

        query = "select Emp_Card_no from EmployeeMaster where Emp_Status=1 ";
        card_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < card_data.Rows.Count; i++)
        {
            card_numbers.Add(card_data.Rows[i]["Emp_Card_no"].ToString());
        }

        return card_numbers;
    }

    private List<string> GetHolidayGroupCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable holiday_group_data = new DataTable();
        List<string> holiday_group_codes = new List<string>();
        string query = string.Empty;

        query = "select holgrpcode from Holidaygroup";
        holiday_group_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < holiday_group_data.Rows.Count; i++)
        {
            holiday_group_codes.Add(holiday_group_data.Rows[i]["holgrpcode"].ToString());
        }

        return holiday_group_codes;
    }

    private List<string> GetHolidayListCodes()
    {
        DBConnection db_connection = new DBConnection();
        DataTable holiday_list_data = new DataTable();
        List<string> holiday_listArray = new List<string>();
        string query = string.Empty;

        query = "select holcode from HolidayListDetails ";
        holiday_list_data = db_connection.ReturnDataTable(query);

        for (int i = 0; i < holiday_list_data.Rows.Count; i++)
        {
            holiday_listArray.Add(holiday_list_data.Rows[i]["holcode"].ToString());
        }

        return holiday_listArray;
    }

    private void SetPrivilges(string mode, string empid, int Is_Manager)
    {
        Permissions permissions = new Permissions();

        try
        {
            //he is manager
            if (Is_Manager == 1)
            {
                //employee_params = setManager(mode, empid);
                //db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_params);
                permissions.setManager(mode, empid);
            }

            //he is HR manager
            if (Is_Manager == 3)
            {
                //employee_params = setManager(mode, empid);
                //db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_params);
                permissions.setHRManager(mode, empid);
            }

            //he is employee
            if (Is_Manager == 2)
            {
                //employee_params = setEmployee(mode, empid);
                //db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulatePrivilege_Employee", employee_params);
                permissions.setEmployee(mode, empid);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "SET_PRIVILEGES");
        }
    }

    [WebMethod]
    public static ReturnObject DoEmployeeImport(string file_name)
    {
        masters_import_masters page_object = new masters_import_masters();

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        DataTable employee_data = new DataTable();
        DataTable hr_branch_list_data = new DataTable();

        bool valid_flag = true, HrBranchFlag = false;
        int employee_count = 0, row_number = 2, AccessLevel = 0, is_manager = 0, LoginAccessLevel = 0;

        List<string> companyList = null, branchList = null, departmentList = null, designationList = null, employee_categoryList = null, shiftList = null
            , employee_cardList = null;

        string upload_path = string.Empty, ExcelFullPath = string.Empty, query = string.Empty, update_query = string.Empty,
             employee_id = string.Empty, employee_name = string.Empty, employee_card_no = string.Empty, email_id = string.Empty,
             employee_phone = string.Empty, employee_address = string.Empty, employee_gender = string.Empty, company_code = string.Empty,
             branch_code = string.Empty, department_code = string.Empty, designation_code = string.Empty, employee_category_code = string.Empty,
             shift_code = string.Empty, manager_id = string.Empty, return_message = string.Empty, current_user = string.Empty,
             employee_dob = string.Empty, employee_doj = string.Empty;

        DataRow first_row = null;

        try
        {
            current_user = HttpContext.Current.Session["username"].ToString();
            LoginAccessLevel = Convert.ToInt32(HttpContext.Current.Session["access_level"].ToString());

            upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            ExcelFullPath = HttpContext.Current.Server.MapPath("~/" + upload_path + "/" + file_name);

            //read data from  importexcel file 
            employee_data = ExcelImport.ImportExcelToDataTable(ExcelFullPath, "");

            //remove 2 row from excel sheet
            first_row = employee_data.Rows[0];
            employee_data.Rows.Remove(first_row);

            //check databatable rows count
            if (employee_data.Rows.Count > 0)
            {
                //read all master data from db into array s
                companyList = page_object.GetCompanyCodes();
                branchList = page_object.GetBranchCodes();
                departmentList = page_object.GetDepartmentCodes();
                designationList = page_object.GetDesignationCodes();
                shiftList = page_object.GetShiftCodes();
                employee_categoryList = page_object.GetCategoryCodes();
                //employee_cardList = page_object.GetCardNumbers();

                query = "Select BranchCode From TbManagerHrBranchMapping Where ManagerID='" + current_user + "'";
                hr_branch_list_data = db_connection.ReturnDataTable(query);

                foreach (DataRow row in employee_data.Rows)
                {
                    //row number while reading data from excel
                    row_number++;

                    //read employee Id
                    employee_id = row[0].ToString().Trim();

                    //check branch code with logged in user
                    if (hr_branch_list_data.Rows.Count > 0)
                    {
                        foreach (DataRow dr in hr_branch_list_data.Rows)
                        {
                            if (dr["BranchCode"].ToString() == branch_code)
                            {
                                HrBranchFlag = true;
                            }
                        }
                    }

                    //validate with DB
                    if (!string.IsNullOrEmpty(employee_id))
                    {
                        if (Regex.IsMatch(employee_id, @"^[a-zA-Z0-9]*$"))
                        {
                            //read all data from excel 
                            employee_name = row[1].ToString().Trim();
                            employee_card_no = row[2].ToString().Trim();
                            employee_dob = row[3].ToString().Trim();

                            try
                            {
                                // employee_dob = Convert.ToDateTime(row[3].ToString().Trim()).ToString("yyyy-MM-dd");

                                employee_doj = Convert.ToDateTime(row[4].ToString().Trim()).ToString("yyyy-MM-dd");
                            }
                            catch (Exception ex)
                            {
                                return_message += "Please ensure the Date format for Date of Joining & Date of Birth follows yyyy-mm-dd at row no " + row_number + Environment.NewLine;
                                continue;
                            }

                            company_code = row[5].ToString().Trim();
                            branch_code = row[6].ToString().Trim();
                            department_code = row[7].ToString().Trim();
                            designation_code = row[8].ToString().Trim();
                            employee_category_code = row[9].ToString().Trim();
                            employee_address = row[10].ToString().Trim();
                            employee_phone = row[11].ToString().Trim();
                            manager_id = row[13].ToString().Trim();
                            email_id = row[14].ToString().Trim();
                            shift_code = row[15].ToString().Trim();
                            employee_gender = row[17].ToString().Trim();

                            if (string.IsNullOrEmpty(row[12].ToString().Trim()))
                            {
                                is_manager = 0;
                            }
                            else
                            {
                                is_manager = Convert.ToInt32(Convert.ToString(row[12].ToString().Trim()));
                            }

                            //Company Code Validation
                            if (!string.IsNullOrEmpty(company_code))
                            {
                                if (!companyList.Exists(element => element == company_code))
                                {
                                    return_message += "Company code does not exist at row no " + row_number + Environment.NewLine;
                                    valid_flag = false;
                                }
                            }
                            else
                            {
                                return_message += "Company code has not been entered at row no " + row_number + Environment.NewLine;
                                valid_flag = false;
                            }

                            //Branch Code Validation
                            if (!string.IsNullOrEmpty(branch_code))
                            {
                                if (!branchList.Exists(element => element == branch_code))
                                {
                                    return_message += "Branch code does not exist at row no " + row_number + Environment.NewLine;
                                    valid_flag = false;
                                }
                            }
                            //else
                            //{
                            //    return_message += "Branch code has not been entered at row no " + row_number + Environment.NewLine;
                            //    valid_flag = false;
                            //}

                            //Department Code Validation
                            if (!string.IsNullOrEmpty(department_code))
                            {
                                if (!departmentList.Exists(element => element == department_code))
                                {
                                    return_message += "Department code does not exist at row no " + row_number + Environment.NewLine;
                                    valid_flag = false;
                                }
                            }
                            //else
                            //{
                            //    return_message += "Department code has not been entered at row no " + row_number + Environment.NewLine;
                            //    valid_flag = false;
                            //}

                            //Designation Code Validation
                            if (!string.IsNullOrEmpty(designation_code))
                            {
                                if (!designationList.Exists(element => element == designation_code))
                                {
                                    return_message += "Designation code does not exist at row no " + row_number + Environment.NewLine;
                                    valid_flag = false;
                                }
                            }
                            //else
                            //{
                            //    return_message += "Designation code has not been entered at row no " + row_number + Environment.NewLine;
                            //    valid_flag = false;
                            //}

                            //Shift Code Validation
                            if (!string.IsNullOrEmpty(shift_code))
                            {
                                if (!shiftList.Exists(element => element == shift_code))
                                {
                                    return_message += "Shift code does not exist at row no " + row_number + Environment.NewLine;
                                    valid_flag = false;
                                }
                            }
                            //else
                            //{
                            //    return_message += "Shift code has not been entered at row no " + row_number + Environment.NewLine;
                            //    valid_flag = false;
                            //}

                            //EmployeeCategory Code Validation
                            if (!string.IsNullOrEmpty(employee_category_code))
                            {
                                if (!employee_categoryList.Exists(element => element == employee_category_code))
                                {
                                    return_message += "EmployeeCategory code does not exist at row no " + row_number + Environment.NewLine;
                                    valid_flag = false;
                                }
                            }
                            else
                            {
                                return_message += "EmployeeCategory code has not been entered at row no " + row_number + Environment.NewLine;
                                valid_flag = false;
                            }

                            query = "select count(Emp_Code) from EmployeeMaster where Emp_Code='" + employee_id + "'and emp_status=1";
                            employee_count = db_connection.GetRecordCount(query);

                            //if employee_count=0 means insert or employee_count>0 means update record
                            if (employee_count > 0)
                            {
                                //EmployeeCardNumber Validation
                                if (!string.IsNullOrEmpty(employee_card_no))
                                {
                                    if (db_connection.GetRecordCount("select count(*) from EmployeeMaster where Emp_Card_No = '" + employee_card_no + "' and Emp_Code != '" + employee_id + "' ") > 0)
                                    {
                                        return_message += "EmployeeCard Number has been taken. row no " + row_number + Environment.NewLine;
                                        valid_flag = false;
                                    }
                                }
                                else
                                {

                                    employee_card_no = employee_id;
                                    //return_message += "EmployeeCard Number has not been entered at row no " + row_number + Environment.NewLine;
                                    //valid_flag = false;
                                    if (db_connection.GetRecordCount("select count(*) from EmployeeMaster where Emp_Card_No = '" + employee_card_no + "' and Emp_Code != '" + employee_id + "' ") > 0)
                                    {
                                        return_message += "EmployeeCard Number has been taken. row no " + row_number + Environment.NewLine;
                                        valid_flag = false;
                                    }
                                }

                                if (valid_flag == true)
                                {
                                    //update employee data
                                    query = @"Update EmployeeMaster set Emp_Code='" + employee_id + "'," + " Emp_Name='" + employee_name + "'," +
                                        " Emp_Card_No='" + employee_card_no + "'," + " Emp_DOB='" + employee_dob + "'," + " Emp_DOJ='" + employee_doj +
                                        "'," + " Emp_Company='" + company_code + "'," + " Emp_Branch='" + branch_code + "'," + " Emp_Department='" + department_code +
                                        "'," + " Emp_Designation='" + designation_code + "'," + " Emp_Employee_Category='" + employee_category_code + "'," +
                                        " Emp_Address='" + employee_address + "'," + " Emp_Phone='" + employee_phone + "'," + " IsManager='" + is_manager +
                                        "'," + " Managerid='" + manager_id + "'," + " Emp_Email='" + email_id + "'," + " Emp_Shift_Detail='" + shift_code +
                                        "'," + " Emp_Status='" + 1 + "'," + " Emp_gender='" + employee_gender + "'" + " where Emp_Code='" + employee_id + "'";

                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    //update access level
                                    if (Convert.ToInt32(is_manager) == 1)
                                    {
                                        AccessLevel = 1;
                                        page_object.SetPrivilges("U", employee_id, AccessLevel);
                                    }
                                    else if (Convert.ToInt32(is_manager) == 0)
                                    {
                                        AccessLevel = 2;
                                        page_object.SetPrivilges("U", employee_id, AccessLevel);
                                    }


                                    //Update login table also for access levels
                                    query = "update login set UserName='" + employee_id + "',Password='password',Access='" + AccessLevel + "',status='1' where Empid='" + employee_id + "'";
                                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                                    return_message += "Employee ID " + employee_id + " updated successfully" + Environment.NewLine;

                                    // remove the employee card no from the list
                                    //employee_cardList.Remove(employee_card_no);
                                    //employee_cardList.Add(employee_card_no);
                                }
                            }
                            else
                            {
                                //EmployeeCardNumber Validation
                                if (!string.IsNullOrEmpty(employee_card_no))
                                {
                                    if (db_connection.GetRecordCount("select count(*) from EmployeeMaster where Emp_Card_No = '" + employee_card_no + "' ") > 0)
                                    {
                                        return_message += "EmployeeCard Number has been taken. row no " + row_number + Environment.NewLine;
                                        valid_flag = false;
                                    }
                                }
                                else
                                {
                                    employee_card_no = employee_id;
                                    //return_message += "EmployeeCard Number has not been entered at row no " + row_number + Environment.NewLine;
                                    //valid_flag = false;
                                    if (db_connection.GetRecordCount("select count(*) from EmployeeMaster where Emp_Card_No = '" + employee_card_no + "' ") > 0)
                                    {
                                        return_message += "EmployeeCard Number has been taken. row no " + row_number + Environment.NewLine;
                                        valid_flag = false;
                                    }
                                }

                                if (valid_flag == true)
                                {
                                    if (HrBranchFlag == true || LoginAccessLevel == 0)
                                    {
                                        //insert employee master
                                        query = @"Insert into EmployeeMaster(Emp_Code,Emp_Name,Emp_Card_No,Emp_DOB,Emp_DOJ,Emp_Dol,Emp_Company,Emp_Branch,Emp_Department,
                                            Emp_Designation,Emp_Employee_Category,Emp_Address,Emp_Phone,Passport_Exp_Date,Visa_Exp_Date,IsManager,Managerid,Emp_Email,Emp_Shift_Detail,Emp_Status,
                                            Emp_gender)values('" + employee_id + "','" + employee_name + "','" + employee_card_no + "','" + employee_dob + "','" +
                                            employee_doj + "','1900-01-01','" + company_code + "','" + branch_code + "','" + department_code + "','" + designation_code + "','" +
                                            employee_category_code + "','" + employee_address + "','" + employee_phone + "','1900-01-01','1900-01-01','" + is_manager + "','" + manager_id +
                                            "','" + email_id + "','" + shift_code + "','" + 1 + "','" + employee_gender + "')";

                                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                                        // checking the access level before inserting
                                        if (Convert.ToInt32(is_manager) == 1)
                                        {
                                            AccessLevel = 1;
                                            page_object.SetPrivilges("I", employee_id, AccessLevel);
                                        }
                                        else if (Convert.ToInt32(is_manager) == 0)
                                        {
                                            AccessLevel = 2;
                                            page_object.SetPrivilges("I", employee_id, AccessLevel);
                                        }

                                        //insert login details
                                        query = "insert into login(Empid,UserName,Password,Access,status)values('" + employee_id + "','" + employee_id + "','password','" + AccessLevel + "','1')";
                                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                                        return_message += "Employee ID " + employee_id + " is inserted successfully at line no: " + row_number + Environment.NewLine;
                                        // Add the card no back to the list. This needs to be checked in each iteration to avoid duplicates.
                                        //employee_cardList.Add(employee_card_no);
                                    }
                                    else
                                    {
                                        return_message += "Branch_ID: " + branch_code + " for employee: " + employee_id + " Does Not Belongs to Manager/HrManager " + current_user + Environment.NewLine;
                                    }
                                }
                            }
                            // resetting the valid_flag 
                            valid_flag = true;
                            AccessLevel = 0;
                        }
                        else
                        {
                            return_message += "Employee ID is not valid on Line No: " + row_number;
                        }
                    }
                    else
                    {
                        return_message += "Employee ID is null or empty on Line No: " + row_number;
                    }

                }
                return_object.status = "success";
                return_object.return_data = return_message;
            }
            else
            {
                return_object.status = "error";
                return_object.return_data = "An error occurred while loading employee Data. Please try again. If the error persists, please contact Support.";
            }

        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_EMPLOYEE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while importing employee Data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    public bool ValidateCode(string code)
    {
        return Regex.IsMatch(code, @"^[a-zA-Z0-9_\-]*$");
    }

    public string DoCompanyMasterImport(DataTable company_data)
    {
        List<string> company_list = GetCompanyCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2;

        bool
            is_valid = true;

        string
            return_message = string.Empty,
            query = string.Empty,
            company_code = string.Empty,
            company_name = string.Empty;

        foreach (DataRow row in company_data.Rows)
        {
            company_code = row["Company Code"].ToString().Trim();
            company_name = row["Company Name"].ToString().Trim();

            if (!string.IsNullOrEmpty(company_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(company_code))
                {
                    return_message += "Company Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_name))
                {
                    return_message += "Company Name cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (company_list.Exists(element => element == company_code))
                    {
                        query = "update CompanyMaster set " + " CompanyName='" + company_name + "' where CompanyCode='" + company_code + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                        return_message += "Company details updated. Row Number: " + row_number + Environment.NewLine;
                    }
                    else
                    {
                        query = "insert into CompanyMaster(CompanyCode,CompanyName)values('" + company_code + "','" + company_name + "')";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        company_list.Add(company_code);

                        return_message += "Company details saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }

            }
            else
            {
                return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF COMPANY MASTER SHEET ******";

        return return_message;
    }

    public string DoHolidayGroupMasterImport(DataTable holiday_group_data)
    {
        List<string> company_list = GetCompanyCodes();
        List<string> holiday_group_list = GetHolidayGroupCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2,
            temp_number = 0;

        bool
            is_valid = true;

        string
            query = string.Empty,
            return_message = string.Empty,
            holiday_group_code = string.Empty,
            holiday_group_name = string.Empty,
            company_code = string.Empty,
            max_days = string.Empty,
            max_restricted_days = string.Empty;

        foreach (DataRow row in holiday_group_data.Rows)
        {
            holiday_group_code = row["Holiday Group Code"].ToString().Trim();
            holiday_group_name = row["Holiday Group Name"].ToString().Trim();
            company_code = row["Company Code"].ToString().Trim();
            max_days = row["Max Days"].ToString().Trim();
            max_restricted_days = row["Max Restricted Holiday Count"].ToString().Trim();

            if (!string.IsNullOrEmpty(holiday_group_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(holiday_group_code))
                {
                    return_message += "Holiday Group Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(holiday_group_name))
                {
                    return_message += "Holiday Group Name cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_code))
                {
                    return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!company_list.Exists(element => element == company_code))
                {
                    return_message += "Company Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(max_days))
                {
                    return_message += "Max Days cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!int.TryParse(max_days, out temp_number))
                {
                    return_message += "Max Days is not a numeric value. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(max_restricted_days))
                {
                    return_message += "Max Restricted Holidays Count cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!int.TryParse(max_restricted_days, out temp_number))
                {
                    return_message += "Max Restricted Holidays Count is not a numeric value. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (holiday_group_list.Exists(element => element == holiday_group_code))
                    {
                        query = "update HolidayGroup set " + "holgrpname = '" + holiday_group_name + "' , maxdays = '" + max_days + "', maxHolidayCount = '" + max_restricted_days + "' where holgrpcode='" + holiday_group_code + "' and companycode='" + company_code + "' ";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                        return_message += "Holiday Group updated. Row Number: " + row_number + Environment.NewLine;
                    }
                    else
                    {
                        query = "insert into HolidayGroup(holgrpcode, holgrpname, companycode, maxdays, maxHolidayCount)values('" + holiday_group_code + "','" + holiday_group_name + "','" + company_code + "', '" + max_days + "', '" + max_restricted_days + "' )";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        holiday_group_list.Add(holiday_group_code);

                        return_message += "Holiday Group saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }
            }
            else
            {
                return_message += "Holiday Group Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF HOLIDAY GROUP MASTER SHEET ******";

        return return_message;
    }

    public string DoHolidayMasterImport(DataTable holiday_data)
    {
        List<string> company_list = GetCompanyCodes();
        List<string> holiday_list = GetHolidayListCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2;

        bool
            is_valid = true;

        string
            query = string.Empty,
            return_message = string.Empty,
            holiday_code = string.Empty,
            holiday_name = string.Empty,
            company_code = string.Empty,
            holiday_type = string.Empty,
            from_date = string.Empty,
            to_date = string.Empty;

        foreach (DataRow row in holiday_data.Rows)
        {
            holiday_code = row["Holiday Code"].ToString().Trim();
            holiday_name = row["Holiday Name"].ToString().Trim();
            company_code = row["Company Code"].ToString().Trim();
            from_date = Convert.ToDateTime(row["Holiday From"].ToString().Trim()).ToString("dd-MMM-yyyy");
            to_date = Convert.ToDateTime(row["Holiday To"].ToString().Trim()).ToString("dd-MMM-yyyy");
            holiday_type = row["Holiday Type"].ToString().Trim();

            if (!string.IsNullOrEmpty(holiday_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(holiday_code))
                {
                    return_message += "Holiday Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(holiday_name))
                {
                    return_message += "Holiday Name cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_code))
                {
                    return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!company_list.Exists(element => element == company_code))
                {
                    return_message += "Company Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(from_date))
                {
                    return_message += "From Date cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(to_date))
                {
                    return_message += "To Date cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (Convert.ToDateTime(from_date) > Convert.ToDateTime(to_date))
                {
                    return_message += "From Date cannot be greater than To Date. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(holiday_type))
                {
                    return_message += "To Date cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (holiday_list.Exists(element => element == holiday_code))
                    {
                        query = "update HolidayListDetails set holname='" + holiday_name + "', holfrom = '" + from_date + "', holto = '" + to_date + "', holtype = '" + holiday_type + "' where holcode='" + holiday_code + "' and  companycode='" + company_code + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                        return_message += "HolidayList updated. Row Number: " + row_number + Environment.NewLine;
                    }
                    else
                    {
                        query = "insert into HolidayListDetails(holcode,holname,companycode, holfrom, holto, holtype)values('" + holiday_code + "','" + holiday_name + "','" + company_code + "', '" + from_date + "', '" + to_date + "', '" + holiday_type + "')";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        holiday_list.Add(holiday_code);

                        return_message += "Holiday saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }
            }
            else
            {
                return_message += "Holiday Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF HOLIDAY MASTER SHEET ******";

        return return_message;
    }

    public string DoBranchMasterImport(DataTable branch_data)
    {
        List<string> company_list = GetCompanyCodes();
        List<string> holiday_group_list = GetHolidayGroupCodes();
        List<string> branch_list = GetBranchCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2;

        bool
            is_valid = true;

        string
            query = string.Empty,
            return_message = string.Empty,
            branch_code = string.Empty,
            branch_name = string.Empty,
            company_code = string.Empty,
            holiday_group_code = string.Empty;

        foreach (DataRow row in branch_data.Rows)
        {
            branch_code = row["Branch Code"].ToString().Trim();
            branch_name = row["Branch Name"].ToString().Trim();
            company_code = row["Company Code"].ToString().Trim();
            holiday_group_code = row["Holiday Code"].ToString().Trim();

            if (!string.IsNullOrEmpty(branch_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(branch_code))
                {
                    return_message += "Branch Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(branch_name))
                {
                    return_message += "Branch Name cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_code))
                {
                    return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!company_list.Exists(element => element == company_code))
                {
                    return_message += "Company Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(holiday_group_code))
                {
                    return_message += "Holiday Group Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!holiday_group_list.Exists(element => element == holiday_group_code))
                {
                    return_message += "Holiday Group Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (branch_list.Exists(element => element == branch_code))
                    {
                        query = "select count(*) from BranchMaster where BranchCode='" + branch_code + "' and  CompanyCode='" + company_code + "' and HolidayCode = '" + holiday_group_code + "' ";

                        if (db_connection.RecordExist(query))
                        {
                            query = "update BranchMaster set  BranchName='" + branch_name + "', HolidayCode = '" + holiday_group_code + "' where  BranchCode='" + branch_code + "' and  CompanyCode='" + company_code + "'";
                            db_connection.ExecuteQuery_WithOutReturnValue(query);

                            return_message += "Branch details updated. Row Number: " + row_number + Environment.NewLine;
                        }
                        else
                        {
                            query = "insert into BranchMaster(BranchCode,BranchName,CompanyCode,HolidayCode)values('" + branch_code + "','" + branch_name + "','" + company_code + "', '" + holiday_group_code + "')";
                            db_connection.ExecuteQuery_WithOutReturnValue(query);
                            branch_list.Add(branch_code);

                            return_message += "Branch details saved. Row Number: " + row_number + Environment.NewLine;
                        }

                    }
                    else
                    {
                        query = "insert into BranchMaster(BranchCode,BranchName,CompanyCode,HolidayCode)values('" + branch_code + "','" + branch_name + "','" + company_code + "', '" + holiday_group_code + "')";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        branch_list.Add(branch_code);

                        return_message += "Branch details saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }
            }
            else
            {
                return_message += "Branch Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF BRANCH MASTER SHEET ******";

        return return_message;
    }

    public string DoDepartmentMasterImport(DataTable department_data)
    {
        List<string> company_list = GetCompanyCodes();
        List<string> department_list = GetDepartmentCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2;

        bool
            is_valid = true;

        string
            query = string.Empty,
            return_message = string.Empty,
            department_code = string.Empty,
            department_name = string.Empty,
            company_code = string.Empty;

        foreach (DataRow row in department_data.Rows)
        {
            department_code = row["Department Code"].ToString().Trim();
            department_name = row["Department Name"].ToString().Trim();
            company_code = row["Company Code"].ToString().Trim();

            if (!string.IsNullOrEmpty(department_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(department_code))
                {
                    return_message += "Department Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(department_name))
                {
                    return_message += "Department Name cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_code))
                {
                    return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!company_list.Exists(element => element == company_code))
                {
                    return_message += "Company Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (department_list.Exists(element => element == department_code))
                    {
                        query = "update DeptMaster set DeptName='" + department_name + "' where CompanyCode='" + company_code + "' and DeptCode='" + department_code + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                        return_message += "Department details updated. Row Number: " + row_number + Environment.NewLine;
                    }
                    else
                    {
                        query = "insert into DeptMaster(DeptCode,DeptName,CompanyCode)values('" + department_code + "','" + department_name + "','" + company_code + "')";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        department_list.Add(department_code);

                        return_message += "Department details saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }

            }
            else
            {
                return_message += "Department Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF DEPARTMENT MASTER SHEET ******";

        return return_message;
    }

    public string DoDesignationMasterImport(DataTable designation_data)
    {
        List<string> company_list = GetCompanyCodes();
        List<string> designation_list = GetDesignationCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2;

        bool
            is_valid = true;

        string
            query = string.Empty,
            return_message = string.Empty,
            designation_code = string.Empty,
            designation_name = string.Empty,
            company_code = string.Empty;

        foreach (DataRow row in designation_data.Rows)
        {
            designation_code = row["Designation Code"].ToString().Trim();
            designation_name = row["Designation Name"].ToString().Trim();
            company_code = row["Company Code"].ToString().Trim();

            if (!string.IsNullOrEmpty(designation_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(designation_code))
                {
                    return_message += "Designation Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(designation_name))
                {
                    return_message += "Designation Name cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_code))
                {
                    return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!company_list.Exists(element => element == company_code))
                {
                    return_message += "Company Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (designation_list.Exists(element => element == designation_code))
                    {
                        query = "update DesigMaster set DesigName='" + designation_name + "' where CompanyCode='" + company_code + "' and DeptCode='" + designation_code + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);

                        return_message += "Designation details updated. Row Number: " + row_number + Environment.NewLine;
                    }
                    else
                    {
                        query = "insert into DesigMaster(DesigCode, DesigName, CompanyCode) values ('" + designation_code + "','" + designation_name + "','" + company_code + "')";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        designation_list.Add(designation_code);

                        return_message += "Designation details saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }

            }
            else
            {
                return_message += "Designation Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF DESIGNATION MASTER SHEET ******";

        return return_message;
    }

    public string DoEmployeeCategoryMasterImport(DataTable employee_category_data)
    {
        List<string> company_list = GetCompanyCodes();
        List<string> employee_category_list = GetCategoryCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2;

        bool
            is_valid = true;

        string
            query = string.Empty,
            return_message = string.Empty,
            employee_category_code = string.Empty,
            employee_category_name = string.Empty,
            company_code = string.Empty,
            total_hours = string.Empty,
            include_process = string.Empty;

        foreach (DataRow row in employee_category_data.Rows)
        {
            employee_category_code = row["Employee Category Code"].ToString().Trim();
            employee_category_name = row["Employee Category Name"].ToString().Trim();
            company_code = row["Company Code"].ToString().Trim();
            total_hours = row["Total Hours"].ToString().Trim();
            include_process = row["Include Process"].ToString().Trim();

            if (!string.IsNullOrEmpty(employee_category_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(employee_category_code))
                {
                    return_message += "Employee Category Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(employee_category_name))
                {
                    return_message += "Employee Category Name cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_code))
                {
                    return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!company_list.Exists(element => element == company_code))
                {
                    return_message += "Company Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (employee_category_list.Exists(element => element == employee_category_code))
                    {
                        query = "update EmployeeCategoryMaster set EmpCategoryName='" + employee_category_name + "', Totalhrs = '" + total_hours + "', includeprocess = '" + include_process + "' where CompanyCode = '" + company_code + "' and  EmpCategoryCode='" + employee_category_code + "'";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        return_message += "Employee Category details updated. Row Number: " + row_number + Environment.NewLine;
                    }
                    else
                    {
                        query = "insert into EmployeeCategoryMaster(EmpCategoryCode,EmpCategoryName,CompanyCode,totalhrs,includeprocess)values('" + employee_category_code + "','" + employee_category_name + "','" + company_code + "','" + total_hours + "', '" + include_process + "')";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        employee_category_list.Add(employee_category_code);

                        return_message += "Employee Category saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }

            }
            else
            {
                return_message += "Employee Category Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF EMPLOYEE CATEGORY MASTER SHEET ******";

        return return_message;
    }

    public string DoLeaveMasterImport(DataTable leave_data)
    {
        List<string> company_list = GetCompanyCodes();
        List<string> employee_category_list = GetCategoryCodes();
        List<string> leave_list = GetLeaveCodes();
        DBConnection db_connection = new DBConnection();

        int
            row_number = 2;

        bool
            is_valid = true;

        string
            query = string.Empty,
            return_message = string.Empty,
            employee_category_code = string.Empty,
            leave_code = string.Empty,
            leave_name = string.Empty,
            company_code = string.Empty,
            max_leave = string.Empty,
            max_leave_carry_forward = string.Empty,
            week_off_flag = string.Empty;

        foreach (DataRow row in leave_data.Rows)
        {
            employee_category_code = row["Employee Category Code"].ToString().Trim();
            company_code = row["Company Code"].ToString().Trim();
            leave_code = row["Leave Code"].ToString().Trim();
            leave_name = row["Leave Name"].ToString().Trim();
            max_leave = row["Max Leave"].ToString().Trim();
            max_leave_carry_forward = row["Max Leave Carry Forward"].ToString().Trim();
            week_off_flag = row["Week Off Flag"].ToString().Trim();

            if (!string.IsNullOrEmpty(leave_code))
            {
                // validate the company code for special chars
                if (!ValidateCode(leave_code))
                {
                    return_message += "Leave Code is invalid. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(leave_name))
                {
                    return_message += "Leave Name is empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(employee_category_code))
                {
                    return_message += "Employee Category Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!employee_category_list.Exists(element => element == employee_category_code))
                {
                    return_message += "Employee Category Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (string.IsNullOrEmpty(company_code))
                {
                    return_message += "Company Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (!company_list.Exists(element => element == company_code))
                {
                    return_message += "Company Code doesn't exist. Row Number: " + row_number + System.Environment.NewLine;
                    is_valid = false;
                }

                if (is_valid)
                {
                    if (leave_list.Exists(element => element == leave_code))
                    {
                        query = "update LeaveMaster set LeaveName='" + leave_name + "', MaxLeave = '" + max_leave + "', MaxLeaveCarryForward = '" + max_leave_carry_forward + "', woflag = '" + week_off_flag + "' where CompanyCode = '" + company_code + "' and  EmployeeCategoryCode='" + employee_category_code + "' and LeaveCode = '" + leave_code + "' ";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        return_message += "Leave updated. Row Number: " + row_number + Environment.NewLine;
                    }
                    else
                    {
                        query = "insert into LeaveMaster(CompanyCode, EmployeeCategoryCode, LeaveCode, LeaveName, MaxLeave, MaxLeaveCarryForward, woflag) values ('" + company_code + "', '" + employee_category_code + "', '" + leave_code + "', '" + leave_name + "', '" + max_leave + "', '" + max_leave_carry_forward + "', '" + week_off_flag + "')";
                        db_connection.ExecuteQuery_WithOutReturnValue(query);
                        leave_list.Add(leave_code);

                        return_message += "Leave saved. Row Number: " + row_number + Environment.NewLine;
                    }
                }

            }
            else
            {
                return_message += "Leave Code cannot be empty. Row Number: " + row_number + System.Environment.NewLine;
            }

            is_valid = true;
            row_number++;
        }

        return_message += "****** END OF LEAVE MASTER SHEET ******";

        return return_message;
    }

    [WebMethod]
    public static ReturnObject DoCompanyImport(string file_name)
    {
        masters_import_masters page_object = new masters_import_masters();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        DataTable
            company_data = new DataTable(),
            holiday_data = new DataTable(),
            holiday_group_data = new DataTable(),
            branch_data = new DataTable(),
            department_data = new DataTable(),
            designation_data = new DataTable(),
            employee_category_data = new DataTable(),
            leave_data = new DataTable();

        string
            upload_path = string.Empty,
            excel_full_path = string.Empty,
            return_message = string.Empty;

        try
        {
            upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString();
            excel_full_path = HttpContext.Current.Server.MapPath("~/" + upload_path + "/" + file_name);

            return_message += System.Environment.NewLine + "Started At: " + DateTime.Now.ToString();

            company_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Company Master");
            holiday_group_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Holiday Group Master");
            holiday_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Holiday Master");
            branch_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Branch Master");
            department_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Department Master");
            designation_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Designation Master");
            employee_category_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Employee Category Master");
            leave_data = ExcelImport.ImportExcelToDataTable(excel_full_path, "Leave Master");

            return_message += page_object.DoCompanyMasterImport(company_data);
            return_message += page_object.DoHolidayGroupMasterImport(holiday_group_data);
            return_message += page_object.DoHolidayMasterImport(holiday_data);
            return_message += page_object.DoBranchMasterImport(branch_data);
            return_message += page_object.DoDepartmentMasterImport(department_data);
            return_message += page_object.DoDesignationMasterImport(designation_data);
            return_message += page_object.DoEmployeeCategoryMasterImport(employee_category_data);
            return_message += page_object.DoLeaveMasterImport(leave_data);

            return_message += System.Environment.NewLine + "Complete At: " + DateTime.Now.ToString();

            return_object.status = "success";
            return_object.return_data = return_message;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, page, "IMPORT_EMPLOYEE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading employee Data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    private Hashtable setManager(string mode, string empid)
    {
        Hashtable hash_params = new Hashtable();
        hash_params.Add("Holiday_Group_Master", "0");
        hash_params.Add("Transaction_Management", "1");
        hash_params.Add("Roster_Template", "1");
        hash_params.Add("OverTime_Approval", "0");
        hash_params.Add("On_Duty_Report", "1");
        hash_params.Add("Access_Permission", "0");
        hash_params.Add("Company_Data_Management", "1");
        hash_params.Add("Muster_Roll_Report", "1");
        hash_params.Add("OverTime_Application", "1");
        hash_params.Add("Salary_Report", "0");
        hash_params.Add("PayRoll_Report", "1");
        hash_params.Add("Device_Management", "0");
        hash_params.Add("OverTime_Details", "0");
        hash_params.Add("Empid", empid);
        hash_params.Add("Detailed_Monthly_TimeSheet", "1");
        hash_params.Add("Monthly_Overtime_Report", "1");
        hash_params.Add("License", "0");
        hash_params.Add("Leave_Card", "1");
        hash_params.Add("Payroll_Link_Report", "1");
        hash_params.Add("Leave_Details", "1");
        hash_params.Add("Employee_Master", "0");
        hash_params.Add("Branch_Master", "0");
        hash_params.Add("Download_and_Process", "0");
        hash_params.Add("Attandance_Data_Management", "0");
        hash_params.Add("Daily_Reports", "1");
        hash_params.Add("Notification", "0");
        hash_params.Add("Unprocessed_Data", "0");
        hash_params.Add("Set_Temporary_Report_File_Path", "0");
        hash_params.Add("Deductions", "0");
        hash_params.Add("Configuration_Settings", "0");
        hash_params.Add("Update", "1");
        hash_params.Add("Shift_RosterEmployee", "1");
        hash_params.Add("Apply_Comp_Off", "1");
        hash_params.Add("Leave_Assign", "0");
        hash_params.Add("Late_Comers", "1");
        hash_params.Add("Set_Shift_Roster_Template_Log_File_Path", "0");
        hash_params.Add("Request", "0");
        hash_params.Add("Early_Leavers", "1");
        hash_params.Add("Employee_Reports", "1");
        hash_params.Add("Template_Upload", "0");
        hash_params.Add("OD_Leave_Application", "1");
        hash_params.Add("Payroll", "0");
        hash_params.Add("Function_Key", "0");
        hash_params.Add("Individual_Payslips", "0");
        hash_params.Add("OT_Eligibility_Master", "0");
        hash_params.Add("Leave_Application", "1");
        hash_params.Add("Manual_Punch_Application", "1");
        hash_params.Add("Monthly_Salary_Data", "0");
        hash_params.Add("Salary_Details", "0");
        hash_params.Add("OD_Leave_Approval", "1");
        hash_params.Add("Set_Downloader_Log_File_Path", "0");
        hash_params.Add("Template_Download", "0");
        hash_params.Add("USB_Download_and_Process", "0");
        hash_params.Add("Manual_Entry", "1");
        hash_params.Add("Device_Information", "0");
        hash_params.Add("Log_Report", "0");
        hash_params.Add("Reports", "1");
        hash_params.Add("Shift_Roster_Template", "1");
        hash_params.Add("Import_Export_Template", "1");
        hash_params.Add("Daily_Employee_Punch_Report", "1");
        hash_params.Add("Leave_Approval", "1");
        hash_params.Add("Company_Master", "0");
        hash_params.Add("Creation", "1");
        hash_params.Add("Daily_Performance_Report", "1");
        hash_params.Add("Department_Master", "0");
        hash_params.Add("Account", "0");
        hash_params.Add("User_Management", "0");
        hash_params.Add("Set_Fingerprint_Template_File_Path", "0");
        hash_params.Add("Template_Management", "0");
        hash_params.Add("Daily_Overtime_Report", "1");
        hash_params.Add("Manual_Punch_Approval", "1");
        hash_params.Add("Break_Report", "0");
        hash_params.Add("Leave_Available", "1");
        hash_params.Add("Employee_List", "1");
        hash_params.Add("Regulatory_Report", "0");
        hash_params.Add("Shift_Roster", "1");
        hash_params.Add("Manual_Punch_Details", "1");
        hash_params.Add("Leave_Management", "1");
        hash_params.Add("Shift_Master", "1");
        hash_params.Add("Monthly_Reports", "1");
        hash_params.Add("Leave_Master", "0");
        hash_params.Add("Masters", "1");
        hash_params.Add("Comp_Off", "1");
        hash_params.Add("Employee_Category", "0");
        hash_params.Add("Enroll_Card", "0");
        hash_params.Add("Reprocess", "0");
        hash_params.Add("Leave_Register", "1");
        hash_params.Add("Approve_Comp_Off", "1");
        hash_params.Add("Daily_Attendance_Report", "1");
        hash_params.Add("Missing_Swipe", "1");
        hash_params.Add("Mode", mode);
        hash_params.Add("Daily_Performance_InOut_Report", "1");
        hash_params.Add("Holiday_Master", "0");
        hash_params.Add("Assignment", "1");
        hash_params.Add("Calculate_Pay_Details", "0");
        hash_params.Add("OT_Management", "0");
        hash_params.Add("Register", "0");
        hash_params.Add("OD_Leave_Details", "1");
        hash_params.Add("Shift_Settings", "0");
        hash_params.Add("Designation_Master", "0");
        hash_params.Add("DailyPerformancePerINOutReport", "0");
        hash_params.Add("Muster_Roll_Raw_Report", "1");
        hash_params.Add("Shift_Roster_Report", "1");

        return hash_params;
    }

    private Hashtable setEmployee(string mode, string empid)
    {
        Hashtable hash_params = new Hashtable();
        hash_params.Add("Holiday_Group_Master", "0");
        hash_params.Add("Transaction_Management", "1");
        hash_params.Add("Roster_Template", "1");
        hash_params.Add("OverTime_Approval", "0");
        hash_params.Add("On_Duty_Report", "1");
        hash_params.Add("Access_Permission", "0");
        hash_params.Add("Company_Data_Management", "0");
        hash_params.Add("Muster_Roll_Report", "1");
        hash_params.Add("OverTime_Application", "1");
        hash_params.Add("Salary_Report", "0");
        hash_params.Add("PayRoll_Report", "1");
        hash_params.Add("Device_Management", "0");
        hash_params.Add("OverTime_Details", "1");
        hash_params.Add("Empid", empid);
        hash_params.Add("Detailed_Monthly_TimeSheet", "1");
        hash_params.Add("Monthly_Overtime_Report", "1");
        hash_params.Add("License", "0");
        hash_params.Add("Leave_Card", "1");
        hash_params.Add("Payroll_Link_Report", "1");
        hash_params.Add("Leave_Details", "1");
        hash_params.Add("Employee_Master", "0");
        hash_params.Add("Branch_Master", "0");
        hash_params.Add("Download_and_Process", "0");
        hash_params.Add("Attandance_Data_Management", "0");
        hash_params.Add("Daily_Reports", "1");
        hash_params.Add("Notification", "0");
        hash_params.Add("Unprocessed_Data", "0");
        hash_params.Add("Set_Temporary_Report_File_Path", "0");
        hash_params.Add("Deductions", "0");
        hash_params.Add("Configuration_Settings", "0");
        hash_params.Add("Update", "0");
        hash_params.Add("Shift_RosterEmployee", "0");
        hash_params.Add("Apply_Comp_Off", "1");
        hash_params.Add("Leave_Assign", "0");
        hash_params.Add("Late_Comers", "1");
        hash_params.Add("Set_Shift_Roster_Template_Log_File_Path", "0");
        hash_params.Add("Request", "0");
        hash_params.Add("Early_Leavers", "1");
        hash_params.Add("Employee_Reports", "1");
        hash_params.Add("Template_Upload", "0");
        hash_params.Add("OD_Leave_Application", "1");
        hash_params.Add("Payroll", "0");
        hash_params.Add("Function_Key", "0");
        hash_params.Add("Individual_Payslips", "0");
        hash_params.Add("OT_Eligibility_Master", "0");
        hash_params.Add("Leave_Application", "1");
        hash_params.Add("Manual_Punch_Application", "1");
        hash_params.Add("Monthly_Salary_Data", "0");
        hash_params.Add("Salary_Details", "0");
        hash_params.Add("OD_Leave_Approval", "0");
        hash_params.Add("Set_Downloader_Log_File_Path", "0");
        hash_params.Add("Template_Download", "0");
        hash_params.Add("USB_Download_and_Process", "0");
        hash_params.Add("Manual_Entry", "1");
        hash_params.Add("Device_Information", "0");
        hash_params.Add("Log_Report", "0");
        hash_params.Add("Reports", "1");
        hash_params.Add("Shift_Roster_Template", "0");
        hash_params.Add("Import_Export_Template", "0");
        hash_params.Add("Daily_Employee_Punch_Report", "1");
        hash_params.Add("Leave_Approval", "0");
        hash_params.Add("Company_Master", "0");
        hash_params.Add("Creation", "0");
        hash_params.Add("Daily_Performance_Report", "1");
        hash_params.Add("Department_Master", "0");
        hash_params.Add("Account", "0");
        hash_params.Add("User_Management", "0");
        hash_params.Add("Set_Fingerprint_Template_File_Path", "0");
        hash_params.Add("Template_Management", "0");
        hash_params.Add("Daily_Overtime_Report", "1");
        hash_params.Add("Manual_Punch_Approval", "0");
        hash_params.Add("Break_Report", "0");
        hash_params.Add("Leave_Available", "1");
        hash_params.Add("Employee_List", "1");
        hash_params.Add("Regulatory_Report", "0");
        hash_params.Add("Shift_Roster", "0");
        hash_params.Add("Manual_Punch_Details", "1");
        hash_params.Add("Leave_Management", "1");
        hash_params.Add("Shift_Master", "0");
        hash_params.Add("Monthly_Reports", "1");
        hash_params.Add("Leave_Master", "0");
        hash_params.Add("Masters", "0");
        hash_params.Add("Comp_Off", "1");
        hash_params.Add("Employee_Category", "0");
        hash_params.Add("Enroll_Card", "0");
        hash_params.Add("Reprocess", "0");
        hash_params.Add("Leave_Register", "1");
        hash_params.Add("Approve_Comp_Off", "0");
        hash_params.Add("Daily_Attendance_Report", "1");
        hash_params.Add("Missing_Swipe", "1");
        hash_params.Add("Mode", mode);
        hash_params.Add("Daily_Performance_InOut_Report", "1");
        hash_params.Add("Holiday_Master", "0");
        hash_params.Add("Assignment", "0");
        hash_params.Add("Calculate_Pay_Details", "0");
        hash_params.Add("OT_Management", "1");
        hash_params.Add("Register", "0");
        hash_params.Add("OD_Leave_Details", "1");
        hash_params.Add("Shift_Settings", "0");
        hash_params.Add("Designation_Master", "0");
        hash_params.Add("DailyPerformancePerINOutReport", "1");
        hash_params.Add("Muster_Roll_Raw_Report", "0");
        hash_params.Add("Shift_Roster_Report", "0");
        return hash_params;
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

    private string[] GetShiftCodesForTransaction()
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

    [WebMethod]
    public static ReturnObject DoImport(string file_name)
    {
        masters_import_masters page_object = new masters_import_masters();

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
            shift_code_array = page_object.GetShiftCodesForTransaction();
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
                            else
                            {
                                return_message += Environment.NewLine + "Shift Code '" + Shift_Code + "' is Blank for row number  " + row_number;
                                IsShift = false;
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
}
